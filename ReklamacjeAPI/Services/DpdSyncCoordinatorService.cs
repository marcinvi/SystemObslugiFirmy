using MySqlConnector;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Services;

public class DpdSyncCoordinatorService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DpdSyncCoordinatorService> _logger;

    // STATIC — stan współdzielony między instancjami Scoped
    private static readonly SemaphoreSlim _gate = new(1, 1);
    private static readonly SyncServiceStatusDto _status = new()
    {
        Name = "DPD",
        MetricLabel = "Zmiany"
    };

    public DpdSyncCoordinatorService(IConfiguration configuration, ILogger<DpdSyncCoordinatorService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public SyncServiceStatusDto GetStatusSnapshot() => Clone(_status);

    public async Task<SyncServiceStatusDto> TriggerSyncAsync(string source)
    {
        if (!await _gate.WaitAsync(0))
        {
            return GetStatusSnapshot();
        }

        try
        {
            _status.IsRunning = true;
            _status.LastStartedAt = DateTime.Now;
            _status.LastError = null;

            await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await connection.OpenAsync();

            const string sql = @"
                SELECT COUNT(*)
                FROM Przesylki p
                WHERE p.CzyDoreczona = 0
                  AND p.OstatniStatus LIKE '[%'
                  AND p.OstatniStatus <> IFNULL(p.LastNotificationStatus, '')";

            await using var command = new MySqlCommand(sql, connection);
            _status.MetricValue = Convert.ToInt32(await command.ExecuteScalarAsync());
            _status.LastSuccess = true;
            _status.LastFinishedAt = DateTime.Now;

            await WriteSyncRunAsync(connection, "DPD", _status.LastStartedAt ?? DateTime.Now, true, _status.MetricValue);

            _logger.LogInformation("[DPD] source={Source}, pending-changes={Count}", source, _status.MetricValue);
            return GetStatusSnapshot();
        }
        catch (Exception ex)
        {
            _status.LastSuccess = false;
            _status.LastError = ex.Message;
            _status.LastFinishedAt = DateTime.Now;
            _logger.LogError(ex, "[DPD] błąd synchronizacji");

            try
            {
                await using var errConn = DbConnectionFactory.CreateDefaultConnection(_configuration);
                await errConn.OpenAsync();
                await WriteSyncRunAsync(errConn, "DPD", _status.LastStartedAt ?? DateTime.Now, false, 0, ex.Message);
            }
            catch { /* best-effort */ }

            return GetStatusSnapshot();
        }
        finally
        {
            _status.IsRunning = false;
            _gate.Release();
        }
    }

    private async Task WriteSyncRunAsync(MySqlConnection conn, string serviceName, DateTime startedAt,
        bool success, int itemsProcessed, string? errorMessage = null)
    {
        try
        {
            await using var cmd = new MySqlCommand(@"
                INSERT INTO SyncRuns (source, started_at, finished_at, ok, rows_written, error_message)
                VALUES (@src, @started, NOW(), @ok, @rows, @err)", conn);
            cmd.Parameters.AddWithValue("@src", serviceName);
            cmd.Parameters.AddWithValue("@started", startedAt);
            cmd.Parameters.AddWithValue("@ok", success ? 1 : 0);
            cmd.Parameters.AddWithValue("@rows", itemsProcessed);
            cmd.Parameters.AddWithValue("@err", (object?)errorMessage ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WriteSyncRunAsync failed for {Service}", serviceName);
        }
    }

    private static SyncServiceStatusDto Clone(SyncServiceStatusDto source)
    {
        return new SyncServiceStatusDto
        {
            Name = source.Name,
            IsRunning = source.IsRunning,
            LastSuccess = source.LastSuccess,
            LastStartedAt = source.LastStartedAt,
            LastFinishedAt = source.LastFinishedAt,
            LastError = source.LastError,
            MetricValue = source.MetricValue,
            MetricLabel = source.MetricLabel
        };
    }
}
