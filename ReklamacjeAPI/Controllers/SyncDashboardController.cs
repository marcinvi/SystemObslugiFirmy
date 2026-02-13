using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

/// <summary>
/// Unified dashboard endpoint — jedno wywołanie zwraca WSZYSTKO co Forms potrzebuje.
/// Dzięki temu 50 użytkowników nie odpytuje Google/Allegro/DPD bezpośrednio,
/// tylko pobiera status z API, które synchronizuje dane w tle (BackgroundServices).
/// </summary>
[Authorize]
[ApiController]
[Route("api/sync-dashboard")]
public class SyncDashboardController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AllegroSyncCoordinatorService _allegroSync;
    private readonly DpdSyncCoordinatorService _dpdSync;
    private readonly GoogleSyncCoordinatorService _googleSync;
    private readonly ILogger<SyncDashboardController> _logger;

    public SyncDashboardController(
        IConfiguration configuration,
        AllegroSyncCoordinatorService allegroSync,
        DpdSyncCoordinatorService dpdSync,
        GoogleSyncCoordinatorService googleSync,
        ILogger<SyncDashboardController> logger)
    {
        _configuration = configuration;
        _allegroSync = allegroSync;
        _dpdSync = dpdSync;
        _googleSync = googleSync;
        _logger = logger;
    }

    /// <summary>
    /// Główny endpoint dashboardu — wszystko w jednym wywołaniu.
    /// ReklamacjeControl powinien odpytywać ten endpoint co ~60 sekund.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<SyncDashboardDto>>> GetDashboard()
    {
        try
        {
            var dto = new SyncDashboardDto();

            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            // === SEKWENCYJNE POBIERANIE DANYCH Z BAZY ===
            // MySQL nie wspiera wielu równoczesnych poleceń na jednym połączeniu,
            // dlatego wykonujemy zapytania jedno po drugim.
            await FillCountersAsync(conn, dto);
            await FillProcessingComplaintsAsync(conn, dto);
            await FillRemindersAsync(conn, dto);
            await FillChangeLogAsync(conn, dto);

            // === STATUSY SYNC Z PAMIĘCI (zero kosztu) ===
            FillSyncStatuses(dto);

            dto.GeneratedAt = DateTime.Now;

            return Ok(ApiResponse<SyncDashboardDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd pobierania sync-dashboard");
            return StatusCode(500, ApiResponse<SyncDashboardDto>.ErrorResponse($"Błąd: {ex.Message}"));
        }
    }

    /// <summary>
    /// Lekki endpoint — tylko liczniki i statusy (bez list zgłoszeń/przypomnień).
    /// Do użycia przy częstszym pollingu (co 15-30 sekund).
    /// </summary>
    [HttpGet("counters")]
    public async Task<ActionResult<ApiResponse<SyncDashboardDto>>> GetCountersOnly()
    {
        try
        {
            var dto = new SyncDashboardDto();

            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            await FillCountersAsync(conn, dto);
            FillSyncStatuses(dto);

            dto.GeneratedAt = DateTime.Now;

            return Ok(ApiResponse<SyncDashboardDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd pobierania sync-dashboard/counters");
            return StatusCode(500, ApiResponse<SyncDashboardDto>.ErrorResponse($"Błąd: {ex.Message}"));
        }
    }

    // ========== PRYWATNE METODY WYPEŁNIAJĄCE DTO ==========

    private async Task FillCountersAsync(MySqlConnection conn, SyncDashboardDto dto)
    {
        // Bazowe wartości z pamięci usług sync (działają nawet gdy DB ma starszy schemat).
        var allegroStatus = _allegroSync.GetStatusSnapshot();
        dto.UnregisteredAllegroCount = allegroStatus.UnregisteredDisputesCount;
        dto.AllegroNewMessages = allegroStatus.DisputesWithNewMessages;

        var googleStatus = _googleSync.GetStatusSnapshot();
        dto.UnregisteredGoogleCount = googleStatus.MetricValue;

        // Zwroty + email + ewentualna korekta Allegro z DB.
        try
        {
            dto.UnregisteredAllegroCount = await GetUnregisteredAllegroCountAsync(conn, dto.UnregisteredAllegroCount);

            await using var returnsCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM NiezarejestrowaneZwrotyReklamacyjne WHERE IFNULL(CzyZarejestrowane, 0) = 0", conn);
            dto.UnregisteredReturnsCount = Convert.ToInt32(await returnsCmd.ExecuteScalarAsync());

            await using var emailCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM CentrumKontaktu WHERE Typ = 'Mail' AND Kierunek = 'IN' AND DataWyslania > DATE_SUB(NOW(), INTERVAL 1 DAY)", conn);
            dto.EmailUnreadCount = Convert.ToInt32(await emailCmd.ExecuteScalarAsync());

            await using var newMsgCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM AllegroDisputes WHERE IFNULL(HasNewMessages,0) = 1", conn);
            dto.AllegroNewMessages = Convert.ToInt32(await newMsgCmd.ExecuteScalarAsync());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Błąd pobierania liczników z DB - użyto fallbacku z usług synchronizacji");
        }
    }

    private async Task<int> GetUnregisteredAllegroCountAsync(MySqlConnection conn, int fallback)
    {
        try
        {
            await using var cmd = new MySqlCommand(@"
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'AllegroDisputes'
                  AND COLUMN_NAME = 'CzyZarejestrowane'", conn);

            var hasCzy = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            var sql = hasCzy
                ? "SELECT COUNT(*) FROM AllegroDisputes WHERE IFNULL(CzyZarejestrowane, 0) = 0"
                : "SELECT COUNT(*) FROM AllegroDisputes WHERE IFNULL(IsRegistered, 0) = 0";

            await using var countCmd = new MySqlCommand(sql, conn);
            return Convert.ToInt32(await countCmd.ExecuteScalarAsync());
        }
        catch
        {
            return fallback;
        }
    }

    private async Task FillProcessingComplaintsAsync(MySqlConnection conn, SyncDashboardDto dto)
    {
        const string sql = @"
            SELECT
                z.NrZgloszenia,
                CASE
                    WHEN k.NazwaFirmy IS NOT NULL AND k.NazwaFirmy != '' AND k.ImieNazwisko IS NOT NULL AND k.ImieNazwisko != '' 
                        THEN CONCAT(k.NazwaFirmy, ' | ', k.ImieNazwisko)
                    WHEN k.NazwaFirmy IS NOT NULL AND k.NazwaFirmy != '' THEN k.NazwaFirmy
                    WHEN k.ImieNazwisko IS NOT NULL AND k.ImieNazwisko != '' THEN k.ImieNazwisko
                    ELSE 'Brak klienta'
                END AS Klient,
                COALESCE(p.NazwaKrotka, '') AS Produkt,
                COALESCE(z.OpisUsterki, '') AS OpisUsterki,
                DATEDIFF(NOW(), z.DataZgloszenia) AS DniPoZgloszeniu
            FROM Zgloszenia z
            LEFT JOIN Klienci k ON z.KlientID = k.Id
            LEFT JOIN Produkty p ON z.ProduktID = p.Id
            WHERE z.StatusOgolny = 'Procesowana'
            ORDER BY z.Id DESC";

        try
        {
            await using var cmd = new MySqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                dto.ProcessingComplaints.Add(new DashboardComplaintDto
                {
                    NrZgloszenia = reader["NrZgloszenia"]?.ToString() ?? "",
                    Klient = reader["Klient"]?.ToString() ?? "Brak klienta",
                    Produkt = reader["Produkt"]?.ToString() ?? "",
                    DniPoZgloszeniu = SafeInt(reader, "DniPoZgloszeniu")
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Błąd pobierania procesowanych zgłoszeń");
        }
    }

    private async Task FillRemindersAsync(MySqlConnection conn, SyncDashboardDto dto)
    {
        const string sql = @"
            SELECT Id, Tresc, DotyczyZgloszenia, Status
            FROM Przypomnienia
            WHERE Status = 'Nowe' OR Status = 'Active' OR Status IS NULL OR Status = ''
            ORDER BY DataPrzypomnienia DESC
            LIMIT 200";

        try
        {
            await using var cmd = new MySqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tresc = reader["Tresc"]?.ToString() ?? "";
                var cat = ClassifyCategory(tresc);
                dto.Reminders.Add(new DashboardReminderDto
                {
                    Id = SafeInt(reader, "Id"),
                    Tresc = tresc,
                    DotyczyZgloszenia = reader["DotyczyZgloszenia"]?.ToString() ?? "",
                    Kategoria = cat,
                    Kolor = GetColorForCategory(cat, tresc)
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Błąd pobierania przypomnień");
        }
    }

    private async Task FillChangeLogAsync(MySqlConnection conn, SyncDashboardDto dto)
    {
        const string sql = @"
            SELECT 
                DATE_FORMAT(Data, '%d-%m %H:%i') AS Kiedy, 
                Akcja AS Zdarzenie, 
                Uzytkownik, 
                DotyczyZgloszenia AS NrZgloszenia 
            FROM Dziennik 
            ORDER BY Id DESC 
            LIMIT 100";

        try
        {
            await using var cmd = new MySqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                dto.ChangeLog.Add(new ChangeLogEntryDto
                {
                    Kiedy = reader["Kiedy"]?.ToString() ?? "",
                    Zdarzenie = reader["Zdarzenie"]?.ToString() ?? "",
                    Uzytkownik = reader["Uzytkownik"]?.ToString() ?? "",
                    NrZgloszenia = reader["NrZgloszenia"]?.ToString() ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Błąd pobierania dziennika");
        }
    }

    private void FillSyncStatuses(SyncDashboardDto dto)
    {
        var allegro = _allegroSync.GetStatusSnapshot();
        var dpd = _dpdSync.GetStatusSnapshot();
        var google = _googleSync.GetStatusSnapshot();

        dto.Services = new List<SyncServiceInfoDto>
        {
            new()
            {
                Name = "Allegro",
                Status = allegro.IsRunning ? "Trwa..." : (allegro.LastRunSuccess ? "OK" : "Błąd"),
                Details = allegro.LastError ?? $"Nowe: {allegro.UnregisteredDisputesCount}, Wiadomości: {allegro.DisputesWithNewMessages}",
                IsRunning = allegro.IsRunning,
                LastRunAt = allegro.LastCompletedAt,
                LastRunSuccess = allegro.LastRunSuccess
            },
            new()
            {
                Name = "Google Sheets",
                Status = google.IsRunning ? "Trwa..." : (google.LastSuccess ? "OK" : (google.LastError != null ? "Błąd" : "Oczekiwanie...")),
                Details = google.LastError ?? $"Wiersze: {google.MetricValue}",
                IsRunning = google.IsRunning,
                LastRunAt = google.LastFinishedAt,
                LastRunSuccess = google.LastSuccess
            },
            new()
            {
                Name = "Przesyłki DPD",
                Status = dpd.IsRunning ? "Trwa..." : (dpd.LastSuccess ? "OK" : (dpd.LastError != null ? "Błąd" : "Oczekiwanie...")),
                Details = dpd.LastError ?? $"Zmiany: {dpd.MetricValue}",
                IsRunning = dpd.IsRunning,
                LastRunAt = dpd.LastFinishedAt,
                LastRunSuccess = dpd.LastSuccess
            }
        };
    }

    // ========== HELPERS ==========

    private static int SafeInt(MySqlDataReader reader, string col)
    {
        try { return Convert.ToInt32(reader[col]); }
        catch { return 0; }
    }

    private static string ClassifyCategory(string t)
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

    private static string GetColorForCategory(string cat, string t)
    {
        var text = (t ?? "").ToUpperInvariant();
        if (text.Contains("[PROBLEM]") || text.Contains("[ZWROT]") || text.Contains("[ZGUBIONA]")) return "#CD5C5C";
        if (cat == "Kurier") return "#6495ED";
        if (cat == "Czas na decyzję") return "#FFA500";
        return "#D3D3D3";
    }
}
