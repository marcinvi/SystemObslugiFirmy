namespace Reklamacje_Dane
{
    partial class FormAllegroChat
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
            this.flpChatContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.panelNewMessage = new System.Windows.Forms.Panel();
            this.txtNewMessage = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.panelNewMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpChatContainer
            // 
            this.flpChatContainer.AutoScroll = true;
            this.flpChatContainer.BackColor = System.Drawing.Color.WhiteSmoke;
            this.flpChatContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpChatContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpChatContainer.Location = new System.Drawing.Point(0, 0);
            this.flpChatContainer.Name = "flpChatContainer";
            this.flpChatContainer.Size = new System.Drawing.Size(782, 473);
            this.flpChatContainer.TabIndex = 0;
            this.flpChatContainer.WrapContents = false;
            // 
            // panelNewMessage
            // 
            this.panelNewMessage.BackColor = System.Drawing.SystemColors.Control;
            this.panelNewMessage.Controls.Add(this.txtNewMessage);
            this.panelNewMessage.Controls.Add(this.btnSendMessage);
            this.panelNewMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelNewMessage.Location = new System.Drawing.Point(0, 473);
            this.panelNewMessage.Name = "panelNewMessage";
            this.panelNewMessage.Padding = new System.Windows.Forms.Padding(5);
            this.panelNewMessage.Size = new System.Drawing.Size(782, 80);
            this.panelNewMessage.TabIndex = 1;
            // 
            // txtNewMessage
            // 
            this.txtNewMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNewMessage.Location = new System.Drawing.Point(5, 5);
            this.txtNewMessage.Multiline = true;
            this.txtNewMessage.Name = "txtNewMessage";
            this.txtNewMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNewMessage.Size = new System.Drawing.Size(697, 70);
            this.txtNewMessage.TabIndex = 1;
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSendMessage.Location = new System.Drawing.Point(702, 5);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(75, 70);
            this.btnSendMessage.TabIndex = 0;
            this.btnSendMessage.Text = "Wyślij";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // FormAllegroChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.flpChatContainer);
            this.Controls.Add(this.panelNewMessage);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "FormAllegroChat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Czat Dyskusji Allegro";
            this.Load += new System.EventHandler(this.FormAllegroChat_Load);
            this.panelNewMessage.ResumeLayout(false);
            this.panelNewMessage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpChatContainer;
        private System.Windows.Forms.Panel panelNewMessage;
        private System.Windows.Forms.TextBox txtNewMessage;
        private System.Windows.Forms.Button btnSendMessage;
    }
}