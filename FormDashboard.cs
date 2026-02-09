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
        private PhoneApiClient _phoneApiClient;
        private Timer _timerPhone;
        private Panel _phoneStatusIndicator;
          private Label _phoneStatusLabel;
           private ToolTip _phoneTooltip = new ToolTip();

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
            // Wskaźnik statusu: kropka + etykieta
            _phoneStatusIndicator = new Panel
            {
                Size = new Size(14, 14),
                Location = new Point(this.panelTop.Width - 240, 17),
                BackColor = Color.Gray,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _phoneStatusIndicator.Paint += (s, e) =>
            {
                // Rysuj okrągłą kropkę
                using (var brush = new SolidBrush(_phoneStatusIndicator.BackColor))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillEllipse(brush, 0, 0, 13, 13);
                }
            };

            _phoneStatusLabel = new Label
            {
                Text = "Telefon: Łączenie...",
                AutoSize = true,
                Location = new Point(this.panelTop.Width - 220, 15),
                ForeColor = Color.DimGray,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            _phoneTooltip.SetToolTip(_phoneStatusLabel, "Automatyczne połączenie z telefonem na podstawie konta użytkownika");
            _phoneTooltip.SetToolTip(_phoneStatusIndicator, "Status połączenia z telefonem");

            this.panelTop.Controls.Add(_phoneStatusIndicator);
            this.panelTop.Controls.Add(_phoneStatusLabel);
        }


        private async Task AutoconnectPhoneAsync()
        {
            try
            {
                // Pobierz adres API z konfiguracji
                string apiBaseUrl = ResolveApiBaseUrl();

                // Utwórz klienta API z loginem bieżącego użytkownika
                _phoneApiClient = new PhoneApiClient(apiBaseUrl, SessionManager.CurrentUserLogin ?? _fullName);

                // Sprawdź status telefonu
                bool isOnline = await _phoneApiClient.CheckPhoneOnlineAsync();

                UpdatePhoneStatusUI(isOnline);

                // Uruchom timer do pollowania zdarzeń
                if (_timerPhone == null)
                {
                    _timerPhone = new Timer { Interval = 3000 }; // 3 sekundy
                    _timerPhone.Tick += TimerPhone_Tick;
                }
                _timerPhone.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd autoconnect telefonu: {ex.Message}");
                UpdatePhoneStatusUI(false);
            }
        }
        private void UpdatePhoneStatusUI(bool isOnline)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdatePhoneStatusUI(isOnline); });
                return;
            }

            if (isOnline)
            {
                _phoneStatusIndicator.BackColor = Color.LimeGreen;
                _phoneStatusLabel.Text = "Telefon: Połączony";
                _phoneStatusLabel.ForeColor = Color.ForestGreen;
            }
            else
            {
                _phoneStatusIndicator.BackColor = Color.Red;
                _phoneStatusLabel.Text = "Telefon: Niepołączony";
                _phoneStatusLabel.ForeColor = Color.Gray;
            }

            _phoneStatusIndicator.Invalidate(); // Przerysuj kropkę

            string tooltip = isOnline
                ? $"Telefon online (ostatnio: {_phoneApiClient?.LastSeen:HH:mm:ss})"
                : "Telefon offline - uruchom aplikację ENA na telefonie i zaloguj się na to samo konto";
            _phoneTooltip.SetToolTip(_phoneStatusLabel, tooltip);
            _phoneTooltip.SetToolTip(_phoneStatusIndicator, tooltip);
        }


       

        private static string ResolveApiBaseUrl()
        {
            string baseUrl = global::System.Configuration.ConfigurationManager.AppSettings["ReklamacjeApiBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "http://localhost:5000";
            }

            baseUrl = NormalizeLocalApiBaseUrl(baseUrl);

            string localIp = GetLocalIpv4Address();
            if (string.IsNullOrWhiteSpace(localIp))
            {
                return baseUrl;
            }

            return ReplaceLoopbackHost(baseUrl, localIp);
        }

        private static string NormalizeLocalApiBaseUrl(string baseUrl)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
            {
                return baseUrl;
            }

            if (!IsLoopbackHost(uri.Host))
            {
                return baseUrl;
            }

            if (IsPortOpen("127.0.0.1", uri.Port))
            {
                return baseUrl;
            }

            string detected = TryDetectLocalApiBaseUrl();
            return string.IsNullOrWhiteSpace(detected) ? baseUrl : detected;
        }

        private static string ReplaceLoopbackHost(string baseUrl, string host)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
            {
                return baseUrl;
            }

            if (!IsLoopbackHost(uri.Host))
            {
                return baseUrl;
            }

            var builder = new UriBuilder(uri)
            {
                Host = host
            };
            return builder.Uri.ToString().TrimEnd('/');
        }

        private static bool IsLoopbackHost(string host)
        {
            return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase);
        }

        private static string TryDetectLocalApiBaseUrl()
        {
            var candidates = new[]
            {
                "http://localhost:50875",
                "http://localhost:5000",
                "https://localhost:50876"
            };

            foreach (var candidate in candidates)
            {
                if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
                {
                    continue;
                }

                if (IsPortOpen("127.0.0.1", uri.Port))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        private static bool IsPortOpen(string host, int port, int timeoutMs = 200)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var connectTask = client.ConnectAsync(host, port);
                    return connectTask.Wait(timeoutMs) && client.Connected;
                }
            }
            catch
            {
                return false;
            }
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

     


        private async void TimerPhone_Tick(object sender, EventArgs e)
        {
            if (_phoneApiClient == null) return;

            try
            {
                // 1. Sprawdź status telefonu (co tick)
                bool isOnline = await _phoneApiClient.CheckPhoneOnlineAsync();
                UpdatePhoneStatusUI(isOnline);

                if (!isOnline) return;

                // 2. Pobierz zdarzenia z API
                var events = await _phoneApiClient.GetEventsAsync();

                foreach (var evt in events)
                {
                    switch (evt.EventType)
                    {
                        case "CALL_RINGING":
                            await HandleIncomingCall(evt);
                            break;

                        case "CALL_IDLE":
                            HandleCallIdle();
                            break;

                        case "SMS_RECEIVED":
                            await HandleIncomingSms(evt);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd timer telefonu: {ex.Message}");
            }
        }

        private async Task HandleIncomingCall(PhoneEventItem evt)
        {
            string numer = evt.PhoneNumber ?? "";
            if (string.IsNullOrWhiteSpace(numer) || numer == "unknown")
            {
                // Numer nieznany - nadal wyświetl popup
                numer = "Nieznany numer";
            }

            // Debounce - ten sam numer w ciągu 5 sekund
            if (numer == _lastCallerNumber && (DateTime.Now - _lastCallTime).TotalSeconds < 5)
                return;

            // Reset flag dla nowego połączenia
            if (numer != _lastCallerNumber)
                _userClosedCallPopup = false;

            _lastCallerNumber = numer;
            _lastCallTime = DateTime.Now;

            if (_isCallPopupOpen || _userClosedCallPopup) return;

            _isCallPopupOpen = true;

            string normalizedNumer = _databaseService.NormalizujNumer(numer);
            var klient = await _databaseService.ZnajdzKlientaPoNumerzeAsync(normalizedNumer);
            DataTable dtZgloszenia = await _databaseService.PobierzZgloszeniaWgTelefonuAsync(normalizedNumer);

            // FormPolaczenie musi działać z nowym PhoneApiClient
            // (patrz zmiana w FormPolaczenie poniżej)
            FormPolaczenie popup = new FormPolaczenie(normalizedNumer, klient, dtZgloszenia, _databaseService, _phoneApiClient);

            popup.FormClosed += (s, args) =>
            {
                _isCallPopupOpen = false;
                _userClosedCallPopup = true;
            };

            popup.Show();
        }

        private void HandleCallIdle()
        {
            _isCallPopupOpen = false;
            _lastCallerNumber = "";
            _userClosedCallPopup = false;
        }

        private async Task HandleIncomingSms(PhoneEventItem evt)
        {
            string numer = evt.PhoneNumber ?? "";
            string tresc = evt.Content ?? "";

            // Zapisz do bazy
            await _databaseService.ZapiszNowySmsAsync(numer, tresc, "Odebrane");

            // Pokaż popup
            var smsPopup = new FormSmsPopup(numer, tresc, _databaseService, _phoneApiClient);
            smsPopup.Show();

            notifyIcon1.ShowBalloonTip(3000, "Nowy SMS", $"Od: {numer}", ToolTipIcon.Info);
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
            // Nie trzeba rozparowywać - wystarczy że timer się zatrzyma
            // Telefon dalej będzie wysyłał heartbeaty, ale WinForms ich nie odczyta
            _phoneApiClient = null;
        }

    }
}
