using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
using Reklamacje_Dane.Allegro.Returns;
using System;
using System.Collections.Generic;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Reklamacje_Dane
{
    /// <summary>
    /// Rozszerzony serwis synchronizacji Allegro - WERSJA 3.2 BUYER FIX
    /// ════════════════════════════════════════════════════════════════
    /// 
    /// NAPRAWY W WERSJI 3.2:
    /// ✅ NAPRAWIONO: Pobieranie danych kupującego z orderDetails.Buyer zamiast issue.Buyer (które jest NULL)
    /// ✅ USUNIĘTO: Niepotrzebne wywołanie GetBuyerEmailAsync()
    /// 
    /// NAPRAWY W WERSJI 3.1:
    /// ✅ NAPRAWIONO: Błędną logikę porównania issues (teraz sprawdza count)
    /// ✅ NAPRAWIONO: Błąd MySQL przy zapisie LastMessageId
    /// ✅ DODANO: Weryfikację istnienia kolumny LastMessageId
    /// ✅ POPRAWIONO: Synchronizacja ZAWSZE pobiera wszystkie issues i zapisuje do bazy
    /// 
    /// ZMIANY W WERSJI 3.0:
    /// ✅ QUICK CHECK - sprawdza totalCount przed pełną synchronizacją
    /// ✅ INTELIGENTNA SYNCHRONIZACJA CZATU - sprawdza LastMessageId
    /// ✅ POMIJA synchronizację gdy brak zmian
    /// </summary>
    public class AllegroSyncServiceExtended
    {
        private class AllegroAccountDetails
        {
            public int Id { get; set; }
            public string ClientId { get; set; }
            public string ClientSecretEncrypted { get; set; }
        }

        #region Helper Methods - Bezpieczne parsowanie

        private decimal? SafeParseDecimal(string value, string returnId = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                value = value.Replace(" ", "").Replace(",", "");

                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }

                if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("pl-PL"), out decimal resultPL))
                {
                    return resultPL;
                }

                System.Diagnostics.Debug.WriteLine(
                    $"OSTRZEŻENIE: Nie można sparsować kwoty '{value}'" +
                    (returnId != null ? $" dla zwrotu/issue {returnId}" : ""));

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"BŁĄD parsowania kwoty '{value}'" +
                    (returnId != null ? $" dla zwrotu/issue {returnId}" : "") +
                    $": {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// ⭐ NOWA METODA: Sprawdza czy kolumna LastMessageId istnieje w tabeli
        /// </summary>
        private async Task<bool> CheckLastMessageIdColumnExists(MySqlConnection con)
        {
            try
            {
                var cmd = new MySqlCommand(@"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = 'reklamacjedb' 
                      AND TABLE_NAME = 'allegrodisputes' 
                      AND COLUMN_NAME = 'LastMessageId'", con);
                
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt64(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Publiczne metody synchronizacji

        public async Task<AllegroCompleteSyncResult> SynchronizeAllAsync(IProgress<string> progress)
        {
            var result = new AllegroCompleteSyncResult();

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();

                var accounts = await GetAuthorizedAccountsAsync(con);
                if (!accounts.Any())
                {
                    System.Diagnostics.Debug.WriteLine("Nie znaleziono żadnych autoryzowanych kont Allegro.");
                    return result;
                }

                foreach (var account in accounts)
                {
                    progress?.Report($"Synchronizacja konta ID: {account.Id}");

                    try
                    {
                        var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(account.Id, con);
                        if (apiClient == null) continue;

                        progress?.Report("Pobieranie zwrotów...");
                        var returnsResult = await SynchronizeReturnsForAccountAsync(apiClient, account.Id, con);
                        result.TotalReturnsProcessed += returnsResult.TotalProcessed;
                        result.NewReturnsFound += returnsResult.NewReturns;
                        result.UpdatedReturns += returnsResult.UpdatedReturns;

                        progress?.Report("Pobieranie dyskusji i reklamacji...");
                        var issuesResult = await SynchronizeIssuesForAccountAsync_Fixed(apiClient, account.Id, con, progress);
                        result.TotalIssuesProcessed += issuesResult.TotalProcessed;
                        result.NewIssuesFound += issuesResult.NewIssues;
                        result.IssuesWithNewMessages += issuesResult.IssuesWithNewMessages;

                        progress?.Report($"Konto {account.Id} - OK");
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessages.Add($"Konto {account.Id}: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Błąd synchronizacji konta {account.Id}: {ex.Message}");
                    }
                }

                result.CompletedAt = DateTime.Now;
            }

            return result;
        }

        public async Task<ReturnsSyncResult> SynchronizeReturnsAsync(IProgress<string> progress)
        {
            var result = new ReturnsSyncResult();

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();

                var accounts = await GetAuthorizedAccountsAsync(con);
                int accountIndex = 0;
                int totalAccounts = accounts.Count;

                progress?.Report($"Znaleziono {totalAccounts} kont Allegro");

                foreach (var account in accounts)
                {
                    accountIndex++;
                    try
                    {
                        progress?.Report($"Konto {accountIndex} z {totalAccounts} (ID: {account.Id}) - łączenie...");

                        var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(account.Id, con);
                        if (apiClient == null)
                        {
                            progress?.Report($"Konto {accountIndex} z {totalAccounts} (ID: {account.Id}) - brak API client, pomijam");
                            continue;
                        }

                        progress?.Report($"Konto {accountIndex} z {totalAccounts} (ID: {account.Id}) - pobieranie zwrotów...");

                        var accountResult = await SynchronizeReturnsForAccountAsync(apiClient, account.Id, con, progress, accountIndex, totalAccounts);
                        result.TotalProcessed += accountResult.TotalProcessed;
                        result.NewReturns += accountResult.NewReturns;
                        result.UpdatedReturns += accountResult.UpdatedReturns;

                        progress?.Report($"Konto {accountIndex} z {totalAccounts} (ID: {account.Id}) - OK (nowych: {accountResult.NewReturns}, zaktualizowanych: {accountResult.UpdatedReturns})");
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessages.Add($"Konto {account.Id}: {ex.Message}");
                        progress?.Report($"Konto {accountIndex} z {totalAccounts} (ID: {account.Id}) - BŁĄD: {ex.Message}");
                    }
                }

                progress?.Report($"Zakończono: {result.TotalProcessed} zwrotów, {result.NewReturns} nowych, {result.UpdatedReturns} zaktualizowanych");
            }

            return result;
        }

        /// <summary>
        /// ⭐ NAPRAWIONA synchronizacja issues - WERSJA 3.1
        /// </summary>
        public async Task<IssuesSyncResult> SynchronizeIssuesAsync(IProgress<string> progress)
        {
            var result = new IssuesSyncResult();

            System.Diagnostics.Debug.WriteLine("✅ SynchronizeIssuesAsync - START (v3.2 BUYER FIX)");

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();

                var accounts = await GetAuthorizedAccountsAsync(con);

                System.Diagnostics.Debug.WriteLine($"✅ Znaleziono kont Allegro: {accounts.Count}");

                if (!accounts.Any())
                {
                    System.Diagnostics.Debug.WriteLine("❌ Nie znaleziono żadnych autoryzowanych kont Allegro.");
                    return result;
                }

                foreach (var account in accounts)
                {
                    System.Diagnostics.Debug.WriteLine($"\n--- Konto ID: {account.Id} ---");

                    try
                    {
                        var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(account.Id, con);

                        if (apiClient == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"❌ Konto {account.Id}: apiClient = null");
                            result.ErrorMessages.Add($"Konto {account.Id}: Nie można utworzyć API client");
                            continue;
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ Konto {account.Id}: API Client utworzony");

                        var accountResult = await SynchronizeIssuesForAccountAsync_Fixed(
                            apiClient, account.Id, con, progress);

                        System.Diagnostics.Debug.WriteLine($"✅ Konto {account.Id}: Przetworzonych={accountResult.TotalProcessed}, Nowych={accountResult.NewIssues}, NowychWiadomości={accountResult.IssuesWithNewMessages}");

                        result.TotalProcessed += accountResult.TotalProcessed;
                        result.NewIssues += accountResult.NewIssues;
                        result.IssuesWithNewMessages += accountResult.IssuesWithNewMessages;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Konto {account.Id}: EXCEPTION - {ex.Message}");
                        result.ErrorMessages.Add($"Konto {account.Id}: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"\n✅ SynchronizeIssuesAsync - KONIEC: Przetworzonych={result.TotalProcessed}, Nowych={result.NewIssues}, NowychWiadomości={result.IssuesWithNewMessages}");
            }

            return result;
        }

        #endregion

        #region Synchronizacja zwrotów

        private async Task<ReturnsSyncResult> SynchronizeReturnsForAccountAsync(
            AllegroApiClient apiClient,
            int accountId,
            MySqlConnection con,
            IProgress<string> progress = null,
            int accountIndex = 0,
            int totalAccounts = 0)
        {
            var result = new ReturnsSyncResult();
            var logId = await LogSyncStartAsync(accountId, "RETURNS", con);

            try
            {
                var allReturns = await GetAllReturnsFromApiAsync(apiClient);
                result.TotalProcessed = allReturns.Count;

                string accountLabel = totalAccounts > 0
                    ? $"[Konto {accountIndex}/{totalAccounts}]"
                    : $"[Konto {accountId}]";

                progress?.Report($"{accountLabel} Pobrano {allReturns.Count} zwrotów z API");

                int returnIndex = 0;
                foreach (var returnData in allReturns)
                {
                    returnIndex++;
                    try
                    {
                        // Raportuj co 5 zwrotów (lub zawsze jeśli < 20)
                        if (allReturns.Count < 20 || returnIndex % 5 == 0 || returnIndex == allReturns.Count)
                        {
                            progress?.Report($"{accountLabel} Zwrot {returnIndex} z {allReturns.Count}: {returnData.Id}");
                        }

                        OrderDetails orderDetails = null;
                        if (!string.IsNullOrEmpty(returnData.OrderId))
                        {
                            orderDetails = await apiClient.GetOrderDetailsByCheckoutFormIdAsync(returnData.OrderId);
                        }

                        bool isNew = await UpsertReturnAsync(returnData, orderDetails, accountId, con);

                        if (isNew)
                        {
                            result.NewReturns++;
                        }
                        else
                        {
                            result.UpdatedReturns++;
                        }

                        if (returnData.Items != null && returnData.Items.Count > 1)
                        {
                            await SaveReturnItemsAsync(returnData, con);
                        }
                    }
                    catch (Exception exReturn)
                    {
                        result.ErrorMessages.Add($"Zwrot {returnData.Id}: {exReturn.Message}");
                        progress?.Report($"{accountLabel} BŁĄD zwrot {returnData.Id}: {exReturn.Message}");
                        System.Diagnostics.Debug.WriteLine($"Błąd przetwarzania zwrotu {returnData.Id}: {exReturn.Message}");
                    }
                }

                await LogSyncCompleteAsync(logId, "SUCCESS", result.TotalProcessed, result.NewReturns, result.UpdatedReturns, con);
            }
            catch (Exception ex)
            {
                await LogSyncCompleteAsync(logId, "FAILED", result.TotalProcessed, result.NewReturns, result.UpdatedReturns, con, ex.Message);
                throw;
            }

            return result;
        }

        private async Task<List<AllegroCustomerReturn>> GetAllReturnsFromApiAsync(AllegroApiClient apiClient)
        {
            var allReturns = new List<AllegroCustomerReturn>();
            int offset = 0;
            int limit = 1000;

            while (true)
            {
                var response = await apiClient.GetCustomerReturnsAsync(limit, offset);

                if (response?.CustomerReturns != null && response.CustomerReturns.Any())
                {
                    allReturns.AddRange(response.CustomerReturns);

                    if (response.CustomerReturns.Count < limit)
                    {
                        break;
                    }

                    offset += limit;
                }
                else
                {
                    break;
                }
            }

            return allReturns;
        }

        private async Task<bool> UpsertReturnAsync(
            AllegroCustomerReturn returnData,
            OrderDetails orderDetails,
            int accountId,
            MySqlConnection con)
        {
            var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM AllegroCustomerReturns WHERE AllegroReturnId = @ReturnId", con);
            checkCmd.Parameters.AddWithValue("@ReturnId", returnData.Id);
            long count = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());
            bool isNew = (count == 0);

            var firstItem = returnData.Items?.FirstOrDefault();
            var firstParcel = returnData.Parcels?.FirstOrDefault();

            string sql = isNew ? GetInsertReturnSql() : GetUpdateReturnSql();

            using (var cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@AllegroReturnId", returnData.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AllegroAccountId", accountId);
                cmd.Parameters.AddWithValue("@ReferenceNumber", returnData.ReferenceNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OrderId", returnData.OrderId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerLogin", returnData.Buyer?.Login ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerEmail", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedAt", returnData.CreatedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StatusAllegro", returnData.Status ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@Waybill", firstParcel?.Waybill ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TransportingWaybill", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CarrierName", firstParcel?.CarrierId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TransportingCarrierId", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SenderPhoneNumber", (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@ProductName", firstItem?.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OfferId", firstItem?.OfferId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Quantity", firstItem?.Quantity ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductPrice", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductPriceCurrency", "PLN");

                cmd.Parameters.AddWithValue("@ReturnReasonType", firstItem?.Reason?.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReturnReasonComment", firstItem?.Reason?.UserComment ?? (object)DBNull.Value);

                if (orderDetails != null)
                {
                    cmd.Parameters.AddWithValue("@PaymentType", orderDetails.Payment?.Type ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentProvider", orderDetails.Payment?.Provider ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentFinishedAt", orderDetails.Payment?.FinishedAt ?? (object)DBNull.Value);

                    decimal? paidAmount = null;
                    if (orderDetails.Payment?.PaidAmount?.Amount != null)
                    {
                        paidAmount = SafeParseDecimal(orderDetails.Payment.PaidAmount.Amount, returnData.Id);
                    }
                    cmd.Parameters.AddWithValue("@PaidAmount", paidAmount ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@FulfillmentStatus", orderDetails.Fulfillment?.Status ?? (object)DBNull.Value);

                    var deliveryAddr = orderDetails.Delivery?.Address;
                    cmd.Parameters.AddWithValue("@Delivery_FirstName", deliveryAddr?.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_LastName", deliveryAddr?.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_Street", deliveryAddr?.Street ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_ZipCode", deliveryAddr?.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_City", deliveryAddr?.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_PhoneNumber", deliveryAddr?.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_CountryCode", deliveryAddr?.CountryCode ?? "PL");
                    cmd.Parameters.AddWithValue("@Delivery_CompanyName", deliveryAddr?.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeliveryMethod", orderDetails.Delivery?.Method?.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeliveryMethodId", orderDetails.Delivery?.Method?.Id ?? (object)DBNull.Value);

                    var buyerAddr = orderDetails.Buyer?.Address;
                    cmd.Parameters.AddWithValue("@Buyer_FirstName", orderDetails.Buyer?.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_LastName", orderDetails.Buyer?.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_Street", buyerAddr?.Street ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_ZipCode", buyerAddr?.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_City", buyerAddr?.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_PhoneNumber", orderDetails.Buyer?.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_CountryCode", buyerAddr?.CountryCode ?? "PL");
                    cmd.Parameters.AddWithValue("@Buyer_CompanyName", buyerAddr?.CompanyName ?? (object)DBNull.Value);

                    var invoiceAddr = orderDetails.Invoice?.Address?.Company?.Address;
                    var invoiceNatural = orderDetails.Invoice?.Address?.NaturalPerson?.Address;
                    var invoiceAddrFinal = invoiceAddr ?? invoiceNatural;

                    cmd.Parameters.AddWithValue("@Invoice_CompanyName",
                        orderDetails.Invoice?.Address?.Company?.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_TaxId",
                        orderDetails.Invoice?.Address?.Company?.TaxId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_Street", invoiceAddrFinal?.Street ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_ZipCode", invoiceAddrFinal?.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_City", invoiceAddrFinal?.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_CountryCode", invoiceAddrFinal?.CountryCode ?? "PL");
                    cmd.Parameters.AddWithValue("@InvoiceRequired", orderDetails.Invoice?.Required == true ? 1 : 0);

                    cmd.Parameters.AddWithValue("@MarketplaceId", orderDetails.Marketplace?.Id ?? "allegro-pl");
                    cmd.Parameters.AddWithValue("@BoughtAt", orderDetails.BoughtAt ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderJsonDetails", JsonConvert.SerializeObject(orderDetails));
                }
                else
                {
                    cmd.Parameters.AddWithValue("@PaymentType", DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentProvider", DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentFinishedAt", DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaidAmount", DBNull.Value);
                    cmd.Parameters.AddWithValue("@FulfillmentStatus", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_FirstName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_LastName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_Street", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_ZipCode", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_City", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_PhoneNumber", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Delivery_CountryCode", "PL");
                    cmd.Parameters.AddWithValue("@Delivery_CompanyName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeliveryMethod", DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeliveryMethodId", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_FirstName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_LastName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_Street", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_ZipCode", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_City", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_PhoneNumber", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_CountryCode", "PL");
                    cmd.Parameters.AddWithValue("@Buyer_CompanyName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_CompanyName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_TaxId", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_Street", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_ZipCode", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_City", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Invoice_CountryCode", "PL");
                    cmd.Parameters.AddWithValue("@InvoiceRequired", 0);
                    cmd.Parameters.AddWithValue("@MarketplaceId", "allegro-pl");
                    cmd.Parameters.AddWithValue("@BoughtAt", DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderJsonDetails", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(returnData));
                cmd.Parameters.AddWithValue("@LastSyncAt", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();
            }

            if (orderDetails != null)
            {
                try
                {
                    await UpsertOrderDetailsAsync(orderDetails, accountId, con);
                    await UpsertOrderItemsAsync(orderDetails, con);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[WARN] Nie udało się zapisać tabel AllegroOrderDetails/AllegroOrderItems dla {orderDetails.Id}: {ex.Message}");
                }
            }

            return isNew;
        }

        private async Task SaveReturnItemsAsync(AllegroCustomerReturn returnData, MySqlConnection con)
        {
            var getIdCmd = new MySqlCommand(
                "SELECT Id FROM AllegroCustomerReturns WHERE AllegroReturnId = @ReturnId LIMIT 1", con);
            getIdCmd.Parameters.AddWithValue("@ReturnId", returnData.Id);
            var returnId = Convert.ToInt32(await getIdCmd.ExecuteScalarAsync());

            var deleteCmd = new MySqlCommand(
                "DELETE FROM AllegroReturnItems WHERE ReturnId = @ReturnId", con);
            deleteCmd.Parameters.AddWithValue("@ReturnId", returnId);
            await deleteCmd.ExecuteNonQueryAsync();

            foreach (var item in returnData.Items)
            {
                var insertCmd = new MySqlCommand(@"
                    INSERT INTO AllegroReturnItems 
                    (ReturnId, OfferId, ProductName, Quantity, Price, Currency, ReasonType, ReasonComment, ProductUrl, JsonDetails)
                    VALUES 
                    (@ReturnId, @OfferId, @ProductName, @Quantity, @Price, @Currency, @ReasonType, @ReasonComment, @ProductUrl, @JsonDetails)",
                    con);

                insertCmd.Parameters.AddWithValue("@ReturnId", returnId);
                insertCmd.Parameters.AddWithValue("@OfferId", item.OfferId ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ProductName", item.Name ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Quantity", item.Quantity ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Price", DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Currency", "PLN");
                insertCmd.Parameters.AddWithValue("@ReasonType", item.Reason?.Type ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ReasonComment", item.Reason?.UserComment ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ProductUrl", DBNull.Value);
                insertCmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(item));

                await insertCmd.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region Synchronizacja dyskusji - FIXED v3.1

        /// <summary>
        /// ⭐ NAPRAWIONA WERSJA v3.1 - prosta i niezawodna logika
        /// </summary>
        private async Task<IssuesSyncResult> SynchronizeIssuesForAccountAsync_Fixed(
            AllegroApiClient apiClient,
            int accountId,
            MySqlConnection con,
            IProgress<string> progress = null)
        {
            var result = new IssuesSyncResult();
            var logId = await LogSyncStartAsync(accountId, "ISSUES", con);

            try
            {
                var lastSyncTime = await GetLastSuccessfulSyncTimeAsync(accountId, con);
                if (lastSyncTime != null)
                {
                    var timeSinceLastSync = DateTime.Now - lastSyncTime.Value;
                    if (timeSinceLastSync.TotalMinutes < 1)
                    {
                        progress?.Report($"Konto {accountId}: Sync {timeSinceLastSync.TotalMinutes:F1} min temu - pomijam");
                        await LogSyncCompleteAsync(logId, "SKIPPED", 0, 0, 0, con, "Too recent sync");
                        return result;
                    }
                }

                progress?.Report($"Konto {accountId}: Pobieranie listy issues...");
                var allIssuesFromApi = await apiClient.GetIssuesAsync();

                if (allIssuesFromApi == null || !allIssuesFromApi.Any())
                {
                    await LogSyncCompleteAsync(logId, "SUCCESS", 0, 0, 0, con);
                    return result;
                }

                result.TotalProcessed = allIssuesFromApi.Count;
                bool hasLastMessageIdColumn = await CheckLastMessageIdColumnExists(con);
                int current = 0;

                foreach (var issueShort in allIssuesFromApi)
                {
                    current++;
                    if (current % 10 == 0 || current == allIssuesFromApi.Count)
                    {
                        progress?.Report($"Konto {accountId}: {current}/{allIssuesFromApi.Count}...");
                    }

                    try
                    {
                        var dbState = await GetIssueSyncStateAsync(issueShort.Id, con);
                        bool isNewIssue = dbState == null;
                        bool statusChanged = dbState != null && !string.Equals(dbState.StatusAllegro ?? string.Empty, issueShort.CurrentState?.Status ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                        bool dueDateChanged = dbState != null && dbState.DecisionDueDate != issueShort.DecisionDueDate;
                        int apiMessagesCount = issueShort.Chat?.MessagesCount ?? 0;
                        bool hasNewMessagesByCount = isNewIssue || apiMessagesCount > dbState.LastMessageCount;

                        bool requiresBackfill = !isNewIssue && await RequiresIssueBackfillAsync(issueShort.Id, con);

                        if (isNewIssue || requiresBackfill)
                        {
                            var fullIssue = await apiClient.GetIssueDetailsAsync(issueShort.Id);
                            if (fullIssue == null)
                            {
                                result.ErrorMessages.Add($"Issue {issueShort.Id}: Nie można pobrać szczegółów");
                                continue;
                            }

                            OrderDetails orderDetails = null;
                            if (!string.IsNullOrEmpty(fullIssue.CheckoutForm?.Id))
                            {
                                orderDetails = await apiClient.GetOrderDetailsByCheckoutFormIdAsync(fullIssue.CheckoutForm.Id);
                            }

                            bool inserted = await UpsertIssueAsync(fullIssue, orderDetails, accountId, con);
                            if (inserted)
                            {
                                result.NewIssues++;
                            }

                            if (requiresBackfill)
                            {
                                System.Diagnostics.Debug.WriteLine($"[BACKFILL] Issue {issueShort.Id}: uzupełniono brakujące dane relacyjne.");
                            }
                        }
                        else if (statusChanged || dueDateChanged)
                        {
                            await UpdateIssueFromListAsync(issueShort, con);
                        }

                        if (hasNewMessagesByCount)
                        {
                            bool hasNewMessages = await SynchronizeChatForIssueAsync_Fixed(apiClient, issueShort, con, hasLastMessageIdColumn);
                            if (hasNewMessages)
                            {
                                result.IssuesWithNewMessages++;
                            }
                        }
                    }
                    catch (Exception exIssue)
                    {
                        result.ErrorMessages.Add($"Issue {issueShort.Id}: {exIssue.Message}");
                    }
                }

                progress?.Report($"Konto {accountId}: ✅ OK! (Nowych: {result.NewIssues}, Czaty: {result.IssuesWithNewMessages})");
                await LogSyncCompleteAsync(logId, "SUCCESS", result.TotalProcessed, result.NewIssues, result.IssuesWithNewMessages, con);
            }
            catch (Exception ex)
            {
                progress?.Report($"Konto {accountId}: ❌ BŁĄD!");
                await LogSyncCompleteAsync(logId, "FAILED", result.TotalProcessed, result.NewIssues, result.IssuesWithNewMessages, con, ex.Message);
                throw;
            }

            return result;
        }

        private async Task<DateTime?> GetLastSuccessfulSyncTimeAsync(int accountId, MySqlConnection con)
        {
            var cmd = new MySqlCommand(@"
                SELECT CompletedAt 
                FROM AllegroSyncLog 
                WHERE AllegroAccountId = @AccountId 
                  AND SyncType = 'ISSUES' 
                  AND Status = 'SUCCESS'
                ORDER BY CompletedAt DESC 
                LIMIT 1", con);

            cmd.Parameters.AddWithValue("@AccountId", accountId);

            var result = await cmd.ExecuteScalarAsync();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToDateTime(result);
            }

            return null;
        }

        private sealed class IssueSyncState
        {
            public int LastMessageCount { get; set; }
            public string StatusAllegro { get; set; }
            public DateTime? DecisionDueDate { get; set; }
        }

        private async Task<bool> RequiresIssueBackfillAsync(string disputeId, MySqlConnection con)
        {
            using (var cmd = new MySqlCommand(@"
                SELECT 1
                FROM AllegroDisputes
                WHERE DisputeId = @DisputeId
                  AND (
                        BuyerFirstName IS NULL
                        OR BuyerLastName IS NULL
                        OR BuyerEmail IS NULL
                        OR DeliveryStreet IS NULL
                        OR DeliveryZipCode IS NULL
                        OR DeliveryCity IS NULL
                        OR DeliveryPhoneNumber IS NULL
                        OR Expectations IS NULL
                        OR InitialMessageText IS NULL
                        OR CreatedAt IS NULL
                        OR Status IS NULL
                        OR ProductName IS NULL
                        OR BoughtAt IS NULL
                        OR OrderJsonDetails IS NULL
                      )
                LIMIT 1", con))
            {
                cmd.Parameters.AddWithValue("@DisputeId", disputeId);
                var result = await cmd.ExecuteScalarAsync();
                return result != null && result != DBNull.Value;
            }
        }

        private async Task<IssueSyncState> GetIssueSyncStateAsync(string disputeId, MySqlConnection con)
        {
            using (var cmd = new MySqlCommand(@"
                SELECT LastMessageCount, StatusAllegro, DecisionDueDate
                FROM AllegroDisputes
                WHERE DisputeId = @DisputeId
                LIMIT 1", con))
            {
                cmd.Parameters.AddWithValue("@DisputeId", disputeId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        return null;
                    }

                    return new IssueSyncState
                    {
                        LastMessageCount = reader["LastMessageCount"] == DBNull.Value ? 0 : Convert.ToInt32(reader["LastMessageCount"]),
                        StatusAllegro = reader["StatusAllegro"]?.ToString(),
                        DecisionDueDate = reader["DecisionDueDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DecisionDueDate"])
                    };
                }
            }
        }

        private async Task UpdateIssueFromListAsync(Issue issueShort, MySqlConnection con)
        {
            string status = issueShort.CurrentState?.Status ?? string.Empty;
            bool needsDecision = (status == "IN_PROGRESS" || status == "OPENED")
                                 && issueShort.DecisionDueDate.HasValue
                                 && issueShort.DecisionDueDate.Value > DateTime.Now;

            using (var cmd = new MySqlCommand(@"
                UPDATE AllegroDisputes
                SET StatusAllegro = @StatusAllegro,
                    DecisionDueDate = @DecisionDueDate,
                    LastCheckedAt = @LastCheckedAt,
                    NeedsDecision = @NeedsDecision
                WHERE DisputeId = @DisputeId", con))
            {
                cmd.Parameters.AddWithValue("@StatusAllegro", issueShort.CurrentState?.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DecisionDueDate", issueShort.DecisionDueDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastCheckedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@NeedsDecision", needsDecision ? 1 : 0);
                cmd.Parameters.AddWithValue("@DisputeId", issueShort.Id);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<int> SynchronizeChatsOnlyAsync(
            AllegroApiClient apiClient,
            int accountId,
            MySqlConnection con,
            IProgress<string> progress = null)
        {
            int issuesWithNewMessages = 0;

            // ⭐ SPRAWDŹ czy kolumna LastMessageId istnieje
            bool hasLastMessageIdColumn = await CheckLastMessageIdColumnExists(con);

            var issues = new List<string>();
            using (var cmd = new MySqlCommand(
                "SELECT DisputeId FROM AllegroDisputes WHERE AllegroAccountId = @AccountId", con))
            {
                cmd.Parameters.AddWithValue("@AccountId", accountId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        issues.Add(reader["DisputeId"].ToString());
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"[SYNC CHATS] Sprawdzanie {issues.Count} issues");

            int current = 0;
            foreach (var issueId in issues)
            {
                current++;
                if (current % 10 == 0)
                {
                    progress?.Report($"Konto {accountId}: Czaty {current}/{issues.Count}...");
                }

                try
                {
                    var issue = await apiClient.GetIssueDetailsAsync(issueId);
                    if (issue == null) continue;

                    bool hasNewMessages = await SynchronizeChatForIssueAsync_Fixed(
                        apiClient, issue, con, hasLastMessageIdColumn);
                    if (hasNewMessages)
                    {
                        issuesWithNewMessages++;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Czat {issueId}: {ex.Message}");
                }
            }

            return issuesWithNewMessages;
        }

        private async Task ProcessSingleIssueAsync(
            AllegroApiClient apiClient,
            Issue issueShort,
            int accountId,
            MySqlConnection con,
            IssuesSyncResult result)
        {
            try
            {
                var issue = await apiClient.GetIssueDetailsAsync(issueShort.Id);
                if (issue == null) return;

                OrderDetails orderDetails = null;
                if (!string.IsNullOrEmpty(issue.CheckoutForm?.Id))
                {
                    orderDetails = await apiClient.GetOrderDetailsByCheckoutFormIdAsync(issue.CheckoutForm.Id);
                }

                // ✅ v3.2: Dane kupującego pobierane z orderDetails.Buyer (issue.Buyer jest NULL)
                // BuyerLogin i BuyerEmail będą przekazane bezpośrednio z orderDetails do UpsertIssueAsync

                bool isNew = await UpsertIssueAsync(issue, orderDetails, accountId, con);
                if (isNew) result.NewIssues++;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Issue {issueShort.Id}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ERROR] Issue {issueShort.Id}: {ex.Message}");
            }
        }

        private async Task<string> GetBuyerEmailAsync(AllegroApiClient apiClient, string checkoutFormId)
        {
            try
            {
                return await apiClient.GetBuyerEmailAsync(checkoutFormId);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Synchronizacja czatu - FIXED v3.1

        /// <summary>
        /// ⭐ NAPRAWIONA synchronizacja czatu - obsługuje brak kolumny LastMessageId
        /// </summary>
        private async Task<bool> SynchronizeChatForIssueAsync_Fixed(
            AllegroApiClient apiClient,
            Issue issue,
            MySqlConnection con,
            bool hasLastMessageIdColumn)
        {
            try
            {
                var messagesFromApi = await apiClient.GetChatAsync(issue.Id);

                if (messagesFromApi == null || !messagesFromApi.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[CHAT] Issue {issue.Id}: Brak wiadomości");
                    return false;
                }

                var latestMessage = messagesFromApi.First();
                string latestMessageId = latestMessage.Id;

                // ⭐ NAPRAWKA: Sprawdź LastMessageId TYLKO jeśli kolumna istnieje
                if (hasLastMessageIdColumn)
                {
                    string lastMessageIdInDb = null;
                    using (var cmd = new MySqlCommand(
                        "SELECT LastMessageId FROM AllegroDisputes WHERE DisputeId = @DisputeId", con))
                    {
                        cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                        var result = await cmd.ExecuteScalarAsync();
                        lastMessageIdInDb = result?.ToString();
                    }

                    if (latestMessageId == lastMessageIdInDb)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CHAT SKIP] Issue {issue.Id}: Brak nowych wiadomości");
                        return false;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[CHAT SYNC] Issue {issue.Id}: {messagesFromApi.Count} wiadomości");

                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
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
                            insertCmd.Parameters.AddWithValue("@CreatedAt", msg.CreatedAt);
                            insertCmd.Parameters.AddWithValue("@HasAttachments",
                                msg.Attachments != null && msg.Attachments.Any() ? 1 : 0);
                            insertCmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(msg));

                            await insertCmd.ExecuteNonQueryAsync();

                            if (msg.Attachments != null && msg.Attachments.Any())
                            {
                                await SaveChatAttachmentsAsync(msg, con, transaction);
                            }

                            await SyncMessageToCentrumKontaktuAsync(msg, issue, con, transaction);
                        }

                        // ⭐ NAPRAWKA: Aktualizuj LastMessageId TYLKO jeśli kolumna istnieje
                        string updateSql = hasLastMessageIdColumn
                            ? @"UPDATE AllegroDisputes 
                                SET 
                                    LastMessageCount = @Count, 
                                    LastMessageId = @LastMessageId,
                                    HasNewMessages = 1
                                WHERE DisputeId = @DisputeId"
                            : @"UPDATE AllegroDisputes 
                                SET 
                                    LastMessageCount = @Count, 
                                    HasNewMessages = 1
                                WHERE DisputeId = @DisputeId";

                        var updateCmd = new MySqlCommand(updateSql, con, transaction);
                        updateCmd.Parameters.AddWithValue("@Count", messagesFromApi.Count);
                        if (hasLastMessageIdColumn)
                        {
                            updateCmd.Parameters.AddWithValue("@LastMessageId", latestMessageId);
                        }
                        updateCmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                        await updateCmd.ExecuteNonQueryAsync();

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine($"[CHAT ERROR] Transaction rollback: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT ERROR] Issue {issue.Id}: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        private async Task SyncMessageToCentrumKontaktuAsync(
            Reklamacje_Dane.Allegro.Issues.ChatMessage message,
            Issue issue,
            MySqlConnection con,
            MySqlTransaction transaction)
        {
            string nrZgloszenia = null;
            using (var cmd = new MySqlCommand(
                "SELECT ComplaintId FROM AllegroDisputes WHERE DisputeId = @DisputeId", con, transaction))
            {
                cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                var result = await cmd.ExecuteScalarAsync();
                nrZgloszenia = result?.ToString();
            }

            if (string.IsNullOrEmpty(nrZgloszenia) || nrZgloszenia == "0")
                return;

            using (var checkCmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM CentrumKontaktu 
                WHERE Kanal = 'Allegro' AND MetadataJson LIKE CONCAT('%', @MessageId, '%')",
                con, transaction))
            {
                checkCmd.Parameters.AddWithValue("@MessageId", message.Id);
                long count = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());
                if (count > 0) return;
            }

            var insertCmd = new MySqlCommand(@"
                INSERT INTO CentrumKontaktu 
                (ZgloszenieID, Typ, Kierunek, Nadawca, Odbiorca, Tresc, 
                 DataWyslania, DataOdbioru, Status, Priorytet, Kanal, MetadataJson)
                VALUES 
                (@ZgloszenieID, @Typ, @Kierunek, @Nadawca, @Odbiorca, @Tresc,
                 @DataWyslania, @DataOdbioru, @Status, @Priorytet, @Kanal, @MetadataJson)",
                con, transaction);

            insertCmd.Parameters.AddWithValue("@ZgloszenieID", nrZgloszenia);
            insertCmd.Parameters.AddWithValue("@Typ", "Chat Allegro");
            string kierunek = message.Author?.Role == "SELLER" ? "OUT" : "IN";
            insertCmd.Parameters.AddWithValue("@Kierunek", kierunek);
            insertCmd.Parameters.AddWithValue("@Nadawca", message.Author?.Login ?? "Allegro");
            insertCmd.Parameters.AddWithValue("@Odbiorca", kierunek == "IN" ? "System" : message.Author?.Login);
            insertCmd.Parameters.AddWithValue("@Tresc", message.Text ?? "");
            insertCmd.Parameters.AddWithValue("@DataWyslania", message.CreatedAt);
            insertCmd.Parameters.AddWithValue("@DataOdbioru", message.CreatedAt);
            insertCmd.Parameters.AddWithValue("@Status", "Dostarczona");
            insertCmd.Parameters.AddWithValue("@Priorytet", "Normalny");
            insertCmd.Parameters.AddWithValue("@Kanal", "Allegro");

            var metadata = new
            {
                MessageId = message.Id,
                DisputeId = issue.Id,
                IssueType = issue.Type,
                HasAttachments = message.Attachments?.Count > 0
            };
            insertCmd.Parameters.AddWithValue("@MetadataJson", JsonConvert.SerializeObject(metadata));

            await insertCmd.ExecuteNonQueryAsync();
        }

        private async Task SaveChatAttachmentsAsync(
            Reklamacje_Dane.Allegro.Issues.ChatMessage message,
            MySqlConnection con,
            MySqlTransaction transaction)
        {
            foreach (var attachment in message.Attachments)
            {
                var insertCmd = new MySqlCommand(@"
                    INSERT INTO AllegroChatAttachments 
                    (MessageId, AttachmentId, FileName, Url, Downloaded, DownloadedAt, LocalPath)
                    VALUES 
                    (@MessageId, @AttachmentId, @FileName, @Url, 0, NULL, NULL)",
                    con, transaction);

                insertCmd.Parameters.AddWithValue("@MessageId", message.Id);
                insertCmd.Parameters.AddWithValue("@AttachmentId", DBNull.Value);
                insertCmd.Parameters.AddWithValue("@FileName", attachment.FileName ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Url", attachment.Url ?? (object)DBNull.Value);

                await insertCmd.ExecuteNonQueryAsync();
            }
        }

        #endregion

        #region Upsert Issue

        private async Task<bool> UpsertIssueAsync(
            Issue issue,
            OrderDetails orderDetails,
            int accountId,
            MySqlConnection con)
        {
            var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM AllegroDisputes WHERE DisputeId = @DisputeId", con);
            checkCmd.Parameters.AddWithValue("@DisputeId", issue.Id);
            long count = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());
            bool isNew = (count == 0);

            string sql = isNew ? GetInsertIssueSql() : GetUpdateIssueSql();

            using (var cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                cmd.Parameters.AddWithValue("@AllegroAccountId", accountId);
                cmd.Parameters.AddWithValue("@Type", issue.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReferenceNumber", issue.ReferenceNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Subject", issue.Subject ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", issue.Description ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@StatusAllegro", issue.CurrentState?.Status ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@OpenedAt", issue.OpenedDate);
                cmd.Parameters.AddWithValue("@DecisionDueDate", issue.DecisionDueDate ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@ClosedAt", issue.ClosedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastCheckedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreatedAt", issue.OpenedDate);
                cmd.Parameters.AddWithValue("@Status", issue.CurrentState?.Status ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@OrderId", issue.CheckoutForm?.Id ?? (object)DBNull.Value);
                
                // ✅ v3.2 BUYER FIX: Dane kupującego z orderDetails.Buyer zamiast issue.Buyer (które jest NULL)
                cmd.Parameters.AddWithValue("@BuyerLogin", orderDetails?.Buyer?.Login ?? issue.Buyer?.Login ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerEmail", orderDetails?.Buyer?.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerFirstName", orderDetails?.Buyer?.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerLastName", orderDetails?.Buyer?.LastName ?? (object)DBNull.Value);

                var deliveryAddress = orderDetails?.Delivery?.Address;
                cmd.Parameters.AddWithValue("@DeliveryCompanyName", deliveryAddress?.CompanyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeliveryStreet", deliveryAddress?.Street ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeliveryZipCode", deliveryAddress?.ZipCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeliveryCity", deliveryAddress?.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeliveryPhoneNumber", deliveryAddress?.PhoneNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeliveryCompany", deliveryAddress?.CompanyName ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@ProductId", issue.Product?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OfferId", issue.Offer?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductName", issue.Offer?.Name ?? (object)DBNull.Value);

                var matchingLineItem = orderDetails?.LineItems?.FirstOrDefault(li =>
                    string.Equals(li?.Offer?.Id, issue.Offer?.Id, StringComparison.OrdinalIgnoreCase))
                    ?? orderDetails?.LineItems?.FirstOrDefault();

                cmd.Parameters.AddWithValue("@ProductEAN", matchingLineItem?.Offer?.Product?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductSKU", matchingLineItem?.Offer?.External?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceNumber", orderDetails?.Invoice?.InvoiceNumber ?? (object)DBNull.Value);

                var firstExpectation = issue.Expectations?.FirstOrDefault();
                cmd.Parameters.AddWithValue("@Expectations", firstExpectation?.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InitialMessageText", issue.Chat?.InitialMessage?.Text ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InitialMessageCount", issue.Chat?.MessagesCount ?? 0);

                cmd.Parameters.AddWithValue("@ExpectationType", firstExpectation?.Name ?? (object)DBNull.Value);

                decimal? refundAmount = null;
                if (firstExpectation?.Refund?.Amount != null)
                {
                    refundAmount = SafeParseDecimal(firstExpectation.Refund.Amount, issue.Id);
                }
                cmd.Parameters.AddWithValue("@ExpectationRefundAmount", refundAmount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpectationRefundCurrency",
                    firstExpectation?.Refund?.Currency ?? "PLN");

                cmd.Parameters.AddWithValue("@ReasonType", issue.Reason?.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReasonDescription", issue.Reason?.Description ?? (object)DBNull.Value);

                DateTime? boughtAt = null;
                if (orderDetails?.BoughtAt != null)
                {
                    boughtAt = orderDetails.BoughtAt;
                }
                cmd.Parameters.AddWithValue("@BoughtAt", boughtAt ?? (object)DBNull.Value);

                string status = issue.CurrentState?.Status ?? "";
                bool needsDecision = (status == "IN_PROGRESS" || status == "OPENED")
                                     && issue.DecisionDueDate.HasValue
                                     && issue.DecisionDueDate.Value > DateTime.Now;
                cmd.Parameters.AddWithValue("@NeedsDecision", needsDecision ? 1 : 0);

                int? complaintId = await FindRelatedComplaintAsync(issue, orderDetails, con);
                cmd.Parameters.AddWithValue("@ComplaintId", complaintId ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(issue));
                cmd.Parameters.AddWithValue("@OrderJsonDetails",
                    orderDetails != null ? JsonConvert.SerializeObject(orderDetails) : (object)DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }

            if (orderDetails != null)
            {
                try
                {
                    await UpsertOrderDetailsAsync(orderDetails, accountId, con);
                    await UpsertOrderItemsAsync(orderDetails, con);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[WARN] Nie udało się zapisać tabel AllegroOrderDetails/AllegroOrderItems dla {orderDetails.Id}: {ex.Message}");
                }
            }

            return isNew;
        }

        /// <summary>
        /// ⚠️ TYMCZASOWO WYŁĄCZONE - naprawia błąd MySQL "Unknown column 'Email'"
        /// </summary>
        private async Task<int?> FindRelatedComplaintAsync(
            Issue issue,
            OrderDetails orderDetails,
            MySqlConnection con)
        {
            // TODO: Sprawdzić dokładną nazwę kolumny email w tabeli Zgloszenia
            return null;
        }

        private async Task UpsertOrderDetailsAsync(OrderDetails orderDetails, int accountId, MySqlConnection con)
        {
            if (string.IsNullOrWhiteSpace(orderDetails?.Id))
            {
                return;
            }

            using (var cmd = new MySqlCommand(@"
                INSERT INTO AllegroOrderDetails
                (OrderId, AllegroAccountId, Status, FulfillmentStatus, BoughtAt, UpdatedAt,
                 BuyerLogin, BuyerEmail, BuyerFirstName, BuyerLastName, BuyerPhoneNumber,
                 Delivery_Street, Delivery_City, Delivery_ZipCode, Delivery_CountryCode, Delivery_CompanyName,
                 PaymentType, PaymentProvider, PaymentFinishedAt, PaidAmount, PaidCurrency,
                 InvoiceRequired, Invoice_CompanyName, Invoice_TaxId, Invoice_Street, Invoice_City, Invoice_ZipCode, Invoice_CountryCode,
                 MessageToSeller, JsonDetails, LastSyncAt)
                VALUES
                (@OrderId, @AllegroAccountId, @Status, @FulfillmentStatus, @BoughtAt, @UpdatedAt,
                 @BuyerLogin, @BuyerEmail, @BuyerFirstName, @BuyerLastName, @BuyerPhoneNumber,
                 @Delivery_Street, @Delivery_City, @Delivery_ZipCode, @Delivery_CountryCode, @Delivery_CompanyName,
                 @PaymentType, @PaymentProvider, @PaymentFinishedAt, @PaidAmount, @PaidCurrency,
                 @InvoiceRequired, @Invoice_CompanyName, @Invoice_TaxId, @Invoice_Street, @Invoice_City, @Invoice_ZipCode, @Invoice_CountryCode,
                 @MessageToSeller, @JsonDetails, @LastSyncAt)
                ON DUPLICATE KEY UPDATE
                 AllegroAccountId = VALUES(AllegroAccountId),
                 Status = VALUES(Status),
                 FulfillmentStatus = VALUES(FulfillmentStatus),
                 BoughtAt = VALUES(BoughtAt),
                 UpdatedAt = VALUES(UpdatedAt),
                 BuyerLogin = VALUES(BuyerLogin),
                 BuyerEmail = VALUES(BuyerEmail),
                 BuyerFirstName = VALUES(BuyerFirstName),
                 BuyerLastName = VALUES(BuyerLastName),
                 BuyerPhoneNumber = VALUES(BuyerPhoneNumber),
                 Delivery_Street = VALUES(Delivery_Street),
                 Delivery_City = VALUES(Delivery_City),
                 Delivery_ZipCode = VALUES(Delivery_ZipCode),
                 Delivery_CountryCode = VALUES(Delivery_CountryCode),
                 Delivery_CompanyName = VALUES(Delivery_CompanyName),
                 PaymentType = VALUES(PaymentType),
                 PaymentProvider = VALUES(PaymentProvider),
                 PaymentFinishedAt = VALUES(PaymentFinishedAt),
                 PaidAmount = VALUES(PaidAmount),
                 PaidCurrency = VALUES(PaidCurrency),
                 InvoiceRequired = VALUES(InvoiceRequired),
                 Invoice_CompanyName = VALUES(Invoice_CompanyName),
                 Invoice_TaxId = VALUES(Invoice_TaxId),
                 Invoice_Street = VALUES(Invoice_Street),
                 Invoice_City = VALUES(Invoice_City),
                 Invoice_ZipCode = VALUES(Invoice_ZipCode),
                 Invoice_CountryCode = VALUES(Invoice_CountryCode),
                 MessageToSeller = VALUES(MessageToSeller),
                 JsonDetails = VALUES(JsonDetails),
                 LastSyncAt = VALUES(LastSyncAt)", con))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderDetails.Id);
                cmd.Parameters.AddWithValue("@AllegroAccountId", accountId);
                cmd.Parameters.AddWithValue("@Status", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FulfillmentStatus", orderDetails.Fulfillment?.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BoughtAt", orderDetails.BoughtAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@BuyerLogin", orderDetails.Buyer?.Login ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerEmail", orderDetails.Buyer?.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerFirstName", orderDetails.Buyer?.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerLastName", orderDetails.Buyer?.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerPhoneNumber", orderDetails.Buyer?.PhoneNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Delivery_Street", orderDetails.Delivery?.Address?.Street ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Delivery_City", orderDetails.Delivery?.Address?.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Delivery_ZipCode", orderDetails.Delivery?.Address?.ZipCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Delivery_CountryCode", "PL");
                cmd.Parameters.AddWithValue("@Delivery_CompanyName", orderDetails.Delivery?.Address?.CompanyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PaymentType", orderDetails.Payment?.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PaymentProvider", orderDetails.Payment?.Provider ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PaymentFinishedAt", orderDetails.Payment?.FinishedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PaidAmount", SafeParseDecimal(orderDetails.Payment?.PaidAmount?.Amount) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PaidCurrency", orderDetails.Payment?.PaidAmount?.Currency ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceRequired", orderDetails.Invoice != null ? 1 : 0);
                cmd.Parameters.AddWithValue("@Invoice_CompanyName", orderDetails.Invoice?.Address?.CompanyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Invoice_TaxId", orderDetails.Invoice?.Address?.TaxId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Invoice_Street", orderDetails.Invoice?.Address?.Street ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Invoice_City", orderDetails.Invoice?.Address?.City ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Invoice_ZipCode", orderDetails.Invoice?.Address?.ZipCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Invoice_CountryCode", "PL");
                cmd.Parameters.AddWithValue("@MessageToSeller", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(orderDetails));
                cmd.Parameters.AddWithValue("@LastSyncAt", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task UpsertOrderItemsAsync(OrderDetails orderDetails, MySqlConnection con)
        {
            if (string.IsNullOrWhiteSpace(orderDetails?.Id) || orderDetails.LineItems == null || !orderDetails.LineItems.Any())
            {
                return;
            }

            using (var deleteCmd = new MySqlCommand("DELETE FROM AllegroOrderItems WHERE OrderId = @OrderId", con))
            {
                deleteCmd.Parameters.AddWithValue("@OrderId", orderDetails.Id);
                await deleteCmd.ExecuteNonQueryAsync();
            }

            foreach (var item in orderDetails.LineItems)
            {
                using (var insertCmd = new MySqlCommand(@"
                    INSERT INTO AllegroOrderItems
                    (OrderId, OfferId, Name, Quantity, Price, Currency, ReconciliationAmount, ImageUrl, JsonDetails)
                    VALUES
                    (@OrderId, @OfferId, @Name, @Quantity, @Price, @Currency, @ReconciliationAmount, @ImageUrl, @JsonDetails)", con))
                {
                    insertCmd.Parameters.AddWithValue("@OrderId", orderDetails.Id);
                    insertCmd.Parameters.AddWithValue("@OfferId", item.Offer?.Id ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@Name", item.Offer?.Name ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    insertCmd.Parameters.AddWithValue("@Price", SafeParseDecimal(item.Price?.Amount) ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@Currency", item.Price?.Currency ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@ReconciliationAmount", DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@ImageUrl", DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(item));
                    await insertCmd.ExecuteNonQueryAsync();
                }
            }
        }

        #endregion

        #region SQL Generators

        private string GetInsertReturnSql()
        {
            return @"
                INSERT INTO AllegroCustomerReturns 
                (AllegroReturnId, AllegroAccountId, ReferenceNumber, OrderId, BuyerLogin, BuyerEmail, 
                 CreatedAt, StatusAllegro, Waybill, TransportingWaybill, CarrierName, TransportingCarrierId, SenderPhoneNumber,
                 ProductName, OfferId, Quantity, ProductPrice, ProductPriceCurrency,
                 ReturnReasonType, ReturnReasonComment,
                 PaymentType, PaymentProvider, PaymentFinishedAt, PaidAmount, FulfillmentStatus,
                 Delivery_FirstName, Delivery_LastName, Delivery_Street, Delivery_ZipCode, Delivery_City, Delivery_PhoneNumber, Delivery_CountryCode, Delivery_CompanyName,
                 DeliveryMethod, DeliveryMethodId,
                 Buyer_FirstName, Buyer_LastName, Buyer_Street, Buyer_ZipCode, Buyer_City, Buyer_PhoneNumber, Buyer_CountryCode, Buyer_CompanyName,
                 Invoice_CompanyName, Invoice_TaxId, Invoice_Street, Invoice_ZipCode, Invoice_City, Invoice_CountryCode, InvoiceRequired,
                 MarketplaceId, BoughtAt, JsonDetails, OrderJsonDetails, LastSyncAt)
                VALUES 
                (@AllegroReturnId, @AllegroAccountId, @ReferenceNumber, @OrderId, @BuyerLogin, @BuyerEmail,
                 @CreatedAt, @StatusAllegro, @Waybill, @TransportingWaybill, @CarrierName, @TransportingCarrierId, @SenderPhoneNumber,
                 @ProductName, @OfferId, @Quantity, @ProductPrice, @ProductPriceCurrency,
                 @ReturnReasonType, @ReturnReasonComment,
                 @PaymentType, @PaymentProvider, @PaymentFinishedAt, @PaidAmount, @FulfillmentStatus,
                 @Delivery_FirstName, @Delivery_LastName, @Delivery_Street, @Delivery_ZipCode, @Delivery_City, @Delivery_PhoneNumber, @Delivery_CountryCode, @Delivery_CompanyName,
                 @DeliveryMethod, @DeliveryMethodId,
                 @Buyer_FirstName, @Buyer_LastName, @Buyer_Street, @Buyer_ZipCode, @Buyer_City, @Buyer_PhoneNumber, @Buyer_CountryCode, @Buyer_CompanyName,
                 @Invoice_CompanyName, @Invoice_TaxId, @Invoice_Street, @Invoice_ZipCode, @Invoice_City, @Invoice_CountryCode, @InvoiceRequired,
                 @MarketplaceId, @BoughtAt, @JsonDetails, @OrderJsonDetails, @LastSyncAt)";
        }

        private string GetUpdateReturnSql()
        {
            return @"
                UPDATE AllegroCustomerReturns SET
                    ReferenceNumber = @ReferenceNumber,
                    OrderId = @OrderId,
                    BuyerLogin = @BuyerLogin,
                    BuyerEmail = @BuyerEmail,
                    StatusAllegro = @StatusAllegro,
                    Waybill = @Waybill,
                    TransportingWaybill = @TransportingWaybill,
                    CarrierName = @CarrierName,
                    TransportingCarrierId = @TransportingCarrierId,
                    SenderPhoneNumber = @SenderPhoneNumber,
                    ProductName = @ProductName,
                    OfferId = @OfferId,
                    Quantity = @Quantity,
                    ProductPrice = @ProductPrice,
                    ProductPriceCurrency = @ProductPriceCurrency,
                    ReturnReasonType = @ReturnReasonType,
                    ReturnReasonComment = @ReturnReasonComment,
                    PaymentType = @PaymentType,
                    PaymentProvider = @PaymentProvider,
                    PaymentFinishedAt = @PaymentFinishedAt,
                    PaidAmount = @PaidAmount,
                    FulfillmentStatus = @FulfillmentStatus,
                    Delivery_FirstName = @Delivery_FirstName,
                    Delivery_LastName = @Delivery_LastName,
                    Delivery_Street = @Delivery_Street,
                    Delivery_ZipCode = @Delivery_ZipCode,
                    Delivery_City = @Delivery_City,
                    Delivery_PhoneNumber = @Delivery_PhoneNumber,
                    Delivery_CountryCode = @Delivery_CountryCode,
                    Delivery_CompanyName = @Delivery_CompanyName,
                    DeliveryMethod = @DeliveryMethod,
                    DeliveryMethodId = @DeliveryMethodId,
                    Buyer_FirstName = @Buyer_FirstName,
                    Buyer_LastName = @Buyer_LastName,
                    Buyer_Street = @Buyer_Street,
                    Buyer_ZipCode = @Buyer_ZipCode,
                    Buyer_City = @Buyer_City,
                    Buyer_PhoneNumber = @Buyer_PhoneNumber,
                    Buyer_CountryCode = @Buyer_CountryCode,
                    Buyer_CompanyName = @Buyer_CompanyName,
                    Invoice_CompanyName = @Invoice_CompanyName,
                    Invoice_TaxId = @Invoice_TaxId,
                    Invoice_Street = @Invoice_Street,
                    Invoice_ZipCode = @Invoice_ZipCode,
                    Invoice_City = @Invoice_City,
                    Invoice_CountryCode = @Invoice_CountryCode,
                    InvoiceRequired = @InvoiceRequired,
                    MarketplaceId = @MarketplaceId,
                    BoughtAt = @BoughtAt,
                    JsonDetails = @JsonDetails,
                    OrderJsonDetails = @OrderJsonDetails,
                    LastSyncAt = @LastSyncAt
                WHERE AllegroReturnId = @AllegroReturnId";
        }

        private string GetInsertIssueSql()
        {
            return @"
                INSERT INTO AllegroDisputes 
                (DisputeId, AllegroAccountId, Type, ReferenceNumber, Subject, Description, StatusAllegro,
                 OpenedAt, DecisionDueDate, ClosedAt, LastCheckedAt, CreatedAt, Status, OrderId, BuyerLogin, BuyerEmail, BuyerFirstName, BuyerLastName,
                 DeliveryCompanyName, DeliveryStreet, DeliveryZipCode, DeliveryCity, DeliveryPhoneNumber, DeliveryCompany,
                 ProductId, OfferId, ProductName, ProductEAN, ProductSKU, InvoiceNumber,
                 Expectations, InitialMessageText, InitialMessageCount,
                 ExpectationType, ExpectationRefundAmount, ExpectationRefundCurrency,
                 ReasonType, ReasonDescription, BoughtAt, NeedsDecision, LastMessageCount, HasNewMessages,
                 ComplaintId, JsonDetails, OrderJsonDetails)
                VALUES 
                (@DisputeId, @AllegroAccountId, @Type, @ReferenceNumber, @Subject, @Description, @StatusAllegro,
                 @OpenedAt, @DecisionDueDate, @ClosedAt, @LastCheckedAt, @CreatedAt, @Status, @OrderId, @BuyerLogin, @BuyerEmail, @BuyerFirstName, @BuyerLastName,
                 @DeliveryCompanyName, @DeliveryStreet, @DeliveryZipCode, @DeliveryCity, @DeliveryPhoneNumber, @DeliveryCompany,
                 @ProductId, @OfferId, @ProductName, @ProductEAN, @ProductSKU, @InvoiceNumber,
                 @Expectations, @InitialMessageText, @InitialMessageCount,
                 @ExpectationType, @ExpectationRefundAmount, @ExpectationRefundCurrency,
                 @ReasonType, @ReasonDescription, @BoughtAt, @NeedsDecision, 0, 0,
                 @ComplaintId, @JsonDetails, @OrderJsonDetails)";
        }

        private string GetUpdateIssueSql()
        {
            return @"
                UPDATE AllegroDisputes SET
                    Type = @Type,
                    ReferenceNumber = @ReferenceNumber,
                    Subject = @Subject,
                    Description = @Description,
                    StatusAllegro = @StatusAllegro,
                    OpenedAt = @OpenedAt,
                    DecisionDueDate = @DecisionDueDate,
                    ClosedAt = @ClosedAt,
                    LastCheckedAt = @LastCheckedAt,
                    CreatedAt = @CreatedAt,
                    Status = @Status,
                    OrderId = @OrderId,
                    BuyerLogin = @BuyerLogin,
                    BuyerEmail = @BuyerEmail,
                    BuyerFirstName = @BuyerFirstName,
                    BuyerLastName = @BuyerLastName,
                    DeliveryCompanyName = @DeliveryCompanyName,
                    DeliveryStreet = @DeliveryStreet,
                    DeliveryZipCode = @DeliveryZipCode,
                    DeliveryCity = @DeliveryCity,
                    DeliveryPhoneNumber = @DeliveryPhoneNumber,
                    DeliveryCompany = @DeliveryCompany,
                    ProductId = @ProductId,
                    OfferId = @OfferId,
                    ProductName = @ProductName,
                    ProductEAN = @ProductEAN,
                    ProductSKU = @ProductSKU,
                    InvoiceNumber = @InvoiceNumber,
                    Expectations = @Expectations,
                    InitialMessageText = @InitialMessageText,
                    InitialMessageCount = @InitialMessageCount,
                    ExpectationType = @ExpectationType,
                    ExpectationRefundAmount = @ExpectationRefundAmount,
                    ExpectationRefundCurrency = @ExpectationRefundCurrency,
                    ReasonType = @ReasonType,
                    ReasonDescription = @ReasonDescription,
                    BoughtAt = @BoughtAt,
                    NeedsDecision = @NeedsDecision,
                    JsonDetails = @JsonDetails,
                    OrderJsonDetails = @OrderJsonDetails
                WHERE DisputeId = @DisputeId";
        }

        #endregion

        #region Pomocnicze metody

        private async Task<List<AllegroAccountDetails>> GetAuthorizedAccountsAsync(MySqlConnection con)
        {
            var accounts = new List<AllegroAccountDetails>();
            using (var cmd = new MySqlCommand(
                "SELECT Id, ClientId, ClientSecretEncrypted FROM AllegroAccounts WHERE IsAuthorized = 1", con))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        accounts.Add(new AllegroAccountDetails
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ClientId = reader["ClientId"].ToString(),
                            ClientSecretEncrypted = EncryptionHelper.DecryptString(reader["ClientSecretEncrypted"].ToString())
                        });
                    }
                }
            }
            return accounts;
        }

        private async Task<int> LogSyncStartAsync(int accountId, string syncType, MySqlConnection con)
        {
            var cmd = new MySqlCommand(@"
                INSERT INTO AllegroSyncLog (AllegroAccountId, SyncType, StartedAt, Status)
                VALUES (@AccountId, @SyncType, @StartedAt, 'IN_PROGRESS');
                SELECT LAST_INSERT_ID();", con);

            cmd.Parameters.AddWithValue("@AccountId", accountId);
            cmd.Parameters.AddWithValue("@SyncType", syncType);
            cmd.Parameters.AddWithValue("@StartedAt", DateTime.Now);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        private async Task LogSyncCompleteAsync(
            int logId,
            string status,
            int processed,
            int created,
            int updated,
            MySqlConnection con,
            string errorMessage = null)
        {
            var cmd = new MySqlCommand(@"
                UPDATE AllegroSyncLog 
                SET CompletedAt = @CompletedAt, 
                    Status = @Status, 
                    ItemsProcessed = @Processed, 
                    ItemsNew = @Created, 
                    ItemsUpdated = @Updated, 
                    ErrorMessage = @ErrorMessage
                WHERE Id = @Id", con);

            cmd.Parameters.AddWithValue("@Id", logId);
            cmd.Parameters.AddWithValue("@CompletedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@Processed", processed);
            cmd.Parameters.AddWithValue("@Created", created);
            cmd.Parameters.AddWithValue("@Updated", updated);
            cmd.Parameters.AddWithValue("@ErrorMessage", errorMessage ?? (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        #endregion

        #region Klasy wynikowe

        public class AllegroCompleteSyncResult
        {
            public int TotalReturnsProcessed { get; set; }
            public int NewReturnsFound { get; set; }
            public int UpdatedReturns { get; set; }
            public int TotalIssuesProcessed { get; set; }
            public int NewIssuesFound { get; set; }
            public int IssuesWithNewMessages { get; set; }
            public DateTime CompletedAt { get; set; }
            public List<string> ErrorMessages { get; set; } = new List<string>();
        }

        public class ReturnsSyncResult
        {
            public int TotalProcessed { get; set; }
            public int NewReturns { get; set; }
            public int UpdatedReturns { get; set; }
            public List<string> ErrorMessages { get; set; } = new List<string>();
        }

        public class IssuesSyncResult
        {
            public int TotalProcessed { get; set; }
            public int NewIssues { get; set; }
            public int IssuesWithNewMessages { get; set; }
            public List<string> ErrorMessages { get; set; } = new List<string>();
        }

        #endregion
    }
}
