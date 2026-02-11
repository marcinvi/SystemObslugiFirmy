namespace ReklamacjeAPI.DTOs;

public class SyncServiceStatusDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsRunning { get; set; }
    public bool LastSuccess { get; set; }
    public DateTime? LastStartedAt { get; set; }
    public DateTime? LastFinishedAt { get; set; }
    public string? LastError { get; set; }
    public int MetricValue { get; set; }
    public string MetricLabel { get; set; } = string.Empty;
}

public class OperationsSyncSnapshotDto
{
    public SyncServiceStatusDto Google { get; set; } = new();
    public SyncServiceStatusDto Dpd { get; set; } = new();
}
