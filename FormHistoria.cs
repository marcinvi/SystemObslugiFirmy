using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Reklamacje_Dane; // Twój namespace

namespace Reklamacje_Dane
{
    public partial class FormHistoria : Form
    {
        public FormHistoria()
        {
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Ustawienia okna
            this.Text = "Centrum Kontaktu i Historii";
            this.Size = new System.Drawing.Size(1300, 850);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tworzymy Twoją kontrolkę
            ContactHistoryControl historiaCtrl = new ContactHistoryControl();

            // Dokujemy ją, żeby wypełniła całe okno
            historiaCtrl.Dock = DockStyle.Fill;

            // Dodajemy kontrolkę do formularza
            this.Controls.Add(historiaCtrl);

            this.ResumeLayout(false);
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