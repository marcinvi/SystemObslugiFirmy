using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormDodajEdytujKontoAllegro : Form
    {
        private readonly int? accountId; // Null, jeśli dodajemy nowe konto

        public FormDodajEdytujKontoAllegro(int? id = null)
        {
            InitializeComponent();
            this.accountId = id;
            this.StartPosition = FormStartPosition.CenterParent;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormDodajEdytujKontoAllegro_Load(object sender, EventArgs e)
        {
            if (accountId.HasValue)
            {
                this.Text = "Edytuj Konto Allegro";
                await LoadAccountData();
            }
            else
            {
                this.Text = "Dodaj Nowe Konto Allegro";
            }
        }

        private async Task LoadAccountData()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("SELECT AccountName, ClientId, ClientSecretEncrypted FROM AllegroAccounts WHERE Id = @id", con);
                    cmd.Parameters.AddWithValue("@id", accountId.Value);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            txtAccountName.Text = reader["AccountName"].ToString();
                            txtClientId.Text = reader["ClientId"].ToString();

                            string encryptedSecret = reader["ClientSecretEncrypted"].ToString();
                            txtClientSecret.Text = EncryptionHelper.DecryptString(encryptedSecret);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wczytywania danych konta: {ex.Message}", "Błąd krytyczny");
                this.Close();
            }
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountName.Text) ||
                string.IsNullOrWhiteSpace(txtClientId.Text) ||
                string.IsNullOrWhiteSpace(txtClientSecret.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd Walidacji");
                return;
            }

            string encryptedSecret = EncryptionHelper.EncryptString(txtClientSecret.Text);

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    MySqlCommand cmd;

                    if (accountId.HasValue) // Tryb Edycji
                    {
                        cmd = new MySqlCommand("UPDATE AllegroAccounts SET AccountName = @name, ClientId = @clientId, ClientSecretEncrypted = @secret WHERE Id = @id", con);
                        cmd.Parameters.AddWithValue("@id", accountId.Value);
                    }
                    else // Tryb Dodawania
                    {
                        cmd = new MySqlCommand("INSERT INTO AllegroAccounts (AccountName, ClientId, ClientSecretEncrypted) VALUES (@name, @clientId, @secret)", con);
                    }

                    cmd.Parameters.AddWithValue("@name", txtAccountName.Text.Trim());
                    cmd.Parameters.AddWithValue("@clientId", txtClientId.Text.Trim());
                    cmd.Parameters.AddWithValue("@secret", encryptedSecret);

                    await cmd.ExecuteNonQueryAsync();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu danych konta: {ex.Message}", "Błąd krytyczny");
            }
        }

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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