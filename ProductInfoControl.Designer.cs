namespace Reklamacje_Dane
{
    partial class ProductInfoControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Kontenery
            this.panelDisplay = new System.Windows.Forms.Panel();
            this.panelEdit = new System.Windows.Forms.Panel();
            this.tlpEditLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlButtons = new System.Windows.Forms.Panel();

            // Display Labels
            this.lblTytul = new System.Windows.Forms.Label();
            this.lblProdukt = new System.Windows.Forms.Label();
            this.lblKategoria = new System.Windows.Forms.Label();
            this.lblNrSeryjny = new System.Windows.Forms.Label();
            this.lblFaktura = new System.Windows.Forms.Label();
            this.lblSkad = new System.Windows.Forms.Label();
            this.pnlDisplayContent = new System.Windows.Forms.Panel(); // Kontener dla treści

            // Edit Controls
            this.label5 = new System.Windows.Forms.Label(); // Produkt
            this.txtProduktDisplay = new System.Windows.Forms.TextBox(); // Tylko do wyświetlania
            this.btnSzukajProduktu = new System.Windows.Forms.Button(); // Nowy Przycisk Lupy

            this.label2 = new System.Windows.Forms.Label(); // SN
            this.txtNrSeryjny = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label(); // Faktura
            this.txtNrFaktury = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label(); // Data
            this.txtDataZakupu = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();

            // Przyciski Akcji
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();

            // Menu Kontekstowe
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.edytujDaneZakupuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edytujProduktToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.skopiujKodEnovaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujMailaProducentaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.skopiujNrFakturyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujNrSeryjnyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.panelDisplay.SuspendLayout();
            this.pnlDisplayContent.SuspendLayout();
            this.panelEdit.SuspendLayout();
            this.tlpEditLayout.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();

            // 
            // ProductInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelEdit);
            this.Controls.Add(this.panelDisplay);
            this.Name = "ProductInfoControl";
            this.Size = new System.Drawing.Size(600, 220);

            // =================================================================
            // PANEL DISPLAY (WIDOK)
            // =================================================================
            this.panelDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDisplay.ContextMenuStrip = this.contextMenuStrip1;
            this.panelDisplay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelDisplay.Padding = new System.Windows.Forms.Padding(10);

            this.panelDisplay.Controls.Add(this.pnlDisplayContent);
            this.panelDisplay.Controls.Add(this.lblTytul);

            this.lblTytul.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTytul.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTytul.ForeColor = System.Drawing.Color.Gray;
            this.lblTytul.Height = 25;
            this.lblTytul.Text = "DANE PRODUKTU I ZAKUPU";

            this.pnlDisplayContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDisplayContent.Controls.Add(this.lblSkad);
            this.pnlDisplayContent.Controls.Add(this.lblFaktura);
            this.pnlDisplayContent.Controls.Add(this.lblNrSeryjny);
            this.pnlDisplayContent.Controls.Add(this.lblKategoria);
            this.pnlDisplayContent.Controls.Add(this.lblProdukt);

            // 1. Nazwa Produktu
            this.lblProdukt.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProdukt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblProdukt.ForeColor = System.Drawing.Color.FromArgb(25, 118, 210);
            this.lblProdukt.Height = 50;
            this.lblProdukt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblProdukt.Text = "Nazwa Produktu";

            // 2. Kategoria
            this.lblKategoria.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKategoria.Height = 25;
            this.lblKategoria.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKategoria.Text = "📦 Kategoria | Producent";

            // 3. SN
            this.lblNrSeryjny.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNrSeryjny.Height = 25;
            this.lblNrSeryjny.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblNrSeryjny.ForeColor = System.Drawing.Color.DimGray;
            this.lblNrSeryjny.Text = "🔢 SN: 123456789";

            // 4. Faktura
            this.lblFaktura.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFaktura.Height = 25;
            this.lblFaktura.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFaktura.Text = "🧾 FV/123/45 z dnia: 2024-01-01";

            // 5. Skąd
            this.lblSkad.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSkad.Height = 25;
            this.lblSkad.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSkad.ForeColor = System.Drawing.Color.ForestGreen;
            this.lblSkad.Text = "🛒 Źródło: Truck-Shop";


            // =================================================================
            // PANEL EDIT (EDYCJA)
            // =================================================================
            this.panelEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEdit.Visible = false;
            this.panelEdit.Padding = new System.Windows.Forms.Padding(5);
            this.panelEdit.AutoScroll = true;
            this.panelEdit.Controls.Add(this.tlpEditLayout);
            this.panelEdit.Controls.Add(this.pnlButtons);

            // Layout Tabeli
            this.tlpEditLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpEditLayout.ColumnCount = 2;
            this.tlpEditLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEditLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));

            // Wiersze (Auto = dopasuj do wysokości kontrolek)
            this.tlpEditLayout.RowCount = 6;
            for (int i = 0; i < 6; i++) this.tlpEditLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            // Rząd 1: Produkt (Z przyciskiem lupy)
            this.tlpEditLayout.Controls.Add(CreateLabel("Wybierz Produkt:"), 0, 0);

            // Panel dla TextBoxa i Przycisku Lupy
            System.Windows.Forms.Panel pnlProdSearch = new System.Windows.Forms.Panel();
            pnlProdSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlProdSearch.Height = 35;

            this.btnSzukajProduktu.Text = "🔍";
            this.btnSzukajProduktu.Width = 40;
            this.btnSzukajProduktu.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSzukajProduktu.BackColor = System.Drawing.Color.LightGray;
            this.btnSzukajProduktu.Click += new System.EventHandler(this.btnSzukajProduktu_Click);

            this.txtProduktDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProduktDisplay.ReadOnly = true; // Użytkownik nie wpisuje ręcznie, tylko wybiera lupą
            this.txtProduktDisplay.BackColor = System.Drawing.Color.WhiteSmoke;

            pnlProdSearch.Controls.Add(this.txtProduktDisplay);
            pnlProdSearch.Controls.Add(this.btnSzukajProduktu);

            this.tlpEditLayout.SetColumnSpan(pnlProdSearch, 2);
            this.tlpEditLayout.Controls.Add(pnlProdSearch, 0, 1);

            // Rząd 2
            this.tlpEditLayout.Controls.Add(CreateLabel("Numer Seryjny:"), 0, 2);
            this.tlpEditLayout.Controls.Add(CreateLabel("Numer Faktury:"), 1, 2);
            this.tlpEditLayout.Controls.Add(this.txtNrSeryjny, 0, 3);
            this.tlpEditLayout.Controls.Add(this.txtNrFaktury, 1, 3);

            // Rząd 3
            this.tlpEditLayout.Controls.Add(CreateLabel("Data Zakupu:"), 0, 4);

            // Panel Daty (Text + Picker)
            System.Windows.Forms.Panel pnlDate = new System.Windows.Forms.Panel();
            pnlDate.Dock = System.Windows.Forms.DockStyle.Top;
            pnlDate.Height = 30;

            this.dateTimePicker1.Width = 25;
            this.dateTimePicker1.Dock = System.Windows.Forms.DockStyle.Right;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);

            this.txtDataZakupu.Dock = System.Windows.Forms.DockStyle.Fill;

            pnlDate.Controls.Add(this.txtDataZakupu);
            pnlDate.Controls.Add(this.dateTimePicker1);

            this.tlpEditLayout.Controls.Add(pnlDate, 0, 5);

            // Konfiguracja kontrolek
            this.txtNrSeryjny.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtNrFaktury.Dock = System.Windows.Forms.DockStyle.Top;


            // Panel Przycisków (Dół)
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Height = 45;
            this.pnlButtons.Controls.Add(this.btnZapisz);
            this.pnlButtons.Controls.Add(this.btnAnuluj);

            this.btnZapisz.Text = "Zapisz";
            this.btnZapisz.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnZapisz.Width = 80;
            this.btnZapisz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);

            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAnuluj.Width = 80;
            this.btnAnuluj.BackColor = System.Drawing.Color.LightGray;
            this.btnAnuluj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnuluj.Click += new System.EventHandler(this.btnAnuluj_Click);


            // Menu Kontekstowe
            this.edytujDaneZakupuToolStripMenuItem.Text = "✏️ Edytuj dane zakupu";
            this.edytujDaneZakupuToolStripMenuItem.Click += new System.EventHandler(this.edytujDaneZakupuToolStripMenuItem_Click);

            this.edytujProduktToolStripMenuItem.Text = "📦 Edytuj produkt (w bazie)";
            this.edytujProduktToolStripMenuItem.Click += new System.EventHandler(this.edytujProduktToolStripMenuItem_Click);

            this.skopiujKodEnovaToolStripMenuItem.Text = "Skopiuj: Kod Enova";
            this.skopiujKodEnovaToolStripMenuItem.Click += new System.EventHandler(this.skopiujKodEnovaToolStripMenuItem_Click);

            this.skopiujMailaProducentaToolStripMenuItem.Text = "Skopiuj: E-mail Producenta";
            this.skopiujMailaProducentaToolStripMenuItem.Click += new System.EventHandler(this.skopiujMailaProducentaToolStripMenuItem_Click);

            this.skopiujNrFakturyToolStripMenuItem.Text = "Skopiuj: Nr Faktury";
            this.skopiujNrFakturyToolStripMenuItem.Click += new System.EventHandler(this.skopiujNrFakturyToolStripMenuItem_Click);

            this.skopiujNrSeryjnyToolStripMenuItem.Text = "Skopiuj: Nr Seryjny";
            this.skopiujNrSeryjnyToolStripMenuItem.Click += new System.EventHandler(this.skopiujNrSeryjnyToolStripMenuItem_Click);

            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.edytujDaneZakupuToolStripMenuItem,
                this.edytujProduktToolStripMenuItem,
                this.toolStripSeparator2,
                this.skopiujKodEnovaToolStripMenuItem,
                this.skopiujMailaProducentaToolStripMenuItem,
                this.toolStripSeparator3,
                this.skopiujNrFakturyToolStripMenuItem,
                this.skopiujNrSeryjnyToolStripMenuItem
            });
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);

            this.panelDisplay.ResumeLayout(false);
            this.pnlDisplayContent.ResumeLayout(false);
            this.panelEdit.ResumeLayout(false);
            this.tlpEditLayout.ResumeLayout(false);
            this.tlpEditLayout.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label CreateLabel(string text)
        {
            return new System.Windows.Forms.Label() { Text = text, AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 8F), ForeColor = System.Drawing.Color.Gray, Margin = new System.Windows.Forms.Padding(0, 2, 0, 0) };
        }

        #endregion

        // Kontrolki
        private System.Windows.Forms.Panel panelDisplay;
        private System.Windows.Forms.Panel pnlDisplayContent;
        private System.Windows.Forms.Label lblTytul;
        private System.Windows.Forms.Label lblProdukt;
        private System.Windows.Forms.Label lblKategoria;
        private System.Windows.Forms.Label lblNrSeryjny;
        private System.Windows.Forms.Label lblFaktura;
        private System.Windows.Forms.Label lblSkad;

        private System.Windows.Forms.Panel panelEdit;
        private System.Windows.Forms.TableLayoutPanel tlpEditLayout;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Button btnZapisz;

        private System.Windows.Forms.ComboBox comboProdukt; // (Nie używany już, ale zostawiam by kod kompilował, usunięty z layoutu)
        private System.Windows.Forms.TextBox txtProduktDisplay;
        private System.Windows.Forms.Button btnSzukajProduktu;

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDataZakupu;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNrFaktury;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNrSeryjny;

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem edytujDaneZakupuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edytujProduktToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem skopiujKodEnovaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujMailaProducentaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem skopiujNrFakturyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujNrSeryjnyToolStripMenuItem;
    }
}