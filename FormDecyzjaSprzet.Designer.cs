namespace Reklamacje_Dane
{
    partial class FormDecyzjaSprzet
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnCzesci = new System.Windows.Forms.Button();
            this.btnStan = new System.Windows.Forms.Button();
            this.btnUtylizacja = new System.Windows.Forms.Button();
            this.btnIgnoruj = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblInfo.Location = new System.Drawing.Point(0, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Padding = new System.Windows.Forms.Padding(10);
            this.lblInfo.Size = new System.Drawing.Size(382, 70);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Zamykasz reklamację jako Wymiana lub Zwrot.\r\nCo fizycznie dzieje się ze zwracanym" +
    " sprzętem?";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCzesci
            // 
            this.btnCzesci.BackColor = System.Drawing.Color.Purple;
            this.btnCzesci.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCzesci.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCzesci.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCzesci.ForeColor = System.Drawing.Color.White;
            this.btnCzesci.Location = new System.Drawing.Point(40, 80);
            this.btnCzesci.Name = "btnCzesci";
            this.btnCzesci.Size = new System.Drawing.Size(300, 45);
            this.btnCzesci.TabIndex = 1;
            this.btnCzesci.Text = "NA CZĘŚCI (Dawca)";
            this.btnCzesci.UseVisualStyleBackColor = false;
            this.btnCzesci.Click += new System.EventHandler(this.btnCzesci_Click);
            // 
            // btnStan
            // 
            this.btnStan.BackColor = System.Drawing.Color.ForestGreen;
            this.btnStan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnStan.ForeColor = System.Drawing.Color.White;
            this.btnStan.Location = new System.Drawing.Point(40, 135);
            this.btnStan.Name = "btnStan";
            this.btnStan.Size = new System.Drawing.Size(300, 45);
            this.btnStan.TabIndex = 2;
            this.btnStan.Text = "POWRÓT NA STAN (Pełnowartościowy)";
            this.btnStan.UseVisualStyleBackColor = false;
            this.btnStan.Click += new System.EventHandler(this.btnStan_Click);
            // 
            // btnUtylizacja
            // 
            this.btnUtylizacja.BackColor = System.Drawing.Color.Gray;
            this.btnUtylizacja.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUtylizacja.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUtylizacja.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUtylizacja.ForeColor = System.Drawing.Color.White;
            this.btnUtylizacja.Location = new System.Drawing.Point(40, 190);
            this.btnUtylizacja.Name = "btnUtylizacja";
            this.btnUtylizacja.Size = new System.Drawing.Size(300, 45);
            this.btnUtylizacja.TabIndex = 3;
            this.btnUtylizacja.Text = "UTYLIZACJA (Złom)";
            this.btnUtylizacja.UseVisualStyleBackColor = false;
            this.btnUtylizacja.Click += new System.EventHandler(this.btnUtylizacja_Click);
            // 
            // btnIgnoruj
            // 
            this.btnIgnoruj.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnIgnoruj.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIgnoruj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIgnoruj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnIgnoruj.ForeColor = System.Drawing.Color.Black;
            this.btnIgnoruj.Location = new System.Drawing.Point(40, 245);
            this.btnIgnoruj.Name = "btnIgnoruj";
            this.btnIgnoruj.Size = new System.Drawing.Size(300, 45);
            this.btnIgnoruj.TabIndex = 4;
            this.btnIgnoruj.Text = "BRAK AKCJI (Np. sprzęt został u klienta)";
            this.btnIgnoruj.UseVisualStyleBackColor = false;
            this.btnIgnoruj.Click += new System.EventHandler(this.btnIgnoruj_Click);
            // 
            // FormDecyzjaSprzet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(382, 313);
            this.Controls.Add(this.btnIgnoruj);
            this.Controls.Add(this.btnUtylizacja);
            this.Controls.Add(this.btnStan);
            this.Controls.Add(this.btnCzesci);
            this.Controls.Add(this.lblInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDecyzjaSprzet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Decyzja Magazynowa";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnCzesci;
        private System.Windows.Forms.Button btnStan;
        private System.Windows.Forms.Button btnUtylizacja;
        private System.Windows.Forms.Button btnIgnoruj;
    }
}