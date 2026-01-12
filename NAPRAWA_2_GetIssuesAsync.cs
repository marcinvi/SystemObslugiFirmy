// ========================================
// NAPRAWA #2: GetIssuesAsync - Prawidłowe mapowanie i pobieranie szczegółów
// Problem: Type = Status (błędne), brak szczegółów Issue
// Priorytet: 🔴 WYSOKI
// ========================================

// LOKALIZACJA: AllegroApiClient.cs - dodaj nową metodę i popraw istniejące

// ========================================
// CZĘŚĆ 1: Dodaj nową metodę do pobierania szczegółów Issue
// ========================================

/// <summary>
/// Pobiera pełne szczegóły Issue (reklamacji/dyskusji)
/// </summary>
/// <param name="issueId">ID Issue</param>
/// <returns>Pełny obiekt Issue ze wszystkimi danymi</returns>
public async Task<Issue> GetIssueDetailsAsync(string issueId)
{
    if (string.IsNullOrEmpty(issueId))
        return null;

    try
    {
        System.Diagnostics.Debug.WriteLine($"[API] GET /sale/issues/{issueId} - pobieranie szczegółów...");
        
        var issue = await GetAsync<Issue>($"/sale/issues/{issueId}", ApiBetaV1);
        
        if (issue != null)
        {
            System.Diagnostics.Debug.WriteLine($"[SUCCESS] Pobrano szczegóły Issue {issueId}: Type={issue.Type}, Status={issue.CurrentState?.Status}");
        }
        
        return issue;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR] Błąd pobierania szczegółów Issue {issueId}: {ex.Message}");
        return null;
    }
}

// ========================================
// CZĘŚĆ 2: Popraw mapowanie w GetPagedIssuesAsync
// ========================================

// ZAMIEŃ TO:
/*
private async Task<List<Issue>> GetPagedIssuesAsync(int limit, int offset, List<string> queryParams)
{
    var finalParams = new List<string>(queryParams)
    {
        $"limit={limit}",
        $"offset={offset}"
    };

    string endpoint = $"/sale/issues?{string.Join("&", finalParams)}";
    var response = await GetAsync<IssuesListResponse>(endpoint, ApiBetaV1);
    
    if (response?.Issues == null)
    {
        return new List<Issue>();
    }

    // ❌ BŁĘDNE MAPOWANIE - Type = Status
    return response.Issues.Select(dto => new Issue
    {
        Id = dto.Id,
        Subject = dto.Subject?.Name,
        Type = dto.Status,            // ❌ BŁĄD!
        OpenedDate = dto.CreatedAt,
        Buyer = new IssueUser { Login = dto.Buyer?.Login }
    }).ToList();
}
*/

// NA TO:
private async Task<List<Issue>> GetPagedIssuesAsync(int limit, int offset, List<string> queryParams)
{
    var finalParams = new List<string>(queryParams)
    {
        $"limit={limit}",
        $"offset={offset}"
    };

    string endpoint = $"/sale/issues?{string.Join("&", finalParams)}";
    var response = await GetAsync<IssuesListResponse>(endpoint, ApiBetaV1);
    
    if (response?.Issues == null)
    {
        return new List<Issue>();
    }

    // ✅ NAPRAWIONE: Pobierz tylko ID-ki, szczegóły pobierzemy osobno
    return response.Issues.Select(dto => new Issue
    {
        Id = dto.Id,
        Subject = dto.Subject?.Name,
        // ✅ Status zostawiamy jako string, Type pobierzemy w szczegółach
        CurrentState = new CurrentState { Status = dto.Status },
        OpenedDate = dto.CreatedAt,
        Buyer = new IssueUser { Login = dto.Buyer?.Login }
    }).ToList();
}

// ========================================
// CZĘŚĆ 3: Zmień logikę synchronizacji w AllegroSyncServiceExtended.cs
// ========================================

// ZNAJDŹ metodę SynchronizeIssuesForAccountAsync i ZMIEŃ:

/*
foreach (var issue in allIssues)
{
    try
    {
        // Pobierz szczegóły zamówienia
        OrderDetails orderDetails = null;
        if (!string.IsNullOrEmpty(issue.CheckoutForm?.Id))
        {
            orderDetails = await apiClient.GetOrderDetailsByCheckoutFormIdAsync(issue.CheckoutForm.Id);
        }
        // ...
    }
}
*/

// NA TO:

foreach (var issueShort in allIssues)
{
    try
    {
        // ✅ NAPRAWIONE: Najpierw pobierz pełne szczegóły Issue
        var issue = await apiClient.GetIssueDetailsAsync(issueShort.Id);
        
        if (issue == null)
        {
            result.ErrorMessages.Add($"Issue {issueShort.Id}: Nie można pobrać szczegółów");
            System.Diagnostics.Debug.WriteLine($"[ERROR] Nie można pobrać szczegółów Issue {issueShort.Id}");
            continue;
        }
        
        // Teraz mamy pełne dane:
        // - issue.Type (CLAIM/DISCUSSION)
        // - issue.Description
        // - issue.Expectations
        // - issue.Reason
        // - issue.Product
        // - issue.Offer
        // - issue.DecisionDueDate
        
        // Pobierz szczegóły zamówienia
        OrderDetails orderDetails = null;
        if (!string.IsNullOrEmpty(issue.CheckoutForm?.Id))
        {
            orderDetails = await apiClient.GetOrderDetailsByCheckoutFormIdAsync(issue.CheckoutForm.Id);
        }

        // ⭐ NOWE: Pobierz BuyerEmail z osobnego endpointu
        string buyerEmail = null;
        if (!string.IsNullOrEmpty(issue.CheckoutForm?.Id))
        {
            buyerEmail = await GetBuyerEmailAsync(apiClient, issue.CheckoutForm.Id);
        }

        // Upsert issue do bazy (teraz z pełnymi danymi!)
        bool isNew = await UpsertIssueAsync(issue, orderDetails, buyerEmail, accountId, con);

        if (isNew)
        {
            result.NewIssues++;
        }

        // Synchronizacja czatu
        bool hasNewMessages = await SynchronizeChatForIssueAsync(apiClient, issue, con);
        if (hasNewMessages)
        {
            result.IssuesWithNewMessages++;
        }
    }
    catch (Exception exIssue)
    {
        result.ErrorMessages.Add($"Issue {issueShort.Id}: {exIssue.Message}");
        System.Diagnostics.Debug.WriteLine($"Błąd przetwarzania issue {issueShort.Id}: {exIssue.Message}");
    }
}

// ========================================
// KONIEC NAPRAWY #2
// ========================================