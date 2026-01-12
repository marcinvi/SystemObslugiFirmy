using System;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class ProductInfoControl : UserControl
    {
        private string _nrZgloszenia;
        private int _produktId;
        private bool _isInEditMode = false;

        // Dane tymczasowe
        private string _producentNazwa;
        private string originalProduktNazwa, originalSeryjny, originalFaktura, originalData;

        // Zmienna przechowująca wybrane ID z nowego okna wyszukiwania
        private int _wybranyNowyProduktId = 0;

        public event EventHandler DataChanged;
        public event EventHandler<int> EditProductRequested;

        public ProductInfoControl()
        {
            InitializeComponent();
        }

        public async Task LoadPurchaseData(string nrZgloszenia)
        {
            if (string.IsNullOrEmpty(nrZgloszenia)) { ShowEmpty(); return; }
            this._nrZgloszenia = nrZgloszenia;

            try
            {
                using (var con = DatabaseHelper.GetConnectionAsync())
                {
                    await con.OpenAsync();
                    string query = @"SELECT z.ProduktID, z.NrSeryjny, z.NrFaktury, z.DataZakupu, z.Skad,
                                     p.NazwaKrotka, p.Kategoria, p.Producent
                                     FROM Zgloszenia z
                                     LEFT JOIN Produkty p ON z.ProduktID = p.Id
                                     WHERE z.NrZgloszenia = @nrZgloszenia";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this._nrZgloszenia);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                _produktId = reader["ProduktID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ProduktID"]);
                                _producentNazwa = reader["Producent"]?.ToString();

                                string nazwaKrotka = reader["NazwaKrotka"]?.ToString() ?? "Brak produktu";
                                string kategoria = reader["Kategoria"]?.ToString();

                                originalProduktNazwa = nazwaKrotka;
                                originalSeryjny = reader["NrSeryjny"]?.ToString() ?? "";
                                originalFaktura = reader["NrFaktury"]?.ToString() ?? "";
                                originalData = reader["DataZakupu"]?.ToString() ?? "";

                                // Ustawianie Labeli (Widok)
                                lblProdukt.Text = nazwaKrotka;
                                lblNrSeryjny.Text = $"🔢 SN: {originalSeryjny}";
                                lblFaktura.Text = $"🧾 {originalFaktura} z dnia: {originalData}";
                                lblKategoria.Text = $"📦 {kategoria} | {_producentNazwa}";
                                lblSkad.Text = $"🛒 Źródło: {WypelnijLabelSkad(reader["Skad"]?.ToString(), originalFaktura)}";

                                contextMenuStrip1.Enabled = true;
                            }
                            else { ShowEmpty(); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania: " + ex.Message);
                ShowEmpty();
            }
        }

        private void ShowEmpty()
        {
            lblProdukt.Text = "Brak danych produktu";
            lblNrSeryjny.Text = "";
            lblFaktura.Text = "";
            lblKategoria.Text = "";
            lblSkad.Text = "";
            contextMenuStrip1.Enabled = false;
        }

        private string WypelnijLabelSkad(string skad, string nrFaktury)
        {
            if (!string.IsNullOrWhiteSpace(skad)) return skad;
            nrFaktury = nrFaktury ?? "";
            if (nrFaktury.StartsWith("FV", StringComparison.OrdinalIgnoreCase)) return "Truck-Shop";
            if (nrFaktury.EndsWith("FV", StringComparison.OrdinalIgnoreCase)) return "Enatruck";
            return "Brak informacji";
        }

        private void EnterEditMode()
        {
            if (string.IsNullOrEmpty(_nrZgloszenia)) return;
            _isInEditMode = true;

            // Wypełnianie pól edycji
            _wybranyNowyProduktId = _produktId; // Domyślnie to co było
            txtProduktDisplay.Text = originalProduktNazwa; // Pokazujemy nazwę w polu readonly

            txtNrSeryjny.Text = originalSeryjny;
            txtNrFaktury.Text = originalFaktura;
            txtDataZakupu.Text = originalData;

            panelDisplay.Visible = false;
            panelEdit.Visible = true;
        }

        private void LeaveEditMode()
        {
            _isInEditMode = false;
            panelEdit.Visible = false;
            panelDisplay.Visible = true;
        }

        // OBSŁUGA PRZYCISKU SZUKAJ (NOWOŚĆ)
        private void btnSzukajProduktu_Click(object sender, EventArgs e)
        {
            // Otwieramy nowe okno wyszukiwania
            using (var searchForm = new FormProduktSelect())
            {
                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    _wybranyNowyProduktId = searchForm.WybranyProduktId;
                    txtProduktDisplay.Text = searchForm.WybranaNazwa;
                }
            }
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            // Logujemy zmiany tylko dla dziennika
            if (_wybranyNowyProduktId != _produktId) sb.AppendLine($"Produkt zmieniony na ID: {_wybranyNowyProduktId}");
            if (txtNrSeryjny.Text != originalSeryjny) sb.AppendLine($"SN zmieniony na: {txtNrSeryjny.Text}");

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "UPDATE Zgloszenia SET ProduktID = @produktID, NrSeryjny = @nrSeryjny, NrFaktury = @nrFaktury, DataZakupu = @dataZakupu WHERE NrZgloszenia = @nrZgloszenia";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@produktID", _wybranyNowyProduktId);
                        cmd.Parameters.AddWithValue("@nrSeryjny", txtNrSeryjny.Text);
                        cmd.Parameters.AddWithValue("@nrFaktury", txtNrFaktury.Text);
                        cmd.Parameters.AddWithValue("@dataZakupu", txtDataZakupu.Text);
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this._nrZgloszenia);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                if (sb.Length > 0)
                {
                    var dziennik = new DziennikLogger();
                    await dziennik.DodajAsync(Program.fullName, "Edycja produktu: " + sb.ToString().Replace("\r\n", ", "), this._nrZgloszenia);
                }

                MessageBox.Show("Zapisano zmiany.", "Sukces");
                LeaveEditMode();
                DataChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
            }
        }

        private void btnAnuluj_Click(object sender, EventArgs e) => LeaveEditMode();
        private void edytujDaneZakupuToolStripMenuItem_Click(object sender, EventArgs e) => EnterEditMode();
        private void edytujProduktToolStripMenuItem_Click(object sender, EventArgs e) => EditProductRequested?.Invoke(this, _produktId);
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) => txtDataZakupu.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");

        // Metody kopiowania (bez zmian logicznych)
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            skopiujKodEnovaToolStripMenuItem.Visible = (_produktId > 0);
            skopiujMailaProducentaToolStripMenuItem.Visible = !string.IsNullOrEmpty(_producentNazwa);
            skopiujNrFakturyToolStripMenuItem.Visible = !string.IsNullOrEmpty(originalFaktura);
            skopiujNrSeryjnyToolStripMenuItem.Visible = !string.IsNullOrEmpty(originalSeryjny);
        }

        private async void skopiujKodEnovaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var res = await new MySqlCommand("SELECT KodEnova FROM Produkty WHERE Id=" + _produktId, con).ExecuteScalarAsync();
                    if (res != null) Clipboard.SetText(res.ToString());
                }
            }
            catch { }
        }

        private async void skopiujMailaProducentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("SELECT KontaktMail FROM Producenci WHERE NazwaProducenta=@np", con);
                    cmd.Parameters.AddWithValue("@np", _producentNazwa);
                    var res = await cmd.ExecuteScalarAsync();
                    if (res != null) Clipboard.SetText(res.ToString());
                }
            }
            catch { }
        }

        private void skopiujNrFakturyToolStripMenuItem_Click(object sender, EventArgs e) { if (!string.IsNullOrEmpty(originalFaktura)) Clipboard.SetText(originalFaktura); }
        private void skopiujNrSeryjnyToolStripMenuItem_Click(object sender, EventArgs e) { if (!string.IsNullOrEmpty(originalSeryjny)) Clipboard.SetText(originalSeryjny); }

        public Dictionary<string, string> GetDataForPrinting()
        {
            return new Dictionary<string, string> {
                { "Produkt", lblProdukt.Text },
                { "SN", lblNrSeryjny.Text },
                { "Faktura", lblFaktura.Text }
            };
        }
    }
}