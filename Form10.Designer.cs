namespace Reklamacje_Dane
{
    partial class Form10
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
            this.labelOpisUsterki = new System.Windows.Forms.Label();
            this.textBoxOpisUsterki = new System.Windows.Forms.TextBox();
            this.labelOkolicznoscWady = new System.Windows.Forms.Label();
            this.textBoxOkolicznoscWady = new System.Windows.Forms.TextBox();
            this.buttonWyslij = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelOpisUsterki
            // 
            this.labelOpisUsterki.AutoSize = true;
            this.labelOpisUsterki.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelOpisUsterki.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelOpisUsterki.Location = new System.Drawing.Point(12, 15);
            this.labelOpisUsterki.Name = "labelOpisUsterki";
            this.labelOpisUsterki.Size = new System.Drawing.Size(116, 20);
            this.labelOpisUsterki.TabIndex = 6;
            this.labelOpisUsterki.Text = "Treść zgłoszenia";
            // 
            // textBoxOpisUsterki
            // 
            this.textBoxOpisUsterki.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOpisUsterki.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxOpisUsterki.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOpisUsterki.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOpisUsterki.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxOpisUsterki.Location = new System.Drawing.Point(16, 38);
            this.textBoxOpisUsterki.Multiline = true;
            this.textBoxOpisUsterki.Name = "textBoxOpisUsterki";
            this.textBoxOpisUsterki.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxOpisUsterki.Size = new System.Drawing.Size(447, 187);
            this.textBoxOpisUsterki.TabIndex = 7;
            // 
            // labelOkolicznoscWady
            // 
            this.labelOkolicznoscWady.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelOkolicznoscWady.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelOkolicznoscWady.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelOkolicznoscWady.Location = new System.Drawing.Point(12, 239);
            this.labelOkolicznoscWady.Name = "labelOkolicznoscWady";
            this.labelOkolicznoscWady.Size = new System.Drawing.Size(125, 20);
            this.labelOkolicznoscWady.TabIndex = 8;
            this.labelOkolicznoscWady.Text = "Adres e-mail:";
            // 
            // textBoxOkolicznoscWady
            // 
            this.textBoxOkolicznoscWady.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOkolicznoscWady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxOkolicznoscWady.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOkolicznoscWady.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOkolicznoscWady.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxOkolicznoscWady.Location = new System.Drawing.Point(143, 236);
            this.textBoxOkolicznoscWady.Name = "textBoxOkolicznoscWady";
            this.textBoxOkolicznoscWady.Size = new System.Drawing.Size(320, 27);
            this.textBoxOkolicznoscWady.TabIndex = 9;
            // 
            // buttonWyslij
            // 
            this.buttonWyslij.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWyslij.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.buttonWyslij.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWyslij.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonWyslij.ForeColor = System.Drawing.Color.White;
            this.buttonWyslij.Location = new System.Drawing.Point(353, 272);
            this.buttonWyslij.Name = "buttonWyslij";
            this.buttonWyslij.Size = new System.Drawing.Size(110, 33);
            this.buttonWyslij.TabIndex = 12;
            this.buttonWyslij.Text = "Wyślij";
            this.buttonWyslij.UseVisualStyleBackColor = false;
            this.buttonWyslij.Click += new System.EventHandler(this.buttonWyslij_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.BackColor = System.Drawing.Color.DarkOrange;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(16, 272);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 33);
            this.button1.TabIndex = 27;
            this.button1.Text = "Przetłumacz";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form10
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(475, 317);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonWyslij);
            this.Controls.Add(this.textBoxOkolicznoscWady);
            this.Controls.Add(this.labelOkolicznoscWady);
            this.Controls.Add(this.textBoxOpisUsterki);
            this.Controls.Add(this.labelOpisUsterki);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(493, 364);
            this.Name = "Form10";
            this.Text = "Formularz ogólny";
            this.Load += new System.EventHandler(this.Form10_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelOpisUsterki;
        private System.Windows.Forms.TextBox textBoxOpisUsterki;
        private System.Windows.Forms.Label labelOkolicznoscWady;
        private System.Windows.Forms.TextBox textBoxOkolicznoscWady;
        private System.Windows.Forms.Button buttonWyslij;
        private System.Windows.Forms.Button button1;
    }
}