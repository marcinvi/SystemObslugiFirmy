# ANALIZA MagazynControl i Konspekt Aplikacji Android

**Data utworzenia:** 2026-01-22  
**Wersja:** 1.0  
**Autor analizy:** System automatyczny

---

## 1. ANALIZA MagazynControl

### 1.1 Główne Funkcje MagazynControl.cs

#### **Funkcje inicjalizacyjne:**
1. **MagazynControl(string fullName, string userRole)** - Konstruktor inicjalizujący kontrolkę
2. **MagazynControl_Load()** - Ładowanie danych przy starcie
3. **InitializeUiElements()** - Inicjalizacja elementów UI
4. **AttachEventHandlers()** - Podpinanie obsługi zdarzeń

#### **Funkcje zarządzania danymi:**
5. **LoadDataAsync()** - Główna funkcja ładująca wszystkie dane
6. **LoadAllegroAccountsAsync()** - Ładowanie kont Allegro
7. **LoadReturnsFromDbAsync()** - Ładowanie zwrotów z bazy danych
8. **LoadDziennikAsync()** - Ładowanie dziennika zmian
9. **UpdateFilterCountsAsync()** - Aktualizacja liczników filtrów

#### **Funkcje synchronizacji z Allegro:**
10. **btnFetchReturns_Click()** - Synchronizacja zwrotów z API Allegro
11. **SaveReturnsToDbAsync()** - Zapisywanie zwrotów do bazy danych
12. **GetOrderDetailsByCheckoutFormIdAsync()** - Pobieranie szczegółów zamówienia
13. **GetInvoicesForOrderAsync()** - Pobieranie faktur

#### **Funkcje filtrowania:**
14. **FilterButton_Click()** - Obsługa kliknięć w przyciski filtrów
15. **SetActiveFilterButton()** - Ustawianie aktywnego filtru
16. **comboFilterStatusAllegro_SelectedIndexChanged()** - Filtrowanie po statusie Allegro

#### **Funkcje skanera:**
17. **TxtScannerInput_KeyDown()** - Obsługa skanera kodów kreskowych
18. **ExtractCoreWaybill()** - Wyciąganie numeru listu z zeskanowanego kodu
19. **FindRowInGrid()** - Wyszukiwanie wiersza w siatce danych

#### **Funkcje nawigacji:**
20. **OpenReturnForm()** - Otwieranie formularza szczegółów zwrotu
21. **dgvReturns_CellDoubleClick()** - Otwieranie szczegółów po dwukliknięciu

#### **Funkcje dodawania zwrotów:**
22. **btnDodajRecznie_Click()** - Dodawanie zwrotu ręcznie

#### **Funkcje formatowania:**
23. **FormatReturnsGrid()** - Formatowanie siatki zwrotów
24. **dgvReturns_DataBindingComplete()** - Kolorowanie wierszy po statusie
25. **TranslateStatus()** - Tłumaczenie statusów EN -> PL
26. **TranslateStatusToApi()** - Tłumaczenie statusów PL -> EN

#### **Funkcje logowania:**
27. **LogToDziennikAsync()** - Logowanie akcji do dziennika

#### **Funkcje automatyczne:**
28. **SyncTimer_Tick()** - Automatyczna synchronizacja co 60 sekund
29. **OnZwrotyChanged()** - Obsługa zdarzenia zmiany zwrotów

### 1.2 Funkcje MagazynControl.SendDecisionEmails.cs

#### **Funkcje wysyłki maili:**
30. **btnWyslijZwrotyMail_Click()** - Obsługa przycisku wysyłki maili
31. **SendDecisionEmailsAsync()** - Główna funkcja wysyłki
32. **EnsureDefaultSenderEmailAsync()** - Weryfikacja konta nadawcy
33. **LoadPendingReturnsGroupedBySalesmanAsync()** - Grupowanie zwrotów po handlowcach
34. **BuildHtmlForSalesmanAsync()** - Budowanie HTML maila
35. **SendOutlookMailAsync()** - Wysyłanie maila przez Outlook
36. **GetRecipientsForManualReturnAsync()** - Pobieranie odbiorców dla zwrotu ręcznego
37. **GetOpiekunIdForAccountAsync()** - Pobieranie opiekuna konta
38. **GetActiveDelegateIdAsync()** - Pobieranie aktywnego zastępcy
39. **ResolveSalesmanAsync()** - Rozwiązywanie danych handlowca
40. **GetOutlookAccounts()** - Pobieranie kont z Outlooka
41. **PromptOutlookAccount()** - Dialog wyboru konta
42. **FindOutlookAccount()** - Wyszukiwanie konta w Outlooku
43. **AddMagazynLogEntryAsync()** - Dodawanie wpisu do dziennika magazynu

---

## 2. FORMY POWIĄZANE Z MagazynControl

### 2.1 FormProgress
**Lokalizacja:** `FormProgress.cs`  
**Przeznaczenie:** Wyświetlanie paska postępu podczas długich operacji

**Funkcje:**
1. **UpdateProgress(int current, int total, string message)** - Aktualizacja postępu
2. **EnableSpellCheckOnAllTextBoxes()** - Sprawdzanie pisowni (funkcja pomocnicza)

**Komponenty UI:**
- ProgressBar (progressBar)
- Label statusu (lblStatus)

---

### 2.2 FormZwrotSzczegoly
**Lokalizacja:** `FormZwrotSzczegoly.cs`  
**Przeznaczenie:** Szczegóły zwrotu dla pracownika magazynu

**Funkcje:**
1. **FormZwrotSzczegoly(int returnDbId, string fullName)** - Konstruktor
2. **FormZwrotSzczegoly_Load()** - Ładowanie danych
3. **LoadReturnDataAsync()** - Ładowanie danych zwrotu
4. **LoadStatusesAsync()** - Ładowanie statusów produktu
5. **PopulateControlsAsync()** - Wypełnianie kontrolek danymi
6. **btnShowOtherAddresses_Click()** - Wyświetlanie innych adresów
7. **btnZapisz_Click()** - Zapisywanie zmian
8. **btnPrzekazDoHandlowca_Click()** - Przekazywanie do handlowca
9. **btnAnuluj_Click()** - Anulowanie
10. **LogToDziennikAsync()** - Logowanie do dziennika
11. **ResolveUwagiMagazynuColumnAsync()** - Rozwiązywanie nazwy kolumny uwag

**Komponenty UI (zakładki):**
- **Zakładka 1 - Zwracany produkt:**
  - lblProductName, lblOfferId, lblQuantity, lblReason
