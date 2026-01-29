using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
namespace Reklamacje_Dane
{
    public partial class FormPodsumowanieZwrotu : Form
    {
        private readonly int _returnDbId;
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly DatabaseService _dbServiceBaza;
        private DataRow _dbDataRow;
        private string _currentDecision = string.Empty;

        public FormPodsumowanieZwrotu(int returnDbId)
        {
            InitializeComponent();
            _returnDbId = returnDbId;
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
        
        }

        private async void FormPodsumowanieZwrotu_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            await LoadReturnDataAsync();
            await PopulateControls();
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
                }
                else
                {
                    MessageBox.Show("Nie odnaleziono zwrotu o podanym ID.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania szczegółów zwrotu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async Task PopulateControls()
        {
            if (_dbDataRow == null) return;

            lblTitle.Text = $"Podsumowanie Zwrotu: {_dbDataRow["ReferenceNumber"]}";
            this.Text = lblTitle.Text;

            // Info ogólne
            if (_dbDataRow["AllegroAccountId"] != DBNull.Value)
            {
                lblAllegroAccount.Text = (await _dbServiceBaza.ExecuteScalarAsync("SELECT AccountName FROM AllegroAccounts WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["AllegroAccountId"])))?.ToString() ?? "Nieznane";
            }
            lblBuyerLogin.Text = _dbDataRow["BuyerLogin"]?.ToString();
            lblOrderDate.Text = FormatDateTime(_dbDataRow["CreatedAt"]);
            lblInvoice.Text = _dbDataRow["InvoiceNumber"]?.ToString() ?? "Brak";

            // Ocena magazynu
            if (_dbDataRow["StanProduktuId"] != DBNull.Value)
            {
                lblStanProduktu.Text = (await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Nazwa FROM Statusy WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["StanProduktuId"])))?.ToString() ?? "Brak";
            }
            if (_dbDataRow["PrzyjetyPrzezId"] != DBNull.Value)
            {
                lblPrzyjetyPrzez.Text = (await _dbServiceBaza.ExecuteScalarAsync("SELECT \"Nazwa Wyświetlana\" FROM Uzytkownicy WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["PrzyjetyPrzezId"])))?.ToString() ?? "Brak";
            }
            lblUwagiMagazynu.Text = _dbDataRow["UwagiMagazynu"]?.ToString();
            lblDataPrzyjecia.Text = FormatDateTime(_dbDataRow["DataPrzyjecia"]);

            // Decyzja handlowca
            string decyzja = "Brak";
            if (_dbDataRow["DecyzjaHandlowcaId"] != DBNull.Value)
            {
                decyzja = (await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Nazwa FROM Statusy WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["DecyzjaHandlowcaId"])))?.ToString() ?? "Brak";
            }
            lblDecyzjaHandlowca.Text = decyzja;
            lblKomentarzHandlowca.Text = _dbDataRow["KomentarzHandlowca"]?.ToString();
            _currentDecision = decyzja;

            await ConfigureDecisionActionsAsync(decyzja);
        }

        private async Task ConfigureDecisionActionsAsync(string decyzja)
        {
            panelPonownaWysylka.Visible = false;
            btnPrzekazanoDoReklamacji.Visible = false;
            btnNaPolke.Visible = false;
            btnArchiwizuj.Visible = false;
            btnZatwierdzPonowna.Visible = false;

            if (string.IsNullOrWhiteSpace(decyzja))
            {
                return;
            }

            if (IsDecisionPonownaWysylka(decyzja))
            {
                panelPonownaWysylka.Visible = true;
                btnZatwierdzPonowna.Visible = true;
                dtpPonownaData.Value = DateTime.Today;
                await LoadCarrierOptionsAsync();
                return;
            }

            if (IsDecisionReklamacje(decyzja))
            {
                btnPrzekazanoDoReklamacji.Visible = true;
                return;
            }

            if (IsDecisionNaPolke(decyzja))
            {
                btnNaPolke.Visible = true;
                return;
            }

            btnArchiwizuj.Visible = true;
        }

        private async Task LoadCarrierOptionsAsync()
        {
            try
            {
                var dt = await _dbServiceMagazyn.GetDataTableAsync(@"
                    SELECT DISTINCT CarrierName
                    FROM AllegroCustomerReturns
                    WHERE CarrierName IS NOT NULL AND CarrierName <> ''
                    ORDER BY CarrierName");

                comboCarrier.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    comboCarrier.Items.Add(row["CarrierName"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd wczytywania przewoźników: {ex.Message}");
            }
        }

        private async void btnZatwierdzPonowna_Click(object sender, EventArgs e)
        {
            var przewoznik = comboCarrier.Text?.Trim();
            var numerListu = txtNumerListu.Text?.Trim();
            var dataWysylki = dtpPonownaData.Value.Date;

            if (string.IsNullOrWhiteSpace(przewoznik))
            {
                MessageBox.Show("Wybierz przewoźnika.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(numerListu))
            {
                MessageBox.Show("Podaj numer listu przewozowego.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var actionText = $"Ponowna wysyłka: data {dataWysylki:dd.MM.yyyy}, przewoźnik {przewoznik}, list {numerListu}.";
            await FinalizeReturnAsync(actionText, przewoznik, numerListu);
        }

        private async void btnPrzekazanoDoReklamacji_Click(object sender, EventArgs e)
        {
            await FinalizeReturnAsync("Przekazano fizycznie na reklamacje.", null, null);
        }

        private async void btnNaPolke_Click(object sender, EventArgs e)
        {
            await FinalizeReturnAsync("Odłożone na stan magazynowy.", null, null);
        }

        private async void btnArchiwizuj_Click(object sender, EventArgs e)
        {
            await FinalizeReturnAsync("Zwrot zakończony zgodnie z decyzją.", null, null);
        }

        private async Task FinalizeReturnAsync(string actionText, string? carrierName, string? waybill)
        {
            var confirm = MessageBox.Show("Czy na pewno chcesz zakończyć zwrot zgodnie z decyzją?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var statusZakonczonyId = await _dbServiceMagazyn.ExecuteScalarAsync(
                    "SELECT Id FROM Statusy WHERE Nazwa = 'Zakończony' AND TypStatusu = 'StatusWewnetrzny' LIMIT 1");

                if (statusZakonczonyId == null)
                {
                    MessageBox.Show("Brak statusu 'Zakończony' w bazie danych. Skonfiguruj go w ustawieniach.", "Błąd Konfiguracji", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var updateQuery = @"
                    UPDATE AllegroCustomerReturns
                    SET StatusWewnetrznyId = @statusId,
                        CarrierName = COALESCE(@carrierName, CarrierName),
                        Waybill = COALESCE(@waybill, Waybill)
                    WHERE Id = @id";

                await _dbServiceMagazyn.ExecuteNonQueryAsync(
                    updateQuery,
                    new MySqlParameter("@statusId", statusZakonczonyId),
                    new MySqlParameter("@carrierName", (object?)carrierName ?? DBNull.Value),
                    new MySqlParameter("@waybill", (object?)waybill ?? DBNull.Value),
                    new MySqlParameter("@id", _returnDbId));

                var userName = GuessCurrentUserDisplayName();
                await _dbServiceMagazyn.ExecuteNonQueryAsync(
                    "INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc) VALUES (@id, NOW(), @user, @tresc)",
                    new MySqlParameter("@id", _returnDbId),
                    new MySqlParameter("@user", userName),
                    new MySqlParameter("@tresc", actionText));

                await _dbServiceMagazyn.ExecuteNonQueryAsync(
                    "INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId) VALUES (@data, @user, @akcja, @id)",
                    new MySqlParameter("@data", DateTime.Now),
                    new MySqlParameter("@user", userName),
                    new MySqlParameter("@akcja", actionText),
                    new MySqlParameter("@id", _returnDbId));

                ToastManager.ShowToast("Sukces", "Zwrot został zakończony.", NotificationType.Success);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas kończenia zwrotu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private static bool IsDecisionPonownaWysylka(string decyzja)
            => decyzja.Equals("Ponowna wysyłka", StringComparison.OrdinalIgnoreCase)
               || decyzja.IndexOf("ponown", StringComparison.OrdinalIgnoreCase) >= 0
               || decyzja.IndexOf("wysył", StringComparison.OrdinalIgnoreCase) >= 0;

        private static bool IsDecisionReklamacje(string decyzja)
            => decyzja.Equals("Na dział Reklamacji", StringComparison.OrdinalIgnoreCase)
               || decyzja.Equals("Przekaż do reklamacji", StringComparison.OrdinalIgnoreCase)
               || decyzja.IndexOf("reklam", StringComparison.OrdinalIgnoreCase) >= 0;

        private static bool IsDecisionNaPolke(string decyzja)
            => decyzja.Equals("Na półkę", StringComparison.OrdinalIgnoreCase)
               || decyzja.IndexOf("półk", StringComparison.OrdinalIgnoreCase) >= 0
               || decyzja.IndexOf("polk", StringComparison.OrdinalIgnoreCase) >= 0;

        private static string GuessCurrentUserDisplayName()
        {
            try
            {
                var env = Environment.UserName;
                if (!string.IsNullOrWhiteSpace(env)) return env;
            }
            catch
            {
            }

            return "Magazyn";
        }
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
       
}
}
