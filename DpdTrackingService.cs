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
        public int OdbiorcaId { get; set; } // Kluczowe do filtrowania powiadomień
    }

    public class DpdTrackingService
    {
        public event Action<string> ProgressUpdated;
        private readonly object _webViewIgnored;

        public DpdTrackingService(object webViewIgnored = null)
        {
            _webViewIgnored = webViewIgnored;
        }

        public async Task UpdateAllActiveShipmentsStatusAsync()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {
                // 1. Pobieramy przesyłki (Tylko te niedoręczone)
                var shipments = await GetActiveShipmentsAsync();
                if (!shipments.Any()) return;

                // 2. Pobieramy ID Twojej firmy, żeby wiedzieć kiedy paczka wraca do Ciebie
                int idFirmyWlasnej = await GetOwnFirmIdAsync();

                // 3. Pobieramy dane logowania DPD
                var dbCreds = await GetDpdCredentialsAsync();
                if (dbCreds == null)
                {
                    ProgressUpdated?.Invoke("BŁĄD DPD: Brak konfiguracji API (login/hasło).");
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
                        // Zapytanie do API DPD
                        var response = await client.getEventsForWaybillV1Async(shipment.NumerListu, eventsSelectTypeEnum.ALL, "PL", authData);
                        var eventsList = response.@return?.eventsList;

                        if (eventsList != null && eventsList.Length > 0)
                        {
                            var historyData = ConvertEventsToRawData(eventsList);

                            // Zapisujemy całą historię do bazy (dla podglądu szczegółów)
                            await SaveShipmentHistoryAsync(shipment.Id, historyData);

                            // --- LOGIKA FILTROWANIA POWIADOMIEŃ ---

                            // Szukamy najnowszego statusu, który nie jest "śmieciem" (typu powiadomienie SMS od DPD)
                            var meaningfulEvent = historyData
                                .OrderByDescending(x => DateTime.Parse($"{x.Data} {x.Godzina}"))
                                .FirstOrDefault(x => !IsNotificationStatus(x.BusinessCode));

                            // Jeśli same śmieci, bierzemy cokolwiek
                            if (meaningfulEvent == null) meaningfulEvent = historyData.LastOrDefault();

                            if (meaningfulEvent != null)
                            {
                                string code = meaningfulEvent.BusinessCode;
                                string description = meaningfulEvent.Opis;

                                // Domyślnie status jest czysty (bez nawiasów = brak powiadomienia w Asystencie)
                                string finalStatus = description;

                                // Sprawdzamy czy to paczka DO NAS (np. zwrot od klienta lub dostawa części)
                                bool isIncomingToMe = (idFirmyWlasnej > 0 && shipment.OdbiorcaId == idFirmyWlasnej);

                                // A. PROBLEMY (Zawsze powiadamiaj - Czerwony)
                                if (IsProblemCode(code))
                                {
                                    finalStatus = $"[PROBLEM] {description}";
                                }
                                // B. ZWROTY (Zawsze powiadamiaj - Żółty/Czerwony)
                                else if (IsReturnCode(code))
                                {
                                    finalStatus = $"[ZWROT] {description}";
                                }
                                // C. DORĘCZONO (Zawsze powiadamiaj - Zielony)
                                else if (IsDeliveredCode(code))
                                {
                                    finalStatus = $"[DORĘCZONA] {description}";
                                }
                                // D. WYDANIE KURIEROWI (Tylko jeśli do nas - Niebieski)
                                else if (IsOutForDeliveryCode(code))
                                {
                                    if (isIncomingToMe)
                                    {
                                        // DO NAS: Dodajemy prefiks -> Asystent pokaże powiadomienie
                                        finalStatus = $"[W DORĘCZENIU] {description}";
                                    }
                                    else
                                    {
                                        // DO KLIENTA: Zostawiamy czysty opis -> Asystent zignoruje, brak spamu
                                        finalStatus = description;
                                    }
                                }

                                // Aktualizacja w bazie (tylko jeśli status się zmienił)
                                if (shipment.OstatniStatus != finalStatus)
                                {
                                    // Sprawdzamy czy to status końcowy (żeby odhaczyć "CzyDoreczona")
                                    bool isFinal = IsDeliveredCode(code) || (IsReturnCode(code) && description.Contains("Dostarczono"));

                                    await UpdateShipmentStatusAsync(shipment.Id, finalStatus, isFinal);

                                    // Wywołujemy zdarzenie tylko dla statusów z nawiasem [] (czyli tych ważnych)
                                    if (finalStatus.StartsWith("["))
                                    {
                                        ProgressUpdated?.Invoke($"DPD: {shipment.NumerListu} -> {finalStatus}");
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // Ignorujemy błędy logowania (częste przy wygasłej sesji), logujemy inne
                        if (!ex.Message.Contains("Login failed"))
                            Console.WriteLine($"DPD Error {shipment.NumerListu}: {ex.Message}");
                    }
                }
                client.Close();
            }
            catch (System.Exception ex)
            {
                ProgressUpdated?.Invoke($"Błąd serwisu DPD: {ex.Message}");
            }
        }

        // --- PUBLICZNE API DLA OKNA FORM_DPD_TRACKING (TABELA) ---
        public async Task<List<PrzesylkaViewModel>> GetActiveShipmentsForDisplayAsync()
        {
            var result = new List<PrzesylkaViewModel>();
            // Pobieramy dane z tabeli Przesylki i dołączamy nazwy klientów
            string query = @"SELECT p.Id, p.NumerListu, p.NrZgloszenia, p.OstatniStatus, 
                             CONCAT(IFNULL(nad.ImieNazwisko, ''), ' ', IFNULL(nad.NazwaFirmy, '')) AS Nadawca,
                             CONCAT(IFNULL(odb.ImieNazwisko, ''), ' ', IFNULL(odb.NazwaFirmy, '')) AS Odbiorca 
                             FROM Przesylki p 
                             LEFT JOIN Klienci nad ON p.NadawcaId = nad.Id 
                             LEFT JOIN Klienci odb ON p.OdbiorcaId = odb.Id 
                             WHERE p.CzyDoreczona = 0 ORDER BY p.Id DESC";

            await RunDbActionWithRetryAsync(async () =>
            {
                using (var con = Database.GetNewOpenConnection())
                using (var cmd = new MySqlCommand(query, con))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new PrzesylkaViewModel
                        {
                            Id = reader.GetInt32(0),
                            NumerListu = GetSafeString(reader, "NumerListu"),
                            NrZgloszenia = GetSafeString(reader, "NrZgloszenia"),
                            OstatniStatus = GetSafeString(reader, "OstatniStatus"),
                            NazwaNadawcy = GetSafeString(reader, "Nadawca").Trim(),
                            NazwaOdbiorcy = GetSafeString(reader, "Odbiorca").Trim()
                        });
                    }
                }
            });
            return result;
        }

        // --- MAPOWANIE KODÓW STATUSÓW DPD ---

        private bool IsNotificationStatus(string code) => code.StartsWith("1703") || code.StartsWith("5117"); // SMS/Email/Awizacja

        private bool IsOutForDeliveryCode(string code)
        {
            // 1102 - Wydanie przesyłki kurierowi do doręczenia
            return code.StartsWith("1102");
        }

        private bool IsDeliveredCode(string code)
        {
            // 1901 - Doręczono, 1902 - Z zastrzeżeniami, 5119/6001/7019 - Pickup
            return code.StartsWith("1901") || code.StartsWith("1902") ||
                   code.StartsWith("5013") || code.StartsWith("5119") ||
                   code.StartsWith("6001") || code.StartsWith("7019");
        }

        private bool IsReturnCode(string code)
        {
            // 2304xx - Zwroty do nadawcy
            return code.StartsWith("2304");
        }

        private bool IsProblemCode(string code)
        {
            // 04xxxx - Przesyłka nieprzygotowana
            // 20xxxx - Niedoręczona (adresat nieobecny, zamknięte)
            // 21xxxx - Błędny adres
            // 2303xx - Zaginiona / Likwidacja

            if (code.StartsWith("04")) return true;
            if (code.StartsWith("20")) return true;
            if (code.StartsWith("21")) return true;
            if (code.StartsWith("2303")) return true;

            return false;
        }

        // --- METODY BAZODANOWE I POMOCNICZE ---

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
                        OstatniStatus = reader["OstatniStatus"].ToString(),
                        // Bezpieczne rzutowanie ID odbiorcy
                        OdbiorcaId = reader["OdbiorcaId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["OdbiorcaId"])
                    });
                }
            }
            return list;
        }

        private List<RawTrackingData> ConvertEventsToRawData(customerEventV3[] events)
        {
            var list = new List<RawTrackingData>();
            foreach (var ev in events)
            {
                string fullDesc = ev.description;
                if (ev.eventDataList != null)
                {
                    foreach (var data in ev.eventDataList)
                        if (!string.IsNullOrEmpty(data.value) && (data.code == "receiverName" || data.code == "link")) fullDesc += $" ({data.value})";
                }
                DateTime dt;
                if (!DateTime.TryParse(ev.eventTime.ToString(), out dt)) dt = DateTime.Now;

                list.Add(new RawTrackingData { Data = dt.ToString("yyyy-MM-dd"), Godzina = dt.ToString("HH:mm:ss"), Opis = fullDesc, Oddzial = ev.depot ?? "", BusinessCode = ev.businessCode });
            }
            return list.OrderBy(x => x.Data).ThenBy(x => x.Godzina).ToList();
        }

        private async Task SaveShipmentHistoryAsync(int shipmentId, List<RawTrackingData> history)
        {
            var existing = new HashSet<string>();
            using (var c = Database.GetNewOpenConnection())
            {
                using (var cmd = new MySqlCommand("SELECT DataStatusu, OpisStatusu FROM HistoriaPrzesylek WHERE PrzesylkaId=@id", c))
                {
                    cmd.Parameters.AddWithValue("@id", shipmentId);
                    using (var r = await cmd.ExecuteReaderAsync()) while (await r.ReadAsync()) existing.Add(r.GetDateTime(0).ToString("yyyy-MM-dd HH:mm:ss") + r.GetString(1));
                }
                var toIns = history.Where(x => !existing.Contains(DateTime.Parse($"{x.Data} {x.Godzina}").ToString("yyyy-MM-dd HH:mm:ss") + x.Opis)).ToList();
                if (!toIns.Any()) return;
                using (var t = c.BeginTransaction())
                {
                    foreach (var i in toIns)
                    {
                        using (var cmd = new MySqlCommand("INSERT INTO HistoriaPrzesylek (PrzesylkaId, DataStatusu, OpisStatusu, Oddzial) VALUES (@p,@d,@o,@od)", c, t))
                        {
                            cmd.Parameters.AddWithValue("@p", shipmentId);
                            cmd.Parameters.AddWithValue("@d", DateTime.Parse($"{i.Data} {i.Godzina}"));
                            cmd.Parameters.AddWithValue("@o", i.Opis);
                            cmd.Parameters.AddWithValue("@od", i.Oddzial);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    t.Commit();
                }
            }
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

        // --- LOGOWANIE I CREDENTIALS ---

        private class DpdCredentials { public string Login; public string Password; }
        private async Task<DpdCredentials> GetDpdCredentialsAsync()
        {
            var c = new DpdCredentials();
            try
            {
                using (var conn = Database.GetNewOpenConnection())
                using (var cmd = new MySqlCommand("SELECT Klucz, WartoscZaszyfrowana FROM Ustawienia WHERE Klucz IN ('loginapi', 'hasloapi')", conn))
                using (var r = await cmd.ExecuteReaderAsync())
                    while (await r.ReadAsync())
                    {
                        if (r.GetString(0) == "loginapi") c.Login = r.GetString(1);
                        if (r.GetString(0) == "hasloapi") c.Password = r.GetString(1);
                    }
            }
            catch { return null; }
            return (string.IsNullOrEmpty(c.Login)) ? null : c;
        }

        private async Task RunDbActionWithRetryAsync(Func<Task> dbAction)
        {
            int attempt = 0;
            var random = new Random();
            while (true)
            {
                try { await dbAction(); return; }
                catch (MySqlException ex)
                {
                    attempt++;
                    if ((ex.ErrorCode == 5 || ex.ErrorCode == 6) && attempt <= 5) { await Task.Delay(500 + random.Next(0, 500)); continue; }
                    throw;
                }
            }
        }

        private string GetSafeString(System.Data.Common.DbDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetValue(ordinal).ToString();
        }

        public async Task MarkShipmentAsCompletedAsync(int id)
        {
            await UpdateShipmentStatusAsync(id, "[ZAKOŃCZONA RĘCZNIE]", true);
        }

        // Metoda do pobierania historii pojedynczej paczki (dla okna szczegółów)
        public async Task<List<HistoriaPrzesylki>> GetShipmentHistoryAsync(string numerListu)
        {
            var creds = await GetDpdCredentialsAsync();
            if (creds == null) return new List<HistoriaPrzesylki>();
            try
            {
                var authData = new authDataV1 { login = creds.Login, password = creds.Password, channel = "APP" };
                var client = new DPDInfoServicesObjEventsClient();
                var response = await client.getEventsForWaybillV1Async(numerListu, eventsSelectTypeEnum.ALL, "PL", authData);
                client.Close();
                var events = response.@return?.eventsList;
                if (events != null)
                {
                    var raw = ConvertEventsToRawData(events);
                    return raw.Select(x => new HistoriaPrzesylki { DataStatusu = DateTime.Parse($"{x.Data} {x.Godzina}"), OpisStatusu = x.Opis, Oddzial = x.Oddzial }).OrderByDescending(x => x.DataStatusu).ToList();
                }
            }
            catch { }
            return new List<HistoriaPrzesylki>();
        }
    }
}