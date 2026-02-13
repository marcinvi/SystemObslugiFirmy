namespace ReklamacjeAPI.DTOs;

public class AllegroSyncStatusDto
{
    public bool IsRunning { get; set; }
    public DateTime? LastStartedAt { get; set; }
    public DateTime? LastCompletedAt { get; set; }
    public bool LastRunSuccess { get; set; }
    public string? LastError { get; set; }
    public int NewDisputesFoundLastRun { get; set; }
    public int UpdatedDisputesLastRun { get; set; }
    public int ChatsSyncedLastRun { get; set; }
    public int UnregisteredDisputesCount { get; set; }
    public int DisputesWithNewMessages { get; set; }
}

public class AllegroSyncRunResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public AllegroSyncStatusDto Status { get; set; } = new();
}
