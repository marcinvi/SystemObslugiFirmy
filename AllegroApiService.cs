using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane.Allegro
{
    /// <summary>
    /// Cienki serwis pomocniczy:
    /// - tworzy i inicjalizuje AllegroApiClient dla danego konta,
    /// - zapisuje odświeżone tokeny do bazy (w kolumnach *Encrypted* + ISO 8601 dla daty).
    /// Używa DatabaseService, więc działa poprawnie na udziale sieciowym (DELETE, timeout, retry).
    /// </summary>
    public sealed class AllegroApiService
    {
        private readonly DatabaseService _db;
        // Dodano pole do przechowywania istniejącego połączenia
        private readonly MySqlConnection _con;

        // Istniejący konstruktor, używany do operacji poza głównym cyklem synchronizacji
        public AllegroApiService(string connectionString)
        {
            _db = new DatabaseService(connectionString);
        }

        // NOWY KONSTRUKTOR: Używany do operacji w ramach jednego połączenia
        public AllegroApiService(MySqlConnection con)
        {
            _con = con ?? throw new ArgumentNullException(nameof(con));
        }

        /// <summary>
        /// Zwraca w pełni gotowego klienta Allegro (z wczytanym/ewentualnie odświeżonym tokenem).
        /// Konstruktor klienta to: AllegroApiClient(string clientId, string clientSecret).
        /// </summary>
        public async Task<AllegroApiClient> CreateClientForAccountAsync(int accountId)
        {
            // Nazwy kolumn zgodne z Twoim kodem AllegroApiClient / bazą:
            // ClientId, ClientSecretEncrypted (sekret zaszyfrowany)
            const string sql = @"SELECT ClientId, ClientSecretEncrypted
                                 FROM AllegroAccounts
                                 WHERE Id = @id
                                 LIMIT 1";

            DataTable dt;

            // Sprawdzamy, czy używamy istniejącego połączenia, czy tworzymy nowe
            if (_con != null)
            {
                using (var command = new MySqlCommand(sql, _con))
                {
                    command.Parameters.AddWithValue("@id", accountId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        dt = new DataTable();
                        dt.Load(reader);
                    }
                }
            }
            else
            {
                dt = await _db.GetDataTableAsync(sql, new MySqlParameter("@id", accountId)).ConfigureAwait(false);
            }

            if (dt.Rows.Count == 0)
                throw new InvalidOperationException("Nie znaleziono konta Allegro o Id=" + accountId);

            var row = dt.Rows[0];
            var clientId = Convert.ToString(row["ClientId"]);
            var clientSecretEncrypted = Convert.ToString(row["ClientSecretEncrypted"]);
            var clientSecret = EncryptionHelper.DecryptString(clientSecretEncrypted);

            // Tworzymy klienta dokładnie wg Twojego konstruktora:
            var client = new AllegroApiClient(clientId, clientSecret);

            // To wczyta AccessTokenEncrypted/RefreshTokenEncrypted/TokenExpirationDate
            // i samo odświeży token, jeśli kończy się za 5 minut.
            await client.InitializeAsync(accountId).ConfigureAwait(false);

            return client;
        }

        /// <summary>
        /// Zapisuje odświeżone tokeny do bazy w kolumnach *Encrypted* oraz datę w ISO 8601 (zgodnie z Twoim kodem).
        /// </summary>
        public async Task SaveRefreshedTokenAsync(int accountId, string accessToken, string refreshToken, DateTime expiresAtUtc)
        {
            var accessEnc = EncryptionHelper.EncryptString(accessToken ?? string.Empty);
            string refreshEnc = null;
            if (!string.IsNullOrEmpty(refreshToken))
                refreshEnc = EncryptionHelper.EncryptString(refreshToken);

            const string sql = @"
UPDATE AllegroAccounts
SET AccessTokenEncrypted   = @a,
    RefreshTokenEncrypted  = COALESCE(@r, RefreshTokenEncrypted),
    TokenExpirationDate    = @e,
    IsAuthorized           = 1
WHERE Id = @id";

            // Uwaga: TokenExpirationDate zapisujemy jako string w formacie "o" (ISO 8601),
            // bo Twój AllegroApiClient czyta to jako string i parsuje DateTime.Parse(...).
            var p = new[]
            {
                new MySqlParameter("@a", accessEnc),
                new MySqlParameter("@r", (object) (refreshEnc ?? (string)null) ?? DBNull.Value),
                new MySqlParameter("@e", expiresAtUtc.ToString("o")),
                new MySqlParameter("@id", accountId)
            };

            // Sprawdzamy, czy używamy istniejącego połączenia, czy tworzymy nowe
            if (_con != null)
            {
                using (var command = new MySqlCommand(sql, _con))
                {
                    command.Parameters.AddRange(p);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            else
            {
                await _db.ExecuteNonQueryAsync(sql, p).ConfigureAwait(false);
            }
        }
    }
}