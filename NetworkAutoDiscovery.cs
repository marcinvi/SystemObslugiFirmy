using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Automatyczne wykrywanie IP telefonu i REST API w sieci lokalnej
    /// </summary>
    public class NetworkAutoDiscovery
    {
        /// <summary>
        /// Znajduje telefon Android z aplikacją ENA w sieci lokalnej
        /// Skanuje sieć i sprawdza port 8080
        /// </summary>
        public static async Task<string> FindPhoneInNetworkAsync(Action<string> progressCallback = null)
        {
            try
            {
                progressCallback?.Invoke("🔍 Szukam telefonu w sieci...");

                // Pobierz lokalny IP komputera
                string localIp = GetLocalIPAddress();
                if (string.IsNullOrEmpty(localIp))
                {
                    progressCallback?.Invoke("❌ Nie można określić lokalnego IP");
                    return null;
                }

                progressCallback?.Invoke($"📍 Twoje IP: {localIp}");

                // Wyodrębnij prefiks sieci (np. 192.168.1)
                string networkPrefix = string.Join(".", localIp.Split('.').Take(3));
                progressCallback?.Invoke($"🌐 Skanuję sieć: {networkPrefix}.0/24");

                // Skanuj sieć (1-254)
                var tasks = new List<Task<string>>();
                for (int i = 1; i <= 254; i++)
                {
                    string ip = $"{networkPrefix}.{i}";
                    if (ip == localIp) continue; // Pomiń własny IP

                    tasks.Add(CheckIfPhoneAsync(ip, progressCallback));
                }

                // Czekaj na wszystkie
                var results = await Task.WhenAll(tasks);

                // Znajdź pierwszy działający telefon
                var phoneIp = results.FirstOrDefault(ip => !string.IsNullOrEmpty(ip));

                if (!string.IsNullOrEmpty(phoneIp))
                {
                    progressCallback?.Invoke($"✅ Znaleziono telefon: {phoneIp}");
                    return phoneIp;
                }
                else
                {
                    progressCallback?.Invoke("❌ Nie znaleziono telefonu w sieci");
                    return null;
                }
            }
            catch (Exception ex)
            {
                progressCallback?.Invoke($"❌ Błąd: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Sprawdza czy pod danym IP jest telefon z aplikacją ENA
        /// </summary>
        private static async Task<string> CheckIfPhoneAsync(string ip, Action<string> progressCallback)
        {
            TcpClient client = null;
            try
            {
                // Sprawdź czy host odpowiada (ping)
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(ip, 100); // 100ms timeout
                    if (reply.Status != IPStatus.Success)
                    {
                        return null;
                    }
                }

                progressCallback?.Invoke($"🔍 Sprawdzam {ip}...");

                // Sprawdź czy port 8080 jest otwarty
                client = new TcpClient();
                var connectTask = client.ConnectAsync(ip, 8080);
                var timeoutTask = Task.Delay(500); // 500ms timeout

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                
                if (completedTask == connectTask)
                {
                    try
                    {
                        // Sprawdź czy nie było błędu podczas połączenia
                        await connectTask; // To może rzucić wyjątek
                        
                        if (client.Connected)
                        {
                            // Port otwarty - sprawdź czy to ENA
                            client.Close();
                            client.Dispose();
                            
                            var phoneClient = new PhoneClient(ip);
                            var status = await phoneClient.CheckCallStatus();
                            
                            if (status != null)
                            {
                                // To jest telefon z ENA!
                                progressCallback?.Invoke($"✅ Telefon znaleziony: {ip}");
                                return ip;
                            }
                        }
                    }
                    catch
                    {
                        // Błąd połączenia - pomiń ten host
                        return null;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                // Zawsze zamknij i zwolnij TcpClient
                try
                {
                    if (client != null)
                    {
                        client.Close();
                        client.Dispose();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Znajduje REST API w sieci lokalnej
        /// Sprawdza localhost i sieć lokalną
        /// </summary>
        public static async Task<string> FindApiInNetworkAsync(Action<string> progressCallback = null)
        {
            try
            {
                progressCallback?.Invoke("🔍 Szukam REST API...");

                // Lista możliwych lokalizacji API
                var possibleUrls = new List<string>
                {
                    "https://localhost:5001",
                    "http://localhost:5000",
                    "https://127.0.0.1:5001",
                    "http://127.0.0.1:5000"
                };

                // Dodaj lokalne IP
                string localIp = GetLocalIPAddress();
                if (!string.IsNullOrEmpty(localIp))
                {
                    possibleUrls.Add($"https://{localIp}:5001");
                    possibleUrls.Add($"http://{localIp}:5000");
                }

                // Sprawdź każdy URL
                foreach (var url in possibleUrls)
                {
                    progressCallback?.Invoke($"🔍 Sprawdzam {url}...");

                    bool isAvailable = await ApiSyncService.TestConnectionAsync(url);
                    if (isAvailable)
                    {
                        progressCallback?.Invoke($"✅ Znaleziono API: {url}");
                        return url;
                    }
                }

                // Jeśli nie znaleziono na localhost, skanuj sieć
                progressCallback?.Invoke("🌐 Skanuję sieć lokalną...");
                
                string networkPrefix = string.Join(".", localIp.Split('.').Take(3));
                var networkTasks = new List<Task<string>>();

                for (int i = 1; i <= 254; i++)
                {
                    string ip = $"{networkPrefix}.{i}";
                    if (ip == localIp) continue;

                    networkTasks.Add(CheckIfApiAsync(ip, progressCallback));
                }

                var results = await Task.WhenAll(networkTasks);
                var apiUrl = results.FirstOrDefault(url => !string.IsNullOrEmpty(url));

                if (!string.IsNullOrEmpty(apiUrl))
                {
                    progressCallback?.Invoke($"✅ Znaleziono API: {apiUrl}");
                    return apiUrl;
                }

                progressCallback?.Invoke("❌ Nie znaleziono REST API");
                return null;
            }
            catch (Exception ex)
            {
                progressCallback?.Invoke($"❌ Błąd: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Sprawdza czy pod danym IP jest REST API
        /// </summary>
        private static async Task<string> CheckIfApiAsync(string ip, Action<string> progressCallback)
        {
            try
            {
                // Sprawdź HTTPS
                string httpsUrl = $"https://{ip}:5001";
                if (await ApiSyncService.TestConnectionAsync(httpsUrl))
                {
                    return httpsUrl;
                }

                // Sprawdź HTTP
                string httpUrl = $"http://{ip}:5000";
                if (await ApiSyncService.TestConnectionAsync(httpUrl))
                {
                    return httpUrl;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Pobiera lokalne IP komputera
        /// </summary>
        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        // Preferuj IP zaczynające się od 192.168
                        if (ip.ToString().StartsWith("192.168"))
                        {
                            return ip.ToString();
                        }
                    }
                }

                // Jeśli nie znaleziono 192.168, zwróć pierwsze IPv4
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Automatyczna pełna konfiguracja - znajduje wszystko
        /// </summary>
        public static async Task<AutoConfigResult> AutoConfigureAsync(Action<string> progressCallback = null)
        {
            var result = new AutoConfigResult();

            try
            {
                progressCallback?.Invoke("🚀 Rozpoczynam automatyczną konfigurację...");

                // 1. Znajdź REST API
                progressCallback?.Invoke("");
                progressCallback?.Invoke("📡 KROK 1/3: Szukam REST API...");
                result.ApiUrl = await FindApiInNetworkAsync(progressCallback);

                if (!string.IsNullOrEmpty(result.ApiUrl))
                {
                    result.ApiFound = true;
                    
                    // Inicjalizuj API
                    try
                    {
                        ApiSyncService.Initialize(result.ApiUrl);
                        progressCallback?.Invoke("✅ REST API skonfigurowane!");
                    }
                    catch (Exception ex)
                    {
                        progressCallback?.Invoke($"⚠️ Błąd inicjalizacji API: {ex.Message}");
                    }
                }
                else
                {
                    progressCallback?.Invoke("⚠️ REST API nie znalezione - synchronizacja nie będzie działać");
                }

                // 2. Znajdź telefon
                progressCallback?.Invoke("");
                progressCallback?.Invoke("📱 KROK 2/3: Szukam telefonu Android...");
                result.PhoneIp = await FindPhoneInNetworkAsync(progressCallback);

                if (!string.IsNullOrEmpty(result.PhoneIp))
                {
                    result.PhoneFound = true;
                    
                    // Zapisz IP telefonu
                    Properties.Settings.Default.PhoneIP = result.PhoneIp;
                    Properties.Settings.Default.Save();
                    
                    progressCallback?.Invoke("✅ Telefon skonfigurowany!");
                }
                else
                {
                    progressCallback?.Invoke("⚠️ Telefon nie znaleziony - SMS i dzwonienie nie będzie działać");
                }

                // 3. Podsumowanie
                progressCallback?.Invoke("");
                progressCallback?.Invoke("📊 KROK 3/3: Podsumowanie konfiguracji");
                progressCallback?.Invoke("");
                progressCallback?.Invoke("═══════════════════════════════════");
                
                if (result.ApiFound)
                {
                    progressCallback?.Invoke($"✅ REST API: {result.ApiUrl}");
                }
                else
                {
                    progressCallback?.Invoke("❌ REST API: Nie znaleziono");
                }

                if (result.PhoneFound)
                {
                    progressCallback?.Invoke($"✅ Telefon: {result.PhoneIp}:8080");
                }
                else
                {
                    progressCallback?.Invoke("❌ Telefon: Nie znaleziono");
                }

                progressCallback?.Invoke("═══════════════════════════════════");
                progressCallback?.Invoke("");

                if (result.ApiFound && result.PhoneFound)
                {
                    progressCallback?.Invoke("🎉 Konfiguracja zakończona pomyślnie!");
                    result.Success = true;
                }
                else if (result.ApiFound || result.PhoneFound)
                {
                    progressCallback?.Invoke("⚠️ Częściowa konfiguracja - niektóre funkcje mogą nie działać");
                    result.Success = true;
                }
                else
                {
                    progressCallback?.Invoke("❌ Nie znaleziono żadnych urządzeń");
                    result.Success = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                progressCallback?.Invoke($"❌ Błąd krytyczny: {ex.Message}");
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }
    }

    /// <summary>
    /// Wynik automatycznej konfiguracji
    /// </summary>
    public class AutoConfigResult
    {
        public bool Success { get; set; }
        public bool ApiFound { get; set; }
        public bool PhoneFound { get; set; }
        public string ApiUrl { get; set; }
        public string PhoneIp { get; set; }
        public string ErrorMessage { get; set; }
    }
}
