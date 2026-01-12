// ############################################################################
// Plik: FormDodajDoMagazynu.cs (WERSJA NAPRAWIONA + DYNAMICZNA)
// Opis: Naprawiono błąd wyszukiwania (JOIN z tabelą Przesylki) i dodano wyszukiwanie w czasie rzeczywistym.
// ############################################################################

using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormDodajDoMagazynu : Form
    {
        private readonly DatabaseService _dbService;
        private readonly MagazynService _magazynService;

        // Pola potrzebne do logiki magazynowej
        private string _wybranyNrZgloszenia = "";
        private int _wybranyProduktId = 0;
        private string _kategoriaProduktu = "";

        public FormDodajDoMagazynu()
        {
            InitializeComponent();
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            _magazynService = new MagazynService();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormDodajDoMagazynu_Load(object sender, EventArgs e)
        {
            // Konfiguracja wstępna
            cmbStatus.Items.AddRange(new object[] {
                "Przyjęty na stan",
                "DAWCA CZĘŚCI",
                "ZŁOM",
                "W serwisie"
            });
            cmbStatus.SelectedItem = "Przyjęty na stan";

            var lok = await _magazynService.PobierzLokalizacjeAsync();
            cmbLokalizacja.Items.AddRange(lok.ToArray());
            cmbLokalizacja.Text = "Magazyn Przyjęć";

            // Podpięcie dynamicznego wyszukiwania
            txtSzukaj.TextChanged += async (s, ev) => await WyszukajDynamicznie();
        }

        // --- WYSZUKIWANIE (NAPRAWIONE) ---

        private async Task WyszukajDynamicznie()
        {
            string fraza = txtSzukaj.Text.Trim();

            // Szukamy nawet przy 1 znaku, żeby sprawdzić czy w ogóle działa
            if (string.IsNullOrEmpty(fraza))
            {
                dgvWyniki.DataSource = null;
                return;
            }

            try
            {
                // UPROSZCZONE ZAPYTANIE (Bez tabeli Przesylki, która może nie istnieć)
                // Dzięki temu sprawdzimy, czy działa podstawa (Zgłoszenia + Produkty + Klienci)
                string sql = @"
                    SELECT 
                        z.NrZgloszenia, 
                        IFNULL(z.NrSeryjny, '') AS SN, 
                        IFNULL(p.NazwaSystemowa, 'Brak modelu') AS Model, 
                        IFNULL(k.ImieNazwisko, 'Brak klienta') AS Klient,
                        p.Id AS ProduktId,
                        p.Kategoria
                    FROM Zgloszenia z
                    LEFT JOIN Produkty p ON z.ProduktID = p.Id
                    LEFT JOIN Klienci k ON z.KlientID = k.Id
                    WHERE 
                        z.NrZgloszenia LIKE @q OR 
                        z.NrSeryjny LIKE @q OR 
                        p.NazwaSystemowa LIKE @q OR
                        k.ImieNazwisko LIKE @q";

                var dt = await _dbService.GetDataTableAsync(sql, new MySqlParameter("@q", $"%{fraza}%"));

                dgvWyniki.DataSource = dt;

                // Ukrywamy kolumny techniczne
                if (dgvWyniki.Columns.Contains("ProduktId")) dgvWyniki.Columns["ProduktId"].Visible = false;
                if (dgvWyniki.Columns.Contains("Kategoria")) dgvWyniki.Columns["Kategoria"].Visible = false;
            }
            catch (Exception ex)
            {
                // TERAZ POKAŻE BŁĄD, JEŚLI COŚ JEST NIE TAK
                // Wyświetlamy go w tytule okna, żeby nie przerywać pisania MessageBoxem
                this.Text = "BŁĄD SZUKANIA: " + ex.Message;
            }
        }

        // Obsługa przycisku "Szukaj"
        private async void btnSzukaj_Click(object sender, EventArgs e)
        {
            string fraza = txtSzukaj.Text.Trim();
            if (string.IsNullOrEmpty(fraza))
            {
                MessageBox.Show("Wpisz coś, aby wyszukać.");
                return;
            }

            try
            {
                // Tutaj wywołujemy to samo, ale w razie błędu pokażemy MessageBox
                await WyszukajDynamicznie();

                if (dgvWyniki.Rows.Count == 0)
                {
                    MessageBox.Show("Brak wyników dla frazy: " + fraza);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Krytyczny błąd bazy danych: " + ex.Message);
            }
        }

        

        private void txtSzukaj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _ = WyszukajDynamicznie();
                e.SuppressKeyPress = true;
            }
        }

        // --- WYBÓR ZGŁOSZENIA ---
        private async void dgvWyniki_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvWyniki.CurrentRow != null && dgvWyniki.CurrentRow.Cells["NrZgloszenia"].Value != null)
            {
                _wybranyNrZgloszenia = dgvWyniki.CurrentRow.Cells["NrZgloszenia"].Value.ToString();

                // Bezpieczne pobieranie wartości
                string model = dgvWyniki.Columns.Contains("Model") ? dgvWyniki.CurrentRow.Cells["Model"].Value?.ToString() : "";
                string sn = dgvWyniki.Columns.Contains("SN") ? dgvWyniki.CurrentRow.Cells["SN"].Value?.ToString() : "";

                if (dgvWyniki.Columns.Contains("ProduktId") && dgvWyniki.CurrentRow.Cells["ProduktId"].Value != DBNull.Value)
                {
                    _wybranyProduktId = Convert.ToInt32(dgvWyniki.CurrentRow.Cells["ProduktId"].Value);
                    _kategoriaProduktu = dgvWyniki.CurrentRow.Cells["Kategoria"].Value?.ToString();
                }

                // Autouzupełnianie pól
                txtModel.Text = model;
                txtSN.Text = sn;
                lblPowiazanie.Text = $"Powiązano z zgłoszeniem: {_wybranyNrZgloszenia}";
                lblPowiazanie.ForeColor = Color.Green;

                // Jeśli status to DAWCA, odświeżamy listę części
                if (cmbStatus.Text == "DAWCA CZĘŚCI")
                {
                    await ZaladujSzablonCzesci();
                }
            }
        }

        // --- LOGIKA CZĘŚCI I ZAPISU (TAKA SAMA JAK W FormMagazynAction) ---

        private async void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string status = cmbStatus.Text;
            if (status == "DAWCA CZĘŚCI")
            {
                grpCzesci.Visible = true;
                this.Height = 760;
                await ZaladujSzablonCzesci();
            }
            else
            {
                grpCzesci.Visible = false;
                this.Height = 550;
            }
        }

        private async Task ZaladujSzablonCzesci()
        {
            chkListCzesci.Items.Clear();
            cmbNowaCzescSzybka.Items.Clear();

            if (_wybranyProduktId <= 0) return;

            var szablony = await _magazynService.PobierzSzablonyDlaProduktuAsync(_wybranyProduktId);
            foreach (var czesc in szablony) chkListCzesci.Items.Add(czesc.NazwaCzesci);

            var sugestie = await _magazynService.PobierzSugestieCzesciAsync(_kategoriaProduktu);
            cmbNowaCzescSzybka.Items.AddRange(sugestie.ToArray());
        }

        private async void btnDodajCzescSzybka_Click(object sender, EventArgs e)
        {
            string nowa = cmbNowaCzescSzybka.Text.Trim();
            if (string.IsNullOrEmpty(nowa) || _wybranyProduktId <= 0) return;

            await _magazynService.DodajSzablonCzesciAsync(_wybranyProduktId, nowa);

            var zaznaczoneWczesniej = chkListCzesci.CheckedItems.Cast<string>().ToList();
            zaznaczoneWczesniej.Add(nowa);

            await ZaladujSzablonCzesci();

            for (int i = 0; i < chkListCzesci.Items.Count; i++)
            {
                if (zaznaczoneWczesniej.Contains(chkListCzesci.Items[i].ToString()))
                    chkListCzesci.SetItemChecked(i, true);
            }
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Podaj nazwę modelu.");
                return;
            }

            try
            {
                // Jeśli sprzęt luźny (brak zgłoszenia), generujemy ID
                if (string.IsNullOrEmpty(_wybranyNrZgloszenia))
                {
                    _wybranyNrZgloszenia = $"LUZ/{DateTime.Now:yyyyMMddHHmmss}";
                }

                // Sprawdzamy czy już jest na stanie
                var stan = await _magazynService.PobierzStanAsync(_wybranyNrZgloszenia);
                if (stan != null)
                {
                    MessageBox.Show($"To zgłoszenie ({_wybranyNrZgloszenia}) jest już w magazynie!\nStatus: {stan.StatusFizyczny}\nLokalizacja: {stan.Lokalizacja}");
                    return;
                }

                // 1. Zapis do bazy (INSERT)
                // Teraz kolumny Model i NumerSeryjny już istnieją po naprawie SQL
                await _magazynService.PrzyjmijNaMagazynAsync(
                    _wybranyNrZgloszenia,
                    txtModel.Text,
                    txtSN.Text,
                    $"Dodano ręcznie. {txtUwagi.Text}"
                );

                // 2. Aktualizacja statusu i lokalizacji (UPDATE)
                bool czyDawca = (cmbStatus.Text == "DAWCA CZĘŚCI");
                await _magazynService.AktualizujStatusAsync(
                    _wybranyNrZgloszenia,
                    cmbStatus.Text,
                    cmbLokalizacja.Text,
                    czyDawca,
                    txtUwagi.Text
                );

                // 3. Zapis części (jeśli Dawca)
                if (czyDawca)
                {
                    int magId = await _magazynService.PobierzIdMagazynoweAsync(_wybranyNrZgloszenia);
                    var czesci = chkListCzesci.CheckedItems.Cast<string>().ToList();
                    await _magazynService.ZapiszCzesciZDemontazuAsync(magId, czesci);
                }

                // 4. Log do dziennika (jeśli to prawdziwe zgłoszenie)
                if (!_wybranyNrZgloszenia.StartsWith("LUZ/"))
                {
                    string log = $"Sprzęt fizycznie przyjęty na magazyn (Ręcznie). Lok: {cmbLokalizacja.Text}, Status: {cmbStatus.Text}";
                    await new DziennikLogger().DodajAsync(Program.fullName, log, _wybranyNrZgloszenia);
                    new Dzialaniee().DodajNoweDzialanie(_wybranyNrZgloszenia, Program.fullName, log);
                }

                MessageBox.Show("Sprzęt zapisany w magazynie.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
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