- **Zakładka 2 - Przesyłka:**
  - lblWaybill, lblCarrier
- **Zakładka 3 - Dane kupującego:**
  - lblBuyerName, lblBuyerAddress, lblBuyerPhone, btnShowOtherAddresses
- **Zakładka 4 - Panel Magazynu:**
  - comboStanProduktu, txtUwagiMagazynu, btnZapisz, btnPrzekazDoHandlowca

---

### 2.3 FormPodsumowanieZwrotu
**Lokalizacja:** `FormPodsumowanieZwrotu.cs`  
**Przeznaczenie:** Podsumowanie zakończonego zwrotu (tylko do odczytu)

**Funkcje:**
1. **FormPodsumowanieZwrotu(int returnDbId)** - Konstruktor
2. **FormPodsumowanieZwrotu_Load()** - Ładowanie danych
3. **LoadReturnDataAsync()** - Ładowanie danych zwrotu
4. **PopulateControls()** - Wypełnianie kontrolek
5. **btnArchiwizuj_Click()** - Archiwizacja zwrotu
6. **FormatDateTime()** - Formatowanie dat

**Komponenty UI:**
- lblTitle (tytuł)
- **Sekcja "Info ogólne":** lblAllegroAccount, lblBuyerLogin, lblOrderDate, lblInvoice
- **Sekcja "Ocena magazynu":** lblStanProduktu, lblPrzyjetyPrzez, lblUwagiMagazynu, lblDataPrzyjecia
- **Sekcja "Decyzja handlowca":** lblDecyzjaHandlowca, lblKomentarzHandlowca
- **Przyciski:** btnArchiwizuj, btnPrzekazanoDoReklamacji

---

### 2.4 FormDodajZwrotReczny
**Lokalizacja:** `FormDodajZwrotReczny.cs`  
**Przeznaczenie:** Dodawanie zwrotu ręcznego (bez API Allegro)

**Funkcje:**
1. **FormDodajZwrotReczny()** - Konstruktor
2. **FormDodajZwrotReczny_Load()** - Ładowanie danych
3. **LoadHandlowcyAsync()** - Ładowanie listy handlowców
4. **LoadStanProduktuAsync()** - Ładowanie statusów produktu
5. **LoadProductSuggestionsAsync()** - Ładowanie podpowiedzi produktów
6. **LoadCarrierSuggestionsAsync()** - Ładowanie podpowiedzi przewoźników
7. **GenerateNewReferenceNumber()** - Generowanie numeru zwrotu (R/XXX/MM/YY)
8. **btnZapisz_Click()** - Zapisywanie nowego zwrotu
9. **chkWszyscyHandlowcy_CheckedChanged()** - Zaznaczanie wszystkich handlowców
10. **ResolveUwagiMagazynuColumnAsync()** - Rozwiązywanie nazwy kolumny

**Komponenty UI:**
- **Dane nadawcy:** txtImieNazwisko, txtUlica, txtKodPocztowy, txtMiasto, txtTelefon
- **Dane zwrotu:** txtNumerListu, comboPrzewoznik, comboProdukt
- **Stan produktu:** comboStanProduktu, txtUwagi
- **Odbiorcy:** checkedListBoxHandlowcy, chkWszyscyHandlowcy
- **Przyciski:** btnZapisz, btnAnuluj

---

### 2.5 KomunikatorControl
**Lokalizacja:** `KomunikatorControl.cs`  
**Przeznaczenie:** System wiadomości wewnętrznych

**Funkcje:**
1. **KomunikatorControl(params)** - Konstruktor
2. **KomunikatorControl_Load()** - Ładowanie przy starcie
3. **LoadMessagesAsync()** - Ładowanie wiadomości
4. **MessageBubble_DoubleClick()** - Otwieranie szczegółów po dwukliknięciu
5. **MessageBubble_ReplyClicked()** - Obsługa odpowiedzi
6. **MarkAsRead()** - Oznaczanie jako przeczytane
7. **btnNowaWiadomosc_Click()** - Tworzenie nowej wiadomości

**Komponenty UI:**
- flowLayoutPanelMessages (lista wiadomości)
- btnNowaWiadomosc
- MessageBubbleControl (niestandardowa kontrolka pojedynczej wiadomości)

---

## 3. STRUKTURA BAZY DANYCH

### 3.1 Baza główna (Baza.db / MySQL)

**Tabela: Uzytkownicy**
```sql
- Id (INT, PRIMARY KEY)
- Nazwa Wyświetlana (VARCHAR)
- Email (VARCHAR)
- Rola (VARCHAR) // 'Magazyn', 'Handlowiec', 'Admin'
- IsActive (BOOLEAN)
```

**Tabela: AllegroAccounts**
```sql
- Id (INT, PRIMARY KEY)
- AccountName (VARCHAR)
- AccessTokenEncrypted (TEXT)
- RefreshTokenEncrypted (TEXT)
- TokenExpirationDate (DATETIME)
- IsAuthorized (BOOLEAN)
```

**Tabela: Ustawienia**
```sql
- Klucz (VARCHAR, PRIMARY KEY)
- WartoscZaszyfrowana (TEXT)
```

### 3.2 Baza magazynowa (Magazyn.db / MySQL)

**Tabela: AllegroCustomerReturns** (główna tabela zwrotów)
```sql
- Id (INT, PRIMARY KEY, AUTO_INCREMENT)
- AllegroReturnId (VARCHAR) // ID z API Allegro
- AllegroAccountId (INT, FOREIGN KEY)
- ReferenceNumber (VARCHAR, UNIQUE) // Numer zwrotu
- OrderId (VARCHAR) // ID zamówienia
- BuyerLogin (VARCHAR)
- BuyerFullName (VARCHAR)
- CreatedAt (DATETIME)
- StatusAllegro (VARCHAR) // 'DELIVERED', 'IN_TRANSIT', etc.
- Waybill (VARCHAR) // Numer listu przewozowego
- CarrierName (VARCHAR)
- JsonDetails (TEXT) // Pełne dane JSON z API
- InvoiceNumber (VARCHAR)
- ProductName (VARCHAR)
- OfferId (VARCHAR)
- Quantity (INT)
- PaymentType (VARCHAR)
- FulfillmentStatus (VARCHAR)

// Adres dostawy
- Delivery_FirstName (VARCHAR)
- Delivery_LastName (VARCHAR)
- Delivery_Street (VARCHAR)
- Delivery_ZipCode (VARCHAR)
- Delivery_City (VARCHAR)
- Delivery_PhoneNumber (VARCHAR)

// Adres kupującego
- Buyer_FirstName (VARCHAR)
- Buyer_LastName (VARCHAR)
- Buyer_Street (VARCHAR)
- Buyer_ZipCode (VARCHAR)
- Buyer_City (VARCHAR)
- Buyer_PhoneNumber (VARCHAR)

// Adres fakturowy
- Invoice_CompanyName (VARCHAR)
- Invoice_TaxId (VARCHAR)
- Invoice_Street (VARCHAR)
- Invoice_ZipCode (VARCHAR)
- Invoice_City (VARCHAR)

// Status i decyzje
- StatusWewnetrznyId (INT, FOREIGN KEY -> Statusy)
- StanProduktuId (INT, FOREIGN KEY -> Statusy)
- DecyzjaHandlowcaId (INT, FOREIGN KEY -> Statusy)
- UwagiMagazynu (TEXT)
- KomentarzHandlowca (TEXT)
- PrzyjetyPrzezId (INT, FOREIGN KEY -> Uzytkownicy)
- DataPrzyjecia (DATETIME)

// Zwroty ręczne
- IsManual (BOOLEAN)
- ManualSenderDetails (TEXT) // JSON z danymi nadawcy
```

