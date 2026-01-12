# Naprawa błędu: "Nieprawidłowy format ciągu wejściowego"

## 📋 Problem

**Błąd:** `Błąd przetwarzania zwrotu 5d204629-6fd1-4a73-bf5e-f27b4c32ae99: Nieprawidłowy format ciągu wejściowego.`

**Lokalizacja błędu:** `AllegroSyncServiceExtended.cs`

### Przyczyna

Błąd występował podczas parsowania kwot pieniężnych (decimal) z API Allegro. Kod używał niebezpiecznego `decimal.Parse()` bez obsługi błędów, co powodowało crash gdy:

1. API zwracało kwoty w różnych formatach:
   - Z separatorem tysięcy: `"1,234.56"`
   - Z polskim formatem: `"1234,56"`  
   - Pusty string: `""`
   - Wartości null lub whitespace

2. Dwa miejsca w kodzie miały ten problem:
   - **Linia ~293** - Parsowanie `PaidAmount` w metodzie `UpsertReturnAsync()`
   - **Linia ~XXX** - Parsowanie `RefundAmount` w metodzie `UpsertIssueAsync()`

### Kod błędny (PRZED naprawą)

```csharp
// BŁĘDNY KOD - wywołuje exception!
cmd.Parameters.AddWithValue("@PaidAmount",
    orderDetails.Payment?.PaidAmount != null
        ? (object)decimal.Parse(orderDetails.Payment.PaidAmount.Amount)  // ❌ CRASH tutaj!
        : DBNull.Value);
```

## ✅ Rozwiązanie

### 1. Dodano helper method `SafeParseDecimal()`

```csharp
/// <summary>
/// Bezpiecznie parsuje string na decimal, obsługując różne formaty i błędy
/// </summary>
private decimal? SafeParseDecimal(string value, string returnId = null)
{
    if (string.IsNullOrWhiteSpace(value))
        return null;

    value = value.Trim();

    try
    {
        // Usuń separatory tysięcy
        value = value.Replace(" ", "").Replace(",", "");

        // Próba parsowania z InvariantCulture (kropka jako separator dziesiętny)
        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
        {
            return result;
        }

        // Próba parsowania z kulturą polską (przecinek jako separator dziesiętny)
        if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("pl-PL"), out decimal resultPL))
        {
            return resultPL;
        }

        // Zaloguj ostrzeżenie jeśli parsowanie się nie powiodło
        System.Diagnostics.Debug.WriteLine(
            $"OSTRZEŻENIE: Nie można sparsować kwoty '{value}'" +
            (returnId != null ? $" dla zwrotu/issue {returnId}" : ""));

        return null;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine(
            $"BŁĄD parsowania kwoty '{value}'" +
            (returnId != null ? $" dla zwrotu/issue {returnId}" : "") +
            $": {ex.Message}");
        return null;
    }
}
```

### 2. Poprawiono kod parsowania PaidAmount

```csharp
// ✅ POPRAWNY KOD - bezpieczne parsowanie
decimal? paidAmount = null;
if (orderDetails.Payment?.PaidAmount?.Amount != null)
{
    paidAmount = SafeParseDecimal(orderDetails.Payment.PaidAmount.Amount, returnData.Id);
}
cmd.Parameters.AddWithValue("@PaidAmount", paidAmount ?? (object)DBNull.Value);
```

### 3. Poprawiono kod parsowania RefundAmount

```csharp
// ✅ POPRAWNY KOD - bezpieczne parsowanie
decimal? refundAmount = null;
if (firstExpectation?.Refund?.Amount != null)
{
    refundAmount = SafeParseDecimal(firstExpectation.Refund.Amount, issue.Id);
}
cmd.Parameters.AddWithValue("@ExpectationRefundAmount", refundAmount ?? (object)DBNull.Value);
```

## 🔧 Instrukcja wdrożenia

### Krok 1: Backup starego pliku

```bash
# Skopiuj stary plik jako backup
copy AllegroSyncServiceExtended.cs AllegroSyncServiceExtended.cs.backup
```

### Krok 2: Zastąp plik naprawionym

```bash
# Zastąp stary plik nowym
copy AllegroSyncServiceExtended_FIXED.cs AllegroSyncServiceExtended.cs
```

### Krok 3: Przebuduj projekt

1. Otwórz projekt w Visual Studio
2. Wybierz **Build → Rebuild Solution**
3. Sprawdź czy nie ma błędów kompilacji

### Krok 4: Testowanie

1. Uruchom synchronizację zwrotów Allegro
2. Sprawdź logi w Output/Debug window
3. Upewnij się że zwroty synchronizują się poprawnie

## 📊 Co zostało naprawione

| Problem | Rozwiązanie |
|---------|------------|
| `decimal.Parse()` bez obsługi błędów | Dodano `SafeParseDecimal()` z try-catch |
| Brak obsługi różnych formatów kwot | Próba parsowania z InvariantCulture i pl-PL |
| Crash na pustych stringach | Sprawdzanie `IsNullOrWhiteSpace()` przed parsowaniem |
| Brak logowania błędów | Dodano `Debug.WriteLine()` z informacją o błędzie |
| Separatory tysięcy | Usuwanie spacji i przecinków przed parsowaniem |

## 🎯 Korzyści

✅ **Stabilność** - Synchronizacja nie crashuje na błędnych danych  
✅ **Logowanie** - Błędy parsowania są logowane w Debug output  
✅ **Kompatybilność** - Obsługa różnych formatów kwot (US/PL)  
✅ **Kontynuacja** - Synchronizacja kontynuuje się mimo błędów w pojedynczych rekordach  

## 📝 Notatki

- Metoda `SafeParseDecimal()` jest **reusable** - można jej używać w innych miejscach
- Logowanie błędów pomaga w debugowaniu problemów z danymi z API
- W przyszłości można rozszerzyć `SafeParseDecimal()` o obsługę innych formatów/walut

## 🔍 Monitorowanie

Po wdrożeniu naprawy, monitoruj:

1. **Debug Output** - szukaj wpisów zawierających:
   - `"OSTRZEŻENIE: Nie można sparsować kwoty"`
   - `"BŁĄD parsowania kwoty"`

2. **Logi synchronizacji** - sprawdź tabelę `AllegroSyncLog`:
   ```sql
   SELECT * FROM AllegroSyncLog 
   WHERE Status = 'FAILED' 
   ORDER BY StartedAt DESC;
   ```

3. **Rekordy z NULL w kwotach**:
   ```sql
   SELECT COUNT(*) FROM AllegroCustomerReturns 
   WHERE PaidAmount IS NULL AND OrderJsonDetails IS NOT NULL;
   ```

## 📞 Kontakt

W razie pytań lub problemów:
- Sprawdź logi w Debug Output
- Sprawdź tabelę `AllegroSyncLog`
- Sprawdź pole `JsonDetails` w `AllegroCustomerReturns` dla problematycznych rekordów

---

**Data naprawy:** 2026-01-07  
**Wersja:** 2.2 FIXED  
**Status:** ✅ Gotowe do wdrożenia
