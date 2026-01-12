namespace Reklamacje_Dane
{
    partial class FormDodajNadwyzke
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtProdukt = new System.Windows.Forms.TextBox();
            this.btnSzukajProduktu = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbCzesc = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numIlosc = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbLokalizacja = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUwagi = new System.Windows.Forms.TextBox();
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numIlosc)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wybierz Produkt (Model):";
            // 
            // txtProdukt
            // 
            this.txtProdukt.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtProdukt.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtProdukt.Location = new System.Drawing.Point(24, 43);
            this.txtProdukt.Name = "txtProdukt";
            this.txtProdukt.ReadOnly = true;
            this.txtProdukt.Size = new System.Drawing.Size(250, 30);
            this.txtProdukt.TabIndex = 1;
            this.txtProdukt.Text = "Kliknij lupę ->";
            // 
            // btnSzukajProduktu
            // 
            this.btnSzukajProduktu.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnSzukajProduktu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSzukajProduktu.ForeColor = System.Drawing.Color.White;
            this.btnSzukajProduktu.Location = new System.Drawing.Point(280, 42);
            this.btnSzukajProduktu.Name = "btnSzukajProduktu";
            this.btnSzukajProduktu.Size = new System.Drawing.Size(74, 32);
            this.btnSzukajProduktu.TabIndex = 2;
            this.btnSzukajProduktu.Text = "🔍";
            this.btnSzukajProduktu.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(20, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Nazwa Części:";
            // 
            // cmbCzesc
            // 
            this.cmbCzesc.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbCzesc.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbCzesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCzesc.FormattingEnabled = true;
            this.cmbCzesc.Location = new System.Drawing.Point(24, 113);
            this.cmbCzesc.Name = "cmbCzesc";
            this.cmbCzesc.Size = new System.Drawing.Size(330, 31);
            this.cmbCzesc.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ilość sztuk:";
            // 
            // numIlosc
            // 
            this.numIlosc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numIlosc.Location = new System.Drawing.Point(24, 183);
            this.numIlosc.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numIlosc.Name = "numIlosc";
            this.numIlosc.Size = new System.Drawing.Size(100, 30);
            this.numIlosc.TabIndex = 6;
            this.numIlosc.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(150, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Lokalizacja:";
            // 
            // cmbLokalizacja
            // 
            this.cmbLokalizacja.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbLokalizacja.FormattingEnabled = true;
            this.cmbLokalizacja.Location = new System.Drawing.Point(154, 182);
            this.cmbLokalizacja.Name = "cmbLokalizacja";
            this.cmbLokalizacja.Size = new System.Drawing.Size(200, 31);
            this.cmbLokalizacja.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 230);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(161, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Uwagi (skąd pochodzi):";
            // 
            // txtUwagi
            // 
            this.txtUwagi.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUwagi.Location = new System.Drawing.Point(24, 253);
            this.txtUwagi.Name = "txtUwagi";
            this.txtUwagi.Size = new System.Drawing.Size(330, 30);
            this.txtUwagi.TabIndex = 10;
            // 
            // btnZapisz
            // 
            this.btnZapisz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(204, 310);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(150, 45);
            this.btnZapisz.TabIndex = 11;
            this.btnZapisz.Text = "ZAPISZ NA STAN";
            this.btnZapisz.UseVisualStyleBackColor = false;
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnuluj.Location = new System.Drawing.Point(24, 310);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(100, 45);
            this.btnAnuluj.TabIndex = 12;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            // 
            // FormDodajNadwyzke
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(382, 383);
            this.Controls.Add(this.btnAnuluj);
            this.Controls.Add(this.btnZapisz);
            this.Controls.Add(this.txtUwagi);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbLokalizacja);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numIlosc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbCzesc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSzukajProduktu);
            this.Controls.Add(this.txtProdukt);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDodajNadwyzke";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Przyjęcie Nadwyżki / Gratisu";
            ((System.ComponentModel.ISupportInitialize)(this.numIlosc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProdukt;
        private System.Windows.Forms.Button btnSzukajProduktu;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbCzesc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numIlosc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbLokalizacja;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUwagi;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.Button btnAnuluj;
    }
}