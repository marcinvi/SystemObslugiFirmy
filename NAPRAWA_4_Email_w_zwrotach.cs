// ========================================
// NAPRAWA #4: Email kupującego w zwrotach
// Problem: BuyerEmail zawsze NULL w AllegroCustomerReturns
// Priorytet: 🔴 KRYTYCZNY
// Wymaga: Naprawy #1 (GetBuyerEmailAsync musi być naprawione!)
// ========================================

// LOKALIZACJA: AllegroSyncServiceExtended.cs - metoda UpsertReturnAsync

// ZNAJDŹ TEN FRAGMENT (około linia 330):
/*
// Dane z zamówienia (jeśli dostępne)
if (orderDetails != null)
{
    cmd.Parameters.AddWithValue("@PaymentType", orderDetails.Payment?.Type ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@PaymentProvider", orderDetails.Payment?.Provider ?? (object)DBNull.Value);
    cmd.Parameters.AddWithValue("@PaymentFinishedAt", orderDetails.Payment?.FinishedAt ?? (object)DBNull.Value);
    
    // ⭐ NAPRAWIONO: Bezpieczne parsowanie kwoty PaidAmount
    decimal? paidAmount = null;
    if (orderDetails.Payment?.PaidAmount?.Amount != null)
    {
        paidAmount = SafeParseDecimal(orderDetails.Payment.PaidAmount.Amount, returnData.Id);
    }
    cmd.Parameters.AddWithValue("@PaidAmount", paidAmount ?? (object)DBNull.Value);
    
    cmd.Parameters.AddWithValue("@FulfillmentStatus", orderDetails.Fulfillment?.Status ?? (object)DBNull.Value);
    // ... reszta kodu ...
}
*/

// I ZMIEŃ NA:

// ========================================
// CZĘŚĆ 1: Pobierz email kupującego
// ========================================

// DODAJ PRZED blokiem "if (orderDetails != null)" na początku metody UpsertReturnAsync:

// ✅ NAPRAWIONE: Pobieranie emaila kupującego
string buyerEmail = null;

// Próba 1: Z OrderDetails.Buyer.Email
if (orderDetails?.Buyer?.Email != null)
{
    buyerEmail = orderDetails.Buyer.Email;
    System.Diagnostics.Debug.WriteLine($"[ZWROT {returnData.Id}] Email pobrany z OrderDetails: {buyerEmail}");
}

// Próba 2: Jeśli brak, pobierz z osobnego endpointu
if (string.IsNullOrEmpty(buyerEmail) && !string.IsNullOrEmpty(returnData.OrderId))
{
    try
    {
        buyerEmail = await apiClient.GetBuyerEmailAsync(returnData.OrderId);
        if (!string.IsNullOrEmpty(buyerEmail))
        {
            System.Diagnostics.Debug.WriteLine($"[ZWROT {returnData.Id}] Email pobrany z GetBuyerEmailAsync: {buyerEmail}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[ZWROT {returnData.Id}] WARNING: Nie można pobrać emaila!");
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ZWROT {returnData.Id}] ERROR pobierania emaila: {ex.Message}");
    }
}

// Próba 3: Z returnData.Buyer (jeśli API to zwraca - sprawdź!)
if (string.IsNullOrEmpty(buyerEmail) && returnData.Buyer != null)
{
    // Sprawdź czy AllegroCustomerReturn ma pole Buyer.Email
    // Jeśli tak, odkomentuj poniższą linię:
    // buyerEmail = returnData.Buyer.Email;
}

// ========================================
// CZĘŚĆ 2: Zmień parametry SQL
// ========================================

// ZNAJDŹ LINIĘ:
// cmd.Parameters.AddWithValue("@BuyerEmail", (object)DBNull.Value); // Brak w API zwrotów

// I ZAMIEŃ NA:
cmd.Parameters.AddWithValue("@BuyerEmail", buyerEmail ?? (object)DBNull.Value);

// ========================================
// CZĘŚĆ 3: Również w bloku ELSE (gdy brak orderDetails)
// ========================================

// ZNAJDŹ FRAGMENT:
/*
else
{
    // Brak danych z zamówienia - ustaw NULL
    cmd.Parameters.AddWithValue("@PaymentType", DBNull.Value);
    cmd.Parameters.AddWithValue("@PaymentProvider", DBNull.Value);
    // ... inne pola ...
}
*/

