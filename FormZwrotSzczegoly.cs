// ######################################################################################
// Plik: FormZwrotSzczegoly.cs (WERSJA DOSTOSOWANA DO NOWOCZESNEGO UI)
// Opis: Zaktualizowana logika formularza, w pełni kompatybilna z nowym
//       plikiem Designer.cs. Usunięto błędy kompilacji i dostosowano
//       wypełnianie kontrolek do nowego układu opartego na kartach.
// ######################################################################################

using Reklamacje_Dane.Allegro.Returns;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Linq;

namespace Reklamacje_Dane
{
    public partial class FormZwrotSzczegoly : Form
    {
        private readonly int _returnDbId;
        private readonly string _fullName;
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly DatabaseService _dbServiceBaza;
        private DataRow _dbDataRow;
        private string _uwagiMagazynuColumnName = "UwagiMagazynu";

        private class StatusItem
        {
            public int Id { get; set; }
            public string Nazwa { get; set; }
            public override string ToString() => Nazwa;
        }

        public FormZwrotSzczegoly(int returnDbId, string fullName)
        {
            InitializeComponent();
            _returnDbId = returnDbId;
            _fullName = fullName;
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());

            // Podpięcie event handlerów - najlepiej robić to w konstruktorze
            this.Load += FormZwrotSzczegoly_Load;
           
            this.btnPrzekazDoHandlowca.Click += new System.EventHandler(this.btnPrzekazDoHandlowca_Click);
            this.btnAnuluj.Click += new System.EventHandler(this.btnAnuluj_Click);
            this.btnShowOtherAddresses.Click += new System.EventHandler(this.btnShowOtherAddresses_Click);
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormZwrotSzczegoly_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            await LoadReturnDataAsync();
            await LoadStatusesAsync();
            await PopulateControlsAsync(); // Zmieniono na async, by móc pobrać dodatkowe dane
            this.Cursor = Cursors.Default;
        }

