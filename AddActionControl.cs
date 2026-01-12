// Plik: AddActionControl.cs
using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Reklamacje_Dane
{
    public partial class AddActionControl : UserControl
    {
        private string _nrZgloszenia;
        private string _clientEmail;
        private string _clientPhone;
        private string _allegroDisputeId;
        private int _allegroAccountId;

        // Flagi stanu dla przełączników
        private bool _sendEmail = false;
        private bool _sendSms = false;
        private bool _sendAllegro = false;

        public event EventHandler ActionAdded;

        public AddActionControl()
        {
            InitializeComponent();
            this.Visible = false;
        }

        public async Task PrepareAndShow(string nrZgloszenia)
        {
            this.Visible = false;
            this._nrZgloszenia = nrZgloszenia;
            await LoadPrerequisites();

            btnToggleEmail.Visible = !string.IsNullOrEmpty(_clientEmail);
            btnToggleSms.Visible = !string.IsNullOrEmpty(_clientPhone);
            btnToggleAllegro.Visible = !string.IsNullOrEmpty(_allegroDisputeId);

            txtAction.Clear();
            _sendEmail = false;
            _sendSms = false;
            _sendAllegro = false;
            UpdateToggleButtonStates();

            this.Visible = true;
            this.BringToFront();
            txtAction.Focus();
        }

        private void UpdateToggleButtonStates()
        {
            btnToggleEmail.BackColor = _sendEmail ? Color.FromArgb(33, 150, 243) : Color.LightGray;
            btnToggleEmail.ForeColor = _sendEmail ? Color.White : Color.Black;

            btnToggleSms.BackColor = _sendSms ? Color.FromArgb(33, 150, 243) : Color.LightGray;
            btnToggleSms.ForeColor = _sendSms ? Color.White : Color.Black;

            btnToggleAllegro.BackColor = _sendAllegro ? Color.FromArgb(33, 150, 243) : Color.LightGray;
            btnToggleAllegro.ForeColor = _sendAllegro ? Color.White : Color.Black;
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

        private async Task LoadPrerequisites()
        {
            try
            {
                var dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
                var data = await dbService.GetClientNotificationDataAsync(_nrZgloszenia);

                if (data != null)
                {
                    _clientEmail = data.Email;
                    _clientPhone = data.Telefon;
                    _allegroDisputeId = data.AllegroDisputeId;
                    _allegroAccountId = data.AllegroAccountId;
                }
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd wczytywania", "Błąd wczytywania danych klienta do powiadomień: " + ex.Message, NotificationType.Error);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAction.Text))
            {
                ToastManager.ShowToast("Błąd walidacji", "Treść działania nie może być pusta.", NotificationType.Warning);
                return;
            }

            btnSave.Enabled = false;
            btnCancel.Enabled = false;

            bool notificationsSent = await HandleNotifications();
            if (!notificationsSent)
            {
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                return;
            }

            try
            {
                // Używamy transakcji dla atomowości operacji zapisu
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            Dzialaniee dzialanie = new Dzialaniee();
                            dzialanie.DodajNoweDzialanie(con, transaction, _nrZgloszenia, Program.fullName, txtAction.Text);

                            DziennikLogger dziennik = new DziennikLogger();
                            await dziennik.DodajAsync(con, transaction, Program.fullName, "Dodano działanie: " + txtAction.Text, _nrZgloszenia);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Błąd zapisu do bazy danych.", ex);
                        }
                    }
                }

                UpdateManager.NotifySubscribers();
                ActionAdded?.Invoke(this, EventArgs.Empty);
                this.Visible = false;
                ToastManager.ShowToast("Sukces", "Działanie zostało pomyślnie dodane.", NotificationType.Success);
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

        private async Task<bool> HandleNotifications()
        {
            if (!_sendEmail && !_sendSms && !_sendAllegro)
            {
                return true;
            }

            string defaultMessage = $"Informujemy, że do zgłoszenia zostało dodane działanie: {txtAction.Text}";
            string messageToSend = ShowEditMessageDialog(defaultMessage);

            if (messageToSend == null)
            {
                return false;
            }

            if (_sendEmail) await SendEmail(messageToSend);
            if (_sendSms) await SendSms(messageToSend);
            if (_sendAllegro) await SendAllegroMessage(messageToSend);

            return true;
        }

        private string ShowEditMessageDialog(string defaultMessage)
        {
            using (var form = new Form())
            {
                using (var textBox = new TextBox())
                using (var buttonOk = new Button())
                using (var buttonCancel = new Button())
                {
                    form.Text = "Edytuj treść powiadomienia";
                    form.ClientSize = new System.Drawing.Size(500, 200);
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog; // Ulepszenie: stały rozmiar

                    textBox.Text = defaultMessage;
                    textBox.Multiline = true;
                    textBox.Dock = DockStyle.Fill;
                    textBox.ScrollBars = ScrollBars.Vertical; // Ulepszenie: scrollbar

                    buttonOk.Text = "Wyślij";
                    buttonOk.DialogResult = DialogResult.OK;
                    buttonOk.Dock = DockStyle.Bottom;
                    buttonOk.BackColor = Color.ForestGreen; // Ulepszenie: spójna kolorystyka
                    buttonOk.FlatStyle = FlatStyle.Flat;
                    buttonOk.ForeColor = Color.White;

                    buttonCancel.Text = "Anuluj";
                    buttonCancel.DialogResult = DialogResult.Cancel;
                    buttonCancel.Dock = DockStyle.Bottom;
                    buttonCancel.BackColor = Color.Gray; // Ulepszenie: spójna kolorystyka
                    buttonCancel.FlatStyle = FlatStyle.Flat;
                    buttonCancel.ForeColor = Color.White;

                    form.Controls.Add(textBox);
                    form.Controls.Add(buttonCancel);
                    form.Controls.Add(buttonOk);
                    form.AcceptButton = buttonOk;
                    form.CancelButton = buttonCancel;

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        return textBox.Text;
                    }
                    return null;
                }
            }
        }

        private async Task SendEmail(string message)
        {
            // SYMULACJA
            ToastManager.ShowToast("Symulacja E-mail", $"Przygotowano wiadomość do {_clientEmail}.", NotificationType.Info);
            await Task.CompletedTask;
        }

        private async Task SendSms(string message)
        {
            // SYMULACJA
            ToastManager.ShowToast("Symulacja SMS", $"Przygotowano wiadomość do {_clientPhone}.", NotificationType.Info);
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}