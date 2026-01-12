using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class Form2
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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.lblHeaderStatus = new System.Windows.Forms.Label();

            this.panelLeftSidebar = new System.Windows.Forms.Panel();
            this.panelCentralContainer = new System.Windows.Forms.Panel();

            this.tlpTopInfo = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();

            // --- MENU BOCZNE (CIEMNE) ---
            this.btnPrintToPdf = new System.Windows.Forms.Button();
            this.btnMagazyn = new System.Windows.Forms.Button();
            this.btnAllegroModule = new System.Windows.Forms.Button();
            this.pnlMenuSeparator = new System.Windows.Forms.Panel();
            this.button11 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonWyslijMail = new System.Windows.Forms.Button();

            // --- OPIS I HISTORIA ---
            this.pnlHistoryWrapper = new System.Windows.Forms.Panel();
            this.panelOpisBox = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnZapiszOpis = new System.Windows.Forms.Button();
            this.lblOpisHeader = new System.Windows.Forms.Label();

            this.lblHistoryHeader = new System.Windows.Forms.Label();
            this.btnAddAction = new System.Windows.Forms.Button();
            this.panelHistoriaContainer = new System.Windows.Forms.Panel();
            this.flowLayoutPanelHistory = new System.Windows.Forms.FlowLayoutPanel();
            this.flowDocuments = new System.Windows.Forms.FlowLayoutPanel();
            this.lblDocumentsHeader = new System.Windows.Forms.Label();

            // --- CZAT ---
            this.pnlChatWrapper = new System.Windows.Forms.Panel();
            this.lblChatHeader = new System.Windows.Forms.Label();
            this.flowChatRight = new System.Windows.Forms.FlowLayoutPanel();

            // --- INNE ---
            this.pnlShipmentHistory = new System.Windows.Forms.Panel();
            this.dgvHistoriaPrzesylki = new System.Windows.Forms.DataGridView();
            this.btnBackToDetails = new System.Windows.Forms.Button();

            this.contextMenuKurier = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.zamowOdKlientaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zamowDoKlientaMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.productInfoControl1 = new Reklamacje_Dane.ProductInfoControl();
            this.clientInfoControl1 = new Reklamacje_Dane.ClientInfoControl();
            this.label15 = new System.Windows.Forms.Label();

            // --- INICJALIZACJA ---
            this.panelHeader.SuspendLayout();
            this.panelLeftSidebar.SuspendLayout();
            this.panelCentralContainer.SuspendLayout();
            this.tlpTopInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.pnlHistoryWrapper.SuspendLayout();
            this.panelOpisBox.SuspendLayout();
            this.panelHistoriaContainer.SuspendLayout();
            this.pnlChatWrapper.SuspendLayout();
            this.pnlShipmentHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistoriaPrzesylki)).BeginInit();
            this.contextMenuKurier.SuspendLayout();
            this.SuspendLayout();

            // ==============================================================================
            // KONFIGURACJA WIZUALNA
            // ==============================================================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.MinimumSize = new System.Drawing.Size(1200, 800);
            this.Name = "Form2";
            this.Text = "Centrum Obsługi Zgłoszenia";
            this.WindowState = FormWindowState.Maximized;

            // --- NAGŁÓWEK (CIEMNONIEBIESKI) ---
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 192);
            this.panelHeader.Controls.Add(this.lblHeaderStatus);
            this.panelHeader.Controls.Add(this.lblHeaderTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 80;
            this.panelHeader.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);

            this.lblHeaderTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.White;
            this.lblHeaderTitle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblHeaderTitle.Height = 45;
            this.lblHeaderTitle.Text = "Ładowanie zgłoszenia...";

            this.lblHeaderStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeaderStatus.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblHeaderStatus.ForeColor = System.Drawing.Color.FromArgb(200, 230, 255);
            this.lblHeaderStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblHeaderStatus.Height = 35;
            this.lblHeaderStatus.Text = "";

            // --- MENU LEWE (CIEMNE, SPÓJNE Z NAGŁÓWKIEM) ---
            this.panelLeftSidebar.BackColor = System.Drawing.Color.FromArgb(13, 71, 161); // Ciemniejszy odcień niebieskiego
            this.panelLeftSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeftSidebar.Width = 260;
            this.panelLeftSidebar.AutoScroll = true;
            this.panelLeftSidebar.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);

            // Kolejność odwrotna (Dock=Top)
            this.panelLeftSidebar.Controls.Add(this.btnPrintToPdf);
            this.panelLeftSidebar.Controls.Add(this.btnMagazyn);
            this.panelLeftSidebar.Controls.Add(this.btnAllegroModule);
            this.panelLeftSidebar.Controls.Add(this.pnlMenuSeparator);
            this.panelLeftSidebar.Controls.Add(this.button11);
            this.panelLeftSidebar.Controls.Add(this.button9);
            this.panelLeftSidebar.Controls.Add(this.button8);
            this.panelLeftSidebar.Controls.Add(this.button7);
            this.panelLeftSidebar.Controls.Add(this.button6);
            this.panelLeftSidebar.Controls.Add(this.button5);
            this.panelLeftSidebar.Controls.Add(this.button4);
            this.panelLeftSidebar.Controls.Add(this.button3);
            this.panelLeftSidebar.Controls.Add(this.button2);
            this.panelLeftSidebar.Controls.Add(this.button1);
            this.panelLeftSidebar.Controls.Add(this.buttonWyslijMail);

            // Kolory przycisków w menu: Tło Ciemne, Tekst Biały
            Color btnBg = Color.FromArgb(13, 71, 161); // Tło menu
            Color btnHover = Color.FromArgb(21, 101, 192); // Jaśniejszy przy najechaniu

            ConfigureMenuButton(buttonWyslijMail, "📧 Kontakt z Klientem", btnBg, btnHover);
            ConfigureMenuButton(button1, "🚚 Zamów Kuriera", btnBg, btnHover);
            ConfigureMenuButton(button2, "🏭 Zgłoś do Producenta", btnBg, btnHover);
            ConfigureMenuButton(button3, "⚖️ Decyzja Producenta", btnBg, btnHover);
            ConfigureMenuButton(button4, "🔄 Zmień Status", btnBg, btnHover);
            ConfigureMenuButton(button5, "📄 Wydanie WRL", btnBg, btnHover);
            ConfigureMenuButton(button6, "📄 Korekta KWZ", btnBg, btnHover);
            ConfigureMenuButton(button7, "💰 Faktura Zysk/Koszt", btnBg, btnHover);
            ConfigureMenuButton(button8, "📎 Dodaj Plik", btnBg, btnHover);
            ConfigureMenuButton(button9, "📂 Zobacz Pliki", btnBg, btnHover);
            ConfigureMenuButton(button11, "📦 Śledzenie DPD", btnBg, btnHover);

            pnlMenuSeparator.Dock = DockStyle.Top; pnlMenuSeparator.Height = 20;

            ConfigureMenuButton(btnAllegroModule, "🟠 Moduł Allegro", Color.FromArgb(230, 81, 0), Color.FromArgb(239, 108, 0)); // Pomarańczowy
            ConfigureMenuButton(btnMagazyn, "🔧 Magazyn Części", Color.FromArgb(38, 50, 56), Color.FromArgb(55, 71, 79)); // Ciemnoszary

            ConfigureMenuButton(btnPrintToPdf, "🖨️ Drukuj PDF", Color.FromArgb(183, 28, 28), Color.FromArgb(211, 47, 47)); // Czerwony
            btnPrintToPdf.Dock = DockStyle.Bottom;

            // --- GŁÓWNY KONTENER ---
            this.panelCentralContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCentralContainer.Controls.Add(this.splitContainerMain);
            this.panelCentralContainer.Controls.Add(this.pnlShipmentHistory); // Ukryte
            this.panelCentralContainer.Controls.Add(this.tlpTopInfo);
            this.panelCentralContainer.Padding = new System.Windows.Forms.Padding(15);

            // --- GÓRA: KLIENT I PRODUKT ---
            this.tlpTopInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpTopInfo.Height = 240;
            this.tlpTopInfo.ColumnCount = 2;
            this.tlpTopInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTopInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTopInfo.Controls.Add(this.clientInfoControl1, 0, 0);
            this.tlpTopInfo.Controls.Add(this.productInfoControl1, 1, 0);
            this.clientInfoControl1.Dock = DockStyle.Fill;
            this.clientInfoControl1.Margin = new Padding(0, 0, 10, 10);
            this.productInfoControl1.Dock = DockStyle.Fill;
            this.productInfoControl1.Margin = new Padding(10, 0, 0, 10);

            // --- SPLITTER ---
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(15, 255);
            this.splitContainerMain.SplitterDistance = 650;
            this.splitContainerMain.SplitterWidth = 10;
            this.splitContainerMain.BackColor = Color.FromArgb(230, 230, 230);

            // LEWA STRONA (HISTORIA)
            this.pnlHistoryWrapper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHistoryWrapper.BackColor = Color.White;
            this.pnlHistoryWrapper.Padding = new Padding(20);
            this.splitContainerMain.Panel1.Controls.Add(this.pnlHistoryWrapper);

            // Kolejność dodawania (Fill na końcu w kodzie = środek na ekranie)

            // 5. Historia (Wypełnia resztę) - TU BYŁ PROBLEM Z UCINANIEM
            this.panelHistoriaContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHistoriaContainer.Padding = new Padding(1); // Ramka
            this.panelHistoriaContainer.BackColor = Color.LightGray;
            this.pnlHistoryWrapper.Controls.Add(this.panelHistoriaContainer);

            // Wewnętrzny panel na historię (biały)
            Panel pnlHistInner = new Panel();
            pnlHistInner.Dock = DockStyle.Fill;
            pnlHistInner.BackColor = Color.White;
            this.panelHistoriaContainer.Controls.Add(pnlHistInner);

            this.flowLayoutPanelHistory.Dock = DockStyle.Fill;
            this.flowLayoutPanelHistory.AutoScroll = true; // WAŻNE
            this.flowLayoutPanelHistory.FlowDirection = FlowDirection.TopDown;
            this.flowLayoutPanelHistory.WrapContents = false; // WAŻNE: Nie zawijaj, tylko scrolluj w dół
            pnlHistInner.Controls.Add(this.flowLayoutPanelHistory);

            // 4. Przycisk Dodaj
            this.btnAddAction.Text = "+ Dodaj notatkę / działanie";
            this.btnAddAction.Dock = DockStyle.Top;
            this.btnAddAction.Height = 35;
            this.btnAddAction.BackColor = Color.FromArgb(227, 242, 253);
            this.btnAddAction.FlatStyle = FlatStyle.Flat;
            this.btnAddAction.FlatAppearance.BorderSize = 0;
            this.btnAddAction.ForeColor = Color.FromArgb(13, 71, 161);
            this.btnAddAction.Cursor = Cursors.Hand;
            this.pnlHistoryWrapper.Controls.Add(this.btnAddAction);

            // 3. Nagłówek Historii
            this.lblHistoryHeader.Text = "📋 Działania Wewnętrzne";
            this.lblHistoryHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblHistoryHeader.Dock = DockStyle.Top;
            this.lblHistoryHeader.Height = 40;
            this.lblHistoryHeader.TextAlign = ContentAlignment.BottomLeft;
            this.pnlHistoryWrapper.Controls.Add(this.lblHistoryHeader);

            // 2. Dokumenty
            this.flowDocuments.Dock = DockStyle.Top;
            this.flowDocuments.AutoSize = true;
            this.flowDocuments.MinimumSize = new Size(0, 30);
            this.flowDocuments.Padding = new Padding(0, 5, 0, 10);
            this.pnlHistoryWrapper.Controls.Add(this.flowDocuments);

            this.lblDocumentsHeader.Text = "Dokumenty:";
            this.lblDocumentsHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblDocumentsHeader.ForeColor = Color.Gray;
            this.lblDocumentsHeader.Dock = DockStyle.Top;
            this.pnlHistoryWrapper.Controls.Add(this.lblDocumentsHeader);

            // 1. Opis Usterki
            this.panelOpisBox.Dock = DockStyle.Top;
            this.panelOpisBox.Height = 120;
            this.panelOpisBox.Padding = new Padding(1);
            this.panelOpisBox.BackColor = Color.LightGray;

            this.textBox1.Dock = DockStyle.Fill;
            this.textBox1.Multiline = true;
            this.textBox1.BorderStyle = BorderStyle.None;
            this.panelOpisBox.Controls.Add(this.textBox1);

            this.btnZapiszOpis.Text = "Zapisz zmianę opisu";
            this.btnZapiszOpis.Dock = DockStyle.Bottom;
            this.btnZapiszOpis.Height = 25;
            this.btnZapiszOpis.BackColor = Color.ForestGreen;
            this.btnZapiszOpis.ForeColor = Color.White;
            this.btnZapiszOpis.FlatStyle = FlatStyle.Flat;
            this.btnZapiszOpis.FlatAppearance.BorderSize = 0;
            this.panelOpisBox.Controls.Add(this.btnZapiszOpis);

            this.pnlHistoryWrapper.Controls.Add(this.panelOpisBox);

            this.lblOpisHeader.Text = "📝 Opis Usterki";
            this.lblOpisHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblOpisHeader.Dock = DockStyle.Top;
            this.lblOpisHeader.Height = 35;
            this.pnlHistoryWrapper.Controls.Add(this.lblOpisHeader);

            // PRAWA STRONA (Czat)
            this.pnlChatWrapper.Dock = DockStyle.Fill;
            this.pnlChatWrapper.BackColor = Color.White;
            this.pnlChatWrapper.Padding = new Padding(0);
            this.splitContainerMain.Panel2.Controls.Add(this.pnlChatWrapper);

            this.lblChatHeader.Text = "💬 Korespondencja (E-mail, SMS, Allegro)";
            this.lblChatHeader.Dock = DockStyle.Top;
            this.lblChatHeader.Height = 40;
            this.lblChatHeader.BackColor = Color.WhiteSmoke;
            this.lblChatHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.lblChatHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.pnlChatWrapper.Controls.Add(this.lblChatHeader);

            this.flowChatRight.Dock = DockStyle.Fill;
            this.flowChatRight.AutoScroll = true;
            this.flowChatRight.FlowDirection = FlowDirection.TopDown;
            this.flowChatRight.WrapContents = false;
            this.flowChatRight.BackColor = Color.FromArgb(250, 250, 250);
            this.pnlChatWrapper.Controls.Add(this.flowChatRight);
            this.flowChatRight.BringToFront();

            // Menu Kontekstowe
            this.zamowOdKlientaMenuItem.Text = "Odbiór OD Klienta";
            this.zamowDoKlientaMenuItem.Text = "Wysyłka DO Klienta";
            this.contextMenuKurier.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.zamowOdKlientaMenuItem, this.zamowDoKlientaMenuItem });
            this.button1.ContextMenuStrip = this.contextMenuKurier;

            // Ukryty panel DPD
            this.pnlShipmentHistory.Dock = DockStyle.Fill;
            this.pnlShipmentHistory.Visible = false;
            this.pnlShipmentHistory.Controls.Add(this.dgvHistoriaPrzesylki);
            this.pnlShipmentHistory.Controls.Add(this.btnBackToDetails);
            this.dgvHistoriaPrzesylki.Dock = DockStyle.Fill;
            this.btnBackToDetails.Dock = DockStyle.Bottom;
            this.btnBackToDetails.Text = "WRÓĆ";

            // SKŁADANIE
            this.Controls.Add(this.panelCentralContainer);
            this.Controls.Add(this.panelLeftSidebar);
            this.Controls.Add(this.panelHeader);

            this.panelHeader.ResumeLayout(false);
            this.panelLeftSidebar.ResumeLayout(false);
            this.panelCentralContainer.ResumeLayout(false);
            this.tlpTopInfo.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.pnlHistoryWrapper.ResumeLayout(false);
            this.pnlHistoryWrapper.PerformLayout();
            this.panelOpisBox.ResumeLayout(false);
            this.panelOpisBox.PerformLayout();
            this.panelHistoriaContainer.ResumeLayout(false);
            this.pnlChatWrapper.ResumeLayout(false);
            this.pnlShipmentHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistoriaPrzesylki)).EndInit();
            this.contextMenuKurier.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void ConfigureMenuButton(Button btn, string text, Color bgColor, Color hoverColor)
        {
            btn.Text = "  " + text;
            btn.Dock = DockStyle.Top;
            btn.Height = 45;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = bgColor;
            btn.ForeColor = Color.White;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Font = new Font("Segoe UI", 10F);
            btn.Cursor = Cursors.Hand;

            // Efekt hover
            btn.MouseEnter += (s, e) => { btn.BackColor = hoverColor; };
            btn.MouseLeave += (s, e) => { btn.BackColor = bgColor; };
        }

        // Deklaracje
        private Panel panelHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderStatus;
        private Panel panelLeftSidebar;
        private Panel pnlMenuSeparator;
        private Panel panelCentralContainer;
        private TableLayoutPanel tlpTopInfo;
        private SplitContainer splitContainerMain;

        private Panel pnlHistoryWrapper;
        private Label lblOpisHeader;
        private Panel panelOpisBox;
        private TextBox textBox1;
        private Button btnZapiszOpis;
        private Label lblDocumentsHeader;
        private FlowLayoutPanel flowDocuments;
        private Label lblHistoryHeader;
        private Button btnAddAction;
        private Panel panelHistoriaContainer;
        private FlowLayoutPanel flowLayoutPanelHistory;

        private Panel pnlChatWrapper;
        private Label lblChatHeader;
        private FlowLayoutPanel flowChatRight;
        private Panel pnlShipmentHistory;
        private DataGridView dgvHistoriaPrzesylki;
        private Button btnBackToDetails;
        private Reklamacje_Dane.ProductInfoControl productInfoControl1;
        private Reklamacje_Dane.ClientInfoControl clientInfoControl1;
        private ContextMenuStrip contextMenuKurier;
        private ToolStripMenuItem zamowOdKlientaMenuItem;
        private ToolStripMenuItem zamowDoKlientaMenuItem;
        private Button buttonWyslijMail;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button11;
        private Button btnAllegroModule;
        private Button btnMagazyn;
        private Button btnPrintToPdf;
        private Label label15;

        #endregion
    }
}