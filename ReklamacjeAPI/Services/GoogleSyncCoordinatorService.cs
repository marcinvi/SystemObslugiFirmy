using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Services;

public class GoogleSyncCoordinatorService
{
    private const string SpreadsheetId = "1VXGP4Cckt6NmSHtiv-Um7nqg-itLMczAGd-5a_Tc4Ds";
    private static readonly string[] Sheets = ["B", "Z"];

    private readonly ILogger<GoogleSyncCoordinatorService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private readonly SyncServiceStatusDto _status = new()
    {
        Name = "Google",
        MetricLabel = "Wiersze"
    };

    public GoogleSyncCoordinatorService(
        ILogger<GoogleSyncCoordinatorService> logger,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _configuration = configuration;
        _environment = environment;
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

            var credentialPath = ResolveCredentialPath();
            if (string.IsNullOrWhiteSpace(credentialPath) || !File.Exists(credentialPath))
            {
                throw new InvalidOperationException("Brak pliku credentials Google dla synchronizacji.");
            }

            GoogleCredential credential;
            await using (var stream = File.OpenRead(credentialPath))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            }

            var service = new SheetsService(new BaseClientService.Initializer { HttpClientInitializer = credential });
            var total = 0;
            foreach (var sheet in Sheets)
            {
                var response = await service.Spreadsheets.Values.Get(SpreadsheetId, $"{sheet}!A:A").ExecuteAsync();
                if (response.Values != null)
                {
                    total += response.Values.Count(row => row.Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString())));
                }
            }

            _status.MetricValue = Math.Max(0, total - 2);
            _status.LastSuccess = true;
            _status.LastFinishedAt = DateTime.Now;
            _logger.LogInformation("[Google] source={Source}, rows={Rows}", source, _status.MetricValue);

            return GetStatusSnapshot();
        }
        catch (Exception ex)
        {
            _status.LastSuccess = false;
            _status.LastError = ex.Message;
            _status.LastFinishedAt = DateTime.Now;
            _logger.LogError(ex, "[Google] błąd synchronizacji");
            return GetStatusSnapshot();
        }
        finally
        {
            _status.IsRunning = false;
            _gate.Release();
        }
    }

    private string? ResolveCredentialPath()
    {
        var configured = _configuration["GoogleSync:CredentialFile"];
        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(configured))
        {
            candidates.Add(Path.IsPathRooted(configured)
                ? configured
                : Path.Combine(_environment.ContentRootPath, configured));
        }

        candidates.Add(Path.Combine(_environment.ContentRootPath, "reklamacje-baza-c36d05b0ffdb.json"));
        candidates.Add(Path.Combine(_environment.ContentRootPath, "reklamacje-baza-ed853b4e33f7.json"));
        candidates.Add(Path.Combine(_environment.ContentRootPath, "..", "reklamacje-baza-c36d05b0ffdb.json"));
        candidates.Add(Path.Combine(_environment.ContentRootPath, "..", "reklamacje-baza-ed853b4e33f7.json"));

        return candidates.FirstOrDefault(File.Exists);
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
