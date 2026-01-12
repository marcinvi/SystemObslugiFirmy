// Plik: Form13.cs
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
    public partial class Form13 : Form
    {
        private int? selectedKlientId = null;
        private int? selectedProduktId = null;
        private bool isKlientFirmowy = false;
        private readonly DatabaseService _dbService;
        private Timer searchDebounceTimer;

        public Form13()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void Form13_Load(object sender, EventArgs e)
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            searchDebounceTimer = new Timer { Interval = 300 };
            searchDebounceTimer.Tick += async (s, ev) =>
            {
                searchDebounceTimer.Stop();
                if (tabControl1.SelectedTab == tabPageKlient) await SearchKlienciAsync();
                else if (tabControl1.SelectedTab == tabPageProdukt) await SearchProduktyAsync();
            };

            SetupDataGridViews();
            UpdateStepUI();
        }

        private void SetupDataGridViews()
        {
            dataGridViewKlienci.CellClick += dataGridViewKlienci_CellClick;
            dataGridViewProdukty.CellClick += dataGridViewProdukty_CellClick;

            dataGridViewKlienci.AutoGenerateColumns = false;
            dataGridViewKlienci.Columns.Clear();
            dataGridViewKlienci.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DaneDoWyswietlenia", HeaderText = "Klient / Firma", FillWeight = 100, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dataGridViewKlienci.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Visible = false });

            dataGridViewProdukty.AutoGenerateColumns = false;
            dataGridViewProdukty.Columns.Clear();
            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NazwaKrotka", HeaderText = "Nazwa Produktu", FillWeight = 50, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Producent", HeaderText = "Producent", FillWeight = 30, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KodEnova", HeaderText = "Kod Enova", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KodProducenta", HeaderText = "Kod Producenta", FillWeight = 10, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Visible = false });

            // Ustawienie domyślnego stanu pól nowego klienta
            pnlNowyKlient.Visible = false;
        }

        #region --- Wizard Logic ---

        private async void btnDalej_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageKlient)
            {
                if (chkNowyKlient.Checked)
                {
                    selectedKlientId = await DodajNowegoKlientaAsync();
                    if (selectedKlientId == null) return;
                }
                else if (selectedKlientId == null)
                {
                    ToastManager.ShowToast("Błąd walidacji", "Proszę wybrać istniejącego klienta z listy.", NotificationType.Warning);
                    return;
                }
            }
            if (tabControl1.SelectedTab == tabPageProdukt && selectedProduktId == null)
            {
                ToastManager.ShowToast("Błąd walidacji", "Proszę wybrać produkt z listy.", NotificationType.Warning);
                return;
            }

            if (tabControl1.SelectedIndex < tabControl1.TabCount - 1)
            {
                tabControl1.SelectedIndex++;
            }
            else
            {
                await ZapiszZgloszenieAsync();
            }
        }

        private void btnWstecz_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex > 0)
            {
                tabControl1.SelectedIndex--;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStepUI();
        }

        private void UpdateStepUI()
        {
            var regularFont = new Font("Segoe UI", 10.2F, FontStyle.Regular);
            var boldFont = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold);
            var activeColor = Color.FromArgb(33, 150, 243);
            var inactiveColor = Color.Gray;

            lblStep1.Font = regularFont; lblStep1.ForeColor = inactiveColor;
            lblStep2.Font = regularFont; lblStep2.ForeColor = inactiveColor;
            lblStep3.Font = regularFont; lblStep3.ForeColor = inactiveColor;

            int currentIndex = tabControl1.SelectedIndex;
            if (currentIndex == 0) { lblStep1.Font = boldFont; lblStep1.ForeColor = activeColor; }
            else if (currentIndex == 1) { lblStep2.Font = boldFont; lblStep2.ForeColor = activeColor; }
            else if (currentIndex == 2) { lblStep3.Font = boldFont; lblStep3.ForeColor = activeColor; }

            btnWstecz.Enabled = (currentIndex > 0);
            btnDalej.Text = (currentIndex == tabControl1.TabCount - 1) ? "Zapisz Zgłoszenie" : "Dalej >";
        }
        #endregion

        #region --- Business Logic ---
        private async Task<int?> DodajNowegoKlientaAsync()
        {
            if (string.IsNullOrWhiteSpace(txtNowyImieNazwisko.Text) && string.IsNullOrWhiteSpace(txtNowyNazwaFirmy.Text))
            {
                ToastManager.ShowToast("Błąd walidacji", "Podczas dodawania nowego klienta, wymagane jest pole 'Imię i Nazwisko' lub 'Nazwa Firmy'.", NotificationType.Warning);
                return null;
            }

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        string query = "INSERT INTO Klienci (ImieNazwisko, NazwaFirmy, NIP, Ulica, KodPocztowy, Miejscowosc, Email, Telefon) VALUES (@imie, @firma, @nip, @ulica, @kod, @miasto, @mail, @tel); SELECT LAST_INSERT_ID();";
                        using (var cmd = new MySqlCommand(query, con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@imie", txtNowyImieNazwisko.Text);
                            cmd.Parameters.AddWithValue("@firma", txtNowyNazwaFirmy.Text);
                            cmd.Parameters.AddWithValue("@nip", txtNowyNIP.Text);
                            cmd.Parameters.AddWithValue("@ulica", txtNowyUlica.Text);
                            cmd.Parameters.AddWithValue("@kod", txtNowyKodPocztowy.Text);
                            cmd.Parameters.AddWithValue("@miasto", txtNowyMiejscowosc.Text);
                            cmd.Parameters.AddWithValue("@mail", txtNowyMail.Text);
                            cmd.Parameters.AddWithValue("@tel", txtNowyTelefon.Text);
                            long newId = (long)await cmd.ExecuteScalarAsync();
                            transaction.Commit();
                            this.isKlientFirmowy = !string.IsNullOrWhiteSpace(txtNowyNazwaFirmy.Text) || !string.IsNullOrWhiteSpace(txtNowyNIP.Text);
                            chkFirma.Checked = this.isKlientFirmowy;
                            ToastManager.ShowToast("Sukces", "Nowy klient został dodany.", NotificationType.Success);
                            return (int)newId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd", "Błąd dodawania nowego klienta: " + ex.Message, NotificationType.Error);
                return null;
            }
        }

        private async Task SearchKlienciAsync()
        {
            var results = await SearchAndSuggestionService.SearchKlienciAsync(txtSzukajKlienta.Text);
            dataGridViewKlienci.DataSource = results.Select(r => new { r.Id, DaneKlienta = r.DaneDoWyswietlenia, Adres = $"{r.Ulica}, {r.KodPocztowy} {r.Miejscowosc}", Kontakt = $"{r.Telefon} / {r.Email}", r.RelevanceScore }).ToList();
            dataGridViewKlienci.Columns["Id"].Visible = false;
            dataGridViewKlienci.Columns["RelevanceScore"].Visible = false;
        }

        private async Task SearchProduktyAsync()
        {
            var results = await SearchAndSuggestionService.SearchProduktyAsync(txtSzukajProduktu.Text);
            dataGridViewProdukty.DataSource = results;
        }

        private void SprawdzGwarancje()
        {
            DateTime dataZakupu = dtpDataZakupu.Value;
            int miesiaceGwarancji = this.isKlientFirmowy ? 12 : 24;
            DateTime dataKoncaGwarancji = dataZakupu.AddMonths(miesiaceGwarancji);
            if (DateTime.Now > dataKoncaGwarancji)
            {
                lblGwarancjaStatus.Text = $"Gwarancja zakończona ({dataKoncaGwarancji:dd.MM.yyyy})";
                lblGwarancjaStatus.ForeColor = Color.Red;
            }
            else
            {
                lblGwarancjaStatus.Text = $"Gwarancja ważna do {dataKoncaGwarancji:dd.MM.yyyy}";
                lblGwarancjaStatus.ForeColor = Color.ForestGreen;
            }
            lblGwarancjaStatus.Visible = true;
        }

        private async Task ZapiszZgloszenieAsync()
        {
            if (selectedKlientId == null) { ToastManager.ShowToast("Błąd", "Nie wybrano klienta.", NotificationType.Error); return; }
            if (selectedProduktId == null) { ToastManager.ShowToast("Błąd", "Nie wybrano produktu.", NotificationType.Error); return; }
            if (string.IsNullOrWhiteSpace(txtOpisUsterki.Text))
            {
                ToastManager.ShowToast("Błąd", "Opis usterki nie może być pusty.", NotificationType.Warning);
                tabControl1.SelectedTab = tabPageUsterka;
                return;
            }

            try
            {
                string nowyNrZgloszenia;
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            string biezacyRokSkrot = DateTime.Now.ToString("yy");
                            string wzorzecNumeru = $"R/%/{biezacyRokSkrot}";
                            var cmdOstatniNumer = new MySqlCommand("SELECT NrZgloszenia FROM Zgloszenia WHERE NrZgloszenia LIKE @wzorzec ORDER BY CAST(SUBSTRING(SUBSTRING(NrZgloszenia, LOCATE('/', NrZgloszenia) + 1), 1, LOCATE('/', SUBSTRING(NrZgloszenia, LOCATE('/', NrZgloszenia) + 1)) - 1) AS SIGNED) DESC LIMIT 1", con, transaction);
                            cmdOstatniNumer.Parameters.AddWithValue("@wzorzec", wzorzecNumeru);
                            var ostatniNumer = (await cmdOstatniNumer.ExecuteScalarAsync())?.ToString();
                            int nastepnyNumer = 1;
                            if (!string.IsNullOrEmpty(ostatniNumer))
                            {
                                nastepnyNumer = int.Parse(ostatniNumer.Split('/')[1]) + 1;
                            }
                            nowyNrZgloszenia = $"R/{nastepnyNumer}/{biezacyRokSkrot}";

                            string queryInsert = @"INSERT INTO Zgloszenia (NrZgloszenia, KlientID, ProduktID, DataZgloszenia, DataZakupu, NrFaktury, NrSeryjny, OpisUsterki, GwarancjaPlatna, StatusOgolny, StatusKlient, StatusProducent, CzekamyNaDostawe)
                                                 VALUES (@nr, @klientId, @produktId, @dataZglosz, @dataZakup, @nrFv, @nrSn, @opis, @typ, 'Procesowana', 'Zgłoszone', 'Oczekuje na zgłoszenie', 'Nie Czekamy')";
                            using (var cmd = new MySqlCommand(queryInsert, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@nr", nowyNrZgloszenia);
                                cmd.Parameters.AddWithValue("@klientId", selectedKlientId);
                                cmd.Parameters.AddWithValue("@produktId", selectedProduktId);
                                cmd.Parameters.AddWithValue("@dataZglosz", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmd.Parameters.AddWithValue("@dataZakup", dtpDataZakupu.Value.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@nrFv", txtNumerFaktury.Text);
                                cmd.Parameters.AddWithValue("@nrSn", txtnrSeryjny.Text);
                                cmd.Parameters.AddWithValue("@opis", txtOpisUsterki.Text);
                                cmd.Parameters.AddWithValue("@typ", rbPlatna.Checked ? "Płatna" : "Gwarancyjna");
                                await cmd.ExecuteNonQueryAsync();
                            }
                            await new DziennikLogger().DodajAsync(con, transaction, Program.fullName, "Utworzono nowe zgłoszenie", nowyNrZgloszenia);
                            new Dzialaniee().DodajNoweDzialanie(con, transaction, nowyNrZgloszenia, Program.fullName, "Utworzono zgłoszenie");
                            transaction.Commit();
                            ToastManager.ShowToast("Sukces", $"Utworzono nowe zgłoszenie: {nowyNrZgloszenia}", NotificationType.Success);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ToastManager.ShowToast("Błąd krytyczny", $"Wystąpił błąd podczas zapisu zgłoszenia: {ex.Message}", NotificationType.Error);
                            return;
                        }
                    }
                }
                UpdateManager.NotifySubscribers();
                this.Close();
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd krytyczny", $"Wystąpił błąd podczas zapisu zgłoszenia: {ex.Message}", NotificationType.Error);
            }
        }
        #endregion

        #region --- Control Events ---

        private async void txtSzukajKlienta_TextChanged(object sender, EventArgs e)
        {
            selectedKlientId = null;
            lblKlientInfo.Text = "";
            searchDebounceTimer.Stop();
            searchDebounceTimer.Start();
        }

        private void chkNowyKlient_CheckedChanged(object sender, EventArgs e)
        {
            bool nowyKlient = chkNowyKlient.Checked;
            pnlNowyKlient.Visible = nowyKlient;
            dataGridViewKlienci.Visible = !nowyKlient;
            txtSzukajKlienta.Enabled = !nowyKlient;
            if (nowyKlient)
            {
                selectedKlientId = null;
                lblKlientInfo.Text = "";
                dataGridViewKlienci.ClearSelection();
            }
        }

        private async void txtSzukajProduktu_TextChanged(object sender, EventArgs e)
        {
            selectedProduktId = null;
            searchDebounceTimer.Stop();
            searchDebounceTimer.Start();
        }

        private async void dataGridViewKlienci_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var klientVM = dataGridViewKlienci.Rows[e.RowIndex].DataBoundItem as ClientSearchResult;
            if (klientVM != null)
            {
                selectedKlientId = klientVM.Id;
                isKlientFirmowy = !string.IsNullOrWhiteSpace(klientVM.NazwaFirmy);
                chkFirma.Checked = isKlientFirmowy;

                lblKlientInfo.Text = klientVM.DaneDoWyswietlenia;
                lblKlientInfo.ForeColor = Color.ForestGreen;

                btnDalej_Click(sender, e);
            }
        }

        private async void dataGridViewProdukty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var produktVM = dataGridViewProdukty.Rows[e.RowIndex].DataBoundItem as ProductSearchResult;
            if (produktVM != null)
            {
                selectedProduktId = produktVM.Id;
                lblProduktInfo.Text = produktVM.NazwaKrotka;
                lblProduktInfo.ForeColor = Color.ForestGreen;

                btnDalej_Click(sender, e);
            }
        }

        private async void txtnrSeryjny_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtnrSeryjny.Text))
            {
                var warning = await SearchAndSuggestionService.CheckSerialNumberExistsAsync(txtnrSeryjny.Text);
                if (!string.IsNullOrEmpty(warning))
                {
                    ToastManager.ShowToast("Ostrzeżenie", warning, NotificationType.Warning);
                }
            }
        }

        private void dtpDataZakupu_ValueChanged(object sender, EventArgs e)
        {
            SprawdzGwarancje();
        }

        private void chkFirma_CheckedChanged(object sender, EventArgs e)
        {
            isKlientFirmowy = chkFirma.Checked;
            SprawdzGwarancje();
        }
        #endregion
    
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