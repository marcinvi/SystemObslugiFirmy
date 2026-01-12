# 🔧 INSTRUKCJA NAPRAWY MODUŁU ZWROTÓW - KOMPLETNA
**Data:** 2026-01-07  
**Status:** GOTOWE DO WDROŻENIA

---

## ⚠️ PROBLEM
```
MySqlException: Unknown column 's2.Nazwa' in 'SELECT'
```

**Przyczyna:** Brak tabeli `Statusy` lub nieprawidłowa struktura tabeli `AllegroCustomerReturns`

---

## ✅ ROZWIĄZANIE - 3 KROKI

### 🔸 KROK 1: Wykonaj skrypt SQL (5 min)

1. Otwórz **MySQL Workbench** lub **phpMyAdmin**
2. Połącz się z bazą danych
3. Otwórz plik: `FIX_STATUSY_I_ZWROTY_KOMPLETNE.sql`
4. Wykonaj cały skrypt (F5 lub przycisk Execute)

**Co zrobi skrypt:**
- ✅ Utworzy tabelę `Statusy` z poprawn strukturą
- ✅ Doda 22 domyślne statusy (StatusWewnetrzny, StanProduktu, DecyzjaHandlowca)
- ✅ Doda brakujące kolumny do `AllegroCustomerReturns`:
  - `StatusWewnetrznyId`
  - `StanProduktuId`
  - `DecyzjaHandlowcaId`
  - `UwagiMagazyn`
  - `UwagiHandlowiec`
  - `ZgloszenieId`
- ✅ Ustawi domyślne statusy dla istniejących zwrotów
- ✅ Utworzy tabele `MagazynDziennik` i `AllegroReturnItems`

**Weryfikacja:**
```sql
-- Sprawdź czy są statusy
SELECT * FROM Statusy ORDER BY TypStatusu, Kolejnosc;
-- Powinno pokazać 22 statusy

-- Sprawdź strukturę zwrotów
SHOW COLUMNS FROM AllegroCustomerReturns LIKE '%Status%';
-- Powinno pokazać: StatusWewnetrznyId, StanProduktuId, DecyzjaHandlowcaId
```

---

### 🔸 KROK 2: Rebuild projektu (2 min)

1. Otwórz **Visual Studio**
2. Kliknij **Build** → **Rebuild Solution**
3. Sprawdź output:
   - ✅ **0 errors** - OK!
   - ⚠️ Ostrzeżenia można ignorować

**Jeśli są błędy:**
- Zamknij Visual Studio
- Usuń folder `bin` i `obj`
- Otwórz ponownie i Rebuild

---

### 🔸 KROK 3: Test aplikacji (10 min)

#### A) Uruchom aplikację
```
F5 lub Start
```

#### B) Przejdź do modułu Magazyn

**Test 1: Ładowanie zwrotów**
- ✅ Lista zwrotów powinna się załadować BEZ błędów
- ✅ Kolumny widoczne: Numer Zwrotu, List Przewozowy, Status Allegro, Kupujący, Stan Produktu, Status Wewnętrzny, Decyzja Handlowca

**Test 2: Filtry**
- Kliknij "Oczekuje na przyjęcie" - powinno filtrować
- Kliknij "W drodze do nas" - powinno filtrować
- Sprawdź czy liczby się zgadzają (np. "Oczekuje na przyjęcie (5)")

**Test 3: Wyszukiwanie**
- Wpisz numer listu przewozowego → Enter
- Powinien znaleźć zwrot ALBO zapytać czy dodać ręcznie

**Test 4: Skanowanie**
- Pole "Skaner" → wpisz/zeskanuj numer listu → Enter
- Powinien otworzyć formularz szczegółów zwrotu

**Test 5: Pobieranie z Allegro**
- Kliknij "Pobierz zwroty z Allegro"
- Wybierz konto
- Poczekaj na synchronizację
- Sprawdź czy nowe zwroty się pojawiły

#### C) Test formularza szczegółów zwrotu

**Otwórz dowolny zwrot (double-click)**

