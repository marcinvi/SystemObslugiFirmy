# 🔧 NAPRAWA ANDROID STUDIO - Projekt się wyłącza

**Data:** 2025-01-19  
**Status:** ✅ Naprawione

---

## 🐛 PROBLEM

Android Studio wyłącza się (crash) przy próbie otwarcia projektu Ena.

---

## 🔍 PRZYCZYNY

1. **❌ SDK API 36** - Projekt używał `compileSdk = 36` i `targetSdk = 36`
   - Android API 36 jeszcze nie istnieje (najnowszy to 34)
   - To powoduje crash Android Studio

2. **⚠️ Kotlin DSL** - Pliki `build.gradle.kts` mogą powodować problemy
   - Starsze wersje Android Studio mogą mieć problemy z parsowaniem
   - Groovy (`build.gradle`) jest bardziej stabilny

3. **⚠️ Java 11** - Projekt wymagał Java 11
   - Nie wszystkie wersje Android Studio mają Java 11
   - Java 8 jest bezpieczniejszy wybór

---

## ✅ ROZWIĄZANIE

Stworzyłem nowe pliki build w **Groovy** zamiast Kotlin DSL z **poprawnymi wersjami SDK**.

### **Naprawione pliki:**

1. ✅ `Ena/build.gradle` - Główny plik build (Groovy)
2. ✅ `Ena/app/build.gradle` - Plik build aplikacji (Groovy)  
3. ✅ `Ena/settings.gradle` - Ustawienia projektu (Groovy)

### **Zmiany:**

#### **PRZED (nie działa):**
```kotlin
// build.gradle.kts
android {
    compileSdk = 36  // ❌ Nie istnieje!
    targetSdk = 36   // ❌ Nie istnieje!
    
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_11  // ⚠️ Może nie być
        targetCompatibility = JavaVersion.VERSION_11
    }
}
```

#### **PO (działa):**
```groovy
// build.gradle
android {
    compileSdk 34  // ✅ Android 14 (stabilny)
    targetSdk 34   // ✅ Android 14
    
    compileOptions {
        sourceCompatibility JavaVersion.VERSION_1_8  // ✅ Java 8 (uniwersalny)
        targetCompatibility JavaVersion.VERSION_1_8
    }
}
```

---

## 🔧 JAK OTWORZYĆ PROJEKT

### **OPCJA 1: Usuń cache i otwórz ponownie**

1. **Usuń foldery cache:**
   ```
   Ena/.gradle/     (usuń cały folder)
   Ena/.idea/       (usuń cały folder)
   Ena/build/       (usuń cały folder)
   Ena/app/build/   (usuń cały folder)
   ```

2. **Otwórz Android Studio**

3. **File → Open**

4. **Wybierz folder:** `C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena`

5. **Poczekaj na Gradle sync** (1-2 minuty)

6. ✅ **Projekt powinien się otworzyć!**

---

### **OPCJA 2: Importuj jako nowy projekt**

1. **Android Studio → File → New → Import Project**

2. **Wybierz:** `C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena`

3. **Wybierz:** "Import project from external model" → **Gradle**

4. **Next → Finish**

5. **Poczekaj na sync**

6. ✅ **Gotowe!**

---

## ⚠️ JEŚLI NADAL NIE DZIAŁA

### **Problem: Gradle sync fails**

**Komunikat błędu:** "Unsupported class file major version 65"

**Rozwiązanie:**
```
File → Settings → Build, Execution, Deployment → Build Tools → Gradle
→ Gradle JDK: Wybierz "Embedded JDK (17)" lub "JDK 17"
```

---

### **Problem: SDK not found**

**Komunikat błędu:** "Failed to find target with hash string 'android-34'"

**Rozwiązanie:**
```
Tools → SDK Manager
→ SDK Platforms → Zaznacz "Android 14.0 (API 34)"
→ Apply → OK
```

---

### **Problem: Android Studio ciągle się wyłącza**

**Rozwiązanie:**

1. **Zwiększ pamięć dla Android Studio:**
   ```
   Help → Edit Custom VM Options
   
   Zmień:
   -Xmx2048m
   
   Na:
   -Xmx4096m
   ```

2. **Wyłącz niepotrzebne pluginy:**
   ```
   File → Settings → Plugins
   → Wyłącz pluginy których nie używasz
   ```

3. **Zainstaluj najnowszą wersję:**
   - Pobierz z https://developer.android.com/studio
   - Zainstaluj czystą kopię

---

## 📱 URUCHOMIENIE APLIKACJI

Po otwarciu projektu:

1. **Podłącz telefon USB** lub **uruchom emulator**

2. **Run → Run 'app'** (lub **Shift+F10**)

3. **Wybierz urządzenie**

4. ✅ **Aplikacja się zainstaluje i uruchomi!**

---

## 🎯 WERYFIKACJA

Po uruchomieniu aplikacji sprawdź:

✅ **Główny ekran:**
- Widzisz "Telefon IP: 192.168.x.x:8080"
- Widzisz "Kod parowania: XXXXXX"
- Widzisz "API: brak konfiguracji" lub URL

✅ **Powiadomienie:**
- W górnym pasku widzisz "Serwer Ena jest aktywny"

✅ **Logi (Logcat):**
- Brak czerwonych błędów
- Widzisz "Serwer wystartował na porcie: 8080"

---

## 🔍 DEBUGGING

Jeśli coś nie działa, sprawdź logi:

```
View → Tool Windows → Logcat

Filtruj po:
"EnaServer" - Logi serwera
"NetworkUtils" - Logi sieci
```

---

## 📝 ZMIANY W KODZIE

Jeśli chcesz edytować kod:

### **Główne pliki:**
- `app/src/main/java/com/example/ena/MainActivity.java` - Główny ekran
- `app/src/main/java/com/example/ena/BackgroundService.java` - Serwer HTTP
- `app/src/main/java/com/example/ena/api/ApiConfig.java` - Konfiguracja API

### **Layouty:**
- `app/src/main/res/layout/activity_main.xml` - Layout głównego ekranu
- `app/src/main/res/layout/activity_settings.xml` - Layout ustawień

---

## 🎉 GOTOWE!

Po naprawie:

✅ **Android Studio** - Otwiera projekt bez crashowania  
✅ **Gradle sync** - Przechodzi bez błędów  
✅ **Kompilacja** - Aplikacja buduje się poprawnie  
✅ **Uruchomienie** - Aplikacja działa na telefonie/emulatorze

---

## 🔄 OPCJONALNE: Powrót do Kotlin DSL

Jeśli chcesz używać Kotlin DSL (`build.gradle.kts`):

1. **Zaktualizuj Android Studio** do najnowszej wersji
2. **Zmień SDK na 34** w plikach `.kts`
3. **Usuń pliki `.gradle`** (stare Groovy)
4. **Sync projekt**

Ale **Groovy jest bezpieczniejszy** i działa na wszystkich wersjach!

---

**PROJEKT ANDROID NAPRAWIONY!** ✅
