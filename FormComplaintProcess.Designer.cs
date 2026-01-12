namespace Reklamacje_Dane
{
    partial class FormComplaintProcess
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // Main Panels
            this.headerPanel = new System.Windows.Forms.Panel();
            this.stepperPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mainContentPanel = new System.Windows.Forms.Panel();

            // Header Controls
            this.lblComplaintNumber = new System.Windows.Forms.Label();
            this.picSource = new System.Windows.Forms.PictureBox();
            this.clientInfoControl = new Reklamacje_Dane.ClientInfoControl();
            this.productInfoControl = new Reklamacje_Dane.ProductInfoControl();

            // All Panels for Steps
            this.step1_ReceptionPanel = new System.Windows.Forms.Panel();
            this.step2_LogisticsPanel = new System.Windows.Forms.Panel();
            // ... (dalsze panele będą dodawane w miarę rozwoju)

            this.SuspendLayout();

            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Padding = new System.Windows.Forms.Padding(10);
            this.headerPanel.Size = new System.Drawing.Size(1000, 100);
            this.headerPanel.Controls.Add(this.lblComplaintNumber);
            this.headerPanel.Controls.Add(this.picSource);
            this.headerPanel.Controls.Add(this.clientInfoControl);
            this.headerPanel.Controls.Add(this.productInfoControl);

            // lblComplaintNumber
            this.lblComplaintNumber.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblComplaintNumber.Location = new System.Drawing.Point(50, 10);

            // picSource
            this.picSource.Location = new System.Drawing.Point(10, 10);
            this.picSource.Size = new System.Drawing.Size(32, 32);

            // clientInfoControl & productInfoControl
            this.clientInfoControl.Location = new System.Drawing.Point(300, 10);
            this.productInfoControl.Location = new System.Drawing.Point(600, 10);

            // 
            // stepperPanel
            // 
            this.stepperPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(30)))), ((int)(((byte)(54)))));
            this.stepperPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.stepperPanel.Width = 220;
            this.stepperPanel.Padding = new System.Windows.Forms.Padding(10);
            this.stepperPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;

            // 
            // mainContentPanel
            // 
            this.mainContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContentPanel.Padding = new System.Windows.Forms.Padding(20);
            this.mainContentPanel.BackColor = System.Drawing.Color.White;

            // Step 1 Panel Content
            this.step1_ReceptionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            var lblStep1Title = new System.Windows.Forms.Label { Text = "Krok 1: Weryfikacja i Decyzja o Zwrocie", Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold), AutoSize = true };
            var btnRequireReturn = new System.Windows.Forms.Button { Text = "Wymagaj zwrotu produktu", Location = new System.Drawing.Point(10, 40), Size = new System.Drawing.Size(200, 40) };
            var btnNoReturn = new System.Windows.Forms.Button { Text = "Nie wymagaj zwrotu", Location = new System.Drawing.Point(220, 40), Size = new System.Drawing.Size(200, 40) };
            this.step1_ReceptionPanel.Controls.Add(lblStep1Title);
            this.step1_ReceptionPanel.Controls.Add(btnRequireReturn);
            this.step1_ReceptionPanel.Controls.Add(btnNoReturn);

            // Step 2 Panel Content
            this.step2_LogisticsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.step2_LogisticsPanel.Visible = false; // Initially hidden
            var lblStep2Title = new System.Windows.Forms.Label { Text = "Krok 2: Logistyka Odbioru", Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold), AutoSize = true };
            var btnOrderCourier = new System.Windows.Forms.Button { Text = "Zamów kuriera od klienta", Location = new System.Drawing.Point(10, 40), Size = new System.Drawing.Size(200, 40) };
            var chkProductReceived = new System.Windows.Forms.CheckBox { Text = "Produkt otrzymano (status DPD)", Location = new System.Drawing.Point(10, 90), AutoSize = true, Enabled = false };
            var chkConditionOk = new System.Windows.Forms.CheckBox { Text = "Stan zgodny z opisem", Location = new System.Drawing.Point(10, 120), AutoSize = true };
            var chkIsComplete = new System.Windows.Forms.CheckBox { Text = "Produkt kompletny", Location = new System.Drawing.Point(10, 150), AutoSize = true };
           
            // With this corrected version:
            var txtMissingParts = new System.Windows.Forms.TextBox
            {
                Location = new System.Drawing.Point(10, 180),
                Size = new System.Drawing.Size(400, 80),
                Multiline = true
            };
            // Add a Label above the TextBox to indicate its purpose
            var lblMissingParts = new System.Windows.Forms.Label
            {
                Text = "Opisz braki lub uwagi...",
                Location = new System.Drawing.Point(10, 160),
                AutoSize = true
            };
            this.step2_LogisticsPanel.Controls.Add(lblMissingParts);
            this.step2_LogisticsPanel.Controls.Add(txtMissingParts);
            this.step2_LogisticsPanel.Controls.Add(lblStep2Title);
            this.step2_LogisticsPanel.Controls.Add(btnOrderCourier);
            this.step2_LogisticsPanel.Controls.Add(chkProductReceived);
            this.step2_LogisticsPanel.Controls.Add(chkConditionOk);
            this.step2_LogisticsPanel.Controls.Add(chkIsComplete);
            this.step2_LogisticsPanel.Controls.Add(txtMissingParts);

            this.mainContentPanel.Controls.Add(this.step1_ReceptionPanel);
            this.mainContentPanel.Controls.Add(this.step2_LogisticsPanel);

            // 
            // FormComplaintProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Name = "FormComplaintProcess";
            this.Text = "Proces Zgłoszenia Reklamacyjnego";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Controls.Add(this.mainContentPanel);
            this.Controls.Add(this.stepperPanel);
            this.Controls.Add(this.headerPanel);

            this.ResumeLayout(false);
        }

        #endregion

        // Main Panels
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.FlowLayoutPanel stepperPanel;
        private System.Windows.Forms.Panel mainContentPanel;

        // Header Controls
        private System.Windows.Forms.Label lblComplaintNumber;
        private System.Windows.Forms.PictureBox picSource;
        private Reklamacje_Dane.ClientInfoControl clientInfoControl;
        private Reklamacje_Dane.ProductInfoControl productInfoControl;

        // Step Panels
        private System.Windows.Forms.Panel step1_ReceptionPanel;
        private System.Windows.Forms.Panel step2_LogisticsPanel;
        // ... (kolejne zostaną dodane później)
    }
}