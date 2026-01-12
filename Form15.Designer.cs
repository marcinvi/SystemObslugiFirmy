namespace Reklamacje_Dane
{
    partial class Form15
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelListy = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panelDetali = new System.Windows.Forms.Panel();
            this.btnZarzadzajProducentami = new System.Windows.Forms.Button();
            this.btnNowaKategoria = new System.Windows.Forms.Button();
            this.comboProducent = new System.Windows.Forms.ComboBox();
            this.comboKategoria = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Kodporducenta = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.KodEnova = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Nazwakrotka = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.NazwaSystemowa = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonUsun = new System.Windows.Forms.Button();
            this.buttonZapisz = new System.Windows.Forms.Button();
            this.buttonNowy = new System.Windows.Forms.Button();
            this.panelListy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panelDetali.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelListy
            // 
            this.panelListy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelListy.BackColor = System.Drawing.Color.White;
            this.panelListy.Controls.Add(this.dataGridView1);
            this.panelListy.Controls.Add(this.textBox7);
            this.panelListy.Controls.Add(this.label8);
            this.panelListy.Location = new System.Drawing.Point(12, 12);
            this.panelListy.Name = "panelListy";
            this.panelListy.Padding = new System.Windows.Forms.Padding(10);
            this.panelListy.Size = new System.Drawing.Size(300, 480);
            this.panelListy.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(226)))), ((int)(((byte)(244)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(13, 64);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(226)))), ((int)(((byte)(244)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(274, 403);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // textBox7
            // 
            this.textBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox7.Location = new System.Drawing.Point(13, 31);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(274, 27);
            this.textBox7.TabIndex = 1;
            this.textBox7.TextChanged += new System.EventHandler(this.textBox7_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(10, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 20);
            this.label8.TabIndex = 0;
            this.label8.Text = "Wyszukaj...";
            // 
            // panelDetali
            // 
            this.panelDetali.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDetali.BackColor = System.Drawing.Color.White;
            this.panelDetali.Controls.Add(this.btnZarzadzajProducentami);
            this.panelDetali.Controls.Add(this.btnNowaKategoria);
            this.panelDetali.Controls.Add(this.comboProducent);
            this.panelDetali.Controls.Add(this.comboKategoria);
            this.panelDetali.Controls.Add(this.label6);
            this.panelDetali.Controls.Add(this.Kodporducenta);
            this.panelDetali.Controls.Add(this.label5);
            this.panelDetali.Controls.Add(this.KodEnova);
            this.panelDetali.Controls.Add(this.label4);
            this.panelDetali.Controls.Add(this.Nazwakrotka);
            this.panelDetali.Controls.Add(this.label3);
            this.panelDetali.Controls.Add(this.label1);
            this.panelDetali.Controls.Add(this.NazwaSystemowa);
            this.panelDetali.Controls.Add(this.label2);
            this.panelDetali.Location = new System.Drawing.Point(318, 12);
            this.panelDetali.Name = "panelDetali";
            this.panelDetali.Padding = new System.Windows.Forms.Padding(10);
            this.panelDetali.Size = new System.Drawing.Size(607, 429);
            this.panelDetali.TabIndex = 2;
            // 
            // btnZarzadzajProducentami
            // 
            this.btnZarzadzajProducentami.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZarzadzajProducentami.Location = new System.Drawing.Point(544, 292);
            this.btnZarzadzajProducentami.Name = "btnZarzadzajProducentami";
            this.btnZarzadzajProducentami.Size = new System.Drawing.Size(50, 28);
            this.btnZarzadzajProducentami.TabIndex = 16;
            this.btnZarzadzajProducentami.Text = "...";
            this.btnZarzadzajProducentami.UseVisualStyleBackColor = true;
            this.btnZarzadzajProducentami.Click += new System.EventHandler(this.btnZarzadzajProducentami_Click);
            // 
            // btnNowaKategoria
            // 
            this.btnNowaKategoria.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNowaKategoria.Location = new System.Drawing.Point(544, 224);
            this.btnNowaKategoria.Name = "btnNowaKategoria";
            this.btnNowaKategoria.Size = new System.Drawing.Size(50, 28);
            this.btnNowaKategoria.TabIndex = 15;
            this.btnNowaKategoria.Text = "+";
            this.btnNowaKategoria.UseVisualStyleBackColor = true;
            this.btnNowaKategoria.Click += new System.EventHandler(this.btnNowaKategoria_Click);
            // 
            // comboProducent
            // 
            this.comboProducent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboProducent.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboProducent.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboProducent.FormattingEnabled = true;
            this.comboProducent.Location = new System.Drawing.Point(16, 292);
            this.comboProducent.Name = "comboProducent";
            this.comboProducent.Size = new System.Drawing.Size(522, 28);
            this.comboProducent.TabIndex = 14;
            // 
            // comboKategoria
            // 
            this.comboKategoria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboKategoria.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboKategoria.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboKategoria.FormattingEnabled = true;
            this.comboKategoria.Location = new System.Drawing.Point(16, 224);
            this.comboKategoria.Name = "comboKategoria";
            this.comboKategoria.Size = new System.Drawing.Size(522, 28);
            this.comboKategoria.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(313, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "Kod Producenta";
            // 
            // Kodporducenta
            // 
            this.Kodporducenta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Kodporducenta.Location = new System.Drawing.Point(316, 160);
            this.Kodporducenta.Name = "Kodporducenta";
            this.Kodporducenta.Size = new System.Drawing.Size(278, 27);
            this.Kodporducenta.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(13, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Kod Enova";
            // 
            // KodEnova
            // 
            this.KodEnova.Location = new System.Drawing.Point(16, 160);
            this.KodEnova.Name = "KodEnova";
            this.KodEnova.Size = new System.Drawing.Size(278, 27);
            this.KodEnova.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(13, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Nazwa Krótka";
            // 
            // Nazwakrotka
            // 
            this.Nazwakrotka.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Nazwakrotka.Location = new System.Drawing.Point(16, 96);
            this.Nazwakrotka.Name = "Nazwakrotka";
            this.Nazwakrotka.Size = new System.Drawing.Size(578, 27);
            this.Nazwakrotka.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(13, 201);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Kategoria";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(13, 269);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Producent";
            // 
            // NazwaSystemowa
            // 
            this.NazwaSystemowa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NazwaSystemowa.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.NazwaSystemowa.Location = new System.Drawing.Point(16, 31);
            this.NazwaSystemowa.Name = "NazwaSystemowa";
            this.NazwaSystemowa.Size = new System.Drawing.Size(578, 34);
            this.NazwaSystemowa.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(13, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nazwa Systemowa";
            // 
            // buttonUsun
            // 
            this.buttonUsun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonUsun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.buttonUsun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUsun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonUsun.ForeColor = System.Drawing.Color.White;
            this.buttonUsun.Location = new System.Drawing.Point(785, 457);
            this.buttonUsun.Name = "buttonUsun";
            this.buttonUsun.Size = new System.Drawing.Size(140, 35);
            this.buttonUsun.TabIndex = 7;
            this.buttonUsun.Text = "Usuń";
            this.buttonUsun.UseVisualStyleBackColor = false;
            this.buttonUsun.Click += new System.EventHandler(this.buttonUsun_Click);
            // 
            // buttonZapisz
            // 
            this.buttonZapisz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonZapisz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.buttonZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonZapisz.ForeColor = System.Drawing.Color.White;
            this.buttonZapisz.Location = new System.Drawing.Point(627, 457);
            this.buttonZapisz.Name = "buttonZapisz";
            this.buttonZapisz.Size = new System.Drawing.Size(152, 35);
            this.buttonZapisz.TabIndex = 6;
            this.buttonZapisz.Text = "Dodaj/Zapisz";
            this.buttonZapisz.UseVisualStyleBackColor = false;
            this.buttonZapisz.Click += new System.EventHandler(this.buttonZapisz_Click);
            // 
            // buttonNowy
            // 
            this.buttonNowy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNowy.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.buttonNowy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNowy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.buttonNowy.ForeColor = System.Drawing.Color.White;
            this.buttonNowy.Location = new System.Drawing.Point(481, 457);
            this.buttonNowy.Name = "buttonNowy";
            this.buttonNowy.Size = new System.Drawing.Size(140, 35);
            this.buttonNowy.TabIndex = 5;
            this.buttonNowy.Text = "Nowy";
            this.buttonNowy.UseVisualStyleBackColor = false;
            this.buttonNowy.Click += new System.EventHandler(this.buttonNowy_Click);
            // 
            // Form15
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(937, 504);
            this.Controls.Add(this.buttonUsun);
            this.Controls.Add(this.buttonZapisz);
            this.Controls.Add(this.buttonNowy);
            this.Controls.Add(this.panelDetali);
            this.Controls.Add(this.panelListy);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(955, 551);
            this.Name = "Form15";
            this.Text = "Zarządzanie produktami";
            this.Load += new System.EventHandler(this.Form15_Load);
            this.panelListy.ResumeLayout(false);
            this.panelListy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panelDetali.ResumeLayout(false);
            this.panelDetali.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelListy;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panelDetali;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NazwaSystemowa;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Nazwakrotka;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox KodEnova;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Kodporducenta;
        private System.Windows.Forms.ComboBox comboKategoria;
        private System.Windows.Forms.ComboBox comboProducent;
        private System.Windows.Forms.Button btnNowaKategoria;
        private System.Windows.Forms.Button btnZarzadzajProducentami;
        private System.Windows.Forms.Button buttonUsun;
        private System.Windows.Forms.Button buttonZapisz;
        private System.Windows.Forms.Button buttonNowy;
    }
}