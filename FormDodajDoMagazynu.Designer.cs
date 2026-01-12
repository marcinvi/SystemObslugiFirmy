namespace Reklamacje_Dane
{
    partial class FormDodajDoMagazynu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpWyszukiwanie = new System.Windows.Forms.GroupBox();
            this.dgvWyniki = new System.Windows.Forms.DataGridView();
            this.btnSzukaj = new System.Windows.Forms.Button();
            this.txtSzukaj = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpDane = new System.Windows.Forms.GroupBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.grpCzesci = new System.Windows.Forms.GroupBox();
            this.chkListCzesci = new System.Windows.Forms.CheckedListBox();
            this.cmbNowaCzescSzybka = new System.Windows.Forms.ComboBox();
            this.btnDodajCzescSzybka = new System.Windows.Forms.Button();
            this.lblPowiazanie = new System.Windows.Forms.Label();
            this.btnZapisz = new System.Windows.Forms.Button();
            this.txtUwagi = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbLokalizacja = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grpWyszukiwanie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWyniki)).BeginInit();
            this.grpDane.SuspendLayout();
            this.grpCzesci.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpWyszukiwanie
            // 
            this.grpWyszukiwanie.Controls.Add(this.dgvWyniki);
            this.grpWyszukiwanie.Controls.Add(this.btnSzukaj);
            this.grpWyszukiwanie.Controls.Add(this.txtSzukaj);
            this.grpWyszukiwanie.Controls.Add(this.label1);
            this.grpWyszukiwanie.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpWyszukiwanie.Location = new System.Drawing.Point(10, 10);
            this.grpWyszukiwanie.Name = "grpWyszukiwanie";
            this.grpWyszukiwanie.Padding = new System.Windows.Forms.Padding(10);
            this.grpWyszukiwanie.Size = new System.Drawing.Size(562, 180);
            this.grpWyszukiwanie.TabIndex = 0;
            this.grpWyszukiwanie.TabStop = false;
            this.grpWyszukiwanie.Text = "KROK 1: Znajdź zgłoszenie (Opcjonalne)";
            // 
            // dgvWyniki
            // 
            this.dgvWyniki.AllowUserToAddRows = false;
            this.dgvWyniki.AllowUserToDeleteRows = false;
            this.dgvWyniki.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWyniki.BackgroundColor = System.Drawing.Color.White;
            this.dgvWyniki.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWyniki.Location = new System.Drawing.Point(13, 65);
            this.dgvWyniki.MultiSelect = false;
            this.dgvWyniki.Name = "dgvWyniki";
            this.dgvWyniki.ReadOnly = true;
            this.dgvWyniki.RowHeadersVisible = false;
            this.dgvWyniki.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWyniki.Size = new System.Drawing.Size(536, 100);
            this.dgvWyniki.TabIndex = 3;
            this.dgvWyniki.SelectionChanged += new System.EventHandler(this.dgvWyniki_SelectionChanged);
            // 
            // btnSzukaj
            // 
            this.btnSzukaj.Location = new System.Drawing.Point(449, 32);
            this.btnSzukaj.Name = "btnSzukaj";
            this.btnSzukaj.Size = new System.Drawing.Size(100, 27);
            this.btnSzukaj.TabIndex = 2;
            this.btnSzukaj.Text = "Szukaj";
            this.btnSzukaj.UseVisualStyleBackColor = true;
            this.btnSzukaj.Click += new System.EventHandler(this.btnSzukaj_Click);
            // 
            // txtSzukaj
            // 
            this.txtSzukaj.Location = new System.Drawing.Point(143, 32);
            this.txtSzukaj.Name = "txtSzukaj";
            this.txtSzukaj.Size = new System.Drawing.Size(300, 27);
            this.txtSzukaj.TabIndex = 1;
            this.txtSzukaj.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSzukaj_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Szukaj (SN, nr...):";
            // 
            // grpDane
            // 
            this.grpDane.Controls.Add(this.labelStatus);
            this.grpDane.Controls.Add(this.cmbStatus);
            this.grpDane.Controls.Add(this.grpCzesci);
            this.grpDane.Controls.Add(this.lblPowiazanie);
            this.grpDane.Controls.Add(this.btnZapisz);
            this.grpDane.Controls.Add(this.txtUwagi);
            this.grpDane.Controls.Add(this.label5);
            this.grpDane.Controls.Add(this.cmbLokalizacja);
            this.grpDane.Controls.Add(this.label4);
            this.grpDane.Controls.Add(this.txtSN);
            this.grpDane.Controls.Add(this.label3);
            this.grpDane.Controls.Add(this.txtModel);
            this.grpDane.Controls.Add(this.label2);
            this.grpDane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDane.Location = new System.Drawing.Point(10, 190);
            this.grpDane.Name = "grpDane";
            this.grpDane.Size = new System.Drawing.Size(562, 563);
            this.grpDane.TabIndex = 1;
            this.grpDane.TabStop = false;
            this.grpDane.Text = "KROK 2: Dane Sprzętu";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.Location = new System.Drawing.Point(13, 155);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(57, 20);
            this.labelStatus.TabIndex = 12;
            this.labelStatus.Text = "Status:";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(110, 152);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(439, 28);
            this.cmbStatus.TabIndex = 11;
            this.cmbStatus.SelectedIndexChanged += new System.EventHandler(this.cmbStatus_SelectedIndexChanged);
            // 
            // grpCzesci
            // 
            this.grpCzesci.Controls.Add(this.chkListCzesci);
            this.grpCzesci.Controls.Add(this.cmbNowaCzescSzybka);
            this.grpCzesci.Controls.Add(this.btnDodajCzescSzybka);
            this.grpCzesci.Location = new System.Drawing.Point(17, 275);
            this.grpCzesci.Name = "grpCzesci";
            this.grpCzesci.Size = new System.Drawing.Size(532, 220);
            this.grpCzesci.TabIndex = 10;
            this.grpCzesci.TabStop = false;
            this.grpCzesci.Text = "Odzysk Części (Dostępne dla statusu DAWCA)";
            this.grpCzesci.Visible = false;
            // 
            // chkListCzesci
            // 
            this.chkListCzesci.CheckOnClick = true;
            this.chkListCzesci.Location = new System.Drawing.Point(6, 26);
            this.chkListCzesci.Name = "chkListCzesci";
            this.chkListCzesci.Size = new System.Drawing.Size(520, 158);
            this.chkListCzesci.TabIndex = 0;
            // 
            // cmbNowaCzescSzybka
            // 
            this.cmbNowaCzescSzybka.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbNowaCzescSzybka.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbNowaCzescSzybka.Location = new System.Drawing.Point(6, 188);
            this.cmbNowaCzescSzybka.Name = "cmbNowaCzescSzybka";
            this.cmbNowaCzescSzybka.Size = new System.Drawing.Size(380, 28);
            this.cmbNowaCzescSzybka.TabIndex = 1;
            // 
            // btnDodajCzescSzybka
            // 
            this.btnDodajCzescSzybka.Location = new System.Drawing.Point(392, 187);
            this.btnDodajCzescSzybka.Name = "btnDodajCzescSzybka";
            this.btnDodajCzescSzybka.Size = new System.Drawing.Size(134, 29);
            this.btnDodajCzescSzybka.TabIndex = 2;
            this.btnDodajCzescSzybka.Text = "Dodaj";
            this.btnDodajCzescSzybka.Click += new System.EventHandler(this.btnDodajCzescSzybka_Click);
            // 
            // lblPowiazanie
            // 
            this.lblPowiazanie.AutoSize = true;
            this.lblPowiazanie.ForeColor = System.Drawing.Color.Blue;
            this.lblPowiazanie.Location = new System.Drawing.Point(17, 30);
            this.lblPowiazanie.Name = "lblPowiazanie";
            this.lblPowiazanie.Size = new System.Drawing.Size(262, 20);
            this.lblPowiazanie.TabIndex = 9;
            this.lblPowiazanie.Text = "Sprzęt niepowiązany (Luźny)";
            // 
            // btnZapisz
            // 
            this.btnZapisz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnZapisz.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(3, 510);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(556, 50);
            this.btnZapisz.TabIndex = 8;
            this.btnZapisz.Text = "ZAPISZ W MAGAZYNIE";
            this.btnZapisz.UseVisualStyleBackColor = false;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);
            // 
            // txtUwagi
            // 
            this.txtUwagi.Location = new System.Drawing.Point(110, 190);
            this.txtUwagi.Multiline = true;
            this.txtUwagi.Name = "txtUwagi";
            this.txtUwagi.Size = new System.Drawing.Size(439, 70);
            this.txtUwagi.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Uwagi:";
            // 
            // cmbLokalizacja
            // 
            this.cmbLokalizacja.FormattingEnabled = true;
            this.cmbLokalizacja.Location = new System.Drawing.Point(110, 110);
            this.cmbLokalizacja.Name = "cmbLokalizacja";
            this.cmbLokalizacja.Size = new System.Drawing.Size(439, 28);
            this.cmbLokalizacja.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "Lokalizacja:";
            // 
            // txtSN
            // 
            this.txtSN.Location = new System.Drawing.Point(110, 70);
            this.txtSN.Name = "txtSN";
            this.txtSN.Size = new System.Drawing.Size(439, 27);
            this.txtSN.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "S/N:";
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(110, 110); // Błąd w designerze, poprawione w kodzie niżej
            this.txtModel.Location = new System.Drawing.Point(300, 30); // Tymczasowe
            this.txtModel.Visible = false; // Model pobieramy z bazy, użytkownik widzi label
            // 
            // txtModel (Poprawka)
            //
            this.txtModel.Location = new System.Drawing.Point(110, 60); // Przesunięte
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(439, 27);
            this.txtModel.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Model:";
            // 
            // FormDodajDoMagazynu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 763); // Wyższy
            this.Controls.Add(this.grpDane);
            this.Controls.Add(this.grpWyszukiwanie);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDodajDoMagazynu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Przyjęcie Sprzętu na Magazyn (Ręczne)";
            this.Load += new System.EventHandler(this.FormDodajDoMagazynu_Load);
            this.grpWyszukiwanie.ResumeLayout(false);
            this.grpWyszukiwanie.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWyniki)).EndInit();
            this.grpDane.ResumeLayout(false);
            this.grpDane.PerformLayout();
            this.grpCzesci.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox grpWyszukiwanie;
        private System.Windows.Forms.DataGridView dgvWyniki;
        private System.Windows.Forms.Button btnSzukaj;
        private System.Windows.Forms.TextBox txtSzukaj;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpDane;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.TextBox txtUwagi;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbLokalizacja;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblPowiazanie;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.GroupBox grpCzesci;
        private System.Windows.Forms.CheckedListBox chkListCzesci;
        private System.Windows.Forms.ComboBox cmbNowaCzescSzybka;
        private System.Windows.Forms.Button btnDodajCzescSzybka;
    }
}