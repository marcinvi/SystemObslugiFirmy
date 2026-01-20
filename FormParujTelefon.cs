using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Formularz do parowania telefonu z aplikacją Windows Forms
    /// Umożliwia wprowadzenie IP telefonu i kodu parowania
    /// </summary>
    public partial class FormParujTelefon : Form
    {
        private TextBox txtIpTelefonu;
        private TextBox txtKodParowania;
        private Button btnParuj;
        private Button btnTestPolaczenia;
        private Button btnQrPair;
        private Label lblStatus;
        private Label lblInstrukcja;
        private Label lblIpLabel;
        private Label lblKodLabel;
        private ProgressBar progressBar;

        public string PhoneIp { get; private set; }

        public FormParujTelefon()
        {
            InitializeComponent();
            LoadSavedIp();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.ClientSize = new Size(500, 430);
            this.Text = "Parowanie z telefonem Android";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Instrukcja
            lblInstrukcja = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(460, 80),
                Text = "INSTRUKCJA PAROWANIA:\n\n" +
                       "1. Otwórz aplikację ENA na telefonie Android\n" +
                       "2. Zanotuj adres IP telefonu i kod parowania\n" +
                       "3. Wpisz poniżej IP i kod, a następnie kliknij 'Paruj telefon'",
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            // Label IP
            lblIpLabel = new Label
            {
                Location = new Point(20, 120),
                Size = new Size(150, 20),
                Text = "Adres IP telefonu:",
                Font = new Font(this.Font, FontStyle.Bold)
            };

            // TextBox IP
            txtIpTelefonu = new TextBox
            {
                Location = new Point(20, 145),
                Size = new Size(280, 25),
                Font = new Font("Consolas", 10F),
                Text = "192.168.1."
            };

            // Button Test
            btnTestPolaczenia = new Button
            {
                Location = new Point(310, 143),
                Size = new Size(170, 28),
                Text = "🔍 Test połączenia",
                Font = new Font(this.Font, FontStyle.Regular)
            };
            btnTestPolaczenia.Click += BtnTestPolaczenia_Click;

            // Label Kod
            lblKodLabel = new Label
            {
                Location = new Point(20, 185),
                Size = new Size(150, 20),
                Text = "Kod parowania:",
                Font = new Font(this.Font, FontStyle.Bold)
            };

            // TextBox Kod
            txtKodParowania = new TextBox
            {
                Location = new Point(20, 210),
                Size = new Size(280, 25),
                Font = new Font("Consolas", 10F),
                CharacterCasing = CharacterCasing.Upper,
                MaxLength = 6
            };

            // ProgressBar
            progressBar = new ProgressBar
            {
                Location = new Point(20, 250),
                Size = new Size(460, 23),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            // Label Status
            lblStatus = new Label
            {
                Location = new Point(20, 280),
                Size = new Size(460, 40),
                Text = "Wypełnij powyższe pola i kliknij 'Paruj telefon'",
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.AliceBlue,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Button Paruj
            btnParuj = new Button
            {
                Location = new Point(20, 330),
                Size = new Size(460, 35),
                Text = "📱 PARUJ TELEFON",
                Font = new Font(this.Font.FontFamily, 11F, FontStyle.Bold),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnParuj.FlatAppearance.BorderSize = 0;
            btnParuj.Click += BtnParuj_Click;

            // Button QR
            btnQrPair = new Button
            {
                Location = new Point(20, 375),
                Size = new Size(460, 35),
                Text = "📷 PARUJ PRZEZ QR",
                Font = new Font(this.Font.FontFamily, 10F, FontStyle.Bold),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnQrPair.FlatAppearance.BorderSize = 0;
            btnQrPair.Click += BtnQrPair_Click;

            // Add controls
            this.Controls.Add(lblInstrukcja);
            this.Controls.Add(lblIpLabel);
            this.Controls.Add(txtIpTelefonu);
            this.Controls.Add(btnTestPolaczenia);
            this.Controls.Add(lblKodLabel);
            this.Controls.Add(txtKodParowania);
            this.Controls.Add(progressBar);
            this.Controls.Add(lblStatus);
            this.Controls.Add(btnParuj);
            this.Controls.Add(btnQrPair);

            this.ResumeLayout(false);
        }

        private void LoadSavedIp()
        {
            try
            {
                string savedIp = Properties.Settings.Default.PhoneIP;
                if (!string.IsNullOrEmpty(savedIp))
                {
                    txtIpTelefonu.Text = savedIp;
                }
            }
            catch { }
        }

        private async void BtnTestPolaczenia_Click(object sender, EventArgs e)
        {
            string ip = txtIpTelefonu.Text.Trim();

            if (string.IsNullOrEmpty(ip))
            {
                UpdateStatus("⚠️ Wpisz adres IP telefonu!", Color.Orange);
                return;
            }

            SetControlsEnabled(false);
            progressBar.Visible = true;
            UpdateStatus("🔍 Testuję połączenie...", Color.DodgerBlue);

            try
            {
                var phoneClient = new PhoneClient(ip);
                var status = await phoneClient.CheckCallStatus();

                if (status != null)
                {
                    UpdateStatus("✅ Połączenie udane! Telefon wykryty.", Color.Green);
                    // Zapisz IP
                    Properties.Settings.Default.PhoneIP = ip;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    UpdateStatus("❌ Brak odpowiedzi. Sprawdź IP i czy aplikacja ENA działa.", Color.Red);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Błąd: {ex.Message}", Color.Red);
            }
            finally
            {
                progressBar.Visible = false;
                SetControlsEnabled(true);
            }
        }

        private async void BtnParuj_Click(object sender, EventArgs e)
        {
            string ip = txtIpTelefonu.Text.Trim();
            string kod = txtKodParowania.Text.Trim();

            await TryPairAndConfigureAsync(ip, kod, showErrors: true);
        }

        private async Task TryPairAndConfigureAsync(string ip, string kod, bool showErrors)
        {
            if (string.IsNullOrEmpty(ip))
            {
                if (showErrors)
                {
                    UpdateStatus("⚠️ Wpisz adres IP telefonu!", Color.Orange);
                    txtIpTelefonu.Focus();
                }
                return;
            }

            if (string.IsNullOrEmpty(kod))
            {
                if (showErrors)
                {
                    UpdateStatus("⚠️ Wpisz kod parowania z aplikacji Android!", Color.Orange);
                    txtKodParowania.Focus();
                }
                return;
            }

            if (kod.Length != 6)
            {
                if (showErrors)
                {
                    UpdateStatus("⚠️ Kod parowania musi mieć 6 znaków!", Color.Orange);
                    txtKodParowania.Focus();
                }
                return;
            }

            SetControlsEnabled(false);
            progressBar.Visible = true;
            UpdateStatus("📱 Parowanie z telefonem...", Color.DodgerBlue);

            try
            {
                var phoneClient = new PhoneClient(ip);
                bool success = await phoneClient.PairAsync(kod);

                if (success)
                {
                    // Zapisz IP
                    Properties.Settings.Default.PhoneIP = ip;
                    Properties.Settings.Default.Save();

                    await ConfigurePhoneAsync(phoneClient, kod);

                    UpdateStatus("✅ SPAROWANO POMYŚLNIE!", Color.Green);

                    if (showErrors)
                    {
                        MessageBox.Show(
                            "Telefon został pomyślnie sparowany!\n\n" +
                            $"IP telefonu: {ip}\n" +
                            "Możesz teraz wysyłać SMS i dzwonić z poziomu aplikacji.",
                            "Sukces",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }

                    PhoneIp = ip;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    UpdateStatus("❌ Błąd parowania - sprawdź kod i spróbuj ponownie", Color.Red);
                    txtKodParowania.Clear();
                    txtKodParowania.Focus();
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Błąd: {ex.Message}", Color.Red);

                if (showErrors)
                {
                    MessageBox.Show(
                        $"Błąd podczas parowania:\n\n{ex.Message}\n\n" +
                        "Sprawdź czy:\n" +
                        "• Telefon jest w tej samej sieci Wi-Fi\n" +
                        "• Aplikacja ENA jest uruchomiona\n" +
                        "• Kod parowania jest poprawny",
                        "Błąd",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            finally
            {
                progressBar.Visible = false;
                SetControlsEnabled(true);
            }
        }

        private async void BtnQrPair_Click(object sender, EventArgs e)
        {
            string localIp = GetLocalIpv4Address();
            if (string.IsNullOrWhiteSpace(localIp))
            {
                UpdateStatus("❌ Nie udało się ustalić IP komputera.", Color.Red);
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
                    UpdateStatus($"❌ Błąd uruchomienia QR: {ex.Message}", Color.Red);
                    return;
                }

                using (var qrForm = new FormQrPairing(payload, server))
                {
                    var result = qrForm.ShowDialog(this);
                    if (result == DialogResult.OK && qrForm.PairingRequest != null)
                    {
                        txtIpTelefonu.Text = qrForm.PairingRequest.PhoneIp;
                        txtKodParowania.Text = qrForm.PairingRequest.PairingCode;
                        await TryPairAndConfigureAsync(qrForm.PairingRequest.PhoneIp, qrForm.PairingRequest.PairingCode, showErrors: false);
                    }
                }
            }
        }

        private async Task ConfigurePhoneAsync(PhoneClient phoneClient, string pairingCode)
        {
            if (phoneClient == null || string.IsNullOrWhiteSpace(pairingCode))
            {
                return;
            }

            string apiBaseUrl = ResolveApiBaseUrl();
            string userName = SessionManager.CurrentUserLogin ?? string.Empty;
            await phoneClient.ConfigureAsync(pairingCode, apiBaseUrl, userName);
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
                foreach (var address in host.AddressList)
                {
                    if (address.AddressFamily == global::System.Net.Sockets.AddressFamily.InterNetwork &&
                        !global::System.Net.IPAddress.IsLoopback(address))
                    {
                        return address.ToString();
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        private void UpdateStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private void SetControlsEnabled(bool enabled)
        {
            txtIpTelefonu.Enabled = enabled;
            txtKodParowania.Enabled = enabled;
            btnParuj.Enabled = enabled;
            btnTestPolaczenia.Enabled = enabled;
        }
    }
}
