

using System.Drawing;

namespace Reklamacje_Dane
{
    partial class Form9
    {
        private System.ComponentModel.IContainer components = null;

        // Deklaracja kontrolek
        private System.Windows.Forms.Label labelKodProduktuHella;
        private System.Windows.Forms.TextBox textBoxKodProduktuHella;
        private System.Windows.Forms.Label labelNumerFVZakupowej;
        private System.Windows.Forms.TextBox textBoxNumerFVZakupowej;
        private System.Windows.Forms.Label labelDataFVZakupowej;
        private System.Windows.Forms.DateTimePicker dateTimePickerDataFVZakupowej;
        private System.Windows.Forms.Label labelOpisUsterki;
        private System.Windows.Forms.TextBox textBoxOpisUsterki;
        private System.Windows.Forms.Label labelOkolicznoscWady;
        private System.Windows.Forms.TextBox textBoxOkolicznoscWady;
        private System.Windows.Forms.Label labelUwagi;
        private System.Windows.Forms.Button buttonWyslij;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.Button button1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta Windows Forms

        private void InitializeComponent()
        {
            this.labelKodProduktuHella = new System.Windows.Forms.Label();
            this.textBoxKodProduktuHella = new System.Windows.Forms.TextBox();
            this.labelNumerFVZakupowej = new System.Windows.Forms.Label();
            this.textBoxNumerFVZakupowej = new System.Windows.Forms.TextBox();
            this.labelDataFVZakupowej = new System.Windows.Forms.Label();
            this.dateTimePickerDataFVZakupowej = new System.Windows.Forms.DateTimePicker();
            this.labelOpisUsterki = new System.Windows.Forms.Label();
            this.textBoxOpisUsterki = new System.Windows.Forms.TextBox();
            this.labelOkolicznoscWady = new System.Windows.Forms.Label();
            this.textBoxOkolicznoscWady = new System.Windows.Forms.TextBox();
            this.labelUwagi = new System.Windows.Forms.Label();
            this.buttonWyslij = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.SuspendLayout();
            // 
            // labelKodProduktuHella
            // 
            this.labelKodProduktuHella.AutoSize = true;
            this.labelKodProduktuHella.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelKodProduktuHella.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelKodProduktuHella.Location = new System.Drawing.Point(12, 15);
            this.labelKodProduktuHella.Name = "labelKodProduktuHella";
            this.labelKodProduktuHella.Size = new System.Drawing.Size(156, 20);
            this.labelKodProduktuHella.TabIndex = 0;
            this.labelKodProduktuHella.Text = "Kod produktu Strands:";
            // 
            // textBoxKodProduktuHella
            // 
            this.textBoxKodProduktuHella.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxKodProduktuHella.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxKodProduktuHella.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxKodProduktuHella.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxKodProduktuHella.Location = new System.Drawing.Point(174, 12);
            this.textBoxKodProduktuHella.Name = "textBoxKodProduktuHella";
            this.textBoxKodProduktuHella.Size = new System.Drawing.Size(300, 27);
            this.textBoxKodProduktuHella.TabIndex = 1;
            // 
            // labelNumerFVZakupowej
            // 
            this.labelNumerFVZakupowej.AutoSize = true;
            this.labelNumerFVZakupowej.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelNumerFVZakupowej.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelNumerFVZakupowej.Location = new System.Drawing.Point(12, 45);
            this.labelNumerFVZakupowej.Name = "labelNumerFVZakupowej";
            this.labelNumerFVZakupowej.Size = new System.Drawing.Size(152, 20);
            this.labelNumerFVZakupowej.TabIndex = 2;
            this.labelNumerFVZakupowej.Text = "Numer FV zakupowej:";
            // 
            // textBoxNumerFVZakupowej
            // 
            this.textBoxNumerFVZakupowej.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxNumerFVZakupowej.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxNumerFVZakupowej.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxNumerFVZakupowej.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxNumerFVZakupowej.Location = new System.Drawing.Point(174, 43);
            this.textBoxNumerFVZakupowej.Name = "textBoxNumerFVZakupowej";
            this.textBoxNumerFVZakupowej.Size = new System.Drawing.Size(300, 27);
            this.textBoxNumerFVZakupowej.TabIndex = 3;
            // 
            // labelDataFVZakupowej
            // 
            this.labelDataFVZakupowej.AutoSize = true;
            this.labelDataFVZakupowej.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelDataFVZakupowej.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelDataFVZakupowej.Location = new System.Drawing.Point(12, 75);
            this.labelDataFVZakupowej.Name = "labelDataFVZakupowej";
            this.labelDataFVZakupowej.Size = new System.Drawing.Size(139, 20);
            this.labelDataFVZakupowej.TabIndex = 4;
            this.labelDataFVZakupowej.Text = "Data FV zakupowej:";
            // 
            // dateTimePickerDataFVZakupowej
            // 
            this.dateTimePickerDataFVZakupowej.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dateTimePickerDataFVZakupowej.Location = new System.Drawing.Point(174, 75);
            this.dateTimePickerDataFVZakupowej.Name = "dateTimePickerDataFVZakupowej";
            this.dateTimePickerDataFVZakupowej.Size = new System.Drawing.Size(300, 27);
            this.dateTimePickerDataFVZakupowej.TabIndex = 5;
            // 
            // labelOpisUsterki
            // 
            this.labelOpisUsterki.AutoSize = true;
            this.labelOpisUsterki.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelOpisUsterki.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelOpisUsterki.Location = new System.Drawing.Point(12, 105);
            this.labelOpisUsterki.Name = "labelOpisUsterki";
            this.labelOpisUsterki.Size = new System.Drawing.Size(89, 20);
            this.labelOpisUsterki.TabIndex = 6;
            this.labelOpisUsterki.Text = "Opis usterki:";
            // 
            // textBoxOpisUsterki
            // 
            this.textBoxOpisUsterki.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxOpisUsterki.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOpisUsterki.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOpisUsterki.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxOpisUsterki.Location = new System.Drawing.Point(174, 105);
            this.textBoxOpisUsterki.Multiline = true;
            this.textBoxOpisUsterki.Name = "textBoxOpisUsterki";
            this.textBoxOpisUsterki.Size = new System.Drawing.Size(300, 60);
            this.textBoxOpisUsterki.TabIndex = 7;
            // 
            // labelOkolicznoscWady
            // 
            this.labelOkolicznoscWady.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelOkolicznoscWady.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelOkolicznoscWady.Location = new System.Drawing.Point(12, 206);
            this.labelOkolicznoscWady.Name = "labelOkolicznoscWady";
            this.labelOkolicznoscWady.Size = new System.Drawing.Size(125, 41);
            this.labelOkolicznoscWady.TabIndex = 8;
            this.labelOkolicznoscWady.Text = "Numer z kabla:";
            // 
            // textBoxOkolicznoscWady
            // 
            this.textBoxOkolicznoscWady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxOkolicznoscWady.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOkolicznoscWady.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOkolicznoscWady.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxOkolicznoscWady.Location = new System.Drawing.Point(174, 204);
            this.textBoxOkolicznoscWady.Multiline = true;
            this.textBoxOkolicznoscWady.Name = "textBoxOkolicznoscWady";
            this.textBoxOkolicznoscWady.Size = new System.Drawing.Size(300, 30);
            this.textBoxOkolicznoscWady.TabIndex = 9;
            // 
            // labelUwagi
            // 
            this.labelUwagi.AutoSize = true;
            this.labelUwagi.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelUwagi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelUwagi.Location = new System.Drawing.Point(12, 252);
            this.labelUwagi.Name = "labelUwagi";
            this.labelUwagi.Size = new System.Drawing.Size(139, 20);
            this.labelUwagi.TabIndex = 10;
            this.labelUwagi.Text = "Jak długo używany?";
            // 
            // buttonWyslij
            // 
            this.buttonWyslij.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.buttonWyslij.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWyslij.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonWyslij.ForeColor = System.Drawing.Color.White;
            this.buttonWyslij.Location = new System.Drawing.Point(219, 439);
            this.buttonWyslij.Name = "buttonWyslij";
            this.buttonWyslij.Size = new System.Drawing.Size(75, 30);
            this.buttonWyslij.TabIndex = 12;
            this.buttonWyslij.Text = "Wyślij";
            this.buttonWyslij.UseVisualStyleBackColor = false;
            this.buttonWyslij.Click += new System.EventHandler(this.buttonWyslij_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.label1.Location = new System.Drawing.Point(12, 338);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "Na ile V?";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.label2.Location = new System.Drawing.Point(12, 292);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 41);
            this.label2.TabIndex = 13;
            this.label2.Text = "Typ pojazdu";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.label4.Location = new System.Drawing.Point(12, 383);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 41);
            this.label4.TabIndex = 17;
            this.label4.Text = "Gdzie motowany?";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.numericUpDown1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericUpDown1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.numericUpDown1.Location = new System.Drawing.Point(173, 254);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(49, 27);
            this.numericUpDown1.TabIndex = 21;
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "day",
            "month",
            "years"});
            this.comboBox1.Location = new System.Drawing.Point(228, 252);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 28);
            this.comboBox1.TabIndex = 22;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.comboBox2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Truck",
            "Passenger car"});
            this.comboBox2.Location = new System.Drawing.Point(173, 293);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 28);
            this.comboBox2.TabIndex = 23;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // comboBox3
            // 
            this.comboBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.comboBox3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "12V",
            "24V",
            "36V"});
            this.comboBox3.Location = new System.Drawing.Point(173, 334);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(121, 28);
            this.comboBox3.TabIndex = 24;
            // 
            // comboBox4
            // 
            this.comboBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.comboBox4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "Front",
            "Back ",
            "Side"});
            this.comboBox4.Location = new System.Drawing.Point(173, 384);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(121, 28);
            this.comboBox4.TabIndex = 25;
            // 
            // comboBox5
            // 
            this.comboBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.comboBox5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Items.AddRange(new object[] {
            "Low",
            "High"});
            this.comboBox5.Location = new System.Drawing.Point(300, 384);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(121, 28);
            this.comboBox5.TabIndex = 26;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(334, 171);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 30);
            this.button1.TabIndex = 27;
            this.button1.Text = "Przetłumacz";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(489, 12);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(872, 457);
            this.webView21.TabIndex = 28;
            this.webView21.ZoomFactor = 1D;
            // 
            // Form9
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(487, 481);
            this.Controls.Add(this.webView21);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox5);
            this.Controls.Add(this.comboBox4);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonWyslij);
            this.Controls.Add(this.labelUwagi);
            this.Controls.Add(this.textBoxOkolicznoscWady);
            this.Controls.Add(this.labelOkolicznoscWady);
            this.Controls.Add(this.textBoxOpisUsterki);
            this.Controls.Add(this.labelOpisUsterki);
            this.Controls.Add(this.dateTimePickerDataFVZakupowej);
            this.Controls.Add(this.labelDataFVZakupowej);
            this.Controls.Add(this.textBoxNumerFVZakupowej);
            this.Controls.Add(this.labelNumerFVZakupowej);
            this.Controls.Add(this.textBoxKodProduktuHella);
            this.Controls.Add(this.labelKodProduktuHella);
            this.Name = "Form9";
            this.Text = "Formularz dla Strands";
            this.Load += new System.EventHandler(this.Form7_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
     
    }
}