**Tabela: Statusy**
```sql
- Id (INT, PRIMARY KEY)
- Nazwa (VARCHAR)
- TypStatusu (VARCHAR) // 'StatusWewnetrzny', 'StanProduktu', 'DecyzjaHandlowca'
- Kolor (VARCHAR) // dla UI
```

**Przykładowe statusy:**
- StatusWewnetrzny:
  - "Oczekuje na przyjęcie"
  - "Oczekuje na decyzję handlowca"
  - "Zakończony"
  - "Archiwalny"
  
- StanProduktu:
  - "Nowy, nieużywany"
  - "Używany, sprawny"
  - "Uszkodzony"
  - "Niekompletny"
  
- DecyzjaHandlowca:
  - "Zwrot kosztów"
  - "Wymiana"
  - "Przekaż do reklamacji"
  - "Odrzuć zwrot"

**Tabela: MagazynDziennik**
```sql
- Id (INT, PRIMARY KEY, AUTO_INCREMENT)
- Data (DATETIME)
- Uzytkownik (VARCHAR)
- Akcja (TEXT)
- DotyczyZwrotuId (INT, NULLABLE)
```

**Tabela: Wiadomosci**
```sql
- Id (INT, PRIMARY KEY, AUTO_INCREMENT)
- NadawcaId (INT, FOREIGN KEY -> Uzytkownicy)
- OdbiorcaId (INT, FOREIGN KEY -> Uzytkownicy)
- Tytul (VARCHAR, NULLABLE)
- Tresc (TEXT)
- DataWyslania (DATETIME)
- DotyczyZwrotuId (INT, NULLABLE)
- CzyPrzeczytana (BOOLEAN, DEFAULT 0)
- CzyOdpowiedziano (BOOLEAN, DEFAULT 0)
```

**Tabela: ZwrotDzialania** (historia działań dla zwrotu)
```sql
- Id (INT, PRIMARY KEY, AUTO_INCREMENT)
- ZwrotId (INT, FOREIGN KEY -> AllegroCustomerReturns)
- Data (DATETIME)
- Uzytkownik (VARCHAR)
- Tresc (TEXT)
```

**Tabela: AllegroAccountOpiekun** (przypisanie handlowców do kont)
```sql
- AllegroAccountId (INT, FOREIGN KEY)
- OpiekunId (INT, FOREIGN KEY -> Uzytkownicy)
- PRIMARY KEY (AllegroAccountId, OpiekunId)
```

**Tabela: Delegacje** (zastępstwa handlowców)
```sql
- Id (INT, PRIMARY KEY)
- UzytkownikId (INT, FOREIGN KEY -> Uzytkownicy)
- ZastepcaId (INT, FOREIGN KEY -> Uzytkownicy)
- DataOd (DATE)
- DataDo (DATE)
- CzyAktywna (BOOLEAN, DEFAULT 1)
```

---

## 4. PRZEPŁYW DANYCH W SYSTEMIE

### 4.1 Synchronizacja zwrotów z Allegro

```
1. Użytkownik klika "Synchronizuj"
2. System pobiera listę kont Allegro z bazy głównej
3. Dla każdego konta:
   a. Tworzy klienta API z tokenami dostępowymi
   b. Pobiera zwroty z ostatnich 60 dni (API: /sale/customer-returns)
   c. Dla każdego zwrotu pobiera:
      - Szczegóły zamówienia (API: /order/checkout-forms/{id})
      - Fakturę (API: /billing/invoices)
   d. Zapisuje/aktualizuje dane w tabeli AllegroCustomerReturns
4. Wyświetla pasek postępu
5. Loguje akcję do dziennika
6. Odświeża widok
```

### 4.2 Przyjmowanie zwrotu przez magazyn

```
1. Pracownik skanuje kod kreskowy z listu przewozowego
2. System wyciąga numer z kodu (obsługa formatów DPD i innych)
3. Wyszukuje zwrot w bazie po numerze listu lub numerze zwrotu
4. Jeśli nie znaleziono - oferuje dodanie ręczne
5. Jeśli znaleziono - otwiera FormZwrotSzczegoly
6. Pracownik:
   a. Wybiera stan produktu z listy
   b. Wpisuje uwagi
   c. Klika "Zapisz" lub "Przekaż do handlowca"
7. System:
   a. Zapisuje dane (stan, uwagi, pracownik, data)
   b. Jeśli "Przekaż do handlowca":
      - Zmienia status na "Oczekuje na decyzję handlowca"
      - Sprawdza opiekuna konta (tabela AllegroAccountOpiekun)
      - Sprawdza delegacje (tabela Delegacje)
      - Wysyła wiadomość do odpowiedniego handlowca
      - Loguje akcję
```

### 4.3 Decyzja handlowca (poza MagazynControl)

```
1. Handlowiec otwiera wiadomość w KomunikatorControl
2. System otwiera FormHandlowiecSzczegoly
3. Handlowiec:
   a. Przegląda dane zwrotu
   b. Wybiera decyzję (zwrot kosztów, wymiana, reklamacja, odrzucenie)
   c. Wpisuje komentarz
   d. Zapisuje
4. System:
   a. Zmienia status na "Zakończony"
   b. Zapisuje decyzję
   c. Loguje akcję
```

### 4.4 Wysyłka powiadomień mailowych

