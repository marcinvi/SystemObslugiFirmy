using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Reklamacje_Dane
{
    public partial class FormComplaintProcess : Form
    {
        private readonly string _nrZgloszenia;
        private List<Panel> _stepPanels = new List<Panel>();

        public FormComplaintProcess(string numerZgloszenia)
        {
            InitializeComponent();
            _nrZgloszenia = numerZgloszenia;

            this.Load += FormComplaintProcess_Load;

            // Inicjalizacja paneli kroków
            _stepPanels.Add(step1_ReceptionPanel);
            _stepPanels.Add(step2_LogisticsPanel);
            // ... (tutaj dodamy kolejne panele)
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormComplaintProcess_Load(object sender, EventArgs e)
        {
            // Tutaj w przyszłości załadujemy wszystkie dane zgłoszenia
            lblComplaintNumber.Text = _nrZgloszenia;

            // Na razie symulujemy, że zaczynamy od kroku 1
            GoToStep(0);
            InitializeStepper();
        }

        private void InitializeStepper()
        {
            stepperPanel.Controls.Clear();

            // Tutaj w przyszłości stworzymy dynamicznie przyciski steppera
            // Na razie tworzymy je statycznie dla demonstracji
            var step1Button = CreateStepperButton("Weryfikacja", true);
            var step2Button = CreateStepperButton("Logistyka", false);
            var step3Button = CreateStepperButton("Serwis", false);
            var step4Button = CreateStepperButton("Decyzja", false);
            var step5Button = CreateStepperButton("Zamknięcie", false);

            stepperPanel.Controls.Add(step1Button);
            stepperPanel.Controls.Add(step2Button);
            stepperPanel.Controls.Add(step3Button);
            stepperPanel.Controls.Add(step4Button);
            stepperPanel.Controls.Add(step5Button);
        }

        private Button CreateStepperButton(string text, bool isActive)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Size = new Size(200, 45);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.Gainsboro;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.BackColor = isActive ? Color.FromArgb(46, 51, 73) : Color.Transparent;
            // Tutaj dodamy logikę do zmiany ikony (np. галочка) i obsługi kliknięć
            return btn;
        }

        private void GoToStep(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= _stepPanels.Count) return;

            // Ukryj wszystkie panele
            foreach (var panel in _stepPanels)
            {
                panel.Visible = false;
            }

            // Pokaż tylko aktywny panel
            _stepPanels[stepIndex].Visible = true;

            // Tutaj w przyszłości zaktualizujemy wygląd steppera
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