# 📱 Rozwiązanie problemu z pobieraniem zwrotów w aplikacji Android

## Problem
Aplikacja Android pokazuje błąd: **"Failed to connect to /10.5.0.106 (port 50875)"**

## Co się stało?
Telefon próbuje połączyć się ze starym adresem IP komputera, który się zmienił.

---

## ✅ ROZWIĄZANIE (5 minut)

### OPCJA 1: Ponowne parowanie przez QR (NAJŁATWIEJSZE) 🎯

1. **Na komputerze:**
   - Uruchom aplikację Desktop
   - Przejdź do: **Ustawienia** → **Paruj telefon**
   - Kliknij przycisk: **📷 PARUJ PRZEZ QR**
   - Pojawi się kod QR

2. **Na telefonie:**
   - Otwórz aplikację **ENA**
   - Zeskanuj kod QR z komputera
   
3. **Gotowe!**
   - System automatycznie zaktualizuje wszystkie ustawienia
   - Możesz zamknąć okno parowania

---

### OPCJA 2: Parowanie ręczne

1. **Na telefonie (aplikacja ENA):**
   - Otwórz aplikację
   - Zapisz **Kod parowania** (6 cyfr)
   - Zapisz **Adres IP telefonu** (np. 192.168.1.100)

2. **Na komputerze:**
   - Otwórz aplikację Desktop
   - Przejdź do: **Ustawienia** → **Paruj telefon**
   - Wpisz **IP telefonu**
   - Wpisz **Kod parowania**
   - Kliknij: **📱 PARUJ TELEFON**

3. **Gotowe!**
   - Poczekaj na komunikat: "Telefon został pomyślnie sparowany"

---

## 🔧 Jeśli nadal nie działa

### Krok 1: Sprawdź czy API działa
1. Otwórz przeglądarkę na komputerze
2. Wpisz adres: `http://localhost:50875/health`
3. Powinieneś zobaczyć: `{"status":"healthy",...}`
4. Jeśli nie - uruchom **ReklamacjeAPI**

### Krok 2: Sprawdź sieć WiFi
1. **Telefon i komputer MUSZĄ być w tej samej sieci WiFi**
2. Sprawdź na telefonie: Ustawienia → WiFi → Nazwa sieci
3. Sprawdź na komputerze: Ustawienia → Sieć i Internet → WiFi → Nazwa sieci
4. Jeśli różne - podłącz oba urządzenia do tej samej sieci

### Krok 3: Uruchom test połączenia
1. Na komputerze otwórz folder projektu
2. Uruchom plik: **TEST_POLACZENIA.bat**
3. Postępuj zgodnie z instrukcjami na ekranie

---

## 💡 Wskazówki

### ✅ Dobre praktyki
- Zawsze używaj **parowania przez QR** - jest szybsze i pewniejsze
- Po sparowaniu telefon automatycznie znajdzie serwer, nawet jeśli IP się zmieni
- Nie musisz przeprowadzać parowania przy każdym uruchomieniu aplikacji

### ⚠️ Częste błędy
- **"Brak adresu API"** - przeprowadź parowanie ponownie
- **"Niepoprawny kod parowania"** - wpisz kod dokładnie tak jak pokazuje telefon
- **Brak połączenia** - sprawdź czy jesteś w tej samej sieci WiFi

---

## 📞 Potrzebujesz pomocy?

1. Uruchom **TEST_POLACZENIA.bat** i zapisz wyniki
2. Sprawdź logi w aplikacji Android (Android Studio → Logcat)
3. Skontaktuj się z administratorem z następującymi informacjami:
   - Wersja aplikacji ENA
   - Wynik testu połączenia
   - Komunikat błędu

---

## 🎉 Po naprawie

Sprawdź czy wszystko działa:
1. Otwórz aplikację ENA na telefonie
2. Przejdź do listy zwrotów
3. Pociągnij w dół aby odświeżyć
4. Zwroty powinny się załadować

**Jeśli załadowało się - problem rozwiązany! 🎉**
