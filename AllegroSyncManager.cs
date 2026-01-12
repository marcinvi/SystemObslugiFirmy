using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class AllegroSyncManager
    {
        private static AllegroSyncManager _instance;
        public static AllegroSyncManager Instance => _instance ?? (_instance = new AllegroSyncManager());

        private readonly AllegroSyncService _syncService;
        private Timer _syncTimer;
        private volatile bool _isSyncRunning = false;
        private readonly object _syncLock = new object();

        public event Action<AllegroSyncResult> SyncCompleted;
        public event Action<string> SyncStatusChanged;

        private AllegroSyncManager()
        {
            _syncService = new AllegroSyncService();
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
                SyncStatusChanged?.Invoke("Rozpoczęto synchronizację z Allegro...");
                // #################### POPRAWKA BŁĘDU KOMPILACJI ####################
                // Przekazujemy `null` jako argument `progress`, ponieważ ten menedżer
                // działa w tle i nie ma bezpośredniego dostępu do interfejsu użytkownika.
                var result = await _syncService.SynchronizeDisputesAsync(null);
                SyncCompleted?.Invoke(result);
                SyncStatusChanged?.Invoke("Synchronizacja zakończona.");
            }
            catch (Exception ex)
            {
                // Logowanie błędów
                System.Diagnostics.Debug.WriteLine($"Błąd podczas synchronizacji Allegro: {ex.Message}");
                SyncStatusChanged?.Invoke("Błąd synchronizacji.");
            }
            finally
            {
                _isSyncRunning = false;
            }
        }
    }
}