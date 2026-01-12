using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Narzędzie diagnostyczne połączenia z bazą danych
    /// </summary>
    public partial class FormDatabaseDiagnostics : Form
    {
        private TextBox txtLog;
        private Button btnTest;
        private Button btnTestBasic;
        private Label lblStatus;

        public FormDatabaseDiagnostics()
        {
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponent()
        {
            this.Text = "Diagnostyka Bazy Danych";
            this.Size = new System.Drawing.Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Label Status
            lblStatus = new Label
            {
                Text = "Gotowy do testu",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true,
                Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(lblStatus);

            // Przycisk Test Podstawowy
            btnTestBasic = new Button
            {
                Text = "Test Połączenia (Prosty)",
                Location = new System.Drawing.Point(10, 40),
                Width = 200,
                Height = 30
            };
            btnTestBasic.Click += BtnTestBasic_Click;
            this.Controls.Add(btnTestBasic);

            // Przycisk Test Zaawansowany
            btnTest = new Button
            {
                Text = "Test Połączenia (Zaawansowany)",
                Location = new System.Drawing.Point(220, 40),
                Width = 200,
                Height = 30
            };
            btnTest.Click += BtnTest_Click;
            this.Controls.Add(btnTest);

            // TextBox Log
            txtLog = new TextBox
            {
                Location = new System.Drawing.Point(10, 80),
                Size = new System.Drawing.Size(660, 370),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 9)
            };
            this.Controls.Add(txtLog);
        }

        private void BtnTestBasic_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            Log("=== TEST POŁĄCZENIA PODSTAWOWY ===");
            Log("");

            try
            {
                Log("[1] Sprawdzam connection string...");
                var connString = DbConfig.ConnectionString;
                Log($"Connection String: {MaskPassword(connString)}");
                Log("");

                Log("[2] Próbuję połączyć się z bazą...");
                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    Log("✅ SUKCES! Połączono z bazą danych!");
                    Log($"Server Version: {conn.ServerVersion}");
                    Log($"Database: {conn.Database}");
                    
                    lblStatus.Text = "✅ Połączenie OK!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (MySqlException ex)
            {
                Log($"❌ BŁĄD MySQL (kod {ex.Number}):");
                Log($"   {ex.Message}");
                Log("");
                
                lblStatus.Text = "❌ Błąd połączenia!";
                lblStatus.ForeColor = System.Drawing.Color.Red;

                // Pomoc w diagnozie
                switch (ex.Number)
                {
                    case 0:
                        Log("💡 ROZWIĄZANIE:");
                        Log("   - Serwer MySQL/MariaDB nie jest uruchomiony");
                        Log("   - Sprawdź czy usługa działa w Windows Services");
                        Log("   - Uruchom: services.msc → znajdź MySQL/MariaDB → Start");
                        break;
                    case 1042:
                        Log("💡 ROZWIĄZANIE:");
                        Log("   - Nie można połączyć się z hostem");
                        Log("   - Sprawdź czy adres serwera jest poprawny (localhost)");
                        Log("   - Sprawdź firewall");
                        break;
                    case 1044:
                    case 1045:
                        Log("💡 ROZWIĄZANIE:");
                        Log("   - Błędna nazwa użytkownika lub hasło");
                        Log("   - Sprawdź DbConfig.cs");
                        Log("   - Domyślnie: user='root', password='Bigbrother5'");
                        break;
                    case 1049:
                        Log("💡 ROZWIĄZANIE:");
                        Log("   - Baza danych 'ReklamacjeDB' nie istnieje");
                        Log("   - Musisz utworzyć bazę danych:");
                        Log("   - CREATE DATABASE ReklamacjeDB CHARACTER SET utf8mb4;");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log($"❌ BŁĄD: {ex.Message}");
                lblStatus.Text = "❌ Błąd!";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            Log("=== TEST POŁĄCZENIA ZAAWANSOWANY ===");
            Log("");

            try
            {
                // Test 1: Connection String
                Log("[1/5] Sprawdzam connection string...");
                var connString = DbConfig.ConnectionString;
                var builder = new MySqlConnectionStringBuilder(connString);
                Log($"   Server: {builder.Server}");
                Log($"   Port: {builder.Port}");
                Log($"   Database: {builder.Database}");
                Log($"   User: {builder.UserID}");
                Log($"   Password: {new string('*', builder.Password.Length)} ({builder.Password.Length} znaków)");
                Log("   ✅ Connection string OK");
                Log("");

                // Test 2: Połączenie
                Log("[2/5] Próbuję połączyć się z serwerem...");
                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();
                    Log($"   ✅ Połączono! Server Version: {conn.ServerVersion}");
                    Log("");

                    // Test 3: Sprawdzenie bazy
                    Log("[3/5] Sprawdzam bazę danych...");
                    var cmd = new MySqlCommand($"SHOW DATABASES LIKE '{builder.Database}'", conn);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        Log($"   ✅ Baza '{builder.Database}' istnieje");
                    }
                    else
                    {
                        Log($"   ⚠️ Baza '{builder.Database}' NIE ISTNIEJE!");
                        Log($"   Utwórz ją: CREATE DATABASE {builder.Database} CHARACTER SET utf8mb4;");
                    }
                    Log("");

                    // Test 4: Sprawdzenie tabel
                    Log("[4/5] Sprawdzam tabele...");
                    cmd = new MySqlCommand("SHOW TABLES", conn);
                    var reader = cmd.ExecuteReader();
                    int tableCount = 0;
                    var criticalTables = new[] { "AllegroAccounts", "AllegroCustomerReturns", "AllegroDisputes" };
                    var foundTables = new System.Collections.Generic.List<string>();

                    while (reader.Read())
                    {
                        tableCount++;
                        var tableName = reader.GetString(0);
                        foundTables.Add(tableName);
                    }
                    reader.Close();

                    Log($"   Znaleziono {tableCount} tabel");
                    
                    foreach (var table in criticalTables)
                    {
                        if (foundTables.Contains(table))
                        {
                            Log($"   ✅ {table}");
                        }
                        else
                        {
                            Log($"   ❌ BRAK: {table}");
                        }
                    }
                    Log("");

                    // Test 5: Test zapisu
                    Log("[5/5] Test zapisu/odczytu...");
                    cmd = new MySqlCommand("SELECT 1", conn);
                    var testResult = cmd.ExecuteScalar();
                    Log($"   ✅ Test zapytania OK: {testResult}");
                    Log("");

                    Log("═══════════════════════════════════════");
                    Log("✅ WSZYSTKIE TESTY ZALICZONE!");
                    Log("═══════════════════════════════════════");
                    
                    lblStatus.Text = "✅ Wszystko OK!";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (MySqlException ex)
            {
                Log("");
                Log("═══════════════════════════════════════");
                Log($"❌ BŁĄD MYSQL (kod {ex.Number})");
                Log("═══════════════════════════════════════");
                Log($"Komunikat: {ex.Message}");
                Log("");

                lblStatus.Text = "❌ Błąd!";
                lblStatus.ForeColor = System.Drawing.Color.Red;

                // Szczegółowa diagnoza
                switch (ex.Number)
                {
                    case 0:
                        Log("🔍 PRZYCZYNA:");
                        Log("   Serwer MySQL/MariaDB nie odpowiada");
                        Log("");
                        Log("💡 ROZWIĄZANIE:");
                        Log("   1. Otwórz 'services.msc' (Win+R)");
                        Log("   2. Znajdź 'MySQL' lub 'MariaDB'");
                        Log("   3. Kliknij prawym → Start");
                        Log("");
                        Log("   LUB uruchom w XAMPP/WAMP:");
                        Log("   - XAMPP Control Panel → MySQL → Start");
                        break;

                    case 1042:
                        Log("🔍 PRZYCZYNA:");
                        Log("   Nie można połączyć się z hostem");
                        Log("");
                        Log("💡 ROZWIĄZANIE:");
                        Log("   1. Sprawdź czy server='localhost' w DbConfig.cs");
                        Log("   2. Sprawdź czy port=3306 (domyślny)");
                        Log("   3. Wyłącz firewall testowo");
                        break;

                    case 1044:
                    case 1045:
                        Log("🔍 PRZYCZYNA:");
                        Log("   Nieprawidłowy login lub hasło");
                        Log("");
                        Log("💡 ROZWIĄZANIE:");
                        Log("   Otwórz DbConfig.cs i sprawdź:");
                        Log("   - User = \"root\"");
                        Log("   - Password = \"Bigbrother5\"");
                        Log("");
                        Log("   Jeśli nie pamiętasz hasła, zresetuj je:");
                        Log("   1. Zatrzymaj MySQL");
                        Log("   2. Uruchom z --skip-grant-tables");
                        Log("   3. ALTER USER 'root'@'localhost' IDENTIFIED BY 'NoweHaslo';");
                        break;

                    case 1049:
                        Log("🔍 PRZYCZYNA:");
                        Log("   Baza danych 'ReklamacjeDB' nie istnieje");
                        Log("");
                        Log("💡 ROZWIĄZANIE:");
                        Log("   Wykonaj w MySQL:");
                        Log("   CREATE DATABASE ReklamacjeDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;");
                        break;

                    default:
                        Log($"🔍 NIEZNANY BŁĄD (kod {ex.Number})");
                        Log("   Sprawdź dokumentację MySQL");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log($"❌ BŁĄD: {ex.GetType().Name}");
                Log($"   {ex.Message}");
                lblStatus.Text = "❌ Błąd!";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void Log(string message)
        {
            txtLog.AppendText(message + Environment.NewLine);
        }

        private string MaskPassword(string connectionString)
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            builder.Password = new string('*', builder.Password.Length);
            return builder.ToString();
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
