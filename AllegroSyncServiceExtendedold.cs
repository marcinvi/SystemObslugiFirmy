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
    /// Rozszerzony serwis synchronizacji Allegro - WERSJA 2.3 AUDITED
    /// ZMIANY W WERSJI 2.3:
    /// - NAPRAWIONO: GetBuyerEmailAsync - dodano autoryzację Bearer token (Problem #1)
    /// - NAPRAWIONO: GetIssueDetailsAsync - pobieranie pełnych szczegółów Issue (Problem #2)
    /// - NAPRAWIONO: GetChatAsync - dodano paginację dla wszystkich wiadomości (Problem #3)
    /// - NAPRAWIONO: SynchronizeIssuesForAccountAsync - używa GetIssueDetailsAsync
    /// 
    /// ZMIANY W WERSJI 2.2:
    /// - NAPRAWIONO: Bezpieczne parsowanie kwot (decimal) z obsługą różnych formatów
    /// - NAPRAWIONO: Obsługa błędu "Nieprawidłowy format ciągu wejściowego"
    /// - DODANO: Helper method SafeParseDecimal() dla bezpiecznej konwersji
    /// 
    /// ZMIANY W WERSJI 2.1:
    /// - Synchronizacja WSZYSTKICH czatów (nawet zamkniętych)
    /// - Pobieranie BuyerEmail z osobnego endpointu
    /// - Wykrywanie ClosedAt dla zamkniętych issues
    /// - Pobieranie ProductEAN, ProductSKU, InvoiceNumber
    /// - Automatyczne dodawanie do CentrumKontaktu
    /// - Logika ComplaintId (NULL/0/wartość)
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

        /// <summary>
        /// Bezpiecznie parsuje string na decimal, obsługując różne formaty i błędy
        /// </summary>
        /// <param name="value">Wartość string do sparsowania</param>
        /// <param name="returnId">ID zwrotu/issue dla logowania błędów (opcjonalne)</param>
        /// <returns>Wartość decimal lub null jeśli parsowanie nie powiodło się</returns>
        private decimal? SafeParseDecimal(string value, string returnId = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Usuń białe znaki
            value = value.Trim();

            // Sprawdź czy wartość jest pusta po trim
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                // Usuń separatory tysięcy (spacje, przecinki w formacie UK/US)
                value = value.Replace(" ", "").Replace(",", "");

                // Próba parsowania z kultura invariant (używa kropki jako separator dziesiętny)
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }

                // Próba parsowania z kulturą polską (używa przecinka jako separator dziesiętny)
                if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("pl-PL"), out decimal resultPL))
                {
                    return resultPL;
                }

                // Jeśli nic nie zadziałało, zaloguj błąd
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

        #endregion

        #region Publiczne metody synchronizacji

        /// <summary>
        /// Główna metoda synchronizacji - zwroty + dyskusje
        /// </summary>
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

                        // Synchronizacja zwrotów
                        progress?.Report("Pobieranie zwrotów...");
                        var returnsResult = await SynchronizeReturnsForAccountAsync(apiClient, account.Id, con);
                        result.TotalReturnsProcessed += returnsResult.TotalProcessed;
                        result.NewReturnsFound += returnsResult.NewReturns;
                        result.UpdatedReturns += returnsResult.UpdatedReturns;

                        // Synchronizacja dyskusji i reklamacji
                        progress?.Report("Pobieranie dyskusji i reklamacji...");
                        var issuesResult = await SynchronizeIssuesForAccountAsync(apiClient, account.Id, con);
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

        /// <summary>
        /// Synchronizacja tylko zwrotów
        /// </summary>
        public async Task<ReturnsSyncResult> SynchronizeReturnsAsync(IProgress<string> progress)
        {
            var result = new ReturnsSyncResult();

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();

                var accounts = await GetAuthorizedAccountsAsync(con);
                foreach (var account in accounts)
                {
                    try
                    {
                        var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(account.Id, con);
                        if (apiClient == null) continue;

                        var accountResult = await SynchronizeReturnsForAccountAsync(apiClient, account.Id, con);
                        result.TotalProcessed += accountResult.TotalProcessed;
                        result.NewReturns += accountResult.NewReturns;
                        result.UpdatedReturns += accountResult.UpdatedReturns;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessages.Add($"Konto {account.Id}: {ex.Message}");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Synchronizacja tylko dyskusji i reklamacji
        /// </summary>
        public async Task<IssuesSyncResult> SynchronizeIssuesAsync(IProgress<string> progress)
        {
            var result = new IssuesSyncResult();

            // ✅ DEBUG 1: Start
            System.Diagnostics.Debug.WriteLine("✅ SynchronizeIssuesAsync - START");

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                
                // ✅ DEBUG 2: Połączenie z bazą
                System.Diagnostics.Debug.WriteLine("✅ Połączenie z bazą - OK");

                var accounts = await GetAuthorizedAccountsAsync(con);
                
                // ✅ DEBUG 3: Liczba kont
                System.Diagnostics.Debug.WriteLine($"✅ Znaleziono kont Allegro: {accounts.Count}");
                
                if (!accounts.Any())
                {
                    System.Diagnostics.Debug.WriteLine("❌ Nie znaleziono żadnych autoryzowanych kont Allegro.");
                    System.Windows.Forms.MessageBox.Show(
                        "❌ ALLEGRO SYNC - BRAK KONT:\n\n" +
                        "Nie znaleziono autoryzowanych kont Allegro w bazie!\n\n" +
                        "Sprawdź tabelę: AllegroAccounts\n" +
                        "WHERE IsAuthorized = 1",
                        "Brak kont Allegro",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning
                    );
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

                        var accountResult = await SynchronizeIssuesForAccountAsync(apiClient, account.Id, con);
                        
                        System.Diagnostics.Debug.WriteLine($"✅ Konto {account.Id}: Przetworzono {accountResult.TotalProcessed}, Nowych: {accountResult.NewIssues}");
                        
                        result.TotalProcessed += accountResult.TotalProcessed;
                        result.NewIssues += accountResult.NewIssues;
                        result.IssuesWithNewMessages += accountResult.IssuesWithNewMessages;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Konto {account.Id}: EXCEPTION - {ex.Message}\n{ex.StackTrace}");
                        result.ErrorMessages.Add($"Konto {account.Id}: {ex.Message}");
                        
                        // ✅ DEBUG: Pokaż błąd
                        System.Windows.Forms.MessageBox.Show(
                            $"❌ ALLEGRO SYNC ERROR (Konto {account.Id}):\n\n" +
                            $"Message: {ex.Message}\n\n" +
                            $"Stack: {ex.StackTrace?.Substring(0, Math.Min(500, ex.StackTrace?.Length ?? 0))}",
                            "Błąd synchronizacji konta",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Error
                        );
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"\n✅ SynchronizeIssuesAsync - KONIEC: Przetworzonych={result.TotalProcessed}, Nowych={result.NewIssues}");
            }

            return result;
        }

        #endregion

        #region Synchronizacja zwrotów

        private async Task<ReturnsSyncResult> SynchronizeReturnsForAccountAsync(
            AllegroApiClient apiClient,
            int accountId,
            MySqlConnection con)
        {
            var result = new ReturnsSyncResult();
            var logId = await LogSyncStartAsync(accountId, "RETURNS", con);

            try
            {
                // Pobierz wszystkie zwroty z API
                var allReturns = await GetAllReturnsFromApiAsync(apiClient);
                result.TotalProcessed = allReturns.Count;

                foreach (var returnData in allReturns)
                {
                    try
                    {
                        // Pobierz szczegóły zamówienia
                        OrderDetails orderDetails = null;
                        if (!string.IsNullOrEmpty(returnData.OrderId))
                        {
                            orderDetails = await apiClient.GetOrderDetailsByCheckoutFormIdAsync(returnData.OrderId);
                        }

                        // Upsert zwrotu do bazy
                        bool isNew = await UpsertReturnAsync(returnData, orderDetails, accountId, con);

                        if (isNew)
                        {
                            result.NewReturns++;
                        }
                        else
                        {
                            result.UpdatedReturns++;
                        }

                        // Zapisz produkty zwrotu (jeśli jest więcej niż 1)
                        if (returnData.Items != null && returnData.Items.Count > 1)
                        {
                            await SaveReturnItemsAsync(returnData, con);
                        }
                    }
                    catch (Exception exReturn)
                    {
                        result.ErrorMessages.Add($"Zwrot {returnData.Id}: {exReturn.Message}");
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
                        break; // Ostatnia strona
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
            // Sprawdź czy zwrot już istnieje
            var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM AllegroCustomerReturns WHERE AllegroReturnId = @ReturnId", con);
            checkCmd.Parameters.AddWithValue("@ReturnId", returnData.Id);
            long count = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());
            bool isNew = (count == 0);

            // Przygotuj dane
            var firstItem = returnData.Items?.FirstOrDefault();
            var firstParcel = returnData.Parcels?.FirstOrDefault();

            string sql = isNew ? GetInsertReturnSql() : GetUpdateReturnSql();

            using (var cmd = new MySqlCommand(sql, con))
            {
                // Podstawowe dane zwrotu
                cmd.Parameters.AddWithValue("@AllegroReturnId", returnData.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AllegroAccountId", accountId);
                cmd.Parameters.AddWithValue("@ReferenceNumber", returnData.ReferenceNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OrderId", returnData.OrderId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerLogin", returnData.Buyer?.Login ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerEmail", (object)DBNull.Value); // Brak w API zwrotów
                cmd.Parameters.AddWithValue("@CreatedAt", returnData.CreatedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StatusAllegro", returnData.Status ?? (object)DBNull.Value);

                // Dane przesyłki
                cmd.Parameters.AddWithValue("@Waybill", firstParcel?.Waybill ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TransportingWaybill", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CarrierName", firstParcel?.CarrierId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TransportingCarrierId", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SenderPhoneNumber", (object)DBNull.Value);

                // Dane produktu (pierwszy item)
                cmd.Parameters.AddWithValue("@ProductName", firstItem?.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OfferId", firstItem?.OfferId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Quantity", firstItem?.Quantity ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductPrice", (object)DBNull.Value); // Brak w API zwrotów
                cmd.Parameters.AddWithValue("@ProductPriceCurrency", "PLN");

                // Powód zwrotu
                cmd.Parameters.AddWithValue("@ReturnReasonType", firstItem?.Reason?.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReturnReasonComment", firstItem?.Reason?.UserComment ?? (object)DBNull.Value);

                // Dane z zamówienia (jeśli dostępne)
                if (orderDetails != null)
                {
                    cmd.Parameters.AddWithValue("@PaymentType", orderDetails.Payment?.Type ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentProvider", orderDetails.Payment?.Provider ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentFinishedAt", orderDetails.Payment?.FinishedAt ?? (object)DBNull.Value);
                    
                    // ⭐ NAPRAWIONO: Bezpieczne parsowanie kwoty PaidAmount
                    decimal? paidAmount = null;
                    if (orderDetails.Payment?.PaidAmount?.Amount != null)
                    {
                        paidAmount = SafeParseDecimal(orderDetails.Payment.PaidAmount.Amount, returnData.Id);
                    }
                    cmd.Parameters.AddWithValue("@PaidAmount", paidAmount ?? (object)DBNull.Value);
                    
                    cmd.Parameters.AddWithValue("@FulfillmentStatus", orderDetails.Fulfillment?.Status ?? (object)DBNull.Value);

                    // Adres dostawy
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

                    // Adres kupującego
                    var buyerAddr = orderDetails.Buyer?.Address;
                    cmd.Parameters.AddWithValue("@Buyer_FirstName", orderDetails.Buyer?.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_LastName", orderDetails.Buyer?.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_Street", buyerAddr?.Street ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_ZipCode", buyerAddr?.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_City", buyerAddr?.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_PhoneNumber", orderDetails.Buyer?.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Buyer_CountryCode", buyerAddr?.CountryCode ?? "PL");
                    cmd.Parameters.AddWithValue("@Buyer_CompanyName", buyerAddr?.CompanyName ?? (object)DBNull.Value);

                    // Faktura
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

                    // Dodatkowe dane
                    cmd.Parameters.AddWithValue("@MarketplaceId", orderDetails.Marketplace?.Id ?? "allegro-pl");
                    cmd.Parameters.AddWithValue("@BoughtAt", orderDetails.BoughtAt ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderJsonDetails", JsonConvert.SerializeObject(orderDetails));
                }
                else
                {
                    // Brak danych z zamówienia - ustaw NULL
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

                // JSON backup
                cmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(returnData));
                cmd.Parameters.AddWithValue("@LastSyncAt", DateTime.Now);

                await cmd.ExecuteNonQueryAsync();
            }

            return isNew;
        }

        private async Task SaveReturnItemsAsync(AllegroCustomerReturn returnData, MySqlConnection con)
        {
            // Pobierz ID zwrotu z bazy
            var getIdCmd = new MySqlCommand(
                "SELECT Id FROM AllegroCustomerReturns WHERE AllegroReturnId = @ReturnId LIMIT 1", con);
            getIdCmd.Parameters.AddWithValue("@ReturnId", returnData.Id);
            var returnId = Convert.ToInt32(await getIdCmd.ExecuteScalarAsync());

            // Usuń stare produkty (jeśli istnieją)
            var deleteCmd = new MySqlCommand(
                "DELETE FROM AllegroReturnItems WHERE ReturnId = @ReturnId", con);
            deleteCmd.Parameters.AddWithValue("@ReturnId", returnId);
            await deleteCmd.ExecuteNonQueryAsync();

            // Dodaj nowe produkty
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
                insertCmd.Parameters.AddWithValue("@Price", DBNull.Value); // Brak w API zwrotów
                insertCmd.Parameters.AddWithValue("@Currency", "PLN");
                insertCmd.Parameters.AddWithValue("@ReasonType", item.Reason?.Type ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ReasonComment", item.Reason?.UserComment ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ProductUrl", DBNull.Value); // Brak w API
                insertCmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(item));

                await insertCmd.ExecuteNonQueryAsync();
            }
        }

        #endregion

        // ═══════════════════════════════════════════════════════════════════════════════
        // ALLEGRO SYNC V3.0 ULTIMATE - METODY DO DODANIA/ZASTĄPIENIA
        // ═══════════════════════════════════════════════════════════════════════════════
        // INSTRUKCJA:
        // 1. Otwórz AllegroSyncServiceExtended.cs
        // 2. Znajdź region "Synchronizacja dyskusji i reklamacji"
        // 3. ZASTĄP wszystkie metody w tym regionie poniższym kodem
        // 4. Zapisz (Ctrl+S) i kompiluj (Ctrl+Shift+B)
        // ═══════════════════════════════════════════════════════════════════════════════

        #region Synchronizacja dyskusji i reklamacji - OPTIMIZED v3.0 ULTIMATE

        /// <summary>
        /// OPTYMALIZACJA v3.0 ULTIMATE - 3 WARSTWY:
        /// 1. Sync Log Check (skip jeśli sync < 5 min temu)
        /// 2. First Issue Check (porównanie ID pierwszej issue z ostatnią w bazie)
        /// 3. Last Message ID Check (skip czatów gdy brak zmian)
        /// </summary>
        private async Task<IssuesSyncResult> SynchronizeIssuesForAccountAsync_Optimized(
            AllegroApiClient apiClient,
            int accountId,
            MySqlConnection con,
            IProgress<string> progress = null)
        {
            var result = new IssuesSyncResult();
            var logId = await LogSyncStartAsync(accountId, "ISSUES", con);

            try
            {
                // ═══════════════════════════════════════════════════════
                // WARSTWA 1: SYNC LOG CHECK
                // ═══════════════════════════════════════════════════════
                var lastSyncTime = await GetLastSuccessfulSyncTimeAsync(accountId, con);

                if (lastSyncTime != null)
                {
                    var timeSinceLastSync = DateTime.Now - lastSyncTime.Value;

                    if (timeSinceLastSync.TotalMinutes < 5)
                    {
                        progress?.Report($"Konto {accountId}: Sync {timeSinceLastSync.TotalMinutes:F1} min temu - pomijam");
                        System.Diagnostics.Debug.WriteLine($"[SYNC SKIP] Konto {accountId}: Ostatnia sync {timeSinceLastSync.TotalMinutes:F1} min temu");
                        await LogSyncCompleteAsync(logId, "SKIPPED", 0, 0, 0, con, "Too recent sync");
                        return result;
                    }
                }

                progress?.Report($"Konto {accountId}: Sprawdzanie...");

                // ═══════════════════════════════════════════════════════
                // WARSTWA 2: FIRST ISSUE CHECK (1 API call!)
                // ═══════════════════════════════════════════════════════
                System.Diagnostics.Debug.WriteLine($"[SYNC] Konto {accountId}: Pobieranie issues z API...");

                var allIssuesFromApi = await apiClient.GetIssuesAsync();

                if (allIssuesFromApi == null || !allIssuesFromApi.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[SYNC] Konto {accountId}: Brak issues w API");
                    await LogSyncCompleteAsync(logId, "SUCCESS", 0, 0, 0, con);
                    return result;
                }

                // API zwraca posortowane wg daty - PIERWSZA issue = NAJNOWSZA!
                var firstIssueFromApi = allIssuesFromApi.First();
                var lastIssueInDb = await GetLastIssueIdFromDbAsync(accountId, con);

                System.Diagnostics.Debug.WriteLine($"[SYNC COMPARE] API first: {firstIssueFromApi.Id}, DB last: {lastIssueInDb}");

                if (firstIssueFromApi.Id == lastIssueInDb)
                {
                    // Issues są aktualne - synchronizujemy TYLKO czaty!
                    progress?.Report($"Konto {accountId}: Issues OK - sprawdzam czaty...");
                    System.Diagnostics.Debug.WriteLine($"[SYNC QUICK] Issues aktualne - tylko czaty");

                    result.IssuesWithNewMessages = await SynchronizeChatsOnlyAsync(apiClient, accountId, con, progress);

                    progress?.Report($"Konto {accountId}: ✅ OK! (Issues: 0, Czaty: {result.IssuesWithNewMessages})");
                    await LogSyncCompleteAsync(logId, "SUCCESS", 0, 0, result.IssuesWithNewMessages, con);
                    return result;
                }

                // ═══════════════════════════════════════════════════════
                // WARSTWA 2B: PEŁNA SYNCHRONIZACJA ISSUES (są nowe!)
                // ═══════════════════════════════════════════════════════
                progress?.Report($"Konto {accountId}: Nowe issues - synchronizuję...");
                System.Diagnostics.Debug.WriteLine($"[SYNC FULL] Znaleziono nowe issues - pełna sync");

                result.TotalProcessed = allIssuesFromApi.Count;

                int current = 0;
                foreach (var issueShort in allIssuesFromApi)
                {
                    current++;
                    if (current % 10 == 0 || current == allIssuesFromApi.Count)
                    {
                        progress?.Report($"Konto {accountId}: Issues {current}/{allIssuesFromApi.Count}...");
                    }

                    await ProcessSingleIssueAsync(apiClient, issueShort, accountId, con, result);
                }

                System.Diagnostics.Debug.WriteLine($"[SYNC] Issues: {result.TotalProcessed} (Nowych: {result.NewIssues})");

                // ═══════════════════════════════════════════════════════
                // WARSTWA 3: LAST MESSAGE ID CHECK (per issue)
                // ═══════════════════════════════════════════════════════
                progress?.Report($"Konto {accountId}: Czaty...");

                result.IssuesWithNewMessages = await SynchronizeChatsOnlyAsync(apiClient, accountId, con, progress);

                System.Diagnostics.Debug.WriteLine($"[SYNC] Czaty: {result.IssuesWithNewMessages} z nowymi wiadomościami");

                progress?.Report($"Konto {accountId}: ✅ OK! (Nowych: {result.NewIssues}, Czaty: {result.IssuesWithNewMessages})");
                await LogSyncCompleteAsync(logId, "SUCCESS", result.TotalProcessed, result.NewIssues, result.IssuesWithNewMessages, con);
            }
            catch (Exception ex)
            {
                progress?.Report($"Konto {accountId}: ❌ BŁĄD!");
                System.Diagnostics.Debug.WriteLine($"[ERROR] Konto {accountId}: {ex.Message}");
                await LogSyncCompleteAsync(logId, "FAILED", result.TotalProcessed, result.NewIssues, result.IssuesWithNewMessages, con, ex.Message);
                throw;
            }

            return result;
        }

        /// <summary>
        /// WARSTWA 1 HELPER: Pobiera czas ostatniej udanej synchronizacji
        /// </summary>
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

            return null; // Pierwsza synchronizacja
        }

        /// <summary>
        /// WARSTWA 2 HELPER: Pobiera ID ostatniej issue z bazy (najnowszej wg LastCheckedAt)
        /// </summary>
        private async Task<string> GetLastIssueIdFromDbAsync(int accountId, MySqlConnection con)
        {
            var cmd = new MySqlCommand(@"
                SELECT DisputeId 
                FROM AllegroDisputes 
                WHERE AllegroAccountId = @AccountId 
                ORDER BY LastCheckedAt DESC 
                LIMIT 1", con);

            cmd.Parameters.AddWithValue("@AccountId", accountId);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString();
        }

        /// <summary>
        /// WARSTWA 3: Synchronizuje TYLKO czaty (bez issues)
        /// </summary>
        private async Task<int> SynchronizeChatsOnlyAsync(
            AllegroApiClient apiClient,
            int accountId,
            MySqlConnection con,
            IProgress<string> progress = null)
        {
            int issuesWithNewMessages = 0;

            // Pobierz wszystkie issues z bazy
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

                    bool hasNewMessages = await SynchronizeChatForIssueAsync_Optimized(apiClient, issue, con);
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

        /// <summary>
        /// Przetwarza pojedynczy issue (bez czatu)
        /// </summary>
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

                string buyerEmail = null;
                if (!string.IsNullOrEmpty(issue.CheckoutForm?.Id))
                {
                    buyerEmail = await GetBuyerEmailAsync(apiClient, issue.CheckoutForm.Id);
                }

                bool isNew = await UpsertIssueAsync(issue, orderDetails, buyerEmail, accountId, con);
                if (isNew) result.NewIssues++;
            }
            catch (Exception ex)
            {
                result.ErrorMessages.Add($"Issue {issueShort.Id}: {ex.Message}");
            }
        }

        /// <summary>
        /// Pobiera BuyerEmail
        /// </summary>
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

        /// <summary>
        /// WARSTWA 3: Inteligentna synchronizacja czatu z LastMessageId check
        /// </summary>
        private async Task<bool> SynchronizeChatForIssueAsync_Optimized(
            AllegroApiClient apiClient,
            Issue issue,
            MySqlConnection con)
        {
            try
            {
                // Pobierz wiadomości z API (od najnowszej!)
                var messagesFromApi = await apiClient.GetChatAsync(issue.Id);

                if (messagesFromApi == null || !messagesFromApi.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[CHAT] Issue {issue.Id}: Brak wiadomości");
                    return false;
                }

                // Pierwsza wiadomość = NAJNOWSZA
                var latestMessage = messagesFromApi.First();
                string latestMessageId = latestMessage.Id;

                // Pobierz LastMessageId z bazy
                string lastMessageIdInDb = null;
                using (var cmd = new MySqlCommand(
                    "SELECT LastMessageId FROM AllegroDisputes WHERE DisputeId = @DisputeId", con))
                {
                    cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                    var result = await cmd.ExecuteScalarAsync();
                    lastMessageIdInDb = result?.ToString();
                }

                // QUICK CHECK: Czy są nowe wiadomości?
                if (latestMessageId == lastMessageIdInDb)
                {
                    System.Diagnostics.Debug.WriteLine($"[CHAT SKIP] Issue {issue.Id}: Brak nowych wiadomości");
                    return false;
                }

                // SYNC: Są nowe wiadomości!
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

                        // Aktualizuj LastMessageId
                        var updateCmd = new MySqlCommand(@"
                            UPDATE AllegroDisputes 
                            SET 
                                LastMessageCount = @Count, 
                                LastMessageId = @LastMessageId,
                                HasNewMessages = 1
                            WHERE DisputeId = @DisputeId",
                            con, transaction);
                        updateCmd.Parameters.AddWithValue("@Count", messagesFromApi.Count);
                        updateCmd.Parameters.AddWithValue("@LastMessageId", latestMessageId);
                        updateCmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                        await updateCmd.ExecuteNonQueryAsync();

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CHAT ERROR] Issue {issue.Id}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Dodaje wiadomość do CentrumKontaktu
        /// </summary>
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
                 OpenedAt, DecisionDueDate, ClosedAt, LastCheckedAt, OrderId, BuyerLogin, BuyerEmail,
                 ProductId, OfferId, ProductName, ProductEAN, ProductSKU, InvoiceNumber,
                 ExpectationType, ExpectationRefundAmount, ExpectationRefundCurrency,
                 ReasonType, ReasonDescription, BoughtAt, NeedsDecision, LastMessageCount, HasNewMessages,
                 ComplaintId, JsonDetails, OrderJsonDetails)
                VALUES 
                (@DisputeId, @AllegroAccountId, @Type, @ReferenceNumber, @Subject, @Description, @StatusAllegro,
                 @OpenedAt, @DecisionDueDate, @ClosedAt, @LastCheckedAt, @OrderId, @BuyerLogin, @BuyerEmail,
                 @ProductId, @OfferId, @ProductName, @ProductEAN, @ProductSKU, @InvoiceNumber,
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
                    OrderId = @OrderId,
                    BuyerLogin = @BuyerLogin,
                    BuyerEmail = @BuyerEmail,
                    ProductId = @ProductId,
                    OfferId = @OfferId,
                    ProductName = @ProductName,
                    ProductEAN = @ProductEAN,
                    ProductSKU = @ProductSKU,
                    InvoiceNumber = @InvoiceNumber,
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
