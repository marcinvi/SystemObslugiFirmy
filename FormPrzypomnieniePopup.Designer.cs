namespace Reklamacje_Dane
{
    partial class FormPrzypomnieniePopup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container(); // <--- TO MUSI BYĆ PIERWSZE!
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnCloseX = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTresc = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnSnooze = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.timerAnimacja = new System.Windows.Forms.Timer(this.components);
            this.ctxSnooze = new System.Windows.Forms.ContextMenuStrip(this.components); // Teraz zadziała, bo components już istnieje

            this.panelHeader.SuspendLayout();
            this.SuspendLayout();

            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.SteelBlue;
            this.panelHeader.Controls.Add(this.btnCloseX);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(400, 35);
            this.panelHeader.TabIndex = 0;

            // 
            // btnCloseX
            // 
            this.btnCloseX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseX.AutoSize = true;
            this.btnCloseX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCloseX.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCloseX.ForeColor = System.Drawing.Color.White;
            this.btnCloseX.Location = new System.Drawing.Point(375, 6);
            this.btnCloseX.Name = "btnCloseX";
            this.btnCloseX.Size = new System.Drawing.Size(21, 23);
            this.btnCloseX.TabIndex = 1;
            this.btnCloseX.Text = "X";
            this.btnCloseX.Click += new System.EventHandler(this.btnCloseX_Click);

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(10, 7);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(128, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "PRZYPOMNIENIE";

            // 
            // lblTresc
            // 
            this.lblTresc.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTresc.Location = new System.Drawing.Point(15, 50);
            this.lblTresc.Name = "lblTresc";
            this.lblTresc.Size = new System.Drawing.Size(370, 70);
            this.lblTresc.TabIndex = 1;
            this.lblTresc.Text = "Treść przypomnienia...";

            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblInfo.Location = new System.Drawing.Point(16, 125);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(95, 20);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "Zgłoszenie: --";

            // 
            // btnSnooze
            // 
            this.btnSnooze.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSnooze.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSnooze.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSnooze.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSnooze.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSnooze.Location = new System.Drawing.Point(135, 160);
            this.btnSnooze.Name = "btnSnooze";
            this.btnSnooze.Size = new System.Drawing.Size(120, 35);
            this.btnSnooze.TabIndex = 3;
            this.btnSnooze.Text = "Przełóż ▼";
            this.btnSnooze.UseVisualStyleBackColor = false;
            this.btnSnooze.Click += new System.EventHandler(this.btnSnooze_Click);

            // 
            // btnDone
            // 
            this.btnDone.BackColor = System.Drawing.Color.SeaGreen;
            this.btnDone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDone.FlatAppearance.BorderSize = 0;
            this.btnDone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDone.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDone.ForeColor = System.Drawing.Color.White;
            this.btnDone.Location = new System.Drawing.Point(265, 160);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(120, 35);
            this.btnDone.TabIndex = 4;
            this.btnDone.Text = "WYKONANE";
            this.btnDone.UseVisualStyleBackColor = false;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);

            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpen.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpen.Location = new System.Drawing.Point(15, 160);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(110, 35);
            this.btnOpen.TabIndex = 5;
            this.btnOpen.Text = "Otwórz Zgł.";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);

            // 
            // timerAnimacja
            // 
            this.timerAnimacja.Interval = 20;
            this.timerAnimacja.Tick += new System.EventHandler(this.timerAnimacja_Tick);

            // 
            // ctxSnooze
            // 
            this.ctxSnooze.Name = "ctxSnooze";
            this.ctxSnooze.Size = new System.Drawing.Size(61, 4);

            // 
            // FormPrzypomnieniePopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 210);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnSnooze);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblTresc);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormPrzypomnieniePopup";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PrzypomnieniePopup";
            this.TopMost = true;
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label btnCloseX;
        private System.Windows.Forms.Label lblTresc;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnSnooze;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Timer timerAnimacja;
        private System.Windows.Forms.ContextMenuStrip ctxSnooze;
    }
}