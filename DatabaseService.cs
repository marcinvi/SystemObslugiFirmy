using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    // Definicje pomocnicze
    public enum NotificationType { Info, Success, Warning, Error }

    public class ClientNotificationData
    {
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string AllegroDisputeId { get; set; }
        public int AllegroAccountId { get; set; }
    }

    public sealed class DatabaseService
    {
        private readonly string _connectionString;
        private const int MaxRetries = 15;
        private const int BaseDelayMs = 500;
        private const int MaxDelayMs = 2000;
        private const int CommandTimeoutSeconds = 60; // Timeout dla wszystkich komend

        public DatabaseService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
        }

        // ====================================================================
        // 1. METODY PUBLICZNE - PODSTAWOWE (Z RETRY)
        // ====================================================================

        public async Task<int> ExecuteNonQueryAsync(string query, params MySqlParameter[] parameters)
        {
            using (var timer = PerformanceLogger.Instance.StartTimer(query))
            {
                try
                {
                    return await ExecuteWithRetryAsync(async (con) =>
                    {
                        using (var cmd = new MySqlCommand(query, con))
                        {
                            cmd.CommandTimeout = CommandTimeoutSeconds;
                            if (parameters != null) cmd.Parameters.AddRange(parameters);
                            return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                    }, isWriteOperation: true);
                }
                catch (Exception ex)
                {
                    timer.MarkError(ex.Message);
                    throw;
                }
            }
        }

        public async Task<DataTable> GetDataTableAsync(string query, params MySqlParameter[] parameters)
        {
            using (var timer = PerformanceLogger.Instance.StartTimer(query))
            {
                try
                {
                    return await ExecuteWithRetryAsync(async (con) =>
                    {
                        using (var cmd = new MySqlCommand(query, con))
                        {
                            cmd.CommandTimeout = CommandTimeoutSeconds;
                            if (parameters != null) cmd.Parameters.AddRange(parameters);
                            using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                            {
                                var dt = new DataTable();
                                dt.Load(reader);
                                return dt;
                            }
                        }
                    }, isWriteOperation: false);
                }
                catch (Exception ex)
                {
                    timer.MarkError(ex.Message);
                    throw;
                }
            }
        }

        public async Task<object> ExecuteScalarAsync(string query, params MySqlParameter[] parameters)
        {
            using (var timer = PerformanceLogger.Instance.StartTimer(query))
            {
                try
                {
                    return await ExecuteWithRetryAsync(async (con) =>
                    {
                        using (var cmd = new MySqlCommand(query, con))
                        {
                            cmd.CommandTimeout = CommandTimeoutSeconds;
                            if (parameters != null) cmd.Parameters.AddRange(parameters);
                            return await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                        }
                    }, isWriteOperation: false);
                }
                catch (Exception ex)
                {
                    timer.MarkError(ex.Message);
                    throw;
                }
            }
        }

        // ====================================================================
        // 2. METODY DO OBSŁUGI TRANSAKCJI (NAPRAWIA BŁĄD W DziennikLogger)
        // ====================================================================

        public async Task ExecuteNonQueryAsync(MySqlConnection connection, MySqlTransaction transaction, string query, params MySqlParameter[] parameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            // Wykonujemy zapytanie na istniejącym połączeniu i transakcji
            using (var cmd = new MySqlCommand(query, connection, transaction))
            {
                cmd.CommandTimeout = CommandTimeoutSeconds;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        // ====================================================================
        // 3. METODY SPECJALISTYCZNE (NAPRAWIA BŁĄD W AddActionControl)
        // ====================================================================

        public Task<ClientNotificationData> GetClientNotificationDataAsync(string nrZgloszenia)
        {
            return ExecuteWithRetryAsync<ClientNotificationData>(async con =>
            {
                const string query = @"SELECT k.Email, k.Telefon, z.allegroDisputeId, z.allegroAccountId
                                     FROM Zgloszenia z
                                     LEFT JOIN Klienci k ON z.KlientID = k.Id
                                     WHERE z.NrZgloszenia = @nrZgloszenia";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.CommandTimeout = CommandTimeoutSeconds;
                    cmd.Parameters.AddWithValue("@nrZgloszenia", nrZgloszenia);
                    using (var r = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await r.ReadAsync().ConfigureAwait(false))
                        {
                            var data = new ClientNotificationData();
                            object allegroIdObj = r["allegroAccountId"];
                            data.Email = r["Email"] as string;
                            data.Telefon = r["Telefon"] as string;
                            data.AllegroDisputeId = r["allegroDisputeId"] as string;
                            data.AllegroAccountId = allegroIdObj is DBNull ? 0 : Convert.ToInt32(allegroIdObj);
                            return data;
                        }
                    }
                }
                return null;
            }, isWriteOperation: false);
        }

        public string NormalizujNumer(string numer)
        {
            if (string.IsNullOrEmpty(numer)) return "";
            string czysty = new string(numer.Where(char.IsDigit).ToArray());
            if (czysty.Length > 9 && czysty.StartsWith("48")) czysty = czysty.Substring(2);
            if (czysty.Length > 9) czysty = czysty.Substring(czysty.Length - 9);
            return czysty;
        }

        public async Task<Klient> ZnajdzKlientaPoNumerzeAsync(string numerPrzychodzacy)
        {
            string szukany = NormalizujNumer(numerPrzychodzacy);
            if (szukany.Length < 7) return null;

            return await ExecuteWithRetryAsync<Klient>(async (con) =>
            {
                string sql = @"SELECT * FROM Klienci
                               WHERE REPLACE(REPLACE(REPLACE(Telefon, ' ', ''), '-', ''), '+48', '') LIKE CONCAT('%', @numer, '%')
                               LIMIT 1";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.CommandTimeout = CommandTimeoutSeconds;
                    cmd.Parameters.AddWithValue("@numer", szukany);
                    using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            return new Klient
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ImieNazwisko = reader["ImieNazwisko"].ToString(),
                                NazwaFirmy = reader["NazwaFirmy"].ToString(),
                                Telefon = reader["Telefon"].ToString(),
                                Email = reader["Email"].ToString()
                            };
                        }
                    }
                }
                return null;
            }, isWriteOperation: false);
        }
        // Metoda szukająca aktywnych zgłoszeń dla danego numeru telefonu
        public async Task<DataTable> PobierzZgloszeniaWgTelefonuAsync(string numerTelefonu)
        {
            string czysty = NormalizujNumer(numerTelefonu);

            // ZMIANA: Dodano "p.NazwaSystemowa AS Produkt"
            string sql = @"
        SELECT
            z.Id,
            z.NrZgloszenia,
            z.StatusOgolny AS Status,
            p.NazwaSystemowa AS Produkt
        FROM Zgloszenia z
        JOIN Klienci k ON z.KlientID = k.Id
        LEFT JOIN Produkty p ON z.ProduktID = p.Id
        WHERE REPLACE(REPLACE(REPLACE(k.Telefon, ' ', ''), '-', ''), '+48', '') LIKE CONCAT('%', @nr, '%')
        ORDER BY z.Id DESC";

            return await GetDataTableAsync(sql, new MySqlParameter("@nr", czysty));
        }

        // Metoda przypisująca ostatni SMS od danego numeru do konkretnego zgłoszenia
        public async Task PrzypiszOstatniSmsDoZgloszeniaAsync(string numerTelefonu, string nrZgloszenia)
        {
            // 1. Pobierz ID zgłoszenia
            string sqlId = "SELECT Id FROM Zgloszenia WHERE NrZgloszenia = @nr LIMIT 1";
            object zglIdObj = await ExecuteScalarAsync(sqlId, new MySqlParameter("@nr", nrZgloszenia));

            if (zglIdObj == null) return;
            int zgloszenieId = Convert.ToInt32(zglIdObj);

            // 2. Zaktualizuj ostatni wpis w CentrumKontaktu dla tego numeru
            // Szukamy po Tytule (bo tam zapisywaliśmy numer przy odbiorze SMS)
            string updateSql = @"
                UPDATE CentrumKontaktu 
                SET ZgloszenieID = @zid 
                WHERE Tytul = @tel AND Typ = 'SMSOtrzymany' 
                AND Id = (SELECT MAX(Id) FROM CentrumKontaktu WHERE Tytul = @tel)";

            await ExecuteNonQueryAsync(updateSql,
                new MySqlParameter("@zid", zgloszenieId),
                new MySqlParameter("@tel", numerTelefonu));
        }
        public async Task ZapiszNowySmsAsync(string numer, string tresc, string kierunek)
        {
            // 1. Najpierw próbujemy zidentyfikować klienta po numerze
            var klient = await ZnajdzKlientaPoNumerzeAsync(numer);

            // Jeśli klient istnieje, bierzemy jego ID, w przeciwnym razie NULL
            object klientIdDb = DBNull.Value;
            if (klient != null)
            {
                klientIdDb = klient.Id;
            }

            // 2. Ustalamy Typ wiadomości dla Centrum Kontaktu
            // Jeśli kierunek to "Odebrane", wpisujemy "SMSOtrzymany", w przeciwnym razie "SMS"
            string typKontaktu = (kierunek == "Odebrane") ? "SMSOtrzymany" : "SMS";

            // 3. Ustalamy autora (Użytkownika)
            // Jeśli wiadomość przychodzi, autorem jest "Klient" (lub numer telefonu)
            string autor = (kierunek == "Odebrane") ? "Klient" : Program.fullName; // Zakładam, że Program.fullName to zalogowany user

            // 4. Zapisujemy do istniejącej tabeli CentrumKontaktu
            string sql = @"INSERT INTO CentrumKontaktu 
                           (KlientID, DataWyslania, Typ, Tytul, Tresc, Uzytkownik) 
                           VALUES 
                           (@kid, @data, @typ, @tytul, @tresc, @user)";

            await ExecuteNonQueryAsync(sql,
                new MySqlParameter("@kid", klientIdDb),
                new MySqlParameter("@data", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new MySqlParameter("@typ", typKontaktu),
                new MySqlParameter("@tytul", numer), // W tytule zapisujemy numer telefonu nadawcy
                new MySqlParameter("@tresc", tresc),
                new MySqlParameter("@user", autor)
            );
        }

        public async Task<List<string>> PobierzAktywneZgloszeniaKlientaAsync(int klientId)
        {
            return await ExecuteWithRetryAsync(async (con) =>
            {
                var list = new List<string>();
                if (klientId <= 0) return list;

                string sql = "SELECT NrZgloszenia, StatusOgolny FROM Zgloszenia WHERE KlientID = @id AND StatusOgolny != 'Zakończone'";
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.CommandTimeout = CommandTimeoutSeconds;
                    cmd.Parameters.AddWithValue("@id", klientId);
                    using (var r = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await r.ReadAsync().ConfigureAwait(false))
                        {
                            list.Add($"{r["NrZgloszenia"]} ({r["StatusOgolny"]})");
                        }
                    }
                }
                return list;
            }, isWriteOperation: false);
        }

        // ====================================================================
        // 4. MECHANIZM RETRY (WEWNĘTRZNY)
        // ====================================================================

        private async Task<T> ExecuteWithRetryAsync<T>(Func<MySqlConnection, Task<T>> action, bool isWriteOperation)
        {
            int attempt = 0;
            Random rnd = new Random();
            Exception lastException = null;

            while (attempt < MaxRetries)
            {
                try
                {
                    using (var connection = new MySqlConnection(_connectionString))
                    {
                        await connection.OpenAsync().ConfigureAwait(false);
                        return await action(connection).ConfigureAwait(false);
                    }
                }
                catch (MySqlException ex)
                {
                    lastException = ex;
                    attempt++;

                    // Błędy, które nie powinny być retryowane
                    if (ex.Number == 1062) // Duplikat klucza
                    {
                        throw; // Nie retryuj błędów logicznych
                    }

                    // Błędy związane z połączeniem lub lockami - można retryować
                    bool shouldRetry = ex.Number == 1205 || // Lock wait timeout
                                      ex.Number == 1213 || // Deadlock
                                      ex.Number == 2002 || // Connection error
                                      ex.Number == 2006 || // Server gone away
                                      ex.Number == 2013;   // Lost connection

                    if (!shouldRetry || attempt >= MaxRetries)
                    {
                        throw;
                    }

                    // Exponential backoff z jitterem
                    int delay = Math.Min(BaseDelayMs * (int)Math.Pow(2, attempt - 1), MaxDelayMs);
                    int jitter = rnd.Next(-100, 100);
                    await Task.Delay(delay + jitter).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    throw; // Inne wyjątki nie są retryowane
                }
            }

            throw lastException ?? new Exception("Retry limit exceeded");
        }
    }
}