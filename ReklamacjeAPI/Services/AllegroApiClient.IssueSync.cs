// =============================================================================
// AllegroApiClient.IssueSync.cs — Rozszerzenie API clienta o metody synchronizacji
// NOWE: Pełne parsowanie issues z metadanymi czatu + pobieranie wiadomości
// =============================================================================

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReklamacjeAPI.Services;

// Rozszerzenie istniejącego AllegroApiClient o nowe metody
public partial class AllegroApiClient
{
    // =========================================================================
    // NOWA METODA: Pobiera issues z PEŁNYMI danymi (w tym chat metadata)
    // Zamiast starego GetIssuesAsync które parsowało tylko 5 pól
    // =========================================================================
    public async Task<AllegroIssuesPageResult> GetIssuesFullPageAsync(
        int accountId, int limit = 100, int offset = 0)
    {
        var endpoint = $"{ApiBaseUrl}/sale/issues?limit={limit}&offset={offset}";
        var body = await SendRawAsync(accountId, HttpMethod.Get, endpoint, null,
            AcceptBetaV1, AcceptPublicV1);

        if (string.IsNullOrWhiteSpace(body))
            return new AllegroIssuesPageResult();

        using var doc = JsonDocument.Parse(body);
        var result = new AllegroIssuesPageResult();

        if (!doc.RootElement.TryGetProperty("issues", out var issuesEl)
            || issuesEl.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var item in issuesEl.EnumerateArray())
        {
            var issue = ParseFullIssue(item);
            if (!string.IsNullOrWhiteSpace(issue.Id))
                result.Issues.Add(issue);
        }

        return result;
    }

    // =========================================================================
    // NOWA METODA: Pobiera wiadomości czatu dla danego issue
    // GET /sale/issues/{issueId}/chat
    // =========================================================================
    public async Task<List<AllegroIssueChatMessageDto>> GetIssueChatMessagesAsync(
        int accountId, string issueId)
    {
        var endpoint = $"{ApiBaseUrl}/sale/issues/{issueId}/chat";
        var body = await SendRawAsync(accountId, HttpMethod.Get, endpoint, null,
            AcceptBetaV1, AcceptPublicV1);

        if (string.IsNullOrWhiteSpace(body))
            return new List<AllegroIssueChatMessageDto>();

        using var doc = JsonDocument.Parse(body);

        if (!doc.RootElement.TryGetProperty("chat", out var chatEl)
            || chatEl.ValueKind != JsonValueKind.Array)
            return new List<AllegroIssueChatMessageDto>();

        var messages = new List<AllegroIssueChatMessageDto>();
        foreach (var msgEl in chatEl.EnumerateArray())
        {
            messages.Add(new AllegroIssueChatMessageDto
            {
                Id = msgEl.GetStringProp("id"),
                Text = msgEl.GetStringProp("text"),
                CreatedAt = msgEl.GetDateTimeProp("createdAt"),
                AuthorLogin = msgEl.TryGetProperty("author", out var authorEl)
                    ? authorEl.GetStringProp("login") : null,
                AuthorRole = msgEl.TryGetProperty("author", out var authorEl2)
                    ? authorEl2.GetStringProp("role") : null,
                Attachments = ParseAttachments(msgEl),
                RawJson = msgEl.GetRawText()
            });
        }

        return messages;
    }

    // =========================================================================
    // ROZSZERZONA METODA: OrderDetails z pełnymi polami
    // =========================================================================
    public async Task<AllegroFullOrderDetailsDto?> GetFullOrderDetailsAsync(
        int accountId, string checkoutFormId)
    {
        if (string.IsNullOrWhiteSpace(checkoutFormId))
            return null;

        var endpoint = $"{ApiBaseUrl}/order/checkout-forms/{checkoutFormId}";
        try
        {
            var body = await SendRawAsync(accountId, HttpMethod.Get, endpoint, null,
                AcceptPublicV1, null);

            if (string.IsNullOrWhiteSpace(body))
                return null;

            var dto = JsonSerializer.Deserialize<AllegroFullOrderDetailsDto>(body, JsonOptions);
            if (dto != null)
                dto.RawJson = body;
            return dto;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Order {Id} not found", checkoutFormId);
            return null;
        }
    }

