// Plik: Form3.cs
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.Data;
using System.Drawing;

namespace Reklamacje_Dane
{
    public partial class Form3 : Form
    {
        private int? selectedKlientId = null;
        private int? initialKlientId = null;
        private readonly DatabaseService _dbService;
        private Timer searchDebounceTimer;

        // NOWOŚĆ: Pole do przechowania informacji, czy startować w trybie zmiany
        private bool _startInChangeMode = false;

        private const int OriginalPanel2Top = 250;
        private const int OriginalPanel1Top = 451;

        public int? NowoWybranyKlientId { get; private set; }
        public bool CzyDaneZostalyZmienione { get; private set; } = false;

        public Form3()
        {
            InitializeComponent();
            InitializeSearchTimer();
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // ZMIANA: Dodajemy opcjonalny parametr do konstruktora
        public Form3(int idKlienta, bool startInChangeMode = false)
        {
            InitializeComponent();
            InitializeSearchTimer();
            this.initialKlientId = idKlienta;
            this._startInChangeMode = startInChangeMode; // NOWOŚĆ: Zapisujemy informację
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        }

        private void InitializeSearchTimer()
        {
            searchDebounceTimer = new Timer { Interval = 300 };
            searchDebounceTimer.Tick += async (s, ev) =>
            {
                searchDebounceTimer.Stop();
                await LoadKlienciAsync(txtWyszukajKlienta.Text);
            };
        }

        private async void Form3_Load(object sender, EventArgs e)
        {
            panel2.Location = new Point(panel2.Location.X, OriginalPanel2Top);
            panel1.Location = new Point(panel1.Location.X, OriginalPanel1Top);

            if (initialKlientId.HasValue)
            {
                Text = "Edycja / Zmiana klienta";
                chkZmienKlienta.Visible = true;
                label10.Visible = false;
                txtWyszukajKlienta.Visible = false;
                dataGridViewKlienci.Visible = false;
                this.selectedKlientId = initialKlientId.Value;

                await WypelnijPolaEdycjiAsync(initialKlientId.Value);
                await WczytajZgloszeniaKlientaAsync(initialKlientId.Value);
                btnDodaj.Visible = false;
                btnUsun.Visible = false;

                // NOWOŚĆ: Jeśli Form2 kazał włączyć tryb zmiany, robimy to automatycznie
                if (_startInChangeMode)
                {
                    chkZmienKlienta.Checked = true;
                }
            }
            else
            {
                Text = "Zarządzanie klientami";
                chkZmienKlienta.Visible = false;
                await LoadKlienciAsync();
                WyczyscPola();
                btnEdytuj.Visible = false;
                btnUsun.Visible = false;
            }
        }

        private async void chkZmienKlienta_CheckedChanged(object sender, EventArgs e)
        {
            bool wlaczWyszukiwanie = chkZmienKlienta.Checked;
            label10.Visible = wlaczWyszukiwanie;
            txtWyszukajKlienta.Visible = wlaczWyszukiwanie;
            dataGridViewKlienci.Visible = wlaczWyszukiwanie;

            SetEditingFieldsReadOnly(wlaczWyszukiwanie);

            btnEdytuj.Text = wlaczWyszukiwanie ? "Zmień klienta" : "Zapisz zmiany";
            btnDodaj.Visible = !wlaczWyszukiwanie;
            btnNowy.Visible = !wlaczWyszukiwanie;
            btnUsun.Visible = !wlaczWyszukiwanie;

            if (wlaczWyszukiwanie)
            {
                await LoadKlienciAsync();
                panel2.Location = new Point(panel2.Location.X, dataGridViewKlienci.Bottom + 20);
                panel1.Location = new Point(panel1.Location.X, panel2.Bottom + 20);
            }
            else
            {
                panel2.Location = new Point(panel2.Location.X, OriginalPanel2Top);
                panel1.Location = new Point(panel1.Location.X, OriginalPanel1Top);
                if (selectedKlientId.HasValue)
                {
                    await WypelnijPolaEdycjiAsync(selectedKlientId.Value);
                    await WczytajZgloszeniaKlientaAsync(selectedKlientId.Value);
                }
            }
        }

        private void SetEditingFieldsReadOnly(bool isReadOnly)
        {
            Color backColor = isReadOnly ? SystemColors.Control : SystemColors.Window;
            foreach (var textBox in panel2.Controls.OfType<TextBox>())
            {
                textBox.ReadOnly = isReadOnly;
                textBox.BackColor = backColor;
            }
        }


        private async void btnEdytuj_Click(object sender, EventArgs e)
        {
            if (chkZmienKlienta.Checked)
            {
                if (selectedKlientId == null || dataGridViewKlienci.SelectedRows.Count == 0)
                {
                    ToastManager.ShowToast("Błąd walidacji", "Proszę wybrać z listy klienta, którego chcesz przypisać.", NotificationType.Warning);
                    return;
                }
                this.NowoWybranyKlientId = selectedKlientId;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                if (selectedKlientId == null)
                {
                    ToastManager.ShowToast("Informacja", "Najpierw wybierz klienta z listy, aby edytować jego dane.", NotificationType.Warning);
                    return;
                }
                try
                {
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        using (var transaction = con.BeginTransaction())
                        {
                            try
                            {
                                string query = @"UPDATE Klienci SET ImieNazwisko = @imie, NazwaFirmy = @firma, NIP = @nip, Ulica = @ulica, 
                                                 KodPocztowy = @kod, Miejscowosc = @miasto, Email = @mail, Telefon = @tel WHERE Id = @id";
                                using (var cmd = new MySqlCommand(query, con, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@id", selectedKlientId.Value);
                                    cmd.Parameters.AddWithValue("@imie", txtNowyImieNazwisko.Text);
                                    cmd.Parameters.AddWithValue("@firma", txtNazwaFirmy.Text);
                                    cmd.Parameters.AddWithValue("@nip", txtNIP.Text);
                                    cmd.Parameters.AddWithValue("@ulica", txtUlicaNr.Text);
                                    cmd.Parameters.AddWithValue("@kod", txtKodPocztowy.Text);
                                    cmd.Parameters.AddWithValue("@miasto", txtMiejscowosc.Text);
                                    cmd.Parameters.AddWithValue("@mail", txtMail.Text);
                                    cmd.Parameters.AddWithValue("@tel", txtTelefon.Text);
                                    await cmd.ExecuteNonQueryAsync();
                                }

                                DziennikLogger dziennik = new DziennikLogger();
                                await dziennik.DodajAsync(con, transaction, Program.fullName, "Zaktualizowano dane klienta", "0");

                                transaction.Commit();
                                this.CzyDaneZostalyZmienione = true;
                                this.DialogResult = DialogResult.OK;
                                ToastManager.ShowToast("Sukces", "Dane klienta zostały zaktualizowane.", NotificationType.Success);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }
                    }
                    UpdateManager.NotifySubscribers();
                    await LoadKlienciAsync(txtWyszukajKlienta.Text);
                }
                catch (Exception ex) { ToastManager.ShowToast("Błąd zapisu", "Błąd zapisu zmian klienta: " + ex.Message, NotificationType.Error); }
            }
        }

        private async void btnUsun_Click(object sender, EventArgs e)
        {
            if (selectedKlientId == null)
            {
                ToastManager.ShowToast("Informacja", "Najpierw wybierz klienta z listy, aby go usunąć.", NotificationType.Warning);
                return;
            }

            var confirmResult = MessageBox.Show("Czy na pewno chcesz usunąć wybranego klienta? Spowoduje to usunięcie jego historii zgłoszeń.",
                                                 "Potwierdź usunięcie",
                                                 MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        using (var transaction = con.BeginTransaction())
                        {
                            string queryZgloszenia = "DELETE FROM Zgloszenia WHERE KlientID = @id";
                            using (var cmd = new MySqlCommand(queryZgloszenia, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@id", selectedKlientId.Value);
                                await cmd.ExecuteNonQueryAsync();
                            }

                            string queryKlient = "DELETE FROM Klienci WHERE Id = @id";
                            using (var cmd = new MySqlCommand(queryKlient, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@id", selectedKlientId.Value);
                                await cmd.ExecuteNonQueryAsync();
                            }

                            transaction.Commit();
                            ToastManager.ShowToast("Sukces", "Klient został pomyślnie usunięty.", NotificationType.Success);
                        }
                    }
                    UpdateManager.NotifySubscribers();
                    await LoadKlienciAsync();
                    WyczyscPola();
                }
                catch (Exception ex)
                {
                    ToastManager.ShowToast("Błąd usuwania", "Wystąpił błąd podczas usuwania klienta: " + ex.Message, NotificationType.Error);
                }
            }
        }

        private async Task LoadKlienciAsync(string fraza = "")
        {
            var wasSelected = selectedKlientId;
            dataGridViewKlienci.CellClick -= dataGridViewKlienci_CellClick;
            dataGridViewKlienci.DataSource = null;
            var klienci = new List<object>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "SELECT Id, ImieNazwisko, NazwaFirmy, Email, Telefon FROM Klienci WHERE (@fraza = '' OR ImieNazwisko LIKE @wzorzec OR NazwaFirmy LIKE @wzorzec OR NIP LIKE @wzorzec) ORDER BY ImieNazwisko";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@fraza", fraza.Trim());
                        cmd.Parameters.AddWithValue("@wzorzec", $"%{fraza.Trim()}%");
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                klienci.Add(new
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    ImieNazwisko = reader["ImieNazwisko"].ToString(),
                                    NazwaFirmy = reader["NazwaFirmy"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Telefon = reader["Telefon"].ToString()
                                });
                            }
                        }
                    }
                }
                dataGridViewKlienci.DataSource = klienci;
                if (dataGridViewKlienci.Columns.Contains("Id")) dataGridViewKlienci.Columns["Id"].Visible = false;

