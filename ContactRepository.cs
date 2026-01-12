using System;
using System.Collections.Generic;
using System.Data;          // Ważne dla DataTable
using MySql.Data.MySqlClient;   // Ważne dla MySqlConnection
using System.Linq;
using System.Text;
using Dapper;               // Ważne dla Query<T>

namespace Reklamacje_Dane
{
    public class ContactRepository
    {
      private string _connectionString => DbConfig.ConnectionString;

        // =================================================================
        // SEKCJA: KONTA POCZTOWE (POPRAWIONE)
        // =================================================================

        public void EnsureEmailTableExists()
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS KontaPocztowe (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        NazwaWyswietlana TEXT,
                        AdresEmail VARCHAR(255) NOT NULL,
                        Login TEXT,
                        Haslo TEXT NOT NULL,
                        Protokol VARCHAR(50) DEFAULT 'POP3',
                        Pop3Host TEXT,
                        Pop3Port INT,
                        Pop3Ssl INT,
                        ImapHost TEXT,
                        ImapPort INT,
                        ImapSsl INT,
                        SmtpHost TEXT,
                        SmtpPort INT,
                        SmtpSsl INT,
                        PodpisHtml LONGTEXT,
                        CzyDomyslne INT DEFAULT 0
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";
                using (var cmd = new MySqlCommand(sql, conn)) cmd.ExecuteNonQuery();
            
        }
        }

        public void DodajKontoPocztowe(KontoPocztowe konto)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                // Jeśli to konto ma być domyślne, zdejmij flagę z innych
                if (konto.CzyDomyslne)
                {
                    new MySqlCommand("UPDATE KontaPocztowe SET CzyDomyslne = 0", conn).ExecuteNonQuery();
                }

                // Hasło (szyfrowane lub nie, zależy od SecurityHelpera - tu zakładam, że jest prosty tekst, jeśli SecurityHelper nie działa)
                // string encryptedPass = SecurityHelper.Encrypt(konto.Haslo); 
                // Używamy hasła wprost, chyba że masz działający SecurityHelper
                string encryptedPass = konto.Haslo;

                string sql = @"
                    INSERT INTO KontaPocztowe 
                    (NazwaWyswietlana, AdresEmail, Login, Haslo, Protokol,
                     Pop3Host, Pop3Port, Pop3Ssl, 
                     ImapHost, ImapPort, ImapSsl,
                     SmtpHost, SmtpPort, SmtpSsl, 
                     PodpisHtml, CzyDomyslne)
                    VALUES 
                    (@Nazwa, @Email, @Login, @Haslo, @Protokol,
                     @Pop3Host, @Pop3Port, @Pop3Ssl,
                     @ImapHost, @ImapPort, @ImapSsl,
                     @SmtpHost, @SmtpPort, @SmtpSsl,
                     @Podpis, @Domyslne)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nazwa", konto.NazwaWyswietlana ?? "");
                    cmd.Parameters.AddWithValue("@Email", konto.AdresEmail ?? "");
                    cmd.Parameters.AddWithValue("@Login", konto.Login ?? "");
                    cmd.Parameters.AddWithValue("@Haslo", encryptedPass);
                    cmd.Parameters.AddWithValue("@Protokol", konto.Protokol ?? "POP3");

                    cmd.Parameters.AddWithValue("@Pop3Host", konto.Pop3Host ?? "");
                    cmd.Parameters.AddWithValue("@Pop3Port", konto.Pop3Port);
                    cmd.Parameters.AddWithValue("@Pop3Ssl", konto.Pop3Ssl ? 1 : 0);

                    cmd.Parameters.AddWithValue("@ImapHost", konto.ImapHost ?? "");
                    cmd.Parameters.AddWithValue("@ImapPort", konto.ImapPort);
                    cmd.Parameters.AddWithValue("@ImapSsl", konto.ImapSsl ? 1 : 0);

                    cmd.Parameters.AddWithValue("@SmtpHost", konto.SmtpHost ?? "");
                    cmd.Parameters.AddWithValue("@SmtpPort", konto.SmtpPort);
                    cmd.Parameters.AddWithValue("@SmtpSsl", konto.SmtpSsl ? 1 : 0);

                    // Tutaj wpada HTML z edytora
                    cmd.Parameters.AddWithValue("@Podpis", konto.Podpis ?? "");
                    cmd.Parameters.AddWithValue("@Domyslne", konto.CzyDomyslne ? 1 : 0);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AktualizujKonto(KontoPocztowe konto)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                if (konto.CzyDomyslne)
                {
                    new MySqlCommand("UPDATE KontaPocztowe SET CzyDomyslne = 0", conn).ExecuteNonQuery();
                }

