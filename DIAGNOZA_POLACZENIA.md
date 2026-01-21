# Diagnoza problemu połączenia z API

## Problem
Błąd: `Failed to connect to /10.5.0.106 (port 50875)`

## Analiza przyczyny

### 1. Przepływ URL podczas parowania:

**Desktop → Android:**
```
FormParujTelefon.ResolveApiBaseUrl() 
→ App.config: "http://localhost:50875"
→ ReplaceLoopbackHost() zamienia "localhost" na IP komputera (np. "10.5.0.106")
→ Wysyła: "http://10.5.0.106:50875"
```

**Android zapisuje:**
```java
// BackgroundService.java - endpoint /pair/config
ApiConfig.setBaseUrl(context, apiBaseUrl.trim());
// Problem: NIE ustawia fallbackBaseUrl!
```

### 2. Android próbuje użyć API:

**Pierwszy request (ApiClient.java):**
```java
String base = ApiConfig.getBaseUrl(context); // "http://10.5.0.106:50875"
get("api/returns" + query, type, callback);
```

**Jeśli połączenie się nie powiedzie:**
```java
retryGetWithFallback(path, type, callback, e);
  ↓
String fallback = ApiConfig.getFallbackBaseUrl(context); // PUSTY!
String fallbackUrl = buildUrlWithBase(fallback, path); // NULL lub źle sformatowany!
```

## Znalezione problemy:

### Problem 1: Fallback URL nie jest ustawiony
W `BackgroundService.java` endpoint `/pair/config`:
- ✅ Ustawia `baseUrl`
- ❌ NIE ustawia `fallbackBaseUrl`

### Problem 2: Mechanizm fallback może nadpisać poprawny URL
W `ApiClient.java` metoda `retryGetWithFallback`:
```java
if (response.isSuccessful()) {
    ApiConfig.setBaseUrl(context, fallback); // ← NIEBEZPIECZNE!
}
```
Jeśli fallback jest pusty, może zapisać pusty string jako baseUrl!

### Problem 3: Brak walidacji URL w ApiConfig
`ApiConfig.setBaseUrl()` przyjmuje każdy string, nawet pusty lub źle sformatowany.

## Rozwiązanie

### Krok 1: Popraw BackgroundService.java
Dodaj ustawienie fallbackBaseUrl w metodzie obsługującej `/pair/config`.

### Krok 2: Dodaj walidację URL w ApiClient.java
Sprawdź czy fallback URL nie jest pusty przed próbą użycia.

### Krok 3: Dodaj walidację w metodach retry
Upewnij się, że nie nadpisujemy poprawnego URL pustym fallback URL.
