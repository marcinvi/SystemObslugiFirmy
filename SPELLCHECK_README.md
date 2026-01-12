# System sprawdzania pisowni po polsku - Dokumentacja

## Przegląd

System sprawdzania pisowni został dodany do projektu i wykorzystuje bibliotekę **NHunspell** ze słownikiem języka polskiego. System oferuje:

- ✅ Automatyczne sprawdzanie pisowni w czasie rzeczywistym
- ✅ Podkreślanie błędów na czerwono (RichTextBox)
- ✅ Menu kontekstowe z sugestiami poprawek (PPM)
- ✅ Możliwość dodawania słów do własnego słownika
- ✅ Obsługa wszystkich TextBoxów i RichTextBoxów w projekcie

## Pliki dodane do projektu

1. **SpellCheckHelper.cs** - Główna klasa do sprawdzania pisowni (singleton)
2. **TextBoxExtensions.cs** - Rozszerzona z metodami EnableSpellCheck/DisableSpellCheck
3. **SpellCheckControls.cs** - Niestandardowe kontrolki (SpellCheckRichTextBox, SpellCheckTextBox)
4. **SpellCheckInjector.cs** - Narzędzie do automatycznego dodawania sprawdzania do wszystkich formularzy
5. **FormSpellCheckTest.cs** - Formularz testowy i narzędzie konfiguracyjne

## Pliki słownika

- **pl_PL.aff** - Plik reguł słownika polskiego (już w projekcie)
- **pl_PL.dic** - Słownik podstawowy języka polskiego (już w projekcie)
- **custom_dictionary.txt** - Słownik własny użytkownika (tworzony automatycznie)

## Szybki start - Automatyczne dodanie do wszystkich formularzy

### Metoda 1: Użycie FormSpellCheckTest (ZALECANE)

1. Uruchom aplikację
2. Otwórz formularz `FormSpellCheckTest`
3. Kliknij przycisk **"Dodaj sprawdzanie pisowni do wszystkich formularzy"**
4. Potwierdź operację
5. Przebuduj projekt (Build -> Rebuild Solution)
6. Gotowe! Wszystkie TextBoxy mają teraz sprawdzanie pisowni

### Metoda 2: Kod programistyczny

```csharp
// W Program.cs lub dowolnym miejscu startowym:
var injector = new SpellCheckInjector(Application.StartupPath);
injector.ProcessAllForms();
```

## Ręczne dodanie sprawdzania pisowni

### Opcja 1: Dla pojedynczego TextBox/RichTextBox

W konstruktorze formularza (po InitializeComponent):

```csharp
public Form1()
{
    InitializeComponent();
    
    // Dla RichTextBox - z podkreślaniem błędów
    richTextBox1.EnableSpellCheck(true);
    
    // Dla zwykłego TextBox - bez podkreślania (tylko menu kontekstowe)
    textBox1.EnableSpellCheck(false);
}
```

### Opcja 2: Dla wszystkich TextBoxów w formularzu

Dodaj tę metodę do formularza:

```csharp
private void EnableSpellCheckOnAllTextBoxes()
{
    foreach (Control control in GetAllControls(this))
    {
        if (control is RichTextBox richTextBox)
        {
            richTextBox.EnableSpellCheck(true);
        }
        else if (control is TextBox textBox)
        {
            textBox.EnableSpellCheck(false);
        }
    }
}

private IEnumerable<Control> GetAllControls(Control container)
{
    foreach (Control control in container.Controls)
    {
        yield return control;
        if (control.HasChildren)
        {
            foreach (Control child in GetAllControls(control))
                yield return child;
        }
    }
}
```

I wywołaj w konstruktorze:

```csharp
public Form1()
{
    InitializeComponent();
    EnableSpellCheckOnAllTextBoxes();
}
```

### Opcja 3: Użycie niestandardowych kontrolek

W Designer.cs zamień:

```csharp
// Zamiast:
private System.Windows.Forms.RichTextBox richTextBox1;

// Użyj:
private Reklamacje_Dane.SpellCheckRichTextBox richTextBox1;
```

```csharp
// Zamiast:
this.richTextBox1 = new System.Windows.Forms.RichTextBox();

// Użyj:
this.richTextBox1 = new Reklamacje_Dane.SpellCheckRichTextBox();
```

## Funkcje użytkownika

### Sprawdzanie pisowni w czasie rzeczywistym

- Błędnie napisane słowa są automatycznie podkreślane na czerwono (tylko RichTextBox)
- System sprawdza pisownię podczas wpisywania tekstu

### Menu kontekstowe (PPM)

Kliknij prawym przyciskiem myszy na podkreślone słowo:

1. **Sugestie poprawek** (do 10 propozycji) - kliknij aby zastąpić słowo
2. **Dodaj "[słowo]" do słownika** - dodaje słowo do słownika własnego
3. **Ignoruj** - ignoruje błąd w bieżącej sesji

### Słownik własny

- Słowa dodane do słownika są zapisywane w pliku `custom_dictionary.txt`
- Słownik jest współdzielony między wszystkimi formularzami
- Można wyczyścić słownik używając FormSpellCheckTest

## Konfiguracja

### Wyłączenie sprawdzania pisowni dla konkretnego TextBox

```csharp
// Wyłącz sprawdzanie
textBox1.DisableSpellCheck();

// Lub dla SpellCheckRichTextBox/SpellCheckTextBox:
spellCheckRichTextBox1.SpellCheckEnabled = false;
```

### Programowe sprawdzanie tekstu

