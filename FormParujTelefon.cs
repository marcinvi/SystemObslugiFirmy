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
            this.ClientSize = new Size(500, 380);
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

            if (string.IsNullOrEmpty(ip))
            {
                UpdateStatus("⚠️ Wpisz adres IP telefonu!", Color.Orange);
                txtIpTelefonu.Focus();
                return;
            }

            if (string.IsNullOrEmpty(kod))
            {
                UpdateStatus("⚠️ Wpisz kod parowania z aplikacji Android!", Color.Orange);
                txtKodParowania.Focus();
                return;
            }

            if (kod.Length != 6)
            {
                UpdateStatus("⚠️ Kod parowania musi mieć 6 znaków!", Color.Orange);
                txtKodParowania.Focus();
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

                    UpdateStatus("✅ SPAROWANO POMYŚLNIE!", Color.Green);

                    MessageBox.Show(
                        "Telefon został pomyślnie sparowany!\n\n" +
                        $"IP telefonu: {ip}\n" +
                        "Możesz teraz wysyłać SMS i dzwonić z poziomu aplikacji.",
                        "Sukces",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

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
            finally
            {
                progressBar.Visible = false;
                SetControlsEnabled(true);
            }
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
