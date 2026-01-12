namespace Reklamacje_Dane
{
    partial class FormEmail
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">Prawda, jeśli zarządzane zasoby powinny zostać usunięte; w przeciwnym razie fałsz.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy Windows

        /// <summary>
        /// Wymagana metoda obsługi projektanta - nie modyfikuj
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxKontaktMail = new System.Windows.Forms.TextBox();
            this.textBoxTrescWiadomosci = new System.Windows.Forms.TextBox();
            this.buttonWyslij = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.webView22 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView22)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxKontaktMail
            // 
            this.textBoxKontaktMail.Location = new System.Drawing.Point(12, 12);
            this.textBoxKontaktMail.Name = "textBoxKontaktMail";
            this.textBoxKontaktMail.ReadOnly = true;
            this.textBoxKontaktMail.Size = new System.Drawing.Size(360, 22);
            this.textBoxKontaktMail.TabIndex = 0;
            // 
            // textBoxTrescWiadomosci
            // 
            this.textBoxTrescWiadomosci.Location = new System.Drawing.Point(12, 38);
            this.textBoxTrescWiadomosci.Multiline = true;
            this.textBoxTrescWiadomosci.Name = "textBoxTrescWiadomosci";
            this.textBoxTrescWiadomosci.Size = new System.Drawing.Size(360, 200);
            this.textBoxTrescWiadomosci.TabIndex = 1;
            // 
            // buttonWyslij
            // 
            this.buttonWyslij.Location = new System.Drawing.Point(297, 244);
            this.buttonWyslij.Name = "buttonWyslij";
            this.buttonWyslij.Size = new System.Drawing.Size(75, 23);
            this.buttonWyslij.TabIndex = 2;
            this.buttonWyslij.Text = "Wyślij";
            this.buttonWyslij.UseVisualStyleBackColor = true;
            this.buttonWyslij.Click += new System.EventHandler(this.buttonWyslij_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(73, 244);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(193, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Przetłumacz";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // webView22
            // 
            this.webView22.AllowExternalDrop = true;
            this.webView22.CreationProperties = null;
            this.webView22.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView22.Location = new System.Drawing.Point(389, 12);
            this.webView22.Name = "webView22";
            this.webView22.Size = new System.Drawing.Size(858, 255);
            this.webView22.TabIndex = 5;
            this.webView22.ZoomFactor = 1D;
            // 
            // FormEmail
            // 
            this.ClientSize = new System.Drawing.Size(381, 279);
            this.Controls.Add(this.webView22);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonWyslij);
            this.Controls.Add(this.textBoxTrescWiadomosci);
            this.Controls.Add(this.textBoxKontaktMail);
            this.Name = "FormEmail";
            this.Text = "Wyślij email do producenta";
            this.Load += new System.EventHandler(this.FormEmail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.webView22)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxKontaktMail;
        private System.Windows.Forms.TextBox textBoxTrescWiadomosci;
        private System.Windows.Forms.Button buttonWyslij;
        private System.Windows.Forms.Button button1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView22;
    }
}