                // Hasło (szyfrowane lub nie)
                // string encryptedPass = SecurityHelper.Encrypt(konto.Haslo);
                string encryptedPass = konto.Haslo;

                string sql = @"
                    UPDATE KontaPocztowe SET 
                       NazwaWyswietlana=@Nazwa, 
                       AdresEmail=@Email, 
                       Login=@Login, 
                       Haslo=@Haslo, 
                       Protokol=@Protokol,
                       Pop3Host=@Pop3Host, Pop3Port=@Pop3Port, Pop3Ssl=@Pop3Ssl,
                       ImapHost=@ImapHost, ImapPort=@ImapPort, ImapSsl=@ImapSsl,
                       SmtpHost=@SmtpHost, SmtpPort=@SmtpPort, SmtpSsl=@SmtpSsl,
                       PodpisHtml=@Podpis, 
                       CzyDomyslne=@Domyslne
                    WHERE Id=@Id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nazwa", konto.NazwaWyswietlana ?? "");
                    cmd.Parameters.AddWithValue("@Email", konto.AdresEmail ?? "");
                    cmd.Parameters.AddWithValue("@Login", konto.Login ?? "");
                    cmd.Parameters.AddWithValue("@Haslo", encryptedPass);
                    cmd.Parameters.AddWithValue("@Protokol", konto.Protokol ?? "POP3");

                    cmd.Parameters.AddWithValue("@Pop3Host", konto.Pop3Host ?? "");
                    cmd.Parameters.AddWithValue("@Pop3Port", konto.Pop3Port);
                    cmd.Parameters.AddWithValue("@Pop3Ssl", konto.Pop3Ssl ? 1 : 0);

                    cmd.Parameters.AddWithValue("@ImapHost", konto.ImapHost ?? "");
                    cmd.Parameters.AddWithValue("@ImapPort", konto.ImapPort);
                    cmd.Parameters.AddWithValue("@ImapSsl", konto.ImapSsl ? 1 : 0);

                    cmd.Parameters.AddWithValue("@SmtpHost", konto.SmtpHost ?? "");
                    cmd.Parameters.AddWithValue("@SmtpPort", konto.SmtpPort);
                    cmd.Parameters.AddWithValue("@SmtpSsl", konto.SmtpSsl ? 1 : 0);

                    cmd.Parameters.AddWithValue("@Podpis", konto.Podpis ?? "");
                    cmd.Parameters.AddWithValue("@Domyslne", konto.CzyDomyslne ? 1 : 0);

