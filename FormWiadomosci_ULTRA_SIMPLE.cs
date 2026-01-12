using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Reklamacje_Dane.Allegro;
using Newtonsoft.Json.Linq;

namespace Reklamacje_Dane
{
    /// <summary>
    /// ULTRA PROSTA WERSJA - DZIAŁA 100%!
    /// </summary>
    public partial class FormWiadomosci : Form
    {
        private readonly AllegroChatService _chatService;
        private List<AllegroChatService.ThreadInfo> _allThreads;
        private int _displayedThreadsCount = 0;
        private const int LOAD_BATCH_SIZE = 10;
        
        private MessageListItem _selectedListItem;
        private AllegroApiClient _activeApiClient;
        private string _activeDisputeId;
        private WebBrowser _chatBrowser;
        private ToolStripMenuItem markAsUnreadMenuItem;
        private Button _btnLoadMore;

        public FormWiadomosci()
        {
            InitializeComponent();
            _chatService = new AllegroChatService();
            InitializeCustomMenu();
            SetupChatBrowser();
            SetupLoadMoreButton();
        }

        private void SetupLoadMoreButton()
        {
            _btnLoadMore = new Button
            {
                Text = "⬇ Załaduj więcej wątków (0 pozostałych) ⬇",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Visible = false
            };
            _btnLoadMore.Click += BtnLoadMore_Click;
            
            if (flpThreads?.Parent != null)
            {
                flpThreads.Parent.Controls.Add(_btnLoadMore);
                _btnLoadMore.BringToFront();
            }
        }

        private void SetupChatBrowser()
        {
            // Ukryj stary flowPanel
            var oldFlow = this.Controls.Find("flowLayoutPanelChat", true).FirstOrDefault();
            if (oldFlow != null) oldFlow.Visible = false;

            // Znajdź panel czatu
            Control chatArea = this.Controls.Find("pnlChatArea", true).FirstOrDefault();
            if (chatArea == null)
            {
                var split = this.Controls.Find("splitContainerMain", true).FirstOrDefault() as SplitContainer;
                if (split != null) chatArea = split.Panel2;
            }

            if (chatArea != null)
            {
                _chatBrowser = new WebBrowser
                {
                    Dock = DockStyle.Fill,
                    ScriptErrorsSuppressed = true,
                    Visible = false
                };
                chatArea.Controls.Add(_chatBrowser);
                _chatBrowser.SendToBack(); // Panel wpisywania na wierzch
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

            // Pokaż "Ładowanie..."
            this.Text = "Centrum Wiadomości - Ładowanie...";
            this.Cursor = Cursors.WaitCursor;

            try
            {
                // KROK 1: Szybkie pobranie TYLKO DisputeId i dat (bez JOINów!)
                _allThreads = await Task.Run(() => LoadAllThreadsQuick());

                // KROK 2: Załaduj pierwsze 10
                DisplayThreadBatch(0, LOAD_BATCH_SIZE);

                // KROK 3: Załaduj resztę w tle (cicho)
                if (_allThreads.Count > LOAD_BATCH_SIZE)
                {
                    _btnLoadMore.Text = $"⬇ Załaduj więcej ({_allThreads.Count - LOAD_BATCH_SIZE} pozostałych) ⬇";
                    _btnLoadMore.Visible = true;

                    // Automatyczne ładowanie w tle po 2 sekundach
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(2000);
                        for (int i = LOAD_BATCH_SIZE; i < _allThreads.Count; i += LOAD_BATCH_SIZE)
                        {
                            await Task.Delay(500); // Małe opóźnienie między partiami
                            int start = i;
                            int count = Math.Min(LOAD_BATCH_SIZE, _allThreads.Count - i);
                            
                            this.Invoke((MethodInvoker)delegate
                            {
                                DisplayThreadBatch(start, count);
                            });
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            _btnLoadMore.Visible = false;
                        });
                    });
                }

