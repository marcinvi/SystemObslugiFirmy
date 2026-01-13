using System;
using System.ComponentModel;
using System.Linq;

namespace Reklamacje_Dane
{
    public class ComplaintViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Nr Zgłoszenia")]
        public string NrZgloszenia { get; set; }

        [DisplayName("Data")]
        public DateTime? DataZgloszenia { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Status Klient")]
        public string StatusKlient { get; set; }

        [DisplayName("Status Producent")]
        public string StatusProducent { get; set; }

        [DisplayName("Klient")]
        public string Klient { get; set; }

        [DisplayName("Klient - Imię Nazwisko")]
        public string KlientImieNazwisko { get; set; }

        [DisplayName("Klient - Nazwa Firmy")]
        public string KlientNazwaFirmy { get; set; }

        [DisplayName("NIP")]
        public string KlientNip { get; set; }

        [DisplayName("Klient - Ulica")]
        public string KlientUlica { get; set; }

        [DisplayName("Klient - Kod Pocztowy")]
        public string KlientKodPocztowy { get; set; }

        [DisplayName("Klient - Miejscowość")]
        public string KlientMiejscowosc { get; set; }

        [DisplayName("Klient - Email")]
        public string KlientEmail { get; set; }

        [DisplayName("Klient - Telefon")]
        public string KlientTelefon { get; set; }

        [DisplayName("Produkt")]
        public string Produkt { get; set; }

        [DisplayName("Nazwa Systemowa")]
        public string NazwaSystemowa { get; set; }

        [DisplayName("Model")]
        public string NazwaKrotka { get; set; }

        [DisplayName("Kod Enova")]
        public string KodEnova { get; set; }

        [DisplayName("Kod Prod.")]
        public string KodProducenta { get; set; }

        [DisplayName("Kategoria")]
        public string Kategoria { get; set; }

        [DisplayName("Wymagania Produktu")]
        public string ProduktWymagania { get; set; }

        [DisplayName("Producent")]
        public string Producent { get; set; }

        [DisplayName("Producent - Kontakt Mail")]
        public string ProducentKontaktMail { get; set; }

        [DisplayName("Producent - Adres")]
        public string ProducentAdres { get; set; }

        [DisplayName("Producent - PL/ENG")]
        public string ProducentPlEng { get; set; }

        [DisplayName("Producent - Język")]
        public string ProducentJezyk { get; set; }

        [DisplayName("Producent - Formularz")]
        public string ProducentFormularz { get; set; }

        [DisplayName("Producent - Wymagania")]
        public string ProducentWymagania { get; set; }

        [DisplayName("Data Zakupu")]
        public DateTime? DataZakupu { get; set; }

        [DisplayName("S/N")]
        public string SN { get; set; }

        [DisplayName("Faktura")]
        public string FV { get; set; }

        [DisplayName("Nr Faktury Przychodu")]
        public string NrFakturyPrzychodu { get; set; }

        [DisplayName("Kwota Faktury Przychodu Netto")]
        public double? KwotaFakturyPrzychoduNetto { get; set; }

        [DisplayName("Nr Faktury Kosztowej")]
        public string NrFakturyKosztowej { get; set; }

        [DisplayName("Źródło")]
        public string Skad { get; set; }

        [DisplayName("Opis Usterki")]
        public string OpisUsterki { get; set; }

        [DisplayName("Produkt (opis)")]
        public string ProduktOpis { get; set; }

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
        public int? CzyNotaRozliczona { get; set; }

        [DisplayName("Kwota Zwrotu")]
        public string KwotaZwrotu { get; set; }

        [DisplayName("Działania")]
        public string Dzialania { get; set; }

        [Browsable(false)]
        public string SearchVector { get; private set; }

        public void BuildSearchVector()
        {
            SearchVector = string.Join(" ", new[]
            {
                NrZgloszenia,
                DataZgloszenia?.ToString("yyyy-MM-dd HH:mm"),
                Status,
                StatusKlient,
                StatusProducent,
                Klient,
                KlientImieNazwisko,
                KlientNazwaFirmy,
                KlientNip,
                KlientUlica,
                KlientKodPocztowy,
                KlientMiejscowosc,
                KlientEmail,
                KlientTelefon,
                Produkt,
                NazwaSystemowa,
                NazwaKrotka,
                KodEnova,
                KodProducenta,
                Kategoria,
                ProduktWymagania,
                Producent,
                ProducentKontaktMail,
                ProducentAdres,
                ProducentPlEng,
                ProducentJezyk,
                ProducentFormularz,
                ProducentWymagania,
                SN,
                FV,
                NrFakturyPrzychodu,
                KwotaFakturyPrzychoduNetto?.ToString(),
                NrFakturyKosztowej,
                Skad,
                DataZakupu?.ToString("yyyy-MM-dd"),
                OpisUsterki,
                ProduktOpis,
                AllegroBuyerLogin,
                AllegroOrderId,
                AllegroDisputeId,
                AllegroAccountId,
                GwarancjaPlatna,
                CzekamyNaDostawe,
                NrWRL,
                NrKWZ2,
                NrRMA,
                NrKPZN,
                CzyNotaRozliczona?.ToString(),
                KwotaZwrotu,
                Dzialania
            }.Where(value => !string.IsNullOrWhiteSpace(value))).ToLowerInvariant();
        }
    }
}
