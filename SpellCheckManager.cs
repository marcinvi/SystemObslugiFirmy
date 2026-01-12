using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public static class SpellCheckManager
    {
        private static readonly ConditionalWeakTable<Control, object> _processedControls =
            new ConditionalWeakTable<Control, object>();

        public static void EnableSpellCheckForAllTextBoxes(Control root)
        {
            if (root == null)
                return;

            ApplyToControlTree(root);

            root.ControlAdded -= OnControlAdded;
            root.ControlAdded += OnControlAdded;
        }

        private static void OnControlAdded(object sender, ControlEventArgs e)
        {
            if (e?.Control == null)
                return;

            ApplyToControlTree(e.Control);

            if (sender is Control parent)
            {
                parent.ControlAdded -= OnControlAdded;
                parent.ControlAdded += OnControlAdded;
            }
        }

        private static void ApplyToControlTree(Control control)
        {
            if (control == null)
                return;

            if (control is TextBoxBase textBox)
            {
                if (!_processedControls.TryGetValue(textBox, out _))
                {
                    bool highlightErrors = textBox is RichTextBox;
                    textBox.EnableSpellCheck(highlightErrors);
                    _processedControls.Add(textBox, new object());
                    textBox.Disposed += OnControlDisposed;
                }
            }

            foreach (Control child in control.Controls)
            {
                ApplyToControlTree(child);
            }
        }

        private static void OnControlDisposed(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                _processedControls.Remove(control);
                control.Disposed -= OnControlDisposed;
            }
        }
    }
}
