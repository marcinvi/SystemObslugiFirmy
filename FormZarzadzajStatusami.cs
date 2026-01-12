// Plik: FormZarzadzajStatusami.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormZarzadzajStatusami : Form
    {
        private readonly StatusService _statusService;
        private readonly string _typStatusu = "DecyzjaHandlowca";

        public FormZarzadzajStatusami()
        {
            InitializeComponent();
            _statusService = new StatusService(MagazynDatabaseHelper.GetConnectionString(), _typStatusu);
            this.Load += FormZarzadzajStatusami_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormZarzadzajStatusami_Load(object sender, EventArgs e)
        {
            await LoadStatusesAsync();
        }

        private async Task LoadStatusesAsync()
        {
            listBoxStatusy.Items.Clear();
            try
            {
                var statuses = await _statusService.GetStatusesAsync();
                foreach (var status in statuses)
                {
                    listBoxStatusy.Items.Add(status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania statusów: " + ex.Message);
            }
        }

        private void listBoxStatusy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxStatusy.SelectedItem != null)
            {
                txtNazwaStatusu.Text = listBoxStatusy.SelectedItem.ToString();
            }
        }

        private async void btnDodaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNazwaStatusu.Text)) return;
            try
            {
                await _statusService.AddStatusAsync(txtNazwaStatusu.Text.Trim());
                txtNazwaStatusu.Clear();
                await LoadStatusesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie można dodać statusu (prawdopodobnie już istnieje).\n\n" + ex.Message, "Błąd");
            }
        }

        private async void btnUsun_Click(object sender, EventArgs e)
        {
            if (listBoxStatusy.SelectedItem == null) return;
            string selectedStatus = listBoxStatusy.SelectedItem.ToString();

            // Zabezpieczenie przed usunięciem stałych opcji
            if (selectedStatus == "Całe na stan" || selectedStatus == "Przekaż do reklamacji")
            {
                MessageBox.Show("Nie można usunąć domyślnych statusów.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Czy na pewno chcesz usunąć ten status?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    await _statusService.DeleteStatusAsync(selectedStatus);
                    txtNazwaStatusu.Clear();
                    await LoadStatusesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas usuwania statusu: " + ex.Message);
                }
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
