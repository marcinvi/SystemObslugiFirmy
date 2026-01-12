namespace Reklamacje_Dane
{
    partial class FormWybierzProdukt
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dgvStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTop = new System.Windows.Forms.Panel();
            this.txtSzukaj = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvProdukty = new System.Windows.Forms.DataGridView();
            this.btnWybierz = new System.Windows.Forms.Button();

            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdukty)).BeginInit();
            this.SuspendLayout();

            // Panel Górny
            this.panelTop.Controls.Add(this.btnWybierz);
            this.panelTop.Controls.Add(this.txtSzukaj);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 60;
            this.panelTop.BackColor = System.Drawing.Color.WhiteSmoke;

            // Label
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Text = "Wpisz nazwę, kod enova, kod producenta lub producenta:";

            // TextBox (Szukajka)
            this.txtSzukaj.Location = new System.Drawing.Point(15, 29);
            this.txtSzukaj.Size = new System.Drawing.Size(400, 23);
            this.txtSzukaj.TabIndex = 0;

            // Przycisk Wybierz
            this.btnWybierz.Location = new System.Drawing.Point(430, 27);
            this.btnWybierz.Size = new System.Drawing.Size(100, 27);
            this.btnWybierz.Text = "WYBIERZ";
            this.btnWybierz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnWybierz.ForeColor = System.Drawing.Color.White;
            this.btnWybierz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWybierz.Click += new System.EventHandler(this.btnWybierz_Click);

            // Grid
            this.dgvProdukty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProdukty.AllowUserToAddRows = false;
            this.dgvProdukty.AllowUserToDeleteRows = false;
            this.dgvProdukty.ReadOnly = true;
            this.dgvProdukty.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProdukty.RowHeadersVisible = false;
            this.dgvProdukty.BackgroundColor = System.Drawing.Color.White;
            this.dgvProdukty.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // Stylizacja
            dgvStyle.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            this.dgvProdukty.DefaultCellStyle = dgvStyle;
            this.dgvProdukty.DoubleClick += new System.EventHandler(this.dgvProdukty_DoubleClick);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.dgvProdukty);
            this.Controls.Add(this.panelTop);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wyszukiwarka Produktów";

            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdukty)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox txtSzukaj;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvProdukty;
        private System.Windows.Forms.Button btnWybierz;
    }
}