using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// RichTextBox z automatycznym sprawdzaniem pisowni w języku polskim
    /// </summary>
    [ToolboxItem(true)]
    [Description("RichTextBox z automatycznym sprawdzaniem pisowni po polsku")]
    public class SpellCheckRichTextBox : RichTextBox
    {
        private bool _spellCheckEnabled = true;

        /// <summary>
        /// Określa czy sprawdzanie pisowni jest włączone
        /// </summary>
        [Category("Sprawdzanie pisowni")]
        [Description("Określa czy sprawdzanie pisowni jest włączone")]
        [DefaultValue(true)]
        public bool SpellCheckEnabled
        {
            get { return _spellCheckEnabled; }
            set
            {
                if (_spellCheckEnabled != value)
                {
                    _spellCheckEnabled = value;
                    
                    if (_spellCheckEnabled)
                    {
                        // Rzutowanie na TextBoxBase, ponieważ RichTextBox dziedziczy z TextBoxBase
                        ((TextBoxBase)this).EnableSpellCheck(true);
                    }
                    else
                    {
                        ((TextBoxBase)this).DisableSpellCheck();
                    }
                }
            }
        }

        public SpellCheckRichTextBox()
        {
            InitializeSpellCheck();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            
            if (_spellCheckEnabled && !DesignMode)
            {
                InitializeSpellCheck();
            }
        }

        private void InitializeSpellCheck()
        {
            if (DesignMode)
                return;

            try
            {
                if (_spellCheckEnabled)
                {
                    ((TextBoxBase)this).EnableSpellCheck(true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd inicjalizacji sprawdzania pisowni: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((TextBoxBase)this).DisableSpellCheck();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// TextBox bazowy - nie wspiera kolorowania błędów, ale oferuje menu kontekstowe
    /// </summary>
    [ToolboxItem(true)]
    [Description("TextBox z podstawowym sprawdzaniem pisowni po polsku (bez podkreślania)")]
    public class SpellCheckTextBox : TextBox
    {
        private bool _spellCheckEnabled = true;

        /// <summary>
        /// Określa czy sprawdzanie pisowni jest włączone
        /// </summary>
        [Category("Sprawdzanie pisowni")]
        [Description("Określa czy sprawdzanie pisowni jest włączone")]
        [DefaultValue(true)]
        public bool SpellCheckEnabled
        {
            get { return _spellCheckEnabled; }
            set
            {
                if (_spellCheckEnabled != value)
                {
                    _spellCheckEnabled = value;
                    
                    if (_spellCheckEnabled)
                    {
                        // Rzutowanie na TextBoxBase, ponieważ TextBox dziedziczy z TextBoxBase
                        ((TextBoxBase)this).EnableSpellCheck(false); // false = nie podkreślaj (to tylko TextBox)
                    }
                    else
                    {
                        ((TextBoxBase)this).DisableSpellCheck();
                    }
                }
            }
        }

        public SpellCheckTextBox()
        {
            InitializeSpellCheck();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            
            if (_spellCheckEnabled && !DesignMode)
            {
                InitializeSpellCheck();
            }
        }

        private void InitializeSpellCheck()
        {
            if (DesignMode)
                return;

            try
            {
                if (_spellCheckEnabled)
                {
                    ((TextBoxBase)this).EnableSpellCheck(false); // false = bez podkreślania dla zwykłego TextBox
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd inicjalizacji sprawdzania pisowni: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((TextBoxBase)this).DisableSpellCheck();
            }
            base.Dispose(disposing);
        }
    }
}
