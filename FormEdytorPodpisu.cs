using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormEdytorPodpisu : Form
    {
        // Właściwość do pobrania wyniku przez główne okno
        public string WynikowyHtml { get; private set; }

        public FormEdytorPodpisu(string obecnyPodpis)
        {
            InitializeComponent(); // To wywołuje kod z pliku Designer.cs

            // Ustawiamy domyślny szablon, jeśli podpis jest pusty
            if (string.IsNullOrWhiteSpace(obecnyPodpis))
            {
                txtHtml.Text = "<div style='font-family: Arial; font-size: 14px; color: #333;'>\r\n" +
                               "  <br>--<br>\r\n" +
                               "  <b>{{PracownikImie}}</b><br>\r\n" +
                               "  Specjalista ds. Reklamacji<br>\r\n" +
                               "  <a href='mailto:{{PracownikEmail}}' style='color: #0056b3;'>{{PracownikEmail}}</a>\r\n" +
                               "</div>";
            }
            else
            {
                txtHtml.Text = obecnyPodpis;
            }

            OdswiezPodglad();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // --- LOGIKA BIZNESOWA ---

        private void TxtHtml_TextChanged(object sender, EventArgs e)
        {
            OdswiezPodglad();
        }

        private void OdswiezPodglad()
        {
            if (browserPreview != null)
            {
                // Pobieramy dane zalogowanego użytkownika (lub domyślne, jeśli puste)
                // Upewnij się, że w Program.cs masz pola public static fullName i currentUserEmail
                string mojeImie = !string.IsNullOrEmpty(Program.fullName) ? Program.fullName : "Jan Kowalski (Brak danych)";
                string mojEmail = !string.IsNullOrEmpty(Program.currentUserEmail) ? Program.currentUserEmail : "email@firma.pl";

                // W podglądzie zamieniamy zmienne na KONKRETY, żebyś widział efekt
                // Ale w txtHtml (kodzie) zmienne {{...}} zostają!
                string podglad = txtHtml.Text
                    .Replace("{{PracownikImie}}", mojeImie)
                    .Replace("{{PracownikEmail}}", mojEmail);

                browserPreview.DocumentText = podglad;
            }
        }

        private void BtnZapisz_Click(object sender, EventArgs e)
        {
            WynikowyHtml = txtHtml.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnAnuluj_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // --- POMOCNICY EDYCJI ---

        private void BtnWstawPracownika_Click(object sender, EventArgs e)
        {
            WstawWMejsceKursora("{{PracownikImie}}");
        }

        private void BtnWstawEmail_Click(object sender, EventArgs e)
        {
            WstawWMejsceKursora("{{PracownikEmail}}");
        }

        private void BtnWstawSzablon_Click(object sender, EventArgs e)
        {
            txtHtml.AppendText("\r\n<br><b>EnaTruck Sp. z o.o.</b><br>ul. Przykładowa 1");
        }

        private void BtnWstawObrazek_Click(object sender, EventArgs e)
        {
            txtHtml.AppendText("\r\n<br><img src='https://www.enatruck.pl/gfx/local/top.png' width='150'>");
        }

        private void WstawWMejsceKursora(string tekst)
        {
            int selectionIndex = txtHtml.SelectionStart;
            txtHtml.Text = txtHtml.Text.Insert(selectionIndex, tekst);
            txtHtml.SelectionStart = selectionIndex + tekst.Length;
            txtHtml.Focus();
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