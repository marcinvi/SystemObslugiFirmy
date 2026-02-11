using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;

namespace ReklamacjeAPI.Services;

public sealed class AllegroTokenRefreshBackgroundService : BackgroundService
{
    private static readonly ConcurrentDictionary<int, SemaphoreSlim> AccountLocks = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AllegroTokenRefreshBackgroundService> _logger;

    public AllegroTokenRefreshBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<AllegroTokenRefreshBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Uruchomiono background service odświeżania tokenów Allegro.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RefreshExpiringTokensAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas cyklu odświeżania tokenów Allegro.");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    private async Task RefreshExpiringTokensAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var credentialsService = scope.ServiceProvider.GetRequiredService<AllegroCredentialsService>();
        var allegroApiClient = scope.ServiceProvider.GetRequiredService<AllegroApiClient>();

        var accounts = await credentialsService.GetAuthorizedAccountsAsync();
        foreach (var account in accounts)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (account.TokenExpirationDate.HasValue && account.TokenExpirationDate.Value > DateTime.Now.AddHours(2))
            {
                continue;
            }

            var gate = AccountLocks.GetOrAdd(account.Id, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync(cancellationToken);
            try
            {
                await allegroApiClient.ForceRefreshTokenAsync(account.Id);
                _logger.LogInformation("Odświeżono token Allegro dla konta {AccountId} ({AccountName}).", account.Id, account.Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Nie udało się odświeżyć tokenu Allegro dla konta {AccountId} ({AccountName}).", account.Id, account.Name);
            }
            finally
            {
                gate.Release();
            }
        }
    }
}
