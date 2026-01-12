namespace Reklamacje_Dane
{
    partial class Form12
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
            this.cmbStatusOgolny = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbStatusKlient = new System.Windows.Forms.ComboBox();
            this.chkPoinformujKlienta = new System.Windows.Forms.CheckBox();
            this.btnZapisz = new System.Windows.Forms.Button();

            this.lblNrWRL = new System.Windows.Forms.Label();
            this.txtNrWRL = new System.Windows.Forms.TextBox();
            this.lblKwotaZwrotu = new System.Windows.Forms.Label();
            this.txtKwotaZwrotu = new System.Windows.Forms.TextBox();

            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(25, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status ogólny:";
            // 
            // cmbStatusOgolny
            // 
            this.cmbStatusOgolny.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusOgolny.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbStatusOgolny.FormattingEnabled = true;
            this.cmbStatusOgolny.Location = new System.Drawing.Point(29, 48);
            this.cmbStatusOgolny.Name = "cmbStatusOgolny";
            this.cmbStatusOgolny.Size = new System.Drawing.Size(395, 28);
            this.cmbStatusOgolny.TabIndex = 1;
            this.cmbStatusOgolny.SelectedIndexChanged += new System.EventHandler(this.cmbStatusOgolny_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(25, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Status klienta:";
            // 
            // cmbStatusKlient
            // 
            this.cmbStatusKlient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusKlient.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbStatusKlient.FormattingEnabled = true;
            this.cmbStatusKlient.Location = new System.Drawing.Point(29, 119);
            this.cmbStatusKlient.Name = "cmbStatusKlient";
            this.cmbStatusKlient.Size = new System.Drawing.Size(395, 28);
            this.cmbStatusKlient.TabIndex = 3;
            this.cmbStatusKlient.SelectedIndexChanged += new System.EventHandler(this.cmbStatusKlient_SelectedIndexChanged);
            // 
            // lblNrWRL
            // 
            this.lblNrWRL.AutoSize = true;
            this.lblNrWRL.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblNrWRL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblNrWRL.Location = new System.Drawing.Point(25, 167);
            this.lblNrWRL.Name = "lblNrWRL";
            this.lblNrWRL.Size = new System.Drawing.Size(65, 20);
            this.lblNrWRL.TabIndex = 6;
            this.lblNrWRL.Text = "Nr WRL:";
            this.lblNrWRL.Visible = false;
            // 
            // txtNrWRL
            // 
            this.txtNrWRL.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNrWRL.Location = new System.Drawing.Point(29, 190);
            this.txtNrWRL.Name = "txtNrWRL";
            this.txtNrWRL.Size = new System.Drawing.Size(395, 27);
            this.txtNrWRL.TabIndex = 7;
            this.txtNrWRL.Visible = false;
            // 
            // lblKwotaZwrotu
            // 
            this.lblKwotaZwrotu.AutoSize = true;
            this.lblKwotaZwrotu.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblKwotaZwrotu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblKwotaZwrotu.Location = new System.Drawing.Point(25, 167);
            this.lblKwotaZwrotu.Name = "lblKwotaZwrotu";
            this.lblKwotaZwrotu.Size = new System.Drawing.Size(109, 20);
            this.lblKwotaZwrotu.TabIndex = 8;
            this.lblKwotaZwrotu.Text = "Kwota zwrotu:";
            this.lblKwotaZwrotu.Visible = false;
            // 
            // txtKwotaZwrotu
            // 
            this.txtKwotaZwrotu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtKwotaZwrotu.Location = new System.Drawing.Point(29, 190);
            this.txtKwotaZwrotu.Name = "txtKwotaZwrotu";
            this.txtKwotaZwrotu.Size = new System.Drawing.Size(395, 27);
            this.txtKwotaZwrotu.TabIndex = 9;
            this.txtKwotaZwrotu.Visible = false;
            //
            // chkPoinformujKlienta
            // 
            this.chkPoinformujKlienta.AutoSize = true;
            this.chkPoinformujKlienta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkPoinformujKlienta.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkPoinformujKlienta.Location = new System.Drawing.Point(29, 240);
            this.chkPoinformujKlienta.Name = "chkPoinformujKlienta";
            this.chkPoinformujKlienta.Size = new System.Drawing.Size(262, 24);
            this.chkPoinformujKlienta.TabIndex = 4;
            this.chkPoinformujKlienta.Text = "Poinformuj klienta o zmianie statusu";
            this.chkPoinformujKlienta.UseVisualStyleBackColor = true;
            // 
            // btnZapisz
            // 
            this.btnZapisz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(164, 280);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(123, 38);
            this.btnZapisz.TabIndex = 5;
            this.btnZapisz.Text = "Zapisz zmiany";
            this.btnZapisz.UseVisualStyleBackColor = false;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);
            // 
            // Form12
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(452, 343);
            this.Controls.Add(this.btnZapisz);
            this.Controls.Add(this.chkPoinformujKlienta);
            this.Controls.Add(this.cmbStatusKlient);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbStatusOgolny);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblNrWRL);
            this.Controls.Add(this.txtNrWRL);
            this.Controls.Add(this.lblKwotaZwrotu);
            this.Controls.Add(this.txtKwotaZwrotu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form12";
            this.Text = "Zmiana Statusu Reklamacji";
            this.Load += new System.EventHandler(this.Form12_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbStatusOgolny;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbStatusKlient;
        private System.Windows.Forms.CheckBox chkPoinformujKlienta;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.Label lblNrWRL;
        private System.Windows.Forms.TextBox txtNrWRL;
        private System.Windows.Forms.Label lblKwotaZwrotu;
        private System.Windows.Forms.TextBox txtKwotaZwrotu;
    }
}