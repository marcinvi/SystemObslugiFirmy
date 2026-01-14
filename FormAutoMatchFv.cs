using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    public partial class FormAutoMatchFv : Form
    {
        private readonly DatabaseService _dbService;
        private readonly BindingSource _proposedBinding = new BindingSource();
        private readonly BindingSource _reviewBinding = new BindingSource();
        private List<AppliedMatch> _lastApplied = new List<AppliedMatch>();
        private List<ReviewMatch> _lastReview = new List<ReviewMatch>();
        private string _reportFolder;
        private string _appliedReportPath;
        private string _reviewReportPath;
        private string _missingReportPath;
        private HashSet<string> _documentNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool _isBusy;

        public FormAutoMatchFv()
        {
            InitializeComponent();
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            chkYear2022.Checked = true;
            chkYear2023.Checked = true;
            chkYear2024.Checked = true;
            chkYear2025.Checked = true;
            chkIncludeClient.Checked = true;
            nudScoreThreshold.Value = 140;
            nudMinDelta.Value = 30;

            dgvProposed.AutoGenerateColumns = true;
            dgvReview.AutoGenerateColumns = true;
            lblStatus.Text = "Status: gotowy";
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            await RunMatchingAsync();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (_isBusy) return;
            if (_lastApplied == null || _lastApplied.Count == 0)
            {
                MessageBox.Show("Brak proponowanych uzupełnień do zapisu. Uruchom skanowanie.", "Brak danych", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var previewLines = _lastApplied.Take(10)
                .Select(m => $"{m.ZgloszenieId}: {FormatValue(m.OldNrFaktury)} -> {FormatValue(m.NewNrFaktury)} | {FormatValue(m.OldDataZakupu)} -> {FormatValue(m.NewDataZakupu)} | score {m.Score}")
                .ToList();

            string message = $"Zostanie zaktualizowanych: {_lastApplied.Count} zgłoszeń." +
                             Environment.NewLine +
                             Environment.NewLine +
                             "Przykłady (pierwsze 10):" +
                             Environment.NewLine +
                             string.Join(Environment.NewLine, previewLines) +
                             Environment.NewLine +
                             Environment.NewLine +
                             "Czy na pewno zapisać zmiany do bazy?";

            if (MessageBox.Show(message, "Potwierdzenie zapisu", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            await SaveMatchesAsync();
        }

        private async Task RunMatchingAsync()
        {
            if (_isBusy) return;

            var years = GetSelectedYears();
            if (years.Count == 0)
            {
                MessageBox.Show("Wybierz co najmniej jeden rok raportu.", "Brak lat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetUiBusy(true);
            ResetResults();

            var progress = new Progress<ProgressInfo>(UpdateProgress);

            try
            {
                _reportFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "raporty");
                Directory.CreateDirectory(_reportFolder);
                var files = years.Select(y => Path.Combine(_reportFolder, $"{y}.txt")).ToList();

                progress.Report(new ProgressInfo { Percent = 5, Message = "Wczytywanie raportów enova..." });
                var loadResult = await Task.Run(() => CsvSaleLoader.LoadDocuments(files, progress));
                _documentNumbers = loadResult.DocumentNumbers;

                progress.Report(new ProgressInfo { Percent = 30, Message = "Pobieranie zgłoszeń z bazy..." });
                var matchInputs = await LoadMatchInputsAsync();

                progress.Report(new ProgressInfo { Percent = 50, Message = "Dopasowywanie dokumentów..." });
                var matcher = new AutoMatcher();
                var options = new AutoMatchOptions
                {
                    IncludeClientMatch = chkIncludeClient.Checked,
                    ScoreThreshold = (int)nudScoreThreshold.Value,
                    MinScoreDelta = (int)nudMinDelta.Value
                };

                var matchResult = await Task.Run(() => matcher.Match(matchInputs, loadResult.Documents, options, progress));
                _lastApplied = matchResult.AppliedMatches
                    .Where(m => !string.IsNullOrWhiteSpace(m.NewNrFaktury) || !string.IsNullOrWhiteSpace(m.NewDataZakupu))
                    .ToList();
                _lastReview = matchResult.ReviewMatches;

                progress.Report(new ProgressInfo { Percent = 80, Message = "Generowanie raportów CSV..." });
                await GenerateReportsAsync();

                BindGrids();

                lblStatus.Text = $"Status: gotowy. Proponowane: {_lastApplied.Count}, do weryfikacji: {_lastReview.Count}.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dopasowywania: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Status: błąd podczas dopasowywania";
            }
            finally
            {
                SetUiBusy(false);
            }
        }

        private void BindGrids()
        {
            _proposedBinding.DataSource = _lastApplied;
            dgvProposed.DataSource = _proposedBinding;

            var reviewRows = _lastReview.Select(r => new ReviewMatchRow
            {
                ZgloszenieId = r.ZgloszenieId,
                Reason = r.Reason,
                Top1Dokument = r.Top1?.Dokument,
                Top1Score = r.Top1?.Score ?? 0,
                Top1Date = r.Top1?.DocDate,
                Top1Kontrahent = r.Top1?.Kontrahent,
                Top1Towar = r.Top1?.TowarSample,
                Top2Dokument = r.Top2?.Dokument,
                Top2Score = r.Top2?.Score ?? 0,
                Top2Date = r.Top2?.DocDate,
                Top2Kontrahent = r.Top2?.Kontrahent,
                Top2Towar = r.Top2?.TowarSample
            }).ToList();

            _reviewBinding.DataSource = reviewRows;
            dgvReview.DataSource = _reviewBinding;
        }

        private async Task<List<MatchInput>> LoadMatchInputsAsync()
        {
            const string query = @"SELECT z.Id, z.ProduktID, z.KlientID, z.DataZgloszenia, z.DataZakupu, z.NrFaktury,
                                          p.NazwaSystemowa, p.NazwaKrotka, p.KodEnova, p.KodProducenta,
                                          k.ImieNazwisko, k.NazwaFirmy, k.KodPocztowy, k.Miejscowosc
                                   FROM Zgloszenia z
                                   INNER JOIN Produkty p ON z.ProduktID = p.Id
                                   INNER JOIN Klienci k ON z.KlientID = k.Id
                                   WHERE (z.NrFaktury IS NULL OR z.NrFaktury = '' OR z.DataZakupu IS NULL OR z.DataZakupu = '')
                                     AND z.ProduktID IS NOT NULL AND z.KlientID IS NOT NULL";

            var dt = await _dbService.GetDataTableAsync(query);
            var list = new List<MatchInput>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MatchInput
                {
                    Id = Convert.ToInt32(row["Id"]),
                    DataZgloszenia = row["DataZgloszenia"]?.ToString(),
                    DataZakupu = row["DataZakupu"]?.ToString(),
                    NrFaktury = row["NrFaktury"]?.ToString(),
                    Product = new ProductInfo
                    {
                        NazwaSystemowa = row["NazwaSystemowa"]?.ToString(),
                        NazwaKrotka = row["NazwaKrotka"]?.ToString(),
                        KodEnova = row["KodEnova"]?.ToString(),
                        KodProducenta = row["KodProducenta"]?.ToString()
                    },
                    Client = new ClientInfo
                    {
                        ImieNazwisko = row["ImieNazwisko"]?.ToString(),
                        NazwaFirmy = row["NazwaFirmy"]?.ToString(),
                        KodPocztowy = row["KodPocztowy"]?.ToString(),
                        Miejscowosc = row["Miejscowosc"]?.ToString()
                    }
                });
            }

            return list;
        }

        private async Task GenerateReportsAsync()
        {
            _appliedReportPath = Path.Combine(_reportFolder, "applied_matches.csv");
            _reviewReportPath = Path.Combine(_reportFolder, "review_matches.csv");
            _missingReportPath = Path.Combine(_reportFolder, "fv_missing_in_csv.csv");

            await Task.Run(() => WriteAppliedReport(_appliedReportPath, _lastApplied));
            await Task.Run(() => WriteReviewReport(_reviewReportPath, _lastReview));
            var missing = await GetMissingFvNumbersAsync();
            await Task.Run(() => WriteMissingReport(_missingReportPath, missing));
        }

        private async Task<List<string>> GetMissingFvNumbersAsync()
        {
            const string query = @"SELECT NrFaktury FROM Zgloszenia
                                   WHERE NrFaktury IS NOT NULL AND NrFaktury <> ''
                                     AND (NrFaktury LIKE '%/FV%' OR NrFaktury LIKE '%/FVW%')";
            var dt = await _dbService.GetDataTableAsync(query);
            var missing = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                string nr = row["NrFaktury"]?.ToString();
                if (string.IsNullOrWhiteSpace(nr)) continue;
                string normalized = nr.Trim();
                if (!_documentNumbers.Contains(normalized))
                {
                    missing.Add(nr);
                }
            }

            return missing.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private async Task SaveMatchesAsync()
        {
            if (_lastApplied == null || _lastApplied.Count == 0) return;

            int updated = 0;
            int skipped = 0;

            const string updateSql = @"UPDATE Zgloszenia
                                       SET NrFaktury = CASE WHEN @newNr IS NULL OR @newNr = '' THEN NrFaktury ELSE @newNr END,
                                           DataZakupu = CASE WHEN @newDate IS NULL OR @newDate = '' THEN DataZakupu ELSE @newDate END
                                       WHERE Id = @id";

            SetUiBusy(true);
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var tx = con.BeginTransaction())
                    {
                        foreach (var match in _lastApplied)
                        {
                            if (string.IsNullOrWhiteSpace(match.NewNrFaktury) && string.IsNullOrWhiteSpace(match.NewDataZakupu))
                            {
                                skipped++;
                                continue;
                            }

                            bool nrSame = !string.IsNullOrWhiteSpace(match.NewNrFaktury) && string.Equals(match.NewNrFaktury, match.OldNrFaktury, StringComparison.OrdinalIgnoreCase);
                            bool dateSame = !string.IsNullOrWhiteSpace(match.NewDataZakupu) && string.Equals(match.NewDataZakupu, match.OldDataZakupu, StringComparison.OrdinalIgnoreCase);
                            if (nrSame && dateSame)
                            {
                                skipped++;
                                continue;
                            }

                            var parameters = new[]
                            {
                                new MySqlParameter("@newNr", match.NewNrFaktury ?? string.Empty),
                                new MySqlParameter("@newDate", match.NewDataZakupu ?? string.Empty),
                                new MySqlParameter("@id", match.ZgloszenieId)
                            };

                            await _dbService.ExecuteNonQueryAsync(con, tx, updateSql, parameters);
                            updated++;
                        }

                        tx.Commit();
                    }
                }

                MessageBox.Show($"Zapis zakończony. Zaktualizowano: {updated}, pominięto: {skipped}." +
                                Environment.NewLine +
                                $"Raporty: {_appliedReportPath}, {_reviewReportPath}, {_missingReportPath}",
                    "Zapis zakończony", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu do bazy: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUiBusy(false);
            }
        }

        private void WriteAppliedReport(string path, List<AppliedMatch> matches)
        {
            var lines = new List<string>
            {
                "Id;OldNr;NewNr;OldDate;NewDate;Score;DocDate;Kontrahent;TowarSample"
            };

            foreach (var match in matches)
            {
                lines.Add(string.Join(";", new[]
                {
                    match.ZgloszenieId.ToString(),
                    EscapeCsv(match.OldNrFaktury),
                    EscapeCsv(match.NewNrFaktury),
                    EscapeCsv(match.OldDataZakupu),
                    EscapeCsv(match.NewDataZakupu),
                    match.Score.ToString(),
                    EscapeCsv(match.DocDate),
                    EscapeCsv(match.Kontrahent),
                    EscapeCsv(match.TowarSample)
                }));
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        private void WriteReviewReport(string path, List<ReviewMatch> matches)
        {
            var lines = new List<string>
            {
                "Id;Reason;Top1Doc;Top1Score;Top1Date;Top1Kontrahent;Top1Towar;Top2Doc;Top2Score;Top2Date;Top2Kontrahent;Top2Towar"
            };

            foreach (var match in matches)
            {
                lines.Add(string.Join(";", new[]
                {
                    match.ZgloszenieId.ToString(),
                    EscapeCsv(match.Reason),
                    EscapeCsv(match.Top1?.Dokument),
                    match.Top1?.Score.ToString() ?? string.Empty,
                    EscapeCsv(match.Top1?.DocDate),
                    EscapeCsv(match.Top1?.Kontrahent),
                    EscapeCsv(match.Top1?.TowarSample),
                    EscapeCsv(match.Top2?.Dokument),
                    match.Top2?.Score.ToString() ?? string.Empty,
                    EscapeCsv(match.Top2?.DocDate),
                    EscapeCsv(match.Top2?.Kontrahent),
                    EscapeCsv(match.Top2?.TowarSample)
                }));
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        private void WriteMissingReport(string path, List<string> missingNumbers)
        {
            var lines = new List<string> { "NrFaktury" };
            foreach (var nr in missingNumbers)
            {
                lines.Add(EscapeCsv(nr));
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        private string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var value = input.Replace("\r", " ").Replace("\n", " ");
            bool mustQuote = value.Contains(";") || value.Contains("\"");
            if (mustQuote)
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        private void ResetResults()
        {
            _lastApplied = new List<AppliedMatch>();
            _lastReview = new List<ReviewMatch>();
            _proposedBinding.DataSource = null;
            _reviewBinding.DataSource = null;
            dgvProposed.DataSource = null;
            dgvReview.DataSource = null;
        }

        private void UpdateProgress(ProgressInfo info)
        {
            if (info == null) return;
            int percent = Math.Max(0, Math.Min(100, info.Percent));
            progressBar.Value = percent;
            lblStatus.Text = $"Status: {info.Message}";
        }

        private void SetUiBusy(bool isBusy)
        {
            _isBusy = isBusy;
            btnScan.Enabled = !isBusy;
            btnSave.Enabled = !isBusy;
            chkYear2022.Enabled = !isBusy;
            chkYear2023.Enabled = !isBusy;
            chkYear2024.Enabled = !isBusy;
            chkYear2025.Enabled = !isBusy;
            chkIncludeClient.Enabled = !isBusy;
            nudScoreThreshold.Enabled = !isBusy;
            nudMinDelta.Enabled = !isBusy;
        }

        private List<int> GetSelectedYears()
        {
            var years = new List<int>();
            if (chkYear2022.Checked) years.Add(2022);
            if (chkYear2023.Checked) years.Add(2023);
            if (chkYear2024.Checked) years.Add(2024);
            if (chkYear2025.Checked) years.Add(2025);
            return years;
        }

        private string FormatValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "(puste)" : value;
        }

        private sealed class ReviewMatchRow
        {
            public int ZgloszenieId { get; set; }
            public string Reason { get; set; }
            public string Top1Dokument { get; set; }
            public int Top1Score { get; set; }
            public string Top1Date { get; set; }
            public string Top1Kontrahent { get; set; }
            public string Top1Towar { get; set; }
            public string Top2Dokument { get; set; }
            public int Top2Score { get; set; }
            public string Top2Date { get; set; }
            public string Top2Kontrahent { get; set; }
            public string Top2Towar { get; set; }
        }
    }
}
