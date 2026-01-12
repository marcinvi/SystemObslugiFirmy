using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exception = System.Exception;

namespace Reklamacje_Dane
{
    public partial class FormDodajDzialanie : Form
    {
        // --- Pola z obu klas ---
        private readonly string _nrZgloszenia;
        private readonly DatabaseService _dbService;

        private string _clientEmail;
        private string _clientPhone;
        private string _allegroDisputeId;
        private int _allegroAccountId;

        private bool _sendEmail = false;
        private bool _sendSms = false;
        private bool _sendAllegro = false;

        // --- Konstruktor ---
        public FormDodajDzialanie(string nrZgloszenia)
        {
            InitializeComponent();
            _nrZgloszenia = nrZgloszenia;
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());

            // Ustawienia formularza
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = $"Dodaj działanie do zgłoszenia {_nrZgloszenia}";
            this.Load += FormDodajDzialanie_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // --- Metody ładowania danych (połączone) ---
        private async void FormDodajDzialanie_Load(object sender, EventArgs e)
        {
            // Równoległe ładowanie danych dla wydajności
            await Task.WhenAll(LoadTemplatesAsync(), LoadPrerequisitesAsync());

            // Ustawienie widoczności przycisków powiadomień
            btnToggleEmail.Visible = !string.IsNullOrEmpty(_clientEmail);
            btnToggleSms.Visible = !string.IsNullOrEmpty(_clientPhone);
            btnToggleAllegro.Visible = !string.IsNullOrEmpty(_allegroDisputeId);

            UpdateToggleButtonStates();
            txtAction.Focus();
        }

        private async Task LoadTemplatesAsync()
        {
            try
            {
                comboBoxTemplates.Items.Clear();
                comboBoxTemplates.Items.Add("-- Wybierz szablon --");

                var dt = await _dbService.GetDataTableAsync("SELECT Tresc FROM SzablonyDzialan ORDER BY Kolejnosc, Tresc");
                foreach (DataRow row in dt.Rows)
                {
                    comboBoxTemplates.Items.Add(row["Tresc"].ToString());
                }

                comboBoxTemplates.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się wczytać szablonów działań: " + ex.Message, "Błąd");
            }
        }

        private async Task LoadPrerequisitesAsync()
        {
            try
            {
                string query = @"SELECT k.Email, k.Telefon, z.allegroDisputeId, z.allegroAccountId
                                 FROM Zgloszenia z
                                 LEFT JOIN Klienci k ON z.KlientID = k.Id
                                 WHERE z.NrZgloszenia = @nrZgloszenia";

                var dt = await _dbService.GetDataTableAsync(query, new MySqlParameter("@nrZgloszenia", _nrZgloszenia));

                if (dt.Rows.Count > 0)
                {
                    _clientEmail = dt.Rows[0]["Email"]?.ToString();
                    _clientPhone = dt.Rows[0]["Telefon"]?.ToString();
                    _allegroDisputeId = dt.Rows[0]["allegroDisputeId"]?.ToString();
                    _allegroAccountId = dt.Rows[0]["allegroAccountId"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["allegroAccountId"]);
                }
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd wczytywania", "Błąd wczytywania danych klienta do powiadomień: " + ex.Message, NotificationType.Error);
            }
        }

