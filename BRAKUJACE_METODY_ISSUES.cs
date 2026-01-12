// ═══════════════════════════════════════════════════════════════════════════════
// BRAKUJĄCE METODY - DODAJ DO AllegroSyncServiceExtended.cs
// ═══════════════════════════════════════════════════════════════════════════════
// INSTRUKCJA:
// 1. Otwórz AllegroSyncServiceExtended.cs
// 2. Znajdź region "#region Synchronizacja czatu - OPTIMIZED v3.0"
// 3. ZARAZ PO TYM REGIONIE (przed #region SQL Generators) wklej poniższy kod
// ═══════════════════════════════════════════════════════════════════════════════

        #region Upsert Issue - helper methods

        /// <summary>
        /// Dodaje lub aktualizuje issue w bazie danych
        /// </summary>
        private async Task<bool> UpsertIssueAsync(
            Issue issue,
            OrderDetails orderDetails,
            string buyerEmail,
            int accountId,
            MySqlConnection con)
        {
            // Sprawdź czy issue już istnieje
            var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM AllegroDisputes WHERE DisputeId = @DisputeId", con);
            checkCmd.Parameters.AddWithValue("@DisputeId", issue.Id);
            long count = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());
            bool isNew = (count == 0);

            // Przygotuj dane
            var firstLineItem = issue.LineItems?.FirstOrDefault();
            
            string sql = isNew ? GetInsertIssueSql() : GetUpdateIssueSql();

            using (var cmd = new MySqlCommand(sql, con))
            {
                // Podstawowe dane issue
                cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
                cmd.Parameters.AddWithValue("@AllegroAccountId", accountId);
                cmd.Parameters.AddWithValue("@Type", issue.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReferenceNumber", issue.ReferenceNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Subject", issue.Subject ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", issue.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StatusAllegro", issue.Status ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OpenedAt", issue.OpenedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DecisionDueDate", issue.DecisionDueDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ClosedAt", issue.ClosedAt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastCheckedAt", DateTime.Now);

                // Dane zamówienia
                cmd.Parameters.AddWithValue("@OrderId", issue.CheckoutForm?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerLogin", issue.Buyer?.Login ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerEmail", buyerEmail ?? (object)DBNull.Value);

                // Dane produktu (pierwszy line item)
                cmd.Parameters.AddWithValue("@ProductId", firstLineItem?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OfferId", firstLineItem?.Offer?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductName", firstLineItem?.Offer?.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductEAN", (object)DBNull.Value); // Brak w API
                cmd.Parameters.AddWithValue("@ProductSKU", (object)DBNull.Value); // Brak w API
                cmd.Parameters.AddWithValue("@InvoiceNumber", (object)DBNull.Value); // TODO: z orderDetails

                // Oczekiwania kupującego
                cmd.Parameters.AddWithValue("@ExpectationType", issue.Expectation?.Type ?? (object)DBNull.Value);
                
                decimal? refundAmount = null;
                if (issue.Expectation?.RefundAmount?.Amount != null)
                {
                    refundAmount = SafeParseDecimal(issue.Expectation.RefundAmount.Amount, issue.Id);
                }
                cmd.Parameters.AddWithValue("@ExpectationRefundAmount", refundAmount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpectationRefundCurrency", 
                    issue.Expectation?.RefundAmount?.Currency ?? "PLN");

                // Powód reklamacji
                cmd.Parameters.AddWithValue("@ReasonType", issue.Reason?.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ReasonDescription", issue.Reason?.Description ?? (object)DBNull.Value);

                // Data zakupu
                DateTime? boughtAt = null;
                if (orderDetails?.BoughtAt != null)
                {
                    boughtAt = orderDetails.BoughtAt;
                }
                cmd.Parameters.AddWithValue("@BoughtAt", boughtAt ?? (object)DBNull.Value);

                // Czy wymaga decyzji?
                bool needsDecision = (issue.Status == "IN_PROGRESS" || issue.Status == "OPENED") 
                                     && issue.DecisionDueDate.HasValue 
                                     && issue.DecisionDueDate.Value > DateTime.Now;
                cmd.Parameters.AddWithValue("@NeedsDecision", needsDecision ? 1 : 0);

                // ComplaintId - próbuj znaleźć powiązane zgłoszenie
                int? complaintId = await FindRelatedComplaintAsync(issue, orderDetails, con);
                cmd.Parameters.AddWithValue("@ComplaintId", complaintId ?? (object)DBNull.Value);

                // JSON backup
                cmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(issue));
                cmd.Parameters.AddWithValue("@OrderJsonDetails", 
                    orderDetails != null ? JsonConvert.SerializeObject(orderDetails) : (object)DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
            }

            return isNew;
        }

        /// <summary>
        /// Próbuje znaleźć powiązane zgłoszenie w bazie danych
        /// </summary>
        private async Task<int?> FindRelatedComplaintAsync(
            Issue issue,
            OrderDetails orderDetails,
            MySqlConnection con)
        {
            // Strategia 1: Szukaj po emailu kupującego
            if (orderDetails?.Buyer?.Email != null)
            {
                var cmd = new MySqlCommand(@"
                    SELECT NrZgloszenia 
                    FROM Zgloszenia 
                    WHERE Email = @Email 
                    ORDER BY DataZgloszenia DESC 
                    LIMIT 1", con);
                cmd.Parameters.AddWithValue("@Email", orderDetails.Buyer.Email);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            // Strategia 2: Szukaj po numerze telefonu
            if (orderDetails?.Buyer?.PhoneNumber != null)
            {
                var cmd = new MySqlCommand(@"
                    SELECT NrZgloszenia 
                    FROM Zgloszenia 
                    WHERE NrTelefonu = @Phone 
                    ORDER BY DataZgloszenia DESC 
                    LIMIT 1", con);
                cmd.Parameters.AddWithValue("@Phone", orderDetails.Buyer.PhoneNumber);
                var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            // Nie znaleziono - zwróć NULL
            return null;
        }

        #endregion
