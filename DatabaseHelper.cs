// ############################################################################
// Plik: DatabaseHelper.cs (POPRAWIONY)
// ############################################################################

using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Reklamacje_Dane.Allegro;

namespace Reklamacje_Dane
{
    public static class DatabaseHelper
    {
        // Pobieramy bezpieczny string z DbConfig
        public static string BazaConnectionString => DbConfig.ConnectionStringBaza;

        // Metoda kompatybilności (używana w wielu formularzach)
        public static string GetConnectionString() => BazaConnectionString;

        /// <summary>
        /// Zwraca nowe połączenie.
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(BazaConnectionString);
        }

        /// <summary>
        /// Zwraca nowe połączenie (alias dla GetConnection, używany w kodzie async).
        /// </summary>
        public static MySqlConnection GetConnectionAsync()
        {
            return new MySqlConnection(BazaConnectionString);
        }

        // --- Obsługa Allegro (bez zmian w logice, tylko użycie bezpiecznego połączenia) ---

        public static async Task<List<dynamic>> GetAuthorizedAccountsForTokenRefreshAsync()
        {
            var accounts = new List<dynamic>();
            var query = "SELECT Id, AccountName, AccessTokenEncrypted, RefreshTokenEncrypted, TokenExpirationDate FROM AllegroAccounts WHERE IsAuthorized = 1";

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        accounts.Add(new
                        {
                            Id = reader.GetInt32(0),
                            AccountName = reader.GetString(1),
                            AccessTokenEncrypted = reader.GetString(2),
                            RefreshTokenEncrypted = reader.GetString(3),
                            TokenExpirationDate = reader.GetString(4)
                        });
                    }
                }
            }
            return accounts;
        }

        public static Task<AllegroApiClient> GetApiClientForAccountAsync(int accountId, MySqlConnection con)
        {
            // Ważne: używamy istniejącego połączenia przekazanego w parametrze
            var svc = new AllegroApiService(con);
            return svc.CreateClientForAccountAsync(accountId);
        }

        public static Task SaveRefreshedTokenAsync(int accountId, string accessToken, string refreshToken, DateTime expiresAtUtc)
        {
            var svc = new AllegroApiService(BazaConnectionString);
            return svc.SaveRefreshedTokenAsync(accountId, accessToken, refreshToken, expiresAtUtc);
        }

        public static void EnsureInitialized()
        {
            _ = BazaConnectionString;
        }
    }
}