        private async Task LoadReturnDataAsync()
        {
            try
            {
                _uwagiMagazynuColumnName = await ResolveUwagiMagazynuColumnAsync();
                var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT * FROM AllegroCustomerReturns WHERE Id = @id", new MySqlParameter("@id", _returnDbId));
                if (dt.Rows.Count > 0)
                {
                    _dbDataRow = dt.Rows[0];
                }
                else
                {
                    throw new Exception("Nie odnaleziono zwrotu o podanym ID w bazie danych.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania szczegółów zwrotu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async Task LoadStatusesAsync()
        {
            try
            {
                var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT Id, Nazwa FROM Statusy WHERE TypStatusu = 'StanProduktu' ORDER BY Nazwa");
                var statusy = new List<StatusItem>();
                foreach (DataRow row in dt.Rows)
                {
                    statusy.Add(new StatusItem { Id = Convert.ToInt32(row["Id"]), Nazwa = row["Nazwa"].ToString() });
                }
                comboStanProduktu.DataSource = statusy;
                comboStanProduktu.DisplayMember = "Nazwa";
                comboStanProduktu.ValueMember = "Id";

                if (_dbDataRow != null && _dbDataRow["StanProduktuId"] != DBNull.Value)
                {
                    comboStanProduktu.SelectedValue = Convert.ToInt32(_dbDataRow["StanProduktuId"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania statusów: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PopulateControlsAsync()
        {
            if (_dbDataRow == null) return;

            // --- Pobieranie dodatkowych danych (nazwy konta, statusu) ---
            string allegroAccountName = "N/A (Zwrot ręczny)";
            if (_dbDataRow["AllegroAccountId"] != DBNull.Value)
            {
                int accountId = Convert.ToInt32(_dbDataRow["AllegroAccountId"]);
                allegroAccountName = (await _dbServiceBaza.ExecuteScalarAsync("SELECT AccountName FROM AllegroAccounts WHERE Id = @id", new MySqlParameter("@id", accountId)))?.ToString() ?? "Nieznane";
            }

            string statusWewnetrzny = "Brak statusu";
            if (_dbDataRow["StatusWewnetrznyId"] != DBNull.Value)
            {
                int statusId = Convert.ToInt32(_dbDataRow["StatusWewnetrznyId"]);
                statusWewnetrzny = (await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Nazwa FROM Statusy WHERE Id = @id", new MySqlParameter("@id", statusId)))?.ToString() ?? "Nieznany";
            }

            // --- Wypełnianie kontrolek w nowym layoucie ---

            // Nagłówek
            this.Text = $"Szczegóły zwrotu: {_dbDataRow["ReferenceNumber"]}"; // Zachowujemy dla paska tytułowego okna
            lblReturnNumber.Text = $"Zwrot #{_dbDataRow["ReferenceNumber"]}";
            lblReturnStatus.Text = $"Status: {statusWewnetrzny}";

            // Karta: Zwracany produkt
            lblProductName.Text = _dbDataRow["ProductName"]?.ToString() ?? "Brak";
            lblOfferId.Text = _dbDataRow["OfferId"]?.ToString() ?? "Brak";
            lblQuantity.Text = _dbDataRow["Quantity"]?.ToString() ?? "Brak";
            if (_dbDataRow["JsonDetails"] != DBNull.Value && !string.IsNullOrEmpty(_dbDataRow["JsonDetails"].ToString()))
            {
                var returnDetails = JsonConvert.DeserializeObject<AllegroCustomerReturn>(_dbDataRow["JsonDetails"].ToString());
                var item = returnDetails?.Items?.FirstOrDefault();
                string reasonType = item?.Reason?.Type;
                string reasonLabel = TranslateReturnReasonType(reasonType);
                string reasonDescription = string.IsNullOrWhiteSpace(reasonLabel)
                    ? reasonType
                    : $"{reasonLabel} ({reasonType})";
                string userComment = item?.Reason?.UserComment;
                lblReason.Text = string.IsNullOrWhiteSpace(reasonDescription)
                    ? "Brak"
                    : string.IsNullOrWhiteSpace(userComment)
                        ? reasonDescription
                        : $"{reasonDescription} — {userComment}";
            }
            else
            {
                lblReason.Text = "Brak (zwrot ręczny)";
            }

            // Karta: Przesyłka
            lblWaybill.Text = _dbDataRow["Waybill"]?.ToString() ?? "Brak";
            lblCarrier.Text = _dbDataRow["CarrierName"]?.ToString() ?? "Brak";

            // Karta: Dane kupującego
            string buyerName = $"{_dbDataRow["Delivery_FirstName"]} {_dbDataRow["Delivery_LastName"]}".Trim();
            if (string.IsNullOrEmpty(buyerName))
            {
                buyerName = $"{_dbDataRow["Buyer_FirstName"]} {_dbDataRow["Buyer_LastName"]}".Trim();
            }
            if (string.IsNullOrEmpty(buyerName))
            {
                buyerName = _dbDataRow["BuyerLogin"]?.ToString() ?? "Brak danych";
            }
            lblBuyerName.Text = buyerName;


            string mainAddress = "Brak adresu dostawy";
            if (!string.IsNullOrEmpty(_dbDataRow["Delivery_Street"]?.ToString()))
            {
                mainAddress = $"{_dbDataRow["Delivery_Street"]}, {_dbDataRow["Delivery_ZipCode"]} {_dbDataRow["Delivery_City"]}";
            }
            else if (!string.IsNullOrEmpty(_dbDataRow["Buyer_Street"]?.ToString()))
            {
                mainAddress = $"{_dbDataRow["Buyer_Street"]}, {_dbDataRow["Buyer_ZipCode"]} {_dbDataRow["Buyer_City"]}";
            }
            lblBuyerAddress.Text = mainAddress;

            string phoneNumber = _dbDataRow["Delivery_PhoneNumber"]?.ToString();
            if (string.IsNullOrEmpty(phoneNumber))
            {
                phoneNumber = _dbDataRow["Buyer_PhoneNumber"]?.ToString();
            }
            lblBuyerPhone.Text = phoneNumber ?? "Brak";


            // Karta: Panel Magazynu
            txtUwagiMagazynu.Text = GetUwagiMagazynuValue();
            lblOpiekunValue.Text = await GetOpiekunInfoAsync();
        }

        private void btnShowOtherAddresses_Click(object sender, EventArgs e)
        {
            if (_dbDataRow == null) return;

            // Używamy nowego, dedykowanego formularza zamiast MessageBox
            using (var formAdresy = new FormWyswietlAdresy(_dbDataRow))
            {
                formAdresy.ShowDialog(this);
            }
        }

        private async Task LogToDziennikAsync(string akcja)
        {
            string query = "INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId) VALUES (@data, @uzytkownik, @akcja, @id)";
            var parameters = new[]
            {
                new MySqlParameter("@data", DateTime.Now),
                new MySqlParameter("@uzytkownik", _fullName),
                new MySqlParameter("@akcja", akcja),
                new MySqlParameter("@id", _returnDbId)
            };
            await _dbServiceMagazyn.ExecuteNonQueryAsync(query, parameters);
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            if (comboStanProduktu.SelectedValue == null)
            {
                MessageBox.Show("Proszę wybrać stan produktu.", "Walidacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = $@"UPDATE AllegroCustomerReturns SET
                                    StanProduktuId = @stanId,
                                    {_uwagiMagazynuColumnName} = @uwagi,
                                    DataPrzyjecia = @data,
                                    PrzyjetyPrzezId = @pracownikId
                                 WHERE Id = @id";

                var parameters = new[]
                {
                    new MySqlParameter("@stanId", ((StatusItem)comboStanProduktu.SelectedItem).Id),
                    new MySqlParameter("@uwagi", txtUwagiMagazynu.Text.Trim()),
                    new MySqlParameter("@data", DateTime.Now),
                    new MySqlParameter("@pracownikId", SessionManager.CurrentUserId),
                    new MySqlParameter("@id", _returnDbId)
                };

                await _dbServiceMagazyn.ExecuteNonQueryAsync(query, parameters);
                await LogToDziennikAsync($"Zapisano zmiany. Stan produktu: {comboStanProduktu.Text}. Uwagi: {txtUwagiMagazynu.Text}");

                MessageBox.Show("Zmiany zostały zapisane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnPrzekazDoHandlowca_Click(object sender, EventArgs e)
        {
            if (comboStanProduktu.SelectedValue == null)
            {
                MessageBox.Show("Przed przekazaniem należy określić stan produktu.", "Walidacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "Czy na pewno chcesz zapisać zmiany i przekazać ten zwrot do decyzji handlowca?",
                "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            try
            {
                if (_dbDataRow["AllegroAccountId"] == DBNull.Value)
                {
                    MessageBox.Show(
                        "Nie można automatycznie przekazać zwrotu ręcznego. Skontaktuj się z handlowcem bezpośrednio.",
                        "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var accountId = Convert.ToInt32(_dbDataRow["AllegroAccountId"]);

                // 1) Opiekun konta
                var opiekunIdObj = await _dbServiceMagazyn.ExecuteScalarAsync(
                    "SELECT OpiekunId FROM AllegroAccountOpiekun WHERE AllegroAccountId = @id",
                    new MySqlParameter("@id", accountId));

                if (opiekunIdObj == null || opiekunIdObj == DBNull.Value)
                {
                    MessageBox.Show("Dla tego konta Allegro nie przypisano opiekuna (handlowca). " +
                                    "Przejdź do Ustawień, aby go przypisać.",
                                    "Błąd Konfiguracji", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var opiekunId = Convert.ToInt32(opiekunIdObj);
                int finalRecipientId = opiekunId;

                // Nazwy użytkowników (do logów)
                string opiekunName = (await _dbServiceBaza.ExecuteScalarAsync(
                    "SELECT \"Nazwa Wyświetlana\" FROM Uzytkownicy WHERE Id = @id",
                    new MySqlParameter("@id", opiekunId)))?.ToString();

                // 2) Delegacje (jeśli są)
                string informacjaODelegacji = "";
                string finalRecipientName = opiekunName;

                var zastepcaIdObj = await _dbServiceMagazyn.ExecuteScalarAsync(
                    "SELECT ZastepcaId FROM Delegacje " +
                    "WHERE UzytkownikId = @opiekunId " +
                    "AND CURDATE() BETWEEN DataOd AND DataDo " +
                    "AND CzyAktywna = 1",
                    new MySqlParameter("@opiekunId", opiekunId));

                if (zastepcaIdObj != null && zastepcaIdObj != DBNull.Value)
                {
                    finalRecipientId = Convert.ToInt32(zastepcaIdObj);
                    string zastepcaName = (await _dbServiceBaza.ExecuteScalarAsync(
                        "SELECT \"Nazwa Wyświetlana\" FROM Uzytkownicy WHERE Id = @id",
                        new MySqlParameter("@id", finalRecipientId)))?.ToString();
                    informacjaODelegacji = $"\n\n(Zadanie przekierowane do '{zastepcaName}' " +
                                           $"z powodu aktywnej delegacji dla '{opiekunName}')";
                    finalRecipientName = zastepcaName;
                }

                // 3) Status docelowy
                var statusIdObj = await _dbServiceMagazyn.ExecuteScalarAsync(
                    "SELECT Id FROM Statusy WHERE Nazwa = 'Oczekuje na decyzję handlowca' AND TypStatusu = 'StatusWewnetrzny'");
                if (statusIdObj == null)
                {
                    MessageBox.Show("Nie znaleziono w bazie wymaganego statusu 'Oczekuje na decyzję handlowca'.",
                        "Błąd Konfiguracji", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int statusId = Convert.ToInt32(statusIdObj);

                string stanName = comboStanProduktu.Text;
                string userName = _fullName;

                using (var con = MagazynDatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        // 3a) Aktualizacja zwrotu
                        var cmdUpdate = new MySqlCommand($@"
                    UPDATE AllegroCustomerReturns SET
                        StanProduktuId = @stanId,
                        {_uwagiMagazynuColumnName} = @uwagi,
                        DataPrzyjecia = @data,
                        PrzyjetyPrzezId = @pracownikId,
                        StatusWewnetrznyId = @statusId
                    WHERE Id = @id", con, transaction);

                        cmdUpdate.Parameters.AddWithValue("@stanId", ((StatusItem)comboStanProduktu.SelectedItem).Id);
                        cmdUpdate.Parameters.AddWithValue("@uwagi", txtUwagiMagazynu.Text.Trim());
                        cmdUpdate.Parameters.AddWithValue("@data", DateTime.Now);
                        cmdUpdate.Parameters.AddWithValue("@pracownikId", SessionManager.CurrentUserId);
                        cmdUpdate.Parameters.AddWithValue("@statusId", statusId);
                        cmdUpdate.Parameters.AddWithValue("@id", _returnDbId);
                        await cmdUpdate.ExecuteNonQueryAsync();

                        // 3b) Wiadomość do odbiorcy
                        string trescWiadomosci =
                            $"Prośba o decyzję dla zwrotu nr {_dbDataRow["ReferenceNumber"]} od {_dbDataRow["BuyerLogin"]}";
                        var cmdWiadomosc = new MySqlCommand(@"
                    INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tresc, DataWyslania, DotyczyZwrotuId) 
                    VALUES (@nadawcaId, @odbiorcaId, @tresc, @data, @zwrotId)", con, transaction);
                        cmdWiadomosc.Parameters.AddWithValue("@nadawcaId", SessionManager.CurrentUserId);
                        cmdWiadomosc.Parameters.AddWithValue("@odbiorcaId", finalRecipientId);
                        cmdWiadomosc.Parameters.AddWithValue("@tresc", trescWiadomosci);
                        cmdWiadomosc.Parameters.AddWithValue("@data", DateTime.Now);
                        cmdWiadomosc.Parameters.AddWithValue("@zwrotId", _returnDbId);
                        await cmdWiadomosc.ExecuteNonQueryAsync();

                        // 3c) Działania (z nazwą adresata + ewentualna delegacja)
                        string trescDzialania =
                            $"Zwrot przekazany do: {finalRecipientName} (ID: {finalRecipientId}). " +
                            $"Stan: '{stanName}', Uwagi: {txtUwagiMagazynu.Text.Trim()}";
                        if (!string.IsNullOrWhiteSpace(informacjaODelegacji))
                            trescDzialania += " " + informacjaODelegacji;

                        var cmdDzialanie = new MySqlCommand(@"
                    INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc) 
                    VALUES (@zwrotId, @data, @uzytkownik, @tresc)", con, transaction);
                        cmdDzialanie.Parameters.AddWithValue("@zwrotId", _returnDbId);
                        cmdDzialanie.Parameters.AddWithValue("@data", DateTime.Now);
                        cmdDzialanie.Parameters.AddWithValue("@uzytkownik", userName);
                        cmdDzialanie.Parameters.AddWithValue("@tresc", trescDzialania);
                        await cmdDzialanie.ExecuteNonQueryAsync();

                        // 3d) Dziennik (czytelne: do kogo)
                        var cmdDziennik = new MySqlCommand(
                            "INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId) " +
                            "VALUES (@data, @uzytkownik, @akcja, @id)", con, transaction);
                        cmdDziennik.Parameters.AddWithValue("@data", DateTime.Now);
                        cmdDziennik.Parameters.AddWithValue("@uzytkownik", userName);
                        cmdDziennik.Parameters.AddWithValue("@akcja",
                            $"Przekazano zwrot do: {finalRecipientName} (ID: {finalRecipientId}).");
                        cmdDziennik.Parameters.AddWithValue("@id", _returnDbId);
                        await cmdDziennik.ExecuteNonQueryAsync();

                        transaction.Commit();
                        AppEvents.RaiseZwrotyChanged();
                    }
                }
                ToastManager.ShowToast("Sukces", "Zadanie zostało przekazane do handlowca." + informacjaODelegacji, NotificationType.Success);
                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas przekazywania zwrotu: " + ex.Message,
                    "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

           
        

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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

        private async Task<string> ResolveUwagiMagazynuColumnAsync()
        {
            const string query = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'AllegroCustomerReturns'
                  AND COLUMN_NAME IN ('UwagiMagazynu', 'UwagiMagazyn')
                LIMIT 1";
            var result = await _dbServiceMagazyn.ExecuteScalarAsync(query);
            return result?.ToString() ?? "UwagiMagazynu";
        }

        private string GetUwagiMagazynuValue()
        {
            if (_dbDataRow?.Table?.Columns.Contains("UwagiMagazynu") == true)
            {
                return _dbDataRow["UwagiMagazynu"]?.ToString();
            }
            if (_dbDataRow?.Table?.Columns.Contains("UwagiMagazyn") == true)
            {
                return _dbDataRow["UwagiMagazyn"]?.ToString();
            }
            return string.Empty;
        }

        private string TranslateReturnReasonType(string reasonType)
        {
            if (string.IsNullOrWhiteSpace(reasonType))
            {
                return string.Empty;
            }

            switch (reasonType)
            {
                case "DONT_LIKE_IT":
                    return "Nie podoba się";
                case "NOT_AS_DESCRIBED":
                    return "Nie zgodny z opisem";
                case "DAMAGED":
                    return "Uszkodzony";
                case "MISSING_PARTS":
                    return "Brakujące elementy";
                case "WRONG_ITEM":
                    return "Niewłaściwy produkt";
                case "NO_LONGER_NEEDED":
                    return "Niepotrzebny";
                case "BETTER_PRICE_FOUND":
                    return "Znaleziono lepszą cenę";
                case "ORDERED_BY_MISTAKE":
                    return "Zakup przez pomyłkę";
                case "DEFECTIVE":
                    return "Wadliwy";
                case "DELIVERED_TOO_LATE":
                    return "Dostarczony zbyt późno";
                case "PRODUCT_DOES_NOT_WORK":
                    return "Produkt nie działa";
                case "QUALITY_UNSATISFACTORY":
                    return "Niezadowalająca jakość";
                default:
                    return string.Empty;
            }
        }
}
}
