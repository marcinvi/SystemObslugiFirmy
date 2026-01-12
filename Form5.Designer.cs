// Plik: Form5.Designer.cs
namespace Reklamacje_Dane
{
    partial class Form5
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView dgvNadawcaSuggestions;
        private System.Windows.Forms.DataGridView dgvOdbiorcaSuggestions;

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
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelNadawcaTelefon = new System.Windows.Forms.Label();
            this.dgvNadawcaSuggestions = new System.Windows.Forms.DataGridView();
            this.labelNadawcaMail = new System.Windows.Forms.Label();
            this.labelNadawcaMiasto = new System.Windows.Forms.Label();
            this.labelNadawcaKodPocztowy = new System.Windows.Forms.Label();
            this.labelNadawcaUlica = new System.Windows.Forms.Label();
            this.labelNadawcaFirma = new System.Windows.Forms.Label();
            this.labelNadawcaNazwa = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNadawcaTelefon = new System.Windows.Forms.TextBox();
            this.textBoxNadawcaMail = new System.Windows.Forms.TextBox();
            this.textBoxNadawcaMiasto = new System.Windows.Forms.TextBox();
            this.textBoxNadawcaKodPocztowy = new System.Windows.Forms.TextBox();
            this.textBoxNadawcaUlica = new System.Windows.Forms.TextBox();
            this.textBoxNadawcaFirma = new System.Windows.Forms.TextBox();
            this.textBoxNadawcaNazwa = new System.Windows.Forms.TextBox();
            this.textBoxNadawcaSearch = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvOdbiorcaSuggestions = new System.Windows.Forms.DataGridView();
            this.labelOdbiorcaTelefon = new System.Windows.Forms.Label();
            this.labelOdbiorcaMail = new System.Windows.Forms.Label();
            this.labelOdbiorcaMiasto = new System.Windows.Forms.Label();
            this.labelOdbiorcaKodPocztowy = new System.Windows.Forms.Label();
            this.labelOdbiorcaUlica = new System.Windows.Forms.Label();
            this.labelOdbiorcaFirma = new System.Windows.Forms.Label();
            this.labelOdbiorcaNazwa = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxOdbiorcaTelefon = new System.Windows.Forms.TextBox();
            this.textBoxOdbiorcaMail = new System.Windows.Forms.TextBox();
            this.textBoxOdbiorcaMiasto = new System.Windows.Forms.TextBox();
            this.textBoxOdbiorcaKodPocztowy = new System.Windows.Forms.TextBox();
            this.textBoxOdbiorcaUlica = new System.Windows.Forms.TextBox();
            this.textBoxOdbiorcaFirma = new System.Windows.Forms.TextBox();
            this.textBoxOdbiorcaNazwa = new System.Windows.Forms.TextBox();
            this.textBoxOdbiorcaSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.waga = new System.Windows.Forms.TextBox();
            this.cowysylamy = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Zamow = new System.Windows.Forms.Button();
            this.zwrotna = new System.Windows.Forms.CheckBox();
            this.zlecenie = new System.Windows.Forms.CheckBox();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNadawcaSuggestions)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOdbiorcaSuggestions)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nadawca";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.labelNadawcaTelefon);
            this.panel1.Controls.Add(this.dgvNadawcaSuggestions);
            this.panel1.Controls.Add(this.labelNadawcaMail);
            this.panel1.Controls.Add(this.labelNadawcaMiasto);
            this.panel1.Controls.Add(this.labelNadawcaKodPocztowy);
            this.panel1.Controls.Add(this.labelNadawcaUlica);
            this.panel1.Controls.Add(this.labelNadawcaFirma);
            this.panel1.Controls.Add(this.labelNadawcaNazwa);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBoxNadawcaTelefon);
            this.panel1.Controls.Add(this.textBoxNadawcaMail);
            this.panel1.Controls.Add(this.textBoxNadawcaMiasto);
            this.panel1.Controls.Add(this.textBoxNadawcaKodPocztowy);
            this.panel1.Controls.Add(this.textBoxNadawcaUlica);
            this.panel1.Controls.Add(this.textBoxNadawcaFirma);
            this.panel1.Controls.Add(this.textBoxNadawcaNazwa);
            this.panel1.Controls.Add(this.textBoxNadawcaSearch);
            this.panel1.Location = new System.Drawing.Point(16, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(370, 363);
            this.panel1.TabIndex = 1;
            // 
            // labelNadawcaTelefon
            // 
            this.labelNadawcaTelefon.AutoSize = true;
            this.labelNadawcaTelefon.Location = new System.Drawing.Point(12, 299);
            this.labelNadawcaTelefon.Name = "labelNadawcaTelefon";
            this.labelNadawcaTelefon.Size = new System.Drawing.Size(58, 20);
            this.labelNadawcaTelefon.TabIndex = 17;
            this.labelNadawcaTelefon.Text = "Telefon";
            this.labelNadawcaTelefon.Click += new System.EventHandler(this.labelNadawcaTelefon_Click);
            // 
            // dgvNadawcaSuggestions
            // 
            this.dgvNadawcaSuggestions.AllowUserToAddRows = false;
            this.dgvNadawcaSuggestions.AllowUserToDeleteRows = false;
            this.dgvNadawcaSuggestions.AllowUserToResizeRows = false;
            this.dgvNadawcaSuggestions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNadawcaSuggestions.BackgroundColor = System.Drawing.Color.White;
            this.dgvNadawcaSuggestions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvNadawcaSuggestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNadawcaSuggestions.ColumnHeadersVisible = false;
            this.dgvNadawcaSuggestions.Location = new System.Drawing.Point(14, 44);
            this.dgvNadawcaSuggestions.MultiSelect = false;
            this.dgvNadawcaSuggestions.Name = "dgvNadawcaSuggestions";
            this.dgvNadawcaSuggestions.ReadOnly = true;
            this.dgvNadawcaSuggestions.RowHeadersVisible = false;
            this.dgvNadawcaSuggestions.RowHeadersWidth = 51;
            this.dgvNadawcaSuggestions.RowTemplate.Height = 24;
            this.dgvNadawcaSuggestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNadawcaSuggestions.Size = new System.Drawing.Size(340, 120);
            this.dgvNadawcaSuggestions.TabIndex = 10;
            this.dgvNadawcaSuggestions.Visible = false;
            // 
            // labelNadawcaMail
            // 
            this.labelNadawcaMail.AutoSize = true;
            this.labelNadawcaMail.Location = new System.Drawing.Point(15, 249);
            this.labelNadawcaMail.Name = "labelNadawcaMail";
            this.labelNadawcaMail.Size = new System.Drawing.Size(38, 20);
            this.labelNadawcaMail.TabIndex = 16;
            this.labelNadawcaMail.Text = "Mail";
            // 
            // labelNadawcaMiasto
            // 
            this.labelNadawcaMiasto.AutoSize = true;
            this.labelNadawcaMiasto.Location = new System.Drawing.Point(113, 200);
            this.labelNadawcaMiasto.Name = "labelNadawcaMiasto";
            this.labelNadawcaMiasto.Size = new System.Drawing.Size(54, 20);
            this.labelNadawcaMiasto.TabIndex = 15;
            this.labelNadawcaMiasto.Text = "Miasto";
            // 
            // labelNadawcaKodPocztowy
            // 
            this.labelNadawcaKodPocztowy.AutoSize = true;
            this.labelNadawcaKodPocztowy.Location = new System.Drawing.Point(3, 200);
            this.labelNadawcaKodPocztowy.Name = "labelNadawcaKodPocztowy";
            this.labelNadawcaKodPocztowy.Size = new System.Drawing.Size(104, 20);
            this.labelNadawcaKodPocztowy.TabIndex = 14;
            this.labelNadawcaKodPocztowy.Text = "Kod pocztowy";
            // 
            // labelNadawcaUlica
            // 
            this.labelNadawcaUlica.AutoSize = true;
            this.labelNadawcaUlica.Location = new System.Drawing.Point(11, 148);
            this.labelNadawcaUlica.Name = "labelNadawcaUlica";
            this.labelNadawcaUlica.Size = new System.Drawing.Size(42, 20);
            this.labelNadawcaUlica.TabIndex = 13;
            this.labelNadawcaUlica.Text = "Ulica";
            // 
            // labelNadawcaFirma
            // 
            this.labelNadawcaFirma.AutoSize = true;
            this.labelNadawcaFirma.Location = new System.Drawing.Point(10, 101);
            this.labelNadawcaFirma.Name = "labelNadawcaFirma";
            this.labelNadawcaFirma.Size = new System.Drawing.Size(46, 20);
            this.labelNadawcaFirma.TabIndex = 12;
            this.labelNadawcaFirma.Text = "Firma";
            // 
            // labelNadawcaNazwa
            // 
            this.labelNadawcaNazwa.AutoSize = true;
            this.labelNadawcaNazwa.Location = new System.Drawing.Point(11, 48);
            this.labelNadawcaNazwa.Name = "labelNadawcaNazwa";
            this.labelNadawcaNazwa.Size = new System.Drawing.Size(110, 20);
            this.labelNadawcaNazwa.TabIndex = 11;
            this.labelNadawcaNazwa.Text = "Imię i nazwisko";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(12, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(183, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Szukaj Imię i Nazwisko / Firma";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // textBoxNadawcaTelefon
            // 
            this.textBoxNadawcaTelefon.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNadawcaTelefon.Location = new System.Drawing.Point(15, 317);
            this.textBoxNadawcaTelefon.Name = "textBoxNadawcaTelefon";
            this.textBoxNadawcaTelefon.Size = new System.Drawing.Size(340, 27);
            this.textBoxNadawcaTelefon.TabIndex = 7;
            this.textBoxNadawcaTelefon.TextChanged += new System.EventHandler(this.textBoxNadawcaTelefon_TextChanged);
            // 
            // textBoxNadawcaMail
            // 
            this.textBoxNadawcaMail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNadawcaMail.Location = new System.Drawing.Point(14, 269);
            this.textBoxNadawcaMail.Name = "textBoxNadawcaMail";
            this.textBoxNadawcaMail.Size = new System.Drawing.Size(340, 27);
            this.textBoxNadawcaMail.TabIndex = 6;
            this.textBoxNadawcaMail.TextChanged += new System.EventHandler(this.textBoxNadawcaMail_TextChanged);
            // 
            // textBoxNadawcaMiasto
            // 
            this.textBoxNadawcaMiasto.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNadawcaMiasto.Location = new System.Drawing.Point(110, 219);
            this.textBoxNadawcaMiasto.Name = "textBoxNadawcaMiasto";
            this.textBoxNadawcaMiasto.Size = new System.Drawing.Size(244, 27);
            this.textBoxNadawcaMiasto.TabIndex = 5;
            // 
            // textBoxNadawcaKodPocztowy
            // 
            this.textBoxNadawcaKodPocztowy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNadawcaKodPocztowy.Location = new System.Drawing.Point(15, 219);
            this.textBoxNadawcaKodPocztowy.Name = "textBoxNadawcaKodPocztowy";
            this.textBoxNadawcaKodPocztowy.Size = new System.Drawing.Size(90, 27);
            this.textBoxNadawcaKodPocztowy.TabIndex = 4;
            // 
            // textBoxNadawcaUlica
            // 
            this.textBoxNadawcaUlica.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNadawcaUlica.Location = new System.Drawing.Point(14, 170);
            this.textBoxNadawcaUlica.Name = "textBoxNadawcaUlica";
            this.textBoxNadawcaUlica.Size = new System.Drawing.Size(340, 27);
            this.textBoxNadawcaUlica.TabIndex = 3;
            // 
            // textBoxNadawcaFirma
            // 
            this.textBoxNadawcaFirma.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNadawcaFirma.Location = new System.Drawing.Point(14, 118);
            this.textBoxNadawcaFirma.Name = "textBoxNadawcaFirma";
            this.textBoxNadawcaFirma.Size = new System.Drawing.Size(340, 27);
            this.textBoxNadawcaFirma.TabIndex = 2;
            // 
            // textBoxNadawcaNazwa
            // 
            this.textBoxNadawcaNazwa.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNadawcaNazwa.Location = new System.Drawing.Point(14, 71);
            this.textBoxNadawcaNazwa.Name = "textBoxNadawcaNazwa";
            this.textBoxNadawcaNazwa.Size = new System.Drawing.Size(340, 27);
            this.textBoxNadawcaNazwa.TabIndex = 1;
            // 
            // textBoxNadawcaSearch
            // 
            this.textBoxNadawcaSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNadawcaSearch.Location = new System.Drawing.Point(14, 14);
            this.textBoxNadawcaSearch.Name = "textBoxNadawcaSearch";
            this.textBoxNadawcaSearch.Size = new System.Drawing.Size(340, 27);
            this.textBoxNadawcaSearch.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.Controls.Add(this.dgvOdbiorcaSuggestions);
            this.panel2.Controls.Add(this.labelOdbiorcaTelefon);
            this.panel2.Controls.Add(this.labelOdbiorcaMail);
            this.panel2.Controls.Add(this.labelOdbiorcaMiasto);
            this.panel2.Controls.Add(this.labelOdbiorcaKodPocztowy);
            this.panel2.Controls.Add(this.labelOdbiorcaUlica);
            this.panel2.Controls.Add(this.labelOdbiorcaFirma);
            this.panel2.Controls.Add(this.labelOdbiorcaNazwa);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.textBoxOdbiorcaTelefon);
            this.panel2.Controls.Add(this.textBoxOdbiorcaMail);
            this.panel2.Controls.Add(this.textBoxOdbiorcaMiasto);
            this.panel2.Controls.Add(this.textBoxOdbiorcaKodPocztowy);
            this.panel2.Controls.Add(this.textBoxOdbiorcaUlica);
            this.panel2.Controls.Add(this.textBoxOdbiorcaFirma);
            this.panel2.Controls.Add(this.textBoxOdbiorcaNazwa);
            this.panel2.Controls.Add(this.textBoxOdbiorcaSearch);
            this.panel2.Location = new System.Drawing.Point(401, 32);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(370, 363);
            this.panel2.TabIndex = 9;
            // 
            // dgvOdbiorcaSuggestions
            // 
            this.dgvOdbiorcaSuggestions.AllowUserToAddRows = false;
            this.dgvOdbiorcaSuggestions.AllowUserToDeleteRows = false;
            this.dgvOdbiorcaSuggestions.AllowUserToResizeRows = false;
            this.dgvOdbiorcaSuggestions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOdbiorcaSuggestions.BackgroundColor = System.Drawing.Color.White;
            this.dgvOdbiorcaSuggestions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvOdbiorcaSuggestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOdbiorcaSuggestions.ColumnHeadersVisible = false;
            this.dgvOdbiorcaSuggestions.Location = new System.Drawing.Point(14, 44);
            this.dgvOdbiorcaSuggestions.MultiSelect = false;
            this.dgvOdbiorcaSuggestions.Name = "dgvOdbiorcaSuggestions";
            this.dgvOdbiorcaSuggestions.ReadOnly = true;
            this.dgvOdbiorcaSuggestions.RowHeadersVisible = false;
            this.dgvOdbiorcaSuggestions.RowHeadersWidth = 51;
            this.dgvOdbiorcaSuggestions.RowTemplate.Height = 24;
            this.dgvOdbiorcaSuggestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOdbiorcaSuggestions.Size = new System.Drawing.Size(340, 120);
            this.dgvOdbiorcaSuggestions.TabIndex = 11;
            this.dgvOdbiorcaSuggestions.Visible = false;
            // 
            // labelOdbiorcaTelefon
            // 
            this.labelOdbiorcaTelefon.AutoSize = true;
            this.labelOdbiorcaTelefon.Location = new System.Drawing.Point(11, 299);
            this.labelOdbiorcaTelefon.Name = "labelOdbiorcaTelefon";
            this.labelOdbiorcaTelefon.Size = new System.Drawing.Size(58, 20);
            this.labelOdbiorcaTelefon.TabIndex = 18;
            this.labelOdbiorcaTelefon.Text = "Telefon";
            // 
            // labelOdbiorcaMail
            // 
            this.labelOdbiorcaMail.AutoSize = true;
            this.labelOdbiorcaMail.Location = new System.Drawing.Point(15, 249);
            this.labelOdbiorcaMail.Name = "labelOdbiorcaMail";
            this.labelOdbiorcaMail.Size = new System.Drawing.Size(38, 20);
            this.labelOdbiorcaMail.TabIndex = 17;
            this.labelOdbiorcaMail.Text = "Mail";
            // 
            // labelOdbiorcaMiasto
            // 
            this.labelOdbiorcaMiasto.AutoSize = true;
            this.labelOdbiorcaMiasto.Location = new System.Drawing.Point(113, 200);
            this.labelOdbiorcaMiasto.Name = "labelOdbiorcaMiasto";
            this.labelOdbiorcaMiasto.Size = new System.Drawing.Size(54, 20);
            this.labelOdbiorcaMiasto.TabIndex = 16;
            this.labelOdbiorcaMiasto.Text = "Miasto";
            // 
            // labelOdbiorcaKodPocztowy
            // 
            this.labelOdbiorcaKodPocztowy.AutoSize = true;
            this.labelOdbiorcaKodPocztowy.Location = new System.Drawing.Point(3, 200);
            this.labelOdbiorcaKodPocztowy.Name = "labelOdbiorcaKodPocztowy";
            this.labelOdbiorcaKodPocztowy.Size = new System.Drawing.Size(104, 20);
            this.labelOdbiorcaKodPocztowy.TabIndex = 15;
            this.labelOdbiorcaKodPocztowy.Text = "Kod pocztowy";
            // 
            // labelOdbiorcaUlica
            // 
            this.labelOdbiorcaUlica.AutoSize = true;
            this.labelOdbiorcaUlica.Location = new System.Drawing.Point(15, 148);
            this.labelOdbiorcaUlica.Name = "labelOdbiorcaUlica";
            this.labelOdbiorcaUlica.Size = new System.Drawing.Size(42, 20);
            this.labelOdbiorcaUlica.TabIndex = 14;
            this.labelOdbiorcaUlica.Text = "Ulica";
            // 
            // labelOdbiorcaFirma
            // 
            this.labelOdbiorcaFirma.AutoSize = true;
            this.labelOdbiorcaFirma.Location = new System.Drawing.Point(15, 101);
            this.labelOdbiorcaFirma.Name = "labelOdbiorcaFirma";
            this.labelOdbiorcaFirma.Size = new System.Drawing.Size(46, 20);
            this.labelOdbiorcaFirma.TabIndex = 13;
            this.labelOdbiorcaFirma.Text = "Firma";
            // 
            // labelOdbiorcaNazwa
            // 
            this.labelOdbiorcaNazwa.AutoSize = true;
            this.labelOdbiorcaNazwa.Location = new System.Drawing.Point(11, 48);
            this.labelOdbiorcaNazwa.Name = "labelOdbiorcaNazwa";
            this.labelOdbiorcaNazwa.Size = new System.Drawing.Size(110, 20);
            this.labelOdbiorcaNazwa.TabIndex = 12;
            this.labelOdbiorcaNazwa.Text = "Imię i nazwisko";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(12, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(183, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Szukaj Imię i Nazwisko / Firma";
            // 
            // textBoxOdbiorcaTelefon
            // 
            this.textBoxOdbiorcaTelefon.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOdbiorcaTelefon.Location = new System.Drawing.Point(14, 317);
            this.textBoxOdbiorcaTelefon.Name = "textBoxOdbiorcaTelefon";
            this.textBoxOdbiorcaTelefon.Size = new System.Drawing.Size(340, 27);
            this.textBoxOdbiorcaTelefon.TabIndex = 7;
            // 
            // textBoxOdbiorcaMail
            // 
            this.textBoxOdbiorcaMail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOdbiorcaMail.Location = new System.Drawing.Point(14, 269);
            this.textBoxOdbiorcaMail.Name = "textBoxOdbiorcaMail";
            this.textBoxOdbiorcaMail.Size = new System.Drawing.Size(340, 27);
            this.textBoxOdbiorcaMail.TabIndex = 6;
            // 
            // textBoxOdbiorcaMiasto
            // 
            this.textBoxOdbiorcaMiasto.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOdbiorcaMiasto.Location = new System.Drawing.Point(110, 218);
            this.textBoxOdbiorcaMiasto.Name = "textBoxOdbiorcaMiasto";
            this.textBoxOdbiorcaMiasto.Size = new System.Drawing.Size(244, 27);
            this.textBoxOdbiorcaMiasto.TabIndex = 5;
            // 
            // textBoxOdbiorcaKodPocztowy
            // 
            this.textBoxOdbiorcaKodPocztowy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOdbiorcaKodPocztowy.Location = new System.Drawing.Point(14, 219);
            this.textBoxOdbiorcaKodPocztowy.Name = "textBoxOdbiorcaKodPocztowy";
            this.textBoxOdbiorcaKodPocztowy.Size = new System.Drawing.Size(90, 27);
            this.textBoxOdbiorcaKodPocztowy.TabIndex = 4;
            // 
            // textBoxOdbiorcaUlica
            // 
            this.textBoxOdbiorcaUlica.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOdbiorcaUlica.Location = new System.Drawing.Point(14, 170);
            this.textBoxOdbiorcaUlica.Name = "textBoxOdbiorcaUlica";
            this.textBoxOdbiorcaUlica.Size = new System.Drawing.Size(340, 27);
            this.textBoxOdbiorcaUlica.TabIndex = 3;
            // 
            // textBoxOdbiorcaFirma
            // 
            this.textBoxOdbiorcaFirma.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOdbiorcaFirma.Location = new System.Drawing.Point(14, 118);
            this.textBoxOdbiorcaFirma.Name = "textBoxOdbiorcaFirma";
            this.textBoxOdbiorcaFirma.Size = new System.Drawing.Size(340, 27);
            this.textBoxOdbiorcaFirma.TabIndex = 2;
            // 
            // textBoxOdbiorcaNazwa
            // 
            this.textBoxOdbiorcaNazwa.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOdbiorcaNazwa.Location = new System.Drawing.Point(14, 71);
            this.textBoxOdbiorcaNazwa.Name = "textBoxOdbiorcaNazwa";
            this.textBoxOdbiorcaNazwa.Size = new System.Drawing.Size(340, 27);
            this.textBoxOdbiorcaNazwa.TabIndex = 1;
            // 
            // textBoxOdbiorcaSearch
            // 
            this.textBoxOdbiorcaSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOdbiorcaSearch.Location = new System.Drawing.Point(14, 14);
            this.textBoxOdbiorcaSearch.Name = "textBoxOdbiorcaSearch";
            this.textBoxOdbiorcaSearch.Size = new System.Drawing.Size(340, 27);
            this.textBoxOdbiorcaSearch.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(397, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Odbiorca";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.waga);
            this.panel3.Controls.Add(this.cowysylamy);
            this.panel3.Location = new System.Drawing.Point(16, 421);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(755, 50);
            this.panel3.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.label8.ForeColor = System.Drawing.Color.DimGray;
            this.label8.Location = new System.Drawing.Point(374, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 17);
            this.label8.TabIndex = 12;
            this.label8.Text = "Waga (kg)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.label7.ForeColor = System.Drawing.Color.DimGray;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 17);
            this.label7.TabIndex = 11;
            this.label7.Text = "Zawartość";
            // 
            // waga
            // 
            this.waga.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.waga.Location = new System.Drawing.Point(377, 16);
            this.waga.Name = "waga";
            this.waga.Size = new System.Drawing.Size(354, 27);
            this.waga.TabIndex = 1;
            this.waga.Text = "20";
            // 
            // cowysylamy
            // 
            this.cowysylamy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cowysylamy.Location = new System.Drawing.Point(6, 16);
            this.cowysylamy.Name = "cowysylamy";
            this.cowysylamy.Size = new System.Drawing.Size(365, 27);
            this.cowysylamy.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(12, 398);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "Paczka";
            // 
            // Zamow
            // 
            this.Zamow.BackColor = System.Drawing.Color.ForestGreen;
            this.Zamow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Zamow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.Zamow.ForeColor = System.Drawing.Color.White;
            this.Zamow.Location = new System.Drawing.Point(622, 477);
            this.Zamow.Name = "Zamow";
            this.Zamow.Size = new System.Drawing.Size(149, 36);
            this.Zamow.TabIndex = 13;
            this.Zamow.Text = "Zamów kuriera";
            this.Zamow.UseVisualStyleBackColor = false;
            this.Zamow.Click += new System.EventHandler(this.Zamow_Click);
            // 
            // zwrotna
            // 
            this.zwrotna.AutoSize = true;
            this.zwrotna.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.zwrotna.Location = new System.Drawing.Point(16, 483);
            this.zwrotna.Name = "zwrotna";
            this.zwrotna.Size = new System.Drawing.Size(132, 24);
            this.zwrotna.TabIndex = 15;
            this.zwrotna.Text = "Paczka zwrotna";
            this.zwrotna.UseVisualStyleBackColor = true;
            // 
            // zlecenie
            // 
            this.zlecenie.AutoSize = true;
            this.zlecenie.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.zlecenie.Location = new System.Drawing.Point(154, 483);
            this.zlecenie.Name = "zlecenie";
            this.zlecenie.Size = new System.Drawing.Size(144, 24);
            this.zlecenie.TabIndex = 16;
            this.zlecenie.Text = "Zlecenie odbioru";
            this.zlecenie.UseVisualStyleBackColor = true;
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(777, 32);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(555, 721);
            this.webView21.TabIndex = 20;
            this.webView21.ZoomFactor = 1D;
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(776, 586);
            this.Controls.Add(this.webView21);
            this.Controls.Add(this.zlecenie);
            this.Controls.Add(this.zwrotna);
            this.Controls.Add(this.Zamow);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "Form5";
            this.Text = "Zamów Kuriera DPD";
            this.Load += new System.EventHandler(this.Form5_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNadawcaSuggestions)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOdbiorcaSuggestions)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxNadawcaSearch;
        private System.Windows.Forms.TextBox textBoxNadawcaNazwa;
        private System.Windows.Forms.TextBox textBoxNadawcaFirma;
        private System.Windows.Forms.TextBox textBoxNadawcaUlica;
        private System.Windows.Forms.TextBox textBoxNadawcaKodPocztowy;
        private System.Windows.Forms.TextBox textBoxNadawcaMiasto;
        private System.Windows.Forms.TextBox textBoxNadawcaMail;
        private System.Windows.Forms.TextBox textBoxNadawcaTelefon;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBoxOdbiorcaTelefon;
        private System.Windows.Forms.TextBox textBoxOdbiorcaMail;
        private System.Windows.Forms.TextBox textBoxOdbiorcaMiasto;
        private System.Windows.Forms.TextBox textBoxOdbiorcaKodPocztowy;
        private System.Windows.Forms.TextBox textBoxOdbiorcaUlica;
        private System.Windows.Forms.TextBox textBoxOdbiorcaFirma;
        private System.Windows.Forms.TextBox textBoxOdbiorcaNazwa;
        private System.Windows.Forms.TextBox textBoxOdbiorcaSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox waga;
        private System.Windows.Forms.TextBox cowysylamy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button Zamow;
        private System.Windows.Forms.CheckBox zwrotna;
        private System.Windows.Forms.CheckBox zlecenie;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Label labelNadawcaTelefon;
        private System.Windows.Forms.Label labelNadawcaMail;
        private System.Windows.Forms.Label labelNadawcaMiasto;
        private System.Windows.Forms.Label labelNadawcaKodPocztowy;
        private System.Windows.Forms.Label labelNadawcaUlica;
        private System.Windows.Forms.Label labelNadawcaFirma;
        private System.Windows.Forms.Label labelOdbiorcaTelefon;
        private System.Windows.Forms.Label labelOdbiorcaMail;
        private System.Windows.Forms.Label labelOdbiorcaMiasto;
        private System.Windows.Forms.Label labelOdbiorcaKodPocztowy;
        private System.Windows.Forms.Label labelOdbiorcaUlica;
        private System.Windows.Forms.Label labelOdbiorcaFirma;
        private System.Windows.Forms.Label labelOdbiorcaNazwa;
        private System.Windows.Forms.Label labelNadawcaNazwa;
    }
}