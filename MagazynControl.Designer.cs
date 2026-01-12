// ############################################################################
// Plik: MagazynControl.Designer.cs (WERSJA OSTATECZNA)
// Opis: Plik projektanta dla kontrolki modułu Magazynu.
//       Układ został poprawiony, aby był responsywny i czytelny.
// ############################################################################

namespace Reklamacje_Dane
{
    partial class MagazynControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnFetchReturns = new System.Windows.Forms.Button();
            this.comboAllegroAccounts = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.refreshIcon = new System.Windows.Forms.PictureBox();
            this.lblLastRefresh = new System.Windows.Forms.Label();
            this.panelFiltry = new System.Windows.Forms.Panel();
            this.panelFiltryButtons = new System.Windows.Forms.Panel();
            this.btnFilterWszystkie = new System.Windows.Forms.Button();
            this.btnFilterWDrodze = new System.Windows.Forms.Button();
            this.btnFilterPoDecyzji = new System.Windows.Forms.Button();
            this.btnFilterOczekujeNaDecyzje = new System.Windows.Forms.Button();
            this.btnFilterOczekujace = new System.Windows.Forms.Button();
            this.comboFilterStatusAllegro = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvReturns = new System.Windows.Forms.DataGridView();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.btnDodajRecznie = new System.Windows.Forms.Button();
            this.txtScannerInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bottomSplitContainer = new System.Windows.Forms.SplitContainer();
            this.panelKomunikator = new System.Windows.Forms.Panel();
            this.dataGridViewChangeLog = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refreshIcon)).BeginInit();
            this.panelFiltry.SuspendLayout();
            this.panelFiltryButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReturns)).BeginInit();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bottomSplitContainer)).BeginInit();
            this.bottomSplitContainer.Panel1.SuspendLayout();
            this.bottomSplitContainer.Panel2.SuspendLayout();
            this.bottomSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChangeLog)).BeginInit();
            this.SuspendLayout();
            // 



            // 
            // btnWyslijZwrotyMail
            // 
            this.btnWyslijZwrotyMail = new System.Windows.Forms.Button();
            this.btnWyslijZwrotyMail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWyslijZwrotyMail.AutoSize = true;
            this.btnWyslijZwrotyMail.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnWyslijZwrotyMail.Location = new System.Drawing.Point( /* dopasuj X */ 600, /* dopasuj Y */ 5);
            this.btnWyslijZwrotyMail.Name = "btnWyslijZwrotyMail";
            this.btnWyslijZwrotyMail.Size = new System.Drawing.Size(190, 26);
            this.btnWyslijZwrotyMail.TabIndex = 999;
            this.btnWyslijZwrotyMail.Text = "Wyślij zwroty do decyzji…";
            this.btnWyslijZwrotyMail.UseVisualStyleBackColor = true;
            this.btnWyslijZwrotyMail.Click += new System.EventHandler(this.btnWyslijZwrotyMail_Click);

            // ... jeżeli masz panel górny, dodaj do niego, np.:
            // this.topPanel.Controls.Add(this.btnWyslijZwrotyMail);

            // a jeśli nie – dodaj bezpośrednio do kontrolki:
            this.Controls.Add(this.btnWyslijZwrotyMail);




            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTop.Controls.Add(this.btnFetchReturns);
            this.panelTop.Controls.Add(this.comboAllegroAccounts);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.refreshIcon);
            this.panelTop.Controls.Add(this.lblLastRefresh);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1147, 45);
            this.panelTop.TabIndex = 2;
            // 
            // btnFetchReturns
            // 
            this.btnFetchReturns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnFetchReturns.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFetchReturns.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFetchReturns.ForeColor = System.Drawing.Color.White;
            this.btnFetchReturns.Location = new System.Drawing.Point(400, 5);
            this.btnFetchReturns.Name = "btnFetchReturns";
            this.btnFetchReturns.Size = new System.Drawing.Size(150, 35);
            this.btnFetchReturns.TabIndex = 2;
            this.btnFetchReturns.Text = "Synchronizuj";
            this.btnFetchReturns.UseVisualStyleBackColor = false;
            // 
            // comboAllegroAccounts
            // 
            this.comboAllegroAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAllegroAccounts.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboAllegroAccounts.FormattingEnabled = true;
            this.comboAllegroAccounts.Location = new System.Drawing.Point(130, 8);
            this.comboAllegroAccounts.Name = "comboAllegroAccounts";
            this.comboAllegroAccounts.Size = new System.Drawing.Size(250, 28);
            this.comboAllegroAccounts.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Konto Allegro:";
            // 
            // refreshIcon
            // 
            this.refreshIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.refreshIcon.Location = new System.Drawing.Point(900, 10);
            this.refreshIcon.Name = "refreshIcon";
            this.refreshIcon.Size = new System.Drawing.Size(22, 22);
            this.refreshIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.refreshIcon.TabIndex = 3;
            this.refreshIcon.TabStop = false;
            // 
            // lblLastRefresh
            // 
            this.lblLastRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastRefresh.AutoSize = true;
            this.lblLastRefresh.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLastRefresh.Location = new System.Drawing.Point(930, 12);
            this.lblLastRefresh.Name = "lblLastRefresh";
            this.lblLastRefresh.Size = new System.Drawing.Size(186, 20);
            this.lblLastRefresh.TabIndex = 4;
            this.lblLastRefresh.Text = "Ostatnie odświeżenie: brak";
            // 
            // panelFiltry
            // 
            this.panelFiltry.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelFiltry.Controls.Add(this.panelFiltryButtons);
            this.panelFiltry.Controls.Add(this.comboFilterStatusAllegro);
            this.panelFiltry.Controls.Add(this.label5);
            this.panelFiltry.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltry.Location = new System.Drawing.Point(0, 45);
            this.panelFiltry.Name = "panelFiltry";
            this.panelFiltry.Size = new System.Drawing.Size(1147, 53);
            this.panelFiltry.TabIndex = 4;
            // 
            // panelFiltryButtons
            //
            this.panelFiltryButtons.Controls.Add(this.Wyslijmail);
            this.panelFiltryButtons.Controls.Add(this.btnFilterWszystkie);
            this.panelFiltryButtons.Controls.Add(this.btnFilterWDrodze);
            this.panelFiltryButtons.Controls.Add(this.btnFilterPoDecyzji);
            this.panelFiltryButtons.Controls.Add(this.btnFilterOczekujeNaDecyzje);
            this.panelFiltryButtons.Controls.Add(this.btnFilterOczekujace);
            this.panelFiltryButtons.Location = new System.Drawing.Point(10, 5);
            this.panelFiltryButtons.Name = "panelFiltryButtons";
            this.panelFiltryButtons.Size = new System.Drawing.Size(850, 40);
            this.panelFiltryButtons.TabIndex = 5;
            //
            // btnFilterWszystkie
           
            // 
            this.btnFilterWszystkie.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFilterWszystkie.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterWszystkie.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnFilterWszystkie.Name = "btnFilterWszystkie";
            this.btnFilterWszystkie.Size = new System.Drawing.Size(150, 40);
            this.btnFilterWszystkie.TabIndex = 5;
            this.btnFilterWszystkie.Text = "Wszystkie";
            this.btnFilterWszystkie.UseVisualStyleBackColor = true;
            // 
            // btnFilterWDrodze
            // 
            this.btnFilterWDrodze.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFilterWDrodze.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterWDrodze.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnFilterWDrodze.Name = "btnFilterWDrodze";
            this.btnFilterWDrodze.Size = new System.Drawing.Size(150, 40);
            this.btnFilterWDrodze.TabIndex = 6;
            this.btnFilterWDrodze.Text = "W drodze do nas";
            this.btnFilterWDrodze.UseVisualStyleBackColor = true;
            // 
            // btnFilterPoDecyzji
            // 
            this.btnFilterPoDecyzji.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFilterPoDecyzji.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterPoDecyzji.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnFilterPoDecyzji.Name = "btnFilterPoDecyzji";
            this.btnFilterPoDecyzji.Size = new System.Drawing.Size(150, 40);
            this.btnFilterPoDecyzji.TabIndex = 2;
            this.btnFilterPoDecyzji.Text = "Po decyzji";
            this.btnFilterPoDecyzji.UseVisualStyleBackColor = true;
            // 
            // btnFilterOczekujeNaDecyzje
            // 
            this.btnFilterOczekujeNaDecyzje.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFilterOczekujeNaDecyzje.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterOczekujeNaDecyzje.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnFilterOczekujeNaDecyzje.Name = "btnFilterOczekujeNaDecyzje";
            this.btnFilterOczekujeNaDecyzje.Size = new System.Drawing.Size(180, 40);
            this.btnFilterOczekujeNaDecyzje.TabIndex = 1;
            this.btnFilterOczekujeNaDecyzje.Text = "Oczekuje na decyzję";
            this.btnFilterOczekujeNaDecyzje.UseVisualStyleBackColor = true;
            // 
            // btnFilterOczekujace
            // 
            this.btnFilterOczekujace.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFilterOczekujace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterOczekujace.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFilterOczekujace.Name = "btnFilterOczekujace";
            this.btnFilterOczekujace.Size = new System.Drawing.Size(180, 40);
            this.btnFilterOczekujace.TabIndex = 0;
            this.btnFilterOczekujace.Text = "Oczekuje na przyjęcie";
            this.btnFilterOczekujace.UseVisualStyleBackColor = true;
            // 
            // comboFilterStatusAllegro
            // 
            this.comboFilterStatusAllegro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboFilterStatusAllegro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFilterStatusAllegro.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboFilterStatusAllegro.FormattingEnabled = true;
            this.comboFilterStatusAllegro.Location = new System.Drawing.Point(957, 13);
            this.comboFilterStatusAllegro.Name = "comboFilterStatusAllegro";
            this.comboFilterStatusAllegro.Size = new System.Drawing.Size(180, 28);
            this.comboFilterStatusAllegro.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.Location = new System.Drawing.Point(846, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Status Allegro:";
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 98);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.dgvReturns);
            this.mainSplitContainer.Panel1.Controls.Add(this.panelSearch);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.bottomSplitContainer);
            this.mainSplitContainer.Size = new System.Drawing.Size(1147, 655);
            this.mainSplitContainer.SplitterDistance = 400;
            this.mainSplitContainer.TabIndex = 5;
            // 
            // dgvReturns
            // 
            this.dgvReturns.AllowUserToAddRows = false;
            this.dgvReturns.AllowUserToDeleteRows = false;
            this.dgvReturns.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dgvReturns.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReturns.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvReturns.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvReturns.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReturns.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvReturns.ColumnHeadersHeight = 30;
            this.dgvReturns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReturns.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvReturns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReturns.EnableHeadersVisualStyles = false;
            this.dgvReturns.Location = new System.Drawing.Point(0, 0);
            this.dgvReturns.Name = "dgvReturns";
            this.dgvReturns.ReadOnly = true;
            this.dgvReturns.RowHeadersVisible = false;
            this.dgvReturns.RowTemplate.Height = 28;
            this.dgvReturns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReturns.Size = new System.Drawing.Size(897, 400);
            this.dgvReturns.TabIndex = 3;
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelSearch.Controls.Add(this.lblTotalCount);
            this.panelSearch.Controls.Add(this.btnDodajRecznie);
            this.panelSearch.Controls.Add(this.txtScannerInput);
            this.panelSearch.Controls.Add(this.label3);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.label2);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSearch.Location = new System.Drawing.Point(897, 0);
            this.panelSearch.MinimumSize = new System.Drawing.Size(250, 0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(250, 400);
            this.panelSearch.TabIndex = 2;
            // 
            // lblTotalCount
            // 
            this.lblTotalCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalCount.AutoSize = true;
            this.lblTotalCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTotalCount.Location = new System.Drawing.Point(7, 210);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(170, 20);
            this.lblTotalCount.TabIndex = 5;
            this.lblTotalCount.Text = "Wyświetlono: 0 zwrotów";
            // 
            // btnDodajRecznie
            // 
            this.btnDodajRecznie.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDodajRecznie.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDodajRecznie.Location = new System.Drawing.Point(10, 150);
            this.btnDodajRecznie.Name = "btnDodajRecznie";
            this.btnDodajRecznie.Size = new System.Drawing.Size(230, 35);
            this.btnDodajRecznie.TabIndex = 4;
            this.btnDodajRecznie.Text = "➕ Dodaj zwrot ręcznie";
            this.btnDodajRecznie.UseVisualStyleBackColor = true;
            // 
            // txtScannerInput
            // 
            this.txtScannerInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScannerInput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtScannerInput.Location = new System.Drawing.Point(10, 100);
            this.txtScannerInput.Name = "txtScannerInput";
            this.txtScannerInput.Size = new System.Drawing.Size(230, 27);
            this.txtScannerInput.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(7, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Skaner (nr listu):";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.Location = new System.Drawing.Point(10, 40);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(230, 27);
            this.txtSearch.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(7, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Wyszukaj zwrot:";
            // 
            // bottomSplitContainer
            // 
            this.bottomSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.bottomSplitContainer.Name = "bottomSplitContainer";
            // 
            // bottomSplitContainer.Panel1
            // 
            this.bottomSplitContainer.Panel1.Controls.Add(this.panelKomunikator);
            // 
            // bottomSplitContainer.Panel2
            // 
            this.bottomSplitContainer.Panel2.Controls.Add(this.dataGridViewChangeLog);
            this.bottomSplitContainer.Size = new System.Drawing.Size(1147, 251);
            this.bottomSplitContainer.SplitterDistance = 570;
            this.bottomSplitContainer.TabIndex = 0;
            // 
            // panelKomunikator
            // 
            this.panelKomunikator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelKomunikator.Location = new System.Drawing.Point(0, 0);
            this.panelKomunikator.Name = "panelKomunikator";
            this.panelKomunikator.Size = new System.Drawing.Size(570, 251);
            this.panelKomunikator.TabIndex = 0;
            // 
            // dataGridViewChangeLog
            // 
            this.dataGridViewChangeLog.AllowUserToAddRows = false;
            this.dataGridViewChangeLog.AllowUserToDeleteRows = false;
            this.dataGridViewChangeLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewChangeLog.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewChangeLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewChangeLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewChangeLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewChangeLog.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewChangeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewChangeLog.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewChangeLog.Name = "dataGridViewChangeLog";
            this.dataGridViewChangeLog.ReadOnly = true;
            this.dataGridViewChangeLog.RowHeadersVisible = false;
            this.dataGridViewChangeLog.RowTemplate.Height = 24;
            this.dataGridViewChangeLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewChangeLog.Size = new System.Drawing.Size(573, 251);
            this.dataGridViewChangeLog.TabIndex = 0;
            // 
            // MagazynControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.panelFiltry);
            this.Controls.Add(this.panelTop);
            this.Name = "MagazynControl";
            this.Size = new System.Drawing.Size(1147, 753);
            this.Load += new System.EventHandler(this.MagazynControl_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refreshIcon)).EndInit();
            this.panelFiltry.ResumeLayout(false);
            this.panelFiltry.PerformLayout();
            this.panelFiltryButtons.ResumeLayout(false);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReturns)).EndInit();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.bottomSplitContainer.Panel1.ResumeLayout(false);
            this.bottomSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bottomSplitContainer)).EndInit();
            this.bottomSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChangeLog)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnFetchReturns;
        private System.Windows.Forms.ComboBox comboAllegroAccounts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox refreshIcon;
        private System.Windows.Forms.Label lblLastRefresh;
        private System.Windows.Forms.Panel panelFiltry;
        private System.Windows.Forms.Panel panelFiltryButtons;
        private System.Windows.Forms.Button btnFilterWszystkie;
        private System.Windows.Forms.Button Wyslijmail;
        private System.Windows.Forms.Button btnFilterWDrodze;
        private System.Windows.Forms.Button btnFilterPoDecyzji;
        private System.Windows.Forms.Button btnFilterOczekujeNaDecyzje;
        private System.Windows.Forms.Button btnFilterOczekujace;
        private System.Windows.Forms.ComboBox comboFilterStatusAllegro;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.DataGridView dgvReturns;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label lblTotalCount;
        private System.Windows.Forms.Button btnDodajRecznie;
        private System.Windows.Forms.TextBox txtScannerInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer bottomSplitContainer;
        private System.Windows.Forms.Panel panelKomunikator;
        private System.Windows.Forms.DataGridView dataGridViewChangeLog;
        private System.Windows.Forms.Button btnWyslijZwrotyMail;
    }
}
