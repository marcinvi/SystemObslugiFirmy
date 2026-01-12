using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class AllegroOpiekunowieControl
    {
        private System.ComponentModel.IContainer components = null;
        private Panel header;
        private Label lblTitle;
        private Button btnRefresh;
        private Button btnSaveAll;
        private DataGridView dgv;

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
            btnSaveAll = new Button();
            dgv = new DataGridView();

            // header
            header.Dock = DockStyle.Top;
            header.Height = 48;
            header.BackColor = Color.WhiteSmoke;
            header.Padding = new Padding(12);
            header.Controls.Add(lblTitle);
            header.Controls.Add(btnRefresh);
            header.Controls.Add(btnSaveAll);

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTitle.Text = "Opiekunowie kont Allegro";

            btnRefresh.Text = "Odśwież";
            btnRefresh.Width = 100; btnRefresh.Height = 28;
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnSaveAll.Text = "Zapisz zmiany";
            btnSaveAll.Width = 120; btnSaveAll.Height = 28;
            btnSaveAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // dynamiczne pozycje w headerze
            header.Resize += (s, e) =>
            {
                btnSaveAll.Left = header.Width - btnSaveAll.Width - 12;
                btnSaveAll.Top = (header.Height - btnSaveAll.Height) / 2;
                btnRefresh.Left = btnSaveAll.Left - btnRefresh.Width - 8;
                btnRefresh.Top = btnSaveAll.Top;
                lblTitle.Left = 12; lblTitle.Top = btnSaveAll.Top + 3;
            };

            // grid
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;

            // root
            this.BackColor = Color.White;
            this.Controls.Add(dgv);
            this.Controls.Add(header);
            this.Name = "AllegroOpiekunowieControl";
            this.Size = new Size(900, 600);
        }
    }
}
