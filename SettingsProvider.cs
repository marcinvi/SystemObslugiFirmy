using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using Reklamacje_Dane.Allegro;

namespace Reklamacje_Dane
{
    public class AllegroAccountDetails
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public AllegroToken Token { get; set; }
    }

    public static class SettingsProvider
    {
        public static async Task<AllegroAccountDetails> GetDefaultAllegroAccountAsync()
        {
            AllegroAccountDetails account = null;
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("SELECT Id, ClientId, ClientSecretEncrypted, AccessTokenEncrypted, RefreshTokenEncrypted, TokenExpirationDate FROM AllegroAccounts WHERE IsDefault = 1 LIMIT 1", con);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            account = new AllegroAccountDetails
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ClientId = reader["ClientId"].ToString(),
                                ClientSecret = EncryptionHelper.DecryptString(reader["ClientSecretEncrypted"].ToString())
                            };

                            string encryptedAccessToken = reader["AccessTokenEncrypted"]?.ToString();
                            if (!string.IsNullOrEmpty(encryptedAccessToken) && DateTime.TryParse(reader["TokenExpirationDate"]?.ToString(), out DateTime expDate))
                            {
                                account.Token = new AllegroToken
                                {
                                    AccessToken = EncryptionHelper.DecryptString(encryptedAccessToken),
                                    RefreshToken = EncryptionHelper.DecryptString(reader["RefreshTokenEncrypted"].ToString()),
                                    ExpirationDate = expDate
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd wczytywania konta Allegro: {ex.Message}");
                return null;
            }
            return account;
        }
    }
}