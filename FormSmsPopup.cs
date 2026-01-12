using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient; // Upewnij się, że masz ten using do parametrów SQL
using System.Drawing;
using System.Threading.Tasks; // Potrzebne do Task
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormSmsPopup : Form
    {
        private readonly string _numer;
        private readonly string _tresc;
        private readonly DatabaseService _db;
        private readonly PhoneClient _client;

        private ComboBox cmbZgloszenia, cmbSzablony;
        private TextBox txtOdpowiedz;

        public FormSmsPopup(string numer, string tresc, DatabaseService db, PhoneClient client)
        {
            _numer = numer;
            _tresc = tresc;
            _db = db;
            _client = client;

            InitializeComponent_Manual();
            LoadDataAsync();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void LoadDataAsync()
        {
            try
            {
                // 1. Załaduj zgłoszenia powiązane z numerem telefonu (Domyślne zachowanie)
                DataTable dtZgl = await _db.PobierzZgloszeniaWgTelefonuAsync(_numer);
                WypelnijComboZgloszen(dtZgl);

                // 2. Załaduj szablony
                DataTable dtSzab = await _db.GetDataTableAsync("SELECT Nazwa, Tresc FROM SzablonySMS");
                cmbSzablony.DisplayMember = "Nazwa";
                foreach (DataRow r in dtSzab.Rows)
                {
                    cmbSzablony.Items.Add(new { Nazwa = r["Nazwa"].ToString(), Tresc = r["Tresc"].ToString() });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania danych: " + ex.Message);
            }
        }

        // Metoda pomocnicza do wypełniania listy (używana przy starcie i przy wyszukiwaniu)
        private void WypelnijComboZgloszen(DataTable dt)
        {
            cmbZgloszenia.Items.Clear();
            cmbZgloszenia.Items.Add("-- Nie przypisuj do zgłoszenia --");

            foreach (DataRow r in dt.Rows)
            {
                // Sprawdzamy czy kolumny istnieją, żeby uniknąć błędów przy różnych zapytaniach
                string nr = r.Table.Columns.Contains("NrZgloszenia") ? r["NrZgloszenia"].ToString() : "";
                string dane = "";

                if (r.Table.Columns.Contains("ImieNazwisko")) dane = r["ImieNazwisko"].ToString();
                else if (r.Table.Columns.Contains("NazwaFirmy")) dane = r["NazwaFirmy"].ToString();

                cmbZgloszenia.Items.Add($"{nr} - {dane}");
            }

            if (cmbZgloszenia.Items.Count > 1)
                cmbZgloszenia.SelectedIndex = 1; // Wybierz pierwsze znalezione
            else
                cmbZgloszenia.SelectedIndex = 0;
        }

        private void CmbSzablony_SelectedIndexChanged(object sender, EventArgs e)
        {
            dynamic selected = cmbSzablony.SelectedItem;
            if (selected != null && cmbSzablony.SelectedIndex > -1)
            {
                txtOdpowiedz.Text = selected.Tresc;
            }
        }

        private async void BtnSzukaj_Click(object sender, EventArgs e)
        {
            string fraza = ShowInputDialog("Podaj numer zgłoszenia, nazwisko lub nazwę firmy:", "Szukaj zgłoszenia");

            if (string.IsNullOrWhiteSpace(fraza)) return;

            try
            {
                string sql = "SELECT NrZgloszenia, ImieNazwisko FROM Zgloszenia WHERE NrZgloszenia LIKE @f OR ImieNazwisko LIKE @f OR NazwaFirmy LIKE @f ORDER BY Id DESC LIMIT 20";

                // Tworzymy parametr dla bezpiecznego zapytania
                var param = new MySqlParameter("@f", $"%{fraza}%");

                DataTable dt = await _db.GetDataTableAsync(sql, param);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Nie znaleziono zgłoszeń pasujących do frazy.");
                }
                else
                {
                    WypelnijComboZgloszen(dt);
                    MessageBox.Show($"Znaleziono {dt.Rows.Count} pasujących zgłoszeń.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wyszukiwania: " + ex.Message);
            }
        }

        private void BtnZobaczZgloszenie_Click(object sender, EventArgs e)
        {
            if (cmbZgloszenia.SelectedIndex <= 0) return;

            // Pobieramy numer z formatu "NR/2023 - Jan Kowalski"
            string nr = cmbZgloszenia.SelectedItem.ToString().Split('-')[0].Trim();

            // Otwieramy formularz podglądu
            new Form2(nr).Show();
        }

        private async void BtnWyslij_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOdpowiedz.Text)) return;

            try
            {
                bool sukces = await _client.SendSmsAsync(_numer, txtOdpowiedz.Text);
                if (sukces)
                {
                    await _db.ZapiszNowySmsAsync(_numer, txtOdpowiedz.Text, "Wysłane");

                    // Logika przypisania
                    if (cmbZgloszenia.SelectedIndex > 0)
                    {
                        string nrZgloszenia = cmbZgloszenia.SelectedItem.ToString().Split('-')[0].Trim();

                        // Tutaj wywołujemy przypisanie. 
                        // UWAGA: Metoda w DatabaseService musi obsługiwać przypisanie po NrZgloszenia, 
                        // a nie tylko szukać po numerze telefonu, skoro pozwalamy na ręczny wybór.
                        await _db.PrzypiszOstatniSmsDoZgloszeniaAsync(_numer, nrZgloszenia);
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wysyłka nie powiodła się (błąd bramki SMS).");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wysyłania: " + ex.Message);
            }
        }

        // Prosty InputDialog bez referencji do Microsoft.VisualBasic
        private string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, AutoSize = true };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340 };
            Button confirmation = new Button() { Text = "Szukaj", Left = 260, Width = 100, Top = 80, DialogResult = DialogResult.OK };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void InitializeComponent_Manual()
        {
            this.Text = "Centrum SMS - " + _numer;
            this.Size = new Size(550, 500);
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            Label l1 = new Label { Text = "Otrzymano:", Location = new Point(20, 10), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            TextBox t1 = new TextBox { Text = _tresc, Location = new Point(20, 30), Width = 490, Height = 60, Multiline = true, ReadOnly = true, BackColor = Color.LightYellow };

            Label l2 = new Label { Text = "Wybierz szablon:", Location = new Point(20, 100), AutoSize = true };
            cmbSzablony = new ComboBox { Location = new Point(20, 120), Width = 490, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSzablony.SelectedIndexChanged += CmbSzablony_SelectedIndexChanged;

            Label l3 = new Label { Text = "Twoja odpowiedź:", Location = new Point(20, 155), AutoSize = true };
            txtOdpowiedz = new TextBox { Location = new Point(20, 175), Width = 490, Height = 100, Multiline = true };

            // Sekcja przypisywania
            Label l4 = new Label { Text = "Przypisanie do reklamacji:", Location = new Point(20, 290), AutoSize = true };

            // Zmniejszona szerokość Combo, żeby zmieścić przycisk Szukaj
            cmbZgloszenia = new ComboBox { Location = new Point(20, 310), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };

            // Nowy przycisk SZUKAJ
            Button btnSearch = new Button { Text = "🔍 Szukaj", Location = new Point(310, 309), Width = 90, Height = 25, BackColor = Color.WhiteSmoke };
            btnSearch.Click += BtnSzukaj_Click;

            // Przycisk ZOBACZ
            Button btnLook = new Button { Text = "Zobacz", Location = new Point(410, 309), Width = 100, Height = 25 };
            btnLook.Click += BtnZobaczZgloszenie_Click;

            Button btnSend = new Button { Text = "WYŚLIJ SMS", Location = new Point(20, 370), Width = 490, Height = 50, BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSend.Click += BtnWyslij_Click;

            this.Controls.AddRange(new Control[] { l1, t1, l2, cmbSzablony, l3, txtOdpowiedz, l4, cmbZgloszenia, btnSearch, btnLook, btnSend });
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