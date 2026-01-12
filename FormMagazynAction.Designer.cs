namespace Reklamacje_Dane
{
    partial class FormMagazynAction
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDane = new System.Windows.Forms.TabPage();
            this.lblInfoProdukt = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboStatus = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbLokalizacja = new System.Windows.Forms.ComboBox();
            this.btnDodajLok = new System.Windows.Forms.Button();
            this.btnUsunLok = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUwagi = new System.Windows.Forms.TextBox();
            this.grpCzesci = new System.Windows.Forms.GroupBox();
            this.lblBrakSzablonu = new System.Windows.Forms.Label();
            this.chkListCzesci = new System.Windows.Forms.CheckedListBox();
            this.cmbNowaCzescSzybka = new System.Windows.Forms.ComboBox();
            this.btnDodajCzescSzybka = new System.Windows.Forms.Button();
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.tabZdjecia = new System.Windows.Forms.TabPage();
            this.flowZdjecia = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDodajZdjecie = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabDane.SuspendLayout();
            this.grpCzesci.SuspendLayout();
            this.tabZdjecia.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDane);
            this.tabControl1.Controls.Add(this.tabZdjecia);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(500, 600);
            this.tabControl1.TabIndex = 0;
            // 
            // tabDane
            // 
            this.tabDane.Controls.Add(this.btnAnuluj);
            this.tabDane.Controls.Add(this.btnZapisz);
            this.tabDane.Controls.Add(this.grpCzesci);
            this.tabDane.Controls.Add(this.txtUwagi);
            this.tabDane.Controls.Add(this.label3);
            this.tabDane.Controls.Add(this.btnUsunLok);
            this.tabDane.Controls.Add(this.btnDodajLok);
            this.tabDane.Controls.Add(this.cmbLokalizacja);
            this.tabDane.Controls.Add(this.label2);
            this.tabDane.Controls.Add(this.comboStatus);
            this.tabDane.Controls.Add(this.label1);
            this.tabDane.Controls.Add(this.lblInfoProdukt);
            this.tabDane.Location = new System.Drawing.Point(4, 25);
            this.tabDane.Name = "tabDane";
            this.tabDane.Padding = new System.Windows.Forms.Padding(3);
            this.tabDane.Size = new System.Drawing.Size(492, 571);
            this.tabDane.TabIndex = 0;
            this.tabDane.Text = "Dane i Części";
            this.tabDane.UseVisualStyleBackColor = true;
            // 
            // lblInfoProdukt
            // 
            this.lblInfoProdukt.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lblInfoProdukt.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblInfoProdukt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblInfoProdukt.Location = new System.Drawing.Point(3, 3);
            this.lblInfoProdukt.Name = "lblInfoProdukt";
            this.lblInfoProdukt.Padding = new System.Windows.Forms.Padding(10);
            this.lblInfoProdukt.Size = new System.Drawing.Size(486, 60);
            this.lblInfoProdukt.TabIndex = 0;
            this.lblInfoProdukt.Text = "Produkt: ...\r\nS/N: ...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Status fizyczny:";
            // 
            // comboStatus
            // 
            this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStatus.FormattingEnabled = true;
            this.comboStatus.Location = new System.Drawing.Point(16, 98);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(456, 28);
            this.comboStatus.TabIndex = 2;
            this.comboStatus.SelectedIndexChanged += new System.EventHandler(this.comboStatus_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lokalizacja:";
            // 
            // cmbLokalizacja
            // 
            this.cmbLokalizacja.FormattingEnabled = true;
            this.cmbLokalizacja.Location = new System.Drawing.Point(16, 165);
            this.cmbLokalizacja.Name = "cmbLokalizacja";
            this.cmbLokalizacja.Size = new System.Drawing.Size(350, 28);
            this.cmbLokalizacja.TabIndex = 4;
            // 
            // btnDodajLok
            // 
            this.btnDodajLok.Location = new System.Drawing.Point(372, 164);
            this.btnDodajLok.Name = "btnDodajLok";
            this.btnDodajLok.Size = new System.Drawing.Size(45, 29);
            this.btnDodajLok.TabIndex = 5;
            this.btnDodajLok.Text = "+";
            this.btnDodajLok.UseVisualStyleBackColor = true;
            this.btnDodajLok.Click += new System.EventHandler(this.btnDodajLok_Click);
            // 
            // btnUsunLok
            // 
            this.btnUsunLok.Location = new System.Drawing.Point(423, 164);
            this.btnUsunLok.Name = "btnUsunLok";
            this.btnUsunLok.Size = new System.Drawing.Size(45, 29);
            this.btnUsunLok.TabIndex = 6;
            this.btnUsunLok.Text = "-";
            this.btnUsunLok.UseVisualStyleBackColor = true;
            this.btnUsunLok.Click += new System.EventHandler(this.btnUsunLok_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 205);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Uwagi:";
            // 
            // txtUwagi
            // 
            this.txtUwagi.Location = new System.Drawing.Point(16, 228);
            this.txtUwagi.Multiline = true;
            this.txtUwagi.Name = "txtUwagi";
            this.txtUwagi.Size = new System.Drawing.Size(456, 60);
            this.txtUwagi.TabIndex = 8;
            // 
            // grpCzesci
            // 
            this.grpCzesci.Controls.Add(this.lblBrakSzablonu);
            this.grpCzesci.Controls.Add(this.chkListCzesci);
            this.grpCzesci.Controls.Add(this.cmbNowaCzescSzybka);
            this.grpCzesci.Controls.Add(this.btnDodajCzescSzybka);
            this.grpCzesci.Location = new System.Drawing.Point(16, 304);
            this.grpCzesci.Name = "grpCzesci";
            this.grpCzesci.Size = new System.Drawing.Size(456, 190);
            this.grpCzesci.TabIndex = 9;
            this.grpCzesci.TabStop = false;
            this.grpCzesci.Text = "Odzysk Części (Zaznacz sprawne)";
            this.grpCzesci.Visible = false;
            // 
            // lblBrakSzablonu
            // 
            this.lblBrakSzablonu.BackColor = System.Drawing.Color.MistyRose;
            this.lblBrakSzablonu.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBrakSzablonu.ForeColor = System.Drawing.Color.DarkRed;
            this.lblBrakSzablonu.Location = new System.Drawing.Point(3, 23);
            this.lblBrakSzablonu.Name = "lblBrakSzablonu";
            this.lblBrakSzablonu.Size = new System.Drawing.Size(450, 40);
            this.lblBrakSzablonu.TabIndex = 3;
            this.lblBrakSzablonu.Text = "Lista części jest pusta.";
            this.lblBrakSzablonu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBrakSzablonu.Visible = false;
            // 
            // chkListCzesci
            // 
            this.chkListCzesci.CheckOnClick = true;
            this.chkListCzesci.Location = new System.Drawing.Point(3, 23);
            this.chkListCzesci.Name = "chkListCzesci";
            this.chkListCzesci.Size = new System.Drawing.Size(450, 130);
            this.chkListCzesci.TabIndex = 0;
            // 
            // cmbNowaCzescSzybka
            // 
            this.cmbNowaCzescSzybka.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbNowaCzescSzybka.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbNowaCzescSzybka.FormattingEnabled = true;
            this.cmbNowaCzescSzybka.Location = new System.Drawing.Point(6, 158);
            this.cmbNowaCzescSzybka.Name = "cmbNowaCzescSzybka";
            this.cmbNowaCzescSzybka.Size = new System.Drawing.Size(300, 28);
            this.cmbNowaCzescSzybka.TabIndex = 1;
            // 
            // btnDodajCzescSzybka
            // 
            this.btnDodajCzescSzybka.Location = new System.Drawing.Point(312, 157);
            this.btnDodajCzescSzybka.Name = "btnDodajCzescSzybka";
            this.btnDodajCzescSzybka.Size = new System.Drawing.Size(138, 29);
            this.btnDodajCzescSzybka.TabIndex = 2;
            this.btnDodajCzescSzybka.Text = "Dodaj do listy";
            this.btnDodajCzescSzybka.UseVisualStyleBackColor = true;
            this.btnDodajCzescSzybka.Click += new System.EventHandler(this.btnDodajCzescSzybka_Click);
            // 
            // btnZapisz
            // 
            this.btnZapisz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(352, 510);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(120, 40);
            this.btnZapisz.TabIndex = 10;
            this.btnZapisz.Text = "Zapisz";
            this.btnZapisz.UseVisualStyleBackColor = false;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnuluj.Location = new System.Drawing.Point(226, 510);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(120, 40);
            this.btnAnuluj.TabIndex = 11;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            this.btnAnuluj.Click += new System.EventHandler(this.btnAnuluj_Click);
            // 
            // tabZdjecia
            // 
            this.tabZdjecia.Controls.Add(this.flowZdjecia);
            this.tabZdjecia.Controls.Add(this.btnDodajZdjecie);
            this.tabZdjecia.Location = new System.Drawing.Point(4, 25);
            this.tabZdjecia.Name = "tabZdjecia";
            this.tabZdjecia.Padding = new System.Windows.Forms.Padding(3);
            this.tabZdjecia.Size = new System.Drawing.Size(492, 571);
            this.tabZdjecia.TabIndex = 1;
            this.tabZdjecia.Text = "Zdjęcia / Dokumentacja";
            this.tabZdjecia.UseVisualStyleBackColor = true;
            // 
            // flowZdjecia
            // 
            this.flowZdjecia.AutoScroll = true;
            this.flowZdjecia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowZdjecia.Location = new System.Drawing.Point(3, 3);
            this.flowZdjecia.Name = "flowZdjecia";
            this.flowZdjecia.Size = new System.Drawing.Size(486, 525);
            this.flowZdjecia.TabIndex = 0;
            // 
            // btnDodajZdjecie
            // 
            this.btnDodajZdjecie.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnDodajZdjecie.Location = new System.Drawing.Point(3, 528);
            this.btnDodajZdjecie.Name = "btnDodajZdjecie";
            this.btnDodajZdjecie.Size = new System.Drawing.Size(486, 40);
            this.btnDodajZdjecie.TabIndex = 1;
            this.btnDodajZdjecie.Text = "+ Dodaj zdjęcie z pliku";
            this.btnDodajZdjecie.UseVisualStyleBackColor = true;
            this.btnDodajZdjecie.Click += new System.EventHandler(this.btnDodajZdjecie_Click);
            // 
            // FormMagazynAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 600);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMagazynAction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Akcja Magazynowa";
            this.tabControl1.ResumeLayout(false);
            this.tabDane.ResumeLayout(false);
            this.tabDane.PerformLayout();
            this.grpCzesci.ResumeLayout(false);
            this.tabZdjecia.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDane;
        private System.Windows.Forms.TabPage tabZdjecia;
        private System.Windows.Forms.Label lblInfoProdukt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbLokalizacja;
        private System.Windows.Forms.Button btnDodajLok;
        private System.Windows.Forms.Button btnUsunLok;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUwagi;
        private System.Windows.Forms.GroupBox grpCzesci;
        private System.Windows.Forms.CheckedListBox chkListCzesci;
        private System.Windows.Forms.Label lblBrakSzablonu;
        private System.Windows.Forms.ComboBox cmbNowaCzescSzybka;
        private System.Windows.Forms.Button btnDodajCzescSzybka;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.FlowLayoutPanel flowZdjecia;
        private System.Windows.Forms.Button btnDodajZdjecie;
    }
}