Formularz powinien pokazać:
- ✅ Dane kupującego
- ✅ Dane produktu
- ✅ Stan produktu (dropdown z opcjami)
- ✅ Status wewnętrzny
- ✅ Przyciski akcji

---

## 📊 WERYFIKACJA BAZY PO WDROŻENIU

### Sprawdź statusy
```sql
-- Statusy wewnętrzne (cykl życia zwrotu)
SELECT * FROM Statusy WHERE TypStatusu = 'StatusWewnetrzny';
/*
Oczekiwane:
1. Oczekuje na przyjęcie
2. Przyjęty do magazynu
3. W trakcie weryfikacji
4. Oczekuje na decyzję handlowca
5. Zakończony
6. Anulowany
7. Archiwalny
*/

-- Stany produktu (fizyczny stan)
SELECT * FROM Statusy WHERE TypStatusu = 'StanProduktu';
/*
Oczekiwane:
1. Nowy / Nieużywany
2. Używany - Stan Dobry
3. Używany - Stan Zadowalający
4. Używany - Stan Zły
5. Uszkodzony
6. Niekompletny
7. Brak produktu w przesyłce
*/

-- Decyzje handlowca
SELECT * FROM Statusy WHERE TypStatusu = 'DecyzjaHandlowca';
/*
Oczekiwane:
1. Zwrot pieniędzy - Pełna kwota
2. Zwrot pieniędzy - Częściowy
3. Wymiana na nowy produkt
4. Naprawa gwarancyjna
5. Odrzucenie zwrotu
6. Do dalszej analizy
7. Przekazanie do producenta
*/
```

### Sprawdź zwroty
```sql
-- Ile zwrotów ma przypisany status
SELECT 
    COUNT(*) as TotalZwrotow,
    SUM(CASE WHEN StatusWewnetrznyId IS NOT NULL THEN 1 ELSE 0 END) as ZeStatusem,
    SUM(CASE WHEN StatusWewnetrznyId IS NULL THEN 1 ELSE 0 END) as BezStatusu
FROM AllegroCustomerReturns;

-- Zwroty według statusu wewnętrznego
SELECT 
    s.Nazwa as StatusWewnetrzny,
    COUNT(acr.Id) as LiczbaZwrotow
FROM AllegroCustomerReturns acr
LEFT JOIN Statusy s ON acr.StatusWewnetrznyId = s.Id
GROUP BY s.Nazwa
ORDER BY LiczbaZwrotow DESC;
```

---

## 🎯 FUNKCJONALNOŚĆ MODUŁU ZWROTÓW

### 📦 MAGAZYN (MagazynControl)

**Główne funkcje:**
1. **Lista zwrotów** - wyświetla wszystkie zwroty z Allegro
2. **Filtry** 
   - Oczekuje na przyjęcie (DELIVERED, status = "Oczekuje na przyjęcie")
   - Oczekuje na decyzję handlowca
   - Po decyzji (Zakończony)
   - W drodze (IN_TRANSIT)
   - Wszystkie
3. **Wyszukiwanie** - po numerze zwrotu, liście przewozowym, nazwisku, produkcie
4. **Skanowanie** - pole skanera do szybkiego wyszukiwania po kodzie kreskowym
5. **Pobieranie z Allegro** - synchronizacja zwrotów z API
6. **Dodaj ręcznie** - dodanie zwrotu spoza Allegro
7. **Szczegóły zwrotu** - double-click otwiera formularz

**Kolumny w tabeli:**
- Numer Zwrotu (ReferenceNumber)
- Numer Listu (Waybill)
- Status Allegro (tłumaczony na polski)
- Kupujący (Delivery/Buyer imię nazwisko lub login)
- Data Utworzenia
- Przewoźnik (CarrierName)
- Stan Produktu (dropdown: Nowy, Używany, Uszkodzony...)
- Status Wewnętrzny (Oczekuje na przyjęcie, W trakcie weryfikacji...)
- Decyzja Handlowca (Zwrot pieniędzy, Wymiana, Odrzucenie...)

---

### 💼 HANDLOWIEC (HandlowiecControl)

