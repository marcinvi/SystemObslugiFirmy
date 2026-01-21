using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Serwis odpowiedzialny za operacje na użytkownikach (rejestracja, sprawdzenie nazw użytkowników).
    /// </summary>
    public class UserService
    {
        private readonly DatabaseService _dbService;

        public UserService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        /// <summary>
        /// Sprawdza, czy dany login istnieje w tabeli użytkowników.
        /// </summary>
        /// <param name="username">Login do sprawdzenia.</param>
        /// <returns>Zwraca true, jeśli login jest już używany.</returns>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            try
            {
                // ##### POPRAWKA: Użycie poprawnej nazwy kolumny "Login" #####
                const string query = "SELECT COUNT(*) FROM Uzytkownicy WHERE Login = @username";
                var result = await _dbService.ExecuteScalarAsync(
                    query,
                    new MySqlParameter("@username", username));

                return Convert.ToInt64(result) > 0;
            }
            catch (Exception ex)
            {
                // Zmieniono na bardziej informatywny komunikat
                Console.WriteLine($"[UserService] Błąd podczas sprawdzania nazwy użytkownika: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Rejestruje nowego użytkownika.
        /// </summary>
        /// <param name="username">Login</param>
        /// <param name="hashedPassword">Zahasłowane hasło</param>
        /// <param name="fullName">Imię i nazwisko</param>
        /// <param name="role">Rola użytkownika</param>
        public async Task RegisterUserAsync(string username, string hashedPassword, string fullName, string role)
        {
            try
            {
                // ##### POPRAWKA: Użycie poprawnych nazw kolumn z bazy danych #####
                const string query = "INSERT INTO Uzytkownicy (Login, `Hasło`, `Nazwa Wyświetlana`, Rola) VALUES (@login, @haslo, @nazwaWyswietlana, @rola)";

                await _dbService.ExecuteNonQueryAsync(
                    query,
                    new MySqlParameter("@login", username),
                    new MySqlParameter("@haslo", hashedPassword),
                    new MySqlParameter("@nazwaWyswietlana", fullName),
                    new MySqlParameter("@rola", role));

                Console.WriteLine($"[UserService] Zarejestrowano użytkownika {username}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Błąd podczas rejestracji użytkownika: {ex.Message}");
                throw;
            }
        }
    }
}
