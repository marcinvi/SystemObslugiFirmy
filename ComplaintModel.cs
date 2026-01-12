using System;

namespace Reklamacje_Dane
{
    // Ta klasa reprezentuje jeden wiersz w tabeli. Jest lekka i szybka.
    public class ComplaintModel
    {
        public int Id { get; set; }
        public string NrZgloszenia { get; set; }
        public DateTime? DataZgloszenia { get; set; }
        public string NrFaktury { get; set; }
        public string NrSeryjny { get; set; }
        public string StatusOgolny { get; set; }
        public string StatusKlient { get; set; }
        public string StatusProducent { get; set; }
        public string Skad { get; set; }

        // Dane z innych tabel (złączone)
        public string ImieNazwisko { get; set; }
        public string NazwaFirmy { get; set; }
        public string NazwaSystemowa { get; set; }
        public string NazwaKrotka { get; set; }
        public string NazwaProducenta { get; set; }
    }
}