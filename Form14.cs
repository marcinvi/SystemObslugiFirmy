// Plik: Form14.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing; // Dodano brakującą dyrektywę using

namespace Reklamacje_Dane
{
    public partial class Form14 : Form
    {
        private readonly DatabaseService _dbService;
        private readonly SheetsService _sheetsService;
        private readonly ComplaintRepository _complaintRepository;
        private int? selectedKlientId;
        private int? selectedProduktId;
        private Timer searchKlientDebounceTimer;
        private Timer searchProduktDebounceTimer;

        public Form14()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            _sheetsService = GetSheetsService();
            _complaintRepository = new ComplaintRepository();
            InitializeSearchTimers();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeSearchTimers()
        {
            searchKlientDebounceTimer = new Timer { Interval = 300 };
            searchKlientDebounceTimer.Tick += async (s, ev) =>
            {
                searchKlientDebounceTimer.Stop();
                await ShowKlientSuggestionsAsync(txtKlient.Text);
            };
            dgvKlientSuggestions.Leave += (s, ev) => dgvKlientSuggestions.Visible = false;

            searchProduktDebounceTimer = new Timer { Interval = 300 };
            searchProduktDebounceTimer.Tick += async (s, ev) =>
            {
                searchProduktDebounceTimer.Stop();
                await ShowProduktSuggestionsAsync(txtProdukt.Text);
            };
            dgvProduktSuggestions.Leave += (s, ev) => dgvProduktSuggestions.Visible = false;
        }

