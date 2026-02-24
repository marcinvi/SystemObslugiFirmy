using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormPrzypomnienia : Form
    {
        private readonly PrzypomnieniaService _service;

        public FormPrzypomnienia()
        {
            InitializeComponent();
            _service = new PrzypomnieniaService(new DatabaseService(DatabaseHelper.GetConnectionString()));

            // Konfiguracja filtrów 
            cmbFiltr.Items.Clear();
            cmbFiltr.Items.AddRange(new object[] {
                "Wszystkie aktywne",
                "Moje aktywne",
                "Zaległe",
                "Zrealizowane dzisiaj"
            });
            cmbFiltr.SelectedIndex = 0;

            this.Load += async (s, e) => await LoadDataAsync();
            this.btnOdswiez.Click += async (s, e) => await LoadDataAsync();
            this.cmbFiltr.SelectedIndexChanged += async (s, e) => await LoadDataAsync();
            this.btnDodajNowe.Click += btnDodajNowe_Click;

            // Upewnij się, że panel się skaluje wraz z oknem
            if (flowLayoutZadania != null)
            {
                this.Resize += (s, e) => {
                    flowLayoutZadania.SuspendLayout();
                    foreach (Control c in flowLayoutZadania.Controls)
                    {
                        c.Width = flowLayoutZadania.ClientSize.Width - 25;
                    }
                    flowLayoutZadania.ResumeLayout(true);
                };
            }

            EnableSpellCheckOnAllTextBoxes();
        }

        private async Task LoadDataAsync()
        {
            if (flowLayoutZadania == null) return;

            try
            {
                flowLayoutZadania.SuspendLayout();
                flowLayoutZadania.Controls.Clear();

                string filter = cmbFiltr.SelectedItem?.ToString();
                var dt = await _service.GetRemindersAsync(filter, Program.fullName);

                if (dt == null || dt.Rows.Count == 0)
                {
                    Label lblBrak = new Label
                    {
                        Text = "Super! Nie masz obecnie żadnych zadań do wykonania na tej liście 🎉",
                        Font = new Font("Segoe UI", 12, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        AutoSize = false,
                        Width = flowLayoutZadania.Width - 30,
                        Height = 100,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Margin = new Padding(10, 50, 0, 0)
                    };
                    flowLayoutZadania.Controls.Add(lblBrak);
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    flowLayoutZadania.Controls.Add(CreateReminderCard(row));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania zadań: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                flowLayoutZadania.ResumeLayout(true);
            }
        }

        private Panel CreateReminderCard(DataRow row)
        {
            int id = Convert.ToInt32(row["Id"]);
            string tresc = row["Tresc"].ToString();
            string nrZgloszenia = row["DotyczyZgloszenia"] != DBNull.Value ? row["DotyczyZgloszenia"].ToString() : "Brak przypisanego zgłoszenia";
            string dlaKogo = row["PrzypisanyUzytkownik"] != DBNull.Value ? row["PrzypisanyUzytkownik"].ToString() : "Wszyscy";
            string status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Nowe";

            DateTime termin = DateTime.MaxValue;
            if (row["DataPrzypomnienia"] != DBNull.Value)
            {
                DateTime.TryParse(row["DataPrzypomnienia"].ToString(), out termin);
            }

            bool isOverdue = termin < DateTime.Now && status != "Completed";
            bool isCompleted = status == "Completed";
            bool isUrgent = tresc.Contains("[PILNE]") || tresc.Contains("[PROBLEM DPD]");

            // GŁÓWNY KONTENER KARTY
            Panel pnlCard = new Panel
            {
                Width = flowLayoutZadania.ClientSize.Width - 25,
                Height = 110,
                Margin = new Padding(10, 5, 10, 5),
                BackColor = isCompleted ? Color.FromArgb(245, 245, 245) : Color.White,
                Cursor = Cursors.Hand
            };

            // Pasek boczny oznaczający priorytet/status
            Panel pnlSide = new Panel
            {
                Width = 8,
                Dock = DockStyle.Left,
                BackColor = isCompleted ? Color.LightGray : (isOverdue || isUrgent) ? Color.Crimson : Color.DodgerBlue
            };
            pnlCard.Controls.Add(pnlSide);

            // GŁÓWNY TYTUŁ ZADANIA
            Label lblTresc = new Label
            {
                Text = tresc,
                Font = new Font("Segoe UI", 11, isCompleted ? FontStyle.Strikeout : FontStyle.Bold),
                ForeColor = isCompleted ? Color.Gray : Color.Black,
                AutoSize = false,
                Location = new Point(20, 10),
                Width = pnlCard.Width - 180,
                Height = 45
            };

            // PODTYTUŁ (Zgłoszenie + Dla kogo)
            Label lblSubTitle = new Label
            {
                Text = $"📄 Zgłoszenie: {nrZgloszenia}   👤 Wykonuje: {dlaKogo}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(20, lblTresc.Bottom)
            };

            // TERMIN WYKONANIA
            string terminTekst = termin == DateTime.MaxValue ? "Brak terminu" : termin.ToString("dd.MM.yyyy HH:mm");
            if (isOverdue) terminTekst += " (Zaległe!)";

            Label lblTermin = new Label
            {
                Text = $"⏰ Termin: {terminTekst}",
                Font = new Font("Segoe UI", 9, isOverdue ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isCompleted ? Color.Gray : isOverdue ? Color.Red : Color.DarkGreen,
                AutoSize = true,
                Location = new Point(20, lblSubTitle.Bottom + 5)
            };

            // PRZYCISKI AKCJI (widoczne tylko jeśli zadanie nie jest zrealizowane)
            if (!isCompleted)
            {
                Button btnDone = new Button
                {
                    Text = "✅ Zrealizuj",
                    BackColor = Color.FromArgb(232, 245, 233), // Jasnozielony
                    ForeColor = Color.DarkGreen,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(110, 35),
                    Location = new Point(pnlCard.Width - 130, 15),
                    Cursor = Cursors.Hand
                };
                btnDone.FlatAppearance.BorderSize = 0;
                btnDone.Click += async (s, e) => {
                    if (MessageBox.Show("Zakończyć to zadanie?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        await _service.MarkAsDoneAsync(id, Program.fullName);
                        await LoadDataAsync();
                    }
                };

                Button btnSnooze = new Button
                {
                    Text = "⏳ Przełóż",
                    BackColor = Color.FromArgb(255, 243, 224), // Jasnopomarańczowy
                    ForeColor = Color.DarkOrange,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(110, 30),
                    Location = new Point(pnlCard.Width - 130, 55),
                    Cursor = Cursors.Hand
                };
                btnSnooze.FlatAppearance.BorderSize = 0;
                btnSnooze.Click += async (s, e) => {
                    // Domyślnie odkłada o 1 dzień. Jeśli chcesz dokładniejszego wyboru, można tu podpiąć FormSnoozeWybor
                    await _service.SnoozeAsync(id, 1, Program.fullName);
                    await LoadDataAsync();
                };

                pnlCard.Controls.Add(btnDone);
                pnlCard.Controls.Add(btnSnooze);
            }

            // Obsługa kliknięcia w kartę -> otwiera zgłoszenie
            EventHandler openTicket = (s, e) => {
                if (!string.IsNullOrEmpty(nrZgloszenia) && nrZgloszenia.Contains("/"))
                {
                    new Form2(nrZgloszenia).Show();
                }
            };

            pnlCard.Click += openTicket;
            lblTresc.Click += openTicket;
            lblSubTitle.Click += openTicket;
            lblTermin.Click += openTicket;

            // Dodajemy kontrolki do karty
            pnlCard.Controls.Add(lblTresc);
            pnlCard.Controls.Add(lblSubTitle);
            pnlCard.Controls.Add(lblTermin);

            // Ramka dolna (dla oddzielenia kart, jeśli tło formularza jest białe)
            Panel pnlBorder = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.WhiteSmoke };
            pnlCard.Controls.Add(pnlBorder);

            return pnlCard;
        }

        private async void btnDodajNowe_Click(object sender, EventArgs e)
        {
            using (var form = new FormDodajPrzypomnienie())
            {
                if (form.ShowDialog(this) == DialogResult.OK) await LoadDataAsync();
            }
        }

        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
            }
        }

        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;
                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control)) yield return child;
                }
            }
        }
    }
}