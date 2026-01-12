using Microsoft.VisualBasic; // Wymaga dodania referencji do Microsoft.VisualBasic w projekcie
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormStanMagazynowy : Form
    {
        private readonly MagazynService _service;

        // Cache danych (pełne listy pobrane z bazy)
        private List<StanMagazynowy> _cacheUrzadzenia;
        private List<DostepnaCzescView> _cacheCzesci;
        private List<MagazynService.StanIlosciowyView> _cachePodsumowanie;
        private List<DostepnaCzescView> _cacheHistoria;

        // Kontrolka globalnego wyszukiwania
        private TextBox txtSzukajGlobalnie;

        public FormStanMagazynowy()
        {
            InitializeComponent();
            _service = new MagazynService();

            // Konfiguracja okna
            this.Size = new Size(1250, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Inicjalizacja UI
            InicjalizujGlobalnaWyszukiwarke();
            KonfigurujZdarzenia();

            this.Load += FormStanMagazynowy_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InicjalizujGlobalnaWyszukiwarke()
        {
            // Przesunięcie przycisków
            btnDodajRecznie.Left = 380;
            btnOdswiez.Left = btnDodajRecznie.Right + 10;
            if (btnDodajNadwyzke != null) btnDodajNadwyzke.Left = btnOdswiez.Right + 10;

            // Ikona i Pole tekstowe
            Label lblIcon = new Label { Text = "🔍", AutoSize = true, Location = new Point(12, 16), Font = new Font("Segoe UI", 12) };
            txtSzukajGlobalnie = new TextBox
            {
                Location = new Point(50, 18),
                Width = 300,
                Font = new Font("Segoe UI", 10),
            };

            // Podpięcie zdarzenia filtrowania
            txtSzukajGlobalnie.TextChanged += (s, e) => OdswiezWidokZfiltrami();

            panelTop.Controls.Add(lblIcon);
            panelTop.Controls.Add(txtSzukajGlobalnie);
        }

        private void KonfigurujZdarzenia()
        {
            // Zmiana zakładki czyści filtr globalny i odświeża dane
            tabControl.SelectedIndexChanged += async (s, e) => {
                txtSzukajGlobalnie.Text = "";
                await ZaladujAktywnaZakladke();
            };

            // Malowanie wierszy (kolory statusów)
            dgvUrzadzenia.RowPrePaint += DgvUrzadzenia_RowPrePaint;

            // Obsługa dwukliku (Akcje)
            dgvUrzadzenia.CellDoubleClick += DgvUrzadzenia_CellDoubleClick;
            dgvPodsumowanie.CellDoubleClick += DgvPodsumowanie_CellDoubleClick;
            dgvCzesci.CellDoubleClick += DgvCzesci_CellDoubleClick;
        }

        private async void FormStanMagazynowy_Load(object sender, EventArgs e)
        {
            // Stylizacja gridów na starcie
            UstawStylGrida(dgvUrzadzenia);
            UstawStylGrida(dgvCzesci);
            UstawStylGrida(dgvPodsumowanie);
            UstawStylGrida(dgvHistoria);

            await ZaladujAktywnaZakladke();
        }

        // =================================================================================
        // 1. LOGIKA ŁADOWANIA DANYCH
        // =================================================================================

        private async Task ZaladujAktywnaZakladke()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (tabControl.SelectedTab == tabUrzadzenia)
                {
                    _cacheUrzadzenia = await _service.PobierzWszystkieUrzadzeniaAsync();
                    dgvUrzadzenia.DataSource = _cacheUrzadzenia;
                    FormatujGridUrzadzenia();
                    BudujFiltryDlaGrid(dgvUrzadzenia, pnlFilterUrzadzenia);
                }
                else if (tabControl.SelectedTab == tabCzesci)
                {
                    _cacheCzesci = await _service.PobierzWszystkieCzesciAsync();
                    dgvCzesci.DataSource = _cacheCzesci;
                    FormatujGridCzesci();
                    BudujFiltryDlaGrid(dgvCzesci, pnlFilterCzesci);
                }
                else if (tabControl.SelectedTab == tabPodsumowanie)
                {
                    _cachePodsumowanie = await _service.PobierzPodsumowanieCzesciAsync();
                    dgvPodsumowanie.DataSource = _cachePodsumowanie;
                    FormatujGridPodsumowanie();
                    BudujFiltryDlaGrid(dgvPodsumowanie, pnlFilterPodsumowanie);
                }
                else if (tabControl.SelectedTab == tabHistoria)
                {
                    _cacheHistoria = await _service.PobierzHistorieZuzytychCzesciAsync();
                    dgvHistoria.DataSource = _cacheHistoria;
                    FormatujGridHistoria();
                    BudujFiltryDlaGrid(dgvHistoria, pnlFilterHistoria);
                }

                // Przywrócenie filtrowania po odświeżeniu (jeśli coś jest wpisane w globalnej lub kolumnach)
                OdswiezWidokZfiltrami();
            }
            catch (Exception ex) { MessageBox.Show("Błąd ładowania danych: " + ex.Message); }
            finally { this.Cursor = Cursors.Default; }
        }

        // =================================================================================
        // 2. LOGIKA FILTROWANIA (GLOBALNA + KOLUMNOWA)
        // =================================================================================

        private void OdswiezWidokZfiltrami()
        {
            string frazaGlobalna = txtSzukajGlobalnie.Text;

            // Filtrowanie URZĄDZEŃ
            if (tabControl.SelectedTab == tabUrzadzenia && _cacheUrzadzenia != null)
            {
                var filtered = _cacheUrzadzenia.Where(x =>
                    SprawdzGlobalny(x, frazaGlobalna) &&
                    SprawdzKolumny(x, pnlFilterUrzadzenia)
                ).ToList();
                dgvUrzadzenia.DataSource = filtered;
            }
            // Filtrowanie CZĘŚCI
            else if (tabControl.SelectedTab == tabCzesci && _cacheCzesci != null)
            {
                var filtered = _cacheCzesci.Where(x =>
                    SprawdzGlobalny(x, frazaGlobalna) &&
                    SprawdzKolumny(x, pnlFilterCzesci)
                ).ToList();
                dgvCzesci.DataSource = filtered;
            }
            // Filtrowanie PODSUMOWANIA
            else if (tabControl.SelectedTab == tabPodsumowanie && _cachePodsumowanie != null)
            {
                var filtered = _cachePodsumowanie.Where(x =>
                    SprawdzGlobalny(x, frazaGlobalna) &&
                    SprawdzKolumny(x, pnlFilterPodsumowanie)
                ).ToList();
                dgvPodsumowanie.DataSource = filtered;
            }
            // Filtrowanie HISTORII
            else if (tabControl.SelectedTab == tabHistoria && _cacheHistoria != null)
            {
                var filtered = _cacheHistoria.Where(x =>
                    SprawdzGlobalny(x, frazaGlobalna) &&
                    SprawdzKolumny(x, pnlFilterHistoria)
                ).ToList();
                dgvHistoria.DataSource = filtered;
            }
        }

        // Pomocnicza: Sprawdza czy obiekt pasuje do Lupy (szuka we wszystkich polach)
        private bool SprawdzGlobalny(object item, string fraza)
        {
            if (string.IsNullOrWhiteSpace(fraza)) return true;
            fraza = fraza.ToLower();

            foreach (PropertyInfo prop in item.GetType().GetProperties())
            {
                // Pomijamy ID i pola techniczne
                if (prop.Name == "Id" || prop.Name.Contains("Id")) continue;

                var val = prop.GetValue(item)?.ToString();
                if (val != null && val.ToLower().Contains(fraza)) return true;
            }
            return false;
        }

        // Pomocnicza: Sprawdza czy obiekt pasuje do filtrów nad kolumnami
        private bool SprawdzKolumny(object item, Panel panel)
        {
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is TextBox txt && !string.IsNullOrEmpty(txt.Text) && txt.Tag is string propName)
                {
                    var prop = item.GetType().GetProperty(propName);
                    if (prop != null)
                    {
                        var val = prop.GetValue(item)?.ToString() ?? "";
                        if (!val.ToLower().Contains(txt.Text.ToLower())) return false;
                    }
                }
            }
            return true;
        }

        // =================================================================================
        // 3. BUDOWANIE UI DLA FILTRÓW
        // =================================================================================

        private void BudujFiltryDlaGrid(DataGridView dgv, Panel panel)
        {
            // Czyścimy stare kontrolki
            while (panel.Controls.Count > 0)
            {
                var ctrl = panel.Controls[0];
                panel.Controls.Remove(ctrl);
                ctrl.Dispose();
            }

            dgv.ColumnWidthChanged -= Dgv_ColumnWidthChanged;
            dgv.ColumnWidthChanged += Dgv_ColumnWidthChanged;
            dgv.Tag = panel;

            int leftOffset = dgv.RowHeadersVisible ? dgv.RowHeadersWidth : 0;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (!col.Visible) continue;

                TextBox txt = new TextBox();
                txt.Tag = col.DataPropertyName;
                txt.Location = new Point(leftOffset, 5);
                txt.Width = col.Width - 4;
                txt.Height = 22;
                txt.Font = new Font("Segoe UI", 9);
                txt.BackColor = Color.White;

                // Zmiana tekstu wywołuje główne filtrowanie
                txt.TextChanged += (s, e) => OdswiezWidokZfiltrami();

                panel.Controls.Add(txt);
                leftOffset += col.Width;
            }
        }

        private void Dgv_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (sender is DataGridView dgv && dgv.Tag is Panel panel)
            {
                PozycjonujFiltry(dgv, panel);
            }
        }

        private void PozycjonujFiltry(DataGridView dgv, Panel panel)
        {
            int leftOffset = dgv.RowHeadersVisible ? dgv.RowHeadersWidth : 0;
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is TextBox txt && txt.Tag is string propName)
                {
                    if (dgv.Columns.Contains(propName) && dgv.Columns[propName].Visible)
                    {
                        txt.Left = leftOffset;
                        txt.Width = dgv.Columns[propName].Width - 4;
                        leftOffset += dgv.Columns[propName].Width;
                        txt.Visible = true;
                    }
                    else
                    {
                        txt.Visible = false;
                    }
                }
            }
        }

        // =================================================================================
        // 4. KOLOROWANIE WIERSZY (STATUSY)
        // =================================================================================

        private void DgvUrzadzenia_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var item = dgvUrzadzenia.Rows[e.RowIndex].DataBoundItem as StanMagazynowy;

            if (item != null)
            {
                // 1. ZŁOM / Utylizacja -> Szary
                if (!string.IsNullOrEmpty(item.StatusFizyczny) &&
                   (item.StatusFizyczny.Contains("ZŁOM") || item.StatusFizyczny.Contains("Utylizacja")))
                {
                    dgvUrzadzenia.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvUrzadzenia.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DimGray;
                }
                // 2. KOMPLETNY (Ma szablon i ma wszystkie części) -> Zielony
                else if (item.LiczbaCzesciRazem > 0 && item.LiczbaCzesciDostepnych >= item.LiczbaCzesciRazem)
                {
                    dgvUrzadzenia.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220); // Jasna zieleń
                    dgvUrzadzenia.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = Color.SeaGreen;
                }
                // 3. BRAKI (Ma szablon, ale brakuje części) -> Żółty
                else if (item.LiczbaCzesciRazem > 0 && item.LiczbaCzesciDostepnych < item.LiczbaCzesciRazem)
                {
                    dgvUrzadzenia.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LemonChiffon;
                }
            }
        }

        // =================================================================================
        // 5. INTERAKCJE (KLIKNIĘCIA I AKCJE)
        // =================================================================================

        private async void DgvUrzadzenia_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var item = dgvUrzadzenia.Rows[e.RowIndex].DataBoundItem as StanMagazynowy;
            if (item != null)
            {
                using (var form = new FormMagazynAction(item.NrZgloszenia, item.ProduktId, item.Model, item.NumerSeryjny, ""))
                {
                    form.ShowDialog();
                    if (form.CzyZmieniono) await ZaladujAktywnaZakladke();
                }
            }
        }

        private void DgvPodsumowanie_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var item = dgvPodsumowanie.Rows[e.RowIndex].DataBoundItem as MagazynService.StanIlosciowyView;

            if (item != null)
            {
                // Przekierowanie do zakładki "Części" z filtrowaniem
                tabControl.SelectedTab = tabCzesci;

                // Przeszukujemy panel filtrów, aby znaleźć pole dla "NazwaCzesci"
                foreach (Control c in pnlFilterCzesci.Controls)
                {
                    if (c is TextBox txt && txt.Tag?.ToString() == "NazwaCzesci")
                    {
                        txt.Text = item.NazwaCzesci; // To wywoła filtrowanie
                        break;
                    }
                }
            }
        }

        private async void DgvCzesci_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var item = dgvCzesci.Rows[e.RowIndex].DataBoundItem as DostepnaCzescView;

            if (item != null)
            {
                // Otwarcie okna szczegółów (z przyciskami Użyj/Usuń)
                using (var form = new FormCzescSzczegoly(item))
                {
                    form.ShowDialog();
                    // Po zamknięciu odświeżamy listę, bo część mogła zostać usunięta lub wydana
                    await ZaladujAktywnaZakladke();
                }
            }
        }

        // --- BUTTONY I MENU ---

        private async void btnOdswiez_Click(object sender, EventArgs e) => await ZaladujAktywnaZakladke();

        private void btnDodajRecznie_Click(object sender, EventArgs e)
        {
            using (var f = new FormDodajDoMagazynu())
            {
                f.ShowDialog();
                _ = ZaladujAktywnaZakladke();
            }
        }

        private void btnDodajNadwyzke_Click(object sender, EventArgs e)
        {
            using (var f = new FormDodajNadwyzke())
            {
                if (f.ShowDialog() == DialogResult.OK) _ = ZaladujAktywnaZakladke();
            }
        }

        // Menu kontekstowe (opcjonalne, ale zachowane dla kompatybilności)
        private void EdytujUrzadzenie_Click(object sender, EventArgs e) => DgvUrzadzenia_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvUrzadzenia.CurrentRow?.Index ?? -1));

        private async void UsunUrzadzenie_Click(object sender, EventArgs e)
        {
            if (dgvUrzadzenia.CurrentRow?.DataBoundItem is StanMagazynowy item &&
                MessageBox.Show("Czy na pewno usunąć to urządzenie z magazynu?\nHistoria zostanie zachowana.", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await _service.UsunUrzadzenieZMagazynuAsync(item.Id);
                await ZaladujAktywnaZakladke();
            }
        }

        private void UzyjWNaprawie_Click(object sender, EventArgs e) => DgvCzesci_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvCzesci.CurrentRow?.Index ?? -1));
        private void UsunCzesc_Click(object sender, EventArgs e) => DgvCzesci_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvCzesci.CurrentRow?.Index ?? -1));

        private async void itemPrzywrocCzesc_Click(object sender, EventArgs e)
        {
            if (dgvHistoria.CurrentRow?.DataBoundItem is DostepnaCzescView item)
            {
                if (MessageBox.Show("Przywrócić tę część na stan magazynowy?", "Potwierdź", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    await _service.PrzywrocCzescNaStanAsync(item.Id);
                    await ZaladujAktywnaZakladke();
                }
            }
        }

        // =================================================================================
        // 6. STYLIZACJA I FORMATOWANIE TABEL
        // =================================================================================

        private void UstawStylGrida(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Nagłówek
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 66, 91);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.EnableHeadersVisualStyles = false;

            // Wiersze
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.RowTemplate.Height = 35;

            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;

            // Double buffering
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgv, new object[] { true });
        }

        private void FormatujGridUrzadzenia()
        {
            var dgv = dgvUrzadzenia;
            if (dgv.DataSource == null) return;
            dgv.ReadOnly = true;

            Ukryj(dgv, "Id", "ProduktId", "LiczbaCzesciRazem", "LiczbaCzesciDostepnych", "OczekiwanaLiczbaCzesci", "UwagiMagazynowe");

            Nazwij(dgv, "NrZgloszenia", "Zgłoszenie", 100);
            Nazwij(dgv, "Producent", "Producent", 120);
            Nazwij(dgv, "Model", "Produkt / Model", 250);
            Nazwij(dgv, "NumerSeryjny", "S/N", 150);
            Nazwij(dgv, "StatusFizyczny", "Status", 130);
            Nazwij(dgv, "Lokalizacja", "Lokalizacja", 100);
            Nazwij(dgv, "CzyDawca", "Dawca?", 60);

            // Stan/Uwagi
            Nazwij(dgv, "Uwagi", "Stan Kompletności / Uwagi", 200);
            if (dgv.Columns.Contains("Uwagi")) dgv.Columns["Uwagi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void FormatujGridCzesci()
        {
            var dgv = dgvCzesci;
            dgv.ReadOnly = true;
            Ukryj(dgv, "Id", "SnDawcy", "StanOpis", "ProduktId");

            Nazwij(dgv, "NazwaCzesci", "Nazwa Części", 200);
            if (dgv.Columns.Contains("NazwaCzesci")) dgv.Columns["NazwaCzesci"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Nazwij(dgv, "ModelDawcy", "Pochodzenie (Model)", 250);
            Nazwij(dgv, "ZgloszenieDawcy", "Zgł. Dawcy", 120);
            Nazwij(dgv, "Lokalizacja", "Lokalizacja", 120);
            Nazwij(dgv, "TypPochodzenia", "Typ", 100);
        }

        private void FormatujGridPodsumowanie()
        {
            var dgv = dgvPodsumowanie;
            dgv.ReadOnly = true;

            // Ukrywamy kolumnę "Modele", bo prosiłeś
            Ukryj(dgv, "Modele");

            Nazwij(dgv, "NazwaCzesci", "Nazwa Części", 400);
            if (dgv.Columns.Contains("NazwaCzesci")) dgv.Columns["NazwaCzesci"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Nazwij(dgv, "Ilosc", "Ilość", 150);
        }

        private void FormatujGridHistoria()
        {
            var dgv = dgvHistoria;
            dgv.ReadOnly = true;
            Ukryj(dgv, "Id", "SnDawcy", "ProduktId", "StanOpis");

            Nazwij(dgv, "NazwaCzesci", "Część", 250);
            if (dgv.Columns.Contains("NazwaCzesci")) dgv.Columns["NazwaCzesci"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Nazwij(dgv, "ModelDawcy", "Dawca", 250);
            Nazwij(dgv, "ZgloszenieDawcy", "Użyto w (Zgł)", 150);
            Nazwij(dgv, "Lokalizacja", "Data Użycia", 150);
        }

        // Helpery
        private void Ukryj(DataGridView dgv, params string[] cols) { foreach (var c in cols) if (dgv.Columns.Contains(c)) dgv.Columns[c].Visible = false; }
        private void Nazwij(DataGridView dgv, string col, string txt, int w) { if (dgv.Columns.Contains(col)) { dgv.Columns[col].HeaderText = txt; dgv.Columns[col].Width = w; } }
    
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