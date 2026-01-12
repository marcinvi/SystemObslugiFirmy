using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Manager do preładowania danych w tle przy starcie aplikacji.
    /// UŻYCIE: Wywołaj BackgroundDataLoader.Instance.StartPreloading() 
    /// w konstruktorze lub Load event głównego formularza (np. ReklamacjeControl).
    /// </summary>
    public sealed class BackgroundDataLoader
    {
        private static readonly Lazy<BackgroundDataLoader> _instance = 
            new Lazy<BackgroundDataLoader>(() => new BackgroundDataLoader());
        
        public static BackgroundDataLoader Instance => _instance.Value;

        private readonly FastDataService _service = new FastDataService();
        private Task _loadingTask;
        private bool _isLoading;
        private bool _isLoaded;

        private BackgroundDataLoader() { }

        /// <summary>
        /// Rozpoczyna preładowanie danych w tle. 
        /// Bezpieczne do wywołania wielokrotnie - ignoruje kolejne wywołania jeśli ładowanie trwa.
        /// </summary>
        public void StartPreloading()
        {
            if (_isLoading || _isLoaded) return;

            _isLoading = true;
            _loadingTask = Task.Run(async () =>
            {
                try
                {
                    // Sprawdź czy dane już są w cache
                    if (DataCache.Instance.HasData())
                    {
                        _isLoaded = true;
                        _isLoading = false;
                        return;
                    }

                    // Pobierz dane z bazy
                    var data = await _service.LoadAllComplaintsAsync();
                    
                    // Zapisz do cache
                    DataCache.Instance.SetData(data);
                    
                    _isLoaded = true;
                }
                catch (Exception ex)
                {
                    // W przypadku błędu, loguj i kontynuuj
                    System.Diagnostics.Debug.WriteLine($"BackgroundDataLoader Error: {ex.Message}");
                }
                finally
                {
                    _isLoading = false;
                }
            });
        }

        /// <summary>
        /// Sprawdza czy preładowanie zostało zakończone.
        /// </summary>
        public bool IsLoaded => _isLoaded;

        /// <summary>
        /// Czeka na zakończenie preładowania (opcjonalne).
        /// </summary>
        public async Task WaitForLoadingAsync()
        {
            if (_loadingTask != null && !_loadingTask.IsCompleted)
            {
                await _loadingTask;
            }
        }

        /// <summary>
        /// Wymusza przeładowanie danych (np. po aktualizacji w bazie).
        /// </summary>
        public void ForceReload()
        {
            _isLoaded = false;
            DataCache.Instance.Clear();
            StartPreloading();
        }
    }

    /// <summary>
    /// PRZYKŁADOWE UŻYCIE W ReklamacjeControl:
    /// 
    /// public ReklamacjeControl()
    /// {
    ///     InitializeComponent();
    ///     
    ///     // Rozpocznij preładowanie w tle przy starcie aplikacji
    ///     BackgroundDataLoader.Instance.StartPreloading();
    /// }
    /// 
    /// // Możesz też dodać pokazywanie statusu:
    /// private void ReklamacjeControl_Load(object sender, EventArgs e)
    /// {
    ///     if (BackgroundDataLoader.Instance.IsLoaded)
    ///     {
    ///         statusLabel.Text = "Dane gotowe";
    ///     }
    ///     else
    ///     {
    ///         statusLabel.Text = "Ładowanie danych w tle...";
    ///         // Opcjonalnie: zaczekaj na zakończenie
    ///         // await BackgroundDataLoader.Instance.WaitForLoadingAsync();
    ///         // statusLabel.Text = "Dane gotowe";
    ///     }
    /// }
    /// 
    /// ZALETY:
    /// - Dane są już w pamięci gdy użytkownik otwiera WyszukiwarkaZgloszenForm
    /// - Natychmiastowe otwarcie formularza bez czekania na bazę
    /// - Nie spowalnia startu aplikacji (działa w tle)
    /// - Automatyczne cache'owanie
    /// 
    /// WADY:
    /// - Zużywa ~50-100MB RAM (w zależności od ilości danych)
    /// - Jeśli baza jest wolna, może trwać kilka sekund
    /// 
    /// REKOMENDACJA:
    /// - Używaj jeśli użytkownicy często otwierają wyszukiwarkę
    /// - NIE używaj jeśli:
    ///   * Aplikacja działa na słabych komputerach (mało RAM)
    ///   * Wyszukiwarka rzadko używana
    ///   * Baza danych jest bardzo duża (>100k rekordów)
    /// </summary>
}