        // --- Obsługa zdarzeń kontrolek ---
        private void comboBoxTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTemplates.SelectedIndex > 0)
            {
                txtAction.Text = comboBoxTemplates.SelectedItem.ToString();
            }
        }

        private void btnToggleEmail_Click(object sender, EventArgs e)
        {
            _sendEmail = !_sendEmail;
            UpdateToggleButtonStates();
        }

        private void btnToggleSms_Click(object sender, EventArgs e)
        {
            _sendSms = !_sendSms;
            UpdateToggleButtonStates();
        }

        private void btnToggleAllegro_Click(object sender, EventArgs e)
        {
            _sendAllegro = !_sendAllegro;
            UpdateToggleButtonStates();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // --- Główna logika zapisu (połączona) ---
        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAction.Text))
            {
                ToastManager.ShowToast("Błąd walidacji", "Treść działania nie może być pusta.", NotificationType.Warning);
                return;
            }

            btnSave.Enabled = false;
            btnCancel.Enabled = false;

            // Krok 1: Obsługa powiadomień
            bool notificationsHandled = await HandleNotifications();
            if (!notificationsHandled) // Użytkownik anulował edycję wiadomości
            {
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                return;
            }

            // Krok 2: Zapis do bazy danych z użyciem transakcji
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Zapis działania
                            Dzialaniee dzialanie = new Dzialaniee();
                            dzialanie.DodajNoweDzialanie(con, transaction, _nrZgloszenia, Program.fullName, txtAction.Text.Trim());

                            // Zapis do dziennika
                            DziennikLogger dziennik = new DziennikLogger();
                            await dziennik.DodajAsync(con, transaction, Program.fullName, "Dodano działanie: " + txtAction.Text.Trim(), _nrZgloszenia);

                            transaction.Commit();
                            ToastManager.ShowToast("Sukces", "Działanie zostało pomyślnie dodane.", NotificationType.Success);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Błąd zapisu do bazy danych.", ex);
                        }
                    }
                }

                UpdateManager.NotifySubscribers();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd krytyczny", "Wystąpił błąd podczas zapisywania działania: " + ex.Message, NotificationType.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        // --- Metody do obsługi powiadomień (z Form18) ---
        private void UpdateToggleButtonStates()
        {
            btnToggleEmail.BackColor = _sendEmail ? Color.FromArgb(33, 150, 243) : Color.LightGray;
            btnToggleEmail.ForeColor = _sendEmail ? Color.White : Color.Black;

            btnToggleSms.BackColor = _sendSms ? Color.FromArgb(33, 150, 243) : Color.LightGray;
            btnToggleSms.ForeColor = _sendSms ? Color.White : Color.Black;

            btnToggleAllegro.BackColor = _sendAllegro ? Color.FromArgb(33, 150, 243) : Color.LightGray;
            btnToggleAllegro.ForeColor = _sendAllegro ? Color.White : Color.Black;
        }

        private async Task<bool> HandleNotifications()
        {
            if (!_sendEmail && !_sendSms && !_sendAllegro)
            {
                return true; // Brak powiadomień do wysłania
            }

            string defaultMessage = $"Informujemy, że do zgłoszenia zostało dodane działanie: {txtAction.Text.Trim()}";
            string messageToSend = ShowEditMessageDialog(defaultMessage);

            if (messageToSend == null)
            {
                return false; // Użytkownik anulował wysyłkę
            }

            if (_sendEmail) await SendEmail(messageToSend);
            if (_sendSms) await SendSms(messageToSend);
            if (_sendAllegro) await SendAllegroMessage(messageToSend);

            return true;
        }

        private string ShowEditMessageDialog(string defaultMessage)
        {
            using (var form = new Form())
            using (var textBox = new TextBox())
            using (var buttonOk = new Button())
            using (var buttonCancel = new Button())
            {
                form.Text = "Edytuj treść powiadomienia";
                form.ClientSize = new System.Drawing.Size(500, 200);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;

                textBox.Text = defaultMessage;
                textBox.Multiline = true;
                textBox.Dock = DockStyle.Fill;
                textBox.ScrollBars = ScrollBars.Vertical;

                buttonOk.Text = "Wyślij";
                buttonOk.DialogResult = DialogResult.OK;
                buttonOk.Dock = DockStyle.Bottom;
                buttonOk.BackColor = Color.ForestGreen;
                buttonOk.FlatStyle = FlatStyle.Flat;
                buttonOk.ForeColor = Color.White;

                buttonCancel.Text = "Anuluj";
                buttonCancel.DialogResult = DialogResult.Cancel;
                buttonCancel.Dock = DockStyle.Bottom;
                buttonCancel.BackColor = Color.Gray;
                buttonCancel.FlatStyle = FlatStyle.Flat;
                buttonCancel.ForeColor = Color.White;

                form.Controls.Add(textBox);
                form.Controls.Add(buttonCancel);
                form.Controls.Add(buttonOk);
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }
        }

        private async Task SendEmail(string message)
        {
            try
            {
                var outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                if (outlookApp.Session.Accounts.Count == 0)
                {
                    ToastManager.ShowToast("Błąd Outlook", "Nie znaleziono kont e-mail w programie Outlook.", NotificationType.Warning);
                    return;
                }

                Account selectedAccount = ChooseOutlookAccount(outlookApp.Session.Accounts);
                if (selectedAccount == null) return;

                MailItem mail = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);
                mail.To = _clientEmail;
                mail.Subject = $"Aktualizacja w zgłoszeniu nr {_nrZgloszenia}";
                mail.Body = message;
                mail.SendUsingAccount = selectedAccount;
                mail.Display(false);
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd wysyłki e-mail", $"Nie udało się utworzyć wiadomości w Outlook.\n\nSzczegóły: {ex.Message}", NotificationType.Error);
            }
            await Task.CompletedTask;
        }

        private Account ChooseOutlookAccount(Accounts accounts)
        {
            if (accounts.Count == 1) return accounts[1];

            using (var form = new Form())
            {
                var listBox = new ListBox { Dock = DockStyle.Fill };
                foreach (Account account in accounts) { listBox.Items.Add(account.DisplayName); }
                listBox.SelectedIndex = 0;

                var buttonOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom, Padding = new Padding(5) };
                var labelInfo = new Label { Text = "Wybierz konto, z którego chcesz wysłać wiadomość:", Dock = DockStyle.Top, Padding = new Padding(10), AutoSize = true };

                form.Text = "Wybierz konto e-mail";
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.ClientSize = new System.Drawing.Size(400, 220);
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.Controls.AddRange(new Control[] { listBox, labelInfo, buttonOk });
                form.AcceptButton = buttonOk;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string selectedName = listBox.SelectedItem.ToString();
                    return accounts.Cast<Account>().FirstOrDefault(acc => acc.DisplayName == selectedName);
                }
            }
            return null;
        }

        private async Task SendSms(string message)
        {
            if (string.IsNullOrWhiteSpace(_clientPhone))
            {
                ToastManager.ShowToast("Błąd wysyłki SMS", "Brak numeru telefonu dla tego klienta.", NotificationType.Warning);
                return;
            }
            try
            {
                string encodedMessage = Uri.EscapeDataString(message);
                string phoneNumberDigitsOnly = new string(_clientPhone.Where(char.IsDigit).ToArray());
                string uri = $"sms:{phoneNumberDigitsOnly}?body={encodedMessage}";
                Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd automatyzacji", "Nie udało się uruchomić aplikacji do wysyłania SMS. Upewnij się, że 'Łącze z telefonem' jest skonfigurowane." + ex.Message, NotificationType.Error);
            }
            await Task.CompletedTask;
        }

        private async Task SendAllegroMessage(string message)
        {
            if (string.IsNullOrEmpty(_allegroDisputeId)) return;
            try
            {
                using (var con = Database.GetNewOpenConnection())
                {
                    var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(_allegroAccountId, con);
                    if (apiClient == null)
                    {
                        ToastManager.ShowToast("Błąd Allegro", "Nie udało się uzyskać klienta API dla konta Allegro.", NotificationType.Error);
                        return;
                    }
                    var request = new NewMessageRequest
                    {
                        Text = message,
                        Type = "REGULAR",
                        Attachments = new List<NewMessageAttachment>()
                    };
                    await apiClient.SendMessageAsync(_allegroDisputeId, request);
                    ToastManager.ShowToast("Sukces", "Wiadomość została wysłana w dyskusji Allegro.", NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd krytyczny", $"Błąd podczas wysyłania wiadomości na Allegro: {ex.Message}", NotificationType.Error);
            }
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