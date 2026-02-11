namespace ReklamacjeAPI.Services;

public sealed class AllegroSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AllegroSyncBackgroundService> _logger;

    public AllegroSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<AllegroSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[AllegroSyncBackground] Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var coordinator = scope.ServiceProvider.GetRequiredService<AllegroSyncCoordinatorService>();
                await coordinator.TriggerSyncAsync("background");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AllegroSyncBackground] Unexpected error during scheduled sync.");
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

        _logger.LogInformation("[AllegroSyncBackground] Service stopped.");
    }
}
