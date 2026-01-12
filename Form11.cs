// ############################################################################
// Plik: Form11.cs (POPRAWIONY - NIE USUWA DANYCH INNYCH PÓL)
// Opis: Poprawiono błąd nadpisywania WRL/KPZN pustymi wartościami przy zmianie statusu.
// ############################################################################

using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class Form11 : Form
    {
        private readonly string _nrZgloszenia;

        private readonly Dictionary<string, string> _statusMappings = new Dictionary<string, string>
        {
            { "Uznana - nowy produkt", "Uznana - czekamy na dostawę nowego produktu" },
            { "Uznana - nota korygująca", "Uznana - czekamy na notę korygującą" },
            { "Uznana - zakończona - naprawiona przez serwis", "Uznana - zakończona - naprawiona przez serwis" },
            { "Weryfikacja - wysłano do producenta", "Weryfikacja - wysłano do producenta" },
            { "Nieuwzględniona przez producenta", "Nieuwzględniona przez producenta" }
        };

        public Form11(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this._nrZgloszenia = nrZgloszenia;
            this.Text = $"Status u producenta - Zgłoszenie {_nrZgloszenia}";
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form11_Load(object sender, EventArgs e)
        {
            InicjalizujKontrolki();
            await WczytajAktualnyStan();
            AktualizujWidocznoscKontrolek();
        }

        private void InicjalizujKontrolki()
        {
            statusProducentaComboBox.Items.AddRange(_statusMappings.Keys.ToArray());
            statusProducentaComboBox.SelectedIndexChanged += (s, ev) => AktualizujWidocznoscKontrolek();
            towarDostarczonyCheckBox.CheckedChanged += (s, ev) => AktualizujWidocznoscKontrolek();
            notaWystawionaCheckBox.CheckedChanged += (s, ev) => AktualizujWidocznoscKontrolek();
        }

        private async Task WczytajAktualnyStan()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("SELECT StatusProducent, CzekamyNaDostawe, NrKWZ2, nrRMA, NrKPZN, NrWRL FROM Zgloszenia WHERE NrZgloszenia = @nr", con);
                    cmd.Parameters.AddWithValue("@nr", _nrZgloszenia);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string statusProducentaDB = reader["StatusProducent"]?.ToString();
                            statusProducentaComboBox.SelectedItem = MapujStatusZBazyNaUI(statusProducentaDB);

                            // Logika checkboxów
                            towarDostarczonyCheckBox.Checked = statusProducentaDB == "Uznana - Rozliczona - Nowy produkt";
                            notaWystawionaCheckBox.Checked = (statusProducentaDB != null && statusProducentaDB.StartsWith("Nota wprowadzona")) || statusProducentaDB == "Uznana - Rozliczona - Nota korygująca";
                            notaRozliczonaCheckBox.Checked = statusProducentaDB == "Uznana - Rozliczona - Nota korygująca";

                            // Wypełniamy WSZYSTKIE pola danymi z bazy
                            // Dzięki temu, nawet jak są ukryte, mają poprawne wartości
                            textBoxKwz2.Text = reader["NrKWZ2"]?.ToString();
                            textBoxKwz2Nota.Text = reader["NrKWZ2"]?.ToString();
                            textBoxNumerNoty.Text = reader["nrRMA"]?.ToString();
                            textBoxKpzn.Text = reader["NrKPZN"]?.ToString();
                            textBoxNrwrl.Text = reader["NrWRL"]?.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania: " + ex.Message);
            }
        }

        private string MapujStatusZBazyNaUI(string statusZBazy)
        {
            if (string.IsNullOrEmpty(statusZBazy)) return null;
            if (statusZBazy.Contains("dostawę") || statusZBazy.Contains("Nowy produkt")) return "Uznana - nowy produkt";
            if (statusZBazy.Contains("notę") || statusZBazy.Contains("Nota")) return "Uznana - nota korygująca";
            if (statusZBazy.Contains("naprawiona")) return "Uznana - zakończona - naprawiona przez serwis";
            return _statusMappings.FirstOrDefault(x => x.Value == statusZBazy).Key;
        }

        private void AktualizujWidocznoscKontrolek()
        {
            panelNowyProdukt.Visible = false;
            panelNota.Visible = false;
            panelNaprawa.Visible = false;

            string wybranyStatus = statusProducentaComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(wybranyStatus)) return;

            switch (wybranyStatus)
            {
                case "Uznana - nowy produkt":
                    panelNowyProdukt.Visible = true;
                    labelKwz2.Visible = textBoxKwz2.Visible = towarDostarczonyCheckBox.Checked;
                    break;
                case "Uznana - nota korygująca":
                    panelNota.Visible = true;
                    bool widoczne = notaWystawionaCheckBox.Checked;
                    labelNumerNoty.Visible = textBoxNumerNoty.Visible = widoczne;
                    labelKwz2Nota.Visible = textBoxKwz2Nota.Visible = widoczne;
                    labelKpzn.Visible = textBoxKpzn.Visible = widoczne;
                    notaRozliczonaCheckBox.Visible = widoczne;
                    break;
                case "Uznana - zakończona - naprawiona przez serwis":
                    panelNaprawa.Visible = true;
                    break;
            }
        }

        private async void zatwierdzButton_Click(object sender, EventArgs e)
        {
            string wybranyStatusUI = statusProducentaComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(wybranyStatusUI))
            {
                MessageBox.Show("Proszę wybrać status.");
                return;
            }

            // ### POPRAWKA KLUCZOWA ###
            // Inicjalizujemy zmienne wartościami z TextBoxów.
            // Dzięki temu, jeśli TextBox jest ukryty, pobieramy jego starą wartość (wczytaną z bazy),
            // a nie nadpisujemy jej pustym ciągiem.
            string finalnyStatusProducenta = "";
            string czekamyNaDostawe = "";

            string nrWrl = textBoxNrwrl.Text.Trim(); // <--- ZACHOWUJEMY WRL
            string nrRma = textBoxNumerNoty.Text.Trim();
            string nrKpzn = textBoxKpzn.Text.Trim();

            // KWZ 2 jest w dwóch miejscach, bierzemy z tego, który jest widoczny lub z dowolnego (są zsynchronizowane przy wczytaniu)
            string nrKwz2 = panelNowyProdukt.Visible ? textBoxKwz2.Text.Trim() :
                           (panelNota.Visible ? textBoxKwz2Nota.Text.Trim() :
                           (!string.IsNullOrEmpty(textBoxKwz2.Text) ? textBoxKwz2.Text : textBoxKwz2Nota.Text));

            var logMessage = new StringBuilder();

            switch (wybranyStatusUI)
            {
                case "Uznana - nowy produkt":
                    if (towarDostarczonyCheckBox.Checked)
                    {
                        finalnyStatusProducenta = "Uznana - Rozliczona - Nowy produkt";
                        czekamyNaDostawe = "Nie czekamy";
                        // nrKwz2 jest już pobrany wyżej z textBoxKwz2
                        logMessage.Append($"Status: {finalnyStatusProducenta}. Zapisano nr KWZ2: {nrKwz2}");
                    }
                    else
                    {
                        finalnyStatusProducenta = "Uznana - czekamy na dostawę nowego produktu";
                        czekamyNaDostawe = "Czekamy";
                        logMessage.Append($"Status: {finalnyStatusProducenta}.");
                    }
                    break;

                case "Uznana - nota korygująca":
                    if (notaWystawionaCheckBox.Checked)
                    {
                        // nrRma, nrKwz2, nrKpzn - pobrane na początku
                        if (notaRozliczonaCheckBox.Checked)
                        {
                            finalnyStatusProducenta = "Uznana - Rozliczona - Nota korygująca";
                            logMessage.Append($"Status: {finalnyStatusProducenta}. Nota ({nrRma}) rozliczona.");
                        }
                        else
                        {
                            finalnyStatusProducenta = "Nota wprowadzona - czekamy na rozliczenie";
                            logMessage.Append($"Status: {finalnyStatusProducenta}. Nota ({nrRma}) wprowadzona.");
                        }
                    }
                    else
                    {
                        finalnyStatusProducenta = "Uznana - czekamy na notę korygującą";
                        logMessage.Append($"Status: {finalnyStatusProducenta}.");
                    }
                    break;

                case "Uznana - zakończona - naprawiona przez serwis":
                    finalnyStatusProducenta = "Uznana - zakończona - naprawiona przez serwis";
                    // nrWrl - pobrane na początku
                    logMessage.Append($"Status: {finalnyStatusProducenta}. Zapisano nr WRL: {nrWrl}");
                    break;

                default:
                    finalnyStatusProducenta = _statusMappings[wybranyStatusUI];
                    logMessage.Append($"Status: {finalnyStatusProducenta}.");
                    break;
            }

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    // Aktualizujemy WSZYSTKIE pola, ale wartościami, które pobraliśmy z textboxów
                    // (więc te ukryte zachowują swoje stare wartości)
                    var cmd = new MySqlCommand(@"UPDATE Zgloszenia SET 
                                                    StatusProducent = @status, 
                                                    CzekamyNaDostawe = @czekamy,
                                                    NrKWZ2 = @kwz2,
                                                    nrRMA = @rma,
                                                    NrKPZN = @kpzn,
                                                    NrWRL = @wrl
                                                WHERE NrZgloszenia = @nr", con);

                    cmd.Parameters.AddWithValue("@status", finalnyStatusProducenta);
                    cmd.Parameters.AddWithValue("@czekamy", czekamyNaDostawe);
                    cmd.Parameters.AddWithValue("@kwz2", nrKwz2);
                    cmd.Parameters.AddWithValue("@rma", nrRma);
                    cmd.Parameters.AddWithValue("@kpzn", nrKpzn);
                    cmd.Parameters.AddWithValue("@wrl", nrWrl);
                    cmd.Parameters.AddWithValue("@nr", _nrZgloszenia);

                    await cmd.ExecuteNonQueryAsync();
                }

                // Logowanie
                string finalLog = $"Zmieniono status producenta. {logMessage}";
                var dziennik = new DziennikLogger();
                await dziennik.DodajAsync(Program.fullName, finalLog, _nrZgloszenia);

                // Nowy system dymków korzysta z tabeli Dzialania, więc dodajemy też tam:
                new Dzialaniee().DodajNoweDzialanie(_nrZgloszenia, Program.fullName, finalLog);

                UpdateManager.NotifySubscribers();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
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