                if (wasSelected.HasValue) SelectClientById(wasSelected.Value);
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd wczytywania", "Błąd wczytywania listy klientów: " + ex.Message, NotificationType.Error);
            }
            finally
            {
                dataGridViewKlienci.CellClick += dataGridViewKlienci_CellClick;
                if (dataGridViewKlienci.Rows.Count == 1)
                {
                    dataGridViewKlienci.Rows[0].Selected = true;
                    await HandleSelectionChange(0);
                }
                else if (dataGridViewKlienci.Rows.Count == 0)
                {
                    WyczyscPola();
                }
            }
        }

        private void SelectClientById(int clientId)
        {
            foreach (DataGridViewRow row in dataGridViewKlienci.Rows)
            {
                if (Convert.ToInt32(row.Cells["Id"].Value) == clientId)
                {
                    row.Selected = true;
                    dataGridViewKlienci.FirstDisplayedScrollingRowIndex = row.Index;
                    return;
                }
            }
        }

        private async void dataGridViewKlienci_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                await HandleSelectionChange(e.RowIndex);
            }
        }

        private async Task HandleSelectionChange(int rowIndex)
        {
            selectedKlientId = Convert.ToInt32(dataGridViewKlienci.Rows[rowIndex].Cells["Id"].Value);
            if (chkZmienKlienta.Checked)
            {
                this.NowoWybranyKlientId = selectedKlientId;
            }
            await WypelnijPolaEdycjiAsync(selectedKlientId.Value);
            await WczytajZgloszeniaKlientaAsync(selectedKlientId.Value);
            btnEdytuj.Visible = true;
            btnUsun.Visible = !initialKlientId.HasValue;
        }

        private async Task WypelnijPolaEdycjiAsync(int id)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "SELECT * FROM Klienci WHERE Id = @id";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                txtNowyImieNazwisko.Text = reader["ImieNazwisko"].ToString();
                                txtNazwaFirmy.Text = reader["NazwaFirmy"].ToString();
                                txtNIP.Text = reader["NIP"].ToString();
                                txtUlicaNr.Text = reader["Ulica"].ToString();
                                txtKodPocztowy.Text = reader["KodPocztowy"].ToString();
                                txtMiejscowosc.Text = reader["Miejscowosc"].ToString();
                                txtMail.Text = reader["Email"].ToString();
                                txtTelefon.Text = reader["Telefon"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { ToastManager.ShowToast("Błąd wczytywania", "Błąd wczytywania danych klienta do edycji: " + ex.Message, NotificationType.Error); }
        }

        private async Task WczytajZgloszeniaKlientaAsync(int idKlienta)
        {
            dataGridViewZgloszenia.DataSource = null;
            var zgloszenia = new List<object>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = @"SELECT z.NrZgloszenia, z.DataZgloszenia, p.NazwaKrotka, z.StatusOgolny
                                     FROM Zgloszenia z LEFT JOIN Produkty p ON z.ProduktID = p.Id
                                     WHERE z.KlientID = @id ORDER BY z.DataZgloszenia DESC";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", idKlienta);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                zgloszenia.Add(new
                                {
                                    Numer = reader["NrZgloszenia"].ToString(),
                                    Data = DateTime.Parse(reader["DataZgloszenia"].ToString()).ToString("dd.MM.yyyy"),
                                    Produkt = reader["NazwaKrotka"].ToString(),
                                    Status = reader["StatusOgolny"].ToString()
                                });
                            }
                        }
                    }
                }
                dataGridViewZgloszenia.DataSource = zgloszenia;
            }
            catch (Exception ex) { ToastManager.ShowToast("Błąd wczytywania", "Błąd wczytywania zgłoszeń klienta: " + ex.Message, NotificationType.Error); }
        }

        private void btnNowy_Click(object sender, EventArgs e) => WyczyscPola();

        private void WyczyscPola()
        {
            selectedKlientId = null;
            dataGridViewKlienci.ClearSelection();
            dataGridViewZgloszenia.DataSource = null;
            txtNowyImieNazwisko.Clear();
            txtNazwaFirmy.Clear();
            txtNIP.Clear();
            txtUlicaNr.Clear();
            txtKodPocztowy.Clear();
            txtMiejscowosc.Clear();
            txtMail.Clear();
            txtTelefon.Clear();
            txtNowyImieNazwisko.Focus();
            btnEdytuj.Visible = false;
            btnUsun.Visible = false;
        }

        private async void btnDodaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNowyImieNazwisko.Text))
            {
                ToastManager.ShowToast("Błąd walidacji", "Pole 'Imię i nazwisko' jest wymagane.", NotificationType.Warning);
                return;
            }
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            string query = "INSERT INTO Klienci (ImieNazwisko, NazwaFirmy, NIP, Ulica, KodPocztowy, Miejscowosc, Email, Telefon) VALUES (@imie, @firma, @nip, @ulica, @kod, @miasto, @mail, @tel)";
                            using (var cmd = new MySqlCommand(query, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@imie", txtNowyImieNazwisko.Text);
                                cmd.Parameters.AddWithValue("@firma", txtNazwaFirmy.Text);
                                cmd.Parameters.AddWithValue("@nip", txtNIP.Text);
                                cmd.Parameters.AddWithValue("@ulica", txtUlicaNr.Text);
                                cmd.Parameters.AddWithValue("@kod", txtKodPocztowy.Text);
                                cmd.Parameters.AddWithValue("@miasto", txtMiejscowosc.Text);
                                cmd.Parameters.AddWithValue("@mail", txtMail.Text);
                                cmd.Parameters.AddWithValue("@tel", txtTelefon.Text);
                                await cmd.ExecuteNonQueryAsync();
                            }
                            DziennikLogger dziennik = new DziennikLogger();
                            await dziennik.DodajAsync(con, transaction, Program.fullName, $"Dodano nowego klienta: {txtNowyImieNazwisko.Text}", "0");

                            transaction.Commit();
                            ToastManager.ShowToast("Sukces", "Nowy klient został dodany.", NotificationType.Success);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                UpdateManager.NotifySubscribers();
                await LoadKlienciAsync(txtWyszukajKlienta.Text);
                WyczyscPola();
            }
            catch (Exception ex) { ToastManager.ShowToast("Błąd dodawania", "Błąd dodawania nowego klienta: " + ex.Message, NotificationType.Error); }
        }

        private void dataGridViewZgloszenia_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string nrZgloszenia = dataGridViewZgloszenia.Rows[e.RowIndex].Cells["Numer"].Value.ToString();
                using (var form2 = new Form2(nrZgloszenia))
                {
                    form2.ShowDialog();
                }
            }
        }

        private void txtWyszukajKlienta_TextChanged(object sender, EventArgs e)
        {
            searchDebounceTimer.Stop();
            searchDebounceTimer.Start();
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