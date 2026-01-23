using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class ReklamacjeControl : UserControl
    {
        // Pola Timerów
        private readonly ToolTip _statusTooltip = new ToolTip();
        private System.Timers.Timer _popupCheckTimer;
        private System.Timers.Timer _logCheckTimer;
        private System.Timers.Timer _googleSheetSyncTimer;
        private System.Timers.Timer _allegroSyncTimer;
        private System.Timers.Timer _emailSyncTimer; // Timer Maili
        private System.Timers.Timer _remindersCheckTimer;
        private System.Timers.Timer _shipmentCheckTimer;
        private System.Timers.Timer _returnsSyncTimer;

        // Flagi blokujące
        private volatile bool _isCheckingPopups = false;
        private volatile bool _isCheckingLogs = false;
        private volatile bool _isCheckingGoogleSheets = false;
        private volatile bool _isCheckingAllegro = false;
        private volatile bool _isCheckingEmails = false; // Flaga Maili
        private volatile bool _isCheckingReminders = false;
        private volatile bool _isCheckingShipments = false;
        private volatile bool _isCheckingReturns = false;

        // Dane
        private long _lastLogId = 0;
        private int _lastGoogleSheetRows = -1;
        private int _lastUnregisteredReturns = -1;
        private readonly HashSet<int> _shownReminders = new HashSet<int>();
        private readonly Dictionary<string, string> _syncStatus = new Dictionary<string, string>();
        private int _isLoadingData;
        private int _pendingLoad;

        // Serwisy
        private readonly AllegroSyncService _allegroSyncService;
        private readonly EmailService _emailService; // Serwis Maili
        private ShipmentNotificationService _shipmentNotificationService;
        private readonly string _fullName;
        private readonly string _userRole;
        private WebView2 _privateWebView;

        // Google Config
        private const string GoogleSpreadsheetId = "1VXGP4Cckt6NmSHtiv-Um7nqg-itLMczAGd-5a_Tc4Ds";
        private static readonly string[] GoogleSheetsToRead = new[] { "B", "Z" };

        // UI Helpers
        private Button _tabDecyzjaBtn, _tabKurierBtn, _tabReczneBtn;
        private string _remindersActiveCategory = "Czas na decyzję";
        private ContextMenuStrip _reminderCardCtx;

        public ReklamacjeControl(string fullName, string userRole)
        {
            InitializeComponent();

            _fullName = fullName;
            _userRole = userRole;

            // Inicjalizacja serwisów
            _allegroSyncService = new AllegroSyncService();
            _emailService = new EmailService();

            EnsureProcessingGridScrollable();

            this.Load += ReklamacjeControl_Load;
            this.Disposed += ReklamacjeControl_Disposed;
            this.Resize += ReklamacjeControl_Resize;

            UpdateManager.OnUpdateNeeded += HandleUpdateNeeded;

            // Menu Context
            otwórzZgłoszenieToolStripMenuItem.Click += otwórzZgłoszenieToolStripMenuItem_Click;
            dodajPrzypomnienieToolStripMenuItem.Click += dodajPrzypomnienieToolStripMenuItem_Click;
            usunZgloszenieToolStripMenuItem.Click += usunZgloszenieToolStripMenuItem_Click;
            kopiujNumerZgłoszeniaToolStripMenuItem.Click += kopiujNumerZgłoszeniaToolStripMenuItem_Click;

            // Przyciski Menu
            btnHome.Click += menuStronaGlowna_Click;
            btnNewGoogle.Click += menuNiezarejestrowaneGoogle_Click;
            btnNewAllegro.Click += menuNiezarejestrowaneAllegro_Click;
            btnNewReturn.Click += menuNiezarejestrowaneZwroty_Click;
            btnAddManual.Click += menuDodajNowe_Click;
            btnAllCases.Click += menuWszystkieZgloszenia_Click;
            btnChat.Click += menuCzatAllegro_Click;
            btnReminders.Click += menuPrzypomnienia_Click;
            btnClients.Click += menuKlienci_Click;
            btnProducts.Click += menuProdukty_Click;
            btnProducers.Click += menuProducenci_Click;
            btnSettings.Click += menuUstawienia_Click;
            btnTracking.Click += menuSledzeniePrzesylek_Click;
            btnWarehouse.Click += (s, e) => new FormStanMagazynowy().Show();
            btnEmail.Click += (s, e) => new FormSkrzynka().Show();
            btnContactCenter.Click += (s, e) =>
            {
                HighlightMenuButton(s); // Jeśli używasz podświetlania aktywnego przycisku
                new FormHistoria().Show();
            };
            btnRefresh.Click += refreshIcon_Click;
            lblLastRefresh.Click += lblLastRefresh_Click;

            // Inicjalizacja UI
            BuildRemindersTabsBar();
            InitializeSyncStatuses();

            txtFilterProcessing.TextChanged += txtFilterProcessing_TextChanged;
            dataGridViewProcessing.CellDoubleClick += anyDataGridView_CellClick;
            dataGridViewChangeLog.CellDoubleClick += anyDataGridView_CellClick;

            _statusTooltip.AutoPopDelay = 15000;
            _statusTooltip.InitialDelay = 500;
            _statusTooltip.ReshowDelay = 500;
        }

        private void InitializeSyncStatuses()
        {
            UpdateSyncStatus("Allegro", "Oczekiwanie...", "");
            UpdateSyncStatus("E-mail", "Oczekiwanie...", ""); // Dodano E-mail
            UpdateSyncStatus("Google Sheets", "Oczekiwanie...", "");
            UpdateSyncStatus("Przesyłki DPD", "Oczekiwanie...", "");
            UpdateSyncStatus("Magazyn Zwrotów", "Oczekiwanie...", "");
            UpdateSyncStatus("Przypomnienia", "Oczekiwanie...", "");
            UpdateSyncStatus("Dziennik", "Oczekiwanie...", "");
        }

        private void SetActivity(string text)
        {
            SafeInvoke(() => {
                if (lblSyncActivity != null)
                {
                    lblSyncActivity.Text = text;
                    lblSyncActivity.ForeColor = text.ToLower().Contains("błąd") ? Color.Red : Color.SteelBlue;
                }
            });
        }

        private async void ReklamacjeControl_Load(object sender, EventArgs e)
        {
            // 1. Inicjalizacja lekkich rzeczy (WebView, Serwisy)
            _privateWebView = new WebView2 { Visible = false };
            this.Controls.Add(_privateWebView);
            // WebView inicjujemy w tle, nie czekamy na niego
            _ = _privateWebView.EnsureCoreWebView2Async(null);

            _shipmentNotificationService = new ShipmentNotificationService(this.FindForm(), _privateWebView);

            try { await ReminderService.InitializeAsync(); } catch { }

            ReklamacjeControl_Resize(null, null);

            // 2. Startujemy ładowanie danych (nie blokujemy UI)
            // Używamy FireAndForgetSafe, żeby formularz się pokazał od razu, a dane "wskoczyły" za chwilę
            RequestDataReload();

            // 3. Timery startujemy z lekkim opóźnieniem, żeby nie zamulać startu
            InitializeTimers();

            // Zadania tła uruchamiamy z opóźnieniem 2-5 sekund, żeby najpierw załadowało się UI
            Task.Delay(2000).ContinueWith(_ => RunEmailSync().FireAndForgetSafe(this));
            Task.Delay(3000).ContinueWith(_ => CheckShipmentsAndNotify().FireAndForgetSafe(this));
            Task.Delay(4000).ContinueWith(_ => RunAllegroSync().FireAndForgetSafe(this));
            Task.Delay(5000).ContinueWith(_ => GenerateAutomaticRemindersAsync().FireAndForgetSafe(this));
            Task.Delay(6000).ContinueWith(_ => RunReturnsSync().FireAndForgetSafe(this));
            Task.Delay(7000).ContinueWith(_ => RunGoogleSheetsSync().FireAndForgetSafe(this));
        }

        private async Task LoadDataAsync()
        {
            if (Interlocked.Exchange(ref _isLoadingData, 1) == 1)
            {
                Interlocked.Exchange(ref _pendingLoad, 1);
                return;
            }

            try
            {
                SetActivity("Odświeżanie danych...");

                // Uruchamiamy wszystkie 3 ładowania równolegle
                var task1 = LoadProcessingCasesAsync();
                var task2 = RebuildRemindersCardsAsync();
                var task3 = LoadChangeLogAsync();
                var task4 = UpdateAllegroChatUnreadCountAsync();

                // Czekamy na wszystkie (ale asynchronicznie, UI nie zamarznie)
                await Task.WhenAll(task1, task2, task3, task4);

                SafeInvoke(() => lblLastRefresh.Text = "Odświeżono: " + DateTime.Now.ToString("HH:mm"));
            }
            catch (Exception ex)
            {
                // Logujemy błąd, ale nie pokazujemy MessageBoxa przy każdym odświeżeniu, bo to irytujące
                Console.WriteLine("Błąd ładowania danych: " + ex.Message);
            }
            finally
            {
                SetActivity("");
                Interlocked.Exchange(ref _isLoadingData, 0);
                if (Interlocked.Exchange(ref _pendingLoad, 0) == 1)
                {
                    RequestDataReload();
                }
            }
        }

        private async Task UpdateAllegroChatUnreadCountAsync()
        {
            try
            {
                int count = 0;
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("SELECT COUNT(*) FROM AllegroDisputes WHERE HasNewMessages = 1", con);
                    count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                SafeInvoke(() =>
                {
                    btnChat.Text = $"💬 Czat Allegro ({count})";
                    btnChat.ForeColor = count > 0 ? Color.Orange : Color.FromArgb(180, 190, 210);
                });
            }
            catch
            {
                // ignoruj błąd odświeżania licznika
            }
        }

        private void ReklamacjeControl_Resize(object sender, EventArgs e)
        {
            if (splitContainerBottom.Width > 0) splitContainerBottom.SplitterDistance = splitContainerBottom.Width / 2;
        }

        // --- LOGIKA BIZNESOWA I ŁADOWANIE DANYCH ---

        private async Task LoadProcessingCasesAsync()
        {
            string query = @"
                SELECT
                    z.NrZgloszenia,
                    CASE
                        WHEN k.NazwaFirmy IS NOT NULL AND k.NazwaFirmy != '' AND k.ImieNazwisko IS NOT NULL AND k.ImieNazwisko != '' THEN CONCAT(k.NazwaFirmy, ' | ', k.ImieNazwisko)
                        WHEN k.NazwaFirmy IS NOT NULL AND k.NazwaFirmy != '' THEN k.NazwaFirmy
                        WHEN k.ImieNazwisko IS NOT NULL AND k.ImieNazwisko != '' THEN k.ImieNazwisko
                        ELSE 'Brak klienta'
                    END AS Klient,
                    p.NazwaKrotka AS Produkt,
                    z.OpisUsterki,
                    DATEDIFF(NOW(), z.DataZgloszenia) AS DniPoZgloszeniu
                FROM Zgloszenia z
                LEFT JOIN Klienci k ON z.KlientID = k.Id
                LEFT JOIN Produkty p ON z.ProduktID = p.Id
                WHERE z.StatusOgolny = 'Procesowana'
                ORDER BY
                    CAST(SUBSTRING(z.NrZgloszenia, LOCATE('/', z.NrZgloszenia) + 1) AS SIGNED) DESC,
                    CAST(SUBSTRING(z.NrZgloszenia, 1, LOCATE('/', z.NrZgloszenia) - 1) AS SIGNED) DESC";

            await LoadTableDataAsync(dataGridViewProcessing, query);

            SafeInvoke(() =>
            {
                var cols = dataGridViewProcessing.Columns;
                if (cols.Contains("OpisUsterki")) cols["OpisUsterki"].Visible = false;
                if (cols.Contains("NrZgloszenia")) { cols["NrZgloszenia"].HeaderText = "Nr"; cols["NrZgloszenia"].Width = 80; cols["NrZgloszenia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; }
                if (cols.Contains("DniPoZgloszeniu")) { cols["DniPoZgloszeniu"].HeaderText = "Dni"; cols["DniPoZgloszeniu"].Width = 50; cols["DniPoZgloszeniu"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; }
                if (cols.Contains("Klient")) cols["Klient"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                if (cols.Contains("Produkt")) cols["Produkt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            });
        }

      

        // --- TIMERY ---

        private void InitializeTimers()
        {
            _logCheckTimer = NewTimer(10000, async () => await CheckForLogChanges());
            _emailSyncTimer = NewTimer(120000, async () => await RunEmailSync()); // Co 2 minuty
            _googleSheetSyncTimer = NewTimer(60000, async () => await RunGoogleSheetsSync());
            _allegroSyncTimer = NewTimer(60000, async () => await RunAllegroSync());
            _remindersCheckTimer = NewTimer(3600000, async () => await GenerateAutomaticRemindersAsync());
            _shipmentCheckTimer = NewTimer(300000, async () => await CheckShipmentsAndNotify());
            _returnsSyncTimer = NewTimer(60000, async () => await RunReturnsSync());
            _popupCheckTimer = NewTimer(60000, async () => await CheckForDueRemindersAndPopup());
        }

        private static System.Timers.Timer NewTimer(double interval, Func<Task> callback)
        {
            var t = new System.Timers.Timer(interval) { AutoReset = true };
            t.Elapsed += async (s, ev) => await callback();
            t.Start();
            return t;
        }

        // --- ZADANIA TŁA ---

        private async Task RunEmailSync()
        {
            if (_isCheckingEmails) return; _isCheckingEmails = true;
            try
            {
                SetActivity("E-mail: Pobieranie...");
                // 1. Pobieranie z serwera do bazy
                await _emailService.PobierzPoczteDlaWszystkichKontAsync();

                // 2. Liczenie nieprzeczytanych (zakładam, że nowe to te z ostatnich 24h)
                int count = 0;
                using (var con = Database.GetNewOpenConnection())
                {
                    // Liczba maili przychodzących z ostatnich 24h
                    string sql = "SELECT COUNT(*) FROM CentrumKontaktu WHERE Typ='Mail' AND Kierunek='IN' AND DataWyslania > DATE_SUB(NOW(), INTERVAL 1 DAY)";
                    count = Convert.ToInt32(await new MySqlCommand(sql, con).ExecuteScalarAsync());
                }

                SafeInvoke(() => {
                    btnEmail.Text = $"📧 Skrzynka Email ({count})";
                    if (count > 0)
                    {
                        btnEmail.ForeColor = Color.Orange;
                        UpdateManager.NotifySubscribers();
                    }
                    else btnEmail.ForeColor = Color.FromArgb(180, 190, 210);
                });

                UpdateSyncStatus("E-mail", "OK", $"Ostatnio: {DateTime.Now:HH:mm}");
            }
            catch (Exception ex) { UpdateSyncStatus("E-mail", "Błąd", ex.Message); }
            finally { _isCheckingEmails = false; SetActivity(""); }
        }

        private async Task CheckShipmentsAndNotify()
        {
            if (_isCheckingShipments) return; _isCheckingShipments = true;
            try
            {
                SetActivity("DPD: Pobieranie statusów...");
                await _shipmentNotificationService.CheckAndNotifyAsync();
                UpdateSyncStatus("Przesyłki DPD", "OK", "Sprawdzono pomyślnie");
            }
            catch (Exception ex) { UpdateSyncStatus("Przesyłki DPD", "Błąd", ex.Message); }
            finally { _isCheckingShipments = false; SetActivity(""); }
        }

        private async Task RunAllegroSync()
        {
            if (_isCheckingAllegro) return; _isCheckingAllegro = true;
            try
            {
                SetActivity("Allegro: Łączenie z API...");
                var progress = new Progress<string>(msg => SetActivity($"Allegro: {msg}"));
                var result = await _allegroSyncService.SynchronizeDisputesAsync(progress);
                SafeInvoke(() => {
                    btnNewAllegro.Text = $"🟠 Nowe Allegro ({result.UnregisteredDisputesCount})";
                    btnChat.Text = $"💬 Czat Allegro ({result.DisputesWithNewMessages})";
                    if (result.NewDisputesFound > 0) UpdateManager.NotifySubscribers();
                });
                UpdateSyncStatus("Allegro", "OK", $"Pobrano {result.UnregisteredDisputesCount} nowych");
            }
            catch (Exception ex) { UpdateSyncStatus("Allegro", "Błąd", ex.Message); }
            finally { _isCheckingAllegro = false; SetActivity(""); }
        }

        private async Task RunGoogleSheetsSync()
        {
            if (_isCheckingGoogleSheets) return; _isCheckingGoogleSheets = true;
            try
            {
                SetActivity("Google: Pobieranie arkuszy...");
                await UpdateGoogleSheetRowCountAsync();
                UpdateSyncStatus("Google Sheets", "OK", "Synchronizacja zakończona");
            }
            catch (Exception ex) { UpdateSyncStatus("Google Sheets", "Błąd", ex.Message); }
            finally { _isCheckingGoogleSheets = false; SetActivity(""); }
        }

        private async Task RunReturnsSync()
        {
            if (_isCheckingReturns) return; _isCheckingReturns = true;
            try
            {
                SetActivity("Zwroty: Sprawdzanie bazy...");
                int count = 0;
                using (var c = Database.GetNewOpenConnection())
                {
                    count = Convert.ToInt32(await new MySqlCommand("SELECT COUNT(*) FROM NiezarejestrowaneZwrotyReklamacyjne WHERE IFNULL(CzyZarejestrowane,0)=0", c).ExecuteScalarAsync());
                }
                SafeInvoke(() => { btnNewReturn.Text = $"↩️ Nowe Zwroty ({count})"; });
                UpdateSyncStatus("Magazyn Zwrotów", "OK", $"Znaleziono {count} nowych");
            }
            catch (Exception ex) { UpdateSyncStatus("Magazyn Zwrotów", "Błąd", ex.Message); }
            finally { _isCheckingReturns = false; SetActivity(""); }
        }

        private async Task GenerateAutomaticRemindersAsync()
        {
            if (_isCheckingReminders) return; _isCheckingReminders = true;
            try
            {
                SetActivity("Przypomnienia: Analiza terminów...");
                bool refresh = await ReminderService.GenerateAutomaticRemindersAsync(10, 3);
                if (refresh) { HandleUpdateNeeded(); await RebuildRemindersCardsAsync(); }
                UpdateSyncStatus("Przypomnienia", "OK", "Przeanalizowano terminy");
            }
            catch (Exception ex) { UpdateSyncStatus("Przypomnienia", "Błąd", ex.Message); }
            finally { _isCheckingReminders = false; SetActivity(""); }
        }

        private async Task CheckForLogChanges()
        {
            if (_isCheckingLogs) return; _isCheckingLogs = true;
            try
            {
                long maxId = 0;
                using (var c = Database.GetNewOpenConnection()) maxId = Convert.ToInt64(await new MySqlCommand("SELECT MAX(Id) FROM Dziennik", c).ExecuteScalarAsync());
                if (maxId > _lastLogId) { _lastLogId = maxId; HandleUpdateNeeded(); }
                UpdateSyncStatus("Dziennik", "OK", "Bieżący");
            }
            catch { }
            finally { _isCheckingLogs = false; }
        }

        // --- DANE POMOCNICZE ---

        private async Task UpdateGoogleSheetRowCountAsync()
        {
            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream("reklamacje-baza-c36d05b0ffdb.json", FileMode.Open, FileAccess.Read))
                    credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
                var service = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = credential });
                int total = 0;
                foreach (var s in GoogleSheetsToRead)
                {
                    var r = await service.Spreadsheets.Values.Get(GoogleSpreadsheetId, s + "!A:A").ExecuteAsync();
                    if (r.Values != null) total += r.Values.Count;
                }
                int count = Math.Max(0, total - 2);
                SafeInvoke(() => {
                    btnNewGoogle.Text = $"🟢 Nowe Google ({count})";
                    _lastGoogleSheetRows = count;
                });
            }
            catch { throw; }
        }

        private async Task LoadChangeLogAsync()
        {
            string q = "SELECT DATE_FORMAT(Data, '%d-%m %H:%i') AS Kiedy, Akcja AS Zdarzenie, Uzytkownik, DotyczyZgloszenia AS NrZgloszenia FROM Dziennik ORDER BY Id DESC LIMIT 100";
            await LoadTableDataAsync(dataGridViewChangeLog, q);
            SafeInvoke(() => {
                if (dataGridViewChangeLog.Columns.Contains("NrZgloszenia")) dataGridViewChangeLog.Columns["NrZgloszenia"].Visible = false;
                if (dataGridViewChangeLog.Columns.Contains("Kiedy")) dataGridViewChangeLog.Columns["Kiedy"].Width = 90;
                if (dataGridViewChangeLog.Columns.Contains("Uzytkownik")) dataGridViewChangeLog.Columns["Uzytkownik"].Width = 80;
                if (dataGridViewChangeLog.Columns.Contains("Zdarzenie")) dataGridViewChangeLog.Columns["Zdarzenie"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            });
        }

        private async Task CheckForDueRemindersAndPopup()
        {
            if (_isCheckingPopups || this.IsDisposed || !this.IsHandleCreated) return;
            _isCheckingPopups = true;
            try
            {
                string sql = @"SELECT * FROM Przypomnienia WHERE (Status = 'Nowe' OR Status = 'Active' OR Status IS NULL OR Status = '') AND DataPrzypomnienia <= NOW() AND (PrzypisanyUzytkownik = @user OR PrzypisanyUzytkownik IS NULL OR PrzypisanyUzytkownik = '')";
                DataTable dt;
                using (var con = Database.GetNewOpenConnection())
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@user", _fullName);
                    using (var adapter = new MySqlDataAdapter(cmd)) { dt = new DataTable(); adapter.Fill(dt); }
                }

                var dueReminders = new List<DataRow>();
                foreach (DataRow row in dt.Rows)
                {
                    if (_shownReminders.Contains(Convert.ToInt32(row["Id"]))) continue;
                    dueReminders.Add(row);
                }
                if (dueReminders.Count == 0) return;

                if (dueReminders.Count > 3)
                {
                    SafeInvoke(() => { new FormPrzypomnieniePopup(0, $"Masz {dueReminders.Count} zaległych powiadomień!", "Wiele zgłoszeń", "Wysoki").Show(); });
                    foreach (var r in dueReminders) _shownReminders.Add(Convert.ToInt32(r["Id"]));
                }
                else
                {
                    foreach (var row in dueReminders)
                    {
                        int id = Convert.ToInt32(row["Id"]);
                        SafeInvoke(() => { new FormPrzypomnieniePopup(id, row["Tresc"].ToString(), row["DotyczyZgloszenia"]?.ToString(), row["Priorytet"]?.ToString()).Show(); });
                        _shownReminders.Add(id);
                        await Task.Delay(200);
                    }
                }
            }
            catch { }
            finally { _isCheckingPopups = false; }
        }

        // --- UI HELPERS ---

        private void SafeInvoke(MethodInvoker action) { if (!this.IsDisposed && this.IsHandleCreated) this.BeginInvoke(action); }

        private void UpdateSyncStatus(string service, string status, string details)
        {
            lock (_syncStatus)
            {
                _syncStatus[service] = $"{service}: {status} ({DateTime.Now:HH:mm})" + (string.IsNullOrEmpty(details) ? "" : $" -> {details}");
            }
            ApplySyncStatusTooltip();
        }

        private void ApplySyncStatusTooltip()
        {
            SafeInvoke(() =>
            {
                string[] lines;
                lock (_syncStatus) { lines = _syncStatus.Values.OrderBy(v => v).ToArray(); }

                bool anyError = lines.Any(l => l.Contains("Błąd"));
                lblSyncStatus.Text = anyError ? "Synchronizacja: BŁĄD" : "Synchronizacja: OK";
                lblSyncStatus.ForeColor = anyError ? Color.Red : Color.ForestGreen;

                string fullText = "Status usług:\n\n" + string.Join("\n", lines);
                if (_statusTooltip.GetToolTip(lblSyncStatus) != fullText)
                    _statusTooltip.SetToolTip(lblSyncStatus, fullText);
            });
        }

        private void RequestDataReload()
        {
            IWin32Window owner = this.FindForm();
            if (owner == null)
            {
                owner = this;
            }
            LoadDataAsync().FireAndForgetSafe(owner);
        }

        private void HandleUpdateNeeded() => SafeInvoke(RequestDataReload);
        private void refreshIcon_Click(object sender, EventArgs e) => RequestDataReload();
        private void lblLastRefresh_Click(object sender, EventArgs e) => RequestDataReload();

        // --- ZAKŁADKI PRZYPOMNIEŃ ---

        private void BuildRemindersTabsBar()
        {
            // 1. Czyścimy panel
            remindersTabsBar.Controls.Clear();

            // 2. Tworzymy przyciski
            _tabDecyzjaBtn = CreateTabButton("Czas na decyzję", (s, e) => SetActiveReminderTab("Czas na decyzję"));
            _tabKurierBtn = CreateTabButton("Kurier", (s, e) => SetActiveReminderTab("Kurier"));
            _tabReczneBtn = CreateTabButton("Ręczne", (s, e) => SetActiveReminderTab("Ręczne"));

            // 3. Dodajemy je do panelu (KLUCZOWE: najpierw dodaj, żeby system policzył rozmiar)
            remindersTabsBar.Controls.Add(_tabDecyzjaBtn);
            remindersTabsBar.Controls.Add(_tabKurierBtn);
            remindersTabsBar.Controls.Add(_tabReczneBtn);

            // 4. Układamy je obok siebie (teraz Width będzie poprawny)
            int currentX = 5;  // Margines od lewej krawędzi
            int gap = 10;      // Odstęp między przyciskami
            int topY = 3;      // Odstęp od góry

            // Przycisk 1
            _tabDecyzjaBtn.Location = new Point(currentX, topY);
            currentX += _tabDecyzjaBtn.Width + gap;

            // Przycisk 2
            _tabKurierBtn.Location = new Point(currentX, topY);
            currentX += _tabKurierBtn.Width + gap;

            // Przycisk 3
            _tabReczneBtn.Location = new Point(currentX, topY);

            // 5. Inicjalizacja menu kontekstowego i domyślnej zakładki
            _reminderCardCtx = new ContextMenuStrip();
            _reminderCardCtx.Items.Add("✅ Oznacz jako wykonane", null, async (s, e) => await MarkSelectedCardDoneAsync());

            SetActiveReminderTab("Czas na decyzję");
        }

        private Button CreateTabButton(string t, EventHandler click)
        {
            var b = new Button
            {
                Text = t,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink, // Przycisk dopasuje się do treści + Paddingu
                Padding = new Padding(12, 5, 12, 5),       // Wewnętrzny margines (żeby tekst nie dotykał krawędzi)
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Height = 30,
                Cursor = Cursors.Hand,
                BackColor = Color.WhiteSmoke,
                ForeColor = Color.Gray,
                UseVisualStyleBackColor = false
            };
            b.Click += click;
            return b;
        }

        private void SetActiveReminderTab(string cat)
        {
            _remindersActiveCategory = cat;

            // Aktualizacja wyglądu przycisków (Podświetlenie aktywnego)
            HighlightTab(_tabDecyzjaBtn, cat == "Czas na decyzję");
            HighlightTab(_tabKurierBtn, cat == "Kurier");
            HighlightTab(_tabReczneBtn, cat == "Ręczne");

            RebuildRemindersCardsAsync();
        }

        private void HighlightTab(Button b, bool isActive)
        {
            if (isActive)
            {
                b.BackColor = Color.FromArgb(21, 101, 192); // Niebieski aktywny
                b.ForeColor = Color.White;
            }
            else
            {
                b.BackColor = Color.WhiteSmoke; // Szary nieaktywny
                b.ForeColor = Color.Gray;
            }
        }

       

        private async Task RebuildRemindersCardsAsync()
        {
            try
            {
                // Zatrzymujemy rysowanie, żeby nie migało przy odświeżaniu
                flowLayoutPanelReminders.SuspendLayout();

                // Usuwamy stare karty (ważne, żeby wyczyścić listę przed dodaniem nowych)
                // Uwaga: W WinForms przy dużej liczbie kontrolek warto je też Dispose(), 
                // ale przy kilkunastu przypomnieniach Clear() wystarczy.
                foreach (Control ctrl in flowLayoutPanelReminders.Controls) ctrl.Dispose();
                flowLayoutPanelReminders.Controls.Clear();

                // Pobieramy dane z bazy
                var reminders = await ReminderService.GetActiveRemindersAsync();

                foreach (var r in reminders)
                {
                    // 1. FILTROWANIE ZAKŁADEK
                    // Sprawdzamy, czy przypomnienie pasuje do aktualnie wybranej zakładki (np. "Kurier")
                    if (ClassifyCategoryForCard(r.Tresc) != _remindersActiveCategory) continue;

                    // 2. TWORZENIE KARTY
                    var c = new StandardReminderCard
                    {
                        ReminderId = r.Id,
                        ReminderText = r.Tresc,
                        ComplaintNumber = r.DotyczyZgloszenia ?? ""
                    };

                    // 3. KOLORYZACJA (LOGIKA BIZNESOWA)
                    string textUpper = r.Tresc.ToUpper();

                    if (textUpper.Contains("[PROBLEM]") ||
                        textUpper.Contains("[ZWROT]") ||
                        textUpper.Contains("[ZGUBIONA]"))
                    {
                        c.IndicatorColor = Color.IndianRed; // Czerwony (Problemy krytyczne)
                    }
                    else if (textUpper.Contains("[PRZESYŁKA]") ||
                             textUpper.Contains("[W DORĘCZENIU]") ||
                             textUpper.Contains("DORĘCZENIU"))
                    {
                        c.IndicatorColor = Color.CornflowerBlue; // Niebieski (Info logistyczne)
                    }
                    else if (textUpper.StartsWith("[AUTO]") ||
                             textUpper.Contains("PILNE") ||
                             textUpper.Contains("TERMIN"))
                    {
                        c.IndicatorColor = Color.Orange; // Pomarańczowy (Terminy / Auto)
                    }
                    else
                    {
                        c.IndicatorColor = Color.LightGray; // Szary (Zwykłe ręczne)
                    }

                    // 4. PODPIĘCIE ZDARZEŃ
                    c.ContextMenuStrip = _reminderCardCtx; // Menu pod prawym przyciskiem
                    c.Tag = r.Id; // Przechowujemy ID, żeby wiedzieć co usunąć

                    // Kliknięcie w "ZOBACZ" otwiera Form2 ze zgłoszeniem
                    c.GoToComplaintClicked += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(c.ComplaintNumber))
                            new Form2(c.ComplaintNumber).Show();
                    };

                    // Dodajemy kartę do panelu
                    flowLayoutPanelReminders.Controls.Add(c);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd odświeżania przypomnień: " + ex.Message);
            }
            finally
            {
                // Wznawiamy rysowanie
                flowLayoutPanelReminders.ResumeLayout();
            }
        }

        // Metoda pomocnicza decydująca, do której zakładki trafi przypomnienie


        private static string ClassifyCategoryForCard(string t)
        {
            if (string.IsNullOrEmpty(t)) return "Ręczne";
            t = t.ToUpper();

            // 1. Kategoria: KURIER (Priorytet dla problemów transportowych)
            if (t.Contains("[PROBLEM]") ||
                t.Contains("[ZWROT]") ||
                t.Contains("[ZGUBIONA]") ||
                t.Contains("[PRZESYŁKA]") ||
                t.Contains("[W DORĘCZENIU]") ||
                t.Contains("DPD") ||
                t.Contains("KURIER"))
            {
                return "Kurier";
            }

            // 2. Kategoria: CZAS NA DECYZJĘ (Automaty terminowe)
            if (t.StartsWith("[AUTO]") ||
                t.Contains("PILNE") ||
                t.Contains("TERMIN") ||
                t.Contains("DECYZJ"))
            {
                return "Czas na decyzję";
            }

            // 3. Reszta to RĘCZNE
            return "Ręczne";
        }

        private async Task MarkSelectedCardDoneAsync() { if (_reminderCardCtx.SourceControl is Control c) { await ReminderService.MarkAsDoneAsync(Convert.ToInt64(c.Tag)); await RebuildRemindersCardsAsync(); } }

        // --- GRID EVENTS ---

        private async Task LoadTableDataAsync(DataGridView dgv, string q)
        {
            var t = new DataTable();
            try { using (var c = Database.GetNewOpenConnection()) using (var a = new MySqlDataAdapter(q, c)) await Task.Run(() => a.Fill(t)); SafeInvoke(() => dgv.DataSource = t); } catch { }
        }

        private void anyDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && sender is DataGridView dgv && dgv.Columns.Contains("NrZgloszenia"))
            {
                string nr = dgv.Rows[e.RowIndex].Cells["NrZgloszenia"].Value?.ToString();
                if (!string.IsNullOrEmpty(nr)) new Form2(nr).Show();
            }
        }

        // --- MENU CONTEXT ACTIONS ---

        private void otwórzZgłoszenieToolStripMenuItem_Click(object sender, EventArgs e) { if (dataGridViewProcessing.CurrentRow != null) anyDataGridView_CellClick(dataGridViewProcessing, new DataGridViewCellEventArgs(0, dataGridViewProcessing.CurrentRow.Index)); }

        private async void usunZgloszenieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcessing.CurrentRow == null) return;
            string nrZgloszenia = dataGridViewProcessing.CurrentRow.Cells["NrZgloszenia"].Value.ToString();
            var result = MessageBox.Show($"Czy na pewno chcesz przenieść zgłoszenie {nrZgloszenia} do archiwum?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) { await ArchiveComplaintAsync(nrZgloszenia); RequestDataReload(); }
        }

        private void kopiujNumerZgłoszeniaToolStripMenuItem_Click(object sender, EventArgs e) { if (dataGridViewProcessing.CurrentRow != null) Clipboard.SetText(dataGridViewProcessing.CurrentRow.Cells["NrZgloszenia"].Value.ToString()); }

        private async void dodajPrzypomnienieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcessing.CurrentRow == null) return;
            string nr = dataGridViewProcessing.CurrentRow.Cells["NrZgloszenia"].Value.ToString();
            try
            {
                int id = 0;
                using (var c = Database.GetNewOpenConnection())
                {
                    var s = await new MySqlCommand("SELECT Id FROM Zgloszenia WHERE NrZgloszenia='" + nr + "'", c).ExecuteScalarAsync();
                    if (s != null) id = Convert.ToInt32(s);
                }
                if (id > 0) new FormDodajPrzypomnienie(id).Show();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }

        private async Task ArchiveComplaintAsync(string complaintNumber)
        {
            using (var c = Database.GetNewOpenConnection()) using (var t = c.BeginTransaction())
            {
                try
                {
                    using (var cmd = new MySqlCommand("INSERT INTO ZgloszeniaArchiwum SELECT * FROM Zgloszenia WHERE NrZgloszenia=@n", c, t)) { cmd.Parameters.AddWithValue("@n", complaintNumber); await cmd.ExecuteNonQueryAsync(); }
                    using (var cmd = new MySqlCommand("DELETE FROM Zgloszenia WHERE NrZgloszenia=@n", c, t)) { cmd.Parameters.AddWithValue("@n", complaintNumber); await cmd.ExecuteNonQueryAsync(); }
                    t.Commit();
                    ToastManager.ShowToast("Sukces", "Zarchiwizowano.", NotificationType.Success);
                }
                catch { t.Rollback(); }
            }
        }

        private void ReklamacjeControl_Disposed(object sender, EventArgs e)
        {
            StopAndDisposeTimer(_logCheckTimer); StopAndDisposeTimer(_googleSheetSyncTimer); StopAndDisposeTimer(_allegroSyncTimer);
            StopAndDisposeTimer(_remindersCheckTimer); StopAndDisposeTimer(_shipmentCheckTimer); StopAndDisposeTimer(_returnsSyncTimer); StopAndDisposeTimer(_popupCheckTimer); StopAndDisposeTimer(_emailSyncTimer);
            if (_privateWebView != null) _privateWebView.Dispose();
        }
        private static void StopAndDisposeTimer(System.Timers.Timer t) { if (t != null) { t.Stop(); t.Dispose(); } }
        private void EnsureProcessingGridScrollable() { try { dataGridViewProcessing.ScrollBars = ScrollBars.Both; typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(dataGridViewProcessing, true, null); } catch { } }
        private void txtFilterProcessing_TextChanged(object sender, EventArgs e) { if (dataGridViewProcessing.DataSource is DataTable dt) { string f = txtFilterProcessing.Text.Replace("'", "''"); dt.DefaultView.RowFilter = string.IsNullOrWhiteSpace(f) ? "" : $"NrZgloszenia LIKE '%{f}%' OR Klient LIKE '%{f}%'"; } }
        private void HighlightMenuButton(object sender) { foreach (Control c in pnlMenuButtons.Controls) if (c is Button b) { b.BackColor = Color.FromArgb(21, 32, 54); b.ForeColor = Color.FromArgb(180, 190, 210); } if (sender is Button btn) { btn.BackColor = Color.FromArgb(30, 41, 59); btn.ForeColor = Color.White; } }
        private void menuStronaGlowna_Click(object sender, EventArgs e) { HighlightMenuButton(sender); RequestDataReload(); }
        private void menuNiezarejestrowaneGoogle_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormUniversalWizardV2(WizardSource.GoogleSheet).Show(); }
        private void menuNiezarejestrowaneAllegro_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormUniversalWizardV2(WizardSource.Allegro).Show(); }
        private void menuDodajNowe_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormUniversalWizardV2(WizardSource.Manual).Show(); }
        private void menuWszystkieZgloszenia_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new WyszukiwarkaZgloszenForm().Show(); }
        private void menuCzatAllegro_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormWiadomosci().Show(); }
        private void btnContactCenter_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormHistoria().Show();  }
       
    private void menuPrzypomnienia_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormPrzypomnienia().Show(); }
        private void menuKlienci_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new Form3().Show(); }
        private void menuProdukty_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new Form15("1").Show(); }
        private void menuProducenci_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new Form16().Show(); }
        private void menuUstawienia_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormUstawienia().Show(); }
        private void menuSledzeniePrzesylek_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormDpdTracking().Show(); }
        private void menuNiezarejestrowaneZwroty_Click(object sender, EventArgs e) { HighlightMenuButton(sender); try { new FormUniversalWizardV2(WizardSource.Zwroty).Show(); } catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); } }
    }
}
