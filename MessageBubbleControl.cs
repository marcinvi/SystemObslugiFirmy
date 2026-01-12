using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class MessageBubbleControl : UserControl
    {
        // === NOWE, BRAKUJĄCE WŁAŚCIWOŚCI ===
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string MessageTitle { get; set; }
        public int? ReturnId { get; set; }

        // === NOWE, BRAKUJĄCE ZDARZENIE ===
        public event EventHandler ReplyClicked;

        public MessageBubbleControl()
        {
            InitializeComponent();
            this.lblContent.MaximumSize = new Size(this.Width - 20, 0);
            this.lblContent.AutoSize = true;

            // Dodajemy obsługę kliknięcia do przycisku odpowiedzi
            this.btnReply.Click += (s, e) => ReplyClicked?.Invoke(this, EventArgs.Empty);
        }

        // === POPRAWIONA METODA ===
        public void SetMessageData(string sender, string content, DateTime timestamp, bool isRead, bool hasBeenRepliedTo)
        {
            lblSender.Text = sender;
            lblContent.Text = content;
            lblTimestamp.Text = timestamp.ToString("dd.MM.yyyy HH:mm");

            // Ustawianie statusu wizualnego
            if (hasBeenRepliedTo)
            {
               // picStatus.Image = Properties.Resources.replied_icon; // Użyj obrazu strzałki odpowiedzi
                picStatus.Visible = true;
            }
            else if (isRead)
            {
            //    picStatus.Image = Properties.Resources.read_icon; // Użyj obrazu "ptaszka"
                picStatus.Visible = true;
            }
            else
            {
            //    picStatus.Image = Properties.Resources.unread_icon; // Użyj obrazu kropki
                picStatus.Visible = true;
            }

            if (!isRead)
            {
                this.BackColor = Color.LightGoldenrodYellow;
                lblSender.Font = new Font(lblSender.Font, FontStyle.Bold);
                lblContent.Font = new Font(lblContent.Font, FontStyle.Bold);
            }
        }
    }
}