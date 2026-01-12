using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Obiekt reprezentujący jedno konto Allegro przechowywane w bazie.
    /// </summary>
    public class AllegroAccountData
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecretEncrypted { get; set; }
        public string AccessTokenEncrypted { get; set; }
        public string RefreshTokenEncrypted { get; set; }
        public DateTime TokenExpirationDate { get; set; }
        public bool IsAuthorized { get; set; }
    }

    /// <summary>
    /// Serwis obsługujący operacje CRUD na tabeli AllegroAccounts.
    /// </summary>
    public class AllegroAccountService
    {
        private readonly DatabaseService _databaseService;
        private readonly DziennikLogger _logger = new DziennikLogger();

        public AllegroAccountService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Pobiera listę kont Allegro do weryfikacji przy starcie aplikacji.
        /// </summary>
        public async Task<List<AllegroAccountData>> GetAccountsForStartupAuthAsync()
        {
            const string query =
                "SELECT Id, AccountName, ClientId, ClientSecretEncrypted, AccessTokenEncrypted, " +
                "RefreshTokenEncrypted, TokenExpirationDate, IsAuthorized FROM AllegroAccounts;";
            var accounts = new List<AllegroAccountData>();
            try
            {
                DataTable table = await _databaseService.GetDataTableAsync(query);
                foreach (DataRow row in table.Rows)
                {
                    accounts.Add(new AllegroAccountData
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        AccountName = row["AccountName"].ToString(),
                        ClientId = row["ClientId"].ToString(),
                        ClientSecretEncrypted = row["ClientSecretEncrypted"].ToString(),
                        AccessTokenEncrypted = row["AccessTokenEncrypted"]?.ToString(),
                        RefreshTokenEncrypted = row["RefreshTokenEncrypted"]?.ToString(),
                        TokenExpirationDate = row["TokenExpirationDate"] != DBNull.Value
                            ? Convert.ToDateTime(row["TokenExpirationDate"])
                            : DateTime.MinValue,
                        IsAuthorized = Convert.ToBoolean(row["IsAuthorized"])
                    });
                }
            }
            catch (Exception ex)
            {
                // rejestrowanie błędu w dzienniku i na konsolę
                await _logger.DodajAsync(Program.fullName,
                    $"Błąd pobierania kont Allegro: {ex.Message}", "0");
                Console.WriteLine($"Błąd pobierania kont Allegro: {ex.Message}");
            }
            return accounts;
        }

        /// <summary>
        /// Zapisuje odświeżony token dla wybranego konta Allegro.
        /// </summary>
        public async Task SaveRefreshedTokenAsync(int accountId, Reklamacje_Dane.Allegro.AllegroToken token)
        {
            const string query =
                "UPDATE AllegroAccounts SET AccessTokenEncrypted = @access, RefreshTokenEncrypted = @refresh, " +
                "TokenExpirationDate = @expires, IsAuthorized = 1 WHERE Id = @id";
            string encryptedAccessToken = EncryptionHelper.EncryptString(token.AccessToken);
            string encryptedRefreshToken = EncryptionHelper.EncryptString(token.RefreshToken);
            var parameters = new[]
            {
                new MySqlParameter("@access", encryptedAccessToken),
                new MySqlParameter("@refresh", encryptedRefreshToken),
                new MySqlParameter("@expires", token.ExpirationDate),
                new MySqlParameter("@id", accountId)
            };
            try
            {
                await _databaseService.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                await _logger.DodajAsync(Program.fullName,
                    $"Błąd zapisu tokena Allegro: {ex.Message}", "0");
                Console.WriteLine($"Błąd zapisu tokena Allegro: {ex.Message}");
            }
        }

        /// <summary>
        /// Oznacza konto jako nieautoryzowane i czyści tokeny.
        /// </summary>
        public async Task MarkAccountAsUnauthorizedAsync(int accountId)
        {
            const string query =
                "UPDATE AllegroAccounts SET IsAuthorized = 0, AccessTokenEncrypted = NULL, " +
                "RefreshTokenEncrypted = NULL WHERE Id = @id";
            var parameters = new[] { new MySqlParameter("@id", accountId) };
            try
            {
                await _databaseService.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                await _logger.DodajAsync(Program.fullName,
                    $"Błąd oznaczania konta jako nieautoryzowane: {ex.Message}", "0");
                Console.WriteLine($"Błąd oznaczania konta jako nieautoryzowane: {ex.Message}");
            }
        }
    }
}
