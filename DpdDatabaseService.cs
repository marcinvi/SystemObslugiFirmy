// ############################################################################
// Plik: DpdDatabaseService.cs (POPRAWIONY - USUNIĘTO DUPLIKAT DpdTrackingService)
// ############################################################################

using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    // Model reprezentujący zamówienie DPD
    public class DpdOrder
    {
        public int Id { get; set; }
        public string NrZgloszenia { get; set; }
        public string Waybill { get; set; }
        public string OrderType { get; set; }
        public DateTime OrderDate { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string LastStatus { get; set; }
    }

    public class DpdDatabaseService
    {
        // Metoda do zapisu nowego zamówienia kuriera
        public async Task SaveNewOrderAsync(DpdOrder order)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = @"
                        INSERT INTO DpdOrders (NrZgloszenia, Waybill, OrderType, OrderDate, SenderName, SenderAddress, ReceiverName, ReceiverAddress, PackageContent, PackageReference, LastStatus, LastStatusTimestamp)
                        VALUES (@NrZgloszenia, @Waybill, @OrderType, @OrderDate, @SenderName, @SenderAddress, @ReceiverName, @ReceiverAddress, @PackageContent, @PackageReference, @LastStatus, @LastStatusTimestamp)";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@NrZgloszenia", (object)order.NrZgloszenia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Waybill", order.Waybill);
                        cmd.Parameters.AddWithValue("@OrderType", order.OrderType);
                        cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate.ToString("o"));
                        cmd.Parameters.AddWithValue("@SenderName", order.SenderName);
                        // Pełny adres dla historii - placeholder, można rozbudować
                        cmd.Parameters.AddWithValue("@SenderAddress", "Placeholder");
                        cmd.Parameters.AddWithValue("@ReceiverName", order.ReceiverName);
                        cmd.Parameters.AddWithValue("@ReceiverAddress", "Placeholder");
                        cmd.Parameters.AddWithValue("@PackageContent", "Placeholder");
                        cmd.Parameters.AddWithValue("@PackageReference", "Placeholder");
                        cmd.Parameters.AddWithValue("@LastStatus", "Zamówiono");
                        cmd.Parameters.AddWithValue("@LastStatusTimestamp", DateTime.Now.ToString("o"));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd zapisu zamówienia DPD do bazy danych: " + ex.Message, ex);
            }
        }

        // Metoda do pobierania wszystkich zamówień do widoku historii
        public async Task<List<DpdOrder>> GetDpdOrdersAsync()
        {
            var orders = new List<DpdOrder>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "SELECT Id, NrZgloszenia, Waybill, OrderType, OrderDate, SenderName, ReceiverName, LastStatus FROM DpdOrders ORDER BY Id DESC";
                    using (var cmd = new MySqlCommand(query, con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            orders.Add(new DpdOrder
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                NrZgloszenia = reader["NrZgloszenia"]?.ToString(),
                                Waybill = reader["Waybill"].ToString(),
                                OrderType = reader["OrderType"].ToString(),
                                OrderDate = DateTime.Parse(reader["OrderDate"].ToString()),
                                SenderName = reader["SenderName"].ToString(),
                                ReceiverName = reader["ReceiverName"].ToString(),
                                LastStatus = reader["LastStatus"]?.ToString() ?? "Brak statusu"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd odczytu historii zamówień DPD z bazy danych: " + ex.Message, ex);
            }
            return orders;
        }

        public void LogToJournal(string message)
        {
            Console.WriteLine($"DB_JOURNAL_LOG: {message.Trim()}");
        }

        public void LogToActionHistory(int caseId, string action)
        {
            Console.WriteLine($"DB_CASE_LOG (CaseID: {caseId}): {action.Trim()}");
        }

    }
}