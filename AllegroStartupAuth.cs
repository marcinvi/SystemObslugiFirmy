using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Reklamacje_Dane.Allegro; // Potrzebne dla AllegroToken

namespace Reklamacje_Dane
{
    public class AllegroStartupAuth
    {
        private readonly IProgress<(string message, Color color)> _progress;
        private readonly AllegroAccountService _accountService;
        private readonly DziennikLogger _logger = new DziennikLogger();

        public AllegroStartupAuth(IProgress<(string message, Color color)> progress)
            : this(progress, new AllegroAccountService(new DatabaseService(DatabaseHelper.GetConnectionString())))
        {
        }

        public AllegroStartupAuth(IProgress<(string message, Color color)> progress, AllegroAccountService accountService)
        {
            _progress = progress;
            _accountService = accountService;
        }

        public async Task PerformAuthCheckAndRefreshAsync()
        {
            _progress.Report(("Rozpoczynanie weryfikacji autoryzacji kont Allegro...", Color.Blue));

            List<AllegroAccountData> accounts = await _accountService.GetAccountsForStartupAuthAsync();

            if (accounts.Count == 0)
            {
                _progress.Report(("Brak skonfigurowanych kont Allegro do weryfikacji.", Color.Black));
                await Task.Delay(1000);
                return;
            }

            foreach (var account in accounts)
            {
                _progress.Report(($"Sprawdzam konto: {account.AccountName}", Color.Black));
                bool success = await ProcessSingleAllegroAccountAuth(account);

                if (!success)
                {
                    _progress.Report(($"    -> Konto '{account.AccountName}' wymaga ręcznej autoryzacji.", Color.OrangeRed));
                }
            }

            _progress.Report(("Weryfikacja autoryzacji kont Allegro zakończona.", Color.Blue));
            await Task.Delay(1000);
        }

        // Metoda przetwarzająca dane pojedynczego konta Allegro.
        private async Task<bool> ProcessSingleAllegroAccountAuth(AllegroAccountData account)
        {
            try
            {
                string decryptedClientSecret = EncryptionHelper.DecryptString(account.ClientSecretEncrypted);

                if (string.IsNullOrEmpty(account.AccessTokenEncrypted) ||
                    string.IsNullOrEmpty(account.RefreshTokenEncrypted))
                {
                    _progress.Report(($"  -> Brak tokenów dla konta '{account.AccountName}'. Wymagana pełna autoryzacja.", Color.Red));
                    await _accountService.MarkAccountAsUnauthorizedAsync(account.Id);
                    return false;
                }

                AllegroToken currentToken = new AllegroToken
                {
                    AccessToken = EncryptionHelper.DecryptString(account.AccessTokenEncrypted),
                    RefreshToken = EncryptionHelper.DecryptString(account.RefreshTokenEncrypted),
                    ExpirationDate = account.TokenExpirationDate
                };

                AllegroApiClient apiClient = new AllegroApiClient(account.ClientId, decryptedClientSecret)
                {
                    Token = currentToken
                };

                // Sprawdzamy, czy token dostępu wymaga odświeżenia
                if (currentToken.ExpirationDate <= DateTime.Now.AddMinutes(5))
                {
                    _progress.Report(($"  -> Token dla konta '{account.AccountName}' przeterminowany lub bliski wygaśnięcia. Próbuję odświeżyć...", Color.Orange));
                    try
                    {
                        AllegroToken newToken = await apiClient.RefreshTokenAsync();
                        await _accountService.SaveRefreshedTokenAsync(account.Id, newToken);
                        _progress.Report(($"  -> Token dla konta '{account.AccountName}' odświeżony pomyślnie.", Color.Green));
                        return true;
                    }
                    catch (AllegroRefreshTokenException ex)
                    {
                        // Specyficzny błąd - token odświeżający wygasł. Konto wymaga pełnej reautoryzacji.
                        _progress.Report(($"  -> BŁĄD: Token odświeżający dla konta '{account.AccountName}' wygasł. {ex.Message}", Color.Red));
                        await _accountService.MarkAccountAsUnauthorizedAsync(account.Id);
                        ToastManager.ShowToast("Autoryzacja Allegro",
                            $"Sesja dla konta '{account.AccountName}' wygasła. Proszę autoryzować je ponownie w ustawieniach.", NotificationType.Error);
                        await _logger.DodajAsync(Program.fullName,
                            $"Token odświeżający wygasł dla konta {account.AccountName}: {ex.Message}", "0");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        // Inny błąd podczas odświeżania (np. problem z siecią). Nie oznaczaj konta jako nieautoryzowanego.
                        _progress.Report(($"  -> BŁĄD: Nie udało się odświeżyć tokena dla konta '{account.AccountName}'. {ex.Message}", Color.DarkRed));
                        await _logger.DodajAsync(Program.fullName,
                            $"Błąd odświeżania tokena dla konta {account.AccountName}: {ex.Message}", "0");
                        return false;
                    }
                }
                else
                {
                    _progress.Report(($"  -> Token dla konta '{account.AccountName}' jest nadal ważny.", Color.Green));
                    return true;
                }
            }
            catch (Exception ex)
            {
                _progress.Report(($"  -> BŁĄD KRYTYCZNY podczas przetwarzania konta '{account.AccountName}': {ex.Message}", Color.DarkRed));
                await _logger.DodajAsync(Program.fullName,
                    $"Błąd krytyczny przetwarzania konta {account.AccountName}: {ex.Message}", "0");
                return false;
            }
        }
    }
}
