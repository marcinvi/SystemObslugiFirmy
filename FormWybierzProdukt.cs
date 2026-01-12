using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    public partial class FormWybierzProdukt : Form
    {
        private readonly MagazynService _service;
        private DataTable _dtProdukty; // Przechowujemy dane w pamięci dla szybkości

        public int WybraneId { get; private set; }
        public string WybranaNazwa { get; private set; }

        public FormWybierzProdukt()
        {
            InitializeComponent();
            _service = new MagazynService();
            this.Load += FormWybierzProdukt_Load;
            this.txtSzukaj.TextChanged += TxtSzukaj_TextChanged;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormWybierzProdukt_Load(object sender, EventArgs e)
        {
            try
            {
                // Pobieramy WSZYSTKIE kolumny, o które pytałeś
                string sql = "SELECT Id, NazwaSystemowa, NazwaKrotka, KodEnova, KodProducenta, Producent, Kategoria FROM Produkty";

                // Używamy helpera z serwisu (musimy go dodać, lub użyć dbService bezpośrednio)
                // Zakładam, że w MagazynService masz dostęp do DatabaseService
                // Jeśli nie, dodaj metodę w serwisie: public async Task<DataTable> PobierzPelnaListeProduktow() { ... }

                // Szybka implementacja inline (żeby nie zmieniać serwisu 10 razy):
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new MySqlCommand(sql, con))
                    using (var da = new MySqlDataAdapter(cmd))
                    {
                        _dtProdukty = new DataTable();
                        da.Fill(_dtProdukty);
                    }
                }

                dgvProdukty.DataSource = _dtProdukty;
                if (dgvProdukty.Columns.Contains("Id")) dgvProdukty.Columns["Id"].Visible = false;

                // Ustawiamy szerokości (opcjonalnie)
                if (dgvProdukty.Columns.Contains("NazwaSystemowa")) dgvProdukty.Columns["NazwaSystemowa"].FillWeight = 200;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
        }

        private void TxtSzukaj_TextChanged(object sender, EventArgs e)
        {
            if (_dtProdukty == null) return;

            string fraza = txtSzukaj.Text.Trim().Replace("'", "''"); // Zabezpieczenie SQL-like

            if (string.IsNullOrEmpty(fraza))
            {
                _dtProdukty.DefaultView.RowFilter = "";
            }
            else
            {
                // MAGIA: Szukamy po wszystkich kluczowych kolumnach na raz
                _dtProdukty.DefaultView.RowFilter = string.Format(
                    "NazwaSystemowa LIKE '%{0}%' OR " +
                    "NazwaKrotka LIKE '%{0}%' OR " +
                    "KodEnova LIKE '%{0}%' OR " +
                    "KodProducenta LIKE '%{0}%' OR " +
                    "Producent LIKE '%{0}%'",
                    fraza);
            }
        }

        private void btnWybierz_Click(object sender, EventArgs e)
        {
            Wybierz();
        }

        private void dgvProdukty_DoubleClick(object sender, EventArgs e)
        {
            Wybierz();
        }

        private void Wybierz()
        {
            if (dgvProdukty.CurrentRow != null)
            {
                WybraneId = Convert.ToInt32(dgvProdukty.CurrentRow.Cells["Id"].Value);
                WybranaNazwa = dgvProdukty.CurrentRow.Cells["NazwaSystemowa"].Value.ToString();

                // Opcjonalnie doklejamy kod do nazwy dla lepszej czytelności w oknie głównym
                string kod = dgvProdukty.CurrentRow.Cells["KodEnova"].Value?.ToString();
                if (!string.IsNullOrEmpty(kod)) WybranaNazwa = $"[{kod}] {WybranaNazwa}";

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