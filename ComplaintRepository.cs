using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class ComplaintRepository
    {
        public async Task<string> GetNextComplaintNumberAsync(MySqlConnection con, MySqlTransaction transaction)
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                string pattern = $"%/{currentYear}";
                const string query = "SELECT NrZgloszenia FROM Zgloszenia WHERE NrZgloszenia LIKE @pattern ORDER BY CAST(SUBSTRING(NrZgloszenia, 3, LOCATE('/', NrZgloszenia) - 3) AS SIGNED) DESC LIMIT 1";
                using (var cmd = new MySqlCommand(query, con, transaction))
                {
                    cmd.Parameters.AddWithValue("@pattern", pattern);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        string lastNumberStr = result.ToString();
                        string[] parts = lastNumberStr.Split('/');
                        if (parts.Length == 3 && int.TryParse(parts[1], out int lastNumber))
                        {
                            return $"R/{lastNumber + 1}/{currentYear}";
                        }
                    }
                    return $"R/1/{currentYear}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ComplaintRepository] Error in GetNextComplaintNumberAsync: {ex}");
                throw;
            }
        }

        public async Task InsertComplaintAsync(MySqlConnection con, MySqlTransaction transaction, string nrZgloszenia, int klientId, int produktId, DateTime dataZgloszenia, string nrFaktury, string nrSeryjny, string usterka)
        {
            try
            {
                const string query = @"INSERT INTO Zgloszenia (NrZgloszenia, KlientID, ProduktID, DataZgloszenia, NrFaktury, NrSeryjny, Usterka, StatusOgolny)
                                         VALUES (@nrZgloszenia, @klientId, @produktId, @dataZgl, @nrFakt, @nrSer, @usterka, 'Procesowana')";
                using (var cmd = new MySqlCommand(query, con, transaction))
                {
                    cmd.Parameters.AddWithValue("@nrZgloszenia", nrZgloszenia);
                    cmd.Parameters.AddWithValue("@klientId", klientId);
                    cmd.Parameters.AddWithValue("@produktId", produktId);
                    cmd.Parameters.AddWithValue("@dataZgl", dataZgloszenia.Date);
                    cmd.Parameters.AddWithValue("@nrFakt", nrFaktury);
                    cmd.Parameters.AddWithValue("@nrSer", nrSeryjny);
                    cmd.Parameters.AddWithValue("@usterka", usterka);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ComplaintRepository] Error in InsertComplaintAsync: {ex}");
                throw;
            }
        }
    }
}
