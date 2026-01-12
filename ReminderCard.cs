using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class StandardReminderCard : UserControl
    {
        // Publiczne zdarzenia, na które będzie nasłuchiwać formularz nadrzędny
        public event EventHandler MarkAsDoneClicked;
        public event EventHandler GoToComplaintClicked;

        // Właściwości do łatwego zarządzania kartą
        public string ReminderText { get => lblReminderText.Text; set => lblReminderText.Text = value; }
        public int ReminderId { get; set; }

        // Nowa właściwość do zmiany koloru paska bocznego
        public Color IndicatorColor { get => indicatorPanel.BackColor; set => indicatorPanel.BackColor = value; }

        public StandardReminderCard()
        {
            InitializeComponent();
            // Domyślnie ukryj numer zgłoszenia
            this.lblComplaintNumber.Visible = false;
        }

        // Poprawiona, pojedyncza właściwość ComplaintNumber
        public string ComplaintNumber
        {
            get => lblComplaintNumber.Text.Replace("Zgłoszenie: ", "");
            set
            {
                if (!string.IsNullOrEmpty(value) && value != "Brak")
                {
                    lblComplaintNumber.Text = $"Zgłoszenie: {value}";
                    lblComplaintNumber.Visible = true;
                }
                else
                {
                    lblComplaintNumber.Text = "Brak";
                    lblComplaintNumber.Visible = false;
                }
            }
        }

        // Kliknięcie "Otwórz" tylko wywołuje zdarzenie
        private void btnGoToComplaint_Click(object sender, EventArgs e)
        {
            GoToComplaintClicked?.Invoke(this, EventArgs.Empty);
        }

        // Kliknięcie "Wykonane" również tylko wywołuje zdarzenie
        private void btnMarkAsDone_Click(object sender, EventArgs e)
        {
            MarkAsDoneClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}