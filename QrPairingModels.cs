using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    public class QrPairingPayload
    {
        [JsonProperty("pcIp")]
        public string PcIp { get; set; }

        [JsonProperty("pcPort")]
        public int PcPort { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("apiBaseUrl")]
        public string ApiBaseUrl { get; set; }
    }

    public class QrPairingRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("phoneIp")]
        public string PhoneIp { get; set; }

        [JsonProperty("pairingCode")]
        public string PairingCode { get; set; }
    }
}
