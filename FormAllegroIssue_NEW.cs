using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Nowoczesny, czytelny formularz zarządzania dyskusją Allegro z ulepszoną UX.
    /// Wersja 2.0 - Całkowicie przeprojektowana
    /// </summary>
    public partial class FormAllegroIssue : Form
    {
        private readonly string _issueId;
        private AllegroApiClient _apiClient;
        private int _accountId;
        private string _internalComplaintNumber;
        private static readonly HttpClient _fileDownloader = new HttpClient();
        private bool _isLoadingChat = false;

        // UI theme colors
        private static readonly Color PRIMARY_COLOR = Color.FromArgb(0, 120, 215);
        private static readonly Color SUCCESS_COLOR = Color.FromArgb(16, 124, 16);
        private static readonly Color WARNING_COLOR = Color.FromArgb(255, 185, 0);
        private static readonly Color DANGER_COLOR = Color.FromArgb(196, 43, 28);
        private static readonly Color BACKGROUND_COLOR = Color.FromArgb(243, 242, 241);
        private static readonly Color CARD_COLOR = Color.White;

        public FormAllegroIssue(string issueId)
        {
            _issueId = issueId ?? throw new ArgumentNullException(nameof(issueId));
            InitializeComponent();
            CustomizeUI();
            Text = $"Dyskusja Allegro: {_issueId}";
        }

        #region Initialization

        private void CustomizeUI()
        {
            // Główne okno
            this.Font = new Font("Segoe UI", 9.5F);
            this.BackColor = BACKGROUND_COLOR;
            this.MinimumSize = new Size(1200, 700);
            this.WindowState = FormWindowState.Maximized;

            // Styl przycisków
            StyleButton(btnSendMessage, PRIMARY_COLOR);
            StyleButton(btnAddAttachment, PRIMARY_COLOR);
            StyleButton(btnChangeStatus, SUCCESS_COLOR);
            StyleButton(btnReturnRequiredCustom, WARNING_COLOR);
            StyleButton(btnReturnNotRequired, SUCCESS_COLOR);
            StyleButton(btnEndRequest, PRIMARY_COLOR);
            StyleButton(btnViewOrder, PRIMARY_COLOR);
            StyleButton(btnAddTrackingNumber, PRIMARY_COLOR);

            // Tooltips
            var tooltip = new ToolTip();
            tooltip.SetToolTip(btnSendMessage, "Wyślij wiadomość do kupującego");
            tooltip.SetToolTip(btnAddAttachment, "Dodaj załącznik (max 10MB)");
            tooltip.SetToolTip(btnViewOrder, "Zobacz szczegóły zamówienia na Allegro");
            tooltip.SetToolTip(btnReturnRequiredCustom, "Wymagaj zwrotu na koszt kupującego (72h na decyzję)");
            tooltip.SetToolTip(btnReturnNotRequired, "Zwrot nie jest wymagany - sprzedający pokryje koszty");
            tooltip.SetToolTip(btnEndRequest, "Poproś kupującego o zakończenie dyskusji");

            // Placeholder text
            txtNewMessage.ForeColor = Color.Gray;
            txtNewMessage.Text = "Napisz wiadomość do kupującego...";
            txtNewMessage.Enter += (s, e) => {
                if (txtNewMessage.Text == "Napisz wiadomość do kupującego...")
                {
                    txtNewMessage.Text = "";
                    txtNewMessage.ForeColor = Color.Black;
                }
            };
            txtNewMessage.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtNewMessage.Text))
                {
                    txtNewMessage.Text = "Napisz wiadomość do kupującego...";
                    txtNewMessage.ForeColor = Color.Gray;
                }
            };

            txtStatusMessage.ForeColor = Color.Gray;
            txtStatusMessage.Text = "Opcjonalnie: dodaj komentarz do zmiany statusu...";
            txtStatusMessage.Enter += (s, e) => {
                if (txtStatusMessage.Text == "Opcjonalnie: dodaj komentarz do zmiany statusu...")
                {
                    txtStatusMessage.Text = "";
                    txtStatusMessage.ForeColor = Color.Black;
                }
            };
            txtStatusMessage.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtStatusMessage.Text))
                {
                    txtStatusMessage.Text = "Opcjonalnie: dodaj komentarz do zmiany statusu...";
                    txtStatusMessage.ForeColor = Color.Gray;
                }
            };

            // GroupBox styling
            foreach (Control ctrl in pnlActions.Controls.OfType<GroupBox>())
            {
                StyleGroupBox((GroupBox)ctrl);
            }

            // Chat history styling
            pnlChatHistory.BackColor = BACKGROUND_COLOR;
        }

        private void StyleButton(Button btn, Color bgColor)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = bgColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Padding = new Padding(10, 5, 10, 5);

            // Hover effect
            btn.MouseEnter += (s, e) => {
                var b = (Button)s;
                b.BackColor = ControlPaint.Light(b.BackColor, 0.1f);
            };
            btn.MouseLeave += (s, e) => {
                var b = (Button)s;
                b.BackColor = bgColor;
            };
        }

        private void StyleGroupBox(GroupBox gb)
        {
            gb.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            gb.ForeColor = Color.FromArgb(50, 50, 50);
        }

        private async void FormAllegroIssue_Load(object sender, EventArgs e)
        {
            try
            {
                ShowLoadingOverlay(true);

                var account = await GetAccountForIssueAsync(_issueId);
                if (account == null || string.IsNullOrWhiteSpace(account.ClientId) || string.IsNullOrWhiteSpace(account.ClientSecret))
                {
                    ShowError("Nie można załadować konta Allegro dla tej dyskusji.", "Błąd krytyczny");
                    Close();
                    return;
                }

                _accountId = account.Id;
                _apiClient = new AllegroApiClient(account.ClientId, account.ClientSecret);
                await _apiClient.InitializeAsync(account.Id);

                await LoadIssueDetailsAsync();
                PopulateStatusComboBox();
                await LoadChatHistoryAsync();
                await MarkIssueAsRead();

                ShowLoadingOverlay(false);
            }
            catch (Exception ex)
            {
                ShowLoadingOverlay(false);
                HandleApiError(ex, "inicjalizacji formularza");
                Close();
            }
        }

        #endregion

        #region Data Loading

        private async Task LoadIssueDetailsAsync()
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                var cmd = new MySqlCommand(
                    "SELECT ad.*, z.NrZgloszenia " +
                    "FROM AllegroDisputes ad " +
                    "LEFT JOIN Zgloszenia z ON ad.ComplaintId = z.Id " +
                    "WHERE ad.DisputeId = @id", con);
                cmd.Parameters.AddWithValue("@id", _issueId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        ShowError("Nie znaleziono danych dla tego zgłoszenia w lokalnej bazie.", "Błąd");
                        Close();
                        return;
                    }

                    // Check if we need to switch account
                    if (reader["AllegroAccountId"] != DBNull.Value)
                    {
                        var accIdFromDb = Convert.ToInt32(reader["AllegroAccountId"]);
                        if (accIdFromDb != _accountId)
                        {
                            var acc = await GetAccountByIdAsync(accIdFromDb);
                            if (acc != null)
                            {
                                _accountId = acc.Id;
                                _apiClient = new AllegroApiClient(acc.ClientId, acc.ClientSecret);
                                await _apiClient.InitializeAsync(acc.Id);
                            }
                        }
                    }

                    // Update header
                    lblDisputeId.Text = _issueId;
                    lblBuyerLogin.Text = reader["BuyerLogin"] != DBNull.Value ? reader["BuyerLogin"].ToString() : "Brak";

                    _internalComplaintNumber = reader["NrZgloszenia"] != DBNull.Value ? reader["NrZgloszenia"].ToString() : "Brak";
                    lblInternalComplaintId.Text = _internalComplaintNumber;

                    lblProductName.Text = reader["Subject"] != DBNull.Value ? reader["Subject"].ToString() : "Nieznany";

                    string status = reader["StatusAllegro"] != DBNull.Value ? reader["StatusAllegro"].ToString() : "";
                    UpdateStatusDisplay(status);

                    DateTime openedUtc = DateTime.UtcNow;
                    if (reader["OpenedAt"] != DBNull.Value)
                    {
                        if (DateTime.TryParse(reader["OpenedAt"].ToString(), out DateTime opened))
                            openedUtc = DateTime.SpecifyKind(opened, DateTimeKind.Utc);
                    }

                    UpdateDeadlineDisplays(openedUtc);
                }
            }
        }

        private void UpdateStatusDisplay(string status)
        {
            var (translated, color) = TranslateStatus(status);
            lblCurrentStatus.Text = translated;
            lblCurrentStatus.ForeColor = color;
        }

        private void UpdateDeadlineDisplays(DateTime openedUtc)
        {
            var nowUtc = DateTime.UtcNow;
            var hoursSinceOpen = Math.Max(0, (int)(nowUtc - openedUtc).TotalHours);

            // Decision deadline (3 days)
            var decisionDue = openedUtc.AddDays(3);
            var decisionLeft = decisionDue - nowUtc;
            var allowDecision = IsDecisionStillAllowed(openedUtc);

            progressDecision.Maximum = 3 * 24;
            progressDecision.Value = Math.Min(progressDecision.Maximum, hoursSinceOpen);
            progressDecision.ForeColor = allowDecision ? SUCCESS_COLOR : DANGER_COLOR;

            var dueLocal = decisionDue.ToLocalTime();
            var nowLocal = DateTime.Now;

            if (!allowDecision)
            {
                lblDecisionTime.Text = "❌ Termin na decyzję o zwrocie minął!";
                lblDecisionTime.ForeColor = DANGER_COLOR;
                lblDecisionTime.Font = new Font(lblDecisionTime.Font, FontStyle.Bold);
            }
            else
            {
                if (nowLocal.Date == dueLocal.Date && nowLocal > dueLocal)
                {
                    lblDecisionTime.Text = "⚠️ Do końca dzisiejszego dnia";
                    lblDecisionTime.ForeColor = WARNING_COLOR;
                }
                else
                {
                    lblDecisionTime.Text = $"⏱️ {Math.Max(0, decisionLeft.Days)}d {Math.Max(0, decisionLeft.Hours)}h pozostało";
                    lblDecisionTime.ForeColor = (decisionLeft.TotalHours < 24) ? WARNING_COLOR : SUCCESS_COLOR;
                }
            }

            // Resolution deadline (14 days)
            var resolutionDue = openedUtc.AddDays(14);
            var resolutionLeft = resolutionDue - nowUtc;
            progressResolution.Maximum = 14 * 24;
            progressResolution.Value = Math.Min(progressResolution.Maximum, hoursSinceOpen);
            progressResolution.ForeColor = resolutionLeft.TotalSeconds < 0 ? DANGER_COLOR : SUCCESS_COLOR;

            if (resolutionLeft.TotalSeconds < 0)
            {
                lblResolutionTime.Text = "❌ Termin na rozpatrzenie minął!";
                lblResolutionTime.ForeColor = DANGER_COLOR;
                lblResolutionTime.Font = new Font(lblResolutionTime.Font, FontStyle.Bold);
            }
            else
            {
                lblResolutionTime.Text = resolutionLeft.TotalDays < 3
                    ? $"⚠️ {Math.Max(0, resolutionLeft.Days)}d {Math.Max(0, resolutionLeft.Hours)}h pozostało"
                    : $"⏱️ {Math.Max(0, resolutionLeft.Days)}d {Math.Max(0, resolutionLeft.Hours)}h pozostało";
                lblResolutionTime.ForeColor = resolutionLeft.TotalDays < 3 ? WARNING_COLOR : SUCCESS_COLOR;
            }
        }

        private async Task LoadChatHistoryAsync()
        {
            if (_isLoadingChat || _apiClient == null) return;

            try
            {
                _isLoadingChat = true;
                ShowLoadingOverlay(true, "Ładowanie historii czatu...");

                var messages = await _apiClient.GetChatAsync(_issueId);

                pnlChatHistory.SuspendLayout();
                pnlChatHistory.Controls.Clear();

                if (!messages.Any())
                {
                    var emptyLabel = new Label
                    {
                        Text = "Brak wiadomości w tej dyskusji",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Padding = new Padding(20)
                    };
                    pnlChatHistory.Controls.Add(emptyLabel);
                }
                else
                {
                    var allControls = new List<Control>();

                    foreach (var message in messages.OrderBy(m => m.CreatedAt))
                    {
                        // Text bubble
                        if (!string.IsNullOrWhiteSpace(message.Text))
                        {
                            var textBubble = new ChatMessageBubble();
                            bool isSeller = (message?.Author?.Role?.ToUpperInvariant() == "SELLER");
                            textBubble.SetMessage(
                                message?.Author?.Login ?? "—",
                                message.CreatedAt,
                                message.Text,
                                isSeller);
                            allControls.Add(textBubble);
                        }

                        // Attachment bubbles
                        if (message.Attachments != null && message.Attachments.Any())
                        {
                            foreach (var attachment in message.Attachments)
                            {
                                var attachmentBubble = new ChatMessageBubble();
                                bool isSeller = (message?.Author?.Role?.ToUpperInvariant() == "SELLER");
                                attachmentBubble.SetMessage(
                                    message?.Author?.Login ?? "—",
                                    message.CreatedAt,
                                    "",
                                    isSeller);

                                await SetAttachmentInBubble(attachmentBubble, attachment);
                                allControls.Add(attachmentBubble);
                            }
                        }
                    }

                    pnlChatHistory.Controls.AddRange(allControls.ToArray());
                }
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "wczytywania historii czatu");
            }
            finally
            {
                _isLoadingChat = false;
                ShowLoadingOverlay(false);
                pnlChatHistory.ResumeLayout(true);
                if (pnlChatHistory.Controls.Count > 0)
                    pnlChatHistory.ScrollControlIntoView(pnlChatHistory.Controls[pnlChatHistory.Controls.Count - 1]);
            }
        }

        private async Task SetAttachmentInBubble(ChatMessageBubble bubble, PostPurchaseIssueAttachment attachment)
        {
            if (attachment == null || string.IsNullOrEmpty(attachment.Url)) return;

            try
            {
                var (downloadedImage, localPath) = await DownloadAndSaveAttachmentAsync(attachment, _apiClient);

                if (downloadedImage != null)
                {
                    bubble.SetAttachment(downloadedImage, localPath);
                }
                else if (localPath != null)
                {
                    bubble.SetAttachment(attachment.FileName, localPath);
                }
            }
            catch (Exception ex)
            {
                bubble.SetAttachment($"⚠️ Błąd pobierania: {attachment.FileName}", attachment.Url);
                Console.WriteLine($"[Allegro] Błąd pobierania załącznika {attachment.FileName}: {ex.Message}");
            }
        }

        private async Task<(Image image, string localPath)> DownloadAndSaveAttachmentAsync(PostPurchaseIssueAttachment attachment, AllegroApiClient apiClient)
        {
            string safeComplaintNumber = "BezNumeru";
            if (!string.IsNullOrEmpty(_internalComplaintNumber) && _internalComplaintNumber != "Brak")
            {
                safeComplaintNumber = _internalComplaintNumber.Replace('/', '.');
            }

            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane", safeComplaintNumber);
            Directory.CreateDirectory(directoryPath);

            string localFilePath = Path.Combine(directoryPath, attachment.FileName);

            // Check if file already exists
            if (File.Exists(localFilePath))
            {
                try
                {
                    byte[] existingBytes = File.ReadAllBytes(localFilePath);
                    if (IsImage(attachment.FileName))
                    {
                        using (var ms = new MemoryStream(existingBytes))
                        {
                            return (Image.FromStream(ms), localFilePath);
                        }
                    }
                    return (null, localFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Allegro] Błąd odczytu istniejącego pliku {localFilePath}: {ex.Message}");
                }
            }

            // Download file with authorization header
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, attachment.Url))
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiClient.Token.AccessToken);

                HttpResponseMessage response = await _fileDownloader.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                await Task.Run(() => File.WriteAllBytes(localFilePath, fileBytes));

                if (IsImage(attachment.FileName))
                {
                    using (var ms = new MemoryStream(fileBytes))
                    {
                        return (Image.FromStream(ms), localFilePath);
                    }
                }
                return (null, localFilePath);
            }
        }

        #endregion

        #region User Actions

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (_apiClient == null)
            {
                ShowError("Klient API nie jest zainicjalizowany.", "Błąd");
                return;
            }

            var txt = (txtNewMessage.Text ?? "").Trim();
            if (string.IsNullOrEmpty(txt) || txt == "Napisz wiadomość do kupującego...") return;

            try
            {
                btnSendMessage.Enabled = false;
                btnSendMessage.Text = "Wysyłanie...";

                await _apiClient.SendMessageAsync(_issueId, new NewMessageRequest { Text = txt });

                txtNewMessage.Text = "Napisz wiadomość do kupującego...";
                txtNewMessage.ForeColor = Color.Gray;

                await LoadChatHistoryAsync();
                ShowSuccess("Wiadomość została wysłana");
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "wysyłania wiadomości");
            }
            finally
            {
                btnSendMessage.Enabled = true;
                btnSendMessage.Text = "Wyślij";
            }
        }

        private async void btnChangeStatus_Click(object sender, EventArgs e)
        {
            if (cbClaimStatus.SelectedValue == null || string.IsNullOrEmpty(cbClaimStatus.SelectedValue.ToString()))
            {
                ShowWarning("Wybierz status z listy.", "Walidacja");
                return;
            }
            if (_apiClient == null)
            {
                ShowError("Klient API nie jest zainicjalizowany.", "Błąd");
                return;
            }

            string newStatus = cbClaimStatus.SelectedValue.ToString();
            var statusText = txtStatusMessage.Text.Trim();
            if (statusText == "Opcjonalnie: dodaj komentarz do zmiany statusu...")
                statusText = "";

            var request = new ChangeStatusRequest
            {
                Status = newStatus,
                Message = string.IsNullOrWhiteSpace(statusText)
                    ? ((KeyValuePair<string, string>)cbClaimStatus.SelectedItem).Key
                    : statusText
            };

            // Handle partial refund
            if (string.Equals(newStatus, "ACCEPTED_PARTIAL_REFUND", StringComparison.OrdinalIgnoreCase))
            {
                var raw = (txtPartialRefundAmount.Text ?? "").Trim().Replace(',', '.');
                if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    ShowWarning("Wprowadzona kwota częściowego zwrotu jest nieprawidłowa.", "Błąd walidacji");
                    return;
                }

                var currency = (cbPartialRefundCurrency.SelectedItem != null)
                    ? cbPartialRefundCurrency.SelectedItem.ToString()
                    : "PLN";

                request.PartialRefund = new PartialRefund
                {
                    Amount = amount.ToString("F2", CultureInfo.InvariantCulture),
                    Currency = currency
                };
            }

            try
            {
                if (newStatus.StartsWith("REJECTED_", StringComparison.OrdinalIgnoreCase))
                {
                    var confirm = MessageBox.Show("Czy na pewno odrzucić reklamację?", "Potwierdź",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes) return;
                }

                btnChangeStatus.Enabled = false;
                btnChangeStatus.Text = "Zmiana statusu...";

                await _apiClient.ChangeClaimStatusAsync(_issueId, request);

                ShowSuccess("Status reklamacji został pomyślnie zmieniony");
                await LoadIssueDetailsAsync();
                await LoadChatHistoryAsync();
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "zmiany statusu");
            }
            finally
            {
                btnChangeStatus.Enabled = true;
                btnChangeStatus.Text = "Zmień status reklamacji";
            }
        }

        private async void btnReturnRequiredCustom_Click(object sender, EventArgs e)
        {
            await SendActionMessage("RETURN_REQUIRED_CUSTOM", true,
                "Czy na pewno wymagać zwrotu towaru na koszt kupującego?");
        }

        private async void btnReturnNotRequired_Click(object sender, EventArgs e)
        {
            await SendActionMessage("RETURN_NOT_REQUIRED", false,
                "Czy na pewno poinformować, że zwrot towaru nie jest wymagany?");
        }

        private async void btnEndRequest_Click(object sender, EventArgs e)
        {
            await SendActionMessage("END_REQUEST", false,
                "Wysłać prośbę o zakończenie dyskusji?");
        }

        private async Task SendActionMessage(string type, bool requireComment, string confirmText)
        {
            if (_apiClient == null)
            {
                ShowError("Klient API nie jest zainicjalizowany.", "Błąd");
                return;
            }

            var confirm = MessageBox.Show(confirmText, "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            string text = ".";
            if (requireComment)
            {
                using (var prompt = new FormPrompt("Komentarz wymagany", "Podaj komentarz dla klienta (np. adres zwrotu):"))
                {
                    if (prompt.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(prompt.ResultText))
                        text = prompt.ResultText;
                    else
                    {
                        ShowWarning("Komentarz jest wymagany dla tej akcji.", "Anulowano");
                        return;
                    }
                }
            }

            try
            {
                var req = new NewMessageRequest { Type = type, Text = text };
                await _apiClient.SendMessageAsync(_issueId, req);
                await LoadChatHistoryAsync();
                ShowSuccess("Wiadomość/akcja została wysłana");
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "wysyłania akcji");
            }
        }

        private async void btnAddAttachment_Click(object sender, EventArgs e)
        {
            if (_apiClient == null)
            {
                ShowError("Klient API nie jest zainicjalizowany.", "Błąd");
                return;
            }

            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                ShowLoadingOverlay(true, "Wysyłanie załącznika...");

                var fi = new FileInfo(openFileDialog1.FileName);
                if (!fi.Exists)
                {
                    ShowError("Plik nie istnieje.", "Błąd");
                    return;
                }
                if (fi.Length > 10 * 1024 * 1024)
                {
                    ShowError("Maksymalny rozmiar pliku to 10 MB.", "Błąd");
                    return;
                }

                var uploaded = await _apiClient.UploadAttachmentAsync(openFileDialog1.FileName);

                var req = new NewMessageRequest
                {
                    Text = fi.Name,
                    Attachments = new List<NewMessageAttachment> { new NewMessageAttachment { Id = uploaded.Id } }
                };
                await _apiClient.SendMessageAsync(_issueId, req);

                ShowSuccess("Załącznik został wysłany");
                await LoadChatHistoryAsync();
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "dodawania załącznika");
            }
            finally
            {
                Cursor = Cursors.Default;
                ShowLoadingOverlay(false);
            }
        }

        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            try
            {
                string checkoutFormId = GetCheckoutFormIdFromDb(_issueId);
                if (!string.IsNullOrWhiteSpace(checkoutFormId))
                {
                    var url = $"https://allegro.pl/moje-allegro/sprzedaz/zamowienia/szczegoly/{checkoutFormId}";
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else
                {
                    ShowWarning("Brak ID zamówienia w bazie.", "Informacja");
                }
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "otwierania szczegółów zamówienia");
            }
        }

        private void btnAddTrackingNumber_Click(object sender, EventArgs e)
        {
            ShowInfo("Funkcjonalność dodawania numeru przesyłki będzie dostępna wkrótce.", "W przygotowaniu");
        }

        #endregion

        #region Helper Methods

        private void PopulateStatusComboBox()
        {
            var statuses = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("➡️ Wybierz nową akcję...", ""),
                new KeyValuePair<string, string>("✅ Akceptuję naprawę", "ACCEPTED_REPAIR"),
                new KeyValuePair<string, string>("✅ Akceptuję zwrot płatności", "ACCEPTED_REFUND"),
                new KeyValuePair<string, string>("✅ Akceptuję wymianę towaru", "ACCEPTED_EXCHANGE"),
                new KeyValuePair<string, string>("💰 Akceptuję częściowy zwrot", "ACCEPTED_PARTIAL_REFUND"),
                new KeyValuePair<string, string>("❌ Odrzucam – brak spełnionych wymagań", "REJECTED_ADDITIONAL_REQUIREMENTS_NOT_COMPLETED"),
                new KeyValuePair<string, string>("❌ Odrzucam – klient nie zwrócił towaru", "REJECTED_PRODUCT_NOT_RETURNED"),
                new KeyValuePair<string, string>("❌ Odrzucam – uszkodzone przez klienta", "REJECTED_PRODUCT_DAMAGED_BY_USER"),
                new KeyValuePair<string, string>("❌ Odrzucam – produkt zgodny z umową", "REJECTED_PRODUCT_CONFORMS_TO_CONTRACT"),
                new KeyValuePair<string, string>("❌ Odrzucam – drobna wada", "REJECTED_MINOR_DEFECT"),
                new KeyValuePair<string, string>("❌ Odrzucam – inny powód", "REJECTED_OTHER")
            };

            cbClaimStatus.DisplayMember = "Key";
            cbClaimStatus.ValueMember = "Value";
            cbClaimStatus.DataSource = statuses;
            cbClaimStatus.SelectedIndex = 0;

            cbPartialRefundCurrency.Items.Clear();
            cbPartialRefundCurrency.Items.AddRange(new[] { "PLN", "EUR", "USD" });
            cbPartialRefundCurrency.SelectedIndex = 0;
        }

        private void cbClaimStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isPartial = string.Equals(
                cbClaimStatus.SelectedValue != null ? cbClaimStatus.SelectedValue.ToString() : null,
                "ACCEPTED_PARTIAL_REFUND",
                StringComparison.OrdinalIgnoreCase);

            pnlPartialRefund.Visible = isPartial;
        }

        private (string translated, Color color) TranslateStatus(string status)
        {
            var map = new Dictionary<string, (string, Color)>
            {
                { "DISPUTE_ONGOING", ("🔄 W trakcie", PRIMARY_COLOR) },
                { "DISPUTE_UNRESOLVED", ("⚠️ Nierozwiązana", WARNING_COLOR) },
                { "DISPUTE_CLOSED", ("✅ Zamknięta", SUCCESS_COLOR) },
                { "CLAIM_SUBMITTED", ("📋 Złożona", PRIMARY_COLOR) },
                { "CLAIM_ACCEPTED", ("✅ Zaakceptowana", SUCCESS_COLOR) },
                { "CLAIM_REJECTED", ("❌ Odrzucona", DANGER_COLOR) }
            };
            if (string.IsNullOrEmpty(status)) return ("—", Color.Gray);
            return map.TryGetValue(status, out var result) ? result : (status, Color.Gray);
        }

        private static bool IsDecisionStillAllowed(DateTime openedUtc)
        {
            var decisionDueUtc = openedUtc.AddDays(3);
            var nowLocal = DateTime.Now;
            var dueLocal = decisionDueUtc.ToLocalTime();

            if (nowLocal <= dueLocal) return true;
            if (nowLocal.Date == dueLocal.Date) return true;
            return false;
        }

        private bool IsImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" }.Contains(extension);
        }

        private async Task<AllegroFullAccount> GetAccountForIssueAsync(string disputeId)
        {
            const string byIssueSql = @"
                SELECT aa.Id, aa.ClientId, aa.ClientSecretEncrypted
                FROM AllegroAccounts aa
                JOIN AllegroDisputes ad ON ad.AllegroAccountId = aa.Id
                WHERE ad.DisputeId = @id
                LIMIT 1";

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(byIssueSql, con))
                {
                    cmd.Parameters.AddWithValue("@id", disputeId);
                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        if (await rd.ReadAsync())
                        {
                            return new AllegroFullAccount
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                ClientId = rd["ClientId"] != DBNull.Value ? rd["ClientId"].ToString() : null,
                                ClientSecret = EncryptionHelper.DecryptString(
                                    rd["ClientSecretEncrypted"] != DBNull.Value ? rd["ClientSecretEncrypted"].ToString() : string.Empty)
                            };
                        }
                    }
                }
            }

            // Fallback to default account
            const string byDefaultSql = @"
                SELECT Id, ClientId, ClientSecretEncrypted
                FROM AllegroAccounts
                WHERE IsDefault = '1'
                LIMIT 1";

            using (var con2 = DatabaseHelper.GetConnection())
            {
                await con2.OpenAsync();
                using (var cmd2 = new MySqlCommand(byDefaultSql, con2))
                using (var rd2 = await cmd2.ExecuteReaderAsync())
                {
                    if (await rd2.ReadAsync())
                    {
                        return new AllegroFullAccount
                        {
                            Id = Convert.ToInt32(rd2["Id"]),
                            ClientId = rd2["ClientId"] != DBNull.Value ? rd2["ClientId"].ToString() : null,
                            ClientSecret = EncryptionHelper.DecryptString(
                                rd2["ClientSecretEncrypted"] != DBNull.Value ? rd2["ClientSecretEncrypted"].ToString() : string.Empty)
                        };
                    }
                }
            }

            return null;
        }

        private async Task<AllegroFullAccount> GetAccountByIdAsync(int accountId)
        {
            const string sql = @"SELECT Id, ClientId, ClientSecretEncrypted FROM AllegroAccounts WHERE Id = @id LIMIT 1";
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", accountId);
                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        if (await rd.ReadAsync())
                        {
                            return new AllegroFullAccount
                            {
                                Id = Convert.ToInt32(rd["Id"]),
                                ClientId = rd["ClientId"] != DBNull.Value ? rd["ClientId"].ToString() : null,
                                ClientSecret = EncryptionHelper.DecryptString(
                                    rd["ClientSecretEncrypted"] != DBNull.Value ? rd["ClientSecretEncrypted"].ToString() : string.Empty)
                            };
                        }
                    }
                }
            }
            return null;
        }

        private async Task MarkIssueAsRead()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("UPDATE AllegroDisputes SET HasNewMessages = 0 WHERE DisputeId = @id", con);
                    cmd.Parameters.AddWithValue("@id", _issueId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Allegro] Nie udało się oznaczyć wiadomości jako przeczytane: {ex.Message}");
            }
        }

        private string GetCheckoutFormIdFromDb(string disputeId)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new MySqlCommand("SELECT CheckoutFormId FROM AllegroDisputes WHERE DisputeId=@id LIMIT 1", con))
                    {
                        cmd.Parameters.AddWithValue("@id", disputeId);
                        var obj = cmd.ExecuteScalar();
                        return obj?.ToString();
                    }
                }
            }
            catch { return null; }
        }

        #endregion

        #region UI Helpers

        private Panel _loadingOverlay;

        private void ShowLoadingOverlay(bool show, string message = "Ładowanie...")
        {
            if (show)
            {
                if (_loadingOverlay == null)
                {
                    _loadingOverlay = new Panel
                    {
                        Dock = DockStyle.Fill,
                        BackColor = Color.FromArgb(200, 0, 0, 0),
                        Name = "loadingOverlay"
                    };

                    var spinner = new Label
                    {
                        Text = "⏳ " + message,
                        Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                        ForeColor = Color.White,
                        AutoSize = true,
                        BackColor = Color.Transparent
                    };

                    _loadingOverlay.Controls.Add(spinner);
                    spinner.Location = new Point(
                        (_loadingOverlay.Width - spinner.Width) / 2,
                        (_loadingOverlay.Height - spinner.Height) / 2
                    );

                    _loadingOverlay.Resize += (s, e) => {
                        spinner.Location = new Point(
                            (_loadingOverlay.Width - spinner.Width) / 2,
                            (_loadingOverlay.Height - spinner.Height) / 2
                        );
                    };
                }

                if (!this.Controls.Contains(_loadingOverlay))
                {
                    this.Controls.Add(_loadingOverlay);
                    _loadingOverlay.BringToFront();
                }
            }
            else
            {
                if (_loadingOverlay != null && this.Controls.Contains(_loadingOverlay))
                {
                    this.Controls.Remove(_loadingOverlay);
                }
            }
        }

        private void HandleApiError(Exception ex, string action)
        {
            string msg = $"Błąd podczas {action}:\n\n{ex.Message}";
            if (ex is HttpRequestException httpEx && httpEx.Data["ResponseContent"] != null)
            {
                msg += "\n\nOdpowiedź z serwera:\n" + httpEx.Data["ResponseContent"];
            }
            ShowError(msg, "Błąd API");
        }

        private void ShowError(string message, string title = "Błąd")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowWarning(string message, string title = "Ostrzeżenie")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowSuccess(string message, string title = "Sukces")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowInfo(string message, string title = "Informacja")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}
