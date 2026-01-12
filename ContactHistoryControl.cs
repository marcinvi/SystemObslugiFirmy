using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions; // Potrzebne do czyszczenia tagów HTML

namespace Reklamacje_Dane
{
    public partial class ContactHistoryControl : UserControl
    {
        // UI Komponenty
        private SplitContainer splitContainer;
        private TabControl tabControlLeft;
        private ListBox lbThreads;

        // Prawa strona
        private Panel pnlChatHeader;
        private Label lblChatTitle;
        private Button btnGoToClaim;

        private Panel pnlChatContainer;
        private FlowLayoutPanel flowChat;

        // Dane
        private ContactRepository _repo = new ContactRepository();
        private int _currentEntityId;
        private string _currentClaimNumber;
        private bool _isViewingClaims = false;

        public ContactHistoryControl()
        {
            InitializeComponent();
            LoadList(false); // Domyślnie ładuj listę klientów
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1200, 750);
            this.BackColor = Color.White;

            // --- GŁÓWNY PODZIAŁ ---
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel1,
                IsSplitterFixed = false,
                SplitterDistance = 300,
                SplitterWidth = 5,
                BackColor = Color.WhiteSmoke
            };

            // --- LEWA STRONA ---
            var pnlLeft = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            tabControlLeft = new TabControl { Dock = DockStyle.Top, Height = 25 };
            tabControlLeft.TabPages.Add("Klienci");
            tabControlLeft.TabPages.Add("Zgłoszenia");
            tabControlLeft.SelectedIndexChanged += (s, e) => LoadList(tabControlLeft.SelectedIndex == 1);

            lbThreads = new ListBox
            {
                Dock = DockStyle.Fill,
                DrawMode = DrawMode.OwnerDrawVariable,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(245, 246, 247),
                IntegralHeight = false
            };
            lbThreads.MeasureItem += (s, e) => e.ItemHeight = 45;
            lbThreads.DrawItem += LbThreads_DrawItem;
            lbThreads.SelectedIndexChanged += LbThreads_SelectedIndexChanged;

            pnlLeft.Controls.Add(lbThreads);
            pnlLeft.Controls.Add(tabControlLeft);

            splitContainer.Panel1.Controls.Add(pnlLeft);
            splitContainer.Panel1MinSize = 250;

            // --- PRAWA STRONA ---

            // Nagłówek
            pnlChatHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            pnlChatHeader.Paint += (s, e) => e.Graphics.DrawLine(Pens.LightGray, 0, 59, pnlChatHeader.Width, 59);

            // Tytuł rozmowy
            lblChatTitle = new Label
            {
                Text = "Wybierz kontakt",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(15, 10),
                Size = new Size(500, 40),
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };

            // PRZYCISK
            btnGoToClaim = new Button
            {
                Text = "Otwórz zgłoszenie ↗",
                Size = new Size(160, 35),
                Location = new Point(pnlChatHeader.Width - 180, 12),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                BackColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat,
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnGoToClaim.Click += BtnGoToClaim_Click;

            pnlChatHeader.Controls.Add(btnGoToClaim);
            pnlChatHeader.Controls.Add(lblChatTitle);

            // Kontener dymków
            pnlChatContainer = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White };

