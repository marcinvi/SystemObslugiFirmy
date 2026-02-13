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

 

  

    // ===== SYNC DASHBOARD — jeden endpoint, wszystkie dane =====

    public class SyncDashboardApi
    {
        [JsonProperty("unregisteredAllegroCount")]
        public int UnregisteredAllegroCount { get; set; }

        [JsonProperty("allegroNewMessages")]
        public int AllegroNewMessages { get; set; }

        [JsonProperty("unregisteredGoogleCount")]
        public int UnregisteredGoogleCount { get; set; }

        [JsonProperty("unregisteredReturnsCount")]
        public int UnregisteredReturnsCount { get; set; }

        [JsonProperty("emailUnreadCount")]
        public int EmailUnreadCount { get; set; }

        [JsonProperty("services")]
        public List<SyncServiceInfoApi> Services { get; set; } = new List<SyncServiceInfoApi>();

        [JsonProperty("processingComplaints")]
        public List<DashboardComplaintApi> ProcessingComplaints { get; set; } = new List<DashboardComplaintApi>();

        [JsonProperty("reminders")]
        public List<DashboardReminderApi> Reminders { get; set; } = new List<DashboardReminderApi>();

        [JsonProperty("changeLog")]
        public List<ChangeLogEntryApi> ChangeLog { get; set; } = new List<ChangeLogEntryApi>();

        [JsonProperty("generatedAt")]
        public DateTime GeneratedAt { get; set; }
    }

    public class SyncServiceInfoApi
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("status")]
        public string Status { get; set; } = "Oczekiwanie...";

        [JsonProperty("details")]
        public string Details { get; set; } = "";

        [JsonProperty("isRunning")]
        public bool IsRunning { get; set; }

        [JsonProperty("lastRunAt")]
        public DateTime? LastRunAt { get; set; }

        [JsonProperty("lastRunSuccess")]
        public bool LastRunSuccess { get; set; }
    }

    public class DashboardComplaintApi
    {
        [JsonProperty("nrZgloszenia")]
        public string NrZgloszenia { get; set; } = "";

        [JsonProperty("klient")]
        public string Klient { get; set; } = "";

        [JsonProperty("produkt")]
        public string Produkt { get; set; } = "";

        [JsonProperty("opisUsterki")]
        public string OpisUsterki { get; set; } = "";

        [JsonProperty("dniPoZgloszeniu")]
        public int DniPoZgloszeniu { get; set; }
    }

    public class DashboardReminderApi
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tresc")]
        public string Tresc { get; set; } = "";

        [JsonProperty("dotyczyZgloszenia")]
        public string DotyczyZgloszenia { get; set; } = "";

        [JsonProperty("kategoria")]
        public string Kategoria { get; set; } = "";

        [JsonProperty("kolor")]
        public string Kolor { get; set; } = "";
    }

    public class ChangeLogEntryApi
    {
        [JsonProperty("kiedy")]
        public string Kiedy { get; set; } = "";

        [JsonProperty("zdarzenie")]
        public string Zdarzenie { get; set; } = "";

        [JsonProperty("uzytkownik")]
        public string Uzytkownik { get; set; } = "";

        [JsonProperty("nrZgloszenia")]
        public string NrZgloszenia { get; set; } = "";
    }

    // ===== ALLEGRO SYNC STATUS =====

    public class AllegroSyncStatusApi
    {
        [JsonProperty("isRunning")]
        public bool IsRunning { get; set; }

        [JsonProperty("lastStartedAt")]
        public DateTime? LastStartedAt { get; set; }

        [JsonProperty("lastCompletedAt")]
        public DateTime? LastCompletedAt { get; set; }

        [JsonProperty("lastRunSuccess")]
        public bool LastRunSuccess { get; set; }

        [JsonProperty("lastError")]
        public string LastError { get; set; }

        [JsonProperty("newDisputesFoundLastRun")]
        public int NewDisputesFoundLastRun { get; set; }

        [JsonProperty("unregisteredDisputesCount")]
        public int UnregisteredDisputesCount { get; set; }

        [JsonProperty("disputesWithNewMessages")]
        public int DisputesWithNewMessages { get; set; }
    }

    public class AllegroSyncRunResultApi
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = "";

        [JsonProperty("status")]
        public AllegroSyncStatusApi Status { get; set; } = new AllegroSyncStatusApi();
    }

    // ===== OPERATIONS SYNC STATUS (dla ops-sync endpoint) =====

    public class OperationsSyncSnapshotApi
    {
        [JsonProperty("dpd")]
        public SyncServiceStatusApi Dpd { get; set; } = new SyncServiceStatusApi();

        [JsonProperty("google")]
        public SyncServiceStatusApi Google { get; set; } = new SyncServiceStatusApi();
    }

    public class SyncServiceStatusApi
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("isRunning")]
        public bool IsRunning { get; set; }

        [JsonProperty("lastSuccess")]
        public bool LastSuccess { get; set; }

        [JsonProperty("lastStartedAt")]
        public DateTime? LastStartedAt { get; set; }

        [JsonProperty("lastFinishedAt")]
        public DateTime? LastFinishedAt { get; set; }

        [JsonProperty("lastError")]
        public string LastError { get; set; }

        [JsonProperty("metricValue")]
        public int MetricValue { get; set; }

        [JsonProperty("metricLabel")]
        public string MetricLabel { get; set; } = "";
    }
}
