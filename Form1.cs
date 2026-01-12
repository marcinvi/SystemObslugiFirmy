using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class Form1 : Form
    {
        // --- ZMIANA: Trzy niezależne timery dla każdego zadania synchronizacji ---
        private System.Timers.Timer _logCheckTimer;
        private System.Timers.Timer _googleSheetSyncTimer;
        private System.Timers.Timer _allegroSyncTimer;
        private System.Windows.Forms.Label lblAllegroMessages;
        private long _lastLogId = 0;
        private int _lastGoogleSheetRows = -1;

        // --- ZMIANA: Flagi zapobiegające nakładaniu się operacji ---
        private volatile bool _isCheckingLogs = false;
        private volatile bool _isCheckingGoogleSheets = false;
        private volatile bool _isCheckingAllegro = false;

        private readonly AllegroSyncService _allegroSyncService;

        public Form1(string fullName)
        {
            InitializeComponent();
            this.Text = "System Obsługi Reklamacji";
            _allegroSyncService = new AllegroSyncService();

            EnableDoubleBuffering(dataGridViewProcessing);
            EnableDoubleBuffering(dataGridViewReminders);
            EnableDoubleBuffering(dataGridViewChangeLog);

            Program.fullName = fullName;
            labelZalogowany.Text = "Zalogowany: " + fullName;

            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
            UpdateManager.OnUpdateNeeded += HandleUpdateNeeded;

            this.dataGridViewProcessing.CellClick += new DataGridViewCellEventHandler(anyDataGridView_CellClick);
            this.dataGridViewReminders.CellClick += new DataGridViewCellEventHandler(anyDataGridView_CellClick);
            this.dataGridViewChangeLog.CellClick += new DataGridViewCellEventHandler(anyDataGridView_CellClick);
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Zamykanie starego okna (jeśli istnieje)
            Form formDoZamkniecia = Application.OpenForms.OfType<Form19>().FirstOrDefault();
            if (formDoZamkniecia != null)
            {
                formDoZamkniecia.Close();
            }

            await LoadDataAsync();
            InitializeTimers();

            // Uruchomienie synchronizacji od razu po starcie aplikacji
            _ = RunAllegroSync();
            _ = RunGoogleSheetsSync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _logCheckTimer?.Stop();
            _googleSheetSyncTimer?.Stop();
            _allegroSyncTimer?.Stop();
            UpdateManager.OnUpdateNeeded -= HandleUpdateNeeded;
        }

        /// <summary>
        /// Inicjalizuje wszystkie timery z zadanymi interwałami.
        /// </summary>
        private void InitializeTimers()
        {
            // Timer do sprawdzania logów w bazie danych (10 sekund)
            _logCheckTimer = new System.Timers.Timer(10000);
            _logCheckTimer.Elapsed += async (s, ev) => await CheckForLogChanges();
            _logCheckTimer.AutoReset = true;
            _logCheckTimer.Start();

            // Timer do synchronizacji z Google Sheets (30 sekund)
            _googleSheetSyncTimer = new System.Timers.Timer(30000);
            _googleSheetSyncTimer.Elapsed += async (s, ev) => await RunGoogleSheetsSync();
            _googleSheetSyncTimer.AutoReset = true;
            _googleSheetSyncTimer.Start();

            // Timer do synchronizacji z Allegro (30 sekund)
            _allegroSyncTimer = new System.Timers.Timer(30000);
            _allegroSyncTimer.Elapsed += async (s, ev) => await RunAllegroSync();
            _allegroSyncTimer.AutoReset = true;
            _allegroSyncTimer.Start();
        }

        /// <summary>
        /// Uruchamia synchronizację z Allegro.
        /// </summary>
        private async Task RunAllegroSync()
        {
            if (_isCheckingAllegro) return;
            _isCheckingAllegro = true;

            try
            {
                var result = await _allegroSyncService.SynchronizeDisputesAsync();

                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        // Aktualizuj licznik niezarejestrowanych zgłoszeń
                        lblAllegroCount.Text = result.UnregisteredDisputesCount.ToString();
                        lblAllegroCount.Visible = result.UnregisteredDisputesCount > 0;

                        // [NOWA LOGIKA] Aktualizuj licznik nieprzeczytanych wiadomości
                        lblAllegroMessages.Text = result.DisputesWithNewMessages.ToString();
                       // lblAllegroMessages.Visible = result.DisputesWithNewMessages > 0;

                        if (result.DisputesWithNewMessages > 0)
                        {
                            powiadomienie.Text = $"Masz {result.DisputesWithNewMessages} dyskusji z nowymi wiadomościami!";
                            powiadomienie.BackColor = Color.Gold;
                            powiadomienie.ForeColor = Color.Black;
                            powiadomienie.Visible = true;
                        }

                        if (result.NewDisputesFound > 0)
                        {
                            powiadomienie.Text = $"Pobrano {result.NewDisputesFound} nowych dyskusji z Allegro!";
                            powiadomienie.BackColor = Color.DarkOrange;
                            powiadomienie.Visible = true;
                            UpdateManager.NotifySubscribers();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas synchronizacji z Allegro: {ex.Message}");
            }
            finally
            {
                _isCheckingAllegro = false;
            }
        }
        private void lblAllegroMessages_Click(object sender, EventArgs e)
        {
            // Otwórz formularz z listą dyskusji Allegro
            // Załóżmy, że jest to FormAllegroWizard lub inny dedykowany formularz
            using (var form = new FormAllegroManager())
            {
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Uruchamia synchronizację z Google Sheets.
        /// </summary>
        private async Task RunGoogleSheetsSync()
        {
            if (_isCheckingGoogleSheets) return;
            _isCheckingGoogleSheets = true;

            try
            {
                await UpdateGoogleSheetRowCountAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas synchronizacji z Google Sheets: {ex.Message}");
            }
            finally
            {
                _isCheckingGoogleSheets = false;
            }
        }

        /// <summary>
        /// Sprawdza zmiany w dzienniku zdarzeń.
        /// </summary>
        private async Task CheckForLogChanges()
        {
            if (_isCheckingLogs) return;
            _isCheckingLogs = true;

            try
            {
                long currentMaxId = 0;
                string user = "", action = "", complaint_nr = "";

                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new SQLiteCommand("SELECT Id, Uzytkownik, Akcja, DotyczyZgloszenia FROM Dziennik ORDER BY Id DESC LIMIT 1", con))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                currentMaxId = reader.GetInt64(0);
                                user = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                action = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                complaint_nr = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            }
                        }
                    }
                }

                if (currentMaxId > _lastLogId)
                {
                    _lastLogId = currentMaxId;
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            powiadomienie.Text = $"[{user}] {action} (dot. {complaint_nr})";
                            powiadomienie.BackColor = Color.DodgerBlue;
                            powiadomienie.Visible = true;
                            _ = LoadDataAsync();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas sprawdzania dziennika: " + ex.Message);
            }
            finally
            {
                _isCheckingLogs = false;
            }
        }

        private void HandleUpdateNeeded()
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate { _ = LoadDataAsync(); });
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new SQLiteCommand("SELECT MAX(Id) FROM Dziennik", con))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        _lastLogId = (result != null && result != DBNull.Value) ? Convert.ToInt64(result) : 0;
                    }
                }
                await Task.WhenAll(
                    LoadProcessingCasesAsync(),
                    LoadRemindersAsync(),
                    LoadChangeLogAsync()
                );
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblLastRefresh.Text = $"Ostatnie odświeżenie: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show("Błąd podczas ładowania danych: " + ex.Message); }
        }

        private async void anyDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var dgv = sender as DataGridView;
            if (dgv == null) return;

            try
            {
                if (dgv.Columns.Contains("NrZgloszenia"))
                {
                    var cellValue = dgv.Rows[e.RowIndex].Cells["NrZgloszenia"].Value;
                    string numerZgloszenia = cellValue?.ToString();

                    if (!string.IsNullOrEmpty(numerZgloszenia) && numerZgloszenia != "0")
                    {
                        using (var form2 = new Form2(numerZgloszenia))
                        {
                            form2.ShowDialog();
                        }
                        await LoadDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się otworzyć szczegółów zgłoszenia: " + ex.Message);
            }
        }

        private async Task UpdateGoogleSheetRowCountAsync()
        {
            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream("reklamacje-baza-ed853b4e33f7.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { SheetsService.Scope.SpreadsheetsReadonly });
                }
                var service = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = credential });
                string spreadsheetId = "1VXGP4Cckt6NmSHtiv-Um7nqg-itLMczAGd-5a_Tc4Ds";
                string[] sheetsToRead = { "B", "Z" };
                int totalRows = 0;

                foreach (var sheetName in sheetsToRead)
                {
                    var request = service.Spreadsheets.Values.Get(spreadsheetId, $"{sheetName}!A:A");
                    var response = await request.ExecuteAsync();
                    if (response.Values != null)
                    {
                        totalRows += response.Values.Count(row => row.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString())));
                    }
                }
                int complaintCount = Math.Max(0, totalRows - 2); // Odejmujemy 2 wiersze nagłówkowe

                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (complaintCount > _lastGoogleSheetRows && _lastGoogleSheetRows != -1)
                        {
                            powiadomienie.Text = "Nowe zgłoszenie do rejestracji w Google Sheets!";
                            powiadomienie.BackColor = Color.SeaGreen;
                            powiadomienie.Visible = true;
                        }
                        _lastGoogleSheetRows = complaintCount;
                        label10.Text = complaintCount.ToString();
                    });
                }
            }
            catch (Exception ex)
            {
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate { label10.Text = "Błąd"; });
                }
                Console.WriteLine("Błąd pobierania danych z Google Sheets: " + ex.Message);
            }
        }

        private async Task LoadProcessingCasesAsync()
        {
            string query = @"
                SELECT z.NrZgloszenia, COALESCE(k.ImieNazwisko, k.NazwaFirmy, 'Brak klienta') AS Klient,
                       p.NazwaKrotka AS Produkt, z.DataZgloszenia, z.StatusKlient
                FROM Zgloszenia z
                LEFT JOIN Klienci k ON z.KlientID = k.Id
                LEFT JOIN Produkty p ON z.ProduktID = p.Id
                WHERE z.StatusOgolny = 'Procesowana'
                ORDER BY CAST(SUBSTRING(z.NrZgloszenia, LOCATE('/', z.NrZgloszenia) + 1) AS SIGNED) DESC,
                         CAST(SUBSTRING(z.NrZgloszenia, 1, LOCATE('/', z.NrZgloszenia) - 1) AS SIGNED) DESC";
            await LoadTableDataAsync(dataGridViewProcessing, query);
        }

        private async Task LoadRemindersAsync()
        {
            string query = "SELECT Tresc, DotyczyZgloszenia AS NrZgloszenia FROM Przypomnienia WHERE CzyZrealizowane = 0 ORDER BY DataPrzypomnienia ASC";
            await LoadTableDataAsync(dataGridViewReminders, query);
        }

        private async Task LoadChangeLogAsync()
        {
            string query = "SELECT Data, DotyczyZgloszenia AS NrZgloszenia, Uzytkownik, Akcja FROM Dziennik ORDER BY Id DESC LIMIT 100";
            await LoadTableDataAsync(dataGridViewChangeLog, query);
        }

        private async Task LoadTableDataAsync(DataGridView dgv, string query)
        {
            var table = new DataTable();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                using (var adapter = new SQLiteDataAdapter(query, con))
                {
                    await Task.Run(() => adapter.Fill(table));
                }
                if (dgv.IsHandleCreated && !dgv.IsDisposed)
                {
                    dgv.Invoke((MethodInvoker)delegate { dgv.DataSource = table; });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd ładowania danych do tabeli {dgv.Name}: {ex.Message}");
            }
        }

        private void EnableDoubleBuffering(Control c)
        {
            var prop = c.GetType().GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null) prop.SetValue(c, true, null);
        }

        #region --- Metody obsługi zdarzeń UI ---

        private void OpenForm14(object sender, EventArgs e) { using (var form = new Form14()) { form.ShowDialog(); } }

        // --- ZMIANA: Otwieranie nowego formularza Allegro ---
        private void Allegro(object sender, EventArgs e) { using (var form = new FormAllegroWizard()) { form.ShowDialog(); } }
        private void Allegrowiadomosci(object sender, EventArgs e)
        {
            // Zamiast starego formularza, otwieramy nowy manager
            using (var form = new FormAllegroManager())
            {
                form.ShowDialog();
            }
        }
        private void OpenForm13(object sender, EventArgs e) { using (var form = new Form13()) { form.ShowDialog(); } }
        private void OpenFormUstawienia(object sender, EventArgs e) { using (var form = new FormUstawienia()) { form.ShowDialog(); } }
        private void pictureBox8_Click(object sender, EventArgs e) => OpenForm14(sender, e);
        private void lblLastRefresh_Click(object sender, EventArgs e) => _ = LoadDataAsync();
        private void label2_Click(object sender, EventArgs e) => OpenForm14(sender, e);
        private void pictureBox4_Click(object sender, EventArgs e) => Allegro(sender, e);
        private void label4_Click(object sender, EventArgs e) => OpenForm13(sender, e);
        private void pictureBox5_Click(object sender, EventArgs e) => OpenForm13(sender, e);
        private void label3_Click(object sender, EventArgs e)
        {
            // Uruchomienie zaawansowanej wyszukiwarki Form20 z domyślnymi ustawieniami
            var columnsToShow = new HashSet<int>(); // Pusta lista, aby Form20 pokazał wszystko (zgodnie ze zmianą w ApplySettings)
            var columnValues = new Dictionary<int, string>();
            var columnWidths = new Dictionary<int, int>();
            string labelText = "Zaawansowane wyszukiwanie - wszystkie zgłoszenia";

            using (var form20 = new Form20(columnsToShow, "", labelText, columnValues, columnWidths))
            {
                form20.ShowDialog();
            }
        }
        private void labelZalogowany_Click(object sender, EventArgs e) { MessageBox.Show($"Zalogowany jako: {Program.fullName}"); }
        private void powiadomienie_Click(object sender, EventArgs e) { powiadomienie.Visible = false; }
        private void label10_Click(object sender, EventArgs e) => _ = UpdateGoogleSheetRowCountAsync();
        private void dodajNoweToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void pokażWyszystkieToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void oczekująceNaDostawęProsuktuToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void uzględnionePrzezProducentaToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void nowyProduktToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void maZostaćWysłanyToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void dostarczonyToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void notaToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void maZostaćWystawionaToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void zostałaWystawionaToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void nieuzględnionePrzezProducentaToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void wszystykieToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void uzględnioneKlientToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void usterkiNieStwierdzonoToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void poOkresieReklamacjiToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void usterkaZWinyUżytkowaniaToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void brakKontaktuToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void nieuzględnioneKlientowiToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void wysłaneNoweProduktyToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void naprawioneToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void skorygowaneFakturyToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void zarejestrujNoweToolStripMenuItem_Click(object sender, EventArgs e) { MessageBox.Show("Funkcja w przygotowaniu."); }
        private void pictureBox1_Click(object sender, EventArgs e) => OpenFormUstawienia(sender, e);
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