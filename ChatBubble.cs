using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public class ChatBubble : Control
    {
        public string MessageText { get; set; }
        public string DateText { get; set; }
        public bool IsIncoming { get; set; } // True = Lewo, False = Prawo
        public string MsgType { get; set; }  // "SMS", "MAIL", "ALLEGRO"

        public ChatBubble(string text, string date, bool isIncoming, string msgType)
        {
            this.MessageText = text;
            this.DateText = date;
            this.IsIncoming = isIncoming;
            this.MsgType = msgType; // Zapamiętujemy typ, żeby dobrać kolor

            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.Height = 50;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Font msgFont = new Font("Segoe UI", 10);
            Font dateFont = new Font("Segoe UI", 8);
            Font iconFont = new Font("Segoe UI", 8, FontStyle.Bold); // Czcionka dla literki w kółku

            // 1. Obliczanie rozmiaru tekstu
            int maxTextWidth = (int)(this.Width * 0.70);
            if (maxTextWidth < 100) maxTextWidth = 100;

            SizeF msgSize = e.Graphics.MeasureString(MessageText, msgFont, maxTextWidth);

            int bubbleWidth = (int)msgSize.Width + 40;
            int bubbleHeight = (int)msgSize.Height + 35;

            if (this.Height != bubbleHeight + 10)
            {
                this.Height = bubbleHeight + 10;
                return;
            }

            // 2. Pozycja
            int x = IsIncoming ? 10 : (this.Width - bubbleWidth - 10);
            int y = 5;

            Rectangle bubbleRect = new Rectangle(x, y, bubbleWidth, bubbleHeight);

            // 3. Kolory Tła Dymka
            Color bgColor = IsIncoming ? Color.FromArgb(240, 240, 240) : Color.FromArgb(225, 240, 255); // Lekki niebieski dla nas
            Color textColor = Color.Black;
            Color dateColor = Color.Gray;

            // 4. Rysowanie Dymka
            using (GraphicsPath path = GetRoundedPath(bubbleRect, 10))
            using (SolidBrush bgBrush = new SolidBrush(bgColor))
            {
                e.Graphics.FillPath(bgBrush, path);
            }

            // 5. RYSOWANIE IKONY (Kółko z literą)
            DrawTypeIcon(e.Graphics, x + 10, y + 8, iconFont);

            // 6. Rysowanie Tekstów
            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (SolidBrush dateBrush = new SolidBrush(dateColor))
            {
                // Data (przesunięta, bo po lewej jest teraz kółko z ikoną)
                e.Graphics.DrawString(DateText, dateFont, dateBrush, x + 35, y + 8);

                // Treść
                RectangleF textRect = new RectangleF(x + 12, y + 28, msgSize.Width + 5, msgSize.Height + 5);
                e.Graphics.DrawString(MessageText, msgFont, textBrush, textRect);
            }
        }

        private void DrawTypeIcon(Graphics g, int x, int y, Font font)
        {
            string letter = "?";
            Color circleColor = Color.Gray;

            // Logika kolorów i liter
            string type = MsgType.ToUpper();

            if (type.Contains("SMS"))
            {
                letter = "T";
                circleColor = Color.DodgerBlue; // Niebieski
            }
            else if (type.Contains("MAIL") || type.Contains("EMAIL"))
            {
                letter = "M";
                circleColor = Color.SeaGreen; // Zielony
            }
            else if (type.Contains("ALLEGRO"))
            {
                letter = "A";
                circleColor = Color.DarkOrange; // Pomarańczowy
            }

            // Rysowanie kółka
            using (SolidBrush b = new SolidBrush(circleColor))
            {
                g.FillEllipse(b, x, y, 20, 20); // Kółko 20x20px
            }

            // Rysowanie litery (wyśrodkowanej)
            SizeF letterSize = g.MeasureString(letter, font);
            float lx = x + (20 - letterSize.Width) / 2;
            float ly = y + (20 - letterSize.Height) / 2;

            using (SolidBrush b = new SolidBrush(Color.White))
            {
                g.DrawString(letter, font, b, lx, ly);
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}