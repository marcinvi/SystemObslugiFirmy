# SPECYFIKACJA API - Backend dla Aplikacji Android Magazyn

**Wersja:** 1.0  
**Format:** REST API  
**Protokół:** HTTPS  
**Autentykacja:** Bearer Token (JWT)  
**Content-Type:** application/json

---

## 1. AUTENTYKACJA

### 1.1 POST /api/auth/login
**Opis:** Logowanie użytkownika

**Request:**
```json
{
  "email": "string",
  "password": "string",
  "deviceId": "string (optional)",
  "fcmToken": "string (optional dla push notifications)"
}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": 1,
      "displayName": "Jan Kowalski",
      "email": "jan.kowalski@firma.pl",
      "role": "Magazyn",
      "permissions": ["view_returns", "edit_returns", "scan_barcodes"]
    },
    "tokens": {
      "accessToken": "eyJhbGciOiJIUzI1NiIs...",
      "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
      "expiresIn": 3600
    }
  }
}
```

**Response 401:**
```json
{
  "success": false,
  "error": {
    "code": "INVALID_CREDENTIALS",
    "message": "Nieprawidłowy email lub hasło"
  }
}
```

---

### 1.2 POST /api/auth/refresh
**Opis:** Odświeżenie tokenu

**Headers:**
```
Authorization: Bearer {refreshToken}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
    "expiresIn": 3600
  }
}
```

---

### 1.3 POST /api/auth/logout
**Opis:** Wylogowanie użytkownika

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "refreshToken": "string"
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Wylogowano pomyślnie"
}
```

---

## 2. ZWROTY (RETURNS)

### 2.1 GET /api/returns
**Opis:** Lista zwrotów z filtrowaniem

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Query Parameters:**
```
status?: string               // 'pending', 'awaiting_decision', 'completed', 'in_transit', 'all'
statusAllegro?: string        // 'DELIVERED', 'IN_TRANSIT', 'READY_FOR_PICKUP', 'CREATED'
search?: string               // Wyszukiwanie po numerze, nazwisku, produkcie
accountId?: number            // Filtr po koncie Allegro
page?: number                 // Domyślnie 1
limit?: number                // Domyślnie 20
sortBy?: string               // 'createdAt', 'updatedAt', 'referenceNumber'
sortOrder?: string            // 'asc', 'desc' (domyślnie 'desc')
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "returns": [
      {
        "id": 1,
        "referenceNumber": "R/001/01/26",
        "allegroReturnId": "abc-123",
        "accountId": 5,
        "accountName": "Konto Allegro 1",
        "orderId": "xyz-456",
        "buyerLogin": "buyer123",
        "buyerFullName": "Anna Nowak",
        "createdAt": "2026-01-20T10:30:00Z",
        "statusAllegro": "DELIVERED",
        "waybill": "1234567890123456",
        "carrierName": "DPD",
        "productName": "Produkt testowy",
        "offerId": "123456789",
        "quantity": 1,
        "statusWewnetrzny": {
          "id": 1,
          "nazwa": "Oczekuje na przyjęcie",
          "kolor": "#4CAF50"
        },
        "stanProduktu": {
          "id": null,
          "nazwa": null,
          "kolor": null
        },
        "decyzjaHandlowca": {
          "id": null,
          "nazwa": null,
          "kolor": null
        },
        "uwagiMagazynu": null,
        "komentarzHandlowca": null,
        "przyjetyPrzez": null,
        "dataPrzyjecia": null,
        "isManual": false,
        "delivery": {
          "firstName": "Anna",
          "lastName": "Nowak",
          "street": "ul. Testowa 1",
          "zipCode": "00-001",
          "city": "Warszawa",
          "phoneNumber": "+48123456789"
        },
        "buyer": {
          "firstName": "Anna",
          "lastName": "Nowak",
          "street": "ul. Testowa 1",
          "zipCode": "00-001",
          "city": "Warszawa",
          "phoneNumber": "+48123456789"
        },
        "invoice": {
          "companyName": "Firma Sp. z o.o.",
          "taxId": "1234567890",
          "street": "ul. Firmowa 10",
          "zipCode": "00-002",
          "city": "Warszawa"
        }
      }
    ],
    "pagination": {
      "currentPage": 1,
      "totalPages": 5,
      "totalItems": 100,
      "itemsPerPage": 20
    },
    "counts": {
      "pending": 15,
      "awaitingDecision": 8,
      "completed": 50,
      "inTransit": 12,
      "all": 100
    }
  }
}
```

---

### 2.2 GET /api/returns/{id}
**Opis:** Szczegóły zwrotu

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "return": {
      // Wszystkie pola jak w liście + dodatkowo:
      "invoiceNumber": "FV/2026/01/001",
      "paymentType": "CASH_ON_DELIVERY",
      "fulfillmentStatus": "DELIVERED",
      "jsonDetails": { /* pełny JSON z API Allegro */ },
      "manualSenderDetails": { /* dla zwrotów ręcznych */ },
      "history": [
        {
          "id": 1,
          "date": "2026-01-20T10:30:00Z",
          "user": "Jan Kowalski",
          "action": "Zwrot utworzony automatycznie z API Allegro"
        },
        {
          "id": 2,
          "date": "2026-01-21T14:20:00Z",
          "user": "Piotr Nowak",
          "action": "Przyjęty w magazynie. Stan: Nowy, nieużywany"
        }
      ]
    }
  }
}
```

