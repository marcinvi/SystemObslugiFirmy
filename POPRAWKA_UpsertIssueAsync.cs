// ═══════════════════════════════════════════════════════════════════════════════
// POPRAWIONY KOD - ZASTĄP REGION "Upsert Issue - helper methods"
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
                
                // ⭐ POPRAWKA: CurrentState.Status zamiast Status
                cmd.Parameters.AddWithValue("@StatusAllegro", issue.CurrentState?.Status ?? (object)DBNull.Value);
                
                // ⭐ POPRAWKA: OpenedDate zamiast OpenedAt
                cmd.Parameters.AddWithValue("@OpenedAt", issue.OpenedDate);
                cmd.Parameters.AddWithValue("@DecisionDueDate", issue.DecisionDueDate ?? (object)DBNull.Value);
                
                // ⭐ POPRAWKA: Brak ClosedAt w modelu - zawsze NULL
                cmd.Parameters.AddWithValue("@ClosedAt", DBNull.Value);
                cmd.Parameters.AddWithValue("@LastCheckedAt", DateTime.Now);

                // Dane zamówienia
                cmd.Parameters.AddWithValue("@OrderId", issue.CheckoutForm?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerLogin", issue.Buyer?.Login ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerEmail", buyerEmail ?? (object)DBNull.Value);

                // ⭐ POPRAWKA: Dane produktu z Product i Offer (nie LineItems)
                cmd.Parameters.AddWithValue("@ProductId", issue.Product?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@OfferId", issue.Offer?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductName", issue.Offer?.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductEAN", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductSKU", (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceNumber", (object)DBNull.Value);

                // ⭐ POPRAWKA: Expectations to LISTA (bierzemy pierwszy)
                var firstExpectation = issue.Expectations?.FirstOrDefault();
                cmd.Parameters.AddWithValue("@ExpectationType", firstExpectation?.Name ?? (object)DBNull.Value);
                
                decimal? refundAmount = null;
                if (firstExpectation?.Refund?.Amount != null)
                {
                    refundAmount = SafeParseDecimal(firstExpectation.Refund.Amount, issue.Id);
                }
                cmd.Parameters.AddWithValue("@ExpectationRefundAmount", refundAmount ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpectationRefundCurrency", 
                    firstExpectation?.Refund?.Currency ?? "PLN");

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
                string status = issue.CurrentState?.Status ?? "";
                bool needsDecision = (status == "IN_PROGRESS" || status == "OPENED") 
                                     && issue.DecisionDueDate.HasValue 
                                     && issue.DecisionDueDate.Value > DateTime.Now;
                cmd.Parameters.AddWithValue("@NeedsDecision", needsDecision ? 1 : 0);

                // ComplaintId
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

            // Nie znaleziono
            return null;
        }

        #endregion
