using System;
using MySql.Data.MySqlClient; // ZMIANA: Używamy MySql zamiast SQLite
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public static class Database
    {
        public static void InitializeDatabase()
        {
            try
            {
                using (var connection = GetNewOpenConnection())
                {
                    // W MariaDB tabele tworzymy tylko raz przy migracji (HeidiSQL), 
                    // ale zostawiamy sprawdzanie połączenia.

                    // Przykład jednej tabeli (składnia MariaDB), jeśli chcesz mieć pewność:
                    ExecuteSql(connection, @"
                        CREATE TABLE IF NOT EXISTS `Zgloszenia` (
                            `Id` INT AUTO_INCREMENT PRIMARY KEY,
                            `NrZgloszenia` VARCHAR(255) NOT NULL UNIQUE,
                            `KlientID` INT,
                            `ProduktID` INT,
                            `DataZgloszenia` DATETIME,
                            `OpisUsterki` LONGTEXT
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd połączenia z serwerem MariaDB:\n{ex.Message}", "Błąd Bazy");
                throw;
            }
        }

        private static void ExecuteSql(MySqlConnection conn, string sql)
        {
            using (var command = new MySqlCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        // TO JEST KLUCZOWA ZMIANA:
        public static MySqlConnection GetNewOpenConnection()
        {
            // Pobiera ConnectionString z DbConfig (ten z "Server=localhost...")
            var connection = new MySqlConnection(DbConfig.ConnectionStringBaza);
            connection.Open();
            return connection;
        }
    }
}