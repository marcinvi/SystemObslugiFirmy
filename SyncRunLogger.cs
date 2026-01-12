using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class SyncRun
    {
        public long Id { get; set; }
        public string Source { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool Ok { get; set; }
        public int RowsFetched { get; set; }
        public int RowsWritten { get; set; }
        public string Details { get; set; }
        public string ErrorMessage { get; set; }
    }

    public static class SyncRunLogger
    {
        public static async Task<long> StartAsync(string source)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(
                    "INSERT INTO SyncRuns(source, started_at, ok) VALUES(@s, CURRENT_TIMESTAMP, 0); SELECT LAST_INSERT_ID();", con))
                {
                    cmd.Parameters.AddWithValue("@s", source);
                    var id = Convert.ToInt64(await cmd.ExecuteScalarAsync());
                    return id;
                }
            }
        }

        public static async Task SuccessAsync(long id, int fetched, int written, string details = null)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(
                    @"UPDATE SyncRuns SET finished_at=CURRENT_TIMESTAMP, ok=1, rows_fetched=@f, rows_written=@w, details=@d WHERE id=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@f", fetched);
                    cmd.Parameters.AddWithValue("@w", written);
                    cmd.Parameters.AddWithValue("@d", (object)details ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task FailureAsync(long id, Exception ex)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(
                    @"UPDATE SyncRuns SET finished_at=CURRENT_TIMESTAMP, ok=0, error_message=@e WHERE id=@id;", con))
                {
                    cmd.Parameters.AddWithValue("@e", ex.ToString());
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task<List<SyncRun>> LatestAsync(string source, int take = 10)
        {
            var list = new List<SyncRun>();
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(
                    @"SELECT id, source, started_at, finished_at, ok, rows_fetched, rows_written, details, error_message
                      FROM SyncRuns
                      WHERE (@s IS NULL OR source=@s)
                      ORDER BY started_at DESC
                      LIMIT @take;", con))
                {
                    cmd.Parameters.AddWithValue("@s", (object)source ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@take", take);
                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            list.Add(new SyncRun
                            {
                                Id = rd.GetInt64(0),
                                Source = rd.GetString(1),
                                StartedAt = rd.GetDateTime(2),
                                FinishedAt = rd.IsDBNull(3) ? (DateTime?)null : rd.GetDateTime(3),
                                Ok = rd.GetInt32(4) == 1,
                                RowsFetched = rd.IsDBNull(5) ? 0 : rd.GetInt32(5),
                                RowsWritten = rd.IsDBNull(6) ? 0 : rd.GetInt32(6),
                                Details = rd.IsDBNull(7) ? null : rd.GetString(7),
                                ErrorMessage = rd.IsDBNull(8) ? null : rd.GetString(8),
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
