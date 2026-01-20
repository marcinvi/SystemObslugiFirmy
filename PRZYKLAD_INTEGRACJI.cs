using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// PRZYKŁAD INTEGRACJI - Dodaj te fragmenty kodu do swojego głównego formularza
    /// </summary>
    public class IntegracjaExample
    {
        /// <summary>
        /// PRZYKŁAD 1: Dodanie przycisków do głównego formularza
        /// 
        /// Umieść ten kod w metodzie InitializeComponent() lub w konstruktorze głównego formularza
        /// </summary>
        public void DodajPrzyciskiDoGlownegoFormularza(Form mainForm)
        {
            // Panel dla przycisków synchronizacji
            var panelSync = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(600, 60),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.AliceBlue
            };

            // Przycisk: Konfiguracja API
            var btnApiConfig = new Button
            {
                Location = new Point(10, 10),
                Size = new Size(180, 40),
                Text = "⚙️ Konfiguracja API",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnApiConfig.FlatAppearance.BorderSize = 0;
            btnApiConfig.Click += (s, e) =>
            {
                var form = new FormApiConfig();
                form.ShowDialog();
            };

            // Przycisk: Paruj telefon
            var btnParujTelefon = new Button
            {
                Location = new Point(200, 10),
                Size = new Size(180, 40),
                Text = "📱 Paruj telefon",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnParujTelefon.FlatAppearance.BorderSize = 0;
            btnParujTelefon.Click += (s, e) =>
            {
                var form = new FormParujTelefon();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(
                        $"Telefon sparowany!\nIP: {form.PhoneIp}",
                        "Sukces",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            };

            // Przycisk: Status synchronizacji
            var btnStatus = new Button
            {
                Location = new Point(390, 10),
                Size = new Size(200, 40),
                Text = "📊 Status: Nieznany",
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btnStatus.FlatAppearance.BorderSize = 0;

            // Aktualizuj status co sekundę
            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) =>
            {
                try
                {
                    if (ApiSyncService.Instance != null && ApiSyncService.Instance.IsInitialized && ApiSyncService.Instance.IsAuthenticated)
                    {
                        string syncInfo = ApiSyncService.Instance.GetLastSyncInfo();
                        btnStatus.Text = $"📊 Sync: {syncInfo}";
                        btnStatus.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        btnStatus.Text = "📊 Status: Nie zalogowano";
                        btnStatus.BackColor = Color.LightGray;
                    }
                }
                catch
                {
                    btnStatus.Text = "📊 Status: Nieznany";
                    btnStatus.BackColor = Color.LightGray;
                }
            };
            timer.Start();

            // Dodaj przyciski do panelu
            panelSync.Controls.AddRange(new Control[] { btnApiConfig, btnParujTelefon, btnStatus });

            // Dodaj panel do głównego formularza
            mainForm.Controls.Add(panelSync);
        }

        /// <summary>
        /// PRZYKŁAD 2: Inicjalizacja API przy starcie aplikacji
        /// 
        /// Umieść ten kod w Program.cs w metodzie Main(), PRZED Application.Run()
        /// </summary>
        public static void InicjalizujApiPrzyStarcie()
        {
            // Spróbuj załadować zapisany URL API
            try
            {
                string savedUrl = Properties.Settings.Default.ApiBaseUrl;
                if (!string.IsNullOrEmpty(savedUrl))
                {
                    ApiSyncService.Initialize(savedUrl);

                    // Spróbuj auto-login
                    var autoLoginTask = ApiSyncService.Instance.AutoLoginAsync();
                    autoLoginTask.Wait(); // Poczekaj na wynik

                    if (autoLoginTask.Result)
                    {
                        Console.WriteLine("✅ Auto-login do API udany!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Nie udało się auto-zalogować do API: {ex.Message}");
                // Aplikacja i tak się uruchomi, użytkownik będzie musiał zalogować się ręcznie
            }
        }

        /// <summary>
        /// PRZYKŁAD 3: Synchronizacja zgłoszeń z DataGridView
        /// 
        /// Użyj tego kodu żeby załadować zgłoszenia z API do DataGridView
        /// </summary>
        public async void ZaladujZgloszeniaZApi(DataGridView dataGridView)
        {
            if (ApiSyncService.Instance == null || !ApiSyncService.Instance.IsInitialized || !ApiSyncService.Instance.IsAuthenticated)
            {
                MessageBox.Show(
                    "Musisz być zalogowany do API!\n\nKliknij 'Konfiguracja API' i zaloguj się.",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                // Pokaż progress
                var progressForm = new Form
                {
                    Text = "Synchronizacja...",
                    Size = new Size(300, 100),
                    StartPosition = FormStartPosition.CenterScreen,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    ControlBox = false
                };
                var label = new Label
                {
                    Text = "Pobieranie zgłoszeń z API...",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                progressForm.Controls.Add(label);
                progressForm.Show();

                // Pobierz zgłoszenia
                var zgloszenia = await ApiSyncService.Instance.SyncZgloszeniaAsync(forceRefresh: true);

                // Konwertuj do DataTable
                var dataTable = new System.Data.DataTable();
                dataTable.Columns.Add("Nr zgłoszenia", typeof(string));
                dataTable.Columns.Add("Data", typeof(string));
                dataTable.Columns.Add("Klient", typeof(string));
                dataTable.Columns.Add("Produkt", typeof(string));
                dataTable.Columns.Add("Status", typeof(string));
                dataTable.Columns.Add("Usterka", typeof(string));

                foreach (var z in zgloszenia)
                {
                    dataTable.Rows.Add(
                        z.NrZgloszenia,
                        z.DataZgloszenia,
                        z.Klient?.ImieNazwisko ?? "Brak danych",
                        z.Produkt?.Nazwa ?? "Brak danych",
                        z.StatusOgolny ?? "Brak statusu",
                        z.Usterka ?? ""
                    );
                }

                // Ustaw DataSource
                dataGridView.DataSource = dataTable;

                // Zamknij progress
                progressForm.Close();

                MessageBox.Show(
                    $"Zsynchronizowano {zgloszenia.Count} zgłoszeń z API!",
                    "Sukces",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd synchronizacji:\n\n{ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// PRZYKŁAD 4: Wysłanie SMS przez sparowany telefon
        /// 
        /// Użyj tego kodu żeby wysłać SMS z poziomu Windows Forms
        /// </summary>
        public async void WyslijSmsAsync(string numerTelefonu, string tresc)
        {
            try
            {
                // Pobierz zapisane IP telefonu
                string phoneIp = Properties.Settings.Default.PhoneIP;
                
                if (string.IsNullOrEmpty(phoneIp))
                {
                    MessageBox.Show(
                        "Telefon nie jest sparowany!\n\nKliknij 'Paruj telefon' najpierw.",
                        "Błąd",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Utwórz klienta
                var phoneClient = new PhoneClient(phoneIp);

                // Wyślij SMS
                bool success = await phoneClient.SendSmsAsync(numerTelefonu, tresc);

                if (success)
                {
                    MessageBox.Show(
                        $"SMS wysłany!\n\nDo: {numerTelefonu}\nTreść: {tresc}",
                        "Sukces",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Nie udało się wysłać SMS.\n\nSprawdź czy telefon jest podłączony do sieci.",
                        "Błąd",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd wysyłania SMS:\n\n{ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// PRZYKŁAD 5: Aktualizacja statusu zgłoszenia
        /// 
        /// Użyj tego kodu żeby zmienić status zgłoszenia przez API
        /// </summary>
        public async void ZmienStatusZgloszeniaAsync(int zgloszenieId, string nowyStatus)
        {
            if (ApiSyncService.Instance == null || !ApiSyncService.Instance.IsInitialized || !ApiSyncService.Instance.IsAuthenticated)
            {
                MessageBox.Show("Musisz być zalogowany do API!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var zaktualizowane = await ApiSyncService.Instance.UpdateStatusAsync(
                    zgloszenieId, 
                    nowyStatus, 
                    $"Status zmieniony przez {Environment.UserName}"
                );

                MessageBox.Show(
                    $"Status zgłoszenia {zaktualizowane.NrZgloszenia} zmieniony na: {nowyStatus}",
                    "Sukces",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Odśwież listę zgłoszeń
                // ZaladujZgloszeniaZApi(dataGridView);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd zmiany statusu:\n\n{ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// PRZYKŁAD 6: Dodanie notatki do zgłoszenia
        /// 
        /// Użyj tego kodu żeby dodać notatkę do zgłoszenia przez API
        /// </summary>
        public async void DodajNotatkeAsync(int zgloszenieId, string tresc)
        {
            if (ApiSyncService.Instance == null || !ApiSyncService.Instance.IsInitialized || !ApiSyncService.Instance.IsAuthenticated)
            {
                MessageBox.Show("Musisz być zalogowany do API!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var notatka = await ApiSyncService.Instance.AddNotatkaAsync(zgloszenieId, tresc);

                MessageBox.Show(
                    $"Notatka dodana!\n\nData: {notatka.Data}\nUżytkownik: {notatka.Uzytkownik}",
                    "Sukces",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd dodawania notatki:\n\n{ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }

    /// <summary>
    /// PRZYKŁAD 7: Modyfikacja Program.cs
    /// 
    /// Zamień swoją metodę Main() na tę poniżej:
    /// </summary>
    /*
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // NOWE: Inicjalizacja API przy starcie
            IntegracjaExample.InicjalizujApiPrzyStarcie();

            // Uruchom główny formularz
            Application.Run(new Form1());
        }
    }
    */
}
