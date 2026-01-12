namespace Reklamacje_Dane
{
    partial class FormWybierzCzesc
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dgvStyle = new System.Windows.Forms.DataGridViewCellStyle();
            dgvStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvStyle.BackColor = System.Drawing.SystemColors.Window;
            dgvStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            dgvStyle.ForeColor = System.Drawing.SystemColors.ControlText;
            dgvStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dgvStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dgvStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;

            this.panelTop = new System.Windows.Forms.Panel();
            this.btnWybierz = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlFiltry = new System.Windows.Forms.Panel(); // NOWY PANEL NA FILTRY
            this.dgvLista = new System.Windows.Forms.DataGridView();

            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLista)).BeginInit();
            this.SuspendLayout();

            // Panel Górny (Tytuł i przycisk)
            this.panelTop.Controls.Add(this.btnWybierz);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 50;
            this.panelTop.BackColor = System.Drawing.Color.WhiteSmoke;

            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Text = "Bank Części - Wybierz element";

            this.btnWybierz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnWybierz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWybierz.ForeColor = System.Drawing.Color.White;
            this.btnWybierz.Location = new System.Drawing.Point(650, 10);
            this.btnWybierz.Text = "UŻYJ CZĘŚCI";
            this.btnWybierz.Size = new System.Drawing.Size(130, 30);
            this.btnWybierz.Click += new System.EventHandler(this.btnWybierz_Click);

            // Panel Filtrów (Poniżej górnego)
            this.pnlFiltry.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFiltry.Height = 30;
            this.pnlFiltry.BackColor = System.Drawing.Color.WhiteSmoke;

            // Grid
            this.dgvLista.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLista.BackgroundColor = System.Drawing.Color.White;
            this.dgvLista.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLista.AllowUserToAddRows = false;
            this.dgvLista.ReadOnly = true;
            this.dgvLista.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLista.RowHeadersVisible = false;
            this.dgvLista.DefaultCellStyle = dgvStyle;
            this.dgvLista.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLista_CellDoubleClick);

            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);

            // Kolejność dodawania ważna dla Dock!
            this.Controls.Add(this.dgvLista);       // Fill
            this.Controls.Add(this.pnlFiltry);      // Top (pod spodem)
            this.Controls.Add(this.panelTop);       // Top (na samej górze)

            this.Name = "FormWybierzCzesc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wybór części do naprawy";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLista)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnWybierz;
        private System.Windows.Forms.Panel pnlFiltry;
        private System.Windows.Forms.DataGridView dgvLista;
    }
}