    // =========================================================================
    // PRYWATNE HELPERY PARSOWANIA
    // =========================================================================
    private static AllegroIssueFullDto ParseFullIssue(JsonElement item)
    {
        var issue = new AllegroIssueFullDto
        {
            Id = item.GetStringProp("id"),
            Type = item.GetStringProp("type"),
            ReferenceNumber = item.GetStringProp("referenceNumber"),
            Subject = item.GetStringProp("subject"),
            Description = item.GetStringProp("description"),
            OpenedDate = item.GetDateTimeProp("openedDate"),
            DecisionDueDate = item.GetDateTimeProp("decisionDueDate"),
            RawJson = item.GetRawText()
        };

        // currentState
        if (item.TryGetProperty("currentState", out var stateEl))
        {
            issue.Status = stateEl.GetStringProp("status");
            issue.StatusDueDate = stateEl.GetDateTimeProp("statusDueDate")
                                  ?? stateEl.GetDateTimeProp("dueDate");
            issue.ChatActive = stateEl.GetBoolProp("chatActive");
            issue.ReturnRequired = stateEl.GetStringProp("returnRequired");
        }

        // checkoutForm
        if (item.TryGetProperty("checkoutForm", out var cfEl))
        {
            issue.CheckoutFormId = cfEl.GetStringProp("id");
            issue.CheckoutFormCreatedAt = cfEl.GetDateTimeProp("createdAt");
        }

        // buyer
        if (item.TryGetProperty("buyer", out var buyerEl))
        {
            issue.BuyerId = buyerEl.GetStringProp("id");
            issue.BuyerLogin = buyerEl.GetStringProp("login");
        }

        // chat — KLUCZOWE: tu są metadane pozwalające na smart sync
        if (item.TryGetProperty("chat", out var chatEl))
        {
            issue.ChatMessagesCount = chatEl.GetIntProp("messagesCount") ?? 0;

            if (chatEl.TryGetProperty("lastMessage", out var lastMsgEl))
            {
                issue.LastMessageStatus = lastMsgEl.GetStringProp("status");
                issue.LastMessageCreatedAt = lastMsgEl.GetDateTimeProp("createdAt");
            }

            if (chatEl.TryGetProperty("initialMessage", out var initMsgEl))
            {
                issue.InitialMessageId = initMsgEl.GetStringProp("id");
                issue.InitialMessageText = initMsgEl.GetStringProp("text");
                issue.InitialMessageCreatedAt = initMsgEl.GetDateTimeProp("createdAt");

                if (initMsgEl.TryGetProperty("author", out var initAuthorEl))
                {
                    issue.InitialMessageAuthorLogin = initAuthorEl.GetStringProp("login");
                    issue.InitialMessageAuthorRole = initAuthorEl.GetStringProp("role");
                }
            }
        }

        // expectations
        if (item.TryGetProperty("expectations", out var expEl) && expEl.ValueKind == JsonValueKind.Array)
        {
            foreach (var exp in expEl.EnumerateArray())
            {
                issue.Expectations.Add(new AllegroExpectationDto
                {
                    Name = exp.GetStringProp("name"),
                    RefundAmount = exp.TryGetProperty("refund", out var refEl)
                        ? refEl.GetStringProp("amount") : null,
                    RefundCurrency = exp.TryGetProperty("refund", out var refEl2)
                        ? refEl2.GetStringProp("currency") : null,
                });
            }
        }

        // reason
        if (item.TryGetProperty("reason", out var reasonEl))
        {
            issue.ReasonType = reasonEl.GetStringProp("type");
            issue.ReasonDescription = reasonEl.GetStringProp("description");
        }

        // product
        if (item.TryGetProperty("product", out var prodEl))
            issue.ProductId = prodEl.GetStringProp("id");

        // offer
        if (item.TryGetProperty("offer", out var offerEl))
        {
            issue.OfferId = offerEl.GetStringProp("id");
            issue.OfferName = offerEl.GetStringProp("name");
            issue.OfferQuantity = offerEl.GetIntProp("quantity");
        }

        // right
        issue.Right = item.GetStringProp("right");

        return issue;
    }

    private static List<AllegroAttachmentDto> ParseAttachments(JsonElement msgEl)
    {
        var list = new List<AllegroAttachmentDto>();
        if (msgEl.TryGetProperty("attachments", out var attEl) && attEl.ValueKind == JsonValueKind.Array)
        {
            foreach (var att in attEl.EnumerateArray())
            {
                list.Add(new AllegroAttachmentDto
                {
                    FileName = att.GetStringProp("fileName"),
                    Url = att.GetStringProp("url")
                });
            }
        }
        return list;
    }

    // =========================================================================
    // DTOs — Pełne modele synchronizacji
    // =========================================================================

