# DIAGRAMY ARCHITEKTURY - Aplikacja Android Magazyn

## 1. ARCHITEKTURA APLIKACJI

```mermaid
graph TB
    subgraph "Presentation Layer"
        UI[UI - Jetpack Compose]
        VM[ViewModels]
    end
    
    subgraph "Domain Layer"
        UC[Use Cases]
        MODEL[Domain Models]
    end
    
    subgraph "Data Layer"
        REPO[Repositories]
        LOCAL[Room Database]
        REMOTE[Retrofit API]
    end
    
    UI --> VM
    VM --> UC
    UC --> MODEL
    UC --> REPO
    REPO --> LOCAL
    REPO --> REMOTE
```

## 2. PRZEPŁYW DANYCH - SYNCHRONIZACJA ZWROTÓW

```mermaid
sequenceDiagram
    participant User
    participant UI
    participant VM as ViewModel
    participant UC as UseCase
    participant Repo as Repository
    participant DB as Room DB
    participant API as Allegro API
    
    User->>UI: Klik "Synchronizuj"
    UI->>VM: syncReturns()
    VM->>UC: SyncReturnsUseCase.execute()
    UC->>Repo: syncFromApi()
    
    loop For each Allegro account
        Repo->>API: GET /customer-returns
        API-->>Repo: Returns list
        
        loop For each return
            Repo->>API: GET /order/{id}
            API-->>Repo: Order details
            Repo->>API: GET /invoice/{id}
            API-->>Repo: Invoice
        end
        
        Repo->>DB: upsert(returns)
        DB-->>Repo: Success
    end
    
    Repo-->>UC: Sync complete
    UC-->>VM: Success
    VM-->>UI: Update UI
    UI-->>User: Show toast "Zsynchronizowano X zwrotów"
```

## 3. PRZEPŁYW DANYCH - SKANOWANIE KODU

```mermaid
sequenceDiagram
    participant User
    participant Scanner as Scanner Screen
    participant VM as ViewModel
    participant Parser as BarcodeParser
    participant Repo as Repository
    participant DB as Room DB
    
    User->>Scanner: Skanuje kod kreskowy
    Scanner->>VM: processBarcodeResult(barcode)
    VM->>Parser: extractWaybill(barcode)
    
    alt DPD format
        Parser-->>VM: 14-char waybill
    else Generic format
        Parser-->>VM: Extracted number
    end
    
    VM->>Repo: findByWaybill(waybill)
    Repo->>DB: Query return
    
    alt Return found
        DB-->>Repo: Return data
        Repo-->>VM: Return object
        VM->>Scanner: Navigate to details
        Scanner-->>User: Show return details
    else Return not found
        DB-->>Repo: null
        Repo-->>VM: null
        VM->>Scanner: Show dialog
        Scanner-->>User: "Nie znaleziono. Dodać ręcznie?"
    end
```

## 4. PRZEPŁYW DANYCH - PRZEKAZYWANIE DO HANDLOWCA

```mermaid
sequenceDiagram
    participant User
    participant UI as Details Screen
    participant VM as ViewModel
    participant UC as ProcessReturnUseCase
    participant ReturnRepo as ReturnRepository
    participant UserRepo as UserRepository
    participant MsgRepo as MessageRepository
    participant DB as Room DB
    
    User->>UI: Wybiera stan, wpisuje uwagi
    User->>UI: Klik "Przekaż do handlowca"
    UI->>VM: sendToSalesman()
    VM->>UC: execute(returnId, status, notes)
    
    UC->>ReturnRepo: getReturn(returnId)
    ReturnRepo->>DB: Query return
    DB-->>ReturnRepo: Return data
    ReturnRepo-->>UC: Return object
    
    alt Return is manual
        UC->>MsgRepo: getRecipients(returnId)
        MsgRepo-->>UC: List of recipients
    else Return is from Allegro
        UC->>UserRepo: getAccountCaretaker(accountId)
        UserRepo-->>UC: Salesman
        UC->>UserRepo: getActiveDelegate(salesmanId)
        UserRepo-->>UC: Delegate or null
    end
    
    UC->>ReturnRepo: update(return + status + notes)
    ReturnRepo->>DB: Update return
    UC->>MsgRepo: send(message to salesman)
    MsgRepo->>DB: Insert message
    UC->>ReturnRepo: logAction()
    ReturnRepo->>DB: Insert log
    
    DB-->>UC: Success
    UC-->>VM: Result.success
    VM-->>UI: Show toast
    UI-->>User: "Przekazano do handlowca"
```

