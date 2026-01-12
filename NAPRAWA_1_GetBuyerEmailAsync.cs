// ========================================
// NAPRAWA #1: GetBuyerEmailAsync - Dodanie autoryzacji
// Problem: Metoda NIE używa Bearer token, więc ZAWSZE zwraca NULL
// Priorytet: 🔴 KRYTYCZNY
// ========================================

// LOKALIZACJA: AllegroApiClient.cs - zastąp istniejącą metodę

/// <summary>
/// Pobiera email kupującego z osobnego endpointu /sale/checkout-forms/{id}
/// NAPRAWIONE: Dodano autoryzację Bearer token
/// </summary>
/// <param name="checkoutFormId">ID formularza zamówienia (checkoutFormId)</param>
/// <returns>Email kupującego lub null jeśli nie udało się pobrać</returns>
public async Task<string> GetBuyerEmailAsync(string checkoutFormId)
{
    if (string.IsNullOrEmpty(checkoutFormId))
        return null;

    try
    {
        if (Token == null) 
            throw new InvalidOperationException("Klient API nie jest autoryzowany.");
        
        // ✅ NAPRAWIONE: Używamy prawidłowego request z autoryzacją
        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"{ApiUrl}/sale/checkout-forms/{checkoutFormId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
        request.Headers.Accept.Add(ApiPublicV1);
        
        var response = await _httpClient.SendAsync(request);
        await HandleFailedResponse(response, $"pobierania email dla checkout form {checkoutFormId}");
        
        var json = await response.Content.ReadAsStringAsync();
        var checkoutForm = JsonConvert.DeserializeObject<dynamic>(json);
        
        string email = checkoutForm?.buyer?.email?.ToString();
        
        if (!string.IsNullOrEmpty(email))
        {
            System.Diagnostics.Debug.WriteLine($"[SUCCESS] Pobrano email dla {checkoutFormId}: {email}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] Brak emaila w checkout form {checkoutFormId}");
        }
        
        return email;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR] Błąd pobierania email dla {checkoutFormId}: {ex.Message}");
        return null;
    }
}

// ========================================
// KONIEC NAPRAWY #1
// ========================================