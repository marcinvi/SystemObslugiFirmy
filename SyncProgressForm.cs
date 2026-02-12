// ############################################################################
// Plik: SyncProgressForm.cs (WERSJA OSTATECZNA)
// Opis: Po pokazaniu okna uruchamia asynchroniczną synchronizację,
//       raportuje postęp i na końcu odblokowuje przycisk „Uruchom Aplikację”.
// ############################################################################

using System;
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
            btnContinue.Click -= btnContinue_Click;
            btnContinue.Click += btnContinue_Click;
            AcceptButton = btnContinue;

            // Po pokazaniu okna uruchom asynchroniczną synchronizację
            this.Shown += SyncProgressForm_Shown;
        }

        private async void SyncProgressForm_Shown(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus("Weryfikacja połączenia API...");
                AddLogEntry("Start kontroli połączenia i autoryzacji API", Color.Black);

                string apiUrl = (Properties.Settings.Default.ApiBaseUrl ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    AddLogEntry("❌ Brak skonfigurowanego URL API (Ustawienia -> API).", Color.Red);
                    AddLogEntry("Ustaw adres serwera API w sieci, np. https://192.168.x.x:5001", Color.DarkOrange);
                    UpdateStatus("Brak konfiguracji API.");
                    return;
                }

                AddLogEntry($"API URL: {apiUrl}", Color.Black);

                try
                {
                    // Zawsze inicjalizujemy bieżącym URL ze settings, żeby uniknąć pracy na starym adresie.
                    ApiSyncService.Initialize(apiUrl);
                    AddLogEntry("✓ API service zainicjalizowany", Color.ForestGreen);
                }
                catch (Exception ex)
                {
                    AddLogEntry("❌ Błąd inicjalizacji API service: " + ex.Message, Color.Red);
                    UpdateStatus("Błąd inicjalizacji API.");
                    return;
                }

                bool apiReachable = await ApiSyncService.TestConnectionAsync(apiUrl);
                if (!apiReachable)
                {
                    AddLogEntry("❌ API nieosiągalne pod skonfigurowanym adresem.", Color.Red);
                    AddLogEntry("Sprawdź: czy serwer działa, port/firewall i certyfikat HTTPS.", Color.DarkOrange);
                    UpdateStatus("API nieosiągalne.");
                    return;
                }

                AddLogEntry("✓ API odpowiada (health check)", Color.ForestGreen);

                var tokenExpiry = Properties.Settings.Default.ApiTokenExpiry;
                var hasToken = !string.IsNullOrWhiteSpace(Properties.Settings.Default.ApiToken);

                if (!hasToken)
                {
                    AddLogEntry("⚠️ Brak zapisanego tokenu API. Wymagane logowanie w Konfiguracji API.", Color.DarkOrange);
                    UpdateStatus("Brak tokenu API.");
                    return;
                }

                if (tokenExpiry <= DateTime.Now)
                {
                    AddLogEntry($"⚠️ Token API wygasł ({tokenExpiry:yyyy-MM-dd HH:mm}).", Color.DarkOrange);
                    AddLogEntry("Zaloguj się ponownie w Konfiguracji API.", Color.DarkOrange);
                    UpdateStatus("Token API wygasł.");
                    return;
                }

                bool autoLoginOk = await ApiSyncService.Instance.AutoLoginAsync();
                if (autoLoginOk)
                {
                    AddLogEntry("✓ Auto-logowanie API zakończone powodzeniem", Color.ForestGreen);
                    UpdateStatus("API gotowe.");
                }
                else
                {
                    AddLogEntry("❌ Auto-logowanie API nie powiodło się (token odrzucony przez API).", Color.Red);
                    AddLogEntry("Zaloguj się ponownie w Konfiguracji API.", Color.DarkOrange);
                    UpdateStatus("Błąd autoryzacji API.");
                }
            }
            catch (Exception ex)
            {
                _hasErrorOccurred = true;
                UpdateStatus("Błąd weryfikacji API.");
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
            btnContinue.DialogResult = DialogResult.OK;
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
    }
}