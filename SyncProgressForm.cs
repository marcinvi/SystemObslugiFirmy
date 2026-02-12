// ############################################################################
// Plik: SyncProgressForm.cs (WERSJA OSTATECZNA)
// Opis: Po pokazaniu okna uruchamia asynchroniczną synchronizację,
//       raportuje postęp i na końcu odblokowuje przycisk „Uruchom Aplikację”.
// ############################################################################

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

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
                UpdateStatus("Weryfikacja dostępu do bazy danych...");
                AddLogEntry("Dashboard działa w trybie DB-only: API synchronizuje dane do bazy, a formularz czyta bazę.", Color.Black);

                string apiUrl = (Properties.Settings.Default.ApiBaseUrl ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(apiUrl))
                {
                    AddLogEntry($"Info: skonfigurowany URL API = {apiUrl} (http/https zależnie od serwera)", Color.DarkSlateBlue);
                }

                using (var con = Database.GetNewOpenConnection())
                {
                    AddLogEntry("✓ Połączenie z bazą OK", Color.ForestGreen);

                    int unregisteredAllegro = 0;
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM AllegroDisputes WHERE IFNULL(CzyZarejestrowane,0)=0", con))
                        unregisteredAllegro = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                    int newChat = 0;
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM AllegroDisputes WHERE IFNULL(HasNewMessages,0)=1", con))
                        newChat = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                    int newReturns = 0;
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM NiezarejestrowaneZwrotyReklamacyjne WHERE IFNULL(CzyZarejestrowane,0)=0", con))
                        newReturns = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                    AddLogEntry($"✓ DB liczniki: Allegro={unregisteredAllegro}, Czat={newChat}, Zwroty={newReturns}", Color.ForestGreen);

                    try
                    {
                        using (var cmd = new MySqlCommand(@"SELECT source, started_at, finished_at, ok, rows_written
                                                           FROM SyncRuns
                                                           ORDER BY started_at DESC
                                                           LIMIT 3", con))
                        using (var rd = await cmd.ExecuteReaderAsync())
                        {
                            int i = 0;
                            while (await rd.ReadAsync())
                            {
                                i++;
                                var source = rd.IsDBNull(0) ? "?" : rd.GetString(0);
                                var started = rd.IsDBNull(1) ? "?" : rd.GetDateTime(1).ToString("yyyy-MM-dd HH:mm");
                                var ok = !rd.IsDBNull(3) && rd.GetInt32(3) == 1;
                                var written = rd.IsDBNull(4) ? 0 : rd.GetInt32(4);
                                AddLogEntry($"SyncRuns[{i}]: {source} {started} status={(ok ? "OK" : "Błąd")} zapisano={written}", ok ? Color.Black : Color.OrangeRed);
                            }
                        }
                    }
                    catch
                    {
                        AddLogEntry("⚠️ Brak tabeli/rekordów SyncRuns - statusy synchronizacji mogą być ograniczone.", Color.DarkOrange);
                    }
                }

                UpdateStatus("Weryfikacja zakończona.");
                AddLogEntry("✓ Dashboard może działać wyłącznie na danych z bazy.", Color.ForestGreen);
            }
            catch (Exception ex)
            {
                _hasErrorOccurred = true;
                UpdateStatus("Błąd dostępu do bazy danych.");
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