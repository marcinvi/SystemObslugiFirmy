using System;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    // Enum definiujący co robimy ze sprzętem
    public enum DecyzjaMagazynowa
    {
        Anuluj,
        NaCzesci,    // Dawca
        Utylizacja,  // Złom
        NaStan,      // Pełnowartościowy
        BrakAkcji    // Został u klienta / Nie dotyczy
    }

    public partial class FormDecyzjaSprzet : Form
    {
        // Właściwość, którą odczytasz w Form12 po zamknięciu tego okna
        public DecyzjaMagazynowa WybranaDecyzja { get; private set; } = DecyzjaMagazynowa.Anuluj;

        public FormDecyzjaSprzet()
        {
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // --- Obsługa Przycisków ---

        private void btnCzesci_Click(object sender, EventArgs e)
        {
            WybranaDecyzja = DecyzjaMagazynowa.NaCzesci;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnStan_Click(object sender, EventArgs e)
        {
            WybranaDecyzja = DecyzjaMagazynowa.NaStan;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnUtylizacja_Click(object sender, EventArgs e)
        {
            WybranaDecyzja = DecyzjaMagazynowa.Utylizacja;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnIgnoruj_Click(object sender, EventArgs e)
        {
            WybranaDecyzja = DecyzjaMagazynowa.BrakAkcji;
            this.DialogResult = DialogResult.OK;
            this.Close();
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