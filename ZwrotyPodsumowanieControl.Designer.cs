// === ZwrotyPodsumowanieControl.Designer.cs (wizualnie podobny do HandlowiecControl: header + filtry + grid + stopka) ===
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class ZwrotyPodsumowanieControl
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelHeader;
        private Label lblTitle;
        private Button btnRefresh;

        private Panel panelFilters;
        private Label lblSearch;
        private TextBox txtSearch;

        private Label lblHandlowiec;
        private ComboBox cmbHandlowiec;

        private Label lblStatus;
        private ComboBox cmbStatus;

        private CheckBox chkDateFrom;
        private DateTimePicker dtpFrom;
        private CheckBox chkDateTo;
        private DateTimePicker dtpTo;

        private Button btnFilterWszystkie;
        private Button btnFilterDoDecyzji;
        private Button btnFilterZakonczone;

        private Button btnExportCsv;

        private DataGridView dgvReturns;

        private Panel panelFooter;
        private Label lblCount;
        private Label lblTotal;
        private Label lblDoDecyzji;
        private Label lblZakonczone;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.panelHeader = new Panel();
            this.lblTitle = new Label();
            this.btnRefresh = new Button();

            this.panelFilters = new Panel();
            this.lblSearch = new Label();
            this.txtSearch = new TextBox();

            this.lblHandlowiec = new Label();
            this.cmbHandlowiec = new ComboBox();

            this.lblStatus = new Label();
            this.cmbStatus = new ComboBox();

            this.chkDateFrom = new CheckBox();
            this.dtpFrom = new DateTimePicker();
            this.chkDateTo = new CheckBox();
            this.dtpTo = new DateTimePicker();

            this.btnFilterWszystkie = new Button();
            this.btnFilterDoDecyzji = new Button();
            this.btnFilterZakonczone = new Button();

            this.btnExportCsv = new Button();

            this.dgvReturns = new DataGridView();

            this.panelFooter = new Panel();
            this.lblCount = new Label();
            this.lblTotal = new Label();
            this.lblDoDecyzji = new Label();
            this.lblZakonczone = new Label();

            // Control style
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);

            // Header
            this.panelHeader.BackColor = Color.WhiteSmoke;
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Padding = new Padding(16, 10, 16, 10);
            this.panelHeader.Height = 56;

            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(16, 14);
            this.lblTitle.Text = "Zwroty — podsumowanie";

            this.btnRefresh.Text = "Odśwież";
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.Width = 100;
            this.btnRefresh.Height = 32;
            this.btnRefresh.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnRefresh.Location = new System.Drawing.Point(0, 12);

            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Controls.Add(this.btnRefresh);

            // Filters
            this.panelFilters.BackColor = Color.WhiteSmoke;
            this.panelFilters.Dock = DockStyle.Top;
            this.panelFilters.Padding = new Padding(16, 4, 16, 8);
            this.panelFilters.Height = 90;

            this.lblSearch.AutoSize = true; this.lblSearch.Text = "Szukaj:"; this.lblSearch.Location = new System.Drawing.Point(16, 12);
            this.txtSearch.Location = new System.Drawing.Point(70, 9); this.txtSearch.Width = 240; this.txtSearch.Font = new Font("Segoe UI", 9F);

            this.lblHandlowiec.AutoSize = true; this.lblHandlowiec.Text = "Handlowiec:"; this.lblHandlowiec.Location = new System.Drawing.Point(330, 12);
            this.cmbHandlowiec.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbHandlowiec.Location = new System.Drawing.Point(410, 9); this.cmbHandlowiec.Width = 200; this.cmbHandlowiec.Font = new Font("Segoe UI", 9F);

            this.lblStatus.AutoSize = true; this.lblStatus.Text = "Status:"; this.lblStatus.Location = new System.Drawing.Point(630, 12);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Location = new System.Drawing.Point(680, 9); this.cmbStatus.Width = 220; this.cmbStatus.Font = new Font("Segoe UI", 9F);

            this.chkDateFrom.AutoSize = true; this.chkDateFrom.Text = "Od:"; this.chkDateFrom.Location = new System.Drawing.Point(16, 44);
            this.dtpFrom.Format = DateTimePickerFormat.Short; this.dtpFrom.Location = new System.Drawing.Point(70, 42); this.dtpFrom.Width = 120;
            this.chkDateTo.AutoSize = true; this.chkDateTo.Text = "Do:"; this.chkDateTo.Location = new System.Drawing.Point(210, 44);
            this.dtpTo.Format = DateTimePickerFormat.Short; this.dtpTo.Location = new System.Drawing.Point(250, 42); this.dtpTo.Width = 120;

            this.btnFilterWszystkie.Text = "Wszystkie"; this.btnFilterWszystkie.Location = new System.Drawing.Point(410, 42); this.btnFilterWszystkie.Width = 100; this.btnFilterWszystkie.Height = 28; this.btnFilterWszystkie.FlatStyle = FlatStyle.Flat;
            this.btnFilterDoDecyzji.Text = "Do decyzji"; this.btnFilterDoDecyzji.Location = new System.Drawing.Point(520, 42); this.btnFilterDoDecyzji.Width = 100; this.btnFilterDoDecyzji.Height = 28; this.btnFilterDoDecyzji.FlatStyle = FlatStyle.Flat;
            this.btnFilterZakonczone.Text = "Zakończone"; this.btnFilterZakonczone.Location = new System.Drawing.Point(630, 42); this.btnFilterZakonczone.Width = 110; this.btnFilterZakonczone.Height = 28; this.btnFilterZakonczone.FlatStyle = FlatStyle.Flat;

            this.btnExportCsv.Text = "Eksport CSV"; this.btnExportCsv.Location = new System.Drawing.Point(760, 42); this.btnExportCsv.Width = 120; this.btnExportCsv.Height = 28; this.btnExportCsv.FlatStyle = FlatStyle.Flat;

            this.panelFilters.Controls.Add(this.lblSearch);
            this.panelFilters.Controls.Add(this.txtSearch);
            this.panelFilters.Controls.Add(this.lblHandlowiec);
            this.panelFilters.Controls.Add(this.cmbHandlowiec);
            this.panelFilters.Controls.Add(this.lblStatus);
            this.panelFilters.Controls.Add(this.cmbStatus);
            this.panelFilters.Controls.Add(this.chkDateFrom);
            this.panelFilters.Controls.Add(this.dtpFrom);
            this.panelFilters.Controls.Add(this.chkDateTo);
            this.panelFilters.Controls.Add(this.dtpTo);
            this.panelFilters.Controls.Add(this.btnFilterWszystkie);
            this.panelFilters.Controls.Add(this.btnFilterDoDecyzji);
            this.panelFilters.Controls.Add(this.btnFilterZakonczone);
            this.panelFilters.Controls.Add(this.btnExportCsv);

            // Grid
            this.dgvReturns.Dock = DockStyle.Fill;
            this.dgvReturns.BackgroundColor = Color.White;

            // Footer
            this.panelFooter.BackColor = Color.White;
            this.panelFooter.Dock = DockStyle.Bottom;
            this.panelFooter.Padding = new Padding(16, 8, 16, 8);
            this.panelFooter.Height = 40;

            this.lblCount.AutoSize = true; this.lblCount.Text = "Wyświetlono: 0"; this.lblCount.Location = new System.Drawing.Point(16, 12);
            this.lblTotal.AutoSize = true; this.lblTotal.Text = "Razem: 0"; this.lblTotal.Location = new System.Drawing.Point(220, 12);
            this.lblDoDecyzji.AutoSize = true; this.lblDoDecyzji.Text = "Do decyzji: 0"; this.lblDoDecyzji.Location = new System.Drawing.Point(360, 12);
            this.lblZakonczone.AutoSize = true; this.lblZakonczone.Text = "Zakończone: 0"; this.lblZakonczone.Location = new System.Drawing.Point(520, 12);

            this.panelFooter.Controls.Add(this.lblCount);
            this.panelFooter.Controls.Add(this.lblTotal);
            this.panelFooter.Controls.Add(this.lblDoDecyzji);
            this.panelFooter.Controls.Add(this.lblZakonczone);

            // Root
            this.Name = "ZwrotyPodsumowanieControl";
            this.Size = new System.Drawing.Size(1100, 700);

            this.Controls.Add(this.dgvReturns);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelFilters);
            this.Controls.Add(this.panelHeader);

            this.Load += new System.EventHandler(this.ZwrotyPodsumowanieControl_Load);
            this.panelHeader.Resize += (s, e) =>
            {
                this.btnRefresh.Left = this.panelHeader.Width - this.btnRefresh.Width - 16;
                this.btnRefresh.Top = (this.panelHeader.Height - this.btnRefresh.Height) / 2;
            };
        }
    }
}
