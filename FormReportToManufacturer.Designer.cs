using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class FormReportToManufacturer
    {
        private System.ComponentModel.IContainer components = null;

        // KONTROLKI PUBLICZNE
        public ComboBox cmbNadawca;
        public TextBox txtEmailTo;
        public TextBox txtEmailSubject;

        public TabControl tabInput;
        public TabPage pageDane;
        public TabPage pageSpecyficzne;
        public TabPage pageZalaczniki;

        public TextBox txtEdytowalnyKod;
        public TextBox txtEdytowalnyOpis;
        public TextBox txtEmailNotes;
        public TextBox txtEdytowalnaFaktura;
        public TextBox txtEdytowalnySN;
        public DateTimePicker dtpEdytowalnaData;
        public Button btnTranslate;
        public CheckBox chkZachowajPolski;

        public CheckBox chkIncFV;
        public CheckBox chkIncSN;
        public CheckBox chkIncData;

        public Panel pnlSpecyficzneContainer;
        public TextBox txtHellaOkolicznosci;
        public TextBox txtHellaUwagi;
        public TextBox txtStrandsNrKabla;
        public NumericUpDown numStrandsCzas;
        public ComboBox cmbStrandsJednostka;
        public ComboBox cmbStrandsPojazd;
        public ComboBox cmbStrandsNapiecie;
        public ComboBox cmbStrandsMontaz1;
        public ComboBox cmbStrandsMontaz2;

        public ListView listFiles;
        public Panel pnlWebViewContainer;
        public PictureBox picPreview;
        public Label lblPreviewTitle;
        public Button btnShowMailPreview;

        private Button btnSend;
        private Button btnCancel;
        private Label lblWymaganiaInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Kreator Zgłoszenia Reklamacyjnego";
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.BackColor = Color.WhiteSmoke;

            var mainTable = new TableLayoutPanel();
            mainTable.Dock = DockStyle.Fill;
            mainTable.RowCount = 3;
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            this.Controls.Add(mainTable);

            // INFO
            var pnlInfo = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(255, 248, 225), Padding = new Padding(15, 12, 10, 5) };
            lblWymaganiaInfo = new Label { Dock = DockStyle.Fill, Text = "Wymagania Producenta: ...", ForeColor = Color.FromArgb(180, 80, 0), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            pnlInfo.Controls.Add(lblWymaganiaInfo);
            mainTable.Controls.Add(pnlInfo, 0, 0);

            // SPLIT
            var splitMain = new SplitContainer();
            splitMain.Dock = DockStyle.Fill;
            splitMain.FixedPanel = FixedPanel.Panel1;

            // --- POPRAWKA SZEROKOŚCI ---
            splitMain.Panel1MinSize = 500; // Mniejsze minimum, żeby nie blokowało
            this.Shown += (s, e) => { splitMain.SplitterDistance = 850; };

            mainTable.Controls.Add(splitMain, 0, 1);

            mainTable.Controls.Add(splitMain, 0, 1);

            // === LEWY PANEL ===
            var tlpLeft = new TableLayoutPanel();
            tlpLeft.Dock = DockStyle.Fill;
            tlpLeft.RowCount = 2;
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 180F));
            tlpLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            splitMain.Panel1.Controls.Add(tlpLeft);

            // A. Header
            var grpHeader = new GroupBox { Text = "1. Konfiguracja Wysyłki", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.White };
            var pnlHeaderInner = new Panel { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F), Padding = new Padding(10) };

            var lblOd = new Label { Text = "Nadawca:", Location = new Point(10, 20), AutoSize = true, ForeColor = Color.Gray };
            cmbNadawca = new ComboBox { Location = new Point(10, 40), Width = 350, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.AliceBlue };

            var lblDo = new Label { Text = "Odbiorca:", Location = new Point(380, 20), AutoSize = true, ForeColor = Color.Gray };
            txtEmailTo = new TextBox { Location = new Point(380, 40), Width = 350 };

            var lblTemat = new Label { Text = "Temat:", Location = new Point(10, 80), AutoSize = true, ForeColor = Color.Gray };
            txtEmailSubject = new TextBox { Location = new Point(10, 100), Width = 720 };

            pnlHeaderInner.Controls.AddRange(new Control[] { lblOd, cmbNadawca, lblDo, txtEmailTo, lblTemat, txtEmailSubject });
            grpHeader.Controls.Add(pnlHeaderInner);
            tlpLeft.Controls.Add(grpHeader, 0, 0);

            // B. Zakładki
            tabInput = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F), Padding = new Point(15, 8) };

            // KARTA 1: DANE
            pageDane = new TabPage("2. Dane i Treść");
            pageDane.BackColor = Color.White;
            pageDane.AutoScroll = true;
            pageDane.Padding = new Padding(15);

            var flowDane = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
            int fieldW = 750;

            // Kod
            flowDane.Controls.Add(new Label { Text = "Kod Produktu:", AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(3, 10, 3, 0) });
            txtEdytowalnyKod = new TextBox { Width = fieldW, Height = 30 };
            flowDane.Controls.Add(txtEdytowalnyKod);

            // Opis
            flowDane.Controls.Add(new Label { Text = "Opis Usterki (Widoczny w PDF i treści maila):", AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(3, 15, 3, 0) });
            txtEdytowalnyOpis = new TextBox { Width = fieldW, Height = 130, Multiline = true, ScrollBars = ScrollBars.Vertical };
            flowDane.Controls.Add(txtEdytowalnyOpis);

            // Tłumacz
            var pnlTrans = new Panel { Width = fieldW, Height = 40, Margin = new Padding(0, 5, 0, 0) };
            btnTranslate = new Button { Text = "Tłumacz opis (DeepL)", Width = 180, Height = 30, BackColor = Color.Orange, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, Cursor = Cursors.Hand };
            chkZachowajPolski = new CheckBox { Text = "Zachowaj polski tekst (dopisz angielski poniżej)", Location = new Point(200, 5), Checked = true, AutoSize = true };
            pnlTrans.Controls.AddRange(new Control[] { btnTranslate, chkZachowajPolski });
            flowDane.Controls.Add(pnlTrans);

            // --- TABELA DANYCH (Poprawiony układ - MAKSYMALNE ZBLIŻENIE) ---
            var grpOpcje = new GroupBox { Text = "Dane w tabeli zgłoszenia (Włącz/Wyłącz)", Width = fieldW, Height = 220, Margin = new Padding(3, 25, 3, 0) };
            var flowOpcje = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(10) };

            // Zmniejszamy wysokość całego wiersza, skoro elementy będą blisko
            int rowH = 50;

            // Wiersz 1: SN
            var pnlRowSN = new FlowLayoutPanel { Width = 700, Height = rowH, FlowDirection = FlowDirection.TopDown, Padding = new Padding(0) }; // Padding 0
            chkIncSN = new CheckBox
            {
                Text = "Numer Seryjny:",
                Width = 200,
                Height = 22, // <--- KLUCZOWE: Wymuszamy małą wysokość (tylko na tekst)
                Checked = true,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(3, 2, 3, 0) // Dolny margines 0
            };
            txtEdytowalnySN = new TextBox
            {
                Width = 300,
                Margin = new Padding(3, 0, 3, 3) // Górny margines 0 (przykleja do góry)
            };
            pnlRowSN.Controls.Add(chkIncSN);
            pnlRowSN.Controls.Add(txtEdytowalnySN);
            flowOpcje.Controls.Add(pnlRowSN);

            // Wiersz 2: Faktura
            var pnlRowFV = new FlowLayoutPanel { Width = 700, Height = rowH, FlowDirection = FlowDirection.TopDown, Padding = new Padding(0) };
            chkIncFV = new CheckBox
            {
                Text = "Faktura:",
                Width = 200,
                Height = 22, // <--- KLUCZOWE
                Checked = true,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(3, 2, 3, 0)
            };
            txtEdytowalnaFaktura = new TextBox
            {
                Width = 300,
                Margin = new Padding(3, 0, 3, 3)
            };
            pnlRowFV.Controls.Add(chkIncFV);
            pnlRowFV.Controls.Add(txtEdytowalnaFaktura);
            flowOpcje.Controls.Add(pnlRowFV);

            // Wiersz 3: Data
            var pnlRowData = new FlowLayoutPanel { Width = 700, Height = rowH, FlowDirection = FlowDirection.TopDown, Padding = new Padding(0) };
            chkIncData = new CheckBox
            {
                Text = "Data Zakupu:",
                Width = 200,
                Height = 22, // <--- KLUCZOWE
                Checked = true,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(3, 2, 3, 0)
            };
            dtpEdytowalnaData = new DateTimePicker
            {
                Width = 150,
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(0, 0, 3, 3)
            };
            pnlRowData.Controls.Add(chkIncData);
            pnlRowData.Controls.Add(dtpEdytowalnaData);
            flowOpcje.Controls.Add(pnlRowData);

            grpOpcje.Controls.Add(flowOpcje);
            flowDane.Controls.Add(grpOpcje);

            // Uwagi
            flowDane.Controls.Add(new Label { Text = "Dodatkowe uwagi (Tylko w treści maila):", AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Margin = new Padding(3, 20, 3, 0) });
            txtEmailNotes = new TextBox { Width = fieldW, Height = 80, Multiline = true };
            flowDane.Controls.Add(txtEmailNotes);

            pageDane.Controls.Add(flowDane);

            // --- ZAKŁADKA 2 ---
            pageSpecyficzne = new TabPage("3. Formularz Producenta");
            pageSpecyficzne.BackColor = Color.White;
            pnlSpecyficzneContainer = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };
            pageSpecyficzne.Controls.Add(pnlSpecyficzneContainer);

            // --- ZAKŁADKA 3 ---
            pageZalaczniki = new TabPage("4. Załączniki");
            listFiles = new ListView { Dock = DockStyle.Fill, View = View.Details, CheckBoxes = true, FullRowSelect = true, GridLines = true, Font = new Font("Segoe UI", 10F) };
            listFiles.Columns.Add("Nazwa pliku", 700);
            pageZalaczniki.Controls.Add(listFiles);

            tabInput.Controls.Add(pageDane);
            tabInput.Controls.Add(pageZalaczniki);
            tabInput.Controls.Add(pageSpecyficzne);

            tlpLeft.Controls.Add(tabInput, 0, 1);


            // ================= PRAWY PANEL (PODGLĄD) =================
            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            splitMain.Panel2.Controls.Add(rightPanel);

            // 1. Toolbar
            var pnlPreviewToolbar = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = Color.WhiteSmoke, Margin = new Padding(0, 0, 0, 10) };
            btnShowMailPreview = new Button { Text = "WRÓĆ DO PODGLĄDU MAILA", Dock = DockStyle.Right, Width = 220, BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Visible = false, Cursor = Cursors.Hand };
            btnShowMailPreview.FlatAppearance.BorderSize = 0;
            lblPreviewTitle = new Label { Text = "Podgląd: Wiadomość E-mail (HTML)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Padding = new Padding(10, 0, 0, 0) };

            pnlPreviewToolbar.Controls.Add(lblPreviewTitle);
            pnlPreviewToolbar.Controls.Add(btnShowMailPreview);
            rightPanel.Controls.Add(pnlPreviewToolbar);

            // 2. Kontener WebView
            pnlWebViewContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(0) };

            picPreview = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, Visible = false };
            pnlWebViewContainer.Controls.Add(picPreview);

            rightPanel.Controls.Add(pnlWebViewContainer);
            pnlWebViewContainer.BringToFront();
            pnlPreviewToolbar.SendToBack();


            // --- 3. STOPKA ---
            var pnlFooter = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20, 15, 20, 15) };
            pnlFooter.Paint += (s, e) => { e.Graphics.DrawLine(Pens.LightGray, 0, 0, pnlFooter.Width, 0); };

            btnSend = new Button { Text = "WYŚLIJ ZGŁOSZENIE", Dock = DockStyle.Right, Width = 250, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSend.FlatAppearance.BorderSize = 0;

            btnCancel = new Button { Text = "Anuluj", Dock = DockStyle.Left, Width = 120, BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            pnlFooter.Controls.Add(btnSend);
            pnlFooter.Controls.Add(btnCancel);
            mainTable.Controls.Add(pnlFooter, 0, 2);
        }
    }
}