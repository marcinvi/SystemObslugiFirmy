using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;
using System.Linq;

namespace ReklamacjeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("complaints/processing")]
        public async Task<IActionResult> GetProcessingComplaints()
        {
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var zCols = await GetTableColumns(conn, "Zgloszenia");
            var kCols = await GetTableColumns(conn, "Klienci");
            var pCols = await GetTableColumns(conn, "Produkty");

            var zId = PickFirstExisting(zCols, "Id", "IdZgloszenia") ?? "Id";
            var zKlientId = PickFirstExisting(zCols, "KlientID", "IdKlienta") ?? "KlientID";
            var zProduktId = PickFirstExisting(zCols, "ProduktID", "IdProduktu") ?? "ProduktID";
            var kId = PickFirstExisting(kCols, "Id", "IdKlienta") ?? "Id";
            var pId = PickFirstExisting(pCols, "Id", "IdProduktu") ?? "Id";

            var hasNazwaFirmy = kCols.Contains("NazwaFirmy");
            var klientExpr = hasNazwaFirmy
                ? "COALESCE(NULLIF(k.NazwaFirmy, ''), k.ImieNazwisko, 'Brak klienta')"
                : "COALESCE(k.ImieNazwisko, 'Brak klienta')";

            var produktExpr = pCols.Contains("Nazwa")
                ? "COALESCE(p.Nazwa, 'Brak produktu')"
                : "'Brak produktu'";

            var sql = $@"
                SELECT
                    z.`{zId}` AS Id,
                    z.NrZgloszenia,
                    {klientExpr} AS Klient,
                    {produktExpr} AS Produkt,
                    z.DataZgloszenia,
                    COALESCE(z.StatusOgolny, '') AS Status
                FROM Zgloszenia z
                LEFT JOIN Klienci k ON z.`{zKlientId}` = k.`{kId}`
                LEFT JOIN Produkty p ON z.`{zProduktId}` = p.`{pId}`
                WHERE z.StatusOgolny = @status
                ORDER BY z.`{zId}` DESC";

            var complaints = new List<DashboardComplaintDto>();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@status", "Procesowana");
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var dataZgl = reader["DataZgloszenia"] as DateTime? ?? DateTime.UtcNow;
                complaints.Add(new DashboardComplaintDto
                {
                    Id = SafeGetInt(reader, "Id"),
                    NrZgloszenia = reader["NrZgloszenia"]?.ToString() ?? string.Empty,
                    Klient = reader["Klient"]?.ToString() ?? "Brak klienta",
                    Produkt = reader["Produkt"]?.ToString() ?? "Brak produktu",
                    DniPoZgloszeniu = Math.Max(0, (DateTime.Now - dataZgl).Days),
                    Status = reader["Status"]?.ToString() ?? string.Empty
                });
            }

            return Ok(ApiResponse<List<DashboardComplaintDto>>.SuccessResponse(complaints));
        }

        [HttpGet("reminders")]
        public async Task<IActionResult> GetReminders()
        {
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var hasPrzypomnienia = await TableExists(conn, "Przypomnienia");
            if (!hasPrzypomnienia)
            {
                return Ok(ApiResponse<List<DashboardReminderDto>>.SuccessResponse(new List<DashboardReminderDto>()));
            }

            const string sql = @"
                SELECT Id, Tresc, DotyczyZgloszenia, Status
                FROM Przypomnienia
                WHERE Status = 'Nowe' OR Status = 'Active' OR Status IS NULL";

            var reminders = new List<DashboardReminderDto>();
            await using var cmd = new MySqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tresc = reader["Tresc"]?.ToString() ?? string.Empty;
                var cat = ClassifyCategory(tresc);
                reminders.Add(new DashboardReminderDto
                {
                    Id = SafeGetInt(reader, "Id"),
                    Tresc = tresc,
                    DotyczyZgloszenia = reader["DotyczyZgloszenia"]?.ToString() ?? string.Empty,
                    Kategoria = cat,
                    Kolor = GetColorForCategory(cat, tresc)
                });
            }

            return Ok(ApiResponse<List<DashboardReminderDto>>.SuccessResponse(reminders));
        }

        private static async Task<HashSet<string>> GetTableColumns(MySqlConnection conn, string tableName)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            const string sql = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @table";

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@table", tableName);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var name = reader["COLUMN_NAME"]?.ToString();
                if (!string.IsNullOrWhiteSpace(name)) result.Add(name);
            }

            return result;
        }

        private static async Task<bool> TableExists(MySqlConnection conn, string tableName)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @table";

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@table", tableName);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
        }

        private static string? PickFirstExisting(HashSet<string> columns, params string[] candidates)
            => candidates.FirstOrDefault(columns.Contains);

        private static int SafeGetInt(MySqlDataReader reader, string name)
            => int.TryParse(reader[name]?.ToString(), out var v) ? v : 0;

        private string ClassifyCategory(string t)
        {
            if (string.IsNullOrEmpty(t)) return "Ręczne";
            t = t.ToUpperInvariant();

            if (t.Contains("[PROBLEM]") || t.Contains("[ZWROT]") || t.Contains("[ZGUBIONA]") ||
                t.Contains("[PRZESYŁKA]") || t.Contains("[W DORĘCZENIU]") || t.Contains("DPD") || t.Contains("KURIER"))
                return "Kurier";

            if (t.StartsWith("[AUTO]") || t.Contains("PILNE") || t.Contains("TERMIN") || t.Contains("DECYZJ"))
                return "Czas na decyzję";

            return "Ręczne";
        }

        private string GetColorForCategory(string cat, string t)
        {
            var text = (t ?? string.Empty).ToUpperInvariant();
            if (text.Contains("[PROBLEM]") || text.Contains("[ZWROT]") || text.Contains("[ZGUBIONA]")) return "#CD5C5C";
            if (cat == "Kurier") return "#6495ED";
            if (cat == "Czas na decyzję") return "#FFA500";
            return "#D3D3D3";
        }
    }
}
