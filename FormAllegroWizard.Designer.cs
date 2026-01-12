// Plik: FormAllegroWizard.Designer.cs
namespace Reklamacje_Dane
{
    partial class FormAllegroWizard
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblWyswietlanyKlient = new System.Windows.Forms.Label();
            this.lblWyswietlanyProdukt = new System.Windows.Forms.Label();
            this.lblAllegroNrZgloszenia = new System.Windows.Forms.Label();
            this.lblAllegroStatus = new System.Windows.Forms.Label();
            this.btnZatwierdzPrzypisanie = new System.Windows.Forms.Button();
            this.btnPomin = new System.Windows.Forms.Button();
            this.txtSzukajKlienta = new System.Windows.Forms.TextBox();
            this.txtSzukajProduktu = new System.Windows.Forms.TextBox();
            this.dgvKlientSuggestions = new System.Windows.Forms.DataGridView();
            this.dgvProduktSuggestions = new System.Windows.Forms.DataGridView();
            this.lblNrSeryjny = new System.Windows.Forms.Label();
            this.txtNrSeryjny = new System.Windows.Forms.TextBox();
            this.lblNrFaktury = new System.Windows.Forms.Label();
            this.txtNrFaktury = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKlientSuggestions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduktSuggestions)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nr zgłoszenia Allegro:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Status Allegro:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Przypisz klienta:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Przypisz produkt:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(300, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Wybrany klient:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(300, 192);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "Wybrany produkt:";
            // 
            // lblWyswietlanyKlient
            // 
            this.lblWyswietlanyKlient.AutoSize = true;
            this.lblWyswietlanyKlient.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblWyswietlanyKlient.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblWyswietlanyKlient.Location = new System.Drawing.Point(300, 122);
            this.lblWyswietlanyKlient.Name = "lblWyswietlanyKlient";
            this.lblWyswietlanyKlient.Size = new System.Drawing.Size(135, 20);
            this.lblWyswietlanyKlient.TabIndex = 6;
            this.lblWyswietlanyKlient.Text = "[Brak wybranego]";
            this.lblWyswietlanyKlient.Click += new System.EventHandler(this.lblWyswietlanyKlient_Click);
            // 
            // lblWyswietlanyProdukt
            // 
            this.lblWyswietlanyProdukt.AutoSize = true;
            this.lblWyswietlanyProdukt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblWyswietlanyProdukt.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblWyswietlanyProdukt.Location = new System.Drawing.Point(300, 212);
            this.lblWyswietlanyProdukt.Name = "lblWyswietlanyProdukt";
            this.lblWyswietlanyProdukt.Size = new System.Drawing.Size(135, 20);
            this.lblWyswietlanyProdukt.TabIndex = 7;
            this.lblWyswietlanyProdukt.Text = "[Brak wybranego]";
            // 
            // lblAllegroNrZgloszenia
            // 
            this.lblAllegroNrZgloszenia.AutoSize = true;
            this.lblAllegroNrZgloszenia.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAllegroNrZgloszenia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblAllegroNrZgloszenia.Location = new System.Drawing.Point(142, 12);
            this.lblAllegroNrZgloszenia.Name = "lblAllegroNrZgloszenia";
            this.lblAllegroNrZgloszenia.Size = new System.Drawing.Size(96, 20);
            this.lblAllegroNrZgloszenia.TabIndex = 8;
            this.lblAllegroNrZgloszenia.Text = "Ładowanie...";
            // 
            // lblAllegroStatus
            // 
            this.lblAllegroStatus.AutoSize = true;
            this.lblAllegroStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAllegroStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblAllegroStatus.Location = new System.Drawing.Point(142, 42);
            this.lblAllegroStatus.Name = "lblAllegroStatus";
            this.lblAllegroStatus.Size = new System.Drawing.Size(96, 20);
            this.lblAllegroStatus.TabIndex = 9;
            this.lblAllegroStatus.Text = "Ładowanie...";
            // 
            // btnZatwierdzPrzypisanie
            // 
            this.btnZatwierdzPrzypisanie.BackColor = System.Drawing.Color.ForestGreen;
            this.btnZatwierdzPrzypisanie.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZatwierdzPrzypisanie.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZatwierdzPrzypisanie.ForeColor = System.Drawing.Color.White;
            this.btnZatwierdzPrzypisanie.Location = new System.Drawing.Point(403, 396);
            this.btnZatwierdzPrzypisanie.Name = "btnZatwierdzPrzypisanie";
            this.btnZatwierdzPrzypisanie.Size = new System.Drawing.Size(180, 35);
            this.btnZatwierdzPrzypisanie.TabIndex = 10;
            this.btnZatwierdzPrzypisanie.Text = "Zatwierdź i dodaj";
            this.btnZatwierdzPrzypisanie.UseVisualStyleBackColor = false;
            this.btnZatwierdzPrzypisanie.Click += new System.EventHandler(this.btnZatwierdzPrzypisanie_Click_Async);
            // 
            // btnPomin
            // 
            this.btnPomin.BackColor = System.Drawing.Color.Red;
            this.btnPomin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPomin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPomin.ForeColor = System.Drawing.Color.White;
            this.btnPomin.Location = new System.Drawing.Point(12, 396);
            this.btnPomin.Name = "btnPomin";
            this.btnPomin.Size = new System.Drawing.Size(180, 35);
            this.btnPomin.TabIndex = 11;
            this.btnPomin.Text = "Pomiń i zamknij";
            this.btnPomin.UseVisualStyleBackColor = false;
            this.btnPomin.Click += new System.EventHandler(this.btnPomin_Click_Async);
            // 
            // txtSzukajKlienta
            // 
            this.txtSzukajKlienta.Location = new System.Drawing.Point(12, 125);
            this.txtSzukajKlienta.Name = "txtSzukajKlienta";
            this.txtSzukajKlienta.Size = new System.Drawing.Size(262, 27);
            this.txtSzukajKlienta.TabIndex = 12;
            this.txtSzukajKlienta.TextChanged += new System.EventHandler(this.txtSzukajKlienta_TextChanged);
            this.txtSzukajKlienta.Leave += new System.EventHandler(this.txtSzukajKlienta_Leave);
            // 
            // txtSzukajProduktu
            // 
            this.txtSzukajProduktu.Location = new System.Drawing.Point(12, 215);
            this.txtSzukajProduktu.Name = "txtSzukajProduktu";
            this.txtSzukajProduktu.Size = new System.Drawing.Size(262, 27);
            this.txtSzukajProduktu.TabIndex = 13;
            this.txtSzukajProduktu.TextChanged += new System.EventHandler(this.txtSzukajProduktu_TextChanged);
            this.txtSzukajProduktu.Leave += new System.EventHandler(this.txtSzukajProduktu_Leave);
            // 
            // dgvKlientSuggestions
            // 
            this.dgvKlientSuggestions.AllowUserToAddRows = false;
            this.dgvKlientSuggestions.AllowUserToDeleteRows = false;
            this.dgvKlientSuggestions.AllowUserToResizeRows = false;
            this.dgvKlientSuggestions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvKlientSuggestions.BackgroundColor = System.Drawing.Color.White;
            this.dgvKlientSuggestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKlientSuggestions.ColumnHeadersVisible = false;
            this.dgvKlientSuggestions.Location = new System.Drawing.Point(12, 158);
            this.dgvKlientSuggestions.MultiSelect = false;
            this.dgvKlientSuggestions.Name = "dgvKlientSuggestions";
            this.dgvKlientSuggestions.ReadOnly = true;
            this.dgvKlientSuggestions.RowHeadersVisible = false;
            this.dgvKlientSuggestions.RowHeadersWidth = 51;
            this.dgvKlientSuggestions.RowTemplate.Height = 24;
            this.dgvKlientSuggestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKlientSuggestions.Size = new System.Drawing.Size(262, 100);
            this.dgvKlientSuggestions.TabIndex = 14;
            this.dgvKlientSuggestions.Visible = false;
            this.dgvKlientSuggestions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvKlientSuggestions_CellClick);
            // 
            // dgvProduktSuggestions
            // 
            this.dgvProduktSuggestions.AllowUserToAddRows = false;
            this.dgvProduktSuggestions.AllowUserToDeleteRows = false;
            this.dgvProduktSuggestions.AllowUserToResizeRows = false;
            this.dgvProduktSuggestions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProduktSuggestions.BackgroundColor = System.Drawing.Color.White;
            this.dgvProduktSuggestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProduktSuggestions.ColumnHeadersVisible = false;
            this.dgvProduktSuggestions.Location = new System.Drawing.Point(12, 248);
            this.dgvProduktSuggestions.MultiSelect = false;
            this.dgvProduktSuggestions.Name = "dgvProduktSuggestions";
            this.dgvProduktSuggestions.ReadOnly = true;
            this.dgvProduktSuggestions.RowHeadersVisible = false;
            this.dgvProduktSuggestions.RowHeadersWidth = 51;
            this.dgvProduktSuggestions.RowTemplate.Height = 24;
            this.dgvProduktSuggestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProduktSuggestions.Size = new System.Drawing.Size(262, 100);
            this.dgvProduktSuggestions.TabIndex = 15;
            this.dgvProduktSuggestions.Visible = false;
            this.dgvProduktSuggestions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProduktSuggestions_CellClick);
            // 
            // lblNrSeryjny
            // 
            this.lblNrSeryjny.AutoSize = true;
            this.lblNrSeryjny.Location = new System.Drawing.Point(300, 260);
            this.lblNrSeryjny.Name = "lblNrSeryjny";
            this.lblNrSeryjny.Size = new System.Drawing.Size(79, 20);
            this.lblNrSeryjny.TabIndex = 16;
            this.lblNrSeryjny.Text = "Nr Seryjny:";
            // 
            // txtNrSeryjny
            // 
            this.txtNrSeryjny.Location = new System.Drawing.Point(300, 283);
            this.txtNrSeryjny.Name = "txtNrSeryjny";
            this.txtNrSeryjny.Size = new System.Drawing.Size(262, 27);
            this.txtNrSeryjny.TabIndex = 17;
            // 
            // lblNrFaktury
            // 
            this.lblNrFaktury.AutoSize = true;
            this.lblNrFaktury.Location = new System.Drawing.Point(300, 313);
            this.lblNrFaktury.Name = "lblNrFaktury";
            this.lblNrFaktury.Size = new System.Drawing.Size(78, 20);
            this.lblNrFaktury.TabIndex = 18;
            this.lblNrFaktury.Text = "Nr Faktury:";
            // 
            // txtNrFaktury
            // 
            this.txtNrFaktury.Location = new System.Drawing.Point(300, 336);
            this.txtNrFaktury.Name = "txtNrFaktury";
            this.txtNrFaktury.Size = new System.Drawing.Size(262, 27);
            this.txtNrFaktury.TabIndex = 19;
            // 
            // FormAllegroWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(595, 443);
            this.Controls.Add(this.txtNrFaktury);
            this.Controls.Add(this.lblNrFaktury);
            this.Controls.Add(this.txtNrSeryjny);
            this.Controls.Add(this.lblNrSeryjny);
            this.Controls.Add(this.dgvKlientSuggestions);
            this.Controls.Add(this.dgvProduktSuggestions);
            this.Controls.Add(this.txtSzukajProduktu);
            this.Controls.Add(this.txtSzukajKlienta);
            this.Controls.Add(this.btnPomin);
            this.Controls.Add(this.btnZatwierdzPrzypisanie);
            this.Controls.Add(this.lblAllegroStatus);
            this.Controls.Add(this.lblAllegroNrZgloszenia);
            this.Controls.Add(this.lblWyswietlanyProdukt);
            this.Controls.Add(this.lblWyswietlanyKlient);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAllegroWizard";
            this.Text = "Kreator dodawania zgłoszeń Allegro";
            this.Load += new System.EventHandler(this.FormAllegroWizard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKlientSuggestions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduktSuggestions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblWyswietlanyKlient;
        private System.Windows.Forms.Label lblWyswietlanyProdukt;
        private System.Windows.Forms.Label lblAllegroNrZgloszenia;
        private System.Windows.Forms.Label lblAllegroStatus;
        private System.Windows.Forms.Button btnZatwierdzPrzypisanie;
     //   private System.Windows.Forms.Button btnPomin;
        private System.Windows.Forms.TextBox txtSzukajKlienta;
        private System.Windows.Forms.TextBox txtSzukajProduktu;
        private System.Windows.Forms.DataGridView dgvKlientSuggestions;
        private System.Windows.Forms.DataGridView dgvProduktSuggestions;
        private System.Windows.Forms.Label lblNrSeryjny;
        private System.Windows.Forms.TextBox txtNrSeryjny;
        private System.Windows.Forms.Label lblNrFaktury;
        private System.Windows.Forms.TextBox txtNrFaktury;
    }
}