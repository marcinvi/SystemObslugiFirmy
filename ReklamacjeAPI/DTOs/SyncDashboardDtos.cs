namespace ReklamacjeAPI.DTOs;

/// <summary>
/// Jeden endpoint, jeden response — wszystko co ReklamacjeControl potrzebuje
/// aby wyświetlić dashboard bez żadnych bezpośrednich połączeń do bazy/API zewnętrznych.
/// </summary>
public class SyncDashboardDto
{
    // === LICZNIKI DLA PRZYCISKÓW MENU ===
    public int UnregisteredAllegroCount { get; set; }
    public int AllegroNewMessages { get; set; }
    public int UnregisteredGoogleCount { get; set; }
    public int UnregisteredReturnsCount { get; set; }
    public int EmailUnreadCount { get; set; }

    // === STATUSY SYNCHRONIZACJI (tooltip) ===
    public List<SyncServiceInfoDto> Services { get; set; } = new();

    // === DANE DASHBOARDU ===
    public List<DashboardComplaintDto> ProcessingComplaints { get; set; } = new();
    public List<DashboardReminderDto> Reminders { get; set; } = new();
    public List<ChangeLogEntryDto> ChangeLog { get; set; } = new();

    // === TIMESTAMP ===
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}

public class SyncServiceInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Oczekiwanie..."; // OK, Błąd, Oczekiwanie...
    public string Details { get; set; } = string.Empty;
    public bool IsRunning { get; set; }
    public DateTime? LastRunAt { get; set; }
    public bool LastRunSuccess { get; set; }
}

public class ChangeLogEntryDto
{
    public string Kiedy { get; set; } = string.Empty;
    public string Zdarzenie { get; set; } = string.Empty;
    public string Uzytkownik { get; set; } = string.Empty;
    public string NrZgloszenia { get; set; } = string.Empty;
}
