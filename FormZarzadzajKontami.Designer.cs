namespace Reklamacje_Dane
{
    partial class FormZarzadzajKontami
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();

            this.panelTop = new System.Windows.Forms.Panel();
            this.btnZamknij = new System.Windows.Forms.Button();
            this.btnUsun = new System.Windows.Forms.Button();
            this.btnEdytuj = new System.Windows.Forms.Button();
            this.btnDodaj = new System.Windows.Forms.Button();
            this.dgvKonta = new System.Windows.Forms.DataGridView();

            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKonta)).BeginInit();
            this.SuspendLayout();

            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTop.Controls.Add(this.btnZamknij);
            this.panelTop.Controls.Add(this.btnUsun);
            this.panelTop.Controls.Add(this.btnEdytuj);
            this.panelTop.Controls.Add(this.btnDodaj);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 70);
            this.panelTop.TabIndex = 0;

            // 
            // btnDodaj
            // 
            this.btnDodaj.BackColor = System.Drawing.Color.SeaGreen;
            this.btnDodaj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDodaj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnDodaj.ForeColor = System.Drawing.Color.White;
            this.btnDodaj.Location = new System.Drawing.Point(12, 12);
            this.btnDodaj.Name = "btnDodaj";
            this.btnDodaj.Size = new System.Drawing.Size(130, 45);
            this.btnDodaj.TabIndex = 0;
            this.btnDodaj.Text = "➕ Dodaj Nowe";
            this.btnDodaj.UseVisualStyleBackColor = false;
            this.btnDodaj.Click += new System.EventHandler(this.BtnDodaj_Click);

            // 
            // btnEdytuj
            // 
            this.btnEdytuj.BackColor = System.Drawing.Color.SteelBlue;
            this.btnEdytuj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdytuj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnEdytuj.ForeColor = System.Drawing.Color.White;
            this.btnEdytuj.Location = new System.Drawing.Point(148, 12);
            this.btnEdytuj.Name = "btnEdytuj";
            this.btnEdytuj.Size = new System.Drawing.Size(150, 45);
            this.btnEdytuj.TabIndex = 1;
            this.btnEdytuj.Text = "✏️ Edytuj";
            this.btnEdytuj.UseVisualStyleBackColor = false;
            this.btnEdytuj.Click += new System.EventHandler(this.BtnEdytuj_Click);

            // 
            // btnUsun
            // 
            this.btnUsun.BackColor = System.Drawing.Color.IndianRed;
            this.btnUsun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnUsun.ForeColor = System.Drawing.Color.White;
            this.btnUsun.Location = new System.Drawing.Point(304, 12);
            this.btnUsun.Name = "btnUsun";
            this.btnUsun.Size = new System.Drawing.Size(100, 45);
            this.btnUsun.TabIndex = 2;
            this.btnUsun.Text = "🗑️ Usuń";
            this.btnUsun.UseVisualStyleBackColor = false;
            this.btnUsun.Click += new System.EventHandler(this.BtnUsun_Click);

            // 
            // btnZamknij
            // 
            this.btnZamknij.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZamknij.Location = new System.Drawing.Point(788, 12);
            this.btnZamknij.Name = "btnZamknij";
            this.btnZamknij.Size = new System.Drawing.Size(100, 45);
            this.btnZamknij.TabIndex = 3;
            this.btnZamknij.Text = "Zamknij";
            this.btnZamknij.UseVisualStyleBackColor = true;
            this.btnZamknij.Click += new System.EventHandler(this.BtnZamknij_Click);

            // 
            // dgvKonta
            // 
            this.dgvKonta.AllowUserToAddRows = false;
            this.dgvKonta.AllowUserToDeleteRows = false;
            this.dgvKonta.BackgroundColor = System.Drawing.Color.White;
            this.dgvKonta.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvKonta.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;

            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;

            this.dgvKonta.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvKonta.ColumnHeadersHeight = 35;
            this.dgvKonta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvKonta.EnableHeadersVisualStyles = false;
            this.dgvKonta.Location = new System.Drawing.Point(0, 70); // Odsunięcie o wysokość panelu
            this.dgvKonta.MultiSelect = false;
            this.dgvKonta.Name = "dgvKonta";
            this.dgvKonta.ReadOnly = true;
            this.dgvKonta.RowHeadersVisible = false;
            this.dgvKonta.RowTemplate.Height = 24;
            this.dgvKonta.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKonta.Size = new System.Drawing.Size(900, 430);
            this.dgvKonta.TabIndex = 1;
            this.dgvKonta.DoubleClick += new System.EventHandler(this.BtnEdytuj_Click);

            // 
            // FormZarzadzajKontami
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.dgvKonta);
            this.Controls.Add(this.panelTop);
            this.Name = "FormZarzadzajKontami";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zarządzanie Skrzynkami Pocztowymi";

            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKonta)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnDodaj;
        private System.Windows.Forms.Button btnEdytuj;
        private System.Windows.Forms.Button btnUsun;
        private System.Windows.Forms.Button btnZamknij;
        private System.Windows.Forms.DataGridView dgvKonta;
    }
}