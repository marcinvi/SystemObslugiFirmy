# DOKUMENTACJA STRUKTURY BAZY DANYCH
## System Obsługi Reklamacji - MySQL/MariaDB

---

## SPIS TREŚCI

1. [Przegląd](#przegląd)
2. [Diagram ERD](#diagram-erd)
3. [Tabele główne](#tabele-główne)
4. [Tabele pomocnicze](#tabele-pomocnicze)
5. [Widoki](#widoki)
6. [Indeksy i optymalizacja](#indeksy-i-optymalizacja)
7. [Relacje między tabelami](#relacje-między-tabelami)

---

## PRZEGLĄD

Baza danych ReklamacjeDB obsługuje kompleksowy system zarządzania reklamacjami produktów. System integruje się z:
- **Allegro** - automatyczna synchronizacja zgłoszeń
- **DPD** - śledzenie przesyłek
- **Email/SMS** - komunikacja z klientami
- **Google Sheets** - synchronizacja danych

### Statystyki bazy
- **Liczba tabel:** 24 głównych tabeli
- **Liczba widoków:** 2 widoki analityczne
- **Typ bazy:** InnoDB (transakcyjna)
- **Kodowanie:** UTF-8 (utf8mb4)

---

## DIAGRAM ERD

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│  Klienci    │         │ Zgloszenia  │         │  Produkty   │
│             │◄────────┤             ├────────►│             │
│  • Email    │         │ • NrZgłosz. │         │  • Nazwa    │
│  • Telefon  │         │ • Status    │         │  • Model    │
└─────────────┘         └──────┬──────┘         └─────────────┘
                               │
                  ┌────────────┼────────────┐
                  │            │            │
           ┌──────▼──────┐ ┌──▼──────┐ ┌──▼──────────┐
           │  Dzialania  │ │EmailLog │ │  SmsLog     │
           │             │ │         │ │             │
           └─────────────┘ └─────────┘ └─────────────┘
```

---

## TABELE GŁÓWNE

### 1. **Zgloszenia** - Centralna tabela systemu

Główna tabela przechowująca wszystkie zgłoszenia reklamacyjne.

| Kolumna | Typ | Opis |
|---------|-----|------|
| `Id` | INT (PK) | Unikalny identyfikator |
| `NrZgloszenia` | VARCHAR(50) | Format: R/[numer]/[rok] (np. R/123/2025) |
| `KlientID` | INT (FK) | Powiązanie z tabelą Klienci |
| `ProduktID` | INT (FK) | Powiązanie z tabelą Produkty |
| `DataZgloszenia` | DATETIME | Data utworzenia zgłoszenia |
| `DataZakupu` | DATE | Data zakupu produktu |
| `NrFaktury` | VARCHAR(100) | Numer faktury zakupu |
| `NrSeryjny` | VARCHAR(100) | Numer seryjny produktu |
| `Usterka` | TEXT | Opis usterki |
| `StatusOgolny` | VARCHAR(100) | Status (np. "Nowe", "W naprawie") |
| `StatusDpd` | VARCHAR(100) | Status przesyłki DPD |
| `PrzypisanyDo` | INT (FK) | ID użytkownika obsługującego |
| `allegroDisputeId` | VARCHAR(100) | ID zgłoszenia z Allegro |
| `allegroAccountId` | INT | ID konta Allegro |

**Indeksy:**
- PRIMARY: `Id`
- UNIQUE: `NrZgloszenia`
- INDEX: `KlientID`, `ProduktID`, `StatusOgolny`, `allegroDisputeId`

**Przykład użycia:**
```sql
-- Pobierz wszystkie aktywne zgłoszenia z ostatniego miesiąca
SELECT * FROM Zgloszenia 
WHERE StatusOgolny != 'Zamknięte' 
AND DataZgloszenia > DATE_SUB(NOW(), INTERVAL 1 MONTH)
ORDER BY DataZgloszenia DESC;
```

---

### 2. **Klienci** - Dane klientów

Przechowuje informacje o klientach (osoby prywatne i firmy).

| Kolumna | Typ | Opis |
|---------|-----|------|
| `Id` | INT (PK) | Unikalny identyfikator |
| `ImieNazwisko` | VARCHAR(255) | Dla osób prywatnych |
| `NazwaFirmy` | VARCHAR(255) | Dla firm |
| `NIP` | VARCHAR(20) | Numer NIP (dla firm) |
| `Ulica` | VARCHAR(255) | Adres |
| `KodPocztowy` | VARCHAR(10) | Kod pocztowy |
| `Miejscowosc` | VARCHAR(255) | Miasto |
| `Email` | VARCHAR(255) | Email kontaktowy |
| `Telefon` | VARCHAR(50) | Numer telefonu |

**Logika biznesowa:**
- Jeśli `NazwaFirmy` lub `NIP` jest wypełnione → klient firmowy
- W przeciwnym razie → klient prywatny

**Przykład:**
```sql
-- Znajdź klienta po emailu lub telefonie
SELECT * FROM Klienci 
WHERE Email = 'klient@example.com' 
   OR Telefon LIKE '%123456789%'
LIMIT 1;
```

---

### 3. **Produkty** - Katalog produktów

| Kolumna | Typ | Opis |
|---------|-----|------|
| `Id` | INT (PK) | Unikalny identyfikator |
| `Nazwa` | VARCHAR(255) | Nazwa produktu |
| `Producent` | VARCHAR(255) | Producent |
| `Model` | VARCHAR(255) | Model |
| `KodEnova` | VARCHAR(100) | Kod z systemu Enova |
| `KodProducenta` | VARCHAR(100) | Kod producenta |
| `Kategoria` | VARCHAR(100) | Kategoria produktu |

**Przykład:**
```sql
-- Wyszukaj produkty po nazwie lub kodzie
SELECT * FROM Produkty 
WHERE Nazwa LIKE '%laptop%' 
   OR KodEnova = 'ABC123'
ORDER BY Nazwa;
```

---

### 4. **Dzialania** - Historia działań

Każda akcja w ramach zgłoszenia jest rejestrowana.

| Kolumna | Typ | Opis |
|---------|-----|------|
| `Id` | INT (PK) | Unikalny identyfikator |
| `ZgloszenieID` | INT (FK) | Powiązanie ze zgłoszeniem |
| `TypDzialania` | VARCHAR(100) | Typ (np. "Naprawa", "Wymiana") |
| `Opis` | TEXT | Szczegółowy opis |
| `DataDzialania` | DATETIME | Kiedy wykonano |
| `UzytkownikID` | INT (FK) | Kto wykonał |
| `StatusPrzed` | VARCHAR(100) | Status przed akcją |
| `StatusPo` | VARCHAR(100) | Status po akcji |

**Przykład:**
```sql
-- Historia działań dla zgłoszenia
SELECT 
    d.DataDzialania,
    d.TypDzialania,
    d.Opis,
    u.`Nazwa Wyświetlana` AS Uzytkownik,
    d.StatusPrzed,
    d.StatusPo
FROM Dzialania d
LEFT JOIN Uzytkownicy u ON d.UzytkownikID = u.Id
WHERE d.ZgloszenieID = 123
ORDER BY d.DataDzialania DESC;
```

---

### 5. **EmailLog** - Historia emaili

| Kolumna | Typ | Opis |
|---------|-----|------|
| `Id` | INT (PK) | Unikalny identyfikator |
| `ZgloszenieID` | INT (FK) | Powiązanie ze zgłoszeniem |
| `MessageId` | VARCHAR(255) | Unikalny ID wiadomości |
| `Nadawca` | VARCHAR(255) | Email nadawcy |
| `Odbiorca` | VARCHAR(255) | Email odbiorcy |
| `Temat` | VARCHAR(500) | Temat |
| `Tresc` | LONGTEXT | Treść tekstowa |
| `TrescHtml` | LONGTEXT | Treść HTML |
| `Kierunek` | VARCHAR(10) | 'IN' lub 'OUT' |
| `Data` | DATETIME | Data wysłania/odbioru |

**Przykład:**
```sql
-- Wszystkie emaile dla zgłoszenia
SELECT 
    Data,
    Kierunek,
    CASE WHEN Kierunek='IN' THEN Nadawca ELSE Odbiorca END AS 'Kontakt',
    Temat,
    SUBSTRING(Tresc, 1, 100) AS 'Fragment'
FROM EmailLog
WHERE ZgloszenieID = 123
ORDER BY Data DESC;
```

---

### 6. **AllegroAccounts** - Konta Allegro

| Kolumna | Typ | Opis |
|---------|-----|------|
| `Id` | INT (PK) | Unikalny identyfikator |
| `AccountName` | VARCHAR(255) | Nazwa konta |
| `ClientId` | VARCHAR(255) | Client ID z Allegro |
| `ClientSecretEncrypted` | TEXT | Zaszyfrowany secret |
| `AccessTokenEncrypted` | TEXT | Zaszyfrowany access token |
| `RefreshTokenEncrypted` | TEXT | Zaszyfrowany refresh token |
| `TokenExpirationDate` | VARCHAR(50) | Data wygaśnięcia tokenu |
| `IsAuthorized` | TINYINT | Czy autoryzowane (0/1) |

**Bezpieczeństwo:**
- Wszystkie tokeny są szyfrowane przed zapisem
- Używaj `EncryptionHelper` do szyfrowania/deszyfrowania

---

### 7. **AllegroDisputes** - Zgłoszenia z Allegro

| Kolumna | Typ | Opis |
|---------|-----|------|
| `DisputeId` | VARCHAR(100) | ID zgłoszenia z Allegro (UNIQUE) |
| `AccountId` | INT (FK) | Powiązanie z kontem |
| `Status` | VARCHAR(50) | Status w Allegro |
| `BuyerLogin` | VARCHAR(255) | Login kupującego |
| `ProductName` | VARCHAR(500) | Nazwa produktu |
| `OrderId` | VARCHAR(100) | ID zamówienia |
| `HasNewMessages` | TINYINT | Czy są nowe wiadomości |
| `IsRegistered` | TINYINT | Czy zarejestrowane w systemie |
| `ZgloszenieId` | INT (FK) | Powiązanie ze zgłoszeniem |

**Workflow:**
1. Synchronizacja pobiera zgłoszenia z API Allegro
2. `IsRegistered=0` → zgłoszenie czeka na rejestrację
3. Po rejestracji tworzone jest zgłoszenie w tabeli `Zgloszenia`
4. `ZgloszenieId` łączy oba rekordy

---

### 8. **AllegroChatMessages** - Czat Allegro

| Kolumna | Typ | Opis |
|---------|-----|------|
| `MessageId` | VARCHAR(100) | ID wiadomości z Allegro |
| `DisputeId` | VARCHAR(100) | ID zgłoszenia |
| `AuthorLogin` | VARCHAR(255) | Login autora |
| `AuthorRole` | VARCHAR(50) | Rola (BUYER/SELLER) |
| `MessageText` | TEXT | Treść wiadomości |
| `CreatedAt` | DATETIME | Data utworzenia |

**Przykład:**
```sql
-- Wiadomości czatu dla zgłoszenia
SELECT 
    CreatedAt,
    AuthorLogin,
    AuthorRole,
    MessageText
FROM AllegroChatMessages
WHERE DisputeId = 'allegro-dispute-123'
ORDER BY CreatedAt ASC;
```

---

### 9. **Przypomnienia** - System przypomnień

| Kolumna | Typ | Opis |
|---------|-----|------|
| `ticket_id` | VARCHAR(100) | Numer zgłoszenia |
| `source` | VARCHAR(50) | Źródło (AUTO/DPD/MANUAL) |
| `category` | VARCHAR(50) | Kategoria |
| `title` | VARCHAR(500) | Tytuł przypomnienia |
| `due_at` | DATETIME | Termin |
| `status` | VARCHAR(50) | Status (pending/done/cancelled) |
| `dedupe_key` | VARCHAR(255) | Klucz deduplikacji |

**Typy przypomnień:**
- **decision** - przypomnienie o decyzji (Allegro SLA)
- **courier** - przypomnienie o przesyłce (DPD)
- **manual** - ręczne przypomnienie

**Przykład:**
```sql
-- Przypomnienia na dziś
SELECT * FROM Przypomnienia
WHERE status = 'pending'
  AND due_at <= NOW()
ORDER BY due_at ASC;
```

---

### 10. **DpdPrzesylki** - Śledzenie przesyłek

| Kolumna | Typ | Opis |
|---------|-----|------|
| `ZgloszenieId` | INT (FK) | Powiązanie ze zgłoszeniem |
| `NumerPrzesylki` | VARCHAR(100) | Numer listu przewozowego (UNIQUE) |
| `Status` | VARCHAR(100) | Aktualny status |
| `DataDostarczenia` | DATETIME | Kiedy dostarczono |
| `OczekiwanaDataDostawy` | DATE | Oczekiwana data |

---

## TABELE POMOCNICZE

### Magazyn
Zarządzanie stanami magazynowymi części zamiennych.

### RuchyMagazynowe
Historia przyjęć, wydań i korekt magazynowych.

### KontaPocztowe
Konfiguracja kont email (POP3/IMAP/SMTP).

### SzablonyEmail
Szablony wiadomości email z możliwością użycia zmiennych.

### Uzytkownicy
Użytkownicy systemu z hasłami i rolami.

### LogAktywnosci
Pełen dziennik operacji w systemie (audit trail).

---

## WIDOKI

### v_Zgloszenia_Pelne
Widok agregujący dane zgłoszeń z powiązanych tabel.

```sql
SELECT * FROM v_Zgloszenia_Pelne
WHERE StatusOgolny = 'Nowe'
ORDER BY DataZgloszenia DESC;
```

### v_Statystyki_Zgloszen
Statystyki zgłoszeń według statusów.

```sql
SELECT * FROM v_Statystyki_Zgloszen;
```

---

## INDEKSY I OPTYMALIZACJA

### Strategia indeksowania

**1. Klucze główne (PRIMARY KEY)**
- Każda tabela ma kolumnę `Id` jako klucz główny
- AUTO_INCREMENT dla automatycznego generowania

**2. Klucze unikalne (UNIQUE)**
- `Zgloszenia.NrZgloszenia`
- `AllegroDisputes.DisputeId`
- `DpdPrzesylki.NumerPrzesylki`

**3. Indeksy wyszukiwania**
- Kolumny często używane w WHERE: email, telefon, status
- Kolumny używane w JOIN: klucze obce
- Kolumny używane w ORDER BY: daty

**4. Indeksy złożone**
```sql
-- Dla zapytań filtrujących po statusie i dacie jednocześnie
CREATE INDEX idx_status_date ON Zgloszenia(StatusOgolny, DataZgloszenia);
```

---

## RELACJE MIĘDZY TABELAMI

### Relacje 1:N (one-to-many)

```
Klienci (1) ──────► (N) Zgloszenia
Produkty (1) ─────► (N) Zgloszenia
Uzytkownicy (1) ──► (N) Zgloszenia (PrzypisanyDo)
Zgloszenia (1) ───► (N) Dzialania
Zgloszenia (1) ───► (N) EmailLog
Zgloszenia (1) ───► (N) SmsLog
AllegroAccounts (1) ─► (N) AllegroDisputes
```

### Opcje kluczy obcych

**ON DELETE CASCADE**
- `Dzialania` → Zgloszenia
- Jeśli usuniesz zgłoszenie, wszystkie działania również zostaną usunięte

**ON DELETE SET NULL**
- `Zgloszenia` → Klienci
- Jeśli usuniesz klienta, zgłoszenia pozostaną (KlientID = NULL)

---

## NAJLEPSZE PRAKTYKI

### 1. Transakcje
Zawsze używaj transakcji przy operacjach multi-step:
```sql
START TRANSACTION;
-- operacje
COMMIT; -- lub ROLLBACK;
```

### 2. Prepared Statements
W kodzie C# zawsze używaj parametrów:
```csharp
cmd.Parameters.AddWithValue("@nrZgloszenia", numer);
```

### 3. Indeksy
Monitoruj wydajność i dodawaj indeksy dla często używanych zapytań:
```sql
EXPLAIN SELECT * FROM Zgloszenia WHERE StatusOgolny='Nowe';
```

### 4. Archiwizacja
Regularnie archiwizuj stare dane:
```sql
-- Przenieś zamknięte zgłoszenia starsze niż rok do tabeli archiwum
INSERT INTO Zgloszenia_Archiwum 
SELECT * FROM Zgloszenia 
WHERE StatusOgolny='Zamknięte' 
  AND DataZamkniecia < DATE_SUB(NOW(), INTERVAL 1 YEAR);
```

---

## ZAPYTANIA PRZYKŁADOWE

### Dashboard - Statystyki
```sql
SELECT 
    COUNT(*) AS 'Wszystkie',
    SUM(CASE WHEN StatusOgolny='Nowe' THEN 1 ELSE 0 END) AS 'Nowe',
    SUM(CASE WHEN StatusOgolny='W naprawie' THEN 1 ELSE 0 END) AS 'W naprawie',
    SUM(CASE WHEN MONTH(DataZgloszenia)=MONTH(NOW()) THEN 1 ELSE 0 END) AS 'Ten miesiąc'
FROM Zgloszenia
WHERE DataZamkniecia IS NULL;
```

### Wyszukiwanie wielokryterialne
```sql
SELECT 
    z.NrZgloszenia,
    k.ImieNazwisko,
    k.NazwaFirmy,
    p.Nazwa AS Produkt,
    z.StatusOgolny,
    z.DataZgloszenia
FROM Zgloszenia z
LEFT JOIN Klienci k ON z.KlientID = k.Id
LEFT JOIN Produkty p ON z.ProduktID = p.Id
WHERE 
    (k.Email LIKE '%szukane%' OR k.Telefon LIKE '%szukane%')
    AND z.StatusOgolny != 'Zamknięte'
ORDER BY z.DataZgloszenia DESC
LIMIT 50;
```

### Historia komunikacji
```sql
SELECT 
    'EMAIL' AS Typ,
    Data,
    Kierunek,
    Temat AS Tresc,
    Nadawca
FROM EmailLog
WHERE ZgloszenieID = 123

UNION ALL

SELECT 
    'SMS' AS Typ,
    Data,
    Kierunek,
    Tresc,
    NumerTelefonu AS Nadawca
FROM SmsLog
WHERE ZgloszenieID = 123

UNION ALL

SELECT 
    'ALLEGRO' AS Typ,
    CreatedAt AS Data,
    CASE WHEN AuthorRole='BUYER' THEN 'IN' ELSE 'OUT' END AS Kierunek,
    MessageText AS Tresc,
    AuthorLogin AS Nadawca
FROM AllegroChatMessages acm
JOIN AllegroDisputes ad ON acm.DisputeId = ad.DisputeId
WHERE ad.ZgloszenieId = 123

ORDER BY Data DESC;
```

---

**Wersja dokumentu:** 1.0  
**Data ostatniej aktualizacji:** 2025-01-07  
**Autor:** System Obsługi Reklamacji
