using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class EmailSyncResult
    {
        public int UnreadCount { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class EmailService
    {
        private ContactRepository _repo = new ContactRepository();

        // Metoda do szybkiego sprawdzenia licznika (bez pobierania treści)
        public async Task<EmailSyncResult> GetUnreadCountAsync()
        {
            try
            {
                // Tutaj tylko wykonujemy szybki synchron - pobieramy maile
                await PobierzPoczteDlaWszystkichKontAsync();

                // I zwracamy liczbę z bazy (bezpieczniej niż liczyć w locie na serwerze)
                int count = 0;
                using (var con = Database.GetNewOpenConnection())
                {
                    // Liczymy maile z ostatnich 3 dni, które są przychodzące
                    // Możesz tu dodać flagę 'Przeczytane', jeśli ją masz w bazie
                    var cmd = new MySqlCommand("SELECT COUNT(*) FROM CentrumKontaktu WHERE Typ='Mail' AND Kierunek='IN' AND DataWyslania > DATE_SUB(NOW(), INTERVAL 3 DAY)", con);
                    count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
                return new EmailSyncResult { Success = true, UnreadCount = count, Message = "OK" };
            }
            catch (Exception ex)
            {
                return new EmailSyncResult { Success = false, Message = ex.Message };
            }
        }

        public async Task PobierzPoczteDlaWszystkichKontAsync()
        {
            // Pobieramy listę kont (Repozytorium samo zarządza połączeniem)
            var konta = _repo.PobierzWszystkieKonta();

            foreach (var konto in konta)
            {
                await PobierzZKontaAsync(konto);
            }
        }

        private async Task PobierzZKontaAsync(KontoPocztowe konto)
        {
            try
            {
                string password = konto.Haslo;
                // Opcjonalnie: odszyfrowanie hasła

                if (konto.Protokol == "IMAP")
                {
                    using (var client = new ImapClient())
                    {
                        client.Timeout = 15000; // Limit czasu 15s
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        await client.ConnectAsync(konto.ImapHost, konto.ImapPort, konto.ImapSsl);
                        await client.AuthenticateAsync(konto.Login, password);
                        await client.Inbox.OpenAsync(FolderAccess.ReadOnly);

                        // Pobieramy tylko ostatnie 50 maili (żeby nie mulić bazy przy tysiącach)
                        // Lepsza strategia: Szukaj UID > ostatniego zapisanego UID w bazie.
                        // Ale dla uproszczenia bierzemy ostatnie.
                        int total = client.Inbox.Count;
                        int start = Math.Max(0, total - 50);

                        var items = await client.Inbox.FetchAsync(start, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope);

                        foreach (var item in items)
                        {
                            string msgIdStr = item.UniqueId.ToString();
                            string headerId = item.Envelope.MessageId ?? msgIdStr;

                            // Szybkie sprawdzenie w bazie przed pobraniem treści
                            if (_repo.CzyMailIstnieje(headerId)) continue;

                            // Pobieramy treść tylko jeśli nie ma w bazie
                            var message = await client.Inbox.GetMessageAsync(item.UniqueId);
                            ZapiszWiadomoscDoBazy(message, headerId, konto.Id);
                        }
                        await client.DisconnectAsync(true);
                    }
                }
                else // POP3
                {
                    using (var client = new Pop3Client())
                    {
                        client.Timeout = 15000;
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        string host = !string.IsNullOrEmpty(konto.Pop3Host) ? konto.Pop3Host : konto.ImapHost;
                        int port = konto.Pop3Port > 0 ? konto.Pop3Port : 995;

                        await client.ConnectAsync(host, port, konto.Pop3Ssl);
                        await client.AuthenticateAsync(konto.Login, password);

                        // POP3 nie pozwala na łatwe pobieranie "ostatnich", trzeba iterować
                        int count = client.Count;
                        // Sprawdzamy ostatnie 20
                        int start = Math.Max(0, count - 20);

                        for (int i = start; i < count; i++)
                        {
                            var message = await client.GetMessageAsync(i);
                            string headerId = message.MessageId; // POP3 zazwyczaj nie ma UID w tym sensie co IMAP

                            if (string.IsNullOrEmpty(headerId) || _repo.CzyMailIstnieje(headerId)) continue;

                            ZapiszWiadomoscDoBazy(message, headerId, konto.Id);
                        }
                        await client.DisconnectAsync(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd pobierania z {konto.AdresEmail}: {ex.Message}");
            }
        }

        private void ZapiszWiadomoscDoBazy(MimeMessage msg, string uid, int kontoId)
        {
            string nadawcaEmail = msg.From.Mailboxes.FirstOrDefault()?.Address ?? "";
            string nadawcaNazwa = msg.From.ToString();
            string temat = msg.Subject ?? "(Brak tematu)";
            string tresc = !string.IsNullOrEmpty(msg.HtmlBody) ? msg.HtmlBody : msg.TextBody;

            // Szukanie powiązania (Regex)
            int? znalezioneZgloszenieId = null;
            var match = Regex.Match(temat, @"\b(\d{1,5}/\d{2,4})\b");
            if (match.Success)
            {
                znalezioneZgloszenieId = _repo.ZnajdzIdZgloszeniaPoNumerze(match.Groups[1].Value);
            }
            else
            {
                var matchId = Regex.Match(temat, @"\[ID:\s*(\d+)\]");
                if (matchId.Success && int.TryParse(matchId.Groups[1].Value, out int nid))
                    znalezioneZgloszenieId = nid;
            }

            // BEZPIECZNY ZAPIS DO BAZY
            // Używamy globalnego helpera, aby uniknąć "database is locked"
            using (var conn = Database.GetNewOpenConnection())
            {
                string sql = @"
                    INSERT INTO CentrumKontaktu 
                    (KlientID, ZgloszenieID, DataWyslania, Typ, Kierunek, Tresc, Tytul, Uzytkownik, MessageID, AccountID, SenderName) 
                    VALUES 
                    (@KlientId, @ZgloszenieId, @Data, 'Mail', 'IN', @Tresc, @Temat, 'Klient', @MsgId, @AccId, @Sender)";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@KlientId", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ZgloszenieId", znalezioneZgloszenieId.HasValue ? (object)znalezioneZgloszenieId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Data", msg.Date.DateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Tresc", tresc);
                    cmd.Parameters.AddWithValue("@Temat", temat);
                    cmd.Parameters.AddWithValue("@MsgId", uid);
                    cmd.Parameters.AddWithValue("@AccId", kontoId);
                    cmd.Parameters.AddWithValue("@Sender", nadawcaNazwa);
                    cmd.ExecuteNonQuery();
                }
            }

            // Obsługa Załączników
            if (msg.Attachments.Any())
            {
                string folderZalacznikow = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Zalaczniki");
                if (!Directory.Exists(folderZalacznikow)) Directory.CreateDirectory(folderZalacznikow);

                foreach (var attachment in msg.Attachments)
                {
                    if (attachment is MimePart part)
                    {
                        string safeFileName = string.Join("_", part.FileName.Split(Path.GetInvalidFileNameChars()));
                        string unikalnaNazwa = Guid.NewGuid().ToString("N") + "_" + safeFileName;
                        string pelnaSciezka = Path.Combine(folderZalacznikow, unikalnaNazwa);

                        using (var stream = File.Create(pelnaSciezka))
                        {
                            part.Content.DecodeTo(stream);
                        }

                        _repo.ZapiszZalacznikInfo(uid, part.FileName, unikalnaNazwa, part.ContentType.MimeType);
                    }
                }
            }
        }

        // (Metoda WyslijEmailAsync pozostaje bez zmian - SMTP nie blokuje bazy SQLite)
        public async Task WyslijEmailAsync(KontoPocztowe konto, string doKogo, string temat, string trescHtml, List<string> sciezkiZalacznikow)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(konto.NazwaWyswietlana, konto.AdresEmail));
            message.To.Add(MailboxAddress.Parse(doKogo));
            message.Subject = temat;
            var builder = new BodyBuilder { HtmlBody = trescHtml };
            if (sciezkiZalacznikow != null) foreach (var path in sciezkiZalacznikow) if (File.Exists(path)) builder.Attachments.Add(path);
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                SecureSocketOptions sslOption = (konto.SmtpPort == 465) ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto;
                await client.ConnectAsync(konto.SmtpHost, konto.SmtpPort, sslOption);
                await client.AuthenticateAsync(konto.Login, konto.Haslo);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}