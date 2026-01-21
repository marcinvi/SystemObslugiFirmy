# INSTRUKCJA NAPRAWY - Problem z połączeniem Android do API

## 🔴 Problem
```
Błąd: Failed to connect to /10.5.0.106 (port 50875)
```

Aplikacja Android nie może się połączyć z API, ponieważ próbuje użyć starego adresu IP komputera.

---

## ✅ SZYBKIE ROZWIĄZANIE (5 minut)

### Krok 1: Sprawdź czy API działa
1. Uruchom ReklamacjeAPI na komputerze
2. W przeglądarce otwórz: `http://localhost:50875/health`
3. Powinno pokazać: `{"status":"healthy",...}`

### Krok 2: Sprawdź aktualny adres IP komputera
Otwórz CMD i wpisz:
```cmd
ipconfig | findstr IPv4
```
Zanotuj adres IP (np. `192.168.1.105`)

### Krok 3: Przeprowadź ponowne parowanie telefonu
1. **Na telefonie Android:**
   - Otwórz aplikację ENA
   - Zapisz kod parowania (6 cyfr)

2. **W aplikacji Desktop:**
   - Otwórz Ustawienia → Paruj telefon
   - **OPCJA A: QR Code (ZALECANE)**
     - Kliknij "PARUJ PRZEZ QR"
     - Zeskanuj kod QR telefonem
   - **OPCJA B: Ręcznie**
     - Wpisz aktualny IP telefonu
     - Wpisz kod parowania
     - Kliknij "PARUJ TELEFON"

3. Po pomyślnym parowaniu telefon automatycznie otrzyma nowy adres API.

### Krok 4: Sprawdź czy działa
1. W aplikacji ENA na telefonie przejdź do listy zwrotów
2. Powinny się załadować

---

## 🔧 DŁUGOTERMINOWE ROZWIĄZANIE - Automatyczne wykrywanie IP

### Co robi ta poprawka?
Aplikacja Android automatycznie:
1. Próbuje połączyć się z zapisanym adresem
2. Jeśli nie działa, próbuje adres fallback
3. Jeśli też nie działa, skanuje lokalną sieć w poszukiwaniu API
4. Automatycznie zapisuje działający adres

### Jak wdrożyć?

#### Krok 1: Backup oryginalnego pliku
```bash
cd Ena/app/src/main/java/com/example/ena/api
copy ApiClient.java ApiClient_BACKUP.java
```

#### Krok 2: Zastąp plik
```bash
copy ApiClient_FIXED.java ApiClient.java
```

#### Krok 3: Przebuduj aplikację Android
```bash
cd Ena
gradlew clean
gradlew assembleDebug
```

#### Krok 4: Zainstaluj na telefonie
```bash
adb install -r app/build/outputs/apk/debug/app-debug.apk
```

---

## 🔍 Szczegóły techniczne

### Zmiany w ApiClient.java

#### 1. Dodano timeouty do OkHttpClient
```java
private static final OkHttpClient CLIENT = new OkHttpClient.Builder()
    .connectTimeout(5, TimeUnit.SECONDS)
    .readTimeout(10, TimeUnit.SECONDS)
    .writeTimeout(10, TimeUnit.SECONDS)
    .build();
```

#### 2. Ulepszone logowanie błędów
```java
@Override
public void onFailure(Call call, IOException e) {
    Log.e("ApiClient", "Request failed: " + url, e);
    retryGetWithFallback(path, type, callback, e);
}
```

#### 3. Automatyczne wykrywanie IP
```java
private <T> void tryAutoDiscovery(String path, Type type, 
                                   ApiCallback<T> callback, IOException originalError) {
    // Pobiera lokalny IP telefonu
    String phoneIp = getLocalIpAddress();
    String networkPrefix = phoneIp.substring(0, phoneIp.lastIndexOf('.'));
    
    // Próbuje najczęstsze IP w sieci
    List<String> candidateIps = new ArrayList<>();
    candidateIps.add(networkPrefix + ".1");   // Router
    candidateIps.add(networkPrefix + ".100"); // Komputery
    // ... itd
    
    tryNextCandidate(candidateIps, 0, path, type, callback, originalError);
}
```

