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
using Newtonsoft.Json.Linq;

namespace Reklamacje_Dane
{
    public partial class FormAllegroChat : Form
    {
        private readonly string _disputeId;
        private readonly AllegroApiClient _apiClient;
        private readonly string _sellerLogin;

        // Zastępujemy FlowLayoutPanel przeglądarką
        private WebBrowser _chatBrowser;

        public FormAllegroChat(string disputeId, AllegroApiClient apiClient, string sellerLogin)
        {
            InitializeComponent();
            _disputeId = disputeId;
            _apiClient = apiClient;
            _sellerLogin = sellerLogin;
            this.Text = $"Dyskusja Allegro: {_disputeId}";

            InitializeChatBrowser();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeChatBrowser()
        {
            // Znajdź stary panel i ukryj go
            var oldPanel = this.Controls.Find("flpChatContainer", true).FirstOrDefault();
            if (oldPanel != null) oldPanel.Visible = false;

            // Tworzenie przeglądarki
            _chatBrowser = new WebBrowser();
            _chatBrowser.Dock = DockStyle.Fill;
            _chatBrowser.IsWebBrowserContextMenuEnabled = false;
            _chatBrowser.AllowNavigation = false;
            _chatBrowser.ScriptErrorsSuppressed = true;

            this.Controls.Add(_chatBrowser);
            _chatBrowser.BringToFront();

            // Upewnij się, że panel dolny (do pisania) jest na wierzchu
            foreach (Control c in this.Controls)
            {
                if (c is Panel && c.Bottom >= this.Height - 100) c.BringToFront();
            }
        }

        private async void FormAllegroChat_Load(object sender, EventArgs e)
        {
            try
            {
                await MarkDisputeAsReadAsync();
                await LoadMessagesHtmlAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania: " + ex.Message);
            }
        }

        private async Task MarkDisputeAsReadAsync()
        {
            await Task.Run(() =>
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    var cmd = new MySqlCommand("UPDATE AllegroDisputes SET HasNewMessages = 0 WHERE DisputeId = @DisputeId", con);
                    cmd.Parameters.AddWithValue("@DisputeId", _disputeId);
                    cmd.ExecuteNonQuery();
                }
            });
            UpdateManager.NotifySubscribers();
        }

        private async Task LoadMessagesHtmlAsync()
        {
            // 1. Pobierz dane w tle
            var messages = await Task.Run(() => GetMessagesData(_disputeId));

            // 2. Generuj HTML
            var sb = new StringBuilder();
            sb.Append(@"
                <html>
                <head>
                    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
                    <style>
                        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f0f2f5; margin: 0; padding: 10px; }
                        .chat-container { display: flex; flex-direction: column; gap: 10px; padding-bottom: 20px; }
                        .message-row { display: flex; width: 100%; }
                        .row-seller { justify-content: flex-end; }
                        .row-buyer { justify-content: flex-start; }
                        
                        .bubble { 
                            max-width: 80%; 
                            padding: 10px 15px; 
                            border-radius: 18px; 
                            position: relative; 
                            font-size: 14px; 
                            line-height: 1.4;
                            box-shadow: 0 1px 2px rgba(0,0,0,0.1);
                        }
                        
                        .bubble-seller { background-color: #0084ff; color: white; border-bottom-right-radius: 4px; }
                        .bubble-buyer { background-color: #e4e6eb; color: black; border-bottom-left-radius: 4px; }

                        .meta { font-size: 10px; margin-top: 5px; opacity: 0.7; text-align: right; display: block; }
                        .meta-buyer { text-align: left; color: #606060; }
                        .author { font-weight: bold; display: block; font-size: 11px; margin-bottom: 2px; }
                        
                        .attachments { margin-top: 5px; padding-top: 5px; border-top: 1px solid rgba(255,255,255,0.3); }
                        .bubble-buyer .attachments { border-top: 1px solid rgba(0,0,0,0.1); }
                        .attachment-link { text-decoration: none; color: inherit; font-size: 12px; display: block; margin-bottom: 2px; }
                    </style>
                </head>
                <body>
                    <div class='chat-container'>");

            foreach (var msg in messages)
            {
                string rowClass = msg.IsSeller ? "row-seller" : "row-buyer";
                string bubbleClass = msg.IsSeller ? "bubble-seller" : "bubble-buyer";
                string metaClass = msg.IsSeller ? "" : "meta-buyer";
                string dateStr = msg.Date.ToString("dd.MM HH:mm");
                string safeText = System.Net.WebUtility.HtmlEncode(msg.Text).Replace("\n", "<br>");

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

            // 3. Renderowanie
            if (_chatBrowser != null && !_chatBrowser.IsDisposed)
            {
                _chatBrowser.DocumentText = sb.ToString();
            }
        }

        private List<dynamic> GetMessagesData(string disputeId)
        {
            var list = new List<dynamic>();
            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                var cmd = new MySqlCommand(@"
                    SELECT AuthorLogin, CreatedAt, MessageText, AuthorRole, JsonDetails 
                    FROM AllegroChatMessages 
                    WHERE DisputeId = @DisputeId 
                    ORDER BY CreatedAt ASC", con);

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
            return list;
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewMessage.Text) || _apiClient == null) return;

            btnSendMessage.Enabled = false;
            var messageText = txtNewMessage.Text;

            try
            {
                var newMessage = new NewMessageRequest { Text = messageText, Type = "REGULAR" };
                await _apiClient.SendMessageAsync(_disputeId, newMessage);

                txtNewMessage.Clear();

                await Task.Delay(800);
                await LoadMessagesHtmlAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się wysłać wiadomości: {ex.Message}", "Błąd API");
            }
            finally
            {
                btnSendMessage.Enabled = true;
                txtNewMessage.Focus();
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