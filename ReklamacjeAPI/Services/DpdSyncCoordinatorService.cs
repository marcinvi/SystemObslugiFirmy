using MySqlConnector;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Services;

public class DpdSyncCoordinatorService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DpdSyncCoordinatorService> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private readonly SyncServiceStatusDto _status = new()
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

            // Synchronizacja DPD po stronie API: centralna analiza zmian w przesyłkach
            // (statusy ustawiane przez integrację DPD), liczba zmian wymagających reakcji użytkownika.
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

            _logger.LogInformation("[DpdSync] source={Source}, pending-changes={Count}", source, _status.MetricValue);
            return GetStatusSnapshot();
        }
        catch (Exception ex)
        {
            _status.LastSuccess = false;
            _status.LastError = ex.Message;
            _status.LastFinishedAt = DateTime.Now;
            _logger.LogError(ex, "[DpdSync] błąd synchronizacji");
            return GetStatusSnapshot();
        }
        finally
        {
            _status.IsRunning = false;
            _gate.Release();
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
