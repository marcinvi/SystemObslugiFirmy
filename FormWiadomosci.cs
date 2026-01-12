using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Reklamacje_Dane.Allegro;
using Newtonsoft.Json.Linq;

namespace Reklamacje_Dane
{
    /// <summary>
    /// SUPER PROSTA WERSJA - ZAWSZE DZIAŁA!
    /// </summary>
    public partial class FormWiadomosci : Form
    {
        private List<AllegroChatService.ThreadInfo> _allThreads = new List<AllegroChatService.ThreadInfo>();
        private int _displayedCount = 0;
        private const int BATCH_SIZE = 10;
        
        private MessageListItem _selectedListItem;
        private AllegroApiClient _activeApiClient;
        private string _activeDisputeId;
        
        // PROSTY PANEL NA CZAT
        private Panel _chatPanel;
        private FlowLayoutPanel _chatFlow;
        
        private Button _btnLoadMore;
        private readonly AllegroChatService _chatService;
        private ToolStripMenuItem markAsUnreadMenuItem;

        public FormWiadomosci()
        {
            InitializeComponent();
            _chatService = new AllegroChatService();
            
            InitializeCustomMenu();
            SetupSimpleChatPanel();
            SetupLoadMoreButton();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void SetupSimpleChatPanel()
        {
            // Znajdź obszar czatu
            var chatArea = this.Controls.Find("pnlChatArea", true).FirstOrDefault();
            if (chatArea == null)
            {
                var split = this.Controls.Find("splitContainerMain", true).FirstOrDefault() as SplitContainer;
                chatArea = split?.Panel2;
            }

            if (chatArea != null)
            {
                // Główny panel z autoscrollem
                _chatPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    BackColor = Color.White,
                    Visible = false
                };

                // FlowLayoutPanel dla wiadomości
                _chatFlow = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoSize = true,
                    Width = 800,
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };

                _chatPanel.Controls.Add(_chatFlow);
                chatArea.Controls.Add(_chatPanel);
                _chatPanel.BringToFront();

                // Update width on resize
                _chatPanel.Resize += (s, e) =>
                {
                    if (_chatFlow != null)
                        _chatFlow.Width = _chatPanel.Width - 25;
                };
            }
        }

