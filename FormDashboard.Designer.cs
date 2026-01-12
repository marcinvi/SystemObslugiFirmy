// ############################################################################
// Plik: FormDashboard.Designer.cs (WERSJA OSTATECZNA)
// Opis: Plik projektanta dla głównego panelu aplikacji.
// ############################################################################

namespace Reklamacje_Dane
{
    partial class FormDashboard
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
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panelUser = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblUserName = new System.Windows.Forms.Label();
            this.panelTopHeader = new System.Windows.Forms.Panel();
            this.btnToggleMenu = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblCurrentModule = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.menuTransitionTimer = new System.Windows.Forms.Timer(this.components);
            this.panelLeft.SuspendLayout();
            this.panelUser.SuspendLayout();
            this.panelTopHeader.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(52)))), ((int)(((byte)(54)))));
            this.panelLeft.Controls.Add(this.panelMenu);
            this.panelLeft.Controls.Add(this.panelUser);
            this.panelLeft.Controls.Add(this.panelTopHeader);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(140, 585);
            this.panelLeft.TabIndex = 0;
            // 
            // panelMenu
            // 
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMenu.Location = new System.Drawing.Point(0, 65);
            this.panelMenu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.panelMenu.Size = new System.Drawing.Size(140, 455);
            this.panelMenu.TabIndex = 1;
            // 
            // panelUser
            // 
            this.panelUser.Controls.Add(this.btnLogout);
            this.panelUser.Controls.Add(this.lblUserName);
            this.panelUser.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelUser.Location = new System.Drawing.Point(0, 520);
            this.panelUser.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelUser.Name = "panelUser";
            this.panelUser.Size = new System.Drawing.Size(140, 65);
            this.panelUser.TabIndex = 0;
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI Emoji", 12F);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(103, 16);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(30, 32);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "🚪";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lblUserName
            // 
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblUserName.ForeColor = System.Drawing.Color.White;
            this.lblUserName.Location = new System.Drawing.Point(9, 7);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(90, 50);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "Imię Nazwisko";
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelTopHeader
            // 
            this.panelTopHeader.Controls.Add(this.btnToggleMenu);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(0, 0);
            this.panelTopHeader.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(140, 65);
            this.panelTopHeader.TabIndex = 2;
            // 
            // btnToggleMenu
            // 
            this.btnToggleMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleMenu.FlatAppearance.BorderSize = 0;
            this.btnToggleMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleMenu.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnToggleMenu.ForeColor = System.Drawing.Color.White;
            this.btnToggleMenu.Location = new System.Drawing.Point(8, 16);
            this.btnToggleMenu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnToggleMenu.Name = "btnToggleMenu";
            this.btnToggleMenu.Size = new System.Drawing.Size(30, 32);
            this.btnToggleMenu.TabIndex = 2;
            this.btnToggleMenu.Text = "☰";
            this.btnToggleMenu.UseVisualStyleBackColor = true;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.lblCurrentModule);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(140, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(820, 65);
            this.panelTop.TabIndex = 1;
            // 
            // lblCurrentModule
            // 
            this.lblCurrentModule.AutoSize = true;
            this.lblCurrentModule.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold);
            this.lblCurrentModule.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblCurrentModule.Location = new System.Drawing.Point(19, 17);
            this.lblCurrentModule.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCurrentModule.Name = "lblCurrentModule";
            this.lblCurrentModule.Size = new System.Drawing.Size(172, 30);
            this.lblCurrentModule.TabIndex = 0;
            this.lblCurrentModule.Text = "Wybierz moduł";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(140, 65);
            this.panelMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.panelMain.Size = new System.Drawing.Size(820, 520);
            this.panelMain.TabIndex = 2;
            // 
            // menuTransitionTimer
            // 
            this.menuTransitionTimer.Interval = 15;
            // 
            // FormDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 585);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelLeft);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(964, 592);
            this.Name = "FormDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "System Obsługi Firmy";
            this.panelLeft.ResumeLayout(false);
            this.panelUser.ResumeLayout(false);
            this.panelTopHeader.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelUser;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Label lblCurrentModule;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelTopHeader;
        private System.Windows.Forms.Button btnToggleMenu;
        private System.Windows.Forms.Timer menuTransitionTimer;
    }
}
