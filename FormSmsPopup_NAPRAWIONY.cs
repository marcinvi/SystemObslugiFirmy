using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Threading.Tasks;
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
                else if (r.Table.Columns.Contains("Produkt")) dane = r["Produkt"].ToString(); // Dodane dla wyszukiwania

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
                // NAPRAWIONE: Dodano produkt i NazwaFirmy do zapytania
                string sql = @"SELECT z.NrZgloszenia, 
                                      COALESCE(k.ImieNazwisko, k.NazwaFirmy, '') AS ImieNazwisko,
                                      p.NazwaSystemowa AS Produkt
                               FROM Zgloszenia z
                               LEFT JOIN Klienci k ON z.KlientID = k.Id
                               LEFT JOIN Produkty p ON z.ProduktID = p.Id
                               WHERE z.NrZgloszenia LIKE @f 
                                  OR k.ImieNazwisko LIKE @f 
                                  OR k.NazwaFirmy LIKE @f
                               ORDER BY z.Id DESC 
                               LIMIT 20";

                var param = new MySqlParameter("@f", $"%{fraza}%");
                DataTable dt = await _db.GetDataTableAsync(sql, param);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Nie znaleziono zgłoszeń pasujących do frazy.", "Brak wyników", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    WypelnijComboZgloszen(dt);
                    MessageBox.Show($"Znaleziono {dt.Rows.Count} pasujących zgłoszeń.", "Wyniki", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wyszukiwania: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnZobaczZgloszenie_Click(object sender, EventArgs e)
        {
            if (cmbZgloszenia.SelectedIndex <= 0)
            {
                MessageBox.Show("Najpierw wybierz zgłoszenie z listy.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Pobieramy numer z formatu "NR/2023 - Jan Kowalski"
            string nr = cmbZgloszenia.SelectedItem.ToString().Split('-')[0].Trim();

            // Otwieramy formularz podglądu
            new Form2(nr).Show();
        }

        // *** NOWA METODA: Zapisywanie przypisania bez wysyłania SMS ***
        private async void BtnZapiszPrzypisanie_Click(object sender, EventArgs e)
        {
            if (cmbZgloszenia.SelectedIndex <= 0)
            {
                MessageBox.Show("Wybierz zgłoszenie z listy, aby zapisać przypisanie.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string nrZgloszenia = cmbZgloszenia.SelectedItem.ToString().Split('-')[0].Trim();
                
                // Wywołujemy metodę przypisania
                await _db.PrzypiszOstatniSmsDoZgloszeniaAsync(_numer, nrZgloszenia);
                
                MessageBox.Show($"Przypisano SMS do zgłoszenia: {nrZgloszenia}", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Opcjonalnie: zamknij formularz po zapisaniu
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisywania przypisania: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnWyslij_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOdpowiedz.Text))
            {
                MessageBox.Show("Wpisz treść odpowiedzi przed wysłaniem.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool sukces = await _client.SendSmsAsync(_numer, txtOdpowiedz.Text);
                
                if (sukces)
                {
                    // Zapisz wysłany SMS
                    await _db.ZapiszNowySmsAsync(_numer, txtOdpowiedz.Text, "Wysłane");

                    // Przypisz do zgłoszenia jeśli wybrano
                    if (cmbZgloszenia.SelectedIndex > 0)
                    {
                        string nrZgloszenia = cmbZgloszenia.SelectedItem.ToString().Split('-')[0].Trim();
                        await _db.PrzypiszOstatniSmsDoZgloszeniaAsync(_numer, nrZgloszenia);
                    }

                    MessageBox.Show("SMS wysłany pomyślnie!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wysyłka nie powiodła się (błąd bramki SMS).", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wysyłania: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            this.Size = new Size(550, 540); // Zwiększone o 40px dla nowego przycisku
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9);
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // === SEKCJA 1: Otrzymany SMS ===
            Label l1 = new Label 
            { 
                Text = "Otrzymano:", 
                Location = new Point(20, 10), 
                AutoSize = true, 
                Font = new Font("Segoe UI", 9, FontStyle.Bold) 
            };
            
            TextBox t1 = new TextBox 
            { 
                Text = _tresc, 
                Location = new Point(20, 30), 
                Width = 490, 
                Height = 60, 
                Multiline = true, 
                ReadOnly = true, 
                BackColor = Color.LightYellow,
                Font = new Font("Segoe UI", 9)
            };

            // === SEKCJA 2: Szablony odpowiedzi ===
            Label l2 = new Label 
            { 
                Text = "Wybierz szablon:", 
                Location = new Point(20, 100), 
                AutoSize = true 
            };
            
            cmbSzablony = new ComboBox 
            { 
                Location = new Point(20, 120), 
                Width = 490, 
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbSzablony.SelectedIndexChanged += CmbSzablony_SelectedIndexChanged;

            // === SEKCJA 3: Odpowiedź ===
            Label l3 = new Label 
            { 
                Text = "Twoja odpowiedź:", 
                Location = new Point(20, 155), 
                AutoSize = true 
            };
            
            txtOdpowiedz = new TextBox 
            { 
                Location = new Point(20, 175), 
                Width = 490, 
                Height = 100, 
                Multiline = true,
                Font = new Font("Segoe UI", 9)
            };

            // === SEKCJA 4: Przypisanie do reklamacji ===
            Label l4 = new Label 
            { 
                Text = "Przypisanie do reklamacji:", 
                Location = new Point(20, 290), 
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            cmbZgloszenia = new ComboBox 
            { 
                Location = new Point(20, 310), 
                Width = 280, 
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };

            Button btnSearch = new Button 
            { 
                Text = "🔍 Szukaj", 
                Location = new Point(310, 309), 
                Width = 90, 
                Height = 25, 
                BackColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            btnSearch.FlatAppearance.BorderColor = Color.LightGray;
            btnSearch.Click += BtnSzukaj_Click;

            Button btnLook = new Button 
            { 
                Text = "👁 Zobacz", 
                Location = new Point(410, 309), 
                Width = 100, 
                Height = 25,
                BackColor = Color.WhiteSmoke,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            btnLook.FlatAppearance.BorderColor = Color.LightGray;
            btnLook.Click += BtnZobaczZgloszenie_Click;

            // *** NOWY PRZYCISK: Zapisz przypisanie ***
            Button btnSavePrzypisanie = new Button 
            { 
                Text = "💾 ZAPISZ PRZYPISANIE", 
                Location = new Point(20, 345), 
                Width = 490, 
                Height = 40, 
                BackColor = Color.FromArgb(40, 167, 69), // Zielony bootstrap-style
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSavePrzypisanie.FlatAppearance.BorderSize = 0;
            btnSavePrzypisanie.Click += BtnZapiszPrzypisanie_Click;

            // === SEKCJA 5: Wysyłanie SMS ===
            Button btnSend = new Button 
            { 
                Text = "📤 WYŚLIJ SMS", 
                Location = new Point(20, 395), 
                Width = 490, 
                Height = 50, 
                BackColor = Color.FromArgb(0, 122, 204), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += BtnWyslij_Click;

            // Dodaj wszystkie kontrolki
            this.Controls.AddRange(new Control[] 
            { 
                l1, t1,                          // Otrzymany SMS
                l2, cmbSzablony,                 // Szablony
                l3, txtOdpowiedz,                // Odpowiedź
                l4, cmbZgloszenia,               // Przypisanie
                btnSearch, btnLook,              // Przyciki wyszukiwania
                btnSavePrzypisanie,              // NOWY: Zapisz przypisanie
                btnSend                          // Wyślij SMS
            });
        }
    }
}
