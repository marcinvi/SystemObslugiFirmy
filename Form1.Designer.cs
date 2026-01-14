using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView dataGridViewProcessing;
        private System.Windows.Forms.DataGridView dataGridViewReminders;
        private System.Windows.Forms.DataGridView dataGridViewChangeLog;
        private System.Windows.Forms.Label powiadomienie;
        private System.Windows.Forms.Button btnCloseNotification;
        private System.Windows.Forms.Label lblLastRefresh;
        private System.Windows.Forms.PictureBox refreshIcon;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.SplitContainer bottomSplitContainer;
        private System.Windows.Forms.Label labelZalogowany;
        private System.Windows.Forms.Panel leftMenuPanel;
        private System.Windows.Forms.Button btnToggleMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuReminders;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblAllegroMessages = new System.Windows.Forms.Label();
            this.labelZalogowany = new System.Windows.Forms.Label();
            this.lblLastRefresh = new System.Windows.Forms.Label();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.dataGridViewProcessing = new System.Windows.Forms.DataGridView();
            this.bottomSplitContainer = new System.Windows.Forms.SplitContainer();
            this.dataGridViewReminders = new System.Windows.Forms.DataGridView();
            this.dataGridViewChangeLog = new System.Windows.Forms.DataGridView();
            this.powiadomienie = new System.Windows.Forms.Label();
            this.btnCloseNotification = new System.Windows.Forms.Button();
            this.leftMenuPanel = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.lblAllegroCount = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnToggleMenu = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuReminders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dodajNoweToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pokażWyszystkieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oczekująceNaDostawęProsuktuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uzględnionePrzezProducentaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nowyProduktToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maZostaćWysłanyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dostarczonyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maZostaćWystawionaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zostałaWystawionaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nieuzględnionePrzezProducentaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wszystykieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uzględnioneKlientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usterkiNieStwierdzonoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.poOkresieReklamacjiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usterkaZWinyUżytkowaniaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brakKontaktuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nieuzględnioneKlientowiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wysłaneNoweProduktyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.naprawioneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skorygowaneFakturyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zarejestrujNoweToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wyszukajKlientaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dodajKlientaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dodajProducentaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wyszukajProducentaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wyszukajSerwisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dodajSerwisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.produktyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wyszukajProduktyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dodajProduktToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.przypomnieniaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wyświetlPrzypomnieniaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aktualneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zrealizowaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dodajPrzypomnienieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kurierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zamówKureiraDPDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zobaczHistorięZamówieńToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zestawieniaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoMatchFvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProcessing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomSplitContainer)).BeginInit();
            this.bottomSplitContainer.Panel1.SuspendLayout();
            this.bottomSplitContainer.Panel2.SuspendLayout();
            this.bottomSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewReminders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChangeLog)).BeginInit();
            this.leftMenuPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refreshIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // lblAllegroMessages
            // 
            this.lblAllegroMessages.BackColor = System.Drawing.Color.Transparent;
            this.lblAllegroMessages.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAllegroMessages.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblAllegroMessages.ForeColor = System.Drawing.Color.Gold;
            this.lblAllegroMessages.Location = new System.Drawing.Point(38, 543);
            this.lblAllegroMessages.Name = "lblAllegroMessages";
            this.lblAllegroMessages.Size = new System.Drawing.Size(141, 49);
            this.lblAllegroMessages.TabIndex = 0;
            this.lblAllegroMessages.Text = "Wiadomości";
            this.lblAllegroMessages.Click += new System.EventHandler(this.lblAllegroMessages_Click);
            // 
            // labelZalogowany
            // 
            this.labelZalogowany.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelZalogowany.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.labelZalogowany.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelZalogowany.ForeColor = System.Drawing.Color.White;
            this.labelZalogowany.Location = new System.Drawing.Point(1214, 3);
            this.labelZalogowany.Name = "labelZalogowany";
            this.labelZalogowany.Size = new System.Drawing.Size(240, 20);
            this.labelZalogowany.TabIndex = 0;
            this.labelZalogowany.Text = "Zalogowany: Administrator";
            this.labelZalogowany.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelZalogowany.Click += new System.EventHandler(this.labelZalogowany_Click);
            // 
            // lblLastRefresh
            // 
            this.lblLastRefresh.AutoSize = true;
            this.lblLastRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.lblLastRefresh.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLastRefresh.ForeColor = System.Drawing.Color.White;
            this.lblLastRefresh.Location = new System.Drawing.Point(851, 3);
            this.lblLastRefresh.Name = "lblLastRefresh";
            this.lblLastRefresh.Size = new System.Drawing.Size(186, 20);
            this.lblLastRefresh.TabIndex = 1;
            this.lblLastRefresh.Text = "Ostatnie odświeżenie: brak";
            this.lblLastRefresh.Click += new System.EventHandler(this.lblLastRefresh_Click);
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(200, 62);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.dataGridViewProcessing);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.bottomSplitContainer);
            this.mainSplitContainer.Size = new System.Drawing.Size(1254, 699);
            this.mainSplitContainer.SplitterDistance = 498;
            this.mainSplitContainer.SplitterWidth = 8;
            this.mainSplitContainer.TabIndex = 1;
            // 
            // dataGridViewProcessing
            // 
            this.dataGridViewProcessing.AllowUserToAddRows = false;
            this.dataGridViewProcessing.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridViewProcessing.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewProcessing.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewProcessing.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridViewProcessing.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewProcessing.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewProcessing.ColumnHeadersHeight = 29;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewProcessing.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewProcessing.EnableHeadersVisualStyles = false;
            this.dataGridViewProcessing.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dataGridViewProcessing.Location = new System.Drawing.Point(0, -2);
            this.dataGridViewProcessing.MultiSelect = false;
            this.dataGridViewProcessing.Name = "dataGridViewProcessing";
            this.dataGridViewProcessing.ReadOnly = true;
            this.dataGridViewProcessing.RowHeadersVisible = false;
            this.dataGridViewProcessing.RowHeadersWidth = 51;
            this.dataGridViewProcessing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewProcessing.Size = new System.Drawing.Size(1254, 525);
            this.dataGridViewProcessing.TabIndex = 0;
            this.dataGridViewProcessing.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewProcessing_CellContentClick);
            // 
            // bottomSplitContainer
            // 
            this.bottomSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.bottomSplitContainer.Name = "bottomSplitContainer";
            // 
            // bottomSplitContainer.Panel1
            // 
            this.bottomSplitContainer.Panel1.Controls.Add(this.dataGridViewReminders);
            // 
            // bottomSplitContainer.Panel2
            // 
            this.bottomSplitContainer.Panel2.Controls.Add(this.dataGridViewChangeLog);
            this.bottomSplitContainer.Size = new System.Drawing.Size(1254, 193);
            this.bottomSplitContainer.SplitterDistance = 479;
            this.bottomSplitContainer.TabIndex = 0;
            // 
            // dataGridViewReminders
            // 
            this.dataGridViewReminders.AllowUserToAddRows = false;
            this.dataGridViewReminders.AllowUserToDeleteRows = false;
            this.dataGridViewReminders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewReminders.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.dataGridViewReminders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewReminders.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewReminders.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewReminders.ColumnHeadersHeight = 29;
            this.dataGridViewReminders.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dataGridViewReminders.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewReminders.Name = "dataGridViewReminders";
            this.dataGridViewReminders.ReadOnly = true;
            this.dataGridViewReminders.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewReminders.RowHeadersVisible = false;
            this.dataGridViewReminders.RowHeadersWidth = 51;
            this.dataGridViewReminders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewReminders.Size = new System.Drawing.Size(479, 191);
            this.dataGridViewReminders.TabIndex = 1;
            // 
            // dataGridViewChangeLog
            // 
            this.dataGridViewChangeLog.AllowUserToAddRows = false;
            this.dataGridViewChangeLog.AllowUserToDeleteRows = false;
            this.dataGridViewChangeLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewChangeLog.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.dataGridViewChangeLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewChangeLog.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewChangeLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewChangeLog.ColumnHeadersHeight = 29;
            this.dataGridViewChangeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewChangeLog.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dataGridViewChangeLog.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewChangeLog.Name = "dataGridViewChangeLog";
            this.dataGridViewChangeLog.ReadOnly = true;
            this.dataGridViewChangeLog.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewChangeLog.RowHeadersVisible = false;
            this.dataGridViewChangeLog.RowHeadersWidth = 51;
            this.dataGridViewChangeLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewChangeLog.Size = new System.Drawing.Size(771, 193);
            this.dataGridViewChangeLog.TabIndex = 1;
            // 
            // powiadomienie
            // 
            this.powiadomienie.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.powiadomienie.Dock = System.Windows.Forms.DockStyle.Top;
            this.powiadomienie.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.powiadomienie.ForeColor = System.Drawing.Color.White;
            this.powiadomienie.Location = new System.Drawing.Point(0, 30);
            this.powiadomienie.Name = "powiadomienie";
            this.powiadomienie.Padding = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.powiadomienie.Size = new System.Drawing.Size(1454, 32);
            this.powiadomienie.TabIndex = 0;
            this.powiadomienie.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.powiadomienie.Visible = false;
            this.powiadomienie.Click += new System.EventHandler(this.powiadomienie_Click);
            // 
            // btnCloseNotification
            // 
            this.btnCloseNotification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnCloseNotification.FlatAppearance.BorderSize = 0;
            this.btnCloseNotification.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseNotification.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCloseNotification.ForeColor = System.Drawing.Color.White;
            this.btnCloseNotification.Location = new System.Drawing.Point(1422, 27);
            this.btnCloseNotification.Name = "btnCloseNotification";
            this.btnCloseNotification.Size = new System.Drawing.Size(32, 32);
            this.btnCloseNotification.TabIndex = 2;
            this.btnCloseNotification.Text = "✕";
            this.btnCloseNotification.UseVisualStyleBackColor = false;
            this.btnCloseNotification.Visible = false;
            // 
            // leftMenuPanel
            // 
            this.leftMenuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.leftMenuPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leftMenuPanel.Controls.Add(this.lblAllegroMessages);
            this.leftMenuPanel.Controls.Add(this.label10);
            this.leftMenuPanel.Controls.Add(this.lblAllegroCount);
            this.leftMenuPanel.Controls.Add(this.label8);
            this.leftMenuPanel.Controls.Add(this.label7);
            this.leftMenuPanel.Controls.Add(this.label6);
            this.leftMenuPanel.Controls.Add(this.label5);
            this.leftMenuPanel.Controls.Add(this.label4);
            this.leftMenuPanel.Controls.Add(this.label3);
            this.leftMenuPanel.Controls.Add(this.label2);
            this.leftMenuPanel.Controls.Add(this.pictureBox8);
            this.leftMenuPanel.Controls.Add(this.pictureBox7);
            this.leftMenuPanel.Controls.Add(this.pictureBox6);
            this.leftMenuPanel.Controls.Add(this.pictureBox5);
            this.leftMenuPanel.Controls.Add(this.pictureBox4);
            this.leftMenuPanel.Controls.Add(this.pictureBox3);
            this.leftMenuPanel.Controls.Add(this.pictureBox2);
            this.leftMenuPanel.Controls.Add(this.pictureBox1);
            this.leftMenuPanel.Controls.Add(this.btnToggleMenu);
            this.leftMenuPanel.Controls.Add(this.label1);
            this.leftMenuPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftMenuPanel.Location = new System.Drawing.Point(0, 62);
            this.leftMenuPanel.Name = "leftMenuPanel";
            this.leftMenuPanel.Size = new System.Drawing.Size(200, 699);
            this.leftMenuPanel.TabIndex = 3;
            this.leftMenuPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.leftMenuPanel_Paint);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label10.ForeColor = System.Drawing.Color.DarkRed;
            this.label10.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label10.Location = new System.Drawing.Point(33, 183);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 28);
            this.label10.TabIndex = 6;
            this.label10.UseMnemonic = false;
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // lblAllegroCount
            // 
            this.lblAllegroCount.BackColor = System.Drawing.Color.Transparent;
            this.lblAllegroCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblAllegroCount.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblAllegroCount.ForeColor = System.Drawing.Color.DarkRed;
            this.lblAllegroCount.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblAllegroCount.Location = new System.Drawing.Point(38, 357);
            this.lblAllegroCount.Name = "lblAllegroCount";
            this.lblAllegroCount.Size = new System.Drawing.Size(82, 28);
            this.lblAllegroCount.TabIndex = 6;
            this.lblAllegroCount.Text = "Allegor";
            this.lblAllegroCount.UseMnemonic = false;
            // 
            // label8
            // 
            this.label8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label8.Location = new System.Drawing.Point(53, 503);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(146, 40);
            this.label8.TabIndex = 19;
            this.label8.Text = "Ustawienia";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label7.Location = new System.Drawing.Point(53, 442);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 40);
            this.label7.TabIndex = 18;
            this.label7.Text = "Producenci";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label6.Location = new System.Drawing.Point(53, 385);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(146, 40);
            this.label6.TabIndex = 17;
            this.label6.Text = "Produkty";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label5.Location = new System.Drawing.Point(53, 327);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 40);
            this.label5.TabIndex = 16;
            this.label5.Text = "Klienci";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(53, 272);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 40);
            this.label4.TabIndex = 15;
            this.label4.Text = "Dodaj Zgłoszenie";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label3
            // 
            this.label3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(53, 214);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 40);
            this.label3.TabIndex = 14;
            this.label3.Text = "Zgłoszenia";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label2
            // 
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(53, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 40);
            this.label2.TabIndex = 13;
            this.label2.Text = "Nowe Zgłoszenia";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // pictureBox8
            // 
            this.pictureBox8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox8.Image = global::Reklamacje_Dane.Properties.Resources.Obraz16;
            this.pictureBox8.Location = new System.Drawing.Point(3, 152);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(40, 40);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox8.TabIndex = 12;
            this.pictureBox8.TabStop = false;
            this.pictureBox8.Click += new System.EventHandler(this.pictureBox8_Click);
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox7.Image = global::Reklamacje_Dane.Properties.Resources.Obraz15;
            this.pictureBox7.Location = new System.Drawing.Point(3, 91);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(40, 40);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 11;
            this.pictureBox7.TabStop = false;
            this.pictureBox7.Click += new System.EventHandler(this.pictureBox7_Click);
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox6.Image = global::Reklamacje_Dane.Properties.Resources.Obraz14;
            this.pictureBox6.Location = new System.Drawing.Point(3, 214);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(40, 40);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 10;
            this.pictureBox6.TabStop = false;
            this.pictureBox6.Click += new System.EventHandler(this.label3_Click);
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox5.Image = global::Reklamacje_Dane.Properties.Resources.Obraz13;
            this.pictureBox5.Location = new System.Drawing.Point(3, 272);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(40, 40);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 9;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.Click += new System.EventHandler(this.pictureBox5_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = global::Reklamacje_Dane.Properties.Resources.Obraz12;
            this.pictureBox4.Location = new System.Drawing.Point(3, 327);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(40, 40);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 8;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox3.Image = global::Reklamacje_Dane.Properties.Resources.Obraz11;
            this.pictureBox3.Location = new System.Drawing.Point(3, 385);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(40, 40);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 7;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = global::Reklamacje_Dane.Properties.Resources.Obraz10;
            this.pictureBox2.Location = new System.Drawing.Point(3, 442);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 40);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::Reklamacje_Dane.Properties.Resources.Obraz9;
            this.pictureBox1.Location = new System.Drawing.Point(3, 503);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 40);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnToggleMenu
            // 
            this.btnToggleMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnToggleMenu.FlatAppearance.BorderSize = 0;
            this.btnToggleMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleMenu.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnToggleMenu.ForeColor = System.Drawing.Color.White;
            this.btnToggleMenu.Location = new System.Drawing.Point(18, 2);
            this.btnToggleMenu.Name = "btnToggleMenu";
            this.btnToggleMenu.Size = new System.Drawing.Size(30, 30);
            this.btnToggleMenu.TabIndex = 4;
            this.btnToggleMenu.Text = "☰";
            this.btnToggleMenu.UseVisualStyleBackColor = false;
            this.btnToggleMenu.Click += new System.EventHandler(this.btnToggleMenu_Click);
            // 
            // label1
            // 
            this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(53, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "Strona Główna";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // contextMenuReminders
            // 
            this.contextMenuReminders.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuReminders.Name = "contextMenuReminders";
            this.contextMenuReminders.Size = new System.Drawing.Size(61, 4);
            // 
            // mainMenu
            // 
            this.mainMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolStripMenuItem1,
            this.produktyToolStripMenuItem,
            this.przypomnieniaToolStripMenuItem,
            this.kurierToolStripMenuItem,
            this.zestawieniaToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1454, 30);
            this.mainMenu.TabIndex = 5;
            this.mainMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mainMenu_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dodajNoweToolStripMenuItem,
            this.pokażWyszystkieToolStripMenuItem,
            this.zarejestrujNoweToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(95, 26);
            this.fileToolStripMenuItem.Text = "Zgłoszenia";
            // 
            // dodajNoweToolStripMenuItem
            // 
            this.dodajNoweToolStripMenuItem.Name = "dodajNoweToolStripMenuItem";
            this.dodajNoweToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.dodajNoweToolStripMenuItem.Text = "Dodaj nowe";
            this.dodajNoweToolStripMenuItem.Click += new System.EventHandler(this.dodajNoweToolStripMenuItem_Click);
            // 
            // pokażWyszystkieToolStripMenuItem
            // 
            this.pokażWyszystkieToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oczekująceNaDostawęProsuktuToolStripMenuItem,
            this.zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem,
            this.uzględnionePrzezProducentaToolStripMenuItem,
            this.nieuzględnionePrzezProducentaToolStripMenuItem,
            this.wszystykieToolStripMenuItem,
            this.uzględnioneKlientToolStripMenuItem,
            this.nieuzględnioneKlientowiToolStripMenuItem});
            this.pokażWyszystkieToolStripMenuItem.Name = "pokażWyszystkieToolStripMenuItem";
            this.pokażWyszystkieToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.pokażWyszystkieToolStripMenuItem.Text = "Pokaż...";
            this.pokażWyszystkieToolStripMenuItem.Click += new System.EventHandler(this.pokażWyszystkieToolStripMenuItem_Click);
            // 
            // oczekująceNaDostawęProsuktuToolStripMenuItem
            // 
            this.oczekująceNaDostawęProsuktuToolStripMenuItem.Name = "oczekująceNaDostawęProsuktuToolStripMenuItem";
            this.oczekująceNaDostawęProsuktuToolStripMenuItem.Size = new System.Drawing.Size(351, 26);
            this.oczekująceNaDostawęProsuktuToolStripMenuItem.Text = "Oczekujące na dostawę produktu";
            this.oczekująceNaDostawęProsuktuToolStripMenuItem.Click += new System.EventHandler(this.oczekująceNaDostawęProsuktuToolStripMenuItem_Click);
            // 
            // zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem
            // 
            this.zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem.Name = "zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem";
            this.zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem.Size = new System.Drawing.Size(351, 26);
            this.zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem.Text = "Zgłoszone do porducenta (bez decyzji)";
            this.zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem.Click += new System.EventHandler(this.zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem_Click);
            // 
            // uzględnionePrzezProducentaToolStripMenuItem
            // 
            this.uzględnionePrzezProducentaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nowyProduktToolStripMenuItem,
            this.notaToolStripMenuItem});
            this.uzględnionePrzezProducentaToolStripMenuItem.Name = "uzględnionePrzezProducentaToolStripMenuItem";
            this.uzględnionePrzezProducentaToolStripMenuItem.Size = new System.Drawing.Size(351, 26);
            this.uzględnionePrzezProducentaToolStripMenuItem.Text = "Uzględnione przez producenta";
            this.uzględnionePrzezProducentaToolStripMenuItem.Click += new System.EventHandler(this.uzględnionePrzezProducentaToolStripMenuItem_Click);
            // 
            // nowyProduktToolStripMenuItem
            // 
            this.nowyProduktToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maZostaćWysłanyToolStripMenuItem,
            this.dostarczonyToolStripMenuItem});
            this.nowyProduktToolStripMenuItem.Name = "nowyProduktToolStripMenuItem";
            this.nowyProduktToolStripMenuItem.Size = new System.Drawing.Size(186, 26);
            this.nowyProduktToolStripMenuItem.Text = "Nowy produkt";
            this.nowyProduktToolStripMenuItem.Click += new System.EventHandler(this.nowyProduktToolStripMenuItem_Click);
            // 
            // maZostaćWysłanyToolStripMenuItem
            // 
            this.maZostaćWysłanyToolStripMenuItem.Name = "maZostaćWysłanyToolStripMenuItem";
            this.maZostaćWysłanyToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            this.maZostaćWysłanyToolStripMenuItem.Text = "Ma zostać wysłany";
            this.maZostaćWysłanyToolStripMenuItem.Click += new System.EventHandler(this.maZostaćWysłanyToolStripMenuItem_Click);
            // 
            // dostarczonyToolStripMenuItem
            // 
            this.dostarczonyToolStripMenuItem.Name = "dostarczonyToolStripMenuItem";
            this.dostarczonyToolStripMenuItem.Size = new System.Drawing.Size(214, 26);
            this.dostarczonyToolStripMenuItem.Text = "Dostarczony";
            this.dostarczonyToolStripMenuItem.Click += new System.EventHandler(this.dostarczonyToolStripMenuItem_Click);
            // 
            // notaToolStripMenuItem
            // 
            this.notaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maZostaćWystawionaToolStripMenuItem,
            this.zostałaWystawionaToolStripMenuItem});
            this.notaToolStripMenuItem.Name = "notaToolStripMenuItem";
            this.notaToolStripMenuItem.Size = new System.Drawing.Size(186, 26);
            this.notaToolStripMenuItem.Text = "Nota";
            this.notaToolStripMenuItem.Click += new System.EventHandler(this.notaToolStripMenuItem_Click);
            // 
            // maZostaćWystawionaToolStripMenuItem
            // 
            this.maZostaćWystawionaToolStripMenuItem.Name = "maZostaćWystawionaToolStripMenuItem";
            this.maZostaćWystawionaToolStripMenuItem.Size = new System.Drawing.Size(240, 26);
            this.maZostaćWystawionaToolStripMenuItem.Text = "Ma zostać wystawiona";
            this.maZostaćWystawionaToolStripMenuItem.Click += new System.EventHandler(this.maZostaćWystawionaToolStripMenuItem_Click);
            // 
            // zostałaWystawionaToolStripMenuItem
            // 
            this.zostałaWystawionaToolStripMenuItem.Name = "zostałaWystawionaToolStripMenuItem";
            this.zostałaWystawionaToolStripMenuItem.Size = new System.Drawing.Size(240, 26);
            this.zostałaWystawionaToolStripMenuItem.Text = "Została wystawiona";
            this.zostałaWystawionaToolStripMenuItem.Click += new System.EventHandler(this.zostałaWystawionaToolStripMenuItem_Click);
            // 
            // nieuzględnionePrzezProducentaToolStripMenuItem
            // 
            this.nieuzględnionePrzezProducentaToolStripMenuItem.Name = "nieuzględnionePrzezProducentaToolStripMenuItem";
            this.nieuzględnionePrzezProducentaToolStripMenuItem.Size = new System.Drawing.Size(351, 26);
            this.nieuzględnionePrzezProducentaToolStripMenuItem.Text = "Nieuzględnione przez producenta";
            this.nieuzględnionePrzezProducentaToolStripMenuItem.Click += new System.EventHandler(this.nieuzględnionePrzezProducentaToolStripMenuItem_Click);
            // 
            // wszystykieToolStripMenuItem
            // 
            this.wszystykieToolStripMenuItem.Name = "wszystykieToolStripMenuItem";
            this.wszystykieToolStripMenuItem.Size = new System.Drawing.Size(351, 26);
            this.wszystykieToolStripMenuItem.Text = "Wszystykie";
            this.wszystykieToolStripMenuItem.Click += new System.EventHandler(this.wszystykieToolStripMenuItem_Click);
            // 
            // uzględnioneKlientToolStripMenuItem
            // 
            this.uzględnioneKlientToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usterkiNieStwierdzonoToolStripMenuItem,
            this.poOkresieReklamacjiToolStripMenuItem,
            this.usterkaZWinyUżytkowaniaToolStripMenuItem,
            this.brakKontaktuToolStripMenuItem});
            this.uzględnioneKlientToolStripMenuItem.Name = "uzględnioneKlientToolStripMenuItem";
            this.uzględnioneKlientToolStripMenuItem.Size = new System.Drawing.Size(351, 26);
            this.uzględnioneKlientToolStripMenuItem.Text = "Nieuwzględnione klientowi";
            this.uzględnioneKlientToolStripMenuItem.Click += new System.EventHandler(this.uzględnioneKlientToolStripMenuItem_Click);
            // 
            // usterkiNieStwierdzonoToolStripMenuItem
            // 
            this.usterkiNieStwierdzonoToolStripMenuItem.Name = "usterkiNieStwierdzonoToolStripMenuItem";
            this.usterkiNieStwierdzonoToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.usterkiNieStwierdzonoToolStripMenuItem.Text = "Usterki nie stwierdzono";
            this.usterkiNieStwierdzonoToolStripMenuItem.Click += new System.EventHandler(this.usterkiNieStwierdzonoToolStripMenuItem_Click);
            // 
            // poOkresieReklamacjiToolStripMenuItem
            // 
            this.poOkresieReklamacjiToolStripMenuItem.Name = "poOkresieReklamacjiToolStripMenuItem";
            this.poOkresieReklamacjiToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.poOkresieReklamacjiToolStripMenuItem.Text = "Po okresie reklamacji";
            this.poOkresieReklamacjiToolStripMenuItem.Click += new System.EventHandler(this.poOkresieReklamacjiToolStripMenuItem_Click);
            // 
            // usterkaZWinyUżytkowaniaToolStripMenuItem
            // 
            this.usterkaZWinyUżytkowaniaToolStripMenuItem.Name = "usterkaZWinyUżytkowaniaToolStripMenuItem";
            this.usterkaZWinyUżytkowaniaToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.usterkaZWinyUżytkowaniaToolStripMenuItem.Text = "Usterka z winy użytkowania";
            this.usterkaZWinyUżytkowaniaToolStripMenuItem.Click += new System.EventHandler(this.usterkaZWinyUżytkowaniaToolStripMenuItem_Click);
            // 
            // brakKontaktuToolStripMenuItem
            // 
            this.brakKontaktuToolStripMenuItem.Name = "brakKontaktuToolStripMenuItem";
            this.brakKontaktuToolStripMenuItem.Size = new System.Drawing.Size(272, 26);
            this.brakKontaktuToolStripMenuItem.Text = "Brak kontaktu";
            this.brakKontaktuToolStripMenuItem.Click += new System.EventHandler(this.brakKontaktuToolStripMenuItem_Click);
            // 
            // nieuzględnioneKlientowiToolStripMenuItem
            // 
            this.nieuzględnioneKlientowiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wysłaneNoweProduktyToolStripMenuItem,
            this.naprawioneToolStripMenuItem,
            this.skorygowaneFakturyToolStripMenuItem});
            this.nieuzględnioneKlientowiToolStripMenuItem.Name = "nieuzględnioneKlientowiToolStripMenuItem";
            this.nieuzględnioneKlientowiToolStripMenuItem.Size = new System.Drawing.Size(351, 26);
            this.nieuzględnioneKlientowiToolStripMenuItem.Text = "Uwzględnione klientowi";
            this.nieuzględnioneKlientowiToolStripMenuItem.Click += new System.EventHandler(this.nieuzględnioneKlientowiToolStripMenuItem_Click);
            // 
            // wysłaneNoweProduktyToolStripMenuItem
            // 
            this.wysłaneNoweProduktyToolStripMenuItem.Name = "wysłaneNoweProduktyToolStripMenuItem";
            this.wysłaneNoweProduktyToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
            this.wysłaneNoweProduktyToolStripMenuItem.Text = "Wysłano nowy produkty";
            this.wysłaneNoweProduktyToolStripMenuItem.Click += new System.EventHandler(this.wysłaneNoweProduktyToolStripMenuItem_Click);
            // 
            // naprawioneToolStripMenuItem
            // 
            this.naprawioneToolStripMenuItem.Name = "naprawioneToolStripMenuItem";
            this.naprawioneToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
            this.naprawioneToolStripMenuItem.Text = "Naprawione";
            this.naprawioneToolStripMenuItem.Click += new System.EventHandler(this.naprawioneToolStripMenuItem_Click);
            // 
            // skorygowaneFakturyToolStripMenuItem
            // 
            this.skorygowaneFakturyToolStripMenuItem.Name = "skorygowaneFakturyToolStripMenuItem";
            this.skorygowaneFakturyToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
            this.skorygowaneFakturyToolStripMenuItem.Text = "Zwrot pieniędzy";
            this.skorygowaneFakturyToolStripMenuItem.Click += new System.EventHandler(this.skorygowaneFakturyToolStripMenuItem_Click);
            // 
            // zarejestrujNoweToolStripMenuItem
            // 
            this.zarejestrujNoweToolStripMenuItem.Name = "zarejestrujNoweToolStripMenuItem";
            this.zarejestrujNoweToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.zarejestrujNoweToolStripMenuItem.Text = "Zarejestruj nowe";
            this.zarejestrujNoweToolStripMenuItem.Click += new System.EventHandler(this.zarejestrujNoweToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wyszukajKlientaToolStripMenuItem,
            this.dodajKlientaToolStripMenuItem});
            this.editToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(67, 26);
            this.editToolStripMenuItem.Text = "Klienci";
            // 
            // wyszukajKlientaToolStripMenuItem
            // 
            this.wyszukajKlientaToolStripMenuItem.Name = "wyszukajKlientaToolStripMenuItem";
            this.wyszukajKlientaToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.wyszukajKlientaToolStripMenuItem.Text = "Wyszukaj klienta";
            // 
            // dodajKlientaToolStripMenuItem
            // 
            this.dodajKlientaToolStripMenuItem.Name = "dodajKlientaToolStripMenuItem";
            this.dodajKlientaToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.dodajKlientaToolStripMenuItem.Text = "Dodaj klienta";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dodajProducentaToolStripMenuItem,
            this.wyszukajProducentaToolStripMenuItem});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(96, 26);
            this.viewToolStripMenuItem.Text = "Producenci";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // dodajProducentaToolStripMenuItem
            // 
            this.dodajProducentaToolStripMenuItem.Name = "dodajProducentaToolStripMenuItem";
            this.dodajProducentaToolStripMenuItem.Size = new System.Drawing.Size(233, 26);
            this.dodajProducentaToolStripMenuItem.Text = "Dodaj producenta";
            // 
            // wyszukajProducentaToolStripMenuItem
            // 
            this.wyszukajProducentaToolStripMenuItem.Name = "wyszukajProducentaToolStripMenuItem";
            this.wyszukajProducentaToolStripMenuItem.Size = new System.Drawing.Size(233, 26);
            this.wyszukajProducentaToolStripMenuItem.Text = "Wyszukaj producenta";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wyszukajSerwisToolStripMenuItem,
            this.dodajSerwisToolStripMenuItem});
            this.toolStripMenuItem1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(72, 26);
            this.toolStripMenuItem1.Text = "Serwisy";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // wyszukajSerwisToolStripMenuItem
            // 
            this.wyszukajSerwisToolStripMenuItem.Name = "wyszukajSerwisToolStripMenuItem";
            this.wyszukajSerwisToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.wyszukajSerwisToolStripMenuItem.Text = "Wyszukaj serwis";
            // 
            // dodajSerwisToolStripMenuItem
            // 
            this.dodajSerwisToolStripMenuItem.Name = "dodajSerwisToolStripMenuItem";
            this.dodajSerwisToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.dodajSerwisToolStripMenuItem.Text = "Dodaj serwis";
            // 
            // produktyToolStripMenuItem
            // 
            this.produktyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wyszukajProduktyToolStripMenuItem,
            this.dodajProduktToolStripMenuItem});
            this.produktyToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.produktyToolStripMenuItem.Name = "produktyToolStripMenuItem";
            this.produktyToolStripMenuItem.Size = new System.Drawing.Size(81, 26);
            this.produktyToolStripMenuItem.Text = "Produkty";
            // 
            // wyszukajProduktyToolStripMenuItem
            // 
            this.wyszukajProduktyToolStripMenuItem.Name = "wyszukajProduktyToolStripMenuItem";
            this.wyszukajProduktyToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.wyszukajProduktyToolStripMenuItem.Text = "Wyszukaj produkty";
            // 
            // dodajProduktToolStripMenuItem
            // 
            this.dodajProduktToolStripMenuItem.Name = "dodajProduktToolStripMenuItem";
            this.dodajProduktToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.dodajProduktToolStripMenuItem.Text = "Dodaj produkt";
            // 
            // przypomnieniaToolStripMenuItem
            // 
            this.przypomnieniaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wyświetlPrzypomnieniaToolStripMenuItem,
            this.dodajPrzypomnienieToolStripMenuItem});
            this.przypomnieniaToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.przypomnieniaToolStripMenuItem.Name = "przypomnieniaToolStripMenuItem";
            this.przypomnieniaToolStripMenuItem.Size = new System.Drawing.Size(121, 26);
            this.przypomnieniaToolStripMenuItem.Text = "Przypomnienia";
            // 
            // wyświetlPrzypomnieniaToolStripMenuItem
            // 
            this.wyświetlPrzypomnieniaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aktualneToolStripMenuItem,
            this.zrealizowaneToolStripMenuItem});
            this.wyświetlPrzypomnieniaToolStripMenuItem.Name = "wyświetlPrzypomnieniaToolStripMenuItem";
            this.wyświetlPrzypomnieniaToolStripMenuItem.Size = new System.Drawing.Size(254, 26);
            this.wyświetlPrzypomnieniaToolStripMenuItem.Text = "Wyświetl przypomnienia";
            // 
            // aktualneToolStripMenuItem
            // 
            this.aktualneToolStripMenuItem.Name = "aktualneToolStripMenuItem";
            this.aktualneToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.aktualneToolStripMenuItem.Text = "Aktualne";
            // 
            // zrealizowaneToolStripMenuItem
            // 
            this.zrealizowaneToolStripMenuItem.Name = "zrealizowaneToolStripMenuItem";
            this.zrealizowaneToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.zrealizowaneToolStripMenuItem.Text = "Zrealizowane";
            // 
            // dodajPrzypomnienieToolStripMenuItem
            // 
            this.dodajPrzypomnienieToolStripMenuItem.Name = "dodajPrzypomnienieToolStripMenuItem";
            this.dodajPrzypomnienieToolStripMenuItem.Size = new System.Drawing.Size(254, 26);
            this.dodajPrzypomnienieToolStripMenuItem.Text = "Dodaj przypomnienie";
            // 
            // kurierToolStripMenuItem
            // 
            this.kurierToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zamówKureiraDPDToolStripMenuItem,
            this.zobaczHistorięZamówieńToolStripMenuItem});
            this.kurierToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.kurierToolStripMenuItem.Name = "kurierToolStripMenuItem";
            this.kurierToolStripMenuItem.Size = new System.Drawing.Size(62, 26);
            this.kurierToolStripMenuItem.Text = "Kurier";
            // 
            // zamówKureiraDPDToolStripMenuItem
            // 
            this.zamówKureiraDPDToolStripMenuItem.Name = "zamówKureiraDPDToolStripMenuItem";
            this.zamówKureiraDPDToolStripMenuItem.Size = new System.Drawing.Size(266, 26);
            this.zamówKureiraDPDToolStripMenuItem.Text = "Zamów kureira DPD";
            // 
            // zobaczHistorięZamówieńToolStripMenuItem
            // 
            this.zobaczHistorięZamówieńToolStripMenuItem.Name = "zobaczHistorięZamówieńToolStripMenuItem";
            this.zobaczHistorięZamówieńToolStripMenuItem.Size = new System.Drawing.Size(266, 26);
            this.zobaczHistorięZamówieńToolStripMenuItem.Text = "Zobacz historię zamówień";
            // 
            // zestawieniaToolStripMenuItem
            // 
            this.zestawieniaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoMatchFvToolStripMenuItem});
            this.zestawieniaToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.zestawieniaToolStripMenuItem.Name = "zestawieniaToolStripMenuItem";
            this.zestawieniaToolStripMenuItem.Size = new System.Drawing.Size(102, 26);
            this.zestawieniaToolStripMenuItem.Text = "Zestawienia";
            // 
            // autoMatchFvToolStripMenuItem
            // 
            this.autoMatchFvToolStripMenuItem.Name = "autoMatchFvToolStripMenuItem";
            this.autoMatchFvToolStripMenuItem.Size = new System.Drawing.Size(315, 26);
            this.autoMatchFvToolStripMenuItem.Text = "Auto-uzupełnianie FV (enova)";
            this.autoMatchFvToolStripMenuItem.Click += new System.EventHandler(this.autoMatchFvToolStripMenuItem_Click);
            // 
            // refreshIcon
            // 
            this.refreshIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.refreshIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.refreshIcon.Image = global::Reklamacje_Dane.Properties.Resources.refresh_icon;
            this.refreshIcon.Location = new System.Drawing.Point(823, 1);
            this.refreshIcon.Name = "refreshIcon";
            this.refreshIcon.Size = new System.Drawing.Size(22, 20);
            this.refreshIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.refreshIcon.TabIndex = 0;
            this.refreshIcon.TabStop = false;
            this.refreshIcon.Click += new System.EventHandler(this.refreshIcon_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(1454, 761);
            this.Controls.Add(this.refreshIcon);
            this.Controls.Add(this.labelZalogowany);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.lblLastRefresh);
            this.Controls.Add(this.leftMenuPanel);
            this.Controls.Add(this.btnCloseNotification);
            this.Controls.Add(this.powiadomienie);
            this.Controls.Add(this.mainMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "System Obsługi Reklamacji";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProcessing)).EndInit();
            this.bottomSplitContainer.Panel1.ResumeLayout(false);
            this.bottomSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bottomSplitContainer)).EndInit();
            this.bottomSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewReminders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChangeLog)).EndInit();
            this.leftMenuPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refreshIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private ToolStripMenuItem toolStripMenuItem1;
        private PictureBox pictureBox1;
        private PictureBox pictureBox8;
        private PictureBox pictureBox7;
        private PictureBox pictureBox6;
        private PictureBox pictureBox5;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label8;
        private ToolStripMenuItem dodajNoweToolStripMenuItem;
        private ToolStripMenuItem zarejestrujNoweToolStripMenuItem;
        private ToolStripMenuItem pokażWyszystkieToolStripMenuItem;
        private ToolStripMenuItem oczekująceNaDostawęProsuktuToolStripMenuItem;
        private ToolStripMenuItem zgłoszoneDoPorducentabezDecyzjiToolStripMenuItem;
        private ToolStripMenuItem uzględnionePrzezProducentaToolStripMenuItem;
        private ToolStripMenuItem nieuzględnionePrzezProducentaToolStripMenuItem;
        private ToolStripMenuItem wszystykieToolStripMenuItem;
        private ToolStripMenuItem uzględnioneKlientToolStripMenuItem;
        private ToolStripMenuItem nieuzględnioneKlientowiToolStripMenuItem;
        private ToolStripMenuItem wysłaneNoweProduktyToolStripMenuItem;
        private ToolStripMenuItem naprawioneToolStripMenuItem;
        private ToolStripMenuItem skorygowaneFakturyToolStripMenuItem;
        private ToolStripMenuItem wyszukajKlientaToolStripMenuItem;
        private ToolStripMenuItem dodajKlientaToolStripMenuItem;
        private ToolStripMenuItem dodajProducentaToolStripMenuItem;
        private ToolStripMenuItem wyszukajProducentaToolStripMenuItem;
        private ToolStripMenuItem wyszukajSerwisToolStripMenuItem;
        private ToolStripMenuItem dodajSerwisToolStripMenuItem;
        private ToolStripMenuItem produktyToolStripMenuItem;
        private ToolStripMenuItem wyszukajProduktyToolStripMenuItem;
        private ToolStripMenuItem dodajProduktToolStripMenuItem;
        private ToolStripMenuItem przypomnieniaToolStripMenuItem;
        private ToolStripMenuItem wyświetlPrzypomnieniaToolStripMenuItem;
        private ToolStripMenuItem aktualneToolStripMenuItem;
        private ToolStripMenuItem zrealizowaneToolStripMenuItem;
        private ToolStripMenuItem dodajPrzypomnienieToolStripMenuItem;
        private ToolStripMenuItem kurierToolStripMenuItem;
        private ToolStripMenuItem zamówKureiraDPDToolStripMenuItem;
        private ToolStripMenuItem zobaczHistorięZamówieńToolStripMenuItem;
        private ToolStripMenuItem zestawieniaToolStripMenuItem;
        private ToolStripMenuItem autoMatchFvToolStripMenuItem;
        private ToolStripMenuItem nowyProduktToolStripMenuItem;
        private ToolStripMenuItem maZostaćWysłanyToolStripMenuItem;
        private ToolStripMenuItem dostarczonyToolStripMenuItem;
        private ToolStripMenuItem notaToolStripMenuItem;
        private ToolStripMenuItem maZostaćWystawionaToolStripMenuItem;
        private ToolStripMenuItem zostałaWystawionaToolStripMenuItem;
        private ToolStripMenuItem usterkiNieStwierdzonoToolStripMenuItem;
        private ToolStripMenuItem poOkresieReklamacjiToolStripMenuItem;
        private ToolStripMenuItem usterkaZWinyUżytkowaniaToolStripMenuItem;
        private ToolStripMenuItem brakKontaktuToolStripMenuItem;
        private Label label10;
        private Label lblAllegroCount;
        #region Puste metody dla projektanta (zaślepki)

        private void btnToggleMenu_Click(object sender, EventArgs e) { }
        private void dataGridViewProcessing_CellContentClick(object sender, DataGridViewCellEventArgs e) { }






        private void leftMenuPanel_Paint(object sender, PaintEventArgs e) { }
        private void mainMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }



        private void pictureBox7_Click(object sender, EventArgs e) { }


        private void refreshIcon_Click_1(object sender, EventArgs e) { }
        private void toolStripMenuItem1_Click(object sender, EventArgs e) { }
        private void viewToolStripMenuItem_Click(object sender, EventArgs e) { }

        #endregion


    }
}
