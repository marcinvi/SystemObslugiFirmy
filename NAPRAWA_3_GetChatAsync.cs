// ========================================
// NAPRAWA #3: GetChatAsync - Dodanie paginacji
// Problem: Pobiera tylko pierwszą stronę wiadomości (max 100)
// Priorytet: 🟡 ŚREDNI
// ========================================

// LOKALIZACJA: AllegroApiClient.cs - zastąp istniejącą metodę

// ZAMIEŃ TO:
/*
public async Task<List<Reklamacje_Dane.Allegro.Issues.ChatMessage>> GetChatAsync(string issueId)
{
    var response = await GetAsync<Reklamacje_Dane.Allegro.Issues.ChatMessageResponse>(
        $"/sale/issues/{issueId}/chat", 
        ApiBetaV1
    );
    
    if (response?.Chat == null)
    {
        return new List<Reklamacje_Dane.Allegro.Issues.ChatMessage>();
    }

    // ❌ Pobiera TYLKO pierwszą stronę!
    return response.Chat.Select(m => new Reklamacje_Dane.Allegro.Issues.ChatMessage
    {
        Id = m.Id,
        Text = m.Text,
        CreatedAt = m.CreatedAt,
        Author = new Reklamacje_Dane.Allegro.Issues.IssueUser
        {
            Login = m.Author?.Login,
            Role = m.Author?.Role
        }
    }).ToList();
}
*/

// NA TO:

/// <summary>
/// Pobiera wszystkie wiadomości z czatu Issue (z obsługą paginacji)
/// NAPRAWIONE: Pobiera wszystkie strony, nie tylko pierwszą
/// </summary>
/// <param name="issueId">ID Issue</param>
/// <returns>Pełna lista wiadomości</returns>
public async Task<List<Reklamacje_Dane.Allegro.Issues.ChatMessage>> GetChatAsync(string issueId)
{
    var allMessages = new List<Reklamacje_Dane.Allegro.Issues.ChatMessage>();
    int limit = 100;
    int offset = 0;
    int totalFetched = 0;
    
    System.Diagnostics.Debug.WriteLine($"[API] GET /sale/issues/{issueId}/chat - START paginacji");
    
    while (true)
    {
        try
        {
            // ✅ NAPRAWIONE: Dodano paginację (limit i offset)
            var endpoint = $"/sale/issues/{issueId}/chat?limit={limit}&offset={offset}";
            var response = await GetAsync<Reklamacje_Dane.Allegro.Issues.ChatMessageResponse>(
                endpoint, 
                ApiBetaV1
            );
            
            if (response?.Chat == null || !response.Chat.Any())
            {
                System.Diagnostics.Debug.WriteLine($"[API] GET /sale/issues/{issueId}/chat - brak więcej wiadomości (offset={offset})");
                break;
            }
            
            // Konwersja i dodanie do listy
            var messages = response.Chat.Select(m => new Reklamacje_Dane.Allegro.Issues.ChatMessage
            {
                Id = m.Id,
                Text = m.Text,
                CreatedAt = m.CreatedAt,
                Author = new Reklamacje_Dane.Allegro.Issues.IssueUser
                {
                    Login = m.Author?.Login,
                    Role = m.Author?.Role
                },
                Attachments = m.Attachments?.Select(a => new Reklamacje_Dane.Allegro.Issues.ChatAttachment
                {
                    FileName = a.FileName,
                    Url = a.Url
                }).ToList()
            }).ToList();
            
            allMessages.AddRange(messages);
            totalFetched += messages.Count;
            
            System.Diagnostics.Debug.WriteLine($"[API] GET /sale/issues/{issueId}/chat - pobrano {messages.Count} wiadomości (offset={offset}, total={totalFetched})");
            
            // Jeśli pobrano mniej niż limit, to był ostatnia strona
            if (response.Chat.Count < limit)
            {
                System.Diagnostics.Debug.WriteLine($"[API] GET /sale/issues/{issueId}/chat - KONIEC (ostatnia strona)");
                break;
            }
            
            // Przejdź do następnej strony
            offset += limit;
            
            // Zabezpieczenie przed nieskończoną pętlą (max 1000 wiadomości)
            if (totalFetched >= 10000)
            {
                System.Diagnostics.Debug.WriteLine($"[WARNING] Issue {issueId} ma >10000 wiadomości! Przerwano paginację.");
                break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Błąd pobierania strony wiadomości (offset={offset}): {ex.Message}");
            break;
        }
    }
    
    System.Diagnostics.Debug.WriteLine($"[SUCCESS] Pobrano łącznie {allMessages.Count} wiadomości dla Issue {issueId}");
    return allMessages;
}

// ========================================
// DODATKOWA POPRAWA: Obsługa Attachments w modelu Message
// ========================================

// SPRAWDŹ czy w AllegroIssueModels.cs klasa Message ma pole Attachments:

/*
public class Message
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("author")]
    public MessageAuthor Author { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    // ✅ Dodaj to pole jeśli go nie ma:
    [JsonProperty("attachments")]
    public List<MessageAttachment> Attachments { get; set; }
}

// I klasę MessageAttachment:
public class MessageAttachment
{
    [JsonProperty("fileName")]
    public string FileName { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}
*/

// ========================================
// KONIEC NAPRAWY #3
// ========================================