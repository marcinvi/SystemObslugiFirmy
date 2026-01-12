using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Narzędzie do automatycznego dodawania sprawdzania pisowni do wszystkich TextBoxów w projekcie
    /// </summary>
    public class SpellCheckInjector
    {
        private string _projectPath;
        private List<string> _processedFiles = new List<string>();
        private List<string> _errors = new List<string>();

        public SpellCheckInjector(string projectPath)
        {
            _projectPath = projectPath;
        }

        /// <summary>
        /// Przetwarza wszystkie formularze w projekcie
        /// </summary>
        public void ProcessAllForms()
        {
            try
            {
                // Znajdź wszystkie pliki .cs z formularzami
                var csFiles = Directory.GetFiles(_projectPath, "*.cs", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.EndsWith(".Designer.cs") && 
                                !f.EndsWith("SpellCheckHelper.cs") &&
                                !f.EndsWith("SpellCheckControls.cs") &&
                                !f.EndsWith("SpellCheckInjector.cs") &&
                                !f.EndsWith("TextBoxExtensions.cs"))
                    .ToList();

                foreach (var file in csFiles)
                {
                    // Sprawdź czy to jest formularz
                    if (IsFormFile(file))
                    {
                        ProcessFormFile(file);
                    }
                }

                // Wyświetl raport
                ShowReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas przetwarzania: {ex.Message}", "Błąd", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsFormFile(string filePath)
        {
            try
            {
                var content = File.ReadAllText(filePath);
                return content.Contains(": Form") || 
                       content.Contains(": BaseForm") ||
                       content.Contains("partial class Form");
            }
            catch
            {
                return false;
            }
        }

        private void ProcessFormFile(string filePath)
        {
            try
            {
                var content = File.ReadAllText(filePath);
                var className = Path.GetFileNameWithoutExtension(filePath);
                
                // Sprawdź czy formularz już ma metodę EnableSpellCheckOnAllTextBoxes
                if (content.Contains("EnableSpellCheckOnAllTextBoxes"))
                {
                    Console.WriteLine($"Pominięto {className} - już ma sprawdzanie pisowni");
                    return;
                }

                // Znajdź konstruktor
                var constructorPattern = $@"public\s+{Regex.Escape(className)}\s*\([^)]*\)\s*{{";
                var match = Regex.Match(content, constructorPattern);

                if (!match.Success)
                {
                    _errors.Add($"{className}: Nie znaleziono konstruktora");
                    return;
                }

                // Znajdź koniec konstruktora (pierwsze zamknięcie nawiasu klamrowego)
                int constructorStart = match.Index;
                int braceCount = 0;
                int constructorEnd = -1;
                bool inConstructor = false;

                for (int i = constructorStart; i < content.Length; i++)
                {
                    if (content[i] == '{')
                    {
                        braceCount++;
                        inConstructor = true;
                    }
                    else if (content[i] == '}')
                    {
                        braceCount--;
                        if (inConstructor && braceCount == 0)
                        {
                            constructorEnd = i;
                            break;
                        }
                    }
                }

                if (constructorEnd == -1)
                {
                    _errors.Add($"{className}: Nie znaleziono końca konstruktora");
                    return;
                }

                // Dodaj wywołanie metody na końcu konstruktora
                var methodCall = "\r\n\r\n            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów\r\n" +
                                "            EnableSpellCheckOnAllTextBoxes();\r\n        ";

                content = content.Insert(constructorEnd, methodCall);

                // Dodaj metodę EnableSpellCheckOnAllTextBoxes na końcu klasy
                var classEndPattern = @"}\s*}\s*$";
                var classEndMatch = Regex.Match(content, classEndPattern);

                if (classEndMatch.Success)
                {
                    var method = @"
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                // Włącz sprawdzanie pisowni dla wszystkich kontrolek typu TextBox i RichTextBox
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        // Dla zwykłych TextBoxów - bez podkreślania (bo nie obsługują kolorów)
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($""Błąd włączania sprawdzania pisowni: {ex.Message}"");
            }
        }

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;

                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control))
                    {
                        yield return child;
                    }
                }
            }
        }
";

                    int insertPosition = classEndMatch.Index;
                    content = content.Insert(insertPosition, method);
                }

                // Zapisz zmodyfikowany plik
                File.WriteAllText(filePath, content);
                _processedFiles.Add(className);

                Console.WriteLine($"Przetworzono: {className}");
            }
            catch (Exception ex)
            {
                _errors.Add($"{Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        private void ShowReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("===== RAPORT DODAWANIA SPRAWDZANIA PISOWNI =====");
            sb.AppendLine();
            sb.AppendLine($"Przetworzone formularze ({_processedFiles.Count}):");
            foreach (var file in _processedFiles)
            {
                sb.AppendLine($"  ✓ {file}");
            }

            if (_errors.Any())
            {
                sb.AppendLine();
                sb.AppendLine($"Błędy ({_errors.Count}):");
                foreach (var error in _errors)
                {
                    sb.AppendLine($"  ✗ {error}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("===== KONIEC RAPORTU =====");

            var report = sb.ToString();
            Console.WriteLine(report);

            // Zapisz raport do pliku
            File.WriteAllText(Path.Combine(_projectPath, "SpellCheck_Report.txt"), report);

            MessageBox.Show(
                $"Przetworzono {_processedFiles.Count} formularzy.\n" +
                $"Błędów: {_errors.Count}\n\n" +
                "Szczegóły zapisano w pliku SpellCheck_Report.txt",
                "Raport",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
