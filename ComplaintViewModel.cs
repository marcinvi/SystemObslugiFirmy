using System;
using System.ComponentModel; // Ważne dla ładnych nazw kolumn
using System.Linq;

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

        [DisplayName("Status DPD")]
        public string StatusDpd { get; set; }

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

        [DisplayName("Klient - Imię Nazwisko")]
        public string KlientImieNazwisko { get; set; }

        [DisplayName("Klient - Nazwa Firmy")]
        public string KlientNazwaFirmy { get; set; }

        [DisplayName("Klient - Email")]
        public string KlientEmail { get; set; }

        [DisplayName("Klient - Telefon")]
        public string KlientTelefon { get; set; }

        [DisplayName("Klient - Ulica")]
        public string KlientUlica { get; set; }

        [DisplayName("Klient - Kod Pocztowy")]
        public string KlientKodPocztowy { get; set; }

        [DisplayName("Klient - Miejscowość")]
        public string KlientMiejscowosc { get; set; }

        [DisplayName("Wymagania Produktu")]
        public string ProduktWymagania { get; set; }

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

        // To pole jest ukryte (nie ma DisplayName lub ignorowane w logice)
        [Browsable(false)]
        public string SearchVector { get; private set; }

        private static readonly System.Reflection.PropertyInfo[] SearchableProperties =
            typeof(ComplaintViewModel)
                .GetProperties()
                .Where(prop => prop.CanRead && prop.Name != nameof(SearchVector))
                .ToArray();

        public void BuildSearchVector()
        {
            var parts = new System.Collections.Generic.List<string>();

            foreach (var prop in SearchableProperties)
            {
                var value = prop.GetValue(this);
                if (value == null) continue;

                if (value is DateTime dt)
                {
                    parts.Add(dt.ToString("yyyy-MM-dd HH:mm"));
                }
                else
                {
                    parts.Add(value.ToString());
                }
            }

            SearchVector = string.Join(" ", parts).ToLower();
        }
    }
}
