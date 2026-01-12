// ############################################################################
// Plik: SyncProgressForm.cs (WERSJA OSTATECZNA)
// Opis: Po pokazaniu okna uruchamia asynchroniczną synchronizację,
//       raportuje postęp i na końcu odblokowuje przycisk „Uruchom Aplikację”.
// ############################################################################

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class SyncProgressForm : Form
    {
        private bool _hasErrorOccurred = false;

        public SyncProgressForm()
        {
            InitializeComponent();
            lstLog.DrawMode = DrawMode.OwnerDrawFixed;
            lstLog.DrawItem += LstLog_DrawItem;
            btnContinue.DialogResult = DialogResult.OK;
            AcceptButton = btnContinue;

            // Po pokazaniu okna uruchom asynchroniczną synchronizację
            this.Shown += SyncProgressForm_Shown;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void SyncProgressForm_Shown(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus("Odświeżanie tokenów Allegro...");
                AddLogEntry("Rozpoczynam odświeżanie tokenów...", Color.Black);

                var progress = new Progress<(string message, Color color)>(t =>
                {
                    AddLogEntry(t.message, t.color);
                });

                // WYWOŁANIE NOWEJ METODY
                await InitialAllegroSynchronizer.PerformTokenRefreshAsync(progress);

                UpdateStatus("Odświeżanie tokenów zakończone.");
                AddLogEntry("✓ Zakończono", Color.ForestGreen);
            }
            catch (Exception ex)
            {
                _hasErrorOccurred = true;
                UpdateStatus("Błąd odświeżania tokenów.");
                AddLogEntry("Błąd: " + ex.Message, Color.Red);
            }
            finally
            {
                ShowContinueButton();
            }
        }

        public void UpdateStatus(string status)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(() => lblStatus.Text = status));
            }
            else
            {
                lblStatus.Text = status;
            }
        }

        public void AddLogEntry(string message, Color color)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action(() => AddItem(message, color)));
            }
            else
            {
                AddItem(message, color);
            }

            if (color == Color.Red || color == Color.DarkRed || color == Color.OrangeRed)
            {
                _hasErrorOccurred = true;
            }
        }

        private void AddItem(string message, Color color)
        {
            lstLog.Items.Add(new { Text = message, Color = color });
            lstLog.TopIndex = lstLog.Items.Count - 1;
        }

        private void LstLog_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            var item = lstLog.Items[e.Index];
            string itemText = item.GetType().GetProperty("Text")?.GetValue(item, null)?.ToString() ?? item.ToString();
            Color itemColor = (Color)(item.GetType().GetProperty("Color")?.GetValue(item, null) ?? Color.Black);

            using (Brush brush = new SolidBrush(itemColor))
            {
                e.Graphics.DrawString(itemText, e.Font, brush, e.Bounds, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
        }

        public void ShowContinueButton()
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (btnContinue.InvokeRequired)
            {
                btnContinue.Invoke(new Action(() => SetContinueButtonState()));
            }
            else
            {
                SetContinueButtonState();
            }
        }

        private void SetContinueButtonState()
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = progressBar.Maximum;
            btnContinue.Visible = true;
            btnContinue.Enabled = true;
            btnContinue.BringToFront();
            btnContinue.Focus();

            if (_hasErrorOccurred)
            {
                btnContinue.Text = "Kontynuuj (z błędami)";
                btnContinue.BackColor = Color.OrangeRed;
            }
            else
            {
                btnContinue.Text = "Uruchom Aplikację";
                btnContinue.BackColor = Color.ForestGreen;
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    
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
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
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
}
}
