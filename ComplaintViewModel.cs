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

        // To pole jest ukryte (nie ma DisplayName lub ignorowane w logice)
        [Browsable(false)]
        public string SearchVector { get; private set; }

        public void BuildSearchVector()
        {
            // Łączymy wszystko w jeden ciąg dla szybkości
            SearchVector = $"{NrZgloszenia} {Klient} {Produkt} {SN} {FV} {Skad} {Producent} {Status} {NazwaKrotka} {KodProducenta}".ToLower();
        }
    }
}