# 🔍 AUDYT SYNCHRONIZACJI ALLEGRO - Analiza kompletności i poprawności

**Data:** 2026-01-07  
**Audytor:** System AI  
**Status:** 🔴 WYKRYTO POWAŻNE PROBLEMY  

---

## 📊 PODSUMOWANIE WYKONAWCZE

| Kategoria | Stan | Pilność |
|-----------|------|---------|
| **Zwroty (Returns)** | 🟡 Częściowo sprawne | Średnia |
| **Dyskusje (Issues)** | 🔴 Błędy krytyczne | Wysoka |
| **Chat Messages** | 🟡 Niepełne | Średnia |
| **Dane klientów** | 🔴 Brakujące | Wysoka |
| **Adresy** | 🟢 Sprawne | - |

**Ocena ogólna:** 🔴 **WYMAGA NATYCHMIASTOWYCH POPRAWEK**

---

## 🔴 PROBLEM #1: GetBuyerEmailAsync - BŁĄD AUTORYZACJI

### Lokalizacja
`AllegroApiClient.cs` - linia ~640

### Kod:
```csharp
public async Task<string> GetBuyerEmailAsync(string checkoutFormId)
{
    try
    {
        var url = $"https://api.allegro.pl/sale/checkout-forms/{checkoutFormId}";
        var response = await _httpClient.GetAsync(url);  // ❌ BŁĄD!
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        var checkoutForm = JsonConvert.DeserializeObject<dynamic>(json);
        
        return checkoutForm?.buyer?.email?.ToString();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Błąd pobierania email kupującego: {ex.Message}");
        return null;
    }
}
```

### Problem:
❌ **Zapytanie HTTP NIE ZAWIERA tokena autoryzacji!**  
❌ API Allegro zwróci **401 Unauthorized**  
❌ Metoda ZAWSZE zwraca `null`  
❌ Email kupującego **NIGDY** nie jest pobierany!

### Skutki:
- Wszystkie dyskusje/reklamacje mają `BuyerEmail = NULL` w bazie
- Nie można kontaktować się z klientami
- Brak danych do eksportu/raportów

### Poprawka:
```csharp
public async Task<string> GetBuyerEmailAsync(string checkoutFormId)
{
    try
    {
        if (Token == null) 
            throw new InvalidOperationException("Klient API nie jest autoryzowany.");
        
        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"{ApiUrl}/sale/checkout-forms/{checkoutFormId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
        request.Headers.Accept.Add(ApiPublicV1);
        
        var response = await _httpClient.SendAsync(request);
        await HandleFailedResponse(response, $"pobierania email dla {checkoutFormId}");
        
        var json = await response.Content.ReadAsStringAsync();
        var checkoutForm = JsonConvert.DeserializeObject<dynamic>(json);
        
        return checkoutForm?.buyer?.email?.ToString();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Błąd pobierania email kupującego: {ex.Message}");
        return null;
    }
}
```

**Priorytet:** 🔴 **KRYTYCZNY - napraw natychmiast!**

---

## 🔴 PROBLEM #2: GetIssuesAsync - Nieprawidłowe mapowanie

### Lokalizacja
`AllegroApiClient.cs` - linia ~217

### Kod:
```csharp
private async Task<List<Issue>> GetPagedIssuesAsync(int limit, int offset, List<string> queryParams)
{
    // ...
    
    // Konwersja z IssueDto na Issue
    return response.Issues.Select(dto => new Issue
    {
        Id = dto.Id,
        Subject = dto.Subject?.Name,  // ✅ OK
        Type = dto.Status,            // ❌ BŁĄD! Status != Type
        OpenedDate = dto.CreatedAt,   // ✅ OK
        Buyer = new IssueUser { Login = dto.Buyer?.Login }  // ✅ OK
    }).ToList();
}
```

### Problem:
❌ **`Type = dto.Status` jest nieprawidłowe!**
- `Type` to typ dyskusji: `"CLAIM"` (reklamacja) lub `"DISCUSSION"` (dyskusja)
- `Status` to status: `"OPEN"`, `"CLOSED"`, `"WAITING_FOR_SELLER"`, etc.

