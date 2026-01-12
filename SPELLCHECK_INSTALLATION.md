# 📦 INSTALACJA SYSTEMU SPRAWDZANIA PISOWNI - CHECKLIST

## ✅ PLIKI JUŻ W PROJEKCIE (sprawdź czy są)

- [x] **pl_PL.aff** - Plik reguł słownika polskiego
- [x] **pl_PL.dic** - Słownik polski
- [x] **Hunspellx64.dll** - Biblioteka NHunspell (64-bit)
- [x] **Hunspellx86.dll** - Biblioteka NHunspell (32-bit)
- [x] **packages.config** - Zawiera NHunspell v1.2.5554.16953

## 📝 NOWE PLIKI DO DODANIA DO PROJEKTU

### 1. Pliki źródłowe (.cs) - WYMAGANE

Dodaj te pliki do projektu w Visual Studio (Add -> Existing Item):

```
✅ SpellCheckHelper.cs          - Główna logika sprawdzania
✅ TextBoxExtensions.cs          - Metody rozszerzające (ZAKTUALIZOWANY!)
✅ SpellCheckControls.cs         - Niestandardowe kontrolki
✅ SpellCheckInjector.cs         - Automatyczne narzędzie
✅ FormSpellCheckTest.cs         - Formularz testowy
✅ SpellCheckConfig.cs           - Konfiguracja (OPCJONALNY)
```

### 2. Pliki dokumentacji (.md) - ZALECANE

Te pliki NIE muszą być dodane do projektu, ale powinny być w folderze:

```
📄 SPELLCHECK_README.md          - Pełna dokumentacja
📄 QUICK_START_SPELLCHECK.md     - Szybki przewodnik
📄 SPELLCHECK_SUMMARY.md         - Podsumowanie
📄 SPELLCHECK_INSTALLATION.md    - Ten plik
```

### 3. Przykłady i narzędzia - OPCJONALNE

```
📄 PROGRAM_CS_EXAMPLE.cs         - Przykład integracji
📄 APP_CONFIG_SPELLCHECK_EXAMPLE.xml - Przykład konfiguracji
📄 AnalyzeTextBoxes.ps1          - PowerShell script
```

## 🔧 INSTRUKCJA INSTALACJI

### Krok 1: Dodaj pliki do projektu

W Visual Studio:

```
1. Prawy przycisk na projekt -> Add -> Existing Item
2. Zaznacz wszystkie pliki .cs z sekcji "Pliki źródłowe"
3. Kliknij Add
```

Lub przez eksplorator:

```
1. Skopiuj wszystkie pliki .cs do folderu projektu
2. W Visual Studio: Prawy przycisk na projekt -> Add -> Existing Item
3. Wybierz wszystkie skopiowane pliki
```

### Krok 2: Sprawdź referencje

Upewnij się, że projekt ma referencje do:

```
✅ System.Configuration
✅ System.Drawing
✅ System.Windows.Forms
✅ NHunspell (przez NuGet)
```

Jeśli brakuje System.Configuration:

```
1. Prawy przycisk na References -> Add Reference
2. Assemblies -> Framework -> System.Configuration
3. Zaznacz i kliknij OK
```

### Krok 3: Zweryfikuj słowniki

Sprawdź czy te pliki są w folderze projektu:

```
✅ pl_PL.aff
✅ pl_PL.dic
✅ Hunspellx64.dll
✅ Hunspellx86.dll
```

Ustaw ich właściwości (w Solution Explorer):

```
1. Wybierz wszystkie 4 pliki
2. Properties (F4)
3. Copy to Output Directory = "Copy if newer"
```

### Krok 4: Przebuduj projekt

```
Build -> Clean Solution
Build -> Rebuild Solution
```

### Krok 5: Uruchom automatyczne dodawanie

Wybierz jedną z metod:

**Metoda A - Przez formularz:**
```
1. Uruchom aplikację (F5)
2. Otwórz FormSpellCheckTest
3. Kliknij "Dodaj sprawdzanie pisowni do wszystkich formularzy"
```

**Metoda B - Przez parametr:**
```
1. Project Properties -> Debug
2. Command line arguments: --setup-spellcheck
3. Uruchom (F5)
```

**Metoda C - Przez kod:**
```csharp
// Dodaj w Program.cs:
var injector = new SpellCheckInjector(Application.StartupPath);
injector.ProcessAllForms();
```

### Krok 6: Przebuduj projekt ponownie

```
Build -> Rebuild Solution
```

### Krok 7: Testuj!

```
1. Uruchom aplikację
2. Otwórz dowolny formularz z TextBox/RichTextBox
3. Napisz tekst z błędami pisowni
4. Sprawdź czy błędy są podkreślone
5. Kliknij PPM na błędne słowo
6. Zobacz sugestie poprawek
```

## 📋 WERYFIKACJA INSTALACJI

### Test 1: Sprawdź czy pliki są w projekcie

W Solution Explorer powinny być widoczne:

