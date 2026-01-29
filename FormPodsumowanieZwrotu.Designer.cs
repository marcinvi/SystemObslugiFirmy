namespace Reklamacje_Dane
{
    partial class FormPodsumowanieZwrotu
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
            this.panelTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblInvoice = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblAllegroAccount = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblBuyerLogin = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxMagazyn = new System.Windows.Forms.GroupBox();
            this.lblDataPrzyjecia = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblPrzyjetyPrzez = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblUwagiMagazynu = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblStanProduktu = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBoxHandlowiec = new System.Windows.Forms.GroupBox();
            this.lblKomentarzHandlowca = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblDecyzjaHandlowca = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.panelActions = new System.Windows.Forms.Panel();
            this.panelPonownaWysylka = new System.Windows.Forms.Panel();
            this.lblPonownaNumerListu = new System.Windows.Forms.Label();
            this.txtNumerListu = new System.Windows.Forms.TextBox();
            this.lblPonownaPrzewoznik = new System.Windows.Forms.Label();
            this.comboCarrier = new System.Windows.Forms.ComboBox();
            this.lblPonownaData = new System.Windows.Forms.Label();
            this.dtpPonownaData = new System.Windows.Forms.DateTimePicker();
            this.btnPrzekazanoDoReklamacji = new System.Windows.Forms.Button();
            this.btnNaPolke = new System.Windows.Forms.Button();
            this.btnArchiwizuj = new System.Windows.Forms.Button();
            this.btnZatwierdzPonowna = new System.Windows.Forms.Button();
            this.panelTitle.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxMagazyn.SuspendLayout();
            this.groupBoxHandlowiec.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelPonownaWysylka.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.panelTitle.Controls.Add(this.lblTitle);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(0, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(782, 39);
            this.panelTitle.TabIndex = 18;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(782, 39);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Podsumowanie Zwrotu";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblInvoice);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblAllegroAccount);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.lblOrderDate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblBuyerLogin);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox1.Location = new System.Drawing.Point(12, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(758, 100);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Informacje Ogólne";
            // 
            // lblInvoice
            // 
            this.lblInvoice.AutoSize = true;
            this.lblInvoice.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblInvoice.Location = new System.Drawing.Point(490, 65);
            this.lblInvoice.Name = "lblInvoice";
            this.lblInvoice.Size = new System.Drawing.Size(38, 20);
            this.lblInvoice.TabIndex = 7;
            this.lblInvoice.Text = "Brak";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(395, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 20);
            this.label6.TabIndex = 6;
            this.label6.Text = "Nr Faktury:";
            // 
            // lblAllegroAccount
            // 
            this.lblAllegroAccount.AutoSize = true;
            this.lblAllegroAccount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAllegroAccount.Location = new System.Drawing.Point(130, 65);
            this.lblAllegroAccount.Name = "lblAllegroAccount";
            this.lblAllegroAccount.Size = new System.Drawing.Size(38, 20);
            this.lblAllegroAccount.TabIndex = 5;
            this.lblAllegroAccount.Text = "Brak";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(15, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 20);
            this.label11.TabIndex = 4;
            this.label11.Text = "Konto Allegro:";
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblOrderDate.Location = new System.Drawing.Point(490, 37);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(38, 20);
            this.lblOrderDate.TabIndex = 3;
            this.lblOrderDate.Text = "Brak";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(395, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Data utwo.:";
            // 
            // lblBuyerLogin
            // 
            this.lblBuyerLogin.AutoSize = true;
            this.lblBuyerLogin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblBuyerLogin.Location = new System.Drawing.Point(130, 37);
            this.lblBuyerLogin.Name = "lblBuyerLogin";
            this.lblBuyerLogin.Size = new System.Drawing.Size(38, 20);
            this.lblBuyerLogin.TabIndex = 1;
            this.lblBuyerLogin.Text = "Brak";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(15, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kupuj¹cy:";
            // 
            // groupBoxMagazyn
            // 
            this.groupBoxMagazyn.Controls.Add(this.lblDataPrzyjecia);
            this.groupBoxMagazyn.Controls.Add(this.label10);
            this.groupBoxMagazyn.Controls.Add(this.lblPrzyjetyPrzez);
            this.groupBoxMagazyn.Controls.Add(this.label14);
            this.groupBoxMagazyn.Controls.Add(this.lblUwagiMagazynu);
            this.groupBoxMagazyn.Controls.Add(this.label17);
            this.groupBoxMagazyn.Controls.Add(this.lblStanProduktu);
            this.groupBoxMagazyn.Controls.Add(this.label19);
            this.groupBoxMagazyn.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupBoxMagazyn.Location = new System.Drawing.Point(12, 160);
            this.groupBoxMagazyn.Name = "groupBoxMagazyn";
            this.groupBoxMagazyn.Size = new System.Drawing.Size(758, 120);
            this.groupBoxMagazyn.TabIndex = 20;
            this.groupBoxMagazyn.TabStop = false;
            this.groupBoxMagazyn.Text = "Ocena Magazynu";
            // 
            // lblDataPrzyjecia
            // 
            this.lblDataPrzyjecia.AutoSize = true;
            this.lblDataPrzyjecia.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDataPrzyjecia.Location = new System.Drawing.Point(490, 61);
            this.lblDataPrzyjecia.Name = "lblDataPrzyjecia";
            this.lblDataPrzyjecia.Size = new System.Drawing.Size(38, 20);
            this.lblDataPrzyjecia.TabIndex = 7;
            this.lblDataPrzyjecia.Text = "Brak";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(395, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 20);
            this.label10.TabIndex = 6;
            this.label10.Text = "Data przy.:";
            // 
            // lblPrzyjetyPrzez
            // 
            this.lblPrzyjetyPrzez.AutoSize = true;
            this.lblPrzyjetyPrzez.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblPrzyjetyPrzez.Location = new System.Drawing.Point(130, 61);
            this.lblPrzyjetyPrzez.Name = "lblPrzyjetyPrzez";
            this.lblPrzyjetyPrzez.Size = new System.Drawing.Size(38, 20);
            this.lblPrzyjetyPrzez.TabIndex = 5;
            this.lblPrzyjetyPrzez.Text = "Brak";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(15, 61);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(110, 20);
            this.label14.TabIndex = 4;
            this.label14.Text = "Przyjêty przez:";
            // 
            // lblUwagiMagazynu
            // 
            this.lblUwagiMagazynu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUwagiMagazynu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblUwagiMagazynu.Location = new System.Drawing.Point(490, 27);
            this.lblUwagiMagazynu.Name = "lblUwagiMagazynu";
            this.lblUwagiMagazynu.Size = new System.Drawing.Size(262, 20);
            this.lblUwagiMagazynu.TabIndex = 3;
            this.lblUwagiMagazynu.Text = "Brak";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(395, 27);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(57, 20);
            this.label17.TabIndex = 2;
            this.label17.Text = "Uwagi:";
            // 
            // lblStanProduktu
            // 
            this.lblStanProduktu.AutoSize = true;
            this.lblStanProduktu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblStanProduktu.Location = new System.Drawing.Point(130, 27);
            this.lblStanProduktu.Name = "lblStanProduktu";
            this.lblStanProduktu.Size = new System.Drawing.Size(38, 20);
            this.lblStanProduktu.TabIndex = 1;
            this.lblStanProduktu.Text = "Brak";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label19.Location = new System.Drawing.Point(15, 27);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(44, 20);
            this.label19.TabIndex = 0;
            this.label19.Text = "Stan:";
            // 
            // groupBoxHandlowiec
            // 
            this.groupBoxHandlowiec.Controls.Add(this.lblKomentarzHandlowca);
            this.groupBoxHandlowiec.Controls.Add(this.label8);
            this.groupBoxHandlowiec.Controls.Add(this.lblDecyzjaHandlowca);
            this.groupBoxHandlowiec.Controls.Add(this.label15);
            this.groupBoxHandlowiec.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.groupBoxHandlowiec.Location = new System.Drawing.Point(12, 286);
            this.groupBoxHandlowiec.Name = "groupBoxHandlowiec";
            this.groupBoxHandlowiec.Size = new System.Drawing.Size(758, 120);
            this.groupBoxHandlowiec.TabIndex = 21;
            this.groupBoxHandlowiec.TabStop = false;
            this.groupBoxHandlowiec.Text = "Decyzja Handlowca";
            // 
            // lblKomentarzHandlowca
            // 
            this.lblKomentarzHandlowca.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKomentarzHandlowca.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblKomentarzHandlowca.Location = new System.Drawing.Point(130, 61);
            this.lblKomentarzHandlowca.Name = "lblKomentarzHandlowca";
            this.lblKomentarzHandlowca.Size = new System.Drawing.Size(622, 45);
            this.lblKomentarzHandlowca.TabIndex = 3;
            this.lblKomentarzHandlowca.Text = "Brak";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(15, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 20);
            this.label8.TabIndex = 2;
            this.label8.Text = "Komentarz:";
            // 
            // lblDecyzjaHandlowca
            // 
            this.lblDecyzjaHandlowca.AutoSize = true;
            this.lblDecyzjaHandlowca.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDecyzjaHandlowca.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblDecyzjaHandlowca.Location = new System.Drawing.Point(129, 27);
            this.lblDecyzjaHandlowca.Name = "lblDecyzjaHandlowca";
            this.lblDecyzjaHandlowca.Size = new System.Drawing.Size(47, 23);
            this.lblDecyzjaHandlowca.TabIndex = 1;
            this.lblDecyzjaHandlowca.Text = "Brak";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(15, 27);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(66, 20);
            this.label15.TabIndex = 0;
            this.label15.Text = "Decyzja:";
            // 
            // panelActions
            // 
            this.panelActions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelActions.Controls.Add(this.btnZatwierdzPonowna);
            this.panelActions.Controls.Add(this.btnNaPolke);
            this.panelActions.Controls.Add(this.btnPrzekazanoDoReklamacji);
            this.panelActions.Controls.Add(this.btnArchiwizuj);
            this.panelActions.Controls.Add(this.panelPonownaWysylka);
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelActions.Location = new System.Drawing.Point(0, 423);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(782, 140);
            this.panelActions.TabIndex = 22;
            // 
            // panelPonownaWysylka
            // 
            this.panelPonownaWysylka.Controls.Add(this.lblPonownaNumerListu);
            this.panelPonownaWysylka.Controls.Add(this.txtNumerListu);
            this.panelPonownaWysylka.Controls.Add(this.lblPonownaPrzewoznik);
            this.panelPonownaWysylka.Controls.Add(this.comboCarrier);
            this.panelPonownaWysylka.Controls.Add(this.lblPonownaData);
            this.panelPonownaWysylka.Controls.Add(this.dtpPonownaData);
            this.panelPonownaWysylka.Location = new System.Drawing.Point(12, 10);
            this.panelPonownaWysylka.Name = "panelPonownaWysylka";
            this.panelPonownaWysylka.Size = new System.Drawing.Size(520, 120);
            this.panelPonownaWysylka.TabIndex = 4;
            this.panelPonownaWysylka.Visible = false;
            // 
            // lblPonownaNumerListu
            // 
            this.lblPonownaNumerListu.AutoSize = true;
            this.lblPonownaNumerListu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPonownaNumerListu.Location = new System.Drawing.Point(0, 77);
            this.lblPonownaNumerListu.Name = "lblPonownaNumerListu";
            this.lblPonownaNumerListu.Size = new System.Drawing.Size(95, 20);
            this.lblPonownaNumerListu.TabIndex = 5;
            this.lblPonownaNumerListu.Text = "Numer listu:";
            // 
            // txtNumerListu
            // 
            this.txtNumerListu.Location = new System.Drawing.Point(110, 74);
            this.txtNumerListu.Name = "txtNumerListu";
            this.txtNumerListu.Size = new System.Drawing.Size(220, 22);
            this.txtNumerListu.TabIndex = 2;
            // 
            // lblPonownaPrzewoznik
            // 
            this.lblPonownaPrzewoznik.AutoSize = true;
            this.lblPonownaPrzewoznik.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPonownaPrzewoznik.Location = new System.Drawing.Point(0, 41);
            this.lblPonownaPrzewoznik.Name = "lblPonownaPrzewoznik";
            this.lblPonownaPrzewoznik.Size = new System.Drawing.Size(89, 20);
            this.lblPonownaPrzewoznik.TabIndex = 3;
            this.lblPonownaPrzewoznik.Text = "Przewonik:";
            // 
            // comboCarrier
            // 
            this.comboCarrier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.comboCarrier.FormattingEnabled = true;
            this.comboCarrier.Location = new System.Drawing.Point(110, 38);
            this.comboCarrier.Name = "comboCarrier";
            this.comboCarrier.Size = new System.Drawing.Size(220, 24);
            this.comboCarrier.TabIndex = 1;
            // 
            // lblPonownaData
            // 
            this.lblPonownaData.AutoSize = true;
            this.lblPonownaData.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPonownaData.Location = new System.Drawing.Point(0, 6);
            this.lblPonownaData.Name = "lblPonownaData";
            this.lblPonownaData.Size = new System.Drawing.Size(97, 20);
            this.lblPonownaData.TabIndex = 1;
            this.lblPonownaData.Text = "Data wysyki:";
            // 
            // dtpPonownaData
            // 
            this.dtpPonownaData.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPonownaData.Location = new System.Drawing.Point(110, 3);
            this.dtpPonownaData.Name = "dtpPonownaData";
            this.dtpPonownaData.Size = new System.Drawing.Size(140, 22);
            this.dtpPonownaData.TabIndex = 0;
            // 
            // btnPrzekazanoDoReklamacji
            // 
            this.btnPrzekazanoDoReklamacji.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrzekazanoDoReklamacji.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnPrzekazanoDoReklamacji.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrzekazanoDoReklamacji.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrzekazanoDoReklamacji.ForeColor = System.Drawing.Color.White;
            this.btnPrzekazanoDoReklamacji.Location = new System.Drawing.Point(540, 10);
            this.btnPrzekazanoDoReklamacji.Name = "btnPrzekazanoDoReklamacji";
            this.btnPrzekazanoDoReklamacji.Size = new System.Drawing.Size(230, 40);
            this.btnPrzekazanoDoReklamacji.TabIndex = 1;
            this.btnPrzekazanoDoReklamacji.Text = "Przekazano fizycznie na reklamacje";
            this.btnPrzekazanoDoReklamacji.UseVisualStyleBackColor = false;
            this.btnPrzekazanoDoReklamacji.Visible = false;
            this.btnPrzekazanoDoReklamacji.Click += new System.EventHandler(this.btnPrzekazanoDoReklamacji_Click);
            // 
            // btnNaPolke
            // 
            this.btnNaPolke.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNaPolke.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnNaPolke.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNaPolke.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNaPolke.ForeColor = System.Drawing.Color.White;
            this.btnNaPolke.Location = new System.Drawing.Point(540, 10);
            this.btnNaPolke.Name = "btnNaPolke";
            this.btnNaPolke.Size = new System.Drawing.Size(230, 40);
            this.btnNaPolke.TabIndex = 2;
            this.btnNaPolke.Text = "Odoone na stan magazynowy";
            this.btnNaPolke.UseVisualStyleBackColor = false;
            this.btnNaPolke.Visible = false;
            this.btnNaPolke.Click += new System.EventHandler(this.btnNaPolke_Click);
            // 
            // btnZatwierdzPonowna
            // 
            this.btnZatwierdzPonowna.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZatwierdzPonowna.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnZatwierdzPonowna.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZatwierdzPonowna.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZatwierdzPonowna.ForeColor = System.Drawing.Color.White;
            this.btnZatwierdzPonowna.Location = new System.Drawing.Point(540, 70);
            this.btnZatwierdzPonowna.Name = "btnZatwierdzPonowna";
            this.btnZatwierdzPonowna.Size = new System.Drawing.Size(230, 40);
            this.btnZatwierdzPonowna.TabIndex = 3;
            this.btnZatwierdzPonowna.Text = "Zatwierd ponown wysyk";
            this.btnZatwierdzPonowna.UseVisualStyleBackColor = false;
            this.btnZatwierdzPonowna.Visible = false;
            this.btnZatwierdzPonowna.Click += new System.EventHandler(this.btnZatwierdzPonowna_Click);
            // 
            // btnArchiwizuj
            // 
            this.btnArchiwizuj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArchiwizuj.BackColor = System.Drawing.Color.ForestGreen;
            this.btnArchiwizuj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnArchiwizuj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnArchiwizuj.ForeColor = System.Drawing.Color.White;
            this.btnArchiwizuj.Location = new System.Drawing.Point(540, 10);
            this.btnArchiwizuj.Name = "btnArchiwizuj";
            this.btnArchiwizuj.Size = new System.Drawing.Size(230, 40);
            this.btnArchiwizuj.TabIndex = 0;
            this.btnArchiwizuj.Text = "Zwrot zakoczony zgodnie z decyzj";
            this.btnArchiwizuj.UseVisualStyleBackColor = false;
            this.btnArchiwizuj.Click += new System.EventHandler(this.btnArchiwizuj_Click);
            // 
            // FormPodsumowanieZwrotu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(782, 560);
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.groupBoxHandlowiec);
            this.Controls.Add(this.groupBoxMagazyn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelTitle);
            this.MinimumSize = new System.Drawing.Size(800, 610);
            this.Name = "FormPodsumowanieZwrotu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Podsumowanie Zwrotu";
            this.Load += new System.EventHandler(this.FormPodsumowanieZwrotu_Load);
            this.panelTitle.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxMagazyn.ResumeLayout(false);
            this.groupBoxMagazyn.PerformLayout();
            this.groupBoxHandlowiec.ResumeLayout(false);
            this.groupBoxHandlowiec.PerformLayout();
            this.panelActions.ResumeLayout(false);
            this.panelPonownaWysylka.ResumeLayout(false);
            this.panelPonownaWysylka.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Panel panelTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblInvoice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblAllegroAccount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblBuyerLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxMagazyn;
        private System.Windows.Forms.Label lblDataPrzyjecia;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblPrzyjetyPrzez;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblUwagiMagazynu;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblStanProduktu;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBoxHandlowiec;
        private System.Windows.Forms.Label lblKomentarzHandlowca;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblDecyzjaHandlowca;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.Panel panelPonownaWysylka;
        private System.Windows.Forms.Label lblPonownaNumerListu;
        private System.Windows.Forms.TextBox txtNumerListu;
        private System.Windows.Forms.Label lblPonownaPrzewoznik;
        private System.Windows.Forms.ComboBox comboCarrier;
        private System.Windows.Forms.Label lblPonownaData;
        private System.Windows.Forms.DateTimePicker dtpPonownaData;
        private System.Windows.Forms.Button btnPrzekazanoDoReklamacji;
        private System.Windows.Forms.Button btnNaPolke;
        private System.Windows.Forms.Button btnArchiwizuj;
        private System.Windows.Forms.Button btnZatwierdzPonowna;
    }
}