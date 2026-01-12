namespace Reklamacje_Dane
{
    partial class KomunikatorControl
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.flowLayoutPanelMessages = new System.Windows.Forms.FlowLayoutPanel();
            this.panelInput = new System.Windows.Forms.Panel();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnNowaWiadomosc = new System.Windows.Forms.Button(); // Poprawiona nazwa na btnNowaWiadomosc
            this.panelHeader.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(300, 25);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTitle.Location = new System.Drawing.Point(3, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(83, 15);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Komunikator";
            // 
            // flowLayoutPanelMessages
            // 
            this.flowLayoutPanelMessages.AutoScroll = true;
            this.flowLayoutPanelMessages.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanelMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelMessages.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelMessages.Location = new System.Drawing.Point(0, 25);
            this.flowLayoutPanelMessages.Name = "flowLayoutPanelMessages";
            this.flowLayoutPanelMessages.Size = new System.Drawing.Size(300, 335);
            this.flowLayoutPanelMessages.TabIndex = 1;
            this.flowLayoutPanelMessages.WrapContents = false;
            // 
            // panelInput
            // 
            this.panelInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelInput.Controls.Add(this.txtMessage);
            this.panelInput.Controls.Add(this.btnNowaWiadomosc);
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelInput.Location = new System.Drawing.Point(0, 360);
            this.panelInput.Name = "panelInput";
            this.panelInput.Padding = new System.Windows.Forms.Padding(5);
            this.panelInput.Size = new System.Drawing.Size(300, 40);
            this.panelInput.TabIndex = 2;
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(5, 5);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(210, 30);
            this.txtMessage.TabIndex = 0;
            // 
            // btnNowaWiadomosc
            // 
            this.btnNowaWiadomosc.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnNowaWiadomosc.Location = new System.Drawing.Point(215, 5);
            this.btnNowaWiadomosc.Name = "btnNowaWiadomosc";
            this.btnNowaWiadomosc.Size = new System.Drawing.Size(80, 30);
            this.btnNowaWiadomosc.TabIndex = 1;
            this.btnNowaWiadomosc.Text = "Wyślij";
            this.btnNowaWiadomosc.UseVisualStyleBackColor = true;
            // 
            // KomunikatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanelMessages);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.panelHeader);
            this.Name = "KomunikatorControl";
            this.Size = new System.Drawing.Size(300, 400);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMessages;
        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnNowaWiadomosc;
    }
}