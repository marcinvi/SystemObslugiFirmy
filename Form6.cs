using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using OutlookApp = Microsoft.Office.Interop.Outlook;

namespace Reklamacje_Dane
{
    public partial class Form6 : Form
    {
        private string nrZgloszenia;
        private const string OpcjaProducentIdentifier = "PRODUCENT_PLACEHOLDER"; // Identyfikator dla opcji producenta

        // Klasa pomocnicza do przechowywania danych serwisanta
        public class Serwisant
        {
            public string Nazwa { get; set; }
            public string Adres { get; set; }
            public string AdresEmail { get; set; }
            public override string ToString() => Nazwa;
        }

        public Form6(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form6_Load(object sender, EventArgs e)
        {
            await WybierzSerwisantaLubProducenta();
        }

        private async Task WybierzSerwisantaLubProducenta()
        {
            var serwisanci = await PobierzListeSerwisantowAsync();

            // Dodajemy na stałe opcję wysłania maila do producenta na początku listy
            serwisanci.Insert(0, new Serwisant
            {
                Nazwa = ">> Napisz do producenta <<",
                AdresEmail = OpcjaProducentIdentifier // Używamy identyfikatora
            });

            listBoxSerwisanci.DataSource = serwisanci;
            listBoxSerwisanci.DisplayMember = "Nazwa";
        }

        private async Task<List<Serwisant>> PobierzListeSerwisantowAsync()
        {
            var serwisanci = new List<Serwisant>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "SELECT Nazwa, Adres, Email FROM Serwisy ORDER BY Nazwa";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                serwisanci.Add(new Serwisant
                                {
                                    Nazwa = reader["Nazwa"].ToString(),
                                    Adres = reader["Adres"].ToString(),
                                    AdresEmail = reader["Email"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania listy serwisantów: " + ex.Message);
            }
            return serwisanci;
        }

        private async void buttonWybierz_Click(object sender, EventArgs e)
        {
            if (listBoxSerwisanci.SelectedItem is Serwisant wybranaOpcja)
            {
                // Sprawdzamy, czy wybrano opcję "Napisz do producenta"
                if (wybranaOpcja.AdresEmail == OpcjaProducentIdentifier)
                {
                    // Otwieramy Form10
                    var form10 = new Form10(this.nrZgloszenia);
                    this.Hide(); // Ukrywamy bieżący formularz
                    form10.ShowDialog();
                    this.Close(); // Zamykamy po powrocie z Form10
                }
                else
                {
                    // Jeśli wybrano serwisanta, kontynuujemy starą logikę
                    await WyslijEmailDoSerwisanta(wybranaOpcja);
                }
            }
            else
            {
                MessageBox.Show("Proszę wybrać opcję z listy.");
            }
        }

        private async Task WyslijEmailDoSerwisanta(Serwisant serwisant)
        {
            string sciezkaPDF = await GenerujPDFAsync(serwisant);
            if (string.IsNullOrEmpty(sciezkaPDF))
            {
                MessageBox.Show("Nie udało się wygenerować pliku PDF.");
                return;
            }

            string temat = $"Zlecenie serwisowe do zgłoszenia nr: {nrZgloszenia}";
            string tresc = $"Cześć,\n\nW załączniku przesyłam formularz zlecenia serwisowego.\n\nPozdrawiam,\n{Program.fullName}";

            WyslijEmailPrzezOutlook(serwisant.AdresEmail, temat, tresc, sciezkaPDF);
        }

        private async Task<string> GenerujPDFAsync(Serwisant wybranySerwisant)
        {
            // 1. Pobierz potrzebne dane o zgłoszeniu z bazy
            string dataZgloszenia = "", produktInfo = "", nrSeryjny = "", usterka = "";
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = @"SELECT z.DataZgloszenia, p.NazwaKrotka, z.NrSeryjny, z.OpisUsterki 
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
                                dataZgloszenia = reader["DataZgloszenia"].ToString();
                                produktInfo = reader["NazwaKrotka"].ToString();
                                nrSeryjny = reader["NrSeryjny"].ToString();
                                usterka = reader["OpisUsterki"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania danych do PDF: " + ex.Message);
                return null;
            }

            // 2. Generowanie PDF (ta logika pozostaje podobna)
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Formularze", "lodowka.pdf");
            string nazwaFolderuZgloszenia = nrZgloszenia.Replace("/", ".");
            string folderZgloszenia = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane", nazwaFolderuZgloszenia);
            if (!Directory.Exists(folderZgloszenia)) Directory.CreateDirectory(folderZgloszenia);
            string outputPath = Path.Combine(folderZgloszenia, $"Zlecenie_serwisowe_{nazwaFolderuZgloszenia}.pdf");

            try
            {
                PdfDocument document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
                if (document.AcroForm == null) return null;
                document.AcroForm.Elements.SetValue("/NeedAppearances", new PdfBoolean(true));

                SetAndLockField(document, "NrZgloszenia", $"Zgłoszenie nr: {nrZgloszenia}");
                SetAndLockField(document, "Data", $"Z dnia: {dataZgloszenia}");
                SetAndLockField(document, "Produkt", $"{produktInfo} SN: {nrSeryjny}");
                SetAndLockField(document, "Wada", usterka);
                SetAndLockField(document, "Serwisant", wybranySerwisant.Adres);
                SetAndLockField(document, "data", DateTime.Now.ToString("dd.MM.yyyy"));
                SetAndLockField(document, "Podpis", Program.fullName);

                document.Save(outputPath);
                return outputPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd generowania PDF: {ex.Message}");
                return null;
            }
        }

        private void SetAndLockField(PdfDocument document, string fieldName, string value)
        {
            // POPRAWKA: Użycie Fields.Names.Contains() zamiast Fields.Contains()
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
            finally
            {
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
