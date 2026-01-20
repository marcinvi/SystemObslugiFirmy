using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    // ===== MODELE ODPOWIEDZI API =====
    
    public class ApiResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        
        [JsonProperty("data")]
        public T Data { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class PaginatedResponse<T>
    {
        [JsonProperty("items")]
        public List<T> Items { get; set; }
        
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        
        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }
    }

    // ===== MODELE LOGOWANIA =====
    
    public class LoginRequest
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        
        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        
        [JsonProperty("tokenExpiry")]
        public DateTime TokenExpiry { get; set; }
        
        [JsonProperty("user")]
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("login")]
        public string Login { get; set; }
        
        [JsonProperty("nazwaWyswietlana")]
        public string NazwaWyswietlana { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("uprawnienia")]
        public string Uprawnienia { get; set; }
    }

    // ===== MODELE ZGŁOSZEŃ =====
    
    public class ZgloszenieApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("nrZgloszenia")]
        public string NrZgloszenia { get; set; }
        
        [JsonProperty("dataZgloszenia")]
        public string DataZgloszenia { get; set; }
        
        [JsonProperty("dataZakupu")]
        public string DataZakupu { get; set; }
        
        [JsonProperty("statusOgolny")]
        public string StatusOgolny { get; set; }
        
        [JsonProperty("statusKlient")]
        public string StatusKlient { get; set; }
        
        [JsonProperty("statusProducent")]
        public string StatusProducent { get; set; }
        
        [JsonProperty("usterka")]
        public string Usterka { get; set; }
        
        [JsonProperty("nrFaktury")]
        public string NrFaktury { get; set; }
        
        [JsonProperty("nrSeryjny")]
        public string NrSeryjny { get; set; }
        
        [JsonProperty("gwarancyjnaPlatna")]
        public string GwarancyjnaPlatna { get; set; }
        
        [JsonProperty("klient")]
        public KlientApi Klient { get; set; }
        
        [JsonProperty("produkt")]
        public ProduktApi Produkt { get; set; }
        
        [JsonProperty("dzialania")]
        public List<DzialanieApi> Dzialania { get; set; }
        
        [JsonProperty("nrWRL")]
        public string NrWRL { get; set; }
        
        [JsonProperty("nrKWZ2")]
        public string NrKWZ2 { get; set; }
        
        [JsonProperty("gdzieZgloszono")]
        public string GdzieZgloszono { get; set; }
    }

    public class KlientApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("imieNazwisko")]
        public string ImieNazwisko { get; set; }
        
        [JsonProperty("nazwaFirmy")]
        public string NazwaFirmy { get; set; }
        
        [JsonProperty("telefon")]
        public string Telefon { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("ulicaNumerDomu")]
        public string UlicaNumerDomu { get; set; }
        
        [JsonProperty("kodPocztowy")]
        public string KodPocztowy { get; set; }
        
        [JsonProperty("miejscowosc")]
        public string Miejscowosc { get; set; }
        
        [JsonProperty("nip")]
        public string Nip { get; set; }
        
        [JsonProperty("firma")]
        public string Firma { get; set; }
    }

    public class ProduktApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("nazwa")]
        public string Nazwa { get; set; }
        
        [JsonProperty("nazwaKrotka")]
        public string NazwaKrotka { get; set; }
        
        [JsonProperty("producent")]
        public string Producent { get; set; }
        
        [JsonProperty("kategoria")]
        public string Kategoria { get; set; }
        
        [JsonProperty("kodEnova")]
        public string KodEnova { get; set; }
        
        [JsonProperty("kodProducenta")]
        public string KodProducenta { get; set; }
    }

    public class DzialanieApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("data")]
        public DateTime Data { get; set; }
        
        [JsonProperty("tresc")]
        public string Tresc { get; set; }
        
        [JsonProperty("uzytkownik")]
        public string Uzytkownik { get; set; }
        
        [JsonProperty("typ")]
        public string Typ { get; set; }
    }

    // ===== MODELE REQUESTÓW =====
    
    public class StatusUpdateRequest
    {
        [JsonProperty("nowyStatus")]
        public string NowyStatus { get; set; }
        
        [JsonProperty("komentarz")]
        public string Komentarz { get; set; }
    }

    public class NotatkaRequest
    {
        [JsonProperty("tresc")]
        public string Tresc { get; set; }
    }

    public class CreateZgloszenieRequest
    {
        [JsonProperty("klientId")]
        public int KlientId { get; set; }
        
        [JsonProperty("produktId")]
        public int ProduktId { get; set; }
        
        [JsonProperty("usterka")]
        public string Usterka { get; set; }
        
        [JsonProperty("nrFaktury")]
        public string NrFaktury { get; set; }
        
        [JsonProperty("dataZakupu")]
        public string DataZakupu { get; set; }
        
        [JsonProperty("nrSeryjny")]
        public string NrSeryjny { get; set; }
    }

    // ===== MODELE ZWROTÓW =====
    
    public class ZwrotApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("nrZwrotu")]
        public string NrZwrotu { get; set; }
        
        [JsonProperty("dataZwrotu")]
        public DateTime DataZwrotu { get; set; }
        
        [JsonProperty("typ")]
        public string Typ { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("klient")]
        public string Klient { get; set; }
        
        [JsonProperty("telefon")]
        public string Telefon { get; set; }
        
        [JsonProperty("wartosc")]
        public decimal Wartosc { get; set; }
        
        [JsonProperty("pozycje")]
        public List<PozycjaZwrotuApi> Pozycje { get; set; }
    }

    public class PozycjaZwrotuApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("produkt")]
        public string Produkt { get; set; }
        
        [JsonProperty("ilosc")]
        public int Ilosc { get; set; }
        
        [JsonProperty("cena")]
        public decimal Cena { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    // ===== MODELE WIADOMOŚCI =====
    
    public class WiadomoscApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("data")]
        public DateTime Data { get; set; }
        
        [JsonProperty("nadawca")]
        public string Nadawca { get; set; }
        
        [JsonProperty("odbiorca")]
        public string Odbiorca { get; set; }
        
        [JsonProperty("temat")]
        public string Temat { get; set; }
        
        [JsonProperty("tresc")]
        public string Tresc { get; set; }
        
        [JsonProperty("przeczytana")]
        public bool Przeczytana { get; set; }
        
        [JsonProperty("zgloszenieId")]
        public int? ZgloszenieId { get; set; }
    }
}
