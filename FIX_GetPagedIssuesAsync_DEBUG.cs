// ═══════════════════════════════════════════════════════════════════════════════
// DIAGNOZA: Co zwraca API Allegro?
// ═══════════════════════════════════════════════════════════════════════════════
// LOKALIZACJA: AllegroApiClient.cs
// ZASTĄP METODĘ: GetPagedIssuesAsync (ta z komentarzem "Wersja 3.0")
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Pobiera issues z API Allegro z paginacją - WERSJA DEBUG
/// DODANO: Log surowego JSON żeby zobaczyć co zwraca API
/// </summary>
public async Task<IssuesListResponse> GetPagedIssuesAsync(int limit = 100, int offset = 0)
{
    string endpoint = $"/sale/issues?limit={limit}&offset={offset}";

    System.Diagnostics.Debug.WriteLine($"[API] GET {ApiUrl}{endpoint}");

    try
    {
        // ⭐ ZMIANA: Pobierz SUROWY JSON przed deserializacją
        if (Token == null) throw new InvalidOperationException("Klient API nie jest autoryzowany.");
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiUrl}{endpoint}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(ApiBetaV1);
        
        var httpResponse = await _httpClient.SendAsync(request);
        await HandleFailedResponse(httpResponse, $"wykonywania żądania GET dla {endpoint}");
        
        var json = await httpResponse.Content.ReadAsStringAsync();
        
        // ⭐ DEBUG: Wyświetl SUROWY JSON
        System.Diagnostics.Debug.WriteLine($"[RAW JSON] {json.Substring(0, Math.Min(500, json.Length))}...");
        
        // Deserializuj
        var response = JsonConvert.DeserializeObject<IssuesListResponse>(json);

        if (response == null)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Deserializacja zwróciła NULL!");
            return new IssuesListResponse { Issues = new List<IssueDto>(), TotalCount = 0 };
        }

        if (response.Issues == null)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] response.Issues is NULL!");
            response.Issues = new List<IssueDto>();
        }

        // ⭐ KLUCZOWA DIAGNOZA
        System.Diagnostics.Debug.WriteLine($"[DIAGNOSIS] Issues.Count={response.Issues.Count}, TotalCount={response.TotalCount}");
        System.Diagnostics.Debug.WriteLine($"[DIAGNOSIS] JSON zawiera 'totalCount': {json.Contains("\"totalCount\"")}");
        System.Diagnostics.Debug.WriteLine($"[DIAGNOSIS] JSON zawiera 'count': {json.Contains("\"count\"")}");

        return response;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR] GetPagedIssuesAsync exception: {ex.Message}");
        throw;
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// INSTRUKCJA:
// 1. ZASTĄP metodę GetPagedIssuesAsync w AllegroApiClient.cs powyższym kodem
// 2. Zapisz (Ctrl+S)
// 3. Kompiluj (Ctrl+Shift+B)
// 4. Uruchom aplikację (F5)
// 5. Kliknij "Odśwież" Allegro
// 6. Sprawdź Output Window (View → Output) - znajdź linię [DIAGNOSIS]
// 7. Skopiuj CAŁĄ linię [RAW JSON] i [DIAGNOSIS] i pokaż mi
// ═══════════════════════════════════════════════════════════════════════════════
