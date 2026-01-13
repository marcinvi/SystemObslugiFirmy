// Plik: MagazynControl.SendDecisionEmails.cs (Wersja finalna, kompletna)
// C# 7.3 compatible

using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutlookInterop = Microsoft.Office.Interop.Outlook;

namespace Reklamacje_Dane
{
    public partial class MagazynControl : UserControl
    {
        // ====== MODELE ======
        private class PendingReturnItem
        {
            public int Id { get; set; }
            public string Reference { get; set; }
            public string Buyer { get; set; }
            public string Product { get; set; }
            public int Quantity { get; set; }
            public string StanProduktu { get; set; }
            public string UwagiMagazynu { get; set; }
            public int? PrzyjetyPrzezId { get; set; }
            public DateTime? DataPrzyjecia { get; set; }
            public bool IsManual { get; set; }
            public int? AllegroAccountId { get; set; }
            public int? RecipientId { get; set; }
            public string BuyerFullName { get; set; }
        }

        private class SalesmanTarget
        {
            public int UserId;
            public string DisplayName;
            public string Email;
            public override int GetHashCode() => UserId.GetHashCode();
            public override bool Equals(object obj) => (obj is SalesmanTarget other) && UserId == other.UserId;
        }

        // ====== PUBLICZNY HANDLER DO DESIGNERA ======
        private async void btnWyslijZwrotyMail_Click(object sender, EventArgs e)
        {
            await SendDecisionEmailsAsync();
        }

