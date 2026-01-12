using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NHunspell;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Pomocnicza klasa do sprawdzania pisowni w języku polskim
    /// Wykorzystuje NHunspell i słownik pl_PL
    /// </summary>
    public class SpellCheckHelper : IDisposable
    {
        private static SpellCheckHelper _instance;
        private static readonly object _lock = new object();
        private Hunspell _hunspell;
        private HashSet<string> _customDictionary;
        private string _customDictionaryPath;
        private bool _isInitialized = false;

        private SpellCheckHelper()
        {
            _customDictionary = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _customDictionaryPath = Path.Combine(Application.StartupPath, "custom_dictionary.txt");
            LoadCustomDictionary();
        }

        /// <summary>
        /// Singleton instance klasy SpellCheckHelper
        /// </summary>
        public static SpellCheckHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SpellCheckHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Inicjalizacja sprawdzania pisowni
        /// </summary>
        public bool Initialize()
        {
            if (_isInitialized)
                return true;

            try
            {
                string appPath = Application.StartupPath;
                string affPath = Path.Combine(appPath, "pl_PL.aff");
                string dicPath = Path.Combine(appPath, "pl_PL.dic");

                if (!File.Exists(affPath) || !File.Exists(dicPath))
                {
                    MessageBox.Show(
                        "Nie znaleziono plików słownika polskiego (pl_PL.aff lub pl_PL.dic).\n" +
                        "Sprawdzanie pisowni zostanie wyłączone.",
                        "Brak słownika",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return false;
                }

                _hunspell = new Hunspell(affPath, dicPath);
                _isInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd podczas inicjalizacji sprawdzania pisowni: {ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
        }

        /// <summary>
        /// Sprawdza czy słowo jest poprawnie napisane
        /// </summary>
        public bool IsCorrect(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || !_isInitialized)
                return true;

            // Sprawdź czy słowo jest w słowniku własnym
            if (_customDictionary.Contains(word))
                return true;

            // Sprawdź w słowniku Hunspell
            return _hunspell.Spell(word);
        }

        /// <summary>
        /// Zwraca propozycje poprawek dla błędnego słowa
        /// </summary>
        public List<string> GetSuggestions(string word)
        {
            if (!_isInitialized || string.IsNullOrWhiteSpace(word))
                return new List<string>();

            try
            {
                var suggestions = _hunspell.Suggest(word);
                return suggestions?.Take(10).ToList() ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Dodaje słowo do słownika własnego
        /// </summary>
        public void AddToCustomDictionary(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return;

            _customDictionary.Add(word);
            SaveCustomDictionary();
        }

        /// <summary>
        /// Usuwa słowo ze słownika własnego
        /// </summary>
        public void RemoveFromCustomDictionary(string word)
        {
            if (_customDictionary.Remove(word))
            {
                SaveCustomDictionary();
            }
        }

        /// <summary>
        /// Ładuje słownik własny z pliku
        /// </summary>
        private void LoadCustomDictionary()
        {
            try
            {
                if (File.Exists(_customDictionaryPath))
                {
                    var words = File.ReadAllLines(_customDictionaryPath);
                    foreach (var word in words)
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                        {
                            _customDictionary.Add(word.Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania słownika własnego: {ex.Message}");
            }
        }

        /// <summary>
        /// Zapisuje słownik własny do pliku
        /// </summary>
        private void SaveCustomDictionary()
        {
            try
            {
                File.WriteAllLines(_customDictionaryPath, _customDictionary.OrderBy(w => w));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisywania słownika własnego: {ex.Message}");
            }
        }

        /// <summary>
        /// Sprawdza cały tekst i zwraca listę błędnych słów
        /// </summary>
        public List<SpellCheckError> CheckText(string text)
        {
            var errors = new List<SpellCheckError>();

            if (string.IsNullOrWhiteSpace(text) || !_isInitialized)
                return errors;

            // Podział tekstu na słowa
            int position = 0;
            string currentWord = "";
            int wordStart = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (char.IsLetter(c) || c == '-' || c == '\'')
                {
                    if (currentWord.Length == 0)
                        wordStart = i;
                    currentWord += c;
                }
                else
                {
                    if (currentWord.Length > 0)
                    {
                        if (!IsCorrect(currentWord))
                        {
                            errors.Add(new SpellCheckError
                            {
                                Word = currentWord,
                                StartIndex = wordStart,
                                Length = currentWord.Length,
                                Suggestions = GetSuggestions(currentWord)
                            });
                        }
                        currentWord = "";
                    }
                }
            }

            // Sprawdź ostatnie słowo
            if (currentWord.Length > 0 && !IsCorrect(currentWord))
            {
                errors.Add(new SpellCheckError
                {
                    Word = currentWord,
                    StartIndex = wordStart,
                    Length = currentWord.Length,
                    Suggestions = GetSuggestions(currentWord)
                });
            }

            return errors;
        }

        public void Dispose()
        {
            _hunspell?.Dispose();
            _hunspell = null;
            _isInitialized = false;
        }
    }

    /// <summary>
    /// Klasa reprezentująca błąd pisowni
    /// </summary>
    public class SpellCheckError
    {
        public string Word { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public List<string> Suggestions { get; set; }
    }
}
