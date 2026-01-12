using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    // To jest okno (Popup), które wyskakuje z listą zmian statusów
    public class FormPowiadomieniePrzesylka : Form
    {
        private DataGridView _grid;
        private Button _btnOk;
        private List<PrzesylkaAlert> _alerts;

        public FormPowiadomieniePrzesylka(List<PrzesylkaAlert> alerts)
        {
            _alerts = alerts;
            InitializeComponent();
            PopulateGrid();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponent()
        {
            this.Text = "🔔 Aktualizacja Przesyłek";
            this.Size = new Size(600, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true; // Zawsze na wierzchu
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Grid
            _grid = new DataGridView();
            _grid.Dock = DockStyle.Fill;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;
            _grid.RowHeadersVisible = false;
            _grid.BackgroundColor = Color.White;
            _grid.CellDoubleClick += _grid_CellDoubleClick;

            // Kolumny
            _grid.Columns.Add("NrListu", "List Przewozowy");
            _grid.Columns.Add("Zgloszenie", "Zgłoszenie");
            _grid.Columns.Add("Status", "Nowy Status");

            // Przycisk
            _btnOk = new Button();
            _btnOk.Text = "OK, Zrozumiałem";
            _btnOk.Dock = DockStyle.Bottom;
            _btnOk.Height = 45;
            _btnOk.BackColor = Color.FromArgb(21, 101, 192);
            _btnOk.ForeColor = Color.White;
            _btnOk.FlatStyle = FlatStyle.Flat;
            _btnOk.Click += (s, e) => this.Close();

            this.Controls.Add(_grid);
            this.Controls.Add(_btnOk);
        }

        private void PopulateGrid()
        {
            foreach (var alert in _alerts)
            {
                int idx = _grid.Rows.Add(alert.NumerListu, alert.NrZgloszenia, alert.NowyStatus);

                // Kolorowanie
                var row = _grid.Rows[idx];
                string s = alert.NowyStatus.ToUpper();

                if (s.Contains("PROBLEM") || s.Contains("ZWROT") || s.Contains("ZGUBIONA"))
                    row.DefaultCellStyle.ForeColor = Color.Red;
                else if (s.Contains("DORĘCZONA"))
                    row.DefaultCellStyle.ForeColor = Color.Green;
                else if (s.Contains("DORĘCZENIU"))
                    row.DefaultCellStyle.ForeColor = Color.Blue;
            }
        }

        private void _grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string nr = _grid.Rows[e.RowIndex].Cells["Zgloszenie"].Value?.ToString();
                if (!string.IsNullOrEmpty(nr))
                {
                    // Otwieramy zgłoszenie
                    new Form2(nr).Show();
                    this.Close();
                }
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