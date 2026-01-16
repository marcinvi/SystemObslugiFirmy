using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormSzablonyEmail : Form
    {
        private readonly EmailTemplateService _templateService = new EmailTemplateService();
        private readonly StatusService _statusService = new StatusService(DatabaseHelper.GetConnectionString(), "Klient");
        private ComboBox _cmbStatusMap;
        private Button _btnAssignStatus;
        private Button _btnClearStatus;
        private Label _lblStatusMap;

        public FormSzablonyEmail()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeStatusMappingUi();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormSzablonyEmail_Load(object sender, EventArgs e)
        {
            WypelnijKontrolkiFormatowania();
            WyswietlZmienne();
            await WczytajSzablonyAsync();
            await LoadStatusOptionsAsync();
        }

        private void InitializeStatusMappingUi()
        {
            _lblStatusMap = new Label
            {
                Text = "Przypisz do statusu:",
                AutoSize = true,
                Left = listBoxSzablony.Right + 20,
                Top = listBoxSzablony.Top
            };

            _cmbStatusMap = new ComboBox
            {
                Left = _lblStatusMap.Left,
                Top = _lblStatusMap.Bottom + 6,
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _btnAssignStatus = new Button
            {
                Text = "Przypisz",
                Left = _cmbStatusMap.Left,
                Top = _cmbStatusMap.Bottom + 6,
                Width = 100
            };
            _btnAssignStatus.Click += btnAssignStatus_Click;

            _btnClearStatus = new Button
            {
                Text = "Wyczyść",
                Left = _btnAssignStatus.Right + 10,
                Top = _btnAssignStatus.Top,
                Width = 100
            };
            _btnClearStatus.Click += btnClearStatus_Click;

            this.Controls.Add(_lblStatusMap);
            this.Controls.Add(_cmbStatusMap);
            this.Controls.Add(_btnAssignStatus);
            this.Controls.Add(_btnClearStatus);
        }

        private async Task LoadStatusOptionsAsync()
        {
            try
            {
                var statuses = await _statusService.GetStatusesAsync();
                _cmbStatusMap.Items.Clear();
                _cmbStatusMap.Items.AddRange(statuses.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania statusów: " + ex.Message);
            }
        }

        private void WypelnijKontrolkiFormatowania()
        {
            using (var fontsCollection = new InstalledFontCollection())
            {
                foreach (var fontFamily in fontsCollection.Families)
                {
                    cmbFontFamily.Items.Add(fontFamily.Name);
                }
            }
            if (cmbFontFamily.Items.Count > 0) cmbFontFamily.SelectedIndex = 0;
            // Bezpieczniejszy wybór: jeśli Arial istnieje, wybierz go, jeśli nie - pierwszy z listy
            if (cmbFontFamily.Items.Contains("Arial")) cmbFontFamily.SelectedItem = "Arial";

            for (int i = 8; i <= 72; i += 2)
            {
                cmbFontSize.Items.Add(i);
            }
            cmbFontSize.SelectedItem = 10;
        }

        private void WyswietlZmienne()
        {
            flpZmienne.Controls.Clear();
            var zmienne = new List<string>
            {
                "{{NrZgloszenia}}", "{{DataZgloszenia}}", "{{KlientNazwa}}", "{{KlientEmail}}",
                "{{KlientTelefon}}", "{{KlientAdres}}", "{{ProduktNazwa}}", "{{ProduktSN}}",
                "{{OpisUsterki}}", "{{DataZakupu}}", "{{NrFaktury}}", "{{PracownikImie}}",
                "{{StatusOgolny}}", "{{StatusKlient}}"
            };

            foreach (var zmienna in zmienne)
            {
                var linkLabel = new LinkLabel
                {
                    Text = zmienna,
                    AutoSize = true,
                    Margin = new Padding(3),
                    LinkColor = Color.RoyalBlue
                };
                linkLabel.Click += LinkLabelZmienna_Click;
                flpZmienne.Controls.Add(linkLabel);
            }
        }

        private void LinkLabelZmienna_Click(object sender, EventArgs e)
        {
            var linkLabel = sender as LinkLabel;
            if (linkLabel != null)
            {
                rtbTrescSzablonu.SelectedText = linkLabel.Text;
                rtbTrescSzablonu.Focus();
            }
        }

        private async Task WczytajSzablonyAsync()
        {
            listBoxSzablony.DataSource = null;
            try
            {
                // ZMIANA: Teraz metoda zwraca List<SzablonEmail>
                var szablony = await _templateService.GetActiveTemplatesAsync();
                listBoxSzablony.DataSource = szablony;
                listBoxSzablony.DisplayMember = "Nazwa";
                listBoxSzablony.ValueMember = "Id"; // Opcjonalne, ale przydatne
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania szablonów: " + ex.Message);
            }
        }

        private void listBoxSzablony_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ZMIANA: Rzutujemy na SzablonEmail
            if (listBoxSzablony.SelectedItem is SzablonEmail selected)
            {
                txtNazwaSzablonu.Text = selected.Nazwa;
                try
                {
                    rtbTrescSzablonu.Rtf = selected.TrescRtf;
                }
                catch
                {
                    rtbTrescSzablonu.Text = selected.TrescRtf; // Fallback
                }

                _ = LoadTemplateStatusAsync(selected.Id);
            }
        }

        private async Task LoadTemplateStatusAsync(int templateId)
        {
            if (_cmbStatusMap == null) return;

            var status = await _templateService.GetStatusForTemplateAsync(templateId);
            if (string.IsNullOrWhiteSpace(status))
            {
                _cmbStatusMap.SelectedIndex = -1;
                return;
            }

            _cmbStatusMap.SelectedItem = status;
        }

        private async void btnAssignStatus_Click(object sender, EventArgs e)
        {
            if (!(listBoxSzablony.SelectedItem is SzablonEmail selected))
            {
                MessageBox.Show("Wybierz szablon.", "Informacja");
                return;
            }

            if (_cmbStatusMap.SelectedItem == null)
            {
                MessageBox.Show("Wybierz status do przypisania.", "Informacja");
                return;
            }

            await _templateService.UpsertTemplateStatusAsync(selected.Id, _cmbStatusMap.SelectedItem.ToString());
            MessageBox.Show("Przypisano status do szablonu.", "Sukces");
        }

        private async void btnClearStatus_Click(object sender, EventArgs e)
        {
            if (!(listBoxSzablony.SelectedItem is SzablonEmail selected))
            {
                MessageBox.Show("Wybierz szablon.", "Informacja");
                return;
            }

            await _templateService.ClearTemplateStatusAsync(selected.Id);
            _cmbStatusMap.SelectedIndex = -1;
            MessageBox.Show("Usunięto przypisanie statusu.", "Sukces");
        }

        private void btnNowy_Click(object sender, EventArgs e)
        {
            listBoxSzablony.ClearSelected();
            txtNazwaSzablonu.Clear();
            rtbTrescSzablonu.Clear();
            txtNazwaSzablonu.Focus();
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNazwaSzablonu.Text))
            {
                MessageBox.Show("Nazwa szablonu nie może być pusta.", "Błąd");
                return;
            }

            // ZMIANA: Używamy SzablonEmail
            var template = new SzablonEmail
            {
                Id = (listBoxSzablony.SelectedItem as SzablonEmail)?.Id ?? 0,
                Nazwa = txtNazwaSzablonu.Text,
                TrescRtf = rtbTrescSzablonu.Rtf,
                Aktywny = true
            };

            try
            {
                // ZMIANA: Wywołujemy SaveTemplateAsync (którą dodałem do serwisu w poprzednim kroku)
                await _templateService.SaveTemplateAsync(template);
                MessageBox.Show("Szablon został zapisany.", "Sukces");
                await WczytajSzablonyAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu szablonu: " + ex.Message);
            }
        }

        private async void btnUsun_Click(object sender, EventArgs e)
        {
            // ZMIANA: Rzutowanie na SzablonEmail
            if (!(listBoxSzablony.SelectedItem is SzablonEmail selected))
            {
                MessageBox.Show("Wybierz szablon do usunięcia.", "Informacja");
                return;
            }

            if (MessageBox.Show($"Czy na pewno chcesz usunąć szablon '{selected.Nazwa}'?", "Potwierdzenie", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    // ZMIANA: Wywołujemy DeleteTemplateAsync
                    await _templateService.DeleteTemplateAsync(selected.Id);
                    MessageBox.Show("Szablon został usunięty.", "Sukces");
                    await WczytajSzablonyAsync();
                    btnNowy_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd usuwania szablonu: " + ex.Message);
                }
            }
        }

        #region ToolStrip Button Handlers

        private void ZmienStylCzcionki(FontStyle styl)
        {
            if (rtbTrescSzablonu.SelectionFont != null)
            {
                var currentFont = rtbTrescSzablonu.SelectionFont;
                var newStyle = currentFont.Style ^ styl;
                rtbTrescSzablonu.SelectionFont = new Font(currentFont, newStyle);
            }
            rtbTrescSzablonu.Focus();
        }

        private void btnBold_Click(object sender, EventArgs e) => ZmienStylCzcionki(FontStyle.Bold);
        private void btnItalic_Click(object sender, EventArgs e) => ZmienStylCzcionki(FontStyle.Italic);
        private void btnUnderline_Click(object sender, EventArgs e) => ZmienStylCzcionki(FontStyle.Underline);

        private void cmbFontFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null && rtbTrescSzablonu.SelectionFont != null)
            {
                try
                {
                    rtbTrescSzablonu.SelectionFont = new Font(cmbFontFamily.SelectedItem.ToString(), rtbTrescSzablonu.SelectionFont.Size, rtbTrescSzablonu.SelectionFont.Style);
                }
                catch { } // Ignoruj błędy czcionek
                rtbTrescSzablonu.Focus();
            }
        }

        private void cmbFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFontSize.SelectedItem != null && rtbTrescSzablonu.SelectionFont != null)
            {
                try
                {
                    rtbTrescSzablonu.SelectionFont = new Font(rtbTrescSzablonu.SelectionFont.FontFamily, Convert.ToSingle(cmbFontSize.SelectedItem), rtbTrescSzablonu.SelectionFont.Style);
                }
                catch { }
                rtbTrescSzablonu.Focus();
            }
        }
        #endregion
    
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
