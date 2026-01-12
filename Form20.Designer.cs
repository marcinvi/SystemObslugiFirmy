using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class Form20
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.chkListColumns = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.oko = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelFilters = new System.Windows.Forms.Panel();
            this.btnResetFilters = new System.Windows.Forms.Button();
            this.btnApplyFilters = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnFilterStatusProducent = new System.Windows.Forms.Button();
            this.btnFilterStatusKlient = new System.Windows.Forms.Button();
            this.btnFilterStatusOgolny = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpZakupDo = new System.Windows.Forms.DateTimePicker();
            this.dtpZakupOd = new System.Windows.Forms.DateTimePicker();
            this.chkDataZakupu = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpZgloszenieDo = new System.Windows.Forms.DateTimePicker();
            this.dtpZgloszenieOd = new System.Windows.Forms.DateTimePicker();
            this.chkDataZgloszenia = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSaveView = new System.Windows.Forms.Button();
            this.txtGlobalSearch = new System.Windows.Forms.TextBox();
            this.dgvFilterRow = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panelFilters.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilterRow)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeight = 29;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dataGridView1.Location = new System.Drawing.Point(5, 218);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1830, 696);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // chkListColumns
            // 
            this.chkListColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkListColumns.BackColor = System.Drawing.SystemColors.Menu;
            this.chkListColumns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chkListColumns.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.chkListColumns.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.chkListColumns.FormattingEnabled = true;
            this.chkListColumns.Location = new System.Drawing.Point(264, 160);
            this.chkListColumns.Margin = new System.Windows.Forms.Padding(4);
            this.chkListColumns.Name = "chkListColumns";
            this.chkListColumns.Size = new System.Drawing.Size(362, 152);
            this.chkListColumns.TabIndex = 2;
            this.chkListColumns.Visible = false;
            this.chkListColumns.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ChkListColumns_ItemCheck);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(1859, 28);
            this.label1.TabIndex = 3;
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // oko
            // 
            this.oko.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.oko.AutoSize = true;
            this.oko.Cursor = System.Windows.Forms.Cursors.Hand;
            this.oko.Font = new System.Drawing.Font("Segoe UI Semibold", 7F);
            this.oko.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.oko.Location = new System.Drawing.Point(13, 83);
            this.oko.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.oko.Name = "oko";
            this.oko.Size = new System.Drawing.Size(121, 15);
            this.oko.TabIndex = 4;
            this.oko.Text = "Pokaż/Ukryj kolumny";
            this.oko.Click += new System.EventHandler(this.oko_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.label2.Location = new System.Drawing.Point(12, 918);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 23);
            this.label2.TabIndex = 6;
            this.label2.Text = "label2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelFilters
            // 
            this.panelFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFilters.BackColor = System.Drawing.SystemColors.Control;
            this.panelFilters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFilters.Controls.Add(this.btnResetFilters);
            this.panelFilters.Controls.Add(this.btnApplyFilters);
            this.panelFilters.Controls.Add(this.groupBox3);
            this.panelFilters.Controls.Add(this.groupBox2);
            this.panelFilters.Controls.Add(this.groupBox1);
            this.panelFilters.Location = new System.Drawing.Point(5, 31);
            this.panelFilters.Name = "panelFilters";
            this.panelFilters.Size = new System.Drawing.Size(1830, 150);
            this.panelFilters.TabIndex = 8;
            // 
            // btnResetFilters
            // 
            this.btnResetFilters.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnResetFilters.Location = new System.Drawing.Point(1682, 79);
            this.btnResetFilters.Name = "btnResetFilters";
            this.btnResetFilters.Size = new System.Drawing.Size(130, 46);
            this.btnResetFilters.TabIndex = 4;
            this.btnResetFilters.Text = "Wyczyść filtry";
            this.btnResetFilters.UseVisualStyleBackColor = true;
            this.btnResetFilters.Click += new System.EventHandler(this.btnResetFilters_Click);
            // 
            // btnApplyFilters
            // 
            this.btnApplyFilters.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnApplyFilters.Location = new System.Drawing.Point(1682, 22);
            this.btnApplyFilters.Name = "btnApplyFilters";
            this.btnApplyFilters.Size = new System.Drawing.Size(130, 51);
            this.btnApplyFilters.TabIndex = 3;
            this.btnApplyFilters.Text = "Filtruj";
            this.btnApplyFilters.UseVisualStyleBackColor = true;
            this.btnApplyFilters.Click += new System.EventHandler(this.btnApplyFilters_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnFilterStatusProducent);
            this.groupBox3.Controls.Add(this.btnFilterStatusKlient);
            this.groupBox3.Controls.Add(this.btnFilterStatusOgolny);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox3.Location = new System.Drawing.Point(1279, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(384, 123);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filtrowanie statusów";
            // 
            // btnFilterStatusProducent
            // 
            this.btnFilterStatusProducent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFilterStatusProducent.Location = new System.Drawing.Point(15, 84);
            this.btnFilterStatusProducent.Name = "btnFilterStatusProducent";
            this.btnFilterStatusProducent.Size = new System.Drawing.Size(354, 30);
            this.btnFilterStatusProducent.TabIndex = 2;
            this.btnFilterStatusProducent.Text = "Status Producent (Wszystkie)";
            this.btnFilterStatusProducent.UseVisualStyleBackColor = true;
            this.btnFilterStatusProducent.Click += new System.EventHandler(this.btnFilterStatusProducent_Click);
            // 
            // btnFilterStatusKlient
            // 
            this.btnFilterStatusKlient.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFilterStatusKlient.Location = new System.Drawing.Point(15, 54);
            this.btnFilterStatusKlient.Name = "btnFilterStatusKlient";
            this.btnFilterStatusKlient.Size = new System.Drawing.Size(354, 30);
            this.btnFilterStatusKlient.TabIndex = 1;
            this.btnFilterStatusKlient.Text = "Status Klient (Wszystkie)";
            this.btnFilterStatusKlient.UseVisualStyleBackColor = true;
            this.btnFilterStatusKlient.Click += new System.EventHandler(this.btnFilterStatusKlient_Click);
            // 
            // btnFilterStatusOgolny
            // 
            this.btnFilterStatusOgolny.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFilterStatusOgolny.Location = new System.Drawing.Point(15, 24);
            this.btnFilterStatusOgolny.Name = "btnFilterStatusOgolny";
            this.btnFilterStatusOgolny.Size = new System.Drawing.Size(354, 30);
            this.btnFilterStatusOgolny.TabIndex = 0;
            this.btnFilterStatusOgolny.Text = "Status Ogólny (Wszystkie)";
            this.btnFilterStatusOgolny.UseVisualStyleBackColor = true;
            this.btnFilterStatusOgolny.Click += new System.EventHandler(this.btnFilterStatusOgolny_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.dtpZakupDo);
            this.groupBox2.Controls.Add(this.dtpZakupOd);
            this.groupBox2.Controls.Add(this.chkDataZakupu);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.dtpZgloszenieDo);
            this.groupBox2.Controls.Add(this.dtpZgloszenieOd);
            this.groupBox2.Controls.Add(this.chkDataZgloszenia);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox2.Location = new System.Drawing.Point(448, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(813, 123);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Filtrowanie po dacie";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(603, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Do:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label6.Location = new System.Drawing.Point(398, 79);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Od:";
            // 
            // dtpZakupDo
            // 
            this.dtpZakupDo.Enabled = false;
            this.dtpZakupDo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dtpZakupDo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpZakupDo.Location = new System.Drawing.Point(641, 74);
            this.dtpZakupDo.Name = "dtpZakupDo";
            this.dtpZakupDo.Size = new System.Drawing.Size(148, 27);
            this.dtpZakupDo.TabIndex = 7;
            // 
            // dtpZakupOd
            // 
            this.dtpZakupOd.Enabled = false;
            this.dtpZakupOd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dtpZakupOd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpZakupOd.Location = new System.Drawing.Point(436, 74);
            this.dtpZakupOd.Name = "dtpZakupOd";
            this.dtpZakupOd.Size = new System.Drawing.Size(148, 27);
            this.dtpZakupOd.TabIndex = 6;
            // 
            // chkDataZakupu
            // 
            this.chkDataZakupu.AutoSize = true;
            this.chkDataZakupu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chkDataZakupu.Location = new System.Drawing.Point(415, 45);
            this.chkDataZakupu.Name = "chkDataZakupu";
            this.chkDataZakupu.Size = new System.Drawing.Size(114, 24);
            this.chkDataZakupu.TabIndex = 5;
            this.chkDataZakupu.Text = "Data zakupu";
            this.chkDataZakupu.UseVisualStyleBackColor = true;
            this.chkDataZakupu.CheckedChanged += new System.EventHandler(this.chkDataZakupu_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(191, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "Do:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(16, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Od:";
            // 
            // dtpZgloszenieDo
            // 
            this.dtpZgloszenieDo.Enabled = false;
            this.dtpZgloszenieDo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dtpZgloszenieDo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpZgloszenieDo.Location = new System.Drawing.Point(229, 74);
            this.dtpZgloszenieDo.Name = "dtpZgloszenieDo";
            this.dtpZgloszenieDo.Size = new System.Drawing.Size(148, 27);
            this.dtpZgloszenieDo.TabIndex = 2;
            // 
            // dtpZgloszenieOd
            // 
            this.dtpZgloszenieOd.Enabled = false;
            this.dtpZgloszenieOd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dtpZgloszenieOd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpZgloszenieOd.Location = new System.Drawing.Point(54, 74);
            this.dtpZgloszenieOd.Name = "dtpZgloszenieOd";
            this.dtpZgloszenieOd.Size = new System.Drawing.Size(148, 27);
            this.dtpZgloszenieOd.TabIndex = 1;
            // 
            // chkDataZgloszenia
            // 
            this.chkDataZgloszenia.AutoSize = true;
            this.chkDataZgloszenia.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chkDataZgloszenia.Location = new System.Drawing.Point(20, 45);
            this.chkDataZgloszenia.Name = "chkDataZgloszenia";
            this.chkDataZgloszenia.Size = new System.Drawing.Size(137, 24);
            this.chkDataZgloszenia.TabIndex = 0;
            this.chkDataZgloszenia.Text = "Data zgłoszenia";
            this.chkDataZgloszenia.UseVisualStyleBackColor = true;
            this.chkDataZgloszenia.CheckedChanged += new System.EventHandler(this.chkDataZgloszenia_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSaveView);
            this.groupBox1.Controls.Add(this.txtGlobalSearch);
            this.groupBox1.Controls.Add(this.oko);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox1.Location = new System.Drawing.Point(14, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(416, 123);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Wyszukiwanie ogólne (szuka na bieżąco)";
            // 
            // btnSaveView
            // 
            this.btnSaveView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveView.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSaveView.Location = new System.Drawing.Point(244, 83);
            this.btnSaveView.Name = "btnSaveView";
            this.btnSaveView.Size = new System.Drawing.Size(107, 25);
            this.btnSaveView.TabIndex = 10;
            this.btnSaveView.Text = "Zapisz widok";
            this.btnSaveView.UseVisualStyleBackColor = true;
            this.btnSaveView.Click += new System.EventHandler(this.btnSaveView_Click);
            // 
            // txtGlobalSearch
            // 
            this.txtGlobalSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtGlobalSearch.Location = new System.Drawing.Point(16, 45);
            this.txtGlobalSearch.Name = "txtGlobalSearch";
            this.txtGlobalSearch.Size = new System.Drawing.Size(381, 27);
            this.txtGlobalSearch.TabIndex = 0;
            // 
            // dgvFilterRow
            // 
            this.dgvFilterRow.AllowUserToAddRows = false;
            this.dgvFilterRow.AllowUserToDeleteRows = false;
            this.dgvFilterRow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFilterRow.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFilterRow.Location = new System.Drawing.Point(5, 188);
            this.dgvFilterRow.Name = "dgvFilterRow";
            this.dgvFilterRow.RowHeadersWidth = 51;
            this.dgvFilterRow.RowTemplate.Height = 24;
            this.dgvFilterRow.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvFilterRow.Size = new System.Drawing.Size(1830, 29);
            this.dgvFilterRow.TabIndex = 9;
            // 
            // Form20
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1859, 953);
            this.Controls.Add(this.chkListColumns);
            this.Controls.Add(this.dgvFilterRow);
            this.Controls.Add(this.panelFilters);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1400, 800);
            this.Name = "Form20";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form20_FormClosing);
            this.Load += new System.EventHandler(this.Form20_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panelFilters.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilterRow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckedListBox chkListColumns;
        private Label label1;
        private Label oko;
        private Label label2;
        private Panel panelFilters;
        private GroupBox groupBox1;
        private TextBox txtGlobalSearch;
        private GroupBox groupBox2;
        private CheckBox chkDataZgloszenia;
        private DateTimePicker dtpZgloszenieDo;
        private DateTimePicker dtpZgloszenieOd;
        private Label label4;
        private Label label3;
        private Label label5;
        private Label label6;
        private DateTimePicker dtpZakupDo;
        private DateTimePicker dtpZakupOd;
        private CheckBox chkDataZakupu;
        private GroupBox groupBox3;
        private Button btnFilterStatusProducent;
        private Button btnFilterStatusKlient;
        private Button btnFilterStatusOgolny;
        private Button btnResetFilters;
        private Button btnApplyFilters;
        private DataGridView dgvFilterRow;
        private Button btnSaveView;
    }
}