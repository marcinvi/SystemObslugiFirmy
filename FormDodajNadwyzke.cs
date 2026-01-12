using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormDodajNadwyzke : Form
    {
        private readonly MagazynService _service;
        private int _wybranyProduktId = 0;

        public FormDodajNadwyzke()
        {
            InitializeComponent();
            _service = new MagazynService();

            this.Load += FormDodajNadwyzke_Load;

            this.btnSzukajProduktu.Click += btnSzukajProduktu_Click;
            this.btnZapisz.Click += async (s, e) => await Zapisz();
            this.btnAnuluj.Click += (s, e) => this.Close();

            // Podpowiadanie przy wpisywaniu
            cmbCzesc.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCzesc.AutoCompleteSource = AutoCompleteSource.ListItems;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormDodajNadwyzke_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. Ładowanie lokalizacji
                var listaLok = await _service.PobierzLokalizacjeAsync();
                cmbLokalizacja.Items.Clear();
                cmbLokalizacja.Items.AddRange(listaLok.ToArray());

                // 2. Ładowanie WSZYSTKICH znanych nazw części (do podpowiedzi)
                // Dzięki temu jak wpiszesz "Si", system podpowie "Silnik", żebyś użył tej samej nazwy co wcześniej.
                await ZaladujWszystkieZnaneCzesci();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania: " + ex.Message);
            }
        }

        private async Task ZaladujWszystkieZnaneCzesci()
        {
            // Pobieramy unikalne nazwy z bazy, żeby ułatwić grupowanie
            try
            {
                string sql = "SELECT DISTINCT NazwaCzesci FROM DostepneCzesci ORDER BY NazwaCzesci";
                var dt = await new DatabaseService(DatabaseHelper.GetConnectionString()).GetDataTableAsync(sql);

                cmbCzesc.Items.Clear();
                // Dodajemy standardowe opcje na początek
                cmbCzesc.Items.Add("Całe urządzenie");
                cmbCzesc.Items.Add("Zasilacz");
                cmbCzesc.Items.Add("Pilot");

                foreach (DataRow row in dt.Rows)
                {
                    string nazwa = row["NazwaCzesci"].ToString();
                    if (!cmbCzesc.Items.Contains(nazwa)) cmbCzesc.Items.Add(nazwa);
                }
            }
            catch { }
        }

        private void btnSzukajProduktu_Click(object sender, EventArgs e)
        {
            using (var searchForm = new FormWybierzProdukt())
            {
                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    _wybranyProduktId = searchForm.WybraneId;
                    txtProdukt.Text = searchForm.WybranaNazwa;

                    // Jeśli użytkownik jeszcze nic nie wpisał w nazwie części,
                    // spróbujmy załadować szablon dla tego produktu
                    if (string.IsNullOrWhiteSpace(cmbCzesc.Text))
                    {
                        _ = ZaladujSzablonDlaProduktu(_wybranyProduktId);
                    }
                }
            }
        }

        private async Task ZaladujSzablonDlaProduktu(int pid)
        {
            try
            {
                var szablony = await _service.PobierzSzablonyDlaProduktuAsync(pid);
                if (szablony.Count > 0)
                {
                    // Czyścimy i ładujemy tylko dedykowane, jeśli są
                    cmbCzesc.Items.Clear();
                    foreach (var c in szablony) cmbCzesc.Items.Add(c.NazwaCzesci);

                    // Ale dodajmy też "Całe urządzenie" zawsze
                    cmbCzesc.Items.Add("Całe urządzenie");
                }
            }
            catch { }
        }

        private async Task Zapisz()
        {
            if (_wybranyProduktId == 0)
            {
                MessageBox.Show("Wybierz produkt z bazy.");
                return;
            }

            string nazwaCzesci = cmbCzesc.Text.Trim();

            if (string.IsNullOrWhiteSpace(nazwaCzesci))
            {
                // Automatyczna sugestia jeśli puste
                if (MessageBox.Show("Nie podałeś nazwy części.\nCzy chcesz zapisać to jako 'Całe urządzenie'?", "Pytanie", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    nazwaCzesci = "Całe urządzenie";
                }
                else
                {
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(cmbLokalizacja.Text))
            {
                MessageBox.Show("Podaj lokalizację.");
                return;
            }

            try
            {
                int ilosc = (int)numIlosc.Value;
                string lok = cmbLokalizacja.Text;
                string uwagi = txtUwagi.Text;

                await _service.DodajNadwyzkeCzesciAsync(_wybranyProduktId, nazwaCzesci, ilosc, lok, uwagi);

                MessageBox.Show($"Zapisano {ilosc} szt. '{nazwaCzesci}'.\nW zakładce 'Podsumowanie' zostaną one zgrupowane pod nazwą '{nazwaCzesci}'.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
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