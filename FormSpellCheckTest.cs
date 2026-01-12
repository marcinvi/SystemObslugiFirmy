using System;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Formularz testowy sprawdzania pisowni
    /// </summary>
    public partial class FormSpellCheckTest : Form
    {
        public FormSpellCheckTest()
        {
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // 
            // FormSpellCheckTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "FormSpellCheckTest";
            this.Text = "Test sprawdzania pisowni";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Panel górny z przyciskami
            var panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(10)
            };

            var btnAddToAll = new Button
            {
                Text = "Dodaj sprawdzanie pisowni do wszystkich formularzy",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(400, 35),
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold)
            };
            btnAddToAll.Click += BtnAddToAll_Click;

            var btnTest = new Button
            {
                Text = "Test sprawdzania pisowni",
                Location = new System.Drawing.Point(420, 10),
                Size = new System.Drawing.Size(200, 35),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            btnTest.Click += BtnTest_Click;

            var btnViewDictionary = new Button
            {
                Text = "Pokaż słownik własny",
                Location = new System.Drawing.Point(10, 55),
                Size = new System.Drawing.Size(200, 35),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            btnViewDictionary.Click += BtnViewDictionary_Click;

            panelTop.Controls.Add(btnAddToAll);
            panelTop.Controls.Add(btnTest);
            panelTop.Controls.Add(btnViewDictionary);

            // Panel informacyjny
            var lblInfo = new Label
            {
                Dock = DockStyle.Fill,
                Text = "INSTRUKCJA UŻYCIA:\n\n" +
                       "1. Kliknij 'Dodaj sprawdzanie pisowni do wszystkich formularzy' aby automatycznie\n" +
                       "   dodać sprawdzanie pisowni do wszystkich istniejących formularzy w projekcie.\n\n" +
                       "2. Po dodaniu, przebuduj projekt (Build -> Rebuild Solution).\n\n" +
                       "3. Wszystkie TextBoxy i RichTextBoxy będą miały sprawdzanie pisowni po polsku.\n\n" +
                       "FUNKCJE:\n" +
                       "- Automatyczne podkreślanie błędów na czerwono (tylko RichTextBox)\n" +
                       "- Menu kontekstowe PPM z sugestiami poprawek\n" +
                       "- Możliwość dodawania słów do własnego słownika\n" +
                       "- Słownik własny zapisywany w pliku custom_dictionary.txt\n\n" +
                       "RĘCZNE UŻYCIE:\n" +
                       "W dowolnym formularzu możesz dodać sprawdzanie pisowni ręcznie:\n" +
                       "- W konstruktorze formularza wywołaj: textBox1.EnableSpellCheck(true);\n" +
                       "- Lub użyj SpellCheckRichTextBox zamiast RichTextBox\n" +
                       "- Lub użyj SpellCheckTextBox zamiast TextBox",
                Padding = new Padding(20),
                Font = new System.Drawing.Font("Segoe UI", 9.5F),
                AutoSize = false
            };

            this.Controls.Add(lblInfo);
            this.Controls.Add(panelTop);

            this.ResumeLayout(false);
        }

        private void BtnAddToAll_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Ta operacja zmodyfikuje wszystkie pliki formularzy (.cs) w projekcie.\n\n" +
                "Zalecane jest wcześniejsze stworzenie kopii zapasowej projektu.\n\n" +
                "Czy kontynuować?",
                "Ostrzeżenie",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    var injector = new SpellCheckInjector(Application.StartupPath.Replace("\\bin\\Debug", "").Replace("\\bin\\Release", ""));
                    injector.ProcessAllForms();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            var testForm = new Form
            {
                Text = "Test sprawdzania pisowni",
                Size = new System.Drawing.Size(600, 400),
                StartPosition = FormStartPosition.CenterScreen
            };

            var richTextBox = new SpellCheckRichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Text = "Witm w testwoym programie.\n\n" +
                       "To jest tekst z błendami pisowni. Sprawdznik powinien podkrślić błedne słowa na czewrwono.\n\n" +
                       "Kliknij prawym przyciskiem myszy na podkreślone słowo, aby zobaczyć sugestie poprawek.\n\n" +
                       "Możesz również dodać słowo do słownika własnego."
            };

            testForm.Controls.Add(richTextBox);
            testForm.ShowDialog();
        }

        private void BtnViewDictionary_Click(object sender, EventArgs e)
        {
            try
            {
                string dictionaryPath = System.IO.Path.Combine(Application.StartupPath, "custom_dictionary.txt");
                
                if (!System.IO.File.Exists(dictionaryPath))
                {
                    MessageBox.Show("Słownik własny jest pusty.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var words = System.IO.File.ReadAllLines(dictionaryPath);
                
                var viewForm = new Form
                {
                    Text = "Słownik własny",
                    Size = new System.Drawing.Size(400, 500),
                    StartPosition = FormStartPosition.CenterScreen
                };

                var listBox = new ListBox
                {
                    Dock = DockStyle.Fill,
                    Font = new System.Drawing.Font("Segoe UI", 10F)
                };
                listBox.Items.AddRange(words);

                var btnClear = new Button
                {
                    Text = "Wyczyść słownik",
                    Dock = DockStyle.Bottom,
                    Height = 40
                };
                btnClear.Click += (s, ev) =>
                {
                    var result = MessageBox.Show("Czy na pewno chcesz wyczyścić cały słownik własny?", 
                        "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        System.IO.File.Delete(dictionaryPath);
                        viewForm.Close();
                        MessageBox.Show("Słownik został wyczyszczony.", "Informacja", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                };

                viewForm.Controls.Add(listBox);
                viewForm.Controls.Add(btnClear);
                viewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
