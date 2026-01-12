// === FormZwrotPodsumowanie.cs (pełne podsumowanie; bez panelu decyzji; pełna historia + dodawanie wpisów) ===
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormZwrotPodsumowanie : Form
    {
        private readonly int _returnId;
        private readonly DatabaseService _dbMagazyn;
        private readonly DatabaseService _dbBaza;
        private readonly Dictionary<long, string> _users = new Dictionary<long, string>(); // Baza.Uzytkownicy (Id -> Nazwa Wyświetlana)

        public FormZwrotPodsumowanie(int returnId)
        {
            if (returnId <= 0) throw new ArgumentOutOfRangeException(nameof(returnId));
            _returnId = returnId;

            InitializeComponent();

            _dbMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbBaza = new DatabaseService(DatabaseHelper.GetConnectionString());

            this.Load += async (s, e) =>
            {
                try
                {
                    await LoadUsersMapAsync();
                    await LoadSummaryAsync();
                    await LoadHistoryAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas ładowania podsumowania: " + ex.Message, "Błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnDodajWpis.Click += async (s, e) => await AddHistoryEntryAsync();
            btnWroc.Click += (s, e) => this.Close();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // ====== MAPA UŻYTKOWNIKÓW (Baza.db → Uzytkownicy) ======
        private async Task LoadUsersMapAsync()
        {
            try
            {
                var dt = await _dbBaza.GetDataTableAsync(@"SELECT Id, `Nazwa Wyświetlana` AS Nazwa FROM Uzytkownicy");
                _users.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    long id = SafeGetLong(r["Id"]);
                    string nazwa = SafeGetString(r["Nazwa"]);
                    if (id > 0 && !_users.ContainsKey(id)) _users.Add(id, nazwa);
                }
            }
            catch (MySqlException ex) when (ex.Message.Contains("no such table: Uzytkownicy"))
            {
                _users.Clear(); // brak mapy nazw nie blokuje formularza
            }
        }

        // ====== PODSUMOWANIE ZWROTU ======
        private async Task LoadSummaryAsync()
        {
            var dt = await _dbMagazyn.GetDataTableAsync(@"
                SELECT
                    acr.Id,
                    acr.ReferenceNumber,
                    acr.ProductName,
                    acr.CreatedAt,
                    acr.HandlowiecOpiekunId,
                    acr.PrzyjetyPrzezId,
                    acr.DataPrzyjecia,
                    acr.UwagiMagazynu,
                    s2.Nazwa AS StatusWew,
                    s3.Nazwa AS DecyzjaHandl
                FROM AllegroCustomerReturns acr
                LEFT JOIN Statusy s2 ON s2.Id = acr.StatusWewnetrznyId
                LEFT JOIN Statusy s3 ON s3.Id = acr.DecyzjaHandlowcaId
                WHERE acr.Id = @id
                LIMIT 1",
                new MySqlParameter("@id", _returnId)
            );
            if (dt.Rows.Count == 0) throw new Exception($"Nie znaleziono zwrotu Id={_returnId}.");
            var row = dt.Rows[0];

            string nr = SafeGetString(row["ReferenceNumber"]);
            string produkt = SafeGetString(row["ProductName"]);
            string status = SafeGetString(row["StatusWew"]);
            string jakaDecyzja = SafeGetString(row["DecyzjaHandl"]);
            long handId = SafeGetLong(row["HandlowiecOpiekunId"]);
            string ktoDecyzja = MapUser(handId);

            string uwagiMag = SafeGetString(row["UwagiMagazynu"]);

            // Przyjęcie fizyczne
            string ktoPrzyjal = "nie przyjęte";
            DateTime? dataPrzyj = SafeGetDate(row["DataPrzyjecia"]);
            long przyjalId = SafeGetLong(row["PrzyjetyPrzezId"]);
            if (dataPrzyj.HasValue)
            {
                if (przyjalId > 0 && _users.TryGetValue(przyjalId, out var nm)) ktoPrzyjal = nm;
                else ktoPrzyjal = "(przyjęte)";
            }

            // Ostatnia decyzja z dziennika – dokładny autor decyzji i komentarz
            string komuPrzypisacDecyzje = ktoDecyzja;
            string komentarzHand = "";
            var dz = await _dbMagazyn.GetDataTableAsync(@"
                SELECT Data, Uzytkownik, Tresc
                FROM ZwrotDzialania
                WHERE ZwrotId = @id AND Tresc LIKE 'Podjęto decyzję:%'
                ORDER BY Data DESC
                LIMIT 1",
                new MySqlParameter("@id", _returnId)
            );
            if (dz.Rows.Count > 0)
            {
                komuPrzypisacDecyzje = SafeGetString(dz.Rows[0]["Uzytkownik"]);
                komentarzHand = ExtractKomentarz(SafeGetString(dz.Rows[0]["Tresc"]));
            }

            // Wypełnij UI
            txtNrZwrotu.Text = nr;
            txtProdukt.Text = produkt;
            txtKtoPrzyjal.Text = string.IsNullOrWhiteSpace(ktoPrzyjal) ? "nie przyjęte" : ktoPrzyjal;
            txtKtoPodjal.Text = string.IsNullOrWhiteSpace(komuPrzypisacDecyzje) ? ktoDecyzja : komuPrzypisacDecyzje;
            txtJakaDecyzja.Text = jakaDecyzja;
            txtUwagiMagazynu.Text = uwagiMag;
            txtUwagiHandlowca.Text = komentarzHand;
            txtStatus.Text = status;

            this.Text = $"Zwrot: {nr}";
            lblHeader.Text = $"Podsumowanie zwrotu — {nr}";
        }

        // ====== HISTORIA DZIAŁAŃ ======
        private async Task LoadHistoryAsync()
        {
            try
            {
                var dt = await _dbMagazyn.GetDataTableAsync(@"
                    SELECT Data, Uzytkownik, Tresc 
                    FROM ZwrotDzialania
                    WHERE ZwrotId = @id
                    ORDER BY Data DESC",
                    new MySqlParameter("@id", _returnId)
                );

                dgvHistoria.DataSource = dt;
                try { dgvHistoria.EnableDoubleBuffer(); } catch { }
                dgvHistoria.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvHistoria.ReadOnly = true;
                dgvHistoria.RowHeadersVisible = false;
                dgvHistoria.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // styl jak w HandlowiecControl
                var header = dgvHistoria.ColumnHeadersDefaultCellStyle;
                header.BackColor = Color.FromArgb(68, 114, 196);
                header.ForeColor = Color.White;
                header.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                dgvHistoria.EnableHeadersVisualStyles = false;

                dgvHistoria.DefaultCellStyle.Font = new Font("Segoe UI", 8.25F);
                dgvHistoria.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                dgvHistoria.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 150, 243);

                lblHistoriaEmpty.Visible = (dt.Rows.Count == 0);
            }
            catch (MySqlException ex) when (ex.Message.Contains("no such table"))
            {
                // U Ciebie historia jest w ZwrotDzialania; jeśli kiedykolwiek byłaby inaczej – powiedz, dopasuję nazwę.
                lblHistoriaEmpty.Visible = true;
                dgvHistoria.DataSource = new DataTable();
            }
        }

        private async Task AddHistoryEntryAsync()
        {
            string tresc = txtNowyWpis.Text.Trim();
            if (string.IsNullOrWhiteSpace(tresc))
            {
                MessageBox.Show("Wpis nie może być pusty.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string user = GuessCurrentUserDisplayName();
            try
            {
                await _dbMagazyn.ExecuteNonQueryAsync(@"
                    INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc)
                    VALUES (@id, NOW(), @user, @tresc)",
                    new MySqlParameter("@id", _returnId),
                    new MySqlParameter("@user", user),
                    new MySqlParameter("@tresc", tresc)
                );

                txtNowyWpis.Clear();
                await LoadHistoryAsync();
            }
            catch (MySqlException ex) when (ex.Message.Contains("no such table"))
            {
                MessageBox.Show("Brak tabeli ZwrotDzialania – daj znać, jeśli w Twojej bazie ma inną nazwę.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu wpisu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GuessCurrentUserDisplayName()
        {
            // Jeśli macie globalny kontekst zalogowanego użytkownika – podmień to miejsce.
            try
            {
                string env = Environment.UserName;
                if (!string.IsNullOrWhiteSpace(env)) return env;
            }
            catch { }
            return "Podsumowanie";
        }

        private string MapUser(long id) => (id > 0 && _users.TryGetValue(id, out var n)) ? n : "";

        private static long SafeGetLong(object o) { if (o == null || o == DBNull.Value) return 0; if (o is long l) return l; if (o is int i) return i; if (long.TryParse(o.ToString(), out var p)) return p; return 0; }
        private static string SafeGetString(object o) => (o == null || o == DBNull.Value) ? "" : o.ToString();
        private static DateTime? SafeGetDate(object o) { if (o == null || o == DBNull.Value) return null; if (o is DateTime d) return d; if (DateTime.TryParse(o.ToString(), out var p)) return p; return null; }

        private static string ExtractKomentarz(string tresc)
        {
            if (string.IsNullOrWhiteSpace(tresc)) return "";
            var key = "Komentarz:";
            var idx = tresc.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return "";
            return tresc.Substring(idx + key.Length).Trim();
        }
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                // Włącz sprawdzanie pisowni dla wszystkich kontrolek typu TextBox i RichTextBox
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        // Dla zwykłych TextBoxów - bez podkreślania (bo nie obsługują kolorów)
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
            }
        }

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;

                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control))
                    {
                        yield return child;
                    }
                }
            }
        }
}
}