                    cmd.Parameters.AddWithValue("@Id", konto.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<KontoPocztowe> PobierzWszystkieKonta()
        {
            var lista = new List<KontoPocztowe>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM KontaPocztowe";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var k = new KontoPocztowe();
                        k.Id = Convert.ToInt32(reader["Id"]);
                        k.NazwaWyswietlana = reader["NazwaWyswietlana"].ToString();
                        k.AdresEmail = reader["AdresEmail"].ToString();
                        k.Login = reader["Login"].ToString();
                        k.Haslo = reader["Haslo"].ToString();
                        k.Protokol = reader["Protokol"].ToString();

                        k.Pop3Host = reader["Pop3Host"].ToString();
                        k.Pop3Port = Convert.ToInt32(reader["Pop3Port"]);
                        k.Pop3Ssl = SafeGetBool(reader["Pop3Ssl"]); // Używamy SafeGetBool

                        k.ImapHost = reader["ImapHost"]?.ToString() ?? "";
                        k.ImapPort = reader["ImapPort"] != DBNull.Value ? Convert.ToInt32(reader["ImapPort"]) : 0;
                        k.ImapSsl = SafeGetBool(reader["ImapSsl"]);

                        k.SmtpHost = reader["SmtpHost"].ToString();
                        k.SmtpPort = Convert.ToInt32(reader["SmtpPort"]);
                        k.SmtpSsl = SafeGetBool(reader["SmtpSsl"]);

                        // Czytamy z PodpisHtml, mapujemy na Podpis
                        if (reader["PodpisHtml"] != DBNull.Value)
                            k.Podpis = reader["PodpisHtml"].ToString();
                        else
                            k.Podpis = "";

                        k.CzyDomyslne = SafeGetBool(reader["CzyDomyslne"]);

                        lista.Add(k);
                    }
                }
            }
            return lista;
        }

        // Ta metoda jest wymagana przez Twój kod w innych miejscach
        public List<KontoPocztowe> PobierzKontaPocztowe()
        {
            return PobierzWszystkieKonta();
        }

        public void UsunKonto(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM KontaPocztowe WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- POMOCNICZA METODA DO BOOLEAN ---
        private bool SafeGetBool(object dbValue)
        {
            if (dbValue == null || dbValue == DBNull.Value) return false;
            string val = dbValue.ToString().Trim().ToLower();
            if (val == "1") return true;
            if (val == "true") return true;
            return false;
        }

        // =================================================================
        // SEKCJA: ZAŁĄCZNIKI I HISTORIA (ZACHOWANA Z ORYGINAŁU)
        // =================================================================

        public void ZapiszZalacznikInfo(string msgId, string nazwaOryginalna, string nazwaNaDysku, string typ)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Zalaczniki (MessageID, NazwaPliku, NazwaNaDysku, TypMime) 
                                VALUES (@mid, @nazwaOrg, @nazwaDysk, @typ)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mid", msgId);
                    cmd.Parameters.AddWithValue("@nazwaOrg", nazwaOryginalna);
                    cmd.Parameters.AddWithValue("@nazwaDysk", nazwaNaDysku);
                    cmd.Parameters.AddWithValue("@typ", typ);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public string PobierzMessageIdPoId(int idWiersza)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT MessageID FROM CentrumKontaktu WHERE Id = @id";
                return conn.QueryFirstOrDefault<string>(sql, new { id = idWiersza });
            }
        }

        public DataTable PobierzListeZalacznikow(string messageId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, NazwaPliku, NazwaNaDysku FROM Zalaczniki WHERE MessageID = @mid";
                var dt = new DataTable();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mid", messageId);
                    using (var reader = cmd.ExecuteReader()) dt.Load(reader);
                }
                return dt;
            }
        }

        public bool CzyMailIstnieje(string uniqueId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(1) FROM CentrumKontaktu WHERE MessageID = @uid";
                int count = conn.ExecuteScalar<int>(sql, new { uid = uniqueId });
                return count > 0;
            }
        }

        public void ZapiszSmsWychodzacy(string numer, string tresc, int? klientId, int? zgloszenieId)
        {
            string dataTeraz = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = @"
                INSERT INTO CentrumKontaktu 
                (KlientID, ZgloszenieID, DataWyslania, Typ, Tresc, Kierunek, Uzytkownik) 
                VALUES 
                (@KlientId, @ZgloszenieId, @Data, 'SMS', @Tresc, 'OUT', 'Admin')";

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@KlientId", klientId.HasValue ? (object)klientId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ZgloszenieId", zgloszenieId.HasValue ? (object)zgloszenieId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Data", dataTeraz);
                    cmd.Parameters.AddWithValue("@Tresc", tresc);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int? ZnajdzIdZgloszeniaPoNumerze(string numerZgloszenia)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT Id FROM Zgloszenia WHERE NrZgloszenia = @nr LIMIT 1";
                var result = conn.ExecuteScalar<int?>(sql, new { nr = numerZgloszenia });
                return result;
            }
        }

        public bool CzyWiadomoscIstnieje(string messageId)
        {
            if (string.IsNullOrEmpty(messageId)) return false;
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                int count = conn.ExecuteScalar<int>("SELECT COUNT(1) FROM CentrumKontaktu WHERE MessageID = @mid", new { mid = messageId });
                return count > 0;
            }
        }

        public DataTable PobierzMaileDoSkrzynki(bool tylkoNieprzypisane)
        {
            string sql = @"
                SELECT 
                    ck.Id, 
                    IFNULL(kp.NazwaWyswietlana, IFNULL(kp.AdresEmail, 'Nieznane')) AS Konto, 
                    ck.DataWyslania, 
                    IFNULL(ck.SenderName, IFNULL(k.ImieNazwisko, ck.Tytul)) AS Nadawca, 
                    IFNULL(ck.Tytul, '(Brak tematu)') AS Tytul, 
                    IFNULL(z.NrZgloszenia, '') AS NrZgloszenia,
                    ck.Tresc,
                    ck.Typ
                FROM CentrumKontaktu ck
                LEFT JOIN Zgloszenia z ON ck.ZgloszenieID = z.Id
                LEFT JOIN Klienci k ON ck.KlientID = k.Id
                LEFT JOIN KontaPocztowe kp ON ck.AccountID = kp.Id 
                WHERE ck.Typ = 'Mail' ";

            if (tylkoNieprzypisane)
            {
                sql += " AND ck.ZgloszenieID IS NULL";
            }

            sql += " ORDER BY ck.DataWyslania DESC LIMIT 100";

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var dt = new DataTable();
                using (var cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                return dt;
            }
        }

        public void PrzypiszMailDoZgloszenia(int mailId, int zgloszenieId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute("UPDATE CentrumKontaktu SET ZgloszenieID = @zid WHERE Id = @mid", new { zid = zgloszenieId, mid = mailId });
            }
        }

        public List<ConversationThread> GetThreadsByClient()
        {
            string sql = @"
                SELECT
                    IFNULL(k.Id, 0) AS EntityId,
                    TRIM(CONCAT(IFNULL(k.NazwaFirmy, ''), ' ', IFNULL(k.ImieNazwisko, 'Nieznany Klient'))) AS Title,
                    MAX(ck.DataWyslania) AS LastDate,
                    COUNT(*) AS Count
                FROM CentrumKontaktu ck
                LEFT JOIN klienci k ON ck.KlientID = k.Id
                WHERE ck.KlientID IS NOT NULL
                GROUP BY ck.KlientID
                ORDER BY LastDate DESC";

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                return conn.Query<ConversationThread>(sql).ToList();
            }
        }

        public List<ConversationThread> GetThreadsByClaim()
        {
            string sql = @"
                SELECT
                    z.Id AS EntityId,
                    CAST(z.NrZgloszenia AS CHAR) AS ClaimNumber,
                    CONCAT('Zgłoszenie ', IFNULL(z.NrZgloszenia, CAST(z.Id AS CHAR))) AS Title,
                    MAX(ck.DataWyslania) AS LastDate,
                    COUNT(*) AS Count
                FROM CentrumKontaktu ck
                JOIN Zgloszenia z ON ck.ZgloszenieID = z.Id
                GROUP BY z.Id
                ORDER BY LastDate DESC";

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                return conn.Query<ConversationThread>(sql).ToList();
            }
        }

        public List<RawMessage> GetHistoryForThread(int? klientId, int? zgloszenieId)
        {
            var sqlBuilder = new StringBuilder();

            // 1. Centrum Kontaktu
            sqlBuilder.AppendLine(@"
                SELECT
                    CAST(ck.Id AS CHAR) AS Id,
                    ck.DataWyslania AS Data,
                    ck.Typ,
                    CASE WHEN ck.Typ IN ('SMSOtrzymany', 'Mail') THEN 'IN' ELSE 'OUT' END AS Kierunek,
                    ck.Tresc,
                    ck.Uzytkownik AS Autor
                FROM CentrumKontaktu ck
                WHERE 1=1 ");

            if (klientId.HasValue)
            {
                if (klientId == 0) sqlBuilder.AppendLine($" AND ck.KlientID IS NULL");
                else sqlBuilder.AppendLine($" AND ck.KlientID = {klientId}");
            }
            if (zgloszenieId.HasValue) sqlBuilder.AppendLine($" AND ck.ZgloszenieID = {zgloszenieId}");

            // 2. Allegro
            sqlBuilder.AppendLine(" UNION ALL ");
            sqlBuilder.AppendLine(@"
                SELECT 
                    acm.MessageId AS Id,
                    acm.CreatedAt AS Data,
                    'Allegro' AS Typ,
                    CASE WHEN acm.AuthorRole = 'BUYER' THEN 'IN' ELSE 'OUT' END AS Kierunek,
                    acm.MessageText AS Tresc,
                    acm.AuthorLogin AS Autor
                FROM AllegroChatMessages acm
                JOIN AllegroDisputes ad ON acm.DisputeId = ad.DisputeId
                WHERE 1=1 ");

            if (zgloszenieId.HasValue) sqlBuilder.AppendLine($" AND ad.ComplaintId = {zgloszenieId}");
            if (klientId.HasValue && klientId != 0) sqlBuilder.AppendLine($@" AND ad.BuyerEmail IN (SELECT Email FROM klienci WHERE Id = {klientId})");

            sqlBuilder.AppendLine(" ORDER BY 2 DESC");

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                var result = conn.Query<RawMessage>(sqlBuilder.ToString()).ToList();
                // Opcjonalnie: Konwersja RTF do tekstu, jeśli używasz
                // foreach (var r in result) r.Tresc = RtfHelper.ConvertRtfToText(r.Tresc);
                return result;
            }
        }
    }
}