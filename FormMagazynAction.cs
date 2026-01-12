using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Reklamacje_Dane
{
    public partial class FormMagazynAction : Form
    {
        private readonly string _nrZgloszenia;
        private readonly int _produktId;
        private readonly string _kategoriaProduktu;
        private readonly MagazynService _service;

        public bool CzyZmieniono { get; private set; }

        public FormMagazynAction(string nrZgloszenia, int produktId, string model, string sn, string kategoria)
        {
            InitializeComponent();
            _nrZgloszenia = nrZgloszenia;
            _produktId = produktId;
            _kategoriaProduktu = kategoria;
            _service = new MagazynService();

            this.Text = $"Magazyn - Zgłoszenie {_nrZgloszenia} ({model})";
            lblInfoProdukt.Text = $"Produkt: {model}\nS/N: {sn}";

            // NOWA LISTA STATUSÓW FIZYCZNYCH
            comboStatus.Items.Clear();
            comboStatus.Items.AddRange(new object[] {
                "Przyjęty na stan",
                "Na części",            // Dawca
                "Utylizacja",           // Złom
                "Wysłany do producenta",
                "Został u klienta"
            });

            this.Load += FormMagazynAction_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormMagazynAction_Load(object sender, EventArgs e)
        {
            await ZaladujLokalizacje();
            await WczytajDane();
            await ZaladujSugestie();
            await ZaladujZdjecia();
        }

        private async Task WczytajDane()
        {
            var stan = await _service.PobierzStanAsync(_nrZgloszenia);
            if (stan != null)
            {
                comboStatus.SelectedItem = stan.StatusFizyczny;
                cmbLokalizacja.Text = stan.Lokalizacja;
                txtUwagi.Text = stan.Uwagi;
                btnZapisz.Text = "Zapisz zmiany";
                if (stan.CzyDawca) await ZaladujListeCzesci(stan.Id);
            }
            else
            {
                comboStatus.SelectedItem = "Przyjęty na stan";
                cmbLokalizacja.Text = "Magazyn Przyjęć";
                btnZapisz.Text = "Przyjmij na stan";
            }
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            try
            {
                string status = comboStatus.Text;
                bool czyDawca = (status == "Na części"); // Zmieniona nazwa statusu

                // 1. Zapis stanu głównego
                var stan = await _service.PobierzStanAsync(_nrZgloszenia);
                if (stan == null)
                {
                    string sn = ""; string model = "";
                    var lines = lblInfoProdukt.Text.Split('\n');
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("Produkt:")) model = line.Replace("Produkt: ", "").Trim();
                        if (line.StartsWith("S/N:")) sn = line.Replace("S/N: ", "").Trim();
                    }
                    await _service.PrzyjmijNaMagazynAsync(_nrZgloszenia, model, sn, txtUwagi.Text);
                }

                // 2. Aktualizacja
                await _service.AktualizujStatusAsync(_nrZgloszenia, status, cmbLokalizacja.Text, czyDawca, txtUwagi.Text);

                List<string> wybraneCzesci = new List<string>();

                // 3. Jeśli Dawca - zapisujemy części
                if (czyDawca)
                {
                    int magazynId = await _service.PobierzIdMagazynoweAsync(_nrZgloszenia);
                    if (magazynId > 0)
                    {
                        wybraneCzesci = chkListCzesci.CheckedItems.Cast<string>().ToList();
                        await _service.ZapiszCzesciZDemontazuAsync(magazynId, wybraneCzesci);
                    }
                }

                // 4. LOGOWANIE (POPRAWIONE) - Logujemy wszystko, nie tylko części
                string log = $"AKCJA MAGAZYNOWA: Ustawiono status '{status}'. Lokalizacja: {cmbLokalizacja.Text}.";

                if (status == "Utylizacja")
                {
                    log += " Sprzęt przekazano do utylizacji.";
                }
                else if (status == "Został u klienta")
                {
                    log += " Fizycznie sprzęt nie wrócił do serwisu.";
                }
                else if (czyDawca && wybraneCzesci.Count > 0)
                {
                    log += $" Odzyskano części: {string.Join(", ", wybraneCzesci)}.";
                }

                // Zapis logu do bazy
                await new DziennikLogger().DodajAsync(Program.fullName, log, _nrZgloszenia);
                new Dzialaniee().DodajNoweDzialanie(_nrZgloszenia, Program.fullName, log);

                MessageBox.Show($"Zapisano pomyślnie.\nStatus: {status}", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CzyZmieniono = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
            }
        }

        // --- RESZTA KODU BEZ ZMIAN (Lokalizacje, Zdjecia itp.) ---
        // (Wklej tutaj resztę metod: ZaladujLokalizacje, btnDodajLok_Click, comboStatus_SelectedIndexChanged, ZaladujZdjecia itd. z poprzedniej wersji)
        // PAMIĘTAJ o zmianie w comboStatus_SelectedIndexChanged: if (status == "Na części") zamiast "DAWCA CZĘŚCI"

        private async Task ZaladujLokalizacje()
        {
            var lok = await _service.PobierzLokalizacjeAsync();
            cmbLokalizacja.Items.Clear();
            cmbLokalizacja.Items.AddRange(lok.ToArray());
        }

        private async void btnDodajLok_Click(object sender, EventArgs e)
        {
            string nowa = Interaction.InputBox("Nazwa:", "Dodaj lokalizację", "");
            if (!string.IsNullOrWhiteSpace(nowa))
            {
                await _service.DodajLokalizacjeAsync(nowa);
                await ZaladujLokalizacje();
                cmbLokalizacja.SelectedItem = nowa;
            }
        }

        private async void btnUsunLok_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbLokalizacja.Text) && MessageBox.Show("Usunąć?", "Potwierdź", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await _service.UsunLokalizacjeAsync(cmbLokalizacja.Text);
                await ZaladujLokalizacje();
                cmbLokalizacja.Text = "";
            }
        }

        private async void comboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string status = comboStatus.SelectedItem?.ToString();
            // Zmiana nazwy statusu
            grpCzesci.Visible = (status == "Na części");

            if (grpCzesci.Visible && chkListCzesci.Items.Count == 0) await ZaladujSzablonCzesci();

            if (status.Contains("Na części")) comboStatus.BackColor = Color.Plum;
            else if (status.Contains("Utylizacja")) comboStatus.BackColor = Color.Gray;
            else if (status.Contains("klienta")) comboStatus.BackColor = Color.LightYellow;
            else comboStatus.BackColor = Color.White;
        }

        private async Task ZaladujSzablonCzesci()
        {
            chkListCzesci.Items.Clear();
            if (_produktId <= 0) return;
            var szablony = await _service.PobierzSzablonyDlaProduktuAsync(_produktId);
            foreach (var czesc in szablony) chkListCzesci.Items.Add(czesc.NazwaCzesci);

            lblBrakSzablonu.Visible = (chkListCzesci.Items.Count == 0);
            chkListCzesci.Enabled = (chkListCzesci.Items.Count > 0);
        }

        private async Task ZaladujListeCzesci(int magazynId)
        {
            await ZaladujSzablonCzesci();
            var zapisane = await _service.PobierzZapisaneCzesciDlaDawcyAsync(magazynId);
            for (int i = 0; i < chkListCzesci.Items.Count; i++)
            {
                if (zapisane.Contains(chkListCzesci.Items[i].ToString())) chkListCzesci.SetItemChecked(i, true);
            }
        }

        private async Task ZaladujSugestie()
        {
            var sugestie = await _service.PobierzSugestieCzesciAsync(_kategoriaProduktu);
            cmbNowaCzescSzybka.Items.Clear();
            cmbNowaCzescSzybka.Items.AddRange(sugestie.ToArray());
        }

        private async void btnDodajCzescSzybka_Click(object sender, EventArgs e)
        {
            string nowa = cmbNowaCzescSzybka.Text.Trim();
            if (string.IsNullOrEmpty(nowa) || _produktId <= 0) return;

            try
            {
                await _service.DodajSzablonCzesciAsync(_produktId, nowa);
                cmbNowaCzescSzybka.Text = "";
                var zaznaczone = chkListCzesci.CheckedItems.Cast<string>().ToList();
                zaznaczone.Add(nowa);
                await ZaladujSzablonCzesci();
                await ZaladujSugestie();
                for (int i = 0; i < chkListCzesci.Items.Count; i++)
                {
                    if (zaznaczone.Contains(chkListCzesci.Items[i].ToString())) chkListCzesci.SetItemChecked(i, true);
                }
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }

        private async Task ZaladujZdjecia()
        {
            foreach (Control c in flowZdjecia.Controls)
            {
                if (c is PictureBox pb)
                {
                    if (pb.Image != null) pb.Image.Dispose();
                    pb.Dispose();
                }
            }
            flowZdjecia.Controls.Clear();

            int magazynId = await _service.PobierzIdMagazynoweAsync(_nrZgloszenia);
            if (magazynId == 0) return;

            var zdjecia = await _service.PobierzZdjeciaDlaMagazynuAsync(magazynId);
            foreach (var img in zdjecia)
            {
                var pb = new PictureBox
                {
                    Image = img,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Width = 150,
                    Height = 150,
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand
                };
                pb.DoubleClick += (s, ev) => new FormPodgladZdjecia(img).ShowDialog();
                flowZdjecia.Controls.Add(pb);
            }
        }
        public class FormPodgladZdjecia : Form
        {
            public FormPodgladZdjecia(Image img)
            {
                this.Text = "Podgląd";
                this.WindowState = FormWindowState.Maximized;
                this.Controls.Add(new PictureBox
                {
                    Image = img,
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom
                });
            }
        }
        private async void btnDodajZdjecie_Click(object sender, EventArgs e)
        {
            int magazynId = await _service.PobierzIdMagazynoweAsync(_nrZgloszenia);
            if (magazynId == 0)
            {
                MessageBox.Show("Najpierw zapisz przyjęcie towaru.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var ofd = new OpenFileDialog { Multiselect = true, Filter = "Obrazy|*.jpg;*.png;*.jpeg;*.bmp" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in ofd.FileNames) await _service.DodajZdjecieAsync(magazynId, file);
                    await ZaladujZdjecia();
                    MessageBox.Show("Dodano zdjęcia.");
                }
            }

        }

        private void btnAnuluj_Click(object sender, EventArgs e) => this.Close();
    
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