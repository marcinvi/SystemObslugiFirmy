// Plik: FormHandlowiecSzczegolyReczny.cs
// Analogia do FormHandlowiecSzczegoly.cs (bez wywołań API Allegro, z „Przekaż do Magazynu”)

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
    public partial class FormHandlowiecSzczegolyReczny : Form
    {
        private readonly int _returnDbId;
        private readonly string _fullName;
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly DatabaseService _dbServiceBaza;
        private DataRow _dbDataRow;

        private class DecisionItem
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
            public string DatabaseName { get; set; }
            public override string ToString() => DisplayName;
        }

        private static readonly string[] AllowedDecisionOrder =
        {
            "Na półkę",
            "Ponowna wysyłka",
            "Reklamacje",
            "Inne"
        };

        public FormHandlowiecSzczegolyReczny(int returnDbId, string fullName)
        {
            InitializeComponent();
            _returnDbId = returnDbId;
            _fullName = string.IsNullOrWhiteSpace(fullName) ? "Handlowiec" : fullName;
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());

            // Jeśli w Designerze masz przycisk „Przekaż do Magazynu”, podłącz tu:
            try
            {
                var btnPrzekazDoMagazynu = this.Controls.Find("btnPrzekazDoMagazynu", true).FirstOrDefault() as Button;
                if (btnPrzekazDoMagazynu != null)
                    btnPrzekazDoMagazynu.Click += async (_, __) => await BtnPrzekazDoMagazynu_Click();
            }
            catch { /* brak przycisku = brak akcji */ }
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormHandlowiecSzczegolyReczny_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                await LoadReturnDataAsync();
                await LoadDecyzjeAsync();
                await PopulateControls();
                await LoadDzialaniaAsync();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async Task LoadReturnDataAsync()
        {
            try
            {
                var dt = await _dbServiceMagazyn.GetDataTableAsync(
                    "SELECT * FROM AllegroCustomerReturns WHERE Id = @id AND IFNULL(IsManual,0)=1",
                    new MySqlParameter("@id", _returnDbId));

                if (dt.Rows.Count == 0)
                    throw new Exception("Nie odnaleziono ręcznego zwrotu o podanym ID.");

                _dbDataRow = dt.Rows[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania szczegółów zwrotu ręcznego: " + ex.Message, "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async Task LoadDecyzjeAsync()
        {
            try
            {
                var dt = await _dbServiceMagazyn.GetDataTableAsync(
                    "SELECT Id, Nazwa FROM Statusy WHERE TypStatusu = 'DecyzjaHandlowca' ORDER BY Nazwa");

                var decyzje = BuildDecisionItems(dt);

                comboDecyzja.DataSource = decyzje;
                comboDecyzja.DisplayMember = "DisplayName";
                comboDecyzja.ValueMember = "Id";
                comboDecyzja.SelectedIndex = decyzje.Count > 0 ? 0 : -1;

                if (!HasAllRequiredDecisions(decyzje))
                {
                    btnWyslijDecyzje.Enabled = false;
                    btnPrzekazDoReklamacji.Enabled = false;
                    MessageBox.Show(
                        "Brakuje wymaganych decyzji (Na półkę, Ponowna wysyłka, Reklamacje, Inne). " +
                        "Uzupełnij je w ustawieniach statusów.",
                        "Brak konfiguracji",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    btnWyslijDecyzje.Enabled = true;
                    btnPrzekazDoReklamacji.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania statusów decyzji: " + ex.Message, "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PopulateControls()
        {
            if (_dbDataRow == null) return;

            lblTitle.Text = $"Decyzja (ręczny) dla zwrotu: {_dbDataRow["ReferenceNumber"]}";
            this.Text = lblTitle.Text;

            lblProductName.Text = _dbDataRow["ProductName"]?.ToString();
            lblBuyerLogin.Text = _dbDataRow["BuyerLogin"]?.ToString(); // dla ręcznych może być puste – to OK
            lblAllegroAccount.Text = "Ręczny";
            lblOrderDate.Text = FormatDateTime(_dbDataRow["CreatedAt"]);
            lblInvoice.Text = _dbDataRow.Table.Columns.Contains("InvoiceNumber")
                                        ? (_dbDataRow["InvoiceNumber"]?.ToString() ?? "Brak")
                                        : "Brak";

            if (_dbDataRow["StanProduktuId"] != DBNull.Value)
            {
                lblStanProduktu.Text = (await _dbServiceMagazyn.ExecuteScalarAsync(
                    "SELECT Nazwa FROM Statusy WHERE Id = @id",
                    new MySqlParameter("@id", _dbDataRow["StanProduktuId"])))?.ToString() ?? "Brak";
            }

            if (_dbDataRow["PrzyjetyPrzezId"] != DBNull.Value)
            {
                lblPrzyjetyPrzez.Text = (await _dbServiceBaza.ExecuteScalarAsync(
                    "SELECT `Nazwa Wyświetlana` FROM Uzytkownicy WHERE Id = @id",
                    new MySqlParameter("@id", _dbDataRow["PrzyjetyPrzezId"])))?.ToString() ?? "Brak";
            }

            lblUwagiMagazynu.Text = GetUwagiMagazynuValue();
            lblDataPrzyjecia.Text = FormatDateTime(_dbDataRow["DataPrzyjecia"]);

            

            txtKomentarzHandlowca.Text = _dbDataRow["KomentarzHandlowca"]?.ToString();
        }

        private async Task LoadDzialaniaAsync()
        {
            var dt = await _dbServiceMagazyn.GetDataTableAsync(
                "SELECT Data, Uzytkownik, Tresc FROM ZwrotDzialania WHERE ZwrotId = @id ORDER BY Data DESC",
                new MySqlParameter("@id", _returnDbId));

            dgvDzialania.DataSource = dt;
            FormatDzialaniaGrid();
        }

        private void FormatDzialaniaGrid()
        {
            if (dgvDzialania.DataSource == null) return;
            if (dgvDzialania.Columns.Contains("Data")) dgvDzialania.Columns["Data"].Width = 150;
            if (dgvDzialania.Columns.Contains("Uzytkownik")) dgvDzialania.Columns["Uzytkownik"].Width = 150;
            if (dgvDzialania.Columns.Contains("Tresc")) dgvDzialania.Columns["Tresc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
                MessageBox.Show("Treść działania nie może być pusta.", "Informacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            await AddDzialanieAsync(txtNoweDzialanie.Text.Trim());
            txtNoweDzialanie.Clear();
        }

        private async void btnWyslijDecyzje_Click(object sender, EventArgs e)
        {
            if (!(comboDecyzja.SelectedItem is DecisionItem selectedDecision))
            {
                MessageBox.Show("Proszę wybrać decyzję dla magazynu.", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateDecisionInput(selectedDecision))
            {
                return;
            }

            var confirm = MessageBox.Show($"Czy na pewno chcesz wysłać decyzję: '{selectedDecision.DisplayName}'?",
                                          "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            await SendDecisionAsync(selectedDecision);
        }

        private async void btnPrzekazDoReklamacji_Click(object sender, EventArgs e)
        {
            var decision = GetDecisionByDisplayName("Reklamacje");
            if (decision == null)
            {
                MessageBox.Show("Brak decyzji 'Reklamacje' w konfiguracji statusów.", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateDecisionInput(decision))
            {
                return;
            }

            var confirm = MessageBox.Show("Czy na pewno chcesz przekazać zwrot do reklamacji?",
                "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            await SendDecisionAsync(decision);
        }

        private bool ValidateDecisionInput(DecisionItem decision)
        {
            if (decision == null || !AllowedDecisionOrder.Contains(decision.DisplayName))
            {
                MessageBox.Show("Wybrana decyzja nie znajduje się na liście dozwolonych.", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (decision.DisplayName == "Inne" && string.IsNullOrWhiteSpace(txtKomentarzHandlowca.Text))
            {
                MessageBox.Show("Dla decyzji 'Inne' wymagany jest komentarz.", "Walidacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private DecisionItem GetDecisionByDisplayName(string displayName)
        {
            if (comboDecyzja.DataSource is IEnumerable<DecisionItem> items)
            {
                return items.FirstOrDefault(item => item.DisplayName == displayName);
            }

            return null;
        }

        private async Task SendDecisionAsync(DecisionItem selectedDecision)
        {
            string komentarz = txtKomentarzHandlowca.Text.Trim();

            try
            {
                using (var con = MagazynDatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        int? originalMessageId = null;
                        int? originalSenderId = null;

                        using (var cmdFindMsg = new MySqlCommand(
                            "SELECT Id, NadawcaId FROM Wiadomosci WHERE DotyczyZwrotuId = @zwrotId AND OdbiorcaId = @odb ORDER BY Id DESC LIMIT 1",
                            con, transaction))
                        {
                            cmdFindMsg.Parameters.AddWithValue("@zwrotId", _returnDbId);
                            cmdFindMsg.Parameters.AddWithValue("@odb", SessionManager.CurrentUserId);
                            using (var r = await cmdFindMsg.ExecuteReaderAsync())
                            {
                                if (await r.ReadAsync())
                                {
                                    originalMessageId = r.GetInt32(0);
                                    originalSenderId = r.GetInt32(1);
                                }
                            }
                        }

                        var statusZakonczonyId = await _dbServiceMagazyn.ExecuteScalarAsync(
                            "SELECT Id FROM Statusy WHERE Nazwa = 'Zakończony'");

                        using (var cmdUpdate = new MySqlCommand(@"
                                UPDATE AllegroCustomerReturns
                                   SET DecyzjaHandlowcaId = @decyzjaId,
                                       KomentarzHandlowca  = @komentarz,
                                       StatusWewnetrznyId  = @statusId,
                                       DataDecyzji         = @dt
                                 WHERE Id = @id", con, transaction))
                        {
                            cmdUpdate.Parameters.AddWithValue("@decyzjaId", selectedDecision.Id);
                            cmdUpdate.Parameters.AddWithValue("@komentarz", komentarz);
                            cmdUpdate.Parameters.AddWithValue("@statusId", statusZakonczonyId);
                            cmdUpdate.Parameters.AddWithValue("@dt", DateTime.Now);
                            cmdUpdate.Parameters.AddWithValue("@id", _returnDbId);
                            await cmdUpdate.ExecuteNonQueryAsync();
                        }

                        string trescDzialania = $"Podjęto decyzję: {selectedDecision.DisplayName}. Komentarz: {komentarz}";
                        using (var cmdDz = new MySqlCommand(
                            "INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc) VALUES (@id, @d, @u, @t)",
                            con, transaction))
                        {
                            cmdDz.Parameters.AddWithValue("@id", _returnDbId);
                            cmdDz.Parameters.AddWithValue("@d", DateTime.Now);
                            cmdDz.Parameters.AddWithValue("@u", _fullName);
                            cmdDz.Parameters.AddWithValue("@t", trescDzialania);
                            await cmdDz.ExecuteNonQueryAsync();
                        }

                        if (originalMessageId.HasValue && originalSenderId.HasValue)
                        {
                            using (var cmdUpdMsg = new MySqlCommand(
                                "UPDATE Wiadomosci SET CzyOdpowiedziano = 1 WHERE Id = @id",
                                con, transaction))
                            {
                                cmdUpdMsg.Parameters.AddWithValue("@id", originalMessageId.Value);
                                await cmdUpdMsg.ExecuteNonQueryAsync();
                            }

                            using (var cmdReply = new MySqlCommand(@"
                                    INSERT INTO Wiadomosci
                                        (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, ParentMessageId)
                                    VALUES
                                        (@nad, @odb, @t, @tr, @dt, @zwId, @pid)",
                                    con, transaction))
                            {
                                cmdReply.Parameters.AddWithValue("@nad", SessionManager.CurrentUserId);
                                cmdReply.Parameters.AddWithValue("@odb", originalSenderId.Value);
                                cmdReply.Parameters.AddWithValue("@t", "Re: Prośba o decyzję dla zwrotu nr " + _dbDataRow["ReferenceNumber"]);
                                cmdReply.Parameters.AddWithValue("@tr", $"Podjęto decyzję: '{selectedDecision.DisplayName}'. Komentarz: {komentarz}");
                                cmdReply.Parameters.AddWithValue("@dt", DateTime.Now);
                                cmdReply.Parameters.AddWithValue("@zwId", _returnDbId);
                                cmdReply.Parameters.AddWithValue("@pid", originalMessageId.Value);
                                await cmdReply.ExecuteNonQueryAsync();
                            }
                        }

                        if (IsReklamacjeDecision(selectedDecision))
                        {
                            await PrzekazDoReklamacjiAsync(komentarz);
                        }

                        transaction.Commit();
                    }
                }

                MessageBox.Show("Decyzja została zapisana.", "Sukces",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas zapisu decyzji: " + ex.Message, "Błąd krytyczny",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PrzekazDoReklamacjiAsync(string komentarz)
        {
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
                        var typ = string.IsNullOrWhiteSpace(item.Reason.Type) ? "" : item.Reason.Type;
                        var user = string.IsNullOrWhiteSpace(item.Reason.UserComment) ? "" : item.Reason.UserComment;
                        powodKlienta = string.IsNullOrWhiteSpace(user) ? typ : $"{typ}: {user}";
                    }
                }
            }
            catch { }

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

            string imie = _dbDataRow["Delivery_FirstName"]?.ToString();
            string nazwisko = _dbDataRow["Delivery_LastName"]?.ToString();

            if (string.IsNullOrWhiteSpace(imie) && string.IsNullOrWhiteSpace(nazwisko))
            {
                imie = _dbDataRow["Buyer_FirstName"]?.ToString();
                nazwisko = _dbDataRow["Buyer_LastName"]?.ToString();
            }

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

            string nip = "";
            DateTime? dataZakupu = null;

            string nrFv = _dbDataRow.Table.Columns.Contains("InvoiceNumber")
                ? (_dbDataRow["InvoiceNumber"]?.ToString() ?? "")
                : "";
            string nrSn = "Brak";

            string daneKlientaZbiorczo =
                $"{(imie + " " + nazwisko).Trim()} | {ulica}, {kod} {miasto} | tel: {telefon}" +
                (string.IsNullOrWhiteSpace(email) ? "" : $" | e-mail: {email}");

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

        private void btnZarzadzajStatusami_Click(object sender, EventArgs e)
        {
            using (var form = new FormZarzadzajStatusami())
            {
                form.ShowDialog();
                // odśwież dostępne decyzje po zamknięciu
                _ = LoadDecyzjeAsync();
            }
        }

        private static List<DecisionItem> BuildDecisionItems(DataTable dt)
        {
            var map = new Dictionary<string, DecisionItem>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in dt.Rows)
            {
                string dbName = row["Nazwa"]?.ToString();
                string canonical = NormalizeDecisionName(dbName);
                if (string.IsNullOrWhiteSpace(canonical))
                {
                    continue;
                }

                if (!map.ContainsKey(canonical))
                {
                    map[canonical] = new DecisionItem
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        DisplayName = canonical,
                        DatabaseName = dbName
                    };
                }
            }

            var ordered = new List<DecisionItem>();
            foreach (var name in AllowedDecisionOrder)
            {
                if (map.TryGetValue(name, out var item))
                {
                    ordered.Add(item);
                }
            }

            return ordered;
        }

        private static bool HasAllRequiredDecisions(IEnumerable<DecisionItem> decisions)
        {
            var available = decisions.Select(d => d.DisplayName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            return AllowedDecisionOrder.All(available.Contains);
        }

        private static string NormalizeDecisionName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "";
            }

            switch (name.Trim())
            {
                case "Na półkę":
                    return "Na półkę";
                case "Ponowna wysyłka":
                case "Ponowna wysylka":
                    return "Ponowna wysyłka";
                case "Reklamacje":
                case "Przekaż do reklamacji":
                case "Przekaz do reklamacji":
                    return "Reklamacje";
                case "Inne":
                    return "Inne";
                default:
                    return "";
            }
        }

        private static bool IsReklamacjeDecision(DecisionItem decision)
        {
            return decision != null && decision.DisplayName == "Reklamacje";
        }

        // ==========================
        // PRZEKAŻ DO MAGAZYNU (manual)
        // ==========================
        private async Task BtnPrzekazDoMagazynu_Click()
        {
            string komentarz = Prompt("Przekaż do Magazynu", "Komentarz dla magazynu (opcjonalnie):");
            if (komentarz == null) return;

            long statusId = await FindStatusMagazynowyAsync();
            if (statusId > 0)
            {
                await _dbServiceMagazyn.ExecuteNonQueryAsync(@"
                    UPDATE AllegroCustomerReturns
                       SET StatusWewnetrznyId = @sid
                     WHERE Id = @id",
                    new MySqlParameter("@sid", statusId),
                    new MySqlParameter("@id", _returnDbId));
            }

            string tresc = string.IsNullOrWhiteSpace(komentarz)
                ? "Przekazano do Magazynu."
                : $"Przekazano do Magazynu. Komentarz: {komentarz}";

            await AddDzialanieAsync(tresc);

            if (statusId == 0)
                MessageBox.Show("Wysłano do Magazynu (status nie został zmieniony — nie znaleziono statusu z frazą 'magaz').",
                                "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Przekazano do Magazynu.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await PopulateControls();
            await LoadDzialaniaAsync();
        }

        private async Task<long> FindStatusMagazynowyAsync()
        {
            // kolejność prób — dostosuj do swoich nazw
            string[] kandydaci = new[]
            {
                "Przekazano do magazynu",
                "Do Magazynu",
                "Oczekuje na działania magazynu",
                "Oczekuje na magazyn",
                "W magazynie"
            };

            foreach (var name in kandydaci)
            {
                var id = await GetStatusIdByExactNameAsync(name);
                if (id > 0) return id;
            }

            // fallback
            var likeId = await GetStatusIdByLikeAsync("%magaz%");
            return likeId;
        }

        private async Task<long> GetStatusIdByExactNameAsync(string nazwa)
        {
            var dt = await _dbServiceMagazyn.GetDataTableAsync(
                "SELECT Id FROM Statusy WHERE Nazwa = @n LIMIT 1",
                new MySqlParameter("@n", nazwa));
            if (dt.Rows.Count == 0) return 0;
            return Convert.ToInt64(dt.Rows[0]["Id"]);
        }

        private async Task<long> GetStatusIdByLikeAsync(string like)
        {
            var dt = await _dbServiceMagazyn.GetDataTableAsync(
                "SELECT Id FROM Statusy WHERE LOWER(Nazwa) LIKE LOWER(@p) LIMIT 1",
                new MySqlParameter("@p", like));
            if (dt.Rows.Count == 0) return 0;
            return Convert.ToInt64(dt.Rows[0]["Id"]);
        }

        // ====== Pomocnicze ======
        private string Prompt(string title, string label)
        {
            using (var f = new Form())
            using (var txt = new TextBox())
            using (var lbl = new Label())
            using (var ok = new Button())
            using (var cancel = new Button())
            {
                f.Text = title;
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MinimizeBox = false; f.MaximizeBox = false;
                f.ClientSize = new System.Drawing.Size(520, 170);

                lbl.Text = label; lbl.AutoSize = true; lbl.Left = 12; lbl.Top = 12;

                txt.Multiline = true; txt.ScrollBars = ScrollBars.Vertical;
                txt.Left = 12; txt.Top = 36; txt.Width = 496; txt.Height = 80;

                ok.Text = "OK"; ok.DialogResult = DialogResult.OK; ok.Width = 100; ok.Height = 28;
                cancel.Text = "Anuluj"; cancel.DialogResult = DialogResult.Cancel; cancel.Width = 100; cancel.Height = 28;

                f.Controls.Add(lbl); f.Controls.Add(txt); f.Controls.Add(ok); f.Controls.Add(cancel);

                f.Shown += (s, e) =>
                {
                    ok.Left = f.ClientSize.Width - ok.Width - 12;
                    cancel.Left = ok.Left - cancel.Width - 8;
                    ok.Top = cancel.Top = f.ClientSize.Height - ok.Height - 12;
                };

                f.AcceptButton = ok; f.CancelButton = cancel;

                var res = f.ShowDialog(this);
                return res == DialogResult.OK ? txt.Text.Trim() : null;
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
