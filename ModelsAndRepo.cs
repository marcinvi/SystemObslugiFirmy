using System;
using System.Collections.Generic;

namespace Reklamacje_Dane
{
    // 1. Kryteria filtrowania (jeśli używasz wyszukiwarki)
    public class FilterCriteria
    {
        public int? ZgloszenieId { get; set; }
        public int? KlientId { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public bool TylkoNieprzypisane { get; set; }
        public string AllegroLogin { get; set; }
    }

    // 2. Model Wątku (Lista po lewej stronie)
    public class ConversationThread
    {
        public int EntityId { get; set; }      // ID Klienta
        public string Title { get; set; }      // Imię Nazwisko
        public string LastMessage { get; set; } // Ostatnia treść
        public DateTime LastDate { get; set; } // Data
        public int Count { get; set; }         // Liczba wiadomości
        public string ClaimNumber { get; set; }
    }

    // 3. Surowa wiadomość z Bazy
    public class RawMessage
    {
        public string Id { get; set; }
        public DateTime Data { get; set; }
        public string Typ { get; set; }      // SMS, Email, Allegro
        public string Kierunek { get; set; } // IN, OUT
        public string Tresc { get; set; }
        public string Autor { get; set; }    // Kto wysłał
        public string Zrodlo { get; set; }   // Adresat/Nadawca (używane pomocniczo)
    }

    // 4. Wiadomość wyświetlana (Prawa strona - Czat)
    public class DisplayMessage
    {
        public DateTime Data { get; set; }
        public string Tresc { get; set; }
        public string Autor { get; set; }
        public string Kierunek { get; set; } // IN / OUT

        // Ikony
        public bool HasSms { get; set; }
        public bool HasEmail { get; set; }
        public bool HasAllegro { get; set; }

        public List<string> SourceIds { get; set; } = new List<string>();
        public bool IsIncoming => Kierunek == "IN";
    }
}