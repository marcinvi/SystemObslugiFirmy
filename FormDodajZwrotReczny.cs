// Plik: FormDodajZwrotReczny.cs (OSTATECZNA POPRAWKA)

using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    public partial class FormDodajZwrotReczny : Form
    {
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly DatabaseService _dbServiceBaza;
        private readonly string _fullName;

        private class UserItem
        {
            public int Id { get; set; }
            public string NazwaWyswietlana { get; set; }
            public override string ToString() => NazwaWyswietlana;
        }

        private class StatusItem
        {
            public int Id { get; set; }
            public string Nazwa { get; set; }
            public override string ToString() => Nazwa;
        }

        public FormDodajZwrotReczny()
        {
            InitializeComponent();
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
            _fullName = Program.fullName;

            // Uwaga: Zdarzenie CheckedChanged jest usuwane z FormDodajZwrotReczny.Designer.cs,
            // a podpinane dopiero w metodzie Load.
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormDodajZwrotReczny_Load(object sender, EventArgs e)
        {
            await LoadHandlowcyAsync();
            await LoadProductSuggestionsAsync();
            await LoadCarrierSuggestionsAsync();
            await LoadStanProduktuAsync();

            // Ręcznie podpinamy zdarzenie dopiero po pełnym załadowaniu danych.
            chkWszyscyHandlowcy.CheckedChanged += chkWszyscyHandlowcy_CheckedChanged;
        }

        private async Task LoadHandlowcyAsync()
        {
            try
            {
                string query = "SELECT Id, \"Nazwa Wyświetlana\" FROM Uzytkownicy WHERE Rola = 'Handlowiec' ORDER BY \"Nazwa Wyświetlana\"";
                var dt = await _dbServiceBaza.GetDataTableAsync(query);

                var handlowcy = dt.AsEnumerable().Select(row => new UserItem
                {
                    Id = Convert.ToInt32(row["Id"]),
                    NazwaWyswietlana = row["Nazwa Wyświetlana"]?.ToString()
                }).ToList();

                // ——— ważne: bez DataSource, czyszczenie i ręczne dodanie pozycji
                checkedListBoxHandlowcy.BeginUpdate();
                try
                {
                    checkedListBoxHandlowcy.DisplayMember = nameof(UserItem.NazwaWyswietlana);
                    checkedListBoxHandlowcy.ValueMember = nameof(UserItem.Id);
                    checkedListBoxHandlowcy.Items.Clear();

                    foreach (var u in handlowcy)
                        checkedListBoxHandlowcy.Items.Add(u);

                    // Zaznacz wszystko dopiero PO zapełnieniu listy
                    if (chkWszyscyHandlowcy.Checked)
                    {
                        for (int i = 0; i < checkedListBoxHandlowcy.Items.Count; i++)
                            checkedListBoxHandlowcy.SetItemChecked(i, true);
                    }
                }
                finally
                {
                    checkedListBoxHandlowcy.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się wczytać listy handlowców: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async Task LoadStanProduktuAsync()
        {
            try
            {
                var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT Id, Nazwa FROM Statusy WHERE TypStatusu = 'StanProduktu' ORDER BY Nazwa");
                var statusy = dt.AsEnumerable().Select(row => new StatusItem
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nazwa = row["Nazwa"].ToString()
                }).ToList();

                comboStanProduktu.DataSource = statusy;
                comboStanProduktu.DisplayMember = "Nazwa";
                comboStanProduktu.ValueMember = "Id";
                comboStanProduktu.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania statusów: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProductSuggestionsAsync()
        {
            try
            {
                string query = "SELECT DISTINCT ProductName FROM AllegroCustomerReturns WHERE ProductName IS NOT NULL AND TRIM(ProductName) != '' ORDER BY ProductName";
                var dt = await _dbServiceMagazyn.GetDataTableAsync(query);
                var items = dt.AsEnumerable().Select(r => r.Field<string>("ProductName")).ToArray();

                comboProdukt.Items.AddRange(items);
                comboProdukt.AutoCompleteCustomSource.AddRange(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nie udało się wczytać podpowiedzi produktów: " + ex.Message);
            }
        }

        private async Task LoadCarrierSuggestionsAsync()
        {
            try
            {
                string query = "SELECT DISTINCT CarrierName FROM AllegroCustomerReturns WHERE CarrierName IS NOT NULL AND TRIM(CarrierName) != '' ORDER BY CarrierName";
                var dt = await _dbServiceMagazyn.GetDataTableAsync(query);
                var items = dt.AsEnumerable().Select(r => r.Field<string>("CarrierName")).ToArray();

                comboPrzewoznik.Items.AddRange(items);
                comboPrzewoznik.AutoCompleteCustomSource.AddRange(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nie udało się wczytać podpowiedzi przewoźników: " + ex.Message);
            }
        }

        private async Task<string> GenerateNewReferenceNumber()
        {
            string monthYear = DateTime.Now.ToString("MM/yy");
            string pattern = $"R/%/{monthYear}";

            var cmd = new MySqlCommand("SELECT ReferenceNumber FROM AllegroCustomerReturns WHERE ReferenceNumber LIKE @pattern ORDER BY ReferenceNumber DESC LIMIT 1");
            cmd.Parameters.AddWithValue("@pattern", pattern);

            var lastNumberStr = (await _dbServiceMagazyn.ExecuteScalarAsync(cmd.CommandText, cmd.Parameters.Cast<MySqlParameter>().ToArray()))?.ToString();

            int nextId = 1;
            if (lastNumberStr != null)
            {
                try
                {
                    string[] parts = lastNumberStr.Split('/');
                    if (parts.Length == 3)
                    {
                        nextId = int.Parse(parts[1]) + 1;
                    }
                }
                catch { /* Ignoruj błąd parsowania, użyj 1 */ }
            }

            return $"R/{nextId:D3}/{monthYear}";
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumerListu.Text))
            {
                MessageBox.Show("Numer listu przewozowego jest wymagany.", "Walidacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboStanProduktu.SelectedItem == null)
            {
                MessageBox.Show("Stan produktu jest wymagany.", "Walidacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (checkedListBoxHandlowcy.CheckedItems.Count == 0)
            {
                MessageBox.Show("Należy wybrać co najmniej jednego handlowca do powiadomienia.", "Walidacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnZapisz.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            long newReturnId = -1;

            using (var con = MagazynDatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        var cmdStatus = new MySqlCommand("SELECT Id FROM Statusy WHERE Nazwa = 'Oczekuje na decyzję handlowca' AND TypStatusu = 'StatusWewnetrzny'", con, transaction);
                        var statusDocelowyId = await cmdStatus.ExecuteScalarAsync();
                        if (statusDocelowyId == null)
                        {
                            throw new Exception("Krytyczny błąd: W bazie danych brakuje statusu 'Oczekuje na decyzję handlowca'.");
                        }

                        var cmd = new MySqlCommand(@"
                            INSERT INTO AllegroCustomerReturns (
                                ReferenceNumber, Waybill, CreatedAt, DataPrzyjecia, PrzyjetyPrzezId, StatusAllegro, StatusWewnetrznyId, 
                                StanProduktuId, IsManual, ManualSenderDetails, CarrierName, ProductName, 
                                Delivery_FirstName, Delivery_LastName, Delivery_Street, Delivery_ZipCode, Delivery_City, Delivery_PhoneNumber, 
                                UwagiMagazynu, BuyerFullName
                            ) VALUES (
                                @ReferenceNumber, @Waybill, @CreatedAt, @DataPrzyjecia, @PrzyjetyPrzezId, @StatusAllegro, @StatusWewnetrznyId,
                                @StanProduktuId, @IsManual, @ManualSenderDetails, @CarrierName, @ProductName,
                                @Delivery_FirstName, @Delivery_LastName, @Delivery_Street, @Delivery_ZipCode, @Delivery_City, @Delivery_PhoneNumber,
                                @Uwagi, @BuyerFullName
                            );
                            SELECT LAST_INSERT_ID();", con, transaction);

                        string refNumber = await GenerateNewReferenceNumber();
                        var nameParts = txtImieNazwisko.Text.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        var senderDetails = new { FullName = txtImieNazwisko.Text.Trim(), Street = txtUlica.Text.Trim(), ZipCode = txtKodPocztowy.Text.Trim(), City = txtMiasto.Text.Trim(), Phone = txtTelefon.Text.Trim() };

                        cmd.Parameters.AddWithValue("@ReferenceNumber", refNumber);
                        cmd.Parameters.AddWithValue("@Waybill", txtNumerListu.Text.Trim());
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@DataPrzyjecia", DateTime.Now);
                        cmd.Parameters.AddWithValue("@PrzyjetyPrzezId", SessionManager.CurrentUserId);
                        cmd.Parameters.AddWithValue("@StatusAllegro", "MANUAL");
                        cmd.Parameters.AddWithValue("@StatusWewnetrznyId", statusDocelowyId);
                        cmd.Parameters.AddWithValue("@StanProduktuId", ((StatusItem)comboStanProduktu.SelectedItem).Id);
                        cmd.Parameters.AddWithValue("@IsManual", 1);
                        cmd.Parameters.AddWithValue("@ManualSenderDetails", JsonConvert.SerializeObject(senderDetails));
                        cmd.Parameters.AddWithValue("@CarrierName", (object)comboPrzewoznik.Text.Trim() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductName", (object)comboProdukt.Text.Trim() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_FirstName", nameParts.Length > 0 ? nameParts[0] : "");
                        cmd.Parameters.AddWithValue("@Delivery_LastName", nameParts.Length > 1 ? nameParts[1] : "");
                        cmd.Parameters.AddWithValue("@Delivery_Street", (object)txtUlica.Text.Trim() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_ZipCode", (object)txtKodPocztowy.Text.Trim() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_City", (object)txtMiasto.Text.Trim() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_PhoneNumber", (object)txtTelefon.Text.Trim() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Uwagi", (object)txtUwagi.Text.Trim() ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BuyerFullName", (object)txtImieNazwisko.Text.Trim() ?? DBNull.Value);

                        newReturnId = Convert.ToInt64(await cmd.ExecuteScalarAsync());
                        var wybrani = checkedListBoxHandlowcy.CheckedItems.Cast<UserItem>().ToList();
                        string odbiorcyLista = string.Join(", ", wybrani.Select(u => u.NazwaWyswietlana));

                        // 1) Wyślij wiadomości do każdego odbiorcy
                        foreach (UserItem selectedUser in wybrani)
                        {
                            string trescWiadomosci = $"Nowy zwrot ręczny ({refNumber}) oczekuje na Twoją decyzję.";
                            var cmdWiadomosc = new MySqlCommand(
                                "INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tresc, DataWyslania, DotyczyZwrotuId, CzyOdczytana) " +
                                "VALUES (@nadawcaId, @odbiorcaId, @tresc, @data, @zwrotId, 0)", con, transaction);
                            cmdWiadomosc.Parameters.AddWithValue("@nadawcaId", SessionManager.CurrentUserId);
                            cmdWiadomosc.Parameters.AddWithValue("@odbiorcaId", selectedUser.Id);
                            cmdWiadomosc.Parameters.AddWithValue("@tresc", trescWiadomosci);
                            cmdWiadomosc.Parameters.AddWithValue("@data", DateTime.Now);
                            cmdWiadomosc.Parameters.AddWithValue("@zwrotId", newReturnId);
                            await cmdWiadomosc.ExecuteNonQueryAsync();
                        }

                        // 2) Czytelny wpis do DZIAŁAŃ (kto otrzymał)
                        var cmdDzialanie = new MySqlCommand(
                            "INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc) " +
                            "VALUES (@zwrotId, @data, @uzytkownik, @tresc)", con, transaction);
                        cmdDzialanie.Parameters.AddWithValue("@zwrotId", newReturnId);
                        cmdDzialanie.Parameters.AddWithValue("@data", DateTime.Now);
                        cmdDzialanie.Parameters.AddWithValue("@uzytkownik", _fullName);
                        cmdDzialanie.Parameters.AddWithValue("@tresc", $"Zwrot przekazany do decyzji handlowców: {odbiorcyLista}.");
                        await cmdDzialanie.ExecuteNonQueryAsync();

                        // 3) Czytelny wpis do DZIENNIKA (z odbiorcami)
                        var cmdDziennik = new MySqlCommand(
                            "INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId) " +
                            "VALUES (@data, @uzytkownik, @akcja, @id)", con, transaction);
                        cmdDziennik.Parameters.AddWithValue("@data", DateTime.Now);
                        cmdDziennik.Parameters.AddWithValue("@uzytkownik", _fullName);
                        cmdDziennik.Parameters.AddWithValue("@akcja",
                            $"Dodano i przekazano do decyzji zwrot ręczny: {refNumber}. Odbiorcy: {odbiorcyLista}.");
                        cmdDziennik.Parameters.AddWithValue("@id", newReturnId);
                        await cmdDziennik.ExecuteNonQueryAsync();

                        transaction.Commit();
                        AppEvents.RaiseZwrotyChanged();
                        ToastManager.ShowToast("Sukces", "Zadanie zostało przekazane do handlowca.", NotificationType.Success);

                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Wystąpił błąd podczas zapisu: " + ex.Message, "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            this.Cursor = Cursors.Default;
            btnZapisz.Enabled = true;
        }

        private void chkWszyscyHandlowcy_CheckedChanged(object sender, EventArgs e)
        {
            if (checkedListBoxHandlowcy.DataSource != null)
            {
                for (int i = 0; i < checkedListBoxHandlowcy.Items.Count; i++)
                {
                    checkedListBoxHandlowcy.SetItemChecked(i, chkWszyscyHandlowcy.Checked);
                }
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