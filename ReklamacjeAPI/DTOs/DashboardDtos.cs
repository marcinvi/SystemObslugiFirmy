using System;

namespace ReklamacjeAPI.DTOs
{
    public class DashboardComplaintDto
    {
        public int Id { get; set; }
        public string NrZgloszenia { get; set; }
        public string Klient { get; set; } // Nazwa firmy lub Imię Nazwisko
        public string Produkt { get; set; }
        public int DniPoZgloszeniu { get; set; }
        public string Status { get; set; }
    }

    public class DashboardReminderDto
    {
        public int Id { get; set; }
        public string Tresc { get; set; }
        public string DotyczyZgloszenia { get; set; }
        public string Kategoria { get; set; } // Np. "Kurier", "Decyzja" - logika z WinForms
        public string Kolor { get; set; } // Hex koloru dla Androida (np. "#FF0000")
    }
}