        // ====== GŁÓWNA METODA ======
        private async Task SendDecisionEmailsAsync()
        {
            try
            {
                // 1) Ustal/zweryfikuj konto nadawcy z Ustawienia.domyslnymail + Outlook
                string senderEmail = await EnsureDefaultSenderEmailAsync();
                if (string.IsNullOrWhiteSpace(senderEmail))
                {
                    MessageBox.Show("Nie wybrano konta nadawcy. Operacja przerwana.", "Informacja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 2) Pobierz listę pozycji do decyzji i zgrupuj po handlowcach/odbiorcach
                var grouped = await LoadPendingReturnsGroupedBySalesmanAsync();
                if (grouped.Count == 0)
                {
                    MessageBox.Show("Brak zwrotów oczekujących na decyzję handlowca.", "Informacja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 3) Wyślij maile — pasek postępu
                using (var progress = new SimpleProgressForm("Wysyłanie wiadomości..."))
                {
                    progress.Show(this);
                    progress.SetRange(0, grouped.Count);

                    OutlookInterop.Application outlookApp = null;
                    try
                    {
                        outlookApp = new OutlookInterop.Application();

                        // znajdź konto nadawcy w Outlook
                        var account = FindOutlookAccount(outlookApp, senderEmail);
                        if (account == null)
                        {
                            MessageBox.Show(
                                "Wybrane konto nadawcy nie jest dostępne w Outlook. Wybierz ponownie w Ustawieniach i spróbuj ponownie.",
                                "Błąd konta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int step = 0;
                        foreach (var kv in grouped)
                        {
                            var salesman = kv.Key;      // SalesmanTarget
                            var items = kv.Value;       // List<PendingReturnItem>
                            string html = await BuildHtmlForSalesmanAsync(salesman.DisplayName, items);
                            string subject = "Zwroty oczekujące na decyzję";

                            await SendOutlookMailAsync(outlookApp, account, salesman.Email, subject, html);
                            step++;
                            progress.SetValue(step);
                            progress.SetStatus($"Wysłano do: {salesman.DisplayName} ({salesman.Email})");
                            await Task.Delay(100); // kosmetyka UI
                        }
                    }
                    finally
                    {
                        if (outlookApp != null)
                        {
                            try { Marshal.FinalReleaseComObject(outlookApp); } catch { }
                        }
                    }
                }

                // Logowanie sukcesu operacji BEZPOŚREDNIO do dziennika magazynu
                string logMessage = $"Wysłano powiadomienia o zwrotach do decyzji do {grouped.Count} handlowców.";
                await AddMagazynLogEntryAsync(Program.fullName, logMessage, "System-Magazyn");

                MessageBox.Show("Wiadomości zostały wysłane.", "Sukces",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Logowanie błędu BEZPOŚREDNIO do dziennika magazynu
                string errorMessage = $"[BŁĄD KRYTYCZNY] Nie udało się wysłać powiadomień do handlowców: {ex.Message}";
                await AddMagazynLogEntryAsync(Program.fullName, errorMessage, "System-Magazyn");

                MessageBox.Show("Błąd wysyłania wiadomości: " + ex.Message, "Błąd krytyczny",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Dodaje wpis do dziennika w bazie danych magazyn.db.
        /// </summary>
        private async Task AddMagazynLogEntryAsync(string uzytkownik, string opis, string nrZgloszenia = null)
        {
            try
            {
                using (var con = MagazynDatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "INSERT INTO Dziennik (Uzytkownik, Opis, NrZgloszenia, Data) VALUES (@u, @o, @n, @d)";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@u", uzytkownik);
                        cmd.Parameters.AddWithValue("@o", opis);
                        cmd.Parameters.AddWithValue("@n", (object)nrZgloszenia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@d", DateTime.Now);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KRYTYCZNY BŁĄD: Nie udało się zapisać wpisu do dziennika magazynu! {ex.Message}");
            }
        }

        // ====== KROK 1: NADAWCA Z USTAWIEŃ/OUTLOOK ======

        private async Task<string> EnsureDefaultSenderEmailAsync()
        {
            string key = "domyslnymail";
            string saved = await ReadSettingAsync(key);
            List<OutlookAccountInfo> accounts = GetOutlookAccounts();

            if (string.IsNullOrWhiteSpace(saved) || !accounts.Any(a => EmailsEqual(a.SmtpAddress, saved)))
            {
                string selected = PromptOutlookAccount(accounts, string.IsNullOrWhiteSpace(saved)
                    ? "Wybierz konto nadawcy do wysyłki e-mail:"
                    : $"Zapisane konto „{saved}” nie istnieje w Outlook.\nWybierz dostępne konto do wysyłki:");

                if (string.IsNullOrWhiteSpace(selected))
                    return null;

                await SaveSettingAsync(key, selected);
                return selected;
            }

            return saved;
        }

        private class OutlookAccountInfo
        {
            public string DisplayName;
            public string SmtpAddress;
            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(DisplayName) || DisplayName == SmtpAddress) return SmtpAddress;
                return DisplayName + " <" + SmtpAddress + ">";
            }
        }

        private static bool EmailsEqual(string a, string b)
        {
            if (a == null || b == null) return false;
            return string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private List<OutlookAccountInfo> GetOutlookAccounts()
        {
            var list = new List<OutlookAccountInfo>();
            OutlookInterop.Application app = null;
            try
            {
                app = new OutlookInterop.Application();
                var accounts = app.Session.Accounts;
                foreach (OutlookInterop.Account acc in accounts)
                {
                    try
                    {
                        string smtp = SafeGetSmtpAddress(acc);
                        if (!string.IsNullOrWhiteSpace(smtp))
                        {
                            list.Add(new OutlookAccountInfo
                            {
                                DisplayName = acc.DisplayName,
                                SmtpAddress = smtp
                            });
                        }
                    }
                    finally
                    {
                        if (acc != null) Marshal.FinalReleaseComObject(acc);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas pobierania kont Outlook: " + ex.Message);
            }
            finally
            {
                if (app != null) Marshal.FinalReleaseComObject(app);
            }

            var distinct = list
                .GroupBy(x => x.SmtpAddress, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .OrderBy(x => x.SmtpAddress, StringComparer.OrdinalIgnoreCase)
                .ToList();
            return distinct;
        }

        private string SafeGetSmtpAddress(OutlookInterop.Account account)
        {
            const string PR_SMTP_ADDRESS = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E";

            if (account == null) return null;

            try
            {
                if (!string.IsNullOrEmpty(account.SmtpAddress) && account.SmtpAddress.Contains("@"))
                {
                    return account.SmtpAddress;
                }

                OutlookInterop.AddressEntry entry = account.CurrentUser.AddressEntry;
                if (entry != null)
                {
                    if (entry.Type == "EX")
                    {
                        OutlookInterop.ExchangeUser user = entry.GetExchangeUser();
                        if (user != null)
                        {
                            return user.PrimarySmtpAddress;
                        }
                    }
                    else
                    {
                        return entry.PropertyAccessor.GetProperty(PR_SMTP_ADDRESS) as string;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Nie udało się uzyskać adresu SMTP dla konta {account.DisplayName}. Błąd: {ex.Message}");
            }

            return null;
        }

        private string PromptOutlookAccount(List<OutlookAccountInfo> accounts, string title)
        {
            if (accounts == null || accounts.Count == 0)
            {
                MessageBox.Show("Nie wykryto żadnych kont pocztowych w Outlook.", "Brak kont",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            using (var f = new Form())
            using (var lbl = new Label())
            using (var cb = new ComboBox())
            using (var ok = new Button())
            using (var cancel = new Button())
            {
                f.Text = "Wybór konta nadawcy";
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MinimizeBox = false; f.MaximizeBox = false;
                f.ClientSize = new System.Drawing.Size(560, 150);

                lbl.Text = title;
                lbl.AutoSize = false; lbl.Left = 12; lbl.Top = 12; lbl.Width = 536; lbl.Height = 30;

                cb.Left = 12; cb.Top = 46; cb.Width = 536;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.DataSource = accounts;
                cb.DisplayMember = "ToString";
                cb.ValueMember = "SmtpAddress";

                ok.Text = "OK"; ok.DialogResult = DialogResult.OK; ok.Width = 100; ok.Height = 28;
                cancel.Text = "Anuluj"; cancel.DialogResult = DialogResult.Cancel; cancel.Width = 100; cancel.Height = 28;

                f.Controls.Add(lbl); f.Controls.Add(cb); f.Controls.Add(ok); f.Controls.Add(cancel);

                f.Shown += (s, e) =>
                {
                    ok.Left = f.ClientSize.Width - ok.Width - 12;
                    cancel.Left = ok.Left - cancel.Width - 8;
                    ok.Top = cancel.Top = f.ClientSize.Height - ok.Height - 12;
                };

                f.AcceptButton = ok; f.CancelButton = cancel;

                var res = f.ShowDialog(this);
                if (res == DialogResult.OK)
                {
                    var sel = cb.SelectedItem as OutlookAccountInfo;
                    return sel != null ? sel.SmtpAddress : null;
                }
                return null;
            }
        }

        private async Task<string> ReadSettingAsync(string key)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand("SELECT WartoscZaszyfrowana FROM Ustawienia WHERE Klucz=@k LIMIT 1", con))
                {
                    cmd.Parameters.AddWithValue("@k", key);
                    var obj = await cmd.ExecuteScalarAsync();
                    return obj == null || obj == DBNull.Value ? null : Convert.ToString(obj);
                }
            }
        }

        private async Task SaveSettingAsync(string key, string value)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(@"
INSERT INTO Ustawienia (Klucz, WartoscZaszyfrowana)
      VALUES (@k, @v)
	ON DUPLICATE KEY UPDATE WartoscZaszyfrowana=VALUES(WartoscZaszyfrowana);", con))
                {
                    cmd.Parameters.AddWithValue("@k", key);
                    cmd.Parameters.AddWithValue("@v", value ?? "");
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // ====== KROK 2: POBIERZ DANE DO MAILI ======
        private async Task<Dictionary<SalesmanTarget, List<PendingReturnItem>>> LoadPendingReturnsGroupedBySalesmanAsync()
        {
            var result = new Dictionary<SalesmanTarget, List<PendingReturnItem>>();

            int statusDecyzjaId = await GetStatusIdAsync("Oczekuje na decyzję handlowca", "StatusWewnetrzny");
            if (statusDecyzjaId == 0) return result;

            var items = new List<PendingReturnItem>();
            string uwagiColumn = await ResolveUwagiMagazynuColumnAsync();

            using (var conMag = MagazynDatabaseHelper.GetConnection())
            {
                await conMag.OpenAsync();
                string q = $@"
	SELECT acr.Id,
	       acr.ReferenceNumber,
	       acr.BuyerLogin,
	       acr.ProductName,
	       acr.Quantity,
	       acr.StanProduktuId,
	       acr.{uwagiColumn} AS UwagiMagazynu,
	       acr.PrzyjetyPrzezId,
	       acr.DataPrzyjecia,
	       acr.IsManual,
	       acr.BuyerFullName,
	       acr.AllegroAccountId
	FROM AllegroCustomerReturns acr
	WHERE IFNULL(acr.StatusWewnetrznyId,0)=@st;
	";
                using (var cmd = new MySqlCommand(q, conMag))
                {
                    cmd.Parameters.AddWithValue("@st", statusDecyzjaId);
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        while (await r.ReadAsync())
                        {
                            var pi = new PendingReturnItem
                            {
                                Id = r.GetInt32(0),
                                Reference = r.GetString(1),
                                Buyer = r.IsDBNull(9) || r.GetBoolean(9) ? (r.IsDBNull(10) ? r.GetString(2) : r.GetString(10)) : r.GetString(2),
                                Product = r.IsDBNull(3) ? string.Empty : r.GetString(3),
                                Quantity = r.IsDBNull(4) ? 0 : r.GetInt32(4),
                                UwagiMagazynu = r.IsDBNull(6) ? string.Empty : r.GetString(6),
                                PrzyjetyPrzezId = r.IsDBNull(7) ? (int?)null : r.GetInt32(7),
                                DataPrzyjecia = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8),
                                IsManual = !r.IsDBNull(9) && r.GetBoolean(9),
                                AllegroAccountId = r.IsDBNull(11) ? (int?)null : r.GetInt32(11),
                                StanProduktu = await GetStatusNameByIdAsync(r.IsDBNull(5) ? (int?)null : r.GetInt32(5))
                            };
                            items.Add(pi);
                        }
                    }
                }
            }

            // Grupowanie zwrotów na podstawie ich przypisania
            foreach (var item in items)
            {
                List<int> targetUserIds = new List<int>();

                if (item.IsManual)
                {
                    targetUserIds = await GetRecipientsForManualReturnAsync(item.Id);
                }
                else
                {
                    if (item.AllegroAccountId.HasValue)
                    {
                        int? opiekunId = await GetOpiekunIdForAccountAsync(item.AllegroAccountId.Value);
                        if (opiekunId.HasValue)
                        {
                            targetUserIds.Add(opiekunId.Value);
                        }
                    }
                }

                foreach (var targetUserId in targetUserIds)
                {
                    int finalUserId = await GetActiveDelegateIdAsync(targetUserId) ?? targetUserId;

                    SalesmanTarget salesman = await ResolveSalesmanAsync(finalUserId);
                    if (salesman != null)
                    {
                        // Sprawdź, czy SalesmanTarget już istnieje w słowniku, używając Equals
                        var existingKey = result.Keys.FirstOrDefault(k => k.Equals(salesman));

                        if (existingKey == null)
                        {
                            result[salesman] = new List<PendingReturnItem>();
                            result[salesman].Add(item);
                        }
                        else
                        {
                            result[existingKey].Add(item);
                        }
                    }
                }
            }

            return result;
        }

        private async Task<string> ResolveUwagiMagazynuColumnAsync()
        {
            const string query = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'AllegroCustomerReturns'
                  AND COLUMN_NAME IN ('UwagiMagazynu', 'UwagiMagazyn')
                LIMIT 1";
            var result = await _dbServiceMagazyn.ExecuteScalarAsync(query);
            return result?.ToString() ?? "UwagiMagazynu";
        }

        private async Task<List<int>> GetRecipientsForManualReturnAsync(int returnId)
        {
            var recipients = new List<int>();
            using (var con = MagazynDatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand("SELECT OdbiorcaId FROM Wiadomosci WHERE DotyczyZwrotuId=@d", con))
                {
                    cmd.Parameters.AddWithValue("@d", returnId);
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        while (await r.ReadAsync())
                        {
                            if (!r.IsDBNull(0))
                            {
                                recipients.Add(r.GetInt32(0));
                            }
                        }
                    }
                }
            }
            return recipients;
        }

        private async Task<int> GetStatusIdAsync(string nazwa, string typ = null)
        {
            using (var con = MagazynDatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                string q = "SELECT Id FROM Statusy WHERE Nazwa=@n " + (string.IsNullOrWhiteSpace(typ) ? "" : "AND TypStatusu=@t ") + "LIMIT 1";
                using (var cmd = new MySqlCommand(q, con))
                {
                    cmd.Parameters.AddWithValue("@n", nazwa);
                    if (!string.IsNullOrWhiteSpace(typ)) cmd.Parameters.AddWithValue("@t", typ);
                    var o = await cmd.ExecuteScalarAsync();
                    return o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o);
                }
            }
        }

        private async Task<string> GetStatusNameByIdAsync(int? id)
        {
            if (!id.HasValue) return "";
            using (var con = MagazynDatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand("SELECT Nazwa FROM Statusy WHERE Id=@id", con))
                {
                    cmd.Parameters.AddWithValue("@id", id.Value);
                    var o = await cmd.ExecuteScalarAsync();
                    return o == null || o == DBNull.Value ? "" : Convert.ToString(o);
                }
            }
        }

        private async Task<int?> GetOpiekunIdForAccountAsync(int allegroAccountId)
        {
            using (var con = MagazynDatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand("SELECT OpiekunId FROM AllegroAccountOpiekun WHERE AllegroAccountId=@a LIMIT 1", con))
                {
                    cmd.Parameters.AddWithValue("@a", allegroAccountId);
                    var o = await cmd.ExecuteScalarAsync();
                    if (o == null || o == DBNull.Value) return null;
                    return Convert.ToInt32(o);
                }
            }
        }

        private async Task<int?> GetActiveDelegateIdAsync(int userId)
        {
            using (var con = MagazynDatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(@"
SELECT ZastepcaId
FROM Delegacje
WHERE UzytkownikId=@u
  AND CURDATE() BETWEEN DataOd AND DataDo
  AND IFNULL(CzyAktywna,1)=1
LIMIT 1;", con))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    var o = await cmd.ExecuteScalarAsync();
                    if (o == null || o == DBNull.Value) return null;
                    return Convert.ToInt32(o);
                }
            }
        }

        private async Task<SalesmanTarget> ResolveSalesmanAsync(int userId)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(@"SELECT `Nazwa Wyświetlana`, Email FROM Uzytkownicy WHERE Id=@id LIMIT 1", con))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (await r.ReadAsync())
                        {
                            var disp = Convert.ToString(r[0]);
                            var mail = Convert.ToString(r[1]);
                            if (string.IsNullOrWhiteSpace(mail)) return null;
                            var s = new SalesmanTarget
                            {
                                UserId = userId,
                                DisplayName = string.IsNullOrWhiteSpace(disp) ? "(handlowiec)" : disp,
                                Email = mail
                            };
                            return s;
                        }
                    }
                }
            }
            return null;
        }

        // ====== KROK 3: WYSYŁKA OUTLOOK ======
        private OutlookInterop.Account FindOutlookAccount(OutlookInterop.Application app, string smtp)
        {
            try
            {
                var accounts = app.Session.Accounts;
                foreach (OutlookInterop.Account acc in accounts)
                {
                    try
                    {
                        string a = SafeGetSmtpAddress(acc);
                        if (!string.IsNullOrWhiteSpace(a) && EmailsEqual(a, smtp))
                            return acc;
                    }
                    finally
                    {
                        if (acc != null) Marshal.FinalReleaseComObject(acc);
                    }
                }
            }
            catch { }
            return null;
        }

        private Task SendOutlookMailAsync(OutlookInterop.Application app, OutlookInterop.Account account, string to, string subject, string htmlBody)
        {
            return Task.Run(() =>
            {
                OutlookInterop.MailItem mail = null;
                try
                {
                    mail = app.CreateItem(OutlookInterop.OlItemType.olMailItem) as OutlookInterop.MailItem;
                    mail.To = to;
                    mail.Subject = subject;
                    mail.HTMLBody = htmlBody;
                    try
                    {
                        mail.SendUsingAccount = account;
                    }
                    catch { /* bywa niewspierane dla niektórych kont */ }

                    mail.Send();
                }
                finally
                {
                    if (mail != null)
                    {
                        try { Marshal.FinalReleaseComObject(mail); } catch { }
                    }
                }
            });
        }

        // ====== HTML BUILDER ======
        private static void AppendTd(StringBuilder b, string v)
        {
            b.Append("<td>")
             .Append(System.Net.WebUtility.HtmlEncode(v ?? ""))
             .Append("</td>");
        }

        private async Task<string> BuildHtmlForSalesmanAsync(string displayName, List<PendingReturnItem> items)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body style='font-family:Segoe UI,Arial,sans-serif;font-size:12px'>");
            sb.Append("<h3>Zwroty oczekujące na decyzję</h3>");
            sb.Append("<p>Witaj ").Append(System.Net.WebUtility.HtmlEncode(displayName)).Append(",</p>");
            sb.Append("<p>Poniżej lista zwrotów, które oczekują na Twoją decyzję w programie:</p>");

            sb.Append("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse'>");
            sb.Append("<thead style='background:#f0f0f0'><tr>");
            sb.Append("<th>Nr zwrotu</th><th>Klient</th><th>Produkt</th><th>Stan</th><th>Uwagi magazynu</th><th>Przyjęty przez</th><th>Data przyjęcia</th>");
            sb.Append("</tr></thead><tbody>");

            foreach (var it in items)
            {
                string przyjalTxt = "";
                if (it.PrzyjetyPrzezId.HasValue)
                {
                    var (uName, _) = await GetUserDisplayAndEmailAsync(it.PrzyjetyPrzezId.Value);
                    przyjalTxt = uName;
                }

                sb.Append("<tr>");
                AppendTd(sb, it.Reference);
                AppendTd(sb, it.Buyer);
                AppendTd(sb, it.Product);
                AppendTd(sb, it.StanProduktu);
                AppendTd(sb, it.UwagiMagazynu);
                AppendTd(sb, przyjalTxt);
                AppendTd(sb, it.DataPrzyjecia.HasValue ? it.DataPrzyjecia.Value.ToString("dd.MM.yyyy HH:mm") : "");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
            sb.Append("<p>Daj znać co z tym robimy :) Dziękujemy!</p>");
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private async Task<Tuple<string, string>> GetUserDisplayAndEmailAsync(int userId)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(@"SELECT `Nazwa Wyświetlana`, Email FROM Uzytkownicy WHERE Id=@id LIMIT 1", con))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (await r.ReadAsync())
                        {
                            return Tuple.Create(Convert.ToString(r[0]), Convert.ToString(r[1]));
                        }
                    }
                }
            }
            return Tuple.Create("", "");
        }

