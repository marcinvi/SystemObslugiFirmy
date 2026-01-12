// Plik: Form3.Designer.cs
namespace Reklamacje_Dane
{
    partial class Form3
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewKlienci = new System.Windows.Forms.DataGridView();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNowyImieNazwisko = new System.Windows.Forms.TextBox();
            this.txtNazwaFirmy = new System.Windows.Forms.TextBox();
            this.txtNIP = new System.Windows.Forms.TextBox();
            this.txtUlicaNr = new System.Windows.Forms.TextBox();
            this.txtKodPocztowy = new System.Windows.Forms.TextBox();
            this.txtMiejscowosc = new System.Windows.Forms.TextBox();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.txtTelefon = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridViewZgloszenia = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnUsun = new System.Windows.Forms.Button();
            this.btnNowy = new System.Windows.Forms.Button();
            this.btnDodaj = new System.Windows.Forms.Button();
            this.btnEdytuj = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtWyszukajKlienta = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.chkZmienKlienta = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKlienci)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewZgloszenia)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewKlienci
            // 
            this.dataGridViewKlienci.AllowUserToAddRows = false;
            this.dataGridViewKlienci.AllowUserToDeleteRows = false;
            this.dataGridViewKlienci.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewKlienci.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewKlienci.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewKlienci.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewKlienci.Location = new System.Drawing.Point(12, 80);
            this.dataGridViewKlienci.MultiSelect = false;
            this.dataGridViewKlienci.Name = "dataGridViewKlienci";
            this.dataGridViewKlienci.ReadOnly = true;
            this.dataGridViewKlienci.RowHeadersVisible = false;
            this.dataGridViewKlienci.RowHeadersWidth = 51;
            this.dataGridViewKlienci.RowTemplate.Height = 24;
            this.dataGridViewKlienci.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewKlienci.Size = new System.Drawing.Size(844, 164);
            this.dataGridViewKlienci.TabIndex = 0;
            // POPRAWKA: Zmiana zdarzenia z SelectionChanged na CellClick
            this.dataGridViewKlienci.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewKlienci_CellClick);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(572, 72);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 20);
            this.label8.TabIndex = 38;
            this.label8.Text = "Telefon";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(572, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 20);
            this.label7.TabIndex = 37;
            this.label7.Text = "Mail";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(465, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 20);
            this.label6.TabIndex = 36;
            this.label6.Text = "Miejscowość";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(304, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 20);
            this.label5.TabIndex = 35;
            this.label5.Text = "Kod pocztowy";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(304, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 20);
            this.label4.TabIndex = 34;
            this.label4.Text = "Ulica i numer domu";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(10, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 20);
            this.label3.TabIndex = 33;
            this.label3.Text = "NIP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(6, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 20);
            this.label2.TabIndex = 32;
            this.label2.Text = "Nazwa firmy";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 20);
            this.label1.TabIndex = 31;
            this.label1.Text = "Imię i nazwisko";
            // 
            // txtNowyImieNazwisko
            // 
            this.txtNowyImieNazwisko.BackColor = System.Drawing.Color.White;
            this.txtNowyImieNazwisko.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNowyImieNazwisko.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNowyImieNazwisko.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtNowyImieNazwisko.Location = new System.Drawing.Point(15, 41);
            this.txtNowyImieNazwisko.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNowyImieNazwisko.Name = "txtNowyImieNazwisko";
            this.txtNowyImieNazwisko.Size = new System.Drawing.Size(274, 27);
            this.txtNowyImieNazwisko.TabIndex = 2;
            // 
            // txtNazwaFirmy
            // 
            this.txtNazwaFirmy.BackColor = System.Drawing.Color.White;
            this.txtNazwaFirmy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNazwaFirmy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNazwaFirmy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtNazwaFirmy.Location = new System.Drawing.Point(14, 95);
            this.txtNazwaFirmy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNazwaFirmy.Name = "txtNazwaFirmy";
            this.txtNazwaFirmy.Size = new System.Drawing.Size(274, 27);
            this.txtNazwaFirmy.TabIndex = 3;
            // 
            // txtNIP
            // 
            this.txtNIP.BackColor = System.Drawing.Color.White;
            this.txtNIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNIP.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNIP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtNIP.Location = new System.Drawing.Point(50, 130);
            this.txtNIP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNIP.Name = "txtNIP";
            this.txtNIP.Size = new System.Drawing.Size(234, 27);
            this.txtNIP.TabIndex = 4;
            // 
            // txtUlicaNr
            // 
            this.txtUlicaNr.BackColor = System.Drawing.Color.White;
            this.txtUlicaNr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUlicaNr.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtUlicaNr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtUlicaNr.Location = new System.Drawing.Point(308, 40);
            this.txtUlicaNr.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUlicaNr.Name = "txtUlicaNr";
            this.txtUlicaNr.Size = new System.Drawing.Size(251, 27);
            this.txtUlicaNr.TabIndex = 5;
            // 
            // txtKodPocztowy
            // 
            this.txtKodPocztowy.BackColor = System.Drawing.Color.White;
            this.txtKodPocztowy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKodPocztowy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtKodPocztowy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtKodPocztowy.Location = new System.Drawing.Point(308, 95);
            this.txtKodPocztowy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtKodPocztowy.Name = "txtKodPocztowy";
            this.txtKodPocztowy.Size = new System.Drawing.Size(72, 27);
            this.txtKodPocztowy.TabIndex = 6;
            // 
            // txtMiejscowosc
            // 
            this.txtMiejscowosc.BackColor = System.Drawing.Color.White;
            this.txtMiejscowosc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMiejscowosc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMiejscowosc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtMiejscowosc.Location = new System.Drawing.Point(387, 95);
            this.txtMiejscowosc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMiejscowosc.Name = "txtMiejscowosc";
            this.txtMiejscowosc.Size = new System.Drawing.Size(172, 27);
            this.txtMiejscowosc.TabIndex = 7;
            // 
            // txtMail
            // 
            this.txtMail.BackColor = System.Drawing.Color.White;
            this.txtMail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtMail.Location = new System.Drawing.Point(576, 40);
            this.txtMail.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(227, 27);
            this.txtMail.TabIndex = 8;
            // 
            // txtTelefon
            // 
            this.txtTelefon.BackColor = System.Drawing.Color.White;
            this.txtTelefon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTelefon.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTelefon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtTelefon.Location = new System.Drawing.Point(576, 96);
            this.txtTelefon.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTelefon.Name = "txtTelefon";
            this.txtTelefon.Size = new System.Drawing.Size(227, 27);
            this.txtTelefon.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.dataGridViewZgloszenia);
            this.panel1.Location = new System.Drawing.Point(12, 451);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(844, 137);
            this.panel1.TabIndex = 42;
            // 
            // dataGridViewZgloszenia
            // 
            this.dataGridViewZgloszenia.AllowUserToAddRows = false;
            this.dataGridViewZgloszenia.AllowUserToDeleteRows = false;
            this.dataGridViewZgloszenia.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewZgloszenia.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewZgloszenia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewZgloszenia.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewZgloszenia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewZgloszenia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewZgloszenia.EnableHeadersVisualStyles = false;
            this.dataGridViewZgloszenia.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewZgloszenia.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridViewZgloszenia.Name = "dataGridViewZgloszenia";
            this.dataGridViewZgloszenia.ReadOnly = true;
            this.dataGridViewZgloszenia.RowHeadersVisible = false;
            this.dataGridViewZgloszenia.RowHeadersWidth = 51;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(226)))), ((int)(((byte)(244)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dataGridViewZgloszenia.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewZgloszenia.RowTemplate.Height = 28;
            this.dataGridViewZgloszenia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewZgloszenia.Size = new System.Drawing.Size(844, 137);
            this.dataGridViewZgloszenia.TabIndex = 0;
            this.dataGridViewZgloszenia.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewZgloszenia_CellDoubleClick);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnUsun);
            this.panel2.Controls.Add(this.btnNowy);
            this.panel2.Controls.Add(this.btnDodaj);
            this.panel2.Controls.Add(this.btnEdytuj);
            this.panel2.Controls.Add(this.txtNowyImieNazwisko);
            this.panel2.Controls.Add(this.txtNIP);
            this.panel2.Controls.Add(this.txtNazwaFirmy);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.txtMail);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.txtTelefon);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.txtMiejscowosc);
            this.panel2.Controls.Add(this.txtKodPocztowy);
            this.panel2.Controls.Add(this.txtUlicaNr);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(12, 250);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(12, 15, 12, 15);
            this.panel2.Size = new System.Drawing.Size(844, 193);
            this.panel2.TabIndex = 43;
            // 
            // btnUsun
            // 
            this.btnUsun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.btnUsun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUsun.ForeColor = System.Drawing.Color.White;
            this.btnUsun.Location = new System.Drawing.Point(419, 152);
            this.btnUsun.Name = "btnUsun";
            this.btnUsun.Size = new System.Drawing.Size(124, 33);
            this.btnUsun.TabIndex = 79;
            this.btnUsun.Text = "Usuń";
            this.btnUsun.UseVisualStyleBackColor = false;
            this.btnUsun.Click += new System.EventHandler(this.btnUsun_Click);
            // 
            // btnNowy
            // 
            this.btnNowy.BackColor = System.Drawing.Color.DarkGray;
            this.btnNowy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNowy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNowy.ForeColor = System.Drawing.Color.White;
            this.btnNowy.Location = new System.Drawing.Point(14, 160);
            this.btnNowy.Name = "btnNowy";
            this.btnNowy.Size = new System.Drawing.Size(124, 33);
            this.btnNowy.TabIndex = 78;
            this.btnNowy.Text = "Wyczyść pola";
            this.btnNowy.UseVisualStyleBackColor = false;
            this.btnNowy.Click += new System.EventHandler(this.btnNowy_Click);
            // 
            // btnDodaj
            // 
            this.btnDodaj.BackColor = System.Drawing.Color.ForestGreen;
            this.btnDodaj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDodaj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDodaj.ForeColor = System.Drawing.Color.White;
            this.btnDodaj.Location = new System.Drawing.Point(549, 152);
            this.btnDodaj.Name = "btnDodaj";
            this.btnDodaj.Size = new System.Drawing.Size(124, 33);
            this.btnDodaj.TabIndex = 77;
            this.btnDodaj.Text = "Dodaj nowego";
            this.btnDodaj.UseVisualStyleBackColor = false;
            this.btnDodaj.Click += new System.EventHandler(this.btnDodaj_Click);
            // 
            // btnEdytuj
            // 
            this.btnEdytuj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.btnEdytuj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdytuj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEdytuj.ForeColor = System.Drawing.Color.White;
            this.btnEdytuj.Location = new System.Drawing.Point(679, 152);
            this.btnEdytuj.Name = "btnEdytuj";
            this.btnEdytuj.Size = new System.Drawing.Size(124, 33);
            this.btnEdytuj.TabIndex = 45;
            this.btnEdytuj.Text = "Zapisz zmiany";
            this.btnEdytuj.UseVisualStyleBackColor = false;
            this.btnEdytuj.Click += new System.EventHandler(this.btnEdytuj_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Location = new System.Drawing.Point(12, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(209, 28);
            this.label9.TabIndex = 44;
            this.label9.Text = "Zarządzanie klientami";
            // 
            // txtWyszukajKlienta
            // 
            this.txtWyszukajKlienta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtWyszukajKlienta.Location = new System.Drawing.Point(240, 47);
            this.txtWyszukajKlienta.Name = "txtWyszukajKlienta";
            this.txtWyszukajKlienta.Size = new System.Drawing.Size(437, 27);
            this.txtWyszukajKlienta.TabIndex = 1;
            this.txtWyszukajKlienta.TextChanged += new System.EventHandler(this.txtWyszukajKlienta_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(236, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(127, 20);
            this.label10.TabIndex = 46;
            this.label10.Text = "Wyszukaj klienta:";
            // 
            // chkZmienKlienta
            // 
            this.chkZmienKlienta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkZmienKlienta.AutoSize = true;
            this.chkZmienKlienta.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.chkZmienKlienta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.chkZmienKlienta.Location = new System.Drawing.Point(662, 13);
            this.chkZmienKlienta.Name = "chkZmienKlienta";
            this.chkZmienKlienta.Size = new System.Drawing.Size(194, 24);
            this.chkZmienKlienta.TabIndex = 80;
            this.chkZmienKlienta.Text = "Zmień / przypisz klienta";
            this.chkZmienKlienta.UseVisualStyleBackColor = true;
            this.chkZmienKlienta.Visible = false;
            this.chkZmienKlienta.CheckedChanged += new System.EventHandler(this.chkZmienKlienta_CheckedChanged);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(868, 600);
            this.Controls.Add(this.chkZmienKlienta);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtWyszukajKlienta);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridViewKlienci);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(870, 599);
            this.Name = "Form3";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Text = "System reklamacji - klienci";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewKlienci)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewZgloszenia)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewKlienci;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNowyImieNazwisko;
        private System.Windows.Forms.TextBox txtNazwaFirmy;
        private System.Windows.Forms.TextBox txtNIP;
        private System.Windows.Forms.TextBox txtUlicaNr;
        private System.Windows.Forms.TextBox txtKodPocztowy;
        private System.Windows.Forms.TextBox txtMiejscowosc;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.TextBox txtTelefon;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridViewZgloszenia;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnEdytuj;
        private System.Windows.Forms.Button btnDodaj;
        private System.Windows.Forms.Button btnNowy;
        private System.Windows.Forms.TextBox txtWyszukajKlienta;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkZmienKlienta;
        private System.Windows.Forms.Button btnUsun;
    }
}