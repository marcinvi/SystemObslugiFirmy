# Naprawa zapytań SQL - zamiana || na CONCAT()

## Problem
MariaDB nie obsługuje operatora `||` do konkatenacji stringów w SQL.
Trzeba użyć funkcji `CONCAT()`.

## Pliki do naprawy:

### ✅ NAPRAWIONE:
1. DatabaseService.cs (linia 185) - DONE
2. DatabaseService.cs (linia 225) - DONE

### ⚠️ DO NAPRAWY RĘCZNIE:

Otwórz każdy plik i znajdź linie z `||`, następnie zamień na `CONCAT()`:

**ContactRepository.cs (linia 399)**:
```sql
-- STARE:
TRIM(IFNULL(k.NazwaFirmy, '') || ' ' || IFNULL(k.ImieNazwisko, 'Nieznany Klient')) AS Title,

-- NOWE:
TRIM(CONCAT(IFNULL(k.NazwaFirmy, ''), ' ', IFNULL(k.ImieNazwisko, 'Nieznany Klient'))) AS Title,
```

**ContactRepository.cs (linia 421)**:
```sql
-- STARE:
'Zgłoszenie ' || IFNULL(z.NrZgloszenia, CAST(z.Id AS TEXT)) AS Title,

-- NOWE:
CONCAT('Zgłoszenie ', IFNULL(z.NrZgloszenia, CAST(z.Id AS CHAR))) AS Title,
```

**DpdTrackingService.cs (linia 164-165)**:
```sql
-- STARE:
(IFNULL(nad.ImieNazwisko, '') || ' ' || IFNULL(nad.NazwaFirmy, '')) AS Nadawca,
(IFNULL(odb.ImieNazwisko, '') || ' ' || IFNULL(odb.NazwaFirmy, '')) AS Odbiorca

-- NOWE:
CONCAT(IFNULL(nad.ImieNazwisko, ''), ' ', IFNULL(nad.NazwaFirmy, '')) AS Nadawca,
CONCAT(IFNULL(odb.ImieNazwisko, ''), ' ', IFNULL(odb.NazwaFirmy, '')) AS Odbiorca
```

**Form20.cs (linia 150)**:
```csharp
// STARE:
case "Klient": return "k.ImieNazwisko || ' | ' || k.NazwaFirmy";

// NOWE:
case "Klient": return "CONCAT(k.ImieNazwisko, ' | ', k.NazwaFirmy)";
```

**FormDpdTracking.cs (linia 126-127 i 159-160)**:
```sql
-- STARE:
(IFNULL(n.ImieNazwisko, '') || ' ' || IFNULL(n.NazwaFirmy, '')) AS NazwaNadawcy,
(IFNULL(o.ImieNazwisko, '') || ' ' || IFNULL(o.NazwaFirmy, '')) AS NazwaOdbiorcy

-- NOWE:
CONCAT(IFNULL(n.ImieNazwisko, ''), ' ', IFNULL(n.NazwaFirmy, '')) AS NazwaNadawcy,
CONCAT(IFNULL(o.ImieNazwisko, ''), ' ', IFNULL(o.NazwaFirmy, '')) AS NazwaOdbiorcy
```

**ZgloszeniaService.cs (linia 39)**:
```sql
-- STARE:
WHEN k.ImieNazwisko IS NOT NULL AND k.NazwaFirmy IS NOT NULL THEN k.ImieNazwisko || ' | ' || k.NazwaFirmy

-- NOWE:
WHEN k.ImieNazwisko IS NOT NULL AND k.NazwaFirmy IS NOT NULL THEN CONCAT(k.ImieNazwisko, ' | ', k.NazwaFirmy)
```

**WeryfikacjaControl.cs (linia 230)**:
```sql
-- STARE:
(acr.Buyer_FirstName || ' ' || acr.Buyer_LastName) AS Klient

-- NOWE:
CONCAT(acr.Buyer_FirstName, ' ', acr.Buyer_LastName) AS Klient
```

**ShipmentNotificationService.cs (linia 32)**:
```sql
-- STARE:
(k.ImieNazwisko || ' ' || k.NazwaFirmy) AS NazwaOdbiorcy

-- NOWE:
CONCAT(k.ImieNazwisko, ' ', k.NazwaFirmy) AS NazwaOdbiorcy
```

**ReklamacjeControl.cs (linia 219)**:
```sql
-- STARE:
WHEN k.NazwaFirmy IS NOT NULL AND k.NazwaFirmy != '' AND k.ImieNazwisko IS NOT NULL AND k.ImieNazwisko != '' THEN k.NazwaFirmy || ' | ' || k.ImieNazwisko

-- NOWE:
WHEN k.NazwaFirmy IS NOT NULL AND k.NazwaFirmy != '' AND k.ImieNazwisko IS NOT NULL AND k.ImieNazwisko != '' THEN CONCAT(k.NazwaFirmy, ' | ', k.ImieNazwisko)
```

**MagazynControl.cs (linia 341-342 i 384-385)**:
```sql
-- STARE:
COALESCE(acr.Delivery_FirstName || ' ' || acr.Delivery_LastName,
         acr.Buyer_FirstName || ' ' || acr.Buyer_LastName,

-- NOWE:
COALESCE(CONCAT(acr.Delivery_FirstName, ' ', acr.Delivery_LastName),
         CONCAT(acr.Buyer_FirstName, ' ', acr.Buyer_LastName),

-- I:
(acr.Delivery_FirstName || ' ' || acr.Delivery_LastName) LIKE @search OR
(acr.Buyer_FirstName    || ' ' || acr.Buyer_LastName)    LIKE @search OR

-- NOWE:
CONCAT(acr.Delivery_FirstName, ' ', acr.Delivery_LastName) LIKE @search OR
CONCAT(acr.Buyer_FirstName, ' ', acr.Buyer_LastName) LIKE @search OR
```

## Jak naprawić SZYBKO:

W Visual Studio:
1. Ctrl+H (Find and Replace)
2. Use Regular Expressions: TAK
3. Find: `\|\|`
4. Sprawdź każde wystąpienie czy to SQL (nie zmieniaj C# ||)
5. Ręcznie zamień na CONCAT()

## UWAGA:
NIE zamieniaj C# `||` (np. `if (a == null || b == null)`) - to są poprawne operatory logiczne!
Zamieniaj TYLKO w ciągach SQL (string literals).
