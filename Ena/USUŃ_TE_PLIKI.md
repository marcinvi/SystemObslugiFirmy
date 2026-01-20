# ⚠️ KRYTYCZNE: Usuń te pliki przed otwarciem w Android Studio

**Problem:** Masz dwie wersje plików build - powodują konflikt!

## 🗑️ USUŃ TE PLIKI (JEŚLI ISTNIEJĄ):

1. `Ena\build.gradle` ← USUŃ (stary Groovy)
2. `Ena\settings.gradle` ← USUŃ (stary Groovy)
3. `Ena\app\build.gradle` ← USUŃ (stary Groovy)

## ✅ ZOSTAW TE PLIKI:

1. `Ena\build.gradle.kts` ✅ (Kotlin DSL - DOBRY)
2. `Ena\settings.gradle.kts` ✅ (Kotlin DSL - DOBRY)
3. `Ena\app\build.gradle.kts` ✅ (Kotlin DSL - DOBRY)

## 📝 KROK PO KROKU:

### **KROK 1: Usuń stare pliki**
```
1. Idź do: C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena
2. Usuń plik: build.gradle (jeśli istnieje)
3. Usuń plik: settings.gradle (jeśli istnieje)
4. Idź do: Ena\app
5. Usuń plik: build.gradle (jeśli istnieje)
```

### **KROK 2: Usuń cache**
```
1. Usuń folder: Ena\.gradle
2. Usuń folder: Ena\.idea
3. Usuń folder: Ena\build
4. Usuń folder: Ena\app\build
```

### **KROK 3: Otwórz Android Studio**
```
1. Android Studio → File → Close Project (jeśli coś otwarte)
2. File → Open
3. Wybierz: C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena
4. Kliknij OK
5. Poczekaj na Gradle sync (2-3 minuty)
```

### **KROK 4: Jeśli Gradle sync fails**

Sprawdź te ustawienia:

```
File → Settings → Build, Execution, Deployment → Build Tools → Gradle

✅ Gradle JDK: Embedded JDK (C:\Program Files\Android\Android Studio\jbr)
✅ Jeśli Embedded JDK nie działa: wskaż ręcznie JDK 17
✅ Use Gradle from: 'gradle-wrapper.properties' file
```

### **KROK 5: Zainstaluj brakujące SDK**

Jeśli zobaczysz błąd "SDK not found":

```
Tools → SDK Manager
→ SDK Platforms
→ Zaznacz: Android 14.0 (API 34)
→ SDK Tools
→ Zaznacz: Android SDK Build-Tools 34
→ Apply → OK
```

## 🚨 JEŚLI NADAL NIE DZIAŁA:

### **Opcja A: Pełny reset Android Studio**

1. Zamknij Android Studio
2. Usuń folder: `C:\Users\mpaprocki\.AndroidStudio*`
3. Usuń folder: `C:\Users\mpaprocki\.gradle`
4. Uruchom ponownie Android Studio
5. Otwórz projekt

### **Opcja B: Stwórz projekt od nowa**

Jeśli nic nie pomaga, mogę Ci pomóc stworzyć nowy projekt i przenieść kod.

## ✅ JAK SPRAWDZIĆ ŻE DZIAŁA:

Po otwarciu projektu sprawdź:

1. **Gradle sync przeszedł** - Bez błędów w Build Output
2. **Struktura projektu widoczna** - Widzisz foldery app/src/main/java
3. **Kompilacja działa** - Build → Make Project (Ctrl+F9)

## 📝 CO ZMIENIŁEM:

1. ✅ gradle.properties - Zmieniono SDK z 36 na 34
2. ✅ Pliki build są w Kotlin DSL (.kts)
3. ⚠️ Musisz usunąć stare pliki .gradle (Groovy), jeśli istnieją

---

**Usuń pliki .gradle (Groovy) i spróbuj ponownie!**
