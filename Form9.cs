using System;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutlookApp = Microsoft.Office.Interop.Outlook;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using System.Linq;
using DeepL; // Dodano using dla DeepL

namespace Reklamacje_Dane
{
    public partial class Form9 : Form
    {
        private string nrZgloszenia;
        private string deepLApiKey; // Dodano pole na klucz API

        public Form9(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form9_Load(object sender, EventArgs e)
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
                MessageBox.Show("Błąd wczytywania danych do formularza Strands: " + ex.Message);
            }
        }

        private async void buttonWyslij_Click(object sender, EventArgs e)
        {
            string kodProduktu = textBoxKodProduktuHella.Text;
            string numerFV = textBoxNumerFVZakupowej.Text;
            string dataFV = dateTimePickerDataFVZakupowej.Value.ToString("dd.MM.yyyy");
            string opisUsterki = textBoxOpisUsterki.Text;
            string numerZKabla = textBoxOkolicznoscWady.Text;
            string jakDlugo = numericUpDown1.Value + " " + comboBox1.Text;
            string typPojazdu = comboBox2.Text;
            string napiecie = comboBox3.Text;
            string montaz1 = comboBox4.Text;
            string montaz2 = comboBox5.Text;

            string sciezkaPDF = GenerujPDF(kodProduktu, numerFV, dataFV, opisUsterki, numerZKabla, jakDlugo, typPojazdu, napiecie, montaz1, montaz2);
            if (string.IsNullOrEmpty(sciezkaPDF))
            {
                MessageBox.Show("Nie udało się wygenerować pliku PDF.");
                return;
            }

            string adresOdbiorcy = "anette@strands.se;reklamation@strands.se;nora.nilsson@strands.se";
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
                await dziennik.DodajAsync(Program.fullName, "Zgłoszono do Strands", this.nrZgloszenia);
                var dzialanie = new Dzialaniee();
                dzialanie.DodajNoweDzialanie(this.nrZgloszenia, Program.fullName, $"Zgłoszono do Strands");
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

        #region --- Metody Pomocnicze (PDF i E-mail) ---
        private string GenerujPDF(string kodProduktu, string numerFV, string dataFV, string opisUsterki, string numerZKabla, string jakDlugo, string typPojazdu, string napiecie, string montaz1, string montaz2)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Formularze", "Strands.pdf");
            string nazwaFolderuZgloszenia = nrZgloszenia.Replace("/", ".");
            string folderZgloszenia = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane", nazwaFolderuZgloszenia);
            if (!Directory.Exists(folderZgloszenia)) Directory.CreateDirectory(folderZgloszenia);
            string outputPath = Path.Combine(folderZgloszenia, $"Zgloszenie_Strands_{nazwaFolderuZgloszenia}.pdf");

            try
            {
                PdfDocument document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
                if (document.AcroForm == null) return null;
                document.AcroForm.Elements.SetValue("/NeedAppearances", new PdfBoolean(true));

                SetAndLockField(document, "Data", DateTime.Now.ToString("dd.MM.yyyy"));
                SetAndLockField(document, "Nrkabla", numerZKabla);
                SetAndLockField(document, "Nrartykulu", kodProduktu);
                SetAndLockField(document, "Zglaszajacy", Program.fullName);
                SetAndLockField(document, "Nrfaktury", $"{numerFV} of {dataFV}");
                SetAndLockField(document, "Wada", $"{nrZgloszenia} {opisUsterki}");
                SetAndLockField(document, "Jakdlugo", jakDlugo);
                SetAndLockField(document, "Typ", typPojazdu);
                SetAndLockField(document, "V", napiecie);
                SetAndLockField(document, "gdzie", montaz1);
                SetAndLockField(document, "gdzie2", montaz2);

                document.Save(outputPath);
                return outputPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd generowania PDF dla Strands: {ex.Message}");
                return null;
            }
        }

        private void SetAndLockField(PdfDocument document, string fieldName, string value)
        {
            if (document.AcroForm.Fields.Names.Contains(fieldName))
            {
                if (document.AcroForm.Fields[fieldName] is PdfTextField field)
                {
                    field.Value = new PdfString(value);
                    field.ReadOnly = true;
                }
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
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas tworzenia e-maila: " + ex.Message);
            }
        }
        #endregion

        #region Puste metody dla projektanta (zaślepki)
        private void label2_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Form7_Load(object sender, EventArgs e) { }
        #endregion
    
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