**Główne funkcje:**
1. **Lista zwrotów oczekujących** - tylko te ze statusem "Oczekuje na decyzję handlowca"
2. **Szczegóły zwrotu** - pełne info o produkcie, kliencie, powodzie zwrotu
3. **Podejmowanie decyzji:**
   - ✅ Zwrot pieniędzy (pełny/częściowy) - wywołuje API Allegro
   - ✅ Wymiana na nowy produkt
   - ✅ Naprawa gwarancyjna
   - ✅ Odrzucenie zwrotu - wysyła powiadomienie do klienta
   - ✅ Do dalszej analizy
   - ✅ Przekazanie do producenta
4. **Kontakt z klientem** - email, telefon, wiadomość Allegro
5. **Historia działań** - dziennik wszystkich decyzji
6. **Generowanie dokumentów** - faktury, protokoły zwrotu

**Integracja z API Allegro:**
- `RefundPaymentAsync()` - zwrot pieniędzy
- `RejectCustomerReturnAsync()` - odrzucenie zwrotu
- `SendMessageAsync()` - wiadomość do klienta
- `CreateRefundClaimAsync()` - roszczenie zwrotne

---

### 📋 ZWROTY - LISTA (ZwrotyPodsumowanieControl)

**Główne funkcje:**
1. **Pełna lista zwrotów** - wszystkie zwroty w systemie
2. **Zaawansowane filtry:**
   - Status Allegro (DELIVERED, IN_TRANSIT, CREATED...)
   - Status Wewnętrzny (Oczekuje, W weryfikacji, Zakończony...)
   - Stan Produktu (Nowy, Używany, Uszkodzony...)
   - Decyzja Handlowca (Zwrot, Wymiana, Odrzucenie...)
   - Zakres dat (od-do)
   - Konto Allegro
3. **Wyszukiwanie pełnotekstowe** - wszystkie pola
4. **Sortowanie** - po każdej kolumnie
5. **Eksport** - Excel, PDF, CSV
6. **Statystyki:**
   - Liczba zwrotów wg statusu
   - Wartość zwrotów wg decyzji
   - Średni czas obsługi
   - Najczęstsze powody zwrotów
7. **Szczegóły/Edycja** - double-click

**Kolumny dodatkowe:**
- Email kupującego
- Telefon
- Wartość zwrotu
- Data decyzji handlowca
- Przypisany opiekun
- Powiązane zgłoszenie (ID)

---

## 🔄 CYKL ŻYCIA ZWROTU

```
1. UTWORZONO (CREATED)
   ↓ (w Allegro)
   
2. W DRODZE (IN_TRANSIT)
   ↓ (skanowanie w magazynie)
   
3. DOSTARCZONO (DELIVERED)
   → Status: "Oczekuje na przyjęcie"
   ↓ (pracownik magazynu)
   
4. PRZYJĘTY DO MAGAZYNU
   → Status: "Przyjęty do magazynu"
   → Stan produktu: Nowy/Używany/Uszkodzony...
   ↓
   
5. W TRAKCIE WERYFIKACJI
   → Status: "W trakcie weryfikacji"
   → Uwagi magazynowe
   ↓
   
6. OCZEKUJE NA DECYZJĘ HANDLOWCA
   → Status: "Oczekuje na decyzję handlowca"
   ↓ (handlowiec)
   
7. DECYZJA PODJĘTA
   → Decyzja: Zwrot/Wymiana/Odrzucenie/...
   → API Allegro (jeśli zwrot pieniędzy)
   → Email do klienta
   ↓
   
8. ZAKOŃCZONY
   → Status: "Zakończony"
   → Arkusz: FormPodsumowanieZwrotu (read-only)
```

---

## 🐛 TROUBLESHOOTING

### Problem: "Unknown column 's2.Nazwa'"
**Rozwiązanie:** Wykonaj skrypt SQL FIX_STATUSY_I_ZWROTY_KOMPLETNE.sql

### Problem: "Object reference not set..."
**Rozwiązanie:** 
1. Sprawdź czy tabela Statusy ma rekordy: `SELECT COUNT(*) FROM Statusy;`
2. Jeśli 0 → wykonaj część 2 skryptu SQL (INSERT INTO Statusy)