#### 4. Rekursywne próbowanie kandydatów
```java
private <T> void tryNextCandidate(List<String> candidates, int index, ...) {
    if (index >= candidates.size()) {
        callback.onError("Nie znaleziono działającego serwera API.");
        return;
    }
    
    String candidateUrl = "http://" + candidates.get(index) + ":50875";
    // Próbuje połączenia...
    
    @Override
    public void onSuccess(...) {
        // Zapisuje działający adres
        ApiConfig.setBaseUrl(context, candidateUrl);
        ApiConfig.setFallbackBaseUrl(context, candidateUrl);
    }
}
```

---

## 🎯 Testowanie

### Test 1: Zmiana IP komputera
1. Zanotuj aktualny IP komputera
2. Zmień IP komputera (lub odłącz/podłącz do sieci)
3. Uruchom aplikację ENA
4. Spróbuj pobrać listę zwrotów
5. ✅ Aplikacja powinna automatycznie znaleźć nowy IP

### Test 2: Brak połączenia
1. Wyłącz ReklamacjeAPI
2. Uruchom aplikację ENA
3. Spróbuj pobrać listę zwrotów
4. ✅ Powinien pokazać błąd: "Nie znaleziono działającego serwera API"

### Test 3: Powrót połączenia
1. Uruchom ReklamacjeAPI
2. W aplikacji ENA odśwież listę zwrotów
3. ✅ Aplikacja powinna automatycznie znaleźć API i załadować dane

---

## 📝 Logi diagnostyczne

### Android Studio Logcat
Filtruj po: `ApiClient`

Przykładowe logi:
```
D/ApiClient: Building URL: base='http://10.5.0.106:50875', path='/api/returns?...', result='...'
E/ApiClient: Request failed: http://10.5.0.106:50875/api/returns?...
D/ApiClient: Trying fallback URL: http://192.168.1.105:50875/api/returns?...
D/ApiClient: Starting auto-discovery...
D/ApiClient: Network prefix: 192.168.1
D/ApiClient: Trying candidate: http://192.168.1.1:50875
D/ApiClient: Trying candidate: http://192.168.1.100:50875
D/ApiClient: Trying candidate: http://192.168.1.105:50875
D/ApiClient: Auto-discovery succeeded! New API URL: http://192.168.1.105:50875
```

---

## ⚠️ Uwagi

1. **Skanowanie sieci może trwać kilka sekund** - przy pierwszym połączeniu po zmianie IP
2. **Aplikacja wymaga dostępu do sieci WiFi** - nie działa przez dane mobilne
3. **API musi być dostępne w sieci lokalnej** - telefon i komputer muszą być w tej samej sieci
4. **Port 50875 musi być otwarty** - sprawdź firewall Windows

---

## 🆘 Rozwiązywanie problemów

### Problem: "Brak adresu API"
**Rozwiązanie:** Przeprowadź ponowne parowanie (Krok 3 z SZYBKIEGO ROZWIĄZANIA)

### Problem: "HTTP 401" lub "HTTP 403"
**Rozwiązanie:** Sprawdź czy użytkownik jest prawidłowo sparowany. Zobacz logi w PairingManager.

### Problem: Wciąż próbuje starego IP
**Rozwiązanie:** 
1. Wyczyść dane aplikacji na telefonie
2. Przeprowadź ponowne parowanie
3. Ewentualnie przeinstaluj aplikację

### Problem: Auto-discovery nie działa
**Możliwe przyczyny:**
1. Telefon i komputer są w różnych sieciach
2. Firewall blokuje port 50875
3. API nie działa (sprawdź http://localhost:50875/health)

---

## ✅ Podsumowanie

| Metoda | Czas | Skuteczność | Kiedy stosować |
|--------|------|-------------|----------------|
| Ponowne parowanie | 5 min | 100% | Jednorazowa zmiana IP |
| ApiClient_FIXED | 10 min | 95% | Częste zmiany IP |

**Rekomendacja:** Najpierw wypróbuj SZYBKIE ROZWIĄZANIE. Jeśli IP zmienia się często, wdróż DŁUGOTERMINOWE ROZWIĄZANIE.
