// ############################################################################
// Plik: LoginForm.cs (WERSJA POPRAWIONA - FIX ASYNC VOID)
// Opis: Ładowanie użytkowników jest teraz bezpieczne i nie blokuje bazy
//       podczas próby logowania.
// ############################################################################

using System;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks; // Ważne dla Task
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class LoginForm : Form
    {
        public static string fullName;
        public static string login;

        public string UserRole { get; private set; }

        private bool _isLoadingUsernames = false;

        public LoginForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

            // Odpinamy eventy designera, żeby nie strzelały przypadkiem
            comboUsername.SelectedIndexChanged -= new EventHandler(btnLogin_Click);

            this.KeyPreview = true;
            this.KeyDown += LoginForm_KeyDown;

            // PRZENIESIONO LoadUsernames z konstruktora do zdarzenia Load
            this.Load += LoginForm_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
          
        }

        // ZMIANA: async void jest tutaj OK, bo to Event Handler
        private async void LoginForm_Load(object sender, EventArgs e)
        {
            if (_isLoadingUsernames) return; // Zabezpieczenie przed wielokrotnym wywołaniem
            _isLoadingUsernames = true;

            // Blokujemy przycisk logowania na czas ładowania bazy
            btnLogin.Enabled = false;
            await LoadUsernamesAsync();
            btnLogin.Enabled = true;

            _isLoadingUsernames = false;
        }

        // ZMIANA: async Task zamiast async void - pozwala na czekanie (await)
        private async Task LoadUsernamesAsync()
        {
            comboUsername.Items.Clear();
            try
            {
                var dbService = new DatabaseService(DatabaseHelper.GetConnectionString());

                // Używamy GetDataTableAsync, które otwiera i ZAMYKA połączenie
                System.Data.DataTable dt = await dbService.GetDataTableAsync("SELECT DISTINCT Login FROM Uzytkownicy ORDER BY Login");

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    comboUsername.Items.Add(row["Login"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas pobierania nazw użytkowników z bazy danych: " + ex.Message);
            }
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnLogin.PerformClick();
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = comboUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Proszę wprowadzić nazwę użytkownika i hasło.");
                return;
            }

            try
            {
                // Blokujemy UI na czas logowania, żeby user nie klikał 2 razy
                btnLogin.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                string storedHashedPassword = null;
                string fullNameFromDb = null;
                string userRoleFromDb = null;
                int userId = -1;

                var dbService = new DatabaseService(DatabaseHelper.GetConnectionString());

                // Sprawdzenie użytkownika
                System.Data.DataTable dt = await dbService.GetDataTableAsync(
                    "SELECT Id, Hasło, `Nazwa Wyświetlana`, Rola FROM Uzytkownicy WHERE Login = @username",
                    new MySqlParameter("@username", username)
                );

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    userId = Convert.ToInt32(row["Id"]);
                    storedHashedPassword = row["Hasło"].ToString();
                    fullNameFromDb = row["Nazwa Wyświetlana"].ToString();
                    userRoleFromDb = row["Rola"].ToString();
                    login = username;
                }
                else
                {
                    MessageBox.Show("Nieprawidłowa nazwa użytkownika lub hasło.");
                    return;
                }

                // Weryfikacja hasła (CPU-bound, nie blokuje bazy)
                if (VerifyHashedPassword(password, storedHashedPassword))
                {
                    Program.fullName = fullNameFromDb;
                    SessionManager.Login(userId, username);
                    this.UserRole = userRoleFromDb;

                    // Logowanie do dziennika (To jest ten moment ZAPISU)
                    // Teraz jest bezpieczny, bo LoadUsernamesAsync na pewno już dawno zamknęło swoje połączenie.
                    await new DziennikLogger().DodajAsync(Program.fullName, "Zalogowano", "0");

                    this.DialogResult = DialogResult.OK;
                    // Close wywoła się samoczynnie po ustawieniu DialogResult w trybie modalnym, 
                    // ale dla pewności w kodzie async zostawiamy return/close na koniec
                }
                else
                {
                    MessageBox.Show("Nieprawidłowa nazwa użytkownika lub hasło.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas logowania: " + ex.Message);
            }
            finally
            {
                btnLogin.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        // --- METODY POMOCNICZE DO HASŁA (Bez zmian) ---
        private bool VerifyHashedPassword(string enteredPassword, string storedHashedPassword)
        {
            if (string.IsNullOrWhiteSpace(storedHashedPassword)) return false;

            string candidate = storedHashedPassword.Trim();
            int sp = candidate.LastIndexOf(' ');
            if (sp >= 0 && sp < candidate.Length - 1) candidate = candidate.Substring(sp + 1);

            try
            {
                byte[] hashBytes = Convert.FromBase64String(candidate);
                if (hashBytes.Length >= 36)
                {
                    byte[] salt = new byte[16];
                    Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);
                    using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000))
                    {
                        byte[] hash = pbkdf2.GetBytes(hashBytes.Length - 16);
                        for (int i = 0; i < hash.Length; i++) if (hashBytes[i + 16] != hash[i]) return false;
                    }
                    return true;
                }
                // Obsługa starych haseł (SHA256 bez soli)
                if (hashBytes.Length == 32)
                {
                    using (var sha = SHA256.Create())
                    {
                        var h = sha.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                        for (int i = 0; i < 32; i++) if (hashBytes[i] != h[i]) return false;
                        return true;
                    }
                }
            }
            catch { }

            // Obsługa Hex (stare systemy)
            if (candidate.Length == 64 && IsHex(candidate))
            {
                byte[] raw = HexToBytes(candidate);
                using (var sha = SHA256.Create())
                {
                    var h = sha.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                    for (int i = 0; i < 32; i++) if (raw[i] != h[i]) return false;
                    return true;
                }
            }
            return string.Equals(storedHashedPassword, enteredPassword, StringComparison.Ordinal);
        }

        private static bool IsHex(string s)
        {
            foreach (char c in s) if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))) return false;
            return true;
        }

        private static byte[] HexToBytes(string s)
        {
            int len = s.Length / 2;
            var bytes = new byte[len];
            for (int i = 0; i < len; i++) bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            return bytes;
        }

        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            using (RegisterForm registerForm = new RegisterForm())
            {
                registerForm.ShowDialog();
            }
            this.Show();
            // Ponowne ładowanie po rejestracji
            _ = LoadUsernamesAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void sprawdz_Click(object sender, EventArgs e)
        {
            using (var pisownia = new FormSpellCheckTest())
            {
                pisownia.ShowDialog(this);
            }
        }
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
       
}
}