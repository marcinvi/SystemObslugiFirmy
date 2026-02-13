using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class AllegroSyncManager
    {
        private static AllegroSyncManager _instance;
        public static AllegroSyncManager Instance => _instance ?? (_instance = new AllegroSyncManager());

        private Timer _syncTimer;
        private volatile bool _isSyncRunning = false;
        private readonly object _syncLock = new object();

        public event Action<AllegroSyncResult> SyncCompleted;
        public event Action<string> SyncStatusChanged;

        private AllegroSyncManager()
        {
        }

        public void Start(TimeSpan interval)
        {
            if (_syncTimer == null)
            {
                _syncTimer = new Timer(async _ => await RunSync(), null, TimeSpan.Zero, interval);
            }
        }

        public void Stop()
        {
            _syncTimer?.Change(Timeout.Infinite, 0);
            _syncTimer?.Dispose();
            _syncTimer = null;
        }

        public async Task RunSync(bool force = false)
        {
            lock (_syncLock)
            {
                if (_isSyncRunning && !force)
                {
                    return;
                }
                _isSyncRunning = true;
            }

            try
            {
                SyncStatusChanged?.Invoke("Rozpoczęto synchronizację z Allegro przez ReklamacjeAPI...");

                var apiSync = ApiSyncService.Instance;
                if (!apiSync.IsInitialized || !apiSync.IsAuthenticated)
                {
                    throw new InvalidOperationException("Synchronizacja Allegro wymaga aktywnego połączenia i logowania do ReklamacjeAPI.");
                }

                var runResult = await apiSync.TriggerAllegroSyncAsync();
                var status = runResult?.Status ?? new AllegroSyncStatusApi();

                var result = new AllegroSyncResult
                {
                    NewDisputesFound = status.NewDisputesFoundLastRun,
                    UnregisteredDisputesCount = status.UnregisteredDisputesCount,
                    DisputesWithNewMessages = status.DisputesWithNewMessages
                };

                SyncCompleted?.Invoke(result);
                SyncStatusChanged?.Invoke(runResult?.Message ?? "Synchronizacja Allegro przez API zakończona.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas synchronizacji Allegro (API): {ex.Message}");
                SyncStatusChanged?.Invoke($"Błąd synchronizacji przez API: {ex.Message}");
            }
            finally
            {
                _isSyncRunning = false;
            }
        }
    }
}
