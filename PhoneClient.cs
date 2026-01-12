using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Reklamacje_Dane
{
    // Modele
    public class PhoneStatus { public bool dzwoni { get; set; } public string numer { get; set; } }
    public class SmsData { public string number { get; set; } public string content { get; set; } }
    public class PhoneMediaItem { public string url { get; set; } public string mime { get; set; } }

    public class PhoneClient
    {
        public static PhoneClient Instance { get; set; }
        public bool IsConnected => true; // Symulacja dla Form2, w prawdziwym HTTP nie ma stałego połączenia

        private readonly string _phoneIp;
        private readonly HttpClient _client;

        public PhoneClient(string phoneIp)
        {
            _phoneIp = phoneIp;
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
            Instance = this;
        }

        // --- DZWONIENIE (NOWA METODA) ---
        public async Task<bool> Dial(string number)
        {
            try
            {
                string clean = number.Replace(" ", "").Replace("-", "");
                // Wysyłamy żądanie do serwera na telefonie (zakładamy endpoint /call)
                var res = await _client.GetAsync($"http://{_phoneIp}:8080/call?number={Uri.EscapeDataString(clean)}");
                return res.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<PhoneStatus> CheckCallStatus()
        {
            try { return JsonConvert.DeserializeObject<PhoneStatus>(await _client.GetStringAsync($"http://{_phoneIp}:8080/stan")); }
            catch { return null; }
        }

        public async Task<List<SmsData>> CheckNewSms()
        {
            try { return JsonConvert.DeserializeObject<List<SmsData>>(await _client.GetStringAsync($"http://{_phoneIp}:8080/sms")); }
            catch { return new List<SmsData>(); }
        }

        public async Task<bool> SendSmsAsync(string number, string message)
        {
            try
            {
                string clean = number.Replace(" ", "").Replace("-", "");
                var res = await _client.GetAsync($"http://{_phoneIp}:8080/wyslij?numer={Uri.EscapeDataString(clean)}&tresc={Uri.EscapeDataString(message)}");
                return res.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<PhoneMediaItem>> GetPhonePhotosAsync()
        {
            try
            {
                string json = await _client.GetStringAsync($"http://{_phoneIp}:8080/list_photos");
                return JsonConvert.DeserializeObject<List<PhoneMediaItem>>(json);
            }
            catch { return new List<PhoneMediaItem>(); }
        }

        public async Task<byte[]> DownloadPhotoAsync(string remotePath, bool isThumbnail)
        {
            try
            {
                string type = isThumbnail ? "thumb" : "full";
                string url = $"http://{_phoneIp}:8080/get_photo?path={Uri.EscapeDataString(remotePath)}&type={type}";
                return await _client.GetByteArrayAsync(url);
            }
            catch { return null; }
        }
    }
}