namespace Reklamacje_Dane
{
    partial class WeryfikacjaControl
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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.groupBoxZadania = new System.Windows.Forms.GroupBox();
            this.dgvZadania = new System.Windows.Forms.DataGridView();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.groupBoxAkcja = new System.Windows.Forms.GroupBox();
            this.btnPrzekazDoReklamacji = new System.Windows.Forms.Button();
            this.btnPrzypisz = new System.Windows.Forms.Button();
            this.comboHandlowcy = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBoxDaneMagazynowe = new System.Windows.Forms.GroupBox();
            this.txtPrzyjetyPrzez = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtListPrzewozowy = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtUwagiMagazynu = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtStanProduktu = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBoxInfo = new System.Windows.Forms.GroupBox();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnEdytuj = new System.Windows.Forms.Button();
            this.txtDataDodania = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtNumerFaktury = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtProdukt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTelefon = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAdres = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtKlient = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.groupBoxZadania.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvZadania)).BeginInit();
            this.panelDetails.SuspendLayout();
            this.groupBoxAkcja.SuspendLayout();
            this.groupBoxDaneMagazynowe.SuspendLayout();
            this.groupBoxInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(10, 10);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.groupBoxZadania);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.panelDetails);
            this.splitContainerMain.Size = new System.Drawing.Size(1118, 780);
            this.splitContainerMain.SplitterDistance = 627;
            this.splitContainerMain.TabIndex = 0;
            // 
            // groupBoxZadania
            // 
            this.groupBoxZadania.Controls.Add(this.dgvZadania);
            this.groupBoxZadania.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxZadania.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.groupBoxZadania.Location = new System.Drawing.Point(0, 0);
            this.groupBoxZadania.Name = "groupBoxZadania";
            this.groupBoxZadania.Padding = new System.Windows.Forms.Padding(10);
            this.groupBoxZadania.Size = new System.Drawing.Size(627, 780);
            this.groupBoxZadania.TabIndex = 0;
            this.groupBoxZadania.TabStop = false;
            this.groupBoxZadania.Text = "Zwroty oczekujące na weryfikację";
            // 
            // dgvZadania
            // 
            this.dgvZadania.AllowUserToAddRows = false;
            this.dgvZadania.AllowUserToDeleteRows = false;
            this.dgvZadania.AllowUserToResizeRows = false;
            this.dgvZadania.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvZadania.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvZadania.Location = new System.Drawing.Point(10, 28);
            this.dgvZadania.MultiSelect = false;
            this.dgvZadania.Name = "dgvZadania";
            this.dgvZadania.ReadOnly = true;
            this.dgvZadania.RowHeadersVisible = false;
            this.dgvZadania.RowHeadersWidth = 51;
            this.dgvZadania.RowTemplate.Height = 24;
            this.dgvZadania.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvZadania.Size = new System.Drawing.Size(607, 742);
            this.dgvZadania.TabIndex = 0;
            this.dgvZadania.SelectionChanged += new System.EventHandler(this.dgvZadania_SelectionChanged);
            // 
            // panelDetails
            // 
            this.panelDetails.AutoScroll = true;
            this.panelDetails.Controls.Add(this.groupBoxAkcja);
            this.panelDetails.Controls.Add(this.groupBoxDaneMagazynowe);
            this.panelDetails.Controls.Add(this.groupBoxInfo);
            // POPRAWKA: Ustawienie Dock na Fill
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetails.Location = new System.Drawing.Point(0, 0);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(487, 780);
            this.panelDetails.TabIndex = 0;
            // 
            // groupBoxAkcja
            // 
            this.groupBoxAkcja.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAkcja.Controls.Add(this.btnPrzekazDoReklamacji);
            this.groupBoxAkcja.Controls.Add(this.btnPrzypisz);
            this.groupBoxAkcja.Controls.Add(this.comboHandlowcy);
            this.groupBoxAkcja.Controls.Add(this.label8);
            this.groupBoxAkcja.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.groupBoxAkcja.Location = new System.Drawing.Point(3, 583);
            this.groupBoxAkcja.Name = "groupBoxAkcja";
            this.groupBoxAkcja.Size = new System.Drawing.Size(460, 200);
            this.groupBoxAkcja.TabIndex = 2;
            this.groupBoxAkcja.TabStop = false;
            this.groupBoxAkcja.Text = "Akcja";
            // 
            // btnPrzekazDoReklamacji
            // 
            this.btnPrzekazDoReklamacji.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrzekazDoReklamacji.BackColor = System.Drawing.Color.IndianRed;
            this.btnPrzekazDoReklamacji.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrzekazDoReklamacji.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrzekazDoReklamacji.ForeColor = System.Drawing.Color.White;
            this.btnPrzekazDoReklamacji.Location = new System.Drawing.Point(15, 141);
            this.btnPrzekazDoReklamacji.Name = "btnPrzekazDoReklamacji";
            this.btnPrzekazDoReklamacji.Size = new System.Drawing.Size(429, 45);
            this.btnPrzekazDoReklamacji.TabIndex = 3;
            this.btnPrzekazDoReklamacji.Text = "Przekaż do Działu Reklamacji";
            this.btnPrzekazDoReklamacji.UseVisualStyleBackColor = false;
            this.btnPrzekazDoReklamacji.Click += new System.EventHandler(this.btnPrzekazDoReklamacji_Click);
            // 
            // btnPrzypisz
            // 
            this.btnPrzypisz.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrzypisz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnPrzypisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrzypisz.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrzypisz.ForeColor = System.Drawing.Color.White;
            this.btnPrzypisz.Location = new System.Drawing.Point(15, 90);
            this.btnPrzypisz.Name = "btnPrzypisz";
            this.btnPrzypisz.Size = new System.Drawing.Size(429, 45);
            this.btnPrzypisz.TabIndex = 2;
            this.btnPrzypisz.Text = "Przypisz Handlowca";
            this.btnPrzypisz.UseVisualStyleBackColor = false;
            this.btnPrzypisz.Click += new System.EventHandler(this.btnPrzypisz_Click);
            // 
            // comboHandlowcy
            // 
            this.comboHandlowcy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboHandlowcy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboHandlowcy.FormattingEnabled = true;
            this.comboHandlowcy.Location = new System.Drawing.Point(15, 55);
            this.comboHandlowcy.Name = "comboHandlowcy";
            this.comboHandlowcy.Size = new System.Drawing.Size(429, 28);
            this.comboHandlowcy.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(243, 20);
            this.label8.TabIndex = 0;
            this.label8.Text = "Wybierz handlowca do przypisania:";
            // 
            // groupBoxDaneMagazynowe
            // 
            this.groupBoxDaneMagazynowe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDaneMagazynowe.Controls.Add(this.txtPrzyjetyPrzez);
            this.groupBoxDaneMagazynowe.Controls.Add(this.label12);
            this.groupBoxDaneMagazynowe.Controls.Add(this.txtListPrzewozowy);
            this.groupBoxDaneMagazynowe.Controls.Add(this.label11);
            this.groupBoxDaneMagazynowe.Controls.Add(this.txtUwagiMagazynu);
            this.groupBoxDaneMagazynowe.Controls.Add(this.label10);
            this.groupBoxDaneMagazynowe.Controls.Add(this.txtStanProduktu);
            this.groupBoxDaneMagazynowe.Controls.Add(this.label9);
            this.groupBoxDaneMagazynowe.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.groupBoxDaneMagazynowe.Location = new System.Drawing.Point(3, 353);
            this.groupBoxDaneMagazynowe.Name = "groupBoxDaneMagazynowe";
            this.groupBoxDaneMagazynowe.Size = new System.Drawing.Size(460, 230);
            this.groupBoxDaneMagazynowe.TabIndex = 1;
            this.groupBoxDaneMagazynowe.TabStop = false;
            this.groupBoxDaneMagazynowe.Text = "Informacje z Magazynu (tylko do odczytu)";
            // 
            // txtPrzyjetyPrzez
            // 
            this.txtPrzyjetyPrzez.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrzyjetyPrzez.BackColor = System.Drawing.SystemColors.Control;
            this.txtPrzyjetyPrzez.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPrzyjetyPrzez.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPrzyjetyPrzez.Location = new System.Drawing.Point(135, 195);
            this.txtPrzyjetyPrzez.Name = "txtPrzyjetyPrzez";
            this.txtPrzyjetyPrzez.ReadOnly = true;
            this.txtPrzyjetyPrzez.Size = new System.Drawing.Size(309, 20);
            this.txtPrzyjetyPrzez.TabIndex = 7;
            this.txtPrzyjetyPrzez.Text = "-";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(15, 195);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 20);
            this.label12.TabIndex = 6;
            this.label12.Text = "Przyjęcie:";
            // 
            // txtListPrzewozowy
            // 
            this.txtListPrzewozowy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtListPrzewozowy.BackColor = System.Drawing.SystemColors.Control;
            this.txtListPrzewozowy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtListPrzewozowy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtListPrzewozowy.Location = new System.Drawing.Point(135, 65);
            this.txtListPrzewozowy.Name = "txtListPrzewozowy";
            this.txtListPrzewozowy.ReadOnly = true;
            this.txtListPrzewozowy.Size = new System.Drawing.Size(309, 20);
            this.txtListPrzewozowy.TabIndex = 5;
            this.txtListPrzewozowy.Text = "-";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(15, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 20);
            this.label11.TabIndex = 4;
            this.label11.Text = "Nr przesyłki:";
            // 
            // txtUwagiMagazynu
            // 
            this.txtUwagiMagazynu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUwagiMagazynu.BackColor = System.Drawing.SystemColors.Control;
            this.txtUwagiMagazynu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUwagiMagazynu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtUwagiMagazynu.Location = new System.Drawing.Point(15, 115);
            this.txtUwagiMagazynu.Multiline = true;
            this.txtUwagiMagazynu.Name = "txtUwagiMagazynu";
            this.txtUwagiMagazynu.ReadOnly = true;
            this.txtUwagiMagazynu.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUwagiMagazynu.Size = new System.Drawing.Size(429, 65);
            this.txtUwagiMagazynu.TabIndex = 3;
            this.txtUwagiMagazynu.Text = "-";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(15, 90);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(133, 20);
            this.label10.TabIndex = 2;
            this.label10.Text = "Uwagi magazynu:";
            // 
            // txtStanProduktu
            // 
            this.txtStanProduktu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStanProduktu.BackColor = System.Drawing.SystemColors.Control;
            this.txtStanProduktu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtStanProduktu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtStanProduktu.Location = new System.Drawing.Point(135, 35);
            this.txtStanProduktu.Name = "txtStanProduktu";
            this.txtStanProduktu.ReadOnly = true;
            this.txtStanProduktu.Size = new System.Drawing.Size(309, 20);
            this.txtStanProduktu.TabIndex = 1;
            this.txtStanProduktu.Text = "-";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(15, 35);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "Stan produktu:";
            // 
            // groupBoxInfo
            // 
            this.groupBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInfo.Controls.Add(this.btnAnuluj);
            this.groupBoxInfo.Controls.Add(this.btnZapisz);
            this.groupBoxInfo.Controls.Add(this.btnEdytuj);
            this.groupBoxInfo.Controls.Add(this.txtDataDodania);
            this.groupBoxInfo.Controls.Add(this.label7);
            this.groupBoxInfo.Controls.Add(this.txtNumerFaktury);
            this.groupBoxInfo.Controls.Add(this.label6);
            this.groupBoxInfo.Controls.Add(this.txtProdukt);
            this.groupBoxInfo.Controls.Add(this.label5);
            this.groupBoxInfo.Controls.Add(this.txtTelefon);
            this.groupBoxInfo.Controls.Add(this.label4);
            this.groupBoxInfo.Controls.Add(this.txtAdres);
            this.groupBoxInfo.Controls.Add(this.label3);
            this.groupBoxInfo.Controls.Add(this.txtKlient);
            this.groupBoxInfo.Controls.Add(this.label1);
            this.groupBoxInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.groupBoxInfo.Location = new System.Drawing.Point(3, 3);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Size = new System.Drawing.Size(460, 350);
            this.groupBoxInfo.TabIndex = 0;
            this.groupBoxInfo.TabStop = false;
            this.groupBoxInfo.Text = "Dane Zwrotu (edytowalne)";
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnuluj.Location = new System.Drawing.Point(334, 295);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(110, 35);
            this.btnAnuluj.TabIndex = 14;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            this.btnAnuluj.Click += new System.EventHandler(this.btnAnuluj_Click);
            // 
            // btnZapisz
            // 
            this.btnZapisz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZapisz.BackColor = System.Drawing.Color.SeaGreen;
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(218, 295);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(110, 35);
            this.btnZapisz.TabIndex = 13;
            this.btnZapisz.Text = "Zapisz";
            this.btnZapisz.UseVisualStyleBackColor = false;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);
            // 
            // btnEdytuj
            // 
            this.btnEdytuj.Location = new System.Drawing.Point(15, 295);
            this.btnEdytuj.Name = "btnEdytuj";
            this.btnEdytuj.Size = new System.Drawing.Size(110, 35);
            this.btnEdytuj.TabIndex = 12;
            this.btnEdytuj.Text = "Edytuj Dane";
            this.btnEdytuj.UseVisualStyleBackColor = true;
            this.btnEdytuj.Click += new System.EventHandler(this.btnEdytuj_Click);
            // 
            // txtDataDodania
            // 
            this.txtDataDodania.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataDodania.BackColor = System.Drawing.SystemColors.Control;
            this.txtDataDodania.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDataDodania.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDataDodania.Location = new System.Drawing.Point(135, 255);
            this.txtDataDodania.Name = "txtDataDodania";
            this.txtDataDodania.ReadOnly = true;
            this.txtDataDodania.Size = new System.Drawing.Size(309, 20);
            this.txtDataDodania.TabIndex = 11;
            this.txtDataDodania.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(15, 255);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 20);
            this.label7.TabIndex = 10;
            this.label7.Text = "Data dodania:";
            // 
            // txtNumerFaktury
            // 
            this.txtNumerFaktury.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNumerFaktury.BackColor = System.Drawing.SystemColors.Control;
            this.txtNumerFaktury.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNumerFaktury.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNumerFaktury.Location = new System.Drawing.Point(135, 220);
            this.txtNumerFaktury.Name = "txtNumerFaktury";
            this.txtNumerFaktury.ReadOnly = true;
            this.txtNumerFaktury.Size = new System.Drawing.Size(309, 20);
            this.txtNumerFaktury.TabIndex = 9;
            this.txtNumerFaktury.Text = "-";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(15, 220);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Nr faktury:";
            // 
            // txtProdukt
            // 
            this.txtProdukt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProdukt.BackColor = System.Drawing.SystemColors.Control;
            this.txtProdukt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtProdukt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtProdukt.Location = new System.Drawing.Point(15, 163);
            this.txtProdukt.Multiline = true;
            this.txtProdukt.Name = "txtProdukt";
            this.txtProdukt.ReadOnly = true;
            this.txtProdukt.Size = new System.Drawing.Size(429, 45);
            this.txtProdukt.TabIndex = 7;
            this.txtProdukt.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(15, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Produkt:";
            // 
            // txtTelefon
            // 
            this.txtTelefon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTelefon.BackColor = System.Drawing.SystemColors.Control;
            this.txtTelefon.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTelefon.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTelefon.Location = new System.Drawing.Point(85, 105);
            this.txtTelefon.Name = "txtTelefon";
            this.txtTelefon.ReadOnly = true;
            this.txtTelefon.Size = new System.Drawing.Size(359, 20);
            this.txtTelefon.TabIndex = 5;
            this.txtTelefon.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(15, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "Telefon:";
            // 
            // txtAdres
            // 
            this.txtAdres.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdres.BackColor = System.Drawing.SystemColors.Control;
            this.txtAdres.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAdres.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAdres.Location = new System.Drawing.Point(85, 70);
            this.txtAdres.Name = "txtAdres";
            this.txtAdres.ReadOnly = true;
            this.txtAdres.Size = new System.Drawing.Size(359, 20);
            this.txtAdres.TabIndex = 3;
            this.txtAdres.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(15, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Adres:";
            // 
            // txtKlient
            // 
            this.txtKlient.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKlient.BackColor = System.Drawing.SystemColors.Control;
            this.txtKlient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtKlient.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtKlient.Location = new System.Drawing.Point(85, 35);
            this.txtKlient.Name = "txtKlient";
            this.txtKlient.ReadOnly = true;
            this.txtKlient.Size = new System.Drawing.Size(359, 20);
            this.txtKlient.TabIndex = 1;
            this.txtKlient.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(15, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Klient:";
            // 
            // WeryfikacjaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerMain);
            this.Name = "WeryfikacjaControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(1138, 800);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.groupBoxZadania.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvZadania)).EndInit();
            this.panelDetails.ResumeLayout(false);
            this.groupBoxAkcja.ResumeLayout(false);
            this.groupBoxAkcja.PerformLayout();
            this.groupBoxDaneMagazynowe.ResumeLayout(false);
            this.groupBoxDaneMagazynowe.PerformLayout();
            this.groupBoxInfo.ResumeLayout(false);
            this.groupBoxInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.GroupBox groupBoxZadania;
        private System.Windows.Forms.DataGridView dgvZadania;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.GroupBox groupBoxInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxAkcja;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboHandlowcy;
        private System.Windows.Forms.Button btnPrzypisz;
        private System.Windows.Forms.TextBox txtKlient;
        private System.Windows.Forms.TextBox txtAdres;
        private System.Windows.Forms.TextBox txtTelefon;
        private System.Windows.Forms.TextBox txtProdukt;
        private System.Windows.Forms.TextBox txtNumerFaktury;
        private System.Windows.Forms.TextBox txtDataDodania;
        private System.Windows.Forms.Button btnEdytuj;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.GroupBox groupBoxDaneMagazynowe;
        private System.Windows.Forms.TextBox txtStanProduktu;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtUwagiMagazynu;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtListPrzewozowy;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtPrzyjetyPrzez;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnPrzekazDoReklamacji;
    }
}