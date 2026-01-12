// Plik: AdminControl.Designer.cs (Wersja kompletna, z nowymi polami)
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class AdminControl
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
            this.tabControlAdmin = new System.Windows.Forms.TabControl();
            this.tabPageUsers = new System.Windows.Forms.TabPage();
            this.splitContainerUsers = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listViewUsers = new System.Windows.Forms.ListView();
            this.panelUserDetails = new System.Windows.Forms.Panel();
            this.groupBoxUprawnienia = new System.Windows.Forms.GroupBox();
            this.checkedListBoxModules = new System.Windows.Forms.CheckedListBox();
            this.groupBoxDanePodstawowe = new System.Windows.Forms.GroupBox();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.lblLogin = new System.Windows.Forms.Label();
            this.txtNazwaWyswietlana = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboRola = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelAkcje = new System.Windows.Forms.Panel();
            this.btnResetPassword = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.groupBoxDpd = new System.Windows.Forms.GroupBox();
            this.btnSaveDpdSettings = new System.Windows.Forms.Button();
            this.txtDpdPassword = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDpdClientId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDpdLogin = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBoxAllegro = new System.Windows.Forms.GroupBox();
            this.btnAutoryzujKonto = new System.Windows.Forms.Button();
            this.btnUsunKonto = new System.Windows.Forms.Button();
            this.btnDodajKonto = new System.Windows.Forms.Button();
            this.listViewKonta = new System.Windows.Forms.ListView();
            this.groupBoxDeepL = new System.Windows.Forms.GroupBox();
            this.btnSaveDeepLSettings = new System.Windows.Forms.Button();
            this.txtDeepLApiKey = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBoxEmail = new System.Windows.Forms.GroupBox();
            this.btnSaveEmailSettings = new System.Windows.Forms.Button();
            this.txtDefaultEmail = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPageOpiekunowie = new System.Windows.Forms.TabPage();
            this.allegroOpiekunowieControl1 = new Reklamacje_Dane.AllegroOpiekunowieControl();
            this.tabPageDelegacje = new System.Windows.Forms.TabPage();
            this.delegacjeControl1 = new Reklamacje_Dane.DelegacjeControl();
            this.tabControlAdmin.SuspendLayout();
            this.tabPageUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerUsers)).BeginInit();
            this.splitContainerUsers.Panel1.SuspendLayout();
            this.splitContainerUsers.Panel2.SuspendLayout();
            this.splitContainerUsers.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelUserDetails.SuspendLayout();
            this.groupBoxUprawnienia.SuspendLayout();
            this.groupBoxDanePodstawowe.SuspendLayout();
            this.panelAkcje.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.groupBoxDpd.SuspendLayout();
            this.groupBoxAllegro.SuspendLayout();
            this.groupBoxDeepL.SuspendLayout();
            this.groupBoxEmail.SuspendLayout();
            this.tabPageOpiekunowie.SuspendLayout();
            this.tabPageDelegacje.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlAdmin
            // 
            this.tabControlAdmin.Controls.Add(this.tabPageUsers);
            this.tabControlAdmin.Controls.Add(this.tabPageSettings);
            this.tabControlAdmin.Controls.Add(this.tabPageOpiekunowie);
            this.tabControlAdmin.Controls.Add(this.tabPageDelegacje);
            this.tabControlAdmin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlAdmin.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabControlAdmin.Location = new System.Drawing.Point(0, 0);
            this.tabControlAdmin.Name = "tabControlAdmin";
            this.tabControlAdmin.SelectedIndex = 0;
            this.tabControlAdmin.Size = new System.Drawing.Size(900, 750);
            this.tabControlAdmin.TabIndex = 0;
            // 
            // tabPageUsers
            // 
            this.tabPageUsers.Controls.Add(this.splitContainerUsers);
            this.tabPageUsers.Location = new System.Drawing.Point(4, 29);
            this.tabPageUsers.Name = "tabPageUsers";
            this.tabPageUsers.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageUsers.Size = new System.Drawing.Size(892, 717);
            this.tabPageUsers.TabIndex = 0;
            this.tabPageUsers.Text = "👤 Zarządzanie Użytkownikami";
            this.tabPageUsers.UseVisualStyleBackColor = true;
            // 
            // splitContainerUsers
            // 
            this.splitContainerUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerUsers.Location = new System.Drawing.Point(10, 10);
            this.splitContainerUsers.Name = "splitContainerUsers";
            this.splitContainerUsers.Panel1.Controls.Add(this.groupBox1);
            this.splitContainerUsers.Panel2.Controls.Add(this.panelUserDetails);
            this.splitContainerUsers.Size = new System.Drawing.Size(872, 697);
            this.splitContainerUsers.SplitterDistance = 450;
            this.splitContainerUsers.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listViewUsers);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(450, 697);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lista Użytkowników";
            // 
            // listViewUsers
            // 
            this.listViewUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewUsers.FullRowSelect = true;
            this.listViewUsers.HideSelection = false;
            this.listViewUsers.Location = new System.Drawing.Point(10, 28);
            this.listViewUsers.MultiSelect = false;
            this.listViewUsers.Name = "listViewUsers";
            this.listViewUsers.Size = new System.Drawing.Size(430, 659);
            this.listViewUsers.TabIndex = 0;
            this.listViewUsers.UseCompatibleStateImageBehavior = false;
            this.listViewUsers.View = System.Windows.Forms.View.Details;
            this.listViewUsers.SelectedIndexChanged += new System.EventHandler(this.listViewUsers_SelectedIndexChanged);
            // 
            // panelUserDetails
            // 
            this.panelUserDetails.Controls.Add(this.groupBoxUprawnienia);
            this.panelUserDetails.Controls.Add(this.groupBoxDanePodstawowe);
            this.panelUserDetails.Controls.Add(this.panelAkcje);
            this.panelUserDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUserDetails.Enabled = false;
            this.panelUserDetails.Location = new System.Drawing.Point(0, 0);
            this.panelUserDetails.Name = "panelUserDetails";
            this.panelUserDetails.Size = new System.Drawing.Size(418, 697);
            this.panelUserDetails.TabIndex = 0;
            // 
            // groupBoxUprawnienia
            // 
            this.groupBoxUprawnienia.Controls.Add(this.checkedListBoxModules);
            this.groupBoxUprawnienia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxUprawnienia.Location = new System.Drawing.Point(0, 180);
            this.groupBoxUprawnienia.Name = "groupBoxUprawnienia";
            this.groupBoxUprawnienia.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.groupBoxUprawnienia.Size = new System.Drawing.Size(418, 457);
            this.groupBoxUprawnienia.TabIndex = 11;
            this.groupBoxUprawnienia.TabStop = false;
            this.groupBoxUprawnienia.Text = "Dostęp do modułów";
            // 
            // checkedListBoxModules
            // 
            this.checkedListBoxModules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxModules.FormattingEnabled = true;
            this.checkedListBoxModules.Location = new System.Drawing.Point(10, 25);
            this.checkedListBoxModules.Name = "checkedListBoxModules";
            this.checkedListBoxModules.Size = new System.Drawing.Size(398, 422);
            this.checkedListBoxModules.TabIndex = 8;
            // 
            // groupBoxDanePodstawowe
            // 
            this.groupBoxDanePodstawowe.Controls.Add(this.txtLogin);
            this.groupBoxDanePodstawowe.Controls.Add(this.lblLogin);
            this.groupBoxDanePodstawowe.Controls.Add(this.txtNazwaWyswietlana);
            this.groupBoxDanePodstawowe.Controls.Add(this.label1);
            this.groupBoxDanePodstawowe.Controls.Add(this.comboRola);
            this.groupBoxDanePodstawowe.Controls.Add(this.label2);
            this.groupBoxDanePodstawowe.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxDanePodstawowe.Location = new System.Drawing.Point(0, 0);
            this.groupBoxDanePodstawowe.Name = "groupBoxDanePodstawowe";
            this.groupBoxDanePodstawowe.Size = new System.Drawing.Size(418, 180);
            this.groupBoxDanePodstawowe.TabIndex = 10;
            this.groupBoxDanePodstawowe.TabStop = false;
            this.groupBoxDanePodstawowe.Text = "Dane Podstawowe";
            // 
            // txtLogin
            // 
            this.txtLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogin.Location = new System.Drawing.Point(15, 50);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(387, 27);
            this.txtLogin.TabIndex = 1;
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(15, 25);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(49, 20);
            this.lblLogin.TabIndex = 0;
            this.lblLogin.Text = "Login:";
            // 
            // txtNazwaWyswietlana
            // 
            this.txtNazwaWyswietlana.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNazwaWyswietlana.Location = new System.Drawing.Point(15, 100);
            this.txtNazwaWyswietlana.Name = "txtNazwaWyswietlana";
            this.txtNazwaWyswietlana.Size = new System.Drawing.Size(387, 27);
            this.txtNazwaWyswietlana.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nazwa wyświetlana:";
            // 
            // comboRola
            // 
            this.comboRola.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.comboRola.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRola.FormattingEnabled = true;
            this.comboRola.Items.AddRange(new object[] { "Admin", "Handlowiec", "Magazyn", "Reklamacje" });
            this.comboRola.Location = new System.Drawing.Point(15, 145);
            this.comboRola.Name = "comboRola";
            this.comboRola.Size = new System.Drawing.Size(387, 28);
            this.comboRola.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Rola:";
            // 
            // panelAkcje
            // 
            this.panelAkcje.Controls.Add(this.btnResetPassword);
            this.panelAkcje.Controls.Add(this.btnSaveChanges);
            this.panelAkcje.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelAkcje.Location = new System.Drawing.Point(0, 637);
            this.panelAkcje.Name = "panelAkcje";
            this.panelAkcje.Size = new System.Drawing.Size(418, 60);
            this.panelAkcje.TabIndex = 12;
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnResetPassword.Location = new System.Drawing.Point(15, 10);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(120, 40);
            this.btnResetPassword.TabIndex = 10;
            this.btnResetPassword.Text = "Resetuj Hasło";
            this.btnResetPassword.UseVisualStyleBackColor = true;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveChanges.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSaveChanges.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveChanges.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveChanges.ForeColor = System.Drawing.Color.White;
            this.btnSaveChanges.Location = new System.Drawing.Point(248, 10);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(155, 40);
            this.btnSaveChanges.TabIndex = 9;
            this.btnSaveChanges.Text = "Zapisz Zmiany";
            this.btnSaveChanges.UseVisualStyleBackColor = false;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.AutoScroll = true;
            this.tabPageSettings.Controls.Add(this.groupBoxEmail);
            this.tabPageSettings.Controls.Add(this.groupBoxDeepL);
            this.tabPageSettings.Controls.Add(this.groupBoxDpd);
            this.tabPageSettings.Controls.Add(this.groupBoxAllegro);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 29);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(10);
            this.tabPageSettings.Size = new System.Drawing.Size(892, 717);
            this.tabPageSettings.TabIndex = 1;
            this.tabPageSettings.Text = "⚙️ Ustawienia Aplikacji";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxDpd
            // 
            this.groupBoxDpd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDpd.Controls.Add(this.btnSaveDpdSettings);
            this.groupBoxDpd.Controls.Add(this.txtDpdPassword);
            this.groupBoxDpd.Controls.Add(this.label6);
            this.groupBoxDpd.Controls.Add(this.txtDpdClientId);
            this.groupBoxDpd.Controls.Add(this.label5);
            this.groupBoxDpd.Controls.Add(this.txtDpdLogin);
            this.groupBoxDpd.Controls.Add(this.label4);
            this.groupBoxDpd.Location = new System.Drawing.Point(13, 330);
            this.groupBoxDpd.Name = "groupBoxDpd";
            this.groupBoxDpd.Size = new System.Drawing.Size(866, 200);
            this.groupBoxDpd.TabIndex = 1;
            this.groupBoxDpd.TabStop = false;
            this.groupBoxDpd.Text = "Ustawienia DPD";
            // 
            // btnSaveDpdSettings
            // 
            this.btnSaveDpdSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveDpdSettings.Location = new System.Drawing.Point(680, 145);
            this.btnSaveDpdSettings.Name = "btnSaveDpdSettings";
            this.btnSaveDpdSettings.Size = new System.Drawing.Size(180, 40);
            this.btnSaveDpdSettings.TabIndex = 6;
            this.btnSaveDpdSettings.Text = "Zapisz Ustawienia DPD";
            this.btnSaveDpdSettings.UseVisualStyleBackColor = true;
            // 
            // txtDpdPassword
            // 
            this.txtDpdPassword.Location = new System.Drawing.Point(15, 152);
            this.txtDpdPassword.Name = "txtDpdPassword";
            this.txtDpdPassword.PasswordChar = '*';
            this.txtDpdPassword.Size = new System.Drawing.Size(350, 27);
            this.txtDpdPassword.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 20);
            this.label6.TabIndex = 4;
            this.label6.Text = "Hasło";
            // 
            // txtDpdClientId
            // 
            this.txtDpdClientId.Location = new System.Drawing.Point(15, 100);
            this.txtDpdClientId.Name = "txtDpdClientId";
            this.txtDpdClientId.Size = new System.Drawing.Size(350, 27);
            this.txtDpdClientId.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 20);
            this.label5.TabIndex = 2;
            this.label5.Text = "ID Klienta";
            // 
            // txtDpdLogin
            // 
            this.txtDpdLogin.Location = new System.Drawing.Point(15, 50);
            this.txtDpdLogin.Name = "txtDpdLogin";
            this.txtDpdLogin.Size = new System.Drawing.Size(350, 27);
            this.txtDpdLogin.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Login";
            // 
            // groupBoxAllegro
            // 
            this.groupBoxAllegro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAllegro.Controls.Add(this.btnAutoryzujKonto);
            this.groupBoxAllegro.Controls.Add(this.btnUsunKonto);
            this.groupBoxAllegro.Controls.Add(this.btnDodajKonto);
            this.groupBoxAllegro.Controls.Add(this.listViewKonta);
            this.groupBoxAllegro.Location = new System.Drawing.Point(13, 13);
            this.groupBoxAllegro.Name = "groupBoxAllegro";
            this.groupBoxAllegro.Size = new System.Drawing.Size(866, 311);
            this.groupBoxAllegro.TabIndex = 0;
            this.groupBoxAllegro.TabStop = false;
            this.groupBoxAllegro.Text = "Zarządzanie Kontami Allegro";
            // 
            // btnAutoryzujKonto
            // 
            this.btnAutoryzujKonto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAutoryzujKonto.Location = new System.Drawing.Point(265, 260);
            this.btnAutoryzujKonto.Name = "btnAutoryzujKonto";
            this.btnAutoryzujKonto.Size = new System.Drawing.Size(120, 40);
            this.btnAutoryzujKonto.TabIndex = 3;
            this.btnAutoryzujKonto.Text = "Autoryzuj";
            this.btnAutoryzujKonto.UseVisualStyleBackColor = true;
            // 
            // btnUsunKonto
            // 
            this.btnUsunKonto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUsunKonto.Location = new System.Drawing.Point(139, 260);
            this.btnUsunKonto.Name = "btnUsunKonto";
            this.btnUsunKonto.Size = new System.Drawing.Size(120, 40);
            this.btnUsunKonto.TabIndex = 2;
            this.btnUsunKonto.Text = "Usuń";
            this.btnUsunKonto.UseVisualStyleBackColor = true;
            // 
            // btnDodajKonto
            // 
            this.btnDodajKonto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDodajKonto.Location = new System.Drawing.Point(13, 260);
            this.btnDodajKonto.Name = "btnDodajKonto";
            this.btnDodajKonto.Size = new System.Drawing.Size(120, 40);
            this.btnDodajKonto.TabIndex = 1;
            this.btnDodajKonto.Text = "Dodaj";
            this.btnDodajKonto.UseVisualStyleBackColor = true;
            // 
            // listViewKonta
            // 
            this.listViewKonta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewKonta.HideSelection = false;
            this.listViewKonta.Location = new System.Drawing.Point(15, 25);
            this.listViewKonta.Name = "listViewKonta";
            this.listViewKonta.Size = new System.Drawing.Size(835, 225);
            this.listViewKonta.TabIndex = 0;
            this.listViewKonta.UseCompatibleStateImageBehavior = false;
            // 
            // groupBoxDeepL
            // 
            this.groupBoxDeepL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDeepL.Controls.Add(this.btnSaveDeepLSettings);
            this.groupBoxDeepL.Controls.Add(this.txtDeepLApiKey);
            this.groupBoxDeepL.Controls.Add(this.label7);
            this.groupBoxDeepL.Location = new System.Drawing.Point(13, 536);
            this.groupBoxDeepL.Name = "groupBoxDeepL";
            this.groupBoxDeepL.Size = new System.Drawing.Size(866, 95);
            this.groupBoxDeepL.TabIndex = 2;
            this.groupBoxDeepL.TabStop = false;
            this.groupBoxDeepL.Text = "Ustawienia Tłumaczeń (DeepL)";
            // 
            // btnSaveDeepLSettings
            // 
            this.btnSaveDeepLSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveDeepLSettings.Location = new System.Drawing.Point(680, 43);
            this.btnSaveDeepLSettings.Name = "btnSaveDeepLSettings";
            this.btnSaveDeepLSettings.Size = new System.Drawing.Size(180, 40);
            this.btnSaveDeepLSettings.TabIndex = 2;
            this.btnSaveDeepLSettings.Text = "Zapisz Klucz API";
            this.btnSaveDeepLSettings.UseVisualStyleBackColor = true;
            // 
            // txtDeepLApiKey
            // 
            this.txtDeepLApiKey.Location = new System.Drawing.Point(15, 50);
            this.txtDeepLApiKey.Name = "txtDeepLApiKey";
            this.txtDeepLApiKey.PasswordChar = '*';
            this.txtDeepLApiKey.Size = new System.Drawing.Size(450, 27);
            this.txtDeepLApiKey.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Klucz API DeepL:";
            // 
            // groupBoxEmail
            // 
            this.groupBoxEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxEmail.Controls.Add(this.btnSaveEmailSettings);
            this.groupBoxEmail.Controls.Add(this.txtDefaultEmail);
            this.groupBoxEmail.Controls.Add(this.label8);
            this.groupBoxEmail.Location = new System.Drawing.Point(13, 637);
            this.groupBoxEmail.Name = "groupBoxEmail";
            this.groupBoxEmail.Size = new System.Drawing.Size(866, 95);
            this.groupBoxEmail.TabIndex = 3;
            this.groupBoxEmail.TabStop = false;
            this.groupBoxEmail.Text = "Ustawienia E-mail";
            // 
            // btnSaveEmailSettings
            // 
            this.btnSaveEmailSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveEmailSettings.Location = new System.Drawing.Point(680, 43);
            this.btnSaveEmailSettings.Name = "btnSaveEmailSettings";
            this.btnSaveEmailSettings.Size = new System.Drawing.Size(180, 40);
            this.btnSaveEmailSettings.TabIndex = 2;
            this.btnSaveEmailSettings.Text = "Zapisz E-mail";
            this.btnSaveEmailSettings.UseVisualStyleBackColor = true;
            // 
            // txtDefaultEmail
            // 
            this.txtDefaultEmail.Location = new System.Drawing.Point(15, 50);
            this.txtDefaultEmail.Name = "txtDefaultEmail";
            this.txtDefaultEmail.Size = new System.Drawing.Size(450, 27);
            this.txtDefaultEmail.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(220, 20);
            this.label8.TabIndex = 0;
            this.label8.Text = "Domyślny e-mail magazynu (do wysyłek):";
            // 
            // tabPageOpiekunowie
            // 
            this.tabPageOpiekunowie.Controls.Add(this.allegroOpiekunowieControl1);
            this.tabPageOpiekunowie.Location = new System.Drawing.Point(4, 29);
            this.tabPageOpiekunowie.Name = "tabPageOpiekunowie";
            this.tabPageOpiekunowie.Padding = new System.Windows.Forms.Padding(8);
            this.tabPageOpiekunowie.Size = new System.Drawing.Size(892, 717);
            this.tabPageOpiekunowie.TabIndex = 2;
            this.tabPageOpiekunowie.Text = "Opiekunowie Allegro";
            this.tabPageOpiekunowie.UseVisualStyleBackColor = true;
            // 
            // allegroOpiekunowieControl1
            // 
            this.allegroOpiekunowieControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.allegroOpiekunowieControl1.Location = new System.Drawing.Point(8, 8);
            this.allegroOpiekunowieControl1.Name = "allegroOpiekunowieControl1";
            this.allegroOpiekunowieControl1.Size = new System.Drawing.Size(876, 701);
            this.allegroOpiekunowieControl1.TabIndex = 0;
            // 
            // tabPageDelegacje
            // 
            this.tabPageDelegacje.Controls.Add(this.delegacjeControl1);
            this.tabPageDelegacje.Location = new System.Drawing.Point(4, 29);
            this.tabPageDelegacje.Name = "tabPageDelegacje";
            this.tabPageDelegacje.Padding = new System.Windows.Forms.Padding(8);
            this.tabPageDelegacje.Size = new System.Drawing.Size(892, 717);
            this.tabPageDelegacje.TabIndex = 3;
            this.tabPageDelegacje.Text = "Delegacje";
            this.tabPageDelegacje.UseVisualStyleBackColor = true;
            // 
            // delegacjeControl1
            // 
            this.delegacjeControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.delegacjeControl1.Location = new System.Drawing.Point(8, 8);
            this.delegacjeControl1.Name = "delegacjeControl1";
            this.delegacjeControl1.Size = new System.Drawing.Size(876, 701);
            this.delegacjeControl1.TabIndex = 0;
            // 
            // AdminControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlAdmin);
            this.Name = "AdminControl";
            this.Size = new System.Drawing.Size(900, 850);
            this.Load += new System.EventHandler(this.AdminControl_Load);
            this.tabControlAdmin.ResumeLayout(false);
            this.tabPageUsers.ResumeLayout(false);
            this.splitContainerUsers.Panel1.ResumeLayout(false);
            this.splitContainerUsers.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerUsers)).EndInit();
            this.splitContainerUsers.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panelUserDetails.ResumeLayout(false);
            this.groupBoxUprawnienia.ResumeLayout(false);
            this.groupBoxDanePodstawowe.ResumeLayout(false);
            this.groupBoxDanePodstawowe.PerformLayout();
            this.panelAkcje.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.groupBoxDpd.ResumeLayout(false);
            this.groupBoxDpd.PerformLayout();
            this.groupBoxAllegro.ResumeLayout(false);
            this.groupBoxDeepL.ResumeLayout(false);
            this.groupBoxDeepL.PerformLayout();
            this.groupBoxEmail.ResumeLayout(false);
            this.groupBoxEmail.PerformLayout();
            this.tabPageOpiekunowie.ResumeLayout(false);
            this.tabPageDelegacje.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControlAdmin;
        private System.Windows.Forms.TabPage tabPageUsers;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.SplitContainer splitContainerUsers;
        private System.Windows.Forms.ListView listViewUsers;
        private System.Windows.Forms.Panel panelUserDetails;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.TextBox txtNazwaWyswietlana;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboRola;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox checkedListBoxModules;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Button btnResetPassword;
        private System.Windows.Forms.GroupBox groupBoxAllegro;
        private System.Windows.Forms.ListView listViewKonta;
        private System.Windows.Forms.GroupBox groupBoxDpd;
        private System.Windows.Forms.TextBox txtDpdLogin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDpdClientId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDpdPassword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSaveDpdSettings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panelAkcje;
        private System.Windows.Forms.GroupBox groupBoxUprawnienia;
        private System.Windows.Forms.GroupBox groupBoxDanePodstawowe;
        private System.Windows.Forms.Button btnAutoryzujKonto;
        private System.Windows.Forms.Button btnUsunKonto;
        private System.Windows.Forms.Button btnDodajKonto;
        private System.Windows.Forms.GroupBox groupBoxDeepL;
        private System.Windows.Forms.Button btnSaveDeepLSettings;
        private System.Windows.Forms.TextBox txtDeepLApiKey;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBoxEmail;
        private System.Windows.Forms.Button btnSaveEmailSettings;
        private System.Windows.Forms.TextBox txtDefaultEmail;
        private System.Windows.Forms.Label label8;
        private TabPage tabPageOpiekunowie;
        private AllegroOpiekunowieControl allegroOpiekunowieControl1;
        private TabPage tabPageDelegacje;
        private DelegacjeControl delegacjeControl1;
    }
}