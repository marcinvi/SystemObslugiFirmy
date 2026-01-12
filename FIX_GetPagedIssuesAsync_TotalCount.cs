// ============================================================
// NAPRAWA: GetPagedIssuesAsync - dodanie zwracania TotalCount
// ============================================================
// Potrzebne do optymalizacji synchronizacji
// ============================================================

// DODAJ DO AllegroApiClient.cs:

/// <summary>
/// Pobiera issues z API Allegro z paginacją (zwraca pełny response z TotalCount!)
/// </summary>
public async Task<IssuesListResponse> GetPagedIssuesAsync(int limit = 100, int offset = 0)
{
    string endpoint = $"/sale/issues?limit={limit}&offset={offset}";
    
    System.Diagnostics.Debug.WriteLine($"[API] GET {ApiUrl}{endpoint}");
    
    try
    {
        var response = await GetAsync<IssuesListResponse>(endpoint, ApiBetaV1);
        
        if (response == null)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] Response is NULL!");
            return new IssuesListResponse { Issues = new List<IssueDto>(), TotalCount = 0 };
        }
        
        if (response.Issues == null)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] response.Issues is NULL!");
            response.Issues = new List<IssueDto>();
        }
        
        System.Diagnostics.Debug.WriteLine($"[SUCCESS] Pobrano {response.Issues.Count} issues (Total: {response.TotalCount})");
        
        return response;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR] GetPagedIssuesAsync exception: {ex.Message}");
        throw;
    }
}

// ============================================================
// ZAMIEŃ STARĄ PRYWATNĄ METODĘ:
// ============================================================

private async Task<List<Issue>> GetPagedIssuesAsync_OLD(int limit, int offset, List<string> queryParams)
{
    // ... stara wersja ...
}

// NA NOWĄ, KTÓRA UŻYWA PUBLICZNEJ METODY:

private async Task<List<Issue>> GetPagedIssuesAsync_Internal(int limit, int offset, List<string> queryParams)
{
    var finalParams = new List<string>(queryParams)
    {
        $"limit={limit}",
        $"offset={offset}"
    };

    string endpoint = $"/sale/issues?{string.Join("&", finalParams)}";
    
    try
    {
        var response = await GetAsync<IssuesListResponse>(endpoint, ApiBetaV1);
        
        if (response?.Issues == null)
            return new List<Issue>();

        // Konwersja z IssueDto na Issue
        return response.Issues.Select(dto => new Issue
        {
            Id = dto.Id,
            Subject = dto.Subject,
            Type = dto.Status,
            OpenedDate = dto.CreatedAt,
            Buyer = new IssueUser { Login = dto.Buyer?.Login }
        }).ToList();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR] GetPagedIssuesAsync exception: {ex.Message}");
        throw;
    }
}

// ============================================================
// ZAKTUALIZUJ GetIssuesAsync ABY UŻYWAŁA NOWEJ NAZWY:
// ============================================================

public async Task<List<Issue>> GetIssuesAsync(string status = null, DateTime? fromDate = null)
{
    var allIssues = new List<Issue>();
    var queryParams = new List<string>();

    if (!string.IsNullOrEmpty(status))
    {
        queryParams.Add($"currentState.status={status}");
    }
    if (fromDate.HasValue)
    {
        queryParams.Add($"openedAt.gte={fromDate.Value.ToUniversalTime():o}");
    }

    int limit = 100;
    int offset = 0;
    while (true)
    {
        var pagedResponse = await GetPagedIssuesAsync_Internal(limit, offset, queryParams); // ⭐ ZMIANA TUTAJ
        if (pagedResponse.Any())
        {
            allIssues.AddRange(pagedResponse);
            if (pagedResponse.Count < limit) break;
            offset += limit;
        }
        else
        {
            break;
        }
    }
    return allIssues;
}
