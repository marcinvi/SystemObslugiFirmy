using System;
using System.Configuration;
using System.Drawing;
using System.Linq;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Klasa konfiguracyjna dla systemu sprawdzania pisowni
    /// Odczytuje ustawienia z App.config
    /// </summary>
    public static class SpellCheckConfig
    {
        /// <summary>
        /// Czy sprawdzanie pisowni jest włączone globalnie
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["SpellCheck_Enabled"] ?? "true");
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Kod języka słownika (np. pl_PL, en_US)
        /// </summary>
        public static string Language
        {
            get
            {
                return ConfigurationManager.AppSettings["SpellCheck_Language"] ?? "pl_PL";
            }
        }

        /// <summary>
        /// Ścieżka do katalogu ze słownikami (puste = folder aplikacji)
        /// </summary>
        public static string DictionaryPath
        {
            get
            {
                return ConfigurationManager.AppSettings["SpellCheck_DictionaryPath"] ?? "";
            }
        }

        /// <summary>
        /// Maksymalna liczba sugestii do wyświetlenia
        /// </summary>
        public static int MaxSuggestions
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["SpellCheck_MaxSuggestions"] ?? "10");
                }
                catch
                {
                    return 10;
                }
            }
        }

        /// <summary>
        /// Czy podkreślać błędy w tekście
        /// </summary>
        public static bool HighlightErrors
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["SpellCheck_HighlightErrors"] ?? "true");
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Opóźnienie (ms) przed uruchomieniem sprawdzania po wpisaniu tekstu
        /// </summary>
        public static int DebounceMs
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["SpellCheck_DebounceMs"] ?? "800");
                }
                catch
                {
                    return 800;
                }
            }
        }

        /// <summary>
        /// Maksymalna długość tekstu (znaków), dla której podkreślamy błędy
        /// </summary>
        public static int MaxHighlightTextLength
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["SpellCheck_MaxHighlightTextLength"] ?? "4000");
                }
                catch
                {
                    return 4000;
                }
            }
        }

        /// <summary>
        /// Maksymalna liczba podkreślanych błędów (ograniczenie kosztu UI)
        /// </summary>
        public static int MaxHighlightErrors
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["SpellCheck_MaxHighlightErrors"] ?? "200");
                }
                catch
                {
                    return 200;
                }
            }
        }

        /// <summary>
        /// Kolor podkreślenia błędów
        /// </summary>
        public static Color ErrorColor
        {
            get
            {
                try
                {
                    string colorName = ConfigurationManager.AppSettings["SpellCheck_ErrorColor"] ?? "Red";
                    return Color.FromName(colorName);
                }
                catch
                {
                    return Color.Red;
                }
            }
        }

        /// <summary>
        /// Czy pokazywać menu kontekstowe
        /// </summary>
        public static bool EnableContextMenu
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["SpellCheck_ContextMenu"] ?? "true");
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Czy automatycznie zapisywać słownik własny
        /// </summary>
        public static bool AutoSaveCustomDictionary
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["SpellCheck_AutoSaveCustomDictionary"] ?? "true");
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Ścieżka do słownika własnego (puste = custom_dictionary.txt)
        /// </summary>
        public static string CustomDictionaryPath
        {
            get
            {
                return ConfigurationManager.AppSettings["SpellCheck_CustomDictionaryPath"] ?? "";
            }
        }

        /// <summary>
        /// Minimalna długość słowa do sprawdzania
        /// </summary>
        public static int MinWordLength
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["SpellCheck_MinWordLength"] ?? "2");
                }
                catch
                {
                    return 2;
                }
            }
        }

        /// <summary>
        /// Czy ignorować słowa pisane wielkimi literami
        /// </summary>
        public static bool IgnoreAllCaps
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["SpellCheck_IgnoreAllCaps"] ?? "true");
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Czy ignorować słowa zawierające cyfry
        /// </summary>
        public static bool IgnoreWordsWithNumbers
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["SpellCheck_IgnoreWordsWithNumbers"] ?? "true");
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Czy logować błędy sprawdzania pisowni
        /// </summary>
        public static bool EnableLogging
        {
            get
            {
                try
                {
                    return bool.Parse(ConfigurationManager.AppSettings["SpellCheck_EnableLogging"] ?? "false");
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Sprawdza czy słowo powinno być ignorowane na podstawie konfiguracji
        /// </summary>
        public static bool ShouldIgnoreWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return true;

            // Ignoruj zbyt krótkie słowa
            if (word.Length < MinWordLength)
                return true;

            // Ignoruj słowa pisane wielkimi literami
            if (IgnoreAllCaps && word.All(char.IsUpper))
                return true;

            // Ignoruj słowa zawierające cyfry
            if (IgnoreWordsWithNumbers && word.Any(char.IsDigit))
                return true;

            return false;
        }
    }
}
