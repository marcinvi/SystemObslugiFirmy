using Microsoft.Extensions.Hosting;

namespace ReklamacjeAPI.Services;

public sealed class ReturnsSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReturnsSyncBackgroundService> _logger;

    public ReturnsSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ReturnsSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Allegro][Zwroty] Uruchomiono background service synchronizacji zwrotów Allegro.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var returnsService = scope.ServiceProvider.GetRequiredService<ReturnsService>();

                await returnsService.SyncReturnsFromAllegroInternalAsync(
                    request: null,
                    userDisplayName: "SYSTEM_BG",
                    progress: null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Allegro][Zwroty] Błąd podczas cyklu synchronizacji zwrotów.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
