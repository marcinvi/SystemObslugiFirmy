using System.Drawing;

namespace Reklamacje_Dane
{
    partial class Form6
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

        private void InitializeComponent()
        {
            // Nowe style kolorystyczne
            Color backgroundColor = Color.FromArgb(245, 245, 245);
            Color headerColor = Color.FromArgb(33, 150, 243);
            Color accentColor = Color.FromArgb(68, 114, 196);
            Color textColor = Color.FromArgb(51, 51, 51);

            this.listBoxSerwisanci = new System.Windows.Forms.ListBox();
            this.buttonWybierz = new System.Windows.Forms.Button();

            // 
            // listBoxSerwisanci
            // 
            this.listBoxSerwisanci.BackColor = backgroundColor;
            this.listBoxSerwisanci.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxSerwisanci.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listBoxSerwisanci.ForeColor = textColor;
            this.listBoxSerwisanci.FormattingEnabled = true;
            this.listBoxSerwisanci.ItemHeight = 20;
            this.listBoxSerwisanci.Location = new System.Drawing.Point(12, 12);
            this.listBoxSerwisanci.Name = "listBoxSerwisanci";
            this.listBoxSerwisanci.Size = new System.Drawing.Size(360, 182);
            this.listBoxSerwisanci.TabIndex = 0;
            // 
            // buttonWybierz
            // 
            this.buttonWybierz.BackColor = accentColor;
            this.buttonWybierz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWybierz.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonWybierz.ForeColor = Color.White;
            this.buttonWybierz.Location = new System.Drawing.Point(297, 204);
            this.buttonWybierz.Name = "buttonWybierz";
            this.buttonWybierz.Size = new System.Drawing.Size(75, 30);
            this.buttonWybierz.TabIndex = 1;
            this.buttonWybierz.Text = "Wybierz";
            this.buttonWybierz.UseVisualStyleBackColor = false;
            this.buttonWybierz.Click += new System.EventHandler(this.buttonWybierz_Click);
            // 
            // Form6
            // 
            this.BackColor = backgroundColor;
            this.ClientSize = new System.Drawing.Size(384, 239);
            this.Controls.Add(this.buttonWybierz);
            this.Controls.Add(this.listBoxSerwisanci);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "Form6";
            this.Text = "Wybierz Serwisanta lub Wyślij Mail";
            this.Load += new System.EventHandler(this.Form6_Load);
            this.ResumeLayout(false);
        }



        private System.Windows.Forms.ListBox listBoxSerwisanci;
        private System.Windows.Forms.Button buttonWybierz;
    }
}