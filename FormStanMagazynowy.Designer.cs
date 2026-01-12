namespace Reklamacje_Dane
{
    partial class FormStanMagazynowy
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnDodajNadwyzke = new System.Windows.Forms.Button();
            this.btnDodajRecznie = new System.Windows.Forms.Button();
            this.btnOdswiez = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabUrzadzenia = new System.Windows.Forms.TabPage();
            this.dgvUrzadzenia = new System.Windows.Forms.DataGridView();
            this.ctxUrzadzenia = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemEdytujUrzadzenie = new System.Windows.Forms.ToolStripMenuItem();
            this.itemUsunUrzadzenie = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlFilterUrzadzenia = new System.Windows.Forms.Panel();
            this.tabCzesci = new System.Windows.Forms.TabPage();
            this.dgvCzesci = new System.Windows.Forms.DataGridView();
            this.ctxCzesci = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemUzyjWNaprawie = new System.Windows.Forms.ToolStripMenuItem();
            this.itemUsunCzesc = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlFilterCzesci = new System.Windows.Forms.Panel();
            this.tabPodsumowanie = new System.Windows.Forms.TabPage();
            this.dgvPodsumowanie = new System.Windows.Forms.DataGridView();
            this.pnlFilterPodsumowanie = new System.Windows.Forms.Panel();
            this.tabHistoria = new System.Windows.Forms.TabPage();
            this.dgvHistoria = new System.Windows.Forms.DataGridView();
            this.ctxHistoria = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemPrzywrocCzesc = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlFilterHistoria = new System.Windows.Forms.Panel();
            this.panelTop.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabUrzadzenia.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUrzadzenia)).BeginInit();
            this.ctxUrzadzenia.SuspendLayout();
            this.tabCzesci.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCzesci)).BeginInit();
            this.ctxCzesci.SuspendLayout();
            this.tabPodsumowanie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPodsumowanie)).BeginInit();
            this.tabHistoria.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistoria)).BeginInit();
            this.ctxHistoria.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTop.Controls.Add(this.btnDodajNadwyzke);
            this.panelTop.Controls.Add(this.btnDodajRecznie);
            this.panelTop.Controls.Add(this.btnOdswiez);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1100, 48);
            this.panelTop.TabIndex = 1;
            // 
            // btnDodajNadwyzke
            // 
            this.btnDodajNadwyzke.BackColor = System.Drawing.Color.Orange;
            this.btnDodajNadwyzke.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDodajNadwyzke.ForeColor = System.Drawing.Color.White;
            this.btnDodajNadwyzke.Location = new System.Drawing.Point(0, 0);
            this.btnDodajNadwyzke.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDodajNadwyzke.Name = "btnDodajNadwyzke";
            this.btnDodajNadwyzke.Size = new System.Drawing.Size(150, 24);
            this.btnDodajNadwyzke.TabIndex = 0;
            this.btnDodajNadwyzke.Text = "+ Dodaj Nadwyżkę";
            this.btnDodajNadwyzke.UseVisualStyleBackColor = false;
            this.btnDodajNadwyzke.Click += new System.EventHandler(this.btnDodajNadwyzke_Click);
            // 
            // btnDodajRecznie
            // 
            this.btnDodajRecznie.BackColor = System.Drawing.Color.SeaGreen;
            this.btnDodajRecznie.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDodajRecznie.ForeColor = System.Drawing.Color.White;
            this.btnDodajRecznie.Location = new System.Drawing.Point(0, 0);
            this.btnDodajRecznie.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDodajRecznie.Name = "btnDodajRecznie";
            this.btnDodajRecznie.Size = new System.Drawing.Size(150, 24);
            this.btnDodajRecznie.TabIndex = 1;
            this.btnDodajRecznie.Text = "+ Dodaj Ręcznie";
            this.btnDodajRecznie.UseVisualStyleBackColor = false;
            this.btnDodajRecznie.Click += new System.EventHandler(this.btnDodajRecznie_Click);
            // 
            // btnOdswiez
            // 
            this.btnOdswiez.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnOdswiez.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOdswiez.ForeColor = System.Drawing.Color.White;
            this.btnOdswiez.Location = new System.Drawing.Point(0, 0);
            this.btnOdswiez.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOdswiez.Name = "btnOdswiez";
            this.btnOdswiez.Size = new System.Drawing.Size(100, 24);
            this.btnOdswiez.TabIndex = 2;
            this.btnOdswiez.Text = "Odśwież";
            this.btnOdswiez.UseVisualStyleBackColor = false;
            this.btnOdswiez.Click += new System.EventHandler(this.btnOdswiez_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabUrzadzenia);
            this.tabControl.Controls.Add(this.tabCzesci);
            this.tabControl.Controls.Add(this.tabPodsumowanie);
            this.tabControl.Controls.Add(this.tabHistoria);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 48);
            this.tabControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1100, 472);
            this.tabControl.TabIndex = 3;
            // 
            // tabUrzadzenia
            // 
            this.tabUrzadzenia.Controls.Add(this.dgvUrzadzenia);
            this.tabUrzadzenia.Controls.Add(this.pnlFilterUrzadzenia);
            this.tabUrzadzenia.Location = new System.Drawing.Point(4, 25);
            this.tabUrzadzenia.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabUrzadzenia.Name = "tabUrzadzenia";
            this.tabUrzadzenia.Size = new System.Drawing.Size(1092, 443);
            this.tabUrzadzenia.TabIndex = 0;
            this.tabUrzadzenia.Text = "Urządzenia";
            // 
            // dgvUrzadzenia
            // 
            this.dgvUrzadzenia.ColumnHeadersHeight = 29;
            this.dgvUrzadzenia.ContextMenuStrip = this.ctxUrzadzenia;
            this.dgvUrzadzenia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUrzadzenia.Location = new System.Drawing.Point(0, 28);
            this.dgvUrzadzenia.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvUrzadzenia.Name = "dgvUrzadzenia";
            this.dgvUrzadzenia.RowHeadersWidth = 51;
            this.dgvUrzadzenia.Size = new System.Drawing.Size(1092, 415);
            this.dgvUrzadzenia.TabIndex = 0;
            // 
            // ctxUrzadzenia
            // 
            this.ctxUrzadzenia.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxUrzadzenia.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemEdytujUrzadzenie,
            this.itemUsunUrzadzenie});
            this.ctxUrzadzenia.Name = "ctxUrzadzenia";
            this.ctxUrzadzenia.Size = new System.Drawing.Size(192, 52);
            // 
            // itemEdytujUrzadzenie
            // 
            this.itemEdytujUrzadzenie.Name = "itemEdytujUrzadzenie";
            this.itemEdytujUrzadzenie.Size = new System.Drawing.Size(191, 24);
            this.itemEdytujUrzadzenie.Text = "Edytuj / Rozbierz";
            this.itemEdytujUrzadzenie.Click += new System.EventHandler(this.EdytujUrzadzenie_Click);
            // 
            // itemUsunUrzadzenie
            // 
            this.itemUsunUrzadzenie.Name = "itemUsunUrzadzenie";
            this.itemUsunUrzadzenie.Size = new System.Drawing.Size(191, 24);
            this.itemUsunUrzadzenie.Text = "Usuń";
            this.itemUsunUrzadzenie.Click += new System.EventHandler(this.UsunUrzadzenie_Click);
            // 
            // pnlFilterUrzadzenia
            // 
            this.pnlFilterUrzadzenia.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlFilterUrzadzenia.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilterUrzadzenia.Location = new System.Drawing.Point(0, 0);
            this.pnlFilterUrzadzenia.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlFilterUrzadzenia.Name = "pnlFilterUrzadzenia";
            this.pnlFilterUrzadzenia.Size = new System.Drawing.Size(1092, 28);
            this.pnlFilterUrzadzenia.TabIndex = 1;
            // 
            // tabCzesci
            // 
            this.tabCzesci.Controls.Add(this.dgvCzesci);
            this.tabCzesci.Controls.Add(this.pnlFilterCzesci);
            this.tabCzesci.Location = new System.Drawing.Point(4, 25);
            this.tabCzesci.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabCzesci.Name = "tabCzesci";
            this.tabCzesci.Size = new System.Drawing.Size(1092, 443);
            this.tabCzesci.TabIndex = 1;
            this.tabCzesci.Text = "Bank Części";
            // 
            // dgvCzesci
            // 
            this.dgvCzesci.ColumnHeadersHeight = 29;
            this.dgvCzesci.ContextMenuStrip = this.ctxCzesci;
            this.dgvCzesci.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCzesci.Location = new System.Drawing.Point(0, 28);
            this.dgvCzesci.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvCzesci.Name = "dgvCzesci";
            this.dgvCzesci.RowHeadersWidth = 51;
            this.dgvCzesci.Size = new System.Drawing.Size(1092, 415);
            this.dgvCzesci.TabIndex = 0;
            // 
            // ctxCzesci
            // 
            this.ctxCzesci.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxCzesci.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemUzyjWNaprawie,
            this.itemUsunCzesc});
            this.ctxCzesci.Name = "ctxCzesci";
            this.ctxCzesci.Size = new System.Drawing.Size(189, 52);
            // 
            // itemUzyjWNaprawie
            // 
            this.itemUzyjWNaprawie.Name = "itemUzyjWNaprawie";
            this.itemUzyjWNaprawie.Size = new System.Drawing.Size(188, 24);
            this.itemUzyjWNaprawie.Text = "Użyj do naprawy";
            this.itemUzyjWNaprawie.Click += new System.EventHandler(this.UzyjWNaprawie_Click);
            // 
            // itemUsunCzesc
            // 
            this.itemUsunCzesc.Name = "itemUsunCzesc";
            this.itemUsunCzesc.Size = new System.Drawing.Size(188, 24);
            this.itemUsunCzesc.Text = "Usuń część";
            this.itemUsunCzesc.Click += new System.EventHandler(this.UsunCzesc_Click);
            // 
            // pnlFilterCzesci
            // 
            this.pnlFilterCzesci.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlFilterCzesci.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilterCzesci.Location = new System.Drawing.Point(0, 0);
            this.pnlFilterCzesci.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlFilterCzesci.Name = "pnlFilterCzesci";
            this.pnlFilterCzesci.Size = new System.Drawing.Size(1092, 28);
            this.pnlFilterCzesci.TabIndex = 1;
            // 
            // tabPodsumowanie
            // 
            this.tabPodsumowanie.Controls.Add(this.dgvPodsumowanie);
            this.tabPodsumowanie.Controls.Add(this.pnlFilterPodsumowanie);
            this.tabPodsumowanie.Location = new System.Drawing.Point(4, 25);
            this.tabPodsumowanie.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPodsumowanie.Name = "tabPodsumowanie";
            this.tabPodsumowanie.Size = new System.Drawing.Size(1092, 443);
            this.tabPodsumowanie.TabIndex = 2;
            this.tabPodsumowanie.Text = "Podsumowanie";
            // 
            // dgvPodsumowanie
            // 
            this.dgvPodsumowanie.ColumnHeadersHeight = 29;
            this.dgvPodsumowanie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPodsumowanie.Location = new System.Drawing.Point(0, 28);
            this.dgvPodsumowanie.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvPodsumowanie.Name = "dgvPodsumowanie";
            this.dgvPodsumowanie.RowHeadersWidth = 51;
            this.dgvPodsumowanie.Size = new System.Drawing.Size(1092, 415);
            this.dgvPodsumowanie.TabIndex = 0;
            // 
            // pnlFilterPodsumowanie
            // 
            this.pnlFilterPodsumowanie.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlFilterPodsumowanie.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilterPodsumowanie.Location = new System.Drawing.Point(0, 0);
            this.pnlFilterPodsumowanie.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlFilterPodsumowanie.Name = "pnlFilterPodsumowanie";
            this.pnlFilterPodsumowanie.Size = new System.Drawing.Size(1092, 28);
            this.pnlFilterPodsumowanie.TabIndex = 1;
            // 
            // tabHistoria
            // 
            this.tabHistoria.Controls.Add(this.dgvHistoria);
            this.tabHistoria.Controls.Add(this.pnlFilterHistoria);
            this.tabHistoria.Location = new System.Drawing.Point(4, 25);
            this.tabHistoria.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabHistoria.Name = "tabHistoria";
            this.tabHistoria.Size = new System.Drawing.Size(192, 51);
            this.tabHistoria.TabIndex = 3;
            this.tabHistoria.Text = "Historia";
            // 
            // dgvHistoria
            // 
            this.dgvHistoria.ColumnHeadersHeight = 29;
            this.dgvHistoria.ContextMenuStrip = this.ctxHistoria;
            this.dgvHistoria.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistoria.Location = new System.Drawing.Point(0, 28);
            this.dgvHistoria.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvHistoria.Name = "dgvHistoria";
            this.dgvHistoria.RowHeadersWidth = 51;
            this.dgvHistoria.Size = new System.Drawing.Size(192, 23);
            this.dgvHistoria.TabIndex = 0;
            // 
            // ctxHistoria
            // 
            this.ctxHistoria.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxHistoria.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemPrzywrocCzesc});
            this.ctxHistoria.Name = "ctxHistoria";
            this.ctxHistoria.Size = new System.Drawing.Size(189, 28);
            // 
            // itemPrzywrocCzesc
            // 
            this.itemPrzywrocCzesc.Name = "itemPrzywrocCzesc";
            this.itemPrzywrocCzesc.Size = new System.Drawing.Size(188, 24);
            this.itemPrzywrocCzesc.Text = "Przywróć na stan";
            this.itemPrzywrocCzesc.Click += new System.EventHandler(this.itemPrzywrocCzesc_Click);
            // 
            // pnlFilterHistoria
            // 
            this.pnlFilterHistoria.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlFilterHistoria.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilterHistoria.Location = new System.Drawing.Point(0, 0);
            this.pnlFilterHistoria.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlFilterHistoria.Name = "pnlFilterHistoria";
            this.pnlFilterHistoria.Size = new System.Drawing.Size(192, 28);
            this.pnlFilterHistoria.TabIndex = 1;
            // 
            // FormStanMagazynowy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 520);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.panelTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormStanMagazynowy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stan Magazynowy";
            this.panelTop.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabUrzadzenia.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUrzadzenia)).EndInit();
            this.ctxUrzadzenia.ResumeLayout(false);
            this.tabCzesci.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCzesci)).EndInit();
            this.ctxCzesci.ResumeLayout(false);
            this.tabPodsumowanie.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPodsumowanie)).EndInit();
            this.tabHistoria.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistoria)).EndInit();
            this.ctxHistoria.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnDodajNadwyzke;
        private System.Windows.Forms.Button btnDodajRecznie;
        private System.Windows.Forms.Button btnOdswiez;
        private System.Windows.Forms.TabControl tabControl;

        private System.Windows.Forms.TabPage tabUrzadzenia;
        private System.Windows.Forms.DataGridView dgvUrzadzenia;
        private System.Windows.Forms.Panel pnlFilterUrzadzenia;

        private System.Windows.Forms.TabPage tabCzesci;
        private System.Windows.Forms.DataGridView dgvCzesci;
        private System.Windows.Forms.Panel pnlFilterCzesci;

        private System.Windows.Forms.TabPage tabPodsumowanie;
        private System.Windows.Forms.DataGridView dgvPodsumowanie;
        private System.Windows.Forms.Panel pnlFilterPodsumowanie;

        private System.Windows.Forms.TabPage tabHistoria;
        private System.Windows.Forms.DataGridView dgvHistoria;
        private System.Windows.Forms.Panel pnlFilterHistoria;

        private System.Windows.Forms.ContextMenuStrip ctxUrzadzenia;
        private System.Windows.Forms.ToolStripMenuItem itemEdytujUrzadzenie;
        private System.Windows.Forms.ToolStripMenuItem itemUsunUrzadzenie;
        private System.Windows.Forms.ContextMenuStrip ctxCzesci;
        private System.Windows.Forms.ToolStripMenuItem itemUzyjWNaprawie;
        private System.Windows.Forms.ToolStripMenuItem itemUsunCzesc;
        private System.Windows.Forms.ContextMenuStrip ctxHistoria;
        private System.Windows.Forms.ToolStripMenuItem itemPrzywrocCzesc;
    }
}