**Response 404:**
```json
{
  "success": false,
  "error": {
    "code": "RETURN_NOT_FOUND",
    "message": "Nie znaleziono zwrotu o podanym ID"
  }
}
```

---

### 2.3 PUT /api/returns/{id}
**Opis:** Aktualizacja zwrotu

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "stanProduktuId": 2,
  "uwagiMagazynu": "Produkt nieuszkodzony, oryginalne opakowanie",
  "statusWewnetrznyId": 2
}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "return": {
      // Zaktualizowany zwrot
    }
  },
  "message": "Zwrot zaktualizowany pomyślnie"
}
```

---

### 2.4 POST /api/returns
**Opis:** Dodanie zwrotu ręcznego

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "waybill": "1234567890123456",
  "carrierName": "DPD",
  "productName": "Produkt testowy",
  "stanProduktuId": 1,
  "uwagiMagazynu": "Zwrot ręczny",
  "senderDetails": {
    "fullName": "Jan Kowalski",
    "street": "ul. Testowa 1",
    "zipCode": "00-001",
    "city": "Warszawa",
    "phoneNumber": "+48123456789"
  },
  "recipientIds": [5, 6] // IDs handlowców
}
```

**Response 201:**
```json
{
  "success": true,
  "data": {
    "return": {
      "id": 101,
      "referenceNumber": "R/101/01/26",
      // ... reszta danych
    }
  },
  "message": "Zwrot ręczny dodany pomyślnie"
}
```

---

### 2.5 POST /api/returns/{id}/send-to-salesman
**Opis:** Przekazanie zwrotu do handlowca

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "stanProduktuId": 2,
  "uwagiMagazynu": "Produkt sprawny"
}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "return": {
      // Zaktualizowany zwrot
    },
    "recipient": {
      "id": 5,
      "displayName": "Adam Handlowiec",
      "email": "adam@firma.pl"
    },
    "message": {
      "id": 123,
      "title": "Prośba o decyzję dla zwrotu R/001/01/26",
      "sentAt": "2026-01-22T10:00:00Z"
    }
  },
  "message": "Zwrot przekazany do handlowca"
}
```

---

### 2.6 POST /api/returns/sync
**Opis:** Synchronizacja zwrotów z Allegro

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "accountIds": [1, 2],  // Jeśli puste - wszystkie konta
  "daysBack": 60         // Ile dni wstecz synchronizować
}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "synced": 45,
    "updated": 12,
    "created": 33,
    "errors": []
  },
  "message": "Synchronizacja zakończona pomyślnie"
}
```

---

