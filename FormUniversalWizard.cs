using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class ComplaintInitialData
{
    public string Id { get; set; }
    public object OriginalObject { get; set; }

    // ### ZMIANA 1: Rozszerzenie klasy o wymagane pola z Allegro ###
    public int AllegroAccountId { get; set; }
    public string allegroDisputeId { get; set; }
    public string allegroOrderId { get; set; }
    public string allegroBuyerLogin { get; set; }

    // Dane klienta
    public string ImieNazwisko { get; set; }
    public string NazwaFirmy { get; set; }
    public bool IsCompany => !string.IsNullOrWhiteSpace(NazwaFirmy);
    public string NIP { get; set; }
    public string Ulica { get; set; }
    public string KodPocztowy { get; set; }
    public string Miejscowosc { get; set; }
    public string Email { get; set; }
    public string Telefon { get; set; }

    // Dane zgłoszenia
    public string NazwaProduktu { get; set; }
    public string OpisUsterki { get; set; }
    public string NumerFaktury { get; set; }
    public string NumerSeryjny { get; set; }
    public DateTime? DataZakupu { get; set; }
    public string SourceName { get; set; }
}

public class ZgloszenieZArkusza
{
    public IList<object> RowData { get; set; }
    public string SourceSheet { get; set; }
    public int RowIndex { get; set; }
}

namespace Reklamacje_Dane
{
    public partial class FormUniversalWizard : Form
    {
        private readonly WizardSource _source;
        private ComplaintInitialData _initialData;
        private Timer _searchDebounceTimer;

        private ClientViewModel _selectedClientVM = null;
        private int? _selectedProductId = null;
        private bool _isCompany = false;
        private bool _isInitializing = false;
        private readonly DatabaseService _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());

