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
                       CONCAT(k.ImieNazwisko, ' ', k.NazwaFirmy) AS NazwaOdbiorcy
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
                bool clearDeliveryReminder = false;
                bool shouldNotifyUser = false;
                bool isForOwnCompany = idFirmyWlasnej > 0 && odbiorcaId == idFirmyWlasnej;

                // --- LOGIKA BIZNESOWA ---

                if (ostatniStatus.StartsWith("[PROBLEM]") || ostatniStatus.StartsWith("[ZGUBIONA]"))
                {
                    notificationText = $"[PROBLEM DPD] Zgłoszenie {nrZgloszenia}: {ostatniStatus.Substring(9)}";
                    toastTitle = "Problem z Przesyłką";
                    toastType = NotificationType.Error;
                    createReminder = true;
                    shouldNotifyUser = true;
                }
                else if (ostatniStatus.StartsWith("[ZWROT]"))
                {
                    notificationText = $"[ZWROT DPD] Zgłoszenie {nrZgloszenia}: Przesyłka wraca do nadawcy.";
                    toastTitle = "Zwrot Przesyłki";
                    toastType = NotificationType.Warning;
                    createReminder = true;
                    shouldNotifyUser = true;
                }
                else if (ostatniStatus.StartsWith("[W DORĘCZENIU]"))
                {
                    // Powiadomienie i przypomnienie o "w doręczeniu" tylko dla przesyłek do naszej firmy.
                    if (isForOwnCompany)
                    {
                        notificationText = $"[PRZESYŁKA] Do nas: {nrZgloszenia} jest w doręczeniu.";
                        toastTitle = "W Doręczeniu";
                        toastType = NotificationType.Info;
                        createReminder = true;
                        shouldNotifyUser = true;
                    }
                }
                else if (ostatniStatus.StartsWith("[DORĘCZONA]"))
                {
                    clearDeliveryReminder = true;

                    // Powiadomienie o doręczeniu tylko dla przesyłek do naszej firmy.
                    if (isForOwnCompany)
                    {
                        notificationText = $"[PRZESYŁKA] Zgłoszenie {nrZgloszenia} zostało doręczone.";
                        toastTitle = "Doręczono";
                        toastType = NotificationType.Success;
                        shouldNotifyUser = true;
                    }
                }

                // --- WYKONANIE AKCJI ---
                bool hasAction = shouldNotifyUser || createReminder || clearDeliveryReminder;
                if (hasAction)
                {
                    // 1. Toast (opcjonalny)
                    if (shouldNotifyUser && _owner != null && !_owner.IsDisposed && _owner.IsHandleCreated)
                    {
                        _owner.Invoke((MethodInvoker)delegate {
                            ToastManager.ShowToast(toastTitle, notificationText, toastType);
                        });
                    }

                    // 2. Obsługa listy przypomnień
                    if (clearDeliveryReminder)
                    {
                        await ReminderService.DeleteSpecificReminderAsync(nrZgloszenia, "%w doręczeniu%");
                    }

                    if (createReminder)
                    {
                        await AddReminderAsync(notificationText, nrZgloszenia);
                    }

                    // 3. Dodaj do listy alertów (popup) tylko gdy powiadomienie ma być pokazane użytkownikowi
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

                    // 4. Aktualizacja bazy (żeby nie zapętlać notyfikacji)
                    await UpdateLastNotificationStatusAsync(shipmentId, ostatniStatus);
                }
            }

            // 5. Pokaż okno z listą zmian (Popup)
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
            string checkQuery = "SELECT COUNT(*) FROM Przypomnienia WHERE Tresc = @tresc AND DotyczyZgloszenia = @nr AND CzyZrealizowane = 0";
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
                string query = "INSERT INTO Przypomnienia (Tresc, DataPrzypomnienia, CzyZrealizowane, DotyczyZgloszenia) VALUES (@tresc, @data, 0, @nr)";
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

    // <<< TO JEST TA KLASA, KTÓREJ BRAKOWAŁO >>>
    public class PrzesylkaAlert
    {
        public int Id { get; set; }
        public string NumerListu { get; set; }
        public string NrZgloszenia { get; set; }
        public string NowyStatus { get; set; }
    }
}