using System;
using System.Windows.Forms;

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
            SpellCheckManager.EnableSpellCheckForAllTextBoxes(this);
        }
    
    }
}
