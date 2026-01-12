namespace Reklamacje_Dane
{
    partial class MessageCard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblSender;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.CheckBox chkRead;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblSender = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.chkRead = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblSender
            // 
            this.lblSender.AutoSize = true;
            this.lblSender.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSender.Location = new System.Drawing.Point(8, 8);
            this.lblSender.Name = "lblSender";
            this.lblSender.Size = new System.Drawing.Size(76, 15);
            this.lblSender.TabIndex = 0;
            this.lblSender.Text = "Nadawca";
            // 
            // lblTime
            // 
            this.lblTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(240, 8);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(65, 15);
            this.lblTime.TabIndex = 1;
            this.lblTime.Text = "2025-01-01";
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Location = new System.Drawing.Point(11, 30);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(294, 44);
            this.txtMessage.TabIndex = 2;
            // 
            // chkRead
            // 
            this.chkRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkRead.AutoSize = true;
            this.chkRead.Enabled = false;
            this.chkRead.Location = new System.Drawing.Point(254, 82);
            this.chkRead.Name = "chkRead";
            this.chkRead.Size = new System.Drawing.Size(51, 19);
            this.chkRead.TabIndex = 3;
            this.chkRead.Text = "Read";
            this.chkRead.UseVisualStyleBackColor = true;
            // 
            // MessageCard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.chkRead);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblSender);
            this.Name = "MessageCard";
            this.Size = new System.Drawing.Size(320, 110);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}