### 2.7 GET /api/returns/search/waybill/{waybill}
**Opis:** Wyszukiwanie zwrotu po numerze listu

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "return": {
      // Dane zwrotu
    }
  }
}
```

**Response 404:**
```json
{
  "success": false,
  "error": {
    "code": "RETURN_NOT_FOUND",
    "message": "Nie znaleziono zwrotu dla numeru listu: {waybill}"
  }
}
```

---

## 3. STATUSY

### 3.1 GET /api/statuses
**Opis:** Lista wszystkich statusów

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Query Parameters:**
```
type?: string  // 'StatusWewnetrzny', 'StanProduktu', 'DecyzjaHandlowca'
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "statuses": [
      {
        "id": 1,
        "nazwa": "Oczekuje na przyjęcie",
        "typStatusu": "StatusWewnetrzny",
        "kolor": "#4CAF50"
      },
      {
        "id": 2,
        "nazwa": "Oczekuje na decyzję handlowca",
        "typStatusu": "StatusWewnetrzny",
        "kolor": "#FF9800"
      },
      {
        "id": 3,
        "nazwa": "Zakończony",
        "typStatusu": "StatusWewnetrzny",
        "kolor": "#2196F3"
      },
      {
        "id": 10,
        "nazwa": "Nowy, nieużywany",
        "typStatusu": "StanProduktu",
        "kolor": "#4CAF50"
      }
    ]
  }
}
```

---

## 4. WIADOMOŚCI

### 4.1 GET /api/messages
**Opis:** Lista wiadomości

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Query Parameters:**
```
unreadOnly?: boolean  // Tylko nieprzeczytane
page?: number
limit?: number
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "messages": [
      {
        "id": 1,
        "sender": {
          "id": 2,
          "displayName": "Jan Kowalski",
          "role": "Magazyn"
        },
        "recipient": {
          "id": 5,
          "displayName": "Adam Handlowiec",
          "role": "Handlowiec"
        },
        "title": "Prośba o decyzję dla zwrotu R/001/01/26",
        "content": "Zwrot od klienta Anna Nowak",
        "sentAt": "2026-01-22T10:00:00Z",
        "isRead": false,
        "isReplied": false,
        "returnId": 1
      }
    ],
    "pagination": {
      "currentPage": 1,
      "totalPages": 3,
      "totalItems": 50,
      "itemsPerPage": 20
    }
  }
}
```

---

### 4.2 POST /api/messages
**Opis:** Wysłanie nowej wiadomości

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "recipientId": 5,
  "title": "Temat wiadomości",
  "content": "Treść wiadomości",
  "returnId": 1  // Opcjonalne
}
```

**Response 201:**
```json
{
  "success": true,
  "data": {
    "message": {
      "id": 124,
      // ... reszta danych
    }
  },
  "message": "Wiadomość wysłana pomyślnie"
}
```

---

### 4.3 PUT /api/messages/{id}/read
**Opis:** Oznaczenie wiadomości jako przeczytanej

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Wiadomość oznaczona jako przeczytana"
}
```

---

### 4.4 POST /api/messages/{id}/reply
**Opis:** Odpowiedź na wiadomość

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "content": "Treść odpowiedzi"
}
```

**Response 201:**
```json
{
  "success": true,
  "data": {
    "message": {
      // Nowa wiadomość - odpowiedź
    }
  },
  "message": "Odpowiedź wysłana pomyślnie"
}
```

---

## 5. UŻYTKOWNICY

### 5.1 GET /api/users
**Opis:** Lista użytkowników

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Query Parameters:**
```
role?: string  // 'Magazyn', 'Handlowiec', 'Admin'
active?: boolean
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "users": [
      {
        "id": 1,
        "displayName": "Jan Kowalski",
        "email": "jan.kowalski@firma.pl",
        "role": "Magazyn",
        "isActive": true
      },
      {
        "id": 5,
        "displayName": "Adam Handlowiec",
        "email": "adam@firma.pl",
        "role": "Handlowiec",
        "isActive": true
      }
    ]
  }
}
```

---

### 5.2 GET /api/users/{id}/delegate
**Opis:** Sprawdzenie aktywnej delegacji

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "hasDelegate": true,
    "delegate": {
      "id": 6,
      "displayName": "Paweł Zastępca",
      "email": "pawel@firma.pl",
      "delegationStart": "2026-01-20",
      "delegationEnd": "2026-02-05"
    }
  }
}
```

---

## 6. KONTA ALLEGRO

### 6.1 GET /api/allegro/accounts
**Opis:** Lista kont Allegro

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "accounts": [
      {
        "id": 1,
        "accountName": "Konto Allegro 1",
        "isAuthorized": true,
        "caretaker": {
          "id": 5,
          "displayName": "Adam Handlowiec"
        }
      }
    ]
  }
}
```

---

## 7. DZIENNIK

### 7.1 GET /api/journal
**Opis:** Dziennik zmian

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Query Parameters:**
```
returnId?: number  // Filtr po zwrocie
userId?: number    // Filtr po użytkowniku
from?: string      // Data od (YYYY-MM-DD)
to?: string        // Data do (YYYY-MM-DD)
page?: number
limit?: number
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "entries": [
      {
        "id": 1,
        "date": "2026-01-22T10:30:00Z",
        "user": "Jan Kowalski",
        "action": "Zsynchronizowano zwroty. Przetworzono 45 wpisów.",
        "returnId": null
      },
      {
        "id": 2,
        "date": "2026-01-22T11:00:00Z",
        "user": "Piotr Nowak",
        "action": "Przekazano zwrot do: Adam Handlowiec. Stan: 'Nowy, nieużywany'",
        "returnId": 1
      }
    ],
    "pagination": {
      "currentPage": 1,
      "totalPages": 10,
      "totalItems": 200,
      "itemsPerPage": 20
    }
  }
}
```

