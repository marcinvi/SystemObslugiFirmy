namespace Reklamacje_Dane
{
    partial class FormUniversalWizard
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
            this.pnlInitialSelection = new System.Windows.Forms.Panel();
            this.lblInitialSelection = new System.Windows.Forms.Label();
            this.listViewInitialSelection = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControlWizard = new System.Windows.Forms.TabControl();
            this.tabPageKlient = new System.Windows.Forms.TabPage();
            this.lblClientHistoryInfo = new System.Windows.Forms.Label();
            this.pnlConflictResolution = new System.Windows.Forms.Panel();
            this.btnUseDbData = new System.Windows.Forms.Button();
            this.btnUpdateClientData = new System.Windows.Forms.Button();
            this.lblConflictInfo = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSzukajKlienta = new System.Windows.Forms.TextBox();
            this.chkZapiszJakoNowy = new System.Windows.Forms.CheckBox();
            this.groupBoxDaneKlienta = new System.Windows.Forms.GroupBox();
            this.txtImieNazwisko = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTelefon = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMiejscowosc = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtKodPocztowy = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUlica = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtNIP = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtNazwaFirmy = new System.Windows.Forms.TextBox();
            this.dataGridViewPropozycje = new System.Windows.Forms.DataGridView();
            this.tabPageProdukt = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.dataGridViewProdukty = new System.Windows.Forms.DataGridView();
            this.txtSzukajProduktu = new System.Windows.Forms.TextBox();
            this.tabPageUsterka = new System.Windows.Forms.TabPage();
            this.gbSummary = new System.Windows.Forms.GroupBox();
            this.lblSummaryProduct = new System.Windows.Forms.Label();
            this.lblSummaryClient = new System.Windows.Forms.Label();
            this.lblExistingComplaintInfo = new System.Windows.Forms.Label();
            this.gbManualSource = new System.Windows.Forms.GroupBox();
            this.rbEnaTruck = new System.Windows.Forms.RadioButton();
            this.rbTruckShop = new System.Windows.Forms.RadioButton();
            this.lblDataFakturyInfo = new System.Windows.Forms.Label();
            this.lblFakturaInfo = new System.Windows.Forms.Label();
            this.lblSerialInfo = new System.Windows.Forms.Label();
            this.chkFirma = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbPlatna = new System.Windows.Forms.RadioButton();
            this.rbGwarancyjna = new System.Windows.Forms.RadioButton();
            this.lblGwarancjaStatus = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtnrSeryjny = new System.Windows.Forms.TextBox();
            this.dtpDataZakupu = new System.Windows.Forms.DateTimePicker();
            this.txtNumerFaktury = new System.Windows.Forms.TextBox();
            this.txtOpisUsterki = new System.Windows.Forms.TextBox();
            this.btnDalej = new System.Windows.Forms.Button();
            this.btnWstecz = new System.Windows.Forms.Button();
            this.pnlStepper = new System.Windows.Forms.Panel();
            this.lblStep3 = new System.Windows.Forms.Label();
            this.lblStep2 = new System.Windows.Forms.Label();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.pnlInitialSelection.SuspendLayout();
            this.tabControlWizard.SuspendLayout();
            this.tabPageKlient.SuspendLayout();
            this.pnlConflictResolution.SuspendLayout();
            this.groupBoxDaneKlienta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPropozycje)).BeginInit();
            this.tabPageProdukt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProdukty)).BeginInit();
            this.tabPageUsterka.SuspendLayout();
            this.gbSummary.SuspendLayout();
            this.gbManualSource.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnlStepper.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlInitialSelection
            // 
            this.pnlInitialSelection.Controls.Add(this.lblInitialSelection);
            this.pnlInitialSelection.Controls.Add(this.listViewInitialSelection);
            this.pnlInitialSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInitialSelection.Location = new System.Drawing.Point(0, 0);
            this.pnlInitialSelection.Name = "pnlInitialSelection";
            this.pnlInitialSelection.Size = new System.Drawing.Size(982, 553);
            this.pnlInitialSelection.TabIndex = 5;
            this.pnlInitialSelection.Visible = false;
            // 
            // lblInitialSelection
            // 
            this.lblInitialSelection.AutoSize = true;
            this.lblInitialSelection.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblInitialSelection.Location = new System.Drawing.Point(12, 13);
            this.lblInitialSelection.Name = "lblInitialSelection";
            this.lblInitialSelection.Size = new System.Drawing.Size(325, 23);
            this.lblInitialSelection.TabIndex = 1;
            this.lblInitialSelection.Text = "Wybierz zgłoszenie do przetworzenia:";
            // 
            // listViewInitialSelection
            // 
            this.listViewInitialSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewInitialSelection.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewInitialSelection.FullRowSelect = true;
            this.listViewInitialSelection.GridLines = true;
            this.listViewInitialSelection.HideSelection = false;
            this.listViewInitialSelection.Location = new System.Drawing.Point(16, 48);
            this.listViewInitialSelection.MultiSelect = false;
            this.listViewInitialSelection.Name = "listViewInitialSelection";
            this.listViewInitialSelection.Size = new System.Drawing.Size(954, 493);
            this.listViewInitialSelection.TabIndex = 0;
            this.listViewInitialSelection.UseCompatibleStateImageBehavior = false;
            this.listViewInitialSelection.View = System.Windows.Forms.View.Details;
            this.listViewInitialSelection.DoubleClick += new System.EventHandler(this.listViewInitialSelection_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Identyfikator";
            this.columnHeader1.Width = 180;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Klient / Temat";
            this.columnHeader2.Width = 350;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Produkt / Opis";
            this.columnHeader3.Width = 400;
            // 
            // tabControlWizard
            // 
            this.tabControlWizard.Controls.Add(this.tabPageKlient);
            this.tabControlWizard.Controls.Add(this.tabPageProdukt);
            this.tabControlWizard.Controls.Add(this.tabPageUsterka);
            this.tabControlWizard.Location = new System.Drawing.Point(0, 42);
            this.tabControlWizard.Name = "tabControlWizard";
            this.tabControlWizard.SelectedIndex = 0;
            this.tabControlWizard.Size = new System.Drawing.Size(982, 463);
            this.tabControlWizard.TabIndex = 0;
            this.tabControlWizard.SelectedIndexChanged += new System.EventHandler(this.tabControlWizard_SelectedIndexChanged);
            // 
            // tabPageKlient
            // 
            this.tabPageKlient.Controls.Add(this.lblClientHistoryInfo);
            this.tabPageKlient.Controls.Add(this.pnlConflictResolution);
            this.tabPageKlient.Controls.Add(this.label8);
            this.tabPageKlient.Controls.Add(this.txtSzukajKlienta);
            this.tabPageKlient.Controls.Add(this.chkZapiszJakoNowy);
            this.tabPageKlient.Controls.Add(this.groupBoxDaneKlienta);
            this.tabPageKlient.Controls.Add(this.dataGridViewPropozycje);
            this.tabPageKlient.Location = new System.Drawing.Point(4, 29);
            this.tabPageKlient.Name = "tabPageKlient";
            this.tabPageKlient.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKlient.Size = new System.Drawing.Size(974, 430);
            this.tabPageKlient.TabIndex = 0;
            this.tabPageKlient.Text = "Klient";
            this.tabPageKlient.UseVisualStyleBackColor = true;
            // 
            // lblClientHistoryInfo
            // 
            this.lblClientHistoryInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClientHistoryInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblClientHistoryInfo.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblClientHistoryInfo.Location = new System.Drawing.Point(628, 194);
            this.lblClientHistoryInfo.Name = "lblClientHistoryInfo";
            this.lblClientHistoryInfo.Size = new System.Drawing.Size(329, 20);
            this.lblClientHistoryInfo.TabIndex = 20;
            this.lblClientHistoryInfo.Text = "Klient ma już 3 inne zgłoszenia.";
            this.lblClientHistoryInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblClientHistoryInfo.Visible = false;
            // 
            // pnlConflictResolution
            // 
            this.pnlConflictResolution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConflictResolution.BackColor = System.Drawing.Color.LightYellow;
            this.pnlConflictResolution.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlConflictResolution.Controls.Add(this.btnUseDbData);
            this.pnlConflictResolution.Controls.Add(this.btnUpdateClientData);
            this.pnlConflictResolution.Controls.Add(this.lblConflictInfo);
            this.pnlConflictResolution.Location = new System.Drawing.Point(16, 350);
            this.pnlConflictResolution.Name = "pnlConflictResolution";
            this.pnlConflictResolution.Size = new System.Drawing.Size(941, 44);
            this.pnlConflictResolution.TabIndex = 19;
            this.pnlConflictResolution.Visible = false;
            // 
            // btnUseDbData
            // 
            this.btnUseDbData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUseDbData.Location = new System.Drawing.Point(400, 7);
            this.btnUseDbData.Name = "btnUseDbData";
            this.btnUseDbData.Size = new System.Drawing.Size(250, 30);
            this.btnUseDbData.TabIndex = 2;
            this.btnUseDbData.Text = "Użyj danych z bazy (ignoruj nowe)";
            this.btnUseDbData.UseVisualStyleBackColor = true;
            this.btnUseDbData.Click += new System.EventHandler(this.btnUseDbData_Click);
            // 
            // btnUpdateClientData
            // 
            this.btnUpdateClientData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateClientData.Location = new System.Drawing.Point(660, 7);
            this.btnUpdateClientData.Name = "btnUpdateClientData";
            this.btnUpdateClientData.Size = new System.Drawing.Size(270, 30);
            this.btnUpdateClientData.TabIndex = 1;
            this.btnUpdateClientData.Text = "Zaktualizuj dane w bazie na nowe";
            this.btnUpdateClientData.UseVisualStyleBackColor = true;
            this.btnUpdateClientData.Click += new System.EventHandler(this.btnUpdateClientData_Click);
            // 
            // lblConflictInfo
            // 
            this.lblConflictInfo.AutoSize = true;
            this.lblConflictInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblConflictInfo.Location = new System.Drawing.Point(13, 12);
            this.lblConflictInfo.Name = "lblConflictInfo";
            this.lblConflictInfo.Size = new System.Drawing.Size(342, 20);
            this.lblConflictInfo.TabIndex = 0;
            this.lblConflictInfo.Text = "Wykryto różnicę w danych klienta. Co chcesz zrobić?";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 194);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(374, 20);
            this.label8.TabIndex = 18;
            this.label8.Text = "Wyszukaj lub wybierz klienta z poniższych propozycji:";
            // 
            // txtSzukajKlienta
            // 
            this.txtSzukajKlienta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSzukajKlienta.Location = new System.Drawing.Point(16, 217);
            this.txtSzukajKlienta.Name = "txtSzukajKlienta";
            this.txtSzukajKlienta.Size = new System.Drawing.Size(941, 27);
            this.txtSzukajKlienta.TabIndex = 17;
            this.txtSzukajKlienta.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // chkZapiszJakoNowy
            // 
            this.chkZapiszJakoNowy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkZapiszJakoNowy.AutoSize = true;
            this.chkZapiszJakoNowy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chkZapiszJakoNowy.ForeColor = System.Drawing.Color.ForestGreen;
            this.chkZapiszJakoNowy.Location = new System.Drawing.Point(16, 400);
            this.chkZapiszJakoNowy.Name = "chkZapiszJakoNowy";
            this.chkZapiszJakoNowy.Size = new System.Drawing.Size(434, 24);
            this.chkZapiszJakoNowy.TabIndex = 16;
            this.chkZapiszJakoNowy.Text = "Nie ma na liście? Użyj powyższych danych jako nowego klienta";
            this.chkZapiszJakoNowy.UseVisualStyleBackColor = true;
            this.chkZapiszJakoNowy.CheckedChanged += new System.EventHandler(this.chkZapiszJakoNowy_CheckedChanged);
            // 
            // groupBoxDaneKlienta
            // 
            this.groupBoxDaneKlienta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDaneKlienta.Controls.Add(this.txtImieNazwisko);
            this.groupBoxDaneKlienta.Controls.Add(this.label1);
            this.groupBoxDaneKlienta.Controls.Add(this.txtTelefon);
            this.groupBoxDaneKlienta.Controls.Add(this.label2);
            this.groupBoxDaneKlienta.Controls.Add(this.txtMail);
            this.groupBoxDaneKlienta.Controls.Add(this.label3);
            this.groupBoxDaneKlienta.Controls.Add(this.txtMiejscowosc);
            this.groupBoxDaneKlienta.Controls.Add(this.label4);
            this.groupBoxDaneKlienta.Controls.Add(this.txtKodPocztowy);
            this.groupBoxDaneKlienta.Controls.Add(this.label5);
            this.groupBoxDaneKlienta.Controls.Add(this.txtUlica);
            this.groupBoxDaneKlienta.Controls.Add(this.label6);
            this.groupBoxDaneKlienta.Controls.Add(this.txtNIP);
            this.groupBoxDaneKlienta.Controls.Add(this.label7);
            this.groupBoxDaneKlienta.Controls.Add(this.txtNazwaFirmy);
            this.groupBoxDaneKlienta.Location = new System.Drawing.Point(16, 6);
            this.groupBoxDaneKlienta.Name = "groupBoxDaneKlienta";
            this.groupBoxDaneKlienta.Size = new System.Drawing.Size(941, 185);
            this.groupBoxDaneKlienta.TabIndex = 15;
            this.groupBoxDaneKlienta.TabStop = false;
            this.groupBoxDaneKlienta.Text = "Dane Klienta";
            // 
            // txtImieNazwisko
            // 
            this.txtImieNazwisko.Location = new System.Drawing.Point(16, 49);
            this.txtImieNazwisko.Name = "txtImieNazwisko";
            this.txtImieNazwisko.Size = new System.Drawing.Size(286, 27);
            this.txtImieNazwisko.TabIndex = 0;
            this.txtImieNazwisko.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Imię i Nazwisko";
            // 
            // txtTelefon
            // 
            this.txtTelefon.Location = new System.Drawing.Point(460, 142);
            this.txtTelefon.Name = "txtTelefon";
            this.txtTelefon.Size = new System.Drawing.Size(420, 27);
            this.txtTelefon.TabIndex = 7;
            this.txtTelefon.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(313, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Nazwa Firmy";
            // 
            // txtMail
            // 
            this.txtMail.Location = new System.Drawing.Point(16, 142);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(420, 27);
            this.txtMail.TabIndex = 6;
            this.txtMail.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(644, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "NIP";
            // 
            // txtMiejscowosc
            // 
            this.txtMiejscowosc.Location = new System.Drawing.Point(460, 96);
            this.txtMiejscowosc.Name = "txtMiejscowosc";
            this.txtMiejscowosc.Size = new System.Drawing.Size(420, 27);
            this.txtMiejscowosc.TabIndex = 5;
            this.txtMiejscowosc.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Ulica, numer";
            // 
            // txtKodPocztowy
            // 
            this.txtKodPocztowy.Location = new System.Drawing.Point(305, 96);
            this.txtKodPocztowy.Name = "txtKodPocztowy";
            this.txtKodPocztowy.Size = new System.Drawing.Size(131, 27);
            this.txtKodPocztowy.TabIndex = 4;
            this.txtKodPocztowy.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(301, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Kod pocztowy";
            // 
            // txtUlica
            // 
            this.txtUlica.Location = new System.Drawing.Point(16, 96);
            this.txtUlica.Name = "txtUlica";
            this.txtUlica.Size = new System.Drawing.Size(265, 27);
            this.txtUlica.TabIndex = 3;
            this.txtUlica.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(456, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "Miejscowość";
            // 
            // txtNIP
            // 
            this.txtNIP.Location = new System.Drawing.Point(648, 49);
            this.txtNIP.Name = "txtNIP";
            this.txtNIP.Size = new System.Drawing.Size(232, 27);
            this.txtNIP.TabIndex = 2;
            this.txtNIP.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 20);
            this.label7.TabIndex = 14;
            this.label7.Text = "Email";
            // 
            // txtNazwaFirmy
            // 
            this.txtNazwaFirmy.Location = new System.Drawing.Point(317, 49);
            this.txtNazwaFirmy.Name = "txtNazwaFirmy";
            this.txtNazwaFirmy.Size = new System.Drawing.Size(306, 27);
            this.txtNazwaFirmy.TabIndex = 1;
            this.txtNazwaFirmy.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // dataGridViewPropozycje
            // 
            this.dataGridViewPropozycje.AllowUserToAddRows = false;
            this.dataGridViewPropozycje.AllowUserToDeleteRows = false;
            this.dataGridViewPropozycje.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewPropozycje.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewPropozycje.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewPropozycje.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPropozycje.Location = new System.Drawing.Point(16, 250);
            this.dataGridViewPropozycje.MultiSelect = false;
            this.dataGridViewPropozycje.Name = "dataGridViewPropozycje";
            this.dataGridViewPropozycje.ReadOnly = true;
            this.dataGridViewPropozycje.RowHeadersVisible = false;
            this.dataGridViewPropozycje.RowTemplate.Height = 24;
            this.dataGridViewPropozycje.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPropozycje.Size = new System.Drawing.Size(941, 94);
            this.dataGridViewPropozycje.TabIndex = 14;
            this.dataGridViewPropozycje.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPropozycje_CellClick);
            this.dataGridViewPropozycje.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPropozycje_CellDoubleClick);
            this.dataGridViewPropozycje.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridViewPropozycje_CellFormatting);
            // 
            // tabPageProdukt
            // 
            this.tabPageProdukt.Controls.Add(this.label12);
            this.tabPageProdukt.Controls.Add(this.dataGridViewProdukty);
            this.tabPageProdukt.Controls.Add(this.txtSzukajProduktu);
            this.tabPageProdukt.Location = new System.Drawing.Point(4, 29);
            this.tabPageProdukt.Name = "tabPageProdukt";
            this.tabPageProdukt.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProdukt.Size = new System.Drawing.Size(974, 430);
            this.tabPageProdukt.TabIndex = 1;
            this.tabPageProdukt.Text = "Produkt";
            this.tabPageProdukt.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 14);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(378, 20);
            this.label12.TabIndex = 6;
            this.label12.Text = "Wyszukaj produkt (po nazwie, producencie lub kodach):";
            // 
            // dataGridViewProdukty
            // 
            this.dataGridViewProdukty.AllowUserToAddRows = false;
            this.dataGridViewProdukty.AllowUserToDeleteRows = false;
            this.dataGridViewProdukty.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewProdukty.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewProdukty.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewProdukty.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewProdukty.Location = new System.Drawing.Point(16, 70);
            this.dataGridViewProdukty.MultiSelect = false;
            this.dataGridViewProdukty.Name = "dataGridViewProdukty";
            this.dataGridViewProdukty.ReadOnly = true;
            this.dataGridViewProdukty.RowHeadersVisible = false;
            this.dataGridViewProdukty.RowTemplate.Height = 24;
            this.dataGridViewProdukty.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewProdukty.Size = new System.Drawing.Size(941, 354);
            this.dataGridViewProdukty.TabIndex = 5;
            this.dataGridViewProdukty.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewProdukty_CellClick);
            this.dataGridViewProdukty.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewProdukty_CellDoubleClick);
            // 
            // txtSzukajProduktu
            // 
            this.txtSzukajProduktu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSzukajProduktu.Location = new System.Drawing.Point(16, 37);
            this.txtSzukajProduktu.Name = "txtSzukajProduktu";
            this.txtSzukajProduktu.Size = new System.Drawing.Size(941, 27);
            this.txtSzukajProduktu.TabIndex = 4;
            this.txtSzukajProduktu.TextChanged += new System.EventHandler(this.Input_TextChanged_Debounced);
            // 
            // tabPageUsterka
            // 
            this.tabPageUsterka.Controls.Add(this.gbSummary);
            this.tabPageUsterka.Controls.Add(this.lblExistingComplaintInfo);
            this.tabPageUsterka.Controls.Add(this.gbManualSource);
            this.tabPageUsterka.Controls.Add(this.lblDataFakturyInfo);
            this.tabPageUsterka.Controls.Add(this.lblFakturaInfo);
            this.tabPageUsterka.Controls.Add(this.lblSerialInfo);
            this.tabPageUsterka.Controls.Add(this.chkFirma);
            this.tabPageUsterka.Controls.Add(this.groupBox1);
            this.tabPageUsterka.Controls.Add(this.lblGwarancjaStatus);
            this.tabPageUsterka.Controls.Add(this.label19);
            this.tabPageUsterka.Controls.Add(this.label17);
            this.tabPageUsterka.Controls.Add(this.label16);
            this.tabPageUsterka.Controls.Add(this.label15);
            this.tabPageUsterka.Controls.Add(this.txtnrSeryjny);
            this.tabPageUsterka.Controls.Add(this.dtpDataZakupu);
            this.tabPageUsterka.Controls.Add(this.txtNumerFaktury);
            this.tabPageUsterka.Controls.Add(this.txtOpisUsterki);
            this.tabPageUsterka.Location = new System.Drawing.Point(4, 29);
            this.tabPageUsterka.Name = "tabPageUsterka";
            this.tabPageUsterka.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUsterka.Size = new System.Drawing.Size(974, 430);
            this.tabPageUsterka.TabIndex = 2;
            this.tabPageUsterka.Text = "Usterka";
            this.tabPageUsterka.UseVisualStyleBackColor = true;
            // 
            // gbSummary
            // 
            this.gbSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSummary.Controls.Add(this.lblSummaryProduct);
            this.gbSummary.Controls.Add(this.lblSummaryClient);
            this.gbSummary.Location = new System.Drawing.Point(16, 6);
            this.gbSummary.Name = "gbSummary";
            this.gbSummary.Size = new System.Drawing.Size(941, 78);
            this.gbSummary.TabIndex = 16;
            this.gbSummary.TabStop = false;
            this.gbSummary.Text = "Podsumowanie";
            // 
            // lblSummaryProduct
            // 
            this.lblSummaryProduct.AutoSize = true;
            this.lblSummaryProduct.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSummaryProduct.Location = new System.Drawing.Point(13, 48);
            this.lblSummaryProduct.Name = "lblSummaryProduct";
            this.lblSummaryProduct.Size = new System.Drawing.Size(65, 20);
            this.lblSummaryProduct.TabIndex = 1;
            this.lblSummaryProduct.Text = "Produkt: ";
            // 
            // lblSummaryClient
            // 
            this.lblSummaryClient.AutoSize = true;
            this.lblSummaryClient.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSummaryClient.Location = new System.Drawing.Point(13, 23);
            this.lblSummaryClient.Name = "lblSummaryClient";
            this.lblSummaryClient.Size = new System.Drawing.Size(53, 20);
            this.lblSummaryClient.TabIndex = 0;
            this.lblSummaryClient.Text = "Klient: ";
            // 
            // lblExistingComplaintInfo
            // 
            this.lblExistingComplaintInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExistingComplaintInfo.BackColor = System.Drawing.Color.MistyRose;
            this.lblExistingComplaintInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblExistingComplaintInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblExistingComplaintInfo.ForeColor = System.Drawing.Color.DarkRed;
            this.lblExistingComplaintInfo.Location = new System.Drawing.Point(16, 386);
            this.lblExistingComplaintInfo.Name = "lblExistingComplaintInfo";
            this.lblExistingComplaintInfo.Padding = new System.Windows.Forms.Padding(5);
            this.lblExistingComplaintInfo.Size = new System.Drawing.Size(941, 38);
            this.lblExistingComplaintInfo.TabIndex = 15;
            this.lblExistingComplaintInfo.Text = "UWAGA: Ten klient ma już zgłoszenie (nr 015/25) na ten produkt!";
            this.lblExistingComplaintInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblExistingComplaintInfo.Visible = false;
            // 
            // gbManualSource
            // 
            this.gbManualSource.Controls.Add(this.rbEnaTruck);
            this.gbManualSource.Controls.Add(this.rbTruckShop);
            this.gbManualSource.Location = new System.Drawing.Point(300, 172);
            this.gbManualSource.Name = "gbManualSource";
            this.gbManualSource.Size = new System.Drawing.Size(300, 60);
            this.gbManualSource.TabIndex = 14;
            this.gbManualSource.TabStop = false;
            this.gbManualSource.Text = "Źródło zgłoszenia";
            this.gbManualSource.Visible = false;
            // 
            // rbEnaTruck
            // 
            this.rbEnaTruck.AutoSize = true;
            this.rbEnaTruck.Location = new System.Drawing.Point(150, 26);
            this.rbEnaTruck.Name = "rbEnaTruck";
            this.rbEnaTruck.Size = new System.Drawing.Size(95, 24);
            this.rbEnaTruck.TabIndex = 1;
            this.rbEnaTruck.Text = "Ena-Truck";
            this.rbEnaTruck.UseVisualStyleBackColor = true;
            // 
            // rbTruckShop
            // 
            this.rbTruckShop.AutoSize = true;
            this.rbTruckShop.Checked = true;
            this.rbTruckShop.Location = new System.Drawing.Point(17, 26);
            this.rbTruckShop.Name = "rbTruckShop";
            this.rbTruckShop.Size = new System.Drawing.Size(102, 24);
            this.rbTruckShop.TabIndex = 0;
            this.rbTruckShop.TabStop = true;
            this.rbTruckShop.Text = "Truck-Shop";
            this.rbTruckShop.UseVisualStyleBackColor = true;
            // 
            // lblDataFakturyInfo
            // 
            this.lblDataFakturyInfo.AutoSize = true;
            this.lblDataFakturyInfo.ForeColor = System.Drawing.Color.Red;
            this.lblDataFakturyInfo.Location = new System.Drawing.Point(495, 110);
            this.lblDataFakturyInfo.Name = "lblDataFakturyInfo";
            this.lblDataFakturyInfo.Size = new System.Drawing.Size(123, 20);
            this.lblDataFakturyInfo.TabIndex = 13;
            this.lblDataFakturyInfo.Text = "Data Consistency";
            this.lblDataFakturyInfo.Visible = false;
            // 
            // lblFakturaInfo
            // 
            this.lblFakturaInfo.AutoSize = true;
            this.lblFakturaInfo.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblFakturaInfo.Location = new System.Drawing.Point(284, 87);
            this.lblFakturaInfo.Name = "lblFakturaInfo";
            this.lblFakturaInfo.Size = new System.Drawing.Size(91, 20);
            this.lblFakturaInfo.TabIndex = 12;
            this.lblFakturaInfo.Text = "Faktura Info";
            this.lblFakturaInfo.Visible = false;
            // 
            // lblSerialInfo
            // 
            this.lblSerialInfo.AutoSize = true;
            this.lblSerialInfo.ForeColor = System.Drawing.Color.Red;
            this.lblSerialInfo.Location = new System.Drawing.Point(12, 59);
            this.lblSerialInfo.Name = "lblSerialInfo";
            this.lblSerialInfo.Size = new System.Drawing.Size(77, 20);
            this.lblSerialInfo.TabIndex = 11;
            this.lblSerialInfo.Text = "Serial Info";
            this.lblSerialInfo.Visible = false;
            // 
            // chkFirma
            // 
            this.chkFirma.AutoSize = true;
            this.chkFirma.Location = new System.Drawing.Point(16, 142);
            this.chkFirma.Name = "chkFirma";
            this.chkFirma.Size = new System.Drawing.Size(127, 24);
            this.chkFirma.TabIndex = 10;
            this.chkFirma.Text = "Klient firmowy";
            this.chkFirma.UseVisualStyleBackColor = true;
            this.chkFirma.CheckedChanged += new System.EventHandler(this.chkFirma_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbPlatna);
            this.groupBox1.Controls.Add(this.rbGwarancyjna);
            this.groupBox1.Location = new System.Drawing.Point(16, 172);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(262, 60);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Typ reklamacji";
            // 
            // rbPlatna
            // 
            this.rbPlatna.AutoSize = true;
            this.rbPlatna.Location = new System.Drawing.Point(149, 26);
            this.rbPlatna.Name = "rbPlatna";
            this.rbPlatna.Size = new System.Drawing.Size(71, 24);
            this.rbPlatna.TabIndex = 1;
            this.rbPlatna.Text = "Płatna";
            this.rbPlatna.UseVisualStyleBackColor = true;
            // 
            // rbGwarancyjna
            // 
            this.rbGwarancyjna.AutoSize = true;
            this.rbGwarancyjna.Checked = true;
            this.rbGwarancyjna.Location = new System.Drawing.Point(17, 26);
            this.rbGwarancyjna.Name = "rbGwarancyjna";
            this.rbGwarancyjna.Size = new System.Drawing.Size(114, 24);
            this.rbGwarancyjna.TabIndex = 0;
            this.rbGwarancyjna.TabStop = true;
            this.rbGwarancyjna.Text = "Gwarancyjna";
            this.rbGwarancyjna.UseVisualStyleBackColor = true;
            // 
            // lblGwarancjaStatus
            // 
            this.lblGwarancjaStatus.AutoSize = true;
            this.lblGwarancjaStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblGwarancjaStatus.Location = new System.Drawing.Point(168, 143);
            this.lblGwarancjaStatus.Name = "lblGwarancjaStatus";
            this.lblGwarancjaStatus.Size = new System.Drawing.Size(126, 20);
            this.lblGwarancjaStatus.TabIndex = 8;
            this.lblGwarancjaStatus.Text = "Status Gwarancji";
            this.lblGwarancjaStatus.Visible = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(12, 244);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(135, 20);
            this.label19.TabIndex = 7;
            this.label19.Text = "Opis usterki/uwagi:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(284, 110);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(92, 20);
            this.label17.TabIndex = 6;
            this.label17.Text = "Data zakupu";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 87);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(104, 20);
            this.label16.TabIndex = 5;
            this.label16.Text = "Numer Faktury";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 9);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(102, 20);
            this.label15.TabIndex = 4;
            this.label15.Text = "Numer seryjny";
            // 
            // txtnrSeryjny
            // 
            this.txtnrSeryjny.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtnrSeryjny.Location = new System.Drawing.Point(16, 32);
            this.txtnrSeryjny.Name = "txtnrSeryjny";
            this.txtnrSeryjny.Size = new System.Drawing.Size(941, 27);
            this.txtnrSeryjny.TabIndex = 3;
            this.txtnrSeryjny.Leave += new System.EventHandler(this.txtnrSeryjny_Leave);
            // 
            // dtpDataZakupu
            // 
            this.dtpDataZakupu.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDataZakupu.Location = new System.Drawing.Point(288, 133);
            this.dtpDataZakupu.Name = "dtpDataZakupu";
            this.dtpDataZakupu.Size = new System.Drawing.Size(150, 27);
            this.dtpDataZakupu.TabIndex = 2;
            this.dtpDataZakupu.ValueChanged += new System.EventHandler(this.dtpDataZakupu_ValueChanged);
            // 
            // txtNumerFaktury
            // 
            this.txtNumerFaktury.Location = new System.Drawing.Point(16, 110);
            this.txtNumerFaktury.Name = "txtNumerFaktury";
            this.txtNumerFaktury.Size = new System.Drawing.Size(262, 27);
            this.txtNumerFaktury.TabIndex = 1;
            this.txtNumerFaktury.TextChanged += new System.EventHandler(this.txtNumerFaktury_TextChanged);
            this.txtNumerFaktury.Leave += new System.EventHandler(this.txtNumerFaktury_Leave);
            // 
            // txtOpisUsterki
            // 
            this.txtOpisUsterki.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOpisUsterki.Location = new System.Drawing.Point(16, 267);
            this.txtOpisUsterki.Multiline = true;
            this.txtOpisUsterki.Name = "txtOpisUsterki";
            this.txtOpisUsterki.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOpisUsterki.Size = new System.Drawing.Size(941, 113);
            this.txtOpisUsterki.TabIndex = 0;
            // 
            // btnDalej
            // 
            this.btnDalej.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDalej.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.btnDalej.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDalej.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDalej.ForeColor = System.Drawing.Color.White;
            this.btnDalej.Location = new System.Drawing.Point(859, 508);
            this.btnDalej.Name = "btnDalej";
            this.btnDalej.Size = new System.Drawing.Size(111, 35);
            this.btnDalej.TabIndex = 1;
            this.btnDalej.Text = "Dalej >";
            this.btnDalej.UseVisualStyleBackColor = false;
            this.btnDalej.Click += new System.EventHandler(this.btnDalej_Click);
            // 
            // btnWstecz
            // 
            this.btnWstecz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnWstecz.BackColor = System.Drawing.Color.Gray;
            this.btnWstecz.Enabled = false;
            this.btnWstecz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWstecz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnWstecz.ForeColor = System.Drawing.Color.White;
            this.btnWstecz.Location = new System.Drawing.Point(12, 508);
            this.btnWstecz.Name = "btnWstecz";
            this.btnWstecz.Size = new System.Drawing.Size(111, 35);
            this.btnWstecz.TabIndex = 2;
            this.btnWstecz.Text = "< Wstecz";
            this.btnWstecz.UseVisualStyleBackColor = false;
            this.btnWstecz.Click += new System.EventHandler(this.btnWstecz_Click);
            // 
            // pnlStepper
            // 
            this.pnlStepper.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlStepper.Controls.Add(this.lblStep3);
            this.pnlStepper.Controls.Add(this.lblStep2);
            this.pnlStepper.Controls.Add(this.lblStep1);
            this.pnlStepper.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStepper.Location = new System.Drawing.Point(0, 0);
            this.pnlStepper.Name = "pnlStepper";
            this.pnlStepper.Size = new System.Drawing.Size(982, 40);
            this.pnlStepper.TabIndex = 3;
            // 
            // lblStep3
            // 
            this.lblStep3.AutoSize = true;
            this.lblStep3.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            this.lblStep3.ForeColor = System.Drawing.Color.Gray;
            this.lblStep3.Location = new System.Drawing.Point(267, 9);
            this.lblStep3.Name = "lblStep3";
            this.lblStep3.Size = new System.Drawing.Size(85, 23);
            this.lblStep3.TabIndex = 2;
            this.lblStep3.Text = "3. Usterka";
            // 
            // lblStep2
            // 
            this.lblStep2.AutoSize = true;
            this.lblStep2.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            this.lblStep2.ForeColor = System.Drawing.Color.Gray;
            this.lblStep2.Location = new System.Drawing.Point(135, 9);
            this.lblStep2.Name = "lblStep2";
            this.lblStep2.Size = new System.Drawing.Size(105, 23);
            this.lblStep2.TabIndex = 1;
            this.lblStep2.Text = "2. Produkt >";
            // 
            // lblStep1
            // 
            this.lblStep1.AutoSize = true;
            this.lblStep1.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold);
            this.lblStep1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblStep1.Location = new System.Drawing.Point(22, 9);
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Size = new System.Drawing.Size(86, 23);
            this.lblStep1.TabIndex = 0;
            this.lblStep1.Text = "1. Klient >";
            // 
            // FormUniversalWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.Controls.Add(this.pnlStepper);
            this.Controls.Add(this.btnWstecz);
            this.Controls.Add(this.btnDalej);
            this.Controls.Add(this.tabControlWizard);
            this.Controls.Add(this.pnlInitialSelection);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "FormUniversalWizard";
            this.Text = "Nowe Zgłoszenie - Uniwersalny Kreator";
            this.Load += new System.EventHandler(this.FormUniversalWizard_Load);
            this.pnlInitialSelection.ResumeLayout(false);
            this.pnlInitialSelection.PerformLayout();
            this.tabControlWizard.ResumeLayout(false);
            this.tabPageKlient.ResumeLayout(false);
            this.tabPageKlient.PerformLayout();
            this.pnlConflictResolution.ResumeLayout(false);
            this.pnlConflictResolution.PerformLayout();
            this.groupBoxDaneKlienta.ResumeLayout(false);
            this.groupBoxDaneKlienta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPropozycje)).EndInit();
            this.tabPageProdukt.ResumeLayout(false);
            this.tabPageProdukt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProdukty)).EndInit();
            this.tabPageUsterka.ResumeLayout(false);
            this.tabPageUsterka.PerformLayout();
            this.gbSummary.ResumeLayout(false);
            this.gbSummary.PerformLayout();
            this.gbManualSource.ResumeLayout(false);
            this.gbManualSource.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlStepper.ResumeLayout(false);
            this.pnlStepper.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlInitialSelection;
        private System.Windows.Forms.Label lblInitialSelection;
        private System.Windows.Forms.ListView listViewInitialSelection;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TabControl tabControlWizard;
        private System.Windows.Forms.TabPage tabPageKlient;
        private System.Windows.Forms.TabPage tabPageProdukt;
        private System.Windows.Forms.TabPage tabPageUsterka;
        private System.Windows.Forms.Button btnDalej;
        private System.Windows.Forms.Button btnWstecz;
        private System.Windows.Forms.Panel pnlStepper;
        private System.Windows.Forms.Label lblStep1;
        private System.Windows.Forms.Label lblStep2;
        private System.Windows.Forms.Label lblStep3;
        private System.Windows.Forms.GroupBox groupBoxDaneKlienta;
        private System.Windows.Forms.TextBox txtImieNazwisko;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTelefon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMiejscowosc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtKodPocztowy;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUlica;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNIP;
        private System.Windows.Forms.TextBox txtNazwaFirmy;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridViewPropozycje;
        private System.Windows.Forms.TextBox txtSzukajKlienta;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkZapiszJakoNowy;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataGridView dataGridViewProdukty;
        private System.Windows.Forms.TextBox txtSzukajProduktu;
        private System.Windows.Forms.CheckBox chkFirma;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbPlatna;
        private System.Windows.Forms.RadioButton rbGwarancyjna;
        private System.Windows.Forms.Label lblGwarancjaStatus;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtnrSeryjny;
        private System.Windows.Forms.DateTimePicker dtpDataZakupu;
        private System.Windows.Forms.TextBox txtNumerFaktury;
        private System.Windows.Forms.TextBox txtOpisUsterki;
        private System.Windows.Forms.Label lblSerialInfo;
        private System.Windows.Forms.Label lblFakturaInfo;
        private System.Windows.Forms.Label lblDataFakturyInfo;
        private System.Windows.Forms.Panel pnlConflictResolution;
        private System.Windows.Forms.Button btnUseDbData;
        private System.Windows.Forms.Button btnUpdateClientData;
        private System.Windows.Forms.Label lblConflictInfo;
        private System.Windows.Forms.GroupBox gbManualSource;
        private System.Windows.Forms.RadioButton rbEnaTruck;
        private System.Windows.Forms.RadioButton rbTruckShop;
        private System.Windows.Forms.Label lblClientHistoryInfo;
        private System.Windows.Forms.GroupBox gbSummary;
        private System.Windows.Forms.Label lblSummaryProduct;
        private System.Windows.Forms.Label lblSummaryClient;
        private System.Windows.Forms.Label lblExistingComplaintInfo;
    }
}