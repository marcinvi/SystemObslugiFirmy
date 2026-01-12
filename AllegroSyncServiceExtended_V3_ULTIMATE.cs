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
