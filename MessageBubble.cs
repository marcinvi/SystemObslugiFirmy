using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Pojedyncza wiadomość w stylu Messenger/WhatsApp
    /// Piękne bąbelki z zaokrąglonymi rogami!
    /// </summary>
    public class MessageBubble : Panel
    {
        private Label _lblAuthor;
        private Label _lblMessage;
        private Label _lblTime;
        private Panel _bubblePanel;
        private FlowLayoutPanel _attachmentsPanel;
        
        public bool IsSeller { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public DateTime MessageTime { get; set; }
        public string[] Attachments { get; set; }

        public MessageBubble()
        {
            this.Padding = new Padding(10, 5, 10, 5);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            
            CreateControls();
        }

        private void CreateControls()
        {
            // Kontener na bąbelek
            _bubblePanel = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 8, 12, 8),
                MaximumSize = new Size(500, 0)
            };

            // Autor (mały napis na górze)
            _lblAuthor = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Margin = new Padding(0, 0, 0, 3)
            };

            // Treść wiadomości
            _lblMessage = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(480, 0),
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(0)
            };

            // Czas (mały napis na dole)
            _lblTime = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F),
                Margin = new Padding(0, 3, 0, 0)
            };

            // Panel na załączniki
            _attachmentsPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 0),
                Visible = false
            };

            // Dodaj kontrolki do bąbelka
            _bubblePanel.Controls.Add(_lblAuthor);
            _bubblePanel.Controls.Add(_lblMessage);
            _bubblePanel.Controls.Add(_attachmentsPanel);
            _bubblePanel.Controls.Add(_lblTime);

            this.Controls.Add(_bubblePanel);
        }

        public void SetData(string author, string message, DateTime time, bool isSeller, string[] attachments = null)
        {
            Author = author;
            Message = message;
            MessageTime = time;
            IsSeller = isSeller;
            Attachments = attachments;

            UpdateUI();
        }

        private void UpdateUI()
        {
            _lblAuthor.Text = Author;
            _lblMessage.Text = Message;
            _lblTime.Text = MessageTime.ToString("HH:mm");

            // Kolory i pozycjonowanie
            if (IsSeller)
            {
                // Sprzedawca - po prawej, niebieski
                _bubblePanel.BackColor = Color.FromArgb(0, 132, 255);
                _lblAuthor.ForeColor = Color.FromArgb(220, 230, 255);
                _lblMessage.ForeColor = Color.White;
                _lblTime.ForeColor = Color.FromArgb(200, 220, 255);
                this.Dock = DockStyle.Top;
                _bubblePanel.Dock = DockStyle.Right;
            }
            else
            {
                // Kupujący - po lewej, szary
                _bubblePanel.BackColor = Color.FromArgb(240, 240, 240);
                _lblAuthor.ForeColor = Color.FromArgb(100, 100, 100);
                _lblMessage.ForeColor = Color.Black;
                _lblTime.ForeColor = Color.FromArgb(120, 120, 120);
                this.Dock = DockStyle.Top;
                _bubblePanel.Dock = DockStyle.Left;
            }

            // Załączniki
            if (Attachments != null && Attachments.Length > 0)
            {
                _attachmentsPanel.Visible = true;
                _attachmentsPanel.Controls.Clear();

                foreach (var att in Attachments)
                {
                    var attLink = new LinkLabel
                    {
                        Text = "📎 " + att,
                        AutoSize = true,
                        Font = new Font("Segoe UI", 8.5F),
                        LinkColor = IsSeller ? Color.FromArgb(200, 220, 255) : Color.DodgerBlue,
                        Margin = new Padding(0, 2, 0, 2)
                    };
                    _attachmentsPanel.Controls.Add(attLink);
                }
            }

            // Zaokrąglone rogi
            _bubblePanel.Paint += BubblePanel_Paint;
        }

        private void BubblePanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            // Rysuj zaokrąglony prostokąt
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            Rectangle rect = panel.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            int radius = 12; // Promień zaokrąglenia
            using (GraphicsPath path = GetRoundedRect(rect, radius))
            {
                panel.Region = new Region(path);
            }
        }

        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Lewy górny róg
            path.AddArc(arc, 180, 90);

            // Prawy górny róg
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Prawy dolny róg
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Lewy dolny róg
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
