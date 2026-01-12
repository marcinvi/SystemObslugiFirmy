// Plik: FormKontoUstawienia.Designer.cs (Wersja zmodernizowana)
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class FormKontoUstawienia
    {
        private System.ComponentModel.IContainer components = null;
        private TabControl tabs;
        private TabPage tabProfil;
        private TabPage tabDelegacje;
        private TabPage tabCzasPracy;

        private Label lblLogin; private TextBox txtLogin;
        private Label lblHaslo; private TextBox txtHaslo;
        private Label lblNazwa; private TextBox txtNazwa;
        private Label lblEmail; private TextBox txtEmail;
        private Label lblRola; private TextBox txtRola;
        private Button btnSave;

        private Label lblWIP;
        private GroupBox groupBoxProfil;
        private GroupBox groupBoxKontakt;
        private GroupBox groupBoxUprawnienia;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabProfil = new System.Windows.Forms.TabPage();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBoxUprawnienia = new System.Windows.Forms.GroupBox();
            this.lblRola = new System.Windows.Forms.Label();
            this.txtRola = new System.Windows.Forms.TextBox();
            this.groupBoxKontakt = new System.Windows.Forms.GroupBox();
            this.lblNazwa = new System.Windows.Forms.Label();
            this.txtNazwa = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.groupBoxProfil = new System.Windows.Forms.GroupBox();
            this.lblLogin = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.lblHaslo = new System.Windows.Forms.Label();
            this.txtHaslo = new System.Windows.Forms.TextBox();
            this.tabDelegacje = new System.Windows.Forms.TabPage();
            this.tabCzasPracy = new System.Windows.Forms.TabPage();
            this.lblWIP = new System.Windows.Forms.Label();
            this.tabs.SuspendLayout();
            this.tabProfil.SuspendLayout();
            this.groupBoxUprawnienia.SuspendLayout();
            this.groupBoxKontakt.SuspendLayout();
            this.groupBoxProfil.SuspendLayout();
            this.tabCzasPracy.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabProfil);
            this.tabs.Controls.Add(this.tabDelegacje);
            this.tabs.Controls.Add(this.tabCzasPracy);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(884, 561);
            this.tabs.TabIndex = 0;
            // 
            // tabProfil
            // 
            this.tabProfil.Controls.Add(this.btnSave);
            this.tabProfil.Controls.Add(this.groupBoxUprawnienia);
            this.tabProfil.Controls.Add(this.groupBoxKontakt);
            this.tabProfil.Controls.Add(this.groupBoxProfil);
            this.tabProfil.Location = new System.Drawing.Point(4, 24);
            this.tabProfil.Name = "tabProfil";
            this.tabProfil.Padding = new System.Windows.Forms.Padding(10);
            this.tabProfil.Size = new System.Drawing.Size(876, 533);
            this.tabProfil.TabIndex = 0;
            this.tabProfil.Text = "Mój Profil";
            this.tabProfil.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(723, 480);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(140, 40);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Zapisz Zmiany";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // groupBoxUprawnienia
            // 
            this.groupBoxUprawnienia.Controls.Add(this.lblRola);
            this.groupBoxUprawnienia.Controls.Add(this.txtRola);
            this.groupBoxUprawnienia.Location = new System.Drawing.Point(13, 303);
            this.groupBoxUprawnienia.Name = "groupBoxUprawnienia";
            this.groupBoxUprawnienia.Size = new System.Drawing.Size(450, 80);
            this.groupBoxUprawnienia.TabIndex = 2;
            this.groupBoxUprawnienia.TabStop = false;
            this.groupBoxUprawnienia.Text = "Uprawnienia";
            // 
            // lblRola
            // 
            this.lblRola.AutoSize = true;
            this.lblRola.Location = new System.Drawing.Point(17, 36);
            this.lblRola.Name = "lblRola";
            this.lblRola.Size = new System.Drawing.Size(33, 15);
            this.lblRola.TabIndex = 0;
            this.lblRola.Text = "Rola:";
            // 
            // txtRola
            // 
            this.txtRola.BackColor = System.Drawing.SystemColors.Control;
            this.txtRola.Location = new System.Drawing.Point(140, 33);
            this.txtRola.Name = "txtRola";
            this.txtRola.ReadOnly = true;
            this.txtRola.Size = new System.Drawing.Size(290, 23);
            this.txtRola.TabIndex = 0;
            // 
            // groupBoxKontakt
            // 
            this.groupBoxKontakt.Controls.Add(this.lblNazwa);
            this.groupBoxKontakt.Controls.Add(this.txtNazwa);
            this.groupBoxKontakt.Controls.Add(this.lblEmail);
            this.groupBoxKontakt.Controls.Add(this.txtEmail);
            this.groupBoxKontakt.Location = new System.Drawing.Point(13, 160);
            this.groupBoxKontakt.Name = "groupBoxKontakt";
            this.groupBoxKontakt.Size = new System.Drawing.Size(450, 120);
            this.groupBoxKontakt.TabIndex = 1;
            this.groupBoxKontakt.TabStop = false;
            this.groupBoxKontakt.Text = "Dane Kontaktowe";
            // 
            // lblNazwa
            // 
            this.lblNazwa.AutoSize = true;
            this.lblNazwa.Location = new System.Drawing.Point(17, 36);
            this.lblNazwa.Name = "lblNazwa";
            this.lblNazwa.Size = new System.Drawing.Size(111, 15);
            this.lblNazwa.TabIndex = 0;
            this.lblNazwa.Text = "Nazwa wyświetlana:";
            // 
            // txtNazwa
            // 
            this.txtNazwa.Location = new System.Drawing.Point(140, 33);
            this.txtNazwa.Name = "txtNazwa";
            this.txtNazwa.Size = new System.Drawing.Size(290, 23);
            this.txtNazwa.TabIndex = 2;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(17, 75);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(39, 15);
            this.lblEmail.TabIndex = 0;
            this.lblEmail.Text = "Email:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(140, 72);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(290, 23);
            this.txtEmail.TabIndex = 3;
            // 
            // groupBoxProfil
            // 
            this.groupBoxProfil.Controls.Add(this.lblLogin);
            this.groupBoxProfil.Controls.Add(this.txtLogin);
            this.groupBoxProfil.Controls.Add(this.lblHaslo);
            this.groupBoxProfil.Controls.Add(this.txtHaslo);
            this.groupBoxProfil.Location = new System.Drawing.Point(13, 17);
            this.groupBoxProfil.Name = "groupBoxProfil";
            this.groupBoxProfil.Size = new System.Drawing.Size(450, 120);
            this.groupBoxProfil.TabIndex = 0;
            this.groupBoxProfil.TabStop = false;
            this.groupBoxProfil.Text = "Dane Logowania";
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(17, 36);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(40, 15);
            this.lblLogin.TabIndex = 0;
            this.lblLogin.Text = "Login:";
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(140, 33);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(290, 23);
            this.txtLogin.TabIndex = 0;
            // 
            // lblHaslo
            // 
            this.lblHaslo.AutoSize = true;
            this.lblHaslo.Location = new System.Drawing.Point(17, 75);
            this.lblHaslo.Name = "lblHaslo";
            this.lblHaslo.Size = new System.Drawing.Size(73, 15);
            this.lblHaslo.TabIndex = 0;
            this.lblHaslo.Text = "Nowe hasło:";
            // 
            // txtHaslo
            // 
            this.txtHaslo.Location = new System.Drawing.Point(140, 72);
            this.txtHaslo.Name = "txtHaslo";
            this.txtHaslo.Size = new System.Drawing.Size(290, 23);
            this.txtHaslo.TabIndex = 1;
            this.txtHaslo.UseSystemPasswordChar = true;
            // 
            // tabDelegacje
            // 
            this.tabDelegacje.Location = new System.Drawing.Point(4, 24);
            this.tabDelegacje.Name = "tabDelegacje";
            this.tabDelegacje.Padding = new System.Windows.Forms.Padding(3);
            this.tabDelegacje.Size = new System.Drawing.Size(876, 533);
            this.tabDelegacje.TabIndex = 1;
            this.tabDelegacje.Text = "Moje Delegacje";
            this.tabDelegacje.UseVisualStyleBackColor = true;
            // 
            // tabCzasPracy
            // 
            this.tabCzasPracy.Controls.Add(this.lblWIP);
            this.tabCzasPracy.Location = new System.Drawing.Point(4, 24);
            this.tabCzasPracy.Name = "tabCzasPracy";
            this.tabCzasPracy.Padding = new System.Windows.Forms.Padding(20);
            this.tabCzasPracy.Size = new System.Drawing.Size(876, 533);
            this.tabCzasPracy.TabIndex = 2;
            this.tabCzasPracy.Text = "Czas pracy";
            this.tabCzasPracy.UseVisualStyleBackColor = true;
            // 
            // lblWIP
            // 
            this.lblWIP.AutoSize = true;
            this.lblWIP.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblWIP.ForeColor = System.Drawing.Color.Gray;
            this.lblWIP.Location = new System.Drawing.Point(23, 20);
            this.lblWIP.Name = "lblWIP";
            this.lblWIP.Size = new System.Drawing.Size(127, 17);
            this.lblWIP.TabIndex = 0;
            this.lblWIP.Text = "Moduł w budowie…";
            // 
            // FormKontoUstawienia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.tabs);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormKontoUstawienia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ustawienia Konta";
            this.tabs.ResumeLayout(false);
            this.tabProfil.ResumeLayout(false);
            this.groupBoxUprawnienia.ResumeLayout(false);
            this.groupBoxUprawnienia.PerformLayout();
            this.groupBoxKontakt.ResumeLayout(false);
            this.groupBoxKontakt.PerformLayout();
            this.groupBoxProfil.ResumeLayout(false);
            this.groupBoxProfil.PerformLayout();
            this.tabCzasPracy.ResumeLayout(false);
            this.tabCzasPracy.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}