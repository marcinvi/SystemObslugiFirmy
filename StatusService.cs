using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class StatusService
    {
        private readonly DatabaseService _dbService;
        private readonly string _typStatusu;

        public StatusService(string connectionString, string typStatusu)
        {
            _dbService = new DatabaseService(connectionString);
            _typStatusu = typStatusu;
        }

        public async Task<List<string>> GetStatusesAsync()
        {
            try
            {
                var dt = await _dbService.GetDataTableAsync(
                    "SELECT Nazwa FROM Statusy WHERE TypStatusu = @typ ORDER BY Nazwa",
                    new MySqlParameter("@typ", _typStatusu));

                var statuses = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    statuses.Add(row["Nazwa"].ToString());
                }
                return statuses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddStatusAsync(string nazwa)
        {
            try
            {
                await _dbService.ExecuteNonQueryAsync(
                    "INSERT INTO Statusy (Nazwa, TypStatusu) VALUES (@nazwa, @typ)",
                    new MySqlParameter("@nazwa", nazwa),
                    new MySqlParameter("@typ", _typStatusu));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteStatusAsync(string nazwa)
        {
            try
            {
                await _dbService.ExecuteNonQueryAsync(
                    "DELETE FROM Statusy WHERE Nazwa = @nazwa AND TypStatusu = @typ",
                    new MySqlParameter("@nazwa", nazwa),
                    new MySqlParameter("@typ", _typStatusu));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
