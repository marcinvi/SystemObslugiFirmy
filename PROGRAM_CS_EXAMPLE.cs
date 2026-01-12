/*
 * INSTRUKCJA INTEGRACJI SPRAWDZANIA PISOWNI
 * ==========================================
 * 
 * Aby dodać sprawdzanie pisowni do aplikacji, możesz użyć jednej z metod:
 * 
 * METODA 1 - AUTOMATYCZNA (ZALECANA):
 * ------------------------------------
 * 1. Dodaj w Program.cs przed Application.Run():
 * 
 *    // Pokaż formularz konfiguracji sprawdzania pisowni (tylko raz)
 *    if (args.Contains("--setup-spellcheck"))
 *    {
 *        Application.Run(new FormSpellCheckTest());
 *        return;
 *    }
 * 
 * 2. Uruchom aplikację z parametrem: YourApp.exe --setup-spellcheck
 * 3. Kliknij przycisk "Dodaj sprawdzanie pisowni do wszystkich formularzy"
 * 4. Przebuduj projekt
 * 
 * 
 * METODA 2 - PROGRAMISTYCZNA:
 * ----------------------------
 * Dodaj w Program.cs przed Application.Run():
 * 
 *    // Inicjalizuj sprawdzanie pisowni
 *    SpellCheckHelper.Instance.Initialize();
 * 
 * Następnie w każdym formularzu w konstruktorze (po InitializeComponent):
 * 
 *    public FormXYZ()
 *    {
 *        InitializeComponent();
 *        
 *        // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
 *        EnableSpellCheckOnAllTextBoxes();
 *    }
 *    
 *    private void EnableSpellCheckOnAllTextBoxes()
 *    {
 *        foreach (Control control in GetAllControls(this))
 *        {
 *            if (control is RichTextBox richTextBox)
 *                richTextBox.EnableSpellCheck(true);
 *            else if (control is TextBox textBox)
 *                textBox.EnableSpellCheck(false);
 *        }
 *    }
 *    
 *    private IEnumerable<Control> GetAllControls(Control container)
 *    {
 *        foreach (Control control in container.Controls)
 *        {
 *            yield return control;
 *            if (control.HasChildren)
 *            {
 *                foreach (Control child in GetAllControls(control))
 *                    yield return child;
 *            }
 *        }
 *    }
 * 
 * 
 * METODA 3 - JEDEN RAZ DLA WSZYSTKICH:
 * -------------------------------------
 * Uruchom ten kod raz, aby dodać sprawdzanie do wszystkich formularzy:
 * 
 *    var injector = new SpellCheckInjector(Application.StartupPath
 *        .Replace("\\bin\\Debug", "")
 *        .Replace("\\bin\\Release", ""));
 *    injector.ProcessAllForms();
 * 
 * Przebuduj projekt i gotowe!
 * 
 * 
 * PRZYKŁAD Program.cs:
 * --------------------
 */

using System;
using System.Linq;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ===== KONFIGURACJA SPRAWDZANIA PISOWNI =====
            
            // OPCJA A: Pokaż formularz testowy sprawdzania pisowni
            if (args.Contains("--spellcheck-test"))
            {
                Application.Run(new FormSpellCheckTest());
                return;
            }

            // OPCJA B: Automatycznie dodaj sprawdzanie do wszystkich formularzy (uruchom raz)
            if (args.Contains("--setup-spellcheck"))
            {
                var injector = new SpellCheckInjector(
                    Application.StartupPath
                        .Replace("\\bin\\Debug", "")
                        .Replace("\\bin\\Release", ""));
                injector.ProcessAllForms();
                
                MessageBox.Show(
                    "Sprawdzanie pisowni zostało dodane do wszystkich formularzy.\n\n" +
                    "WAŻNE: Teraz przebuduj projekt (Build -> Rebuild Solution)!",
                    "Sukces",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // OPCJA C: Inicjalizuj sprawdzanie pisowni normalnie
            try
            {
                // Inicjalizuj system sprawdzania pisowni
                SpellCheckHelper.Instance.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ostrzeżenie: Nie można zainicjalizować sprawdzania pisowni.\n\n{ex.Message}",
                    "Ostrzeżenie",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // ===== KONIEC KONFIGURACJI =====

            // Normalne uruchomienie aplikacji
            Application.Run(new LoginForm()); // Lub twój główny formularz
        }
    }
}

/*
 * PARAMETRY URUCHOMIENIOWE:
 * -------------------------
 * YourApp.exe                    - Normalne uruchomienie aplikacji
 * YourApp.exe --spellcheck-test  - Formularz testowy sprawdzania pisowni
 * YourApp.exe --setup-spellcheck - Automatyczne dodanie sprawdzania do wszystkich formularzy
 * 
 * 
 * JAK URUCHOMIĆ Z PARAMETRAMI W VISUAL STUDIO:
 * ---------------------------------------------
 * 1. Prawym przyciskiem na projekt -> Properties
 * 2. Debug -> Command line arguments
 * 3. Wpisz: --setup-spellcheck
 * 4. Uruchom projekt (F5)
 * 5. Po zakończeniu usuń parametr i przebuduj projekt
 */
