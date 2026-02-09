using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReklamacjeAPI.DTOs;
using System.Text.Json.Serialization;

namespace ReklamacjeAPI.Controllers;

/// <summary>
/// Kontroler obsługujący komunikację telefon ↔ WinForms przez API.
/// Zastępuje bezpośrednie połączenie HTTP między telefonem a komputerem.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PhoneController : ControllerBase
{
    private readonly string _connectionString;
    private readonly ILogger<PhoneController> _logger;

    public PhoneController(IConfiguration configuration, ILogger<PhoneController> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Brak ConnectionString.");
        _logger = logger;
    }

    // ====================================================================
    // ANDROID → API: Telefon wysyła zdarzenia (dzwonienie, SMS)
    // ====================================================================

    /// <summary>
    /// Android wysyła zdarzenie (CALL_RINGING, CALL_IDLE, SMS_RECEIVED)
    /// </summary>
    [HttpPost("event")]
    public async Task<ActionResult<ApiResponse<object>>> PostEvent([FromBody] PhoneEventRequest request)
    {
        string userLogin = ResolveUserLogin(request.UserLogin);
        if (string.IsNullOrWhiteSpace(userLogin))
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak loginu użytkownika."));

        if (string.IsNullOrWhiteSpace(request.EventType))
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak typu zdarzenia."));

        // Dla CALL_RINGING/CALL_IDLE - oznacz poprzednie nieodczytane zdarzenia CALL jako consumed
        if (request.EventType == "CALL_RINGING" || request.EventType == "CALL_IDLE")
        {
            await using var connClean = new MySqlConnection(_connectionString);
            await connClean.OpenAsync();
            await using var cleanCmd = new MySqlCommand(
                @"UPDATE phone_events SET IsConsumed = 1, ConsumedAt = NOW()
                  WHERE UserLogin = @user AND EventType IN ('CALL_RINGING','CALL_IDLE') AND IsConsumed = 0",
                connClean);
            cleanCmd.Parameters.AddWithValue("@user", userLogin);
            await cleanCmd.ExecuteNonQueryAsync();
        }

        // Wstaw nowe zdarzenie - osobne połączenie
        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new MySqlCommand(
            @"INSERT INTO phone_events (UserLogin, EventType, PhoneNumber, Content, ContactName)
              VALUES (@user, @type, @number, @content, @contact)", conn);
        cmd.Parameters.AddWithValue("@user", userLogin);
        cmd.Parameters.AddWithValue("@type", request.EventType);
        cmd.Parameters.AddWithValue("@number", request.PhoneNumber ?? "");
        cmd.Parameters.AddWithValue("@content", request.Content ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@contact", request.ContactName ?? "");

        await cmd.ExecuteNonQueryAsync();

        _logger.LogInformation("Phone event: {Type} from {User}, number={Number}",
            request.EventType, userLogin, request.PhoneNumber);

        return Ok(ApiResponse<object>.SuccessResponse(null, "OK"));
    }

    // ====================================================================
    // WINFORMS → API: Komputer pobiera zdarzenia z telefonu
    // ====================================================================

    /// <summary>
    /// WinForms pobiera nieodczytane zdarzenia dla danego użytkownika
    /// </summary>
    [HttpGet("events/{login}")]
    public async Task<ActionResult<ApiResponse<List<PhoneEventDto>>>> GetEvents(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            return BadRequest(ApiResponse<List<PhoneEventDto>>.ErrorResponse("Brak loginu."));

        var events = new List<PhoneEventDto>();

        // 1) SELECT - odczytaj zdarzenia (osobne połączenie + reader zamknięty przed UPDATE)
        {
            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(
                @"SELECT Id, EventType, PhoneNumber, Content, ContactName, CreatedAt
                  FROM phone_events
                  WHERE UserLogin = @user AND IsConsumed = 0
                  ORDER BY CreatedAt ASC
                  LIMIT 50", conn);
            cmd.Parameters.AddWithValue("@user", login);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                events.Add(new PhoneEventDto
                {
                    Id = reader.GetInt32("Id"),
                    EventType = reader.GetString("EventType"),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? "" : reader.GetString("PhoneNumber"),
                    Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : reader.GetString("Content"),
                    ContactName = reader.IsDBNull(reader.GetOrdinal("ContactName")) ? "" : reader.GetString("ContactName"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }
            // reader zamknięty automatycznie po wyjściu z await using
        }
        // conn zamknięte po wyjściu z bloku {}

        // 2) UPDATE - oznacz jako consumed (OSOBNE połączenie!)
        if (events.Count > 0)
        {
            await using var conn2 = new MySqlConnection(_connectionString);
            await conn2.OpenAsync();

            var ids = string.Join(",", events.Select(e => e.Id));
            await using var markCmd = new MySqlCommand(
                $"UPDATE phone_events SET IsConsumed = 1, ConsumedAt = NOW() WHERE Id IN ({ids})", conn2);
            await markCmd.ExecuteNonQueryAsync();
        }

        return Ok(ApiResponse<List<PhoneEventDto>>.SuccessResponse(events));
    }

    // ====================================================================
    // WINFORMS → API: Komputer wysyła komendy do telefonu (DIAL, SEND_SMS)
    // ====================================================================

    /// <summary>
    /// WinForms wysyła komendę do telefonu (zadzwoń/wyślij SMS)
    /// </summary>
    [HttpPost("command")]
    public async Task<ActionResult<ApiResponse<object>>> PostCommand([FromBody] PhoneCommandRequest request)
    {
        string userLogin = ResolveUserLogin(request.UserLogin);
        if (string.IsNullOrWhiteSpace(userLogin))
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak loginu użytkownika."));

        if (string.IsNullOrWhiteSpace(request.CommandType))
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak typu komendy."));

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(
            @"INSERT INTO phone_commands (UserLogin, CommandType, PhoneNumber, Content, ResultStatus)
              VALUES (@user, @type, @number, @content, 'PENDING')", conn);
        cmd.Parameters.AddWithValue("@user", userLogin);
        cmd.Parameters.AddWithValue("@type", request.CommandType);
        cmd.Parameters.AddWithValue("@number", request.PhoneNumber ?? "");
        cmd.Parameters.AddWithValue("@content", request.Content ?? (object)DBNull.Value);

        await cmd.ExecuteNonQueryAsync();

        _logger.LogInformation("Phone command: {Type} for {User}, number={Number}",
            request.CommandType, userLogin, request.PhoneNumber);

        return Ok(ApiResponse<object>.SuccessResponse(null, "OK"));
    }

    // ====================================================================
    // ANDROID → API: Telefon pobiera komendy do wykonania
    // ====================================================================

    /// <summary>
    /// Android pobiera niewykonane komendy dla danego użytkownika
    /// </summary>
    [HttpGet("commands/{login}")]
    public async Task<ActionResult<ApiResponse<List<PhoneCommandDto>>>> GetCommands(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            return BadRequest(ApiResponse<List<PhoneCommandDto>>.ErrorResponse("Brak loginu."));

        var commands = new List<PhoneCommandDto>();

        // 1) SELECT - odczytaj komendy (osobne połączenie)
        {
            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(
                @"SELECT Id, CommandType, PhoneNumber, Content, CreatedAt
                  FROM phone_commands
                  WHERE UserLogin = @user AND IsConsumed = 0
                  ORDER BY CreatedAt ASC
                  LIMIT 20", conn);
            cmd.Parameters.AddWithValue("@user", login);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                commands.Add(new PhoneCommandDto
                {
                    Id = reader.GetInt32("Id"),
                    CommandType = reader.GetString("CommandType"),
                    PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? "" : reader.GetString("PhoneNumber"),
                    Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : reader.GetString("Content"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }
            // reader zamknięty po wyjściu z await using
        }
        // conn zamknięte po wyjściu z bloku {}

        // 2) UPDATE - oznacz jako consumed (OSOBNE połączenie!)
        if (commands.Count > 0)
        {
            await using var conn2 = new MySqlConnection(_connectionString);
            await conn2.OpenAsync();

            var ids = string.Join(",", commands.Select(c => c.Id));
            await using var markCmd = new MySqlCommand(
                $"UPDATE phone_commands SET IsConsumed = 1, ConsumedAt = NOW() WHERE Id IN ({ids})", conn2);
            await markCmd.ExecuteNonQueryAsync();
        }

        return Ok(ApiResponse<List<PhoneCommandDto>>.SuccessResponse(commands));
    }

    /// <summary>
    /// Android potwierdza wykonanie komendy
    /// </summary>
    [HttpPost("command/{id}/result")]
    public async Task<ActionResult<ApiResponse<object>>> PostCommandResult(int id, [FromBody] CommandResultRequest request)
    {
        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(
            @"UPDATE phone_commands SET ResultStatus = @status WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@status", request.Status ?? "SUCCESS");
        await cmd.ExecuteNonQueryAsync();

        return Ok(ApiResponse<object>.SuccessResponse(null, "OK"));
    }

    // ====================================================================
    // HEARTBEAT: Telefon sygnalizuje że jest online
    // ====================================================================

    [HttpPost("heartbeat")]
    public async Task<ActionResult<ApiResponse<object>>> PostHeartbeat([FromBody] HeartbeatRequest request)
    {
        string userLogin = ResolveUserLogin(request.UserLogin);
        if (string.IsNullOrWhiteSpace(userLogin))
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak loginu."));

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new MySqlCommand(
            @"INSERT INTO phone_heartbeat (UserLogin, LastHeartbeat, PhoneModel, AppVersion)
              VALUES (@user, NOW(), @model, @version)
              ON DUPLICATE KEY UPDATE LastHeartbeat = NOW(), PhoneModel = @model, AppVersion = @version",
            conn);
        cmd.Parameters.AddWithValue("@user", userLogin);
        cmd.Parameters.AddWithValue("@model", request.PhoneModel ?? "");
        cmd.Parameters.AddWithValue("@version", request.AppVersion ?? "");
        await cmd.ExecuteNonQueryAsync();

        return Ok(ApiResponse<object>.SuccessResponse(null, "OK"));
    }

    /// <summary>
    /// WinForms sprawdza czy telefon użytkownika jest online
    /// (ostatni heartbeat < 90 sekund temu = online)
    /// </summary>
    [HttpGet("status/{login}")]
    public async Task<ActionResult<ApiResponse<PhoneStatusDto>>> GetStatus(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
            return BadRequest(ApiResponse<PhoneStatusDto>.ErrorResponse("Brak loginu."));

        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();

        // Sprawdź status bezpośrednio w SQL - unikamy problemów ze strefą czasową
        await using var cmd = new MySqlCommand(
            @"SELECT LastHeartbeat, PhoneModel, AppVersion,
                     TIMESTAMPDIFF(SECOND, LastHeartbeat, NOW()) AS SecondsSinceHeartbeat
              FROM phone_heartbeat WHERE UserLogin = @user",
            conn);
        cmd.Parameters.AddWithValue("@user", login);

        PhoneStatusDto status;
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var lastHb = reader.GetDateTime("LastHeartbeat");
            var secondsSince = reader.GetInt64("SecondsSinceHeartbeat");
            var isOnline = secondsSince < 90;

            status = new PhoneStatusDto
            {
                IsOnline = isOnline,
                LastSeen = lastHb,
                PhoneModel = reader.IsDBNull(reader.GetOrdinal("PhoneModel")) ? "" : reader.GetString("PhoneModel"),
                AppVersion = reader.IsDBNull(reader.GetOrdinal("AppVersion")) ? "" : reader.GetString("AppVersion")
            };
        }
        else
        {
            status = new PhoneStatusDto
            {
                IsOnline = false,
                LastSeen = null
            };
        }

        return Ok(ApiResponse<PhoneStatusDto>.SuccessResponse(status));
    }

    // ====================================================================
    // HELPERS
    // ====================================================================

    private string ResolveUserLogin(string? login)
    {
        if (!string.IsNullOrWhiteSpace(login)) return login.Trim();

        // Fallback: z nagłówka X-User
        var header = Request.Headers["X-User"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(header)) return header.Trim();

        // Fallback: z tokena JWT
        var claim = User.Identity?.Name;
        if (!string.IsNullOrWhiteSpace(claim)) return claim;

        return null;
    }
}

// ====================================================================
// DTO Models
// ====================================================================

public class PhoneEventRequest
{
    [JsonPropertyName("userLogin")]
    public string? UserLogin { get; set; }

    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = "";

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("contactName")]
    public string? ContactName { get; set; }
}

public class PhoneEventDto
{
    public int Id { get; set; }
    public string EventType { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? Content { get; set; }
    public string ContactName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class PhoneCommandRequest
{
    [JsonPropertyName("userLogin")]
    public string? UserLogin { get; set; }

    [JsonPropertyName("commandType")]
    public string CommandType { get; set; } = "";

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class PhoneCommandDto
{
    public int Id { get; set; }
    public string CommandType { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CommandResultRequest
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

public class HeartbeatRequest
{
    [JsonPropertyName("userLogin")]
    public string? UserLogin { get; set; }

    [JsonPropertyName("phoneModel")]
    public string? PhoneModel { get; set; }

    [JsonPropertyName("appVersion")]
    public string? AppVersion { get; set; }
}

public class PhoneStatusDto
{
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
    public string PhoneModel { get; set; } = "";
    public string AppVersion { get; set; } = "";
}