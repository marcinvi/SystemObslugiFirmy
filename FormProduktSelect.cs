using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public class FormProduktSelect : Form
    {
        public int WybranyProduktId { get; private set; }
        public string WybranaNazwa { get; private set; }

        private TextBox txtSearch;
        private DataGridView dgvProdukty;
        private Button btnWybierz;
        private Button btnAnuluj;

        public FormProduktSelect()
        {
            InitializeComponent();
            LoadProdukty(""); // Ładuj wszystko na start (limit 100)
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponent()
        {
            this.Text = "Wyszukaj Produkt";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10F);

            // Panel Górny (Szukaj)
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10), BackColor = Color.WhiteSmoke };
            Label lblSearch = new Label { Text = "Szukaj (Nazwa, Kod, Producent):", Dock = DockStyle.Top, Height = 20, ForeColor = Color.Gray, Font = new Font("Segoe UI", 9F) };

            txtSearch = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 12F) };
            txtSearch.TextChanged += (s, e) => LoadProdukty(txtSearch.Text);
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down) { dgvProdukty.Focus(); } };

            pnlTop.Controls.Add(txtSearch);
            pnlTop.Controls.Add(lblSearch);

            // Panel Dolny (Przyciski)
            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10), BackColor = Color.WhiteSmoke };

            btnAnuluj = new Button { Text = "Anuluj", Dock = DockStyle.Right, Width = 100, FlatStyle = FlatStyle.Flat, BackColor = Color.LightGray };
            btnAnuluj.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            btnWybierz = new Button { Text = "Wybierz", Dock = DockStyle.Right, Width = 120, FlatStyle = FlatStyle.Flat, BackColor = Color.SteelBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            btnWybierz.Click += BtnWybierz_Click;

            pnlBottom.Controls.Add(btnWybierz);
            pnlBottom.Controls.Add(btnAnuluj); // Kolejność dodawania ma znaczenie przy Dock

            // Tabela
            dgvProdukty = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvProdukty.DoubleClick += BtnWybierz_Click;
            dgvProdukty.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BtnWybierz_Click(s, e); } };

            this.Controls.Add(dgvProdukty);
            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlBottom);
        }

        private async void LoadProdukty(string search)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string sql = @"
                        SELECT Id, NazwaKrotka AS 'Nazwa', KodEnova AS 'Kod Enova', 
                               KodProducenta AS 'Kod Prod.', Producent, Kategoria 
                        FROM Produkty 
                        WHERE NazwaSystemowa LIKE @q 
                           OR NazwaKrotka LIKE @q 
                           OR KodEnova LIKE @q 
                           OR KodProducenta LIKE @q 
                           OR Producent LIKE @q
                        ORDER BY NazwaKrotka LIMIT 100";

                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@q", $"%{search}%");

                        DataTable dt = new DataTable();
                        using (var reader = await cmd.ExecuteReaderAsync()) dt.Load(reader);

                        dgvProdukty.DataSource = dt;
                        if (dgvProdukty.Columns["Id"] != null) dgvProdukty.Columns["Id"].Visible = false;
                    }
                }
            }
            catch { }
        }

        private void BtnWybierz_Click(object sender, EventArgs e)
        {
            if (dgvProdukty.SelectedRows.Count > 0)
            {
                WybranyProduktId = Convert.ToInt32(dgvProdukty.SelectedRows[0].Cells["Id"].Value);
                WybranaNazwa = dgvProdukty.SelectedRows[0].Cells["Nazwa"].Value.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                // Włącz sprawdzanie pisowni dla wszystkich kontrolek typu TextBox i RichTextBox
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        // Dla zwykłych TextBoxów - bez podkreślania (bo nie obsługują kolorów)
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
            }
        }

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;

                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control))
                    {
                        yield return child;
                    }
                }
            }
        }
}
}