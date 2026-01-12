using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormSzablonyDzialan : Form
    {
        private readonly DatabaseService _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());

        public FormSzablonyDzialan()
        {
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormSzablonyDzialan_Load(object sender, EventArgs e)
        {
            await LoadTemplatesAsync();
        }

        private async Task LoadTemplatesAsync()
        {
            listBoxTemplates.Items.Clear();
            var dt = await _dbService.GetDataTableAsync("SELECT Tresc FROM SzablonyDzialan ORDER BY Kolejnosc, Tresc");
            foreach (DataRow row in dt.Rows)
            {
                listBoxTemplates.Items.Add(row["Tresc"].ToString());
            }
        }

        private void listBoxTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplates.SelectedItem != null)
            {
                txtTemplate.Text = listBoxTemplates.SelectedItem.ToString();
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTemplate.Text)) return;
            try
            {
                await _dbService.ExecuteNonQueryAsync("INSERT INTO SzablonyDzialan (Tresc) VALUES (@tresc)", new MySqlParameter("@tresc", txtTemplate.Text.Trim()));
                txtTemplate.Clear();
                await LoadTemplatesAsync();
            }
            catch (Exception ex) { MessageBox.Show("Nie można dodać szablonu (prawdopodobnie już istnieje).\n\n" + ex.Message, "Błąd"); }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (listBoxTemplates.SelectedItem == null || string.IsNullOrWhiteSpace(txtTemplate.Text)) return;
            string oldText = listBoxTemplates.SelectedItem.ToString();
            await _dbService.ExecuteNonQueryAsync("UPDATE SzablonyDzialan SET Tresc = @new WHERE Tresc = @old",
                new MySqlParameter("@new", txtTemplate.Text.Trim()),
                new MySqlParameter("@old", oldText));
            await LoadTemplatesAsync();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBoxTemplates.SelectedItem == null) return;
            if (MessageBox.Show("Czy na pewno chcesz usunąć ten szablon?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await _dbService.ExecuteNonQueryAsync("DELETE FROM SzablonyDzialan WHERE Tresc = @tresc", new MySqlParameter("@tresc", listBoxTemplates.SelectedItem.ToString()));
                txtTemplate.Clear();
                await LoadTemplatesAsync();
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