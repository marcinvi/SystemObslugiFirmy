using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormWybierzCzesc : Form
    {
        private readonly MagazynService _service;
        public DostepnaCzescView WybranaCzesc { get; private set; }

        // Cache do filtrowania
        private List<DostepnaCzescView> _cacheCzesci;

        public FormWybierzCzesc()
        {
            InitializeComponent();
            _service = new MagazynService();
            this.Load += FormWybierzCzesc_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormWybierzCzesc_Load(object sender, EventArgs e)
        {
            await ZaladujDane();
        }

        private async Task ZaladujDane()
        {
            try
            {
                _cacheCzesci = await _service.PobierzDostepneCzesciAsync();
                dgvLista.DataSource = _cacheCzesci;
                FormatujGrid();
                BudujFiltry();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania: " + ex.Message);
            }
        }

        // --- DYNAMICZNE FILTRY (Skopiowane i dopasowane z FormStanMagazynowy) ---

        private void BudujFiltry()
        {
            // Sprzątanie starych kontrolek
            while (pnlFiltry.Controls.Count > 0)
            {
                var c = pnlFiltry.Controls[0];
                pnlFiltry.Controls.RemoveAt(0);
                c.Dispose();
            }

            // Podpięcie synchronizacji szerokości
            dgvLista.ColumnWidthChanged -= PozycjonujFiltry;
            dgvLista.ColumnWidthChanged += PozycjonujFiltry;

            int left = dgvLista.RowHeadersVisible ? dgvLista.RowHeadersWidth : 0;

            foreach (DataGridViewColumn col in dgvLista.Columns)
            {
                if (!col.Visible) continue;

                var txt = new TextBox();
                txt.Tag = col.DataPropertyName; // Wiązanie z polem danych
                txt.Location = new Point(left, 5);
                txt.Width = col.Width - 2;
                txt.Height = 20;
                txt.TextChanged += FiltrujDane; // Obsługa pisania

                pnlFiltry.Controls.Add(txt);
                left += col.Width;
            }
        }

        private void PozycjonujFiltry(object sender, EventArgs e)
        {
            int left = dgvLista.RowHeadersVisible ? dgvLista.RowHeadersWidth : 0;
            foreach (Control ctrl in pnlFiltry.Controls)
            {
                if (ctrl is TextBox txt && txt.Tag is string colName)
                {
                    if (dgvLista.Columns.Contains(colName) && dgvLista.Columns[colName].Visible)
                    {
                        txt.Left = left;
                        txt.Width = dgvLista.Columns[colName].Width - 2;
                        left += dgvLista.Columns[colName].Width;
                    }
                }
            }
        }

        private void FiltrujDane(object sender, EventArgs e)
        {
            if (_cacheCzesci == null) return;

            var filtered = _cacheCzesci.Where(item =>
            {
                foreach (Control ctrl in pnlFiltry.Controls)
                {
                    if (ctrl is TextBox txt && !string.IsNullOrEmpty(txt.Text) && txt.Tag is string propName)
                    {
                        var prop = item.GetType().GetProperty(propName);
                        if (prop != null)
                        {
                            string val = prop.GetValue(item)?.ToString() ?? "";
                            if (!val.ToLower().Contains(txt.Text.ToLower())) return false;
                        }
                    }
                }
                return true;
            }).ToList();

            dgvLista.DataSource = filtered;
        }

        // --- FORMATOWANIE ---

        private void FormatujGrid()
        {
            if (dgvLista.Columns.Contains("Id")) dgvLista.Columns["Id"].Visible = false;
            if (dgvLista.Columns.Contains("SnDawcy")) dgvLista.Columns["SnDawcy"].Visible = false;

            if (dgvLista.Columns.Contains("NazwaCzesci"))
            {
                dgvLista.Columns["NazwaCzesci"].HeaderText = "Nazwa Części";
                dgvLista.Columns["NazwaCzesci"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (dgvLista.Columns.Contains("ModelDawcy")) dgvLista.Columns["ModelDawcy"].HeaderText = "Z modelu";
            if (dgvLista.Columns.Contains("ZgloszenieDawcy")) dgvLista.Columns["ZgloszenieDawcy"].HeaderText = "Nr Zgłoszenia";
            if (dgvLista.Columns.Contains("Lokalizacja")) dgvLista.Columns["Lokalizacja"].HeaderText = "Lokalizacja";
        }

        // --- WYBÓR ---

        private void btnWybierz_Click(object sender, EventArgs e) => WybierzZaznaczona();
        private void dgvLista_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => WybierzZaznaczona();

        private void WybierzZaznaczona()
        {
            if (dgvLista.CurrentRow?.DataBoundItem is DostepnaCzescView item)
            {
                WybranaCzesc = item;
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