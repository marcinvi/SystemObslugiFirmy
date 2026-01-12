using System;
using MySql.Data.MySqlClient; // POTRZEBNE DO POBRANIA MAILA
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    static class Program
    {
        private const string ApplicationSecret = "Twoje-Super-Tajne-Haslo-Aplikacji-123!@#";

        // Zmienne globalne użytkownika
        public static string fullName;
        public static string currentUserEmail; // <--- NOWE POLE

        [STAThread]
        static void Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // ############################################################################
            // ## KROK 1: Inicjalizacja bazy danych                                      ##
            // ############################################################################
            try
            {
                Database.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd krytyczny podczas inicjalizacji bazy danych:\n\n{ex.Message}\n\nAplikacja zostanie zamknięta.",
                    "Błąd Połączenia z Bazą Danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Idle += (s, e) =>
            {
                foreach (Form form in Application.OpenForms)
                {
                    SpellCheckManager.EnableSpellCheckForAllTextBoxes(form);
                }
            };

            // --- Ustawienie klucza szyfrowania ---
            try
            {
                using (var kdf = new Rfc2898DeriveBytes(
                    ApplicationSecret,
                    EncryptionHelper.Salt,
                    50_000,
                    HashAlgorithmName.SHA256))
                {
                    var key = kdf.GetBytes(32);
                    EncryptionHelper.MasterKey = key;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd inicjalizacji klucza: {ex.Message}",
                    "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- Tworzenie kopii zapasowej przy starcie ---
            try
            {
                BackupService.CreateBackupAsync(showSuccessMessage: false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się utworzyć kopii zapasowej baz danych:\n{ex.Message}",
                    "Błąd kopii zapasowej", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // --- Pętla aplikacji: Login -> (Sync) -> Dashboard ---
            while (true)
            {
                // Resetujemy maila przed każdym logowaniem
                currentUserEmail = "";

                using (var loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                        break;

                    // Zabezpieczenie nazwy
                    if (string.IsNullOrWhiteSpace(fullName))
                        fullName = Environment.UserName;

                    var userRole = loginForm.UserRole;

                    // ######################################################################
                    // ## NOWOŚĆ: POBIERANIE MAILA ZALOGOWANEGO UŻYTKOWNIKA Z BAZY DANYCH ##
                    // ######################################################################
                    // ######################################################################
                    // ## POPRAWIONE: POBIERANIE MAILA ZALOGOWANEGO UŻYTKOWNIKA ##
                    // ######################################################################
                    try
                    {
                        using (var conn = new MySqlConnection(DbConfig.ConnectionString))
                        {
                            conn.Open();
                            string sql = "SELECT Email FROM Uzytkownicy WHERE `Nazwa Wyświetlana` = @nazwa LIMIT 1";

                            using (var cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@nazwa", fullName);
                                var result = cmd.ExecuteScalar();

                                if (result != null && result != DBNull.Value)
                                {
                                    currentUserEmail = result.ToString();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Błąd pobierania maila użytkownika: " + ex.Message);
                    }
                    // ######################################################################


                    try
                    {
                        using (var sync = new SyncProgressForm())
                        {
                            var r = sync.ShowDialog();
                            if (r == DialogResult.Abort || r == DialogResult.Cancel)
                                continue;
                        }
                    }
                    catch
                    {
                        // Celowo puste - jeśli formularz nie istnieje, idź dalej.
                    }

                    using (var dashboard = new FormDashboard(fullName, userRole))
                    {
                        dashboard.ShowDialog();

                        if (dashboard.DialogResult == DialogResult.Retry)
                            continue;

                        break;
                    }
                }
            }

            Application.Exit();
        }
    }
}
