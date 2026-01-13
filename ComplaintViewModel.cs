using System;
using System.ComponentModel; // Ważne dla ładnych nazw kolumn

namespace Reklamacje_Dane
{
    public class ComplaintViewModel
    {
        // Te pola będą widoczne w menu wyboru kolumn
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Nr Zgłoszenia")]
        public string NrZgloszenia { get; set; }

        [DisplayName("Data")]
        public DateTime? DataZgloszenia { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Klient")]
        public string Klient { get; set; }

        [DisplayName("Produkt")]
        public string Produkt { get; set; }

        [DisplayName("S/N")]
        public string SN { get; set; }

        [DisplayName("Faktura")]
        public string FV { get; set; }

        [DisplayName("Źródło")]
        public string Skad { get; set; }

        [DisplayName("Producent")]
        public string Producent { get; set; }

        [DisplayName("Model")] // Dodatkowe pole
        public string NazwaKrotka { get; set; }

        [DisplayName("Kod Prod.")] // Dodatkowe pole
        public string KodProducenta { get; set; }

        [DisplayName("Data Zakupu")]
        public DateTime? DataZakupu { get; set; }

       

        [DisplayName("Data Zamknięcia")]
        public DateTime? DataZamkniecia { get; set; }

        [DisplayName("Usterka")]
        public string Usterka { get; set; }

        [DisplayName("Opis Usterki")]
        public string OpisUsterki { get; set; }

        [DisplayName("Uwagi")]
        public string Uwagi { get; set; }

        [DisplayName("Opiekun")]
        public string Opiekun { get; set; }

        [DisplayName("NIP")]
        public string KlientNip { get; set; }

        [DisplayName("Allegro Login")]
        public string AllegroBuyerLogin { get; set; }

        [DisplayName("Allegro Order")]
        public string AllegroOrderId { get; set; }

        [DisplayName("Allegro Dispute")]
        public string AllegroDisputeId { get; set; }

        [DisplayName("Allegro Konto")]
        public string AllegroAccountId { get; set; }
        [DisplayName("Gwarancja Płatna")]
        public string GwarancjaPlatna { get; set; }

        [DisplayName("Status Klient")]
        public string StatusKlient { get; set; }

        [DisplayName("Status Producent")]
        public string StatusProducent { get; set; }

        [DisplayName("Czekamy na Dostawę")]
        public string CzekamyNaDostawe { get; set; }

        [DisplayName("Nr WRL")]
        public string NrWRL { get; set; }

        [DisplayName("Nr KWZ2")]
        public string NrKWZ2 { get; set; }

        [DisplayName("Nr RMA")]
        public string NrRMA { get; set; }

        [DisplayName("Nr KPZN")]
        public string NrKPZN { get; set; }

        [DisplayName("Czy Nota Rozliczona")]
        public string CzyNotaRozliczona { get; set; }

        [DisplayName("Kwota Zwrotu")]
        public string KwotaZwrotu { get; set; }

        [DisplayName("Nr Faktury Przychodu")]
        public string NrFakturyPrzychodu { get; set; }

        [DisplayName("Kwota Przychodu Netto")]
        public double? KwotaFakturyPrzychoduNetto { get; set; }

        [DisplayName("Nr Faktury Kosztowej")]
        public string NrFakturyKosztowej { get; set; }

        [DisplayName("Działania")]
        public string Dzialania { get; set; }

        [DisplayName("Klient (opis)")]
        public string KlientOpis { get; set; }

        [DisplayName("Produkt (opis)")]
        public string ProduktOpis { get; set; }

        [DisplayName("Nazwa Systemowa")]
        public string NazwaSystemowa { get; set; }

        [DisplayName("Kod Enova")]
        public string KodEnova { get; set; }

        [DisplayName("Kategoria")]
        public string Kategoria { get; set; }

        [DisplayName("Wymagania")]
        public string Wymagania { get; set; }

        [DisplayName("Imię i Nazwisko")]
        public string ImieNazwisko { get; set; }

        [DisplayName("Nazwa Firmy")]
        public string NazwaFirmy { get; set; }

        [DisplayName("Ulica")]
        public string Ulica { get; set; }

        [DisplayName("Kod Pocztowy")]
        public string KodPocztowy { get; set; }

        [DisplayName("Miejscowość")]
        public string Miejscowosc { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Telefon")]
        public string Telefon { get; set; }

        // To pole jest ukryte (nie ma DisplayName lub ignorowane w logice)
        [Browsable(false)]
        public string SearchVector { get; private set; }

        public void BuildSearchVector()
        {
            // Łączymy wszystko w jeden ciąg dla szybkości
            SearchVector = $"{NrZgloszenia} {Klient} {Produkt} {SN} {FV} {Skad} {Producent} {Status} {NazwaKrotka} {KodProducenta} {Usterka} {OpisUsterki} {Uwagi} {Opiekun} {AllegroBuyerLogin} {AllegroOrderId} {AllegroDisputeId} {AllegroAccountId} {KlientNip} {GwarancjaPlatna} {StatusKlient} {StatusProducent} {CzekamyNaDostawe} {NrWRL} {NrKWZ2} {NrRMA} {NrKPZN} {CzyNotaRozliczona} {KwotaZwrotu} {NrFakturyPrzychodu} {KwotaFakturyPrzychoduNetto} {NrFakturyKosztowej} {Dzialania} {KlientOpis} {ProduktOpis} {NazwaSystemowa} {KodEnova} {Kategoria} {Wymagania} {ImieNazwisko} {NazwaFirmy} {Ulica} {KodPocztowy} {Miejscowosc} {Email} {Telefon}".ToLower();
        }
    }
}
