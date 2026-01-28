using System.Collections.Concurrent;

namespace ReklamacjeAPI.Services;

public class ReturnSyncProgressService
{
    private readonly ConcurrentDictionary<string, ReturnSyncProgress> _progress = new();

    public ReturnSyncProgress StartJob(string userDisplayName)
    {
        var job = new ReturnSyncProgress
        {
            JobId = Guid.NewGuid().ToString("N"),
            Status = ReturnSyncStatus.Running,
            StartedAt = DateTime.UtcNow,
            UserDisplayName = userDisplayName
        };

        _progress[job.JobId] = job;
        return job;
    }

    public ReturnSyncProgress? GetJob(string jobId)
    {
        return _progress.TryGetValue(jobId, out var job) ? job : null;
    }

    public void UpdateAccount(ReturnSyncProgress job, int currentIndex, int totalAccounts, string accountName, int accountId)
    {
        job.CurrentAccountIndex = currentIndex;
        job.TotalAccounts = totalAccounts;
        job.CurrentAccountName = accountName;
        job.CurrentAccountId = accountId;
        job.CurrentReturnIndex = 0;
        job.TotalReturnsInAccount = 0;
        job.CurrentReturnReference = null;
    }

    public void UpdateReturn(ReturnSyncProgress job, int currentIndex, int totalReturns, string? returnReference)
    {
        job.CurrentReturnIndex = currentIndex;
        job.TotalReturnsInAccount = totalReturns;
        job.CurrentReturnReference = returnReference;
    }

    public void AddError(ReturnSyncProgress job, string message)
    {
        job.Errors.Add(message);
    }

    public void Complete(ReturnSyncProgress job, ReturnSyncSummary summary)
    {
        job.Status = ReturnSyncStatus.Completed;
        job.FinishedAt = DateTime.UtcNow;
        job.Summary = summary;
    }

    public void Fail(ReturnSyncProgress job, string message)
    {
        job.Status = ReturnSyncStatus.Failed;
        job.FinishedAt = DateTime.UtcNow;
        job.Errors.Add(message);
    }
}

public class ReturnSyncProgress
{
    public string JobId { get; set; } = string.Empty;
    public ReturnSyncStatus Status { get; set; } = ReturnSyncStatus.Pending;
    public string UserDisplayName { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int TotalAccounts { get; set; }
    public int CurrentAccountIndex { get; set; }
    public string? CurrentAccountName { get; set; }
    public int? CurrentAccountId { get; set; }
    public int TotalReturnsInAccount { get; set; }
    public int CurrentReturnIndex { get; set; }
    public string? CurrentReturnReference { get; set; }
    public List<string> Errors { get; } = new();
    public ReturnSyncSummary? Summary { get; set; }
}

public class ReturnSyncSummary
{
    public int AccountsProcessed { get; set; }
    public int ReturnsFetched { get; set; }
    public int ReturnsProcessed { get; set; }
}

public enum ReturnSyncStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
