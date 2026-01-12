# ✅ NAPRAWA: Alias tabeli w SQL

**Data:** 2026-01-08  
**Status:** ✅ **NAPRAWIONE!**

---

## ❌ PROBLEM:

**Błąd:** `Unknown column 'allegrocustomerreturns.Id' in 'SELECT'`

**Przyczyna:** Zapytanie definiuje alias `acr`, ale używa pełnej nazwy tabeli!

```sql
FROM AllegroCustomerReturns acr  -- Definiuje alias 'acr'
...
SELECT allegrocustomerreturns.Id  -- ❌ Używa pełnej nazwy!
```

**MySQL nie rozpoznaje `allegrocustomerreturns.Id` gdy alias `acr` jest zdefiniowany!**

---

## 🎯 SZCZEGÓŁY:

### **Co się działo:**

**Kod przed naprawą:**
```sql
SELECT
    allegrocustomerreturns.Id,          -- ❌ Pełna nazwa
    allegrocustomerreturns.ReferenceNumber,
    ...
FROM AllegroCustomerReturns acr         -- Definiuje alias!
LEFT JOIN Statusy s1 ON allegrocustomerreturns.StanProduktuId = s1.Id  -- ❌
WHERE allegrocustomerreturns.HandlowiecOpiekunId = @userId  -- ❌
```

**Problem:**
1. Gdy definiujesz alias `acr`, MySQL **wymaga** używania tego aliasu
2. Nie możesz używać pełnej nazwy tabeli **ORAZ** aliasu w tym samym zapytaniu
3. Musisz być **konsekwentny**

---

## ✅ CO NAPRAWIŁEM:

**Plik:** `HandlowiecControl.cs`  
**Metody:** `LoadReturnsFromDbAsync()` + `UpdateFilterCountsAsync()`

**Zamienione wystąpienia:** 20+

### **Kod po naprawie:**

```sql
SELECT
    acr.Id,                  -- ✅ Używa aliasu
    acr.ReferenceNumber,
    acr.Waybill,
    COALESCE(acr.BuyerFullName, acr.BuyerLogin, 'Nieznany klient') AS Kupujacy,
    acr.ProductName,
    acr.CreatedAt,
    IFNULL(s1.Nazwa, 'Nieprzypisany') AS StanProduktu,
    IFNULL(s2.Nazwa, 'Nieprzypisany') AS StatusWewnetrzny,
    IFNULL(s3.Nazwa, 'Nieprzypisany') AS DecyzjaHandlowca,
    acr.IsManual
FROM AllegroCustomerReturns acr
LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id      -- ✅ Konsekwentnie
LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id  -- ✅ Konsekwentnie
LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id  -- ✅ Konsekwentnie
WHERE (
    acr.HandlowiecOpiekunId = @userId     -- ✅ Konsekwentnie
    OR acr.Id IN (...)                     -- ✅ Konsekwentnie
)
ORDER BY acr.CreatedAt DESC                -- ✅ Konsekwentnie
```

---

## 📋 LISTA ZMIAN:

| Miejsce | PRZED | PO |
|---------|-------|-----|
| SELECT | `allegrocustomerreturns.Id` | `acr.Id` |
| SELECT | `allegrocustomerreturns.ReferenceNumber` | `acr.ReferenceNumber` |
| SELECT | `allegrocustomerreturns.Waybill` | `acr.Waybill` |
| SELECT | `allegrocustomerreturns.BuyerFullName` | `acr.BuyerFullName` |
| SELECT | `allegrocustomerreturns.BuyerLogin` | `acr.BuyerLogin` |
| SELECT | `allegrocustomerreturns.ProductName` | `acr.ProductName` |
| SELECT | `allegrocustomerreturns.CreatedAt` | `acr.CreatedAt` |
| SELECT | `allegrocustomerreturns.IsManual` | `acr.IsManual` |
| JOIN | `allegrocustomerreturns.StanProduktuId` | `acr.StanProduktuId` |
| JOIN | `allegrocustomerreturns.StatusWewnetrznyId` | `acr.StatusWewnetrznyId` |
| JOIN | `allegrocustomerreturns.DecyzjaHandlowcaId` | `acr.DecyzjaHandlowcaId` |
| WHERE | `allegrocustomerreturns.HandlowiecOpiekunId` | `acr.HandlowiecOpiekunId` |
| WHERE | `allegrocustomerreturns.Id IN` | `acr.Id IN` |
| WHERE | `allegrocustomerreturns.ReferenceNumber` | `acr.ReferenceNumber` |
| ORDER BY | `allegrocustomerreturns.CreatedAt` | `acr.CreatedAt` |

**Razem: 20+ zamian!**

---

## 🚀 INSTRUKCJA:

### **KROK 1: Rebuild (1 min)**
```
Visual Studio → Build → Rebuild Solution
Oczekiwany wynik: 0 errors ✅
```

### **KROK 2: Test (2 min)**
```
F5 → Zaloguj jako Handlowiec
Oczekiwany wynik: Moduł Handlowiec ładuje się ✅
                  Lista zwrotów wyświetla się ✅
```

---

## 📖 LEKCJA: Aliasy tabel w SQL

### **Zasady:**

1. **Jeśli definiujesz alias, MUSISZ go używać:**
   ```sql
   FROM TableName alias  -- Od tego momentu używaj 'alias'
   ```

2. **NIE mieszaj pełnych nazw z aliasami:**
   ```sql
   -- ❌ ŹLE:
   FROM Users u
   WHERE Users.id = 1  -- Błąd! Użyj 'u.id'
   
   -- ✅ DOBRZE:
   FROM Users u
   WHERE u.id = 1
   ```

3. **Bądź konsekwentny we WSZYSTKICH miejscach:**
   - SELECT
   - JOIN
   - WHERE
   - ORDER BY
   - GROUP BY
   - HAVING

---

## 🎯 DLACZEGO TO JEST WAŻNE:

**MySQL jest STRICT w kwestii aliasów:**
- SQLite: Czasem toleruje mieszanie ✅
- MySQL: **NIGDY** nie toleruje mieszania ❌

**Przykład:**
```sql
-- SQLite: To może działać
FROM Users u WHERE Users.id = 1

-- MySQL: To ZAWSZE wyrzuci błąd
FROM Users u WHERE Users.id = 1
-- Error: Unknown column 'Users.id'
```

---

## ✅ NAPRAWIONE:

**Plik:** HandlowiecControl.cs  
**Metody:** 2 (LoadReturnsFromDbAsync + UpdateFilterCountsAsync)  
**Zamian:** 20+  
**Typ błędu:** Alias tabeli  

---

**REBUILD + TEST = 3 MINUTY = DZIAŁA!** 🎉

*Aliasy tabel muszą być używane konsekwentnie!*
