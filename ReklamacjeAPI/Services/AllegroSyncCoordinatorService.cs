// =============================================================================
// AllegroSyncCoordinatorService.cs — WERSJA 4.0 SMART SYNC
// =============================================================================
// NAPRAWY:
// ✅ Używa GetIssuesFullPageAsync zamiast GetIssuesAsync (pełne metadane czatu)
// ✅ Porównuje MessagesCount per issue zamiast COUNT(*) issues
// ✅ Pobiera czat TYLKO gdy MessagesCount > LastMessageCount w bazie
// ✅ Pobiera OrderDetails tylko dla NOWYCH issues
// ✅ Wypełnia WSZYSTKIE kolumny w AllegroDisputes (BuyerFirstName, Delivery, etc.)
// ✅ Populuje tabele allegroorderdetails i allegroorderitems
// ✅ Sync attachmentów do AllegroChatAttachments
// ✅ Sync do CentrumKontaktu (czat → historia kontaktu)
// =============================================================================

using MySqlConnector;
using ReklamacjeAPI.DTOs;
using System.Globalization;
using System.Text.Json;

namespace ReklamacjeAPI.Services;

public class AllegroSyncCoordinatorService
{
    private readonly AllegroCredentialsService _credentialsService;
    private readonly AllegroApiClient _allegroApiClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AllegroSyncCoordinatorService> _logger;

    private static readonly SemaphoreSlim _gate = new(1, 1);
    private static AllegroSyncStatusDto _status = new();

    public AllegroSyncCoordinatorService(
        AllegroCredentialsService credentialsService,
        AllegroApiClient allegroApiClient,
        IConfiguration configuration,
        ILogger<AllegroSyncCoordinatorService> logger)
    {
        _credentialsService = credentialsService;
        _allegroApiClient = allegroApiClient;
        _configuration = configuration;
        _logger = logger;
    }

    public AllegroSyncStatusDto GetStatusSnapshot()
    {
        return new AllegroSyncStatusDto
        {
            IsRunning = _status.IsRunning,
            LastStartedAt = _status.LastStartedAt,
            LastCompletedAt = _status.LastCompletedAt,
            LastRunSuccess = _status.LastRunSuccess,
            LastError = _status.LastError,
            NewDisputesFoundLastRun = _status.NewDisputesFoundLastRun,
            UpdatedDisputesLastRun = _status.UpdatedDisputesLastRun,
            ChatsSyncedLastRun = _status.ChatsSyncedLastRun,
            UnregisteredDisputesCount = _status.UnregisteredDisputesCount,
            DisputesWithNewMessages = _status.DisputesWithNewMessages
        };
    }

