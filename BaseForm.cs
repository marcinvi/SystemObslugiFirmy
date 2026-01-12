using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Bazowy formularz, z którego dziedziczą wszystkie inne formularze w aplikacji.
    /// Zapewnia spójny wygląd, czcionkę i zachowanie.
    /// </summary>
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();

            // Ustawienie domyślnych właściwości dla każdego formularza, który będzie dziedziczył z BaseForm
            this.Font = StyleGuide.RegularFont;
            this.BackColor = StyleGuide.FormBackground;
            this.ForeColor = StyleGuide.TextPrimary;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Ustaw ikonę aplikacji (upewnij się, że 'top.ico' jest w zasobach projektu)
            // Jeśli plik ikony nazywa się inaczej lub nie jest w zasobach,
            // Visual Studio może tu pokazać błąd - można go wtedy tymczasowo usunąć.
            // this.Icon = Properties.Resources.top;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
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