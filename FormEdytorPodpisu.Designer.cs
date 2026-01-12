namespace Reklamacje_Dane
{
    partial class FormEdytorPodpisu
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.txtHtml = new System.Windows.Forms.TextBox();
            this.lblKod = new System.Windows.Forms.Label();
            this.browserPreview = new System.Windows.Forms.WebBrowser();
            this.lblPodglad = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.btnWstawObrazek = new System.Windows.Forms.Button();
            this.btnWstawSzablon = new System.Windows.Forms.Button();
            this.btnWstawEmail = new System.Windows.Forms.Button();
            this.btnWstawPracownika = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Vertical;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.txtHtml);
            this.splitContainer.Panel1.Controls.Add(this.lblKod);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.browserPreview);
            this.splitContainer.Panel2.Controls.Add(this.lblPodglad);
            this.splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContainer.Size = new System.Drawing.Size(1100, 670);
            this.splitContainer.SplitterDistance = 550;
            this.splitContainer.SplitterWidth = 5;
            this.splitContainer.TabIndex = 0;
            // 
            // txtHtml
            // 
            this.txtHtml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHtml.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtHtml.Location = new System.Drawing.Point(10, 35);
            this.txtHtml.Multiline = true;
            this.txtHtml.Name = "txtHtml";
            this.txtHtml.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtHtml.Size = new System.Drawing.Size(530, 625);
            this.txtHtml.TabIndex = 1;
            this.txtHtml.TextChanged += new System.EventHandler(this.TxtHtml_TextChanged);
            // 
            // lblKod
            // 
            this.lblKod.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKod.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblKod.Location = new System.Drawing.Point(10, 10);
            this.lblKod.Name = "lblKod";
            this.lblKod.Size = new System.Drawing.Size(530, 25);
            this.lblKod.TabIndex = 0;
            this.lblKod.Text = "Kod HTML (Używaj przycisków, aby wstawić zmienne):";
            // 
            // browserPreview
            // 
            this.browserPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserPreview.Location = new System.Drawing.Point(10, 35);
            this.browserPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.browserPreview.Name = "browserPreview";
            this.browserPreview.Size = new System.Drawing.Size(525, 625);
            this.browserPreview.TabIndex = 1;
            // 
            // lblPodglad
            // 
            this.lblPodglad.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPodglad.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPodglad.Location = new System.Drawing.Point(10, 10);
            this.lblPodglad.Name = "lblPodglad";
            this.lblPodglad.Size = new System.Drawing.Size(525, 25);
            this.lblPodglad.TabIndex = 0;
            this.lblPodglad.Text = "Podgląd na żywo (Twoje dane):";
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBottom.Controls.Add(this.btnZapisz);
            this.panelBottom.Controls.Add(this.btnAnuluj);
            this.panelBottom.Controls.Add(this.btnWstawObrazek);
            this.panelBottom.Controls.Add(this.btnWstawSzablon);
            this.panelBottom.Controls.Add(this.btnWstawEmail);
            this.panelBottom.Controls.Add(this.btnWstawPracownika);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 670);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1100, 80);
            this.panelBottom.TabIndex = 1;
            // 
            // btnZapisz
            // 
            this.btnZapisz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZapisz.BackColor = System.Drawing.Color.SteelBlue;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(940, 20);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(130, 40);
            this.btnZapisz.TabIndex = 5;
            this.btnZapisz.Text = "💾 ZAPISZ";
            this.btnZapisz.UseVisualStyleBackColor = false;
            this.btnZapisz.Click += new System.EventHandler(this.BtnZapisz_Click);
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnuluj.Location = new System.Drawing.Point(830, 20);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(100, 40);
            this.btnAnuluj.TabIndex = 4;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            this.btnAnuluj.Click += new System.EventHandler(this.BtnAnuluj_Click);
            // 
            // btnWstawObrazek
            // 
            this.btnWstawObrazek.Location = new System.Drawing.Point(370, 10);
            this.btnWstawObrazek.Name = "btnWstawObrazek";
            this.btnWstawObrazek.Size = new System.Drawing.Size(110, 30);
            this.btnWstawObrazek.TabIndex = 3;
            this.btnWstawObrazek.Text = "Wstaw Obrazek";
            this.btnWstawObrazek.UseVisualStyleBackColor = true;
            this.btnWstawObrazek.Click += new System.EventHandler(this.BtnWstawObrazek_Click);
            // 
            // btnWstawSzablon
            // 
            this.btnWstawSzablon.Location = new System.Drawing.Point(250, 10);
            this.btnWstawSzablon.Name = "btnWstawSzablon";
            this.btnWstawSzablon.Size = new System.Drawing.Size(110, 30);
            this.btnWstawSzablon.TabIndex = 2;
            this.btnWstawSzablon.Text = "Szablon Firmy";
            this.btnWstawSzablon.UseVisualStyleBackColor = true;
            this.btnWstawSzablon.Click += new System.EventHandler(this.BtnWstawSzablon_Click);
            // 
            // btnWstawEmail
            // 
            this.btnWstawEmail.BackColor = System.Drawing.Color.LightYellow;
            this.btnWstawEmail.Location = new System.Drawing.Point(130, 10);
            this.btnWstawEmail.Name = "btnWstawEmail";
            this.btnWstawEmail.Size = new System.Drawing.Size(110, 30);
            this.btnWstawEmail.TabIndex = 1;
            this.btnWstawEmail.Text = "Wstaw: Email";
            this.btnWstawEmail.UseVisualStyleBackColor = false;
            this.btnWstawEmail.Click += new System.EventHandler(this.BtnWstawEmail_Click);
            // 
            // btnWstawPracownika
            // 
            this.btnWstawPracownika.BackColor = System.Drawing.Color.LightYellow;
            this.btnWstawPracownika.Location = new System.Drawing.Point(12, 10);
            this.btnWstawPracownika.Name = "btnWstawPracownika";
            this.btnWstawPracownika.Size = new System.Drawing.Size(110, 30);
            this.btnWstawPracownika.TabIndex = 0;
            this.btnWstawPracownika.Text = "Wstaw: Imię";
            this.btnWstawPracownika.UseVisualStyleBackColor = false;
            this.btnWstawPracownika.Click += new System.EventHandler(this.BtnWstawPracownika_Click);
            // 
            // FormEdytorPodpisu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 750);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.panelBottom);
            this.Name = "FormEdytorPodpisu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edytor Podpisu HTML";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox txtHtml;
        private System.Windows.Forms.Label lblKod;
        private System.Windows.Forms.WebBrowser browserPreview;
        private System.Windows.Forms.Label lblPodglad;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Button btnWstawObrazek;
        private System.Windows.Forms.Button btnWstawSzablon;
        private System.Windows.Forms.Button btnWstawEmail;
        private System.Windows.Forms.Button btnWstawPracownika;
    }
}