```
1. Użytkownik klika "Wyślij zwroty do decyzji"
2. System:
   a. Sprawdza domyślne konto nadawcy w ustawieniach
   b. Weryfikuje czy konto istnieje w Outlook
   c. Pobiera zwroty ze statusem "Oczekuje na decyzję handlowca"
   d. Grupuje po handlowcach (uwzględniając delegacje)
   e. Dla każdego handlowca:
      - Buduje HTML z tabelą zwrotów
      - Wysyła maila przez Outlook API
   f. Loguje akcję
```

---

## 5. KONSPEKT APLIKACJI ANDROID

### 5.1 Architektura aplikacji

**Wzorzec:** MVVM (Model-View-ViewModel)  
**Język:** Kotlin  
**Minimalna wersja Android:** API 24 (Android 7.0)

### 5.2 Technologie

#### **Biblioteki podstawowe:**
- **Jetpack Compose** - UI (zamiast XML)
- **Room Database** - Lokalna baza danych (cache)
- **Retrofit** - Komunikacja z API
- **Hilt** - Dependency Injection
- **Kotlin Coroutines + Flow** - Asynchroniczność
- **Navigation Component** - Nawigacja

#### **Biblioteki dodatkowe:**
- **ZXing** - Skanowanie kodów kreskowych
- **DataStore** - Przechowywanie preferencji
- **WorkManager** - Synchronizacja w tle
- **OkHttp** - Interceptory dla API
- **Coil** - Ładowanie obrazów (jeśli potrzebne)

### 5.3 Struktura projektu

```
app/
├── data/
│   ├── local/
│   │   ├── dao/
│   │   │   ├── ReturnDao.kt
│   │   │   ├── MessageDao.kt
│   │   │   ├── UserDao.kt
│   │   │   └── StatusDao.kt
│   │   ├── entities/
│   │   │   ├── ReturnEntity.kt
│   │   │   ├── MessageEntity.kt
│   │   │   ├── UserEntity.kt
│   │   │   └── StatusEntity.kt
│   │   └── AppDatabase.kt
│   ├── remote/
│   │   ├── api/
│   │   │   ├── AllegroApiService.kt
│   │   │   ├── MagazynApiService.kt
│   │   │   └── AuthInterceptor.kt
│   │   └── dto/
│   │       ├── ReturnDto.kt
│   │       ├── OrderDetailsDto.kt
│   │       └── InvoiceDto.kt
│   └── repository/
│       ├── ReturnRepository.kt
│       ├── MessageRepository.kt
│       └── UserRepository.kt
│
├── domain/
│   ├── model/
│   │   ├── Return.kt
│   │   ├── Message.kt
│   │   └── User.kt
│   └── usecase/
│       ├── GetReturnsUseCase.kt
│       ├── ScanBarcodeUseCase.kt
│       ├── ProcessReturnUseCase.kt
│       └── SendToSalesmanUseCase.kt
│
├── presentation/
│   ├── main/
│   │   ├── MainActivity.kt
│   │   └── MainViewModel.kt
│   ├── returns_list/
│   │   ├── ReturnsListScreen.kt
│   │   ├── ReturnsListViewModel.kt
│   │   └── components/
│   │       ├── ReturnCard.kt
│   │       └── FilterChips.kt
│   ├── return_details/
│   │   ├── ReturnDetailsScreen.kt
│   │   ├── ReturnDetailsViewModel.kt
│   │   └── components/
│   │       ├── ProductInfoSection.kt
│   │       ├── ShippingInfoSection.kt
│   │       └── BuyerInfoSection.kt
│   ├── scanner/
│   │   ├── ScannerScreen.kt
│   │   └── ScannerViewModel.kt
│   ├── add_manual/
│   │   ├── AddManualReturnScreen.kt
│   │   └── AddManualViewModel.kt
│   └── messages/
│       ├── MessagesScreen.kt
│       └── MessagesViewModel.kt
│
├── di/
│   ├── AppModule.kt
│   ├── DatabaseModule.kt
│   ├── NetworkModule.kt
│   └── RepositoryModule.kt
│
└── util/
    ├── BarcodeParser.kt
    ├── DateFormatter.kt
    └── StatusTranslator.kt
```

### 5.4 Ekrany aplikacji

#### **5.4.1 Ekran główny - Lista zwrotów**

**Komponenty:**
1. **TopAppBar:**
   - Tytuł: "Magazyn - Zwroty"
   - Ikona odświeżania
   - Ikona ustawień
   - Menu z opcjami synchronizacji

2. **Filtry (ChipGroup):**
   - "Oczekuje na przyjęcie (X)"
   - "Oczekuje na decyzję (X)"
   - "Po decyzji (X)"
   - "W drodze do nas (X)"
   - "Wszystkie (X)"

3. **Status Allegro (Dropdown):**
   - Wszystkie
   - Dostarczono
   - W drodze
   - Gotowy do odbioru
   - Utworzono

4. **Pole wyszukiwania:**
   - Hint: "Szukaj po numerze, nazwisku, produkcie..."
   - Ikona wyszukiwania

5. **FloatingActionButton:**
   - Ikona: "+" lub kod kreskowy
   - Akcja: Otwarcie skanera lub menu (dodaj ręcznie / skanuj)

6. **Lista zwrotów (LazyColumn):**
   - Card dla każdego zwrotu:
     ```
     [Kod koloru statusu]
     ReferenceNumber | StatusAllegro
     Kupujący: [Nazwa]
     Produkt: [Nazwa produktu]
     List: [Waybill]
     Data: [CreatedAt]
     Status wewnętrzny: [badge]
     ```
   - Kliknięcie -> Szczegóły
   - Long press -> Menu kontekstowe

7. **Bottom Navigation:**
   - Zwroty (aktywny)
   - Wiadomości
   - Dziennik
   - Profil

#### **5.4.2 Ekran szczegółów zwrotu**

**Layout - Zakładki (TabRow):**

**Zakładka 1: Produkt**
```
┌────────────────────────────────┐
│ Zwracany produkt              │
├────────────────────────────────┤
│ Nazwa: [ProductName]          │
│ Offer ID: [OfferId]           │
│ Ilość: [Quantity]             │
│ Powód zwrotu: [Reason]        │
└────────────────────────────────┘
```

**Zakładka 2: Przesyłka**
```
┌────────────────────────────────┐
│ Informacje o przesyłce        │
├────────────────────────────────┤
│ Numer listu: [Waybill]        │
│ Przewoźnik: [CarrierName]     │
│ Status Allegro: [Status]      │
└────────────────────────────────┘
```