---

## 8. USTAWIENIA

### 8.1 GET /api/settings
**Opis:** Pobranie ustawień użytkownika

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "settings": {
      "notifications": {
        "enabled": true,
        "sound": true,
        "vibration": true
      },
      "sync": {
        "autoSync": true,
        "syncInterval": 15  // minuty
      },
      "scanner": {
        "hapticFeedback": true,
        "soundFeedback": true
      }
    }
  }
}
```

---

### 8.2 PUT /api/settings
**Opis:** Aktualizacja ustawień

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "notifications": {
    "enabled": true,
    "sound": false,
    "vibration": true
  }
}
```

**Response 200:**
```json
{
  "success": true,
  "message": "Ustawienia zaktualizowane"
}
```

---

## 9. SUGESTIE AUTOUZUPEŁNIANIA

### 9.1 GET /api/autocomplete/products
**Opis:** Podpowiedzi nazw produktów

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Query Parameters:**
```
query: string  // Min 2 znaki
limit?: number // Domyślnie 10
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "suggestions": [
      "Produkt testowy 1",
      "Produkt testowy 2",
      "Produkt ABC"
    ]
  }
}
```

---

### 9.2 GET /api/autocomplete/carriers
**Opis:** Podpowiedzi przewoźników

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Query Parameters:**
```
query: string
limit?: number
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "suggestions": [
      "DPD",
      "InPost",
      "Poczta Polska"
    ]
  }
}
```

---

## 10. KODY BŁĘDÓW

| Kod HTTP | Error Code | Opis |
|----------|------------|------|
| 400 | INVALID_REQUEST | Nieprawidłowe zapytanie |
| 401 | INVALID_CREDENTIALS | Nieprawidłowe dane logowania |
| 401 | TOKEN_EXPIRED | Token wygasł |
| 401 | INVALID_TOKEN | Nieprawidłowy token |
| 403 | FORBIDDEN | Brak uprawnień |
| 404 | NOT_FOUND | Nie znaleziono zasobu |
| 404 | RETURN_NOT_FOUND | Nie znaleziono zwrotu |
| 404 | USER_NOT_FOUND | Nie znaleziono użytkownika |
| 409 | CONFLICT | Konflikt danych |
| 422 | VALIDATION_ERROR | Błąd walidacji |
| 429 | RATE_LIMIT_EXCEEDED | Przekroczono limit zapytań |
| 500 | INTERNAL_ERROR | Błąd serwera |
| 503 | SERVICE_UNAVAILABLE | Usługa niedostępna |

---

## 11. RATE LIMITING

**Limity:**
- Auth endpoints: 10 req/min na IP
- GET endpoints: 100 req/min na użytkownika
- POST/PUT/DELETE: 30 req/min na użytkownika

**Headers w odpowiedzi:**
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1706011200
```

---

## 12. PAGINATION

Wszystkie endpointy zwracające listy wspierają paginację:

**Query Parameters:**
```
page: number     // Numer strony (zaczyna od 1)
limit: number    // Ilość elementów na stronę (max 100)
```

**Response zawiera:**
```json
{
  "pagination": {
    "currentPage": 1,
    "totalPages": 10,
    "totalItems": 200,
    "itemsPerPage": 20
  }
}
```

---

## 13. FILTERING & SORTING

**Filtrowanie:**
```
GET /api/returns?status=pending&accountId=1
```

**Sortowanie:**
```
GET /api/returns?sortBy=createdAt&sortOrder=desc
```

**Wyszukiwanie:**
```
GET /api/returns?search=kowalski
```

**Kombinacja:**
```
GET /api/returns?status=pending&search=dpd&sortBy=createdAt&page=2&limit=50
```

---

## 14. WEBHOOKS (OPCJONALNE)

### 14.1 POST /api/webhooks/register
**Opis:** Rejestracja webhooka dla powiadomień push

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "deviceId": "unique-device-id",
  "fcmToken": "firebase-cloud-messaging-token",
  "platform": "android"
}
```

**Response 201:**
```json
{
  "success": true,
  "data": {
    "webhookId": "webhook-123",
    "registered": true
  }
}
```

---

## 15. SYNC QUEUE API (dla offline-first)

