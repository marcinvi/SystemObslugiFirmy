// Plik: FormAllegroWizard.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Reklamacje_Dane.Allegro.Issues;

namespace Reklamacje_Dane
{
    public partial class FormAllegroWizard : Form
    {
        private readonly DatabaseService _dbService;
        private readonly ComplaintRepository _complaintRepository;
        private string _allegroIssueId;
        private string _allegroIssueTitle;
        private int _allegroAccountId;
        private string _allegroBuyerLogin;
        private string _allegroReferenceNumber;
        private string _allegroOpenedDate;
        private int? _selectedKlientId;
        private int? _selectedProduktId;
        private string _allegroKlientName;
        private string _allegroProduktName;
        private string _allegroCheckoutFormId;
        private string _allegroDescription;

        private Timer searchKlientDebounceTimer;
        private Timer searchProduktDebounceTimer;

        public FormAllegroWizard(string issueId, string issueTitle, int allegroAccountId, string buyerLogin, string referenceNumber, string openedDate, string produktName, string checkoutFormId, string description)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            _complaintRepository = new ComplaintRepository();
            InitializeSearchTimers();

            _allegroIssueId = issueId;
            _allegroIssueTitle = issueTitle;
            _allegroAccountId = allegroAccountId;
            _allegroBuyerLogin = buyerLogin;
            _allegroReferenceNumber = referenceNumber;
            _allegroOpenedDate = openedDate;
            _allegroProduktName = produktName;
            _allegroCheckoutFormId = checkoutFormId;
            _allegroDescription = description;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeSearchTimers()
        {
            searchKlientDebounceTimer = new Timer { Interval = 300 };
            searchKlientDebounceTimer.Tick += async (s, ev) =>
            {
                searchKlientDebounceTimer.Stop();
                await ShowKlientSuggestionsAsync(txtSzukajKlienta.Text);
            };

            searchProduktDebounceTimer = new Timer { Interval = 300 };
            searchProduktDebounceTimer.Tick += async (s, ev) =>
            {
                searchProduktDebounceTimer.Stop();
                await ShowProduktSuggestionsAsync(txtSzukajProduktu.Text);
            };
        }

        private async void FormAllegroWizard_Load(object sender, EventArgs e)
        {
            lblAllegroNrZgloszenia.Text = _allegroIssueId;
            lblAllegroStatus.Text = _allegroIssueTitle;
            _allegroKlientName = _allegroBuyerLogin;

            txtSzukajKlienta.Text = _allegroKlientName;
            txtSzukajProduktu.Text = _allegroProduktName;

            await ShowKlientSuggestionsAsync(_allegroKlientName);
            await ShowProduktSuggestionsAsync(_allegroProduktName);
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
                dgvKlientSuggestions.DataSource = results;
                dgvKlientSuggestions.Columns["Id"].Visible = false;
                dgvKlientSuggestions.Columns["DaneDoWyswietlenia"].HeaderText = "Propozycje klientów";
                dgvKlientSuggestions.Visible = true;
                dgvKlientSuggestions.BringToFront();
            }
            else
            {
                dgvKlientSuggestions.Visible = false;
            }
        }