    // =========================================================================
    // GŁÓWNA METODA SYNCHRONIZACJI
    // =========================================================================
    public async Task<AllegroSyncRunResultDto> TriggerSyncAsync(string source)
    {
        if (!await _gate.WaitAsync(0))
        {
            return new AllegroSyncRunResultDto
            {
                Success = true,
                Message = "Synchronizacja Allegro jest już uruchomiona.",
                Status = GetStatusSnapshot()
            };
        }

        try
        {
            _status.IsRunning = true;
            _status.LastStartedAt = DateTime.Now;
            _status.LastError = null;

            _logger.LogInformation("[Allegro][Sync v4.0] Start. Source={Source}", source);

            var accounts = await _credentialsService.GetAuthorizedAccountsAsync();
            int totalNew = 0;
            int totalUpdated = 0;
            int totalChatsSynced = 0;

            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            foreach (var account in accounts)
            {
                try
                {
                    var (newCount, updatedCount, chatsSynced) =
                        await SynchronizeIssuesForAccountAsync(conn, account.Id);

                    totalNew += newCount;
                    totalUpdated += updatedCount;
                    totalChatsSynced += chatsSynced;

                    _logger.LogInformation(
                        "[Allegro][Sync] Konto {Id}: nowe={New}, updated={Upd}, czaty={Chats}",
                        account.Id, newCount, updatedCount, chatsSynced);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Allegro][Sync] Błąd konta {Id}", account.Id);
                }
            }

            _status.NewDisputesFoundLastRun = totalNew;
            _status.UnregisteredDisputesCount = await GetCountAsync(conn,
                "SELECT COUNT(*) FROM AllegroDisputes WHERE ComplaintId IS NULL");
            _status.DisputesWithNewMessages = await GetCountAsync(conn,
                "SELECT COUNT(*) FROM AllegroDisputes WHERE IFNULL(HasNewMessages,0)=1");
            _status.LastRunSuccess = true;
            _status.LastCompletedAt = DateTime.Now;

            await WriteSyncRunAsync(conn, "ALLEGRO", _status.LastStartedAt ?? DateTime.Now, true,
                totalNew + totalUpdated,
                $"new={totalNew}, updated={totalUpdated}, chats={totalChatsSynced}");

            _logger.LogInformation(
                "[Allegro][Sync v4.0] Koniec. New={New}, Updated={Upd}, Chats={Chats}, Unregistered={Unreg}, Unread={Unread}",
                totalNew, totalUpdated, totalChatsSynced,
                _status.UnregisteredDisputesCount, _status.DisputesWithNewMessages);

            return new AllegroSyncRunResultDto
            {
                Success = true,
                Message = $"OK: nowe={totalNew}, zaktualizowane={totalUpdated}, czaty={totalChatsSynced}",
                Status = GetStatusSnapshot()
            };
        }
        catch (Exception ex)
        {
            _status.LastRunSuccess = false;
            _status.LastError = ex.Message;
            _status.LastCompletedAt = DateTime.Now;
            _logger.LogError(ex, "[Allegro][Sync v4.0] Krytyczny błąd");

            try
            {
                await using var errConn = DbConnectionFactory.CreateDefaultConnection(_configuration);
                await errConn.OpenAsync();
                await WriteSyncRunAsync(errConn, "ALLEGRO", _status.LastStartedAt ?? DateTime.Now, false, 0, ex.Message);
            }
            catch { /* best-effort */ }

            return new AllegroSyncRunResultDto
            {
                Success = false,
                Message = $"Błąd: {ex.Message}",
                Status = GetStatusSnapshot()
            };
        }
        finally
        {
            _status.IsRunning = false;
            _gate.Release();
        }
    }

    // =========================================================================
    // SYNCHRONIZACJA ISSUES DLA JEDNEGO KONTA — SMART SYNC
    // =========================================================================
    private async Task<(int newCount, int updatedCount, int chatsSynced)>
        SynchronizeIssuesForAccountAsync(MySqlConnection conn, int accountId)
    {
        int newCount = 0, updatedCount = 0, chatsSynced = 0;

        // ─────────────────────────────────────────────────────────────────
        // KROK 1: Pobierz WSZYSTKIE issues z API (stronicowane, po 100)
        //         GetIssuesFullPageAsync zwraca pełne metadane czatu!
        // ─────────────────────────────────────────────────────────────────
        var allIssues = new List<AllegroApiClient.AllegroIssueFullDto>();
        int offset = 0;
        const int pageSize = 100;

        while (true)
        {
            var page = await _allegroApiClient.GetIssuesFullPageAsync(accountId, pageSize, offset);

            if (page.Issues.Count == 0)
                break;

            allIssues.AddRange(page.Issues);

            if (page.Issues.Count < pageSize)
                break;

            offset += pageSize;
        }

        if (allIssues.Count == 0)
        {
            _logger.LogDebug("[Allegro][Sync] Konto {Id}: brak issues w API", accountId);
            return (0, 0, 0);
        }

        _logger.LogDebug("[Allegro][Sync] Konto {Id}: pobrano {Count} issues z API", accountId, allIssues.Count);

        // ─────────────────────────────────────────────────────────────────
        // KROK 2: Pobierz stan issues z bazy (DisputeId → LastMessageCount)
        //         Jeden SELECT zamiast N osobnych zapytań
        // ─────────────────────────────────────────────────────────────────
        var dbState = await GetIssuesDbStateAsync(conn, accountId);

        // ─────────────────────────────────────────────────────────────────
        // KROK 3: Dla KAŻDEGO issue — zdecyduj co zrobić
        // ─────────────────────────────────────────────────────────────────
        foreach (var issue in allIssues)
        {
            if (string.IsNullOrWhiteSpace(issue.Id))
                continue;

            try
            {
                await EnsureConnectionOpenAsync(conn);

                if (!dbState.TryGetValue(issue.Id, out var existingState))
                {
                    // ── NOWY ISSUE ──────────────────────────────────
                    // Potrzebujemy: OrderDetails + Chat messages
                    AllegroApiClient.AllegroFullOrderDetailsDto? orderDetails = null;

                    if (!string.IsNullOrWhiteSpace(issue.CheckoutFormId))
                    {
                        try
                        {
                            orderDetails = await _allegroApiClient.GetFullOrderDetailsAsync(
                                accountId, issue.CheckoutFormId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "[Allegro][Sync] Nie udało się pobrać OrderDetails dla {OrderId}",
                                issue.CheckoutFormId);
                        }
                    }

                    // Pobierz wiadomości czatu
                    List<AllegroApiClient.AllegroIssueChatMessageDto>? chatMessages = null;
                    if (issue.ChatMessagesCount > 0)
                    {
                        try
                        {
                            chatMessages = await _allegroApiClient.GetIssueChatMessagesAsync(accountId, issue.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "[Allegro][Sync] Nie udało się pobrać czatu dla nowego issue {Id}",
                                issue.Id);
                        }
                    }

                    await InsertFullDisputeAsync(conn, accountId, issue, orderDetails);

                    // Zapisz order do dedykowanych tabel
                    if (orderDetails != null && !string.IsNullOrWhiteSpace(issue.CheckoutFormId))
                    {
                        await UpsertOrderDetailsAsync(conn, accountId, issue.CheckoutFormId, orderDetails);
                        await UpsertOrderItemsAsync(conn, issue.CheckoutFormId, orderDetails);
                    }

                    // Zapisz wiadomości czatu
                    if (chatMessages != null && chatMessages.Count > 0)
                    {
                        var insertedMessages = await SaveChatMessagesAsync(conn, issue.Id, chatMessages);
                        await UpdateLastMessageCountAsync(conn, issue.Id, chatMessages.Count, insertedMessages > 0);
                        chatsSynced++;
                    }

                    newCount++;
                }
                else
                {
                    // ── ISTNIEJĄCY ISSUE ────────────────────────────
                    bool requiresBackfill = await RequiresIssueBackfillAsync(conn, issue.Id);

                    // 1. UPDATE statusu i metadanych z danych listy
                    await UpdateDisputeFromListDataAsync(conn, issue);

                    // 1b. Jeśli rekord ma puste kluczowe kolumny, dociągnij orderDetails i zrób pełny backfill
                    if (requiresBackfill && !string.IsNullOrWhiteSpace(issue.CheckoutFormId))
                    {
                        try
                        {
                            var orderDetails = await _allegroApiClient.GetFullOrderDetailsAsync(accountId, issue.CheckoutFormId);
                            if (orderDetails != null)
                            {
                                await BackfillDisputeFromOrderAsync(conn, issue, orderDetails);
                                await UpsertOrderDetailsAsync(conn, accountId, issue.CheckoutFormId, orderDetails);
                                await UpsertOrderItemsAsync(conn, issue.CheckoutFormId, orderDetails);
                                _logger.LogInformation("[Allegro][Sync][BACKFILL] Uzupełniono puste pola relacyjne dla issue {Id}", issue.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "[Allegro][Sync][BACKFILL] Nie udało się pobrać/zapisać OrderDetails dla issue {Id}", issue.Id);
                        }
                    }

                    // 2. Sprawdź czy są nowe wiadomości (smart check po MessagesCount)
                    int lastMessageCountInDb = existingState.LastMessageCount;
                    int messagesCountInApi = issue.ChatMessagesCount;

                    if (messagesCountInApi > lastMessageCountInDb)
                    {
                        // NOWE WIADOMOŚCI! Pobierz TYLKO czat tego jednego issue
                        _logger.LogInformation(
                            "[Allegro][Sync] Issue {Id}: nowe wiadomości ({Api} > {Db})",
                            issue.Id, messagesCountInApi, lastMessageCountInDb);

                        try
                        {
                            var chatMessages = await _allegroApiClient.GetIssueChatMessagesAsync(
                                accountId, issue.Id);

                            if (chatMessages != null && chatMessages.Count > 0)
                            {
                                var insertedMessages = await SaveChatMessagesAsync(conn, issue.Id, chatMessages);
                                await UpdateLastMessageCountAsync(conn, issue.Id, chatMessages.Count, insertedMessages > 0);
                                chatsSynced++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex,
                                "[Allegro][Sync] Nie udało się pobrać czatu dla issue {Id}", issue.Id);
                        }
                    }
                    // else: MessagesCount == LastMessageCount → nic nie rób z czatem

                    updatedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Allegro][Sync] Błąd przetwarzania issue {Id}", issue.Id);

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    try
                    {
                        await EnsureConnectionOpenAsync(conn);
                        _logger.LogWarning("[Allegro][Sync] Połączenie DB zostało ponownie otwarte po błędzie issue {Id}", issue.Id);
                    }
                    catch (Exception reconnectEx)
                    {
                        _logger.LogError(reconnectEx,
                            "[Allegro][Sync] Nie udało się ponownie otworzyć połączenia DB po błędzie issue {Id}", issue.Id);
                    }
                }
            }
        }

        return (newCount, updatedCount, chatsSynced);
    }

    // =========================================================================
    // ODCZYT STANU Z BAZY — jeden SELECT dla całego konta
    // =========================================================================
    private static async Task<Dictionary<string, IssueDbState>> GetIssuesDbStateAsync(
        MySqlConnection conn, int accountId)
    {
        var result = new Dictionary<string, IssueDbState>(StringComparer.OrdinalIgnoreCase);

        const string sql = @"
            SELECT DisputeId, 
                   IFNULL(LastMessageCount, 0) AS LastMessageCount,
                   StatusAllegro
            FROM AllegroDisputes
            WHERE AllegroAccountId = @AccountId";

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@AccountId", accountId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var disputeId = reader["DisputeId"]?.ToString();
            if (string.IsNullOrWhiteSpace(disputeId)) continue;

            result[disputeId] = new IssueDbState
            {
                LastMessageCount = Convert.ToInt32(reader["LastMessageCount"]),
                StatusAllegro = reader["StatusAllegro"]?.ToString()
            };
        }

        return result;
    }

    private sealed class IssueDbState
    {
        public int LastMessageCount { get; set; }
        public string? StatusAllegro { get; set; }
    }

    // =========================================================================
    // INSERT NOWEGO ISSUE — z pełnymi danymi
    // =========================================================================
    private static async Task InsertFullDisputeAsync(
        MySqlConnection conn,
        int accountId,
        AllegroApiClient.AllegroIssueFullDto issue,
        AllegroApiClient.AllegroFullOrderDetailsDto? order)
    {
        const string sql = @"
INSERT INTO AllegroDisputes (
    DisputeId, AllegroAccountId, Type, ReferenceNumber, Subject, Description,
    StatusAllegro, OpenedAt, DecisionDueDate, ClosedAt, LastCheckedAt, CreatedAt, Status,
    OrderId, BuyerLogin, BuyerEmail,
    BuyerFirstName, BuyerLastName,
    DeliveryStreet, DeliveryZipCode, DeliveryCity, DeliveryPhoneNumber, DeliveryCompanyName, DeliveryCompany,
    ProductId, OfferId, ProductName, ProductEAN, ProductSKU,
    InvoiceNumber,
    Expectations, InitialMessageText, InitialMessageCount,
    ExpectationType, ExpectationRefundAmount, ExpectationRefundCurrency,
    ReasonType, ReasonDescription,
    BoughtAt, NeedsDecision,
    LastMessageCount, HasNewMessages,
    ComplaintId,
    JsonDetails, OrderJsonDetails
) VALUES (
    @DisputeId, @AccountId, @Type, @ReferenceNumber, @Subject, @Description,
    @StatusAllegro, @OpenedAt, @DecisionDueDate, @ClosedAt, NOW(), @CreatedAt, @Status,
    @OrderId, @BuyerLogin, @BuyerEmail,
    @BuyerFirstName, @BuyerLastName,
    @DeliveryStreet, @DeliveryZipCode, @DeliveryCity, @DeliveryPhoneNumber, @DeliveryCompanyName, @DeliveryCompany,
    @ProductId, @OfferId, @ProductName, @ProductEAN, @ProductSKU,
    @InvoiceNumber,
    @Expectations, @InitialMessageText, @InitialMessageCount,
    @ExpectationType, @ExpectationRefundAmount, @ExpectationRefundCurrency,
    @ReasonType, @ReasonDescription,
    @BoughtAt, @NeedsDecision,
    0, 0,
    NULL,
    @JsonDetails, @OrderJsonDetails
)";

        await using var cmd = new MySqlCommand(sql, conn);

        // --- Issue data ---
        cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
        cmd.Parameters.AddWithValue("@AccountId", accountId);
        cmd.Parameters.AddWithValue("@Type", (object?)issue.Type ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ReferenceNumber", (object?)issue.ReferenceNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Subject", (object?)issue.Subject ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Description", (object?)issue.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@StatusAllegro", (object?)issue.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@OpenedAt", issue.OpenedDate ?? DateTime.Now);
        cmd.Parameters.AddWithValue("@DecisionDueDate", (object?)issue.DecisionDueDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClosedAt", (object?)issue.ClosedAt ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CreatedAt", (object?)issue.OpenedDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", (object?)issue.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@OrderId", (object?)issue.CheckoutFormId ?? DBNull.Value);

        // --- Buyer data z OrderDetails (issue.Buyer ma tylko login, brak email/imion) ---
        cmd.Parameters.AddWithValue("@BuyerLogin",
            (object?)order?.Buyer?.Login ?? (object?)issue.BuyerLogin ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerEmail",
            (object?)order?.Buyer?.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerFirstName",
            (object?)order?.Buyer?.FirstName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerLastName",
            (object?)order?.Buyer?.LastName ?? DBNull.Value);

        // --- Delivery data z OrderDetails ---
        cmd.Parameters.AddWithValue("@DeliveryStreet",
            (object?)order?.Delivery?.Address?.Street ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryZipCode",
            (object?)order?.Delivery?.Address?.ResolvedZipCode ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryCity",
            (object?)order?.Delivery?.Address?.City ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryPhoneNumber",
            (object?)order?.Delivery?.Address?.PhoneNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryCompanyName",
            (object?)order?.Delivery?.Address?.CompanyName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryCompany",
            (object?)order?.Delivery?.Address?.CompanyName ?? DBNull.Value);

        // --- Product data ---
        cmd.Parameters.AddWithValue("@ProductId", (object?)issue.ProductId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@OfferId", (object?)issue.OfferId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductName", (object?)issue.OfferName ?? DBNull.Value);

        // EAN i SKU z LineItems (pierwszy item, który pasuje do OfferId)
        string? ean = null, sku = null;
        if (order?.LineItems != null && !string.IsNullOrWhiteSpace(issue.OfferId))
        {
            var matchingItem = order.LineItems.FirstOrDefault(li =>
                li.Offer?.Id == issue.OfferId);
            sku = matchingItem?.Offer?.External?.Id;
            // EAN nie jest bezpośrednio w checkout-forms — musimy użyć product.id z issue
        }
        cmd.Parameters.AddWithValue("@ProductEAN", (object?)ean ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductSKU", (object?)sku ?? DBNull.Value);

        // --- Invoice ---
        cmd.Parameters.AddWithValue("@InvoiceNumber", DBNull.Value); // Allegro nie daje invoice number w checkout-forms

        // --- Expectations ---
        var firstExp = issue.Expectations.FirstOrDefault();
        cmd.Parameters.AddWithValue("@Expectations", (object?)firstExp?.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InitialMessageText", (object?)issue.InitialMessageText ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InitialMessageCount", issue.ChatMessagesCount);
        cmd.Parameters.AddWithValue("@ExpectationType", (object?)firstExp?.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ExpectationRefundAmount",
            SafeParseDecimal(firstExp?.RefundAmount) ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ExpectationRefundCurrency",
            (object?)firstExp?.RefundCurrency ?? "PLN");

        // --- Reason ---
        cmd.Parameters.AddWithValue("@ReasonType", (object?)issue.ReasonType ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ReasonDescription", (object?)issue.ReasonDescription ?? DBNull.Value);

        // --- Computed ---
        cmd.Parameters.AddWithValue("@BoughtAt", (object?)order?.BoughtAt ?? DBNull.Value);

        bool needsDecision = (issue.Status == "IN_PROGRESS" || issue.Status == "OPENED")
                             && issue.DecisionDueDate.HasValue
                             && issue.DecisionDueDate.Value > DateTime.Now;
        cmd.Parameters.AddWithValue("@NeedsDecision", needsDecision ? 1 : 0);

        // --- JSON archiwum ---
        cmd.Parameters.AddWithValue("@JsonDetails", (object?)issue.RawJson ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@OrderJsonDetails", (object?)order?.RawJson ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    // =========================================================================
    // UPDATE ISTNIEJĄCEGO ISSUE — z danych listy (BEZ dodatkowego API call)
    // =========================================================================
    private static async Task UpdateDisputeFromListDataAsync(
        MySqlConnection conn,
        AllegroApiClient.AllegroIssueFullDto issue)
    {
        const string sql = @"
UPDATE AllegroDisputes SET
    StatusAllegro = @StatusAllegro,
    Status = COALESCE(@Status, Status),
    Subject = COALESCE(@Subject, Subject),
    Description = COALESCE(@Description, Description),
    OpenedAt = COALESCE(@OpenedAt, OpenedAt),
    CreatedAt = COALESCE(@CreatedAt, CreatedAt),
    ClosedAt = COALESCE(@ClosedAt, ClosedAt),
    DecisionDueDate = COALESCE(@DecisionDueDate, DecisionDueDate),
    ReferenceNumber = COALESCE(@ReferenceNumber, ReferenceNumber),
    Type = COALESCE(@Type, Type),
    BuyerLogin = COALESCE(@BuyerLogin, BuyerLogin),
    Expectations = COALESCE(@Expectations, Expectations),
    InitialMessageText = COALESCE(@InitialMessageText, InitialMessageText),
    InitialMessageCount = COALESCE(@InitialMessageCount, InitialMessageCount),
    ExpectationType = COALESCE(@ExpectationType, ExpectationType),
    ExpectationRefundAmount = COALESCE(@ExpectationRefundAmount, ExpectationRefundAmount),
    ExpectationRefundCurrency = COALESCE(@ExpectationRefundCurrency, ExpectationRefundCurrency),
    ReasonType = COALESCE(@ReasonType, ReasonType),
    ReasonDescription = COALESCE(@ReasonDescription, ReasonDescription),
    NeedsDecision = @NeedsDecision,
    LastCheckedAt = NOW(),
    JsonDetails = @JsonDetails
WHERE DisputeId = @DisputeId";

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
        cmd.Parameters.AddWithValue("@StatusAllegro", (object?)issue.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", (object?)issue.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Subject", (object?)issue.Subject ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Description", (object?)issue.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@OpenedAt", (object?)issue.OpenedDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CreatedAt", (object?)issue.OpenedDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClosedAt", (object?)issue.ClosedAt ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DecisionDueDate", (object?)issue.DecisionDueDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ReferenceNumber", (object?)issue.ReferenceNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Type", (object?)issue.Type ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerLogin", (object?)issue.BuyerLogin ?? DBNull.Value);
        var firstExp = issue.Expectations.FirstOrDefault();
        cmd.Parameters.AddWithValue("@Expectations", (object?)firstExp?.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InitialMessageText", (object?)issue.InitialMessageText ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InitialMessageCount", issue.ChatMessagesCount);
        cmd.Parameters.AddWithValue("@ExpectationType", (object?)firstExp?.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ExpectationRefundAmount", SafeParseDecimal(firstExp?.RefundAmount) ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@ExpectationRefundCurrency", (object?)firstExp?.RefundCurrency ?? "PLN");
        cmd.Parameters.AddWithValue("@ReasonType", (object?)issue.ReasonType ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ReasonDescription", (object?)issue.ReasonDescription ?? DBNull.Value);

        bool needsDecision = (issue.Status == "IN_PROGRESS" || issue.Status == "OPENED")
                             && issue.DecisionDueDate.HasValue
                             && issue.DecisionDueDate.Value > DateTime.Now;
        cmd.Parameters.AddWithValue("@NeedsDecision", needsDecision ? 1 : 0);

        cmd.Parameters.AddWithValue("@JsonDetails", (object?)issue.RawJson ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<bool> RequiresIssueBackfillAsync(MySqlConnection conn, string? disputeId)
    {
        if (string.IsNullOrWhiteSpace(disputeId))
            return false;

        const string sql = @"
SELECT 1
FROM AllegroDisputes
WHERE DisputeId = @DisputeId
  AND (
        BuyerFirstName IS NULL
        OR BuyerLastName IS NULL
        OR BuyerEmail IS NULL
        OR DeliveryCompanyName IS NULL
        OR DeliveryStreet IS NULL
        OR DeliveryZipCode IS NULL
        OR DeliveryCity IS NULL
        OR DeliveryPhoneNumber IS NULL
        OR Expectations IS NULL
        OR InitialMessageText IS NULL
        OR InitialMessageCount IS NULL
        OR CreatedAt IS NULL
        OR Status IS NULL
        OR ProductName IS NULL
        OR BoughtAt IS NULL
        OR OrderJsonDetails IS NULL
      )
LIMIT 1";

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@DisputeId", disputeId);
        var result = await cmd.ExecuteScalarAsync();
        return result != null && result != DBNull.Value;
    }

    private static async Task BackfillDisputeFromOrderAsync(
        MySqlConnection conn,
        AllegroApiClient.AllegroIssueFullDto issue,
        AllegroApiClient.AllegroFullOrderDetailsDto order)
    {
        const string sql = @"
UPDATE AllegroDisputes SET
    BuyerLogin = COALESCE(@BuyerLogin, BuyerLogin),
    BuyerEmail = COALESCE(@BuyerEmail, BuyerEmail),
    BuyerFirstName = COALESCE(@BuyerFirstName, BuyerFirstName),
    BuyerLastName = COALESCE(@BuyerLastName, BuyerLastName),
    DeliveryCompanyName = COALESCE(@DeliveryCompanyName, DeliveryCompanyName),
    DeliveryCompany = COALESCE(@DeliveryCompany, DeliveryCompany),
    DeliveryStreet = COALESCE(@DeliveryStreet, DeliveryStreet),
    DeliveryZipCode = COALESCE(@DeliveryZipCode, DeliveryZipCode),
    DeliveryCity = COALESCE(@DeliveryCity, DeliveryCity),
    DeliveryPhoneNumber = COALESCE(@DeliveryPhoneNumber, DeliveryPhoneNumber),
    Expectations = COALESCE(@Expectations, Expectations),
    InitialMessageText = COALESCE(@InitialMessageText, InitialMessageText),
    InitialMessageCount = COALESCE(@InitialMessageCount, InitialMessageCount),
    CreatedAt = COALESCE(@CreatedAt, CreatedAt),
    Status = COALESCE(@Status, Status),
    ProductName = COALESCE(@ProductName, ProductName),
    BoughtAt = COALESCE(@BoughtAt, BoughtAt),
    InvoiceNumber = COALESCE(@InvoiceNumber, InvoiceNumber),
    ProductSKU = COALESCE(@ProductSKU, ProductSKU),
    OrderJsonDetails = COALESCE(@OrderJsonDetails, OrderJsonDetails),
    LastCheckedAt = NOW()
WHERE DisputeId = @DisputeId";

        await using var cmd = new MySqlCommand(sql, conn);
        var firstExp = issue.Expectations.FirstOrDefault();
        string? productName = issue.OfferName;
        string? productSku = null;

        if (order.LineItems != null && order.LineItems.Count > 0)
        {
            var matching = !string.IsNullOrWhiteSpace(issue.OfferId)
                ? order.LineItems.FirstOrDefault(li => li.Offer?.Id == issue.OfferId)
                : order.LineItems.FirstOrDefault();

            productName = matching?.Offer?.Name ?? productName;
            productSku = matching?.Offer?.External?.Id;
        }

        cmd.Parameters.AddWithValue("@DisputeId", issue.Id);
        cmd.Parameters.AddWithValue("@BuyerLogin", (object?)order.Buyer?.Login ?? (object?)issue.BuyerLogin ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerEmail", (object?)order.Buyer?.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerFirstName", (object?)order.Buyer?.FirstName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerLastName", (object?)order.Buyer?.LastName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryCompanyName", (object?)order.Delivery?.Address?.CompanyName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryCompany", (object?)order.Delivery?.Address?.CompanyName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryStreet", (object?)order.Delivery?.Address?.Street ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryZipCode", (object?)order.Delivery?.Address?.ResolvedZipCode ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryCity", (object?)order.Delivery?.Address?.City ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryPhoneNumber", (object?)order.Delivery?.Address?.PhoneNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Expectations", (object?)firstExp?.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InitialMessageText", (object?)issue.InitialMessageText ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InitialMessageCount", issue.ChatMessagesCount);
        cmd.Parameters.AddWithValue("@CreatedAt", (object?)issue.OpenedDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", (object?)issue.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductName", (object?)productName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BoughtAt", (object?)order.BoughtAt ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@InvoiceNumber", DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductSKU", (object?)productSku ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@OrderJsonDetails", (object?)order.RawJson ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    // =========================================================================
    // ZAPIS WIADOMOŚCI CZATU — INSERT IGNORE (idempotentne)
    // =========================================================================
    private async Task<int> SaveChatMessagesAsync(
        MySqlConnection conn,
        string disputeId,
        List<AllegroApiClient.AllegroIssueChatMessageDto> messages)
    {
        var insertedMessages = 0;

        foreach (var msg in messages)
        {
            if (string.IsNullOrWhiteSpace(msg.Id))
                continue;

            try
            {
                // INSERT IGNORE — jeśli wiadomość już istnieje, nic nie rób
                const string sql = @"
INSERT IGNORE INTO AllegroChatMessages
    (MessageId, DisputeId, AuthorLogin, AuthorRole, MessageText, CreatedAt, HasAttachments, JsonDetails)
VALUES
    (@MessageId, @DisputeId, @AuthorLogin, @AuthorRole, @MessageText, @CreatedAt, @HasAttachments, @JsonDetails)";

                await using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MessageId", msg.Id);
                cmd.Parameters.AddWithValue("@DisputeId", disputeId);
                cmd.Parameters.AddWithValue("@AuthorLogin", (object?)msg.AuthorLogin ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AuthorRole", (object?)msg.AuthorRole ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MessageText", (object?)msg.Text ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedAt", msg.CreatedAt?.ToString("o") ?? DateTime.Now.ToString("o"));
                cmd.Parameters.AddWithValue("@HasAttachments", msg.Attachments.Count > 0 ? 1 : 0);
                cmd.Parameters.AddWithValue("@JsonDetails", (object?)msg.RawJson ?? DBNull.Value);

                var affectedRows = await cmd.ExecuteNonQueryAsync();
                if (affectedRows > 0)
                {
                    insertedMessages++;
                }

                // Zapisz załączniki
                if (msg.Attachments.Count > 0)
                {
                    await SaveChatAttachmentsAsync(conn, msg.Id, msg.Attachments);
                }

                // Sync do CentrumKontaktu
                await SyncMessageToCentrumKontaktuAsync(conn, disputeId, msg);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[Allegro][Sync] Błąd zapisu wiadomości {MsgId} dla issue {IssueId}",
                    msg.Id, disputeId);
            }
        }

        return insertedMessages;
    }

    // =========================================================================
    // ZAPIS ZAŁĄCZNIKÓW CZATU
    // =========================================================================
    private static async Task SaveChatAttachmentsAsync(
        MySqlConnection conn,
        string messageId,
        List<AllegroApiClient.AllegroAttachmentDto> attachments)
    {
        // Sprawdź czy załączniki dla tej wiadomości już istnieją
        await using (var checkCmd = new MySqlCommand(
            "SELECT COUNT(*) FROM AllegroChatAttachments WHERE MessageId = @MessageId", conn))
        {
            checkCmd.Parameters.AddWithValue("@MessageId", messageId);
            var existing = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
            if (existing > 0) return; // Już zapisane
        }

        foreach (var att in attachments)
        {
            const string sql = @"
INSERT INTO AllegroChatAttachments
    (MessageId, FileName, Url, Downloaded)
VALUES
    (@MessageId, @FileName, @Url, 0)";

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MessageId", messageId);
            cmd.Parameters.AddWithValue("@FileName", (object?)att.FileName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Url", (object?)att.Url ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    // =========================================================================
    // SYNC WIADOMOŚCI → CENTRUM KONTAKTU
    // =========================================================================
    private async Task SyncMessageToCentrumKontaktuAsync(
        MySqlConnection conn,
        string disputeId,
        AllegroApiClient.AllegroIssueChatMessageDto msg)
    {
        try
        {
            // Sprawdź czy issue jest powiązany ze zgłoszeniem
            string? complaintId = null;
            await using (var cmd = new MySqlCommand(
                "SELECT ComplaintId FROM AllegroDisputes WHERE DisputeId = @DisputeId", conn))
            {
                cmd.Parameters.AddWithValue("@DisputeId", disputeId);
                var result = await cmd.ExecuteScalarAsync();
                complaintId = result?.ToString();
            }

            if (string.IsNullOrWhiteSpace(complaintId) || complaintId == "0")
                return;

            // Sprawdź czy wiadomość już istnieje w CentrumKontaktu
            await using (var checkCmd = new MySqlCommand(@"
                SELECT COUNT(*) FROM CentrumKontaktu
                WHERE Kanal = 'Allegro' AND MetadataJson LIKE CONCAT('%', @MessageId, '%')", conn))
            {
                checkCmd.Parameters.AddWithValue("@MessageId", msg.Id);
                var count = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());
                if (count > 0) return;
            }

            string kierunek = msg.AuthorRole == "SELLER" ? "OUT" : "IN";

            await using var insertCmd = new MySqlCommand(@"
INSERT INTO CentrumKontaktu
    (ZgloszenieID, Typ, Kierunek, Nadawca, Odbiorca, Tresc,
     DataWyslania, DataOdbioru, Status, Priorytet, Kanal, MetadataJson)
VALUES
    (@ZgloszenieID, 'Chat Allegro', @Kierunek, @Nadawca, @Odbiorca, @Tresc,
     @DataWyslania, @DataWyslania, 'Dostarczona', 'Normalny', 'Allegro', @MetadataJson)", conn);

            insertCmd.Parameters.AddWithValue("@ZgloszenieID", complaintId);
            insertCmd.Parameters.AddWithValue("@Kierunek", kierunek);
            insertCmd.Parameters.AddWithValue("@Nadawca", (object?)msg.AuthorLogin ?? "Allegro");
            insertCmd.Parameters.AddWithValue("@Odbiorca", kierunek == "IN" ? "System" : (object?)msg.AuthorLogin ?? "Buyer");
            insertCmd.Parameters.AddWithValue("@Tresc", (object?)msg.Text ?? "");
            insertCmd.Parameters.AddWithValue("@DataWyslania", msg.CreatedAt ?? DateTime.Now);

            var metadata = JsonSerializer.Serialize(new
            {
                MessageId = msg.Id,
                DisputeId = disputeId,
                HasAttachments = msg.Attachments.Count > 0
            });
            insertCmd.Parameters.AddWithValue("@MetadataJson", metadata);

            await insertCmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[Allegro][Sync] Nie udało się zsync wiadomości {MsgId} do CentrumKontaktu",
                msg.Id);
        }
    }

    // =========================================================================
    // UPDATE LastMessageCount PO POBRANIU CZATU
    // =========================================================================
    private static async Task UpdateLastMessageCountAsync(
        MySqlConnection conn, string disputeId, int messageCount, bool hasNewMessages)
    {
        const string sql = @"
UPDATE AllegroDisputes SET
    LastMessageCount = @Count,
    HasNewMessages = CASE WHEN @HasNewMessages = 1 THEN 1 ELSE HasNewMessages END
WHERE DisputeId = @DisputeId";

        await using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Count", messageCount);
        cmd.Parameters.AddWithValue("@HasNewMessages", hasNewMessages ? 1 : 0);
        cmd.Parameters.AddWithValue("@DisputeId", disputeId);
        await cmd.ExecuteNonQueryAsync();
    }

    // =========================================================================
    // UPSERT do allegroorderdetails — PEŁNE DANE ZAMÓWIENIA
    // =========================================================================
    private static async Task UpsertOrderDetailsAsync(
        MySqlConnection conn,
        int accountId,
        string orderId,
        AllegroApiClient.AllegroFullOrderDetailsDto order)
    {
        const string sql = @"
INSERT INTO allegroorderdetails (
    OrderId, AllegroAccountId, Status, FulfillmentStatus, BoughtAt, UpdatedAt,
    BuyerLogin, BuyerEmail, BuyerFirstName, BuyerLastName, BuyerPhoneNumber, BuyerCompany, BuyerGuest,
    Delivery_Street, Delivery_City, Delivery_ZipCode, Delivery_CountryCode, Delivery_CompanyName, Delivery_PhoneNumber,
    PaymentType, PaymentProvider, PaymentFinishedAt, PaidAmount, PaidCurrency,
    DeliveryMethodId, DeliveryMethodName, DeliveryCost, DeliverySmart,
    PickupPointId, PickupPointName,
    TotalAmount, TotalCurrency,
    InvoiceRequired, Invoice_CompanyName, Invoice_TaxId,
    Invoice_Street, Invoice_City, Invoice_ZipCode, Invoice_CountryCode,
    MessageToSeller, JsonDetails, LastSyncAt
) VALUES (
    @OrderId, @AccountId, @Status, @FulfillmentStatus, @BoughtAt, @UpdatedAt,
    @BuyerLogin, @BuyerEmail, @BuyerFirstName, @BuyerLastName, @BuyerPhoneNumber, @BuyerCompany, @BuyerGuest,
    @Delivery_Street, @Delivery_City, @Delivery_ZipCode, @Delivery_CountryCode, @Delivery_CompanyName, @Delivery_PhoneNumber,
    @PaymentType, @PaymentProvider, @PaymentFinishedAt, @PaidAmount, @PaidCurrency,
    @DeliveryMethodId, @DeliveryMethodName, @DeliveryCost, @DeliverySmart,
    @PickupPointId, @PickupPointName,
    @TotalAmount, @TotalCurrency,
    @InvoiceRequired, @Invoice_CompanyName, @Invoice_TaxId,
    @Invoice_Street, @Invoice_City, @Invoice_ZipCode, @Invoice_CountryCode,
    @MessageToSeller, @JsonDetails, NOW()
)
ON DUPLICATE KEY UPDATE
    Status = VALUES(Status),
    FulfillmentStatus = VALUES(FulfillmentStatus),
    UpdatedAt = VALUES(UpdatedAt),
    BuyerLogin = COALESCE(VALUES(BuyerLogin), BuyerLogin),
    BuyerEmail = COALESCE(VALUES(BuyerEmail), BuyerEmail),
    BuyerFirstName = COALESCE(VALUES(BuyerFirstName), BuyerFirstName),
    BuyerLastName = COALESCE(VALUES(BuyerLastName), BuyerLastName),
    BuyerPhoneNumber = COALESCE(VALUES(BuyerPhoneNumber), BuyerPhoneNumber),
    PaymentFinishedAt = COALESCE(VALUES(PaymentFinishedAt), PaymentFinishedAt),
    PaidAmount = COALESCE(VALUES(PaidAmount), PaidAmount),
    JsonDetails = VALUES(JsonDetails),
    LastSyncAt = NOW()";

        await using var cmd = new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@OrderId", orderId);
        cmd.Parameters.AddWithValue("@AccountId", accountId);
        cmd.Parameters.AddWithValue("@Status", (object?)order.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FulfillmentStatus", (object?)order.Fulfillment?.Status ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BoughtAt", (object?)order.BoughtAt ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@UpdatedAt", (object?)order.UpdatedAt ?? DBNull.Value);

        // Buyer
        cmd.Parameters.AddWithValue("@BuyerLogin", (object?)order.Buyer?.Login ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerEmail", (object?)order.Buyer?.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerFirstName", (object?)order.Buyer?.FirstName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerLastName", (object?)order.Buyer?.LastName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerPhoneNumber", (object?)order.Buyer?.PhoneNumber ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerCompany", (object?)order.Buyer?.CompanyName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@BuyerGuest", order.Buyer?.Guest == true ? 1 : 0);

        // Delivery address
        var delAddr = order.Delivery?.Address;
        cmd.Parameters.AddWithValue("@Delivery_Street", (object?)delAddr?.Street ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Delivery_City", (object?)delAddr?.City ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Delivery_ZipCode", (object?)delAddr?.ResolvedZipCode ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Delivery_CountryCode", (object?)delAddr?.CountryCode ?? "PL");
        cmd.Parameters.AddWithValue("@Delivery_CompanyName", (object?)delAddr?.CompanyName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Delivery_PhoneNumber", (object?)delAddr?.PhoneNumber ?? DBNull.Value);

        // Payment
        cmd.Parameters.AddWithValue("@PaymentType", (object?)order.Payment?.Type ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PaymentProvider", (object?)order.Payment?.Provider ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PaymentFinishedAt", (object?)order.Payment?.FinishedAt ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PaidAmount", SafeParseDecimal(order.Payment?.PaidAmount?.Amount) ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@PaidCurrency", (object?)order.Payment?.PaidAmount?.Currency ?? "PLN");

        // Delivery method
        cmd.Parameters.AddWithValue("@DeliveryMethodId", (object?)order.Delivery?.Method?.Id ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryMethodName", (object?)order.Delivery?.Method?.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliveryCost", SafeParseDecimal(order.Delivery?.Cost?.Amount) ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@DeliverySmart", order.Delivery?.Smart == true ? 1 : 0);

        // Pickup point
        cmd.Parameters.AddWithValue("@PickupPointId", (object?)order.Delivery?.PickupPoint?.Id ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PickupPointName", (object?)order.Delivery?.PickupPoint?.Name ?? DBNull.Value);

        // Totals (computed from lineItems + delivery cost)
        decimal totalAmount = 0;
        string totalCurrency = "PLN";
        if (order.LineItems != null)
        {
            foreach (var li in order.LineItems)
            {
                var itemPrice = SafeParseDecimal(li.Price?.Amount);
                if (itemPrice.HasValue)
                    totalAmount += itemPrice.Value * li.Quantity;
                if (!string.IsNullOrWhiteSpace(li.Price?.Currency))
                    totalCurrency = li.Price!.Currency!;
            }
        }
        var deliveryCost = SafeParseDecimal(order.Delivery?.Cost?.Amount);
        if (deliveryCost.HasValue)
            totalAmount += deliveryCost.Value;

        cmd.Parameters.AddWithValue("@TotalAmount", totalAmount > 0 ? totalAmount : (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@TotalCurrency", totalCurrency);

        // Invoice
        var invoiceAddr = order.Invoice?.Address;
        cmd.Parameters.AddWithValue("@InvoiceRequired", order.Invoice?.Required == true ? 1 : 0);
        cmd.Parameters.AddWithValue("@Invoice_CompanyName", (object?)invoiceAddr?.Company?.Name ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Invoice_TaxId", (object?)invoiceAddr?.Company?.TaxId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Invoice_Street", (object?)invoiceAddr?.Street ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Invoice_City", (object?)invoiceAddr?.City ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Invoice_ZipCode", (object?)invoiceAddr?.ZipCode ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Invoice_CountryCode", (object?)invoiceAddr?.CountryCode ?? "PL");

        // Message to seller
        cmd.Parameters.AddWithValue("@MessageToSeller", (object?)order.MessageToSeller ?? DBNull.Value);

        // Full JSON
        cmd.Parameters.AddWithValue("@JsonDetails", (object?)order.RawJson ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    // =========================================================================
    // UPSERT do allegroorderitems — POZYCJE ZAMÓWIENIA
    // =========================================================================
    private static async Task UpsertOrderItemsAsync(
        MySqlConnection conn,
        string orderId,
        AllegroApiClient.AllegroFullOrderDetailsDto order)
    {
        if (order.LineItems == null || order.LineItems.Count == 0)
            return;

        // Usuń stare items i wstaw nowe (replace strategy)
        await using (var deleteCmd = new MySqlCommand(
            "DELETE FROM allegroorderitems WHERE OrderId = @OrderId", conn))
        {
            deleteCmd.Parameters.AddWithValue("@OrderId", orderId);
            await deleteCmd.ExecuteNonQueryAsync();
        }

        foreach (var item in order.LineItems)
        {
            const string sql = @"
INSERT INTO allegroorderitems
    (OrderId, OfferId, Name, Quantity, Price, Currency, ReconciliationAmount, ImageUrl, JsonDetails)
VALUES
    (@OrderId, @OfferId, @Name, @Quantity, @Price, @Currency, @ReconciliationAmount, NULL, @JsonDetails)";

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@OfferId", (object?)item.Offer?.Id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Name", (object?)item.Offer?.Name ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@Price", SafeParseDecimal(item.Price?.Amount) ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Currency", (object?)item.Price?.Currency ?? "PLN");
            cmd.Parameters.AddWithValue("@ReconciliationAmount",
                SafeParseDecimal(item.Reconciliation?.Value?.Amount) ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@JsonDetails",
                JsonSerializer.Serialize(item));

            await cmd.ExecuteNonQueryAsync();
        }
    }

    // =========================================================================
    // HELPERY
    // =========================================================================
    private static async Task<int> GetCountAsync(MySqlConnection conn, string sql)
    {
        await EnsureConnectionOpenAsync(conn);
        await using var cmd = new MySqlCommand(sql, conn);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private static async Task EnsureConnectionOpenAsync(MySqlConnection conn)
    {
        if (conn.State == System.Data.ConnectionState.Open)
            return;

        await conn.OpenAsync();
    }

    private static decimal? SafeParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var cleaned = value.Trim().Replace(" ", "").Replace(",", "");

        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;

        return null;
    }

    private async Task WriteSyncRunAsync(MySqlConnection conn, string serviceName, DateTime startedAt,
        bool success, int itemsProcessed, string? errorMessage = null)
    {
        try
        {
            await using var cmd = new MySqlCommand(@"
                INSERT INTO SyncRuns (source, started_at, finished_at, ok, rows_written, error_message)
                VALUES (@src, @started, NOW(), @ok, @rows, @err)", conn);
            cmd.Parameters.AddWithValue("@src", serviceName);
            cmd.Parameters.AddWithValue("@started", startedAt);
            cmd.Parameters.AddWithValue("@ok", success ? 1 : 0);
            cmd.Parameters.AddWithValue("@rows", itemsProcessed);
            cmd.Parameters.AddWithValue("@err", (object?)errorMessage ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WriteSyncRunAsync failed for {Service}", serviceName);
        }
    }
}
