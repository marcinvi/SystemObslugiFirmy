using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class DelegacjeControl
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header;
        private Label lblTitle;
        private Button btnRefresh;

        private SplitContainer split;
        private DataGridView dgv;

        private GroupBox grpEdit;
        private TextBox txtId;
        private Label lblU;
        private Label lblZ;
        private ComboBox cmbUzytkownik;
        private ComboBox cmbZastepca;
        private DateTimePicker dtpOd;
        private DateTimePicker dtpDo;
        private CheckBox chkAktywna;
        private Button btnAdd;
        private Button btnClear;
        private Button btnDelete;
        private CheckBox chkAktywneOnly;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            header = new Panel();
            lblTitle = new Label();
            btnRefresh = new Button();

            split = new SplitContainer();
            dgv = new DataGridView();

            grpEdit = new GroupBox();
            txtId = new TextBox();
            lblU = new Label();
            lblZ = new Label();
            cmbUzytkownik = new ComboBox();
            cmbZastepca = new ComboBox();
            dtpOd = new DateTimePicker();
            dtpDo = new DateTimePicker();
            chkAktywna = new CheckBox();
            btnAdd = new Button();
            btnClear = new Button();
            btnDelete = new Button();
            chkAktywneOnly = new CheckBox();

            // header
            header.Dock = DockStyle.Top;
            header.Height = 48;
            header.BackColor = Color.WhiteSmoke;
            header.Padding = new Padding(12);
            header.Controls.Add(lblTitle);
            header.Controls.Add(btnRefresh);

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitle.Text = "Delegacje (urlopy / zastępstwa)";

            btnRefresh.Text = "Odśwież";
            btnRefresh.Width = 100; btnRefresh.Height = 28;
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            header.Resize += (s, e) =>
            {
                btnRefresh.Left = header.Width - btnRefresh.Width - 12;
                btnRefresh.Top = (header.Height - btnRefresh.Height) / 2;
                lblTitle.Left = 12; lblTitle.Top = btnRefresh.Top + 3;
            };

            // split
            split.Dock = DockStyle.Fill;
            split.SplitterDistance = 550;

            // grid
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;

            // left panel
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
            chkAktywneOnly = new CheckBox { Text = "Tylko aktywne", AutoSize = true };
            chkAktywneOnly.Dock = DockStyle.Top; chkAktywneOnly.Padding = new Padding(0, 0, 0, 8);
            leftPanel.Controls.Add(dgv);
            leftPanel.Controls.Add(chkAktywneOnly);
            split.Panel1.Controls.Add(leftPanel);

            // right editor
            grpEdit.Text = "Edytor";
            grpEdit.Dock = DockStyle.Fill;

            int L = 16, T = 28, W = 250, R = 30, RW = 120, RH = 30, GAP = 32;

            var lblId = new Label { Left = L, Top = T + 3, AutoSize = true, Text = "Id:" };
            txtId.Left = L + 80; txtId.Top = T; txtId.Width = 120; txtId.ReadOnly = true;
            T += GAP;

            lblU.Left = L; lblU.Top = T + 3; lblU.Text = "Użytkownik:"; lblU.AutoSize = true;
            cmbUzytkownik.Left = L + 80; cmbUzytkownik.Top = T; cmbUzytkownik.Width = W; cmbUzytkownik.DropDownStyle = ComboBoxStyle.DropDownList;
            T += GAP;

            lblZ.Left = L; lblZ.Top = T + 3; lblZ.Text = "Zastępca:"; lblZ.AutoSize = true;
            cmbZastepca.Left = L + 80; cmbZastepca.Top = T; cmbZastepca.Width = W; cmbZastepca.DropDownStyle = ComboBoxStyle.DropDownList;
            T += GAP;

            var lblOd = new Label { Left = L, Top = T + 3, AutoSize = true, Text = "Data od:" };
            dtpOd.Left = L + 80; dtpOd.Top = T; dtpOd.Width = W; dtpOd.Format = DateTimePickerFormat.Short;
            T += GAP;

            var lblDo = new Label { Left = L, Top = T + 3, AutoSize = true, Text = "Data do:" };
            dtpDo.Left = L + 80; dtpDo.Top = T; dtpDo.Width = W; dtpDo.Format = DateTimePickerFormat.Short;
            T += GAP;

            chkAktywna.Left = L + 80; chkAktywna.Top = T; chkAktywna.Text = "Aktywna";
            T += GAP + 8;

            btnAdd.Left = L + 80; btnAdd.Top = T; btnAdd.Width = RW; btnAdd.Height = RH; btnAdd.Text = "Zapisz";
            btnClear.Left = btnAdd.Left + btnAdd.Width + 8; btnClear.Top = T; btnClear.Width = RW; btnClear.Height = RH; btnClear.Text = "Wyczyść";
            btnDelete.Left = btnClear.Left + btnClear.Width + 8; btnDelete.Top = T; btnDelete.Width = RW; btnDelete.Height = RH; btnDelete.Text = "Usuń";

            grpEdit.Controls.AddRange(new Control[] {
                lblId, txtId,
                lblU, cmbUzytkownik,
                lblZ, cmbZastepca,
                lblOd, dtpOd,
                lblDo, dtpDo,
                chkAktywna,
                btnAdd, btnClear, btnDelete
            });

            split.Panel2.Controls.Add(grpEdit);

            // root
            this.BackColor = Color.White;
            this.Controls.Add(split);
            this.Controls.Add(header);
            this.Name = "DelegacjeControl";
            this.Size = new Size(900, 600);
        }
    }
}
