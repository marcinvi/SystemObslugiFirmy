using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class Form16 : Form
    {
        private int _wybranyProducentId = -1;
        private string _wybranyProducentNazwa = "";
        private readonly DatabaseService _dbService;

        public Form16()
        {
            InitializeComponent();
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());

            // Inicjalizacja ComboBoxów
            comboJezyk.Items.AddRange(new string[] { "PL", "ENG" });
            comboFormularz.Items.AddRange(new string[] { "Tak", "Nie" });
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form16_Load(object sender, EventArgs e)
        {
            await WczytajListeProducentow();
        }

        private async Task WczytajListeProducentow()
        {
            try
            {
                string query = "SELECT Id, NazwaProducenta FROM Producenci ORDER BY NazwaProducenta";
                DataTable dt = await _dbService.GetDataTableAsync(query);
                dataGridView1.DataSource = dt;
                if (dataGridView1.Columns.Contains("Id"))
                {
                    dataGridView1.Columns["Id"].Visible = false;
                }
                dataGridView1.Columns["NazwaProducenta"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                WyczyscPolaFormularza();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wczytywania listy producentów: {ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WyczyscPolaFormularza()
        {
            dataGridView1.ClearSelection();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboJezyk.SelectedItem = null;
            comboFormularz.SelectedItem = null;
            textBox6.Text = "";
            dataGridViewProducts.DataSource = null;

            _wybranyProducentId = -1;
            _wybranyProducentNazwa = "";

            button2.Text = "Dodaj nowego";
            button2.Enabled = false;
            button3.Enabled = false;
            btnDodajProdukt.Enabled = false;
        }

        private async void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                _wybranyProducentId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                _wybranyProducentNazwa = dataGridView1.SelectedRows[0].Cells["NazwaProducenta"].Value.ToString();

                await WyswietlSzczegolyProducenta(_wybranyProducentId);
                await WyswietlProduktyDlaProducenta(_wybranyProducentNazwa);

                btnDodajProdukt.Enabled = true;
            }
            else
            {
                WyczyscPolaFormularza();
            }
        }

        private async Task WyswietlSzczegolyProducenta(int id)
        {
            try
            {
                string query = "SELECT * FROM Producenci WHERE Id = @id";
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                textBox1.Text = reader["NazwaProducenta"]?.ToString();
                                textBox2.Text = reader["KontaktMail"]?.ToString();
                                textBox3.Text = reader["Adres"]?.ToString();
                                comboJezyk.SelectedItem = reader["Jezyk"]?.ToString();
                                comboFormularz.SelectedItem = reader["Formularz"]?.ToString();
                                textBox6.Text = reader["wymagania"]?.ToString();

                                button2.Text = "Zapisz zmiany";
                                button2.Enabled = true;
                                button3.Enabled = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wczytywania szczegółów producenta: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task WyswietlProduktyDlaProducenta(string nazwaProducenta)
        {
            try
            {
                DataTable dt = new DataTable();
                string query = "SELECT Id, NazwaSystemowa, KodProducenta, Kategoria FROM Produkty WHERE Producent = @producent ORDER BY NazwaSystemowa";

                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var adapter = new MySqlDataAdapter(query, con))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@producent", nazwaProducenta);
                        adapter.Fill(dt);
                    }
                }
                dataGridViewProducts.DataSource = dt;

                if (dataGridViewProducts.Columns.Contains("Id"))
                {
                    dataGridViewProducts.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wczytywania listy produktów: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void dataGridViewProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string productId = dataGridViewProducts.Rows[e.RowIndex].Cells["Id"].Value.ToString();

                using (var formEdycji = new Form15(productId))
                {
                    var result = formEdycji.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        await WyswietlProduktyDlaProducenta(_wybranyProducentNazwa);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WyczyscPolaFormularza();
            _wybranyProducentId = -1;
            button2.Text = "Dodaj nowego";
            button2.Enabled = true;
            textBox1.Focus();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Nazwa producenta nie może być pusta.", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var parametry = new[]
            {
                new MySqlParameter("@nazwa", textBox1.Text.Trim()),
                new MySqlParameter("@mail", textBox2.Text.Trim()),
                new MySqlParameter("@adres", textBox3.Text.Trim()),
                new MySqlParameter("@jezyk", comboJezyk.SelectedItem?.ToString() ?? ""),
                new MySqlParameter("@formularz", comboFormularz.SelectedItem?.ToString() ?? ""),
                new MySqlParameter("@wymagania", textBox6.Text.Trim())
            };
            try
            {
                if (_wybranyProducentId == -1)
                {
                    string query = @"INSERT INTO Producenci (NazwaProducenta, KontaktMail, Adres, Jezyk, Formularz, wymagania) VALUES (@nazwa, @mail, @adres, @jezyk, @formularz, @wymagania)";
                    await _dbService.ExecuteNonQueryAsync(query, parametry);
                    MessageBox.Show("Nowy producent został dodany.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string query = @"UPDATE Producenci SET NazwaProducenta = @nazwa, KontaktMail = @mail, Adres = @adres, Jezyk = @jezyk, Formularz = @formularz, wymagania = @wymagania WHERE Id = @id";
                    var parametryUpdate = new System.Collections.Generic.List<MySqlParameter>(parametry);
                    parametryUpdate.Add(new MySqlParameter("@id", _wybranyProducentId));
                    await _dbService.ExecuteNonQueryAsync(query, parametryUpdate.ToArray());
                    MessageBox.Show("Dane producenta zostały zaktualizowane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                await WczytajListeProducentow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas zapisu danych: {ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (_wybranyProducentId == -1) return;
            var wynik = MessageBox.Show("Czy na pewno chcesz usunąć tego producenta?", "Potwierdzenie usunięcia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (wynik == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Producenci WHERE Id = @id";
                    await _dbService.ExecuteNonQueryAsync(query, new MySqlParameter("@id", _wybranyProducentId));
                    MessageBox.Show("Producent został usunięty.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await WczytajListeProducentow();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas usuwania: {ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnDodajProdukt_Click(object sender, EventArgs e)
        {
            // Otwieramy Form15 w trybie dodawania, przekazując nazwę wybranego producenta
            using (var formDodawania = new Form15(_wybranyProducentNazwa))
            {
                var result = formDodawania.ShowDialog();
                if (result == DialogResult.OK)
                {
                    await WyswietlProduktyDlaProducenta(_wybranyProducentNazwa);
                }
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            string fraza = textBox7.Text;
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"NazwaProducenta LIKE '%{fraza}%'";
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