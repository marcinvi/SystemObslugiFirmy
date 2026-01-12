using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormPrzypomnieniePopup : Form
    {
        private readonly int _reminderId;
        private readonly string _nrZgloszenia;
        private readonly PrzypomnieniaService _service;

        public static event Action OnActionTaken;

        public FormPrzypomnieniePopup(int id, string tresc, string nrZgl, string priorytet)
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine($"[POPUP] Tworzenie okna dla ID: {id}");
            _reminderId = id;
            _nrZgloszenia = nrZgl;
            _service = new PrzypomnieniaService(new DatabaseService(DatabaseHelper.GetConnectionString()));

            lblTresc.Text = tresc;
            lblInfo.Text = string.IsNullOrEmpty(nrZgl) ? "" : $"Dotyczy: {nrZgl}";
            if (string.IsNullOrEmpty(nrZgl)) btnOpen.Visible = false;

            // Kolory
            panelHeader.BackColor = (priorytet == "Wysoki" || tresc.Contains("PILNE")) ? Color.Crimson : Color.SteelBlue;
            lblTitle.Text = (priorytet == "Wysoki") ? "PILNE PRZYPOMNIENIE" : "PRZYPOMNIENIE";

            // Pozycjonowanie
            Rectangle wa = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(wa.Right - this.Width - 10, wa.Bottom);
            timerAnimacja.Start();

            // === KONFIGURACJA MENU PRZEKŁADANIA ===
            btnSnooze.Text = "Przełóż ▼"; // Zmieniamy tekst
            BuildSnoozeMenu();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            System.Diagnostics.Debug.WriteLine($"[POPUP] Okno ID: {_reminderId} pokazane na ekranie.");
        }

        private void BuildSnoozeMenu()
        {
            ctxSnooze.Items.Clear();
            ctxSnooze.Items.Add("Za 15 minut", null, async (s, e) => await SnoozeTime(TimeSpan.FromMinutes(15)));
            ctxSnooze.Items.Add("Za 1 godzinę", null, async (s, e) => await SnoozeTime(TimeSpan.FromHours(1)));
            ctxSnooze.Items.Add("Za 3 godziny", null, async (s, e) => await SnoozeTime(TimeSpan.FromHours(3)));
            ctxSnooze.Items.Add("Jutro rano (09:00)", null, async (s, e) => await SnoozeTomorrow());
            ctxSnooze.Items.Add(new ToolStripSeparator());
            ctxSnooze.Items.Add("Wybierz datę i czas...", null, (s, e) => OpenCalendar());
        }

        private void timerAnimacja_Tick(object sender, EventArgs e)
        {
            int targetY = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height - 10;
            if (this.Top > targetY) this.Top -= 5;
            else timerAnimacja.Stop();
        }

        private void btnCloseX_Click(object sender, EventArgs e) => this.Close();

        // Kliknięcie przycisku otwiera menu
        private void btnSnooze_Click(object sender, EventArgs e)
        {
            ctxSnooze.Show(btnSnooze, new Point(0, btnSnooze.Height));
        }

        // --- LOGIKA PRZEKŁADANIA ---

        private async Task SnoozeTime(TimeSpan ts)
        {
            await ExecuteSnooze(DateTime.Now.Add(ts));
        }

        private async Task SnoozeTomorrow()
        {
            // Jutro o 9:00
            DateTime jutro = DateTime.Now.AddDays(1).Date.AddHours(9);
            await ExecuteSnooze(jutro);
        }

        private void OpenCalendar()
        {
            // Zatrzymujemy to okno na wierzchu, ale otwieramy modal
            using (var form = new FormSnoozeWybor())
            {
                // Pozycjonujemy kalendarz obok popupu
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(this.Left - 50, this.Top - 50);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    _ = ExecuteSnooze(form.SelectedDate);
                }
            }
        }

        private async Task ExecuteSnooze(DateTime date)
        {
            try
            {
                await _service.SnoozeExactAsync(_reminderId, date, Program.fullName);
                OnActionTaken?.Invoke();
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }

        // --- POZOSTAŁE AKCJE ---

        private async void btnDone_Click(object sender, EventArgs e)
        {
            try
            {
                await _service.MarkAsDoneAsync(_reminderId, Program.fullName);
                OnActionTaken?.Invoke();
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_nrZgloszenia) && _nrZgloszenia.Contains("/"))
            {
                new Form2(_nrZgloszenia).Show();
            }
            this.Close();
        }

        // Cień pod oknem
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x20000;
                return cp;
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