namespace Reklamacje_Dane
{
    partial class MessageListItem
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblComplaintNumber = new System.Windows.Forms.Label();
            this.lblAccountName = new System.Windows.Forms.Label();
            this.lblLastMessageDate = new System.Windows.Forms.Label();
            this.lblLastMessageSnippet = new System.Windows.Forms.Label();
            this.lblBuyerLogin = new System.Windows.Forms.Label();
            this.picUnread = new System.Windows.Forms.PictureBox();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUnread)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.White;
            this.pnlMain.Controls.Add(this.picUnread);
            this.pnlMain.Controls.Add(this.lblComplaintNumber);
            this.pnlMain.Controls.Add(this.lblAccountName);
            this.pnlMain.Controls.Add(this.lblLastMessageDate);
            this.pnlMain.Controls.Add(this.lblLastMessageSnippet);
            this.pnlMain.Controls.Add(this.lblBuyerLogin);
            this.pnlMain.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(5, 5);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(390, 110);
            this.pnlMain.TabIndex = 0;
            this.pnlMain.Click += new System.EventHandler(this.Control_Click);
            // 
            // lblComplaintNumber
            // 
            this.lblComplaintNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblComplaintNumber.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblComplaintNumber.ForeColor = System.Drawing.Color.Gray;
            this.lblComplaintNumber.Location = new System.Drawing.Point(217, 33);
            this.lblComplaintNumber.Name = "lblComplaintNumber";
            this.lblComplaintNumber.Size = new System.Drawing.Size(170, 20);
            this.lblComplaintNumber.TabIndex = 4;
            this.lblComplaintNumber.Text = "REK/2024/07/123";
            this.lblComplaintNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblComplaintNumber.Click += new System.EventHandler(this.Control_Click);
            // 
            // lblAccountName
            // 
            this.lblAccountName.AutoSize = true;
            this.lblAccountName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAccountName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(115)))), ((int)(((byte)(183)))));
            this.lblAccountName.Location = new System.Drawing.Point(3, 33);
            this.lblAccountName.Name = "lblAccountName";
            this.lblAccountName.Size = new System.Drawing.Size(103, 20);
            this.lblAccountName.TabIndex = 3;
            this.lblAccountName.Text = "Nazwa Konta";
            this.lblAccountName.Click += new System.EventHandler(this.Control_Click);
            // 
            // lblLastMessageDate
            // 
            this.lblLastMessageDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastMessageDate.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblLastMessageDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblLastMessageDate.Location = new System.Drawing.Point(217, 8);
            this.lblLastMessageDate.Name = "lblLastMessageDate";
            this.lblLastMessageDate.Size = new System.Drawing.Size(170, 20);
            this.lblLastMessageDate.TabIndex = 2;
            this.lblLastMessageDate.Text = "08.07.2024 15:30";
            this.lblLastMessageDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblLastMessageDate.Click += new System.EventHandler(this.Control_Click);
            // 
            // lblLastMessageSnippet
            // 
            this.lblLastMessageSnippet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastMessageSnippet.AutoEllipsis = true;
            this.lblLastMessageSnippet.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblLastMessageSnippet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblLastMessageSnippet.Location = new System.Drawing.Point(3, 62);
            this.lblLastMessageSnippet.Name = "lblLastMessageSnippet";
            this.lblLastMessageSnippet.Size = new System.Drawing.Size(384, 42);
            this.lblLastMessageSnippet.TabIndex = 1;
            this.lblLastMessageSnippet.Text = "To jest fragment ostatniej wiadomości, który może być długi i powinien się zawin" +
    "ąć...";
            this.lblLastMessageSnippet.Click += new System.EventHandler(this.Control_Click);
            // 
            // lblBuyerLogin
            // 
            this.lblBuyerLogin.AutoSize = true;
            this.lblBuyerLogin.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblBuyerLogin.Location = new System.Drawing.Point(3, 5);
            this.lblBuyerLogin.Name = "lblBuyerLogin";
            this.lblBuyerLogin.Size = new System.Drawing.Size(155, 23);
            this.lblBuyerLogin.TabIndex = 0;
            this.lblBuyerLogin.Text = "LoginKupujacego";
            this.lblBuyerLogin.Click += new System.EventHandler(this.Control_Click);
            // 
            // picUnread
            // 
            this.picUnread.BackColor = System.Drawing.Color.DodgerBlue;
            this.picUnread.Location = new System.Drawing.Point(375, 5);
            this.picUnread.Name = "picUnread";
            this.picUnread.Size = new System.Drawing.Size(12, 12);
            this.picUnread.TabIndex = 5;
            this.picUnread.TabStop = false;
            this.picUnread.Visible = false;
            // 
            // MessageListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Controls.Add(this.pnlMain);
            this.Name = "MessageListItem";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(400, 120);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUnread)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblLastMessageSnippet;
        private System.Windows.Forms.Label lblBuyerLogin;
        private System.Windows.Forms.Label lblLastMessageDate;
        private System.Windows.Forms.Label lblComplaintNumber;
        private System.Windows.Forms.Label lblAccountName;
        private System.Windows.Forms.PictureBox picUnread;
    }
}