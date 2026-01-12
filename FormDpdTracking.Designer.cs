using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class FormDpdTracking
    {
        private System.ComponentModel.IContainer components = null;

        // --- GŁÓWNE KONTROLKI ---
        public TabControl tabMain;
        public Button btnCheckStatus;
        public Label statusLabelInfo;

        // --- ZAKŁADKA 1: W DRODZE ---
        public TabPage tabWDrodze;
        public DataGridView dgvPrzesylki; // Lista aktywnych
        public DataGridView dgvHistoriaAktywne; // Historia klikniętej
        public Label lblHistoriaHeader;

        // --- ZAKŁADKA 2: PODSUMOWANIE DNIA ---
        public TabPage tabDzisiaj;
        public DataGridView dgvDzisiajDostarczone;
        public DataGridView dgvDzisiajProblemy;

        // --- ZAKŁADKA 3: ARCHIWUM ---
        public TabPage tabArchiwum;
        public DateTimePicker dtpOd;
        public DateTimePicker dtpDo;
        public ComboBox cmbStatus;
        public Button btnSzukaj;
        public DataGridView dgvArchiwum;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Text = "Centrum Monitoringu DPD";
            this.WindowState = FormWindowState.Maximized;
            this.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.BackColor = Color.White;

            // 1. GŁÓWNY UKŁAD (Tabela: Wiersz 1 = Menu, Wiersz 2 = Zakładki)
            var mainTable = new TableLayoutPanel();
            mainTable.Dock = DockStyle.Fill;
            mainTable.RowCount = 2;
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Pasek górny
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Reszta
            this.Controls.Add(mainTable);

            // 2. PANEL GÓRNY (Wiersz 1)
            var topPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke, Padding = new Padding(5) };

            btnCheckStatus = new Button
            {
                Text = "🔄 Odśwież Statusy (API)",
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.Crimson,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnCheckStatus.FlatAppearance.BorderSize = 0;

            statusLabelInfo = new Label
            {
                Dock = DockStyle.Right,
                AutoSize = false,
                Width = 500,
                TextAlign = ContentAlignment.MiddleRight,
                Text = "Gotowy.",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.Gray
            };

            topPanel.Controls.Add(statusLabelInfo);
            topPanel.Controls.Add(btnCheckStatus);
            mainTable.Controls.Add(topPanel, 0, 0);

            // 3. TAB CONTROL (Wiersz 2)
            tabMain = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };

            // --- TAB 1: W DRODZE ---
            tabWDrodze = new TabPage("🚚 W Drodze / Aktywne");
            tabWDrodze.Padding = new Padding(5);

            var splitAktywne = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 400 };

            dgvPrzesylki = CreateGrid();
            var lblH1 = new Label { Text = "LISTA PRZESYŁEK W DRODZE", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.DimGray, TextAlign = ContentAlignment.BottomLeft };

            dgvHistoriaAktywne = CreateGrid();
            dgvHistoriaAktywne.ColumnHeadersDefaultCellStyle.BackColor = Color.AliceBlue; // Inny kolor dla historii
            lblHistoriaHeader = new Label { Text = "📋 SZCZEGÓŁY STATUSÓW (Wybierz paczkę powyżej)", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SteelBlue, TextAlign = ContentAlignment.BottomLeft };

            splitAktywne.Panel1.Controls.Add(dgvPrzesylki);
            splitAktywne.Panel1.Controls.Add(lblH1);
            splitAktywne.Panel2.Controls.Add(dgvHistoriaAktywne);
            splitAktywne.Panel2.Controls.Add(lblHistoriaHeader);

            tabWDrodze.Controls.Add(splitAktywne);
            tabMain.TabPages.Add(tabWDrodze);

            // --- TAB 2: PODSUMOWANIE DNIA ---
            tabDzisiaj = new TabPage("📅 Podsumowanie Dnia (Dzisiaj)");
            tabDzisiaj.Padding = new Padding(5);

            var splitDzisiaj = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 350 };

            dgvDzisiajDostarczone = CreateGrid();
            var lblH2 = new Label { Text = "✅ DOSTARCZONE DZISIAJ", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.Green, TextAlign = ContentAlignment.BottomLeft };

            dgvDzisiajProblemy = CreateGrid();
            var lblH3 = new Label { Text = "⚠️ PROBLEMY / NIEDOSTARCZONE DZISIAJ", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.Red, TextAlign = ContentAlignment.BottomLeft };

            splitDzisiaj.Panel1.Controls.Add(dgvDzisiajDostarczone);
            splitDzisiaj.Panel1.Controls.Add(lblH2);
            splitDzisiaj.Panel2.Controls.Add(dgvDzisiajProblemy);
            splitDzisiaj.Panel2.Controls.Add(lblH3);

            tabDzisiaj.Controls.Add(splitDzisiaj);
            tabMain.TabPages.Add(tabDzisiaj);

            // --- TAB 3: ARCHIWUM ---
            tabArchiwum = new TabPage("🗄️ Archiwum / Szukaj");
            tabArchiwum.Padding = new Padding(5);

            var panelSearch = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.WhiteSmoke };

            panelSearch.Controls.Add(new Label { Text = "Od:", Location = new Point(10, 15), AutoSize = true });
            dtpOd = new DateTimePicker { Location = new Point(40, 12), Format = DateTimePickerFormat.Short, Width = 100 };
            dtpOd.Value = DateTime.Now.AddDays(-7);

            panelSearch.Controls.Add(new Label { Text = "Do:", Location = new Point(150, 15), AutoSize = true });
            dtpDo = new DateTimePicker { Location = new Point(180, 12), Format = DateTimePickerFormat.Short, Width = 100 };

            panelSearch.Controls.Add(new Label { Text = "Status:", Location = new Point(300, 15), AutoSize = true });
            cmbStatus = new ComboBox { Location = new Point(350, 12), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new object[] { "Wszystkie", "Dostarczona", "W Doręczeniu", "Problem/Zwrot" });
            cmbStatus.SelectedIndex = 0;

            btnSzukaj = new Button { Text = "🔍 Szukaj", Location = new Point(520, 10), Width = 100, Height = 30, BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            panelSearch.Controls.Add(dtpOd); panelSearch.Controls.Add(dtpDo); panelSearch.Controls.Add(cmbStatus); panelSearch.Controls.Add(btnSzukaj);

            dgvArchiwum = CreateGrid();

            tabArchiwum.Controls.Add(dgvArchiwum);
            tabArchiwum.Controls.Add(panelSearch);
            tabMain.TabPages.Add(tabArchiwum);

            mainTable.Controls.Add(tabMain, 0, 1);
        }

        private DataGridView CreateGrid()
        {
            var dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToResizeRows = false;
            dgv.Font = new Font("Segoe UI", 10F);
            dgv.ColumnHeadersHeight = 40;
            dgv.RowTemplate.Height = 35;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);

            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(227, 242, 253);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);

            return dgv;
        }
    }
}