```csharp
var spellChecker = SpellCheckHelper.Instance;
spellChecker.Initialize();

// Sprawdź pojedyncze słowo
bool isCorrect = spellChecker.IsCorrect("słowo");

// Pobierz sugestie
var suggestions = spellChecker.GetSuggestions("słowo");

// Sprawdź cały tekst
var errors = spellChecker.CheckText("Jakiś tekst do sprawdzenia");
foreach (var error in errors)
{
    Console.WriteLine($"Błąd: {error.Word} na pozycji {error.StartIndex}");
    Console.WriteLine($"Sugestie: {string.Join(", ", error.Suggestions)}");
}
```

### Dodanie słowa do słownika programowo

```csharp
SpellCheckHelper.Instance.AddToCustomDictionary("noweSlowo");
```

## Rozwiązywanie problemów

### Sprawdzanie pisowni nie działa

1. Sprawdź czy pliki `pl_PL.aff` i `pl_PL.dic` są w folderze aplikacji
2. Upewnij się, że NHunspell.dll jest dostępny
3. Sprawdź czy `SpellCheckHelper.Instance.Initialize()` zwraca `true`

### Błędy przy budowaniu projektu

1. Przebuduj cały projekt (Build -> Rebuild Solution)
2. Sprawdź czy wszystkie pliki są dodane do projektu
3. Upewnij się, że pakiet NHunspell jest zainstalowany

### Słownik nie zapisuje się

1. Sprawdź uprawnienia do zapisu w folderze aplikacji
2. Plik `custom_dictionary.txt` powinien być w folderze z aplikacją

## Wydajność

System jest zoptymalizowany pod kątem wydajności:

- Singleton pattern dla SpellCheckHelper (jedna instancja dla całej aplikacji)
- Słownik własny ładowany raz przy starcie
- Sprawdzanie tekstu tylko dla zmienionych kontrolek
- Odłożone sprawdzanie podczas szybkiego pisania

## Techniczne szczegóły

### Używane technologie

- **NHunspell 1.2.5554.16953** - Biblioteka sprawdzania pisowni
- **Słownik pl_PL** - Polski słownik OpenOffice/LibreOffice
- **WinForms** - Natywne kontrolki Windows Forms

### Architektura

```
SpellCheckHelper (Singleton)
    ↓
TextBoxExtensions (Metody rozszerzające)
    ↓
SpellCheckControls (Niestandardowe kontrolki)
    ↓
Formularze aplikacji
```

### Wzorce projektowe

- **Singleton** - SpellCheckHelper
- **Extension Methods** - TextBoxExtensions
- **Factory** - SpellCheckControls
- **Observer** - Event handlers dla TextChanged

## FAQ

**Q: Czy mogę używać innych języków?**
A: Tak, wystarczy dodać odpowiednie pliki .aff i .dic dla danego języka i zmodyfikować ścieżki w SpellCheckHelper.

**Q: Czy sprawdzanie działa dla TextBox?**
A: Tak, ale bez podkreślania błędów (tylko menu kontekstowe). Dla pełnej funkcjonalności użyj RichTextBox.

**Q: Czy mogę wyłączyć sprawdzanie dla wszystkich kontrolek?**
A: Tak, nie wywołuj EnableSpellCheck() lub ustaw SpellCheckEnabled = false dla niestandardowych kontrolek.

**Q: Jak zaktualizować słownik polski?**
A: Pobierz nowe pliki pl_PL.aff i pl_PL.dic ze strony LibreOffice i zastąp istniejące pliki.

**Q: Czy słownik własny jest współdzielony między użytkownikami?**
A: Nie, każdy użytkownik ma swój własny słownik w folderze aplikacji.

## Przykłady użycia

### Przykład 1: Prosty formularz z sprawdzaniem

```csharp
public partial class FormNotatka : Form
{
    public FormNotatka()
    {
        InitializeComponent();
        
        // Włącz sprawdzanie dla pola notatki
        richTextBoxNotatka.EnableSpellCheck(true);
    }
}
```

### Przykład 2: Dynamiczne tworzenie kontrolek

```csharp
var richTextBox = new RichTextBox();
richTextBox.Size = new Size(400, 300);
richTextBox.EnableSpellCheck(true);
this.Controls.Add(richTextBox);
```

### Przykład 3: Sprawdzanie przed zapisem

```csharp
private void btnZapisz_Click(object sender, EventArgs e)
{
    var errors = SpellCheckHelper.Instance.CheckText(richTextBox1.Text);
    
    if (errors.Any())
    {
        var result = MessageBox.Show(
            $"Znaleziono {errors.Count} błędów pisowni. Czy chcesz zapisać mimo to?",
            "Błędy pisowni",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );
        
        if (result == DialogResult.No)
            return;
    }
    
    // Zapis...
}
```

## Kontakt i wsparcie

W razie problemów lub pytań:
1. Sprawdź ten plik README
2. Zobacz FormSpellCheckTest dla przykładów
3. Sprawdź kod źródłowy SpellCheckHelper.cs

## Changelog

### Wersja 1.0 (2026-01-12)
- ✅ Pierwsza wersja systemu sprawdzania pisowni
- ✅ Obsługa języka polskiego
- ✅ Menu kontekstowe z sugestiami
- ✅ Słownik własny
- ✅ Automatyczne narzędzie do dodawania do wszystkich formularzy
- ✅ Formularz testowy i konfiguracyjny

## Licencja

System wykorzystuje:
- NHunspell (LGPL/MPL)
- Słownik pl_PL (GPL/LGPL/MPL - LibreOffice)