// I ZMIEŃ LINIĘ Z @BuyerEmail:
// BYŁO:
// cmd.Parameters.AddWithValue("@BuyerEmail", DBNull.Value);

// POWINNO BYĆ:
cmd.Parameters.AddWithValue("@BuyerEmail", buyerEmail ?? (object)DBNull.Value);

// ========================================
// PEŁNY PRZYKŁAD - Jak powinien wyglądać kod:
// ========================================

/*
private async Task<bool> UpsertReturnAsync(
    AllegroCustomerReturn returnData,
    OrderDetails orderDetails,
    int accountId,
    MySqlConnection con)
{
    // ... początek metody (sprawdzenie czy istnieje, etc.) ...
    
    var firstItem = returnData.Items?.FirstOrDefault();
    var firstParcel = returnData.Parcels?.FirstOrDefault();

    // ✅ NAPRAWIONE: Pobieranie emaila kupującego
    string buyerEmail = null;

    // Próba 1: Z OrderDetails.Buyer.Email
    if (orderDetails?.Buyer?.Email != null)
    {
        buyerEmail = orderDetails.Buyer.Email;
        System.Diagnostics.Debug.WriteLine($"[ZWROT {returnData.Id}] Email z OrderDetails: {buyerEmail}");
    }

    // Próba 2: Jeśli brak, pobierz z osobnego endpointu (wymaga apiClient!)
    // UWAGA: Metoda UpsertReturnAsync NIE MA dostępu do apiClient!
    // Trzeba przekazać apiClient jako parametr lub email jako parametr!
    
    string sql = isNew ? GetInsertReturnSql() : GetUpdateReturnSql();

    using (var cmd = new MySqlCommand(sql, con))
    {
        // Podstawowe dane zwrotu
        cmd.Parameters.AddWithValue("@AllegroReturnId", returnData.Id ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@AllegroAccountId", accountId);
        cmd.Parameters.AddWithValue("@ReferenceNumber", returnData.ReferenceNumber ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@OrderId", returnData.OrderId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerLogin", returnData.Buyer?.Login ?? (object)DBNull.Value);
        
        // ✅ NAPRAWIONE: Używamy zmiennej buyerEmail zamiast DBNull.Value
        cmd.Parameters.AddWithValue("@BuyerEmail", buyerEmail ?? (object)DBNull.Value);
        
        // ... reszta kodu ...
    }
}
*/

// ========================================
// WAŻNE: ZMIANA SYGNATURY METODY
// ========================================

// Ponieważ potrzebujemy apiClient, ZMIEŃ sygnaturę metody z:
/*
private async Task<bool> UpsertReturnAsync(
    AllegroCustomerReturn returnData,
    OrderDetails orderDetails,
    int accountId,
    MySqlConnection con)
*/

// NA:
private async Task<bool> UpsertReturnAsync(
    AllegroCustomerReturn returnData,
    OrderDetails orderDetails,
    AllegroApiClient apiClient,  // ✅ DODANE
    int accountId,
    MySqlConnection con)

// I ZMIEŃ WYWOŁANIE w SynchronizeReturnsForAccountAsync:
/*
// BYŁO:
bool isNew = await UpsertReturnAsync(returnData, orderDetails, accountId, con);

// POWINNO BYĆ:
bool isNew = await UpsertReturnAsync(returnData, orderDetails, apiClient, accountId, con);
*/

// ========================================
// WERYFIKACJA
// ========================================

// Po wdrożeniu uruchom SQL:
/*
-- Sprawdź emaile w zwrotach
SELECT 
    COUNT(*) as Total,
    SUM(CASE WHEN BuyerEmail IS NULL THEN 1 ELSE 0 END) as BezEmaila,
    SUM(CASE WHEN BuyerEmail IS NOT NULL THEN 1 ELSE 0 END) as ZEmailem,
    ROUND(SUM(CASE WHEN BuyerEmail IS NOT NULL THEN 1 ELSE 0 END) * 100.0 / COUNT(*), 2) as ProcentZEmailem
FROM AllegroCustomerReturns
WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 7 DAY);

-- Powinno być ProcentZEmailem > 80%
*/

// ========================================
// KONIEC NAPRAWY #4
// ========================================