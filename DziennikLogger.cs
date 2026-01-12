using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class DziennikLogger
    {
        private readonly DatabaseService _dbService;

        public DziennikLogger()
        {
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        }

        public async Task DodajAsync(string uzytkownik, string akcja, string dotyczyZgloszenia)
        {
            try
            {
                string query = "INSERT INTO Dziennik (Data, Uzytkownik, Akcja, DotyczyZgloszenia) VALUES (@data, @uzytkownik, @akcja, @nrZgloszenia)";
                var parameters = new[]
                {
                    new MySqlParameter("@data", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new MySqlParameter("@uzytkownik", uzytkownik),
                    new MySqlParameter("@akcja", akcja),
                    new MySqlParameter("@nrZgloszenia", dotyczyZgloszenia)
                };
                await _dbService.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD KRYTYCZNY: Nie udało się zapisać wpisu do dziennika w bazie danych. Szczegóły: {ex.Message}");
            }
        }

        public async Task DodajAsync(MySqlConnection con, MySqlTransaction transaction, string uzytkownik, string akcja, string dotyczyZgloszenia)
        {
            try
            {
                string query = "INSERT INTO Dziennik (Data, Uzytkownik, Akcja, DotyczyZgloszenia) VALUES (@data, @uzytkownik, @akcja, @nrZgloszenia)";
                var parameters = new[]
                {
                    new MySqlParameter("@data", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                    new MySqlParameter("@uzytkownik", uzytkownik),
                    new MySqlParameter("@akcja", akcja),
                    new MySqlParameter("@nrZgloszenia", dotyczyZgloszenia)
                };
                await _dbService.ExecuteNonQueryAsync(con, transaction, query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD KRYTYCZNY: Nie udało się zapisać wpisu do dziennika w bazie danych. Szczegóły: {ex.Message}");
            }
        }
    }
}
