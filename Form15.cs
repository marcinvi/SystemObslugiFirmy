using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class Form15 : Form
    {
        // Pola podstawowe
        private int _wybranyProduktId = -1;
        private readonly DatabaseService _dbService;

        // Pola dla konstruktorów specjalnych
        private readonly string _producentDoDodania = null;
        private readonly string _idDoEdycji = null;

        // Pola dla modułu MagazynService
        private MagazynService _magazynService;

        // Kontrolki dynamiczne dla sekcji części (tworzone kodem)
        private DataGridView dgvCzesci;
        private ComboBox cmbNowaCzesc; // Zmieniono z TextBox na ComboBox dla sugestii
        private Button btnDodajCzesc;
        private Button btnUsunCzesc;

        // --- KONSTRUKTORY ---

        public Form15()
        {
            InitializeComponent();
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            _magazynService = new MagazynService();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        public Form15(string idProduktuDoEdycji) : this()
        {
            _idDoEdycji = idProduktuDoEdycji;
            if (int.TryParse(idProduktuDoEdycji, out int id)) _wybranyProduktId = id;
        }

        public Form15(string nazwaProducentaDoDodania, bool isNew) : this()
        {
            _producentDoDodania = nazwaProducentaDoDodania;
        }

        // --- LOAD ---

        private async void Form15_Load(object sender, EventArgs e)
        {
            // 1. Budujemy interfejs dla części (podpięty pod panelDetali)
            BudujInterfejsCzesci();

            // 2. Ładujemy dane podstawowe
            await WczytajListeProduktow();
            await WypelnijComboBoxy();

            // 3. Obsługa trybów otwarcia
            if (_idDoEdycji != null)
            {
                // Tryb edycji - zaznaczamy produkt na liście
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Id"].Value.ToString() == _idDoEdycji)
                    {
                        row.Selected = true;
                        break;
                    }
                }

                // Ładujemy części i sugestie
                if (_wybranyProduktId > 0)
                {
                    await ZaladujCzesci();
                    await ZaladujSugestie();
                }
            }
            else if (_producentDoDodania != null)
            {
                // Tryb dodawania z predefiniowanym producentem
                WyczyscPolaFormularza();
                comboProducent.SelectedItem = _producentDoDodania;
            }
        }

        // --- MODUŁ CZĘŚCI ZAMIENNYCH (BUDOWA UI) ---

        private void BudujInterfejsCzesci()
        {
            // Rozszerzamy formularz i panel, żeby zmieścić nową sekcję
            this.Height = 700; // Zwiększamy wysokość okna

            var groupCzesci = new GroupBox
            {
                Text = "Części Zamienne (Szablon dla tego modelu)",
                Location = new Point(16, 340), // Poniżej pola Producent
                Size = new Size(575, 180),     // Dopasowana szerokość
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Tabela części
            dgvCzesci = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(groupCzesci.Width - 20, 110),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // ComboBox do wpisywania/wybierania części (INTELIGENTNE SUGESTIE)
            cmbNowaCzesc = new ComboBox
            {
                Location = new Point(10, 145),
                Width = 300,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                DropDownStyle = ComboBoxStyle.DropDown, // Pozwala wpisać własną nazwę
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };

            // Przycisk Dodaj
            btnDodajCzesc = new Button
            {
                Text = "Dodaj",
                Location = new Point(320, 143),
                Height = 28,
                Width = 80,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDodajCzesc.Click += async (s, ev) => await btnDodajCzesc_Click(s, ev);

            // Przycisk Usuń
            btnUsunCzesc = new Button
            {
                Text = "Usuń",
                Location = new Point(410, 143),
                Height = 28,
                Width = 80,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnUsunCzesc.Click += async (s, ev) => await btnUsunCzesc_Click(s, ev);

            // Dodanie kontrolek do GroupBoxa
            groupCzesci.Controls.Add(dgvCzesci);
            groupCzesci.Controls.Add(cmbNowaCzesc);
            groupCzesci.Controls.Add(btnDodajCzesc);
            groupCzesci.Controls.Add(btnUsunCzesc);

            // Dodanie GroupBoxa do Panelu Detali
            this.panelDetali.Controls.Add(groupCzesci);
        }

        // --- MODUŁ CZĘŚCI ZAMIENNYCH (LOGIKA) ---

        private async Task ZaladujCzesci()
        {
            if (_wybranyProduktId <= 0)
            {
                dgvCzesci.DataSource = null;
                return;
            }

            try
            {
                var czesci = await _magazynService.PobierzSzablonyDlaProduktuAsync(_wybranyProduktId);
                dgvCzesci.DataSource = null;
                dgvCzesci.DataSource = czesci;

                // Ukrywamy kolumny techniczne
                if (dgvCzesci.Columns.Contains("Id")) dgvCzesci.Columns["Id"].Visible = false;
                if (dgvCzesci.Columns.Contains("ProduktId")) dgvCzesci.Columns["ProduktId"].Visible = false;
                if (dgvCzesci.Columns.Contains("NazwaCzesci"))
                {
                    dgvCzesci.Columns["NazwaCzesci"].HeaderText = "Nazwa Części";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania części: " + ex.Message);
            }
        }

        private async Task ZaladujSugestie()
        {
            // Pobieramy kategorię z głównego ComboBoxa
            string kategoria = comboKategoria.Text;

            // Pobieramy posortowaną listę (najpopularniejsze w tej kategorii na górze)
            var sugestie = await _magazynService.PobierzSugestieCzesciAsync(kategoria);

            cmbNowaCzesc.Items.Clear();
            cmbNowaCzesc.Items.AddRange(sugestie.ToArray());
        }

        private async Task btnDodajCzesc_Click(object sender, EventArgs e)
        {
            string nazwa = cmbNowaCzesc.Text.Trim();
            if (string.IsNullOrEmpty(nazwa)) return;

            if (_wybranyProduktId <= 0)
            {
                MessageBox.Show("Najpierw zapisz produkt (kliknij 'Dodaj/Zapisz'), aby móc dodawać do niego części.");
                return;
            }

            try
            {
                await _magazynService.DodajSzablonCzesciAsync(_wybranyProduktId, nazwa);
                cmbNowaCzesc.Text = ""; // Czyścimy pole

                await ZaladujCzesci();    // Odświeżamy listę części tego produktu
                await ZaladujSugestie();  // Odświeżamy sugestie (nowa część trafi do bazy wiedzy)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd dodawania części: " + ex.Message);
            }
        }

        private async Task btnUsunCzesc_Click(object sender, EventArgs e)
        {
            if (dgvCzesci.CurrentRow == null) return;

            var part = dgvCzesci.CurrentRow.DataBoundItem as SzablonCzesci;
            if (part == null) return;

            if (MessageBox.Show($"Usunąć '{part.NazwaCzesci}' z szablonu tego produktu?", "Potwierdzenie", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    await _magazynService.UsunSzablonCzesciAsync(part.Id);
                    await ZaladujCzesci();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd usuwania: " + ex.Message);
                }
            }
        }

        // --- OBSŁUGA PRODUKTU (ORYGINALNA LOGIKA) ---

        private async Task WczytajListeProduktow()
        {
            try
            {
                string query = "SELECT Id, NazwaSystemowa, Producent FROM Produkty ORDER BY NazwaSystemowa";
                DataTable dt = await _dbService.GetDataTableAsync(query);
                dataGridView1.DataSource = dt;
                if (dataGridView1.Columns.Contains("Id"))
                {
                    dataGridView1.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wczytywania listy produktów: {ex.Message}");
            }
        }

        private async Task WypelnijComboBoxy()
        {
            comboProducent.Items.Clear();
            try
            {
                string queryProd = "SELECT NazwaProducenta FROM Producenci ORDER BY NazwaProducenta";
                DataTable dtProd = await _dbService.GetDataTableAsync(queryProd);
                foreach (DataRow row in dtProd.Rows)
                {
                    comboProducent.Items.Add(row["NazwaProducenta"].ToString());
                }
            }
            catch (Exception ex) { MessageBox.Show($"Błąd wczytywania producentów: {ex.Message}"); }

            comboKategoria.Items.Clear();
            try
            {
                string queryKat = "SELECT DISTINCT Kategoria FROM Produkty WHERE Kategoria IS NOT NULL AND Kategoria != '' ORDER BY Kategoria";
                DataTable dtKat = await _dbService.GetDataTableAsync(queryKat);
                foreach (DataRow row in dtKat.Rows)
                {
                    comboKategoria.Items.Add(row["Kategoria"].ToString());
                }
            }
            catch (Exception ex) { MessageBox.Show($"Błąd wczytywania kategorii: {ex.Message}"); }
        }

        private void WyczyscPolaFormularza()
        {
            dataGridView1.ClearSelection();
            NazwaSystemowa.Clear();
            Nazwakrotka.Clear();
            KodEnova.Clear();
            Kodporducenta.Clear();
            comboKategoria.Text = "";
            comboProducent.SelectedItem = null;
            _wybranyProduktId = -1;
            buttonZapisz.Text = "Dodaj/Zapisz";

            // Czyścimy sekcję części
            dgvCzesci.DataSource = null;
            cmbNowaCzesc.Text = "";
        }

        private async void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    _wybranyProduktId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                    string query = "SELECT * FROM Produkty WHERE Id = @id";

                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        using (var cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@id", _wybranyProduktId);
                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    NazwaSystemowa.Text = reader["NazwaSystemowa"].ToString();
                                    Nazwakrotka.Text = reader["NazwaKrotka"].ToString();
                                    KodEnova.Text = reader["KodEnova"].ToString();
                                    Kodporducenta.Text = reader["KodProducenta"].ToString();
                                    comboKategoria.Text = reader["Kategoria"].ToString();
                                    comboProducent.SelectedItem = reader["Producent"].ToString();
                                }
                            }
                        }
                    }
                    buttonZapisz.Text = "Zapisz zmiany";

                    // Po wybraniu produktu, ładujemy jego części i sugestie dla jego kategorii
                    await ZaladujCzesci();
                    await ZaladujSugestie();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd wczytywania szczegółów produktu: {ex.Message}");
                    WyczyscPolaFormularza();
                }
            }
        }

        private void buttonNowy_Click(object sender, EventArgs e)
        {
            WyczyscPolaFormularza();
        }

        private async void buttonZapisz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NazwaSystemowa.Text) || comboProducent.SelectedItem == null)
            {
                MessageBox.Show("Nazwa systemowa i producent są wymagane.", "Błąd Walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var parametry = new[]
            {
                new MySqlParameter("@nazwaSys", NazwaSystemowa.Text.Trim()),
                new MySqlParameter("@nazwaKrotka", Nazwakrotka.Text.Trim()),
                new MySqlParameter("@kodEnova", KodEnova.Text.Trim()),
                new MySqlParameter("@kodProd", Kodporducenta.Text.Trim()),
                new MySqlParameter("@kategoria", comboKategoria.Text.Trim()),
                new MySqlParameter("@producent", comboProducent.SelectedItem.ToString())
            };

            try
            {
                if (_wybranyProduktId == -1) // Tryb dodawania
                {
                    string query = "INSERT INTO Produkty (NazwaSystemowa, NazwaKrotka, KodEnova, KodProducenta, Kategoria, Producent) VALUES (@nazwaSys, @nazwaKrotka, @kodEnova, @kodProd, @kategoria, @producent); SELECT LAST_INSERT_ID();";

                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        using (var cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddRange(parametry);
                            var result = await cmd.ExecuteScalarAsync();
                            _wybranyProduktId = Convert.ToInt32(result);
                        }
                    }
                    MessageBox.Show("Nowy produkt został dodany. Możesz teraz zdefiniować części zamienne.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    buttonZapisz.Text = "Zapisz zmiany";
                }
                else // Tryb edycji
                {
                    string query = "UPDATE Produkty SET NazwaSystemowa = @nazwaSys, NazwaKrotka = @nazwaKrotka, KodEnova = @kodEnova, KodProducenta = @kodProd, Kategoria = @kategoria, Producent = @producent WHERE Id = @id";
                    var parametryUpdate = new List<MySqlParameter>(parametry);
                    parametryUpdate.Add(new MySqlParameter("@id", _wybranyProduktId));
                    await _dbService.ExecuteNonQueryAsync(query, parametryUpdate.ToArray());
                    MessageBox.Show("Zmiany zostały zapisane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await WczytajListeProduktow();
                // Nie czyścimy pól, aby można było od razu dodać części

                if (this.Modal) this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu danych: {ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonUsun_Click(object sender, EventArgs e)
        {
            if (_wybranyProduktId == -1)
            {
                MessageBox.Show("Proszę wybrać produkt do usunięcia.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var wynik = MessageBox.Show("Czy na pewno chcesz usunąć ten produkt?\nUWAGA: Zostaną usunięte również szablony części przypisane do tego produktu!", "Potwierdzenie usunięcia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (wynik == DialogResult.Yes)
            {
                try
                {
                    // Trigger ON DELETE CASCADE w bazie powinien usunąć części, ale logika aplikacji też powinna być czysta
                    string query = "DELETE FROM Produkty WHERE Id = @id";
                    await _dbService.ExecuteNonQueryAsync(query, new MySqlParameter("@id", _wybranyProduktId));

                    MessageBox.Show("Produkt został usunięty.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await WczytajListeProduktow();
                    WyczyscPolaFormularza();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas usuwania: {ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnZarzadzajProducentami_Click(object sender, EventArgs e)
        {
            using (var formProducenci = new Form16())
            {
                formProducenci.ShowDialog();
            }
            await WypelnijComboBoxy();
        }

        private void btnNowaKategoria_Click(object sender, EventArgs e)
        {
            string nowaKategoria = Interaction.InputBox("Wprowadź nazwę nowej kategorii:", "Dodaj kategorię", "");
            if (!string.IsNullOrWhiteSpace(nowaKategoria))
            {
                if (!comboKategoria.Items.Contains(nowaKategoria))
                {
                    comboKategoria.Items.Add(nowaKategoria);
                }
                comboKategoria.SelectedItem = nowaKategoria;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            string fraza = textBox7.Text;
            if (dataGridView1.DataSource is DataTable dt)
            {
                string safeFilter = fraza.Replace("'", "''").Replace("[", "[[]").Replace("%", "[%]");
                dt.DefaultView.RowFilter = $"NazwaSystemowa LIKE '%{safeFilter}%' OR Producent LIKE '%{safeFilter}%'";
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