        private void dgvKlientSuggestions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedItem = dgvKlientSuggestions.Rows[e.RowIndex].DataBoundItem as ClientSearchResult;
                if (selectedItem != null)
                {
                    _selectedKlientId = selectedItem.Id;
                    lblWyswietlanyKlient.Text = selectedItem.DaneDoWyswietlenia;
                    dgvKlientSuggestions.Visible = false;
                }
            }
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
                    _selectedProduktId = selectedItem.Id;
                    lblWyswietlanyProdukt.Text = selectedItem.NazwaKrotka;
                    dgvProduktSuggestions.Visible = false;
                }
            }
        }

        private async void btnZatwierdzPrzypisanie_Click_Async(object sender, EventArgs e)
        {
            if (_selectedKlientId == null || _selectedProduktId == null)
            {
                ToastManager.ShowToast("Błąd walidacji", "Proszę wybrać klienta i produkt z listy propozycji.", NotificationType.Warning);
                return;
            }

            btnZatwierdzPrzypisanie.Enabled = false;

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            string nextNrZgloszenia = await _complaintRepository.GetNextComplaintNumberAsync(con, transaction);

                            string query = @"INSERT INTO Zgloszenia (NrZgloszenia, KlientID, ProduktID, DataZgloszenia, NrFaktury, NrSeryjny, Usterka, StatusOgolny, allegroDisputeId)
                                         VALUES (@nrZgloszenia, @klientId, @produktId, @dataZgl, @nrFakt, @nrSer, @usterka, 'Procesowana', @allegroDisputeId)";
                            using (var cmd = new SQLiteCommand(query, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@nrZgloszenia", nextNrZgloszenia);
                                cmd.Parameters.AddWithValue("@klientId", _selectedKlientId.Value);
                                cmd.Parameters.AddWithValue("@produktId", _selectedProduktId.Value);
                                cmd.Parameters.AddWithValue("@dataZgl", _allegroOpenedDate);
                                cmd.Parameters.AddWithValue("@nrFakt", txtNrFaktury.Text);
                                cmd.Parameters.AddWithValue("@nrSer", txtNrSeryjny.Text);
                                cmd.Parameters.AddWithValue("@usterka", _allegroDescription);
                                cmd.Parameters.AddWithValue("@allegroDisputeId", _allegroIssueId);
                                await cmd.ExecuteNonQueryAsync();
                            }

                            // Po przypisaniu, oznacz zgłoszenie Allegro jako zarejestrowane w lokalnej bazie
                            string updateAllegroQuery = "UPDATE AllegroDisputes SET ComplaintId = @complaintId, Status = 'Zarejestrowano' WHERE DisputeId = @disputeId";
                            using (var updateCmd = new SQLiteCommand(updateAllegroQuery, con, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@complaintId", nextNrZgloszenia);
                                updateCmd.Parameters.AddWithValue("@disputeId", _allegroIssueId);
                                await updateCmd.ExecuteNonQueryAsync();
                            }

                            var dziennik = new DziennikLogger();
                            await dziennik.DodajAsync(con, transaction, Program.fullName, $"Dodano nowe zgłoszenie z Allegro ({_allegroIssueId}): {nextNrZgloszenia}", nextNrZgloszenia);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Błąd podczas zapisu zgłoszenia do bazy danych.", ex);
                        }
                    }
                }
                UpdateManager.NotifySubscribers();
                ToastManager.ShowToast("Sukces", "Zgłoszenie z Allegro zostało pomyślnie dodane do bazy.", NotificationType.Success);
                this.Close();
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd", "Wystąpił błąd podczas dodawania zgłoszenia: " + ex.Message, NotificationType.Error);
                btnZatwierdzPrzypisanie.Enabled = true;
            }
        }

        private void btnPomin_Click_Async(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSzukajKlienta_TextChanged(object sender, EventArgs e)
        {
            searchKlientDebounceTimer.Stop();
            searchKlientDebounceTimer.Start();
        }

        private void txtSzukajProduktu_TextChanged(object sender, EventArgs e)
        {
            searchProduktDebounceTimer.Stop();
            searchProduktDebounceTimer.Start();
        }

        private void txtSzukajKlienta_Leave(object sender, EventArgs e)
        {
            // Opcjonalnie: ukrycie po utracie fokusu
            // dgvKlientSuggestions.Visible = false;
        }

        private void txtSzukajProduktu_Leave(object sender, EventArgs e)
        {
            // Opcjonalnie: ukrycie po utracie fokusu
            // dgvProduktSuggestions.Visible = false;
        }

        private void lblWyswietlanyKlient_Click(object sender, EventArgs e)
        {

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