namespace Reklamacje_Dane
{
    partial class FormEditAction
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtTresc;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Button btnUsun;
        private System.Windows.Forms.Label labelOpis;

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
            this.txtTresc = new System.Windows.Forms.TextBox();
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.btnUsun = new System.Windows.Forms.Button();
            this.labelOpis = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtTresc
            // 
            this.txtTresc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTresc.Location = new System.Drawing.Point(16, 38);
            this.txtTresc.Multiline = true;
            this.txtTresc.Name = "txtTresc";
            this.txtTresc.Size = new System.Drawing.Size(555, 125);
            this.txtTresc.TabIndex = 0;
            // 
            // btnZapisz
            // 
            this.btnZapisz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(451, 178);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(120, 35);
            this.btnZapisz.TabIndex = 1;
            this.btnZapisz.Text = "Zapisz zmiany";
            this.btnZapisz.UseVisualStyleBackColor = false;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnuluj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnuluj.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAnuluj.ForeColor = System.Drawing.Color.White;
            this.btnAnuluj.Location = new System.Drawing.Point(325, 178);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(120, 35);
            this.btnAnuluj.TabIndex = 2;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = false;
            this.btnAnuluj.Click += new System.EventHandler(this.btnAnuluj_Click);
            // 
            // btnUsun
            // 
            this.btnUsun.BackColor = System.Drawing.Color.Crimson;
            this.btnUsun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUsun.ForeColor = System.Drawing.Color.White;
            this.btnUsun.Location = new System.Drawing.Point(16, 178);
            this.btnUsun.Name = "btnUsun";
            this.btnUsun.Size = new System.Drawing.Size(120, 35);
            this.btnUsun.TabIndex = 3;
            this.btnUsun.Text = "Usuń działanie";
            this.btnUsun.UseVisualStyleBackColor = false;
            this.btnUsun.Click += new System.EventHandler(this.btnUsun_Click);
            // 
            // labelOpis
            // 
            this.labelOpis.AutoSize = true;
            this.labelOpis.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelOpis.Location = new System.Drawing.Point(12, 15);
            this.labelOpis.Name = "labelOpis";
            this.labelOpis.Size = new System.Drawing.Size(170, 20);
            this.labelOpis.TabIndex = 4;
            this.labelOpis.Text = "Edytuj treść działania:";
            // 
            // FormEditAction
            // 
            this.AcceptButton = this.btnZapisz;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnuluj;
            this.ClientSize = new System.Drawing.Size(588, 228);
            this.Controls.Add(this.labelOpis);
            this.Controls.Add(this.btnUsun);
            this.Controls.Add(this.btnAnuluj);
            this.Controls.Add(this.btnZapisz);
            this.Controls.Add(this.txtTresc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditAction";
            this.Text = "Edycja Działania";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}