using System;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Sheets.v4.Data; // Ten using jest zbędny, ale na razie go zostawiam, by nie powodować błędów w innych, niezmienionych plikach.
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using OutlookApp = Microsoft.Office.Interop.Outlook;

namespace Reklamacje_Dane
{
    public partial class Form7 : Form
    {
        private string nrZgloszenia;

        public Form7(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form7_Load(object sender, EventArgs e)
        {
            await InicjalizujFormularzAsync();
        }

        private async Task InicjalizujFormularzAsync()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = @"SELECT p.KodProducenta, z.OpisUsterki, z.NrFaktury, z.DataZakupu 
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

                                if (DateTime.TryParse(reader["DataZakupu"].ToString(), out DateTime dataZakupu))
                                {
                                    dateTimePickerDataFVZakupowej.Value = dataZakupu;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania danych do formularza Hella: " + ex.Message);
            }
        }

        private async void buttonWyslij_Click(object sender, EventArgs e)
        {
            string kodProduktu = textBoxKodProduktuHella.Text;
            string numerFV = textBoxNumerFVZakupowej.Text;
            string dataFV = dateTimePickerDataFVZakupowej.Value.ToString("dd.MM.yyyy");
            string opisUsterki = textBoxOpisUsterki.Text;
            string okolicznosci = textBoxOkolicznoscWady.Text;
            string uwagi = textBoxUwagi.Text;

            string sciezkaPDF = GenerujPDF(kodProduktu, numerFV, dataFV, opisUsterki, okolicznosci, uwagi);
            if (string.IsNullOrEmpty(sciezkaPDF))
            {
                MessageBox.Show("Nie udało się wygenerować pliku PDF.");
                return;
            }

            string adresOdbiorcy = "marek.kowalski@forvia.com; marcin.zajac@forvia.com";
            string temat = $"Zgłoszenie reklamacyjne nr: {nrZgloszenia}";
            string tresc = $"Cześć,\n\nW załączniku wysyłam formularz reklamacyjny dotyczący: {kodProduktu}.\nKlient zgłasza: {opisUsterki}.\n\nPozdrawiam.";

            WyslijEmailPrzezOutlook(adresOdbiorcy, temat, tresc, sciezkaPDF);

            // Aktualizacja statusu w bazie danych
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
                await dziennik.DodajAsync(Program.fullName, "Zgłoszono do Hella", this.nrZgloszenia);
                var dzialanie = new Dzialaniee();
                dzialanie.DodajNoweDzialanie(this.nrZgloszenia, Program.fullName, $"Zgłoszono do Hella");
                UpdateManager.NotifySubscribers();
                MessageBox.Show("Wiadomość e-mail została przygotowana. Zgłoszenie zaktualizowane.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas aktualizacji statusu zgłoszenia: " + ex.Message);
            }
        }

        #region --- Metody pomocnicze (PDF i E-mail) ---

        private string GenerujPDF(string kodProduktu, string numerFV, string dataFV, string opisUsterki, string okolicznosci, string uwagi)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Formularze", "Hella.pdf");
            string nazwaFolderuZgloszenia = nrZgloszenia.Replace("/", ".");
            string folderZgloszenia = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane", nazwaFolderuZgloszenia);
            if (!Directory.Exists(folderZgloszenia)) Directory.CreateDirectory(folderZgloszenia);

            string outputPath = Path.Combine(folderZgloszenia, $"Zgloszenie_Hella_{nazwaFolderuZgloszenia}.pdf");

            try
            {
                PdfDocument document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
                if (document.AcroForm == null) return null;
                document.AcroForm.Elements.SetValue("/NeedAppearances", new PdfBoolean(true));

                SetAndLockField(document, "NrZgloszenia", nrZgloszenia);
                SetAndLockField(document, "DataZgloszenia", DateTime.Now.ToString("dd.MM.yyyy"));
                SetAndLockField(document, "NazwaHella", kodProduktu);
                SetAndLockField(document, "Datazakupu", $"FV: {numerFV} z dnia: {dataFV}");
                SetAndLockField(document, "Wada", opisUsterki);
                SetAndLockField(document, "Okolicznosc", okolicznosci);
                SetAndLockField(document, "Uwagi", uwagi);
                SetAndLockField(document, "Zglaszajacy", Program.fullName);
                SetAndLockField(document, "kontakt", "reklamacje@enatruck.com");

                document.Save(outputPath);
                return outputPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd generowania PDF dla Hella: {ex.Message}");
                return null;
            }
        }

        private void SetAndLockField(PdfDocument document, string fieldName, string value)
        {
            // POPRAWIONA WERSJA:
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
                OutlookApp.Application app = new OutlookApp.Application();
                OutlookApp.MailItem mail = (OutlookApp.MailItem)app.CreateItem(OutlookApp.OlItemType.olMailItem);

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