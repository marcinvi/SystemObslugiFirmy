using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeepL; // Dodano using dla DeepL
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using OutlookApp = Microsoft.Office.Interop.Outlook;

namespace Reklamacje_Dane
{
    public partial class Form8 : Form
    {
        private string nrZgloszenia;
        private string deepLApiKey; // Dodano pole na klucz API

        public Form8(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form8_Load(object sender, EventArgs e)
        {
            await InicjalizujFormularzAsync();
        }

        private async Task InicjalizujFormularzAsync()
        {
            // Usunięto inicjalizację WebView2
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    // Zaktualizowano zapytanie o klucz API DeepL
                    string query = @"SELECT 
                                       p.KodProducenta, 
                                       z.OpisUsterki, 
                                       z.NrFaktury, 
                                       z.DataZakupu,
                                       (SELECT u.Wartosczaszyfrowana FROM Ustawienia u WHERE u.klucz = 'DeepL') as DeepLKey
                                     FROM Zgloszenia z
                                     LEFT JOIN Produkty p ON z.ProduktID = p.Id
                                     WHERE z.NrZgloszenia = @nrZgloszenia";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                textBoxKodProduktuHella.Text = reader["KodProducenta"].ToString();
                                textBoxOpisUsterki.Text = reader["OpisUsterki"].ToString();
                                textBoxNumerFVZakupowej.Text = reader["NrFaktury"].ToString();
                                this.deepLApiKey = reader["DeepLKey"]?.ToString(); // Odczyt klucza

                                if (DateTime.TryParse(reader["DataZakupu"].ToString(), out DateTime dataZakupu))
                                {
                                    dateTimePickerDataFVZakupowej.Value = dataZakupu;
                                }
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
                MessageBox.Show("Błąd wczytywania danych do formularza VEPRO: " + ex.Message);
            }
        }

        private async void buttonWyslij_Click(object sender, EventArgs e)
        {
            string kodProduktu = textBoxKodProduktuHella.Text;
            string numerFV = textBoxNumerFVZakupowej.Text;
            string dataFV = dateTimePickerDataFVZakupowej.Value.ToString("dd.MM.yyyy");
            string opisUsterki = textBoxOpisUsterki.Text;

            string sciezkaPDF = GenerujPDF(kodProduktu, numerFV, dataFV, opisUsterki);
            if (string.IsNullOrEmpty(sciezkaPDF))
            {
                MessageBox.Show("Nie udało się wygenerować pliku PDF.");
                return;
            }

            string adresOdbiorcy = "mail@vepro.fi";
            string temat = $"Claim number: {nrZgloszenia}";
            string tresc = $"Hi,\n\nI am sending attached a complaint form regarding: {kodProduktu}.\nClient reports: {opisUsterki}.\n\nBest Regards.";

            WyslijEmailPrzezOutlook(adresOdbiorcy, temat, tresc, sciezkaPDF);

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "UPDATE Zgloszenia SET StatusProducent = 'Zgłoszono do producenta', CzekamyNaDostawe = 'Czekamy' WHERE NrZgloszenia = @nrZgloszenia";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                var dziennik = new DziennikLogger();
                await dziennik.DodajAsync(Program.fullName, "Zgłoszono do VEPRO", this.nrZgloszenia);
                var dzialanie = new Dzialaniee();
                dzialanie.DodajNoweDzialanie(this.nrZgloszenia, Program.fullName, $"Zgłoszono do Vepro");
                UpdateManager.NotifySubscribers();
                MessageBox.Show("Wiadomość e-mail została przygotowana. Zgłoszenie zaktualizowane.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas aktualizacji statusu zgłoszenia: " + ex.Message);
            }
        }

        // Zmieniona metoda tłumaczenia na API
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

                // Tłumaczymy tekst na angielski (Wielka Brytania).
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

        private string GenerujPDF(string kodProduktu, string numerFV, string dataFV, string opisUsterki)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Formularze", "Vepro.pdf");
            string nazwaFolderuZgloszenia = nrZgloszenia.Replace("/", ".");
            string folderZgloszenia = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane", nazwaFolderuZgloszenia);
            if (!Directory.Exists(folderZgloszenia)) Directory.CreateDirectory(folderZgloszenia);
            string outputPath = Path.Combine(folderZgloszenia, $"Zgloszenie_Vepro_{nazwaFolderuZgloszenia}.pdf");

            try
            {
                PdfDocument document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
                if (document.AcroForm == null) return null;
                document.AcroForm.Elements.SetValue("/NeedAppearances", new PdfBoolean(true));

                SetAndLockField(document, "Data", DateTime.Now.ToString("dd.MM.yyyy"));
                SetAndLockField(document, "NrZgloszenia", nrZgloszenia);
                SetAndLockField(document, "Produkt", kodProduktu);
                SetAndLockField(document, "Adres", "reklamacje@enatruck.com");
                SetAndLockField(document, "Zglaszajacy", Program.fullName);
                SetAndLockField(document, "Datazakupu", $"FV: {numerFV} z dnia: {dataFV}");
                SetAndLockField(document, "Wada", opisUsterki);

                document.Save(outputPath);
                return outputPath;
            }
            catch (Exception ex) { MessageBox.Show($"Błąd generowania PDF dla VEPRO: {ex.Message}"); return null; }
        }

        private void SetAndLockField(PdfDocument document, string fieldName, string value)
        {
            if (document.AcroForm.Fields.Names.Contains(fieldName) && document.AcroForm.Fields[fieldName] is PdfTextField field)
            {
                field.Value = new PdfString(value);
                field.ReadOnly = true;
            }
        }

        private void WyslijEmailPrzezOutlook(string adresOdbiorcy, string temat, string tresc, string sciezkaZalacznika)
        {
            try
            {
                var app = new OutlookApp.Application();
                var mail = (OutlookApp.MailItem)app.CreateItem(OutlookApp.OlItemType.olMailItem);
                mail.To = adresOdbiorcy;
                mail.Subject = temat;
                mail.Body = tresc;
                mail.Attachments.Add(sciezkaZalacznika);
                mail.Display(false);
            }
            catch (Exception ex) { MessageBox.Show("Błąd podczas tworzenia e-maila: " + ex.Message); }
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