### Skutki:
- Reklamacje są błędnie klasyfikowane w bazie
- Filtry nie działają poprawnie
- Raporty pokazują błędne dane

### Brakujące pola w mapowaniu:
```csharp
// Obecnie NIE POBIERANE:
- Description (opis dyskusji)
- DecisionDueDate (termin decyzji)
- CheckoutForm (pełne info o zamówieniu)
- Product (info o produkcie)
- Offer (info o ofercie)
- Expectations (czego oczekuje kupujący)
- Reason (powód reklamacji)
- CurrentState (aktualny status)
- ReferenceNumber (numer referencyjny)
```

### Poprawka:
Potrzebujemy **dedykowanego endpointu** do pobierania szczegółów Issue:

```csharp
public async Task<Issue> GetIssueDetailsAsync(string issueId)
{
    return await GetAsync<Issue>($"/sale/issues/{issueId}", ApiBetaV1);
}
```

I używać go w synchronizacji:
```csharp
foreach (var issueShort in allIssues)
{
    // Pobierz pełne szczegóły
    var issue = await apiClient.GetIssueDetailsAsync(issueShort.Id);
    
    // Teraz mamy wszystkie dane!
    await UpsertIssueAsync(issue, orderDetails, buyerEmail, accountId, con);
}
```

**Priorytet:** 🔴 **WYSOKI**

---

## 🟡 PROBLEM #3: Chat Messages - Brak paginacji

### Lokalizacja
`AllegroApiClient.cs` - linia ~235

### Kod:
```csharp
public async Task<List<ChatMessage>> GetChatAsync(string issueId)
{
    var response = await GetAsync<ChatMessageResponse>(
        $"/sale/issues/{issueId}/chat", 
        ApiBetaV1
    );
    
    // ❌ Pobiera TYLKO pierwszą stronę!
    return response?.Chat?.Select(m => new ChatMessage { ... }).ToList();
}
```

### Problem:
❌ API Allegro może zwracać **wiadomości w stronach** (limit 100)  
❌ Jeśli chat ma >100 wiadomości, **starsze nie są pobierane**  
❌ Brak obsługi paginacji!

### Skutki:
- Stare wiadomości nie są synchronizowane
- Historia rozmów jest niekompletna
- Klient może pisać coś ważnego a my tego nie widzimy

### Poprawka:
```csharp
public async Task<List<ChatMessage>> GetChatAsync(string issueId)
{
    var allMessages = new List<ChatMessage>();
    int limit = 100;
    int offset = 0;
    
    while (true)
    {
        var response = await GetAsync<ChatMessageResponse>(
            $"/sale/issues/{issueId}/chat?limit={limit}&offset={offset}", 
            ApiBetaV1
        );
        
        if (response?.Chat == null || !response.Chat.Any())
            break;
        
        allMessages.AddRange(response.Chat.Select(m => new ChatMessage 
        {
            Id = m.Id,
            Text = m.Text,
            CreatedAt = m.CreatedAt,
            Author = new IssueUser
            {
                Login = m.Author?.Login,
                Role = m.Author?.Role
            }
        }));
        
        if (response.Chat.Count < limit)
            break;
        
        offset += limit;
    }
    
    return allMessages;
}
```

**Priorytet:** 🟡 **ŚREDNI**

---

## 🔴 PROBLEM #4: Zwroty - Brak pobrania danych klienta

### Lokalizacja
`AllegroSyncServiceExtended.cs` - metoda `UpsertReturnAsync`

### Problem:
```csharp
cmd.Parameters.AddWithValue("@BuyerEmail", (object)DBNull.Value); // ❌ ZAWSZE NULL!
```

### Analiza:
API zwrotów (`/order/customer-returns`) **NIE ZWRACA emaila kupującego**  
Musimy go pobrać z osobnego endpointu: `/order/checkout-forms/{orderId}`

