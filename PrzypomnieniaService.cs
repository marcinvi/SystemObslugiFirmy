using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class PrzypomnieniaService
    {
        private readonly DatabaseService _db;

        public PrzypomnieniaService(DatabaseService db)
        {
            _db = db;
        }

        public async Task<DataTable> GetRemindersAsync(string filterMode, string currentUser)
        {
            // ZMIANA: Obsługa statusów z Twojej bazy ('Nowe' lub NULL = aktywne)
            // Jeśli Status jest NULL lub pusty lub 'Nowe' -> to jest aktywne zadanie
            // Jeśli Status to 'Completed' -> to jest zakończone

            string baseCondition = "(Status = 'Nowe' OR Status IS NULL OR Status = 'Active' OR Status = '')";

            string sql = "SELECT * FROM Przypomnienia WHERE 1=1";
            var p = new MySqlParameter("@user", currentUser);

            switch (filterMode)
            {
                case "Moje aktywne":
                    // Aktywne ORAZ (Przypisane do mnie LUB puste)
                    sql += $" AND {baseCondition} AND (PrzypisanyUzytkownik = @user OR PrzypisanyUzytkownik IS NULL OR PrzypisanyUzytkownik = '')";
                    break;
                case "Wszystkie aktywne":
                    sql += $" AND {baseCondition}";
                    break;
                case "Zrealizowane dzisiaj":
                    sql += " AND Status = 'Completed' AND DATE(DataOstatniejAkcji) = CURDATE()";
                    break;
                case "Zaległe":
                    sql += $" AND {baseCondition} AND DataPrzypomnienia < NOW()";
                    break;
            }

            // Sortowanie: Najpierw NULL/Wysoki priorytet, potem data (NULL last)
            sql += " ORDER BY CASE WHEN Priorytet = 'Wysoki' THEN 1 ELSE 2 END, DataPrzypomnienia ASC";

            return await _db.GetDataTableAsync(sql, p);
        }

        public async Task MarkAsDoneAsync(int id, string username)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var trans = con.BeginTransaction())
            {
                try
                {
                    string sql = "UPDATE Przypomnienia SET Status = 'Completed', ZrealizowanePrzez = @user, DataOstatniejAkcji = @now WHERE Id = @id";
                    using (var cmd = new MySqlCommand(sql, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@id", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    trans.Commit();
                }
                catch { trans.Rollback(); throw; }
            }
        }

        public async Task SnoozeAsync(int id, int daysToAdd, string username)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var trans = con.BeginTransaction())
            {
                try
                {
                    string newDate = DateTime.Now.AddDays(daysToAdd).ToString("yyyy-MM-dd HH:mm:ss");
                    // Przy snooze resetujemy status na 'Nowe', jeśli był inny
                    string sql = "UPDATE Przypomnienia SET DataPrzypomnienia = @date, Status = 'Nowe', OstatniaAkcjaPrzez = @user, DataOstatniejAkcji = @now WHERE Id = @id";

                    using (var cmd = new MySqlCommand(sql, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@date", newDate);
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@id", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    trans.Commit();
                }
                catch { trans.Rollback(); throw; }
            }
        }
        // Nowa metoda do ustawiania dokładnej daty
        public async Task SnoozeExactAsync(int id, DateTime exactDate, string username)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var trans = con.BeginTransaction())
            {
                try
                {
                    // Resetujemy status na 'Nowe', żeby system znów to wykrył o zadanej godzinie
                    string sql = "UPDATE Przypomnienia SET DataPrzypomnienia = @date, Status = 'Nowe', OstatniaAkcjaPrzez = @user, DataOstatniejAkcji = @now WHERE Id = @id";

                    using (var cmd = new MySqlCommand(sql, con, trans))
                    {
                        cmd.Parameters.AddWithValue("@date", exactDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@id", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    trans.Commit();
                }
                catch { trans.Rollback(); throw; }
            }
        }

        // Dodaję metodę dla FormDodajPrzypomnienie
        public async Task AddAsync(string tresc, string nrZgloszenia, string data, string priorytet, string dlaKogo)
        {
            string sql = @"INSERT INTO Przypomnienia (Tresc, DotyczyZgloszenia, DataPrzypomnienia, Priorytet, PrzypisanyUzytkownik, Status, KtoUtworzyl, created_at) 
                           VALUES (@tresc, @nr, @data, @prio, @user, 'Nowe', @creator, @now)";

            await _db.ExecuteNonQueryAsync(sql,
                new MySqlParameter("@tresc", tresc),
                new MySqlParameter("@nr", nrZgloszenia),
                new MySqlParameter("@data", data),
                new MySqlParameter("@prio", priorytet),
                new MySqlParameter("@user", dlaKogo),
                new MySqlParameter("@creator", Program.fullName),
                new MySqlParameter("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}