        private void SetupLoadMoreButton()
        {
            _btnLoadMore = new Button
            {
                Text = "▼ Załaduj więcej wątków ▼",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            _btnLoadMore.FlatAppearance.BorderSize = 0;
            _btnLoadMore.Click += (s, e) => LoadMoreThreads();

            if (flpThreads?.Parent != null)
            {
                flpThreads.Parent.Controls.Add(_btnLoadMore);
                _btnLoadMore.BringToFront();
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

            this.Text = "Ładowanie wątków...";
            this.Cursor = Cursors.WaitCursor;

            try
            {
                _allThreads = await Task.Run(() => LoadAllThreadsFromDB());
                DisplayThreadBatch(0, BATCH_SIZE);
                this.Text = $"Centrum Wiadomości ({_allThreads.Count})";

                if (_allThreads.Count > BATCH_SIZE)
                {
                    _btnLoadMore.Text = $"▼ Załaduj więcej ({_allThreads.Count - BATCH_SIZE}) ▼";
                    _btnLoadMore.Visible = true;

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(1500);
                        for (int i = BATCH_SIZE; i < _allThreads.Count; i += BATCH_SIZE)
                        {
                            await Task.Delay(300);
                            int start = i;
                            int count = Math.Min(BATCH_SIZE, _allThreads.Count - i);
                            this.Invoke((Action)(() => DisplayThreadBatch(start, count)));
                        }
                        this.Invoke((Action)(() => _btnLoadMore.Visible = false));
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private List<AllegroChatService.ThreadInfo> LoadAllThreadsFromDB()
        {
            var threads = new List<AllegroChatService.ThreadInfo>();

            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                var sql = @"
                    SELECT 
                        AD.DisputeId, AD.BuyerLogin, AD.AllegroAccountId, AD.HasNewMessages,
                        AA.AccountName, Z.NrZgloszenia,
                        (SELECT MessageText FROM AllegroChatMessages 
                         WHERE DisputeId = AD.DisputeId ORDER BY CreatedAt DESC LIMIT 1) as LastMsg,
                        (SELECT CreatedAt FROM AllegroChatMessages 
                         WHERE DisputeId = AD.DisputeId ORDER BY CreatedAt DESC LIMIT 1) as LastDate
                    FROM AllegroDisputes AD
                    JOIN AllegroAccounts AA ON AD.AllegroAccountId = AA.Id
                    LEFT JOIN Zgloszenia Z ON AD.ComplaintId = Z.Id
                    WHERE EXISTS (SELECT 1 FROM AllegroChatMessages WHERE DisputeId = AD.DisputeId)
                    ORDER BY LastDate DESC LIMIT 500";

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
                            LastMessageText = reader["LastMsg"]?.ToString() ?? "",
                            LastMessageDate = reader["LastDate"] != DBNull.Value 
                                ? Convert.ToDateTime(reader["LastDate"]) : DateTime.MinValue
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
                item.SetData(thread.DisputeId, thread.BuyerLogin, thread.AccountName,
                           thread.ComplaintNumber, thread.LastMessageDate, thread.LastMessageText, thread.HasNewMessages);
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
                _displayedCount++;
            }
            flpThreads.ResumeLayout();

            if (_btnLoadMore != null)
            {
                int remaining = _allThreads.Count - _displayedCount;
                _btnLoadMore.Text = remaining > 0 ? $"▼ Załaduj więcej ({remaining}) ▼" : "";
                _btnLoadMore.Visible = remaining > 0;
            }
        }

        private void LoadMoreThreads()
        {
            _btnLoadMore.Enabled = false;
            _btnLoadMore.Text = "Ładowanie...";
            try
            {
                int count = Math.Min(BATCH_SIZE, _allThreads.Count - _displayedCount);
                DisplayThreadBatch(_displayedCount, count);
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

            if (_selectedListItem != null) _selectedListItem.SetSelected(false);
            _selectedListItem = item;
            _selectedListItem.SetSelected(true);

            var thread = item.Tag as AllegroChatService.ThreadInfo;
            if (thread == null) return;

            _activeDisputeId = thread.DisputeId;

            try
            {
                // Pokaż panel
                if (lblNoConversation != null) lblNoConversation.Visible = false;
                var pnlChat = this.Controls.Find("pnlChatArea", true).FirstOrDefault();
                if (pnlChat != null) pnlChat.Visible = true;

                // Wyczyść stare wiadomości
                if (_chatFlow != null)
                {
                    _chatFlow.Controls.Clear();
                    _chatFlow.SuspendLayout();
                }

                // Pokaż panel czatu
                if (_chatPanel != null)
                {
                    _chatPanel.Visible = true;
                }

                // Załaduj wiadomości
                var messages = await Task.Run(() => LoadMessages(_activeDisputeId));

                // DEBUGOWANIE
                if (messages.Count == 0)
                {
                    MessageBox.Show($"Brak wiadomości dla DisputeId: {_activeDisputeId}");
                    return;
                }

                // Dodaj kontrolki
                foreach (var msg in messages)
                {
                    var msgControl = new ChatMessageControl();
                    msgControl.SetData(msg.Login, msg.Text, msg.Date, msg.IsSeller);
                    msgControl.Width = _chatFlow.Width - 20;
                    _chatFlow.Controls.Add(msgControl);
                }

                if (_chatFlow != null)
                {
                    _chatFlow.ResumeLayout();
                }

                // Przewiń na dół
                if (_chatPanel != null)
                {
                    Application.DoEvents();
                    _chatPanel.AutoScrollPosition = new Point(0, _chatFlow.Height);
                }

                // Oznacz jako przeczytane
                if (thread.HasNewMessages)
                {
                    thread.HasNewMessages = false;
                    item.SetData(thread.DisputeId, thread.BuyerLogin, thread.AccountName,
                               thread.ComplaintNumber, thread.LastMessageDate, thread.LastMessageText, false);
                    _ = Task.Run(() => MarkAsRead(_activeDisputeId));
                    UpdateManager.NotifySubscribers();
                }

                // API client w tle
                _ = Task.Run(async () =>
                {
                    try { _activeApiClient = await _chatService.GetApiClientForAccountAsync(thread.AllegroAccountId); }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}\n\n{ex.StackTrace}", "BŁĄD");
            }
        }

        private List<dynamic> LoadMessages(string disputeId)
        {
            var list = new List<dynamic>();
            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                var cmd = new MySqlCommand(
                    "SELECT AuthorLogin, CreatedAt, MessageText, AuthorRole " +
                    "FROM AllegroChatMessages WHERE DisputeId = @id ORDER BY CreatedAt ASC", con);
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
                            IsSeller = reader["AuthorRole"].ToString() == "SELLER"
                        });
                    }
                }
            }
            return list;
        }

        private void MarkAsRead(string disputeId)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    var cmd = new MySqlCommand("UPDATE AllegroDisputes SET HasNewMessages = 0 WHERE DisputeId = @id", con);
                    cmd.Parameters.AddWithValue("@id", disputeId);
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
                
                var messages = await Task.Run(() => LoadMessages(_activeDisputeId));
                _chatFlow.Controls.Clear();
                foreach (var msg in messages)
                {
                    var msgControl = new ChatMessageControl();
                    msgControl.SetData(msg.Login, msg.Text, msg.Date, msg.IsSeller);
                    msgControl.Width = _chatFlow.Width - 20;
                    _chatFlow.Controls.Add(msgControl);
                }
                
                Application.DoEvents();
                _chatPanel.AutoScrollPosition = new Point(0, _chatFlow.Height);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
            finally
            {
                btnSendMessage.Enabled = true;
                txtNewMessage?.Focus();
            }
        }

        private async void markAllAsReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var unread = _allThreads.Where(t => t.HasNewMessages).ToList();
            if (unread.Count == 0)
            {
                MessageBox.Show("Brak nieprzeczytanych.");
                return;
            }

            await Task.Run(() =>
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    var ids = string.Join(",", unread.Select(t => $"'{t.DisputeId}'"));
                    var cmd = new MySqlCommand($"UPDATE AllegroDisputes SET HasNewMessages = 0 WHERE DisputeId IN ({ids})", con);
                    cmd.ExecuteNonQuery();
                }
            });

            foreach (var t in unread) t.HasNewMessages = false;
            MessageBox.Show("Oznaczono jako przeczytane.");
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
                    cmd.Parameters.AddWithValue("@id", _activeDisputeId);
                    cmd.ExecuteNonQuery();
                }
            });

            MessageBox.Show("Oznaczono jako nieprzeczytane.");
            UpdateManager.NotifySubscribers();
        }

        private void btnOpenIssue_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_activeDisputeId))
            {
                new FormAllegroIssue(_activeDisputeId).Show();
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
