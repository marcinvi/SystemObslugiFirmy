using System;
using System.Drawing;
using System.Windows.Forms;
using MailKit.Net.Pop3;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Reklamacje_Dane
{
    public partial class FormDodajKonto : Form
    {
        // POLA FORMULARZA
        private TextBox txtNazwa, txtEmail, txtLogin, txtHaslo;
        private ComboBox cmbProtokol;
        private TextBox txtIncomingHost, txtIncomingPort, txtSmtpHost, txtSmtpPort;
        private CheckBox chkIncomingSsl, chkSmtpSsl, chkDomyslne;

        // Pole podpisu jest ReadOnly, edycja przez guzik
        private TextBox txtPodpis;
        private Button btnTestuj, btnZapisz, btnAnuluj, btnEdytujPodpis;
        private Label lblStatus, lblIncomingHost;

        // ID edytowanego konta (null jeśli nowe)
        private int? _edytowaneKontoId = null;
        private ContactRepository _repo = new ContactRepository();

        // KONSTRUKTOR
        public FormDodajKonto(KontoPocztowe kontoDoEdycji = null)
        {
            InitializeComponent_Manual();

            if (kontoDoEdycji != null)
            {
                // --- TRYB EDYCJI ---
                this.Text = "Edycja Konta Pocztowego";
                _edytowaneKontoId = kontoDoEdycji.Id;

                txtNazwa.Text = kontoDoEdycji.NazwaWyswietlana;
                txtEmail.Text = kontoDoEdycji.AdresEmail;
                txtLogin.Text = kontoDoEdycji.Login;
                txtHaslo.Text = kontoDoEdycji.Haslo;

                // Ustawienie protokołu
                if (!string.IsNullOrEmpty(kontoDoEdycji.Protokol))
                    cmbProtokol.SelectedItem = kontoDoEdycji.Protokol;
                else
                    cmbProtokol.SelectedIndex = 0; // Domyślnie POP3

                // Wypełnienie hostów
                if (kontoDoEdycji.Protokol == "IMAP")
                {
                    txtIncomingHost.Text = kontoDoEdycji.ImapHost;
                    txtIncomingPort.Text = kontoDoEdycji.ImapPort.ToString();
                    chkIncomingSsl.Checked = kontoDoEdycji.ImapSsl;
                }
                else
                {
                    txtIncomingHost.Text = kontoDoEdycji.Pop3Host;
                    txtIncomingPort.Text = kontoDoEdycji.Pop3Port.ToString();
                    chkIncomingSsl.Checked = kontoDoEdycji.Pop3Ssl;
                }

                txtSmtpHost.Text = kontoDoEdycji.SmtpHost;
                txtSmtpPort.Text = kontoDoEdycji.SmtpPort.ToString();
                chkSmtpSsl.Checked = kontoDoEdycji.SmtpSsl;

                txtPodpis.Text = kontoDoEdycji.Podpis;
                chkDomyslne.Checked = kontoDoEdycji.CzyDomyslne;

                // W trybie edycji przycisk zapisu aktywny od razu
                btnZapisz.Enabled = true;
            }
            else
            {
                // --- TRYB DODAWANIA ---
                this.Text = "Dodaj Nowe Konto";
                cmbProtokol.SelectedIndex = 0; // Domyślnie POP3
            }
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // --- ZDARZENIA (EVENTS) ---

        // 1. Otwieranie edytora podpisu (Nowy Formularz)
        private void BtnEdytujPodpis_Click(object sender, EventArgs e)
        {
            using (var editor = new FormEdytorPodpisu(txtPodpis.Text))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    txtPodpis.Text = editor.WynikowyHtml;
                }
            }
        }

        // 2. Zmiana protokołu (POP3 / IMAP)
        private void CmbProtokol_SelectedIndexChanged(object sender, EventArgs e)
        {
            string wybor = cmbProtokol.SelectedItem.ToString();

            if (wybor == "IMAP")
            {
                lblIncomingHost.Text = "IMAP Host:";
                // Sugerowana zmiana portu
                if (txtIncomingPort.Text == "995") txtIncomingPort.Text = "993";
            }
            else // POP3
            {
                lblIncomingHost.Text = "POP3 Host:";
                // Sugerowana zmiana portu
                if (txtIncomingPort.Text == "993") txtIncomingPort.Text = "995";
            }
        }

        // 3. Autokonfiguracja po wpisaniu maila
        private void TxtEmail_Leave(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(email) || !email.Contains("@")) return;

            // Domyślnie login to email
            if (string.IsNullOrEmpty(txtLogin.Text)) txtLogin.Text = email;

            // === PEŁNA AUTOKONFIGURACJA ===

            if (email.EndsWith("@enatruck.com") || email.EndsWith("@enatruck.pl"))
            {
                cmbProtokol.SelectedItem = "POP3";
                txtIncomingHost.Text = "pop3.enatruck.pl";
                txtIncomingPort.Text = "995";
                chkIncomingSsl.Checked = true;

                txtSmtpHost.Text = "smtp.enatruck.pl";
                txtSmtpPort.Text = "587";
                chkSmtpSsl.Checked = true;
            }
            else if (email.EndsWith("@gmail.com"))
            {
                cmbProtokol.SelectedItem = "IMAP"; // Gmail woli IMAP
                txtIncomingHost.Text = "imap.gmail.com";
                txtIncomingPort.Text = "993";
                chkIncomingSsl.Checked = true;

                txtSmtpHost.Text = "smtp.gmail.com";
                txtSmtpPort.Text = "587";
                chkSmtpSsl.Checked = true; // Gmail wymaga STARTTLS na 587 lub SSL na 465
            }
            else if (email.EndsWith("@wp.pl"))
            {
                cmbProtokol.SelectedItem = "IMAP";
                txtIncomingHost.Text = "imap.wp.pl";
                txtIncomingPort.Text = "993";
                chkIncomingSsl.Checked = true;

                txtSmtpHost.Text = "smtp.wp.pl";
                txtSmtpPort.Text = "465";
                chkSmtpSsl.Checked = true;
            }
            else if (email.EndsWith("@onet.pl"))
            {
                cmbProtokol.SelectedItem = "IMAP";
                txtIncomingHost.Text = "imap.poczta.onet.pl";
                txtIncomingPort.Text = "993";
                chkIncomingSsl.Checked = true;

                txtSmtpHost.Text = "smtp.poczta.onet.pl";
                txtSmtpPort.Text = "465";
                chkSmtpSsl.Checked = true;
            }
            else if (email.EndsWith("@interia.pl"))
            {
                cmbProtokol.SelectedItem = "IMAP";
                txtIncomingHost.Text = "poczta.interia.pl";
                txtIncomingPort.Text = "993";
                chkIncomingSsl.Checked = true;

                txtSmtpHost.Text = "poczta.interia.pl";
                txtSmtpPort.Text = "465";
                chkSmtpSsl.Checked = true;
            }
        }

        // 4. Testowanie Połączenia (PEŁNY KOD)
        private async void BtnTestuj_Click(object sender, EventArgs e)
        {
            btnTestuj.Enabled = false;
            lblStatus.ForeColor = Color.Blue;
            string protokol = cmbProtokol.SelectedItem.ToString();
            lblStatus.Text = $"Testowanie {protokol} i SMTP...";

            try
            {
                // ====================================================
                // 1. TEST ODBIERANIA (POP3 / IMAP)
                // ====================================================
                if (protokol == "IMAP")
                {
                    using (var client = new ImapClient())
                    {
                        // Dla Portu 993 wymuszamy SSL, dla 143 Auto
                        var opcjeSsl = int.Parse(txtIncomingPort.Text) == 993 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto;

                        // Wyłączamy walidację certyfikatu (dla pewności przy testach)
                        client.ServerCertificateValidationCallback = (s, c, h, ex) => true;

                        await client.ConnectAsync(txtIncomingHost.Text, int.Parse(txtIncomingPort.Text), opcjeSsl);
                        await client.AuthenticateAsync(txtLogin.Text, txtHaslo.Text);
                        await client.DisconnectAsync(true);
                    }
                }
                else // POP3
                {
                    using (var client = new Pop3Client())
                    {
                        // Dla Portu 995 wymuszamy SSL, dla 110 Auto
                        var opcjeSsl = int.Parse(txtIncomingPort.Text) == 995 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto;

                        client.ServerCertificateValidationCallback = (s, c, h, ex) => true;

                        await client.ConnectAsync(txtIncomingHost.Text, int.Parse(txtIncomingPort.Text), opcjeSsl);
                        await client.AuthenticateAsync(txtLogin.Text, txtHaslo.Text);
                        await client.DisconnectAsync(true);
                    }
                }

                // ====================================================
                // 2. TEST WYSYŁANIA (SMTP) - TU BYŁ BŁĄD 535
                // ====================================================
                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, ex) => true;

                    int port = int.Parse(txtSmtpPort.Text);

                    // Logika doboru szyfrowania dla SMTP
                    SecureSocketOptions opcjeSsl;
                    if (port == 465) opcjeSsl = SecureSocketOptions.SslOnConnect; // Wymuszone SSL
                    else if (port == 587) opcjeSsl = SecureSocketOptions.StartTls; // Wymuszone StartTLS
                    else opcjeSsl = SecureSocketOptions.Auto;

                    await client.ConnectAsync(txtSmtpHost.Text, port, opcjeSsl);

                    // Tutaj następuje logowanie - jeśli tu padnie 535, to Login/Hasło są złe
                    await client.AuthenticateAsync(txtLogin.Text, txtHaslo.Text);

                    await client.DisconnectAsync(true);
                }

                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Sukces! Login i Hasło poprawne.";
                btnZapisz.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = "Błąd: " + ex.Message;

                string porada = "";
                if (ex.Message.Contains("535"))
                    porada = "\n\nPORADA: Serwer odrzucił hasło lub login.\n1. Sprawdź czy w polu 'Login' jest PEŁNY adres e-mail.\n2. Sprawdź czy nie masz spacji na końcu hasła.";

                MessageBox.Show($"Wystąpił błąd połączenia:\n{ex.Message}{porada}", "Błąd Testu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTestuj.Enabled = true;
            }
        }

        // 5. Zapisywanie do bazy
        private void BtnZapisz_Click(object sender, EventArgs e)
        {
            try
            {
                string protokol = cmbProtokol.SelectedItem.ToString();

                var konto = new KontoPocztowe
                {
                    // Używamy ID jeśli edycja, 0 jeśli nowe
                    Id = _edytowaneKontoId ?? 0,

                    NazwaWyswietlana = txtNazwa.Text,
                    AdresEmail = txtEmail.Text,
                    Login = txtLogin.Text,
                    Haslo = txtHaslo.Text,
                    Protokol = protokol,

                    // Zapisujemy hosty zależnie od wyboru
                    Pop3Host = (protokol == "POP3") ? txtIncomingHost.Text : "",
                    Pop3Port = (protokol == "POP3") ? int.Parse(txtIncomingPort.Text) : 0,
                    Pop3Ssl = (protokol == "POP3") ? chkIncomingSsl.Checked : false,

                    ImapHost = (protokol == "IMAP") ? txtIncomingHost.Text : "",
                    ImapPort = (protokol == "IMAP") ? int.Parse(txtIncomingPort.Text) : 0,
                    ImapSsl = (protokol == "IMAP") ? chkIncomingSsl.Checked : false,

                    SmtpHost = txtSmtpHost.Text,
                    SmtpPort = int.Parse(txtSmtpPort.Text),
                    SmtpSsl = chkSmtpSsl.Checked,

                    Podpis = txtPodpis.Text, // Tu trafia HTML z edytora
                    CzyDomyslne = chkDomyslne.Checked
                };

                _repo.EnsureEmailTableExists();

                if (_edytowaneKontoId.HasValue)
                {
                    _repo.AktualizujKonto(konto);
                    MessageBox.Show("Dane konta zostały zaktualizowane!");
                }
                else
                {
                    _repo.DodajKontoPocztowe(konto);
                    MessageBox.Show("Nowe konto zostało dodane!");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu do bazy: " + ex.Message);
            }
        }

        // --- DESIGNER FORMULARZA (Kompletny) ---
        private void InitializeComponent_Manual()
        {
            this.Text = "Kreator Konta Pocztowego";
            this.Size = new Size(500, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            int inputW = 320;

            // --- SEKCJA 1: DANE LOGOWANIA ---
            this.Controls.Add(new Label { Text = "Nazwa konta (np. Biuro):", Location = new Point(20, y + 3), AutoSize = true });
            txtNazwa = new TextBox { Location = new Point(160, y), Width = inputW };
            this.Controls.Add(txtNazwa);
            y += 40;

            this.Controls.Add(new Label { Text = "Adres E-mail:", Location = new Point(20, y + 3), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(160, y), Width = inputW };
            txtEmail.Leave += TxtEmail_Leave; // Podpinamy autokonfigurację
            this.Controls.Add(txtEmail);
            y += 40;

            this.Controls.Add(new Label { Text = "Login:", Location = new Point(20, y + 3), AutoSize = true });
            txtLogin = new TextBox { Location = new Point(160, y), Width = inputW };
            this.Controls.Add(txtLogin);
            y += 40;

            this.Controls.Add(new Label { Text = "Hasło:", Location = new Point(20, y + 3), AutoSize = true });
            txtHaslo = new TextBox { Location = new Point(160, y), Width = inputW, PasswordChar = '*' };
            this.Controls.Add(txtHaslo);
            y += 50;

            // --- SEKCJA 2: SERWERY ---
            GroupBox grp = new GroupBox { Text = "Konfiguracja Serwera", Location = new Point(20, y), Size = new Size(440, 220) };

            // Protokół
            grp.Controls.Add(new Label { Text = "Typ konta:", Location = new Point(10, 30), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) });
            cmbProtokol = new ComboBox { Location = new Point(90, 27), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbProtokol.Items.AddRange(new object[] { "POP3", "IMAP" });
            cmbProtokol.SelectedIndexChanged += CmbProtokol_SelectedIndexChanged;
            grp.Controls.Add(cmbProtokol);

            // Incoming
            lblIncomingHost = new Label { Text = "POP3 Host:", Location = new Point(10, 70), AutoSize = true };
            grp.Controls.Add(lblIncomingHost);

            txtIncomingHost = new TextBox { Location = new Point(90, 67), Width = 150 };
            grp.Controls.Add(txtIncomingHost);

            grp.Controls.Add(new Label { Text = "Port:", Location = new Point(250, 70), AutoSize = true });
            txtIncomingPort = new TextBox { Location = new Point(290, 67), Width = 50, Text = "995" };
            grp.Controls.Add(txtIncomingPort);

            chkIncomingSsl = new CheckBox { Text = "SSL", Location = new Point(350, 67), Checked = true };
            grp.Controls.Add(chkIncomingSsl);

            // Outgoing (SMTP)
            grp.Controls.Add(new Label { Text = "SMTP Host:", Location = new Point(10, 110), AutoSize = true });
            txtSmtpHost = new TextBox { Location = new Point(90, 107), Width = 150 };
            grp.Controls.Add(txtSmtpHost);

            grp.Controls.Add(new Label { Text = "Port:", Location = new Point(250, 110), AutoSize = true });
            txtSmtpPort = new TextBox { Location = new Point(290, 107), Width = 50, Text = "587" };
            grp.Controls.Add(txtSmtpPort);

            chkSmtpSsl = new CheckBox { Text = "SSL", Location = new Point(350, 107), Checked = true };
            grp.Controls.Add(chkSmtpSsl);

            // Domyślne
            chkDomyslne = new CheckBox { Text = "Ustaw jako konto domyślne", Location = new Point(10, 150), AutoSize = true };
            grp.Controls.Add(chkDomyslne);

            this.Controls.Add(grp);
            y += 240;

            // --- SEKCJA 3: PODPIS ---
            this.Controls.Add(new Label { Text = "Podpis wiadomości (HTML):", Location = new Point(20, y), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) });

            // Przycisk edycji podpisu
            btnEdytujPodpis = new Button { Text = "📝 Otwórz Edytor Podpisu", Location = new Point(260, y - 5), Width = 200, Height = 30, BackColor = Color.LightYellow };
            btnEdytujPodpis.Click += BtnEdytujPodpis_Click;
            this.Controls.Add(btnEdytujPodpis);
            y += 35;

            // Pole podpisu (ReadOnly)
            txtPodpis = new TextBox { Location = new Point(20, y), Width = 440, Height = 100, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true, BackColor = Color.WhiteSmoke };
            this.Controls.Add(txtPodpis);
            y += 110;

            // --- SEKCJA 4: PRZYCISKI ---
            lblStatus = new Label { Text = "Wybierz protokół, wpisz hasło i kliknij Testuj", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.Controls.Add(lblStatus);
            y += 30;

            btnTestuj = new Button { Text = "1. Testuj Połączenie", Location = new Point(20, y), Width = 150, Height = 40, BackColor = Color.LightGray };
            btnTestuj.Click += BtnTestuj_Click;

            btnZapisz = new Button { Text = "2. ZAPISZ KONTO", Location = new Point(190, y), Width = 150, Height = 40, BackColor = Color.SteelBlue, ForeColor = Color.White, Enabled = false, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnZapisz.Click += BtnZapisz_Click;

            btnAnuluj = new Button { Text = "Anuluj", Location = new Point(360, y), Width = 100, Height = 40 };
            btnAnuluj.Click += (s, ev) => this.Close();

            this.Controls.Add(btnTestuj);
            this.Controls.Add(btnZapisz);
            this.Controls.Add(btnAnuluj);
        }
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                // Włącz sprawdzanie pisowni dla wszystkich kontrolek typu TextBox i RichTextBox
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        // Dla zwykłych TextBoxów - bez podkreślania (bo nie obsługują kolorów)
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
            }
        }

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;

                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control))
                    {
                        yield return child;
                    }
                }
            }
        }
}
}