using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Formularz do generowania QR Code dla synchronizacji z telefonem Android
    /// </summary>
    public partial class FormQrSync : Form
    {
        private PictureBox pictureBoxQr;
        private Label lblTitle;
        private Label lblInstrukcja;
        private Label lblStatus;
        private Button btnGeneruj;
        private Button btnOdswiez;
        private Label lblDetails;
        private ProgressBar progressBar;
        private Timer expiryTimer;
        private DateTime qrCodeExpiryTime;

        public FormQrSync()
        {
            InitializeComponent();
            GenerateQrCode();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.ClientSize = new Size(600, 750);
            this.Text = "Synchronizacja przez QR Code";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Title
            lblTitle = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(560, 40),
                Text = "📱 SYNCHRONIZACJA PRZEZ QR CODE",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 120, 212)
            };

            // Instrukcja
            lblInstrukcja = new Label
            {
                Location = new Point(20, 70),
                Size = new Size(560, 80),
                Text = "INSTRUKCJA:\n\n" +
                       "1. Otwórz aplikację ENA na telefonie Android\n" +
                       "2. Kliknij \"Skanuj QR Code\"\n" +
                       "3. Zeskanuj poniższy kod kamerą telefonu",
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                Font = new Font("Segoe UI", 9.5F)
            };

            // QR Code PictureBox
            pictureBoxQr = new PictureBox
            {
                Location = new Point(150, 170),
                Size = new Size(300, 300),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            // Status
            lblStatus = new Label
            {
                Location = new Point(20, 490),
                Size = new Size(560, 30),
                Text = "✅ QR Code wygenerowany - ważny przez 5 minut",
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            // Details
            lblDetails = new Label
            {
                Location = new Point(20, 530),
                Size = new Size(560, 100),
                Text = "",
                BackColor = Color.FromArgb(240, 248, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Font = new Font("Consolas", 9F)
            };

            // ProgressBar
            progressBar = new ProgressBar
            {
                Location = new Point(20, 645),
                Size = new Size(560, 10),
                Style = ProgressBarStyle.Continuous,
                Maximum = 300, // 5 minut = 300 sekund
                Value = 300
            };

            // Button Odśwież
            btnOdswiez = new Button
            {
                Location = new Point(20, 670),
                Size = new Size(270, 50),
                Text = "🔄 WYGENERUJ NOWY KOD",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOdswiez.FlatAppearance.BorderSize = 0;
            btnOdswiez.Click += BtnOdswiez_Click;

            // Button Zamknij
            var btnZamknij = new Button
            {
                Location = new Point(310, 670),
                Size = new Size(270, 50),
                Text = "✖ ZAMKNIJ",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                BackColor = Color.FromArgb(128, 128, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnZamknij.FlatAppearance.BorderSize = 0;
            btnZamknij.Click += (s, e) => this.Close();

            // Timer
            expiryTimer = new Timer();
            expiryTimer.Interval = 1000; // 1 sekunda
            expiryTimer.Tick += ExpiryTimer_Tick;

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblInstrukcja);
            this.Controls.Add(pictureBoxQr);
            this.Controls.Add(lblStatus);
            this.Controls.Add(lblDetails);
            this.Controls.Add(progressBar);
            this.Controls.Add(btnOdswiez);
            this.Controls.Add(btnZamknij);

            this.ResumeLayout(false);
        }

        private async void GenerateQrCode()
        {
            try
            {
                lblStatus.Text = "⏳ Generowanie QR Code...";
                lblStatus.ForeColor = Color.Orange;
                btnOdswiez.Enabled = false;

                // Automatyczne wykrywanie API i IP
                var discovery = await NetworkAutoDiscovery.AutoConfigureAsync(null);

                if (!discovery.ApiFound && !discovery.PhoneFound)
                {
                    MessageBox.Show(
                        "Nie można automatycznie wykryć konfiguracji!\n\n" +
                        "Upewnij się, że:\n" +
                        "• REST API jest uruchomione\n" +
                        "• Jesteś w sieci lokalnej\n\n" +
                        "Możesz użyć ręcznego parowania.",
                        "Błąd",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    this.Close();
                    return;
                }

                // Pobierz nazwę użytkownika z sesji
                string userName = SessionManager.CurrentUser?.Login ?? "Użytkownik";

                // Wygeneruj konfigurację
                var config = QrCodeGenerator.GenerateConfig(
                    discovery.ApiUrl ?? "http://localhost:5000",
                    discovery.PhoneIp ?? NetworkAutoDiscovery.GetLocalIPAddress(),
                    userName
                );

                // Wygeneruj QR Code
                string qrJson = QrCodeGenerator.GenerateQrCodeJson(config);
                var qrImage = QrCodeGenerator.GenerateQrCodeImage(qrJson);

                // Wyświetl QR Code
                pictureBoxQr.Image = qrImage;

                // Wyświetl szczegóły
                lblDetails.Text = $"API URL: {config.ApiBaseUrl}\n" +
                                $"Komputer IP: {config.PhoneIp}\n" +
                                $"Kod parowania: {config.PairingCode}\n" +
                                $"Użytkownik: {config.UserName}\n" +
                                $"Wygasa: {config.Timestamp.AddMinutes(5):HH:mm:ss}";

                // Ustaw czas wygaśnięcia
                qrCodeExpiryTime = config.Timestamp.AddMinutes(5);

                // Uruchom timer
                expiryTimer.Start();

                // Status
                lblStatus.Text = "✅ QR Code wygenerowany - ważny przez 5 minut";
                lblStatus.ForeColor = Color.Green;
                btnOdswiez.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd podczas generowania QR Code:\n\n{ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                this.Close();
            }
        }

        private void ExpiryTimer_Tick(object sender, EventArgs e)
        {
            var remainingTime = qrCodeExpiryTime - DateTime.UtcNow;

            if (remainingTime.TotalSeconds <= 0)
            {
                // QR Code wygasł
                expiryTimer.Stop();
                lblStatus.Text = "⏰ QR Code wygasł - wygeneruj nowy";
                lblStatus.ForeColor = Color.Red;
                progressBar.Value = 0;
                pictureBoxQr.Image = null;
                btnOdswiez.Focus();
            }
            else
            {
                // Aktualizuj pasek postępu
                int remainingSeconds = (int)remainingTime.TotalSeconds;
                progressBar.Value = Math.Max(0, Math.Min(300, remainingSeconds));

                // Zmień kolor w zależności od czasu
                if (remainingSeconds < 60)
                {
                    lblStatus.Text = $"⚠️ QR Code wygasa za {remainingSeconds} sekund";
                    lblStatus.ForeColor = Color.Orange;
                }
            }
        }

        private void BtnOdswiez_Click(object sender, EventArgs e)
        {
            expiryTimer.Stop();
            pictureBoxQr.Image = null;
            GenerateQrCode();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            expiryTimer?.Stop();
            expiryTimer?.Dispose();
        }
    }
}
