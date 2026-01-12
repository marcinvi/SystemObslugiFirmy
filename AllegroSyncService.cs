using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Reklamacje_Dane
{
    public class AllegroSyncResult
    {
        public int NewDisputesFound { get; set; }
        public int UnregisteredDisputesCount { get; set; }
        public int DisputesWithNewMessages { get; set; }
    }

    public class AllegroSyncService
    {
        private class AllegroAccountDetails
        {
            public int Id { get; set; }
            public string ClientId { get; set; }
            public string ClientSecretEncrypted { get; set; }
            public Allegro.AllegroToken AccessTokenEncrypted { get; set; }
        }

        // Metoda pomocnicza do logowania, aby kod był czytelniejszy
        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[AllegroSync] {DateTime.Now:HH:mm:ss} > {message}");
        }

        public async Task<AllegroSyncResult> SynchronizeDisputesAsync(IProgress<string> progress)
        {
            Log("=== ROZPOCZĘCIE PEŁNEJ SYNCHRONIZACJI ===");

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                Log("Połączenie z bazą danych otwarte.");

                var accounts = await GetAuthorizedAccountsAsync(con);
                if (!accounts.Any())
                {
                    Log("Błąd: Nie znaleziono żadnych autoryzowanych kont Allegro.");
                    return new AllegroSyncResult();
                }

                // Pobieramy listę ID, które już mamy w bazie
                Log("Pobieranie listy istniejących ID zgłoszeń z bazy...");
                var existingDisputeIds = await GetExistingDisputeIdsAsync(con);
                Log($"Załadowano {existingDisputeIds.Count} istniejących zgłoszeń z lokalnej bazy.");

                int newDisputesCounter = 0;

                foreach (var account in accounts)
                {
                    Log($"--- PRZETWARZANIE KONTA ID: {account.Id} ---");
                    progress?.Report($"Konto {account.Id}: Pobieranie listy zgłoszeń...");

                    try
                    {
                        var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(account.Id, con);
                        if (apiClient == null)
                        {
                            Log($"Pominięto konto {account.Id} - brak klienta API.");
                            continue;
                        }

                        // 1. Pobieramy listę wszystkich dyskusji/reklamacji
                        Log($"Wysyłanie zapytania GET /sale/issues dla konta {account.Id}...");
                        var allIssuesFromApi = await apiClient.GetIssuesAsync();

                        if (allIssuesFromApi == null || !allIssuesFromApi.Any())
                        {
                            Log($"API zwróciło 0 zgłoszeń dla konta {account.Id}.");
                            continue;
                        }

                        Log($"SUKCES API: Pobrano {allIssuesFromApi.Count} zgłoszeń. Rozpoczynam pętlę przetwarzania...");

                        int current = 0;
                        foreach (var issue in allIssuesFromApi)
                        {
                            current++;
                            try
                            {
                                bool isNew = !existingDisputeIds.Contains(issue.Id);
                                string status = issue.CurrentState?.Status ?? "BRAK";

                                Log($"Analiza {current}/{allIssuesFromApi.Count}: ID={issue.Id}, Status={status}, Typ={issue.Type}");

                                if (isNew)
                                {
                                    // === SCENARIUSZ A: NOWE ZGŁOSZENIE ===
                                    newDisputesCounter++;
                                    Log($"-> DECYZJA: TO NOWE ZGŁOSZENIE! (Brak w bazie)");
                                    progress?.Report($"Konto {account.Id}: Pobieranie danych dla NOWEJ dyskusji {current}/{allIssuesFromApi.Count}...");

                                    string checkoutFormId = issue.CheckoutForm?.Id;
                                    Log($"-> Pobieranie szczegółów zamówienia (CheckoutFormId: {checkoutFormId ?? "BRAK"})...");

                                    var orderDetails = await apiClient.GetOrderDetailsByCheckoutFormIdAsync(checkoutFormId);

                                    if (orderDetails != null) Log("-> Pobrano dane zamówienia (adresy, kupujący).");
                                    else Log("-> OSTRZEŻENIE: Brak danych zamówienia (null).");

                                    // Pełny zapis (INSERT)
                                    await InsertNewIssueAsync(issue, orderDetails, account.Id, con);
                                }
                                else
                                {
                                    // === SCENARIUSZ B: ISTNIEJĄCE ZGŁOSZENIE ===
                                    Log($"-> DECYZJA: ZGŁOSZENIE ISTNIEJE. Wykonuję szybki UPDATE statusu.");
                                    await UpdateExistingIssueStatusAsync(issue, con);
                                }

                                // === WSPÓLNE: SYNCHRONIZACJA CZATU ===
                                await SynchronizeChatForIssueAsync(apiClient, issue, con);
                            }
                            catch (Exception exIssue)
                            {
                                Log($"[BŁĄD POJEDYNCZEGO ZGŁOSZENIA] ID {issue.Id}: {exIssue.Message}\nStack: {exIssue.StackTrace}");
                            }
                        }
                    }
                    catch (Exception exAccount)
                    {
                        Log($"[BŁĄD KRYTYCZNY KONTA] ID {account.Id}: {exAccount.Message}");
                    }
                }

                Log("Pobieranie statystyk końcowych...");
                var result = new AllegroSyncResult
                {
                    NewDisputesFound = newDisputesCounter,
                    UnregisteredDisputesCount = await GetUnregisteredDisputesCountAsync(con),
                    DisputesWithNewMessages = await GetUnreadMessagesCountAsync(con)
                };

                Log($"=== KONIEC SYNCHRONIZACJI ===");
                Log($"Wynik: Znaleziono nowych: {result.NewDisputesFound}, Nieprzypisanych w bazie: {result.UnregisteredDisputesCount}, Z nowymi wiadomościami: {result.DisputesWithNewMessages}");

                return result;
            }
        }

        private async Task UpdateExistingIssueStatusAsync(Issue issue, MySqlConnection con)
        {
            Log($"[DB UPDATE] Aktualizacja statusu dla DisputeId: {issue.Id}");

            string sql = @"
                UPDATE AllegroDisputes SET 
                    StatusAllegro = @StatusAllegro,
                    LastCheckedAt = NOW(),
                    ClosedAt = @ClosedAt,
                    ReasonType = @ReasonType,
                    ReasonDescription = @ReasonDescription,
                    ExpectationType = @ExpectationType,
                    ExpectationRefundAmount = @ExpectationRefundAmount,
                    ExpectationRefundCurrency = @ExpectationRefundCurrency,
                    DecisionDueDate = @DecisionDueDate
                WHERE DisputeId = @DisputeId";

            using (var cmd = new MySqlCommand(sql, con))
            {
                var firstExpectation = issue.Expectations?.FirstOrDefault();

                cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                cmd.Parameters.AddWithValue("@StatusAllegro", issue.CurrentState?.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ClosedAt", issue.ClosedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReasonType", issue.Reason?.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReasonDescription", issue.Reason?.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpectationType", firstExpectation?.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpectationRefundAmount", firstExpectation?.Refund?.Amount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpectationRefundCurrency", firstExpectation?.Refund?.Currency ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DecisionDueDate", issue.DecisionDueDate.HasValue ? issue.DecisionDueDate.Value : (object)DBNull.Value);

                int rows = await cmd.ExecuteNonQueryAsync();
                Log($"[DB UPDATE] Zaktualizowano wierszy: {rows}");
            }
        }

        private async Task InsertNewIssueAsync(Issue issue, OrderDetails order, int accountId, MySqlConnection con)
        {
            Log($"[DB INSERT] Rozpoczynam pełny zapis nowego zgłoszenia {issue.Id}");

            string productName = "Nie udało się ustalić produktu";
            if (order?.LineItems != null && order.LineItems.Any())
            {
                if (issue.Type == "CLAIM" && issue.Offer?.Id != null)
                {
                    var specificLineItem = order.LineItems.FirstOrDefault(li => li.Offer?.Id == issue.Offer.Id);
                    productName = specificLineItem?.Offer?.Name ?? order.LineItems.First().Offer.Name;
                }
                else
                {
                    productName = order.LineItems.First().Offer.Name;
                }
            }
            Log($"[DB INFO] Ustalona nazwa produktu: {productName}");

            DateTime? finalBoughtAt = null;
            string dateSource = "BRAK";

            if (order != null && order.BoughtAt > DateTime.MinValue)
            {
                finalBoughtAt = order.BoughtAt;
                dateSource = "Order.BoughtAt";
            }
            else if (issue.CheckoutForm != null)
            {
                finalBoughtAt = issue.CheckoutForm.CreatedAt;
                dateSource = "CheckoutForm.CreatedAt (Fallback)";
            }
            Log($"[DB INFO] Data zakupu: {finalBoughtAt} (Źródło: {dateSource})");

            string sql = @"
                INSERT IGNORE INTO AllegroDisputes 
                (DisputeId, OrderId, AllegroAccountId, Type, Subject, StatusAllegro, OpenedAt, 
                 BuyerLogin, BuyerFirstName, BuyerLastName, BuyerEmail, 
                 DeliveryCompanyName, DeliveryStreet, DeliveryZipCode, DeliveryCity, DeliveryPhoneNumber, 
                 ReasonType, ReasonDescription, Expectations, InitialMessageText, InitialMessageCount, 
                 LastCheckedAt, ProductName, BoughtAt, ComplaintId, LastMessageCount, HasNewMessages, 
                 referenceNumber, decisionDueDate, expectationType, expectationRefundAmount, expectationRefundCurrency) 
                VALUES 
                (@DisputeId, @OrderId, @AllegroAccountId, @Type, @Subject, @StatusAllegro, @OpenedAt, 
                 @BuyerLogin, @BuyerFirstName, @BuyerLastName, @BuyerEmail, 
                 @DeliveryCompanyName, @DeliveryStreet, @DeliveryZipCode, @DeliveryCity, @DeliveryPhoneNumber, 
                 @ReasonType, @ReasonDescription, @Expectations, @InitialMessageText, @InitialMessageCount, 
                 NOW(), @ProductName, @BoughtAt, NULL, 0, 0, 
                 @referenceNumber, @decisionDueDate, @expectationType, @expectationRefundAmount, @expectationRefundCurrency)";

            using (var command = new MySqlCommand(sql, con))
            {
                var firstExpectation = issue.Expectations?.FirstOrDefault();

                command.Parameters.AddWithValue("@DisputeId", issue.Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderId", issue.CheckoutForm?.Id ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AllegroAccountId", accountId);
                command.Parameters.AddWithValue("@Type", issue.Type ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Subject", issue.Subject ?? issue.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@StatusAllegro", issue.CurrentState?.Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OpenedAt", issue.OpenedDate);

                command.Parameters.AddWithValue("@BuyerLogin", issue.Buyer?.Login ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BuyerFirstName", order?.Buyer?.FirstName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BuyerLastName", order?.Buyer?.LastName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BuyerEmail", order?.Buyer?.Email ?? (object)DBNull.Value);

                var addr = order?.Delivery?.Address;
                command.Parameters.AddWithValue("@DeliveryCompanyName", addr?.CompanyName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DeliveryStreet", addr?.Street ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DeliveryZipCode", addr?.ZipCode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DeliveryCity", addr?.City ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DeliveryPhoneNumber", addr?.PhoneNumber ?? (object)DBNull.Value);

                command.Parameters.AddWithValue("@ReasonType", issue.Reason?.Type ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ReasonDescription", issue.Reason?.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Expectations", firstExpectation?.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@InitialMessageText", issue.Chat?.InitialMessage?.Text ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@InitialMessageCount", issue.Chat?.MessagesCount ?? 0);

                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@BoughtAt", finalBoughtAt ?? (object)DBNull.Value);

                command.Parameters.AddWithValue("@referenceNumber", issue.ReferenceNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@decisionDueDate", issue.DecisionDueDate.HasValue ? issue.DecisionDueDate.Value : (object)DBNull.Value);
                command.Parameters.AddWithValue("@expectationType", firstExpectation?.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@expectationRefundAmount", firstExpectation?.Refund?.Amount ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@expectationRefundCurrency", firstExpectation?.Refund?.Currency ?? (object)DBNull.Value);

                int rows = await command.ExecuteNonQueryAsync();
                Log($"[DB INSERT] Zakończono sukcesem. Wstawiono wierszy: {rows}");
            }
        }

        private async Task SynchronizeChatForIssueAsync(AllegroApiClient apiClient, Issue issue, MySqlConnection con)
        {
            int localMessageCount = 0;

            // Sprawdź ile mamy wiadomości lokalnie
            using (var cmd = new MySqlCommand("SELECT LastMessageCount FROM AllegroDisputes WHERE DisputeId = @DisputeId", con))
            {
                cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    localMessageCount = Convert.ToInt32(result);
                }
            }

            // Pobierz wiadomości z API
            // Najpierw sprawdzamy count z obiektu Issue (jeśli dostępny), żeby nie strzelać do API niepotrzebnie
            // Ale dla pewności pobieramy listę, bo Allegro nie zawsze zwraca poprawny count w liście issues
            Log($"[CHAT CHECK] ID {issue.Id}: Lokalnie mamy {localMessageCount} wiadomości.");

            var messagesFromApi = await apiClient.GetChatAsync(issue.Id);
            int apiMessageCount = messagesFromApi?.Count ?? 0;

            Log($"[CHAT CHECK] ID {issue.Id}: API zwróciło {apiMessageCount} wiadomości.");

            if (apiMessageCount > localMessageCount)
            {
                Log($"[CHAT SYNC] Wykryto nowe wiadomości ({apiMessageCount} > {localMessageCount}). Rozpoczynam zapis...");

                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        int insertedCount = 0;
                        foreach (var msg in messagesFromApi)
                        {
                            var insertCmd = new MySqlCommand(@"
                                INSERT IGNORE INTO AllegroChatMessages 
                                (MessageId, DisputeId, AuthorLogin, AuthorRole, MessageText, CreatedAt, HasAttachments, JsonDetails)
                                VALUES 
                                (@MessageId, @DisputeId, @AuthorLogin, @AuthorRole, @MessageText, @CreatedAt, @HasAttachments, @JsonDetails)",
                                con, transaction);

                            insertCmd.Parameters.AddWithValue("@MessageId", msg.Id);
                            insertCmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                            insertCmd.Parameters.AddWithValue("@AuthorLogin", msg.Author?.Login ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@AuthorRole", msg.Author?.Role ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@MessageText", msg.Text ?? (object)DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CreatedAt", msg.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                            insertCmd.Parameters.AddWithValue("@HasAttachments", (msg.Attachments != null && msg.Attachments.Any()) ? 1 : 0);
                            insertCmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(msg));

                            int rows = await insertCmd.ExecuteNonQueryAsync();
                            if (rows > 0) insertedCount++;
                        }

                        Log($"[CHAT SYNC] Zapisano {insertedCount} nowych wiadomości w tabeli AllegroChatMessages.");

                        var updateCmd = new MySqlCommand("UPDATE AllegroDisputes SET LastMessageCount = @Count, HasNewMessages = 1 WHERE DisputeId = @DisputeId", con, transaction);
                        updateCmd.Parameters.AddWithValue("@Count", apiMessageCount);
                        updateCmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                        await updateCmd.ExecuteNonQueryAsync();
                        Log($"[CHAT SYNC] Zaktualizowano licznik i flagę HasNewMessages w AllegroDisputes.");

                        transaction.Commit();
                        Log($"[CHAT SYNC] Transakcja zakończona sukcesem.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log($"[CHAT ERROR] Błąd podczas zapisu czatu! Rollback. Błąd: {ex.Message}");
                        throw;
                    }
                }
            }
            else
            {
                Log($"[CHAT SKIP] Liczba wiadomości zgodna. Pomijam zapis.");
            }
        }

        private async Task<List<AllegroAccountDetails>> GetAuthorizedAccountsAsync(MySqlConnection con)
        {
            var accounts = new List<AllegroAccountDetails>();
            using (var cmd = new MySqlCommand("SELECT Id, ClientId, ClientSecretEncrypted, AccessTokenEncrypted, RefreshTokenEncrypted, TokenExpirationDate FROM AllegroAccounts WHERE IsAuthorized = 1", con))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader["AccessTokenEncrypted"] != DBNull.Value && !string.IsNullOrEmpty(reader["AccessTokenEncrypted"].ToString()))
                        {
                            accounts.Add(new AllegroAccountDetails
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ClientId = reader["ClientId"].ToString(),
                                ClientSecretEncrypted = EncryptionHelper.DecryptString(reader["ClientSecretEncrypted"].ToString()),
                                AccessTokenEncrypted = new Allegro.AllegroToken
                                {
                                    AccessToken = EncryptionHelper.DecryptString(reader["AccessTokenEncrypted"].ToString()),
                                    RefreshToken = EncryptionHelper.DecryptString(reader["RefreshTokenEncrypted"].ToString()),
                                    ExpirationDate = DateTime.Parse(reader["TokenExpirationDate"].ToString())
                                }
                            });
                        }
                    }
                }
            }
            return accounts;
        }

        private async Task<HashSet<string>> GetExistingDisputeIdsAsync(MySqlConnection con)
        {
            var ids = new HashSet<string>();
            using (var cmd = new MySqlCommand("SELECT DisputeId FROM AllegroDisputes", con))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (!reader.IsDBNull(0)) ids.Add(reader.GetString(0));
                    }
                }
            }
            return ids;
        }

        private async Task<int> GetUnregisteredDisputesCountAsync(MySqlConnection con)
        {
            using (var cmd = new MySqlCommand("SELECT COUNT(Id) FROM AllegroDisputes WHERE ComplaintId IS NULL", con))
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }

        private async Task<int> GetUnreadMessagesCountAsync(MySqlConnection con)
        {
            using (var cmd = new MySqlCommand("SELECT COUNT(Id) FROM AllegroDisputes WHERE HasNewMessages = 1", con))
            {
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }
    }
}