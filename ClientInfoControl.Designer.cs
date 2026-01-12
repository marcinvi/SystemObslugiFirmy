namespace Reklamacje_Dane
{
    partial class ClientInfoControl
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
            this.pnlDisplayContent = new System.Windows.Forms.Panel();

            // Display Labels
            this.lblTytul = new System.Windows.Forms.Label();
            this.lblImieNazwisko = new System.Windows.Forms.Label();
            this.lblAdres1 = new System.Windows.Forms.Label();
            this.lblAdres2 = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblTelefon = new System.Windows.Forms.Label();

            // Edit Controls
            this.txtNazwaFirmy = new System.Windows.Forms.TextBox();
            this.txtImieNazwisko = new System.Windows.Forms.TextBox();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.txtTelefon = new System.Windows.Forms.TextBox();
            this.txtUlicaNr = new System.Windows.Forms.TextBox();
            this.txtKodPocztowy = new System.Windows.Forms.TextBox();
            this.txtMiejscowosc = new System.Windows.Forms.TextBox();

            // Przyciski
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();

            // Menu Kontekstowe
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.edytujDaneKlientaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zmieńKlientaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.skopiujImięNazwiskoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujNazwęFirmyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujNIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujAdresMailowyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujNrTelefonuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujUlicęToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujKodPocztowyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skopiujMiejscowośćToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            this.panelDisplay.SuspendLayout();
            this.pnlDisplayContent.SuspendLayout();
            this.panelEdit.SuspendLayout();
            this.tlpEditLayout.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();

            // 
            // ClientInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelEdit);
            this.Controls.Add(this.panelDisplay);
            this.Name = "ClientInfoControl";
            this.Size = new System.Drawing.Size(600, 220);

            // =================================================================
            // PANEL DISPLAY (WIDOK)
            // =================================================================
            this.panelDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDisplay.ContextMenuStrip = this.contextMenuStrip1;
            this.panelDisplay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelDisplay.Controls.Add(this.pnlDisplayContent);
            this.panelDisplay.Controls.Add(this.lblTytul);
            this.panelDisplay.Padding = new System.Windows.Forms.Padding(10);

            this.lblTytul.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTytul.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTytul.ForeColor = System.Drawing.Color.Gray;
            this.lblTytul.Height = 25;
            this.lblTytul.Text = "DANE KLIENTA";

            this.pnlDisplayContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDisplayContent.Controls.Add(this.lblAdres2);
            this.pnlDisplayContent.Controls.Add(this.lblAdres1);
            this.pnlDisplayContent.Controls.Add(this.lblTelefon);
            this.pnlDisplayContent.Controls.Add(this.lblEmail);
            this.pnlDisplayContent.Controls.Add(this.lblImieNazwisko);

            this.lblImieNazwisko.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblImieNazwisko.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblImieNazwisko.ForeColor = System.Drawing.Color.FromArgb(25, 118, 210);
            this.lblImieNazwisko.Height = 50;
            this.lblImieNazwisko.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblImieNazwisko.Text = "Imię Nazwisko | Firma";

            this.lblEmail.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblEmail.Height = 25;
            this.lblEmail.Text = "📧 email@przyklad.com";
            this.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblTelefon.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTelefon.Height = 25;
            this.lblTelefon.Text = "📞 123 456 789";
            this.lblTelefon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTelefon.ForeColor = System.Drawing.Color.ForestGreen;

            this.lblAdres1.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAdres1.Height = 25;
            this.lblAdres1.Text = "🏠 Ulica 123";
            this.lblAdres1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAdres1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);

            this.lblAdres2.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAdres2.Height = 25;
            this.lblAdres2.Text = "00-000 Miasto";
            this.lblAdres2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // =================================================================
            // PANEL EDIT (EDYCJA)
            // =================================================================
            this.panelEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEdit.Visible = false;
            this.panelEdit.Padding = new System.Windows.Forms.Padding(5);
            this.panelEdit.AutoScroll = true; // Zabezpieczenie przed ucinaniem

            // WAŻNE: Kolejność dodawania ma znaczenie dla Docking!
            // Najpierw Buttons (Bottom), potem TLP (Fill)
            this.panelEdit.Controls.Add(this.tlpEditLayout);
            this.panelEdit.Controls.Add(this.pnlButtons);

            // 
            // pnlButtons (Pasek przycisków na dole)
            // 
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Height = 40;
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

            // 
            // tlpEditLayout (Siatka pól)
            // 
            this.tlpEditLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpEditLayout.ColumnCount = 2;
            this.tlpEditLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEditLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));

            // Ustawienia wierszy: AutoSize sprawia, że są ciasno upakowane
            this.tlpEditLayout.RowCount = 6;
            for (int i = 0; i < 6; i++) this.tlpEditLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            // Rząd 1
            this.tlpEditLayout.Controls.Add(CreateLabel("Imię i Nazwisko:"), 0, 0);
            this.tlpEditLayout.Controls.Add(CreateLabel("Nazwa Firmy:"), 1, 0);
            this.tlpEditLayout.Controls.Add(this.txtImieNazwisko, 0, 1);
            this.tlpEditLayout.Controls.Add(this.txtNazwaFirmy, 1, 1);

            // Rząd 2
            this.tlpEditLayout.Controls.Add(CreateLabel("E-mail:"), 0, 2);
            this.tlpEditLayout.Controls.Add(CreateLabel("Telefon:"), 1, 2);
            this.tlpEditLayout.Controls.Add(this.txtMail, 0, 3);
            this.tlpEditLayout.Controls.Add(this.txtTelefon, 1, 3);

            // Rząd 3
            this.tlpEditLayout.Controls.Add(CreateLabel("Ulica:"), 0, 4);
            this.tlpEditLayout.Controls.Add(CreateLabel("Kod / Miasto:"), 1, 4);
            this.tlpEditLayout.Controls.Add(this.txtUlicaNr, 0, 5);

            // Panel dla Kodu i Miasta
            System.Windows.Forms.FlowLayoutPanel flpCity = new System.Windows.Forms.FlowLayoutPanel();
            flpCity.Dock = System.Windows.Forms.DockStyle.Fill;
            flpCity.Margin = new System.Windows.Forms.Padding(0);
            flpCity.AutoSize = true; // Ważne dla AutoScroll
            this.txtKodPocztowy.Width = 70;
            this.txtMiejscowosc.Width = 180;
            flpCity.Controls.Add(this.txtKodPocztowy);
            flpCity.Controls.Add(this.txtMiejscowosc);
            this.tlpEditLayout.Controls.Add(flpCity, 1, 5);

            // TextBoxy
            this.txtImieNazwisko.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtNazwaFirmy.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtMail.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtTelefon.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtUlicaNr.Dock = System.Windows.Forms.DockStyle.Top;

            // =================================================================
            // MENU
            // =================================================================
            this.edytujDaneKlientaToolStripMenuItem.Text = "✏️ Edytuj dane klienta";
            this.edytujDaneKlientaToolStripMenuItem.Click += new System.EventHandler(this.edytujDaneKlientaToolStripMenuItem_Click);

            this.zmieńKlientaToolStripMenuItem.Text = "🔄 Zmień klienta (przypisz innego)";
            this.zmieńKlientaToolStripMenuItem.Click += new System.EventHandler(this.zmieńKlientaToolStripMenuItem_Click);

            this.skopiujImięNazwiskoToolStripMenuItem.Text = "Skopiuj: Imię i Nazwisko";
            this.skopiujImięNazwiskoToolStripMenuItem.Click += new System.EventHandler(this.skopiujImięNazwiskoToolStripMenuItem_Click);

            this.skopiujNazwęFirmyToolStripMenuItem.Text = "Skopiuj: Nazwę Firmy";
            this.skopiujNazwęFirmyToolStripMenuItem.Click += new System.EventHandler(this.skopiujNazwęFirmyToolStripMenuItem_Click);

            this.skopiujNIPToolStripMenuItem.Text = "Skopiuj: NIP";
            this.skopiujNIPToolStripMenuItem.Click += new System.EventHandler(this.skopiujNIPToolStripMenuItem_Click);

            this.skopiujAdresMailowyToolStripMenuItem.Text = "Skopiuj: E-mail";
            this.skopiujAdresMailowyToolStripMenuItem.Click += new System.EventHandler(this.skopiujAdresMailowyToolStripMenuItem_Click);

            this.skopiujNrTelefonuToolStripMenuItem.Text = "Skopiuj: Telefon";
            this.skopiujNrTelefonuToolStripMenuItem.Click += new System.EventHandler(this.skopiujNrTelefonuToolStripMenuItem_Click);

            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.edytujDaneKlientaToolStripMenuItem,
                this.zmieńKlientaToolStripMenuItem,
                this.toolStripSeparator1,
                this.skopiujImięNazwiskoToolStripMenuItem,
                this.skopiujNazwęFirmyToolStripMenuItem,
                this.skopiujNIPToolStripMenuItem,
                this.skopiujAdresMailowyToolStripMenuItem,
                this.skopiujNrTelefonuToolStripMenuItem
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

        // Helper do etykiet - ZMNIEJSZONY MARGINES GÓRNY (z 10 na 2)
        private System.Windows.Forms.Label CreateLabel(string text)
        {
            return new System.Windows.Forms.Label()
            {
                Text = text,
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 8F),
                ForeColor = System.Drawing.Color.Gray,
                Margin = new System.Windows.Forms.Padding(0, 2, 0, 0) // <--- TU BYŁ PROBLEM (było 10)
            };
        }

        #endregion

        // Deklaracje
        private System.Windows.Forms.Panel panelDisplay;
        private System.Windows.Forms.Panel pnlDisplayContent;
        private System.Windows.Forms.Label lblTytul;
        private System.Windows.Forms.Label lblImieNazwisko;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblTelefon;
        private System.Windows.Forms.Label lblAdres1;
        private System.Windows.Forms.Label lblAdres2;

        private System.Windows.Forms.Panel panelEdit;
        private System.Windows.Forms.TableLayoutPanel tlpEditLayout;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Button btnZapisz;

        private System.Windows.Forms.TextBox txtImieNazwisko;
        private System.Windows.Forms.TextBox txtNazwaFirmy;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.TextBox txtTelefon;
        private System.Windows.Forms.TextBox txtUlicaNr;
        private System.Windows.Forms.TextBox txtKodPocztowy;
        private System.Windows.Forms.TextBox txtMiejscowosc;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblImieLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem edytujDaneKlientaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zmieńKlientaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem skopiujImięNazwiskoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujNazwęFirmyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujNIPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujAdresMailowyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujNrTelefonuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujUlicęToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujKodPocztowyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skopiujMiejscowośćToolStripMenuItem;
    }
}