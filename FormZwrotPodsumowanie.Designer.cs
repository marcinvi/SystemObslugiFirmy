// === FormZwrotPodsumowanie.Designer.cs (layout inspirowany FormHandlowiecSzczegoly; bez cardPanelDecyzja) ===
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class FormZwrotPodsumowanie
    {
        private System.ComponentModel.IContainer components = null;

        private Panel headerPanel;
        private Label lblHeader;

        private Panel contentPanel;

        private GroupBox cardPanelInfo;
        private Label lblNrZwrotuLbl;
        private TextBox txtNrZwrotu;
        private Label lblProduktLbl;
        private TextBox txtProdukt;
        private Label lblKtoPrzyjalLbl;
        private TextBox txtKtoPrzyjal;
        private Label lblKtoPodjalLbl;
        private TextBox txtKtoPodjal;
        private Label lblJakaDecyzjaLbl;
        private TextBox txtJakaDecyzja;
        private Label lblStatusLbl;
        private TextBox txtStatus;

        private GroupBox cardPanelUwagi;
        private Label lblUwagiMagazynuLbl;
        private TextBox txtUwagiMagazynu;
        private Label lblUwagiHandlowcaLbl;
        private TextBox txtUwagiHandlowca;

        private GroupBox cardPanelHistoria;
        private DataGridView dgvHistoria;
        private Panel panelAddHistory;
        private TextBox txtNowyWpis;
        private Button btnDodajWpis;
        private Label lblHistoriaEmpty;

        private Panel bottomPanel;
        private Button btnWroc;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.headerPanel = new Panel();
            this.lblHeader = new Label();

            this.contentPanel = new Panel();

            this.cardPanelInfo = new GroupBox();
            this.lblNrZwrotuLbl = new Label();
            this.txtNrZwrotu = new TextBox();
            this.lblProduktLbl = new Label();
            this.txtProdukt = new TextBox();
            this.lblKtoPrzyjalLbl = new Label();
            this.txtKtoPrzyjal = new TextBox();
            this.lblKtoPodjalLbl = new Label();
            this.txtKtoPodjal = new TextBox();
            this.lblJakaDecyzjaLbl = new Label();
            this.txtJakaDecyzja = new TextBox();
            this.lblStatusLbl = new Label();
            this.txtStatus = new TextBox();

            this.cardPanelUwagi = new GroupBox();
            this.lblUwagiMagazynuLbl = new Label();
            this.txtUwagiMagazynu = new TextBox();
            this.lblUwagiHandlowcaLbl = new Label();
            this.txtUwagiHandlowca = new TextBox();

            this.cardPanelHistoria = new GroupBox();
            this.dgvHistoria = new DataGridView();
            this.panelAddHistory = new Panel();
            this.txtNowyWpis = new TextBox();
            this.btnDodajWpis = new Button();
            this.lblHistoriaEmpty = new Label();

            this.bottomPanel = new Panel();
            this.btnWroc = new Button();

            // Form style
            this.Font = new Font("Segoe UI", 9F);
            this.BackColor = Color.White;

            // headerPanel
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 56;
            this.headerPanel.BackColor = Color.FromArgb(25, 118, 210); // #1976D2
            this.headerPanel.Padding = new Padding(16, 10, 16, 10);
            this.headerPanel.Controls.Add(this.lblHeader);

            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.Text = "Podsumowanie zwrotu";
            this.lblHeader.Location = new System.Drawing.Point(16, 14);

            // contentPanel
            this.contentPanel.Dock = DockStyle.Fill;
            this.contentPanel.BackColor = Color.FromArgb(240, 242, 245);
            this.contentPanel.Padding = new Padding(12);

            // cardPanelInfo
            this.cardPanelInfo.Text = "Informacje podstawowe";
            this.cardPanelInfo.Dock = DockStyle.Top;
            this.cardPanelInfo.Height = 165;
            this.cardPanelInfo.Font = new Font("Segoe UI", 9F);
            this.cardPanelInfo.BackColor = Color.White;

            int leftLabel = 12, leftField = 160, top = 24, rowH = 28, fieldW = 420;
            // Nr Zwrotu
            this.lblNrZwrotuLbl.Text = "Numer Zwrotu:";
            this.lblNrZwrotuLbl.AutoSize = true;
            this.lblNrZwrotuLbl.Location = new System.Drawing.Point(leftLabel, top);
            this.txtNrZwrotu.Location = new System.Drawing.Point(leftField, top - 4);
            this.txtNrZwrotu.Width = fieldW;
            this.txtNrZwrotu.ReadOnly = true;

            // Produkt
            top += rowH;
            this.lblProduktLbl.Text = "Produkt:";
            this.lblProduktLbl.AutoSize = true;
            this.lblProduktLbl.Location = new System.Drawing.Point(leftLabel, top);
            this.txtProdukt.Location = new System.Drawing.Point(leftField, top - 4);
            this.txtProdukt.Width = fieldW;
            this.txtProdukt.ReadOnly = true;

            // Kto przyjął
            top += rowH;
            this.lblKtoPrzyjalLbl.Text = "Kto przyjął fizycznie:";
            this.lblKtoPrzyjalLbl.AutoSize = true;
            this.lblKtoPrzyjalLbl.Location = new System.Drawing.Point(leftLabel, top);
            this.txtKtoPrzyjal.Location = new System.Drawing.Point(leftField, top - 4);
            this.txtKtoPrzyjal.Width = fieldW;
            this.txtKtoPrzyjal.ReadOnly = true;

            // Kto podjął
            top += rowH;
            this.lblKtoPodjalLbl.Text = "Kto podjął decyzję:";
            this.lblKtoPodjalLbl.AutoSize = true;
            this.lblKtoPodjalLbl.Location = new System.Drawing.Point(leftLabel, top);
            this.txtKtoPodjal.Location = new System.Drawing.Point(leftField, top - 4);
            this.txtKtoPodjal.Width = fieldW;
            this.txtKtoPodjal.ReadOnly = true;

            // Prawa kolumna
            int col2LeftLabel = leftField + fieldW + 30;
            int col2LeftField = col2LeftLabel + 120;
            int colTop = 24;

            this.lblJakaDecyzjaLbl.Text = "Jaka decyzja:";
            this.lblJakaDecyzjaLbl.AutoSize = true;
            this.lblJakaDecyzjaLbl.Location = new System.Drawing.Point(col2LeftLabel, colTop);
            this.txtJakaDecyzja.Location = new System.Drawing.Point(col2LeftField, colTop - 4);
            this.txtJakaDecyzja.Width = fieldW;
            this.txtJakaDecyzja.ReadOnly = true;

            colTop += rowH;
            this.lblStatusLbl.Text = "Jaki status:";
            this.lblStatusLbl.AutoSize = true;
            this.lblStatusLbl.Location = new System.Drawing.Point(col2LeftLabel, colTop);
            this.txtStatus.Location = new System.Drawing.Point(col2LeftField, colTop - 4);
            this.txtStatus.Width = fieldW;
            this.txtStatus.ReadOnly = true;

            this.cardPanelInfo.Controls.AddRange(new Control[] {
                this.lblNrZwrotuLbl, this.txtNrZwrotu,
                this.lblProduktLbl, this.txtProdukt,
                this.lblKtoPrzyjalLbl, this.txtKtoPrzyjal,
                this.lblKtoPodjalLbl, this.txtKtoPodjal,
                this.lblJakaDecyzjaLbl, this.txtJakaDecyzja,
                this.lblStatusLbl, this.txtStatus
            });

            // cardPanelUwagi
            this.cardPanelUwagi.Text = "Uwagi";
            this.cardPanelUwagi.Dock = DockStyle.Top;
            this.cardPanelUwagi.Height = 150;
            this.cardPanelUwagi.Font = new Font("Segoe UI", 9F);
            this.cardPanelUwagi.BackColor = Color.White;

            this.lblUwagiMagazynuLbl.Text = "Uwagi Magazynu:";
            this.lblUwagiMagazynuLbl.AutoSize = true;
            this.lblUwagiMagazynuLbl.Location = new System.Drawing.Point(12, 26);
            this.txtUwagiMagazynu.Location = new System.Drawing.Point(160, 22);
            this.txtUwagiMagazynu.Width = 540;
            this.txtUwagiMagazynu.Height = 40;
            this.txtUwagiMagazynu.Multiline = true;
            this.txtUwagiMagazynu.ReadOnly = true;
            this.txtUwagiMagazynu.ScrollBars = ScrollBars.Vertical;

            this.lblUwagiHandlowcaLbl.Text = "Uwagi Handlowca:";
            this.lblUwagiHandlowcaLbl.AutoSize = true;
            this.lblUwagiHandlowcaLbl.Location = new System.Drawing.Point(12, 78);
            this.txtUwagiHandlowca.Location = new System.Drawing.Point(160, 74);
            this.txtUwagiHandlowca.Width = 540;
            this.txtUwagiHandlowca.Height = 40;
            this.txtUwagiHandlowca.Multiline = true;
            this.txtUwagiHandlowca.ReadOnly = true;
            this.txtUwagiHandlowca.ScrollBars = ScrollBars.Vertical;

            this.cardPanelUwagi.Controls.AddRange(new Control[] {
                this.lblUwagiMagazynuLbl, this.txtUwagiMagazynu,
                this.lblUwagiHandlowcaLbl, this.txtUwagiHandlowca
            });

            // cardPanelHistoria
            this.cardPanelHistoria.Text = "Historia działań";
            this.cardPanelHistoria.Dock = DockStyle.Fill;
            this.cardPanelHistoria.Font = new Font("Segoe UI", 9F);
            this.cardPanelHistoria.BackColor = Color.White;

            this.dgvHistoria.Dock = DockStyle.Fill;

            this.panelAddHistory.Dock = DockStyle.Bottom;
            this.panelAddHistory.Height = 56;
            this.panelAddHistory.Padding = new Padding(8);

            this.txtNowyWpis.Dock = DockStyle.Fill;
            this.txtNowyWpis.Multiline = false;

            this.btnDodajWpis.Text = "Dodaj wpis";
            this.btnDodajWpis.Dock = DockStyle.Right;
            this.btnDodajWpis.Width = 120;

            this.lblHistoriaEmpty.Text = "Brak wpisów w historii.";
            this.lblHistoriaEmpty.AutoSize = true;
            this.lblHistoriaEmpty.ForeColor = Color.Gray;
            this.lblHistoriaEmpty.Dock = DockStyle.Top;
            this.lblHistoriaEmpty.Padding = new Padding(8, 4, 8, 4);
            this.lblHistoriaEmpty.Visible = false;

            this.panelAddHistory.Controls.Add(this.txtNowyWpis);
            this.panelAddHistory.Controls.Add(this.btnDodajWpis);

            this.cardPanelHistoria.Controls.Add(this.dgvHistoria);
            this.cardPanelHistoria.Controls.Add(this.panelAddHistory);
            this.cardPanelHistoria.Controls.Add(this.lblHistoriaEmpty);

            // bottomPanel
            this.bottomPanel.Dock = DockStyle.Bottom;
            this.bottomPanel.Height = 50;
            this.bottomPanel.Padding = new Padding(12);
            this.bottomPanel.BackColor = Color.WhiteSmoke;

            this.btnWroc.Text = "Wróć";
            this.btnWroc.Width = 110;
            this.btnWroc.Height = 30;
            this.btnWroc.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnWroc.Location = new System.Drawing.Point(0, 0);
            this.bottomPanel.Controls.Add(this.btnWroc);

            // contentPanel composition
            this.contentPanel.Controls.Add(this.cardPanelHistoria);
            this.contentPanel.Controls.Add(this.cardPanelUwagi);
            this.contentPanel.Controls.Add(this.cardPanelInfo);

            // Form
            this.AutoScaleMode = AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Name = "FormZwrotPodsumowanie";
            this.Text = "Podsumowanie zwrotu";

            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.headerPanel);

            // simple layout adjustments
            this.bottomPanel.Resize += (s, e) =>
            {
                this.btnWroc.Left = this.bottomPanel.Width - this.btnWroc.Width - 12;
                this.btnWroc.Top = (this.bottomPanel.Height - this.btnWroc.Height) / 2;
            };
        }
    }
}
