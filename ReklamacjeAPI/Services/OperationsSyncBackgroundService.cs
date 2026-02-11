namespace ReklamacjeAPI.Services;

public sealed class OperationsSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OperationsSyncBackgroundService> _logger;

    public OperationsSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<OperationsSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[OperationsSyncBackground] Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dpd = scope.ServiceProvider.GetRequiredService<DpdSyncCoordinatorService>();
                var google = scope.ServiceProvider.GetRequiredService<GoogleSyncCoordinatorService>();

                await dpd.TriggerSyncAsync("background");
                await google.TriggerSyncAsync("background");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OperationsSyncBackground] Unexpected error during DPD/Google sync.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // graceful shutdown
            }
        }

        _logger.LogInformation("[OperationsSyncBackground] Service stopped.");
    }
}
