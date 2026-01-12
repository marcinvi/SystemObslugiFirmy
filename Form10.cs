using System;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeepL; // Używamy nowej biblioteki DeepL
using OutlookApp = Microsoft.Office.Interop.Outlook;

namespace Reklamacje_Dane
{
    public partial class Form10 : Form
    {
        // Prywatne pola do przechowywania danych pobranych z bazy
        private string nrZgloszenia;
        private string nazwaProducenta;
        private string kodProducenta;
        private string nrSeryjny; // Dodatkowe pole na numer seryjny
        private string opisUsterkiOryginalny;
        private string deepLApiKey; // Pole na klucz API

        public Form10(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // Metoda Load formularza, która inicjalizuje dane
        private async void Form10_Load(object sender, EventArgs e)
        {
            await InicjalizujFormularzAsync();
        }

        // ZMIANA: Zmodyfikowana metoda do pobierania wszystkich potrzebnych danych za jednym razem
        private async Task InicjalizujFormularzAsync()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    // ZMIANA: Zapytanie SQL pobiera teraz dodatkowo numer seryjny (nrSeryjny) i klucz API DeepL
                    string query = @"SELECT 
                                       z.OpisUsterki, 
                                       z.NrSeryjny,
                                       p.KodProducenta, 
                                       pr.NazwaProducenta, 
                                       pr.KontaktMail,
                                       (SELECT u.Wartosczaszyfrowana FROM Ustawienia u WHERE u.klucz = 'DeepL') as DeepLKey
                                   FROM Zgloszenia z
                                   LEFT JOIN Produkty p ON z.ProduktID = p.Id
                                   LEFT JOIN Producenci pr ON p.Producent = pr.NazwaProducenta
                                   WHERE z.NrZgloszenia = @nrZgloszenia";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Zapisujemy pobrane dane do pól prywatnych klasy
                                this.kodProducenta = reader["KodProducenta"]?.ToString();
                                this.opisUsterkiOryginalny = reader["OpisUsterki"]?.ToString();
                                this.nazwaProducenta = reader["NazwaProducenta"]?.ToString();
                                this.nrSeryjny = reader["NrSeryjny"]?.ToString();
                                this.deepLApiKey = reader["DeepLKey"]?.ToString();

                                textBoxOkolicznoscWady.Text = reader["KontaktMail"]?.ToString();

                                // Generujemy i ustawiamy treść e-maila
                                GenerujTrescEmaila();
                            }
                        }
                    }
                }

                // Sprawdzenie, czy klucz API został pobrany
                if (string.IsNullOrWhiteSpace(this.deepLApiKey))
                {
                    MessageBox.Show("Nie znaleziono klucza API dla DeepL w bazie danych. Funkcja tłumaczenia będzie niedostępna.", "Brak klucza API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    button1.Enabled = false; // Wyłączamy przycisk tłumaczenia
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania danych do formularza: " + ex.Message, "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // NOWOŚĆ: Metoda do generowania "inteligentniejszej" treści e-maila
        private void GenerujTrescEmaila()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Dzień dobry,");
            sb.AppendLine();
            sb.AppendLine($"w nawiązaniu do zgłoszenia reklamacyjnego o numerze {this.nrZgloszenia}, zgłaszamy problem z produktem.");
            sb.AppendLine();
            sb.AppendLine("Szczegóły produktu:");
            sb.AppendLine($"- Model / Kod produktu: {this.kodProducenta}");

            // ZMIANA: Dodajemy numer seryjny tylko jeśli istnieje
            if (!string.IsNullOrWhiteSpace(this.nrSeryjny))
            {
                sb.AppendLine($"- Numer seryjny (S/N): {this.nrSeryjny}");
            }

            sb.AppendLine();
            sb.AppendLine("Opis problemu zgłoszony przez klienta:");
            sb.AppendLine($"\"{this.opisUsterkiOryginalny}\"");
            sb.AppendLine();
            sb.AppendLine("Prosimy o informację zwrotną dotyczącą dalszych kroków w tej sprawie.");
            sb.AppendLine();
            sb.AppendLine("W celu zachowania ciągłości korespondencji, uprzejmie prosimy o odpowiadanie na tę wiadomość.");
            sb.AppendLine("Prosimy również, aby w załączanych dokumentach zawsze znajdował się nasz numer zgłoszenia.");
            sb.AppendLine();
            sb.AppendLine("Z poważaniem,");
            sb.AppendLine("Zespół Reklamacji"); // Możesz tu wstawić nazwę swojej firmy

            textBoxOpisUsterki.Text = sb.ToString();
        }

        // Metoda obsługująca przycisk wysyłania e-maila
        private async void buttonWyslij_Click(object sender, EventArgs e)
        {
            string adresOdbiorcy = textBoxOkolicznoscWady.Text;
            string temat = $"Zgłoszenie reklamacyjne / Claim No: {nrZgloszenia} - {kodProducenta}";
            string tresc = textBoxOpisUsterki.Text;

            if (string.IsNullOrWhiteSpace(adresOdbiorcy))
            {
                MessageBox.Show("Proszę podać adres e-mail producenta.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            WyslijEmailPrzezOutlook(adresOdbiorcy, temat, tresc);

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();

                    // ✅ POPRAWKA: Usunięto odwołanie do nieistniejącej kolumny "Producent"
                    string query = "UPDATE Zgloszenia SET StatusProducent = 'Zgłoszono do producenta', CzekamyNaDostawe = 'Nie Czekamy' WHERE NrZgloszenia = @nrZgloszenia";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        // ✅ POPRAWKA: Usunięto parametr, bo nie jest już używany w zapytaniu
                        // cmd.Parameters.AddWithValue("@producent", this.nazwaProducenta); 
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Ten kod zadziała poprawnie, bo 'this.nazwaProducenta' została pobrana przy ładowaniu formularza
                var dziennik = new DziennikLogger();
                await dziennik.DodajAsync(Program.fullName, $"Zgłoszono do {this.nazwaProducenta}", this.nrZgloszenia);
                var dzialanie = new Dzialaniee();
                dzialanie.DodajNoweDzialanie(this.nrZgloszenia, Program.fullName, $"Zgłoszono do {this.nazwaProducenta}");
                UpdateManager.NotifySubscribers();
                MessageBox.Show("Wiadomość e-mail została przygotowana. Zgłoszenie zaktualizowane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas aktualizacji statusu zgłoszenia: " + ex.Message, "Błąd bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ZMIANA: Całkowicie nowa logika przycisku tłumaczenia z użyciem API
        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.deepLApiKey))
            {
                MessageBox.Show("Klucz API DeepL jest nieprawidłowy lub nie został wczytany.", "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var translator = new Translator(this.deepLApiKey);
                string tekstDoTlumaczenia = textBoxOpisUsterki.Text;

                // Tłumaczymy tekst na angielski (Wielka Brytania). Możesz zmienić na "en-US" lub inny język.
                var translatedText = await translator.TranslateTextAsync(
                    tekstDoTlumaczenia,
                    LanguageCode.Polish,
                    LanguageCode.EnglishBritish
                );

                textBoxOpisUsterki.Text = translatedText.Text;
                MessageBox.Show("Tekst został przetłumaczony.", "Tłumaczenie zakończone", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas tłumaczenia: " + ex.Message, "Błąd DeepL API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Metoda do wysyłania e-maila przez Outlook (bez zmian)
        private void WyslijEmailPrzezOutlook(string adresOdbiorcy, string temat, string tresc)
        {
            try
            {
                var app = new OutlookApp.Application();
                var mail = (OutlookApp.MailItem)app.CreateItem(OutlookApp.OlItemType.olMailItem);
                mail.To = adresOdbiorcy;
                mail.Subject = temat;
                mail.Body = tresc;
                mail.Display(false); // Wyświetla okno e-maila do weryfikacji przez użytkownika
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas tworzenia e-maila w aplikacji Outlook: " + ex.Message, "Błąd Outlook", MessageBoxButtons.OK, MessageBoxIcon.Error);
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