    public sealed class AllegroIssuesPageResult
    {
        public List<AllegroIssueFullDto> Issues { get; set; } = new();
    }

    public sealed class AllegroIssueFullDto
    {
        // Identyfikacja
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string? Right { get; set; }

        // Daty
        public DateTime? OpenedDate { get; set; }
        public DateTime? DecisionDueDate { get; set; }

        // Status
        public string? Status { get; set; }
        public DateTime? StatusDueDate { get; set; }
        public bool? ChatActive { get; set; }
        public string? ReturnRequired { get; set; }

        // Zamówienie
        public string? CheckoutFormId { get; set; }
        public DateTime? CheckoutFormCreatedAt { get; set; }

        // Kupujący
        public string? BuyerId { get; set; }
        public string? BuyerLogin { get; set; }

        // Chat metadata — klucz do smart sync!
        public int ChatMessagesCount { get; set; }
        public string? LastMessageStatus { get; set; }
        public DateTime? LastMessageCreatedAt { get; set; }
        public string? InitialMessageId { get; set; }
        public string? InitialMessageText { get; set; }
        public DateTime? InitialMessageCreatedAt { get; set; }
        public string? InitialMessageAuthorLogin { get; set; }
        public string? InitialMessageAuthorRole { get; set; }

        // Oczekiwania / reklamacja
        public List<AllegroExpectationDto> Expectations { get; set; } = new();
        public string? ReasonType { get; set; }
        public string? ReasonDescription { get; set; }

        // Produkt / oferta
        public string? ProductId { get; set; }
        public string? OfferId { get; set; }
        public string? OfferName { get; set; }
        public int? OfferQuantity { get; set; }

        // Raw JSON do archiwizacji
        public string? RawJson { get; set; }
    }

    public sealed class AllegroExpectationDto
    {
        public string? Name { get; set; }
        public string? RefundAmount { get; set; }
        public string? RefundCurrency { get; set; }
    }

