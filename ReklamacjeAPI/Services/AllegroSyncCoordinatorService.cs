using MySqlConnector;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Services;

public class AllegroSyncCoordinatorService
{
    private readonly AllegroCredentialsService _credentialsService;
    private readonly AllegroApiClient _allegroApiClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AllegroSyncCoordinatorService> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private AllegroSyncStatusDto _status = new();

    public AllegroSyncCoordinatorService(
        AllegroCredentialsService credentialsService,
        AllegroApiClient allegroApiClient,
        IConfiguration configuration,
        ILogger<AllegroSyncCoordinatorService> logger)
    {
        _credentialsService = credentialsService;
        _allegroApiClient = allegroApiClient;
        _configuration = configuration;
        _logger = logger;
    }

    public AllegroSyncStatusDto GetStatusSnapshot()
    {
        return new AllegroSyncStatusDto
        {
            IsRunning = _status.IsRunning,
            LastStartedAt = _status.LastStartedAt,
            LastCompletedAt = _status.LastCompletedAt,
            LastRunSuccess = _status.LastRunSuccess,
            LastError = _status.LastError,
            NewDisputesFoundLastRun = _status.NewDisputesFoundLastRun,
            UnregisteredDisputesCount = _status.UnregisteredDisputesCount,
            DisputesWithNewMessages = _status.DisputesWithNewMessages
        };
    }

    public async Task<AllegroSyncRunResultDto> TriggerSyncAsync(string source)
    {
        if (!await _gate.WaitAsync(0))
        {
            return new AllegroSyncRunResultDto
            {
                Success = true,
                Message = "Synchronizacja Allegro jest już uruchomiona.",
                Status = GetStatusSnapshot()
            };
        }

        try
        {
            _status.IsRunning = true;
            _status.LastStartedAt = DateTime.Now;
            _status.LastError = null;

            _logger.LogInformation("[AllegroSync] Start synchronizacji. Source={Source}", source);

            var accounts = await _credentialsService.GetAuthorizedAccountsAsync();
            var newCounter = 0;

            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            foreach (var account in accounts)
            {
                var issues = await _allegroApiClient.GetIssuesAsync(account.Id, 100, 0);
                foreach (var issue in issues)
                {
                    if (string.IsNullOrWhiteSpace(issue.Id))
                    {
                        continue;
                    }

                    var exists = await ExistsDisputeAsync(conn, issue.Id!);
                    if (!exists)
                    {
                        await InsertBasicDisputeAsync(conn, account.Id, issue);
                        newCounter++;
                    }
                    else
                    {
                        await UpdateBasicDisputeAsync(conn, issue);
                    }
                }
            }

            _status.NewDisputesFoundLastRun = newCounter;
            _status.UnregisteredDisputesCount = await GetUnregisteredDisputesCountAsync(conn);
            _status.DisputesWithNewMessages = await GetUnreadDisputesCountAsync(conn);
            _status.LastRunSuccess = true;
            _status.LastCompletedAt = DateTime.Now;

            _logger.LogInformation("[AllegroSync] Koniec synchronizacji. New={New}, Unregistered={Unregistered}, Unread={Unread}",
                _status.NewDisputesFoundLastRun,
                _status.UnregisteredDisputesCount,
                _status.DisputesWithNewMessages);

            return new AllegroSyncRunResultDto
            {
                Success = true,
                Message = "Synchronizacja Allegro zakończona.",
                Status = GetStatusSnapshot()
            };
        }
        catch (Exception ex)
        {
            _status.LastRunSuccess = false;
            _status.LastError = ex.Message;
            _status.LastCompletedAt = DateTime.Now;
            _logger.LogError(ex, "[AllegroSync] Błąd synchronizacji Allegro");

            return new AllegroSyncRunResultDto
            {
                Success = false,
                Message = $"Błąd synchronizacji Allegro: {ex.Message}",
                Status = GetStatusSnapshot()
            };
        }
        finally
        {
            _status.IsRunning = false;
            _gate.Release();
        }
    }

    private static async Task<bool> ExistsDisputeAsync(MySqlConnection conn, string disputeId)
    {
        await using var cmd = new MySqlCommand("SELECT COUNT(*) FROM AllegroDisputes WHERE DisputeId=@id", conn);
        cmd.Parameters.AddWithValue("@id", disputeId);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    private static async Task InsertBasicDisputeAsync(MySqlConnection conn, int accountId, AllegroApiClient.AllegroIssueSummaryDto issue)
    {
        const string sql = @"
INSERT INTO AllegroDisputes
(DisputeId, AllegroAccountId, Type, Subject, StatusAllegro, OpenedAt, LastCheckedAt, LastMessageCount, HasNewMessages)
VALUES
(@DisputeId, @AccountId, @Type, @Subject, @Status, @OpenedAt, NOW(), 0, 0)";

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
        cmd.Parameters.AddWithValue("@AccountId", accountId);
        cmd.Parameters.AddWithValue("@Type", (object?)issue.Type ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Subject", (object?)issue.Subject ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", (object?)issue.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@OpenedAt", issue.OpenedAt ?? DateTime.Now);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task UpdateBasicDisputeAsync(MySqlConnection conn, AllegroApiClient.AllegroIssueSummaryDto issue)
    {
        const string sql = @"
UPDATE AllegroDisputes
SET StatusAllegro=@Status,
    Subject=@Subject,
    LastCheckedAt=NOW()
WHERE DisputeId=@DisputeId";

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Status", (object?)issue.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Subject", (object?)issue.Subject ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<int> GetUnregisteredDisputesCountAsync(MySqlConnection conn)
    {
        await using var cmd = new MySqlCommand("SELECT COUNT(*) FROM AllegroDisputes WHERE ComplaintId IS NULL", conn);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private static async Task<int> GetUnreadDisputesCountAsync(MySqlConnection conn)
    {
        await using var cmd = new MySqlCommand("SELECT COUNT(*) FROM AllegroDisputes WHERE IFNULL(HasNewMessages,0)=1", conn);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }
}
