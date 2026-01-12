using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics; // Potrzebne do Process.Start

namespace Reklamacje_Dane
{
    public class FormPodgladEmail : Form
    {
        // Pola
        private ContactRepository _repo = new ContactRepository();
        private ListBox listZalaczniki;
        private WebBrowser browser;
        private string _messageId; // UID wiadomości (klucz do załączników)

        public FormPodgladEmail(string htmlContent, string messageId, string tytul = "Podgląd wiadomości")
        {
            // 1. Ustawienia Okna
            this.Text = tytul;
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            // POPRAWKA: Używamy SystemIcons.Application zamiast nieistniejącego Email
            this.Icon = SystemIcons.Application;

            _messageId = messageId;

            // 2. Tworzenie Panelu Bocznego (Załączniki)
            Panel panelBoczny = new Panel
            {
                Dock = DockStyle.Right,
                Width = 250,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };

            Label lblNaglowek = new Label
            {
                Text = "📎 Załączniki:",
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Label lblInfo = new Label
            {
                Text = "(Kliknij 2x aby otworzyć)",
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8, FontStyle.Italic)
            };

            listZalaczniki = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            listZalaczniki.DoubleClick += ListZalaczniki_DoubleClick;

            panelBoczny.Controls.Add(listZalaczniki);
            panelBoczny.Controls.Add(lblInfo);
            panelBoczny.Controls.Add(lblNaglowek);

            // 3. Tworzenie Przeglądarki (Treść)
            browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true // Ukrywa błędy skryptów JS w mailach
            };

            // Obsługa maili bez struktury HTML (samego tekstu)
            if (!string.IsNullOrEmpty(htmlContent))
            {
                if (!htmlContent.Contains("<body") && !htmlContent.Contains("<div"))
                {
                    // Ubieramy zwykły tekst w ładny font
                    browser.DocumentText = $"<html><body style='font-family: Segoe UI, Arial, sans-serif; font-size: 14px;'>{htmlContent}</body></html>";
                }
                else
                {
                    browser.DocumentText = htmlContent;
                }
            }
            else
            {
                browser.DocumentText = "<html><body><i>(Brak treści wiadomości)</i></body></html>";
            }

            // 4. Dodawanie do formularza
            this.Controls.Add(browser);      // Wypełni środek
            this.Controls.Add(panelBoczny);  // Doklei się do prawej

            // 5. Ładowanie danych
            ZaladujZalaczniki();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void ZaladujZalaczniki()
        {
            try
            {
                // Pobieramy listę z bazy (tylko nazwy, bez treści)
                DataTable dt = _repo.PobierzListeZalacznikow(_messageId);
                listZalaczniki.Items.Clear();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string nazwaOrg = row["NazwaPliku"].ToString();
                        string nazwaDysk = row["NazwaNaDysku"].ToString();

                        // Dodajemy obiekt pomocniczy do listy
                        listZalaczniki.Items.Add(new ZalacznikItem
                        {
                            NazwaWyswietlana = nazwaOrg,
                            NazwaPlikuNaDysku = nazwaDysk
                        });
                    }
                }
                else
                {
                    listZalaczniki.Items.Add("(Brak załączników)");
                    listZalaczniki.Enabled = false; // Szarzymy listę
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania załączników: " + ex.Message);
            }
        }

        private void ListZalaczniki_DoubleClick(object sender, EventArgs e)
        {
            // Sprawdzamy, czy kliknięto w poprawny element (nie w puste pole)
            if (listZalaczniki.SelectedItem is ZalacznikItem item)
            {
                try
                {
                    // Budujemy pełną ścieżkę do folderu z programem + Zalaczniki
                    string folderZalacznikow = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zalaczniki");
                    string pelnaSciezka = Path.Combine(folderZalacznikow, item.NazwaPlikuNaDysku);

                    if (File.Exists(pelnaSciezka))
                    {
                        // Uruchamiamy plik w domyślnym programie systemu Windows
                        Process.Start(pelnaSciezka);
                    }
                    else
                    {
                        MessageBox.Show($"Nie znaleziono pliku na dysku!\nSzukano: {pelnaSciezka}\n\nByć może został usunięty ręcznie.", "Błąd pliku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie udało się otworzyć pliku: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Klasa pomocnicza do przechowywania danych w ListBox
        private class ZalacznikItem
        {
            public string NazwaWyswietlana { get; set; }   // To co widzi użytkownik (np. "Faktura.pdf")
            public string NazwaPlikuNaDysku { get; set; }  // To co jest na dysku (np. "guid_Faktura.pdf")

            // ListBox wyświetla to, co zwraca metoda ToString()
            public override string ToString()
            {
                return "📄 " + NazwaWyswietlana;
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