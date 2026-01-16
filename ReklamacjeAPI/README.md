# 🚀 Reklamacje API - REST API Backend

REST API dla systemu obsługi reklamacji, zintegrowane z aplikacją Windows Form i Android (ENA).

## 📋 Spis treści

- [Technologie](#technologie)
- [Wymagania](#wymagania)
- [Instalacja](#instalacja)
- [Konfiguracja](#konfiguracja)
- [Uruchomienie](#uruchomienie)
- [Testowanie](#testowanie)
- [Endpoints](#endpoints)
- [Troubleshooting](#troubleshooting)

---

## 🛠️ Technologie

- **.NET 8.0** - Framework aplikacji
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core 8** - ORM
- **MySQL/MariaDB** - Baza danych
- **JWT** - Autentykacja
- **Swagger/OpenAPI** - Dokumentacja API
- **BCrypt** - Hashowanie haseł

---

## 📦 Wymagania

### Software:
- **.NET 8.0 SDK** - [Pobierz tutaj](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MariaDB** (lub MySQL) - Istniejąca baza `ReklamacjeDB`
- **Visual Studio 2022** (opcjonalnie) lub **VS Code**

### Baza danych:
Aplikacja używa istniejącej bazy MariaDB z systemu Windows Form.

---

## 📥 Instalacja

### Krok 1: Clone / Otwórz projekt

```bash
cd C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\ReklamacjeAPI
```

### Krok 2: Restore packages

```bash
dotnet restore
```

---

## ⚙️ Konfiguracja

### 1. Connection String

Edytuj `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=ReklamacjeDB;User=root;Password=TWOJE_HASLO;"
  }
}
```

**🔧 ZMIEŃ:**
- `Password=TWOJE_HASLO` - Wpisz hasło do MariaDB

### 2. JWT Secret

**WAŻNE:** Zmień `JwtSettings:Secret` w `appsettings.json`:

```json
{
  "JwtSettings": {
    "Secret": "WYGENERUJ-LOSOWY-KLUCZ-MIN-32-ZNAKI-ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
    "Issuer": "ReklamacjeAPI",
    "Audience": "ReklamacjeClients",
    "ExpiryMinutes": 60
  }
}
```

**🔐 Generowanie losowego klucza (PowerShell):**
```powershell
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

---

## 🔧 Przygotowanie bazy danych

### Sprawdź istniejące tabele:

```sql
USE ReklamacjeDB;
SHOW TABLES;
```

Powinieneś zobaczyć:
- `Uzytkownicy`
- `Klienci`
- `Produkty`
- `Zgloszenia`
- `Dzialania`
- `Pliki`

### Dodaj brakujące tabele (jeśli potrzebne):

```sql
-- Tabela RefreshTokens (dla JWT)
CREATE TABLE IF NOT EXISTS RefreshTokens (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Token VARCHAR(500) NOT NULL,
    ExpiryDate DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    IsRevoked BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (UserId) REFERENCES Uzytkownicy(IdUzytkownika) ON DELETE CASCADE,
    INDEX idx_token (Token),
    INDEX idx_user (UserId)
);

-- Dodaj indeksy dla wydajności (jeśli nie istnieją)
ALTER TABLE Zgloszenia ADD INDEX idx_status (StatusOgolny);
ALTER TABLE Zgloszenia ADD INDEX idx_data (DataZgloszenia);
ALTER TABLE Klienci ADD INDEX idx_telefon (Telefon);
ALTER TABLE Klienci ADD INDEX idx_email (Email);
```

### Utwórz testowego użytkownika:

```sql
-- Hasło: test123 (bcrypt hash)
INSERT INTO Uzytkownicy (Login, HasloHash, NazwaWyswietlana, Email, Aktywny, DataDodania)
VALUES (
    'admin',
    '$2a$11$6ZwFqYqKl2.xP9LV8vCqO.K3fWGdZOZ2XJoQQRq2QhZl8CQqNlQfK',
    'Administrator',
    'admin@reklamacje.pl',
    TRUE,
    NOW()
);
```

**Dane logowania:**
- Login: `admin`
- Hasło: `test123`

---

## 🚀 Uruchomienie

### Metoda 1: Visual Studio 2022

1. Otwórz `ReklamacjeAPI.sln`
2. Naciśnij **F5** (lub kliknij "▶ https")
3. Swagger UI otworzy się automatycznie w przeglądarce

### Metoda 2: dotnet CLI

```bash
dotnet run
```

Aplikacja uruchomi się na:
- **HTTPS:** `https://localhost:5001`
- **HTTP:** `http://localhost:5000`
- **Swagger UI:** `https://localhost:5001/` (root)

---

## 🧪 Testowanie

### Krok 1: Otwórz Swagger UI

Przejdź do: `https://localhost:5001/`

### Krok 2: Zaloguj się (Get JWT Token)

1. Znajdź endpoint **POST /api/Auth/login**
2. Kliknij "Try it out"
3. Wpisz:
```json
{
  "login": "admin",
  "password": "test123"
}
```
4. Kliknij "Execute"
5. **Skopiuj** `token` z response

### Krok 3: Autoryzuj w Swagger

1. Kliknij przycisk **🔓 Authorize** (góra strony)
2. Wpisz: `Bearer TWOJ_TOKEN` (WKLEJ skopiowany token)
3. Kliknij "Authorize"
4. ✅ Teraz masz dostęp do wszystkich endpoints!

### Krok 4: Testuj endpoints

**Przykłady:**

**1. Pobierz moje zgłoszenia:**
```
GET /api/Zgloszenia/moje?page=1&pageSize=20
```

**2. Pobierz szczegóły zgłoszenia:**
```
GET /api/Zgloszenia/1
```

**3. Zmień status:**
```
PATCH /api/Zgloszenia/1/status
Body:
{
  "statusOgolny": "W realizacji",
  "komentarz": "Rozpoczęto pracę"
}
```

**4. Dodaj notatkę:**
```
POST /api/Zgloszenia/1/notatka
Body:
{
  "opis": "Testowa notatka"
}
```

---

## 📚 Endpoints

### 🔐 Auth (`/api/Auth`)

| Method | Endpoint | Opis | Auth? |
|--------|----------|------|-------|
| POST | `/login` | Logowanie (JWT) | ❌ |
| POST | `/refresh` | Odświeżenie tokenu | ❌ |
| POST | `/logout` | Wylogowanie | ✅ |
| GET | `/validate` | Walidacja tokenu | ✅ |

### 📋 Zgłoszenia (`/api/Zgloszenia`)

| Method | Endpoint | Opis | Auth? |
|--------|----------|------|-------|
| GET | `/` | Lista wszystkich | ✅ |
| GET | `/moje` | Moje zgłoszenia | ✅ |
| GET | `/{id}` | Szczegóły | ✅ |
| POST | `/` | Utwórz nowe | ✅ |
| PUT | `/{id}` | Aktualizuj | ✅ |
| PATCH | `/{id}/status` | Zmień status | ✅ |
| POST | `/{id}/notatka` | Dodaj notatkę | ✅ |
| GET | `/{id}/dzialania` | Historia działań | ✅ |
| DELETE | `/{id}` | Usuń | ✅ |

### 👥 Klienci (`/api/Klienci`)

| Method | Endpoint | Opis | Auth? |
|--------|----------|------|-------|
| GET | `/` | Lista klientów | ✅ |
| GET | `/{id}` | Szczegóły klienta | ✅ |
| GET | `/search?query=jan` | Wyszukiwanie | ✅ |
| POST | `/` | Utwórz klienta | ✅ |

---

## 🐛 Troubleshooting

### Problem: "Failed to connect to database"

**Rozwiązanie:**
1. Sprawdź czy MariaDB działa:
   ```bash
   mysql -u root -p
   ```
2. Sprawdź connection string w `appsettings.json`
3. Sprawdź hasło i port (domyślnie 3306)

### Problem: "JWT Secret not configured"

**Rozwiązanie:**
- Dodaj/zmień `JwtSettings:Secret` w `appsettings.json` (min. 32 znaki)

### Problem: "401 Unauthorized" w Swagger

**Rozwiązanie:**
1. Zaloguj się przez `/api/Auth/login`
2. Skopiuj token
3. Kliknij 🔓 Authorize
4. Wpisz: `Bearer TWOJ_TOKEN`

### Problem: "Table doesn't exist"

**Rozwiązanie:**
- Uruchom migrations:
  ```bash
  dotnet ef database update
  ```
- Lub ręcznie utwórz brakujące tabele (patrz sekcja: Przygotowanie bazy danych)

---

## 📊 Monitoring

### Health Check

```bash
curl https://localhost:5001/health
```

Response:
```json
{
  "status": "healthy",
  "timestamp": "2025-01-16T12:00:00Z"
}
```

---

## 🔗 Integracja

### Android App (ENA)

W aplikacji Android ustaw:

```kotlin
// Config.kt
const val API_BASE_URL = "https://localhost:5001/"
```

### Windows Form

W aplikacji Windows Form:

```csharp
// Config
private const string ApiBaseUrl = "https://localhost:5001/api/";
```

---

## 📝 Logi

Logi są wyświetlane w konsoli podczas działania aplikacji.

**Włącz szczegółowe logi** w `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

---

## 🎉 Status

✅ **GOTOWE:**
- JWT Authentication
- CRUD Zgłoszeń
- CRUD Klientów
- Swagger Documentation
- Entity Framework Core
- MySQL/MariaDB Integration

📝 **DO ZROBIENIA:**
- Endpoints dla Produktów
- Upload plików
- Push Notifications
- Rate Limiting
- Logging (Serilog)

---

## 📞 Wsparcie

Jeśli masz pytania:
1. Sprawdź logi w konsoli
2. Sprawdź Swagger UI (`https://localhost:5001/`)
3. Sprawdź connection string w `appsettings.json`

---

**Data:** 2025-01-16  
**Wersja:** 1.0  
**Framework:** .NET 8.0

---

```
╔══════════════════════════════════════════════════════════════╗
║                                                              ║
║   🎉 REST API GOTOWE DO UŻYCIA!                             ║
║                                                              ║
║   1. Zmień connection string (appsettings.json)             ║
║   2. Zmień JWT secret (appsettings.json)                    ║
║   3. Utwórz testowego usera (SQL)                           ║
║   4. Uruchom: dotnet run                                    ║
║   5. Testuj: https://localhost:5001/                        ║
║                                                              ║
╚══════════════════════════════════════════════════════════════╝
```
