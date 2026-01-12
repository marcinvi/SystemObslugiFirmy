// Plik: FormWyswietlAdresy.Designer.cs (WERSJA POPRAWIONA)
// Opis: Naprawiono problem nachodzenia na siebie kontrolek w przypadku
//       wielolinijkowych adresów poprzez wyłączenie AutoSize i dostosowanie
//       położenia etykiet.
namespace Reklamacje_Dane
{
    partial class FormWyswietlAdresy
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
            this.panelDostawy = new System.Windows.Forms.Panel();
            this.lblDeliveryPhone = new System.Windows.Forms.Label();
            this.lblDeliveryAddress = new System.Windows.Forms.Label();
            this.lblDeliveryName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelKupujacego = new System.Windows.Forms.Panel();
            this.lblBuyerPhone = new System.Windows.Forms.Label();
            this.lblBuyerAddress = new System.Windows.Forms.Label();
            this.lblBuyerName = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panelFaktury = new System.Windows.Forms.Panel();
            this.lblInvoiceTaxId = new System.Windows.Forms.Label();
            this.lblInvoiceAddress = new System.Windows.Forms.Label();
            this.lblInvoiceCompany = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnZamknij = new System.Windows.Forms.Button();
            this.panelDostawy.SuspendLayout();
            this.panelKupujacego.SuspendLayout();
            this.panelFaktury.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDostawy
            // 
            this.panelDostawy.BackColor = System.Drawing.Color.White;
            this.panelDostawy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDostawy.Controls.Add(this.lblDeliveryPhone);
            this.panelDostawy.Controls.Add(this.lblDeliveryAddress);
            this.panelDostawy.Controls.Add(this.lblDeliveryName);
            this.panelDostawy.Controls.Add(this.label1);
            this.panelDostawy.Location = new System.Drawing.Point(12, 12);
            this.panelDostawy.Name = "panelDostawy";
            this.panelDostawy.Size = new System.Drawing.Size(350, 165);
            this.panelDostawy.TabIndex = 0;
            // 
            // lblDeliveryPhone
            // 
            this.lblDeliveryPhone.AutoSize = true;
            this.lblDeliveryPhone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDeliveryPhone.Location = new System.Drawing.Point(15, 130);
            this.lblDeliveryPhone.Name = "lblDeliveryPhone";
            this.lblDeliveryPhone.Size = new System.Drawing.Size(58, 20);
            this.lblDeliveryPhone.TabIndex = 3;
            this.lblDeliveryPhone.Text = "Telefon";
            // 
            // lblDeliveryAddress
            // 
            this.lblDeliveryAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDeliveryAddress.Location = new System.Drawing.Point(15, 75);
            this.lblDeliveryAddress.Name = "lblDeliveryAddress";
            this.lblDeliveryAddress.Size = new System.Drawing.Size(320, 45);
            this.lblDeliveryAddress.TabIndex = 2;
            this.lblDeliveryAddress.Text = "Adres";
            // 
            // lblDeliveryName
            // 
            this.lblDeliveryName.AutoSize = true;
            this.lblDeliveryName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDeliveryName.Location = new System.Drawing.Point(15, 50);
            this.lblDeliveryName.Name = "lblDeliveryName";
            this.lblDeliveryName.Size = new System.Drawing.Size(110, 20);
            this.lblDeliveryName.TabIndex = 1;
            this.lblDeliveryName.Text = "Imię i Nazwisko";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "ADRES DOSTAWY";
            // 
            // panelKupujacego
            // 
            this.panelKupujacego.BackColor = System.Drawing.Color.White;
            this.panelKupujacego.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelKupujacego.Controls.Add(this.lblBuyerPhone);
            this.panelKupujacego.Controls.Add(this.lblBuyerAddress);
            this.panelKupujacego.Controls.Add(this.lblBuyerName);
            this.panelKupujacego.Controls.Add(this.label5);
            this.panelKupujacego.Location = new System.Drawing.Point(378, 12);
            this.panelKupujacego.Name = "panelKupujacego";
            this.panelKupujacego.Size = new System.Drawing.Size(350, 165);
            this.panelKupujacego.TabIndex = 4;
            // 
            // lblBuyerPhone
            // 
            this.lblBuyerPhone.AutoSize = true;
            this.lblBuyerPhone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBuyerPhone.Location = new System.Drawing.Point(15, 130);
            this.lblBuyerPhone.Name = "lblBuyerPhone";
            this.lblBuyerPhone.Size = new System.Drawing.Size(58, 20);
            this.lblBuyerPhone.TabIndex = 3;
            this.lblBuyerPhone.Text = "Telefon";
            // 
            // lblBuyerAddress
            // 
            this.lblBuyerAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBuyerAddress.Location = new System.Drawing.Point(15, 75);
            this.lblBuyerAddress.Name = "lblBuyerAddress";
            this.lblBuyerAddress.Size = new System.Drawing.Size(320, 45);
            this.lblBuyerAddress.TabIndex = 2;
            this.lblBuyerAddress.Text = "Adres";
            // 
            // lblBuyerName
            // 
            this.lblBuyerName.AutoSize = true;
            this.lblBuyerName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBuyerName.Location = new System.Drawing.Point(15, 50);
            this.lblBuyerName.Name = "lblBuyerName";
            this.lblBuyerName.Size = new System.Drawing.Size(110, 20);
            this.lblBuyerName.TabIndex = 1;
            this.lblBuyerName.Text = "Imię i Nazwisko";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.label5.Location = new System.Drawing.Point(15, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(193, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "ADRES KUPUJĄCEGO";
            // 
            // panelFaktury
            // 
            this.panelFaktury.BackColor = System.Drawing.Color.White;
            this.panelFaktury.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFaktury.Controls.Add(this.lblInvoiceTaxId);
            this.panelFaktury.Controls.Add(this.lblInvoiceAddress);
            this.panelFaktury.Controls.Add(this.lblInvoiceCompany);
            this.panelFaktury.Controls.Add(this.label9);
            this.panelFaktury.Location = new System.Drawing.Point(12, 193);
            this.panelFaktury.Name = "panelFaktury";
            this.panelFaktury.Size = new System.Drawing.Size(716, 165);
            this.panelFaktury.TabIndex = 5;
            // 
            // lblInvoiceTaxId
            // 
            this.lblInvoiceTaxId.AutoSize = true;
            this.lblInvoiceTaxId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInvoiceTaxId.Location = new System.Drawing.Point(15, 130);
            this.lblInvoiceTaxId.Name = "lblInvoiceTaxId";
            this.lblInvoiceTaxId.Size = new System.Drawing.Size(33, 20);
            this.lblInvoiceTaxId.TabIndex = 3;
            this.lblInvoiceTaxId.Text = "NIP";
            // 
            // lblInvoiceAddress
            // 
            this.lblInvoiceAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInvoiceAddress.Location = new System.Drawing.Point(15, 75);
            this.lblInvoiceAddress.Name = "lblInvoiceAddress";
            this.lblInvoiceAddress.Size = new System.Drawing.Size(685, 45);
            this.lblInvoiceAddress.TabIndex = 2;
            this.lblInvoiceAddress.Text = "Adres";
            // 
            // lblInvoiceCompany
            // 
            this.lblInvoiceCompany.AutoSize = true;
            this.lblInvoiceCompany.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInvoiceCompany.Location = new System.Drawing.Point(15, 50);
            this.lblInvoiceCompany.Name = "lblInvoiceCompany";
            this.lblInvoiceCompany.Size = new System.Drawing.Size(92, 20);
            this.lblInvoiceCompany.TabIndex = 1;
            this.lblInvoiceCompany.Text = "Nazwa Firmy";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.label9.Location = new System.Drawing.Point(15, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(183, 23);
            this.label9.TabIndex = 0;
            this.label9.Text = "ADRES DO FAKTURY";
            // 
            // btnZamknij
            // 
            this.btnZamknij.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnZamknij.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnZamknij.Location = new System.Drawing.Point(608, 374);
            this.btnZamknij.Name = "btnZamknij";
            this.btnZamknij.Size = new System.Drawing.Size(120, 35);
            this.btnZamknij.TabIndex = 6;
            this.btnZamknij.Text = "Zamknij";
            this.btnZamknij.UseVisualStyleBackColor = true;
            // 
            // FormWyswietlAdresy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(742, 423);
            this.Controls.Add(this.btnZamknij);
            this.Controls.Add(this.panelFaktury);
            this.Controls.Add(this.panelKupujacego);
            this.Controls.Add(this.panelDostawy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormWyswietlAdresy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wszystkie dostępne adresy";
            this.panelDostawy.ResumeLayout(false);
            this.panelDostawy.PerformLayout();
            this.panelKupujacego.ResumeLayout(false);
            this.panelKupujacego.PerformLayout();
            this.panelFaktury.ResumeLayout(false);
            this.panelFaktury.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDostawy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDeliveryPhone;
        private System.Windows.Forms.Label lblDeliveryAddress;
        private System.Windows.Forms.Label lblDeliveryName;
        private System.Windows.Forms.Panel panelKupujacego;
        private System.Windows.Forms.Label lblBuyerPhone;
        private System.Windows.Forms.Label lblBuyerAddress;
        private System.Windows.Forms.Label lblBuyerName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelFaktury;
        private System.Windows.Forms.Label lblInvoiceTaxId;
        private System.Windows.Forms.Label lblInvoiceAddress;
        private System.Windows.Forms.Label lblInvoiceCompany;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnZamknij;
    }
}