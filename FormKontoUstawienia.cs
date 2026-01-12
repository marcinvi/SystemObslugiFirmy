// Plik: FormKontoUstawienia.cs (Wersja zmodernizowana)
using System;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormKontoUstawienia : Form
    {
        private readonly DatabaseService _dbBaza;
        private readonly long _userId;
        private string _currentPasswordHash;

        public FormKontoUstawienia(long userId)
        {
            _userId = userId;
            InitializeComponent();
            _dbBaza = new DatabaseService(DatabaseHelper.GetConnectionString());

            this.Load += async (_, __) => await LoadUserAsync();
            btnSave.Click += async (_, __) => await SaveUserAsync();

            // Inicjalizujemy kontrolkę delegacji, przekazując ID użytkownika do filtrowania
            var dlg = new DelegacjeControl(_userId);
            tabDelegacje.Controls.Add(dlg);
            dlg.Dock = DockStyle.Fill;

            // Zaktualizowany, bardziej opisowy tekst dla zakładki "Czas Pracy"
            lblWIP.Text = "Moduł 'Czas pracy' jest w przygotowaniu.\n\n" +
                          "W przyszłości znajdziesz tutaj narzędzia do zarządzania\n" +
                          "swoim czasem pracy, urlopami, zwolnieniami chorobowymi\n" +
                          "oraz do ewidencji i rozliczania nadgodzin.";
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async Task LoadUserAsync()
        {
            var dt = await _dbBaza.GetDataTableAsync(
                @"SELECT Id, Login, `Hasło` AS Haslo, `Nazwa Wyświetlana` AS Nazwa, IFNULL(Email,'') AS Email, IFNULL(Rola,'') AS Rola
                  FROM Uzytkownicy WHERE Id=@id",
                new MySqlParameter("@id", _userId));
            if (dt.Rows.Count == 0) { MessageBox.Show("Nie znaleziono użytkownika."); Close(); return; }

            var r = dt.Rows[0];
            txtLogin.Text = Convert.ToString(r["Login"]);

            // Zapisujemy stary hash, a pole hasła jest puste i zamaskowane
            _currentPasswordHash = Convert.ToString(r["Haslo"]);
            txtHaslo.Text = "";

            txtNazwa.Text = Convert.ToString(r["Nazwa"]);
            txtEmail.Text = Convert.ToString(r["Email"]);
            txtRola.Text = Convert.ToString(r["Rola"]);
        }

        private async Task SaveUserAsync()
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Login jest wymagany.", "Walidacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Logika aktualizacji hasła
            string passwordToSave = _currentPasswordHash; // Domyślnie używamy starego hasła
            if (!string.IsNullOrEmpty(txtHaslo.Text))
            {
                // Jeśli użytkownik wpisał nowe hasło, hashujemy je
                passwordToSave = CreateHashedPassword(txtHaslo.Text);
            }

            await _dbBaza.ExecuteNonQueryAsync(@"
                UPDATE Uzytkownicy SET Login=@l, `Hasło`=@h, `Nazwa Wyświetlana`=@n, Email=@e
                WHERE Id=@id",
                new MySqlParameter("@l", txtLogin.Text.Trim()),
                new MySqlParameter("@h", passwordToSave),
                new MySqlParameter("@n", txtNazwa.Text.Trim()),
                new MySqlParameter("@e", txtEmail.Text.Trim()),
                new MySqlParameter("@id", _userId));

            MessageBox.Show("Zmiany zostały zapisane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Aktualizujemy dane w formularzu
            await LoadUserAsync();
        }

        // Metoda do hashowania hasła, zgodna z RegisterForm
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