### Obecna logika:
```csharp
// Dane z zamówienia (jeśli dostępne)
if (orderDetails != null)
{
    cmd.Parameters.AddWithValue("@PaymentType", orderDetails.Payment?.Type ?? ...);
    // ... inne dane ...
}
else
{
    // ❌ Brak emaila również w "else"!
    cmd.Parameters.AddWithValue("@BuyerEmail", DBNull.Value);
}
```

### Skutki:
- **WSZYSTKIE zwroty** mają `BuyerEmail = NULL`
- Nie można wysłać powiadomienia o decyzji
- Brak kontaktu z klientem

### Poprawka:
```csharp
// Pobierz email z checkout form
string buyerEmail = null;
if (orderDetails != null)
{
    buyerEmail = orderDetails.Buyer?.Email;
}

// Jeśli dalej brak, spróbuj osobnym endpointem
if (string.IsNullOrEmpty(buyerEmail) && !string.IsNullOrEmpty(returnData.OrderId))
{
    buyerEmail = await apiClient.GetBuyerEmailAsync(returnData.OrderId);
}

cmd.Parameters.AddWithValue("@BuyerEmail", buyerEmail ?? (object)DBNull.Value);
```

**Ale UWAGA:** Najpierw napraw `GetBuyerEmailAsync` (Problem #1)!

**Priorytet:** 🔴 **KRYTYCZNY**

---

## 🟡 PROBLEM #5: Zwroty - Niepełne dane produktu

### Lokalizacja
`AllegroSyncServiceExtended.cs` - metoda `UpsertReturnAsync`

### Problem:
```csharp
cmd.Parameters.AddWithValue("@ProductPrice", (object)DBNull.Value); // ❌ Brak w API zwrotów
```

### Analiza:
API `/order/customer-returns` **NIE ZWRACA ceny produktu**  
Musimy ją pobrać z `OrderDetails.LineItems`

### Poprawka:
```csharp
// Znajdź ceny produktów ze szczegółów zamówienia
decimal? productPrice = null;
if (orderDetails?.LineItems != null && firstItem?.OfferId != null)
{
    var matchingLineItem = orderDetails.LineItems
        .FirstOrDefault(li => li.Offer?.Id == firstItem.OfferId);
    
    if (matchingLineItem != null)
    {
        productPrice = SafeParseDecimal(matchingLineItem.Price?.Amount, returnData.Id);
    }
}

cmd.Parameters.AddWithValue("@ProductPrice", productPrice ?? (object)DBNull.Value);
```

**Priorytet:** 🟡 **ŚREDNI**

---

## 🟡 PROBLEM #6: Issues - Brak ProductEAN i InvoiceNumber

### Lokalizacja
`AllegroSyncServiceExtended.cs` - metoda `UpsertIssueAsync`

### Kod:
```csharp
// ⭐ NOWE: Pobierz EAN i SKU
// UWAGA: API Allegro nie zwraca Offer.Product.Id bezpośrednio
// EAN może być dostępny w innych miejscach w przyszłości
productEAN = null; // TODO: Sprawdzić strukturę API
productSKU = specificLineItem?.Offer?.External?.Id;
```

```csharp
// ⭐ InvoiceNumber - API nie zwraca bezpośrednio numeru faktury
string invoiceNumber = null;
// TODO: Sprawdzić czy można pobrać z OrderDetails w przyszłości
```

### Problem:
Komentarze TODO sugerują, że **nie sprawdzono wszystkich możliwości**

### Analiza:
1. **ProductEAN:** 
   - Może być dostępne w `Product.id` (jeśli to EAN)
   - Może być w offers API: `/sale/offers/{offerId}`
   - Może być w produktach: `/sale/products/{productId}`

2. **InvoiceNumber:**
   - Dostępne w: `/order/checkout-forms/{id}/invoices`
   - Struktura: `Invoice.invoiceNumber`

### Poprawka:
```csharp
// 1. Pobierz EAN z API offers (jeśli potrzebne)
string productEAN = null;
if (!string.IsNullOrEmpty(issue.Offer?.Id))
{
    try
    {
        var offerDetails = await apiClient.GetAsync<OfferDetails>(
            $"/sale/offers/{issue.Offer.Id}", 
            ApiPublicV1
        );
        productEAN = offerDetails?.Product?.Ean;
    }
    catch { /* Ignore */ }
}

// 2. Pobierz numer faktury
string invoiceNumber = null;
if (!string.IsNullOrEmpty(issue.CheckoutForm?.Id))
{
    try
    {
        var invoices = await apiClient.GetInvoicesForOrderAsync(issue.CheckoutForm.Id);
        invoiceNumber = invoices?.FirstOrDefault()?.InvoiceNumber;
    }
    catch { /* Ignore */ }
}
```

**Priorytet:** 🟢 **NISKI** (Nice to have)

---

## 🔴 PROBLEM #7: Brak obsługi statusu REJECTED dla zwrotów

### Lokalizacja
`AllegroSyncServiceExtended.cs` - brak obsługi

### Problem:
Zwrot może mieć status:
- `CREATED` - utworzony
- `ACCEPTED` - zaakceptowany
- `REJECTED` - odrzucony ❌ **NIE OBSŁUGIWANE!**
- `COMPLETED` - zakończony

### Skutki:
- Nie wiadomo czy zwrot został odrzucony
- Klient nie dostaje informacji zwrotnej
- Brak rekordu w historii decyzji

### Poprawka:
Dodać obsługę w synchronizacji:
```csharp
// Po upsert zwrotu, sprawdź status
if (returnData.Status == "REJECTED" && returnData.Rejection != null)
{
    // Zapisz informację o odrzuceniu
    await SaveRejectionDetailsAsync(returnData.Id, returnData.Rejection, con);
    
    // Opcjonalnie: wyślij powiadomienie do klienta
    // await SendRejectionNotificationAsync(returnData);
}
```

**Priorytet:** 🔴 **WYSOKI**

---

## 🟡 PROBLEM #8: Synchronizacja - Brak inkrementalnej aktualizacji

### Lokalizacja
`AllegroSyncServiceExtended.cs` - metoda `GetAllReturnsFromApiAsync`

### Kod:
```csharp
private async Task<List<AllegroCustomerReturn>> GetAllReturnsFromApiAsync(AllegroApiClient apiClient)
{
    var allReturns = new List<AllegroCustomerReturn>();
    int offset = 0;
    int limit = 1000;

    while (true)
    {
        var response = await apiClient.GetCustomerReturnsAsync(limit, offset);
        // ❌ Pobiera WSZYSTKIE zwroty za każdym razem!
        // ...
    }
    return allReturns;
}
```

### Problem:
❌ **Synchronizacja pobiera WSZYSTKIE zwroty** za każdym razem  
❌ Jeśli masz 10,000 zwrotów, synchronizacja trwa **bardzo długo**  
❌ Marnuje API quota i czas

### Rozwiązanie:
Synchronizacja **inkrementalna** - tylko nowe/zmienione:

```csharp
private async Task<List<AllegroCustomerReturn>> GetAllReturnsFromApiAsync(
    AllegroApiClient apiClient,
    DateTime? fromDate = null)
{
    // Jeśli nie podano, weź datę ostatniej synchronizacji
    if (!fromDate.HasValue)
    {
        fromDate = await GetLastReturnsSyncDateAsync();
    }
    
    var filters = new Dictionary<string, string>();
    if (fromDate.HasValue)
    {
        // API Allegro wspiera filtr createdAt.gte
        filters["createdAt.gte"] = fromDate.Value.ToUniversalTime().ToString("o");
    }
    
    var allReturns = new List<AllegroCustomerReturn>();
    int offset = 0;
    int limit = 1000;

    while (true)
    {
        var response = await apiClient.GetCustomerReturnsAsync(limit, offset, filters);
        // Teraz pobiera tylko nowe!
        // ...
    }
    return allReturns;
}
```

**Priorytet:** 🟡 **ŚREDNI** (optymalizacja)

---

## 📊 SCENARIUSZE - CZY SĄ OBSŁUGIWANE?

### ✅ OBSŁUGIWANE:
- [x] Zwrot jednoproduktowy
- [x] Zwrot wieloproduktowy (po naprawie tabeli AllegroReturnItems)
- [x] Dyskusja z wiadomościami
- [x] Reklamacja z oczekiwaniem zwrotu
- [x] Dane adresowe (dostawa, faktura, kupujący)
- [x] Parsowanie kwot różnych formatów (po naprawie SafeParseDecimal)

### ❌ NIE OBSŁUGIWANE / BŁĘDNE:
- [ ] Email kupującego (Problem #1 i #4)
- [ ] Typ dyskusji (CLAIM vs DISCUSSION) (Problem #2)
- [ ] Pełne szczegóły Issue (Problem #2)
- [ ] Wiadomości >100 w chacie (Problem #3)
- [ ] Cena produktu w zwrocie (Problem #5)
- [ ] Odrzucone zwroty (Problem #7)
- [ ] Synchronizacja inkrementalna (Problem #8)

### ⚠️ CZĘŚCIOWO OBSŁUGIWANE:
- [~] ProductEAN i InvoiceNumber (Problem #6) - można dodać
- [~] Załączniki w wiadomościach - są zapisywane ale nie pobierane

---

## 🎯 PLAN NAPRAWY - PRIORYTETY

### 🔴 PRIORYTET 1 - KRYTYCZNE (napraw dziś!)
1. **Problem #1:** Naprawa `GetBuyerEmailAsync` - dodaj autoryzację
2. **Problem #4:** Pobieranie emaila w zwrotach
3. **Problem #2:** Mapowanie Type vs Status w Issues

### 🟡 PRIORYTET 2 - WAŻNE (napraw w tym tygodniu)
4. **Problem #3:** Paginacja chat messages
5. **Problem #7:** Obsługa REJECTED status
6. **Problem #5:** Cena produktu w zwrotach

### 🟢 PRIORYTET 3 - OPTYMALIZACJE (nice to have)
7. **Problem #8:** Synchronizacja inkrementalna
8. **Problem #6:** ProductEAN i InvoiceNumber

---

## 📝 DODATKOWE REKOMENDACJE

### 1. Logging
Dodać szczegółowe logi dla każdego API call:
```csharp
System.Diagnostics.Debug.WriteLine($"[API] GET /sale/issues/{issueId} - START");
// ... call ...
System.Diagnostics.Debug.WriteLine($"[API] GET /sale/issues/{issueId} - OK (200)");
```

### 2. Error handling
Dodać retry logic dla przejściowych błędów (429, 503):
```csharp
private async Task<T> GetWithRetryAsync<T>(string endpoint, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await GetAsync<T>(endpoint);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("429") || ex.Message.Contains("503"))
        {
            if (i == maxRetries - 1) throw;
            await Task.Delay((i + 1) * 1000); // Exponential backoff
        }
    }
}
```

### 3. Testy jednostkowe
Utworzyć testy dla:
- SafeParseDecimal (różne formaty)
- Mapowanie IssueDto -> Issue
- Obsługa NULL values

### 4. Monitorowanie
Dodać metryki:
- Liczba pobranych Issues/Returns
- Liczba błędów API
- Czas synchronizacji
- % Issues z emailem vs bez

---

## 📄 NASTĘPNE KROKI

1. ✅ Przeczytaj cały audyt
2. ⏳ Napraw Problem #1 (GetBuyerEmailAsync)
3. ⏳ Napraw Problem #4 (Email w zwrotach)
4. ⏳ Napraw Problem #2 (Mapowanie Issues)
5. ⏳ Przetestuj na małej próbie danych
6. ⏳ Deploy do produkcji
7. ⏳ Monitoruj przez 24h

---

**Status audytu:** ✅ ZAKOŃCZONY  
**Data:** 2026-01-07 23:50 CET  
**Następna aktualizacja:** Po implementacji poprawek