            flowChat = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 20)
            };

            pnlChatContainer.Controls.Add(flowChat);
            pnlChatContainer.SizeChanged += (s, e) => {
                flowChat.Width = pnlChatContainer.ClientSize.Width - 20;
                foreach (Control c in flowChat.Controls) c.Width = flowChat.ClientSize.Width - 10;
            };

            splitContainer.Panel2.Controls.Add(pnlChatContainer);
            splitContainer.Panel2.Controls.Add(pnlChatHeader);

            this.Controls.Add(splitContainer);
        }

        // ================= LOGIKA LEWEJ STRONY =================

        private void LoadList(bool showClaims)
        {
            _isViewingClaims = showClaims;
            lbThreads.Items.Clear();
            lblChatTitle.Text = "Wybierz element z listy";

            btnGoToClaim.Visible = false;

            flowChat.Controls.Clear();

            try
            {
                var data = showClaims ? _repo.GetThreadsByClaim() : _repo.GetThreadsByClient();
                foreach (var item in data) lbThreads.Items.Add(item);
                AutoSizeSplitter();
            }
            catch (Exception ex) { MessageBox.Show("Błąd bazy: " + ex.Message); }
        }

        private void AutoSizeSplitter()
        {
            if (lbThreads.Items.Count == 0) return;
            int maxWidth = 0;
            using (Graphics g = lbThreads.CreateGraphics())
            {
                Font font = new Font("Segoe UI", 10, FontStyle.Bold);
                foreach (ConversationThread item in lbThreads.Items)
                {
                    int w = (int)g.MeasureString(item.Title, font).Width;
                    if (w > maxWidth) maxWidth = w;
                }
            }
            int newWidth = maxWidth + 90;
            if (newWidth < 280) newWidth = 280;
            if (newWidth > 550) newWidth = 550;
            splitContainer.SplitterDistance = newWidth;
        }

        private void LbThreads_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lbThreads.Items.Count) return;
            ConversationThread item = (ConversationThread)lbThreads.Items[e.Index];
            e.DrawBackground();

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            using (Brush bgBrush = new SolidBrush(isSelected ? Color.FromArgb(204, 229, 255) : Color.White))
                e.Graphics.FillRectangle(bgBrush, e.Bounds);

            e.Graphics.DrawLine(Pens.LightGray, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            Font titleFont = new Font("Segoe UI", 10, isSelected ? FontStyle.Bold : FontStyle.Regular);
            Font dateFont = new Font("Segoe UI", 8);
            Rectangle rect = e.Bounds;
            rect.Inflate(-10, -5);

            e.Graphics.DrawString(item.Title, titleFont, Brushes.Black, rect.X, rect.Y + 10);

            string dateStr = item.LastDate.ToString("dd.MM");
            SizeF dateSize = e.Graphics.MeasureString(dateStr, dateFont);
            e.Graphics.DrawString(dateStr, dateFont, Brushes.Gray, rect.Right - dateSize.Width, rect.Y + 12);

            e.DrawFocusRectangle();
        }

        private void LbThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbThreads.SelectedItem is ConversationThread item)
            {
                lblChatTitle.Text = item.Title;
                _currentEntityId = item.EntityId;
                _currentClaimNumber = item.ClaimNumber;

                if (_isViewingClaims && !string.IsNullOrEmpty(_currentClaimNumber))
                {
                    btnGoToClaim.Visible = true;
                    btnGoToClaim.Text = $"Otwórz {_currentClaimNumber} ↗";
                }
                else
                {
                    btnGoToClaim.Visible = false;
                }

                LoadChat(_currentEntityId, _isViewingClaims);
            }
        }

        private void BtnGoToClaim_Click(object sender, EventArgs e)
        {
            if (_isViewingClaims && !string.IsNullOrEmpty(_currentClaimNumber))
            {
                try
                {
                    Form2 form = new Form2(_currentClaimNumber);
                    form.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd: " + ex.Message);
                }
            }
        }

        // ================= LOGIKA CZATU =================

        private void LoadChat(int id, bool isClaim)
        {
            flowChat.SuspendLayout();
            flowChat.Controls.Clear();

            try
            {
                // 1. Pobieramy historię z bazy
                List<RawMessage> messages = _repo.GetHistoryForThread(
                    isClaim ? (int?)null : id,
                    isClaim ? id : (int?)null
                );

                // 2. Iterujemy przez wiadomości
                foreach (var msg in messages)
                {
                    bool isIncoming = (msg.Kierunek == "IN");
                    string typeCode = "UNKNOWN";
                    string t = (msg.Typ ?? "").ToUpper();

                    if (t.Contains("SMS")) typeCode = "SMS";
                    else if (t.Contains("MAIL") || t.Contains("EMAIL") || t.Contains("MULTI")) typeCode = "MAIL"; // Multi to też mail
                    else if (t.Contains("ALLEGRO")) typeCode = "ALLEGRO";

                    // 3. Przygotowanie tekstu (CZYŚCIMY RTF I HTML)
                    string textToDisplay = msg.Tresc ?? "";

                    // A) Jeśli to RTF (zaczyna się od { ) - czyścimy
                    if (textToDisplay.TrimStart().StartsWith("{\\rtf"))
                    {
                        textToDisplay = StripRtf(textToDisplay);
                    }

                    // B) Jeśli to HTML (zawiera tagi) - czyścimy na tekst
                    if (textToDisplay.Contains("<br>") || textToDisplay.Contains("<div>"))
                    {
                        // Prosta konwersja HTML -> Tekst
                        textToDisplay = textToDisplay.Replace("<br>", "\n").Replace("<br/>", "\n");
                        textToDisplay = Regex.Replace(textToDisplay, "<.*?>", string.Empty);
                    }

                    // Przycięcie długich wiadomości w podglądzie
                    if (textToDisplay.Length > 800)
                        textToDisplay = textToDisplay.Substring(0, 800) + "\n[...] (Kliknij 2x, aby zobaczyć całość)";

                    // 4. Tworzymy dymek
                    ChatBubble bubble = new ChatBubble(textToDisplay, msg.Data.ToString("dd.MM HH:mm"), isIncoming, typeCode);
                    bubble.Width = flowChat.ClientSize.Width - 25;

                    // 5. OBSŁUGA PODGLĄDU MAILI
                    if (typeCode == "MAIL")
                    {
                        // Przekazujemy ORYGINALNĄ (brzydką) treść do podglądu, bo tam jest RTF/HTML
                        bubble.Tag = msg.Tresc;

                        ToolTip tt = new ToolTip();
                        tt.SetToolTip(bubble, "Kliknij dwukrotnie, aby otworzyć e-mail i pobrać załączniki.");

                        bubble.DoubleClick += (s, ev) =>
                        {
                            try
                            {
                                int idWiersza = 0;
                                int.TryParse(msg.Id, out idWiersza);
                                string realUid = _repo.PobierzMessageIdPoId(idWiersza);
                                if (string.IsNullOrEmpty(realUid)) realUid = msg.Id;

                                if (bubble.Tag != null)
                                {
                                    var podglad = new FormPodgladEmail(
                                        bubble.Tag.ToString(), // Oryginalny RTF/HTML
                                        realUid,
                                        $"E-mail z dnia {msg.Data}"
                                    );
                                    podglad.Show();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Błąd otwierania podglądu: " + ex.Message);
                            }
                        };
                    }

                    flowChat.Controls.Add(bubble);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania czatu: " + ex.Message);
            }
            finally
            {
                flowChat.ResumeLayout();
                if (flowChat.Controls.Count > 0)
                {
                    pnlChatContainer.VerticalScroll.Value = pnlChatContainer.VerticalScroll.Maximum;
                }
                pnlChatContainer.PerformLayout();
            }
        }

        // --- METODA DO CZYSZCZENIA RTF ---
        // Używa wbudowanego RichTextBoxa w tle (niewidocznego) do konwersji
        private string StripRtf(string rtfString)
        {
            try
            {
                using (RichTextBox rtb = new RichTextBox())
                {
                    rtb.Rtf = rtfString;
                    return rtb.Text;
                }
            }
            catch
            {
                return rtfString; // W razie błędu zwróć oryginał
            }
        }
    }
}