// Plik: Faktura.Designer.cs
namespace Reklamacje_Dane
{
    partial class FakturaForm
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
            this.Rodzaj = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.NrFaktury = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxCurrency = new System.Windows.Forms.ComboBox();
            this.chkBezVAT = new System.Windows.Forms.CheckBox();
            this.numBrutto = new System.Windows.Forms.NumericUpDown();
            this.numNetto = new System.Windows.Forms.NumericUpDown();
            this.pnlKosztowa = new System.Windows.Forms.Panel();
            this.Odkogo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numBrutto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNetto)).BeginInit();
            this.pnlKosztowa.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(32, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rodzaj";
            // 
            // Rodzaj
            // 
            this.Rodzaj.BackColor = System.Drawing.Color.White;
            this.Rodzaj.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Rodzaj.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Rodzaj.FormattingEnabled = true;
            this.Rodzaj.Items.AddRange(new object[] {
            "Zysk",
            "Kosztowa"});
            this.Rodzaj.Location = new System.Drawing.Point(107, 20);
            this.Rodzaj.Name = "Rodzaj";
            this.Rodzaj.Size = new System.Drawing.Size(200, 28);
            this.Rodzaj.TabIndex = 1;
            this.Rodzaj.SelectedIndexChanged += new System.EventHandler(this.Rodzaj_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(2, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Numer Faktury";
            // 
            // NrFaktury
            // 
            this.NrFaktury.BackColor = System.Drawing.Color.White;
            this.NrFaktury.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NrFaktury.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.NrFaktury.Location = new System.Drawing.Point(107, 103);
            this.NrFaktury.Name = "NrFaktury";
            this.NrFaktury.Size = new System.Drawing.Size(200, 27);
            this.NrFaktury.TabIndex = 3;
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(131, 237);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(100, 33);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Dodaj";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.Location = new System.Drawing.Point(103, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Kwota brutto";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.Location = new System.Drawing.Point(215, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Kwota Netto";
            // 
            // comboBoxCurrency
            // 
            this.comboBoxCurrency.BackColor = System.Drawing.Color.White;
            this.comboBoxCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCurrency.FormattingEnabled = true;
            this.comboBoxCurrency.Items.AddRange(new object[] {
            "PLN",
            "EUR",
            "USD",
            "GBP",
            "CHF"});
            this.comboBoxCurrency.Location = new System.Drawing.Point(12, 156);
            this.comboBoxCurrency.Name = "comboBoxCurrency";
            this.comboBoxCurrency.Size = new System.Drawing.Size(89, 28);
            this.comboBoxCurrency.TabIndex = 11;
            // 
            // chkBezVAT
            // 
            this.chkBezVAT.AutoSize = true;
            this.chkBezVAT.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkBezVAT.Location = new System.Drawing.Point(107, 196);
            this.chkBezVAT.Name = "chkBezVAT";
            this.chkBezVAT.Size = new System.Drawing.Size(126, 24);
            this.chkBezVAT.TabIndex = 12;
            this.chkBezVAT.Text = "Faktura bez VAT";
            this.chkBezVAT.UseVisualStyleBackColor = true;
            this.chkBezVAT.CheckedChanged += new System.EventHandler(this.chkBezVAT_CheckedChanged);
            // 
            // numBrutto
            // 
            this.numBrutto.BackColor = System.Drawing.Color.White;
            this.numBrutto.DecimalPlaces = 2;
            this.numBrutto.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numBrutto.Location = new System.Drawing.Point(107, 157);
            this.numBrutto.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numBrutto.Name = "numBrutto";
            this.numBrutto.Size = new System.Drawing.Size(91, 27);
            this.numBrutto.TabIndex = 13;
            this.numBrutto.ThousandsSeparator = true;
            this.numBrutto.ValueChanged += new System.EventHandler(this.numBrutto_ValueChanged);
            // 
            // numNetto
            // 
            this.numNetto.BackColor = System.Drawing.Color.White;
            this.numNetto.DecimalPlaces = 2;
            this.numNetto.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numNetto.Location = new System.Drawing.Point(204, 157);
            this.numNetto.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numNetto.Name = "numNetto";
            this.numNetto.Size = new System.Drawing.Size(103, 27);
            this.numNetto.TabIndex = 14;
            this.numNetto.ThousandsSeparator = true;
            this.numNetto.ValueChanged += new System.EventHandler(this.numNetto_ValueChanged);
            // 
            // pnlKosztowa
            // 
            this.pnlKosztowa.Controls.Add(this.Odkogo);
            this.pnlKosztowa.Controls.Add(this.label3);
            this.pnlKosztowa.Location = new System.Drawing.Point(3, 52);
            this.pnlKosztowa.Name = "pnlKosztowa";
            this.pnlKosztowa.Size = new System.Drawing.Size(332, 40);
            this.pnlKosztowa.TabIndex = 15;
            this.pnlKosztowa.Visible = false;
            // 
            // Odkogo
            // 
            this.Odkogo.BackColor = System.Drawing.Color.White;
            this.Odkogo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Odkogo.FormattingEnabled = true;
            this.Odkogo.Location = new System.Drawing.Point(104, 5);
            this.Odkogo.Name = "Odkogo";
            this.Odkogo.Size = new System.Drawing.Size(200, 28);
            this.Odkogo.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(10, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Od kogo?";
            // 
            // FakturaForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(346, 282);
            this.Controls.Add(this.pnlKosztowa);
            this.Controls.Add(this.numNetto);
            this.Controls.Add(this.numBrutto);
            this.Controls.Add(this.chkBezVAT);
            this.Controls.Add(this.comboBoxCurrency);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.NrFaktury);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Rodzaj);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FakturaForm";
            this.Text = "Dodaj fakturę";
            this.Load += new System.EventHandler(this.FakturaForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numBrutto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNetto)).EndInit();
            this.pnlKosztowa.ResumeLayout(false);
            this.pnlKosztowa.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Rodzaj;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NrFaktury;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxCurrency;
        private System.Windows.Forms.CheckBox chkBezVAT;
        private System.Windows.Forms.NumericUpDown numBrutto;
        private System.Windows.Forms.NumericUpDown numNetto;
        private System.Windows.Forms.Panel pnlKosztowa;
        private System.Windows.Forms.ComboBox Odkogo;
        private System.Windows.Forms.Label label3;
    }
}