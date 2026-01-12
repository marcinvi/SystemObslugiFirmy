using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
namespace Reklamacje_Dane
{
    public partial class FormEmail : Form
    {
        public string KontaktMail { get; private set; }
        public string TrescWiadomosci { get; private set; }

        public FormEmail(string kontaktMail)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            textBoxKontaktMail.Text = kontaktMail;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void buttonWyslij_Click(object sender, EventArgs e)
        {
            // Przypisz wartość z TextBox do zmiennej
            TrescWiadomosci = textBoxTrescWiadomosci.Text;
            this.DialogResult = DialogResult.OK;



            this.Close();
        }





        private async void FormEmail_Load(object sender, EventArgs e)
        {
            await webView22.EnsureCoreWebView2Async();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // Sprawdź, czy WebView2 jest gotowe
            if (webView22.CoreWebView2 == null)
            {
                MessageBox.Show("WebView2 nie jest jeszcze gotowe. Spróbuj ponownie.");
                return;
            }
            webView22.Source = new Uri("https://www.deepl.com/pl/translator#pl/en-gb/" + textBoxTrescWiadomosci.Text);
            await Task.Delay(3000); // Poczekaj 5 sekund, aby strona mogła się załadować
            string script = @"
    (function() {
        let svgElement = document.querySelector('svg[data-name=""CopyToClipboardMedium""]');
        if (svgElement) {
            svgElement.dispatchEvent(new MouseEvent('click', { bubbles: true }));
            return 'Kliknięto element SVG!';
        }
        return 'Element SVG nie znaleziony!';
    })();
    ";

            // Wykonaj skrypt i odczekaj na wynik
            var result = await webView22.CoreWebView2.ExecuteScriptAsync(script);


            // Poczekaj chwilę, aby system zdążył skopiować tekst
            await Task.Delay(1000);

            // Wyczyść TextBox
            textBoxTrescWiadomosci.Clear();

            // Pobierz zawartość schowka i wklej do TextBox
            if (Clipboard.ContainsText())
            {
                textBoxTrescWiadomosci.Text = Clipboard.GetText();
            }
        }

        private void webView21_Click(object sender, EventArgs e)
        {

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