**Zakładka 3: Kupujący**
```
┌────────────────────────────────┐
│ Dane kupującego               │
├────────────────────────────────┤
│ Imię i nazwisko: [Name]       │
│ Adres: [Street, Zip, City]    │
│ Telefon: [Phone]              │
│ [Przycisk: Pokaż inne adresy] │
└────────────────────────────────┘
```

**Zakładka 4: Magazyn**
```
┌────────────────────────────────┐
│ Ocena magazynu                │
├────────────────────────────────┤
│ Stan produktu:                │
│ [Dropdown: Nowy/Używany/etc.] │
│                               │
│ Uwagi magazynu:               │
│ [TextField - multiline]       │
│                               │
│ [Przycisk: Zapisz]            │
│ [Przycisk: Przekaż handlowcowi]│
└────────────────────────────────┘
```

**Bottom Bar:**
- Przycisk "Wstecz"
- Przycisk "Historia" (pokazuje ZwrotDzialania)

#### **5.4.3 Ekran skanera**

**Layout:**
```
┌─────────────────────────────────┐
│        [Camera Preview]         │
│                                 │
│     [Ramka skanowania]          │
│                                 │
│  "Zeskanuj kod kreskowy listu"  │
│                                 │
│  [Ikona latarki]                │
└─────────────────────────────────┘
```

**Funkcjonalność:**
1. Włącza aparat
2. Skanuje kody kreskowe (1D i 2D)
3. Parsuje numer listu (obsługa DPD i innych)
4. Wyszukuje w bazie
5. Jeśli znaleziono -> otwiera szczegóły
6. Jeśli nie -> dialog z opcją dodania ręcznego

#### **5.4.4 Ekran dodawania zwrotu ręcznego**

**Sekcje:**

**1. Dane nadawcy**
```
[TextField] Imię i nazwisko *
[TextField] Ulica
[TextField] Kod pocztowy
[TextField] Miasto
[TextField] Telefon
```

**2. Dane zwrotu**
```
[TextField] Numer listu przewozowego *
[AutoComplete] Przewoźnik
[AutoComplete] Produkt
```

**3. Stan produktu**
```
[Dropdown] Stan produktu *
[TextField] Uwagi (multiline)
```

**4. Odbiorcy (handlowcy)**
```
[Checkbox] Wszyscy handlowcy
[List z checkboxami] Lista handlowców
```

**Bottom Bar:**
```
[Przycisk: Anuluj] [Przycisk: Zapisz i przekaż]
```

#### **5.4.5 Ekran wiadomości**

**Layout:**
```
┌─────────────────────────────────┐
│ TopAppBar: "Wiadomości"         │
├─────────────────────────────────┤
│ [FloatingActionButton: +]       │
│                                 │
│ [Lista wiadomości - Cards]      │
│ ┌─────────────────────────────┐ │
│ │ [Avatar] Nadawca            │ │
│ │ Tytuł wiadomości            │ │
│ │ Fragment treści...          │ │
│ │ Data | [Badge: nieprzecz.] │ │
│ └─────────────────────────────┘ │
│                                 │
└─────────────────────────────────┘
```

**Funkcjonalność:**
- Tap -> Otwiera szczegóły wiadomości + opcja odpowiedzi
- Long press -> Menu (oznacz jako przeczytaną, usuń)
- Badge przy nieprzeczytanych

#### **5.4.6 Ekran podsumowania zwrotu**

**Layout (tylko odczyt):**
```
┌────────────────────────────────┐
│ TopAppBar: "Podsumowanie"      │
├────────────────────────────────┤
│ === Info ogólne ===            │
│ Konto Allegro: [Name]          │
│ Kupujący: [Login]              │
│ Data zamówienia: [Date]        │
│ Faktura: [Number]              │
│                                │
│ === Ocena magazynu ===         │
│ Stan: [Stan]                   │
│ Przyjął: [Pracownik]           │
│ Data: [Data]                   │
│ Uwagi: [Uwagi]                 │
│                                │
│ === Decyzja handlowca ===      │
│ Decyzja: [Decyzja]             │
│ Komentarz: [Komentarz]         │
│                                │
│ [Przycisk: Archiwizuj]         │
└────────────────────────────────┘
```

#### **5.4.7 Ekran dziennika**

**Layout:**
```
┌────────────────────────────────┐
│ TopAppBar: "Dziennik zmian"    │
├────────────────────────────────┤
│ [Lista wpisów - LazyColumn]    │
│ ┌────────────────────────────┐ │
│ │ [Ikona] Użytkownik         │ │
│ │ Akcja...                   │ │
│ │ Data | Nr zwrotu           │ │
│ └────────────────────────────┘ │
│                                │
└────────────────────────────────┘
```

---

### 5.5 Przepływ danych - szczegóły implementacji

#### **5.5.1 Synchronizacja zwrotów**

**WorkManager - PeriodicWorkRequest:**
```kotlin
class SyncReturnsWorker @AssistedInject constructor(
    @Assisted appContext: Context,
    @Assisted workerParams: WorkerParameters,
    private val repository: ReturnRepository
) : CoroutineWorker(appContext, workerParams) {

    override suspend fun doWork(): Result {
        return try {
            repository.syncReturnsFromApi()
            Result.success()
        } catch (e: Exception) {
            Result.retry()
        }
    }
}

// Harmonogram: co 60 minut
val syncRequest = PeriodicWorkRequestBuilder<SyncReturnsWorker>(60, TimeUnit.MINUTES)
    .setConstraints(
        Constraints.Builder()
            .setRequiredNetworkType(NetworkType.CONNECTED)
            .build()
    )
    .build()
```

**ReturnRepository.syncReturnsFromApi():**
```kotlin
suspend fun syncReturnsFromApi() {
    // 1. Pobierz konta Allegro
    val accounts = allegroApiService.getAuthorizedAccounts()
    
    // 2. Dla każdego konta
    accounts.forEach { account ->
        // 3. Pobierz zwroty z ostatnich 60 dni
        val returns = allegroApiService.getCustomerReturns(
            accountId = account.id,
            dateFrom = LocalDateTime.now().minusDays(60)
        )
        
        // 4. Dla każdego zwrotu pobierz szczegóły
        returns.forEach { return ->
            val orderDetails = allegroApiService.getOrderDetails(return.orderId)
            val invoice = allegroApiService.getInvoice(return.orderId)
            
            // 5. Zapisz/zaktualizuj w lokalnej bazie
            returnDao.upsert(
                return.toEntity(
                    accountId = account.id,
                    orderDetails = orderDetails,
                    invoice = invoice
                )
            )
        }
    }
    
    // 6. Zaloguj akcję
    logAction("Zsynchronizowano ${returns.size} zwrotów")
}
```

