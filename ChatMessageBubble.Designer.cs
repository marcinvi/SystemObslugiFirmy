namespace Reklamacje_Dane
{
    partial class ChatMessageBubble
    {
        /// <summary> 
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod wygenerowany przez Projektanta składników

        /// <summary> 
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować 
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlBubble = new System.Windows.Forms.Panel();
            this.picAttachment = new System.Windows.Forms.PictureBox();
            this.lnkAttachment = new System.Windows.Forms.LinkLabel();
            this.lblText = new System.Windows.Forms.Label();
            this.lblReadStatus = new System.Windows.Forms.Label();
            this.lblTimestamp = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.pnlBubble.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAttachment)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBubble
            // 
            this.pnlBubble.AutoSize = true;
            this.pnlBubble.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlBubble.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlBubble.Controls.Add(this.picAttachment);
            this.pnlBubble.Controls.Add(this.lnkAttachment);
            this.pnlBubble.Controls.Add(this.lblText);
            this.pnlBubble.Controls.Add(this.lblReadStatus);
            this.pnlBubble.Controls.Add(this.lblTimestamp);
            this.pnlBubble.Controls.Add(this.lblAuthor);
            this.pnlBubble.Location = new System.Drawing.Point(10, 10);
            this.pnlBubble.Margin = new System.Windows.Forms.Padding(3, 3, 50, 3);
            this.pnlBubble.MaximumSize = new System.Drawing.Size(600, 0);
            this.pnlBubble.MinimumSize = new System.Drawing.Size(200, 0);
            this.pnlBubble.Name = "pnlBubble";
            this.pnlBubble.Padding = new System.Windows.Forms.Padding(10);
            this.pnlBubble.Size = new System.Drawing.Size(200, 122);
            this.pnlBubble.TabIndex = 0;
            // 
            // picAttachment
            // 
            this.picAttachment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAttachment.Location = new System.Drawing.Point(13, 99);
            this.picAttachment.MaximumSize = new System.Drawing.Size(580, 400);
            this.picAttachment.Name = "picAttachment";
            this.picAttachment.Size = new System.Drawing.Size(174, 10);
            this.picAttachment.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAttachment.TabIndex = 5;
            this.picAttachment.TabStop = false;
            this.picAttachment.Visible = false;
            // 
            // lnkAttachment
            // 
            this.lnkAttachment.AutoSize = true;
            this.lnkAttachment.Location = new System.Drawing.Point(13, 71);
            this.lnkAttachment.Name = "lnkAttachment";
            this.lnkAttachment.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lnkAttachment.Size = new System.Drawing.Size(77, 26);
            this.lnkAttachment.TabIndex = 4;
            this.lnkAttachment.TabStop = true;
            this.lnkAttachment.Text = "link do pliku";
            this.lnkAttachment.Visible = false;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(13, 35);
            this.lblText.MaximumSize = new System.Drawing.Size(580, 0);
            this.lblText.Name = "lblText";
            this.lblText.Padding = new System.Windows.Forms.Padding(0, 5, 0, 15);
            this.lblText.Size = new System.Drawing.Size(117, 36);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Treść wiadomości";
            // 
            // lblReadStatus
            // 
            this.lblReadStatus.AutoSize = true;
            this.lblReadStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblReadStatus.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblReadStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblReadStatus.Location = new System.Drawing.Point(10, 90);
            this.lblReadStatus.Name = "lblReadStatus";
            this.lblReadStatus.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblReadStatus.Size = new System.Drawing.Size(124, 22);
            this.lblReadStatus.TabIndex = 3;
            this.lblReadStatus.Text = "Przeczytane przez: ...";
            this.lblReadStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTimestamp
            // 
            this.lblTimestamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimestamp.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTimestamp.ForeColor = System.Drawing.Color.Gray;
            this.lblTimestamp.Location = new System.Drawing.Point(82, 10);
            this.lblTimestamp.Name = "lblTimestamp";
            this.lblTimestamp.Size = new System.Drawing.Size(108, 17);
            this.lblTimestamp.TabIndex = 2;
            this.lblTimestamp.Text = "01.01. 12:00";
            this.lblTimestamp.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAuthor.Location = new System.Drawing.Point(13, 10);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(50, 20);
            this.lblAuthor.TabIndex = 1;
            this.lblAuthor.Text = "Autor";
            // 
            // ChatMessageBubble
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.pnlBubble);
            this.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.Name = "ChatMessageBubble";
            this.Size = new System.Drawing.Size(260, 135);
            this.pnlBubble.ResumeLayout(false);
            this.pnlBubble.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAttachment)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBubble;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblTimestamp;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Label lblReadStatus;
        private System.Windows.Forms.PictureBox picAttachment;
        private System.Windows.Forms.LinkLabel lnkAttachment;
    }
}