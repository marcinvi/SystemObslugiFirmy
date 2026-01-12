using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormWprowadzNumer : Form
    {
        public string NumerPrzesylki { get; private set; }

        public FormWprowadzNumer()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void btnZatwierdz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumerListu.Text))
            {
                MessageBox.Show("Numer listu nie może być pusty.", "Błąd");
                return;
            }
            this.NumerPrzesylki = txtNumerListu.Text.Trim();
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