using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using QRCoder;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Model konfiguracji dla QR Code
    /// </summary>
    public class QrSyncConfig
    {
        [JsonProperty("version")]
        public string Version { get; set; } = "1.0";

        [JsonProperty("type")]
        public string Type { get; set; } = "ENA_SYNC";

        [JsonProperty("config")]
        public ConfigData Config { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        public class ConfigData
        {
            [JsonProperty("apiBaseUrl")]
            public string ApiBaseUrl { get; set; }

            [JsonProperty("phoneIp")]
            public string PhoneIp { get; set; }

            [JsonProperty("pairingCode")]
            public string PairingCode { get; set; }

            [JsonProperty("userName")]
            public string UserName { get; set; }

            [JsonProperty("timestamp")]
            public DateTime Timestamp { get; set; }
        }
    }

    /// <summary>
    /// Generator QR Code dla synchronizacji z Android
    /// </summary>
    public static class QrCodeGenerator
    {
        private const int QR_CODE_SIZE = 20; // Rozmiar modułu QR
        private const int QR_IMAGE_SIZE = 300; // Rozmiar obrazka w pikselach

        /// <summary>
        /// Generuje konfigurację dla QR Code
        /// </summary>
        public static QrSyncConfig.ConfigData GenerateConfig(string apiBaseUrl, string phoneIp, string userName)
        {
            // Wygeneruj losowy kod parowania (6 cyfr)
            var random = new Random();
            var pairingCode = random.Next(100000, 999999).ToString();

            return new QrSyncConfig.ConfigData
            {
                ApiBaseUrl = apiBaseUrl,
                PhoneIp = phoneIp,
                PairingCode = pairingCode,
                UserName = userName,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Generuje JSON dla QR Code z podpisem
        /// </summary>
        public static string GenerateQrCodeJson(QrSyncConfig.ConfigData config)
        {
            var qrConfig = new QrSyncConfig
            {
                Config = config
            };

            // Wygeneruj JSON bez podpisu
            var jsonWithoutSignature = JsonConvert.SerializeObject(qrConfig.Config, Formatting.None);

            // Oblicz podpis SHA256
            qrConfig.Signature = ComputeSha256Hash(jsonWithoutSignature);

            // Zwróć pełny JSON z podpisem
            return JsonConvert.SerializeObject(qrConfig, Formatting.None);
        }

        /// <summary>
        /// Generuje obrazek QR Code
        /// </summary>
        public static Bitmap GenerateQrCodeImage(string jsonData)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                    jsonData, 
                    QRCodeGenerator.ECCLevel.Q // Średni poziom korekcji błędów
                );
                
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Wygeneruj czarno-biały QR Code
                    Bitmap qrCodeImage = qrCode.GetGraphic(
                        QR_CODE_SIZE,
                        Color.Black,
                        Color.White,
                        true
                    );

                    // Zwróć kopię aby uniknąć problemów z dysponowaniem
                    return new Bitmap(qrCodeImage);
                }
            }
        }

        /// <summary>
        /// Oblicza hash SHA256 dla danych
        /// </summary>
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - zwraca tablicę bajtów
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Konwertuj tablicę bajtów na string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Weryfikuje podpis QR Code
        /// </summary>
        public static bool VerifySignature(QrSyncConfig qrConfig)
        {
            try
            {
                // Wygeneruj JSON bez podpisu
                var jsonWithoutSignature = JsonConvert.SerializeObject(qrConfig.Config, Formatting.None);

                // Oblicz oczekiwany podpis
                var expectedSignature = ComputeSha256Hash(jsonWithoutSignature);

                // Porównaj podpisy
                return qrConfig.Signature == expectedSignature;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sprawdza czy QR Code nie wygasł
        /// </summary>
        public static bool IsExpired(QrSyncConfig qrConfig, int expiryMinutes = 5)
        {
            var expiryTime = qrConfig.Config.Timestamp.AddMinutes(expiryMinutes);
            return DateTime.UtcNow > expiryTime;
        }

        /// <summary>
        /// Parsuje JSON z QR Code
        /// </summary>
        public static QrSyncConfig ParseQrCodeJson(string jsonData)
        {
            try
            {
                return JsonConvert.DeserializeObject<QrSyncConfig>(jsonData);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Niepoprawny format QR Code: {ex.Message}");
            }
        }

        /// <summary>
        /// Waliduje kompletną konfigurację z QR Code
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateConfig(string jsonData)
        {
            try
            {
                // 1. Parsuj JSON
                var config = ParseQrCodeJson(jsonData);

                // 2. Sprawdź wersję
                if (config.Version != "1.0")
                {
                    return (false, $"Nieobsługiwana wersja: {config.Version}");
                }

                // 3. Sprawdź typ
                if (config.Type != "ENA_SYNC")
                {
                    return (false, $"Niepoprawny typ: {config.Type}");
                }

                // 4. Sprawdź czy nie wygasł
                if (IsExpired(config))
                {
                    var age = DateTime.UtcNow - config.Config.Timestamp;
                    return (false, $"QR Code wygasł {age.TotalMinutes:F1} minut temu");
                }

                // 5. Sprawdź podpis
                if (!VerifySignature(config))
                {
                    return (false, "Niepoprawny podpis - dane mogły zostać zmodyfikowane");
                }

                // 6. Sprawdź czy pola nie są puste
                if (string.IsNullOrWhiteSpace(config.Config.ApiBaseUrl))
                {
                    return (false, "Brak URL API");
                }

                if (string.IsNullOrWhiteSpace(config.Config.PhoneIp))
                {
                    return (false, "Brak IP telefonu");
                }

                if (string.IsNullOrWhiteSpace(config.Config.PairingCode) || config.Config.PairingCode.Length != 6)
                {
                    return (false, "Niepoprawny kod parowania");
                }

                // Wszystko OK
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Błąd walidacji: {ex.Message}");
            }
        }
    }
}
