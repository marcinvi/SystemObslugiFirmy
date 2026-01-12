using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class MessageListItem : UserControl
    {
        public string DisputeId { get; private set; }

        public MessageListItem()
        {
            InitializeComponent();
        }

        public void SetData(string disputeId, string buyerLogin, string accountName, string complaintNumber, DateTime lastMessageDate, string snippet, bool hasNewMessages)
        {
            DisputeId = disputeId;
            lblBuyerLogin.Text = buyerLogin;
            lblAccountName.Text = accountName;
            lblComplaintNumber.Text = string.IsNullOrEmpty(complaintNumber) ? "" : complaintNumber;
            lblLastMessageDate.Text = lastMessageDate.ToString("g");
            lblLastMessageSnippet.Text = snippet.Replace(Environment.NewLine, " ");

            picUnread.Visible = hasNewMessages;
            lblBuyerLogin.Font = hasNewMessages ? new Font(lblBuyerLogin.Font, FontStyle.Bold) : new Font(lblBuyerLogin.Font, FontStyle.Regular);
        }

        public void SetSelected(bool isSelected)
        {
            pnlMain.BackColor = isSelected ? Color.LightSteelBlue : Color.White;
        }

        private void Control_Click(object sender, EventArgs e)
        {
            // Przekaż zdarzenie Click dalej, aby formularz nadrzędny mógł je obsłużyć
            this.OnClick(e);
        }
    }
}