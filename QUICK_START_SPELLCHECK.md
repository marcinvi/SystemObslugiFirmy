# 🔤 Sprawdzanie Pisowni - Szybki Start

## ✅ Co zostało dodane?

System sprawdzania pisowni po polsku dla wszystkich TextBoxów i RichTextBoxów w aplikacji.

### Nowe pliki:
- ✅ `SpellCheckHelper.cs` - główna logika sprawdzania
- ✅ `TextBoxExtensions.cs` - rozszerzenia dla TextBox
- ✅ `SpellCheckControls.cs` - niestandardowe kontrolki
- ✅ `SpellCheckInjector.cs` - automatyczne narzędzie
- ✅ `FormSpellCheckTest.cs` - formularz testowy
- ✅ `SPELLCHECK_README.md` - pełna dokumentacja
- ✅ `PROGRAM_CS_EXAMPLE.cs` - przykład integracji
- ✅ `AnalyzeTextBoxes.ps1` - skrypt analizy

## 🚀 Jak użyć? (3 kroki)

### Krok 1: Uruchom narzędzie automatyczne

**Opcja A - Przez formularz:**
```
1. Uruchom aplikację
2. Otwórz FormSpellCheckTest
3. Kliknij "Dodaj sprawdzanie pisowni do wszystkich formularzy"
```

**Opcja B - Przez parametr:**
```bash
YourApp.exe --setup-spellcheck
```

**Opcja C - Przez kod:**
```csharp
var injector = new SpellCheckInjector(Application.StartupPath);
injector.ProcessAllForms();
```

### Krok 2: Przebuduj projekt

```
Build -> Rebuild Solution
```

### Krok 3: Gotowe! ✨

Wszystkie TextBoxy mają teraz sprawdzanie pisowni!

## 📋 Funkcje dla użytkownika

### 1. Automatyczne podkreślanie błędów (RichTextBox)
- Błędne słowa są podkreślone na czerwono
- Działa w czasie rzeczywistym podczas pisania

### 2. Menu kontekstowe (PPM na podkreślone słowo)
- **Sugestie poprawek** - do 10 propozycji
- **Dodaj do słownika** - zapisz słowo jako poprawne
- **Ignoruj** - pomiń błąd w tej sesji

### 3. Własny słownik
- Dodane słowa zapisywane w `custom_dictionary.txt`
- Współdzielony między wszystkimi formularzami
- Można wyczyścić przez FormSpellCheckTest

## 🎯 Przykłady użycia

### Przykład 1: Pojedynczy TextBox

```csharp
public Form1()
{
    InitializeComponent();
    
    // Włącz sprawdzanie dla RichTextBox (z podkreślaniem)
    richTextBox1.EnableSpellCheck(true);
    
    // Włącz dla TextBox (bez podkreślania, tylko menu)
    textBox1.EnableSpellCheck(false);
}
```

### Przykład 2: Wszystkie TextBoxy w formularzu

```csharp
public Form1()
{
    InitializeComponent();
    
    // Automatycznie dodane przez narzędzie
    EnableSpellCheckOnAllTextBoxes();
}
```

### Przykład 3: Nowa kontrolka SpellCheckRichTextBox

```csharp
// W Designer.cs zamień:
this.richTextBox1 = new System.Windows.Forms.RichTextBox();

// na:
this.richTextBox1 = new Reklamacje_Dane.SpellCheckRichTextBox();
```

## ⚙️ Konfiguracja

### Wyłącz sprawdzanie dla konkretnego TextBox

```csharp
textBox1.DisableSpellCheck();

// Lub:
spellCheckRichTextBox1.SpellCheckEnabled = false;
```

### Sprawdź tekst programowo

```csharp
var errors = SpellCheckHelper.Instance.CheckText("Tekst do sprawdzenia");
foreach (var error in errors)
{
    Console.WriteLine($"Błąd: {error.Word}");
    Console.WriteLine($"Sugestie: {string.Join(", ", error.Suggestions)}");
}
```

### Dodaj słowo do słownika

```csharp
SpellCheckHelper.Instance.AddToCustomDictionary("noweSlowo");
```

## 🔧 Rozwiązywanie problemów

### ❌ Sprawdzanie nie działa

**Przyczyna:** Brak plików słownika
**Rozwiązanie:**
```
1. Sprawdź czy pl_PL.aff i pl_PL.dic są w folderze aplikacji
2. Sprawdź czy NHunspell.dll jest dostępny
3. Uruchom FormSpellCheckTest -> "Test sprawdzania pisowni"
```

### ❌ Błędy kompilacji

**Przyczyna:** Projekt nie został przebudowany
**Rozwiązanie:**
```
Build -> Clean Solution
Build -> Rebuild Solution
```

### ❌ Słownik nie zapisuje się

**Przyczyna:** Brak uprawnień zapisu
**Rozwiązanie:**
```
Uruchom aplikację jako administrator (jeden raz)
```

## 📊 Analiza projektu

Sprawdź ile kontrolek wymaga aktualizacji:

```powershell
# W PowerShell
.\AnalyzeTextBoxes.ps1

# Wygeneruje raport TextBox_Analysis_Report.txt
```

## 📚 Dokumentacja

Pełna dokumentacja: `SPELLCHECK_README.md`
Przykład integracji: `PROGRAM_CS_EXAMPLE.cs`

## ✨ Co dalej?

1. **Przetestuj** - Uruchom FormSpellCheckTest
2. **Sprawdź** - Otwórz dowolny formularz z TextBox
3. **Użyj** - Napisz tekst z błędem i kliknij PPM

## 💡 Wskazówki

- **RichTextBox** = pełne podkreślanie błędów
- **TextBox** = tylko menu kontekstowe (bez podkreślania)
- **SpellCheckRichTextBox** = automatyczne sprawdzanie
- **Własny słownik** = współdzielony między formularzami

## ⚡ Wydajność

System jest zoptymalizowany:
- ✅ Singleton pattern dla SpellCheckHelper
- ✅ Jednorazowe ładowanie słownika
- ✅ Sprawdzanie tylko zmienionych kontrolek
- ✅ Brak wpływu na UX

## 📝 Licencja

Wykorzystuje:
- NHunspell (LGPL/MPL)
- Słownik pl_PL (GPL/LGPL/MPL)

---

**Pytania?** Zobacz `SPELLCHECK_README.md` dla szczegółów.