#### **5.5.2 Skanowanie kodu**

**ScannerViewModel:**
```kotlin
fun processBarcodeResult(barcode: String) {
    viewModelScope.launch {
        // 1. Wyciągnij numer listu
        val waybill = BarcodeParser.extractWaybill(barcode)
        
        // 2. Szukaj w bazie
        val return = returnRepository.findByWaybill(waybill)
        
        if (return != null) {
            // 3a. Znaleziono - otwórz szczegóły
            _navigationEvent.emit(
                NavigationEvent.OpenReturnDetails(return.id)
            )
        } else {
            // 3b. Nie znaleziono - zapytaj użytkownika
            _dialogEvent.emit(
                DialogEvent.NotFound(
                    waybill = waybill,
                    onAddManual = { openAddManualScreen(waybill) }
                )
            )
        }
    }
}
```

**BarcodeParser.extractWaybill():**
```kotlin
object BarcodeParser {
    // Wzorce dla różnych przewoźników
    private val DPD_PATTERN = Regex("^%.{7}([a-zA-Z0-9]{14})")
    private val GENERIC_PATTERN = Regex("[a-zA-Z0-9]{10,}")
    
    fun extractWaybill(barcode: String): String {
        // DPD: % + 7 znaków + 14-znakowy numer
        DPD_PATTERN.find(barcode)?.let { match ->
            return match.groupValues[1]
        }
        
        // Ogólny: długi ciąg alfanumeryczny
        GENERIC_PATTERN.find(barcode)?.let { match ->
            return match.value
        }
        
        // Fallback: usuń wszystkie znaki specjalne
        return barcode.replace(Regex("[^a-zA-Z0-9]"), "")
    }
}
```

#### **5.5.3 Przekazywanie do handlowca**

**ProcessReturnUseCase:**
```kotlin
suspend fun processAndSendToSalesman(
    returnId: Int,
    productStatus: String,
    notes: String
): Result<Unit> {
    return try {
        // 1. Pobierz dane zwrotu
        val return = returnRepository.getById(returnId)
        
        // 2. Zaktualizuj status i dane
        returnRepository.update(
            returnId = returnId,
            productStatus = productStatus,
            notes = notes,
            processedBy = sessionManager.currentUserId,
            processedAt = LocalDateTime.now(),
            internalStatus = "Oczekuje na decyzję handlowca"
        )
        
        // 3. Znajdź odpowiedniego handlowca
        val salesman = if (return.isManual) {
            // Zwrot ręczny - odbiorcy z wiadomości
            messageRepository.getRecipients(returnId)
        } else {
            // Zwrot Allegro - opiekun konta
            val accountCaretaker = userRepository.getAccountCaretaker(return.accountId)
            
            // Sprawdź delegacje
            userRepository.getActiveDelegate(accountCaretaker.id)
                ?: accountCaretaker
        }
        
        // 4. Wyślij wiadomość
        messageRepository.send(
            from = sessionManager.currentUserId,
            to = salesman.id,
            title = "Prośba o decyzję dla zwrotu ${return.referenceNumber}",
            content = "Zwrot od ${return.buyerName}",
            returnId = returnId
        )
        
        // 5. Zaloguj akcję
        actionRepository.log(
            returnId = returnId,
            user = sessionManager.currentUserName,
            action = "Przekazano do ${salesman.displayName}. Stan: $productStatus"
        )
        
        Result.success(Unit)
    } catch (e: Exception) {
        Result.failure(e)
    }
}
```

---

### 5.6 Synchronizacja offline-first

**Strategia:**
1. **Lokalna baza Room** - źródło prawdy dla UI
2. **API jako backup** - tylko do synchronizacji
3. **Conflict resolution** - last-write-wins (timestamp)

**Flow danych:**
```
UI <- ViewModel <- Repository <- [Local DB] -> Repository <- [Remote API]
                                      ↑                            ↓
                                      └────────── sync ─────────────┘
```

**Implementacja w Repository:**
```kotlin
class ReturnRepository @Inject constructor(
    private val returnDao: ReturnDao,
    private val apiService: MagazynApiService,
    private val networkMonitor: NetworkMonitor
) {
    // UI zawsze czyta z lokalnej bazy
    fun getReturns(): Flow<List<Return>> {
        return returnDao.getAllFlow()
            .map { entities -> entities.map { it.toDomain() } }
    }
    
    // Zapis lokalny + synchronizacja w tle
    suspend fun updateReturn(update: ReturnUpdate) {
        // 1. Zapisz lokalnie (natychmiastowo)
        returnDao.update(update.toEntity())
        
        // 2. Oznacz jako "do synchronizacji"
        syncQueueDao.enqueue(
            SyncItem(
                type = SyncType.UPDATE_RETURN,
                data = update.toJson(),
                timestamp = System.currentTimeMillis()
            )
        )
        
        // 3. Jeśli jest internet, synchronizuj od razu
        if (networkMonitor.isOnline()) {
            trySyncNow()
        }
        // Jeśli nie ma - WorkManager zrobi to później
    }
    
    private suspend fun trySyncNow() {
        val queue = syncQueueDao.getAll()
        queue.forEach { item ->
            try {
                when (item.type) {
                    SyncType.UPDATE_RETURN -> {
                        val update = item.data.fromJson<ReturnUpdate>()
                        apiService.updateReturn(update)
                        syncQueueDao.delete(item)
                    }
                    // ... inne typy
                }
            } catch (e: Exception) {
                // Zostaw w kolejce, spróbuje później
            }
        }
    }
}
```

---

### 5.7 Bezpieczeństwo

#### **5.7.1 Autoryzacja**

**Token storage:**
```kotlin
// DataStore (szyfrowane)
class SecurePreferences @Inject constructor(
    private val context: Context
) {
    private val encryptedPrefs = EncryptedSharedPreferences.create(
        context,
        "secure_prefs",
        MasterKey.Builder(context)
            .setKeyScheme(MasterKey.KeyScheme.AES256_GCM)
            .build(),
        EncryptedSharedPreferences.PrefKeyEncryptionScheme.AES256_SIV,
        EncryptedSharedPreferences.PrefValueEncryptionScheme.AES256_GCM
    )
    
    suspend fun saveAuthToken(token: String) {
        encryptedPrefs.edit()
            .putString("auth_token", token)
            .apply()
    }
    
    suspend fun getAuthToken(): String? {
        return encryptedPrefs.getString("auth_token", null)
    }
}
```

