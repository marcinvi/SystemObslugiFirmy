using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Reklamacje_Dane.Allegro;
using Newtonsoft.Json.Linq; // Do obsługi załączników

namespace Reklamacje_Dane
{
    public partial class FormWiadomosci : Form
    {
        private readonly AllegroChatService _chatService;
        private List<AllegroChatService.ThreadInfo> _threads;
        private MessageListItem _selectedListItem;
        private AllegroApiClient _activeApiClient;
        private string _activeDisputeId;

        // Przeglądarka do błyskawicznego wyświetlania czatu
        private WebBrowser _chatBrowser;

        // Menu kontekstowe
        private ToolStripMenuItem markAsUnreadMenuItem;

        public FormWiadomosci()
        {
            InitializeComponent();
            _chatService = new AllegroChatService();
            InitializeCustomMenu();
            InitializeChatBrowser();
        }

        private void InitializeChatBrowser()
        {
            // 1. Bezpiecznie ukrywamy stary, wolny panel (szukamy po nazwie, żeby nie wywaliło błędu)
            var oldContainer = this.Controls.Find("flowLayoutPanelChat", true).FirstOrDefault();
            if (oldContainer != null) oldContainer.Visible = false;

            // Szukamy panelu głównego obszaru czatu
            Control chatArea = this.Controls.Find("pnlChatArea", true).FirstOrDefault();

            // Jeśli nie ma pnlChatArea, próbujemy znaleźć splitContainer i jego prawy panel
            if (chatArea == null)
            {
                var split = this.Controls.Find("splitContainer1", true).FirstOrDefault() as SplitContainer;
                if (split != null) chatArea = split.Panel2;
            }

            // Jeśli nadal null, to znaczy że nazwy w Designerze są inne - dodajemy do formy jako fallback
            if (chatArea == null) chatArea = this;

            if (lblNoConversation != null) lblNoConversation.Visible = true;

            // 2. Tworzymy szybką przeglądarkę
            _chatBrowser = new WebBrowser();
            _chatBrowser.Dock = DockStyle.Fill;
            _chatBrowser.IsWebBrowserContextMenuEnabled = false; // Blokada menu pod prawym
            _chatBrowser.AllowNavigation = false;
            _chatBrowser.ScriptErrorsSuppressed = true; // Ukrywanie błędów skryptów
            _chatBrowser.Visible = false; // Ukryta do momentu kliknięcia

            // Dodajemy do panelu
            chatArea.Controls.Add(_chatBrowser);
            _chatBrowser.BringToFront();

            // 3. Upewniamy się, że panel wpisywania (txtNewMessage) jest NA WIERZCHU
            foreach (Control c in chatArea.Controls)
            {
                // Zakładamy, że panel wpisywania jest na dole (Dock = Bottom) lub ma nazwę panelMessageInput
                if (c != _chatBrowser && (c.Dock == DockStyle.Bottom || c.Name.Contains("Input") || c.Name.Contains("Message")))
                {
                    c.BringToFront();
                }
            }
        }

        private void InitializeCustomMenu()
        {
            markAsUnreadMenuItem = new ToolStripMenuItem("Oznacz jako nieprzeczytane");
            markAsUnreadMenuItem.Click += MarkAsUnread_Click;
            contextMenuThreads.Items.Add(new ToolStripSeparator());
            contextMenuThreads.Items.Add(markAsUnreadMenuItem);
        }

        private async void FormWiadomosci_Load(object sender, EventArgs e)
        {
            if (flpThreads != null)
            {
                flpThreads.ContextMenuStrip = contextMenuThreads;
            }
            await LoadThreadsAsync();
        }

        // === LEWA STRONA: LISTA WĄTKÓW ===
        private async Task LoadThreadsAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                // Pobieranie w tle
                _threads = await Task.Run(() => _chatService.GetLatestThreadsAsync());

                flpThreads.SuspendLayout();

                // Czyścimy listę
                for (int i = flpThreads.Controls.Count - 1; i >= 0; i--)
                {
                    var c = flpThreads.Controls[i];
                    flpThreads.Controls.RemoveAt(i);
                    c.Dispose();
                }