## 5. ARCHITEKTURA BAZY DANYCH - ER DIAGRAM

```mermaid
erDiagram
    AllegroCustomerReturns ||--o{ Wiadomosci : "dotyczy"
    AllegroCustomerReturns ||--o{ ZwrotDzialania : "ma"
    AllegroCustomerReturns }o--|| Statusy : "ma StatusWewnetrzny"
    AllegroCustomerReturns }o--|| Statusy : "ma StanProduktu"
    AllegroCustomerReturns }o--|| Statusy : "ma DecyzjaHandlowca"
    AllegroCustomerReturns }o--|| Uzytkownicy : "przyjety przez"
    AllegroAccounts ||--o{ AllegroCustomerReturns : "zawiera"
    AllegroAccounts ||--o{ AllegroAccountOpiekun : "ma opiekuna"
    Uzytkownicy ||--o{ AllegroAccountOpiekun : "opiekuje sie"
    Uzytkownicy ||--o{ Wiadomosci : "wysyla"
    Uzytkownicy ||--o{ Wiadomosci : "otrzymuje"
    Uzytkownicy ||--o{ Delegacje : "deleguje"
    Uzytkownicy ||--o{ Delegacje : "jest zastepca"
    
    AllegroCustomerReturns {
        int Id PK
        string ReferenceNumber UK
        string Waybill
        string StatusAllegro
        int StatusWewnetrznyId FK
        int StanProduktuId FK
        int DecyzjaHandlowcaId FK
        int AllegroAccountId FK
        int PrzyjetyPrzezId FK
        datetime DataPrzyjecia
        boolean IsManual
        text JsonDetails
        text UwagiMagazynu
        text KomentarzHandlowca
    }
    
    Statusy {
        int Id PK
        string Nazwa
        string TypStatusu
        string Kolor
    }
    
    Uzytkownicy {
        int Id PK
        string NazwaWyswietlana
        string Email
        string Rola
        boolean IsActive
    }
    
    Wiadomosci {
        int Id PK
        int NadawcaId FK
        int OdbiorcaId FK
        string Tytul
        text Tresc
        datetime DataWyslania
        int DotyczyZwrotuId FK
        boolean CzyPrzeczytana
        boolean CzyOdpowiedziano
    }
    
    ZwrotDzialania {
        int Id PK
        int ZwrotId FK
        datetime Data
        string Uzytkownik
        text Tresc
    }
    
    AllegroAccounts {
        int Id PK
        string AccountName
        text AccessTokenEncrypted
        text RefreshTokenEncrypted
        datetime TokenExpirationDate
        boolean IsAuthorized
    }
    
    AllegroAccountOpiekun {
        int AllegroAccountId FK
        int OpiekunId FK
    }
    
    Delegacje {
        int Id PK
        int UzytkownikId FK
        int ZastepcaId FK
        date DataOd
        date DataDo
        boolean CzyAktywna
    }
```

## 6. NAVIGATION GRAPH

```mermaid
graph TD
    Start[Splash Screen] --> Login[Login Screen]
    Login --> Main[Main Screen - Returns List]
    
    Main --> Details[Return Details]
    Main --> Scanner[Scanner Screen]
    Main --> AddManual[Add Manual Return]
    Main --> Messages[Messages Screen]
    Main --> Journal[Journal Screen]
    Main --> Settings[Settings Screen]
    
    Details --> OtherAddresses[Other Addresses Dialog]
    Details --> History[Return History]
    Details --> Summary[Return Summary]
    
    Scanner --> Details
    Scanner --> AddManual
    
    Messages --> MessageDetails[Message Details Screen]
    MessageDetails --> ReplyMessage[Reply Message Screen]
    MessageDetails --> Details
    
    AddManual --> SelectSalesmen[Select Salesmen Dialog]
```

## 7. STATE MANAGEMENT - ViewModel

