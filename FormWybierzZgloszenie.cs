using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public class FormWybierzZgloszenie : Form
    {
        private TextBox txtSzukaj;
        private DataGridView dgvLista;
        private DataTable _dtCache;
        public string WybranyNumerZgloszenia { get; private set; }

        public FormWybierzZgloszenie()
        {
            InitializeComponent_Modern();
            LoadData();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponent_Modern()
        {
            // Okno
            this.Text = "Wybierz Zgłoszenie";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // --- HEADER ---
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.FromArgb(45, 66, 91) };

            Label lblTitle = new Label
            {
                Text = "WYBIERZ ZGŁOSZENIE DOCELOWE",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 10)
            };

            // Ikona lupy
            Label lblIcon = new Label { Text = "🔍", ForeColor = Color.LightGray, Font = new Font("Segoe UI", 12), AutoSize = true, Location = new Point(20, 38) };

            // Pole wyszukiwania
            txtSzukaj = new TextBox
            {
                Location = new Point(50, 38),
                Width = 400,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White
            };
            txtSzukaj.TextChanged += TxtSzukaj_TextChanged;
            // Placeholder hack
            // txtSzukaj.PlaceholderText = "Wpisz numer, klienta, SN..."; 

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblIcon);
            pnlHeader.Controls.Add(txtSzukaj);

            // --- GRID ---
            dgvLista = new DataGridView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 35 }
            };

            // Style Grida
            dgvLista.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvLista.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvLista.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvLista.ColumnHeadersHeight = 40;
            dgvLista.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvLista.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 236, 255); // Jasny niebieski select
            dgvLista.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvLista.DefaultCellStyle.Padding = new Padding(5, 0, 0, 0);

            // Event wyboru
            dgvLista.DoubleClick += (s, e) => Wybierz();

            this.Controls.Add(dgvLista);
            this.Controls.Add(pnlHeader);
        }

        private void LoadData()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    string sql = @"
                        SELECT 
                            z.NrZgloszenia, 
                            z.DataZgloszenia,
                            z.NrSeryjny, 
                            IFNULL(k.NazwaFirmy, k.ImieNazwisko) as Klient,
                            p.NazwaSystemowa as Produkt,
                            z.StatusOgolny
                        FROM Zgloszenia z
                        LEFT JOIN klienci k ON z.KlientID = k.Id
                        LEFT JOIN Produkty p ON z.ProduktID = p.Id
                        ORDER BY z.Id DESC";

                    using (var cmd = new MySqlCommand(sql, con))
                    using (var da = new MySqlDataAdapter(cmd))
                    {
                        _dtCache = new DataTable();
                        da.Fill(_dtCache);
                    }
                }
                dgvLista.DataSource = _dtCache;
                FormatujGrid();
            }
            catch (Exception ex) { MessageBox.Show("Błąd bazy: " + ex.Message); }
        }

        private void FormatujGrid()
        {
            if (dgvLista.Columns.Contains("NrZgloszenia")) { dgvLista.Columns["NrZgloszenia"].HeaderText = "Nr Zgłoszenia"; dgvLista.Columns["NrZgloszenia"].Width = 120; }
            if (dgvLista.Columns.Contains("DataZgloszenia")) { dgvLista.Columns["DataZgloszenia"].HeaderText = "Data"; dgvLista.Columns["DataZgloszenia"].Width = 100; }
            if (dgvLista.Columns.Contains("NrSeryjny")) { dgvLista.Columns["NrSeryjny"].HeaderText = "S/N"; dgvLista.Columns["NrSeryjny"].Width = 120; }
            if (dgvLista.Columns.Contains("Klient")) { dgvLista.Columns["Klient"].HeaderText = "Klient"; dgvLista.Columns["Klient"].Width = 200; }
            if (dgvLista.Columns.Contains("StatusOgolny")) { dgvLista.Columns["StatusOgolny"].HeaderText = "Status"; dgvLista.Columns["StatusOgolny"].Width = 150; }
            if (dgvLista.Columns.Contains("Produkt")) { dgvLista.Columns["Produkt"].HeaderText = "Produkt"; dgvLista.Columns["Produkt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
        }

        private void TxtSzukaj_TextChanged(object sender, EventArgs e)
        {
            if (_dtCache == null) return;
            string fraza = txtSzukaj.Text.Trim().Replace("'", "''");

            if (string.IsNullOrEmpty(fraza))
            {
                _dtCache.DefaultView.RowFilter = "";
            }
            else
            {
                _dtCache.DefaultView.RowFilter = string.Format(
                    "NrZgloszenia LIKE '%{0}%' OR NrSeryjny LIKE '%{0}%' OR Klient LIKE '%{0}%' OR Produkt LIKE '%{0}%' OR StatusOgolny LIKE '%{0}%'",
                    fraza);
            }
        }

        private void Wybierz()
        {
            if (dgvLista.CurrentRow != null)
            {
                WybranyNumerZgloszenia = dgvLista.CurrentRow.Cells["NrZgloszenia"].Value.ToString();
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