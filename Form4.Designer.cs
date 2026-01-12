namespace Reklamacje_Dane
{
    partial class Form4
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.buttonWyslij = new System.Windows.Forms.Button();
            this.groupBoxKanaly = new System.Windows.Forms.GroupBox();
            this.lblNadawca = new System.Windows.Forms.Label();
            this.cmbKontoEmail = new System.Windows.Forms.ComboBox();
            this.btnToggleAllegro = new System.Windows.Forms.Button();
            this.btnToggleSms = new System.Windows.Forms.Button();
            this.btnToggleEmail = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvTemplates = new System.Windows.Forms.DataGridView();
            this.rtbPodgladWiadomosci = new System.Windows.Forms.RichTextBox();
            this.toolStripFormatowanie = new System.Windows.Forms.ToolStrip();
            this.cmbCzcionka = new System.Windows.Forms.ToolStripComboBox();
            this.cmbRozmiar = new System.Windows.Forms.ToolStripComboBox();
            this.btnBold = new System.Windows.Forms.ToolStripButton();
            this.btnItalic = new System.Windows.Forms.ToolStripButton();
            this.btnUnderline = new System.Windows.Forms.ToolStripButton();
            this.panelZalaczniki = new System.Windows.Forms.Panel();
            this.lbxAttachments = new System.Windows.Forms.ListBox();
            this.btnDodajZalacznik = new System.Windows.Forms.Button();
            this.btnUsunZalacznik = new System.Windows.Forms.Button();
            this.lblZalaczniki = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            this.groupBoxKanaly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemplates)).BeginInit();
            this.toolStripFormatowanie.SuspendLayout();
            this.panelZalaczniki.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.buttonWyslij);
            this.panelTop.Controls.Add(this.groupBoxKanaly);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1182, 160);
            this.panelTop.TabIndex = 0;
            // 
            // buttonWyslij
            // 
            this.buttonWyslij.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWyslij.BackColor = System.Drawing.Color.SteelBlue;
            this.buttonWyslij.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.buttonWyslij.ForeColor = System.Drawing.Color.White;
            this.buttonWyslij.Location = new System.Drawing.Point(932, 40);
            this.buttonWyslij.Name = "buttonWyslij";
            this.buttonWyslij.Size = new System.Drawing.Size(220, 80);
            this.buttonWyslij.TabIndex = 0;
            this.buttonWyslij.Text = "WYŚLIJ";
            this.buttonWyslij.UseVisualStyleBackColor = false;
            this.buttonWyslij.Click += new System.EventHandler(this.buttonWyslij_Click);
            // 
            // groupBoxKanaly
            // 
            this.groupBoxKanaly.Controls.Add(this.lblNadawca);
            this.groupBoxKanaly.Controls.Add(this.cmbKontoEmail);
            this.groupBoxKanaly.Controls.Add(this.btnToggleAllegro);
            this.groupBoxKanaly.Controls.Add(this.btnToggleSms);
            this.groupBoxKanaly.Controls.Add(this.btnToggleEmail);
            this.groupBoxKanaly.Location = new System.Drawing.Point(12, 12);
            this.groupBoxKanaly.Name = "groupBoxKanaly";
            this.groupBoxKanaly.Size = new System.Drawing.Size(550, 140);
            this.groupBoxKanaly.TabIndex = 0;
            this.groupBoxKanaly.TabStop = false;
            this.groupBoxKanaly.Text = "Ustawienia wysyłki";
            // 
            // lblNadawca
            // 
            this.lblNadawca.AutoSize = true;
            this.lblNadawca.Location = new System.Drawing.Point(20, 80);
            this.lblNadawca.Name = "lblNadawca";
            this.lblNadawca.Size = new System.Drawing.Size(135, 16);
            this.lblNadawca.TabIndex = 0;
            this.lblNadawca.Text = "Wybierz konto e-mail:";
            // 
            // cmbKontoEmail
            // 
            this.cmbKontoEmail.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKontoEmail.FormattingEnabled = true;
            this.cmbKontoEmail.Location = new System.Drawing.Point(20, 100);
            this.cmbKontoEmail.Name = "cmbKontoEmail";
            this.cmbKontoEmail.Size = new System.Drawing.Size(330, 24);
            this.cmbKontoEmail.TabIndex = 1;
            // 
            // btnToggleAllegro
            // 
            this.btnToggleAllegro.Location = new System.Drawing.Point(360, 30);
            this.btnToggleAllegro.Name = "btnToggleAllegro";
            this.btnToggleAllegro.Size = new System.Drawing.Size(160, 40);
            this.btnToggleAllegro.TabIndex = 2;
            this.btnToggleAllegro.Text = "ALLEGRO";
            this.btnToggleAllegro.Click += new System.EventHandler(this.btnToggleAllegro_Click);
            // 
            // btnToggleSms
            // 
            this.btnToggleSms.Location = new System.Drawing.Point(190, 30);
            this.btnToggleSms.Name = "btnToggleSms";
            this.btnToggleSms.Size = new System.Drawing.Size(160, 40);
            this.btnToggleSms.TabIndex = 3;
            this.btnToggleSms.Text = "SMS";
            this.btnToggleSms.Click += new System.EventHandler(this.btnToggleSms_Click);
            // 
            // btnToggleEmail
            // 
            this.btnToggleEmail.Location = new System.Drawing.Point(20, 30);
            this.btnToggleEmail.Name = "btnToggleEmail";
            this.btnToggleEmail.Size = new System.Drawing.Size(160, 40);
            this.btnToggleEmail.TabIndex = 4;
            this.btnToggleEmail.Text = "E-MAIL";
            this.btnToggleEmail.Click += new System.EventHandler(this.btnToggleEmail_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 160);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvTemplates);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtbPodgladWiadomosci);
            this.splitContainer1.Panel2.Controls.Add(this.toolStripFormatowanie);
            this.splitContainer1.Panel2.Controls.Add(this.panelZalaczniki);
            this.splitContainer1.Size = new System.Drawing.Size(1182, 593);
            this.splitContainer1.SplitterDistance = 394;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgvTemplates
            // 
            this.dgvTemplates.AllowDrop = true;
            this.dgvTemplates.AllowUserToAddRows = false;
            this.dgvTemplates.BackgroundColor = System.Drawing.Color.White;
            this.dgvTemplates.ColumnHeadersHeight = 29;
            this.dgvTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTemplates.Location = new System.Drawing.Point(0, 0);
            this.dgvTemplates.Name = "dgvTemplates";
            this.dgvTemplates.RowHeadersWidth = 51;
            this.dgvTemplates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTemplates.Size = new System.Drawing.Size(394, 593);
            this.dgvTemplates.TabIndex = 0;
            this.dgvTemplates.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTemplates_CellValueChanged);
            this.dgvTemplates.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvTemplates_CurrentCellDirtyStateChanged);
            this.dgvTemplates.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvTemplates_DragDrop);
            this.dgvTemplates.DragOver += new System.Windows.Forms.DragEventHandler(this.dgvTemplates_DragOver);
            this.dgvTemplates.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvTemplates_MouseDown);
            this.dgvTemplates.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgvTemplates_MouseMove);
            // 
            // rtbPodgladWiadomosci
            // 
            this.rtbPodgladWiadomosci.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPodgladWiadomosci.Location = new System.Drawing.Point(0, 31);
            this.rtbPodgladWiadomosci.Name = "rtbPodgladWiadomosci";
            this.rtbPodgladWiadomosci.Size = new System.Drawing.Size(784, 442);
            this.rtbPodgladWiadomosci.TabIndex = 0;
            this.rtbPodgladWiadomosci.Text = "";
            // 
            // toolStripFormatowanie
            // 
            this.toolStripFormatowanie.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripFormatowanie.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbCzcionka,
            this.cmbRozmiar,
            this.btnBold,
            this.btnItalic,
            this.btnUnderline});
            this.toolStripFormatowanie.Location = new System.Drawing.Point(0, 0);
            this.toolStripFormatowanie.Name = "toolStripFormatowanie";
            this.toolStripFormatowanie.Size = new System.Drawing.Size(784, 31);
            this.toolStripFormatowanie.TabIndex = 1;
            // 
            // cmbCzcionka
            // 
            this.cmbCzcionka.Items.AddRange(new object[] {
            "Segoe UI",
            "Arial",
            "Times New Roman"});
            this.cmbCzcionka.Name = "cmbCzcionka";
            this.cmbCzcionka.Size = new System.Drawing.Size(121, 31);
            this.cmbCzcionka.SelectedIndexChanged += new System.EventHandler(this.cmbCzcionka_SelectedIndexChanged);
            // 
            // cmbRozmiar
            // 
            this.cmbRozmiar.Items.AddRange(new object[] {
            "8",
            "10",
            "12",
            "14",
            "18"});
            this.cmbRozmiar.Name = "cmbRozmiar";
            this.cmbRozmiar.Size = new System.Drawing.Size(121, 31);
            this.cmbRozmiar.SelectedIndexChanged += new System.EventHandler(this.cmbRozmiar_SelectedIndexChanged);
            // 
            // btnBold
            // 
            this.btnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBold.Name = "btnBold";
            this.btnBold.Size = new System.Drawing.Size(29, 28);
            this.btnBold.Text = "B";
            this.btnBold.Click += new System.EventHandler(this.btnBold_Click);
            // 
            // btnItalic
            // 
            this.btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnItalic.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Size = new System.Drawing.Size(29, 28);
            this.btnItalic.Text = "I";
            this.btnItalic.Click += new System.EventHandler(this.btnItalic_Click);
            // 
            // btnUnderline
            // 
            this.btnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUnderline.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.btnUnderline.Name = "btnUnderline";
            this.btnUnderline.Size = new System.Drawing.Size(29, 28);
            this.btnUnderline.Text = "U";
            this.btnUnderline.Click += new System.EventHandler(this.btnUnderline_Click);
            // 
            // panelZalaczniki
            // 
            this.panelZalaczniki.Controls.Add(this.lbxAttachments);
            this.panelZalaczniki.Controls.Add(this.btnDodajZalacznik);
            this.panelZalaczniki.Controls.Add(this.btnUsunZalacznik);
            this.panelZalaczniki.Controls.Add(this.lblZalaczniki);
            this.panelZalaczniki.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelZalaczniki.Location = new System.Drawing.Point(0, 473);
            this.panelZalaczniki.Name = "panelZalaczniki";
            this.panelZalaczniki.Size = new System.Drawing.Size(784, 120);
            this.panelZalaczniki.TabIndex = 2;
            // 
            // lbxAttachments
            // 
            this.lbxAttachments.ItemHeight = 16;
            this.lbxAttachments.Location = new System.Drawing.Point(10, 30);
            this.lbxAttachments.Name = "lbxAttachments";
            this.lbxAttachments.Size = new System.Drawing.Size(600, 68);
            this.lbxAttachments.TabIndex = 0;
            // 
            // btnDodajZalacznik
            // 
            this.btnDodajZalacznik.Location = new System.Drawing.Point(630, 30);
            this.btnDodajZalacznik.Name = "btnDodajZalacznik";
            this.btnDodajZalacznik.Size = new System.Drawing.Size(75, 23);
            this.btnDodajZalacznik.TabIndex = 1;
            this.btnDodajZalacznik.Text = "Dodaj";
            this.btnDodajZalacznik.Click += new System.EventHandler(this.btnDodajZalacznik_Click);
            // 
            // btnUsunZalacznik
            // 
            this.btnUsunZalacznik.Location = new System.Drawing.Point(630, 60);
            this.btnUsunZalacznik.Name = "btnUsunZalacznik";
            this.btnUsunZalacznik.Size = new System.Drawing.Size(75, 23);
            this.btnUsunZalacznik.TabIndex = 2;
            this.btnUsunZalacznik.Text = "Usuń";
            this.btnUsunZalacznik.Click += new System.EventHandler(this.btnUsunZalacznik_Click);
            // 
            // lblZalaczniki
            // 
            this.lblZalaczniki.Location = new System.Drawing.Point(10, 10);
            this.lblZalaczniki.Name = "lblZalaczniki";
            this.lblZalaczniki.Size = new System.Drawing.Size(100, 23);
            this.lblZalaczniki.TabIndex = 3;
            this.lblZalaczniki.Text = "Załączniki:";
            // 
            // Form4
            // 
            this.ClientSize = new System.Drawing.Size(1182, 753);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelTop);
            this.Name = "Form4";
            this.Text = "Kreator Wiadomości";
            this.Load += new System.EventHandler(this.Form4_Load);
            this.panelTop.ResumeLayout(false);
            this.groupBoxKanaly.ResumeLayout(false);
            this.groupBoxKanaly.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemplates)).EndInit();
            this.toolStripFormatowanie.ResumeLayout(false);
            this.toolStripFormatowanie.PerformLayout();
            this.panelZalaczniki.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button buttonWyslij;
        private System.Windows.Forms.GroupBox groupBoxKanaly;
        private System.Windows.Forms.Label lblNadawca;
        private System.Windows.Forms.ComboBox cmbKontoEmail;
        private System.Windows.Forms.Button btnToggleAllegro;
        private System.Windows.Forms.Button btnToggleSms;
        private System.Windows.Forms.Button btnToggleEmail;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvTemplates;
        private System.Windows.Forms.RichTextBox rtbPodgladWiadomosci;
        private System.Windows.Forms.ToolStrip toolStripFormatowanie;
        private System.Windows.Forms.ToolStripComboBox cmbCzcionka;
        private System.Windows.Forms.ToolStripComboBox cmbRozmiar;
        private System.Windows.Forms.ToolStripButton btnBold;
        private System.Windows.Forms.ToolStripButton btnItalic;
        private System.Windows.Forms.ToolStripButton btnUnderline;
        private System.Windows.Forms.Panel panelZalaczniki;
        private System.Windows.Forms.ListBox lbxAttachments;
        private System.Windows.Forms.Button btnUsunZalacznik;
        private System.Windows.Forms.Button btnDodajZalacznik;
        private System.Windows.Forms.Label lblZalaczniki;
    }
}