namespace Reklamacje_Dane
{
    partial class FormSzablonyEmail
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
            this.listBoxSzablony = new System.Windows.Forms.ListBox();
            this.txtNazwaSzablonu = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rtbTrescSzablonu = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnBold = new System.Windows.Forms.ToolStripButton();
            this.btnItalic = new System.Windows.Forms.ToolStripButton();
            this.btnUnderline = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmbFontFamily = new System.Windows.Forms.ToolStripComboBox();
            this.cmbFontSize = new System.Windows.Forms.ToolStripComboBox();
            this.btnNowy = new System.Windows.Forms.Button();
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnUsun = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.flpZmienne = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxSzablony
            // 
            this.listBoxSzablony.FormattingEnabled = true;
            this.listBoxSzablony.ItemHeight = 16;
            this.listBoxSzablony.Location = new System.Drawing.Point(12, 38);
            this.listBoxSzablony.Name = "listBoxSzablony";
            this.listBoxSzablony.Size = new System.Drawing.Size(239, 452);
            this.listBoxSzablony.TabIndex = 0;
            this.listBoxSzablony.SelectedIndexChanged += new System.EventHandler(this.listBoxSzablony_SelectedIndexChanged);
            // 
            // txtNazwaSzablonu
            // 
            this.txtNazwaSzablonu.Location = new System.Drawing.Point(271, 38);
            this.txtNazwaSzablonu.Name = "txtNazwaSzablonu";
            this.txtNazwaSzablonu.Size = new System.Drawing.Size(597, 22);
            this.txtNazwaSzablonu.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(268, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Nazwa szablonu:";
            // 
            // rtbTrescSzablonu
            // 
            this.rtbTrescSzablonu.AcceptsTab = true;
            this.rtbTrescSzablonu.Location = new System.Drawing.Point(271, 121);
            this.rtbTrescSzablonu.Name = "rtbTrescSzablonu";
            this.rtbTrescSzablonu.Size = new System.Drawing.Size(597, 268);
            this.rtbTrescSzablonu.TabIndex = 4;
            this.rtbTrescSzablonu.Text = "";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBold,
            this.btnItalic,
            this.btnUnderline,
            this.toolStripSeparator1,
            this.cmbFontFamily,
            this.cmbFontSize});
            this.toolStrip1.Location = new System.Drawing.Point(271, 71);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(325, 31);
            this.toolStrip1.TabIndex = 5;
            // 
            // btnBold
            // 
            this.btnBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBold.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBold.Name = "btnBold";
            this.btnBold.Size = new System.Drawing.Size(29, 28);
            this.btnBold.Text = "B";
            this.btnBold.Click += new System.EventHandler(this.btnBold_Click);
            // 
            // btnItalic
            // 
            this.btnItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnItalic.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.btnItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnItalic.Name = "btnItalic";
            this.btnItalic.Size = new System.Drawing.Size(29, 28);
            this.btnItalic.Text = "I";
            this.btnItalic.Click += new System.EventHandler(this.btnItalic_Click);
            // 
            // btnUnderline
            // 
            this.btnUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUnderline.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.btnUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUnderline.Name = "btnUnderline";
            this.btnUnderline.Size = new System.Drawing.Size(29, 28);
            this.btnUnderline.Text = "U";
            this.btnUnderline.Click += new System.EventHandler(this.btnUnderline_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // cmbFontFamily
            // 
            this.cmbFontFamily.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbFontFamily.Name = "cmbFontFamily";
            this.cmbFontFamily.Size = new System.Drawing.Size(150, 31);
            this.cmbFontFamily.SelectedIndexChanged += new System.EventHandler(this.cmbFontFamily_SelectedIndexChanged);
            // 
            // cmbFontSize
            // 
            this.cmbFontSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbFontSize.Name = "cmbFontSize";
            this.cmbFontSize.Size = new System.Drawing.Size(75, 31);
            this.cmbFontSize.SelectedIndexChanged += new System.EventHandler(this.cmbFontSize_SelectedIndexChanged);
            // 
            // btnNowy
            // 
            this.btnNowy.Location = new System.Drawing.Point(12, 506);
            this.btnNowy.Name = "btnNowy";
            this.btnNowy.Size = new System.Drawing.Size(75, 35);
            this.btnNowy.TabIndex = 6;
            this.btnNowy.Text = "Nowy";
            this.btnNowy.UseVisualStyleBackColor = true;
            this.btnNowy.Click += new System.EventHandler(this.btnNowy_Click);
            // 
            // btnZapisz
            // 
            this.btnZapisz.Location = new System.Drawing.Point(793, 506);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(75, 35);
            this.btnZapisz.TabIndex = 7;
            this.btnZapisz.Text = "Zapisz";
            this.btnZapisz.UseVisualStyleBackColor = true;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);
            // 
            // btnUsun
            // 
            this.btnUsun.Location = new System.Drawing.Point(176, 506);
            this.btnUsun.Name = "btnUsun";
            this.btnUsun.Size = new System.Drawing.Size(75, 35);
            this.btnUsun.TabIndex = 8;
            this.btnUsun.Text = "Usuń";
            this.btnUsun.UseVisualStyleBackColor = true;
            this.btnUsun.Click += new System.EventHandler(this.btnUsun_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(274, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Treść szablonu:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(268, 403);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Dostępne zmienne:";
            // 
            // flpZmienne
            // 
            this.flpZmienne.Location = new System.Drawing.Point(271, 422);
            this.flpZmienne.Name = "flpZmienne";
            this.flpZmienne.Size = new System.Drawing.Size(597, 78);
            this.flpZmienne.TabIndex = 11;
            // 
            // FormSzablonyEmail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 553);
            this.Controls.Add(this.flpZmienne);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnUsun);
            this.Controls.Add(this.btnZapisz);
            this.Controls.Add(this.btnNowy);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.rtbTrescSzablonu);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNazwaSzablonu);
            this.Controls.Add(this.listBoxSzablony);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSzablonyEmail";
            this.Text = "Zarządzanie Szablonami Wiadomości";
            this.Load += new System.EventHandler(this.FormSzablonyEmail_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSzablony;
        private System.Windows.Forms.TextBox txtNazwaSzablonu;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox rtbTrescSzablonu;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnBold;
        private System.Windows.Forms.ToolStripButton btnItalic;
        private System.Windows.Forms.ToolStripButton btnUnderline;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox cmbFontFamily;
        private System.Windows.Forms.ToolStripComboBox cmbFontSize;
        private System.Windows.Forms.Button btnNowy;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.Button btnUsun;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FlowLayoutPanel flpZmienne;
    }
}