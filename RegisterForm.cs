using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public partial class RegisterForm : Form
    {
        private readonly UserService _userService;

        public RegisterForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            var dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            _userService = new UserService(dbService);
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            // Można zostawić puste
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string role = comboRole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Proszę wypełnić wszystkie pola.", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (await _userService.UsernameExistsAsync(username))
                {
                    MessageBox.Show("Nazwa użytkownika jest już zajęta. Wybierz inną.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string hashedPassword = CreateHashedPassword(password);
                await _userService.RegisterUserAsync(username, hashedPassword, fullName, role);

                MessageBox.Show("Zarejestrowano pomyślnie.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close(); // Zamknij formularz po pomyślnej rejestracji
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas rejestracji: " + ex.Message, "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Logika haszowania hasła jest uniwersalna i poprawna.
        private string CreateHashedPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                string hashedPassword = Convert.ToBase64String(hashBytes);
                return hashedPassword;
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