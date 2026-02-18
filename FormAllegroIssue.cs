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
using System.Reflection;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Nowoczesny formularz zarządzania dyskusją Allegro.
    /// Wersja Finalna - Zawiera wszystkie poprawki UI, Logiki i Bazy Danych.
    /// </summary>
    public partial class FormAllegroIssue : Form
    {
        // Pola prywatne
        private readonly string _issueId;
        private AllegroApiClient _apiClient;
        private int _accountId;
        private string _internalComplaintNumber;
        private static readonly HttpClient _fileDownloader = new HttpClient();
        private bool _isLoadingChat = false;

        // Kolory motywu
        private static readonly Color PRIMARY_COLOR = Color.FromArgb(0, 120, 215);
        private static readonly Color SUCCESS_COLOR = Color.FromArgb(16, 124, 16);
        private static readonly Color WARNING_COLOR = Color.FromArgb(255, 185, 0);
        private static readonly Color DANGER_COLOR = Color.FromArgb(196, 43, 28);
        private static readonly Color BACKGROUND_COLOR = Color.FromArgb(243, 242, 241);

        // Nakładka ładowania
        private Panel _loadingOverlay;

        public FormAllegroIssue(string issueId)
        {
            _issueId = issueId ?? throw new ArgumentNullException(nameof(issueId));

            // Fix migotania (Double Buffering dla Formularza)
            this.DoubleBuffered = true;

            InitializeComponent();
            CustomizeUI();

            Text = $"Dyskusja Allegro: {_issueId}";

            // Pokaż overlay od razu, zanim formularz w pełni się narysuje
            ShowLoadingOverlay(true, "Inicjalizacja...");

            EnableSpellCheckOnAllTextBoxes();
        }

        #region Initialization & Layout

        private void CustomizeUI()
        {
            this.Font = new Font("Segoe UI", 9.5F);
            this.BackColor = BACKGROUND_COLOR;
            this.MinimumSize = new Size(1200, 700);
            this.WindowState = FormWindowState.Maximized;

            // Stylowanie przycisków
            StyleButton(btnSendMessage, PRIMARY_COLOR);
            StyleButton(btnAddAttachment, PRIMARY_COLOR);
            StyleButton(btnChangeStatus, SUCCESS_COLOR);
            StyleButton(btnReturnRequiredCustom, WARNING_COLOR);
            StyleButton(btnReturnNotRequired, SUCCESS_COLOR);
            StyleButton(btnEndRequest, PRIMARY_COLOR);
            StyleButton(btnViewOrder, PRIMARY_COLOR);
            StyleButton(btnAddTrackingNumber, PRIMARY_COLOR);

            // Placeholdery
            SetupPlaceholder(txtNewMessage, "Napisz wiadomość do kupującego...");
            SetupPlaceholder(txtStatusMessage, "Opcjonalnie: dodaj komentarz do zmiany statusu...");

            // Stylowanie grup
            foreach (Control ctrl in pnlActions.Controls.OfType<GroupBox>())
            {
                StyleGroupBox((GroupBox)ctrl);
            }

            // Fix migotania panelu historii
            pnlChatHistory.BackColor = BACKGROUND_COLOR;
            EnableDoubleBuffering(pnlChatHistory);
        }

        // --- KLUCZOWA METODA: REFLOW LAYOUT ---
        // Układa elementy jeden pod drugim, eliminując puste przestrzenie po ukrytych kontrolkach
        private void ReflowLayout()
        {
            pnlActions.SuspendLayout();
            int y = 10; // Margines górny
            int spacing = 15; // Odstęp między elementami

            // 1. Panel Statusu (zawsze widoczny, ale zmienia wysokość)
            if (gbStatusAndDeadlines.Visible)
            {
                gbStatusAndDeadlines.Location = new Point(gbStatusAndDeadlines.Location.X, y);

                // Oblicz potrzebną wysokość na podstawie widocznych dzieci
                int contentBottom = 0;
                foreach (Control c in gbStatusAndDeadlines.Controls)
                {
                    if (c.Visible) contentBottom = Math.Max(contentBottom, c.Bottom);
                }
                gbStatusAndDeadlines.Height = contentBottom + 20; // + padding

                y += gbStatusAndDeadlines.Height + spacing;
            }

            // 2. Decyzja o zwrocie (tylko reklamacje)
            if (gbReturnDecision.Visible)
            {
                gbReturnDecision.Location = new Point(gbReturnDecision.Location.X, y);
                y += gbReturnDecision.Height + spacing;
            }

            // 3. Zmiana Statusu (tylko reklamacje)
            if (gbChangeStatus.Visible)
            {
                gbChangeStatus.Location = new Point(gbChangeStatus.Location.X, y);
                y += gbChangeStatus.Height + spacing;
            }

            // 4. Przycisk Zakończ (tylko dyskusje)
            if (btnEndRequest.Visible)
            {
                btnEndRequest.Location = new Point(btnEndRequest.Location.X, y);
                y += btnEndRequest.Height + spacing;
            }

            // 5. Zamówienie (zawsze widoczne, przesuwa się pod spód)
            if (gbOrder.Visible)
            {
                gbOrder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                gbOrder.Location = new Point(gbOrder.Location.X, y);
                y += gbOrder.Height + spacing;
            }

            pnlActions.ResumeLayout(true);
        }

        private async void FormAllegroIssue_Load(object sender, EventArgs e)
        {
            try
            {
                ShowLoadingOverlay(true, "Pobieranie danych...");

                var account = await GetAccountForIssueAsync(_issueId);
                if (account == null)
                {
                    ShowError("Brak konta Allegro powiązanego z tą dyskusją.", "Błąd konfiguracji");
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
                HandleApiError(ex, "ładowania formularza");
                Close();
            }
        }

        #endregion

        #region Logic & Data Loading

        private async Task LoadIssueDetailsAsync()
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                var cmd = new MySqlCommand(
                    "SELECT ad.*, z.NrZgloszenia " +
                    "FROM allegrodisputes ad " +
                    "LEFT JOIN zgloszenia z ON ad.ComplaintId = z.Id " +
                    "WHERE ad.DisputeId = @id", con);
                cmd.Parameters.AddWithValue("@id", _issueId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        ShowError("Brak danych w bazie dla tego ID dyskusji.", "Błąd danych");
                        Close();
                        return;
                    }

                    // Obsługa zmiany konta (jeśli dyskusja jest na innym koncie niż domyślne)
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

                    // Wypełnianie danych UI
                    lblDisputeId.Text = _issueId;
                    lblBuyerLogin.Text = reader["BuyerLogin"]?.ToString() ?? "-";
                    _internalComplaintNumber = reader["NrZgloszenia"]?.ToString() ?? "-";
                    lblInternalComplaintId.Text = _internalComplaintNumber;
                    lblProductName.Text = reader["Subject"]?.ToString() ?? "Nieznany produkt";

                    UpdateStatusDisplay(reader["StatusAllegro"]?.ToString());

                    // Typ i dostosowanie interfejsu (Dyskusja vs Reklamacja)
                    string type = reader["Type"]?.ToString() ?? "DISPUTE";
                    AdjustInterfaceForIssueType(type, reader);
                }
            }
        }

        private void AdjustInterfaceForIssueType(string type, System.Data.Common.DbDataReader reader)
        {
            bool isClaim = type != null && type.ToUpper().Contains("CLAIM");

            DateTime openedUtc = DateTime.UtcNow;
            if (reader["OpenedAt"] != DBNull.Value && DateTime.TryParse(reader["OpenedAt"].ToString(), out DateTime opened))
            {
                openedUtc = DateTime.SpecifyKind(opened, DateTimeKind.Utc);
            }

            if (isClaim)
            {
                // *** REKLAMACJA ***
                this.Text = $"Reklamacja Allegro: {_issueId}";

                // Pokaż panele decyzyjne
                gbReturnDecision.Visible = true;
                gbChangeStatus.Visible = true;

                // Ukryj przycisk zakończenia (bo są decyzje statusowe)
                btnEndRequest.Visible = false;

                // Pokaż liczniki czasu
                lblDecisionTime.Visible = true;
                progressDecision.Visible = true;
                lblResolutionTime.Text = "Czas na rozpatrzenie (14 dni):";
                progressResolution.Visible = true;

                UpdateDeadlineDisplays(openedUtc);
            }
            else
            {
                // *** DYSKUSJA ***
                this.Text = $"Dyskusja Allegro: {_issueId}";

                // Ukryj panele reklamacyjne
                gbReturnDecision.Visible = false;
                gbChangeStatus.Visible = false;

                // Pokaż przycisk zakończenia
                btnEndRequest.Visible = true;

                // Ukryj zbędne paski czasu
                lblDecisionTime.Visible = false;
                progressDecision.Visible = false;
                progressResolution.Visible = false;

                // Uproszczony tekst dla dyskusji
                lblResolutionTime.Text = "Regulaminowy czas na rozwiązanie: 30 dni";
                lblResolutionTime.ForeColor = Color.Black;
                // Przesuń etykietę wyżej, w miejsce ukrytego decision time
                lblResolutionTime.Location = new Point(lblDecisionTime.Location.X, lblDecisionTime.Location.Y);
            }

            // Na koniec przelicz układ (Reflow), żeby usunąć dziury
            ReflowLayout();
        }

        private void UpdateStatusDisplay(string status)
        {
            var result = TranslateStatus(status);
            lblCurrentStatus.Text = result.translated;
            lblCurrentStatus.ForeColor = result.color;
        }

        private void UpdateDeadlineDisplays(DateTime openedUtc)
        {
            var nowUtc = DateTime.UtcNow;
            var hoursSinceOpen = Math.Max(0, (int)(nowUtc - openedUtc).TotalHours);

            var decisionDue = openedUtc.AddDays(3);
            var decisionLeft = decisionDue - nowUtc;
            var allowDecision = IsDecisionStillAllowed(openedUtc);

            progressDecision.Maximum = 3 * 24;
            progressDecision.Value = Math.Min(progressDecision.Maximum, hoursSinceOpen);
            progressDecision.ForeColor = allowDecision ? SUCCESS_COLOR : DANGER_COLOR;

            if (!allowDecision)
            {
                lblDecisionTime.Text = "❌ Termin na decyzję minął!";
                lblDecisionTime.ForeColor = DANGER_COLOR;
            }
            else
            {
                lblDecisionTime.Text = $"⏱️ Czas na decyzję: {Math.Max(0, decisionLeft.Days)}d {Math.Max(0, decisionLeft.Hours)}h";
                lblDecisionTime.ForeColor = (decisionLeft.TotalHours < 24) ? WARNING_COLOR : SUCCESS_COLOR;
            }

            var resolutionDue = openedUtc.AddDays(14);
            var resolutionLeft = resolutionDue - nowUtc;
            progressResolution.Maximum = 14 * 24;
            progressResolution.Value = Math.Min(progressResolution.Maximum, hoursSinceOpen);
            progressResolution.ForeColor = resolutionLeft.TotalSeconds < 0 ? DANGER_COLOR : SUCCESS_COLOR;
        }

        private async Task LoadChatHistoryAsync()
        {
            if (_isLoadingChat || _apiClient == null) return;

            try
            {
                _isLoadingChat = true;
                var messages = await _apiClient.GetChatAsync(_issueId);

                pnlChatHistory.SuspendLayout();
                pnlChatHistory.Controls.Clear();

                if (!messages.Any())
                {
                    var lbl = new Label { Text = "Brak wiadomości", AutoSize = true, ForeColor = Color.Gray, Padding = new Padding(20) };
                    pnlChatHistory.Controls.Add(lbl);
                }
                else
                {
                    var controls = new List<Control>();
                    foreach (var msg in messages.OrderBy(m => m.CreatedAt))
                    {
                        bool isSeller = (msg.Author?.Role?.ToUpperInvariant() == "SELLER");

                        if (!string.IsNullOrWhiteSpace(msg.Text))
                        {
                            var bubble = new ChatMessageBubble();
                            bubble.SetMessage(msg.Author?.Login ?? "-", msg.CreatedAt, msg.Text, isSeller);
                            controls.Add(bubble);
                        }

                        if (msg.Attachments != null)
                        {
                            foreach (var att in msg.Attachments)
                            {
                                var bubble = new ChatMessageBubble();
                                bubble.SetMessage(msg.Author?.Login ?? "-", msg.CreatedAt, "", isSeller);
                                await SetAttachmentInBubble(bubble, att);
                                controls.Add(bubble);
                            }
                        }
                    }
                    pnlChatHistory.Controls.AddRange(controls.ToArray());
                }
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "wczytywania historii czatu");
            }
            finally
            {
                _isLoadingChat = false;
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
                var result = await DownloadAndSaveAttachmentAsync(attachment, _apiClient);
                if (result.image != null) bubble.SetAttachment(result.image, result.localPath);
                else bubble.SetAttachment(attachment.FileName, result.localPath);
            }
            catch
            {
                bubble.SetAttachment($"Plik: {attachment.FileName}", attachment.Url);
            }
        }

        private async Task<(Image image, string localPath)> DownloadAndSaveAttachmentAsync(PostPurchaseIssueAttachment attachment, AllegroApiClient apiClient)
        {
            string safeNumber = string.IsNullOrEmpty(_internalComplaintNumber) ? "BezNumeru" : _internalComplaintNumber.Replace('/', '.');
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane", safeNumber);
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, attachment.FileName);

            if (!File.Exists(path))
            {
                using (var req = new HttpRequestMessage(HttpMethod.Get, attachment.Url))
                {
                    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiClient.Token.AccessToken);
                    var resp = await _fileDownloader.SendAsync(req);
                    var bytes = await resp.Content.ReadAsByteArrayAsync();
                    await Task.Run(() => File.WriteAllBytes(path, bytes));
                }
            }

            if (IsImage(attachment.FileName))
            {
                try
                {
                    using (var ms = new MemoryStream(File.ReadAllBytes(path))) return (Image.FromStream(ms), path);
                }
                catch { return (null, path); }
            }
            return (null, path);
        }

        #endregion

        #region User Actions

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (_apiClient == null) return;
            var txt = txtNewMessage.Text.Trim();
            if (string.IsNullOrEmpty(txt) || txt.StartsWith("Napisz wiadomość")) return;

            try
            {
                btnSendMessage.Text = "Wysyłanie...";
                btnSendMessage.Enabled = false;
                await _apiClient.SendMessageAsync(_issueId, new NewMessageRequest { Text = txt });
                txtNewMessage.Text = "";
                SetupPlaceholder(txtNewMessage, "Napisz wiadomość do kupującego...");
                await LoadChatHistoryAsync();
            }
            catch (Exception ex) { HandleApiError(ex, "wysyłania wiadomości"); }
            finally { btnSendMessage.Text = "Wyślij"; btnSendMessage.Enabled = true; }
        }

        private async void btnChangeStatus_Click(object sender, EventArgs e)
        {
            if (cbClaimStatus.SelectedValue?.ToString() == "") return;

            try
            {
                var msg = txtStatusMessage.Text.StartsWith("Opcjonalnie") ? "" : txtStatusMessage.Text;
                var req = new ChangeStatusRequest { Status = cbClaimStatus.SelectedValue.ToString(), Message = msg };

                // Częściowy zwrot
                if (req.Status == "ACCEPTED_PARTIAL_REFUND")
                {
                    if (decimal.TryParse(txtPartialRefundAmount.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amt))
                    {
                        req.PartialRefund = new PartialRefund { Amount = amt.ToString("F2", CultureInfo.InvariantCulture), Currency = cbPartialRefundCurrency.Text };
                    }
                    else { ShowWarning("Błędna kwota.", "Błąd"); return; }
                }

                await _apiClient.ChangeClaimStatusAsync(_issueId, req);
                ShowSuccess("Status reklamacji został zmieniony.");
                await LoadIssueDetailsAsync(); // Przeładuj, by zaktualizować status w UI
            }
            catch (Exception ex) { HandleApiError(ex, "zmiany statusu"); }
        }

        private async void btnEndRequest_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Czy na pewno wysłać prośbę o zakończenie dyskusji?", "Potwierdź", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    await _apiClient.SendMessageAsync(_issueId, new NewMessageRequest { Type = "END_REQUEST", Text = "." });
                    ShowSuccess("Wysłano prośbę o zakończenie.");
                    await LoadChatHistoryAsync();
                }
                catch (Exception ex) { HandleApiError(ex, "wysyłania prośby"); }
            }
        }

        private async void btnAddAttachment_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ShowLoadingOverlay(true, "Wysyłanie pliku...");
                    var up = await _apiClient.UploadAttachmentAsync(openFileDialog1.FileName);
                    await _apiClient.SendMessageAsync(_issueId, new NewMessageRequest
                    {
                        Text = Path.GetFileName(openFileDialog1.FileName),
                        Attachments = new List<NewMessageAttachment> { new NewMessageAttachment { Id = up.Id } }
                    });
                    await LoadChatHistoryAsync();
                }
                catch (Exception ex) { HandleApiError(ex, "wysyłania pliku"); }
                finally { ShowLoadingOverlay(false); }
            }
        }

        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            string orderId = GetOrderIdFromDb(_issueId);
            if (!string.IsNullOrEmpty(orderId))
                Process.Start(new ProcessStartInfo($"https://salescenter.allegro.com/orders/{orderId}") { UseShellExecute = true });
            else
                ShowWarning("Brak numeru zamówienia w bazie danych.", "Brak danych");
        }

        private async void btnReturnRequiredCustom_Click(object sender, EventArgs e) => await SimpleAction("RETURN_REQUIRED_CUSTOM");
        private async void btnReturnNotRequired_Click(object sender, EventArgs e) => await SimpleAction("RETURN_NOT_REQUIRED");
        private void btnAddTrackingNumber_Click(object sender, EventArgs e) => ShowInfo("Funkcjonalność w przygotowaniu.", "Info");

        private async Task SimpleAction(string type)
        {
            if (MessageBox.Show("Czy na pewno wykonać tę akcję?", "Potwierdzenie", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    await _apiClient.SendMessageAsync(_issueId, new NewMessageRequest { Type = type, Text = "." });
                    await LoadChatHistoryAsync();
                }
                catch (Exception ex) { HandleApiError(ex, "wykonywania akcji"); }
            }
        }

        #endregion

        #region Helpers & Database Methods (BRAKUJĄCE WCZEŚNIEJ METODY)

        // 1. Tłumaczenie statusów (API -> UI)
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

        // 2. Oznaczanie jako przeczytane
        private async Task MarkIssueAsRead()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("UPDATE allegrodisputes SET HasNewMessages = 0 WHERE DisputeId = @id", con);
                    cmd.Parameters.AddWithValue("@id", _issueId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Allegro] Błąd oznaczania jako przeczytane: {ex.Message}");
            }
        }

        // 3. Pobieranie konta Allegro dla dyskusji
        private async Task<AllegroFullAccount> GetAccountForIssueAsync(string disputeId)
        {
            const string byIssueSql = @"
                SELECT aa.Id, aa.ClientId, aa.ClientSecretEncrypted
                FROM AllegroAccounts aa
                JOIN allegrodisputes ad ON ad.AllegroAccountId = aa.Id
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
                                ClientId = rd["ClientId"]?.ToString(),
                                ClientSecret = EncryptionHelper.DecryptString(rd["ClientSecretEncrypted"]?.ToString())
                            };
                        }
                    }
                }
            }

            // Fallback: Konto domyślne
            const string byDefaultSql = "SELECT Id, ClientId, ClientSecretEncrypted FROM AllegroAccounts WHERE IsDefault = '1' LIMIT 1";
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
                            ClientId = rd2["ClientId"]?.ToString(),
                            ClientSecret = EncryptionHelper.DecryptString(rd2["ClientSecretEncrypted"]?.ToString())
                        };
                    }
                }
            }
            return null;
        }

        // 4. Pobieranie konta po ID
        private async Task<AllegroFullAccount> GetAccountByIdAsync(int accountId)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                var cmd = new MySqlCommand("SELECT Id, ClientId, ClientSecretEncrypted FROM AllegroAccounts WHERE Id = @id LIMIT 1", con);
                cmd.Parameters.AddWithValue("@id", accountId);
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (await r.ReadAsync())
                    {
                        return new AllegroFullAccount
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            ClientId = r["ClientId"]?.ToString(),
                            ClientSecret = EncryptionHelper.DecryptString(r["ClientSecretEncrypted"]?.ToString())
                        };
                    }
                }
            }
            return null;
        }

        // 5. Pobieranie OrderId
        private string GetOrderIdFromDb(string disputeId)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new MySqlCommand("SELECT OrderId FROM allegrodisputes WHERE DisputeId=@id LIMIT 1", con))
                    {
                        cmd.Parameters.AddWithValue("@id", disputeId);
                        var obj = cmd.ExecuteScalar();
                        return obj?.ToString();
                    }
                }
            }
            catch { return null; }
        }

        // 6. Inne pomocnicze
        private void SetupPlaceholder(TextBox txt, string placeholder)
        {
            txt.ForeColor = Color.Gray;
            txt.Text = placeholder;
            txt.Enter += (s, e) => { if (txt.Text == placeholder) { txt.Text = ""; txt.ForeColor = Color.Black; } };
            txt.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txt.Text)) { txt.Text = placeholder; txt.ForeColor = Color.Gray; } };
        }

        public static void EnableDoubleBuffering(Control control)
        {
            try { typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, control, new object[] { true }); } catch { }
        }

        private void StyleButton(Button btn, Color bgColor)
        {
            btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = bgColor; btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(bgColor, 0.1f);
            btn.MouseLeave += (s, e) => btn.BackColor = bgColor;
        }

        private void StyleGroupBox(GroupBox gb)
        {
            gb.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            gb.ForeColor = Color.FromArgb(50, 50, 50);
        }

        private void ShowLoadingOverlay(bool show, string message = "Ładowanie...")
        {
            if (show)
            {
                if (_loadingOverlay == null)
                {
                    _loadingOverlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(200, 0, 0, 0) };
                    EnableDoubleBuffering(_loadingOverlay);
                    var lbl = new Label { Text = "⏳ " + message, Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.White, AutoSize = true };
                    _loadingOverlay.Controls.Add(lbl);

                    // Centrowanie przy zmianie rozmiaru
                    _loadingOverlay.Resize += (s, e) => {
                        lbl.Location = new Point((_loadingOverlay.Width - lbl.Width) / 2, (_loadingOverlay.Height - lbl.Height) / 2);
                    };
                }

                // Aktualizuj tekst i pozycję
                var label = (Label)_loadingOverlay.Controls[0];
                label.Text = "⏳ " + message;
                label.Location = new Point((_loadingOverlay.Width - label.Width) / 2, (_loadingOverlay.Height - label.Height) / 2);

                if (!Controls.Contains(_loadingOverlay))
                {
                    Controls.Add(_loadingOverlay);
                    _loadingOverlay.BringToFront();
                }
            }
            else
            {
                if (_loadingOverlay != null && Controls.Contains(_loadingOverlay)) Controls.Remove(_loadingOverlay);
            }
        }

        private void HandleApiError(Exception ex, string action)
        {
            string msg = $"Błąd podczas {action}:\n{ex.Message}";
            ShowError(msg, "Błąd API");
        }

        private void ShowError(string msg, string title) => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowWarning(string msg, string title) => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void ShowSuccess(string msg) => MessageBox.Show(msg, "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowInfo(string msg, string title) => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void EnableSpellCheckOnAllTextBoxes()
        {
            foreach (Control c in GetAllControls(this)) if (c is RichTextBox r) r.EnableSpellCheck(true);
        }

        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control c in container.Controls) { yield return c; if (c.HasChildren) foreach (Control child in GetAllControls(c)) yield return child; }
        }

        private bool IsImage(string f) => new[] { ".jpg", ".png", ".jpeg", ".bmp", ".gif" }.Contains(Path.GetExtension(f).ToLower());

        private static bool IsDecisionStillAllowed(DateTime d) => DateTime.Now <= d.AddDays(3);

        private void PopulateStatusComboBox()
        {
            cbClaimStatus.DataSource = new BindingSource(new Dictionary<string, string> {
                { "Wybierz...", "" },
                { "✅ Akceptuj naprawę", "ACCEPTED_REPAIR" },
                { "✅ Akceptuj zwrot", "ACCEPTED_REFUND" },
                { "✅ Akceptuj wymianę", "ACCEPTED_EXCHANGE" },
                { "💰 Częściowy zwrot", "ACCEPTED_PARTIAL_REFUND" },
                { "❌ Odrzuć (Brak zwrotu)", "REJECTED_PRODUCT_NOT_RETURNED" },
                { "❌ Odrzuć (Uszkodzenie)", "REJECTED_PRODUCT_DAMAGED_BY_USER" },
                { "❌ Odrzuć (Inne)", "REJECTED_OTHER" }
            }, null);
            cbClaimStatus.DisplayMember = "Key"; cbClaimStatus.ValueMember = "Value";

            cbPartialRefundCurrency.Items.Clear();
            cbPartialRefundCurrency.Items.AddRange(new[] { "PLN", "EUR", "USD" });
            cbPartialRefundCurrency.SelectedIndex = 0;
        }

        private void cbClaimStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlPartialRefund.Visible = cbClaimStatus.SelectedValue?.ToString() == "ACCEPTED_PARTIAL_REFUND";
            ReflowLayout(); // Przelicz układ po zmianie widoczności
        }

        #endregion
    }
}