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
        
        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }

        [JsonProperty("totalCount")]
        private int LegacyTotalCount
        {
            set
            {
                if (TotalItems == 0)
                {
                    TotalItems = value;
                }
            }
        }
        
        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        
        [JsonProperty("totalPages")]
        private int LegacyTotalPages
        {
            set
            {
                if (TotalItems == 0 && PageSize > 0 && value > 0)
                {
                    TotalItems = value * PageSize;
                }
            }
        }

        [JsonIgnore]
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);
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
        
        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("waybill")]
        public string Waybill { get; set; }

        [JsonProperty("buyerName")]
        public string BuyerName { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("statusAllegro")]
        public string StatusAllegro { get; set; }

        [JsonProperty("statusWewnetrzny")]
        public string StatusWewnetrzny { get; set; }

        [JsonProperty("stanProduktu")]
        public string StanProduktu { get; set; }

        [JsonProperty("decyzjaHandlowca")]
        public string DecyzjaHandlowca { get; set; }

        [JsonProperty("handlowiecId")]
        public int? HandlowiecId { get; set; }

        [JsonProperty("isManual")]
        public bool IsManual { get; set; }
    }

    public class ZwrotSzczegolyApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("statusWewnetrzny")]
        public string StatusWewnetrzny { get; set; }

        [JsonProperty("statusAllegro")]
        public string StatusAllegro { get; set; }

        [JsonProperty("buyerLogin")]
        public string BuyerLogin { get; set; }

        [JsonProperty("buyerName")]
        public string BuyerName { get; set; }

        [JsonProperty("buyerPhone")]
        public string BuyerPhone { get; set; }

        [JsonProperty("buyerAddress")]
        public string BuyerAddress { get; set; }

        [JsonProperty("deliveryName")]
        public string DeliveryName { get; set; }

        [JsonProperty("deliveryAddress")]
        public string DeliveryAddress { get; set; }

        [JsonProperty("deliveryPhone")]
        public string DeliveryPhone { get; set; }

        [JsonProperty("waybill")]
        public string Waybill { get; set; }

        [JsonProperty("carrierName")]
        public string CarrierName { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("allegroAccountName")]
        public string AllegroAccountName { get; set; }

        [JsonProperty("uwagiMagazynu")]
        public string UwagiMagazynu { get; set; }

        [JsonProperty("stanProduktuId")]
        public int? StanProduktuId { get; set; }

        [JsonProperty("stanProduktuName")]
        public string StanProduktuName { get; set; }

        [JsonProperty("przyjetyPrzezId")]
        public int? PrzyjetyPrzezId { get; set; }

        [JsonProperty("przyjetyPrzezName")]
        public string PrzyjetyPrzezName { get; set; }

        [JsonProperty("dataPrzyjecia")]
        public DateTime? DataPrzyjecia { get; set; }

        [JsonProperty("decyzjaHandlowcaId")]
        public int? DecyzjaHandlowcaId { get; set; }

        [JsonProperty("decyzjaHandlowcaName")]
        public string DecyzjaHandlowcaName { get; set; }

        [JsonProperty("komentarzHandlowca")]
        public string KomentarzHandlowca { get; set; }

        [JsonProperty("dataDecyzji")]
        public DateTime? DataDecyzji { get; set; }

        [JsonProperty("isManual")]
        public bool IsManual { get; set; }

        [JsonProperty("allegroReturnId")]
        public string AllegroReturnId { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }
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