        private SheetsService GetSheetsService()
        {
            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream("reklamacje-baza-ed853b4e33f7.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
                }
                return new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = credential });
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd Google API", "Nie udało się załadować poświadczeń Google: " + ex.Message, NotificationType.Error);
                return null;
            }
        }

        private async void btnWczytajZArkusz_Click(object sender, EventArgs e)
        {
            if (_sheetsService == null)
            {
                ToastManager.ShowToast("Błąd", "Brak połączenia z Google Sheets.", NotificationType.Error);
                return;
            }

            await WczytajZgloszeniaZArkusza();
        }

        private async Task WczytajZgloszeniaZArkusza()
        {
            try
            {
                string spreadsheetId = "1VXGP4Cckt6NmSHtiv-Um7nqg-itLMczAGd-5a_Tc4Ds";
                string zakres = "Baza!A2:AZ";
                var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, zakres);
                var response = await request.ExecuteAsync();
                var values = response.Values;

                if (values != null && values.Count > 0)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Data");
                    dt.Columns.Add("Klient");
                    dt.Columns.Add("Produkt");
                    dt.Columns.Add("Usterka");

                    foreach (var row in values)
                    {
                        if (row.Count > 3 && !string.IsNullOrWhiteSpace(row[0]?.ToString()))
                        {
                            dt.Rows.Add(row[0], row[1], row[2], row[3]);
                        }
                    }
                    dgvZgloszenia.DataSource = dt;
                    dgvZgloszenia.Columns["Data"].Width = 80;
                    dgvZgloszenia.Columns["Klient"].Width = 120;
                    dgvZgloszenia.Columns["Produkt"].Width = 120;
                    dgvZgloszenia.Columns["Usterka"].Width = 160;
                }
                else
                {
                    ToastManager.ShowToast("Informacja", "Brak danych do wczytania z arkusza.", NotificationType.Info);
                }
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd", "Błąd podczas wczytywania danych z Google Sheets: " + ex.Message, NotificationType.Error);
            }
        }

        private void dgvZgloszenia_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvZgloszenia.SelectedRows.Count > 0)
            {
                var row = dgvZgloszenia.SelectedRows[0];
                txtKlient.Text = row.Cells["Klient"].Value?.ToString();
                txtProdukt.Text = row.Cells["Produkt"].Value?.ToString();
                // Pozostałe pola są puste lub nie są dostępne w arkuszu
            }
        }

        private async void txtKlient_TextChanged(object sender, EventArgs e)
        {
            searchKlientDebounceTimer.Stop();
            searchKlientDebounceTimer.Start();
        }

        private async Task ShowKlientSuggestionsAsync(string fraza)
        {
            if (string.IsNullOrWhiteSpace(fraza))
            {
                dgvKlientSuggestions.Visible = false;
                return;
            }

            var results = await SearchAndSuggestionService.SearchKlienciAsync(fraza);
            if (results.Any())
            {
                dgvKlientSuggestions.DataSource = results.Select(r => new { r.Id, r.DaneDoWyswietlenia, r.RelevanceScore }).ToList();
                dgvKlientSuggestions.Columns["Id"].Visible = false;
                dgvKlientSuggestions.Columns["RelevanceScore"].Visible = false;
                dgvKlientSuggestions.Columns["DaneDoWyswietlenia"].HeaderText = "Propozycje klientów";
                dgvKlientSuggestions.Visible = true;
                dgvKlientSuggestions.BringToFront();
            }
            else
            {
                dgvKlientSuggestions.Visible = false;
            }
        }

        private void txtKlient_Leave(object sender, EventArgs e) => dgvKlientSuggestions.Visible = false;
        private void txtProdukt_Leave(object sender, EventArgs e) => dgvProduktSuggestions.Visible = false;
        private void dgvKlientSuggestions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedItem = dgvKlientSuggestions.Rows[e.RowIndex].DataBoundItem as dynamic;
                if (selectedItem != null)
                {
                    selectedKlientId = selectedItem.Id;
                    txtKlient.Text = selectedItem.DaneDoWyswietlenia;
                    dgvKlientSuggestions.Visible = false;
                }
            }
        }

        private async void txtProdukt_TextChanged(object sender, EventArgs e)
        {
            searchProduktDebounceTimer.Stop();
            searchProduktDebounceTimer.Start();
        }

        private async Task ShowProduktSuggestionsAsync(string fraza)
        {
            if (string.IsNullOrWhiteSpace(fraza))
            {
                dgvProduktSuggestions.Visible = false;
                return;
            }

            var results = await SearchAndSuggestionService.SearchProduktyAsync(fraza);
            if (results.Any())
            {
                dgvProduktSuggestions.DataSource = results;
                dgvProduktSuggestions.Columns["Id"].Visible = false;
                dgvProduktSuggestions.Columns["NazwaKrotka"].HeaderText = "Nazwa";
                dgvProduktSuggestions.Columns["Producent"].HeaderText = "Producent";
                dgvProduktSuggestions.Visible = true;
                dgvProduktSuggestions.BringToFront();
            }
            else
            {
                dgvProduktSuggestions.Visible = false;
            }
        }

        private void dgvProduktSuggestions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedItem = dgvProduktSuggestions.Rows[e.RowIndex].DataBoundItem as ProductSearchResult;
                if (selectedItem != null)
                {
                    selectedProduktId = selectedItem.Id;
                    txtProdukt.Text = selectedItem.NazwaKrotka;
                    dgvProduktSuggestions.Visible = false;
                }
            }
        }

        private async void txtNrSeryjny_Leave(object sender, EventArgs e)
        {
            string warning = await SearchAndSuggestionService.CheckSerialNumberExistsAsync(txtNrSeryjny.Text);
            lblNrSeryjnyWarning.Text = warning;
            lblNrSeryjnyWarning.ForeColor = string.IsNullOrEmpty(warning) ? System.Drawing.Color.Black : System.Drawing.Color.Red;
        }

        private async void txtNrFaktury_Leave(object sender, EventArgs e)
        {
            string warning = await SearchAndSuggestionService.CheckInvoiceNumberExistsAsync(txtNrFaktury.Text);
            lblNrFakturyWarning.Text = warning;
            lblNrFakturyWarning.ForeColor = string.IsNullOrEmpty(warning) ? System.Drawing.Color.Black : System.Drawing.Color.Red;
        }
        private void btnDodajKlienta_Click(object sender, EventArgs e)
        {
            using (var form3 = new Form3())
            {
                form3.ShowDialog();
            }
        }

        private void btnDodajProdukt_Click(object sender, EventArgs e)
        {
            using (var form15 = new Form15(null))
            {
                form15.ShowDialog();
            }
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            if (dgvZgloszenia.SelectedRows.Count == 0)
            {
                ToastManager.ShowToast("Informacja", "Proszę wybrać zgłoszenie z listy.", NotificationType.Info);
                return;
            }
            if (selectedKlientId == null || selectedProduktId == null)
            {
                ToastManager.ShowToast("Błąd walidacji", "Proszę wybrać klienta i produkt z listy propozycji.", NotificationType.Warning);
                return;
            }

            btnZapisz.Enabled = false;

            try
            {
                var selectedRow = dgvZgloszenia.SelectedRows[0];
                string dataZgloszenia = selectedRow.Cells["Data"].Value.ToString();
                string usterka = selectedRow.Cells["Usterka"].Value?.ToString() ?? "";

                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            string nrZgloszenia = await _complaintRepository.GetNextComplaintNumberAsync(con, transaction);

                            await _complaintRepository.InsertComplaintAsync(
                                con,
                                transaction,
                                nrZgloszenia,
                                selectedKlientId.Value,
                                selectedProduktId.Value,
                                DateTime.Parse(dataZgloszenia).Date,
                                txtNrFaktury.Text,
                                txtNrSeryjny.Text,
                                usterka);

                            var dziennik = new DziennikLogger();
                            await dziennik.DodajAsync(con, transaction, Program.fullName, $"Dodano nowe zgłoszenie z Google Sheets: {nrZgloszenia}", nrZgloszenia);

                            transaction.Commit();
                            ToastManager.ShowToast("Sukces", $"Dodano nowe zgłoszenie {nrZgloszenia}.", NotificationType.Success);

                            dgvZgloszenia.Rows.Remove(selectedRow);
                            selectedKlientId = null;
                            selectedProduktId = null;
                            txtKlient.Clear();
                            txtProdukt.Clear();
                            txtNrFaktury.Clear();
                            txtNrSeryjny.Clear();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"[Form14] Error while saving complaint: {ex}");
                            throw;
                        }
                    }
                }
                UpdateManager.NotifySubscribers();
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd", "Wystąpił błąd podczas dodawania zgłoszenia: " + ex.Message, NotificationType.Error);
            }
            finally
            {
                btnZapisz.Enabled = true;
            }
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