                var controlsToAdd = new List<Control>();
                foreach (var thread in _threads)
                {
                    var item = new MessageListItem();
                    item.SetData(thread.DisputeId, thread.BuyerLogin, thread.AccountName, thread.ComplaintNumber, thread.LastMessageDate, thread.LastMessageText, thread.HasNewMessages);

                    item.Click += ListItem_Click;
                    item.Width = flpThreads.ClientSize.Width - 25;
                    item.Tag = thread;

                    item.MouseUp += (s, e) => {
                        if (e.Button == MouseButtons.Right)
                        {
                            _selectedListItem = item;
                            _activeDisputeId = thread.DisputeId;

                            if (_selectedListItem != null) _selectedListItem.SetSelected(false);
                            _selectedListItem = item;
                            _selectedListItem.SetSelected(true);

                            contextMenuThreads.Show(Cursor.Position);
                        }
                    };

                    controlsToAdd.Add(item);
                }

                flpThreads.Controls.AddRange(controlsToAdd.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania wątków: {ex.Message}", "Błąd");
            }
            finally
            {
                flpThreads.ResumeLayout();
                this.Cursor = Cursors.Default;
            }
        }

        private async void ListItem_Click(object sender, EventArgs e)
        {
            var clickedItem = sender as MessageListItem;
            if (clickedItem == null || clickedItem == _selectedListItem) return;

            if (_selectedListItem != null) _selectedListItem.SetSelected(false);
            _selectedListItem = clickedItem;
            _selectedListItem.SetSelected(true);

            _activeDisputeId = clickedItem.DisputeId;

            // Zarządzanie widocznością UI
            if (lblNoConversation != null) lblNoConversation.Visible = false;

            // Pokaż przeglądarkę
            if (_chatBrowser != null) _chatBrowser.Visible = true;

            // Pokaż panel czatu (jeśli był ukryty)
            var pnlChat = this.Controls.Find("pnlChatArea", true).FirstOrDefault();
            if (pnlChat != null) pnlChat.Visible = true;

            // Wyświetl "Ładowanie..." w HTML (żeby user wiedział że działa)
            DisplayLoadingScreen();

            // ✅ ŁADOWANIE CZATU HTML (BŁYSKAWICZNE)
            await LoadChatAsHtmlAsync(_activeDisputeId);

            // Oznaczanie jako przeczytane (W TLE)
            var threadInfo = _threads.FirstOrDefault(t => t.DisputeId == _activeDisputeId);
            if (threadInfo != null && threadInfo.HasNewMessages)
            {
                threadInfo.HasNewMessages = false;
                clickedItem.SetData(threadInfo.DisputeId, threadInfo.BuyerLogin, threadInfo.AccountName, threadInfo.ComplaintNumber, threadInfo.LastMessageDate, threadInfo.LastMessageText, false);

                // Fire and forget - nie czekamy na bazę
                _ = Task.Run(() => UpdateReadStatusInDb(_activeDisputeId, 0));
                UpdateManager.NotifySubscribers();
            }
        }

        private void DisplayLoadingScreen()
        {
            if (_chatBrowser != null && !_chatBrowser.IsDisposed)
            {
                // Reset dokumentu
                _chatBrowser.Navigate("about:blank");
                if (_chatBrowser.Document != null)
                {
                    _chatBrowser.Document.Write("<html><body style='font-family:Segoe UI; color:#888; text-align:center; padding-top:50px;'>Wczytywanie wiadomości...</body></html>");
                }
            }
        }

