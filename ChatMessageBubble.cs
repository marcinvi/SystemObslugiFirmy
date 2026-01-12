using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text.pdf.security;
using System.Xml;

namespace Reklamacje_Dane
{
    // NAJWAŻNIEJSZA POPRAWKA: Upewniamy się, że klasa jest częściowa (partial) i dziedziczy po UserControl
    public partial class ChatMessageBubble : UserControl
    {
        public ChatMessageBubble()
        {
            // Ta metoda jest zdefiniowana w pliku .Designer.cs i teraz będzie widoczna
            InitializeComponent();

            // Dodajemy obsługę kliknięć w załączniki
            picAttachment.Click += PicAttachment_Click;
            lnkAttachment.LinkClicked += LnkAttachment_LinkClicked;
        }

        // Metoda do ustawiania zwykłej wiadomości tekstowej
       

        // Nowe metody do ustawiania załączników
        public void SetAttachment(Image image, string localFilePath)
        {
            picAttachment.Image = image;
            picAttachment.Tag = localFilePath; // Zapisujemy ścieżkę do pliku, aby ją otworzyć po kliknięciu
            picAttachment.Visible = true;
            lnkAttachment.Visible = false;
        }

        public void SetAttachment(string fileName, string localFilePath)
        {
            lnkAttachment.Text = fileName;
            lnkAttachment.Tag = localFilePath; // Zapisujemy ścieżkę do pliku
            lnkAttachment.Visible = true;
            picAttachment.Visible = false;
        }

        // Metody obsługujące kliknięcia w załączniki
        private void PicAttachment_Click(object sender, EventArgs e)
        {
            try
            {
                string path = picAttachment.Tag as string;
                if (!string.IsNullOrEmpty(path))
                {
                    // Używamy Process.Start do otwarcia pliku domyślną aplikacją systemową
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie można otworzyć pliku: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LnkAttachment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string path = lnkAttachment.Tag as string;
                if (!string.IsNullOrEmpty(path))
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie można otworzyć pliku: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        public void SetMaxWidth(int width)
        {
            // Set max size for the inner panel and the text label
            // This forces the label to wrap text when it exceeds 'width'
            this.MaximumSize = new Size(width, 0);
            pnlBubble.MaximumSize = new Size(width, 0);
            lblText.MaximumSize = new Size(width - 30, 0); // -30 for internal padding
        }

        public void SetMessage(string author, DateTime timestamp, string text, bool isSeller)
        {
            lblAuthor.Text = author;
            lblTimestamp.Text = timestamp.ToString("g");

            if (string.IsNullOrWhiteSpace(text))
            {
                lblText.Visible = false;
                lblText.Text = string.Empty;
            }
            else
            {
                lblText.Visible = true;
                lblText.Text = text;
            }

            picAttachment.Visible = false;
            lnkAttachment.Visible = false;

            // Apply styles
            if (isSeller)
            {
                pnlBubble.BackColor = Color.FromArgb(220, 248, 198); // Green-ish
                // Important: Reset margins inside the bubble to default
                pnlBubble.Margin = new Padding(3);
                // We handle left/right alignment in the parent Form, not here

                lblAuthor.TextAlign = ContentAlignment.TopRight;
            }
            else
            {
                pnlBubble.BackColor = Color.WhiteSmoke; // Gray-ish
                pnlBubble.Margin = new Padding(3);

                lblAuthor.TextAlign = ContentAlignment.TopLeft;
            }
        }




        public void SetReadStatus(List<string> readerLogins)
        {
            if (readerLogins == null || !readerLogins.Any())
            {
                lblReadStatus.Visible = false;
                return;
            }

            lblReadStatus.Visible = true;
            lblReadStatus.Text = "Przeczytane przez: " + string.Join(", ", readerLogins);
        }
    }
}