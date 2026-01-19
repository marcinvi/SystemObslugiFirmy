# 🚀 QUICK START GUIDE - Reklamacje API

## ⚡ 5-minutowy start

### Krok 1: Przygotuj bazę danych (2 min)

```bash
# Otwórz MySQL/MariaDB
mysql -u root -p

# Uruchom init script
mysql -u root -p ReklamacjeDB < init_database.sql
```

### Krok 2: Skonfiguruj API (1 min)

Edytuj `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=ReklamacjeDB;User=root;Password=TWOJE_HASLO;"
  },
  "JwtSettings": {
    "Secret": "WYGENERUJ-LOSOWY-64-ZNAKOWY-KLUCZ-ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
  }
}
```

### Krok 3: Uruchom API (1 min)

```bash
cd ReklamacjeAPI
dotnet restore
dotnet run
```

Otwórz przeglądarkę: `https://localhost:5001/`

### Krok 4: Testuj! (1 min)

**W Swagger UI:**

1. **Zaloguj się:** POST `/api/Auth/login`
   ```json
   {
     "login": "admin",
     "password": "test123"
   }
   ```

2. **Skopiuj token** z response

3. **Kliknij 🔓 Authorize**, wpisz: `Bearer TWOJ_TOKEN`

4. **Testuj endpoints:** GET `/api/Zgloszenia/moje`

---

## ✅ Gotowe!

**API działa na:**
- Swagger UI: `https://localhost:5001/`
- API endpoint: `https://localhost:5001/api/`

**Adres dla innych urządzeń (telefon/tablet/inna stacja):**
```
http://<IP_SERWERA>:5000
https://<IP_SERWERA>:5001
```

**Skąd wziąć IP?**
- Linux/Mac: `hostname -I`
- Windows: `ipconfig`

Jeśli urządzenia nie widzą API, uruchom z:
```
ASPNETCORE_URLS=http://0.0.0.0:5000;https://0.0.0.0:5001
```

**Dane testowe:**
- Login: `admin` / Hasło: `test123`
- Login: `technik` / Hasło: `test123`

---

## 🔥 Najczęściej używane endpointy

### 1. Login
```bash
POST https://localhost:5001/api/Auth/login
{
  "login": "admin",
  "password": "test123"
}
```

### 2. Moje zgłoszenia
```bash
GET https://localhost:5001/api/Zgloszenia/moje?page=1&pageSize=20
Authorization: Bearer YOUR_TOKEN
```

### 3. Szczegóły zgłoszenia
```bash
GET https://localhost:5001/api/Zgloszenia/1
Authorization: Bearer YOUR_TOKEN
```

### 4. Zmiana statusu
```bash
PATCH https://localhost:5001/api/Zgloszenia/1/status
Authorization: Bearer YOUR_TOKEN
{
  "statusOgolny": "W realizacji",
  "komentarz": "Rozpoczęto naprawę"
}
```

### 5. Dodaj notatkę
```bash
POST https://localhost:5001/api/Zgloszenia/1/notatka
Authorization: Bearer YOUR_TOKEN
{
  "opis": "Wymieniono matrycę"
}
```

---

## 🐛 Szybkie rozwiązywanie problemów

### Problem: "Connection refused"
```bash
# Sprawdź czy MariaDB działa
mysql -u root -p
```

### Problem: "401 Unauthorized"
```
1. Zaloguj się przez /api/Auth/login
2. Skopiuj token
3. W Swagger: Authorize → Bearer TOKEN
```

### Problem: "Table doesn't exist"
```bash
# Uruchom init script
mysql -u root -p ReklamacjeDB < init_database.sql
```

---

## 📚 Więcej informacji

Szczegółowa dokumentacja: `README.md`

---

**Czas: 5 minut | Gotowe do użycia!** 🎉
