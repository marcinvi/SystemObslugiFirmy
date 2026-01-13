using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class MessageService
    {
        public class User
        {
            public int Id { get; set; }
            public string NazwaWyswietlana { get; set; }
            public override string ToString() => NazwaWyswietlana;
        }

        private readonly DatabaseService _bazaService;
        private readonly string _magazynConnectionString;

        public MessageService(string bazaConnectionString, string magazynConnectionString)
        {
            _bazaService = new DatabaseService(bazaConnectionString);
            _magazynConnectionString = magazynConnectionString;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            const string query = "SELECT Id, `Nazwa Wyświetlana` FROM Uzytkownicy ORDER BY `Nazwa Wyświetlana`";
            var dt = await _bazaService.GetDataTableAsync(query);
            var users = new List<User>();
            foreach (DataRow row in dt.Rows)
            {
                users.Add(new User
                {
                    Id = Convert.ToInt32(row["Id"]),
                    NazwaWyswietlana = row["Nazwa Wyświetlana"].ToString()
                });
            }
            return users;
        }

        public async Task SendMessageAsync(int? replyToMessageId, int? relatedReturnId, IEnumerable<int> recipientIds, string title, string body)
        {
            try
            {
                using (var con = new MySqlConnection(_magazynConnectionString))
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        if (replyToMessageId.HasValue)
                        {
                            using (var updateCmd = new MySqlCommand("UPDATE Wiadomosci SET CzyOdpowiedziano = 1 WHERE Id = @id", con, transaction))
                            {
                                updateCmd.Parameters.AddWithValue("@id", replyToMessageId.Value);
                                await updateCmd.ExecuteNonQueryAsync();
                            }
                        }

                        foreach (var recipientId in recipientIds)
                        {
                            using (var insertCmd = new MySqlCommand(
                                "INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, ParentMessageId) VALUES (@nadawca, @odbiorca, @tytul, @tresc, @data, @zwrotId, @parentId)",
                                con,
                                transaction))
                            {
                                insertCmd.Parameters.AddWithValue("@nadawca", SessionManager.CurrentUserId);
                                insertCmd.Parameters.AddWithValue("@odbiorca", recipientId);
                                insertCmd.Parameters.AddWithValue("@tytul", title);
                                insertCmd.Parameters.AddWithValue("@tresc", body);
                                insertCmd.Parameters.AddWithValue("@data", DateTime.Now);
                                insertCmd.Parameters.AddWithValue("@zwrotId", (object)relatedReturnId ?? DBNull.Value);
                                insertCmd.Parameters.AddWithValue("@parentId", (object)replyToMessageId ?? DBNull.Value);
                                await insertCmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                    }
                }
                Console.WriteLine("Wiadomości zostały zapisane w bazie.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Błąd w MessageService.SendMessageAsync: {ex}");
                throw;
            }
        }
    }
}