### 15.1 POST /api/sync/batch
**Opis:** Synchronizacja wsadowa zmian offline

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request:**
```json
{
  "operations": [
    {
      "type": "UPDATE_RETURN",
      "id": 1,
      "timestamp": "2026-01-22T10:00:00Z",
      "data": {
        "stanProduktuId": 2,
        "uwagiMagazynu": "Test"
      }
    },
    {
      "type": "CREATE_MESSAGE",
      "timestamp": "2026-01-22T10:05:00Z",
      "data": {
        "recipientId": 5,
        "content": "Test message"
      }
    }
  ]
}
```

**Response 200:**
```json
{
  "success": true,
  "data": {
    "processed": 2,
    "successful": 2,
    "failed": 0,
    "results": [
      {
        "operation": 0,
        "success": true,
        "id": 1
      },
      {
        "operation": 1,
        "success": true,
        "id": 124
      }
    ]
  }
}
```

---

## 16. PRZYKŁAD IMPLEMENTACJI (PHP Laravel)

```php
// routes/api.php
Route::prefix('v1')->group(function () {
    Route::post('auth/login', [AuthController::class, 'login']);
    Route::post('auth/refresh', [AuthController::class, 'refresh']);
    
    Route::middleware('auth:api')->group(function () {
        Route::post('auth/logout', [AuthController::class, 'logout']);
        
        Route::apiResource('returns', ReturnController::class);
        Route::get('returns/search/waybill/{waybill}', [ReturnController::class, 'searchByWaybill']);
        Route::post('returns/{id}/send-to-salesman', [ReturnController::class, 'sendToSalesman']);
        Route::post('returns/sync', [ReturnController::class, 'sync']);
        
        Route::apiResource('messages', MessageController::class);
        Route::put('messages/{id}/read', [MessageController::class, 'markAsRead']);
        
        Route::get('statuses', [StatusController::class, 'index']);
        Route::get('users', [UserController::class, 'index']);
        Route::get('allegro/accounts', [AllegroController::class, 'accounts']);
        Route::get('journal', [JournalController::class, 'index']);
    });
});

// app/Http/Controllers/ReturnController.php
class ReturnController extends Controller
{
    public function index(Request $request)
    {
        $query = AllegroCustomerReturn::query();
        
        // Filtrowanie
        if ($request->has('status')) {
            $query->whereHas('statusWewnetrzny', function($q) use ($request) {
                $q->where('nazwa', $this->mapStatusFilter($request->status));
            });
        }
        
        if ($request->has('search')) {
            $search = $request->search;
            $query->where(function($q) use ($search) {
                $q->where('ReferenceNumber', 'LIKE', "%{$search}%")
                  ->orWhere('BuyerLogin', 'LIKE', "%{$search}%")
                  ->orWhere('Waybill', 'LIKE', "%{$search}%");
            });
        }
        
        // Sortowanie
        $sortBy = $request->get('sortBy', 'CreatedAt');
        $sortOrder = $request->get('sortOrder', 'desc');
        $query->orderBy($sortBy, $sortOrder);
        
        // Paginacja
        $limit = min($request->get('limit', 20), 100);
        $returns = $query->paginate($limit);
        
        return response()->json([
            'success' => true,
            'data' => [
                'returns' => $returns->items(),
                'pagination' => [
                    'currentPage' => $returns->currentPage(),
                    'totalPages' => $returns->lastPage(),
                    'totalItems' => $returns->total(),
                    'itemsPerPage' => $returns->perPage()
                ]
            ]
        ]);
    }
    
    public function update(Request $request, $id)
    {
        $validator = Validator::make($request->all(), [
            'stanProduktuId' => 'required|exists:Statusy,Id',
            'uwagiMagazynu' => 'nullable|string',
            'statusWewnetrznyId' => 'nullable|exists:Statusy,Id'
        ]);
        
        if ($validator->fails()) {
            return response()->json([
                'success' => false,
                'error' => [
                    'code' => 'VALIDATION_ERROR',
                    'message' => $validator->errors()
                ]
            ], 422);
        }
        
        $return = AllegroCustomerReturn::findOrFail($id);
        $return->update($request->all());
        
        // Logowanie
        MagazynDziennik::create([
            'Data' => now(),
            'Uzytkownik' => auth()->user()->NazwaWyswietlana,
            'Akcja' => "Zaktualizowano zwrot {$return->ReferenceNumber}",
            'DotyczyZwrotuId' => $id
        ]);
        
        return response()->json([
            'success' => true,
            'data' => ['return' => $return],
            'message' => 'Zwrot zaktualizowany pomyślnie'
        ]);
    }
}
```

---

**Koniec specyfikacji API**
