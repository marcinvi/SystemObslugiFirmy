using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class AutomaticReminderService
    {
        public class ReminderNotification
        {
            public string ComplaintNumber { get; set; }
            public string Message { get; set; }
            public bool IsUrgent { get; set; }
        }

        private readonly string _connectionString;

        public AutomaticReminderService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<(bool requiresUiRefresh, List<ReminderNotification> notifications)> GenerateAutomaticRemindersAsync()
        {
            var notifications = new List<ReminderNotification>();
            bool requiresUiRefresh = false;
            const int deadlineDays = 10;
            const int warningThreshold = 3;

            try
            {
                using (var con = new SQLiteConnection(_connectionString))
                {
                    await con.OpenAsync();

                    const string cleanupQuery = @"DELETE FROM Przypomnienia
                                          WHERE Tresc LIKE '[AUTO]%' AND DotyczyZgloszenia IN
                                          (SELECT NrZgloszenia FROM Zgloszenia WHERE StatusOgolny != 'Procesowana')";
                    using (var cleanupCmd = new SQLiteCommand(cleanupQuery, con))
                    {
                        await cleanupCmd.ExecuteNonQueryAsync();
                    }

                    var complaints = new List<(string NrZgloszenia, DateTime DataZgloszenia)>();
                    const string selectQuery = "SELECT NrZgloszenia, DataZgloszenia FROM Zgloszenia WHERE StatusOgolny = 'Procesowana'";
                    using (var command = new SQLiteCommand(selectQuery, con))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            complaints.Add((reader.GetString(0), reader.GetDateTime(1)));
                        }
                    }

                    foreach (var (nrZgloszenia, dataZgloszenia) in complaints)
                    {
                        var daysPassed = (int)(DateTime.Now.Date - dataZgloszenia.Date).TotalDays;
                        var daysLeft = deadlineDays - daysPassed;
                        string newReminderText = null;
                        bool isUrgent = false;

                        if (daysLeft < 0)
                        {
                            isUrgent = true;
                            newReminderText = $"[AUTO] PILNE: Zgłoszenie po terminie o {-daysLeft} {(-daysLeft == 1 ? \"dzień\" : \"dni\")}!";
                        }
                        else if (daysLeft <= warningThreshold)
                        {
                            newReminderText = $"[AUTO] Pozostało {daysLeft} {(daysLeft == 1 ? \"dzień\" : \"dni\")} na odpowiedź.";
                        }

                        using (var checkCmd = new SQLiteCommand("SELECT Tresc FROM Przypomnienia WHERE DotyczyZgloszenia = @nr AND Tresc LIKE '[AUTO]%' AND CzyZrealizowane = 0", con))
                        {
                            checkCmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                            var existingReminderText = (string)await checkCmd.ExecuteScalarAsync();

                            if (newReminderText != null)
                            {
                                if (existingReminderText == null)
                                {
                                    using (var insertCmd = new SQLiteCommand("INSERT INTO Przypomnienia (Tresc, DataPrzypomnienia, CzyZrealizowane, DotyczyZgloszenia) VALUES (@tresc, @data, 0, @nr)", con))
                                    {
                                        insertCmd.Parameters.AddWithValue("@tresc", newReminderText);
                                        insertCmd.Parameters.AddWithValue("@data", DateTime.Now);
                                        insertCmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                                        await insertCmd.ExecuteNonQueryAsync();
                                        requiresUiRefresh = true;
                                        notifications.Add(new ReminderNotification
                                        {
                                            ComplaintNumber = nrZgloszenia,
                                            Message = newReminderText,
                                            IsUrgent = isUrgent
                                        });
                                    }
                                }
                                else if (existingReminderText != newReminderText)
                                {
                                    using (var updateCmd = new SQLiteCommand("UPDATE Przypomnienia SET Tresc = @tresc, DataPrzypomnienia = @data WHERE DotyczyZgloszenia = @nr AND Tresc LIKE '[AUTO]%'", con))
                                    {
                                        updateCmd.Parameters.AddWithValue("@tresc", newReminderText);
                                        updateCmd.Parameters.AddWithValue("@data", DateTime.Now);
                                        updateCmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                                        await updateCmd.ExecuteNonQueryAsync();
                                        requiresUiRefresh = true;
                                    }
                                }
                            }
                            else if (existingReminderText != null)
                            {
                                using (var deleteCmd = new SQLiteCommand("DELETE FROM Przypomnienia WHERE DotyczyZgloszenia = @nr AND Tresc LIKE '[AUTO]%'", con))
                                {
                                    deleteCmd.Parameters.AddWithValue("@nr", nrZgloszenia);
                                    await deleteCmd.ExecuteNonQueryAsync();
                                    requiresUiRefresh = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w AutomaticReminderService: {ex.Message}");
                throw;
            }

            return (requiresUiRefresh, notifications);
        }
    }
}

