using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Formularz do konfiguracji połączenia z REST API
    /// Umożliwia logowanie, testowanie połączenia i synchronizację danych
    /// </summary>
    public partial class FormApiConfig : Form
    {
        private TextBox txtApiUrl;
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnTestConnection;
        private Button btnLogin;
        private Button btnLogout;
        private Button btnSyncNow;
        private Label lblStatus;
        private Label lblLastSync;
        private Label lblUserInfo;
        private GroupBox groupConnection;
        private GroupBox groupAuth;
        private GroupBox groupSync;
        private ProgressBar progressBar;
        private CheckBox chkAutoSync;

        public FormApiConfig()
        {
            InitializeComponent();
            LoadSettings();
            UpdateUIState();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.ClientSize = new Size(600, 550);
            this.Text = "Konfiguracja REST API";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Group: Połączenie
            groupConnection = new GroupBox
            {
                Location = new Point(20, 20),
                Size = new Size(560, 110),
                Text = "Połączenie z API"
            };

            var lblApiUrl = new Label
            {
                Location = new Point(15, 25),
                Size = new Size(100, 20),
                Text = "URL API:"
            };

            txtApiUrl = new TextBox
            {
                Location = new Point(15, 50),
                Size = new Size(400, 25),
                Text = "https://192.168.1.100:5001"
            };

            btnTestConnection = new Button
            {
                Location = new Point(425, 48),
                Size = new Size(120, 28),
                Text = "🔍 Test",
                Font = new Font(this.Font, FontStyle.Regular)
            };
            btnTestConnection.Click += BtnTestConnection_Click;

            lblStatus = new Label
            {
                Location = new Point(15, 80),
                Size = new Size(530, 20),
                Text = "Wprowadź URL API i kliknij 'Test'",
                ForeColor = Color.Gray
            };

            groupConnection.Controls.AddRange(new Control[] { 
                lblApiUrl, txtApiUrl, btnTestConnection, lblStatus 
            });

            // Group: Autoryzacja
            groupAuth = new GroupBox
            {
                Location = new Point(20, 140),
                Size = new Size(560, 150),
                Text = "Logowanie"
            };

            var lblLogin = new Label
            {
                Location = new Point(15, 25),
                Size = new Size(100, 20),
                Text = "Login:"
            };

            txtLogin = new TextBox
            {
                Location = new Point(15, 50),
                Size = new Size(250, 25)
            };

            var lblPassword = new Label
            {
                Location = new Point(280, 25),
                Size = new Size(100, 20),
                Text = "Hasło:"
            };

            txtPassword = new TextBox
            {
                Location = new Point(280, 50),
                Size = new Size(250, 25),
                PasswordChar = '•'
            };

            btnLogin = new Button
            {
                Location = new Point(15, 90),
                Size = new Size(250, 35),
                Text = "🔐 Zaloguj",
                Font = new Font(this.Font.FontFamily, 10F, FontStyle.Bold),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnLogout = new Button
            {
                Location = new Point(280, 90),
                Size = new Size(250, 35),
                Text = "🚪 Wyloguj",
                Font = new Font(this.Font.FontFamily, 10F),
                Enabled = false
            };
            btnLogout.Click += BtnLogout_Click;

            lblUserInfo = new Label
            {
                Location = new Point(15, 130),
                Size = new Size(530, 15),
                Text = "Nie zalogowano",
                ForeColor = Color.Gray,
                Font = new Font(this.Font, FontStyle.Italic)
            };

            groupAuth.Controls.AddRange(new Control[] { 
                lblLogin, txtLogin, lblPassword, txtPassword, 
                btnLogin, btnLogout, lblUserInfo 
            });

            // Group: Synchronizacja
            groupSync = new GroupBox
            {
                Location = new Point(20, 300),
                Size = new Size(560, 180),
                Text = "Synchronizacja danych"
            };

            chkAutoSync = new CheckBox
            {
                Location = new Point(15, 25),
                Size = new Size(250, 20),
                Text = "Automatyczna synchronizacja co 5 min"
            };

            btnSyncNow = new Button
            {
                Location = new Point(15, 55),
                Size = new Size(530, 40),
                Text = "🔄 SYNCHRONIZUJ TERAZ",
                Font = new Font(this.Font.FontFamily, 11F, FontStyle.Bold),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnSyncNow.FlatAppearance.BorderSize = 0;
            btnSyncNow.Click += BtnSyncNow_Click;

            progressBar = new ProgressBar
            {
                Location = new Point(15, 105),
                Size = new Size(530, 23),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            lblLastSync = new Label
            {
                Location = new Point(15, 135),
                Size = new Size(530, 35),
                Text = "Ostatnia synchronizacja: Nigdy\n\n" +
                       "Kliknij 'SYNCHRONIZUJ TERAZ' aby pobrać dane z API",
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.AliceBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            groupSync.Controls.AddRange(new Control[] { 
                chkAutoSync, btnSyncNow, progressBar, lblLastSync 
            });

            // Buttons
            var btnClose = new Button
            {
                Location = new Point(490, 490),
                Size = new Size(90, 30),
                Text = "Zamknij",
                DialogResult = DialogResult.OK
            };

            // Add all to form
            this.Controls.AddRange(new Control[] { 
                groupConnection, groupAuth, groupSync, btnClose 
            });

            this.ResumeLayout(false);
        }

        private void LoadSettings()
        {
            try
            {
                txtApiUrl.Text = Properties.Settings.Default.ApiBaseUrl ?? "https://192.168.1.100:5001";
                txtLogin.Text = Properties.Settings.Default.ApiLogin ?? "";
                chkAutoSync.Checked = Properties.Settings.Default.ApiAutoSync;

                // Spróbuj zainicjalizować API jeśli URL jest ustawiony
                string savedUrl = Properties.Settings.Default.ApiBaseUrl;
                if (!string.IsNullOrEmpty(savedUrl))
                {
                    try
                    {
                        if (!IsApiInitialized())
                        {
                            ApiSyncService.Initialize(savedUrl);
                        }
                        
                        // Spróbuj auto-login
                        _ = TryAutoLoginAsync();
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                Properties.Settings.Default.ApiBaseUrl = txtApiUrl.Text;
                Properties.Settings.Default.ApiLogin = txtLogin.Text;
                Properties.Settings.Default.ApiAutoSync = chkAutoSync.Checked;
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        private async Task TryAutoLoginAsync()
        {
            try
            {
                if (IsApiInitialized() && await ApiSyncService.Instance.AutoLoginAsync())
                {
                    UpdateUIState();
                    UpdateLastSyncInfo();
                }
            }
            catch { }
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            string url = txtApiUrl.Text.Trim();

            if (string.IsNullOrEmpty(url))
            {
                UpdateStatus("⚠️ Wprowadź URL API!", Color.Orange);
                return;
            }

            btnTestConnection.Enabled = false;
            UpdateStatus("🔍 Testuję połączenie...", Color.DodgerBlue);

            try
            {
                bool canConnect = await ApiSyncService.TestConnectionAsync(url);

                if (canConnect)
                {
                    UpdateStatus("✅ Połączenie udane! API działa poprawnie.", Color.Green);
                    
                    // Inicjalizuj service jeśli jeszcze nie
                    if (!IsApiInitialized())
                    {
                        ApiSyncService.Initialize(url);
                    }

                    SaveSettings();
                }
                else
                {
                    UpdateStatus("❌ Nie można połączyć się z API. Sprawdź URL i czy serwer działa.", Color.Red);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Błąd: {ex.Message}", Color.Red);
            }
            finally
            {
                btnTestConnection.Enabled = true;
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string url = txtApiUrl.Text.Trim();
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Wprowadź URL API!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Wprowadź login i hasło!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            UpdateStatus("🔐 Logowanie...", Color.DodgerBlue);

            try
            {
                // Inicjalizuj service jeśli jeszcze nie
                if (!IsApiInitialized())
                {
                    ApiSyncService.Initialize(url);
                }

                bool success = await ApiSyncService.Instance.LoginAsync(login, password);

                if (success)
                {
                    UpdateStatus("✅ Zalogowano pomyślnie!", Color.Green);
                    SaveSettings();
                    UpdateUIState();
                    
                    MessageBox.Show(
                        $"Zalogowano jako: {ApiSyncService.Instance.CurrentUser?.NazwaWyswietlana ?? login}",
                        "Sukces",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    txtPassword.Clear();
                }
                else
                {
                    UpdateStatus("❌ Błąd logowania", Color.Red);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Błąd: {ex.Message}", Color.Red);
                MessageBox.Show(
                    $"Błąd logowania:\n\n{ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsApiInitialized())
                {
                    ApiSyncService.Instance.Logout();
                }
                UpdateStatus("Wylogowano", Color.Gray);
                UpdateUIState();
                txtPassword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wylogowania: {ex.Message}", "Błąd", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSyncNow_Click(object sender, EventArgs e)
        {
            if (!IsApiInitialized())
            {
                MessageBox.Show("API nie jest zainicjalizowane. Sprawdź połączenie.", "Błąd", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSyncNow.Enabled = false;
            progressBar.Visible = true;
            lblLastSync.Text = "⏳ Synchronizacja w toku...";

            try
            {
                var zgloszenia = await ApiSyncService.Instance.SyncZgloszeniaAsync(forceRefresh: true);
                
                lblLastSync.Text = $"✅ Zsynchronizowano {zgloszenia.Count} zgłoszeń\n" +
                                  $"Ostatnia synchronizacja: {DateTime.Now:HH:mm:ss}";
                lblLastSync.ForeColor = Color.Green;

                MessageBox.Show(
                    $"Synchronizacja zakończona!\n\nPobrano {zgloszenia.Count} zgłoszeń z API.",
                    "Sukces",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                lblLastSync.Text = $"❌ Błąd synchronizacji: {ex.Message}";
                lblLastSync.ForeColor = Color.Red;

                MessageBox.Show(
                    $"Błąd synchronizacji:\n\n{ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                progressBar.Visible = false;
                btnSyncNow.Enabled = true;
            }
        }

        private bool IsApiInitialized()
        {
            try
            {
                return ApiSyncService.Instance != null && ApiSyncService.Instance.IsInitialized;
            }
            catch
            {
                return false;
            }
        }

        private void UpdateUIState()
        {
            bool isAuthenticated = IsApiInitialized() && ApiSyncService.Instance.IsAuthenticated;

            btnLogin.Enabled = !isAuthenticated;
            btnLogout.Enabled = isAuthenticated;
            btnSyncNow.Enabled = isAuthenticated;
            txtLogin.Enabled = !isAuthenticated;
            txtPassword.Enabled = !isAuthenticated;

            if (isAuthenticated && ApiSyncService.Instance?.CurrentUser != null)
            {
                var user = ApiSyncService.Instance.CurrentUser;
                lblUserInfo.Text = $"Zalogowany jako: {user.NazwaWyswietlana} ({user.Login})";
                lblUserInfo.ForeColor = Color.Green;
            }
            else
            {
                lblUserInfo.Text = "Nie zalogowano";
                lblUserInfo.ForeColor = Color.Gray;
            }

            UpdateLastSyncInfo();
        }

        private void UpdateLastSyncInfo()
        {
            if (IsApiInitialized() && ApiSyncService.Instance.IsAuthenticated)
            {
                string syncInfo = ApiSyncService.Instance.GetLastSyncInfo();
                lblLastSync.Text = $"Ostatnia synchronizacja: {syncInfo}\n\n" +
                                  "Kliknij 'SYNCHRONIZUJ TERAZ' aby odświeżyć dane";
                lblLastSync.ForeColor = Color.DarkBlue;
            }
        }

        private void UpdateStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }
    }
}
