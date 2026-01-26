// Plik: FormHandlowiecSzczegoly.Designer.cs (WERSJA Z POPRAWKĄ BŁĘDÓW CS1503)
// Opis: Naprawiono błąd kompilacji związany z nieprawidłowym
//       konstruktorem czcionki dla etykiety 'lblStanProduktu'.

namespace Reklamacje_Dane
{
    partial class FormHandlowiecSzczegoly
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
            this.panelTopHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelBottomActions = new System.Windows.Forms.Panel();
            this.btnWyslijDecyzje = new System.Windows.Forms.Button();
            this.btnPrzekazDoReklamacji = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.cardPanelHistoria = new Reklamacje_Dane.CardPanel();
            this.btnDodajDzialanie = new System.Windows.Forms.Button();
            this.txtNoweDzialanie = new System.Windows.Forms.TextBox();
            this.dgvDzialania = new System.Windows.Forms.DataGridView();
            this.lblCardHistoriaTitle = new System.Windows.Forms.Label();
            this.cardPanelDecyzja = new Reklamacje_Dane.CardPanel();
            this.btnOdrzucZwrot = new System.Windows.Forms.Button();
            this.btnZwrotWplaty = new System.Windows.Forms.Button();
            this.btnZarzadzajStatusami = new System.Windows.Forms.Button();
            this.txtKomentarzHandlowca = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboDecyzja = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCardDecyzjaTitle = new System.Windows.Forms.Label();
            this.cardPanelOcenaMagazynu = new Reklamacje_Dane.CardPanel();
            this.lblDataPrzyjecia = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblPrzyjetyPrzez = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblUwagiMagazynu = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblStanProduktu = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblCardOcenaMagazynuTitle = new System.Windows.Forms.Label();
            this.cardPanelDane = new Reklamacje_Dane.CardPanel();
            this.lblInvoice = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblAllegroAccount = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblBuyerLogin = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblProductName = new System.Windows.Forms.Label();
            this.lblCardDaneTitle = new System.Windows.Forms.Label();
            this.panelTopHeader.SuspendLayout();
            this.panelBottomActions.SuspendLayout();
            this.panelMainContainer.SuspendLayout();
            this.cardPanelHistoria.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDzialania)).BeginInit();
            this.cardPanelDecyzja.SuspendLayout();
            this.cardPanelOcenaMagazynu.SuspendLayout();
            this.cardPanelDane.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTopHeader
            // 
            this.panelTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.panelTopHeader.Controls.Add(this.lblTitle);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(0, 0);
            this.panelTopHeader.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(984, 60);
            this.panelTopHeader.TabIndex = 13;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(298, 31);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Decyzja dla zwrotu: ZWR...";
            // 
            // panelBottomActions
            // 
            this.panelBottomActions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBottomActions.Controls.Add(this.btnWyslijDecyzje);
            this.panelBottomActions.Controls.Add(this.btnPrzekazDoReklamacji);
            this.panelBottomActions.Controls.Add(this.btnAnuluj);
            this.panelBottomActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottomActions.Location = new System.Drawing.Point(0, 908);
            this.panelBottomActions.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelBottomActions.Name = "panelBottomActions";
            this.panelBottomActions.Size = new System.Drawing.Size(984, 70);
            this.panelBottomActions.TabIndex = 14;
            // 
            // btnWyslijDecyzje
            // 
            this.btnWyslijDecyzje.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWyslijDecyzje.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnWyslijDecyzje.FlatAppearance.BorderSize = 0;
            this.btnWyslijDecyzje.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWyslijDecyzje.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnWyslijDecyzje.ForeColor = System.Drawing.Color.White;
            this.btnWyslijDecyzje.Location = new System.Drawing.Point(823, 15);
            this.btnWyslijDecyzje.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnWyslijDecyzje.Name = "btnWyslijDecyzje";
            this.btnWyslijDecyzje.Size = new System.Drawing.Size(149, 39);
            this.btnWyslijDecyzje.TabIndex = 11;
            this.btnWyslijDecyzje.Text = "Zatwierdź decyzję";
            this.btnWyslijDecyzje.UseVisualStyleBackColor = false;
            this.btnWyslijDecyzje.Click += new System.EventHandler(this.btnWyslijDecyzje_Click);
            // 
            // btnPrzekazDoReklamacji
            // 
            this.btnPrzekazDoReklamacji.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrzekazDoReklamacji.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnPrzekazDoReklamacji.FlatAppearance.BorderSize = 0;
            this.btnPrzekazDoReklamacji.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrzekazDoReklamacji.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnPrzekazDoReklamacji.ForeColor = System.Drawing.Color.White;
            this.btnPrzekazDoReklamacji.Location = new System.Drawing.Point(606, 15);
            this.btnPrzekazDoReklamacji.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnPrzekazDoReklamacji.Name = "btnPrzekazDoReklamacji";
            this.btnPrzekazDoReklamacji.Size = new System.Drawing.Size(211, 39);
            this.btnPrzekazDoReklamacji.TabIndex = 12;
            this.btnPrzekazDoReklamacji.Text = "Przekaż na reklamacje";
            this.btnPrzekazDoReklamacji.UseVisualStyleBackColor = false;
            this.btnPrzekazDoReklamacji.Click += new System.EventHandler(this.btnPrzekazDoReklamacji_Click);
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnuluj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnuluj.FlatAppearance.BorderSize = 0;
            this.btnAnuluj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnuluj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAnuluj.ForeColor = System.Drawing.Color.Black;
            this.btnAnuluj.Location = new System.Drawing.Point(480, 15);
            this.btnAnuluj.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(120, 39);
            this.btnAnuluj.TabIndex = 10;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = false;
            // 
            // panelMainContainer
            // 
            this.panelMainContainer.AutoScroll = true;
            this.panelMainContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.panelMainContainer.Controls.Add(this.cardPanelHistoria);
            this.panelMainContainer.Controls.Add(this.cardPanelDecyzja);
            this.panelMainContainer.Controls.Add(this.cardPanelOcenaMagazynu);
            this.panelMainContainer.Controls.Add(this.cardPanelDane);
            this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContainer.Location = new System.Drawing.Point(0, 60);
            this.panelMainContainer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.panelMainContainer.Size = new System.Drawing.Size(984, 848);
            this.panelMainContainer.TabIndex = 15;
            // 
            // cardPanelHistoria
            // 
            this.cardPanelHistoria.BackColor = System.Drawing.Color.White;
            this.cardPanelHistoria.BorderRadius = 5;
            this.cardPanelHistoria.Controls.Add(this.btnDodajDzialanie);
            this.cardPanelHistoria.Controls.Add(this.txtNoweDzialanie);
            this.cardPanelHistoria.Controls.Add(this.dgvDzialania);
            this.cardPanelHistoria.Controls.Add(this.lblCardHistoriaTitle);
            this.cardPanelHistoria.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelHistoria.Location = new System.Drawing.Point(11, 583);
            this.cardPanelHistoria.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cardPanelHistoria.Name = "cardPanelHistoria";
            this.cardPanelHistoria.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelHistoria.Size = new System.Drawing.Size(962, 265);
            this.cardPanelHistoria.TabIndex = 3;
            // 
            // btnDodajDzialanie
            // 
            this.btnDodajDzialanie.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDodajDzialanie.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDodajDzialanie.Location = new System.Drawing.Point(835, 219);
            this.btnDodajDzialanie.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDodajDzialanie.Name = "btnDodajDzialanie";
            this.btnDodajDzialanie.Size = new System.Drawing.Size(109, 30);
            this.btnDodajDzialanie.TabIndex = 3;
            this.btnDodajDzialanie.Text = "Dodaj wpis";
            this.btnDodajDzialanie.UseVisualStyleBackColor = true;
            this.btnDodajDzialanie.Click += new System.EventHandler(this.btnDodajDzialanie_Click);
            // 
            // txtNoweDzialanie
            // 
            this.txtNoweDzialanie.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNoweDzialanie.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNoweDzialanie.Location = new System.Drawing.Point(20, 220);
            this.txtNoweDzialanie.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNoweDzialanie.Name = "txtNoweDzialanie";
            this.txtNoweDzialanie.Size = new System.Drawing.Size(808, 27);
            this.txtNoweDzialanie.TabIndex = 2;
            // 
            // dgvDzialania
            // 
            this.dgvDzialania.AllowUserToAddRows = false;
            this.dgvDzialania.AllowUserToDeleteRows = false;
            this.dgvDzialania.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDzialania.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDzialania.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvDzialania.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvDzialania.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDzialania.Location = new System.Drawing.Point(20, 50);
            this.dgvDzialania.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvDzialania.Name = "dgvDzialania";
            this.dgvDzialania.ReadOnly = true;
            this.dgvDzialania.RowHeadersVisible = false;
            this.dgvDzialania.RowHeadersWidth = 51;
            this.dgvDzialania.RowTemplate.Height = 24;
            this.dgvDzialania.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDzialania.Size = new System.Drawing.Size(924, 150);
            this.dgvDzialania.TabIndex = 1;
            // 
            // lblCardHistoriaTitle
            // 
            this.lblCardHistoriaTitle.AutoSize = true;
            this.lblCardHistoriaTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardHistoriaTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardHistoriaTitle.Location = new System.Drawing.Point(15, 14);
            this.lblCardHistoriaTitle.Name = "lblCardHistoriaTitle";
            this.lblCardHistoriaTitle.Size = new System.Drawing.Size(148, 25);
            this.lblCardHistoriaTitle.TabIndex = 0;
            this.lblCardHistoriaTitle.Text = "Historia Działań";
            // 
            // cardPanelDecyzja
            // 
            this.cardPanelDecyzja.BackColor = System.Drawing.Color.White;
            this.cardPanelDecyzja.BorderRadius = 5;
            this.cardPanelDecyzja.Controls.Add(this.btnOdrzucZwrot);
            this.cardPanelDecyzja.Controls.Add(this.btnZwrotWplaty);
            this.cardPanelDecyzja.Controls.Add(this.btnZarzadzajStatusami);
            this.cardPanelDecyzja.Controls.Add(this.txtKomentarzHandlowca);
            this.cardPanelDecyzja.Controls.Add(this.label3);
            this.cardPanelDecyzja.Controls.Add(this.comboDecyzja);
            this.cardPanelDecyzja.Controls.Add(this.label1);
            this.cardPanelDecyzja.Controls.Add(this.lblCardDecyzjaTitle);
            this.cardPanelDecyzja.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelDecyzja.Location = new System.Drawing.Point(11, 315);
            this.cardPanelDecyzja.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cardPanelDecyzja.Name = "cardPanelDecyzja";
            this.cardPanelDecyzja.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelDecyzja.Size = new System.Drawing.Size(962, 268);
            this.cardPanelDecyzja.TabIndex = 2;
            // 
            // btnOdrzucZwrot
            // 
            this.btnOdrzucZwrot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOdrzucZwrot.BackColor = System.Drawing.Color.Firebrick;
            this.btnOdrzucZwrot.FlatAppearance.BorderSize = 0;
            this.btnOdrzucZwrot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOdrzucZwrot.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnOdrzucZwrot.ForeColor = System.Drawing.Color.White;
            this.btnOdrzucZwrot.Location = new System.Drawing.Point(724, 206);
            this.btnOdrzucZwrot.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOdrzucZwrot.Name = "btnOdrzucZwrot";
            this.btnOdrzucZwrot.Size = new System.Drawing.Size(220, 39);
            this.btnOdrzucZwrot.TabIndex = 7;
            this.btnOdrzucZwrot.Text = "Odrzuć w API";
            this.btnOdrzucZwrot.UseVisualStyleBackColor = false;
            this.btnOdrzucZwrot.Click += new System.EventHandler(this.btnOdrzucZwrot_Click);
            // 
            // btnZwrotWplaty
            // 
            this.btnZwrotWplaty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZwrotWplaty.BackColor = System.Drawing.Color.DarkOrange;
            this.btnZwrotWplaty.FlatAppearance.BorderSize = 0;
            this.btnZwrotWplaty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZwrotWplaty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZwrotWplaty.ForeColor = System.Drawing.Color.White;
            this.btnZwrotWplaty.Location = new System.Drawing.Point(490, 206);
            this.btnZwrotWplaty.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnZwrotWplaty.Name = "btnZwrotWplaty";
            this.btnZwrotWplaty.Size = new System.Drawing.Size(220, 39);
            this.btnZwrotWplaty.TabIndex = 6;
            this.btnZwrotWplaty.Text = "Zwróć wpłatę w API";
            this.btnZwrotWplaty.UseVisualStyleBackColor = false;
            this.btnZwrotWplaty.Click += new System.EventHandler(this.btnZwrotWplaty_Click);
            // 
            // btnZarzadzajStatusami
            // 
            this.btnZarzadzajStatusami.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.btnZarzadzajStatusami.Location = new System.Drawing.Point(267, 60);
            this.btnZarzadzajStatusami.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnZarzadzajStatusami.Name = "btnZarzadzajStatusami";
            this.btnZarzadzajStatusami.Size = new System.Drawing.Size(29, 28);
            this.btnZarzadzajStatusami.TabIndex = 5;
            this.btnZarzadzajStatusami.Text = "...";
            this.btnZarzadzajStatusami.UseVisualStyleBackColor = true;
            this.btnZarzadzajStatusami.Click += new System.EventHandler(this.btnZarzadzajStatusami_Click);
            // 
            // txtKomentarzHandlowca
            // 
            this.txtKomentarzHandlowca.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKomentarzHandlowca.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtKomentarzHandlowca.Location = new System.Drawing.Point(20, 145);
            this.txtKomentarzHandlowca.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtKomentarzHandlowca.Multiline = true;
            this.txtKomentarzHandlowca.Name = "txtKomentarzHandlowca";
            this.txtKomentarzHandlowca.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtKomentarzHandlowca.Size = new System.Drawing.Size(926, 45);
            this.txtKomentarzHandlowca.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(16, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Komentarz (opcjonalny):";
            // 
            // comboDecyzja
            // 
            this.comboDecyzja.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDecyzja.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboDecyzja.FormattingEnabled = true;
            this.comboDecyzja.Location = new System.Drawing.Point(20, 60);
            this.comboDecyzja.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboDecyzja.Name = "comboDecyzja";
            this.comboDecyzja.Size = new System.Drawing.Size(240, 28);
            this.comboDecyzja.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(16, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Twoja decyzja (wymagana):";
            // 
            // lblCardDecyzjaTitle
            // 
            this.lblCardDecyzjaTitle.AutoSize = true;
            this.lblCardDecyzjaTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardDecyzjaTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardDecyzjaTitle.Location = new System.Drawing.Point(15, 12);
            this.lblCardDecyzjaTitle.Name = "lblCardDecyzjaTitle";
            this.lblCardDecyzjaTitle.Size = new System.Drawing.Size(125, 25);
            this.lblCardDecyzjaTitle.TabIndex = 0;
            this.lblCardDecyzjaTitle.Text = "Panel Decyzji";
            // 
            // cardPanelOcenaMagazynu
            // 
            this.cardPanelOcenaMagazynu.BackColor = System.Drawing.Color.White;
            this.cardPanelOcenaMagazynu.BorderRadius = 5;
            this.cardPanelOcenaMagazynu.Controls.Add(this.lblDataPrzyjecia);
            this.cardPanelOcenaMagazynu.Controls.Add(this.label14);
            this.cardPanelOcenaMagazynu.Controls.Add(this.lblPrzyjetyPrzez);
            this.cardPanelOcenaMagazynu.Controls.Add(this.label12);
            this.cardPanelOcenaMagazynu.Controls.Add(this.lblUwagiMagazynu);
            this.cardPanelOcenaMagazynu.Controls.Add(this.label10);
            this.cardPanelOcenaMagazynu.Controls.Add(this.lblStanProduktu);
            this.cardPanelOcenaMagazynu.Controls.Add(this.label2);
            this.cardPanelOcenaMagazynu.Controls.Add(this.lblCardOcenaMagazynuTitle);
            this.cardPanelOcenaMagazynu.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelOcenaMagazynu.Location = new System.Drawing.Point(11, 160);
            this.cardPanelOcenaMagazynu.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cardPanelOcenaMagazynu.Name = "cardPanelOcenaMagazynu";
            this.cardPanelOcenaMagazynu.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelOcenaMagazynu.Size = new System.Drawing.Size(962, 155);
            this.cardPanelOcenaMagazynu.TabIndex = 1;
            // 
            // lblDataPrzyjecia
            // 
            this.lblDataPrzyjecia.AutoSize = true;
            this.lblDataPrzyjecia.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDataPrzyjecia.Location = new System.Drawing.Point(757, 50);
            this.lblDataPrzyjecia.Name = "lblDataPrzyjecia";
            this.lblDataPrzyjecia.Size = new System.Drawing.Size(38, 20);
            this.lblDataPrzyjecia.TabIndex = 8;
            this.lblDataPrzyjecia.Text = "Brak";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(637, 50);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(111, 20);
            this.label14.TabIndex = 7;
            this.label14.Text = "Data przyjęcia:";
            // 
            // lblPrzyjetyPrzez
            // 
            this.lblPrzyjetyPrzez.AutoSize = true;
            this.lblPrzyjetyPrzez.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPrzyjetyPrzez.Location = new System.Drawing.Point(478, 50);
            this.lblPrzyjetyPrzez.Name = "lblPrzyjetyPrzez";
            this.lblPrzyjetyPrzez.Size = new System.Drawing.Size(38, 20);
            this.lblPrzyjetyPrzez.TabIndex = 6;
            this.lblPrzyjetyPrzez.Text = "Brak";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(357, 50);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(110, 20);
            this.label12.TabIndex = 5;
            this.label12.Text = "Przyjęty przez:";
            // 
            // lblUwagiMagazynu
            // 
            this.lblUwagiMagazynu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUwagiMagazynu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUwagiMagazynu.Location = new System.Drawing.Point(16, 105);
            this.lblUwagiMagazynu.Name = "lblUwagiMagazynu";
            this.lblUwagiMagazynu.Size = new System.Drawing.Size(930, 39);
            this.lblUwagiMagazynu.TabIndex = 4;
            this.lblUwagiMagazynu.Text = "Brak";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(16, 80);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(133, 20);
            this.label10.TabIndex = 3;
            this.label10.Text = "Uwagi magazynu:";
            // 
            // lblStanProduktu
            // 
            this.lblStanProduktu.AutoSize = true;
            this.lblStanProduktu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStanProduktu.ForeColor = System.Drawing.Color.Firebrick;
            this.lblStanProduktu.Location = new System.Drawing.Point(136, 50);
            this.lblStanProduktu.Name = "lblStanProduktu";
            this.lblStanProduktu.Size = new System.Drawing.Size(41, 20);
            this.lblStanProduktu.TabIndex = 2;
            this.lblStanProduktu.Text = "Brak";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(16, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Stan produktu:";
            // 
            // lblCardOcenaMagazynuTitle
            // 
            this.lblCardOcenaMagazynuTitle.AutoSize = true;
            this.lblCardOcenaMagazynuTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardOcenaMagazynuTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardOcenaMagazynuTitle.Location = new System.Drawing.Point(15, 14);
            this.lblCardOcenaMagazynuTitle.Name = "lblCardOcenaMagazynuTitle";
            this.lblCardOcenaMagazynuTitle.Size = new System.Drawing.Size(160, 25);
            this.lblCardOcenaMagazynuTitle.TabIndex = 0;
            this.lblCardOcenaMagazynuTitle.Text = "Ocena Magazynu";
            // 
            // cardPanelDane
            // 
            this.cardPanelDane.BackColor = System.Drawing.Color.White;
            this.cardPanelDane.BorderRadius = 5;
            this.cardPanelDane.Controls.Add(this.lblInvoice);
            this.cardPanelDane.Controls.Add(this.label11);
            this.cardPanelDane.Controls.Add(this.lblOrderDate);
            this.cardPanelDane.Controls.Add(this.label9);
            this.cardPanelDane.Controls.Add(this.lblAllegroAccount);
            this.cardPanelDane.Controls.Add(this.label7);
            this.cardPanelDane.Controls.Add(this.lblBuyerLogin);
            this.cardPanelDane.Controls.Add(this.label6);
            this.cardPanelDane.Controls.Add(this.lblProductName);
            this.cardPanelDane.Controls.Add(this.lblCardDaneTitle);
            this.cardPanelDane.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelDane.Location = new System.Drawing.Point(11, 10);
            this.cardPanelDane.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cardPanelDane.Name = "cardPanelDane";
            this.cardPanelDane.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelDane.Size = new System.Drawing.Size(962, 150);
            this.cardPanelDane.TabIndex = 0;
            // 
            // lblInvoice
            // 
            this.lblInvoice.AutoSize = true;
            this.lblInvoice.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInvoice.Location = new System.Drawing.Point(498, 114);
            this.lblInvoice.Name = "lblInvoice";
            this.lblInvoice.Size = new System.Drawing.Size(38, 20);
            this.lblInvoice.TabIndex = 10;
            this.lblInvoice.Text = "Brak";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(357, 114);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 20);
            this.label11.TabIndex = 9;
            this.label11.Text = "Nr Faktury:";
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblOrderDate.Location = new System.Drawing.Point(498, 85);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(38, 20);
            this.lblOrderDate.TabIndex = 8;
            this.lblOrderDate.Text = "Brak";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(357, 85);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(128, 20);
            this.label9.TabIndex = 7;
            this.label9.Text = "Data utworzenia:";
            // 
            // lblAllegroAccount
            // 
            this.lblAllegroAccount.AutoSize = true;
            this.lblAllegroAccount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAllegroAccount.Location = new System.Drawing.Point(136, 114);
            this.lblAllegroAccount.Name = "lblAllegroAccount";
            this.lblAllegroAccount.Size = new System.Drawing.Size(38, 20);
            this.lblAllegroAccount.TabIndex = 6;
            this.lblAllegroAccount.Text = "Brak";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(16, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(111, 20);
            this.label7.TabIndex = 5;
            this.label7.Text = "Konto Allegro:";
            // 
            // lblBuyerLogin
            // 
            this.lblBuyerLogin.AutoSize = true;
            this.lblBuyerLogin.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBuyerLogin.Location = new System.Drawing.Point(136, 85);
            this.lblBuyerLogin.Name = "lblBuyerLogin";
            this.lblBuyerLogin.Size = new System.Drawing.Size(38, 20);
            this.lblBuyerLogin.TabIndex = 4;
            this.lblBuyerLogin.Text = "Brak";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(16, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "Klient:";
            // 
            // lblProductName
            // 
            this.lblProductName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProductName.AutoEllipsis = true;
            this.lblProductName.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblProductName.Location = new System.Drawing.Point(16, 50);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(930, 23);
            this.lblProductName.TabIndex = 2;
            this.lblProductName.Text = "Nazwa Produktu...";
            // 
            // lblCardDaneTitle
            // 
            this.lblCardDaneTitle.AutoSize = true;
            this.lblCardDaneTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardDaneTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardDaneTitle.Location = new System.Drawing.Point(15, 14);
            this.lblCardDaneTitle.Name = "lblCardDaneTitle";
            this.lblCardDaneTitle.Size = new System.Drawing.Size(197, 25);
            this.lblCardDaneTitle.TabIndex = 0;
            this.lblCardDaneTitle.Text = "Dane Klienta i Zwrotu";
            // 
            // FormHandlowiecSzczegoly
            // 
            this.AcceptButton = this.btnWyslijDecyzje;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnuluj;
            this.ClientSize = new System.Drawing.Size(984, 978);
            this.Controls.Add(this.panelMainContainer);
            this.Controls.Add(this.panelBottomActions);
            this.Controls.Add(this.panelTopHeader);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(999, 798);
            this.Name = "FormHandlowiecSzczegoly";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Decyzja dla zwrotu";
            this.Load += new System.EventHandler(this.FormHandlowiecSzczegoly_Load);
            this.panelTopHeader.ResumeLayout(false);
            this.panelTopHeader.PerformLayout();
            this.panelBottomActions.ResumeLayout(false);
            this.panelMainContainer.ResumeLayout(false);
            this.cardPanelHistoria.ResumeLayout(false);
            this.cardPanelHistoria.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDzialania)).EndInit();
            this.cardPanelDecyzja.ResumeLayout(false);
            this.cardPanelDecyzja.PerformLayout();
            this.cardPanelOcenaMagazynu.ResumeLayout(false);
            this.cardPanelOcenaMagazynu.PerformLayout();
            this.cardPanelDane.ResumeLayout(false);
            this.cardPanelDane.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTopHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelBottomActions;
        private System.Windows.Forms.Button btnWyslijDecyzje;
        private System.Windows.Forms.Button btnPrzekazDoReklamacji;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Panel panelMainContainer;
        private CardPanel cardPanelDane;
        private System.Windows.Forms.Label lblCardDaneTitle;
        private CardPanel cardPanelOcenaMagazynu;
        private System.Windows.Forms.Label lblCardOcenaMagazynuTitle;
        private CardPanel cardPanelDecyzja;
        private System.Windows.Forms.Label lblCardDecyzjaTitle;
        private CardPanel cardPanelHistoria;
        private System.Windows.Forms.Label lblCardHistoriaTitle;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblBuyerLogin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblAllegroAccount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblInvoice;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblStanProduktu;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblUwagiMagazynu;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblPrzyjetyPrzez;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblDataPrzyjecia;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboDecyzja;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtKomentarzHandlowca;
        private System.Windows.Forms.Button btnZarzadzajStatusami;
        private System.Windows.Forms.Button btnZwrotWplaty;
        private System.Windows.Forms.Button btnOdrzucZwrot;
        private System.Windows.Forms.DataGridView dgvDzialania;
        private System.Windows.Forms.Button btnDodajDzialanie;
        private System.Windows.Forms.TextBox txtNoweDzialanie;
    }
}
