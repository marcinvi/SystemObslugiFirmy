using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Klient HTTP do komunikacji z REST API
    /// Obsługuje autoryzację JWT, CRUD zgłoszeń, synchronizację danych
    /// </summary>
    public class ReklamacjeApiClient
    {
        private readonly HttpClient _httpClient;
        private string _baseUrl;
        private string _token;

        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
        public string BaseUrl => _baseUrl;

        public ReklamacjeApiClient(string baseUrl)
        {
            _baseUrl = baseUrl?.TrimEnd('/');
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        // ===== AUTENTYKACJA =====

        /// <summary>
        /// Logowanie do API - otrzymuje JWT token
        /// </summary>
        public async Task<LoginResponse> LoginAsync(string login, string password)
        {
            try
            {
                var request = new LoginRequest
                {
                    Login = login,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/auth/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd logowania: {response.StatusCode} - {responseBody}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LoginResponse>>(responseBody);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    _token = apiResponse.Data.Token;
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", _token);
                    return apiResponse.Data;
                }

                throw new Exception("Nieprawidłowa odpowiedź serwera");
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas logowania: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ustawia token JWT (jeśli już go masz zapisany)
        /// </summary>
        public void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _token);
        }

        /// <summary>
        /// Wylogowanie - czyści token
        /// </summary>
        public void Logout()
        {
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // ===== ZGŁOSZENIA =====

        /// <summary>
        /// Pobiera listę zgłoszeń z paginacją
        /// </summary>
        public async Task<PaginatedResponse<ZgloszenieApi>> GetZgloszeniaAsync(int page = 1, int pageSize = 50)
        {
            CheckAuthentication();

            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/zgloszenia?page={page}&pageSize={pageSize}");
                
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd pobierania zgłoszeń: {response.StatusCode} - {responseBody}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PaginatedResponse<ZgloszenieApi>>>(responseBody);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }

                throw new Exception("Nieprawidłowa odpowiedź serwera");
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas pobierania zgłoszeń: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Pobiera wszystkie zgłoszenia (wszystkie strony)
        /// </summary>
        public async Task<List<ZgloszenieApi>> GetAllZgloszeniaAsync()
        {
            var allZgloszenia = new List<ZgloszenieApi>();
            int page = 1;
            int pageSize = 100;
            bool hasMore = true;

            while (hasMore)
            {
                var result = await GetZgloszeniaAsync(page, pageSize);
                allZgloszenia.AddRange(result.Items);

                hasMore = page < result.TotalPages;
                page++;
            }

            return allZgloszenia;
        }

        /// <summary>
        /// Pobiera zgłoszenia przypisane do zalogowanego użytkownika
        /// </summary>
        public async Task<PaginatedResponse<ZgloszenieApi>> GetMojeZgloszeniaAsync(int page = 1, int pageSize = 50)
        {
            CheckAuthentication();

            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_baseUrl}/api/zgloszenia/moje?page={page}&pageSize={pageSize}");
                
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd pobierania moich zgłoszeń: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PaginatedResponse<ZgloszenieApi>>>(responseBody);
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas pobierania moich zgłoszeń: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Pobiera szczegóły zgłoszenia po ID
        /// </summary>
        public async Task<ZgloszenieApi> GetZgloszenieByIdAsync(int id)
        {
            CheckAuthentication();

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/zgloszenia/{id}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd pobierania zgłoszenia: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ZgloszenieApi>>(responseBody);
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas pobierania zgłoszenia: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tworzy nowe zgłoszenie
        /// </summary>
        public async Task<ZgloszenieApi> CreateZgloszenieAsync(CreateZgloszenieRequest request)
        {
            CheckAuthentication();

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/zgloszenia", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd tworzenia zgłoszenia: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ZgloszenieApi>>(responseBody);
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas tworzenia zgłoszenia: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Aktualizuje status zgłoszenia
        /// </summary>
        public async Task<ZgloszenieApi> UpdateStatusAsync(int zgloszenieId, string nowyStatus, string komentarz = null)
        {
            CheckAuthentication();

            try
            {
                var statusRequest = new StatusUpdateRequest
                {
                    NowyStatus = nowyStatus,
                    Komentarz = komentarz
                };

                var json = JsonConvert.SerializeObject(statusRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), $"{_baseUrl}/api/zgloszenia/{zgloszenieId}/status")
                {
                    Content = content
                };
                var response = await _httpClient.SendAsync(httpRequest);
                
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd aktualizacji statusu: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ZgloszenieApi>>(responseBody);
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas aktualizacji statusu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Dodaje notatkę do zgłoszenia
        /// </summary>
        public async Task<DzialanieApi> AddNotatkaAsync(int zgloszenieId, string tresc)
        {
            CheckAuthentication();

            try
            {
                var request = new NotatkaRequest { Tresc = tresc };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/api/zgloszenia/{zgloszenieId}/notatka", content);
                
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd dodawania notatki: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<DzialanieApi>>(responseBody);
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas dodawania notatki: {ex.Message}", ex);
            }
        }

        // ===== KLIENCI =====

        /// <summary>
        /// Pobiera listę klientów
        /// </summary>
        public async Task<List<KlientApi>> GetKlienciAsync()
        {
            CheckAuthentication();

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/klienci");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd pobierania klientów: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<KlientApi>>>(responseBody);
                return apiResponse?.Data ?? new List<KlientApi>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas pobierania klientów: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Wyszukuje klientów po frazie
        /// </summary>
        public async Task<List<KlientApi>> SearchKlienciAsync(string query)
        {
            CheckAuthentication();

            try
            {
                var encodedQuery = Uri.EscapeDataString(query);
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/klienci/search?query={encodedQuery}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd wyszukiwania klientów: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<KlientApi>>>(responseBody);
                return apiResponse?.Data ?? new List<KlientApi>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas wyszukiwania klientów: {ex.Message}", ex);
            }
        }

        // ===== ZWROTY =====

        /// <summary>
        /// Pobiera listę zwrotów
        /// </summary>
        public async Task<PaginatedResponse<ZwrotApi>> GetZwrotyAsync(string typ = null, int page = 1, int pageSize = 50)
        {
            CheckAuthentication();

            try
            {
                var url = $"{_baseUrl}/api/returns?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrEmpty(typ))
                {
                    url += $"&type={Uri.EscapeDataString(typ)}";
                }

                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Błąd pobierania zwrotów: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PaginatedResponse<ZwrotApi>>>(responseBody);
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd podczas pobierania zwrotów: {ex.Message}", ex);
            }
        }

        // ===== HEALTH CHECK =====

        /// <summary>
        /// Sprawdza czy API jest dostępne
        /// </summary>
        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // ===== POMOCNICZE =====

        private void CheckAuthentication()
        {
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("Nie jesteś zalogowany. Użyj LoginAsync() najpierw.");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
