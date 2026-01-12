using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// POPRAWIONA wiadomość - wszystko widoczne!
    /// </summary>
    public class ChatMessageControl : Panel
    {
        private Label lblAuthor;
        private Label txtMessage;  // ✅ POPRAWIONE: Label zamiast TextBox!
        private Label lblTime;
        
        public bool IsSeller { get; set; }

        public ChatMessageControl()
        {
            this.AutoSize = false;
            this.Padding = new Padding(15, 10, 15, 10);
            this.Margin = new Padding(5, 3, 5, 3);
            
            CreateControls();
        }

        private void CreateControls()
        {
            // Panel kontenerowy dla wszystkich elementów
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                AutoSize = true,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            container.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Autor
            container.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Wiadomość
            container.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Data

            // Autor
            lblAuthor = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Padding = new Padding(0, 0, 0, 5),
                Dock = DockStyle.Top
            };

            // Treść wiadomości - LABEL (nie TextBox!)
            txtMessage = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(500, 0),
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(0, 0, 0, 5),
                Dock = DockStyle.Top
            };

            // Data
            lblTime = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                Padding = new Padding(0, 5, 0, 0),
                Dock = DockStyle.Top
            };

            container.Controls.Add(lblAuthor, 0, 0);
            container.Controls.Add(txtMessage, 0, 1);
            container.Controls.Add(lblTime, 0, 2);

            this.Controls.Add(container);
        }

        public void SetData(string author, string message, DateTime time, bool isSeller)
        {
            IsSeller = isSeller;
            
            lblAuthor.Text = author;
            txtMessage.Text = message;
            lblTime.Text = time.ToString("dd.MM.yyyy HH:mm");

            // Kolory
            if (isSeller)
            {
                // Ty - jasnoniebieski
                this.BackColor = Color.FromArgb(225, 240, 255);
                lblAuthor.ForeColor = Color.FromArgb(0, 80, 150);
                txtMessage.ForeColor = Color.FromArgb(20, 20, 50);
                txtMessage.BackColor = Color.FromArgb(225, 240, 255);
                lblTime.ForeColor = Color.FromArgb(100, 130, 160);
            }
            else
            {
                // Kupujący - jasnoszary
                this.BackColor = Color.FromArgb(245, 245, 245);
                lblAuthor.ForeColor = Color.FromArgb(80, 80, 80);
                txtMessage.ForeColor = Color.Black;
                txtMessage.BackColor = Color.FromArgb(245, 245, 245);
                lblTime.ForeColor = Color.FromArgb(120, 120, 120);
            }

            // Ustaw wysokość - policz ile miejsca potrzeba
            using (Graphics g = this.CreateGraphics())
            {
                // Wysokość autora
                SizeF authorSize = g.MeasureString(author, lblAuthor.Font);
                
                // Wysokość wiadomości
                SizeF msgSize = g.MeasureString(message, txtMessage.Font, 500);
                
                // Wysokość daty
                SizeF timeSize = g.MeasureString(lblTime.Text, lblTime.Font);
                
                // Suma + padding
                int totalHeight = (int)(authorSize.Height + msgSize.Height + timeSize.Height) + 40;
                this.Height = Math.Max(80, totalHeight);
            }
        }
    }
}
