using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public partial class FormPolaczenie : Form
    {
        private string _numerTelefonu;
        private Klient _klient;

        // Twoje serwisy
        private DatabaseService _db;
        private PhoneClient _client;
        private ContactRepository _repo = new ContactRepository();

        public FormPolaczenie(string numer, Klient klient, DataTable dtZgloszenia, DatabaseService db, PhoneClient client)
        {
            InitializeComponent();

            _numerTelefonu = numer;
            _klient = klient;
            _db = db;
            _client = client;

            UstawDaneInterfejsu();
            WypelnijListeZgloszen(dtZgloszenia);
            ZaladujSzablonySMSAsync();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void UstawDaneInterfejsu()
        {
            lblNumer.Text = FormatujNumer(_numerTelefonu);

            if (_klient != null)
                lblKlient.Text = $"{_klient.ImieNazwisko}\n{_klient.NazwaFirmy}";
            else
            {
                lblKlient.Text = "Nieznany Klient";
                lblKlient.ForeColor = Color.LightGray;
            }
        }

        private void WypelnijListeZgloszen(DataTable dt)
        {
            listZgloszenia.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    // 1. Pobieramy dane z wiersza
                    int id = Convert.ToInt32(row["Id"]);
                    string numer = row["NrZgloszenia"].ToString();
                    string status = row.Table.Columns.Contains("Status") ? row["Status"].ToString() : row["StatusOgolny"].ToString();

                    // 2. Pobieramy PRODUKT (bezpiecznie)
                    string produkt = "Brak produktu";
                    if (dt.Columns.Contains("Produkt") && row["Produkt"] != DBNull.Value)
                    {
                        produkt = row["Produkt"].ToString();
                    }
                    else if (dt.Columns.Contains("NazwaSystemowa") && row["NazwaSystemowa"] != DBNull.Value)
                    {
                        produkt = row["NazwaSystemowa"].ToString();
                    }

                    // 3. Budujemy tekst do wyświetlenia
                    string text = $"#{numer} [{status}]\n{produkt}";

                    // 4. Sprawdzamy czy aktywne
                    bool czyAktywne = !status.ToLower().Contains("zakończ") && !status.ToLower().Contains("zamkni");

                    // 5. Dodajemy do listy (TO NAPRAWIA BŁĄD CS1729)
                    listZgloszenia.Items.Add(new ZgloszenieItem(id, numer, text, czyAktywne));
                }
            }
            else
            {
                // Pusty element informacyjny
                listZgloszenia.Items.Add(new ZgloszenieItem(0, "", "(Brak zgłoszeń w bazie)", false));
            }
        }

        // ========================================================
        // KLIKNIĘCIE OTWIERA FORM2
        // ========================================================
        private void ListZgloszenia_DoubleClick(object sender, EventArgs e)
        {
            // TO NAPRAWIA BŁĄD CS1061 (bo teraz item ma pole Id)
            if (listZgloszenia.SelectedItem is ZgloszenieItem item && item.Id > 0)
            {
                try
                {
                    // Otwieramy Form2 podając numer (np. 55/2023)
                    new Form2(item.NumerString).Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd otwierania zgłoszenia: " + ex.Message);
                }
            }
        }

        // ========================================================
        // RYSOWANIE (KOLORY)
        // ========================================================
        private void ListZgloszenia_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            if (listZgloszenia.Items[e.Index] is ZgloszenieItem item)
            {
                e.DrawBackground();

                Brush textBrush;
                if (item.Id == 0) textBrush = Brushes.Gray;
                else if (item.IsActive) textBrush = Brushes.LightGreen;
                else textBrush = Brushes.WhiteSmoke;

                // Rysujemy tekst
                e.Graphics.DrawString(item.Text, e.Font, textBrush, e.Bounds.X + 5, e.Bounds.Y + 5);
                e.DrawFocusRectangle();
            }
        }

        // ========================================================
        // WYSYŁANIE SMS
        // ========================================================
        private async void BtnWyslijSms_Click(object sender, EventArgs e)
        {
            if (cmbSzablony.SelectedItem is SmsTemplate wybrany && !string.IsNullOrEmpty(wybrany.Tresc))
            {
                btnWyslijSms.Enabled = false;
                btnWyslijSms.Text = "...";

                bool sukces = await _client.SendSmsAsync(_numerTelefonu, wybrany.Tresc);

                if (sukces)
                {
                    // Przypisanie do zgłoszenia (jeśli zaznaczono)
                    int? idZgloszenia = null;
                    if (listZgloszenia.SelectedItem is ZgloszenieItem item && item.Id > 0)
                    {
                        idZgloszenia = item.Id;
                    }

                    // Zapis w bazie
                    _repo.ZapiszSmsWychodzacy(_numerTelefonu, wybrany.Tresc, _klient?.Id, idZgloszenia);

                    MessageBox.Show("Wysłano!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Błąd wysyłania (telefon nie odpowiedział).");
                    btnWyslijSms.Enabled = true;
                    btnWyslijSms.Text = "Wyślij SMS";
                }
            }
        }

        private async void ZaladujSzablonySMSAsync()
        {
            try
            {
                cmbSzablony.Items.Clear();
                cmbSzablony.Items.Add(new SmsTemplate("Wybierz szablon...", ""));

                DataTable dtSzab = await _db.GetDataTableAsync("SELECT Nazwa, Tresc FROM SzablonySMS");
                foreach (DataRow r in dtSzab.Rows)
                {
                    cmbSzablony.Items.Add(new SmsTemplate(r["Nazwa"].ToString(), r["Tresc"].ToString()));
                }
                if (cmbSzablony.Items.Count > 0) cmbSzablony.SelectedIndex = 0;
            }
            catch
            {
                // Fallback
                cmbSzablony.Items.Add(new SmsTemplate("Błąd bazy", ""));
            }
        }

        private void BtnClose_Click(object sender, EventArgs e) => this.Close();

        private string FormatujNumer(string nr)
        {
            if (nr.Length == 9) return $"{nr.Substring(0, 3)} {nr.Substring(3, 3)} {nr.Substring(6, 3)}";
            return nr;
        }

        // ========================================================
        // KLASY POMOCNICZE (TU BYŁ BŁĄD - TERAZ JEST POPRAWIONE)
        // ========================================================

        private class ZgloszenieItem
        {
            // Dodane pole ID, którego brakowało
            public int Id { get; set; }
            public string NumerString { get; set; }
            public string Text { get; set; }
            public bool IsActive { get; set; }

            // Konstruktor z 4 argumentami (naprawia CS1729)
            public ZgloszenieItem(int id, string numerStr, string text, bool isActive)
            {
                Id = id;
                NumerString = numerStr;
                Text = text;
                IsActive = isActive;
            }

            public override string ToString() => Text;
        }

        private class SmsTemplate
        {
            public string Nazwa { get; set; }
            public string Tresc { get; set; }
            public SmsTemplate(string n, string t) { Nazwa = n; Tresc = t; }
            public override string ToString() => Nazwa;
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