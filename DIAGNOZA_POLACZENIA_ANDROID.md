# Diagnoza problemu z połączeniem Android - zwroty

## Problem
Błąd: "Failed to connect to /10.5.0.106 (port 50875)"

## Przyczyna
Aplikacja Android próbuje się połączyć ze starym adresem IP komputera (10.5.0.106:50875), 
który został zapisany podczas poprzedniego parowania. Adres IP komputera prawdopodobnie się zmienił.

## Lokalizacja problemu

### 1. API działa na porcie 50875
- Plik: `ReklamacjeAPI\Properties\launchSettings.json`
- Konfiguracja: `"applicationUrl": "https://0.0.0.0:50876;http://0.0.0.0:50875"`
- API nasłuchuje na WSZYSTKICH interfejsach sieciowych (0.0.0.0)

### 2. Aplikacja Desktop wysyła adres do telefonu
- Plik: `FormParujTelefon.cs`
- Metoda: `ResolveApiBaseUrl()`
- Pobiera IP komputera i tworzy URL: `http://{LOCAL_IP}:50875`

### 3. Telefon zapisuje adres w SharedPreferences
- Plik: `Ena\app\src\main\java\com\example\ena\api\ApiConfig.java`
- Zapisuje adres w: `ena_prefs` -> `base_url`
- Problem: stary adres nie jest automatycznie aktualizowany

### 4. Aplikacja Android używa zapisanego adresu
- Plik: `Ena\app\src\main\java\com\example\ena\api\ApiClient.java`
- Metoda: `buildUrl(String path)`
- Używa: `ApiConfig.getBaseUrl(context)`

## Rozwiązanie

### Opcja 1: Przeprowadzić ponowne parowanie (NAJSZYBSZE)
1. Na telefonie: otwórz aplikację ENA
2. W aplikacji Desktop: Ustawienia -> Paruj telefon
3. Zeskanuj kod QR lub wprowadź kod ręcznie
4. System automatycznie zaktualizuje adres IP

### Opcja 2: Dodać automatyczne wykrywanie IP (DŁUGOTERMINOWE)
Zaimplementować mechanizm w aplikacji Android, który:
1. Próbuje połączyć się z zapisanym adresem
2. Jeśli nie działa, skanuje lokalną sieć w poszukiwaniu API
3. Automatycznie aktualizuje adres

### Opcja 3: Dodać mechanizm fallback (POPRAWKA)
Ulepszyć `ApiClient.java` aby:
1. Próbował różnych wariantów IP
2. Sprawdzał dostępność API przed wysłaniem żądania
3. Zapisywał działający adres

## Implementacja rozwiązania

### Krok 1: Sprawdź aktualny adres IP komputera
```bash
ipconfig | findstr IPv4
```

### Krok 2: Sprawdź czy API działa
```bash
curl http://localhost:50875/health
```

### Krok 3: Przeprowadź ponowne parowanie
Instrukcja w aplikacji Desktop: Ustawienia -> Paruj telefon

### Krok 4 (opcjonalnie): Wdróż poprawkę do ApiClient
Zobacz poniższy kod.
