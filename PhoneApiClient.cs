using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    // ==========================================================================
    // PhoneApiClient - zamiennik PhoneClient
    // Komunikacja przez ReklamacjeAPI zamiast bezpośrednio z telefonem.
    // Nie wymaga IP telefonu ani kodu parowania.
    // ==========================================================================

    public class PhoneApiClient
    {
        public static PhoneApiClient Instance { get; set; }

        private readonly string _apiBaseUrl;
        private readonly string _userLogin;
        private readonly HttpClient _client;

        /// <summary>
        /// Czy telefon jest online (ostatni heartbeat < 90s)
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Ostatni czas widzenia telefonu
        /// </summary>
        public DateTime? LastSeen { get; private set; }

        public PhoneApiClient(string apiBaseUrl, string userLogin)
        {
            // Jeśli apiBaseUrl będzie puste, użyje domyślnego localhost, 
            // ale dzięki App.config będziesz tu podawał właściwy adres.
            _apiBaseUrl = !string.IsNullOrWhiteSpace(apiBaseUrl)
                ? apiBaseUrl.TrimEnd('/')
               : "http://localhost:50875";

            // Oczyszczamy login (używamy tylko pierwszego członu, np. "Marcin"),
            // aby zgadzał się z tym, co telefon wysyła w Heartbeat.
            _userLogin = userLogin?.Split(' ')[0] ?? "";

            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            Instance = this;
        }

        // ==================================================================
        // STATUS TELEFONU
        // ==================================================================

        /// <summary>
        /// Sprawdza czy telefon użytkownika jest online.
        /// Aktualizuje właściwość IsConnected.
        /// </summary>
        public async Task<bool> CheckPhoneOnlineAsync()
        {
            try
            {
                var response = await _client.GetStringAsync(
                    $"{_apiBaseUrl}/api/phone/status/{Uri.EscapeDataString(_userLogin)}");
                var result = JsonConvert.DeserializeObject<ApiResponseWrapper<PhoneStatusResponse>>(response);

                if (result?.Success == true && result.Data != null)
                {
                    IsConnected = result.Data.IsOnline;
                    LastSeen = result.Data.LastSeen;
                    return result.Data.IsOnline;
                }
            }
            catch { }

            IsConnected = false;
            return false;
        }

        // ==================================================================
        // POBIERANIE ZDARZEŃ Z TELEFONU (CALL, SMS)
        // ==================================================================

        /// <summary>
        /// Pobiera nowe zdarzenia z telefonu (dzwonienie, SMS-y)
        /// </summary>
        public async Task<List<PhoneEventItem>> GetEventsAsync()
        {
            try
            {
                var response = await _client.GetStringAsync(
                    $"{_apiBaseUrl}/api/phone/events/{Uri.EscapeDataString(_userLogin)}");
                var result = JsonConvert.DeserializeObject<ApiResponseWrapper<List<PhoneEventItem>>>(response);

                if (result?.Success == true && result.Data != null)
                    return result.Data;
            }
            catch { }

            return new List<PhoneEventItem>();
        }

        // ==================================================================
        // WYSYŁANIE KOMEND DO TELEFONU
        // ==================================================================

        /// <summary>
        /// Wybiera numer na telefonie (DIAL).
        /// Komenda jest wysyłana do API → Android ją pobiera i wykonuje.
        /// Działa nawet gdy aplikacja na telefonie jest zamknięta (BackgroundService).
        /// </summary>
        public async Task<bool> Dial(string number)
        {
            return await SendCommandAsync("DIAL", number, null);
        }

        /// <summary>
        /// Wysyła SMS przez telefon.
        /// </summary>
        public async Task<bool> SendSmsAsync(string number, string message)
        {
            return await SendCommandAsync("SEND_SMS", number, message);
        }

        private async Task<bool> SendCommandAsync(string commandType, string number, string content)
        {
            try
            {
                string clean = (number ?? "").Replace(" ", "").Replace("-", "");
                var payload = new
                {
                    userLogin = _userLogin,
                    commandType = commandType,
                    phoneNumber = clean,
                    content = content
                };

                var json = JsonConvert.SerializeObject(payload);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{_apiBaseUrl}/api/phone/command", httpContent);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // ==================================================================
        // KOMPATYBILNOŚĆ Z ISTNIEJĄCYM KODEM
        // ==================================================================

        /// <summary>
        /// Kompatybilność wsteczna - dawny CheckCallStatus.
        /// Teraz zdarzenia przychodzą przez GetEventsAsync().
        /// Ta metoda nie jest już potrzebna do pollingu, ale zachowana.
        /// </summary>
        public async Task<PhoneStatus> CheckCallStatus()
        {
            // Zwracamy null - zdarzenia przychodzą przez GetEventsAsync
            // Ta metoda istnieje dla kompatybilności
            await Task.CompletedTask;
            return null;
        }

        /// <summary>
        /// Kompatybilność wsteczna - dawne CheckNewSms.
        /// Teraz SMS-y przychodzą jako zdarzenia SMS_RECEIVED w GetEventsAsync().
        /// </summary>
        public async Task<List<SmsData>> CheckNewSms()
        {
            await Task.CompletedTask;
            return new List<SmsData>();
        }
    }

    // ==================================================================
    // MODELE
    // ==================================================================

    public class PhoneEventItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; } = "";

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; } = "";

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; } = "";

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class PhoneStatusResponse
    {
        [JsonProperty("isOnline")]
        public bool IsOnline { get; set; }

        [JsonProperty("lastSeen")]
        public DateTime? LastSeen { get; set; }

        [JsonProperty("phoneModel")]
        public string PhoneModel { get; set; }

        [JsonProperty("appVersion")]
        public string AppVersion { get; set; }
    }

    public class ApiResponseWrapper<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
