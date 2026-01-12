using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormSnoozeWybor : Form
    {
        public DateTime SelectedDate { get; private set; }
        private DateTimePicker dtp;

        public FormSnoozeWybor()
        {
            // Jeśli plik Designer.cs istnieje, ta metoda jest w nim zdefiniowana.
            // Możemy ją zostawić (nic nie zepsuje), ale najważniejsze jest wywołanie naszej metody budującej.
            try { InitializeComponent(); } catch { }

            // Wywołujemy naszą metodę z unikalną nazwą
            BudujInterfejs();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // ZMIENIŁEM NAZWĘ TEJ METODY, ŻEBY NIE KOLIDOWAŁA Z DESIGNEREM
        private void BudujInterfejs()
        {
            this.Text = "Wybierz termin";
            this.Size = new Size(300, 180);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.ControlBox = false; // Bez X, żeby wymusić przyciski

            Label lbl = new Label { Text = "Przełóż przypomnienie na:", Top = 15, Left = 15, AutoSize = true, Font = new Font("Segoe UI", 10) };

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd.MM.yyyy HH:mm"; // Data i Czas
            dtp.Top = 45;
            dtp.Left = 15;
            dtp.Width = 250;
            dtp.Font = new Font("Segoe UI", 11);
            // Domyślnie jutro 9:00
            dtp.Value = DateTime.Now.AddDays(1).Date.AddHours(9);

            Button btnOk = new Button { Text = "Zatwierdź", DialogResult = DialogResult.OK, Top = 90, Left = 150, Width = 115, Height = 35, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Button btnCancel = new Button { Text = "Anuluj", DialogResult = DialogResult.Cancel, Top = 90, Left = 15, Width = 115, Height = 35, BackColor = Color.WhiteSmoke, FlatStyle = FlatStyle.Flat };

            btnOk.Click += (s, e) => { SelectedDate = dtp.Value; };

            this.Controls.Add(lbl);
            this.Controls.Add(dtp);
            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
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