```mermaid
graph LR
    subgraph "ReturnsListViewModel"
        State[UI State]
        Events[UI Events]
        Actions[User Actions]
    end
    
    subgraph "UI State"
        Returns[List of Returns]
        Loading[Loading State]
        Error[Error State]
        Filter[Active Filter]
    end
    
    subgraph "UI Events"
        Navigate[Navigation Events]
        ShowToast[Toast Messages]
        ShowDialog[Dialog Events]
    end
    
    subgraph "User Actions"
        Refresh[Refresh Data]
        ChangeFilter[Change Filter]
        Search[Search Query]
        OpenDetails[Open Details]
    end
    
    Actions --> State
    State --> UI[Compose UI]
    Events --> UI
```

## 8. OFFLINE SYNC STRATEGY

```mermaid
sequenceDiagram
    participant User
    participant UI
    participant Repo as Repository
    participant LocalDB as Local DB
    participant SyncQueue as Sync Queue
    participant API as Remote API
    participant Worker as WorkManager
    
    User->>UI: Updates return
    UI->>Repo: updateReturn()
    
    Note over Repo: Save locally first
    Repo->>LocalDB: Save return
    LocalDB-->>Repo: Success
    Repo-->>UI: Immediate feedback
    UI-->>User: Toast "Zapisano"
    
    Note over Repo: Queue for sync
    Repo->>SyncQueue: Add to queue
    
    alt Network available
        Repo->>API: POST /returns/{id}
        
        alt API Success
            API-->>Repo: 200 OK
            Repo->>SyncQueue: Remove from queue
        else API Error
            API-->>Repo: Error
            Note over Repo: Keep in queue
        end
    else Network unavailable
        Note over Repo: Keep in queue
    end
    
    Note over Worker: Periodic sync (15 min)
    Worker->>SyncQueue: Get pending items
    SyncQueue-->>Worker: List of items
    
    loop For each item
        Worker->>API: Sync item
        alt Success
            API-->>Worker: OK
            Worker->>SyncQueue: Remove item
        else Error
            API-->>Worker: Error
            Note over Worker: Retry later
        end
    end
```

## 9. SECURITY ARCHITECTURE

```mermaid
graph TB
    subgraph "App Layer"
        UI[UI Components]
        VM[ViewModels]
    end
    
    subgraph "Security Layer"
        Auth[Auth Manager]
        Encryption[Encryption Service]
        Keystore[Android Keystore]
    end
    
    subgraph "Network Layer"
        Interceptor[Auth Interceptor]
        SSL[SSL Pinning]
        API[API Client]
    end
    
    subgraph "Storage Layer"
        EncPrefs[Encrypted SharedPrefs]
        RoomDB[(Encrypted Room DB)]
    end
    
    UI --> VM
    VM --> Auth
    Auth --> Encryption
    Encryption --> Keystore
    
    VM --> API
    API --> Interceptor
    API --> SSL
    Interceptor --> Auth
    
    Auth --> EncPrefs
    VM --> RoomDB
    RoomDB --> Encryption
```

## 10. WORK MANAGER - BACKGROUND SYNC

```mermaid
graph TD
    Start[App Start] --> Check{Internet?}
    
    Check -->|Yes| Sync1[Immediate Sync]
    Check -->|No| Schedule[Schedule Worker]
    
    Sync1 --> Done1[Update UI]
    
    Schedule --> Wait[Wait for internet]
    Wait --> Worker[OneTime Worker]
    Worker --> SyncData[Sync Data]
    SyncData --> UpdateDB[Update Local DB]
    UpdateDB --> Done2[Send Notification]
    
    subgraph "Periodic Worker"
        Every15Min[Every 15 minutes]
        CheckNet{Internet?}
        PeriodicSync[Sync Returns]
    end
    
    Every15Min --> CheckNet
    CheckNet -->|Yes| PeriodicSync
    CheckNet -->|No| Every15Min
    PeriodicSync --> UpdateDB
```

