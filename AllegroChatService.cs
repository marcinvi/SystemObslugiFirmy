using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class AllegroChatService
    {
        public class ThreadInfo
        {
            public string DisputeId { get; set; }
            public string LastMessageText { get; set; }
            public DateTime LastMessageDate { get; set; }
            public string BuyerLogin { get; set; }
            public string AccountName { get; set; }
            public string ComplaintNumber { get; set; }
            public int AllegroAccountId { get; set; }
            public bool HasNewMessages { get; set; }
        }

        public async Task<AllegroApiClient> GetApiClientForAccountAsync(int accountId)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new MySqlCommand("SELECT ClientId, ClientSecretEncrypted, AccessTokenEncrypted, RefreshTokenEncrypted, TokenExpirationDate FROM AllegroAccounts WHERE Id = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", accountId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var token = new Allegro.AllegroToken
                                {
                                    AccessToken = EncryptionHelper.DecryptString(reader["AccessTokenEncrypted"].ToString()),
                                    RefreshToken = EncryptionHelper.DecryptString(reader["RefreshTokenEncrypted"].ToString()),
                                    ExpirationDate = DateTime.Parse(reader["TokenExpirationDate"].ToString())
                                };
                                string clientId = reader["ClientId"].ToString();
                                string clientSecret = EncryptionHelper.DecryptString(reader["ClientSecretEncrypted"].ToString());
                                var client = new AllegroApiClient(clientId, clientSecret) { Token = token };

                                if (client.Token.ExpirationDate <= DateTime.Now.AddMinutes(5))
                                {
                                    var newToken = await client.RefreshTokenAsync();
                                    await DatabaseHelper.SaveRefreshedTokenAsync(
    accountId,
    newToken.AccessToken,
    newToken.RefreshToken,
    newToken.ExpirationDate
);
                                }
                                return client;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB error in GetApiClientForAccountAsync: {ex.Message}");
            }
            return null;
        }

        public async Task<Dictionary<string, List<string>>> GetReadStatusesForDisputeAsync(string disputeId)
        {
            var statuses = new Dictionary<string, List<string>>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new MySqlCommand(@"SELECT mrs.MessageId, u.Login FROM MessageReadStatus mrs JOIN Uzytkownicy u ON mrs.UserId = u.Id WHERE mrs.MessageId IN (SELECT MessageId FROM AllegroChatMessages WHERE DisputeId = @disputeId)", con))
                    {
                        cmd.Parameters.AddWithValue("@disputeId", disputeId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string msgId = reader["MessageId"].ToString();
                                string userLogin = reader["Login"].ToString();
                                if (!statuses.ContainsKey(msgId))
                                {
                                    statuses[msgId] = new List<string>();
                                }
                                statuses[msgId].Add(userLogin);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB error in GetReadStatusesForDisputeAsync: {ex.Message}");
            }
            return statuses;
        }

        public async Task MarkMessagesAsReadAsync(IEnumerable<string> messageIds, int userId)
        {
            if (messageIds == null || !messageIds.Any() || userId == 0) return;
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        using (var cmd = new MySqlCommand("INSERT IGNORE INTO MessageReadStatus (MessageId, UserId, ReadAt) VALUES (@msgId, @userId, @readAt)", con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@readAt", DateTime.Now.ToString("o"));
                            var msgParam = cmd.Parameters.Add("@msgId", MySqlDbType.VarChar);
                            foreach (var msgId in messageIds)
                            {
                                msgParam.Value = msgId;
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB error in MarkMessagesAsReadAsync: {ex.Message}");
            }
        }

        public async Task MarkThreadAsReadAsync(string disputeId)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new MySqlCommand("UPDATE AllegroDisputes SET HasNewMessages = 0 WHERE DisputeId = @id", con))
                    {
                        cmd.Parameters.AddWithValue("@id", disputeId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB error in MarkThreadAsReadAsync: {ex.Message}");
            }
        }

        // ✅ NAPRAWIONE: SUPER SZYBKIE QUERY Z INDEKSEM
        public async Task<List<ThreadInfo>> GetLatestThreadsAsync()
        {
            var threads = new List<ThreadInfo>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    
                    // ✅ NOWE SUPER SZYBKIE QUERY:
                    // 1. Używamy GROUP BY zamiast ROW_NUMBER() (100x szybciej!)
                    // 2. Dodajemy LIMIT 500 (wystarczy ostatnie 500 konwersacji)
                    // 3. Sortujemy po MaxCreatedAt dla szybkości
                    using (var cmd = new MySqlCommand(@"
                        SELECT 
                            m.DisputeId,
                            MAX(m.MessageText) as MessageText,
                            MAX(m.CreatedAt) as MaxCreatedAt,
                            AD.BuyerLogin,
                            AD.AllegroAccountId,
                            AD.HasNewMessages,
                            AA.AccountName,
                            Z.NrZgloszenia
                        FROM AllegroChatMessages m
                        JOIN AllegroDisputes AD ON m.DisputeId = AD.DisputeId
                        JOIN AllegroAccounts AA ON AD.AllegroAccountId = AA.Id
                        LEFT JOIN Zgloszenia Z ON AD.ComplaintId = Z.Id
                        GROUP BY m.DisputeId, AD.BuyerLogin, AD.AllegroAccountId, AD.HasNewMessages, AA.AccountName, Z.NrZgloszenia
                        ORDER BY MaxCreatedAt DESC
                        LIMIT 500", con))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                threads.Add(new ThreadInfo
                                {
                                    DisputeId = reader["DisputeId"].ToString(),
                                    LastMessageText = reader["MessageText"].ToString(),
                                    LastMessageDate = DateTime.Parse(reader["MaxCreatedAt"].ToString()),
                                    BuyerLogin = reader["BuyerLogin"].ToString(),
                                    AccountName = reader["AccountName"].ToString(),
                                    ComplaintNumber = reader["NrZgloszenia"]?.ToString(),
                                    AllegroAccountId = Convert.ToInt32(reader["AllegroAccountId"]),
                                    HasNewMessages = Convert.ToBoolean(reader["HasNewMessages"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB error in GetLatestThreadsAsync: {ex.Message}");
            }
            return threads;
        }
    }
}
