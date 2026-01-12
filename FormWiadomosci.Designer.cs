namespace Reklamacje_Dane
{
    partial class FormWiadomosci
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
            this.components = new System.ComponentModel.Container();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.flpThreads = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlChatArea = new System.Windows.Forms.Panel();
            this.flowLayoutPanelChat = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlNewMessage = new System.Windows.Forms.Panel();
            this.btnOpenIssue = new System.Windows.Forms.Button();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.txtNewMessage = new System.Windows.Forms.TextBox();
            this.lblNoConversation = new System.Windows.Forms.Label();
            this.contextMenuThreads = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.markAllAsReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlChatArea.SuspendLayout();
            this.pnlNewMessage.SuspendLayout();
            this.contextMenuThreads.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.flpThreads);
            this.splitContainerMain.Panel1MinSize = 350;
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.pnlRight);
            this.splitContainerMain.Size = new System.Drawing.Size(1262, 673);
            this.splitContainerMain.SplitterDistance = 431;
            this.splitContainerMain.TabIndex = 0;
            // 
            // flpThreads
            // 
            this.flpThreads.AutoScroll = true;
            this.flpThreads.BackColor = System.Drawing.SystemColors.Control;
            this.flpThreads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpThreads.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpThreads.Location = new System.Drawing.Point(0, 0);
            this.flpThreads.Name = "flpThreads";
            this.flpThreads.Size = new System.Drawing.Size(431, 673);
            this.flpThreads.TabIndex = 0;
            this.flpThreads.WrapContents = false;
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.pnlChatArea);
            this.pnlRight.Controls.Add(this.lblNoConversation);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(827, 673);
            this.pnlRight.TabIndex = 0;
            // 
            // pnlChatArea
            // 
            this.pnlChatArea.Controls.Add(this.flowLayoutPanelChat);
            this.pnlChatArea.Controls.Add(this.pnlNewMessage);
            this.pnlChatArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChatArea.Location = new System.Drawing.Point(0, 0);
            this.pnlChatArea.Name = "pnlChatArea";
            this.pnlChatArea.Size = new System.Drawing.Size(827, 673);
            this.pnlChatArea.TabIndex = 1;
            this.pnlChatArea.Visible = false;
            // 
            // flowLayoutPanelChat
            // 
            this.flowLayoutPanelChat.AutoScroll = true;
            this.flowLayoutPanelChat.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanelChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelChat.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelChat.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelChat.Name = "flowLayoutPanelChat";
            this.flowLayoutPanelChat.Padding = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanelChat.Size = new System.Drawing.Size(827, 524);
            this.flowLayoutPanelChat.TabIndex = 1;
            this.flowLayoutPanelChat.WrapContents = false;
            // 
            // pnlNewMessage
            // 
            this.pnlNewMessage.BackColor = System.Drawing.SystemColors.Control;
            this.pnlNewMessage.Controls.Add(this.btnOpenIssue);
            this.pnlNewMessage.Controls.Add(this.btnSendMessage);
            this.pnlNewMessage.Controls.Add(this.txtNewMessage);
            this.pnlNewMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlNewMessage.Location = new System.Drawing.Point(0, 524);
            this.pnlNewMessage.Name = "pnlNewMessage";
            this.pnlNewMessage.Padding = new System.Windows.Forms.Padding(10);
            this.pnlNewMessage.Size = new System.Drawing.Size(827, 149);
            this.pnlNewMessage.TabIndex = 0;
            // 
            // btnOpenIssue
            // 
            this.btnOpenIssue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenIssue.BackColor = System.Drawing.Color.DarkOrange;
            this.btnOpenIssue.FlatAppearance.BorderSize = 0;
            this.btnOpenIssue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenIssue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOpenIssue.ForeColor = System.Drawing.Color.White;
            this.btnOpenIssue.Location = new System.Drawing.Point(696, 77);
            this.btnOpenIssue.Name = "btnOpenIssue";
            this.btnOpenIssue.Size = new System.Drawing.Size(119, 49);
            this.btnOpenIssue.TabIndex = 2;
            this.btnOpenIssue.Text = "Otwórz w oknie";
            this.btnOpenIssue.UseVisualStyleBackColor = false;
            this.btnOpenIssue.Click += new System.EventHandler(this.btnOpenIssue_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnSendMessage.FlatAppearance.BorderSize = 0;
            this.btnSendMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSendMessage.ForeColor = System.Drawing.Color.White;
            this.btnSendMessage.Location = new System.Drawing.Point(696, 22);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(119, 49);
            this.btnSendMessage.TabIndex = 1;
            this.btnSendMessage.Text = "Wyślij";
            this.btnSendMessage.UseVisualStyleBackColor = false;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // txtNewMessage
            // 
            this.txtNewMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewMessage.Location = new System.Drawing.Point(13, 13);
            this.txtNewMessage.Multiline = true;
            this.txtNewMessage.Name = "txtNewMessage";
            this.txtNewMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNewMessage.Size = new System.Drawing.Size(677, 123);
            this.txtNewMessage.TabIndex = 0;
            // 
            // lblNoConversation
            // 
            this.lblNoConversation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoConversation.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblNoConversation.ForeColor = System.Drawing.Color.Gray;
            this.lblNoConversation.Location = new System.Drawing.Point(0, 0);
            this.lblNoConversation.Name = "lblNoConversation";
            this.lblNoConversation.Size = new System.Drawing.Size(827, 673);
            this.lblNoConversation.TabIndex = 0;
            this.lblNoConversation.Text = "Wybierz konwersację z listy po lewej stronie";
            this.lblNoConversation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contextMenuThreads
            // 
            this.contextMenuThreads.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuThreads.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markAllAsReadToolStripMenuItem});
            this.contextMenuThreads.Name = "contextMenuThreads";
            this.contextMenuThreads.Size = new System.Drawing.Size(296, 28);
            // 
            // markAllAsReadToolStripMenuItem
            // 
            this.markAllAsReadToolStripMenuItem.Name = "markAllAsReadToolStripMenuItem";
            this.markAllAsReadToolStripMenuItem.Size = new System.Drawing.Size(295, 24);
            this.markAllAsReadToolStripMenuItem.Text = "Oznacz wszystkie jako odczytane";
            // 
            // FormWiadomosci
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 673);
            this.Controls.Add(this.splitContainerMain);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "FormWiadomosci";
            this.Text = "Centrum Wiadomości Allegro";
            this.Load += new System.EventHandler(this.FormWiadomosci_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlChatArea.ResumeLayout(false);
            this.pnlNewMessage.ResumeLayout(false);
            this.pnlNewMessage.PerformLayout();
            this.contextMenuThreads.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.FlowLayoutPanel flpThreads;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Label lblNoConversation;
        private System.Windows.Forms.Panel pnlChatArea;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelChat;
        private System.Windows.Forms.Panel pnlNewMessage;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.TextBox txtNewMessage;
        private System.Windows.Forms.Button btnOpenIssue;
        private System.Windows.Forms.ContextMenuStrip contextMenuThreads;
        private System.Windows.Forms.ToolStripMenuItem markAllAsReadToolStripMenuItem;
    }
}