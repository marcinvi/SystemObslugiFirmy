// Plik: DpdTrackingModels.cs
using System;

namespace Reklamacje_Dane
{
   
    public class Przesylka
    {
        public int Id { get; set; }
        public string NumerListu { get; set; }
        public string NrZgloszenia { get; set; }
        public string OstatniStatus { get; set; }
        public bool CzyDoreczona { get; set; }
    }

    public class PrzesylkaViewModel
    {
        public int Id { get; set; }
        public string NumerListu { get; set; }
        public string NrZgloszenia { get; set; }
        public string NazwaNadawcy { get; set; }
        public string NazwaOdbiorcy { get; set; }
        public string OstatniStatus { get; set; }
    }

    public class HistoriaPrzesylki
    {
        public DateTime DataStatusu { get; set; }
        public string OpisStatusu { get; set; }
        public string Oddzial { get; set; }
    }

    internal class RawTrackingData
    {
        public string Data { get; set; }
        public string Godzina { get; set; }
        public string Opis { get; set; }
        public string Oddzial { get; set; }
        public string BusinessCode { get; set; } // <--- TO MUSI BY
    }
}