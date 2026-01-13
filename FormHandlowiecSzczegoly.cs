// Plik: FormHandlowiecSzczegoly.cs (WERSJA Z POPRAWKĄ BŁĘDU CS1061)
// Opis: Naprawiono błąd odwołania do właściwości 'Value' w obiekcie 'Delivery',
//       która nie istnieje. Użyto bezpośredniego odwołania do 'Amount'.

using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Returns;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Reklamacje_Dane
{
    public partial class FormHandlowiecSzczegoly : Form
    {
        private readonly int _returnDbId;
        private readonly string _fullName;
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly DatabaseService _dbServiceBaza;
        private DataRow _dbDataRow;
        private OrderDetails _orderDetails;

        private class StatusItem
        {
            public int Id { get; set; }
            public string Nazwa { get; set; }
            public override string ToString() => Nazwa;
        }

        public FormHandlowiecSzczegoly(int returnDbId, string fullName)
        {
            InitializeComponent();
            _returnDbId = returnDbId;
            _fullName = fullName;
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormHandlowiecSzczegoly_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            await LoadReturnDataAsync();
            await LoadDecyzjeAsync();
            await PopulateControls();
            await LoadDzialaniaAsync();
            this.Cursor = Cursors.Default;
        }

        private async Task LoadReturnDataAsync()
        {
            try
            {
                var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT * FROM AllegroCustomerReturns WHERE Id = @id", new MySqlParameter("@id", _returnDbId));
                if (dt.Rows.Count > 0)
                {
                    _dbDataRow = dt.Rows[0];
                    if (_dbDataRow["AllegroAccountId"] != DBNull.Value)
                    {
                        using (var con = Database.GetNewOpenConnection())
                        {
                            var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(Convert.ToInt32(_dbDataRow["AllegroAccountId"]), con);
                            if (apiClient != null && _dbDataRow["OrderId"] != DBNull.Value)
                            {
                                _orderDetails = await apiClient.GetOrderDetailsByCheckoutFormIdAsync(_dbDataRow["OrderId"].ToString());
                            }
                        }
                    }
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

        private async Task LoadDecyzjeAsync()
        {
            try
            {
                var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT Id, Nazwa FROM Statusy WHERE TypStatusu = 'DecyzjaHandlowca' ORDER BY Nazwa");
                var decyzje = new List<StatusItem>();
                foreach (DataRow row in dt.Rows)
                {
                    decyzje.Add(new StatusItem { Id = Convert.ToInt32(row["Id"]), Nazwa = row["Nazwa"].ToString() });
                }
                comboDecyzja.DataSource = decyzje;
                comboDecyzja.DisplayMember = "Nazwa";
                comboDecyzja.ValueMember = "Id";
                comboDecyzja.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania statusów decyzji: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PopulateControls()
        {
            if (_dbDataRow == null) return;

            string allegroAccountName = "N/A";
            if (_dbDataRow["AllegroAccountId"] != DBNull.Value)
            {
                int accountId = Convert.ToInt32(_dbDataRow["AllegroAccountId"]);
                allegroAccountName = (await _dbServiceBaza.ExecuteScalarAsync("SELECT AccountName FROM AllegroAccounts WHERE Id = @id", new MySqlParameter("@id", accountId)))?.ToString() ?? "Nieznane";
            }

            lblTitle.Text = $"Decyzja dla zwrotu: {_dbDataRow["ReferenceNumber"]}";
            this.Text = lblTitle.Text;

            lblProductName.Text = _dbDataRow["ProductName"]?.ToString();
            lblBuyerLogin.Text = _dbDataRow["BuyerLogin"]?.ToString();
            lblAllegroAccount.Text = allegroAccountName;
            lblOrderDate.Text = FormatDateTime(_dbDataRow["CreatedAt"]);
            lblInvoice.Text = _dbDataRow["InvoiceNumber"]?.ToString() ?? "Brak";

            if (_dbDataRow["StanProduktuId"] != DBNull.Value)
            {
                lblStanProduktu.Text = (await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Nazwa FROM Statusy WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["StanProduktuId"])))?.ToString() ?? "Brak";
            }
            if (_dbDataRow["PrzyjetyPrzezId"] != DBNull.Value)
            {
                lblPrzyjetyPrzez.Text = (await _dbServiceBaza.ExecuteScalarAsync("SELECT `Nazwa Wyświetlana` FROM Uzytkownicy WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["PrzyjetyPrzezId"])))?.ToString() ?? "Brak";
            }
            lblUwagiMagazynu.Text = GetUwagiMagazynuValue();
            lblDataPrzyjecia.Text = FormatDateTime(_dbDataRow["DataPrzyjecia"]);

            bool isAllegroReturn = _dbDataRow["AllegroReturnId"] != DBNull.Value && !string.IsNullOrEmpty(_dbDataRow["AllegroReturnId"].ToString());
            btnZwrotWplaty.Enabled = isAllegroReturn;
            btnOdrzucZwrot.Enabled = isAllegroReturn;

            txtKomentarzHandlowca.Text = _dbDataRow["KomentarzHandlowca"]?.ToString();
        }

        private async Task LoadDzialaniaAsync()
        {
            var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT Data, Uzytkownik, Tresc FROM ZwrotDzialania WHERE ZwrotId = @id ORDER BY Data DESC", new MySqlParameter("@id", _returnDbId));
            dgvDzialania.DataSource = dt;
            FormatDzialaniaGrid();
        }

        private void FormatDzialaniaGrid()
        {
            if (dgvDzialania.DataSource == null) return;
            dgvDzialania.Columns["Data"].Width = 150;
            dgvDzialania.Columns["Uzytkownik"].Width = 150;
            dgvDzialania.Columns["Tresc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private async Task AddDzialanieAsync(string tresc)
        {
            if (string.IsNullOrWhiteSpace(tresc)) return;

            string query = "INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc) VALUES (@zwrotId, @data, @uzytkownik, @tresc)";
            var parameters = new[]
            {
                new MySqlParameter("@zwrotId", _returnDbId),
                new MySqlParameter("@data", DateTime.Now),
                new MySqlParameter("@uzytkownik", _fullName),
                new MySqlParameter("@tresc", tresc)
            };
            await _dbServiceMagazyn.ExecuteNonQueryAsync(query, parameters);
            await LoadDzialaniaAsync();
        }

        private async void btnDodajDzialanie_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNoweDzialanie.Text))
            {
                MessageBox.Show("Treść działania nie może być pusta.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            await AddDzialanieAsync(txtNoweDzialanie.Text.Trim());
            txtNoweDzialanie.Clear();
        }

        private async void btnWyslijDecyzje_Click(object sender, EventArgs e)
        {
            if (comboDecyzja.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać decyzję dla magazynu.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedDecision = (StatusItem)comboDecyzja.SelectedItem;
            string komentarz = txtKomentarzHandlowca.Text.Trim();

            var confirm = MessageBox.Show($"Czy na pewno chcesz wysłać decyzję: '{selectedDecision.Nazwa}'?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            try
            {
                using (var con = MagazynDatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        var cmdFindMsg = new MySqlCommand("SELECT Id, NadawcaId FROM Wiadomosci WHERE DotyczyZwrotuId = @zwrotId AND OdbiorcaId = @odbiorcaId ORDER BY Id DESC LIMIT 1", con, transaction);
                        cmdFindMsg.Parameters.AddWithValue("@zwrotId", _returnDbId);
                        cmdFindMsg.Parameters.AddWithValue("@odbiorcaId", SessionManager.CurrentUserId);

                        int? originalMessageId = null;
                        int? originalSenderId = null;
                        using (var reader = await cmdFindMsg.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                originalMessageId = reader.GetInt32(0);
                                originalSenderId = reader.GetInt32(1);
                            }
                        }

                        var statusZakonczonyId = await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Id FROM Statusy WHERE Nazwa = 'Zakończony'");
                        string updateQuery = "UPDATE AllegroCustomerReturns SET DecyzjaHandlowcaId = @decyzjaId, KomentarzHandlowca = @komentarz, StatusWewnetrznyId = @statusId, DataDecyzji = @dataDecyzji WHERE Id = @id";
                        var cmdUpdateReturn = new MySqlCommand(updateQuery, con, transaction);
                        cmdUpdateReturn.Parameters.AddWithValue("@decyzjaId", selectedDecision.Id);
                        cmdUpdateReturn.Parameters.AddWithValue("@komentarz", komentarz);
                        cmdUpdateReturn.Parameters.AddWithValue("@statusId", statusZakonczonyId);
                        cmdUpdateReturn.Parameters.AddWithValue("@dataDecyzji", DateTime.Now);
                        cmdUpdateReturn.Parameters.AddWithValue("@id", _returnDbId);
                        await cmdUpdateReturn.ExecuteNonQueryAsync();

                        string trescDzialania = $"Podjęto decyzję: {selectedDecision.Nazwa}. Komentarz: {komentarz}";
                        var cmdDzialanie = new MySqlCommand("INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc) VALUES (@zwrotId, @data, @uzytkownik, @tresc)", con, transaction);
                        cmdDzialanie.Parameters.AddWithValue("@zwrotId", _returnDbId);
                        cmdDzialanie.Parameters.AddWithValue("@data", DateTime.Now);
                        cmdDzialanie.Parameters.AddWithValue("@uzytkownik", _fullName);
                        cmdDzialanie.Parameters.AddWithValue("@tresc", trescDzialania);
                        await cmdDzialanie.ExecuteNonQueryAsync();

                        if (originalMessageId.HasValue && originalSenderId.HasValue)
                        {
                            var cmdUpdateMsg = new MySqlCommand("UPDATE Wiadomosci SET CzyOdpowiedziano = 1 WHERE Id = @id", con, transaction);
                            cmdUpdateMsg.Parameters.AddWithValue("@id", originalMessageId.Value);
                            await cmdUpdateMsg.ExecuteNonQueryAsync();

                            var cmdReplyMsg = new MySqlCommand(@"INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, ParentMessageId) VALUES (@nadawca, @odbiorca, @tytul, @tresc, @data, @zwrotId, @parentId)", con, transaction);
                            cmdReplyMsg.Parameters.AddWithValue("@nadawca", SessionManager.CurrentUserId);
                            cmdReplyMsg.Parameters.AddWithValue("@odbiorca", originalSenderId.Value);
                            cmdReplyMsg.Parameters.AddWithValue("@tytul", "Re: Prośba o decyzję dla zwrotu nr " + _dbDataRow["ReferenceNumber"]);
                            cmdReplyMsg.Parameters.AddWithValue("@tresc", $"Podjęto decyzję: '{selectedDecision.Nazwa}'. Komentarz: {komentarz}");
                            cmdReplyMsg.Parameters.AddWithValue("@data", DateTime.Now);
                            cmdReplyMsg.Parameters.AddWithValue("@zwrotId", _returnDbId);
                            cmdReplyMsg.Parameters.AddWithValue("@parentId", originalMessageId.Value);
                            await cmdReplyMsg.ExecuteNonQueryAsync();
                        }

                        if (selectedDecision.Nazwa == "Przekaż do reklamacji")
                        {
                            // ===== 1) Złóż OpisUsterki =====
                            // powód klienta: w ręcznych zwykle go nie ma, ale spróbujemy wydobyć z JSON (jeśli jednak jest)
                            string powodKlienta = "";
                            try
                            {
                                if (_dbDataRow.Table.Columns.Contains("JsonDetails") &&
                                    _dbDataRow["JsonDetails"] != DBNull.Value &&
                                    !string.IsNullOrWhiteSpace(_dbDataRow["JsonDetails"]?.ToString()))
                                {
                                    var ret = Newtonsoft.Json.JsonConvert
                                        .DeserializeObject<Reklamacje_Dane.Allegro.Returns.AllegroCustomerReturn>(_dbDataRow["JsonDetails"].ToString());
                                    var item = ret?.Items?.FirstOrDefault();
                                    if (item?.Reason != null)
                                    {
                                        // np. "NOT_AS_DESCRIBED (opis klienta...)"
                                        var typ = string.IsNullOrWhiteSpace(item.Reason.Type) ? "" : item.Reason.Type;
                                        var user = string.IsNullOrWhiteSpace(item.Reason.UserComment) ? "" : item.Reason.UserComment;
                                        powodKlienta = string.IsNullOrWhiteSpace(user) ? typ : $"{typ}: {user}";
                                    }
                                }
                            }
                            catch { /* brak powodu = OK */ }

                            string uwagiMagazynu = GetUwagiMagazynuValue() ?? "";

                            string uwagiHandlowca = komentarz ?? "";

                            var sbOpis = new System.Text.StringBuilder();
                            if (!string.IsNullOrWhiteSpace(powodKlienta))
                                sbOpis.AppendLine($"Powód klienta: {powodKlienta}");
                            if (!string.IsNullOrWhiteSpace(uwagiMagazynu))
                                sbOpis.AppendLine($"Uwagi magazynu: {uwagiMagazynu}");
                            if (!string.IsNullOrWhiteSpace(uwagiHandlowca))
                                sbOpis.AppendLine($"Uwagi handlowca: {uwagiHandlowca}");
                            sbOpis.AppendLine($"Przekazał: {_fullName}");
                            string opisUsterki = sbOpis.ToString().Trim();

                            // ===== 2) Dane klienta / kontakt / adres =====
                            string imie = _dbDataRow["Delivery_FirstName"]?.ToString();
                            string nazwisko = _dbDataRow["Delivery_LastName"]?.ToString();

                            if (string.IsNullOrWhiteSpace(imie) && string.IsNullOrWhiteSpace(nazwisko))
                            {
                                // fallback na Buyer_*
                                imie = _dbDataRow["Buyer_FirstName"]?.ToString();
                                nazwisko = _dbDataRow["Buyer_LastName"]?.ToString();
                            }

                            // nie zawsze mamy email w tabeli zwrotów — podajemy tylko jeśli istnieje kolumna i nie jest pusta
                            string email = _dbDataRow.Table.Columns.Contains("BuyerEmail")
                                ? (_dbDataRow["BuyerEmail"]?.ToString() ?? "")
                                : "";

                            string telefon = _dbDataRow["Delivery_PhoneNumber"]?.ToString();
                            if (string.IsNullOrWhiteSpace(telefon) && _dbDataRow.Table.Columns.Contains("Buyer_PhoneNumber"))
                                telefon = _dbDataRow["Buyer_PhoneNumber"]?.ToString();

                            string ulica = _dbDataRow["Delivery_Street"]?.ToString();
                            string kod = _dbDataRow["Delivery_ZipCode"]?.ToString();
                            string miasto = _dbDataRow["Delivery_City"]?.ToString();

                            if (string.IsNullOrWhiteSpace(ulica) && _dbDataRow.Table.Columns.Contains("Buyer_Street"))
                            {
                                ulica = _dbDataRow["Buyer_Street"]?.ToString();
                                kod = _dbDataRow["Buyer_ZipCode"]?.ToString();
                                miasto = _dbDataRow["Buyer_City"]?.ToString();
                            }

                            string nazwaProduktu = _dbDataRow["ProductName"]?.ToString();
                            string daneProduktu = nazwaProduktu;

                            string nip = "";          // w zwrocie ręcznym zwykle nie mamy NIP — zostawiamy puste
                            DateTime? dataZakupu = null; // brak w danych zwrotu — jeśli nie masz, zostaw null

                            string nrFv = _dbDataRow.Table.Columns.Contains("InvoiceNumber")
                                ? (_dbDataRow["InvoiceNumber"]?.ToString() ?? "")
                                : "";
                            string nrSn = "Brak";

                            // Zbiorcze pole "DaneKlienta" (stare) — zostaje dla zgodności, ale i tak wypełniamy też nowe kolumny Imie/Nazwisko/itd.
                            string daneKlientaZbiorczo =
                                $"{(imie + " " + nazwisko).Trim()} | {ulica}, {kod} {miasto} | tel: {telefon}" +
                                (string.IsNullOrWhiteSpace(email) ? "" : $" | e-mail: {email}");

                            // ===== 3) Insert do Baza.db:NiezarejestrowaneZwrotyReklamacyjne =====
                            using (var conB = DatabaseHelper.GetConnection())
                            {
                                await conB.OpenAsync();
                                using (var cmdIns = new MySqlCommand(@"
            INSERT INTO NiezarejestrowaneZwrotyReklamacyjne
            (
                DataPrzekazania, PrzekazanePrzez, IdZwrotuWMagazynie,
                DaneKlienta, DaneProduktu, NumerFaktury, NumerSeryjny, UwagiMagazynu, KomentarzHandlowca,
                ImieKlienta, NazwiskoKlienta, EmailKlienta, TelefonKlienta,
                AdresUlica, AdresKodPocztowy, AdresMiasto,
                NazwaProduktu, NIP, DataZakupu, OpisUsterki
            )
            VALUES
            (
                @data, @kto, @idZw,
                @daneKlienta, @daneProduktu, @fv, @sn, @uwagiMag, @komHandl,
                @imie, @nazw, @email, @tel,
                @ulica, @kod, @miasto,
                @nazwaProd, @nip, @dataZakupu, @opis
            );", conB))
                                {
                                    cmdIns.Parameters.AddWithValue("@data", DateTime.Now);
                                    cmdIns.Parameters.AddWithValue("@kto", _fullName);
                                    cmdIns.Parameters.AddWithValue("@idZw", _returnDbId);

                                    cmdIns.Parameters.AddWithValue("@daneKlienta", daneKlientaZbiorczo);
                                    cmdIns.Parameters.AddWithValue("@daneProduktu", daneProduktu);
                                    cmdIns.Parameters.AddWithValue("@fv", nrFv);
                                    cmdIns.Parameters.AddWithValue("@sn", nrSn);
                                    cmdIns.Parameters.AddWithValue("@uwagiMag", uwagiMagazynu ?? "");
                                    cmdIns.Parameters.AddWithValue("@komHandl", uwagiHandlowca ?? "");

                                    cmdIns.Parameters.AddWithValue("@imie", imie ?? "");
                                    cmdIns.Parameters.AddWithValue("@nazw", nazwisko ?? "");
                                    cmdIns.Parameters.AddWithValue("@email", email ?? "");
                                    cmdIns.Parameters.AddWithValue("@tel", telefon ?? "");

                                    cmdIns.Parameters.AddWithValue("@ulica", ulica ?? "");
                                    cmdIns.Parameters.AddWithValue("@kod", kod ?? "");
                                    cmdIns.Parameters.AddWithValue("@miasto", miasto ?? "");

                                    cmdIns.Parameters.AddWithValue("@nazwaProd", nazwaProduktu ?? "");
                                    cmdIns.Parameters.AddWithValue("@nip", nip ?? "");
                                    if (dataZakupu.HasValue)
                                        cmdIns.Parameters.AddWithValue("@dataZakupu", dataZakupu.Value);
                                    else
                                        cmdIns.Parameters.AddWithValue("@dataZakupu", DBNull.Value);

                                    cmdIns.Parameters.AddWithValue("@opis", opisUsterki);

                                    await cmdIns.ExecuteNonQueryAsync();
                                }
                            }

                            ToastManager.ShowToast("Przekazano",
                                "Zwrot został przekazany do działu reklamacji (z kompletem danych).",
                                NotificationType.Info);
                        }
                        transaction.Commit();
                    }

                    MessageBox.Show("Decyzja została zapisana.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas zapisu decyzji: " + ex.Message, "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnZarzadzajStatusami_Click(object sender, EventArgs e)
        {
            using (var form = new FormZarzadzajStatusami())
            {
                form.ShowDialog();
                LoadDecyzjeAsync();
            }
        }

        private async void btnOdrzucZwrot_Click(object sender, EventArgs e)
        {
            string customerReturnId = _dbDataRow?["AllegroReturnId"]?.ToString();
            if (string.IsNullOrEmpty(customerReturnId)) return;

            using (var form = new FormOdrzucZwrot())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var con = Database.GetNewOpenConnection())
                        {
                            var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(Convert.ToInt32(_dbDataRow["AllegroAccountId"]), con);
                            await apiClient.RejectCustomerReturnAsync(customerReturnId, form.Result);
                            await AddDzialanieAsync($"[API] Odrzucono zwrot w Allegro. Powód: {form.Result.Rejection.Code}, Uzasadnienie: {form.Result.Rejection.Reason}");
                            ToastManager.ShowToast("Sukces", "Odrzucenie zwrotu zostało wysłane do Allegro.", NotificationType.Success);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Nie udało się odrzucić zwrotu przez API Allegro: " + ex.Message, "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void btnZwrotWplaty_Click(object sender, EventArgs e)
        {
            if (_orderDetails?.Payment?.Id == null)
            {
                MessageBox.Show("Nie można wykonać zwrotu - brak ID płatności w danych zamówienia. Sprawdź, czy dane zamówienia zostały poprawnie załadowane.", "Brak Danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new FormZwrotWplaty(_orderDetails))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var con = Database.GetNewOpenConnection())
                        {
                            var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(Convert.ToInt32(_dbDataRow["AllegroAccountId"]), con);
                            await apiClient.RefundPaymentAsync(form.Result);

                            // ########## POPRAWKA - Naprawiono błąd odwołania do 'Value' ##########
                            var refundAmount = form.Result.LineItems.Sum(li => decimal.Parse(li.Value.Amount.Replace('.', ','))) +
                                               (form.Result.Delivery != null ? decimal.Parse(form.Result.Delivery.Amount.Replace('.', ',')) : 0);

                            await AddDzialanieAsync($"[API] Zlecono zwrot wpłaty w Allegro na kwotę {refundAmount:C}. Komentarz: {form.Result.SellerComment}");
                            ToastManager.ShowToast("Sukces", "Zlecenie zwrotu wpłaty zostało wysłane do Allegro.", NotificationType.Success);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Nie udało się zlecić zwrotu wpłaty przez API Allegro: " + ex.Message, "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private static string FormatDateTime(object value)
        {
            if (value == null || value == DBNull.Value) return "Brak";
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("dd.MM.yyyy HH:mm");
            }

            var stringValue = value.ToString();
            if (DateTime.TryParse(stringValue, CultureInfo.CurrentCulture, DateTimeStyles.None, out var parsed))
            {
                return parsed.ToString("dd.MM.yyyy HH:mm");
            }
            if (DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return parsed.ToString("dd.MM.yyyy HH:mm");
            }

            return "Brak";
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
            return "Brak";
        }
}
}