## 11. SCANNER FLOW - DETAILED

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> RequestPermission: Open Scanner
    RequestPermission --> CheckPermission: User Response
    
    CheckPermission --> InitCamera: Granted
    CheckPermission --> ShowError: Denied
    
    InitCamera --> Scanning
    Scanning --> ProcessBarcode: Barcode Detected
    
    ProcessBarcode --> ParseCode: Raw String
    ParseCode --> SearchDB: Waybill Extracted
    
    SearchDB --> ShowDetails: Found
    SearchDB --> AskAddManual: Not Found
    
    ShowDetails --> [*]
    AskAddManual --> AddManual: User clicks Yes
    AskAddManual --> Scanning: User clicks No
    AddManual --> [*]
    ShowError --> [*]
```

## 12. API ENDPOINTS - REQUEST/RESPONSE FLOW

```mermaid
sequenceDiagram
    participant App
    participant AuthInterceptor
    participant API
    participant Backend
    participant Database
    
    App->>AuthInterceptor: HTTP Request
    AuthInterceptor->>AuthInterceptor: Add Bearer Token
    AuthInterceptor->>API: Authenticated Request
    API->>Backend: Route to endpoint
    
    alt Valid Token
        Backend->>Database: Query data
        Database-->>Backend: Result
        Backend-->>API: 200 OK + Data
        API-->>App: Success Response
    else Expired Token
        Backend-->>API: 401 Unauthorized
        API->>App: Token expired
        App->>API: POST /auth/refresh
        API-->>App: New tokens
        App->>AuthInterceptor: Retry original request
    else Invalid Token
        Backend-->>API: 403 Forbidden
        API-->>App: Unauthorized
        App->>App: Navigate to Login
    end
```

## 13. DATABASE MIGRATION STRATEGY

```mermaid
graph TD
    Start[App Launch] --> CheckVersion{Check DB Version}
    
    CheckVersion -->|Version 1| Current[Use Current Schema]
    CheckVersion -->|Version < 1| Migration[Run Migrations]
    
    Migration --> M1{Migration 1}
    M1 --> M2{Migration 2}
    M2 --> M3{Migration 3}
    M3 --> Current
    
    Current --> Success[App Ready]
    
    Migration -->|Error| Backup[Restore Backup]
    Backup --> Error[Show Error]
    Error --> Recovery[Data Recovery Mode]
```

## 14. NOTIFICATION ARCHITECTURE

```mermaid
graph TB
    subgraph "Trigger Sources"
        API[API Response]
        Worker[Background Worker]
        Local[Local Event]
    end
    
    subgraph "Notification Manager"
        Builder[Notification Builder]
        Channel[Notification Channel]
        Priority[Priority Handler]
    end
    
    subgraph "User Actions"
        Tap[Tap Action]
        Dismiss[Dismiss Action]
        Reply[Quick Reply]
    end
    
    API --> Builder
    Worker --> Builder
    Local --> Builder
    
    Builder --> Channel
    Channel --> Priority
    Priority --> Display[Display Notification]
    
    Display --> Tap
    Display --> Dismiss
    Display --> Reply
    
    Tap --> OpenApp[Open App + Navigate]
    Reply --> SendMessage[Send Message]
```

## 15. ERROR HANDLING STRATEGY

```mermaid
graph TD
    Error[Error Occurs] --> Type{Error Type}
    
    Type -->|Network| NetworkError[Network Error]
    Type -->|Auth| AuthError[Auth Error]
    Type -->|Data| DataError[Data Error]
    Type -->|Unknown| UnknownError[Unknown Error]
    
    NetworkError --> Retry{Can Retry?}
    Retry -->|Yes| QueueRetry[Queue for Retry]
    Retry -->|No| ShowNetworkError[Show Network Error Toast]
    
    AuthError --> Reauth{Can Reauth?}
    Reauth -->|Yes| RefreshToken[Refresh Token]
    Reauth -->|No| ForceLogin[Force Re-login]
    
    DataError --> Log[Log to Crashlytics]
    Log --> ShowDataError[Show Data Error Dialog]
    
    UnknownError --> LogCritical[Log Critical Error]
    LogCritical --> ShowGenericError[Show Generic Error]
    
    QueueRetry --> BackgroundWorker[Background Worker]
    RefreshToken --> RetryOriginal[Retry Original Request]
    ShowNetworkError --> OfflineMode[Continue in Offline Mode]
```

---

**Koniec dokumentu diagramów**
