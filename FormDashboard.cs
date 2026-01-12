using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Label lblIp = new Label { Text = "IP Telefonu:", AutoSize = true, Location = new Point(this.panelTop.Width - 380, 15), ForeColor = Color.DimGray, Anchor = AnchorStyles.Top | AnchorStyles.Right, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            txtPhoneIp = new TextBox { Text = "10.5.0.XXX", Location = new Point(this.panelTop.Width - 300, 12), Width = 110, Anchor = AnchorStyles.Top | AnchorStyles.Right };

            btnConnectPhone = new Button { Text = "Połącz", Location = new Point(this.panelTop.Width - 180, 10), Width = 80, Height = 28, BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.Top | AnchorStyles.Right, Cursor = Cursors.Hand };
            btnConnectPhone.Click += (s, e) => StartPhoneMonitoring(txtPhoneIp.Text, false);

            this.panelTop.Controls.Add(lblIp);
            this.panelTop.Controls.Add(txtPhoneIp);
            this.panelTop.Controls.Add(btnConnectPhone);
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

        private void btnLogout_Click(object sender, EventArgs e) { if (_timerPhone != null) _timerPhone.Stop(); notifyIcon1.Visible = false; this.DialogResult = DialogResult.Retry; this.Close(); }

        protected override void OnFormClosing(FormClosingEventArgs e) { if (_timerPhone != null) _timerPhone.Stop(); notifyIcon1.Visible = false; base.OnFormClosing(e); }
    }
}