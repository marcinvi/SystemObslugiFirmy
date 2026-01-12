namespace Reklamacje_Dane
{
    partial class FormWyswietlWiadomosc
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
            this.txtWiadomosc = new System.Windows.Forms.TextBox();
            this.btnKopiuj = new System.Windows.Forms.Button();
            this.btnZamknij = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtWiadomosc
            // 
            this.txtWiadomosc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtWiadomosc.Location = new System.Drawing.Point(12, 35);
            this.txtWiadomosc.Multiline = true;
            this.txtWiadomosc.Name = "txtWiadomosc";
            this.txtWiadomosc.ReadOnly = true;
            this.txtWiadomosc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWiadomosc.Size = new System.Drawing.Size(560, 150);
            this.txtWiadomosc.TabIndex = 0;
            // 
            // btnKopiuj
            // 
            this.btnKopiuj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.btnKopiuj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKopiuj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnKopiuj.ForeColor = System.Drawing.Color.White;
            this.btnKopiuj.Location = new System.Drawing.Point(145, 191);
            this.btnKopiuj.Name = "btnKopiuj";
            this.btnKopiuj.Size = new System.Drawing.Size(150, 35);
            this.btnKopiuj.TabIndex = 1;
            this.btnKopiuj.Text = "Kopiuj do schowka";
            this.btnKopiuj.UseVisualStyleBackColor = false;
            this.btnKopiuj.Click += new System.EventHandler(this.btnKopiuj_Click);
            // 
            // btnZamknij
            // 
            this.btnZamknij.BackColor = System.Drawing.Color.Gray;
            this.btnZamknij.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZamknij.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZamknij.ForeColor = System.Drawing.Color.White;
            this.btnZamknij.Location = new System.Drawing.Point(301, 191);
            this.btnZamknij.Name = "btnZamknij";
            this.btnZamknij.Size = new System.Drawing.Size(120, 35);
            this.btnZamknij.TabIndex = 2;
            this.btnZamknij.Text = "Zamknij";
            this.btnZamknij.UseVisualStyleBackColor = false;
            this.btnZamknij.Click += new System.EventHandler(this.btnZamknij_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(262, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Przygotowana wiadomość dla klienta:";
            // 
            // FormWyswietlWiadomosc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 238);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnZamknij);
            this.Controls.Add(this.btnKopiuj);
            this.Controls.Add(this.txtWiadomosc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormWyswietlWiadomosc";
            this.Text = "Wiadomość do klienta";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtWiadomosc;
        private System.Windows.Forms.Button btnKopiuj;
        private System.Windows.Forms.Button btnZamknij;
        private System.Windows.Forms.Label label1;
    }
}