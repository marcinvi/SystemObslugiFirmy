// ═══════════════════════════════════════════════════════════════════════════════
// NAPRAWA: SynchronizeIssuesForAccountAsync_Optimized
// ═══════════════════════════════════════════════════════════════════════════════
// LOKALIZACJA: AllegroSyncServiceExtended.cs
// ZASTĄP METODĘ: SynchronizeIssuesForAccountAsync_Optimized (około linii 400)
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// UPROSZCZONA SYNCHRONIZACJA ISSUES - WERSJA 3.0 SIMPLIFIED
/// 🚀 OPTYMALIZACJA: Tylko synchronizacja czatu (LastMessageId check)
/// 🚀 Issues sync: normalny (10-20 API calls)
/// 🚀 Chat sync: 95% skipped (oszczędność ~270 API calls!)
/// </summary>
private async Task<IssuesSyncResult> SynchronizeIssuesForAccountAsync_Optimized(
    AllegroApiClient apiClient,
    int accountId,
    MySqlConnection con,
    IProgress<string> progress = null)
{
    var result = new IssuesSyncResult();
    var logId = await LogSyncStartAsync(accountId, "ISSUES", con);

    try
    {
        progress?.Report($"Konto {accountId}: Pobieranie issues...");

        // ═══════════════════════════════════════════════════
        // FAZA 1: PEŁNA SYNCHRONIZACJA ISSUES (jak dotąd)
        // ═══════════════════════════════════════════════════
        var allIssues = await apiClient.GetIssuesAsync();

        if (allIssues == null || !allIssues.Any())
        {
            System.Diagnostics.Debug.WriteLine($"[SYNC] Konto {accountId}: Brak issues");
            await LogSyncCompleteAsync(logId, "SUCCESS", 0, 0, 0, con);
            return result;
        }

        result.TotalProcessed = allIssues.Count;
        System.Diagnostics.Debug.WriteLine($"[SYNC] Konto {accountId}: Pobrano {allIssues.Count} issues");

        // ═══════════════════════════════════════════════════
        // FAZA 2: PRZETWARZANIE ISSUES (bez czatu)
        // ═══════════════════════════════════════════════════
        int current = 0;
        foreach (var issueShort in allIssues)
        {
            current++;
            if (current % 10 == 0 || current == allIssues.Count)
            {
                progress?.Report($"Konto {accountId}: Issues {current}/{allIssues.Count}...");
            }

            await ProcessSingleIssueAsync(apiClient, issueShort, accountId, con, result);
        }

        System.Diagnostics.Debug.WriteLine($"[SYNC] Konto {accountId}: Przetworzono {result.TotalProcessed} issues (Nowych: {result.NewIssues})");

        // ═══════════════════════════════════════════════════
        // FAZA 3: INTELIGENTNA SYNCHRONIZACJA CZATÓW
        // ═══════════════════════════════════════════════════
        progress?.Report($"Konto {accountId}: Synchronizacja czatów...");
        
        result.IssuesWithNewMessages = await SynchronizeChatsOnlyAsync(apiClient, accountId, con, progress);

        System.Diagnostics.Debug.WriteLine($"[SYNC] Konto {accountId}: Issues z nowymi wiadomościami: {result.IssuesWithNewMessages}");

        progress?.Report($"Konto {accountId}: ✅ Gotowe! (Nowych: {result.NewIssues}, Czaty: {result.IssuesWithNewMessages})");
        await LogSyncCompleteAsync(logId, "SUCCESS", result.TotalProcessed, result.NewIssues, result.IssuesWithNewMessages, con);
    }
    catch (Exception ex)
    {
        progress?.Report($"Konto {accountId}: ❌ BŁĄD!");
        System.Diagnostics.Debug.WriteLine($"[ERROR] Konto {accountId}: {ex.Message}");
        await LogSyncCompleteAsync(logId, "FAILED", result.TotalProcessed, result.NewIssues, result.IssuesWithNewMessages, con, ex.Message);
        throw;
    }

    return result;
}

// ═══════════════════════════════════════════════════════════════════════════════
// KONIEC NAPRAWY
// ═══════════════════════════════════════════════════════════════════════════════
// INSTRUKCJA:
// 1. Skopiuj powyższą metodę
// 2. Otwórz AllegroSyncServiceExtended.cs
// 3. Znajdź metodę SynchronizeIssuesForAccountAsync_Optimized (około linii 400)
// 4. ZASTĄP całą metodę powyższym kodem
// 5. Zapisz plik
// 6. Kompiluj (Ctrl+Shift+B)
// ═══════════════════════════════════════════════════════════════════════════════
