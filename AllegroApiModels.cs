// Plik: AllegroApiModels.cs
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    // Definicja potrzebna dla AllegroApiService
    public class AllegroFullAccount
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpirationDate { get; set; }
    }

    // Definicja potrzebna do odczytania odpowiedzi z API
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        // Ta właściwość (`ExpiresIn`) rozwiązuje błąd CS1061
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
    // NOWA KLASA: Reprezentuje obiekt odpowiedzi z API, który zawiera listę wiadomości
    public class ChatMessageResponse
    {
        [JsonProperty("chat")]
        public List<ChatMessage> Chat { get; set; }
    }

    public class ChatMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("author")]
        public ChatAuthor Author { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("attachment")]
        public ChatAttachment Attachment { get; set; }
    }

    public class ChatAuthor
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }
    }

    public class ChatAttachment
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}

// Definicja wyjątku