### Problem: "Foreign key constraint fails"
**Rozwiązanie:** 
1. Tymczasowo wyłącz: `SET FOREIGN_KEY_CHECKS = 0;`
2. Wykonaj skrypt
3. Włącz: `SET FOREIGN_KEY_CHECKS = 1;`

### Problem: Lista zwrotów pusta
**Przyczyny:**
1. Brak połączenia z Allegro → sprawdź autoryzację kont
2. Brak zwrotów w ostatnich 60 dniach → zmień filtr dat
3. Błąd synchronizacji → sprawdź logi

**Rozwiązanie:**
```sql
-- Sprawdź czy są zwroty w bazie
SELECT COUNT(*) FROM AllegroCustomerReturns;

-- Sprawdź ostatnią synchronizację
SELECT * FROM AllegroSyncLog ORDER BY StartedAt DESC LIMIT 5;
```

### Problem: Nie można podjąć decyzji handlowca
**Przyczyny:**
1. Brak tabeli Statusy → wykonaj skrypt SQL
2. Brak połączenia z API Allegro → sprawdź token
3. Zwrot już zakończony → sprawdź status

### Problem: Skanowanie nie działa
**Rozwiązanie:**
1. Sprawdź czy pole "Skaner" ma focus
2. Spróbuj wpisać numer ręcznie + Enter
3. Sprawdź format numeru listu (regex w `ExtractCoreWaybill`)

---

## 📈 METRYKI SUKCESU

Po wdrożeniu sprawdź:

| Metryka | Oczekiwana wartość | Jak sprawdzić |
|---------|-------------------|---------------|
| Tabela Statusy istnieje | TAK | `SHOW TABLES LIKE 'Statusy';` |
| Liczba statusów | 22 | `SELECT COUNT(*) FROM Statusy;` |
| Kolumny w AllegroCustomerReturns | +6 nowych | `SHOW COLUMNS FROM AllegroCustomerReturns;` |
| Zwroty ze statusem | >0 | `SELECT COUNT(*) FROM AllegroCustomerReturns WHERE StatusWewnetrznyId IS NOT NULL;` |
| Moduł Magazyn ładuje się | BEZ BŁĘDÓW | Test manualny |
| Filtry działają | TAK | Test manualny |
| Skanowanie działa | TAK | Test manualny |

---

## ✅ CHECKLIST WDROŻENIA

### Pre-deployment
- [ ] Backup bazy danych utworzony
- [ ] Plik `FIX_STATUSY_I_ZWROTY_KOMPLETNE.sql` gotowy

### Deployment
- [ ] Skrypt SQL wykonany bez błędów
- [ ] Weryfikacja: 22 statusy w bazie
- [ ] Weryfikacja: Kolumny StatusWewnetrznyId, StanProduktuId, DecyzjaHandlowcaId istnieją
- [ ] Visual Studio: Rebuild - 0 errors
- [ ] Aplikacja uruchamia się

### Post-deployment - Testy
- [ ] Moduł Magazyn otwiera się bez błędów
- [ ] Lista zwrotów ładuje się
- [ ] Filtry działają poprawnie
- [ ] Wyszukiwanie działa
- [ ] Skanowanie działa
- [ ] Pobieranie z Allegro działa
- [ ] Formularz szczegółów otwiera się
- [ ] Zmiana statusu działa
- [ ] Dziennik zapisuje akcje

### Post-deployment - Weryfikacja bazy
- [ ] Wszystkie zwroty mają StatusWewnetrznyId
- [ ] SQL query z JOIN Statusy działa
- [ ] Tabela MagazynDziennik zapisuje akcje

---

## 📞 WSPARCIE

Jeśli po wdrożeniu są problemy:

1. **Sprawdź logi błędów** - Visual Studio Output, Debug
2. **Sprawdź bazę** - wykonaj weryfikację SQL
3. **Przywróć backup** - jeśli coś poszło nie tak
4. **Zgłoś problem** - ze zrzutem ekranu błędu

---

**Powodzenia!** 🚀

*Czas wdrożenia: ~15-20 minut*  
*Poziom trudności: Średni*  
*Wymagane uprawnienia: Administrator bazy danych*
