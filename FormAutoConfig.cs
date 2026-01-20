using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Formularz automatycznej konfiguracji - uruchamia się przy pierwszym starcie
    /// Automatycznie znajduje REST API i telefon w sieci
    /// </summary>
    public partial class FormAutoConfig : Form
    {
        private RichTextBox txtLog;
        private Button btnStart;
        private Button btnSkip;
        private ProgressBar progressBar;
        private Label lblTitle;
        private bool configurationComplete = false;

        public bool ConfigurationSuccessful { get; private set; }

        public FormAutoConfig()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.ClientSize = new Size(700, 500);
            this.Text = "Automatyczna konfiguracja";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormClosing += FormAutoConfig_FormClosing;

            // Title
            lblTitle = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(660, 60),
                Text = "🚀 AUTOMATYCZNA KONFIGURACJA\n\nProgram automatycznie wykryje REST API i telefon Android w sieci.",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.LightBlue
            };

            // Log
            txtLog = new RichTextBox
            {
                Location = new Point(20, 100),
                Size = new Size(660, 300),
                Font = new Font("Consolas", 9F),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Progress bar
            progressBar = new ProgressBar
            {
                Location = new Point(20, 410),
                Size = new Size(660, 25),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            // Button Start
            btnStart = new Button
            {
                Location = new Point(20, 445),
                Size = new Size(320, 40),
                Text = "🔍 START - Wykryj automatycznie",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click;

            // Button Skip
            btnSkip = new Button
            {
                Location = new Point(360, 445),
                Size = new Size(320, 40),
                Text = "⏭️ Pomiń - Skonfiguruję ręcznie",
                Font = new Font("Segoe UI", 11F),
                FlatStyle = FlatStyle.Flat
            };
            btnSkip.Click += BtnSkip_Click;

            // Add controls
            this.Controls.AddRange(new Control[] { 
                lblTitle, txtLog, progressBar, btnStart, btnSkip 
            });

            this.ResumeLayout(false);

            // Dodaj welcome message
            AppendLog("Witaj w systemie obsługi reklamacji!");
            AppendLog("");
            AppendLog("Kliknij 'START' aby automatycznie wykryć:");
            AppendLog("  • REST API (synchronizacja danych)");
            AppendLog("  • Telefon Android (SMS i dzwonienie)");
            AppendLog("");
            AppendLog("Skanowanie sieci zajmie około 30-60 sekund.");
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnSkip.Enabled = false;
            progressBar.Visible = true;

            txtLog.Clear();
            AppendLog("═══════════════════════════════════════════");
            AppendLog("  AUTOMATYCZNA KONFIGURACJA - START");
            AppendLog("═══════════════════════════════════════════");
            AppendLog("");

            try
            {
                var result = await NetworkAutoDiscovery.AutoConfigureAsync(message =>
                {
                    AppendLog(message);
                });

                AppendLog("");
                AppendLog("═══════════════════════════════════════════");

                if (result.Success)
                {
                    ConfigurationSuccessful = true;
                    configurationComplete = true;

                    if (result.ApiFound && result.PhoneFound)
                    {
                        AppendLog("🎉 SUKCES! Wszystko skonfigurowane!", Color.LimeGreen);
                        AppendLog("");
                        AppendLog("Możesz teraz:");
                        AppendLog("  ✅ Synchronizować zgłoszenia z API");
                        AppendLog("  ✅ Wysyłać SMS przez telefon");
                        AppendLog("  ✅ Dzwonić przez telefon");
                        AppendLog("");
                        AppendLog("Formularz zamknie się automatycznie za 5 sekund...");

                        await Task.Delay(5000);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else if (result.ApiFound)
                    {
                        AppendLog("⚠️ CZĘŚCIOWY SUKCES", Color.Orange);
                        AppendLog("");
                        AppendLog("Znaleziono: REST API");
                        AppendLog("Nie znaleziono: Telefon Android");
                        AppendLog("");
                        AppendLog("Możesz synchronizować zgłoszenia, ale SMS i dzwonienie nie będzie działać.");
                        
                        btnStart.Enabled = true;
                        btnStart.Text = "🔄 Spróbuj ponownie";
                        btnSkip.Text = "Kontynuuj bez telefonu";
                    }
                    else if (result.PhoneFound)
                    {
                        AppendLog("⚠️ CZĘŚCIOWY SUKCES", Color.Orange);
                        AppendLog("");
                        AppendLog("Znaleziono: Telefon Android");
                        AppendLog("Nie znaleziono: REST API");
                        AppendLog("");
                        AppendLog("Możesz wysyłać SMS i dzwonić, ale synchronizacja nie będzie działać.");
                        
                        btnStart.Enabled = true;
                        btnStart.Text = "🔄 Spróbuj ponownie";
                        btnSkip.Text = "Kontynuuj bez API";
                    }
                }
                else
                {
                    AppendLog("❌ NIE ZNALEZIONO URZĄDZEŃ", Color.Red);
                    AppendLog("");
                    AppendLog("Sprawdź czy:");
                    AppendLog("  • REST API jest uruchomione (dotnet run)");
                    AppendLog("  • Telefon ma uruchomioną aplikację ENA");
                    AppendLog("  • Wszystkie urządzenia są w tej samej sieci Wi-Fi");
                    AppendLog("");
                    
                    btnStart.Enabled = true;
                    btnStart.Text = "🔄 Spróbuj ponownie";
                    btnSkip.Text = "Skonfiguruję ręcznie";
                }

                AppendLog("═══════════════════════════════════════════");
            }
            catch (Exception ex)
            {
                AppendLog("");
                AppendLog($"❌ BŁĄD: {ex.Message}", Color.Red);
                btnStart.Enabled = true;
                btnSkip.Enabled = true;
            }
            finally
            {
                progressBar.Visible = false;
                btnSkip.Enabled = true;
            }
        }

        private void BtnSkip_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Czy na pewno chcesz pominąć automatyczną konfigurację?\n\n" +
                "Będziesz musiał skonfigurować połączenia ręcznie:\n" +
                "• REST API - w menu 'Konfiguracja API'\n" +
                "• Telefon - w menu 'Paruj telefon'\n\n" +
                "Kontynuować?",
                "Potwierdzenie",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ConfigurationSuccessful = false;
                configurationComplete = true;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void FormAutoConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!configurationComplete)
            {
                var result = MessageBox.Show(
                    "Czy na pewno chcesz zamknąć konfigurację?\n\n" +
                    "Program uruchomi się, ale będziesz musiał skonfigurować połączenia ręcznie.",
                    "Potwierdzenie",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }

        private void AppendLog(string message, Color? color = null)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AppendLog(message, color)));
                return;
            }

            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = color ?? Color.LimeGreen;
            txtLog.AppendText(message + "\n");
            txtLog.SelectionColor = txtLog.ForeColor;
            txtLog.ScrollToCaret();
        }

        /// <summary>
        /// Sprawdza czy aplikacja została już skonfigurowana
        /// </summary>
        public static bool IsAlreadyConfigured()
        {
            try
            {
                string apiUrl = Properties.Settings.Default.ApiBaseUrl;
                string phoneIp = Properties.Settings.Default.PhoneIP;

                // Jeśli mamy zapisane API LUB telefon, uznajemy że aplikacja jest skonfigurowana
                return !string.IsNullOrEmpty(apiUrl) || !string.IsNullOrEmpty(phoneIp);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Uruchamia auto-konfigurację jeśli aplikacja nie jest skonfigurowana
        /// </summary>
        public static void RunIfNeeded()
        {
            if (!IsAlreadyConfigured())
            {
                var form = new FormAutoConfig();
                form.ShowDialog();
            }
        }
    }
}
