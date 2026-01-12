namespace Reklamacje_Dane
{
    partial class MessageBubbleControl
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
            this.lblSender = new System.Windows.Forms.Label();
            this.lblTimestamp = new System.Windows.Forms.Label();
            this.lblContent = new System.Windows.Forms.Label();
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.btnReply = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSender
            // 
            this.lblSender.AutoSize = true;
            this.lblSender.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSender.Location = new System.Drawing.Point(35, 8);
            this.lblSender.Name = "lblSender";
            this.lblSender.Size = new System.Drawing.Size(71, 20);
            this.lblSender.TabIndex = 0;
            this.lblSender.Text = "Nadawca";
            // 
            // lblTimestamp
            // 
            this.lblTimestamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimestamp.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTimestamp.ForeColor = System.Drawing.Color.Gray;
            this.lblTimestamp.Location = new System.Drawing.Point(350, 9);
            this.lblTimestamp.Name = "lblTimestamp";
            this.lblTimestamp.Size = new System.Drawing.Size(140, 20);
            this.lblTimestamp.TabIndex = 1;
            this.lblTimestamp.Text = "dd.MM.yyyy HH:mm";
            this.lblTimestamp.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblContent
            // 
            this.lblContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblContent.Location = new System.Drawing.Point(11, 35);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(479, 23);
            this.lblContent.TabIndex = 2;
            this.lblContent.Text = "Treść wiadomości...";
            // 
            // picStatus
            // 
            this.picStatus.Location = new System.Drawing.Point(10, 9);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(18, 18);
            this.picStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picStatus.TabIndex = 3;
            this.picStatus.TabStop = false;
            // 
            // btnReply
            // 
            this.btnReply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReply.Location = new System.Drawing.Point(415, 62);
            this.btnReply.Name = "btnReply";
            this.btnReply.Size = new System.Drawing.Size(75, 23);
            this.btnReply.TabIndex = 4;
            this.btnReply.Text = "Odpowiedz";
            this.btnReply.UseVisualStyleBackColor = true;
            // 
            // MessageBubbleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnReply);
            this.Controls.Add(this.picStatus);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.lblTimestamp);
            this.Controls.Add(this.lblSender);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 20, 5);
            this.MinimumSize = new System.Drawing.Size(400, 90);
            this.Name = "MessageBubbleControl";
            this.Size = new System.Drawing.Size(500, 90);
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Label lblSender;
        private System.Windows.Forms.Label lblTimestamp;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.Button btnReply;
    }
}