using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public partial class FormSkrzynka : Form
    {
        private DataGridView gridMaile;
        private WebBrowser browserPodglad;
        private CheckBox chkTylkoNieprzypisane;
        private Button btnPobierz, btnPrzypisz;
        private Label lblStatus;

        private ContactRepository _repo = new ContactRepository();
        private EmailService _emailService = new EmailService();

        public FormSkrzynka()
        {
            InitializeComponent_Manual();
            OdswiezListe();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void OdswiezListe()
        {
            try
            {
                DataTable dt = _repo.PobierzMaileDoSkrzynki(chkTylkoNieprzypisane.Checked);
                gridMaile.DataSource = dt;

                // Ukrywamy kolumny techniczne
                if (gridMaile.Columns["Id"] != null) gridMaile.Columns["Id"].Visible = false;
                if (gridMaile.Columns["Tresc"] != null) gridMaile.Columns["Tresc"].Visible = false;
                if (gridMaile.Columns["Typ"] != null) gridMaile.Columns["Typ"].Visible = false;

                // Formatowanie szerokości
                if (gridMaile.Columns["Konto"] != null) gridMaile.Columns["Konto"].Width = 120; // NOWE
                if (gridMaile.Columns["DataWyslania"] != null) gridMaile.Columns["DataWyslania"].Width = 120;
                if (gridMaile.Columns["Nadawca"] != null) gridMaile.Columns["Nadawca"].Width = 200;
                if (gridMaile.Columns["NrZgloszenia"] != null) gridMaile.Columns["NrZgloszenia"].Width = 80;
                if (gridMaile.Columns["Tytul"] != null) gridMaile.Columns["Tytul"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd odświeżania: " + ex.Message);
            }
        }

        private async void BtnPobierz_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Pobieranie poczty z serwera... Proszę czekać.";
            btnPobierz.Enabled = false;

            try
            {
                await _emailService.PobierzPoczteDlaWszystkichKontAsync();
                OdswiezListe();
                lblStatus.Text = "Zakończono pobieranie.";
                MessageBox.Show("Poczta zaktualizowana!");
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Błąd pobierania.";
                MessageBox.Show("Błąd: " + ex.Message);
            }
            finally
            {
                btnPobierz.Enabled = true;
            }
        }

        private void GridMaile_SelectionChanged(object sender, EventArgs e)
        {
            if (gridMaile.SelectedRows.Count > 0)
            {
                var row = gridMaile.SelectedRows[0];
                string tresc = row.Cells["Tresc"].Value.ToString();

                // Wyświetl HTML w przeglądarce
                if (string.IsNullOrEmpty(tresc))
                    browserPodglad.DocumentText = "<html><body><i>Brak treści</i></body></html>";
                else
                    browserPodglad.DocumentText = tresc;
            }
        }

        private void BtnPrzypisz_Click(object sender, EventArgs e)
        {
            if (gridMaile.SelectedRows.Count == 0) return;

            // Pobieramy ID maila
            int mailId = Convert.ToInt32(gridMaile.SelectedRows[0].Cells["Id"].Value);

            // Prosty InputBox do wpisania numeru (można to zrobić ładniej osobnym oknem)
            string numer = Microsoft.VisualBasic.Interaction.InputBox("Podaj numer zgłoszenia (np. 55/2023):", "Przypisz do zgłoszenia");

            if (!string.IsNullOrEmpty(numer))
            {
                int? zglId = _repo.ZnajdzIdZgloszeniaPoNumerze(numer);
                if (zglId.HasValue)
                {
                    _repo.PrzypiszMailDoZgloszenia(mailId, zglId.Value);
                    MessageBox.Show($"Przypisano maila do zgłoszenia #{numer}");
                    OdswiezListe();
                }
                else
                {
                    MessageBox.Show("Nie znaleziono takiego zgłoszenia w bazie.");
                }
            }
        }

        // --- DESIGNER UI ---
        private void InitializeComponent_Manual()
        {
            this.Text = "Skrzynka Odbiorcza (Centrum Wiadomości)";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Panel górny (Przyciski)
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };

            btnPobierz = new Button { Text = "📥 Pobierz/Odśwież", Location = new Point(10, 10), Width = 150, Height = 40, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnPobierz.Click += BtnPobierz_Click;

            chkTylkoNieprzypisane = new CheckBox { Text = "Pokaż tylko nieprzypisane do zgłoszeń", Location = new Point(180, 20), AutoSize = true, Checked = true };
            chkTylkoNieprzypisane.CheckedChanged += (s, e) => OdswiezListe();

            btnPrzypisz = new Button { Text = "🔗 Przypisz do zgłoszenia", Location = new Point(450, 10), Width = 180, Height = 40 };
            btnPrzypisz.Click += BtnPrzypisz_Click;

            lblStatus = new Label { Text = "Gotowy", Location = new Point(650, 22), AutoSize = true, ForeColor = Color.Gray };

            panelTop.Controls.AddRange(new Control[] { btnPobierz, chkTylkoNieprzypisane, btnPrzypisz, lblStatus });

            // Split Container (Lista vs Podgląd)
            SplitContainer split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 400 };

            // Grid
            gridMaile = new DataGridView { Dock = DockStyle.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, ReadOnly = true, AllowUserToAddRows = false, BackgroundColor = Color.White };
            gridMaile.SelectionChanged += GridMaile_SelectionChanged;

            // Browser
            browserPodglad = new WebBrowser { Dock = DockStyle.Fill };

            split.Panel1.Controls.Add(gridMaile);
            split.Panel2.Controls.Add(browserPodglad);

            this.Controls.Add(split);
            this.Controls.Add(panelTop);
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