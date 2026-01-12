using System.Drawing;

namespace Reklamacje_Dane
{
    partial class Form7
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
        private System.Windows.Forms.TextBox textBoxUwagi;
        private System.Windows.Forms.Button buttonWyslij;

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
            this.textBoxUwagi = new System.Windows.Forms.TextBox();
            this.buttonWyslij = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelKodProduktuHella
            // 
            this.labelKodProduktuHella.AutoSize = true;
            this.labelKodProduktuHella.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelKodProduktuHella.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelKodProduktuHella.Location = new System.Drawing.Point(2, 14);
            this.labelKodProduktuHella.Name = "labelKodProduktuHella";
            this.labelKodProduktuHella.Size = new System.Drawing.Size(142, 20);
            this.labelKodProduktuHella.TabIndex = 0;
            this.labelKodProduktuHella.Text = "Kod produktu Hella:";
            // 
            // textBoxKodProduktuHella
            // 
            this.textBoxKodProduktuHella.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxKodProduktuHella.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxKodProduktuHella.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxKodProduktuHella.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxKodProduktuHella.Location = new System.Drawing.Point(150, 12);
            this.textBoxKodProduktuHella.Name = "textBoxKodProduktuHella";
            this.textBoxKodProduktuHella.Size = new System.Drawing.Size(300, 27);
            this.textBoxKodProduktuHella.TabIndex = 1;
            // 
            // labelNumerFVZakupowej
            // 
            this.labelNumerFVZakupowej.AutoSize = true;
            this.labelNumerFVZakupowej.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelNumerFVZakupowej.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelNumerFVZakupowej.Location = new System.Drawing.Point(2, 44);
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
            this.textBoxNumerFVZakupowej.Location = new System.Drawing.Point(150, 42);
            this.textBoxNumerFVZakupowej.Name = "textBoxNumerFVZakupowej";
            this.textBoxNumerFVZakupowej.Size = new System.Drawing.Size(300, 27);
            this.textBoxNumerFVZakupowej.TabIndex = 3;
            // 
            // labelDataFVZakupowej
            // 
            this.labelDataFVZakupowej.AutoSize = true;
            this.labelDataFVZakupowej.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelDataFVZakupowej.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelDataFVZakupowej.Location = new System.Drawing.Point(5, 77);
            this.labelDataFVZakupowej.Name = "labelDataFVZakupowej";
            this.labelDataFVZakupowej.Size = new System.Drawing.Size(139, 20);
            this.labelDataFVZakupowej.TabIndex = 4;
            this.labelDataFVZakupowej.Text = "Data FV zakupowej:";
            // 
            // dateTimePickerDataFVZakupowej
            // 
            this.dateTimePickerDataFVZakupowej.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dateTimePickerDataFVZakupowej.Location = new System.Drawing.Point(150, 72);
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
            this.textBoxOpisUsterki.Location = new System.Drawing.Point(150, 102);
            this.textBoxOpisUsterki.Multiline = true;
            this.textBoxOpisUsterki.Name = "textBoxOpisUsterki";
            this.textBoxOpisUsterki.Size = new System.Drawing.Size(300, 60);
            this.textBoxOpisUsterki.TabIndex = 7;
            // 
            // labelOkolicznoscWady
            // 
            this.labelOkolicznoscWady.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelOkolicznoscWady.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelOkolicznoscWady.Location = new System.Drawing.Point(12, 175);
            this.labelOkolicznoscWady.Name = "labelOkolicznoscWady";
            this.labelOkolicznoscWady.Size = new System.Drawing.Size(125, 41);
            this.labelOkolicznoscWady.TabIndex = 8;
            this.labelOkolicznoscWady.Text = "Okoliczność wykrycia wady:";
            // 
            // textBoxOkolicznoscWady
            // 
            this.textBoxOkolicznoscWady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxOkolicznoscWady.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOkolicznoscWady.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxOkolicznoscWady.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxOkolicznoscWady.Location = new System.Drawing.Point(150, 172);
            this.textBoxOkolicznoscWady.Multiline = true;
            this.textBoxOkolicznoscWady.Name = "textBoxOkolicznoscWady";
            this.textBoxOkolicznoscWady.Size = new System.Drawing.Size(300, 60);
            this.textBoxOkolicznoscWady.TabIndex = 9;
            // 
            // labelUwagi
            // 
            this.labelUwagi.AutoSize = true;
            this.labelUwagi.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelUwagi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.labelUwagi.Location = new System.Drawing.Point(12, 245);
            this.labelUwagi.Name = "labelUwagi";
            this.labelUwagi.Size = new System.Drawing.Size(54, 20);
            this.labelUwagi.TabIndex = 10;
            this.labelUwagi.Text = "Uwagi:";
            // 
            // textBoxUwagi
            // 
            this.textBoxUwagi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.textBoxUwagi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxUwagi.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBoxUwagi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.textBoxUwagi.Location = new System.Drawing.Point(150, 242);
            this.textBoxUwagi.Multiline = true;
            this.textBoxUwagi.Name = "textBoxUwagi";
            this.textBoxUwagi.Size = new System.Drawing.Size(300, 60);
            this.textBoxUwagi.TabIndex = 11;
            // 
            // buttonWyslij
            // 
            this.buttonWyslij.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.buttonWyslij.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWyslij.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonWyslij.ForeColor = System.Drawing.Color.White;
            this.buttonWyslij.Location = new System.Drawing.Point(375, 315);
            this.buttonWyslij.Name = "buttonWyslij";
            this.buttonWyslij.Size = new System.Drawing.Size(75, 30);
            this.buttonWyslij.TabIndex = 12;
            this.buttonWyslij.Text = "Wyślij";
            this.buttonWyslij.UseVisualStyleBackColor = false;
            this.buttonWyslij.Click += new System.EventHandler(this.buttonWyslij_Click);
            // 
            // Form7
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Controls.Add(this.buttonWyslij);
            this.Controls.Add(this.textBoxUwagi);
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
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "Form7";
            this.Text = "Formularz dla Hella Polska Sp z o.o.";
            this.Load += new System.EventHandler(this.Form7_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}