        // === SILNIK HTML (GENIALNA SZYBKOŚĆ) ===
        private async Task LoadChatAsHtmlAsync(string disputeId)
        {
            // 1. Pobierz dane z bazy W TLE (nie blokuje UI)
            var messages = await Task.Run(() => GetMessagesData(disputeId));

            // 2. Pobierz klienta API w tle (żeby był gotowy do wysyłania)
            var threadInfo = _threads.FirstOrDefault(t => t.DisputeId == disputeId);
            if (threadInfo != null)
            {
                _ = Task.Run(async () => {
                    try { _activeApiClient = await _chatService.GetApiClientForAccountAsync(threadInfo.AllegroAccountId); } catch { }
                });
            }

            // 3. Generuj HTML (string operations są bardzo szybkie)
            var sb = new StringBuilder();
            sb.Append(GetCssHeader());
            sb.Append("<body><div class='chat-container'>");

            foreach (var msg in messages)
            {
                string rowClass = msg.IsSeller ? "row-seller" : "row-buyer";
                string bubbleClass = msg.IsSeller ? "bubble-seller" : "bubble-buyer";
                string metaClass = msg.IsSeller ? "" : "meta-buyer";
                string dateStr = msg.Date.ToString("dd.MM.yyyy HH:mm");
                string safeText = System.Net.WebUtility.HtmlEncode(msg.Text).Replace("\n", "<br>");

                // Obsługa załączników
                string attachmentsHtml = "";
                if (!string.IsNullOrEmpty(msg.JsonDetails))
                {
                    try
                    {
                        var json = JObject.Parse(msg.JsonDetails);
                        var attachments = json["attachments"];
                        if (attachments != null && attachments.HasValues)
                        {
                            attachmentsHtml = "<div class='attachments'>";
                            foreach (var att in attachments)
                            {
                                string fileName = att["fileName"]?.ToString() ?? "Plik";
                                string url = att["url"]?.ToString() ?? "#";
                                attachmentsHtml += $"<a href='{url}' target='_blank' class='attachment-link'>📎 {fileName}</a><br>";
                            }
                            attachmentsHtml += "</div>";
                        }
                    }
                    catch { }
                }

                sb.Append($@"
                    <div class='message-row {rowClass}'>
                        <div class='bubble {bubbleClass}'>
                            <span class='author'>{msg.Login}</span>
                            {safeText}
                            {attachmentsHtml}
                            <span class='meta {metaClass}'>{dateStr}</span>
                        </div>
                    </div>");
            }

            sb.Append(@"
                    </div>
                    <script>
                        window.scrollTo(0, document.body.scrollHeight);
                    </script>
                </body>
                </html>");

            // 4. Renderowanie (Wątek UI)
            if (_chatBrowser != null && !_chatBrowser.IsDisposed)
            {
                _chatBrowser.DocumentText = sb.ToString();
            }
        }

        // Metoda pomocnicza do pobierania danych (bezpośrednie SQL dla szybkości)
        private List<dynamic> GetMessagesData(string disputeId)
        {
            var list = new List<dynamic>();
            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                // ✅ OPTYMALIZACJA: Pobieramy tylko ostatnie 200 wiadomości (wystarczy do rozmowy)
                // Jeśli potrzeba starszych - użytkownik może kliknąć "Załaduj więcej"
                var sql = @"
                    SELECT AuthorLogin, CreatedAt, MessageText, AuthorRole, JsonDetails 
                    FROM AllegroChatMessages 
                    WHERE DisputeId = @DisputeId 
                    ORDER BY CreatedAt DESC
                    LIMIT 200";

                var cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@DisputeId", disputeId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Login = reader["AuthorLogin"].ToString(),
                            Date = DateTime.Parse(reader["CreatedAt"].ToString()),
                            Text = reader["MessageText"].ToString(),
                            IsSeller = reader["AuthorRole"].ToString() == "SELLER",
                            JsonDetails = reader["JsonDetails"].ToString()
                        });
                    }
                }
            }
            // ✅ WAŻNE: Odwracamy kolejność (bo pobraliśmy DESC, a wyświetlamy ASC)
            list.Reverse();
            return list;
        }

        private string GetCssHeader()
        {
            return @"
                <html>
                <head>
                    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                    <style>
                        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #ffffff; margin: 0; padding: 15px; }
                        .chat-container { display: flex; flex-direction: column; gap: 8px; padding-bottom: 20px; }
                        .message-row { display: flex; width: 100%; }
                        .row-seller { justify-content: flex-end; }
                        .row-buyer { justify-content: flex-start; }
                        
                        .bubble { 
                            max-width: 85%; 
                            padding: 8px 14px; 
                            border-radius: 12px; 
                            position: relative; 
                            font-size: 13px; 
                            line-height: 1.4;
                            box-shadow: 0 1px 2px rgba(0,0,0,0.1);
                            word-wrap: break-word;
                        }
                        
                        .bubble-seller { background-color: #0082fa; color: white; border-bottom-right-radius: 2px; }
                        .bubble-buyer { background-color: #f0f0f0; color: #1c1e21; border-bottom-left-radius: 2px; }

                        .meta { font-size: 10px; margin-top: 4px; opacity: 0.7; text-align: right; display: block; }
                        .meta-buyer { text-align: left; }
                        .author { font-weight: bold; display: block; font-size: 11px; margin-bottom: 3px; opacity: 0.9; }
                        
                        .attachments { margin-top: 5px; padding-top: 5px; border-top: 1px solid rgba(255,255,255,0.3); }
                        .bubble-buyer .attachments { border-top: 1px solid rgba(0,0,0,0.1); }
                        .attachment-link { text-decoration: none; color: inherit; font-size: 12px; display: block; margin-bottom: 2px; }
                    </style>
                </head>";
        }

        // === WYSYŁANIE WIADOMOŚCI ===
        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewMessage.Text) || _activeApiClient == null || string.IsNullOrEmpty(_activeDisputeId)) return;

            btnSendMessage.Enabled = false;
            var text = txtNewMessage.Text;

            try
            {
                var req = new NewMessageRequest { Text = text, Type = "REGULAR" };
                await _activeApiClient.SendMessageAsync(_activeDisputeId, req);

                txtNewMessage.Clear();

                // Po wysłaniu odświeżamy (mały delay, by Allegro przetworzyło)
                await Task.Delay(500);
                await LoadChatAsHtmlAsync(_activeDisputeId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
            finally
            {
                btnSendMessage.Enabled = true;
                txtNewMessage.Focus();
            }
        }

        // === OBSŁUGA STATUSÓW ===
        private void UpdateReadStatusInDb(string disputeId, int status)
        {
            // Metoda synchroniczna, wywoływana w Task.Run
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    var cmd = new MySqlCommand("UPDATE AllegroDisputes SET HasNewMessages = @status WHERE DisputeId = @id", con);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", disputeId);

                    // ✅ POPRAWIONE: ExecuteNonQuery zamiast ExecuteNonQueryNonQuery
                    cmd.ExecuteNonQuery();
                }
            }
            catch { /* Ignorujemy błędy aktualizacji statusu w tle */ }
        }

        private async void markAllAsReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (_threads == null || !_threads.Any(t => t.HasNewMessages))
                {
                    MessageBox.Show("Brak nowych wiadomości.", "Info");
                    return;
                }

                // Aktualizacja bazy w tle
                await Task.Run(() =>
                {
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        con.Open();
                        var ids = _threads.Where(t => t.HasNewMessages).Select(t => $"'{t.DisputeId}'").ToList();
                        if (ids.Any())
                        {
                            var cmd = new MySqlCommand($"UPDATE AllegroDisputes SET HasNewMessages = 0 WHERE DisputeId IN ({string.Join(",", ids)})", con);
                            cmd.ExecuteNonQuery();
                        }
                    }
                });

                MessageBox.Show("Wszystkie wiadomości zostały oznaczone jako przeczytane.", "Sukces");
                await LoadThreadsAsync();
                UpdateManager.NotifySubscribers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void MarkAsUnread_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_activeDisputeId)) return;

            try
            {
                await Task.Run(() => UpdateReadStatusInDb(_activeDisputeId, 1));
                await LoadThreadsAsync();
                UpdateManager.NotifySubscribers();
                MessageBox.Show("Oznaczono wątek jako nieprzeczytany.", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
        }

        private void btnOpenIssue_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_activeDisputeId))
            {
                var form = new FormAllegroIssue(_activeDisputeId);
                form.Show();
            }
        }
    }
}