        // ====== PROSTA FORMA PROGRESSU ======
        private class SimpleProgressForm : Form
        {
            private ProgressBar _bar;
            private Label _label;

            public SimpleProgressForm(string title)
            {
                Text = title;
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MinimizeBox = false; MaximizeBox = false;
                Width = 520; Height = 140;

                _label = new Label
                {
                    AutoSize = false,
                    Left = 12,
                    Top = 12,
                    Width = 480,
                    Height = 22,
                    Text = "Przygotowywanie…"
                };

                _bar = new ProgressBar
                {
                    Left = 12,
                    Top = 44,
                    Width = 480,
                    Height = 22,
                    Style = ProgressBarStyle.Continuous,
                    Minimum = 0,
                    Maximum = 100,
                    Value = 0
                };

                Controls.Add(_label);
                Controls.Add(_bar);
            }

            public void SetRange(int min, int max)
            {
                if (InvokeRequired) { BeginInvoke((Action)(() => SetRange(min, max))); return; }
                _bar.Minimum = min;
                _bar.Maximum = max <= min ? min + 1 : max;
                _bar.Value = min;
            }

            public void SetValue(int value)
            {
                if (InvokeRequired) { BeginInvoke((Action)(() => SetValue(value))); return; }
                if (value < _bar.Minimum) value = _bar.Minimum;
                if (value > _bar.Maximum) value = _bar.Maximum;
                _bar.Value = value;
            }

            public void SetStatus(string text)
            {
                if (InvokeRequired) { BeginInvoke((Action)(() => SetStatus(text))); return; }
                _label.Text = text ?? "";
            }
        }
    }
}
