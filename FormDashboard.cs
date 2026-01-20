using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace Reklamacje_Dane
{
    public partial class FormDashboard : Form
    {
        // --- DANE UŻYTKOWNIKA I SERWISY ---
        private readonly string _fullName;
        private readonly string _userRole;
        private readonly DatabaseService _databaseService;

        // --- OBSŁUGA TELEFONU ---
        private string _lastCallerNumber = "";
        private DateTime _lastCallTime = DateTime.MinValue;
        private bool _userClosedCallPopup = false;
        private PhoneClient _phoneClient;
        private Timer _timerPhone;
        private TextBox txtPhoneIp;
        private Button btnConnectPhone;
        private Button btnQrPair;
        private bool _isCallPopupOpen = false;

        // --- UI I NAWIGACJA ---
        private readonly Dictionary<string, UserControl> _moduleControls = new Dictionary<string, UserControl>();
        private Button _activeMenuButton = null;
        private bool isMenuCollapsed = false;
        private const int maxMenuWidth = 250;
        private const int minMenuWidth = 60;

        // --- POWIADOMIENIA ---
        private NotifyIcon notifyIcon1;

        public FormDashboard(string fullName, string userRole)
        {
            InitializeComponent();
            _fullName = fullName;
            _userRole = userRole;

            // 1. Serwis Bazy
            _databaseService = new DatabaseService(DatabaseHelper.GetConnectionString());

            // 2. Ikona w Trayu
            notifyIcon1 = new NotifyIcon();
            notifyIcon1.Icon = SystemIcons.Information;
            notifyIcon1.Text = "System Reklamacji - Aktywny";
            notifyIcon1.Visible = true;

            // 3. Ustawienia Okna
            this.WindowState = FormWindowState.Maximized;
            this.Text = $"System Reklamacji - Zalogowany: {_fullName} ({_userRole})";

            // 4. Zdarzenia
            this.Load += FormDashboard_Load;
            this.btnToggleMenu.Click += BtnToggleMenu_Click;
            this.menuTransitionTimer.Tick += MenuTransitionTimer_Tick;

            if (this.btnLogout != null) this.btnLogout.Click += btnLogout_Click;

            // 5. Budowa Panelu Telefonu
            SetupPhonePanel();
        }

        private async void FormDashboard_Load(object sender, EventArgs e)
        {
            lblUserName.Text = _fullName;

            // Wczytaj menu
            await CreateDynamicMenuAsync();

            // AUTOMATYCZNE ŁĄCZENIE Z TELEFONEM
            await AutoconnectPhoneAsync();
        }

        // =================================================================================
        // SEKCJA: TELEFON (ŁĄCZENIE I POLLING)
        // =================================================================================

        private void SetupPhonePanel()
        {
            Label lblIp = new Label { Text = "IP Telefonu:", AutoSize = true, Location = new Point(this.panelTop.Width - 520, 15), ForeColor = Color.DimGray, Anchor = AnchorStyles.Top | AnchorStyles.Right, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            txtPhoneIp = new TextBox { Text = "10.5.0.XXX", Location = new Point(this.panelTop.Width - 430, 12), Width = 110, Anchor = AnchorStyles.Top | AnchorStyles.Right };

            btnConnectPhone = new Button { Text = "Połącz", Location = new Point(this.panelTop.Width - 300, 10), Width = 90, Height = 28, BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.Top | AnchorStyles.Right, Cursor = Cursors.Hand };
            btnConnectPhone.Click += (s, e) => StartPhoneMonitoring(txtPhoneIp.Text, false);

            btnQrPair = new Button { Text = "QR", Location = new Point(this.panelTop.Width - 200, 10), Width = 90, Height = 28, BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.Top | AnchorStyles.Right, Cursor = Cursors.Hand };
            btnQrPair.Click += async (s, e) => await StartQrPairingAsync();

            this.panelTop.Controls.Add(lblIp);
            this.panelTop.Controls.Add(txtPhoneIp);
            this.panelTop.Controls.Add(btnConnectPhone);
            this.panelTop.Controls.Add(btnQrPair);
        }

        private async Task AutoconnectPhoneAsync()
        {
            try
            {
                // Pobierz IP zapisane dla tego konkretnego loginu użytkownika
                string sql = "SELECT OstatnieIP FROM UstawieniaUzytkownika WHERE Uzytkownik = @user";
                object savedIp = await _databaseService.ExecuteScalarAsync(sql, new MySqlParameter("@user", _fullName));

                if (savedIp != null && savedIp != DBNull.Value)
                {
                    string ip = savedIp.ToString();
                    txtPhoneIp.Text = ip;
                    // Cicha próba połączenia bez komunikatów o błędach
                    await Task.Run(() => StartPhoneMonitoring(ip, quiet: true));
                }
            }
            catch { /* Ignoruj błędy przy autostarcie */ }
        }

        private async void StartPhoneMonitoring(string ip, bool quiet)
        {
            if (string.IsNullOrWhiteSpace(ip) || ip.Contains("XXX")) return;

            _phoneClient = new PhoneClient(ip);

            var pairStatus = await _phoneClient.CheckPairStatusAsync();
            if (pairStatus == null)
            {
                if (!quiet)
                {
                    btnConnectPhone.BackColor = Color.Red;
                    btnConnectPhone.Text = "Błąd";
                    MessageBox.Show("Nie można połączyć się z telefonem. Sprawdź IP i sieć Wi-Fi.", "Brak połączenia z telefonem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }

            string pairingCode = null;
            if (!pairStatus.paired)
            {
                if (quiet)
                {
                    return;
                }

                string code = Interaction.InputBox(
                    "Wpisz kod parowania z aplikacji ENA na telefonie.\nKod jest widoczny na ekranie głównym telefonu.",
                    "Parowanie telefonu",
                    "");

                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Parowanie anulowane. Wpisz kod parowania, aby połączyć aplikacje.", "Parowanie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool paired = await _phoneClient.PairAsync(code.Trim());
                if (!paired)
                {
                    btnConnectPhone.BackColor = Color.Red;
                    btnConnectPhone.Text = "Błąd";
                    MessageBox.Show("Nieprawidłowy kod parowania. Sprawdź kod na telefonie i spróbuj ponownie.", "Błąd parowania", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                pairingCode = code.Trim();
            }
            else if (string.IsNullOrWhiteSpace(pairStatus.user) || string.IsNullOrWhiteSpace(pairStatus.apiBaseUrl))
            {
                if (quiet)
                {
                    return;
                }

                string code = Interaction.InputBox(
                    "Telefon jest sparowany, ale brakuje konfiguracji API.\nPodaj kod parowania z telefonu, aby zsynchronizować ustawienia.",
                    "Synchronizacja konfiguracji",
                    "");

                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Synchronizacja anulowana. Bez konfiguracji API moduły zwrotów na telefonie nie zadziałają.", "Synchronizacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                pairingCode = code.Trim();
            }

            if (!string.IsNullOrWhiteSpace(pairingCode))
            {
                string apiBaseUrl = ResolveApiBaseUrl();
                bool configured = await _phoneClient.ConfigureAsync(pairingCode, apiBaseUrl, _fullName);
                if (!configured)
                {
                    MessageBox.Show("Nie udało się przesłać konfiguracji API do telefonu. Sprawdź dostępność API i spróbuj ponownie.", "Błąd konfiguracji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Test połączenia (Endpoint /stan)
            var status = await _phoneClient.CheckCallStatus();

            this.Invoke((MethodInvoker)async delegate
            {
                if (status != null)
                {
                    // Sukces - zapisz IP w bazie pod loginem użytkownika
                    string sqlSave = "REPLACE INTO UstawieniaUzytkownika (Uzytkownik, OstatnieIP) VALUES (@user, @ip)";
                    await _databaseService.ExecuteNonQueryAsync(sqlSave,
                        new MySqlParameter("@user", _fullName),
                        new MySqlParameter("@ip", ip));

                    btnConnectPhone.BackColor = Color.ForestGreen;
                    btnConnectPhone.Text = "Połączono";

                    if (_timerPhone == null)
                    {
                        _timerPhone = new Timer { Interval = 2000 };
                        _timerPhone.Tick += TimerPhone_Tick;
                    }
                    _timerPhone.Start();
                }
                else if (!quiet)
                {
                    btnConnectPhone.BackColor = Color.Red;
                    btnConnectPhone.Text = "Błąd";
                    MessageBox.Show("INSTRUKCJA POŁĄCZENIA:\n\n" +
                        "1. Otwórz aplikację 'Ena Server' na swoim telefonie.\n" +
                        "2. Sprawdź czy telefon i komputer są w tej samej sieci firmowej.\n" +
                        "3. Wpisz IP wyświetlone na ekranie telefonu do pola w programie.\n" +
                        "4. Kliknij 'Połącz' ponownie.", "Brak połączenia z telefonem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        private static string ResolveApiBaseUrl()
        {
            string baseUrl = global::System.Configuration.ConfigurationManager.AppSettings["ReklamacjeApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "http://localhost:5000";
            }

            string localIp = GetLocalIpv4Address();
            if (string.IsNullOrWhiteSpace(localIp))
            {
                return baseUrl;
            }

            if (baseUrl.Contains("localhost"))
            {
                return baseUrl.Replace("localhost", localIp);
            }

            if (baseUrl.Contains("127.0.0.1"))
            {
                return baseUrl.Replace("127.0.0.1", localIp);
            }

            return baseUrl;
        }

        private static string GetLocalIpv4Address()
        {
            try
            {
                var host = global::System.Net.Dns.GetHostEntry(global::System.Net.Dns.GetHostName());
                var address = host.AddressList.FirstOrDefault(a =>
                    a.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetwork &&
                    !global::System.Net.IPAddress.IsLoopback(a));
                return address?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private async Task StartQrPairingAsync()
        {
            string localIp = GetLocalIpv4Address();
            if (string.IsNullOrWhiteSpace(localIp))
            {
                MessageBox.Show("Nie udało się ustalić IP komputera.", "Parowanie QR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            const int port = 5505;

            using (var server = new QrPairingServer(localIp, port))
            {
                var payload = new QrPairingPayload
                {
                    PcIp = localIp,
                    PcPort = port,
                    Token = server.Token,
                    User = SessionManager.CurrentUserLogin ?? string.Empty,
                    ApiBaseUrl = ResolveApiBaseUrl()
                };

                try
                {
                    server.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie udało się uruchomić QR: {ex.Message}", "Parowanie QR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var qrForm = new FormQrPairing(payload, server))
                {
                    var result = qrForm.ShowDialog(this);
                    if (result != DialogResult.OK || qrForm.PairingRequest == null)
                        return;

                    txtPhoneIp.Text = qrForm.PairingRequest.PhoneIp;
                    _phoneClient = new PhoneClient(qrForm.PairingRequest.PhoneIp);

                    bool paired = await _phoneClient.PairAsync(qrForm.PairingRequest.PairingCode);
                    if (!paired)
                    {
                        MessageBox.Show("Nie udało się sparować telefonu po QR. Sprawdź kod i spróbuj ponownie.", "Parowanie QR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string apiBaseUrl = ResolveApiBaseUrl();
                    string userName = SessionManager.CurrentUserLogin ?? string.Empty;
                    await _phoneClient.ConfigureAsync(qrForm.PairingRequest.PairingCode, apiBaseUrl, userName);

                    StartPhoneMonitoring(qrForm.PairingRequest.PhoneIp, quiet: true);
                }
            }
        }


        private async void TimerPhone_Tick(object sender, EventArgs e)
        {
            if (_phoneClient == null) return;

            // 1. Check call status
            var callStatus = await _phoneClient.CheckCallStatus();

            if (callStatus != null && callStatus.dzwoni)
            {
                // Logic to handle debounce and user closing the popup
                // If it's a new number or enough time has passed (e.g., 5 seconds) since the last signal for this number
                if (callStatus.numer != _lastCallerNumber || (DateTime.Now - _lastCallTime).TotalSeconds > 5)
                {
                    // Reset flags for a new call
                    _lastCallerNumber = callStatus.numer;
                    _userClosedCallPopup = false;
                }

                _lastCallTime = DateTime.Now;

                // Only show popup if it's not already open AND the user hasn't actively closed it for this call
                if (!_isCallPopupOpen && !_userClosedCallPopup)
                {
                    _isCallPopupOpen = true;
                    string numer = _databaseService.NormalizujNumer(callStatus.numer);
                    var klient = await _databaseService.ZnajdzKlientaPoNumerzeAsync(numer);
                    DataTable dtZgloszenia = await _databaseService.PobierzZgloszeniaWgTelefonuAsync(numer);

                    FormPolaczenie popup = new FormPolaczenie(numer, klient, dtZgloszenia, _databaseService, _phoneClient);

                    popup.FormClosed += (s, args) =>
                    {
                        _isCallPopupOpen = false;
                        // Mark that the user closed the popup, so don't reopen for THIS call
                        _userClosedCallPopup = true;
                    };

                    popup.Show();
                }
            }
            else
            {
                // No call ringing, reset state
                _isCallPopupOpen = false;
                _lastCallerNumber = "";
                _userClosedCallPopup = false;
            }

            // 2. Sprawdzanie nowych SMS-ów
            var smsy = await _phoneClient.CheckNewSms();
            if (smsy != null && smsy.Count > 0)
            {
                foreach (var sms in smsy)
                {
                    // Zapisz do bazy jako otrzymany
                    await _databaseService.ZapiszNowySmsAsync(sms.number, sms.content, "Odebrane");

                    // Pokaż okno FormSmsPopup (Nowa wersja z szablonami i podglądem)
                    var smsPopup = new FormSmsPopup(sms.number, sms.content, _databaseService, _phoneClient);
                    smsPopup.Show();

                    notifyIcon1.ShowBalloonTip(3000, "Nowy SMS", $"Od: {sms.number}", ToolTipIcon.Info);
                }
            }
        }

        // =================================================================================
        // SEKCJA: MENU I NAWIGACJA (BEZ ZMIAN)
        // =================================================================================

        private async Task CreateDynamicMenuAsync()
        {
            try
            {
                string query = @"SELECT m.NazwaModulu FROM Uprawnienia u JOIN Moduly m ON u.ModulId = m.Id WHERE u.UzytkownikId = @userId ORDER BY m.Id";
                var dt = await _databaseService.GetDataTableAsync(query, new MySqlParameter("@userId", SessionManager.CurrentUserId));
                panelMenu.Controls.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    string moduleName = row["NazwaModulu"].ToString();
                    Button btn = new Button { Text = "▶  " + moduleName, Tag = moduleName, Dock = DockStyle.Top, Height = 45, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.Gainsboro, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(20, 0, 0, 0), Cursor = Cursors.Hand };
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Click += MenuButton_Click;
                    panelMenu.Controls.Add(btn);
                    btn.BringToFront();
                }
                if (panelMenu.Controls.Count > 0) (panelMenu.Controls[panelMenu.Controls.Count - 1] as Button).PerformClick();
            }
            catch (Exception ex) { MessageBox.Show("Błąd menu: " + ex.Message); }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            string moduleName = btn.Tag.ToString();
            SetActiveButton(btn);
            foreach (var ctrl in _moduleControls.Values) ctrl.Visible = false;
            if (_moduleControls.ContainsKey(moduleName)) _moduleControls[moduleName].Visible = true;
            else
            {
                UserControl ctrl = CreateModuleControl(moduleName);
                if (ctrl != null) { _moduleControls.Add(moduleName, ctrl); panelMain.Controls.Add(ctrl); ctrl.Dock = DockStyle.Fill; ctrl.Visible = true; }
            }
            lblCurrentModule.Text = moduleName;
        }

        private UserControl CreateModuleControl(string name)
        {
            switch (name)
            {
                case "Reklamacje": return new ReklamacjeControl(_fullName, _userRole);
                case "Magazyn": return new MagazynControl(_fullName, _userRole);
                case "Handlowiec": return new HandlowiecControl(_fullName, _userRole);
                case "Admin": return new AdminControl();
                case "Zwroty": return new ZwrotyPodsumowanieControl();
                default: return new UserControl();
            }
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeMenuButton != null) { _activeMenuButton.BackColor = Color.FromArgb(45, 52, 54); _activeMenuButton.ForeColor = Color.Gainsboro; }
            btn.BackColor = Color.FromArgb(0, 122, 204); btn.ForeColor = Color.White;
            _activeMenuButton = btn;
        }

        private void BtnToggleMenu_Click(object sender, EventArgs e) { isMenuCollapsed = !isMenuCollapsed; if (!isMenuCollapsed) UpdateMenuButtonsAppearance(); menuTransitionTimer.Start(); }

        private void MenuTransitionTimer_Tick(object sender, EventArgs e)
        {
            if (isMenuCollapsed) { if (panelLeft.Width > minMenuWidth) panelLeft.Width -= 20; else { panelLeft.Width = minMenuWidth; menuTransitionTimer.Stop(); UpdateMenuButtonsAppearance(); } }
            else { if (panelLeft.Width < maxMenuWidth) panelLeft.Width += 20; else { panelLeft.Width = maxMenuWidth; menuTransitionTimer.Stop(); } }
        }
        // 1. Dodaj pole timera
        private Timer _timerEmail;
        private EmailService _emailService = new EmailService();

        // 2. W konstruktorze lub Form_Load uruchom go
        private void SetupEmailTimer()
        {
            _timerEmail = new Timer();
            _timerEmail.Interval = 300000; // 5 minut (300 000 ms)
            _timerEmail.Tick += async (s, e) => await CheckEmailSafe();
            _timerEmail.Start();

            // Opcjonalnie: Sprawdź raz od razu po starcie
            // await CheckEmailSafe(); 
        }

        // 3. Metoda wywoływana przez Timer
        private async Task CheckEmailSafe()
        {
            // Zatrzymujemy timer, żeby nie nałożyły się dwa sprawdzenia
            _timerEmail.Stop();
            try
            {
                await _emailService.PobierzPoczteDlaWszystkichKontAsync();

                // Tutaj możesz odświeżyć widok, jeśli masz otwartą historię
                // np. RefreshActiveView();
            }
            catch { /* Ignorujemy błędy połączenia w tle */ }
            finally
            {
                _timerEmail.Start(); // Wznawiamy timer
            }
        }
        private void UpdateMenuButtonsAppearance()
        {
            foreach (Button btn in panelMenu.Controls.OfType<Button>())
            {
                if (isMenuCollapsed) { btn.Text = "▶"; btn.TextAlign = ContentAlignment.MiddleCenter; btn.Padding = new Padding(0); }
                else { btn.Text = "▶  " + btn.Tag.ToString(); btn.TextAlign = ContentAlignment.MiddleLeft; btn.Padding = new Padding(20, 0, 0, 0); }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (_timerPhone != null) _timerPhone.Stop();
            TriggerPhoneDisconnect();
            notifyIcon1.Visible = false;
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_timerPhone != null) _timerPhone.Stop();
            TriggerPhoneDisconnect();
            notifyIcon1.Visible = false;
            base.OnFormClosing(e);
        }

        private void TriggerPhoneDisconnect()
        {
            if (_phoneClient == null)
            {
                return;
            }
            // Nie rozparowuj telefonu automatycznie przy zamknięciu aplikacji.
            // Połączenie zostanie uznane za nieaktywne po wygaśnięciu "last seen".
            _phoneClient = null;
        }
    }
}