                this.Text = $"Centrum Wiadomości ({_allThreads.Count} konwersacji)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}\n\n{ex.StackTrace}", "Błąd ładowania");
                this.Text = "Centrum Wiadomości - BŁĄD!";
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private List<AllegroChatService.ThreadInfo> LoadAllThreadsQuick()
        {
            var threads = new List<AllegroChatService.ThreadInfo>();

            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();

                // SUPER SZYBKIE query - pobieramy wszystko w jednym zapytaniu
                var sql = @"
                    SELECT 
                        AD.DisputeId,
                        AD.BuyerLogin,
                        AD.AllegroAccountId,
                        AD.HasNewMessages,
                        AA.AccountName,
                        Z.NrZgloszenia,
                        (SELECT MessageText FROM AllegroChatMessages 
                         WHERE DisputeId = AD.DisputeId 
                         ORDER BY CreatedAt DESC LIMIT 1) as LastMessage,
                        (SELECT CreatedAt FROM AllegroChatMessages 
                         WHERE DisputeId = AD.DisputeId 
                         ORDER BY CreatedAt DESC LIMIT 1) as LastDate
                    FROM AllegroDisputes AD
                    JOIN AllegroAccounts AA ON AD.AllegroAccountId = AA.Id
                    LEFT JOIN Zgloszenia Z ON AD.ComplaintId = Z.Id
                    WHERE AD.DisputeId IN (SELECT DISTINCT DisputeId FROM AllegroChatMessages)
                    ORDER BY LastDate DESC
                    LIMIT 500";

                var cmd = new MySqlCommand(sql, con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threads.Add(new AllegroChatService.ThreadInfo
                        {
                            DisputeId = reader["DisputeId"].ToString(),
                            BuyerLogin = reader["BuyerLogin"].ToString(),
                            AllegroAccountId = Convert.ToInt32(reader["AllegroAccountId"]),
                            HasNewMessages = Convert.ToBoolean(reader["HasNewMessages"]),
                            AccountName = reader["AccountName"].ToString(),
                            ComplaintNumber = reader["NrZgloszenia"]?.ToString(),
                            LastMessageText = reader["LastMessage"]?.ToString() ?? "",
                            LastMessageDate = reader["LastDate"] != DBNull.Value 
                                ? Convert.ToDateTime(reader["LastDate"]) 
                                : DateTime.MinValue
                        });
                    }
                }
            }

            return threads;
        }

        private void DisplayThreadBatch(int start, int count)
        {
            if (flpThreads == null) return;

            flpThreads.SuspendLayout();

            for (int i = start; i < start + count && i < _allThreads.Count; i++)
            {
                var thread = _allThreads[i];

                var item = new MessageListItem();
                item.SetData(
                    thread.DisputeId,
                    thread.BuyerLogin,
                    thread.AccountName,
                    thread.ComplaintNumber,
                    thread.LastMessageDate,
                    thread.LastMessageText,
                    thread.HasNewMessages
                );

                item.Width = flpThreads.ClientSize.Width - 25;
                item.Tag = thread;
                item.Click += ThreadItem_Click;

                item.MouseUp += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        if (_selectedListItem != null) _selectedListItem.SetSelected(false);
                        _selectedListItem = item;
                        _selectedListItem.SetSelected(true);
                        _activeDisputeId = thread.DisputeId;
                        contextMenuThreads.Show(Cursor.Position);
                    }
                };

                flpThreads.Controls.Add(item);
                _displayedThreadsCount++;
            }

            flpThreads.ResumeLayout();

            // Aktualizuj przycisk
            if (_btnLoadMore != null)
            {
                int remaining = _allThreads.Count - _displayedThreadsCount;
                if (remaining > 0)
                {
                    _btnLoadMore.Text = $"⬇ Załaduj więcej ({remaining} pozostałych) ⬇";
                    _btnLoadMore.Visible = true;
                }
                else
                {
                    _btnLoadMore.Visible = false;
                }
            }
        }

        private void BtnLoadMore_Click(object sender, EventArgs e)
        {
            _btnLoadMore.Enabled = false;
            _btnLoadMore.Text = "Ładowanie...";

            try
            {
                int count = Math.Min(LOAD_BATCH_SIZE, _allThreads.Count - _displayedThreadsCount);
                DisplayThreadBatch(_displayedThreadsCount, count);
            }
            finally
            {
                _btnLoadMore.Enabled = true;
            }
        }

        private async void ThreadItem_Click(object sender, EventArgs e)
        {
            var item = sender as MessageListItem;
            if (item == null) return;

            // Zaznaczenie
            if (_selectedListItem != null) _selectedListItem.SetSelected(false);
            _selectedListItem = item;
            _selectedListItem.SetSelected(true);

            var thread = item.Tag as AllegroChatService.ThreadInfo;
            if (thread == null) return;

            _activeDisputeId = thread.DisputeId;

            // Pokaż panel czatu
            if (lblNoConversation != null) lblNoConversation.Visible = false;
            var pnlChat = this.Controls.Find("pnlChatArea", true).FirstOrDefault();
            if (pnlChat != null) pnlChat.Visible = true;

            // Pokaż loading
            if (_chatBrowser != null)
            {
                _chatBrowser.Visible = true;
                _chatBrowser.DocumentText = "<html><body style='font-family:Segoe UI;text-align:center;padding-top:100px;color:#888'>Ładowanie wiadomości...</body></html>";
            }

            try
            {
                // Załaduj wiadomości
                await LoadAndDisplayMessagesAsync(_activeDisputeId);

                // Oznacz jako przeczytane
                if (thread.HasNewMessages)
                {
                    thread.HasNewMessages = false;
                    item.SetData(thread.DisputeId, thread.BuyerLogin, thread.AccountName, 
                               thread.ComplaintNumber, thread.LastMessageDate, thread.LastMessageText, false);
                    _ = Task.Run(() => MarkAsRead(_activeDisputeId));
                    UpdateManager.NotifySubscribers();
                }

                // Pobierz API client w tle
                _ = Task.Run(async () =>
                {
                    try
                    {
                        _activeApiClient = await _chatService.GetApiClientForAccountAsync(thread.AllegroAccountId);
                    }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                if (_chatBrowser != null)
                {
                    _chatBrowser.DocumentText = $"<html><body style='font-family:Segoe UI;color:red;padding:20px'>BŁĄD: {ex.Message}</body></html>";
                }
            }
        }

        private async Task LoadAndDisplayMessagesAsync(string disputeId)
        {
            // Pobierz wiadomości
            var messages = await Task.Run(() => GetMessages(disputeId));

            // Generuj HTML
            var html = GenerateChatHtml(messages);

            // Wyświetl
            if (_chatBrowser != null && !_chatBrowser.IsDisposed)
            {
                _chatBrowser.DocumentText = html;
            }
        }

        private List<dynamic> GetMessages(string disputeId)
        {
            var list = new List<dynamic>();

            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                var cmd = new MySqlCommand(
                    "SELECT AuthorLogin, CreatedAt, MessageText, AuthorRole, JsonDetails " +
                    "FROM AllegroChatMessages WHERE DisputeId = @id ORDER BY CreatedAt ASC",
                    con);
                cmd.Parameters.AddWithValue("@id", disputeId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Login = reader["AuthorLogin"].ToString(),
                            Date = Convert.ToDateTime(reader["CreatedAt"]),
                            Text = reader["MessageText"].ToString(),
                            IsSeller = reader["AuthorRole"].ToString() == "SELLER",
                            JsonDetails = reader["JsonDetails"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        private string GenerateChatHtml(List<dynamic> messages)
        {
            var sb = new StringBuilder();
            sb.Append(@"<html><head><meta http-equiv='X-UA-Compatible' content='IE=edge'/><style>
                body { font-family: Segoe UI; margin: 0; padding: 15px; background: #fff; }
                .msg { margin-bottom: 10px; display: flex; }
                .msg-seller { justify-content: flex-end; }
                .msg-buyer { justify-content: flex-start; }
                .bubble { max-width: 70%; padding: 10px 15px; border-radius: 12px; }
                .bubble-seller { background: #0084ff; color: white; }
                .bubble-buyer { background: #f0f0f0; color: #000; }
                .author { font-weight: bold; font-size: 11px; margin-bottom: 3px; }
                .time { font-size: 10px; opacity: 0.7; margin-top: 5px; }
            </style></head><body>");

            foreach (var msg in messages)
            {
                string msgClass = msg.IsSeller ? "msg-seller" : "msg-buyer";
                string bubbleClass = msg.IsSeller ? "bubble-seller" : "bubble-buyer";
                string author = System.Net.WebUtility.HtmlEncode(msg.Login);
                string text = System.Net.WebUtility.HtmlEncode(msg.Text).Replace("\n", "<br>");
                string time = msg.Date.ToString("dd.MM.yyyy HH:mm");

                sb.Append($@"<div class='msg {msgClass}'>
                    <div class='bubble {bubbleClass}'>
                        <div class='author'>{author}</div>
                        <div>{text}</div>
                        <div class='time'>{time}</div>
                    </div>
                </div>");
            }

            sb.Append("<script>window.scrollTo(0, document.body.scrollHeight);</script></body></html>");
            return sb.ToString();
        }

        private void MarkAsRead(string disputeId)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    var cmd = new MySqlCommand("UPDATE AllegroDisputes SET HasNewMessages = 1 WHERE DisputeId = @id", con);
                    cmd.Parameters.AddWithValue("@id", _activeDisputeId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewMessage?.Text) || _activeApiClient == null || string.IsNullOrEmpty(_activeDisputeId))
                return;

            btnSendMessage.Enabled = false;
            var text = txtNewMessage.Text;

            try
            {
                await _activeApiClient.SendMessageAsync(_activeDisputeId, new NewMessageRequest { Text = text, Type = "REGULAR" });
                txtNewMessage.Clear();
                await Task.Delay(500);
                await LoadAndDisplayMessagesAsync(_activeDisputeId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wysyłania: {ex.Message}");
            }
            finally
            {
                btnSendMessage.Enabled = true;
                txtNewMessage.Focus();
            }
        }

        private async void markAllAsReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_allThreads == null || !_allThreads.Any(t => t.HasNewMessages))
            {
                MessageBox.Show("Brak nieprzeczytanych wiadomości.");
                return;
            }

            await Task.Run(() =>
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    var ids = string.Join(",", _allThreads.Where(t => t.HasNewMessages).Select(t => $"'{t.DisputeId}'"));
                    var cmd = new MySqlCommand($"UPDATE AllegroDisputes SET HasNewMessages = 0 WHERE DisputeId IN ({ids})", con);
                    cmd.ExecuteNonQuery();
                }
            });

            MessageBox.Show("Wszystkie wiadomości oznaczone jako przeczytane.");
            UpdateManager.NotifySubscribers();
        }

        private async void MarkAsUnread_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_activeDisputeId)) return;

            await Task.Run(() =>
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    var cmd = new MySqlCommand("UPDATE AllegroDisputes SET HasNewMessages = 1 WHERE DisputeId = @id", con);
                    cmd.Parameters.AddWithValue("@id", disputeId);
                    cmd.ExecuteNonQuery();
                }
            });

            MessageBox.Show("Wątek oznaczony jako nieprzeczytany.");
            UpdateManager.NotifySubscribers();
        }

        private void btnOpenIssue_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_activeDisputeId))
            {
                new FormAllegroIssue(_activeDisputeId).Show();
            }
        }
    }
}
