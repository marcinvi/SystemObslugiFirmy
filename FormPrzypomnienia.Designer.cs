namespace Reklamacje_Dane
{
    partial class FormPrzypomnienia
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dgvStyle = new System.Windows.Forms.DataGridViewCellStyle();

            this.panelTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFiltr = new System.Windows.Forms.ComboBox();
            this.btnDodajNowe = new System.Windows.Forms.Button();
            this.btnOdswiez = new System.Windows.Forms.Button();
            this.dgvPrzypomnienia = new System.Windows.Forms.DataGridView();
            this.ctxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemWykonane = new System.Windows.Forms.ToolStripMenuItem();
            this.itemPrzelozJutro = new System.Windows.Forms.ToolStripMenuItem();
            this.itemPrzelozTydzien = new System.Windows.Forms.ToolStripMenuItem();

            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrzypomnienia)).BeginInit();
            this.ctxMenu.SuspendLayout();
            this.SuspendLayout();

            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTop.Controls.Add(this.btnOdswiez);
            this.panelTop.Controls.Add(this.btnDodajNowe);
            this.panelTop.Controls.Add(this.cmbFiltr);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 50);
            this.panelTop.TabIndex = 0;

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Text = "Widok:";
            // 
            // cmbFiltr
            // 
            this.cmbFiltr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltr.FormattingEnabled = true;
            this.cmbFiltr.Location = new System.Drawing.Point(65, 14);
            this.cmbFiltr.Width = 200;
            this.cmbFiltr.TabIndex = 1;
            // 
            // btnOdswiez
            // 
            this.btnOdswiez.Location = new System.Drawing.Point(280, 13);
            this.btnOdswiez.Text = "Odśwież";
            this.btnOdswiez.Size = new System.Drawing.Size(75, 26);
            // 
            // btnDodajNowe
            // 
            this.btnDodajNowe.BackColor = System.Drawing.Color.SeaGreen;
            this.btnDodajNowe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDodajNowe.ForeColor = System.Drawing.Color.White;
            this.btnDodajNowe.Location = new System.Drawing.Point(750, 10);
            this.btnDodajNowe.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this.btnDodajNowe.Text = "+ Nowe Zadanie";
            this.btnDodajNowe.Size = new System.Drawing.Size(130, 30);
            this.btnDodajNowe.Click += new System.EventHandler(this.btnDodajNowe_Click);

            // 
            // dgvPrzypomnienia
            // 
            this.dgvPrzypomnienia.AllowUserToAddRows = false;
            this.dgvPrzypomnienia.AllowUserToDeleteRows = false;
            this.dgvPrzypomnienia.BackgroundColor = System.Drawing.Color.White;
            this.dgvPrzypomnienia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrzypomnienia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPrzypomnienia.Location = new System.Drawing.Point(0, 50);
            this.dgvPrzypomnienia.MultiSelect = false;
            this.dgvPrzypomnienia.Name = "dgvPrzypomnienia";
            this.dgvPrzypomnienia.ReadOnly = true;
            this.dgvPrzypomnienia.RowHeadersVisible = false;
            this.dgvPrzypomnienia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPrzypomnienia.TabIndex = 1;
            this.dgvPrzypomnienia.ContextMenuStrip = this.ctxMenu;

            // Stylizacja Grida
            dgvStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dgvStyle.BackColor = System.Drawing.SystemColors.Window;
            dgvStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            dgvStyle.SelectionBackColor = System.Drawing.Color.CornflowerBlue;
            dgvStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvPrzypomnienia.DefaultCellStyle = dgvStyle;

            // 
            // ctxMenu
            // 
            this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemWykonane,
            this.itemPrzelozJutro,
            this.itemPrzelozTydzien});
            this.ctxMenu.Name = "ctxMenu";
            this.ctxMenu.Size = new System.Drawing.Size(200, 76);

            this.itemWykonane.Text = "Oznacz jako Wykonane";
            this.itemWykonane.Click += new System.EventHandler(this.itemWykonane_Click);

            this.itemPrzelozJutro.Text = "Przełóż: Jutro (+1 dzień)";
            this.itemPrzelozJutro.Click += new System.EventHandler(this.itemPrzelozJutro_Click);

            this.itemPrzelozTydzien.Text = "Przełóż: Za tydzień (+7 dni)";
            this.itemPrzelozTydzien.Click += new System.EventHandler(this.itemPrzelozTydzien_Click);

            // 
            // FormPrzypomnienia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.dgvPrzypomnienia);
            this.Controls.Add(this.panelTop);
            this.Name = "FormPrzypomnienia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Centrum Zarządzania Zadaniami";

            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrzypomnienia)).EndInit();
            this.ctxMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFiltr;
        private System.Windows.Forms.Button btnDodajNowe;
        private System.Windows.Forms.Button btnOdswiez;
        private System.Windows.Forms.DataGridView dgvPrzypomnienia;
        private System.Windows.Forms.ContextMenuStrip ctxMenu;
        private System.Windows.Forms.ToolStripMenuItem itemWykonane;
        private System.Windows.Forms.ToolStripMenuItem itemPrzelozJutro;
        private System.Windows.Forms.ToolStripMenuItem itemPrzelozTydzien;
    }
}