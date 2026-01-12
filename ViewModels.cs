using System;
using System.Collections.Generic;

namespace Reklamacje_Dane
{

    public enum WizardSource
    {
        Manual,
        Allegro,
        GoogleSheet,
        Zwroty
    }

    public class ComplaintInitialData
    {
        public string Id { get; set; }
        public string ImieNazwisko { get; set; }
        public string NazwaFirmy { get; set; }
        public string NIP { get; set; }
        public string Ulica { get; set; }
        public string KodPocztowy { get; set; }
        public string Miejscowosc { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string NazwaProduktu { get; set; }
        public string OpisUsterki { get; set; }
        public string NumerFaktury { get; set; }
        public string NumerSeryjny { get; set; }
        public DateTime? DataZakupu { get; set; }
        public bool IsCompany => !string.IsNullOrWhiteSpace(NazwaFirmy) || !string.IsNullOrWhiteSpace(NIP);
        public string SourceName { get; set; }
        public object OriginalObject { get; set; }
        public int AllegroAccountId { get; set; }
        public string allegroDisputeId { get; set; }
        public string allegroOrderId { get; set; }
        public string allegroBuyerLogin { get; set; }
    }

    public class ClientViewModel
    {
        public int Id { get; set; }
        public string ImieNazwisko { get; set; }
        public string NazwaFirmy { get; set; }
        public string NIP { get; set; }
        public string Ulica { get; set; }
        public string KodPocztowy { get; set; }
        public string Miejscowosc { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public double RelevanceScore { get; set; }

        public string DaneKlienta => string.IsNullOrWhiteSpace(NazwaFirmy) ? ImieNazwisko : $"{NazwaFirmy} ({ImieNazwisko})";
        public string Adres => $"{Ulica}, {KodPocztowy} {Miejscowosc}".Trim(new char[] { ',', ' ' });
        public string Kontakt => $"Email: {Email} | Tel: {Telefon}";
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Producent { get; set; }
        public string KodEnova { get; set; }
        public string KodProducenta { get; set; }
        public string Kategoria { get; set; }
    }

    // NOWA KLASA do przekazywania informacji o istniejącym zgłoszeniu
    public class ExistingComplaintInfo
    {
        public string NrZgloszenia { get; set; }
        public string NrFaktury { get; set; }
        public string NrSeryjny { get; set; }
        public DateTime? DataZakupu { get; set; }
    }
}