        public FormUniversalWizard(WizardSource source)
        {
            _source = source;
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormUniversalWizard_Load(object sender, EventArgs e)
        {
            _isInitializing = true;
            SetupUI();

            if (_source == WizardSource.Manual)
            {
                pnlInitialSelection.Visible = false;
                tabControlWizard.Visible = true;
                pnlStepper.Visible = true;
                this.Text = "Nowe Zgłoszenie - Kreator Ręczny";
                chkZapiszJakoNowy.Checked = true;
                gbManualSource.Visible = true;
                _initialData = new ComplaintInitialData();
            }
            else
            {
                pnlInitialSelection.Visible = true;
                tabControlWizard.Visible = false;
                pnlStepper.Visible = false;
                await LoadInitialSelectionListAsync();
            }

            _isInitializing = false;
            UpdateStepUI();
        }

        #region Initialization and UI Setup

        private void SetupUI()
        {
            this.Font = new Font("Segoe UI", 9F);
            _searchDebounceTimer = new Timer { Interval = 300 };
            _searchDebounceTimer.Tick += async (s, ev) =>
            {
                _searchDebounceTimer.Stop();
                if (tabControlWizard.SelectedTab == tabPageKlient) await SearchClientsAsync();
                else if (tabControlWizard.SelectedTab == tabPageProdukt) await SearchProductsAsync();
            };

            tabControlWizard.Appearance = TabAppearance.FlatButtons;
            tabControlWizard.ItemSize = new Size(0, 1);
            tabControlWizard.SizeMode = TabSizeMode.Fixed;

            dataGridViewProdukty.AutoGenerateColumns = false;
            dataGridViewProdukty.Columns.Clear();

            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nazwa", HeaderText = "Nazwa Produktu", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, FillWeight = 60, MinimumWidth = 350 });
            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Producent", HeaderText = "Producent", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, FillWeight = 20 });
            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KodEnova", HeaderText = "Kod Enova", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, FillWeight = 10 });
            dataGridViewProdukty.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KodProducenta", HeaderText = "Kod Producenta", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, FillWeight = 10 });
        }

        private async Task LoadInitialSelectionListAsync()
        {
            listViewInitialSelection.Items.Clear();
            listViewInitialSelection.Enabled = false;

            try
            {
                if (_source == WizardSource.Allegro)
                {
                    this.Text = "Nowe Zgłoszenie z Allegro - Kreator";
                    lblInitialSelection.Text = "Wybierz dyskusję do przetworzenia (podwójne kliknięcie):";
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        var cmd = new MySqlCommand("SELECT DisputeId, BuyerLogin, Subject, ProductName FROM AllegroDisputes WHERE ComplaintId IS NULL  ORDER BY OpenedAt DESC", con);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new ListViewItem(reader["DisputeId"].ToString());
                                item.SubItems.Add(reader["BuyerLogin"].ToString());
                                item.SubItems.Add(reader["Subject"].ToString() ?? reader["ProductName"].ToString());
                                item.Tag = reader["DisputeId"].ToString();
                                listViewInitialSelection.Items.Add(item);
                            }
                        }
                    }
                }
                else if (_source == WizardSource.GoogleSheet)
                {
                    this.Text = "Nowe Zgłoszenie z Arkusza - Kreator";
                    lblInitialSelection.Text = "Wybierz zgłoszenie z arkusza do przetworzenia (podwójne kliknięcie):";
                    string[] sheetsToRead = { "B", "Z" };
                    string credentialsPath = "reklamacje-baza-c36d05b0ffdb.json";
                    string spreadsheetId = "1VXGP4Cckt6NmSHtiv-Um7nqg-itLMczAGd-5a_Tc4Ds";
                    foreach (var sheetName in sheetsToRead)
                    {
                        var values = await GoogleSheetsDataService.GetSheetValuesAsync(
                            credentialsPath,
                            spreadsheetId,
                            $"{sheetName}!A:P");
                        if (values != null && values.Count > 0)
                        {
                            for (int i = 1; i < values.Count; i++)
                            {
                                var row = values[i];
                                if (row.Cast<object>().Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString())))
                                {
                                    var item = new ListViewItem(GetValueFromRow(row, 0));
                                    item.SubItems.Add(GetValueFromRow(row, 2));
                                    item.SubItems.Add(GetValueFromRow(row, 12));
                                    item.Tag = new ZgloszenieZArkusza { RowData = row, SourceSheet = sheetName, RowIndex = i + 1 };
                                    listViewInitialSelection.Items.Add(item);
                                }
                            }
                        }
                    }
                }
                else if (_source == WizardSource.Zwroty)
                {
                    this.Text = "Nowe Zgłoszenie ze Zwrotu - Kreator";
                    lblInitialSelection.Text = "Wybierz zwrot do przetworzenia (podwójne kliknięcie):";
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        using (var cmd = new MySqlCommand(@"
                            SELECT Id,
                                   IFNULL(DaneKlienta,'')     AS DaneKlienta,
                                   IFNULL(DaneProduktu,'')    AS DaneProduktu,
                                   IFNULL(NumerFaktury,'')    AS NumerFaktury,
                                   IFNULL(PrzekazanePrzez,'') AS PrzekazanePrzez,
                                   IFNULL(DataPrzekazania,'') AS DataPrzekazania
                            FROM NiezarejestrowaneZwrotyReklamacyjne
                            WHERE IFNULL(CzyZarejestrowane,0)=0
                            ORDER BY STR_TO_DATE(NULLIF(DataPrzekazania,''), '%Y-%m-%d %H:%i:%s') DESC, Id DESC", con))
                        using (var rd = await cmd.ExecuteReaderAsync())
                        {
                            while (await rd.ReadAsync())
                            {
                                var item = new ListViewItem(Convert.ToString(rd["Id"]));
                                item.SubItems.Add(Convert.ToString(rd["DaneKlienta"]));
                                item.SubItems.Add(Convert.ToString(rd["DaneProduktu"]));
                                item.Tag = rd["Id"]; // klucz rekordu
                                listViewInitialSelection.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił krytyczny błąd podczas wczytywania danych źródłowych: {ex.Message}", "Błąd Krytyczny");
                this.Close();
            }
            finally
            {
                listViewInitialSelection.Enabled = true;
            }
        }

        private async Task<ComplaintInitialData> ConvertToInitialData(object sourceData)
        {
            var data = new ComplaintInitialData { OriginalObject = sourceData };

            // ===== Allegro =====
            if (_source == WizardSource.Allegro && sourceData is string disputeId)
            {
                data.Id = disputeId;
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand(
                        "SELECT ad.*, aa.AccountName FROM AllegroDisputes ad JOIN AllegroAccounts aa ON ad.AllegroAccountId = aa.Id WHERE ad.DisputeId = @id",
                        con);
                    cmd.Parameters.AddWithValue("@id", disputeId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            data.AllegroAccountId = Convert.ToInt32(reader["AllegroAccountId"]);
                            data.allegroDisputeId = reader["DisputeId"].ToString();
                            data.allegroOrderId = reader["OrderId"].ToString();
                            data.allegroBuyerLogin = reader["BuyerLogin"].ToString();

                            data.ImieNazwisko = $"{reader["BuyerFirstName"]} {reader["BuyerLastName"]}".Trim();
                            if (string.IsNullOrWhiteSpace(data.ImieNazwisko))
                                data.ImieNazwisko = reader["BuyerLogin"].ToString();

                            data.NazwaFirmy = reader["DeliveryCompanyName"].ToString();
                            data.Ulica = reader["DeliveryStreet"].ToString();
                            data.KodPocztowy = reader["DeliveryZipCode"].ToString();
                            data.Miejscowosc = reader["DeliveryCity"].ToString();
                            data.Email = reader["BuyerEmail"].ToString();
                            data.Telefon = reader["DeliveryPhoneNumber"].ToString();

                            data.NazwaProduktu = reader["ProductName"].ToString();
                            data.OpisUsterki = reader["InitialMessageText"].ToString();

                            if (DateTime.TryParse(reader["BoughtAt"]?.ToString(), out var boughtAt))
                                data.DataZakupu = boughtAt;

                            data.SourceName = $"Allegro - {reader["AccountName"]}";
                        }
                    }
                }
                return data;
            }

            // ===== Google Sheet =====
            if (_source == WizardSource.GoogleSheet && sourceData is ZgloszenieZArkusza sheetRow)
            {
                data.Id = sheetRow.RowIndex.ToString();
                var row = sheetRow.RowData;

                data.ImieNazwisko = GetValueFromRow(row, 2);
                data.NazwaFirmy = GetValueFromRow(row, 1);
                data.Ulica = GetValueFromRow(row, 3);
                data.KodPocztowy = GetValueFromRow(row, 4);
                data.Miejscowosc = GetValueFromRow(row, 5);
                data.Telefon = GetValueFromRow(row, 6);
                data.Email = GetValueFromRow(row, 7);
                data.NIP = GetValueFromRow(row, 13);

                data.NazwaProduktu = GetValueFromRow(row, 10);
                data.OpisUsterki = GetValueFromRow(row, 12);
                data.NumerFaktury = GetValueFromRow(row, 8);
                data.NumerSeryjny = GetValueFromRow(row, 11);

                if (DateTime.TryParse(GetValueFromRow(row, 9), out var dataZakupu))
                    data.DataZakupu = dataZakupu;

                data.SourceName = (sheetRow.SourceSheet == "B") ? "Truck-Shop" : "Ena-Truck";
                return data;
            }

            // ===== Zwroty (Baza.db → NiezarejestrowaneZwrotyReklamacyjne) =====
            if (_source == WizardSource.Zwroty && (sourceData is long || (sourceData is string && long.TryParse((string)sourceData, out _))))
            {
                long niezId = sourceData is long l ? l : Convert.ToInt64((string)sourceData);
                data.Id = niezId.ToString();

                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new MySqlCommand(@"
                SELECT
                    Id,
                    IFNULL(DaneKlienta,'')        AS DaneKlienta,
                    IFNULL(DaneProduktu,'')       AS DaneProduktu,
                    IFNULL(NumerFaktury,'')       AS NumerFaktury,
                    IFNULL(NumerSeryjny,'')       AS NumerSeryjny,
                    IFNULL(ImieKlienta,'')        AS ImieKlienta,
                    IFNULL(NazwiskoKlienta,'')    AS NazwiskoKlienta,
                    IFNULL(EmailKlienta,'')       AS EmailKlienta,
                    IFNULL(TelefonKlienta,'')     AS TelefonKlienta,
                    IFNULL(AdresUlica,'')         AS AdresUlica,
                    IFNULL(AdresKodPocztowy,'')   AS AdresKodPocztowy,
                    IFNULL(AdresMiasto,'')        AS AdresMiasto,
                    IFNULL(NazwaProduktu,'')      AS NazwaProduktu,
                    IFNULL(NIP,'')                AS NIP,
                    DataZakupu,
                    IFNULL(OpisUsterki,'')        AS OpisUsterki,
                    IFNULL(UwagiMagazynu,'')      AS UwagiMagazynu,
                    IFNULL(KomentarzHandlowca,'') AS KomentarzHandlowca,
                    IFNULL(PrzekazanePrzez,'')    AS PrzekazanePrzez
                FROM NiezarejestrowaneZwrotyReklamacyjne
                WHERE Id=@id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", niezId);
                        using (var rd = await cmd.ExecuteReaderAsync())
                        {
                            if (await rd.ReadAsync())
                            {
                                // Dane klienta/adres
                                var imie = Convert.ToString(rd["ImieKlienta"]);
                                var nazw = Convert.ToString(rd["NazwiskoKlienta"]);
                                data.ImieNazwisko = $"{imie} {nazw}".Trim();

                                data.Email = Convert.ToString(rd["EmailKlienta"]);
                                data.Telefon = Convert.ToString(rd["TelefonKlienta"]);
                                data.Ulica = Convert.ToString(rd["AdresUlica"]);
                                data.KodPocztowy = Convert.ToString(rd["AdresKodPocztowy"]);
                                data.Miejscowosc = Convert.ToString(rd["AdresMiasto"]);
                                data.NIP = Convert.ToString(rd["NIP"]);

                                // Produkt/FV/SN/Data
                                var nazwaProduktuNowa = Convert.ToString(rd["NazwaProduktu"]);
                                var daneProduktuStare = Convert.ToString(rd["DaneProduktu"]);
                                data.NazwaProduktu = string.IsNullOrWhiteSpace(nazwaProduktuNowa) ? daneProduktuStare : nazwaProduktuNowa;
                                data.NumerFaktury = Convert.ToString(rd["NumerFaktury"]);
                                data.NumerSeryjny = Convert.ToString(rd["NumerSeryjny"]);
                                if (DateTime.TryParse(Convert.ToString(rd["DataZakupu"]), out var dz))
                                    data.DataZakupu = dz;

                                // Opis usterki (gotowy lub złożony z uwag)
                                var opisZBazy = Convert.ToString(rd["OpisUsterki"]);
                                if (!string.IsNullOrWhiteSpace(opisZBazy))
                                {
                                    data.OpisUsterki = opisZBazy;
                                }
                                else
                                {
                                    var uwagiMag = Convert.ToString(rd["UwagiMagazynu"]);
                                    var uwagiHand = Convert.ToString(rd["KomentarzHandlowca"]);
                                    var przekazal = Convert.ToString(rd["PrzekazanePrzez"]);
                                    var sb = new System.Text.StringBuilder();
                                    if (!string.IsNullOrWhiteSpace(uwagiMag)) sb.AppendLine($"Uwagi magazynu: {uwagiMag}");
                                    if (!string.IsNullOrWhiteSpace(uwagiHand)) sb.AppendLine($"Uwagi handlowca: {uwagiHand}");
                                    if (!string.IsNullOrWhiteSpace(przekazal)) sb.AppendLine($"Przekazał: {przekazal}");
                                    data.OpisUsterki = sb.ToString().Trim();
                                }

                                // Fallback: parsowanie starego pola „DaneKlienta”, jeśli czegoś brakuje
                                if (string.IsNullOrWhiteSpace(data.ImieNazwisko) || string.IsNullOrWhiteSpace(data.Ulica))
                                {
                                    var daneKlientaStare = Convert.ToString(rd["DaneKlienta"]);
                                    if (!string.IsNullOrWhiteSpace(daneKlientaStare))
                                    {
                                        var parts = daneKlientaStare.Split(new[] { " | " }, StringSplitOptions.None);
                                        if (string.IsNullOrWhiteSpace(data.ImieNazwisko) && parts.Length >= 1)
                                            data.ImieNazwisko = parts[0];

                                        if (string.IsNullOrWhiteSpace(data.Ulica) && parts.Length >= 2)
                                        {
                                            var addr = parts[1]; // "Ulica, 00-000 Miasto"
                                            var idx = addr.LastIndexOf(',');
                                            if (idx > -1)
                                            {
                                                data.Ulica = addr.Substring(0, idx).Trim();
                                                var rest = addr.Substring(idx + 1).Trim();
                                                var sp = rest.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                                                if (sp.Length == 2) { data.KodPocztowy = sp[0]; data.Miejscowosc = sp[1]; }
                                            }
                                            else { data.Ulica = addr; }
                                        }

                                        if (string.IsNullOrWhiteSpace(data.Telefon))
                                        {
                                            var telPart = parts.FirstOrDefault(p => p.StartsWith("tel:", StringComparison.OrdinalIgnoreCase));
                                            if (telPart != null) data.Telefon = telPart.Substring(4).Trim();
                                        }
                                        if (string.IsNullOrWhiteSpace(data.Email))
                                        {
                                            var mailPart = parts.FirstOrDefault(p => p.IndexOf("e-mail:", StringComparison.OrdinalIgnoreCase) >= 0);
                                            if (mailPart != null) data.Email = mailPart.Split(new[] { ':' }, 2)[1].Trim();
                                        }
                                    }
                                }

                                data.SourceName = "Allegro — Zwrot";
                            }
                        }
                    }
                }

                return data;
            }

            // Fallback
            data.SourceName = "Nieznane";
            return data;
        }


        private void PopulateFormWithInitialData()
        {
            txtImieNazwisko.Text = _initialData.ImieNazwisko;
            txtNazwaFirmy.Text = _initialData.NazwaFirmy;
            txtNIP.Text = _initialData.NIP;
            txtUlica.Text = _initialData.Ulica;
            txtKodPocztowy.Text = _initialData.KodPocztowy;
            txtMiejscowosc.Text = _initialData.Miejscowosc;
            txtMail.Text = _initialData.Email;
            txtTelefon.Text = _initialData.Telefon;
            txtSzukajProduktu.Text = _initialData.NazwaProduktu;
            txtOpisUsterki.Text = _initialData.OpisUsterki;
            txtNumerFaktury.Text = _initialData.NumerFaktury;
            txtnrSeryjny.Text = _initialData.NumerSeryjny;
            chkFirma.Checked = _initialData.IsCompany;
            if (_initialData.DataZakupu.HasValue && _initialData.DataZakupu.Value >= dtpDataZakupu.MinDate && _initialData.DataZakupu.Value <= dtpDataZakupu.MaxDate)
            {
                dtpDataZakupu.Value = _initialData.DataZakupu.Value;
            }
        }

        private string GetValueFromRow(IList<object> row, int index)
        {
            return row.Count > index && row[index] != null ? row[index].ToString() : "";
        }

        #endregion

        #region Wizard Navigation & UI Logic

        private async void btnDalej_Click(object sender, EventArgs e)
        {
            if (tabControlWizard.SelectedTab == tabPageKlient)
            {
                if (_selectedClientVM == null && !chkZapiszJakoNowy.Checked)
                {
                    MessageBox.Show("Proszę wybrać klienta z listy lub zaznaczyć opcję utworzenia nowego.", "Błąd Walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (tabControlWizard.SelectedTab == tabPageProdukt && _selectedProductId == null)
            {
                MessageBox.Show("Proszę wybrać produkt z listy.", "Błąd Walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (tabControlWizard.SelectedIndex < tabControlWizard.TabCount - 1)
            {
                tabControlWizard.SelectedIndex++;
                if (tabControlWizard.SelectedTab == tabPageProdukt) await SearchProductsAsync();
            }
            else
            {
                await SaveComplaintAsync();
            }
        }

        private void btnWstecz_Click(object sender, EventArgs e)
        {
            if (tabControlWizard.SelectedIndex > 0)
            {
                tabControlWizard.SelectedIndex--;
            }
            else
            {
                if (_source != WizardSource.Manual)
                {
                    pnlInitialSelection.Visible = true;
                    tabControlWizard.Visible = false;
                    pnlStepper.Visible = false;
                }
                else
                {
                    this.Close();
                }
            }
        }

        private async void tabControlWizard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isInitializing)
            {
                UpdateStepUI();
                if (tabControlWizard.SelectedTab == tabPageUsterka)
                {
                    await UpdateSummaryAndHistoryAsync();
                }
            }
        }

        private void UpdateStepUI()
        {
            var regularFont = new Font("Segoe UI", 10.2F, FontStyle.Regular);
            var boldFont = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold);
            var activeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            var inactiveColor = System.Drawing.Color.Gray;

            lblStep1.Font = regularFont; lblStep1.ForeColor = inactiveColor;
            lblStep2.Font = regularFont; lblStep2.ForeColor = inactiveColor;
            lblStep3.Font = regularFont; lblStep3.ForeColor = inactiveColor;

            int currentIndex = tabControlWizard.SelectedIndex;
            if (currentIndex == 0) { lblStep1.Font = boldFont; lblStep1.ForeColor = activeColor; }
            else if (currentIndex == 1) { lblStep2.Font = boldFont; lblStep2.ForeColor = activeColor; }
            else if (currentIndex == 2) { lblStep3.Font = boldFont; lblStep3.ForeColor = activeColor; }

            btnWstecz.Enabled = true;
            btnWstecz.Text = (currentIndex == 0 && _source == WizardSource.Manual) ? "Anuluj" : "< Wstecz";
            btnDalej.Text = (currentIndex == tabControlWizard.TabCount - 1) ? "Zapisz Zgłoszenie" : "Dalej >";
        }
        #endregion

        #region Search and Data Handling
        private void Input_TextChanged_Debounced(object sender, EventArgs e)
        {
            if (_isInitializing) return;
            _selectedClientVM = null;
            pnlConflictResolution.Visible = false;
            lblClientHistoryInfo.Visible = false;
            chkZapiszJakoNowy.Checked = true;
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private async Task SearchClientsAsync()
        {
            if (_isInitializing) return;
            string searchText = $"{txtSzukajKlienta.Text} {txtImieNazwisko.Text} {txtNazwaFirmy.Text} {txtMail.Text}".Trim();
            if (searchText.Length < 2)
            {
                dataGridViewPropozycje.DataSource = null;
                return;
            }
            var results = await SearchAndSuggestionService.SearchClientsAsync(searchText, _initialData);
            dataGridViewPropozycje.DataSource = results;
        }

        private async Task SearchProductsAsync()
        {
            if (_isInitializing) return;
            if (txtSzukajProduktu.Text.Length < 2)
            {
                dataGridViewProdukty.DataSource = null;
                return;
            }
            var results = await SearchAndSuggestionService.SearchProductsAsync(txtSzukajProduktu.Text);
            dataGridViewProdukty.DataSource = results;
        }

        private void dataGridViewPropozycje_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var grid = sender as DataGridView;
            var clientVM = grid.Rows[e.RowIndex].DataBoundItem as ClientViewModel;
            if (_initialData == null || clientVM == null)
            {
                e.CellStyle.BackColor = grid.DefaultCellStyle.BackColor;
                foreach (DataGridViewCell cell in grid.Rows[e.RowIndex].Cells) { cell.ToolTipText = ""; }
                return;
            }
            bool hasMismatch = !clientVM.Email.Equals(_initialData.Email, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(_initialData.Email)
                            || !clientVM.Telefon.Equals(_initialData.Telefon, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(_initialData.Telefon);
            if (hasMismatch)
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightYellow;
                foreach (DataGridViewCell cell in grid.Rows[e.RowIndex].Cells) { cell.ToolTipText = "Wykryto różnice w danych kontaktowych!"; }
            }
            else
            {
                e.CellStyle.BackColor = grid.DefaultCellStyle.BackColor;
                foreach (DataGridViewCell cell in grid.Rows[e.RowIndex].Cells) { cell.ToolTipText = ""; }
            }
        }

        private async void dataGridViewPropozycje_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var clientVM = dataGridViewPropozycje.Rows[e.RowIndex].DataBoundItem as ClientViewModel;
            if (clientVM == null) return;
            _selectedClientVM = clientVM;
            chkZapiszJakoNowy.Checked = false;
            pnlConflictResolution.Visible = (dataGridViewPropozycje.Rows[e.RowIndex].DefaultCellStyle.BackColor == System.Drawing.Color.LightYellow);
            int count = await SearchAndSuggestionService.GetClientComplaintCountAsync(clientVM.Id);
            if (count > 0)
            {
                lblClientHistoryInfo.Text = $"Klient ma już {count} inne/ych zgłoszeń.";
                lblClientHistoryInfo.Visible = true;
            }
            else
            {
                lblClientHistoryInfo.Visible = false;
            }
        }

        private void dataGridViewPropozycje_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var clientVM = dataGridViewPropozycje.Rows[e.RowIndex].DataBoundItem as ClientViewModel;
            if (clientVM == null) return;
            _selectedClientVM = clientVM;
            chkZapiszJakoNowy.Checked = false;
            FillClientDataFromViewModel(clientVM);
            btnDalej_Click(sender, e);
        }

        private void dataGridViewProdukty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var productVM = dataGridViewProdukty.Rows[e.RowIndex].DataBoundItem as ProductViewModel;
            if (productVM == null) return;
            _selectedProductId = productVM.Id;
        }

        private void dataGridViewProdukty_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var productVM = dataGridViewProdukty.Rows[e.RowIndex].DataBoundItem as ProductViewModel;
            if (productVM == null) return;
            _selectedProductId = productVM.Id;
            btnDalej_Click(sender, e);
        }

        private async void listViewInitialSelection_DoubleClick(object sender, EventArgs e)
        {
            if (listViewInitialSelection.SelectedItems.Count == 0) return;
            var selectedTag = listViewInitialSelection.SelectedItems[0].Tag;
            _initialData = await ConvertToInitialData(selectedTag);
            _isInitializing = true;
            PopulateFormWithInitialData();
            _isInitializing = false;
            pnlInitialSelection.Visible = false;
            tabControlWizard.Visible = true;
            pnlStepper.Visible = true;
            UpdateStepUI();
            await SearchClientsAsync();
        }

        private void FillClientDataFromViewModel(ClientViewModel clientVM)
        {
            _isInitializing = true;
            txtImieNazwisko.Text = clientVM.ImieNazwisko;
            txtNazwaFirmy.Text = clientVM.NazwaFirmy;
            txtNIP.Text = clientVM.NIP;
            txtUlica.Text = clientVM.Ulica;
            txtKodPocztowy.Text = clientVM.KodPocztowy;
            txtMiejscowosc.Text = clientVM.Miejscowosc;
            txtMail.Text = clientVM.Email;
            txtTelefon.Text = clientVM.Telefon;
            _isInitializing = false;
        }

        private void btnUseDbData_Click(object sender, EventArgs e)
        {
            if (_selectedClientVM == null) return;
            FillClientDataFromViewModel(_selectedClientVM);
            pnlConflictResolution.Visible = false;
        }

        private async void btnUpdateClientData_Click(object sender, EventArgs e)
        {
            if (_selectedClientVM == null) return;
            var result = MessageBox.Show("Czy na pewno chcesz nadpisać dane tego klienta w bazie danych informacjami wczytanymi ze źródła?", "Potwierdź aktualizację", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "UPDATE Klienci SET ImieNazwisko = @imie, NazwaFirmy = @firma, NIP = @nip, Ulica = @ulica, KodPocztowy = @kod, Miejscowosc = @miasto, Email = @mail, Telefon = @tel WHERE Id = @id";
                    var parameters = new[] {
                        new MySqlParameter("@imie", txtImieNazwisko.Text), new MySqlParameter("@firma", txtNazwaFirmy.Text),
                        new MySqlParameter("@nip", txtNIP.Text), new MySqlParameter("@ulica", txtUlica.Text),
                        new MySqlParameter("@kod", txtKodPocztowy.Text), new MySqlParameter("@miasto", txtMiejscowosc.Text),
                        new MySqlParameter("@mail", txtMail.Text), new MySqlParameter("@tel", txtTelefon.Text),
                        new MySqlParameter("@id", _selectedClientVM.Id)
                    };
                    await _dbService.ExecuteNonQueryAsync(query, parameters);
                    pnlConflictResolution.Visible = false;
                    MessageBox.Show("Dane klienta zostały zaktualizowane.", "Sukces");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie udało się zaktualizować danych klienta: {ex.Message}", "Błąd Bazy Danych");
                }
            }
        }
        #endregion

        #region Validation and Saving
        private async void txtnrSeryjny_Leave(object sender, EventArgs e)
        {
            lblSerialInfo.Visible = false;
            if (!string.IsNullOrWhiteSpace(txtnrSeryjny.Text))
            {
                var (exists, num) = await SearchAndSuggestionService.CheckSerialNumberExistsAsync(txtnrSeryjny.Text);
                if (exists)
                {
                    lblSerialInfo.Text = $"UWAGA: Ten numer istnieje w zgłoszeniu {num}!";
                    lblSerialInfo.Visible = true;
                }
            }
        }

        private void txtNumerFaktury_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumerFaktury.Text) && tabControlWizard.SelectedTab == tabPageUsterka)
            {
                string clientIdentifier = $"*{(string.IsNullOrWhiteSpace(txtImieNazwisko.Text) ? txtNazwaFirmy.Text : txtImieNazwisko.Text)}*";
                Clipboard.SetText(clientIdentifier);
                lblFakturaInfo.ForeColor = System.Drawing.Color.SteelBlue;
                lblFakturaInfo.Text = "Wskazówka: Dane klienta skopiowano do schowka, aby ułatwić szukanie FV w Enovie.";
                lblFakturaInfo.Visible = true;
            }
            CheckInvoice();
        }

        private void txtNumerFaktury_TextChanged(object sender, EventArgs e)
        {
            if (_source == WizardSource.Manual && !_isInitializing)
            {
                string fv = txtNumerFaktury.Text.ToUpper();
                if (fv.StartsWith("FV")) { rbTruckShop.Checked = true; }
                else if (fv.EndsWith("FV") || fv.EndsWith("FVW")) { rbEnaTruck.Checked = true; }
            }
        }

        private void dtpDataZakupu_ValueChanged(object sender, EventArgs e)
        {
            CheckInvoice();
            SprawdzGwarancje();
        }

        private async void CheckInvoice()
        {
            if (string.IsNullOrWhiteSpace(txtNumerFaktury.Text)) { lblFakturaInfo.Visible = false; }
            else
            {
                lblFakturaInfo.ForeColor = System.Drawing.Color.Red;
                var (exists, num) = await SearchAndSuggestionService.CheckInvoiceNumberExistsAsync(txtNumerFaktury.Text);
                if (exists)
                {
                    lblFakturaInfo.Text = $"UWAGA: Ta faktura istnieje już w zgłoszeniu {num}!";
                    lblFakturaInfo.Visible = true;
                }
                else { lblFakturaInfo.Visible = false; }
            }
            lblDataFakturyInfo.Visible = false;
            if (!SearchAndSuggestionService.CheckInvoiceDateConsistency(txtNumerFaktury.Text, dtpDataZakupu.Value))
            {
                lblDataFakturyInfo.Text = "Czy data zakupu jest poprawna? Niespójność z nr faktury.";
                lblDataFakturyInfo.Visible = true;
            }
        }

        private void SprawdzGwarancje()
        {
            DateTime dataZakupu = dtpDataZakupu.Value;
            int miesiaceGwarancji = _isCompany ? 12 : 24;
            DateTime dataKoncaGwarancji = dataZakupu.AddMonths(miesiaceGwarancji);
            if (DateTime.Now > dataKoncaGwarancji)
            {
                lblGwarancjaStatus.Text = $"Gwarancja zakończona ({dataKoncaGwarancji:dd.MM.yyyy})";
                lblGwarancjaStatus.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblGwarancjaStatus.Text = $"Gwarancja ważna do {dataKoncaGwarancji:dd.MM.yyyy}";
                lblGwarancjaStatus.ForeColor = System.Drawing.Color.ForestGreen;
            }
            lblGwarancjaStatus.Visible = true;
        }

        private void chkFirma_CheckedChanged(object sender, EventArgs e)
        {
            _isCompany = chkFirma.Checked;
            SprawdzGwarancje();
        }

        private void chkZapiszJakoNowy_CheckedChanged(object sender, EventArgs e)
        {
            if (chkZapiszJakoNowy.Checked)
            {
                _selectedClientVM = null;
                pnlConflictResolution.Visible = false;
                dataGridViewPropozycje.ClearSelection();
            }
        }

        private async Task UpdateSummaryAndHistoryAsync()
        {
            string clientSummary = "Klient: ";
            if (chkZapiszJakoNowy.Checked)
            {
                clientSummary += string.IsNullOrWhiteSpace(txtNazwaFirmy.Text) ? txtImieNazwisko.Text : $"{txtNazwaFirmy.Text} ({txtImieNazwisko.Text})";
                clientSummary += " (Nowy)";
            }
            else if (_selectedClientVM != null) { clientSummary += _selectedClientVM.DaneKlienta; }
            else { clientSummary += "Nie wybrano"; }
            lblSummaryClient.Text = clientSummary;

            string productSummary = "Produkt: ";
            if (_selectedProductId.HasValue && dataGridViewProdukty.DataSource is List<ProductViewModel> products)
            {
                var product = products.FirstOrDefault(p => p.Id == _selectedProductId.Value);
                if (product != null) { productSummary += product.Nazwa; }
            }
            else { productSummary += "Nie wybrano"; }
            lblSummaryProduct.Text = productSummary;

            lblExistingComplaintInfo.Visible = false;
            int clientId = chkZapiszJakoNowy.Checked ? -1 : (_selectedClientVM?.Id ?? -1);
            if (clientId > 0 && _selectedProductId.HasValue)
            {
                var existing = await SearchAndSuggestionService.CheckForExistingComplaintAsync(clientId, _selectedProductId.Value);
                if (existing != null)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"UWAGA: Ten klient ma już zgłoszenie ({existing.NrZgloszenia}) na ten produkt!");
                    if (!string.IsNullOrWhiteSpace(existing.NrFaktury)) sb.AppendLine($"Nr faktury: {existing.NrFaktury}");
                    if (!string.IsNullOrWhiteSpace(existing.NrSeryjny)) sb.AppendLine($"Nr seryjny: {existing.NrSeryjny}");
                    if (existing.DataZakupu.HasValue) sb.AppendLine($"Data zakupu: {existing.DataZakupu.Value:dd.MM.yyyy}");
                    lblExistingComplaintInfo.Text = sb.ToString();
                    lblExistingComplaintInfo.Visible = true;
                }
            }
        }

        private async Task<List<string>> PerformFinalValidation()
        {
            var warnings = new List<string>();
            if (string.IsNullOrWhiteSpace(txtNumerFaktury.Text)) warnings.Add("• Pole 'Numer Faktury' jest puste.");
            if (!SearchAndSuggestionService.CheckInvoiceDateConsistency(txtNumerFaktury.Text, dtpDataZakupu.Value)) warnings.Add("• Data zakupu nie zgadza się z miesiącem w numerze faktury.");
            if (!string.IsNullOrWhiteSpace(txtnrSeryjny.Text))
            {
                var (exists, num) = await SearchAndSuggestionService.CheckSerialNumberExistsAsync(txtnrSeryjny.Text);
                if (exists) warnings.Add($"• Numer seryjny istnieje już w zgłoszeniu {num}.");
            }
            if (!string.IsNullOrWhiteSpace(txtNumerFaktury.Text))
            {
                var (exists, num) = await SearchAndSuggestionService.CheckInvoiceNumberExistsAsync(txtNumerFaktury.Text);
                if (exists) warnings.Add($"• Numer faktury istnieje już w zgłoszeniu {num}.");
            }
            return warnings;
        }

        private async Task SaveComplaintAsync()
        {
            if (_selectedClientVM == null && !chkZapiszJakoNowy.Checked)
            {
                MessageBox.Show("Nie wybrano klienta.", "Błąd Walidacji"); tabControlWizard.SelectedTab = tabPageKlient; return;
            }
            if (_selectedProductId == null)
            {
                MessageBox.Show("Nie wybrano produktu.", "Błąd Walidacji"); tabControlWizard.SelectedTab = tabPageProdukt; return;
            }
            if (string.IsNullOrWhiteSpace(txtOpisUsterki.Text))
            {
                MessageBox.Show("Opis usterki jest wymagany.", "Błąd Walidacji"); tabControlWizard.SelectedTab = tabPageUsterka; return;
            }
            var warnings = await PerformFinalValidation();
            if (warnings.Any())
            {
                var warningText = "Wykryto potencjalne problemy:\n\n" + string.Join("\n", warnings) + "\n\nCzy chcesz zapisać zgłoszenie pomimo tych uwag?";
                var result = MessageBox.Show(warningText, "Potwierdzenie Zapisu z Uwagami", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) { return; }
            }

            this.Cursor = Cursors.WaitCursor;
            btnDalej.Enabled = false;

            try
            {
                int finalClientId = chkZapiszJakoNowy.Checked ? await AddNewClientAsync() : _selectedClientVM.Id;
                string newComplaintNumber = await GenerateNewComplaintNumber();
                string complaintSource = DetermineComplaintSource();
                await InsertComplaintIntoDb(newComplaintNumber, finalClientId, complaintSource);
                await PerformPostSaveActions(newComplaintNumber);
                var openResult = MessageBox.Show($"Pomyślnie utworzono zgłoszenie nr: {newComplaintNumber}.\n\nCzy chcesz je teraz otworzyć?", "Sukces", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (openResult == DialogResult.Yes) { /* Logika otwierania formularza */ }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił krytyczny błąd podczas zapisu zgłoszenia: {ex.Message}\n\nŚlad stosu:\n{ex.StackTrace}", "Błąd Zapisu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnDalej.Enabled = true;
            }
        }

        private async Task<int> AddNewClientAsync()
        {
            string query = "INSERT INTO Klienci (ImieNazwisko, NazwaFirmy, NIP, Ulica, KodPocztowy, Miejscowosc, Email, Telefon) VALUES (@imie, @firma, @nip, @ulica, @kod, @miasto, @mail, @tel); SELECT LAST_INSERT_ID();";
            var parameters = new[] { new MySqlParameter("@imie", txtImieNazwisko.Text), new MySqlParameter("@firma", txtNazwaFirmy.Text), new MySqlParameter("@nip", txtNIP.Text), new MySqlParameter("@ulica", txtUlica.Text), new MySqlParameter("@kod", txtKodPocztowy.Text), new MySqlParameter("@miasto", txtMiejscowosc.Text), new MySqlParameter("@mail", txtMail.Text), new MySqlParameter("@tel", txtTelefon.Text) };
            var newId = await _dbService.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(newId);
        }

        private async Task<string> GenerateNewComplaintNumber()
        {
            string currentYearSuffix = DateTime.Now.ToString("yy");
            string pattern = $"%/{currentYearSuffix}";
            string lastNumberQuery = "SELECT NrZgloszenia FROM Zgloszenia WHERE NrZgloszenia LIKE @pattern ORDER BY CAST(SUBSTRING(NrZgloszenia, 1, LOCATE('/', NrZgloszenia) - 1) AS SIGNED) DESC LIMIT 1";
            var lastNumberResult = await _dbService.ExecuteScalarAsync(lastNumberQuery, new MySqlParameter("@pattern", pattern));
            int nextNumber = 1;
            if (lastNumberResult != null && !string.IsNullOrEmpty(lastNumberResult.ToString()))
            {
                nextNumber = int.Parse(lastNumberResult.ToString().Split('/')[0]) + 1;
            }
            return $"{nextNumber:D3}/{currentYearSuffix}";
        }

        private string DetermineComplaintSource()
        {
            switch (_source)
            {
                case WizardSource.Allegro:
                case WizardSource.GoogleSheet:
                case WizardSource.Zwroty:
                    return _initialData?.SourceName ?? "Nieznane";
                case WizardSource.Manual:
                    return rbEnaTruck.Checked ? "Ena-Truck" : "Truck-Shop";
                default:
                    return "Nieznane";
            }
        }

        private async Task InsertComplaintIntoDb(string number, int clientId, string source)
        {
            // ### ZMIANA 3: Finalna aktualizacja zapytania INSERT ###
            string query = @"INSERT INTO Zgloszenia (NrZgloszenia, KlientID, ProduktID, DataZgloszenia, DataZakupu, NrFaktury, NrSeryjny, OpisUsterki, GwarancjaPlatna, skad, StatusOgolny, StatusKlient, StatusProducent, allegroDisputeId, allegroOrderId, allegroBuyerLogin, allegroAccountId)
                             VALUES (@nr, @klientId, @produktId, @dataZglosz, @dataZakup, @nrFv, @nrSn, @opis, @typ, @skad, 'Procesowana', 'Zgłoszone', 'Oczekuje na zgłoszenie', @disputeId, @orderId, @buyerLogin, @accountId)";

            var parameters = new List<MySqlParameter> {
                new MySqlParameter("@nr", number),
                new MySqlParameter("@klientId", clientId),
                new MySqlParameter("@produktId", _selectedProductId.Value),
                new MySqlParameter("@dataZglosz", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new MySqlParameter("@dataZakup", dtpDataZakupu.Value.ToString("yyyy-MM-dd")),
                new MySqlParameter("@nrFv", txtNumerFaktury.Text),
                new MySqlParameter("@nrSn", txtnrSeryjny.Text),
                new MySqlParameter("@opis", txtOpisUsterki.Text),
                new MySqlParameter("@typ", rbPlatna.Checked ? "Płatna" : "Gwarancyjna"),
                new MySqlParameter("@skad", source),
                // Nowe parametry (tylko dla Allegro; dla innych -> NULL):
                new MySqlParameter("@disputeId", _source == WizardSource.Allegro ? (object)_initialData.allegroDisputeId : DBNull.Value),
                new MySqlParameter("@orderId", _source == WizardSource.Allegro ? (object)_initialData.allegroOrderId : DBNull.Value),
                new MySqlParameter("@buyerLogin", _source == WizardSource.Allegro ? (object)_initialData.allegroBuyerLogin : DBNull.Value),
                new MySqlParameter("@accountId", _source == WizardSource.Allegro ? (object)_initialData.AllegroAccountId : DBNull.Value)
            };
            await _dbService.ExecuteNonQueryAsync(query, parameters.ToArray());
        }

        private async Task PerformPostSaveActions(string newComplaintNumber)
        {
            string sourceText = DetermineComplaintSource();
            await new DziennikLogger().DodajAsync(Program.fullName, $"Utworzono nowe zgłoszenie ({sourceText})", newComplaintNumber);
            new Dzialaniee().DodajNoweDzialanie(newComplaintNumber, Program.fullName, $"Utworzono zgłoszenie na podstawie danych z: {sourceText}.");

            if (_source == WizardSource.Allegro)
                await UpdateAllegroDisputeStatusAsync(newComplaintNumber);

            if (_source == WizardSource.GoogleSheet)
                await DeleteRowFromSheetAsync();

            if (_source == WizardSource.Zwroty)
                await MarkUnregisteredReturnAsRegisteredAsync();
        }

        private async Task UpdateAllegroDisputeStatusAsync(string newComplaintNumber)
        {
            var complaintIdResult = await _dbService.ExecuteScalarAsync("SELECT Id FROM Zgloszenia WHERE NrZgloszenia = @nr", new MySqlParameter("@nr", newComplaintNumber));
            long complaintId = Convert.ToInt64(complaintIdResult);
            string query = "UPDATE AllegroDisputes SET ComplaintId = @complaintId WHERE DisputeId = @disputeId";
            await _dbService.ExecuteNonQueryAsync(query, new MySqlParameter("@complaintId", complaintId), new MySqlParameter("@disputeId", _initialData.Id));
        }

        private async Task DeleteRowFromSheetAsync()
        {
            var zgloszenie = _initialData.OriginalObject as ZgloszenieZArkusza;
            if (zgloszenie == null) return;
            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream("reklamacje-baza-c36d05b0ffdb.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { SheetsService.Scope.Spreadsheets });
                }
                var service = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = credential });
                string spreadsheetId = "1VXGP4Cckt6NmSHtiv-Um7nqg-itLMczAGd-5a_Tc4Ds";
                var spreadsheet = await service.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
                var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == zgloszenie.SourceSheet);
                if (sheet?.Properties?.SheetId == null) throw new Exception($"Nie można odnaleźć ID arkusza o nazwie '{zgloszenie.SourceSheet}'.");
                var request = new Request { DeleteDimension = new DeleteDimensionRequest { Range = new DimensionRange { SheetId = sheet.Properties.SheetId.Value, Dimension = "ROWS", StartIndex = zgloszenie.RowIndex - 1, EndIndex = zgloszenie.RowIndex } } };
                var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { request } };
                await service.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).ExecuteAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Zgłoszenie zostało zapisane, ale nie udało się usunąć wiersza z Arkusza Google.\nBłąd: {ex.Message}", "Ostrzeżenie");
            }
        }

        private async Task MarkUnregisteredReturnAsRegisteredAsync()
        {
            // Oznacz wybrany zwrot jako zarejestrowany
            long niezId;
            if (!long.TryParse(_initialData?.Id ?? "", out niezId)) return;

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(@"
                    UPDATE NiezarejestrowaneZwrotyReklamacyjne
                       SET CzyZarejestrowane = 1
                     WHERE Id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", niezId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
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
