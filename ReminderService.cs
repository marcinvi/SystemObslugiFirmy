// ############################################################################
// Plik: ReminderService.cs (WERSJA OSTATECZNA, KOMPLETNA I POPRAWNA)
// ############################################################################

using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class ReminderRow
    {
        public long Id { get; set; }
        public string TicketId { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime? DueAt { get; set; }
        public string Status { get; set; }
    }

    public class Reminder
    {
        public int Id { get; set; }
        public string Tresc { get; set; }
        public string DotyczyZgloszenia { get; set; }
        public DateTime? DataPrzypomnienia { get; set; }
    }

    public static class ReminderCategories
    {
        public const string Decision = "decision";
        public const string Courier = "courier";
        public const string Manual = "manual";
    }

    public static class ReminderSources
    {
        public const string Auto = "AUTO";
        public const string Dpd = "DPD";
        public const string Manual = "MANUAL";
    }

    public static class ReminderService
    {
        public static async Task SnoozeAsync(long reminderId, int days)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("UPDATE przypomnienia SET DataPrzypomnienia = DATE_ADD(COALESCE(DataPrzypomnienia, NOW()), INTERVAL @p_days DAY) WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@p_days", days);
                cmd.Parameters.AddWithValue("@id", reminderId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task MarkAsDoneAsync(long reminderId)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("UPDATE przypomnienia SET CzyZrealizowane=1, Status='Completed' WHERE Id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", reminderId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task<List<Reminder>> GetActiveRemindersAsync()
        {
            var reminders = new List<Reminder>();
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand(@"
                SELECT Id, Tresc, DotyczyZgloszenia, DataPrzypomnienia 
                FROM przypomnienia 
                WHERE IFNULL(CzyZrealizowane,0)=0 
                AND (Status != 'Completed' OR Status IS NULL)
                ORDER BY CASE WHEN Tresc LIKE '%PILNE%' THEN 1 ELSE 2 END, DataPrzypomnienia ASC", con))
            using (var rd = await cmd.ExecuteReaderAsync())
            {
                while (await rd.ReadAsync())
                {
                    reminders.Add(new Reminder
                    {
                        Id = rd.GetInt32(0),
                        Tresc = rd.IsDBNull(1) ? "" : rd.GetString(1),
                        DotyczyZgloszenia = rd.IsDBNull(2) ? null : rd.GetString(2),
                        DataPrzypomnienia = rd.IsDBNull(3) ? (DateTime?)null : rd.GetDateTime(3)
                    });
                }
            }
            return reminders;
        }

        public static async Task<bool> GenerateAutomaticRemindersAsync(int deadlineDays, int warningThreshold)
        {
            bool requiresUiRefresh = false;
            await CleanUpAutoRemindersAsync();
            var complaints = await GetProcessingComplaintsAsync();

            foreach (var (nrZgloszenia, dataZgloszenia) in complaints)
            {
                int daysLeft = deadlineDays - (int)(DateTime.Now.Date - dataZgloszenia.Date).TotalDays;
                string newReminderText = null;

                if (daysLeft < 0)
                {
                    int overdue = -daysLeft;
                    newReminderText = $"[AUTO] PILNE: Zgłoszenie po terminie o {overdue} {(overdue == 1 ? "dzień!" : "dni!")}";
                }
                else if (daysLeft <= warningThreshold)
                {
                    newReminderText = $"[AUTO] Czas na decyzję! Pozostało {daysLeft} {(daysLeft == 1 ? "dzień" : "dni")}.";
                }

                var existingReminderText = await GetExistingAutoReminderAsync(nrZgloszenia);

                if (newReminderText != null)
                {
                    if (existingReminderText == null)
                    {
                        await InsertAutoReminderAsync(nrZgloszenia, newReminderText, DateTime.Now);
                        requiresUiRefresh = true;
                    }
                    else if (existingReminderText != newReminderText)
                    {
                        await UpdateAutoReminderAsync(nrZgloszenia, newReminderText, DateTime.Now);
                        requiresUiRefresh = true;
                    }
                }
                else if (existingReminderText != null)
                {
                    await DeleteAutoReminderAsync(nrZgloszenia);
                    requiresUiRefresh = true;
                }
            }
            return requiresUiRefresh;
        }

        #region Metody pomocnicze
        public static async Task CleanUpAutoRemindersAsync()
        {
            using (var con = Database.GetNewOpenConnection())
            {
                var toKeep = new HashSet<long>();
                var byTicket = new Dictionary<string, List<(long id, DateTime when)>>();

                using (var cmd = new MySqlCommand(@"
                    SELECT Id, COALESCE(DotyczyZgloszenia,''), COALESCE(DataPrzypomnienia, created_at)
                    FROM przypomnienia
                    WHERE IFNULL(CzyZrealizowane,0)=0 AND Tresc LIKE '[AUTO]%';", con))
                using (var rd = await cmd.ExecuteReaderAsync())
                {
                    while (await rd.ReadAsync())
                    {
                        if (!byTicket.ContainsKey(rd.GetString(1))) byTicket[rd.GetString(1)] = new List<(long, DateTime)>();
                        byTicket[rd.GetString(1)].Add((rd.GetInt64(0), rd.GetDateTime(2)));
                    }
                }
                foreach (var id in byTicket.Select(kv => kv.Value.OrderByDescending(x => x.when).First().id))
                {
                    toKeep.Add(id);
                }

                if (toKeep.Any())
                {
                    using (var cmd = new MySqlCommand($"UPDATE przypomnienia SET CzyZrealizowane=1, Status='Completed' WHERE IFNULL(CzyZrealizowane,0)=0 AND Tresc LIKE '[AUTO]%' AND Id NOT IN ({string.Join(",", toKeep)});", con))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public static async Task<List<(string nrZgloszenia, DateTime dataZgloszenia)>> GetProcessingComplaintsAsync()
        {
            var result = new List<(string, DateTime)>();
            using (var con = Database.GetNewOpenConnection())
            {
                // Tylko dla zgłoszeń o statusie "Procesowana" i nie zakończonych przez producenta
                string query = @"
                    SELECT NrZgloszenia, DataZgloszenia 
                    FROM Zgloszenia 
                    WHERE StatusOgolny = 'Procesowana' 
                    AND (StatusProducent IS NULL OR StatusProducent != 'Zakończone')";

                using (var cmd = new MySqlCommand(query, con))
                using (var rd = await cmd.ExecuteReaderAsync())
                {
                    while (await rd.ReadAsync())
                    {
                        string nr = rd.IsDBNull(0) ? "" : rd.GetString(0);
                        string rawDate = rd.IsDBNull(1) ? "" : rd.GetString(1);
                        if (DateTime.TryParse(rawDate, out DateTime dt))
                        {
                            result.Add((nr, dt));
                        }
                    }
                }
            }
            return result;
        }

        public static async Task<string> GetExistingAutoReminderAsync(string nrZgloszenia)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("SELECT Tresc FROM przypomnienia WHERE IFNULL(CzyZrealizowane,0)=0 AND Tresc LIKE '[AUTO]%' AND DotyczyZgloszenia=@nr ORDER BY COALESCE(DataPrzypomnienia, created_at) DESC LIMIT 1;", con))
            {
                cmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                return (await cmd.ExecuteScalarAsync())?.ToString();
            }
        }

        public static async Task InsertAutoReminderAsync(string nrZgloszenia, string text, DateTime when)
        {
            using (var con = Database.GetNewOpenConnection())
            // Dodano wymuszenie Status = 'Nowe' oraz IsAutoGenerated = 1
            using (var cmd = new MySqlCommand("INSERT INTO przypomnienia (Tresc, DotyczyZgloszenia, DataPrzypomnienia, CzyZrealizowane, Status, IsAutoGenerated) VALUES (@t, @nr, @dt, 0, 'Nowe', 1);", con))
            {
                cmd.Parameters.AddWithValue("@t", text);
                cmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                cmd.Parameters.AddWithValue("@dt", when);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task UpdateAutoReminderAsync(string nrZgloszenia, string newText, DateTime when)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("UPDATE przypomnienia SET Tresc=@t, DataPrzypomnienia=@dt WHERE IFNULL(CzyZrealizowane,0)=0 AND Tresc LIKE '[AUTO]%' AND DotyczyZgloszenia=@nr;", con))
            {
                cmd.Parameters.AddWithValue("@t", newText);
                cmd.Parameters.AddWithValue("@dt", when);
                cmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task DeleteAutoReminderAsync(string nrZgloszenia)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("UPDATE przypomnienia SET CzyZrealizowane=1, Status='Completed' WHERE IFNULL(CzyZrealizowane,0)=0 AND Tresc LIKE '[AUTO]%' AND DotyczyZgloszenia=@nr;", con))
            {
                cmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task DeleteSpecificReminderAsync(string nrZgloszenia, string textPattern)
        {
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("UPDATE przypomnienia SET CzyZrealizowane=1, Status='Completed' WHERE IFNULL(CzyZrealizowane,0)=0 AND DotyczyZgloszenia=@nr AND Tresc LIKE @pattern;", con))
            {
                cmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                cmd.Parameters.AddWithValue("@pattern", textPattern);
                await cmd.ExecuteNonQueryAsync();
            }
        }
        #endregion
    }
}