**AuthInterceptor:**
```kotlin
class AuthInterceptor @Inject constructor(
    private val securePrefs: SecurePreferences
) : Interceptor {
    override fun intercept(chain: Interceptor.Chain): Response {
        val token = runBlocking { securePrefs.getAuthToken() }
        
        val request = chain.request().newBuilder()
            .addHeader("Authorization", "Bearer $token")
            .build()
            
        return chain.proceed(request)
    }
}
```

#### **5.7.2 Uprawnienia**

**AndroidManifest.xml:**
```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<!-- Opcjonalnie dla wibracji przy skanowaniu -->
<uses-permission android:name="android.permission.VIBRATE" />
```

**Runtime permissions (Camera):**
```kotlin
// W ScannerScreen
val cameraPermission = rememberPermissionState(Manifest.permission.CAMERA)

LaunchedEffect(Unit) {
    if (!cameraPermission.status.isGranted) {
        cameraPermission.launchPermissionRequest()
    }
}
```

---

### 5.8 Testy

#### **5.8.1 Unit testy**

```kotlin
// BarcodeParserTest.kt
class BarcodeParserTest {
    
    @Test
    fun `DPD barcode extracts correct waybill`() {
        val barcode = "%1234567AB12345678901234"
        val result = BarcodeParser.extractWaybill(barcode)
        assertEquals("AB12345678901234", result)
    }
    
    @Test
    fun `Generic barcode extracts alphanumeric sequence`() {
        val barcode = "XYZ-123456789ABC-END"
        val result = BarcodeParser.extractWaybill(barcode)
        assertEquals("123456789ABC", result)
    }
}

// ReturnRepositoryTest.kt
@HiltAndroidTest
class ReturnRepositoryTest {
    
    @get:Rule
    var hiltRule = HiltAndroidRule(this)
    
    @Inject
    lateinit var repository: ReturnRepository
    
    @Inject
    lateinit var database: AppDatabase
    
    @Before
    fun setup() {
        hiltRule.inject()
    }
    
    @Test
    fun `updateReturn saves locally and enqueues sync`() = runTest {
        // Given
        val returnId = 1
        val update = ReturnUpdate(
            returnId = returnId,
            productStatus = "Nowy",
            notes = "Test"
        )
        
        // When
        repository.updateReturn(update)
        
        // Then
        val saved = database.returnDao().getById(returnId)
        assertEquals("Nowy", saved.productStatus)
        
        val syncQueue = database.syncQueueDao().getAll()
        assertTrue(syncQueue.isNotEmpty())
    }
}
```

#### **5.8.2 UI testy**

```kotlin
@HiltAndroidTest
class ReturnsListScreenTest {
    
    @get:Rule(order = 0)
    var hiltRule = HiltAndroidRule(this)
    
    @get:Rule(order = 1)
    val composeRule = createAndroidComposeRule<MainActivity>()
    
    @Test
    fun `clicking return opens details screen`() {
        // Given
        composeRule.onNodeWithText("R/001/01/26").assertExists()
        
        // When
        composeRule.onNodeWithText("R/001/01/26").performClick()
        
        // Then
        composeRule.onNodeWithText("Szczegóły zwrotu").assertExists()
    }
    
    @Test
    fun `filter shows correct number of returns`() {
        // When
        composeRule.onNodeWithText("Oczekuje na przyjęcie").performClick()
        
        // Then
        composeRule.onNodeWithText("(5)").assertExists()
    }
}
```

---

### 5.9 Performance

#### **5.9.1 LazyColumn optymalizacje**

```kotlin
@Composable
fun ReturnsListScreen(
    viewModel: ReturnsListViewModel = hiltViewModel()
) {
    val returns by viewModel.returns.collectAsStateWithLifecycle()
    
    LazyColumn(
        contentPadding = PaddingValues(16.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        items(
            items = returns,
            key = { it.id } // WAŻNE: stabilny key dla optymalizacji
        ) { return ->
            ReturnCard(
                return = return,
                onClick = { viewModel.openDetails(return.id) }
            )
        }
    }
}
```

#### **5.9.2 Image loading (jeśli będą zdjęcia produktów)**

```kotlin
// Coil z cache
AsyncImage(
    model = ImageRequest.Builder(LocalContext.current)
        .data(imageUrl)
        .crossfade(true)
        .memoryCacheKey(imageUrl)
        .diskCacheKey(imageUrl)
        .build(),
    contentDescription = null,
    modifier = Modifier.size(80.dp)
)
```

#### **5.9.3 Paging (dla dużych list)**

```kotlin
// W ReturnDao
@Query("SELECT * FROM returns WHERE statusWewnetrzny = :status ORDER BY createdAt DESC")
fun getReturnsPaged(status: String): PagingSource<Int, ReturnEntity>

// W Repository
fun getReturnsPaged(status: String): Flow<PagingData<Return>> {
    return Pager(
        config = PagingConfig(pageSize = 20, enablePlaceholders = true),
        pagingSourceFactory = { returnDao.getReturnsPaged(status) }
    ).flow.map { pagingData ->
        pagingData.map { it.toDomain() }
    }
}

// W ViewModel
val returns = repository.getReturnsPaged(currentFilter)
    .cachedIn(viewModelScope)

// W UI
val lazyPagingItems = returns.collectAsLazyPagingItems()

LazyColumn {
    items(lazyPagingItems) { return ->
        if (return != null) {
            ReturnCard(return = return)
        }
    }
}
```

---

### 5.10 Wydajność skanera

#### **5.10.1 CameraX - optymalne ustawienia**