    public sealed class AllegroIssueChatMessageDto
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? AuthorLogin { get; set; }
        public string? AuthorRole { get; set; }
        public List<AllegroAttachmentDto> Attachments { get; set; } = new();
        public string? RawJson { get; set; }
    }

    public sealed class AllegroAttachmentDto
    {
        public string? FileName { get; set; }
        public string? Url { get; set; }
    }

    // Rozszerzony OrderDetails z polami brakującymi w starej wersji
    public sealed class AllegroFullOrderDetailsDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("boughtAt")] public DateTime? BoughtAt { get; set; }
        [JsonPropertyName("buyer")] public FullBuyerDto? Buyer { get; set; }
        [JsonPropertyName("delivery")] public FullDeliveryDto? Delivery { get; set; }
        [JsonPropertyName("payment")] public FullPaymentDto? Payment { get; set; }
        [JsonPropertyName("fulfillment")] public OrderFulfillmentDto? Fulfillment { get; set; }
        [JsonPropertyName("invoice")] public FullInvoiceDto? Invoice { get; set; }
        [JsonPropertyName("lineItems")] public List<FullLineItemDto>? LineItems { get; set; }
        [JsonPropertyName("marketplace")] public MarketplaceDto? Marketplace { get; set; }
        [JsonPropertyName("messageToSeller")] public string? MessageToSeller { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
        [JsonPropertyName("updatedAt")] public DateTime? UpdatedAt { get; set; }
        [JsonIgnore] public string? RawJson { get; set; }
    }

    public sealed class FullBuyerDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("login")] public string? Login { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }
        [JsonPropertyName("firstName")] public string? FirstName { get; set; }
        [JsonPropertyName("lastName")] public string? LastName { get; set; }
        [JsonPropertyName("phoneNumber")] public string? PhoneNumber { get; set; }
        [JsonPropertyName("guest")] public bool? Guest { get; set; }
        [JsonPropertyName("companyName")] public string? CompanyName { get; set; }
        [JsonPropertyName("address")] public FullAddressDto? Address { get; set; }
    }

    public sealed class FullDeliveryDto
    {
        [JsonPropertyName("address")] public FullAddressDto? Address { get; set; }
        [JsonPropertyName("method")] public DeliveryMethodDto? Method { get; set; }
        [JsonPropertyName("cost")] public OrderCostDto? Cost { get; set; }
        [JsonPropertyName("smart")] public bool? Smart { get; set; }
        [JsonPropertyName("pickupPoint")] public PickupPointDto? PickupPoint { get; set; }
    }

    public sealed class DeliveryMethodDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
    }

    public sealed class PickupPointDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
    }

    public sealed class FullPaymentDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("type")] public string? Type { get; set; }
        [JsonPropertyName("provider")] public string? Provider { get; set; }
        [JsonPropertyName("finishedAt")] public DateTime? FinishedAt { get; set; }
        [JsonPropertyName("paidAmount")] public OrderCostDto? PaidAmount { get; set; }
    }

    public sealed class FullInvoiceDto
    {
        [JsonPropertyName("required")] public bool? Required { get; set; }
        [JsonPropertyName("address")] public FullInvoiceAddressDto? Address { get; set; }
    }

    public sealed class FullInvoiceAddressDto
    {
        [JsonPropertyName("company")] public OrderInvoiceCompanyDto? Company { get; set; }
        [JsonPropertyName("naturalPerson")] public NaturalPersonDto? NaturalPerson { get; set; }
        [JsonPropertyName("street")] public string? Street { get; set; }
        [JsonPropertyName("zipCode")] public string? ZipCode { get; set; }
        [JsonPropertyName("city")] public string? City { get; set; }
        [JsonPropertyName("countryCode")] public string? CountryCode { get; set; }
    }

    public sealed class NaturalPersonDto
    {
        [JsonPropertyName("firstName")] public string? FirstName { get; set; }
        [JsonPropertyName("lastName")] public string? LastName { get; set; }
    }

    public sealed class FullLineItemDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("quantity")] public int Quantity { get; set; }
        [JsonPropertyName("price")] public OrderCostDto? Price { get; set; }
        [JsonPropertyName("reconciliation")] public ReconciliationDto? Reconciliation { get; set; }
        [JsonPropertyName("offer")] public FullOfferDto? Offer { get; set; }
    }

    public sealed class ReconciliationDto
    {
        [JsonPropertyName("value")] public OrderCostDto? Value { get; set; }
    }

    public sealed class FullOfferDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("external")] public ExternalOfferDto? External { get; set; }
    }

    public sealed class ExternalOfferDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
    }

    public sealed class FullAddressDto
    {
        [JsonPropertyName("firstName")] public string? FirstName { get; set; }
        [JsonPropertyName("lastName")] public string? LastName { get; set; }
        [JsonPropertyName("street")] public string? Street { get; set; }
        [JsonPropertyName("zipCode")] public string? ZipCode { get; set; }
        [JsonPropertyName("postCode")] public string? PostCode { get; set; }
        [JsonPropertyName("city")] public string? City { get; set; }
        [JsonPropertyName("phoneNumber")] public string? PhoneNumber { get; set; }
        [JsonPropertyName("companyName")] public string? CompanyName { get; set; }
        [JsonPropertyName("countryCode")] public string? CountryCode { get; set; }

        // Helper: ZipCode ma różne nazwy w różnych kontekstach API
        public string? ResolvedZipCode => ZipCode ?? PostCode;
    }

    public sealed class MarketplaceDto
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
    }
}

// =========================================================================
// Extension methods do parsowania JsonElement
// =========================================================================
internal static class JsonElementExtensions
{
    public static string? GetStringProp(this JsonElement el, string name)
    {
        if (el.ValueKind != JsonValueKind.Object)
            return null;

        if (el.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
            return prop.GetString();
        return null;
    }

    public static DateTime? GetDateTimeProp(this JsonElement el, string name)
    {
        if (el.ValueKind != JsonValueKind.Object)
            return null;

        if (el.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            var str = prop.GetString();
            if (DateTime.TryParse(str, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                return dt;
        }
        return null;
    }

    public static int? GetIntProp(this JsonElement el, string name)
    {
        if (el.ValueKind != JsonValueKind.Object)
            return null;

        if (el.TryGetProperty(name, out var prop))
        {
            if (prop.ValueKind == JsonValueKind.Number)
                return prop.GetInt32();
            if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out var val))
                return val;
        }
        return null;
    }

    public static bool? GetBoolProp(this JsonElement el, string name)
    {
        if (el.ValueKind != JsonValueKind.Object)
            return null;

        if (el.TryGetProperty(name, out var prop))
        {
            if (prop.ValueKind == JsonValueKind.True) return true;
            if (prop.ValueKind == JsonValueKind.False) return false;
            if (prop.ValueKind == JsonValueKind.String)
            {
                var s = prop.GetString();
                if (bool.TryParse(s, out var b)) return b;
            }
        }
        return null;
    }
}
