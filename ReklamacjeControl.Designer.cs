using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class ReklamacjeControl
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

            // --- GŁÓWNE KONTENERY ---
            this.pnlBackground = new System.Windows.Forms.Panel();
            this.panelLeftMenu = new System.Windows.Forms.Panel();
            this.pnlMenuButtons = new System.Windows.Forms.Panel();
            this.pnlMenuHeader = new System.Windows.Forms.Panel();
            this.lblAppName = new System.Windows.Forms.Label();
            this.panelTopHeader = new System.Windows.Forms.Panel();
            this.lblSyncStatus = new System.Windows.Forms.Label();
            this.lblSyncActivity = new System.Windows.Forms.Label(); // Etykieta "Co robię"
            this.lblLastRefresh = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlDashboardContent = new System.Windows.Forms.Panel();

            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.pnlProcessingWrapper = new System.Windows.Forms.Panel();
            this.dataGridViewProcessing = new System.Windows.Forms.DataGridView();
            this.panelSearchBox = new System.Windows.Forms.Panel();
            this.txtFilterProcessing = new System.Windows.Forms.TextBox();
            this.lblProcessingHeader = new System.Windows.Forms.Label();

            this.splitContainerBottom = new System.Windows.Forms.SplitContainer();
            this.pnlRemindersWrapper = new System.Windows.Forms.Panel();
            this.flowLayoutPanelReminders = new System.Windows.Forms.FlowLayoutPanel();
            this.remindersTabsBar = new System.Windows.Forms.Panel();
            this.lblRemindersHeader = new System.Windows.Forms.Label();

            this.pnlLogWrapper = new System.Windows.Forms.Panel();
            this.dataGridViewChangeLog = new System.Windows.Forms.DataGridView();
            this.lblLogHeader = new System.Windows.Forms.Label();

            this.contextMenuStripProcessing = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.otwórzZgłoszenieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dodajPrzypomnienieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usunZgloszenieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kopiujNumerZgłoszeniaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // Przyciski Menu
            this.btnHome = new System.Windows.Forms.Button();
            this.btnNewGoogle = new System.Windows.Forms.Button();
            this.btnNewAllegro = new System.Windows.Forms.Button();
            this.btnNewReturn = new System.Windows.Forms.Button();
            this.btnAddManual = new System.Windows.Forms.Button();
            this.btnAllCases = new System.Windows.Forms.Button();
            this.btnChat = new System.Windows.Forms.Button();
            this.btnReminders = new System.Windows.Forms.Button();
            this.btnClients = new System.Windows.Forms.Button();
            this.btnProducts = new System.Windows.Forms.Button();
            this.btnProducers = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnTracking = new System.Windows.Forms.Button();
            this.btnWarehouse = new System.Windows.Forms.Button();
            this.btnEmail = new System.Windows.Forms.Button();
            this.btnContactCenter = new System.Windows.Forms.Button();

            // Inicjalizacja Layoutu
            this.pnlBackground.SuspendLayout();
            this.panelLeftMenu.SuspendLayout();
            this.pnlMenuButtons.SuspendLayout();
            this.pnlMenuHeader.SuspendLayout();
            this.panelTopHeader.SuspendLayout();
            this.pnlDashboardContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.pnlProcessingWrapper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProcessing)).BeginInit();
            this.panelSearchBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).BeginInit();
            this.splitContainerBottom.Panel1.SuspendLayout();
            this.splitContainerBottom.Panel2.SuspendLayout();
            this.splitContainerBottom.SuspendLayout();
            this.pnlRemindersWrapper.SuspendLayout();
            this.pnlLogWrapper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChangeLog)).BeginInit();
            this.contextMenuStripProcessing.SuspendLayout();
            this.SuspendLayout();

            // 
            // ReklamacjeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this.Controls.Add(this.pnlBackground);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Name = "ReklamacjeControl";
            this.Size = new System.Drawing.Size(1400, 850);

            // 
            // pnlBackground
            // 
            this.pnlBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackground.Controls.Add(this.pnlDashboardContent);
            this.pnlBackground.Controls.Add(this.panelTopHeader);
            this.pnlBackground.Controls.Add(this.panelLeftMenu);

            // 
            // panelLeftMenu
            // 
            this.panelLeftMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeftMenu.Width = 260;
            this.panelLeftMenu.BackColor = System.Drawing.Color.FromArgb(21, 32, 54);
            this.panelLeftMenu.Controls.Add(this.pnlMenuButtons);
            this.panelLeftMenu.Controls.Add(this.pnlMenuHeader);

            this.pnlMenuHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenuHeader.Height = 70;
            this.pnlMenuHeader.BackColor = System.Drawing.Color.FromArgb(18, 28, 48);
            this.pnlMenuHeader.Controls.Add(this.lblAppName);

            this.lblAppName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAppName.Text = "REKLAMACJE";
            this.lblAppName.ForeColor = Color.White;
            this.lblAppName.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblAppName.TextAlign = ContentAlignment.MiddleCenter;

            this.pnlMenuButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMenuButtons.AutoScroll = true;
            this.pnlMenuButtons.Padding = new Padding(0, 10, 0, 10);

            AddMenuButton(btnEmail, "📧 Skrzynka Email");
            AddMenuButton(btnWarehouse, "📦 Magazyn Części");
            AddMenuButton(btnTracking, "🚚 Śledzenie DPD");
            AddMenuButton(btnSettings, "⚙️ Ustawienia");
            AddMenuButton(btnProducers, "🏭 Producenci");
            AddMenuButton(btnProducts, "📦 Produkty");
            AddMenuButton(btnClients, "👥 Klienci");
            AddMenuButton(btnReminders, "⏰ Przypomnienia");
            AddMenuButton(btnChat, "💬 Czat Allegro");
            AddMenuButton(btnContactCenter, "📞 Centrum Kontaktu");
            AddMenuButton(btnAllCases, "🗂 Wszystkie Zgłoszenia");
            AddMenuButton(btnAddManual, "➕ Dodaj Ręcznie");
            AddMenuButton(btnNewReturn, "↩️ Nowe Zwroty", Color.FromArgb(230, 81, 0));
            AddMenuButton(btnNewAllegro, "🟠 Nowe Allegro", Color.FromArgb(230, 81, 0));
            AddMenuButton(btnNewGoogle, "🟢 Nowe Google", Color.FromArgb(46, 125, 50));
            AddMenuButton(btnHome, "🏠 Strona Główna", Color.FromArgb(41, 121, 255));
          
            // 
            // panelTopHeader (Pasek Górny)
            // 
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Height = 60;
            this.panelTopHeader.BackColor = Color.White;
            this.panelTopHeader.Paint += (s, e) => e.Graphics.DrawLine(Pens.LightGray, 0, 59, panelTopHeader.Width, 59);

            // KOLEJNOŚĆ DOKOWANIA JEST KLUCZOWA DLA WIDOCZNOŚCI
            // Dock=Right "upycha" od prawej do lewej.
            // 1. Najpierw Status (będzie skrajnie po prawej)
            // 2. Potem Aktywność (będzie na lewo od statusu)

            this.panelTopHeader.Controls.Add(this.lblSyncActivity);
            this.panelTopHeader.Controls.Add(this.lblSyncStatus);
            this.panelTopHeader.Controls.Add(this.lblLastRefresh);
            this.panelTopHeader.Controls.Add(this.btnRefresh);

            // Przycisk Odśwież
            this.btnRefresh.Text = "Odśwież";
            this.btnRefresh.Location = new Point(20, 15);
            this.btnRefresh.Size = new Size(100, 30);
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.BackColor = Color.FromArgb(240, 242, 245);
            this.btnRefresh.FlatAppearance.BorderSize = 0;

            this.lblLastRefresh.AutoSize = true;
            this.lblLastRefresh.Location = new Point(135, 20);
            this.lblLastRefresh.ForeColor = Color.Gray;
            this.lblLastRefresh.Text = "Ostatnie odświeżenie: --:--";

            // Status (Skrajnie prawo)
            this.lblSyncStatus.Dock = DockStyle.Right;
            this.lblSyncStatus.TextAlign = ContentAlignment.MiddleRight;
            this.lblSyncStatus.Padding = new Padding(0, 0, 20, 0);
            this.lblSyncStatus.Text = "Synchronizacja: OK";
            this.lblSyncStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblSyncStatus.ForeColor = Color.ForestGreen;
            this.lblSyncStatus.AutoSize = true;

            // Aktywność (Lewo od statusu)
            this.lblSyncActivity.Dock = DockStyle.Right;
            this.lblSyncActivity.TextAlign = ContentAlignment.MiddleRight;
            this.lblSyncActivity.Padding = new Padding(0, 0, 10, 0);
            this.lblSyncActivity.Text = "Oczekiwanie na zadania..."; // Tekst startowy
            this.lblSyncActivity.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            this.lblSyncActivity.ForeColor = Color.SteelBlue;
            this.lblSyncActivity.AutoSize = true;

            // 
            // pnlDashboardContent
            // 
            this.pnlDashboardContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDashboardContent.Padding = new Padding(20);
            this.pnlDashboardContent.Controls.Add(this.splitContainerMain);

            // SPLITTER GŁÓWNY (GÓRA / DÓŁ)
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Orientation = Orientation.Horizontal;
            this.splitContainerMain.SplitterDistance = 450;
            this.splitContainerMain.SplitterWidth = 10;
            this.splitContainerMain.FixedPanel = FixedPanel.None; // Pozwalamy na skalowanie obu

            // 1. GÓRA: TABELA ZGŁOSZEŃ
            this.pnlProcessingWrapper.Dock = DockStyle.Fill;
            this.pnlProcessingWrapper.BackColor = Color.White;
            this.pnlProcessingWrapper.Padding = new Padding(1);
            this.pnlProcessingWrapper.Paint += (s, e) => ControlPaint.DrawBorder(e.Graphics, pnlProcessingWrapper.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            this.splitContainerMain.Panel1.Controls.Add(this.pnlProcessingWrapper);

            this.dataGridViewProcessing.Dock = DockStyle.Fill;
            this.dataGridViewProcessing.BorderStyle = BorderStyle.None;
            this.dataGridViewProcessing.BackgroundColor = Color.White;
            this.dataGridViewProcessing.ContextMenuStrip = this.contextMenuStripProcessing;
            StyleGrid(this.dataGridViewProcessing);

            this.panelSearchBox.Dock = DockStyle.Top;
            this.panelSearchBox.Height = 40;
            this.panelSearchBox.BackColor = Color.White;
            this.panelSearchBox.Controls.Add(this.txtFilterProcessing);

            this.txtFilterProcessing.Dock = DockStyle.Fill;
            this.txtFilterProcessing.Font = new Font("Segoe UI", 12F);
            this.txtFilterProcessing.BorderStyle = BorderStyle.FixedSingle;
            this.txtFilterProcessing.Margin = new Padding(5);

            this.lblProcessingHeader.Text = "  Zgłoszenia w toku";
            this.lblProcessingHeader.Dock = DockStyle.Top;
            this.lblProcessingHeader.Height = 40;
            this.lblProcessingHeader.TextAlign = ContentAlignment.MiddleLeft;
            this.lblProcessingHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblProcessingHeader.BackColor = Color.White;

            this.pnlProcessingWrapper.Controls.Add(this.dataGridViewProcessing);
            this.pnlProcessingWrapper.Controls.Add(this.panelSearchBox);
            this.pnlProcessingWrapper.Controls.Add(this.lblProcessingHeader);


            // 2. DÓŁ: SPLITTER (Lewo: Przypomnienia / Prawo: Logi)
            this.splitContainerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            // WAŻNE: SplitterDistance ustawimy w kodzie (ReklamacjeControl.cs) na 50%
            this.splitContainerBottom.SplitterWidth = 10;
            this.splitContainerBottom.BackColor = Color.FromArgb(240, 242, 245);
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerBottom);

            // LEWO DÓŁ: PRZYPOMNIENIA
            this.pnlRemindersWrapper.Dock = DockStyle.Fill;
            this.pnlRemindersWrapper.BackColor = Color.White;
            this.pnlRemindersWrapper.Padding = new Padding(10);
            this.splitContainerBottom.Panel1.Controls.Add(this.pnlRemindersWrapper);

            this.lblRemindersHeader.Text = "Przypomnienia";
            this.lblRemindersHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblRemindersHeader.Dock = DockStyle.Top;

            this.remindersTabsBar.Dock = DockStyle.Top;
            this.remindersTabsBar.Height = 35;

            this.flowLayoutPanelReminders.Dock = DockStyle.Fill;
            this.flowLayoutPanelReminders.AutoScroll = true;
            this.flowLayoutPanelReminders.BackColor = Color.WhiteSmoke;

            this.pnlRemindersWrapper.Controls.Add(this.flowLayoutPanelReminders);
            this.pnlRemindersWrapper.Controls.Add(this.remindersTabsBar);
            this.pnlRemindersWrapper.Controls.Add(this.lblRemindersHeader);

            // PRAWO DÓŁ: LOGI
            this.pnlLogWrapper.Dock = DockStyle.Fill;
            this.pnlLogWrapper.BackColor = Color.White;
            this.pnlLogWrapper.Padding = new Padding(10);
            this.splitContainerBottom.Panel2.Controls.Add(this.pnlLogWrapper);

            this.lblLogHeader.Text = "Ostatnie zdarzenia";
            this.lblLogHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblLogHeader.Dock = DockStyle.Top;

            this.dataGridViewChangeLog.Dock = DockStyle.Fill;
            this.dataGridViewChangeLog.BorderStyle = BorderStyle.None;
            this.dataGridViewChangeLog.BackgroundColor = Color.White;
            StyleGrid(this.dataGridViewChangeLog);

            this.pnlLogWrapper.Controls.Add(this.dataGridViewChangeLog);
            this.pnlLogWrapper.Controls.Add(this.lblLogHeader);

            // MENU KONTEKSTOWE
            this.otwórzZgłoszenieToolStripMenuItem.Text = "📂 Otwórz zgłoszenie";
            this.dodajPrzypomnienieToolStripMenuItem.Text = "⏰ Dodaj przypomnienie";
            this.usunZgloszenieToolStripMenuItem.Text = "🗑 Archiwizuj";
            this.kopiujNumerZgłoszeniaToolStripMenuItem.Text = "📋 Kopiuj numer";
            this.contextMenuStripProcessing.Items.AddRange(new ToolStripItem[] {
                otwórzZgłoszenieToolStripMenuItem, dodajPrzypomnienieToolStripMenuItem, new ToolStripSeparator(), kopiujNumerZgłoszeniaToolStripMenuItem, usunZgloszenieToolStripMenuItem
            });

            // FINALIZACJA
            this.pnlBackground.ResumeLayout(false);
            this.panelLeftMenu.ResumeLayout(false);
            this.pnlMenuButtons.ResumeLayout(false);
            this.pnlMenuHeader.ResumeLayout(false);
            this.panelTopHeader.ResumeLayout(false);
            this.panelTopHeader.PerformLayout();
            this.pnlDashboardContent.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.pnlProcessingWrapper.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProcessing)).EndInit();
            this.panelSearchBox.ResumeLayout(false);
            this.panelSearchBox.PerformLayout();
            this.splitContainerBottom.Panel1.ResumeLayout(false);
            this.splitContainerBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).EndInit();
            this.splitContainerBottom.ResumeLayout(false);
            this.pnlRemindersWrapper.ResumeLayout(false);
            this.pnlLogWrapper.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewChangeLog)).EndInit();
            this.contextMenuStripProcessing.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void AddMenuButton(Button btn, string text, Color? highlightColor = null)
        {
            btn.Text = "   " + text;
            btn.Dock = DockStyle.Top;
            btn.Height = 50;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(21, 32, 54);
            btn.ForeColor = highlightColor ?? Color.FromArgb(180, 190, 210);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btn.Cursor = Cursors.Hand;
            btn.Padding = new Padding(10, 0, 0, 0);
            this.pnlMenuButtons.Controls.Add(btn);
        }

        private void StyleGrid(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;
            dgv.AllowUserToResizeRows = false;
            dgv.Font = new Font("Segoe UI", 9.5F);
            dgv.ColumnHeadersHeight = 40;
            dgv.RowTemplate.Height = 35;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.DimGray;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 245, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        // DEKLARACJE
        private Panel pnlBackground;
        private Panel panelLeftMenu;
        private Panel pnlMenuButtons;
        private Panel pnlMenuHeader;
        private Label lblAppName;
        private Panel panelTopHeader;
        private Label lblSyncStatus;
        private Label lblSyncActivity;
        private Label lblLastRefresh;
        private Button btnRefresh;
        private Panel pnlDashboardContent;
        private SplitContainer splitContainerMain;
        private Panel pnlProcessingWrapper;
        private DataGridView dataGridViewProcessing;
        private Panel panelSearchBox;
        private TextBox txtFilterProcessing;
        private Label lblProcessingHeader;
        private SplitContainer splitContainerBottom;
        private Panel pnlRemindersWrapper;
        private FlowLayoutPanel flowLayoutPanelReminders;
        private Panel remindersTabsBar;
        private Label lblRemindersHeader;
        private Panel pnlLogWrapper;
        private DataGridView dataGridViewChangeLog;
        private Label lblLogHeader;
        private ContextMenuStrip contextMenuStripProcessing;
        private ToolStripMenuItem otwórzZgłoszenieToolStripMenuItem;
        private ToolStripMenuItem dodajPrzypomnienieToolStripMenuItem;
        private ToolStripMenuItem usunZgloszenieToolStripMenuItem;
        private ToolStripMenuItem kopiujNumerZgłoszeniaToolStripMenuItem;
        private Button btnHome, btnNewGoogle, btnNewAllegro, btnNewReturn, btnAddManual, btnAllCases, btnChat, btnReminders, btnClients, btnProducts, btnProducers, btnSettings, btnTracking, btnWarehouse, btnEmail, btnContactCenter;

        #endregion
    }
}