```kotlin
@Composable
fun ScannerScreen(viewModel: ScannerViewModel) {
    val context = LocalContext.current
    val lifecycleOwner = LocalLifecycleOwner.current
    
    val preview = Preview.Builder().build()
    val imageAnalysis = ImageAnalysis.Builder()
        .setTargetResolution(Size(1280, 720)) // Mniejsza rozdzielczość = szybsze
        .setBackpressureStrategy(ImageAnalysis.STRATEGY_KEEP_ONLY_LATEST)
        .build()
    
    val barcodeScanner = BarcodeScanning.getClient(
        BarcodeScannerOptions.Builder()
            .setBarcodeFormats(
                Barcode.FORMAT_CODE_128,
                Barcode.FORMAT_EAN_13,
                Barcode.FORMAT_QR_CODE
            )
            .build()
    )
    
    LaunchedEffect(Unit) {
        val cameraProvider = ProcessCameraProvider.getInstance(context).await()
        
        imageAnalysis.setAnalyzer(
            ContextCompat.getMainExecutor(context)
        ) { imageProxy ->
            val mediaImage = imageProxy.image
            if (mediaImage != null) {
                val image = InputImage.fromMediaImage(
                    mediaImage,
                    imageProxy.imageInfo.rotationDegrees
                )
                
                barcodeScanner.process(image)
                    .addOnSuccessListener { barcodes ->
                        barcodes.firstOrNull()?.rawValue?.let { barcode ->
                            viewModel.processBarcodeResult(barcode)
                        }
                    }
                    .addOnCompleteListener {
                        imageProxy.close()
                    }
            }
        }
        
        cameraProvider.bindToLifecycle(
            lifecycleOwner,
            CameraSelector.DEFAULT_BACK_CAMERA,
            preview,
            imageAnalysis
        )
    }
}
```

---

### 5.11 Monitoring i Analytics

#### **5.11.1 Crashlytics**

```kotlin
// build.gradle.kts
plugins {
    id("com.google.gms.google-services")
    id("com.google.firebase.crashlytics")
}

dependencies {
    implementation(platform("com.google.firebase:firebase-bom:32.7.0"))
    implementation("com.google.firebase:firebase-crashlytics-ktx")
    implementation("com.google.firebase:firebase-analytics-ktx")
}

// W Application class
class MagazynApp : Application() {
    override fun onCreate() {
        super.onCreate()
        
        FirebaseCrashlytics.getInstance().apply {
            setCrashlyticsCollectionEnabled(!BuildConfig.DEBUG)
        }
    }
}
```

#### **5.11.2 Custom logging**

```kotlin
fun logReturnProcessed(returnId: Int, duration: Long) {
    Firebase.analytics.logEvent("return_processed") {
        param("return_id", returnId.toString())
        param("duration_ms", duration)
    }
}
```

---

## 6. HARMONOGRAM IMPLEMENTACJI

### Faza 1: Podstawy (2 tygodnie)
- [ ] Setup projektu (Gradle, Hilt, Room, Retrofit)
- [ ] Implementacja warstwy danych (DAO, Repository)
- [ ] Ekran logowania i autoryzacji
- [ ] Ekran listy zwrotów (podstawowy)

### Faza 2: Core functionality (3 tygodnie)
- [ ] Skaner kodów kreskowych
- [ ] Ekran szczegółów zwrotu
- [ ] Aktualizacja statusów i przekazywanie do handlowca
- [ ] Synchronizacja z API Allegro

### Faza 3: Dodatkowe funkcje (2 tygodnie)
- [ ] Ekran dodawania zwrotu ręcznego
- [ ] System wiadomości
- [ ] Dziennik zmian
- [ ] Filtrowanie i wyszukiwanie

### Faza 4: Optymalizacja (1 tydzień)
- [ ] Offline-first sync
- [ ] Performance tuning
- [ ] Error handling
- [ ] Loading states

### Faza 5: Testy i deploy (1 tydzień)
- [ ] Unit testy
- [ ] UI testy
- [ ] Beta testing
- [ ] Release na Google Play

**Całkowity czas:** 9 tygodni

---

## 7. WYMAGANIA SYSTEMOWE

### 7.1 Backend API

Aplikacja Android wymaga następujących endpointów:

**Auth:**
- `POST /api/auth/login` - Logowanie użytkownika
- `POST /api/auth/refresh` - Odświeżenie tokenu

**Returns:**
- `GET /api/returns` - Lista zwrotów (z filtrowaniem)
- `GET /api/returns/{id}` - Szczegóły zwrotu
- `PUT /api/returns/{id}` - Aktualizacja zwrotu
- `POST /api/returns` - Dodanie zwrotu ręcznego
- `POST /api/returns/sync` - Synchronizacja z Allegro

**Messages:**
- `GET /api/messages` - Lista wiadomości
- `POST /api/messages` - Wysłanie wiadomości
- `PUT /api/messages/{id}/read` - Oznaczenie jako przeczytane

**Users:**
- `GET /api/users` - Lista użytkowników (dla wyboru handlowca)
- `GET /api/users/{id}/delegate` - Sprawdzenie delegacji

**Statuses:**
- `GET /api/statuses` - Lista statusów (produktu, wewnętrznych)

**Allegro:**
- `GET /api/allegro/accounts` - Lista kont Allegro

### 7.2 Serwer

- **Minimalne wymagania:**
  - MySQL 8.0+
  - PHP 8.0+ lub Node.js 18+ (w zależności od implementacji API)
  - SSL/TLS (HTTPS wymagane dla produkcji)
  - Minimum 2GB RAM, 20GB dysk

---

## 8. BEZPIECZEŃSTWO I ZGODNOŚĆ

### 8.1 RODO
- Szyfrowanie danych w spoczynku (EncryptedSharedPreferences)
- Szyfrowanie komunikacji (HTTPS/TLS)
- Możliwość usunięcia danych użytkownika
- Logi audytowe w tabeli MagazynDziennik

### 8.2 Bezpieczeństwo aplikacji
- Certificate pinning dla API
- ProGuard/R8 obfuscation
- Root detection (opcjonalne)
- Biometric authentication (opcjonalne)

---

## 9. MAINTENANCE I MONITOROWANIE

### 9.1 Crashlytics Dashboard
- Monitorowanie crashy
- ANR (Application Not Responding)
- Fatal errors

### 9.2 Firebase Performance Monitoring
- Czas ładowania ekranów
- Czas synchronizacji API
- Wydajność skanera

### 9.3 Logging
- Lokalne logi (tylko DEBUG build)
- Remote logging (produkcja)
- User events (analytics)

---

## 10. PODSUMOWANIE

Aplikacja Android będzie kompletnym odpowiednikiem modułu MagazynControl z dodatkową funkcjonalnością mobilną (skaner, notyfikacje push). Kluczowe zalety:

✅ **Offline-first** - działa bez internetu  
✅ **Szybki skaner** - CameraX + ML Kit  
✅ **Intuicyjny UI** - Jetpack Compose  
✅ **Bezpieczny** - szyfrowanie, HTTPS  
✅ **Wydajny** - Paging, cache, optymalizacje  
✅ **Testowalny** - Unit + UI tests  
✅ **Skalowalny** - MVVM + Clean Architecture  

---

**Koniec dokumentu**