```
📁 YourProject
  ├─ 📄 SpellCheckHelper.cs
  ├─ 📄 TextBoxExtensions.cs
  ├─ 📄 SpellCheckControls.cs
  ├─ 📄 SpellCheckInjector.cs
  ├─ 📄 FormSpellCheckTest.cs
  ├─ 📄 SpellCheckConfig.cs (opcjonalny)
  ├─ 📄 pl_PL.aff
  ├─ 📄 pl_PL.dic
  ├─ 📄 Hunspellx64.dll
  └─ 📄 Hunspellx86.dll
```

### Test 2: Sprawdź czy kompiluje się

```
Build -> Rebuild Solution
```

Powinno być: **0 errors, 0 warnings** (lub tylko standardowe warnings)

### Test 3: Sprawdź czy działa

```
1. Uruchom FormSpellCheckTest
2. Kliknij "Test sprawdzania pisowni"
3. Sprawdź czy błędy są podkreślone
```

## 🐛 ROZWIĄZYWANIE PROBLEMÓW

### Problem: "The type or namespace name 'NHunspell' could not be found"

**Rozwiązanie:**
```
1. Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution
2. Browse -> Szukaj "NHunspell"
3. Install lub Update do v1.2.5554.16953
```

### Problem: "Could not load file or assembly 'System.Configuration'"

**Rozwiązanie:**
```
1. Prawy przycisk na References -> Add Reference
2. Assemblies -> Framework
3. Zaznacz System.Configuration
4. OK
```

### Problem: "Nie znaleziono plików słownika"

**Rozwiązanie:**
```
1. Sprawdź czy pl_PL.aff i pl_PL.dic są w folderze projektu
2. Ustaw ich właściwości: Copy to Output Directory = "Copy if newer"
3. Przebuduj projekt
```

### Problem: "Hunspell.dll not found"

**Rozwiązanie:**
```
1. Sprawdź czy Hunspellx64.dll i Hunspellx86.dll są w folderze projektu
2. Ustaw ich właściwości: Copy to Output Directory = "Copy if newer"
3. Przebuduj projekt
```

### Problem: "TextBoxExtensions already exists"

**Rozwiązanie:**
```
Plik TextBoxExtensions.cs został ZAKTUALIZOWANY (nie zastąpiony).
Jeśli masz konflikt:
1. Zrób backup swojego TextBoxExtensions.cs
2. Zastąp nowym plikiem
3. Ręcznie przenieś swoje własne metody
```

## 📊 STATYSTYKI INSTALACJI

Po instalacji projekt będzie miał:

```
➕ +6 nowych plików źródłowych (.cs)
➕ +2000+ linii nowego kodu
➕ +4 plików dokumentacji
➕ +1 formularz testowy
✅ 100% backwards compatible
✅ 0 zmian w istniejącym kodzie (przed uruchomieniem narzędzia)
```

## ✨ CO NASTĘPNIE?

Po zainstalowaniu:

1. ✅ Przeczytaj `QUICK_START_SPELLCHECK.md`
2. ✅ Uruchom `FormSpellCheckTest` dla testu
3. ✅ Użyj automatycznego narzędzia dla wszystkich formularzy
4. ✅ Przebuduj projekt
5. ✅ Ciesz się sprawdzaniem pisowni!

## 📞 POMOC

Jeśli masz problemy:

1. 📖 Przeczytaj `SPELLCHECK_README.md` (pełna dokumentacja)
2. 📖 Przeczytaj `QUICK_START_SPELLCHECK.md` (szybki start)
3. 🧪 Uruchom `FormSpellCheckTest` (test i diagnostyka)
4. 📊 Uruchom `AnalyzeTextBoxes.ps1` (analiza projektu)

## 🎯 CHECKLIST KOŃCOWY

Przed zakończeniem instalacji, sprawdź:

- [ ] Wszystkie 6 plików .cs są dodane do projektu
- [ ] Projekt kompiluje się bez błędów
- [ ] pl_PL.aff i pl_PL.dic są w folderze projektu
- [ ] Hunspellx64.dll i Hunspellx86.dll są w folderze projektu
- [ ] Wszystkie 4 pliki słownika mają "Copy to Output Directory" = "Copy if newer"
- [ ] System.Configuration jest w referencjach
- [ ] NHunspell jest zainstalowany przez NuGet
- [ ] FormSpellCheckTest działa i pokazuje test
- [ ] Automatyczne narzędzie zostało uruchomione
- [ ] Projekt został przebudowany po uruchomieniu narzędzia
- [ ] Sprawdzanie pisowni działa w formularzach

## ✅ WSZYSTKO GOTOWE!

Jeśli wszystkie punkty checklisty są zaznaczone, instalacja jest zakończona! 🎉

---

**Gratulacje!** System sprawdzania pisowni jest zainstalowany i gotowy do użycia.

Teraz możesz:
- Cieszyć się automatycznym sprawdzaniem pisowni
- Dodawać słowa do własnego słownika
- Używać sugestii poprawek
- Mieć profesjonalną aplikację z obsługą języka polskiego

**Powodzenia!** 🚀
