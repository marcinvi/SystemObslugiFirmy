using Reklamacje_Dane.DPDInfoService;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    // Model pomocniczy do przetwarzania
    public class PrzesylkaInfo
    {
        public int Id { get; set; }
        public string NumerListu { get; set; }
        public string OstatniStatus { get; set; }
        public int OdbiorcaId { get; set; }
    }

    public class DpdTrackingService
    {
        public event Action<string> ProgressUpdated;

        public DpdTrackingService(object webViewIgnored = null) { }

        public async Task UpdateAllActiveShipmentsStatusAsync()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {
                var shipments = await GetActiveShipmentsAsync();
                if (!shipments.Any()) return;

                int idFirmyWlasnej = await GetOwnFirmIdAsync();
                var dbCreds = await GetDpdCredentialsAsync();
                if (dbCreds == null)
                {
                    ProgressUpdated?.Invoke("BŁĄD DPD: Brak konfiguracji API.");
                    return;
                }

                var authData = new authDataV1
                {
                    login = dbCreds.Login,
                    password = dbCreds.Password,
                    channel = "APP"
                };

                var client = new DPDInfoServicesObjEventsClient();

                foreach (var shipment in shipments)
                {
                    try
                    {
                        var response = await client.getEventsForWaybillV1Async(shipment.NumerListu, eventsSelectTypeEnum.ALL, "PL", authData);
                        var eventsList = response.@return?.eventsList;

                        if (eventsList != null && eventsList.Length > 0)
                        {
                            var historyData = ConvertEventsToRawData(eventsList);
                            await SaveShipmentHistoryAsync(shipment.Id, historyData);

                            // --- NOWA LOGIKA: SKANOWANIE CAŁEJ HISTORII ---

                            // 1. Sprawdzamy czy W CAŁEJ HISTORII wystąpił błąd lub zwrot
                            bool historyHasProblem = historyData.Any(x => IsProblemCode(x.BusinessCode));
                            bool historyHasReturn = historyData.Any(x => IsReturnCode(x.BusinessCode));

                            // 2. Pobieramy NAJNOWSZY status (do wyświetlenia tekstu)
                            var latestEvent = historyData
                                .OrderByDescending(x => DateTime.Parse($"{x.Data} {x.Godzina}"))
                                .FirstOrDefault(x => !IsNotificationStatus(x.BusinessCode)) ?? historyData.LastOrDefault();

                            if (latestEvent != null)
                            {
                                string code = latestEvent.BusinessCode;
                                string description = latestEvent.Opis;
                                string finalStatus = description;
                                bool isIncomingToMe = (idFirmyWlasnej > 0 && shipment.OdbiorcaId == idFirmyWlasnej);

                                // --- HIERARCHIA WAŻNOŚCI (Decyduje o nawiasie []) ---

                                // A. Jeśli już DORĘCZONO (to zawsze wygrywa i kończy temat)
                                if (IsDeliveredCode(code))
                                {
                                    finalStatus = $"[DORĘCZONA] {description}";
                                }
                                // B. Jeśli w historii był PROBLEM, a paczka nie jest jeszcze doręczona
                                else if (historyHasProblem)
                                {
                                    finalStatus = $"[PROBLEM] {description}";
                                }
                                // C. Jeśli w historii był ZWROT do nas
                                else if (historyHasReturn)
                                {
                                    finalStatus = $"[ZWROT] {description}";
                                }
                                // D. Jeśli paczka jedzie DO NAS (id=36)
                                else if (IsOutForDeliveryCode(code) && isIncomingToMe)
                                {
                                    finalStatus = $"[W DORĘCZENIU] {description}";
                                }

                                // Aktualizacja w bazie tylko przy realnej zmianie
                                if (shipment.OstatniStatus != finalStatus)
                                {
                                    // Sprawdzamy czy to status końcowy
                                    bool isFinal = IsDeliveredCode(code) || (IsReturnCode(code) && description.ToLower().Contains("dostarczono"));

                                    await UpdateShipmentStatusAsync(shipment.Id, finalStatus, isFinal);

                                    if (finalStatus.StartsWith("["))
                                    {
                                        ProgressUpdated?.Invoke($"DPD: {shipment.NumerListu} -> {finalStatus}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Login failed"))
                            Console.WriteLine($"DPD Error {shipment.NumerListu}: {ex.Message}");
                    }
                }
                client.Close();
            }
            catch (Exception ex)
            {
                ProgressUpdated?.Invoke($"Błąd serwisu DPD: {ex.Message}");
            }
        }

        public async Task MarkShipmentAsCompletedAsync(int id)
        {
            // Ręczne zamknięcie przesyłki w bazie
            await UpdateShipmentStatusAsync(id, "[ZAKOŃCZONA RĘCZNIE]", true);
        }

        // --- MAPOWANIE KODÓW (zoptymalizowane) ---

        private bool IsNotificationStatus(string code) => code == "1703" || code == "5117";

        private bool IsOutForDeliveryCode(string code) => code == "1102";

        private bool IsDeliveredCode(string code) =>
            new[] { "1901", "1902", "5013", "5119", "6001", "7019" }.Contains(code);

        private bool IsReturnCode(string code) => code.StartsWith("2304");

        private bool IsProblemCode(string code)
        {
            // Lista kodów błędów DPD
            if (code == "04" || code.StartsWith("04")) return true; // Nieprzygotowana
            if (code == "20" || code.StartsWith("20")) return true; // Niedoręczona
            if (code == "21" || code.StartsWith("21")) return true; // Błędny adres
            if (code == "2303") return true; // Zaginiona
            return false;
        }

        // --- KOMUNIKACJA Z BAZĄ ---

        private async Task<List<PrzesylkaInfo>> GetActiveShipmentsAsync()
        {
            var list = new List<PrzesylkaInfo>();
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("SELECT Id, NumerListu, OstatniStatus, OdbiorcaId FROM Przesylki WHERE CzyDoreczona = 0", con))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new PrzesylkaInfo
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        NumerListu = reader["NumerListu"].ToString(),
                        OstatniStatus = reader["OstatniStatus"]?.ToString() ?? "",
                        OdbiorcaId = reader["OdbiorcaId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["OdbiorcaId"])
                    });
                }
            }
            return list;
        }

        private async Task UpdateShipmentStatusAsync(int id, string status, bool delivered)
        {
            using (var c = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("UPDATE Przesylki SET OstatniStatus=@s, CzyDoreczona=@d WHERE Id=@id", c))
            {
                cmd.Parameters.AddWithValue("@s", status);
                cmd.Parameters.AddWithValue("@d", delivered ? 1 : 0);
                cmd.Parameters.AddWithValue("@id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<int> GetOwnFirmIdAsync()
        {
            using (var con = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("SELECT WartoscZaszyfrowana FROM Ustawienia WHERE Klucz = 'IdFirmyWlasnej'", con))
            {
                var res = await cmd.ExecuteScalarAsync();
                return (res != null && int.TryParse(res.ToString(), out int id)) ? id : 0;
            }
        }

        private async Task<DpdCredentials> GetDpdCredentialsAsync()
        {
            var c = new DpdCredentials();
            using (var conn = Database.GetNewOpenConnection())
            using (var cmd = new MySqlCommand("SELECT Klucz, WartoscZaszyfrowana FROM Ustawienia WHERE Klucz IN ('loginapi', 'hasloapi')", conn))
            using (var r = await cmd.ExecuteReaderAsync())
                while (await r.ReadAsync())
                {
                    if (r.GetString(0) == "loginapi") c.Login = r.GetString(1);
                    if (r.GetString(0) == "hasloapi") c.Password = r.GetString(1);
                }
            return string.IsNullOrEmpty(c.Login) ? null : c;
        }

        private List<RawTrackingData> ConvertEventsToRawData(customerEventV3[] events)
        {
            var list = new List<RawTrackingData>();
            foreach (var ev in events)
            {
                string fullDesc = ev.description;
                if (ev.eventDataList != null)
                    foreach (var data in ev.eventDataList)
                        if (!string.IsNullOrEmpty(data.value) && (data.code == "receiverName")) fullDesc += $" ({data.value})";

                DateTime dt = DateTime.TryParse(ev.eventTime.ToString(), out var d) ? d : DateTime.Now;
                list.Add(new RawTrackingData { Data = dt.ToString("yyyy-MM-dd"), Godzina = dt.ToString("HH:mm:ss"), Opis = fullDesc, Oddzial = ev.depot ?? "", BusinessCode = ev.businessCode });
            }
            return list;
        }

        private async Task SaveShipmentHistoryAsync(int shipmentId, List<RawTrackingData> history)
        {
            using (var c = Database.GetNewOpenConnection())
            {
                foreach (var i in history)
                {
                    string q = "INSERT IGNORE INTO HistoriaPrzesylek (PrzesylkaId, DataStatusu, OpisStatusu, Oddzial) VALUES (@p,@d,@o,@od)";
                    using (var cmd = new MySqlCommand(q, c))
                    {
                        cmd.Parameters.AddWithValue("@p", shipmentId);
                        cmd.Parameters.AddWithValue("@d", DateTime.Parse($"{i.Data} {i.Godzina}"));
                        cmd.Parameters.AddWithValue("@o", i.Opis);
                        cmd.Parameters.AddWithValue("@od", i.Oddzial);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        private class DpdCredentials { public string Login; public string Password; }
    }

    public class RawTrackingData
    {
        public string Data { get; set; }
        public string Godzina { get; set; }
        public string Opis { get; set; }
        public string Oddzial { get; set; }
        public string BusinessCode { get; set; }
    }
}