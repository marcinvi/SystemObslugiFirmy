using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public static class TextBoxExtensions
    {
        private const int EM_SETCUEBANNER = 0x1501;
        private static Dictionary<Control, SpellCheckContext> _spellCheckContexts = new Dictionary<Control, SpellCheckContext>();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        public static void SetPlaceholder(this TextBox textBox, string placeholder)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, placeholder);
        }

        /// <summary>
        /// Włącza sprawdzanie pisowni dla TextBox lub RichTextBox
        /// </summary>
        public static void EnableSpellCheck(this TextBoxBase textBox, bool highlightErrors = true)
        {
            if (textBox == null)
                return;

            // Inicjalizuj SpellChecker jeśli jeszcze nie został
            if (!SpellCheckHelper.Instance.Initialize())
                return;

            // Jeśli już ma włączone sprawdzanie, nie dodawaj ponownie
            if (_spellCheckContexts.ContainsKey(textBox))
                return;

            var context = new SpellCheckContext
            {
                Control = textBox,
                HighlightErrors = highlightErrors,
                ContextMenu = new ContextMenuStrip()
            };

            _spellCheckContexts[textBox] = context;

            // Dodaj obsługę zdarzeń
            textBox.TextChanged += OnTextChanged;
            textBox.MouseDown += OnMouseDown;
            textBox.Disposed += OnTextBoxDisposed;

            // Pierwsze sprawdzenie
            if (highlightErrors && textBox is RichTextBox)
            {
                CheckSpelling(textBox);
            }
        }

        /// <summary>
        /// Wyłącza sprawdzanie pisowni dla TextBox lub RichTextBox
        /// </summary>
        public static void DisableSpellCheck(this TextBoxBase textBox)
        {
            if (textBox == null || !_spellCheckContexts.ContainsKey(textBox))
                return;

            var context = _spellCheckContexts[textBox];

            textBox.TextChanged -= OnTextChanged;
            textBox.MouseDown -= OnMouseDown;
            textBox.Disposed -= OnTextBoxDisposed;

            context.ContextMenu?.Dispose();
            _spellCheckContexts.Remove(textBox);

            // Przywróć normalny kolor tekstu
            if (textBox is RichTextBox richTextBox)
            {
                richTextBox.SelectAll();
                richTextBox.SelectionColor = richTextBox.ForeColor;
                richTextBox.Select(0, 0);
            }
        }

        private static void OnTextBoxDisposed(object sender, EventArgs e)
        {
            if (sender is TextBoxBase textBox)
            {
                DisableSpellCheck(textBox);
            }
        }

        private static void OnTextChanged(object sender, EventArgs e)
        {
            if (sender is TextBoxBase textBox && _spellCheckContexts.ContainsKey(textBox))
            {
                var context = _spellCheckContexts[textBox];
                if (context.HighlightErrors && textBox is RichTextBox)
                {
                    CheckSpelling(textBox);
                }
            }
        }

        private static void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            if (!(sender is TextBoxBase textBox) || !_spellCheckContexts.ContainsKey(textBox))
                return;

            var context = _spellCheckContexts[textBox];
            
            // Znajdź słowo pod kursorem
            int charIndex = textBox.GetCharIndexFromPosition(e.Location);
            if (charIndex < 0 || charIndex >= textBox.Text.Length)
                return;

            var wordInfo = GetWordAtPosition(textBox.Text, charIndex);
            if (wordInfo == null)
                return;

            // Sprawdź czy słowo jest błędne
            if (SpellCheckHelper.Instance.IsCorrect(wordInfo.Word))
                return;

            // Pobierz sugestie
            var suggestions = SpellCheckHelper.Instance.GetSuggestions(wordInfo.Word);

            // Zbuduj menu kontekstowe
            context.ContextMenu.Items.Clear();

            if (suggestions.Any())
            {
                // Dodaj sugestie
                foreach (var suggestion in suggestions.Take(10))
                {
                    var menuItem = new ToolStripMenuItem(suggestion);
                    menuItem.Font = new Font(menuItem.Font, FontStyle.Bold);
                    menuItem.Tag = new { Word = wordInfo.Word, Replacement = suggestion, Start = wordInfo.StartIndex, Length = wordInfo.Length };
                    menuItem.Click += (s, ev) =>
                    {
                        var tag = (s as ToolStripMenuItem)?.Tag as dynamic;
                        if (tag != null)
                        {
                            ReplaceWord(textBox, tag.Start, tag.Length, tag.Replacement);
                        }
                    };
                    context.ContextMenu.Items.Add(menuItem);
                }

                context.ContextMenu.Items.Add(new ToolStripSeparator());
            }
            else
            {
                var noSuggestionsItem = new ToolStripMenuItem("(Brak sugestii)");
                noSuggestionsItem.Enabled = false;
                context.ContextMenu.Items.Add(noSuggestionsItem);
                context.ContextMenu.Items.Add(new ToolStripSeparator());
            }

            // Dodaj opcję "Dodaj do słownika"
            var addToDictionaryItem = new ToolStripMenuItem($"Dodaj \"{wordInfo.Word}\" do słownika");
            addToDictionaryItem.Tag = wordInfo.Word;
            addToDictionaryItem.Click += (s, ev) =>
            {
                var word = (s as ToolStripMenuItem)?.Tag as string;
                if (!string.IsNullOrEmpty(word))
                {
                    SpellCheckHelper.Instance.AddToCustomDictionary(word);
                    CheckSpelling(textBox);
                }
            };
            context.ContextMenu.Items.Add(addToDictionaryItem);

            // Dodaj opcję "Ignoruj"
            var ignoreItem = new ToolStripMenuItem("Ignoruj");
            ignoreItem.Click += (s, ev) =>
            {
                // Po prostu nie rób nic - w przyszłości można dodać sesyjną listę ignorowanych słów
            };
            context.ContextMenu.Items.Add(ignoreItem);

            // Pokaż menu
            context.ContextMenu.Show(textBox, e.Location);
        }

        private static void CheckSpelling(TextBoxBase textBox)
        {
            // Sprawdzanie pisowni działa tylko dla RichTextBox ze względu na kolorowanie
            if (!(textBox is RichTextBox richTextBox))
                return;

            // Zapisz pozycję kursora
            int selectionStart = richTextBox.SelectionStart;
            int selectionLength = richTextBox.SelectionLength;

            // Wyłącz odświeżanie
            richTextBox.SuspendLayout();

            // Sprawdź tekst
            var errors = SpellCheckHelper.Instance.CheckText(richTextBox.Text);

            // Najpierw przywróć normalny kolor dla całego tekstu
            richTextBox.SelectAll();
            richTextBox.SelectionColor = richTextBox.ForeColor;

            // Podkreśl błędy
            foreach (var error in errors)
            {
                try
                {
                    richTextBox.Select(error.StartIndex, error.Length);
                    richTextBox.SelectionColor = Color.Red;
                }
                catch
                {
                    // Ignoruj błędy zakresu
                }
            }

            // Przywróć pozycję kursora
            richTextBox.Select(selectionStart, selectionLength);

            // Włącz odświeżanie
            richTextBox.ResumeLayout();
        }

        private static WordInfo GetWordAtPosition(string text, int position)
        {
            if (string.IsNullOrEmpty(text) || position < 0 || position >= text.Length)
                return null;

            // Znajdź początek słowa
            int start = position;
            while (start > 0 && (char.IsLetter(text[start - 1]) || text[start - 1] == '-' || text[start - 1] == '\''))
            {
                start--;
            }

            // Znajdź koniec słowa
            int end = position;
            while (end < text.Length && (char.IsLetter(text[end]) || text[end] == '-' || text[end] == '\''))
            {
                end++;
            }

            if (end <= start)
                return null;

            return new WordInfo
            {
                Word = text.Substring(start, end - start),
                StartIndex = start,
                Length = end - start
            };
        }

        private static void ReplaceWord(TextBoxBase textBox, int start, int length, string replacement)
        {
            int cursorPosition = textBox.SelectionStart;
            
            textBox.Text = textBox.Text.Remove(start, length).Insert(start, replacement);
            
            // Ustaw kursor po zamienionym słowie
            if (cursorPosition >= start && cursorPosition <= start + length)
            {
                textBox.SelectionStart = start + replacement.Length;
            }
            else if (cursorPosition > start + length)
            {
                textBox.SelectionStart = cursorPosition - length + replacement.Length;
            }
            else
            {
                textBox.SelectionStart = cursorPosition;
            }
        }

        private class SpellCheckContext
        {
            public Control Control { get; set; }
            public bool HighlightErrors { get; set; }
            public ContextMenuStrip ContextMenu { get; set; }
        }

        private class WordInfo
        {
            public string Word { get; set; }
            public int StartIndex { get; set; }
            public int Length { get; set; }
        }
    }
}
