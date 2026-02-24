using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace Reklamacje_Dane
{
    public class ShipmentNotificationService
    {
        private readonly DatabaseService _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        private readonly DpdTrackingService _trackingService;
        private readonly Form _owner;

        public ShipmentNotificationService(Form owner, object webViewIgnored = null)
        {
            _owner = owner;
            _trackingService = new DpdTrackingService();
        }

        public async Task CheckAndNotifyAsync()
        {
            // 1. Aktualizacja z API DPD
            await _trackingService.UpdateAllActiveShipmentsStatusAsync();

            // 2. Pobranie zmian (Tylko te, które mają WAŻNY status w nawiasie [])
            string query = @"
                SELECT p.Id, p.NumerListu, p.NrZgloszenia, p.OstatniStatus, p.OdbiorcaId,
                       CONCAT(IFNULL(k.ImieNazwisko,''), ' ', IFNULL(k.NazwaFirmy,'')) AS NazwaOdbiorcy
                FROM Przesylki p
                LEFT JOIN Klienci k ON p.OdbiorcaId = k.Id
                WHERE p.OstatniStatus != IFNULL(p.LastNotificationStatus, '')
                AND p.OstatniStatus LIKE '[%'";

            var dt = await _dbService.GetDataTableAsync(query);
            if (dt.Rows.Count == 0) return;

            int idFirmyWlasnej = await GetOwnFirmIdAsync();

            var alertsToShow = new List<PrzesylkaAlert>();

            foreach (DataRow row in dt.Rows)
            {
                var shipmentId = Convert.ToInt32(row["Id"]);
                var nrZgloszenia = row["NrZgloszenia"].ToString();
                var ostatniStatus = row["OstatniStatus"].ToString();
                var odbiorcaId = row["OdbiorcaId"] == DBNull.Value ? 0 : Convert.ToInt32(row["OdbiorcaId"]);
                var numerListu = row["NumerListu"].ToString();

                string notificationText = null;
                string toastTitle = "";
                NotificationType toastType = NotificationType.Info;

                bool createReminder = false;
                bool shouldNotifyUser = false;
                bool isForOwnCompany = idFirmyWlasnej > 0 && odbiorcaId == idFirmyWlasnej;

                // --- NOWA LOGIKA BIZNESOWA DPD ---

                if (ostatniStatus.StartsWith("[PROBLEM]") || ostatniStatus.StartsWith("[ZGUBIONA]"))
                {
                    string czystyStatus = ostatniStatus.Replace("[PROBLEM]", "").Replace("[ZGUBIONA]", "").Trim();
                    notificationText = $"[PROBLEM DPD] {czystyStatus}";

                    createReminder = true;      // TWORZYMY ZADANIE W FORM2
                    shouldNotifyUser = false;   // NIE WYŚWIETLAMY TOASTA/POPUPA
                }
                else if (ostatniStatus.StartsWith("[ZWROT]"))
                {
                    string czystyStatus = ostatniStatus.Replace("[ZWROT]", "").Trim();
                    notificationText = $"[ZWROT DPD] {czystyStatus} (Przesyłka wraca do nas)";

                    createReminder = true;      // TWORZYMY ZADANIE W FORM2
                    shouldNotifyUser = false;   // NIE WYŚWIETLAMY TOASTA/POPUPA
                }
                else if (ostatniStatus.StartsWith("[W DORĘCZENIU]"))
                {
                    // Powiadomienie tylko dla przesyłek jadących DO NASZEJ FIRMY
                    if (isForOwnCompany)
                    {
                        string czystyStatus = ostatniStatus.Replace("[W DORĘCZENIU]", "").Trim();
                        notificationText = $"[DO NAS] Zgłoszenie {nrZgloszenia}: Kurier doręczy dziś paczkę.";
                        toastTitle = "W Doręczeniu";
                        toastType = NotificationType.Info;

                        createReminder = false;    // NIE TWORZYMY ZADANIA (to tylko info)
                        shouldNotifyUser = true;   // POKAZUJEMY TOAST
                    }
                }
                else if (ostatniStatus.StartsWith("[DORĘCZONA]"))
                {
                    // Powiadomienie tylko dla przesyłek jadących DO NASZEJ FIRMY
                    if (isForOwnCompany)
                    {
                        notificationText = $"Zgłoszenie {nrZgloszenia} zostało właśnie doręczone na magazyn.";
                        toastTitle = "Doręczono";
                        toastType = NotificationType.Success;

                        createReminder = false;    // NIE TWORZYMY ZADANIA
                        shouldNotifyUser = true;   // POKAZUJEMY ZIELONY TOAST
                    }
                }

                // --- WYKONANIE AKCJI ---
                bool hasAction = shouldNotifyUser || createReminder;
                if (hasAction)
                {
                    // 1. Toast (dymek z boku ekranu)
                    if (shouldNotifyUser && _owner != null && !_owner.IsDisposed && _owner.IsHandleCreated)
                    {
                        _owner.Invoke((MethodInvoker)delegate {
                            ToastManager.ShowToast(toastTitle, notificationText, toastType);
                        });
                    }

                    // 2. Obsługa twardych zadań (Przypomnień w Form2)
                    if (createReminder)
                    {
                        await AddReminderAsync(notificationText, nrZgloszenia);
                    }

                    // 3. Dodaj do listy alertów (zbiorczy popup) TYLKO gdy to jest ważne powiadomienie
                    if (shouldNotifyUser)
                    {
                        alertsToShow.Add(new PrzesylkaAlert
                        {
                            Id = shipmentId,
                            NumerListu = numerListu,
                            NrZgloszenia = nrZgloszenia,
                            NowyStatus = ostatniStatus
                        });
                    }

                    // 4. Aktualizacja bazy (oznacz jako obsłużone, żeby nie powtarzać)
                    await UpdateLastNotificationStatusAsync(shipmentId, ostatniStatus);
                }
            }

            // 5. Pokaż okno z listą powiadomień zbiorczych (Popup ze statusem)
            if (alertsToShow.Any())
            {
                if (_owner != null && !_owner.IsDisposed && _owner.IsHandleCreated)
                {
                    _owner.Invoke((MethodInvoker)delegate
                    {
                        var popup = new FormPowiadomieniePrzesylka(alertsToShow);
                        popup.Show();
                    });
                }
                UpdateManager.NotifySubscribers();
            }
        }

        private async Task<int> GetOwnFirmIdAsync()
        {
            try
            {
                using (var con = Database.GetNewOpenConnection())
                using (var cmd = new MySqlCommand("SELECT WartoscZaszyfrowana FROM Ustawienia WHERE Klucz = 'IdFirmyWlasnej'", con))
                {
                    var res = await cmd.ExecuteScalarAsync();
                    if (res != null && int.TryParse(res.ToString(), out int id)) return id;
                }
            }
            catch { }

            return 0;
        }

        private async Task AddReminderAsync(string text, string complaintNumber)
        {
            string checkQuery = "SELECT COUNT(*) FROM Przypomnienia WHERE Tresc = @tresc AND DotyczyZgloszenia = @nr AND (CzyZrealizowane = 0 OR CzyZrealizowane IS NULL)";
            int exists = 0;

            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand(checkQuery, con))
            {
                cmd.Parameters.AddWithValue("@tresc", text);
                cmd.Parameters.AddWithValue("@nr", complaintNumber);
                exists = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            if (exists == 0)
            {
                // Dodajemy przypomnienie z poprawnym domyślnym statusem 'Nowe'
                string query = "INSERT INTO Przypomnienia (Tresc, DataPrzypomnienia, CzyZrealizowane, Status, DotyczyZgloszenia) VALUES (@tresc, @data, 0, 'Nowe', @nr)";
                var parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@tresc", text),
                    new MySqlParameter("@data", DateTime.Now),
                    new MySqlParameter("@nr", complaintNumber)
                };
                await _dbService.ExecuteNonQueryAsync(query, parameters);
            }
        }

        private async Task UpdateLastNotificationStatusAsync(int shipmentId, string status)
        {
            string query = "UPDATE Przesylki SET LastNotificationStatus = @status WHERE Id = @id";
            await _dbService.ExecuteNonQueryAsync(query, new MySqlParameter("@status", status), new MySqlParameter("@id", shipmentId));
        }
    }

    public class PrzesylkaAlert
    {
        public int Id { get; set; }
        public string NumerListu { get; set; }
        public string NrZgloszenia { get; set; }
        public string NowyStatus { get; set; }
    }
}