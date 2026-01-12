using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public static class Zapisz
    {
        // Nazwa klucza blokady w tabeli Ustawienia
        private const string LockKey = "DatabaseLock";

        public static async Task<bool> Sprawdz()
        {
            while (true)
            {
                bool lockAcquired = false;
                try
                {
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        using (var transaction = con.BeginTransaction())
                        {
                            // Sprawdzamy, czy blokada istnieje i jest aktywna
                            string queryLock = "SELECT WartoscZaszyfrowana FROM Ustawienia WHERE Klucz = @klucz";
                            using (var cmd = new MySqlCommand(queryLock, con))
                            {
                                cmd.Parameters.AddWithValue("@klucz", LockKey);
                                var result = await cmd.ExecuteScalarAsync();

                                if (result == null || result.ToString() == "0")
                                {
                                    // Blokada jest wolna, zajmujemy ją
                                    string updateLock = "INSERT OR REPLACE INTO Ustawienia (Klucz, WartoscZaszyfrowana) VALUES (@klucz, @wartosc)";
                                    using (var updateCmd = new MySqlCommand(updateLock, con))
                                    {
                                        updateCmd.Parameters.AddWithValue("@klucz", LockKey);
                                        updateCmd.Parameters.AddWithValue("@wartosc", "1"); // 1 = zablokowane
                                        await updateCmd.ExecuteNonQueryAsync();
                                        lockAcquired = true;
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                    }

                    if (lockAcquired)
                    {
                        return true; // Sukces, mamy blokadę
                    }
                    else
                    {
                        // Blokada jest zajęta, pytamy użytkownika
                        var resultDialog = MessageBox.Show("Baza danych jest w tej chwili używana przez innego użytkownika.\nSpróbować ponownie?", "Zasób zablokowany", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (resultDialog == DialogResult.No)
                        {
                            return false; // Użytkownik zrezygnował
                        }
                        await Task.Delay(2000); // Czekamy chwilę przed ponowną próbą
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas próby blokady bazy danych: " + ex.Message);
                    return false;
                }
            }
        }

        public static async void Zwolnij()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "UPDATE Ustawienia SET WartoscZaszyfrowana = '0' WHERE Klucz = @klucz";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@klucz", LockKey);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd zwalniania blokady: " + ex.Message);
            }
        }
    }
}