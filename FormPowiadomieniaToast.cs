using System;
using System.Drawing;
using System.Windows.Forms;

public partial class FormPowiadomieniaToast : Form
{
    

    public FormPowiadomieniaToast(string title, string message, Reklamacje_Dane.NotificationType type)
    {
        InitializeComponent();

        lblTitle.Text = title;
        lblMessage.Text = message;

        // Ustaw kolor i ikonę w zależności od typu powiadomienia
        switch (type)
        {
            case Reklamacje_Dane.NotificationType.Info:
                this.BackColor = Color.FromArgb(33, 150, 243); // Niebieski
                // pictureBoxIcon.Image = Properties.Resources.info_icon;
                break;
            case Reklamacje_Dane.NotificationType.Success:
                this.BackColor = Color.FromArgb(76, 175, 80); // Zielony
                // pictureBoxIcon.Image = Properties.Resources.success_icon;
                break;
            case Reklamacje_Dane.NotificationType.Warning:
                this.BackColor = Color.FromArgb(255, 193, 7); // Żółty/Złoty
                // pictureBoxIcon.Image = Properties.Resources.warning_icon;
                break;
            case Reklamacje_Dane.NotificationType.Error:
                this.BackColor = Color.FromArgb(244, 67, 54); // Czerwony
                // pictureBoxIcon.Image = Properties.Resources.error_icon;
                break;
        }
    

          
        }

    private void FormPowiadomieniaToast_Load(object sender, EventArgs e)
    {
        // Ustaw pozycję w prawym dolnym rogu ekranu
        Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
        this.Location = new Point(workingArea.Right - this.Size.Width - 10,
                                  workingArea.Bottom - this.Size.Height - 10);

        // Uruchom timer do automatycznego zamknięcia
        timerClose.Start();
    }

    // Zamyka formularz po upływie czasu
    private void timerClose_Tick(object sender, EventArgs e)
    {
        this.Close();
    }

    // Zamyka formularz po kliknięciu przycisku X
    private void btnClose_Click(object sender, EventArgs e)
    {
        this.Close();
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
       

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
      
}
}