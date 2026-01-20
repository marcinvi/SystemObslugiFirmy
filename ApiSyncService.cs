using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Serwis synchronizacji dwukierunkowej między Windows Forms a REST API
    /// Zarządza komunikacją, cache'owaniem i aktualizacjami w czasie rzeczywistym
    /// </summary>
    public class ApiSyncService
    {
        private readonly ReklamacjeApiClient _apiClient;
        private static ApiSyncService _instance;
        private bool _isInitialized = false;
        private UserInfo _currentUser;

        // Cache zgłoszeń
        private List<ZgloszenieApi> _cachedZgloszenia = new List<ZgloszenieApi>();
        private DateTime _lastSyncTime = DateTime.MinValue;

        public static ApiSyncService Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("ApiSyncService nie został zainicjalizowany. Użyj Initialize() najpierw.");
                }
                return _instance;
            }
        }

        public bool IsInitialized => _isInitialized;
        public bool IsAuthenticated => _apiClient?.IsAuthenticated ?? false;
        public UserInfo CurrentUser => _currentUser;
        public string BaseUrl => _apiClient?.BaseUrl;

        private ApiSyncService(string baseUrl)
        {
            _apiClient = new ReklamacjeApiClient(baseUrl);
        }

        /// <summary>
        /// Inicjalizuje serwis z podanym URL API
        /// </summary>
        public static void Initialize(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("URL API nie może być pusty", nameof(baseUrl));
            }

            _instance = new ApiSyncService(baseUrl);
            _instance._isInitialized = true;
        }

        /// <summary>
        /// Sprawdza czy można połączyć się z API
        /// </summary>
        public static async Task<bool> TestConnectionAsync(string baseUrl)
        {
            try
            {
                var testClient = new ReklamacjeApiClient(baseUrl);
                return await testClient.CheckHealthAsync();
            }
            catch
            {
                return false;
            }
        }

        // ===== AUTENTYKACJA =====

        /// <summary>
        /// Loguje użytkownika do API
        /// </summary>
        public async Task<bool> LoginAsync(string login, string password)
        {
            try
            {
                var response = await _apiClient.LoginAsync(login, password);
                _currentUser = response.User;

                // Zapisz token w settings
                SaveToken(response.Token, response.TokenExpiry);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd logowania do API: {ex.Message}", 
                    "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Próbuje zalogować automatycznie używając zapisanego tokenu
        /// </summary>
        public async Task<bool> AutoLoginAsync()
        {
            try
            {
                var token = GetSavedToken();
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }

                var expiry = GetSavedTokenExpiry();
                if (expiry <= DateTime.Now)
                {
                    ClearSavedToken();
                    return false;
                }

                _apiClient.SetToken(token);

                // Spróbuj pobrać dane użytkownika żeby sprawdzić czy token działa
                var zgloszenia = await _apiClient.GetMojeZgloszeniaAsync(1, 1);
                
                return true;
            }
            catch
            {
                ClearSavedToken();
                return false;
            }
        }

        /// <summary>
        /// Wylogowuje użytkownika
        /// </summary>
        public void Logout()
        {
            _apiClient.Logout();
            _currentUser = null;
            _cachedZgloszenia.Clear();
            _lastSyncTime = DateTime.MinValue;
            ClearSavedToken();
        }

        // ===== SYNCHRONIZACJA ZGŁOSZEŃ =====

        /// <summary>
        /// Synchronizuje zgłoszenia z API (pobiera wszystkie)
        /// </summary>
        public async Task<List<ZgloszenieApi>> SyncZgloszeniaAsync(bool forceRefresh = false)
        {
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Musisz być zalogowany żeby synchronizować zgłoszenia");
            }

            try
            {
                // Użyj cache jeśli dane są świeże (< 5 minut)
                if (!forceRefresh && _cachedZgloszenia.Any() && 
                    (DateTime.Now - _lastSyncTime).TotalMinutes < 5)
                {
                    return _cachedZgloszenia;
                }

                // Pobierz wszystkie zgłoszenia
                var zgloszenia = await _apiClient.GetAllZgloszeniaAsync();
                
                _cachedZgloszenia = zgloszenia;
                _lastSyncTime = DateTime.Now;

                return zgloszenia;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd synchronizacji zgłoszeń: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Pobiera zgłoszenie po ID (z cache lub z API)
        /// </summary>
        public async Task<ZgloszenieApi> GetZgloszenieAsync(int id)
        {
            // Sprawdź cache
            var cached = _cachedZgloszenia.FirstOrDefault(z => z.Id == id);
            if (cached != null && (DateTime.Now - _lastSyncTime).TotalMinutes < 5)
            {
                return cached;
            }

            // Pobierz z API
            var zgloszenie = await _apiClient.GetZgloszenieByIdAsync(id);
            
            // Zaktualizuj cache
            var index = _cachedZgloszenia.FindIndex(z => z.Id == id);
            if (index >= 0)
            {
                _cachedZgloszenia[index] = zgloszenie;
            }
            else
            {
                _cachedZgloszenia.Add(zgloszenie);
            }

            return zgloszenie;
        }

        /// <summary>
        /// Tworzy nowe zgłoszenie
        /// </summary>
        public async Task<ZgloszenieApi> CreateZgloszenieAsync(
            int klientId, 
            int produktId, 
            string usterka,
            string nrFaktury = null,
            string dataZakupu = null,
            string nrSeryjny = null)
        {
            var request = new CreateZgloszenieRequest
            {
                KlientId = klientId,
                ProduktId = produktId,
                Usterka = usterka,
                NrFaktury = nrFaktury,
                DataZakupu = dataZakupu,
                NrSeryjny = nrSeryjny
            };

            var noweZgloszenie = await _apiClient.CreateZgloszenieAsync(request);
            
            // Dodaj do cache
            _cachedZgloszenia.Insert(0, noweZgloszenie);

            return noweZgloszenie;
        }

        /// <summary>
        /// Aktualizuje status zgłoszenia
        /// </summary>
        public async Task<ZgloszenieApi> UpdateStatusAsync(int zgloszenieId, string nowyStatus, string komentarz = null)
        {
            var zaktualizowane = await _apiClient.UpdateStatusAsync(zgloszenieId, nowyStatus, komentarz);
            
            // Zaktualizuj cache
            var index = _cachedZgloszenia.FindIndex(z => z.Id == zgloszenieId);
            if (index >= 0)
            {
                _cachedZgloszenia[index] = zaktualizowane;
            }

            return zaktualizowane;
        }

        /// <summary>
        /// Dodaje notatkę do zgłoszenia
        /// </summary>
        public async Task<DzialanieApi> AddNotatkaAsync(int zgloszenieId, string tresc)
        {
            var notatka = await _apiClient.AddNotatkaAsync(zgloszenieId, tresc);
            
            // Zaktualizuj cache zgłoszenia
            var zgloszenie = _cachedZgloszenia.FirstOrDefault(z => z.Id == zgloszenieId);
            if (zgloszenie != null)
            {
                if (zgloszenie.Dzialania == null)
                {
                    zgloszenie.Dzialania = new List<DzialanieApi>();
                }
                zgloszenie.Dzialania.Insert(0, notatka);
            }

            return notatka;
        }

        // ===== KLIENCI =====

        /// <summary>
        /// Pobiera listę klientów
        /// </summary>
        public async Task<List<KlientApi>> GetKlienciAsync()
        {
            return await _apiClient.GetKlienciAsync();
        }

        /// <summary>
        /// Wyszukuje klientów
        /// </summary>
        public async Task<List<KlientApi>> SearchKlienciAsync(string query)
        {
            return await _apiClient.SearchKlienciAsync(query);
        }

        // ===== ZWROTY =====

        /// <summary>
        /// Pobiera zwroty z magazynu
        /// </summary>
        public async Task<List<ZwrotApi>> GetZwrotyMagazynAsync()
        {
            var result = await _apiClient.GetZwrotyAsync("warehouse");
            return result?.Items ?? new List<ZwrotApi>();
        }

        /// <summary>
        /// Pobiera zwroty handlowe
        /// </summary>
        public async Task<List<ZwrotApi>> GetZwrotyHandloweAsync()
        {
            var result = await _apiClient.GetZwrotyAsync("sales");
            return result?.Items ?? new List<ZwrotApi>();
        }

        // ===== POMOCNICZE =====

        /// <summary>
        /// Czyści cache - wymusza ponowne pobranie danych
        /// </summary>
        public void ClearCache()
        {
            _cachedZgloszenia.Clear();
            _lastSyncTime = DateTime.MinValue;
        }

        /// <summary>
        /// Zwraca informacje o ostatniej synchronizacji
        /// </summary>
        public string GetLastSyncInfo()
        {
            if (_lastSyncTime == DateTime.MinValue)
            {
                return "Brak synchronizacji";
            }

            var elapsed = DateTime.Now - _lastSyncTime;
            if (elapsed.TotalMinutes < 1)
            {
                return "Przed chwilą";
            }
            else if (elapsed.TotalMinutes < 60)
            {
                return $"{(int)elapsed.TotalMinutes} min temu";
            }
            else
            {
                return $"{(int)elapsed.TotalHours} godz. temu";
            }
        }

        // ===== ZARZĄDZANIE TOKENEM =====

        private void SaveToken(string token, DateTime expiry)
        {
            try
            {
                Properties.Settings.Default.ApiToken = token;
                Properties.Settings.Default.ApiTokenExpiry = expiry;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisywania tokenu: {ex.Message}");
            }
        }

        private string GetSavedToken()
        {
            try
            {
                return Properties.Settings.Default.ApiToken;
            }
            catch
            {
                return null;
            }
        }

        private DateTime GetSavedTokenExpiry()
        {
            try
            {
                return Properties.Settings.Default.ApiTokenExpiry;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private void ClearSavedToken()
        {
            try
            {
                Properties.Settings.Default.ApiToken = string.Empty;
                Properties.Settings.Default.ApiTokenExpiry = DateTime.MinValue;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd czyszczenia tokenu: {ex.Message}");
            }
        }
    }
}
