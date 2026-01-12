using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class MessageCard : UserControl
    {
        public MessageCard()
        {
            InitializeComponent();
        }

        public string Sender
        {
            get => lblSender.Text;
            set => lblSender.Text = value;
        }

        public string Message
        {
            get => txtMessage.Text;
            set => txtMessage.Text = value;
        }

        public DateTime Timestamp
        {
            get => DateTime.TryParse(lblTime.Text, out var dt) ? dt : DateTime.MinValue;
            set => lblTime.Text = value.ToString("yyyy-MM-dd HH:mm");
        }

        public bool IsRead
        {
            get => chkRead.Checked;
            set => chkRead.Checked = value;
        }
    }
}