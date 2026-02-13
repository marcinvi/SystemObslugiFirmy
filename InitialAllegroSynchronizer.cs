// ############################################################################
// Plik: InitialAllegroSynchronizer.cs (ZOPTYMALIZOWANY)
// ############################################################################

using Reklamacje_Dane.Allegro;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace Reklamacje_Dane
{
    public static class InitialAllegroSynchronizer
    {
        public static async Task PerformSyncAsync(IProgress<(string message, Color color)> progress)
        {
            progress?.Report(("Uruchamianie synchronizacji Allegro przez ReklamacjeAPI...", Color.Black));

            try
            {
                var apiSync = ApiSyncService.Instance;
                if (!apiSync.IsInitialized || !apiSync.IsAuthenticated)
                {
                    throw new InvalidOperationException("Synchronizacja Allegro wymaga aktywnego połączenia i logowania do ReklamacjeAPI.");
                }

                var run = await apiSync.TriggerAllegroSyncAsync();
                if (!run.Success)
                {
                    throw new Exception(run.Message ?? "Nie udało się uruchomić synchronizacji Allegro przez API.");
                }

                progress?.Report((run.Message ?? "Synchronizacja Allegro została uruchomiona przez API.", Color.ForestGreen));
            }
            catch (Exception ex)
            {
                progress?.Report(($"Błąd synchronizacji Allegro przez API: {ex.Message}", Color.Red));
                throw;
            }
        }

        public static async Task PerformTokenRefreshAsync(IProgress<(string message, Color color)> progress)
        {
            progress.Report(("Pobieranie listy kont Allegro...", Color.Black));

            // 1. Pobieramy dane (to jest szybki odczyt, nie blokuje bazy na długo)
            var accounts = await DatabaseHelper.GetAuthorizedAccountsForTokenRefreshAsync();

            if (!accounts.Any())
            {
                progress.Report(("Brak autoryzowanych kont do odświeżenia.", Color.Black));
                return;
            }

            foreach (var account in accounts)
            {
                string accountName = account.AccountName;
                string expirationString = account.TokenExpirationDate;
                int accountId = (int)account.Id;

                // 2. SPRAWDZENIE DATY (Optymalizacja)
                // Sprawdzamy datę w pamięci RAM, zamiast od razu pchać się do bazy/API.
                // Dzięki temu unikamy niepotrzebnego otwierania połączenia, co zmniejsza ryzyko "Database locked".

                bool needsRefresh = true;
                if (DateTime.TryParse(expirationString, out DateTime expireDate))
                {
                    // Jeśli token jest ważny jeszcze przez ponad 2 godziny, pomijamy odświeżanie
                    if (expireDate > DateTime.Now.AddHours(2))
                    {
                        needsRefresh = false;
                        progress.Report(($"Token dla konta {accountName} jest aktualny (Ważny do: {expireDate}).", Color.ForestGreen));
                    }
                }

                if (!needsRefresh) continue;

                // 3. Właściwe odświeżanie (tylko gdy konieczne)
                progress.Report(($"Weryfikacja/Odświeżanie tokena dla: {accountName}...", Color.Black));

                try
                {
                    // Tu korzystamy z AllegroApiService. 
                    // Dzięki ustawieniu 'Busy Timeout=5000' w ConnectionString, 
                    // ta linijka sama poczeka, jeśli baza jest zajęta, bez potrzeby pętli while/retry w C#.
                    var apiService = new AllegroApiService(DatabaseHelper.GetConnectionString());
                    await apiService.CreateClientForAccountAsync(accountId);

                    progress.Report(($"Konto {accountName} zweryfikowane pomyślnie.", Color.ForestGreen));
                }
                catch (Exception ex)
                {
                    // Logujemy błąd, ale nie przerywamy pracy dla innych kont
                    progress.Report(($"Błąd weryfikacji dla {accountName}: {ex.Message}", Color.Red));
                }
            }
        }
    }
}