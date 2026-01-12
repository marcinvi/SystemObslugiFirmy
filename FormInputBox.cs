using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormInputBox : Form
    {
        public string NewFileName { get; private set; }

        public FormInputBox(string prompt, string originalFileName)
        {
            InitializeComponent();
            this.lblPrompt.Text = prompt;
            this.txtInput.Text = originalFileName;
            this.NewFileName = originalFileName;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Prosta walidacja, czy nazwa nie jest pusta i nie zawiera niedozwolonych znaków
            if (string.IsNullOrWhiteSpace(txtInput.Text) || txtInput.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                MessageBox.Show("Podana nazwa pliku jest nieprawidłowa.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.NewFileName = txtInput.Text;
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