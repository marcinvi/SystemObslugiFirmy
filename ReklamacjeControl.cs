using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// ReklamacjeControl v2.2 — DB-ONLY DASHBOARD
    /// 
    /// DANE (zgłoszenia, przypomnienia, dziennik) → bezpośrednio z MySQL (szybko, niezawodnie)
    /// SYNCHRONIZACJA (Allegro, Google, DPD) → po stronie API (BackgroundServices)
    /// DASHBOARD WinForms → czyta wyłącznie MySQL (bez autoryzacji API na stacjach)
    /// 
    /// Dzięki temu 50 użytkowników nie zapycha API Google/Allegro/DPD,
    /// a stacje robocze działają nawet bez logowania do API.
    /// </summary>
    public partial class ReklamacjeControl : UserControl
    {
        // === TIMERY ===
        private readonly ToolTip _statusTooltip = new ToolTip();
        private System.Timers.Timer _popupCheckTimer;
        private System.Timers.Timer _logCheckTimer;
        private System.Timers.Timer _syncStatusTimer;      // Odpytuje DB o statusy sync (Allegro/Google/DPD)
        private System.Timers.Timer _emailSyncTimer;
        private System.Timers.Timer _remindersCheckTimer;
        private System.Timers.Timer _returnsSyncTimer;

        // === FLAGI ===
        private volatile bool _isCheckingPopups = false;
        private volatile bool _isCheckingLogs = false;
        private volatile bool _isCheckingSyncStatus = false;
        private volatile bool _isCheckingEmails = false;
        private volatile bool _isCheckingReminders = false;
        private volatile bool _isCheckingReturns = false;
        private int _isLoadingData;
        private int _pendingLoad;

        // === DANE ===
        private long _lastLogId = 0;
        private readonly HashSet<int> _shownReminders = new HashSet<int>();
        private readonly Dictionary<string, string> _syncStatus = new Dictionary<string, string>();

        // === SERWISY ===
        private readonly EmailService _emailService;
        private ShipmentNotificationService _shipmentNotificationService;
        private readonly string _fullName;
        private readonly string _userRole;
        private WebView2 _privateWebView;

        // === UI ===
        private Button _tabDecyzjaBtn, _tabKurierBtn, _tabReczneBtn;
        private string _remindersActiveCategory = "Czas na decyzję";
        private ContextMenuStrip _reminderCardCtx;

        public ReklamacjeControl(string fullName, string userRole)
        {
            InitializeComponent();

            _fullName = fullName;
            _userRole = userRole;
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
                HighlightMenuButton(s);
                new FormHistoria().Show();
            };
            btnRefresh.Click += refreshIcon_Click;
            lblLastRefresh.Click += lblLastRefresh_Click;

            // Inicjalizacja UI
            BuildRemindersTabsBar();
            InitializeSyncStatuses();
            InitializeMenuCounters();

            txtFilterProcessing.TextChanged += txtFilterProcessing_TextChanged;
            dataGridViewProcessing.CellDoubleClick += anyDataGridView_CellClick;
            dataGridViewChangeLog.CellDoubleClick += anyDataGridView_CellClick;

            _statusTooltip.AutoPopDelay = 15000;
            _statusTooltip.InitialDelay = 500;
            _statusTooltip.ReshowDelay = 500;
        }

        // =====================================================================
        // INICJALIZACJA
        // =====================================================================

        private void InitializeSyncStatuses()
        {
            UpdateSyncStatus("Allegro", "Oczekiwanie...", "");
            UpdateSyncStatus("E-mail", "Oczekiwanie...", "");
            UpdateSyncStatus("Google Sheets", "Oczekiwanie...", "");
            UpdateSyncStatus("Przesyłki DPD", "Oczekiwanie...", "");
            UpdateSyncStatus("Magazyn Zwrotów", "Oczekiwanie...", "");
            UpdateSyncStatus("Przypomnienia", "Oczekiwanie...", "");
            UpdateSyncStatus("Dziennik", "Oczekiwanie...", "");
        }

        private void SetActivity(string text)
        {
            SafeInvoke(() =>
            {
                if (lblSyncActivity != null)
                {
                    lblSyncActivity.Text = text;
                    lblSyncActivity.ForeColor = text.ToLower().Contains("błąd") ? Color.Red : Color.SteelBlue;
                }
            });
        }

        private async void ReklamacjeControl_Load(object sender, EventArgs e)
        {
            _privateWebView = new WebView2 { Visible = false };
            this.Controls.Add(_privateWebView);
            _ = _privateWebView.EnsureCoreWebView2Async(null);

            _shipmentNotificationService = new ShipmentNotificationService(this.FindForm(), _privateWebView);

            try { await ReminderService.InitializeAsync(); } catch { }

            ReklamacjeControl_Resize(null, null);

            // Ładowanie danych z MySQL — ZAWSZE działa
            RequestDataReload();

            // Timery
            InitializeTimers();

            // Zadania tła z opóźnieniem
            Task.Delay(2000).ContinueWith(_ => RunEmailSync().FireAndForgetSafe(this));
            Task.Delay(3000).ContinueWith(_ => PollLocalSyncStatusFromDb().FireAndForgetSafe(this));
            Task.Delay(4000).ContinueWith(_ => PollCountersFromDb().FireAndForgetSafe(this));
            Task.Delay(5000).ContinueWith(_ => GenerateAutomaticRemindersAsync().FireAndForgetSafe(this));
        }

        private void InitializeTimers()
        {
            // Dane z MySQL — niezawodne, zawsze działają
            _logCheckTimer = NewTimer(10000, async () => await CheckForLogChanges());
            _remindersCheckTimer = NewTimer(3600000, async () => await GenerateAutomaticRemindersAsync());
            _returnsSyncTimer = NewTimer(60000, async () => await PollReturnsCountFromDb());
            _popupCheckTimer = NewTimer(60000, async () => await CheckForDueRemindersAndPopup());
            _emailSyncTimer = NewTimer(120000, async () => await RunEmailSync());

            // Statusy i liczniki z API — gdy API dostępne
            _syncStatusTimer = NewTimer(30000, async () => await PollSyncStatusAndCounters());
        }

        private static System.Timers.Timer NewTimer(double interval, Func<Task> callback)
        {
            var t = new System.Timers.Timer(interval) { AutoReset = true };
            t.Elapsed += async (s, ev) => await callback();
            t.Start();
            return t;
        }

        // =====================================================================
        // ŁADOWANIE DANYCH — BEZPOŚREDNIO Z MySQL (NIEZAWODNE)
        // =====================================================================

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

                var task1 = LoadProcessingCasesAsync();
                var task2 = RebuildRemindersCardsAsync();
                var task3 = LoadChangeLogAsync();
                var task4 = UpdateAllegroChatUnreadCountAsync();
                var task5 = UpdateReminderNotificationsCountAsync();

                await Task.WhenAll(task1, task2, task3, task4, task5);

                SafeInvoke(() => lblLastRefresh.Text = "Odświeżono: " + DateTime.Now.ToString("HH:mm"));
            }
            catch (Exception ex)
            {
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

        private async Task UpdateAllegroChatUnreadCountAsync()
        {
            try
            {
                int count = 0;
                using (var con = Database.GetNewOpenConnection())
                {
                    var cmd = new MySqlCommand("SELECT COUNT(*) FROM AllegroDisputes WHERE HasNewMessages = 1", con);
                    count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                SafeInvoke(() =>
                {
                    btnChat.Text = $"💬 Czat Allegro ({count})";
                    btnChat.ForeColor = count > 0 ? Color.Orange : Color.FromArgb(180, 190, 210);
                });
            }
            catch { }
        }

        private async Task LoadChangeLogAsync()
        {
            string q = "SELECT DATE_FORMAT(Data, '%d-%m %H:%i') AS Kiedy, Akcja AS Zdarzenie, Uzytkownik, DotyczyZgloszenia AS NrZgloszenia FROM Dziennik ORDER BY Id DESC LIMIT 100";
            await LoadTableDataAsync(dataGridViewChangeLog, q);
            SafeInvoke(() =>
            {
                if (dataGridViewChangeLog.Columns.Contains("NrZgloszenia")) dataGridViewChangeLog.Columns["NrZgloszenia"].Visible = false;
                if (dataGridViewChangeLog.Columns.Contains("Kiedy")) dataGridViewChangeLog.Columns["Kiedy"].Width = 90;
                if (dataGridViewChangeLog.Columns.Contains("Uzytkownik")) dataGridViewChangeLog.Columns["Uzytkownik"].Width = 80;
                if (dataGridViewChangeLog.Columns.Contains("Zdarzenie")) dataGridViewChangeLog.Columns["Zdarzenie"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            });
        }

        private async Task LoadTableDataAsync(DataGridView dgv, string q)
        {
            var t = new DataTable();
            try
            {
                using (var c = Database.GetNewOpenConnection())
                using (var a = new MySqlDataAdapter(q, c))
                    await Task.Run(() => a.Fill(t));
                SafeInvoke(() => dgv.DataSource = t);
            }
            catch { }
        }

        // =====================================================================
        // SYNC STATUS I LICZNIKI — Z BAZY (DB-ONLY dashboard)
        // API działa centralnie i zapisuje dane do DB,
        // a stanowiska odczytują wyłącznie bazę.
        // =====================================================================

        /// <summary>
        /// Pobiera liczniki bezpośrednio z bazy (tryb DB-only dashboardu).
        /// Wywoływane raz przy starcie z opóźnieniem.
        /// Potem aktualizowane przez PollLocalSyncStatusAndCounters co 30s.
        /// </summary>
        private async Task PollCountersFromDb()
        {
            try
            {
                // Priorytet: API sync-dashboard (aktualne liczniki niezarejestrowanych zgłoszeń).
                // Fallback: bezpośrednie zapytania SQL (tryb DB-only).
                var updatedFromApi = await TryUpdateCountersFromApiAsync();
                if (!updatedFromApi)
                {
                    await UpdateCountersFromDatabaseFallbackAsync();
                    await UpdateGoogleCountFromSyncRunsAsync();
                }

                // Powiadomienia o przypomnieniach są lokalne i niezależne od backendu sync
                await UpdateReminderNotificationsCountAsync();
            }
            catch
            {
                await UpdateCountersFromDatabaseFallbackAsync();
                await UpdateReminderNotificationsCountAsync();
            }
        }

        private async Task<bool> TryUpdateCountersFromApiAsync()
        {
            try
            {
                var baseUrl = (Properties.Settings.Default.ApiBaseUrl ?? string.Empty).Trim();
                var token = (Properties.Settings.Default.ApiToken ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(token))
                    return false;

                var client = new ReklamacjeApiClient(baseUrl);
                client.SetToken(token);
                var counters = await client.GetSyncDashboardCountersAsync();

                SafeInvoke(() =>
                {
                    btnNewGoogle.Text = $"🟢 Nowe Google ({Math.Max(0, counters.UnregisteredGoogleCount)})";
                    btnNewAllegro.Text = $"🟠 Nowe Allegro ({Math.Max(0, counters.UnregisteredAllegroCount)})";
                    btnChat.Text = $"💬 Czat Allegro ({Math.Max(0, counters.AllegroNewMessages)})";
                    btnChat.ForeColor = counters.AllegroNewMessages > 0 ? Color.Orange : Color.FromArgb(180, 190, 210);
                    btnNewReturn.Text = $"↩️ Nowe Zwroty ({Math.Max(0, counters.UnregisteredReturnsCount)})";
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task UpdateCountersFromDatabaseFallbackAsync()
        {
            await UpdateAllegroUnregisteredCountFromDbAsync();
            await UpdateAllegroChatUnreadCountAsync();
            await PollReturnsCountFromDb();
        }

        private async Task UpdateAllegroUnregisteredCountFromDbAsync()
        {
            try
            {
                int count = 0;
                using (var con = Database.GetNewOpenConnection())
                {
                    var sql = @"SELECT COUNT(*) FROM AllegroDisputes WHERE IFNULL(CzyZarejestrowane, 0) = 0";
                    using (var cmd = new MySqlCommand(sql, con))
                        count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                SafeInvoke(() => { btnNewAllegro.Text = $"🟠 Nowe Allegro ({count})"; });
            }
            catch
            {
                SafeInvoke(() => { btnNewAllegro.Text = "🟠 Nowe Allegro (0)"; });
            }
        }

        private async Task PollLocalSyncStatusFromDb()
        {
            if (_isCheckingSyncStatus) return;
            _isCheckingSyncStatus = true;
            try
            {
                await UpdateServiceStatusFromSyncRunsAsync("ALLEGRO", "Allegro");
                await UpdateServiceStatusFromSyncRunsAsync("GOOGLE", "Google Sheets");
                await UpdateServiceStatusFromSyncRunsAsync("DPD", "Przesyłki DPD");
            }
            catch (Exception ex)
            {
                UpdateSyncStatus("Allegro", "Błąd", ex.Message);
                UpdateSyncStatus("Google Sheets", "Błąd", ex.Message);
                UpdateSyncStatus("Przesyłki DPD", "Błąd", ex.Message);
            }
            finally { _isCheckingSyncStatus = false; }
        }

        private async Task UpdateServiceStatusFromSyncRunsAsync(string source, string uiName)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand(@"SELECT started_at, finished_at, ok, rows_written, error_message
                                               FROM SyncRuns
                                               WHERE source = @source
                                               ORDER BY started_at DESC
                                               LIMIT 1", con))
            {
                cmd.Parameters.AddWithValue("@source", source);
                using (var rd = await cmd.ExecuteReaderAsync())
                {
                    if (!await rd.ReadAsync())
                    {
                        UpdateSyncStatus(uiName, "Brak danych", "Brak wpisów w SyncRuns");
                        return;
                    }

                    var finishedAt = rd.IsDBNull(1) ? (DateTime?)null : rd.GetDateTime(1);
                    bool ok = !rd.IsDBNull(2) && rd.GetInt32(2) == 1;
                    int written = rd.IsDBNull(3) ? 0 : rd.GetInt32(3);
                    string error = rd.IsDBNull(4) ? "" : rd.GetString(4);

                    if (finishedAt == null)
                    {
                        UpdateSyncStatus(uiName, "Trwa...", "Synchronizacja w toku");
                    }
                    else if (ok)
                    {
                        UpdateSyncStatus(uiName, "OK", $"Ostatnio zapisano: {written}");
                    }
                    else
                    {
                        UpdateSyncStatus(uiName, "Błąd", string.IsNullOrWhiteSpace(error) ? "Sprawdź wpisy w SyncRuns" : error);
                    }
                }
            }
        }

        private async Task UpdateGoogleCountFromSyncRunsAsync()
        {
            try
            {
                int googleCount = 0;
                using (var con = Database.GetNewOpenConnection())
                {
                    const string sql = @"SELECT IFNULL(rows_written, 0)
                                         FROM SyncRuns
                                         WHERE source = 'GOOGLE' AND IFNULL(ok, 0) = 1
                                         ORDER BY started_at DESC
                                         LIMIT 1";
                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        var scalar = await cmd.ExecuteScalarAsync();
                        googleCount = scalar == null || scalar == DBNull.Value ? 0 : Convert.ToInt32(scalar);
                    }
                }

                SafeInvoke(() => { btnNewGoogle.Text = $"🟢 Nowe Google ({googleCount})"; });

                // Powiadomienia o przypomnieniach są lokalne i niezależne od API
                await UpdateReminderNotificationsCountAsync();
            }
            catch
            {
                await UpdateCountersFromDatabaseFallbackAsync();
                await UpdateReminderNotificationsCountAsync();
            }
        }

        private async Task PollSyncStatusAndCounters()
        {
            await PollLocalSyncStatusFromDb();
            await PollCountersFromDb();
        }

     

        private async Task PollReturnsCountFromDb()
        {
            if (_isCheckingReturns) return;
            _isCheckingReturns = true;
            try
            {
                int count = 0;
                using (var c = Database.GetNewOpenConnection())
                {
                    count = Convert.ToInt32(await new MySqlCommand(
                        "SELECT COUNT(*) FROM NiezarejestrowaneZwrotyReklamacyjne WHERE IFNULL(CzyZarejestrowane,0)=0", c)
                        .ExecuteScalarAsync());
                }
                SafeInvoke(() => { btnNewReturn.Text = $"↩️ Nowe Zwroty ({count})"; });
                UpdateSyncStatus("Magazyn Zwrotów", "OK", $"Znaleziono {count} nowych");
            }
            catch (Exception ex) { UpdateSyncStatus("Magazyn Zwrotów", "Błąd", ex.Message); }
            finally { _isCheckingReturns = false; }
        }

        // =====================================================================
        // EMAIL — LOKALNIE (licznik i synchronizacja skrzynki)
        // =====================================================================

        private async Task RunEmailSync()
        {
            if (_isCheckingEmails) return;
            _isCheckingEmails = true;
            try
            {
                SetActivity("E-mail: Pobieranie...");
                await _emailService.PobierzPoczteDlaWszystkichKontAsync();

                int count = 0;
                using (var con = Database.GetNewOpenConnection())
                {
                    string sql = "SELECT COUNT(*) FROM CentrumKontaktu WHERE Typ='Mail' AND Kierunek='IN' AND DataWyslania > DATE_SUB(NOW(), INTERVAL 1 DAY)";
                    count = Convert.ToInt32(await new MySqlCommand(sql, con).ExecuteScalarAsync());
                }

                SafeInvoke(() =>
                {
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

        // =====================================================================
        // PRZYPOMNIENIA
        // =====================================================================

        private async Task GenerateAutomaticRemindersAsync()
        {
            if (_isCheckingReminders) return;
            _isCheckingReminders = true;
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
            if (_isCheckingLogs) return;
            _isCheckingLogs = true;
            try
            {
                long maxId = 0;
                using (var c = Database.GetNewOpenConnection())
                    maxId = Convert.ToInt64(await new MySqlCommand("SELECT MAX(Id) FROM Dziennik", c).ExecuteScalarAsync());
                if (maxId > _lastLogId) { _lastLogId = maxId; HandleUpdateNeeded(); }
                UpdateSyncStatus("Dziennik", "OK", "Bieżący");
            }
            catch { }
            finally { _isCheckingLogs = false; }
        }

        private async Task CheckForDueRemindersAndPopup()
        {
            if (_isCheckingPopups || this.IsDisposed || !this.IsHandleCreated) return;
            _isCheckingPopups = true;
            try
            {
                string sql = @"SELECT * FROM Przypomnienia 
                    WHERE (Status = 'Nowe' OR Status = 'Active' OR Status IS NULL OR Status = '') 
                    AND DataPrzypomnienia <= NOW() 
                    AND (PrzypisanyUzytkownik = @user OR PrzypisanyUzytkownik IS NULL OR PrzypisanyUzytkownik = '')";

                DataTable dt;
                using (var con = Database.GetNewOpenConnection())
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@user", _fullName);
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        dt = new DataTable();
                        adapter.Fill(dt);
                    }
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

        private async Task RebuildRemindersCardsAsync()
        {
            try
            {
                flowLayoutPanelReminders.SuspendLayout();

                foreach (Control ctrl in flowLayoutPanelReminders.Controls) ctrl.Dispose();
                flowLayoutPanelReminders.Controls.Clear();

                var reminders = await ReminderService.GetActiveRemindersAsync();

                foreach (var r in reminders)
                {
                    if (ClassifyCategoryForCard(r.Tresc) != _remindersActiveCategory) continue;

                    var c = new StandardReminderCard
                    {
                        ReminderId = r.Id,
                        ReminderText = r.Tresc,
                        ComplaintNumber = r.DotyczyZgloszenia ?? ""
                    };

                    string textUpper = r.Tresc.ToUpper();
                    if (textUpper.Contains("[PROBLEM]") || textUpper.Contains("[ZWROT]") || textUpper.Contains("[ZGUBIONA]"))
                        c.IndicatorColor = Color.IndianRed;
                    else if (textUpper.Contains("[PRZESYŁKA]") || textUpper.Contains("[W DORĘCZENIU]") || textUpper.Contains("DORĘCZENIU"))
                        c.IndicatorColor = Color.CornflowerBlue;
                    else if (textUpper.StartsWith("[AUTO]") || textUpper.Contains("PILNE") || textUpper.Contains("TERMIN"))
                        c.IndicatorColor = Color.Orange;
                    else
                        c.IndicatorColor = Color.LightGray;

                    c.ContextMenuStrip = _reminderCardCtx;
                    c.Tag = r.Id;

                    c.GoToComplaintClicked += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(c.ComplaintNumber))
                            new Form2(c.ComplaintNumber).Show();
                    };

                    flowLayoutPanelReminders.Controls.Add(c);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd odświeżania przypomnień: " + ex.Message);
            }
            finally
            {
                flowLayoutPanelReminders.ResumeLayout();
            }
        }

        private async Task UpdateReminderNotificationsCountAsync()
        {
            try
            {
                string sql = @"SELECT COUNT(*) FROM Przypomnienia
                               WHERE (Status = 'Nowe' OR Status = 'Active' OR Status IS NULL OR Status = '')
                               AND DataPrzypomnienia <= NOW()
                               AND (PrzypisanyUzytkownik = @user OR PrzypisanyUzytkownik IS NULL OR PrzypisanyUzytkownik = '')";

                int count = 0;
                using (var con = Database.GetNewOpenConnection())
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@user", _fullName);
                    count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                SafeInvoke(() =>
                {
                    btnReminders.Text = $"⏰ Przypomnienia ({count})";
                    btnReminders.ForeColor = count > 0 ? Color.Orange : Color.FromArgb(180, 190, 210);
                });
            }
            catch { }
        }

        private static string ClassifyCategoryForCard(string t)
        {
            if (string.IsNullOrEmpty(t)) return "Ręczne";
            t = t.ToUpper();

            if (t.Contains("[PROBLEM]") || t.Contains("[ZWROT]") || t.Contains("[ZGUBIONA]") ||
                t.Contains("[PRZESYŁKA]") || t.Contains("[W DORĘCZENIU]") || t.Contains("DPD") || t.Contains("KURIER"))
                return "Kurier";

            if (t.StartsWith("[AUTO]") || t.Contains("PILNE") || t.Contains("TERMIN") || t.Contains("DECYZJ"))
                return "Czas na decyzję";

            return "Ręczne";
        }

        // =====================================================================
        // HELPERY DB-ONLY DASHBOARDU
        // =====================================================================

        // =====================================================================
        // UI HELPERS
        // =====================================================================

        private void SafeInvoke(MethodInvoker action)
        {
            if (!this.IsDisposed && this.IsHandleCreated) this.BeginInvoke(action);
        }

        private void UpdateSyncStatus(string service, string status, string details)
        {
            lock (_syncStatus)
            {
                _syncStatus[service] = $"{service}: {status} ({DateTime.Now:HH:mm})" +
                    (string.IsNullOrEmpty(details) ? "" : $" → {details}");
            }
            ApplySyncStatusTooltip();
        }

        private void ApplySyncStatusTooltip()
        {
            SafeInvoke(() =>
            {
                string[] lines;
                lock (_syncStatus) { lines = _syncStatus.Values.OrderBy(v => v).ToArray(); }

                bool anyProblem = lines.Any(l =>
                    l.IndexOf("Błąd", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    l.IndexOf("Brak autoryzacji", StringComparison.OrdinalIgnoreCase) >= 0);

                lblSyncStatus.Text = anyProblem ? "Synchronizacja: BŁĄD" : "Synchronizacja: OK";
                lblSyncStatus.ForeColor = anyProblem ? Color.Red : Color.ForestGreen;

                string fullText = "Status usług:\n\n" + string.Join("\n", lines);
                if (_statusTooltip.GetToolTip(lblSyncStatus) != fullText)
                    _statusTooltip.SetToolTip(lblSyncStatus, fullText);
            });
        }

        private void InitializeMenuCounters()
        {
            SafeInvoke(() =>
            {
                btnNewGoogle.Text = "🟢 Nowe Google (0)";
                btnNewAllegro.Text = "🟠 Nowe Allegro (0)";
                btnNewReturn.Text = "↩️ Nowe Zwroty (0)";
                btnChat.Text = "💬 Czat Allegro (0)";
                btnReminders.Text = "⏰ Przypomnienia (0)";
            });
        }

        private void RequestDataReload()
        {
            IWin32Window owner = this.FindForm();
            if (owner == null) owner = this;
            LoadDataAsync().FireAndForgetSafe(owner);
        }

        private void HandleUpdateNeeded() => SafeInvoke(RequestDataReload);
        private void refreshIcon_Click(object sender, EventArgs e) => RequestDataReload();
        private void lblLastRefresh_Click(object sender, EventArgs e) => RequestDataReload();

        // =====================================================================
        // ZAKŁADKI PRZYPOMNIEŃ
        // =====================================================================

        private void BuildRemindersTabsBar()
        {
            remindersTabsBar.Controls.Clear();

            _tabDecyzjaBtn = CreateTabButton("Czas na decyzję", (s, e) => SetActiveReminderTab("Czas na decyzję"));
            _tabKurierBtn = CreateTabButton("Kurier", (s, e) => SetActiveReminderTab("Kurier"));
            _tabReczneBtn = CreateTabButton("Ręczne", (s, e) => SetActiveReminderTab("Ręczne"));

            remindersTabsBar.Controls.Add(_tabDecyzjaBtn);
            remindersTabsBar.Controls.Add(_tabKurierBtn);
            remindersTabsBar.Controls.Add(_tabReczneBtn);

            int currentX = 5, gap = 10, topY = 3;
            _tabDecyzjaBtn.Location = new Point(currentX, topY); currentX += _tabDecyzjaBtn.Width + gap;
            _tabKurierBtn.Location = new Point(currentX, topY); currentX += _tabKurierBtn.Width + gap;
            _tabReczneBtn.Location = new Point(currentX, topY);

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
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 5, 12, 5),
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
            HighlightTab(_tabDecyzjaBtn, cat == "Czas na decyzję");
            HighlightTab(_tabKurierBtn, cat == "Kurier");
            HighlightTab(_tabReczneBtn, cat == "Ręczne");
            RebuildRemindersCardsAsync();
        }

        private void HighlightTab(Button b, bool isActive)
        {
            b.BackColor = isActive ? Color.FromArgb(21, 101, 192) : Color.WhiteSmoke;
            b.ForeColor = isActive ? Color.White : Color.Gray;
        }

        private async Task MarkSelectedCardDoneAsync()
        {
            if (_reminderCardCtx.SourceControl is Control c)
            {
                await ReminderService.MarkAsDoneAsync(Convert.ToInt64(c.Tag));
                await RebuildRemindersCardsAsync();
            }
        }

        // =====================================================================
        // GRID EVENTS
        // =====================================================================

        private void anyDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && sender is DataGridView dgv && dgv.Columns.Contains("NrZgloszenia"))
            {
                string nr = dgv.Rows[e.RowIndex].Cells["NrZgloszenia"].Value?.ToString();
                if (!string.IsNullOrEmpty(nr)) new Form2(nr).Show();
            }
        }

        // =====================================================================
        // MENU CONTEXT ACTIONS
        // =====================================================================

        private void otwórzZgłoszenieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcessing.CurrentRow != null)
                anyDataGridView_CellClick(dataGridViewProcessing, new DataGridViewCellEventArgs(0, dataGridViewProcessing.CurrentRow.Index));
        }

        private async void usunZgloszenieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcessing.CurrentRow == null) return;
            string nrZgloszenia = dataGridViewProcessing.CurrentRow.Cells["NrZgloszenia"].Value.ToString();
            var result = MessageBox.Show($"Czy na pewno chcesz przenieść zgłoszenie {nrZgloszenia} do archiwum?",
                "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                await ArchiveComplaintAsync(nrZgloszenia);
                RequestDataReload();
            }
        }

        private void kopiujNumerZgłoszeniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcessing.CurrentRow != null)
                Clipboard.SetText(dataGridViewProcessing.CurrentRow.Cells["NrZgloszenia"].Value.ToString());
        }

        private async void dodajPrzypomnienieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewProcessing.CurrentRow == null) return;
            string nr = dataGridViewProcessing.CurrentRow.Cells["NrZgloszenia"].Value.ToString();
            try
            {
                int id = 0;
                using (var c = Database.GetNewOpenConnection())
                {
                    using (var cmd = new MySqlCommand("SELECT Id FROM Zgloszenia WHERE NrZgloszenia = @nr", c))
                    {
                        cmd.Parameters.AddWithValue("@nr", nr);
                        var s = await cmd.ExecuteScalarAsync();
                        if (s != null) id = Convert.ToInt32(s);
                    }
                }
                if (id > 0) new FormDodajPrzypomnienie(id).Show();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }

        private async Task ArchiveComplaintAsync(string complaintNumber)
        {
            using (var c = Database.GetNewOpenConnection())
            using (var t = c.BeginTransaction())
            {
                try
                {
                    using (var cmd = new MySqlCommand("INSERT INTO ZgloszeniaArchiwum SELECT * FROM Zgloszenia WHERE NrZgloszenia=@n", c, t))
                    { cmd.Parameters.AddWithValue("@n", complaintNumber); await cmd.ExecuteNonQueryAsync(); }
                    using (var cmd = new MySqlCommand("DELETE FROM Zgloszenia WHERE NrZgloszenia=@n", c, t))
                    { cmd.Parameters.AddWithValue("@n", complaintNumber); await cmd.ExecuteNonQueryAsync(); }
                    t.Commit();
                    ToastManager.ShowToast("Sukces", "Zarchiwizowano.", NotificationType.Success);
                }
                catch { t.Rollback(); }
            }
        }

        // =====================================================================
        // RESIZE / DISPOSE
        // =====================================================================

        private void ReklamacjeControl_Resize(object sender, EventArgs e)
        {
            if (splitContainerBottom.Width > 0) splitContainerBottom.SplitterDistance = splitContainerBottom.Width / 2;
        }

        private void ReklamacjeControl_Disposed(object sender, EventArgs e)
        {
            StopAndDisposeTimer(_logCheckTimer);
            StopAndDisposeTimer(_syncStatusTimer);
            StopAndDisposeTimer(_remindersCheckTimer);
            StopAndDisposeTimer(_returnsSyncTimer);
            StopAndDisposeTimer(_popupCheckTimer);
            StopAndDisposeTimer(_emailSyncTimer);
            if (_privateWebView != null) _privateWebView.Dispose();
        }

        private static void StopAndDisposeTimer(System.Timers.Timer t)
        {
            if (t != null) { t.Stop(); t.Dispose(); }
        }

        private void EnsureProcessingGridScrollable()
        {
            try
            {
                dataGridViewProcessing.ScrollBars = ScrollBars.Both;
                typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.SetValue(dataGridViewProcessing, true, null);
            }
            catch { }
        }

        private void txtFilterProcessing_TextChanged(object sender, EventArgs e)
        {
            if (dataGridViewProcessing.DataSource is DataTable dt)
            {
                string f = txtFilterProcessing.Text.Replace("'", "''");
                dt.DefaultView.RowFilter = string.IsNullOrWhiteSpace(f) ? ""
                    : $"NrZgloszenia LIKE '%{f}%' OR Klient LIKE '%{f}%'";
            }
        }

        // =====================================================================
        // NAWIGACJA MENU
        // =====================================================================

        private void HighlightMenuButton(object sender)
        {
            foreach (Control c in pnlMenuButtons.Controls)
                if (c is Button b) { b.BackColor = Color.FromArgb(21, 32, 54); b.ForeColor = Color.FromArgb(180, 190, 210); }
            if (sender is Button btn) { btn.BackColor = Color.FromArgb(30, 41, 59); btn.ForeColor = Color.White; }
        }

        private void menuStronaGlowna_Click(object sender, EventArgs e) { HighlightMenuButton(sender); RequestDataReload(); }
        private void menuNiezarejestrowaneGoogle_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormUniversalWizardV2(WizardSource.GoogleSheet).Show(); }
        private void menuNiezarejestrowaneAllegro_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormUniversalWizardV2(WizardSource.Allegro).Show(); }
        private void menuDodajNowe_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormUniversalWizardV2(WizardSource.Manual).Show(); }
        private void menuWszystkieZgloszenia_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new WyszukiwarkaZgloszenForm().Show(); }
        private void menuCzatAllegro_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormWiadomosci().Show(); }
        private void btnContactCenter_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormHistoria().Show(); }
        private void menuPrzypomnienia_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormPrzypomnienia().Show(); }
        private void menuKlienci_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new Form3().Show(); }
        private void menuProdukty_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new Form15("1").Show(); }
        private void menuProducenci_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new Form16().Show(); }
        private void menuUstawienia_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormUstawienia().Show(); }
        private void menuSledzeniePrzesylek_Click(object sender, EventArgs e) { HighlightMenuButton(sender); new FormDpdTracking().Show(); }
        private void menuNiezarejestrowaneZwroty_Click(object sender, EventArgs e)
        {
            HighlightMenuButton(sender);
            try { new FormUniversalWizardV2(WizardSource.Zwroty).Show(); }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }
    }
}
