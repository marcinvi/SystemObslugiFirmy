using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReklamacjeAPI.DTOs;
using System.Linq;

namespace ReklamacjeAPI.Services;

public class AllegroApiClient
{
    private const string ApiBaseUrl = "https://api.allegro.pl";
    private const string TokenUrl = "https://allegro.pl/auth/oauth/token";
    private const string AcceptPublicV1 = "application/vnd.allegro.public.v1+json";
    private const string AcceptBetaV1 = "application/vnd.allegro.beta.v1+json";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly AllegroCredentialsService _credentialsService;
    private readonly ILogger<AllegroApiClient> _logger;

    public AllegroApiClient(
        HttpClient httpClient,
        AllegroCredentialsService credentialsService,
        ILogger<AllegroApiClient> logger)
    {
        _httpClient = httpClient;
        _credentialsService = credentialsService;
        _logger = logger;
    }

    public async Task RejectCustomerReturnAsync(int accountId, string customerReturnId, RejectCustomerReturnRequestDto request)
    {
        var endpoint = $"{ApiBaseUrl}/order/customer-returns/{customerReturnId}/rejection";
        await SendAsync(accountId, HttpMethod.Post, endpoint, request, AcceptPublicV1);
    }

    public async Task<CustomerReturnListDto?> GetCustomerReturnsAsync(
        int accountId,
        int limit,
        int offset,
        IDictionary<string, string>? filters = null)
    {
        var queryParams = new Dictionary<string, string>(filters ?? new Dictionary<string, string>())
        {
            ["limit"] = limit.ToString(),
            ["offset"] = offset.ToString()
        };
        var query = BuildQueryString(queryParams);
        var endpoint = $"{ApiBaseUrl}/order/customer-returns?{query}";
        return await SendAsync<CustomerReturnListDto>(accountId, HttpMethod.Get, endpoint, null, AcceptBetaV1);
    }

    public async Task<List<InvoiceDto>> GetInvoicesForOrderAsync(int accountId, string checkoutFormId)
    {
        var endpoint = $"{ApiBaseUrl}/order/checkout-forms/{checkoutFormId}/invoices";
        var response = await SendAsync<InvoiceListDto>(accountId, HttpMethod.Get, endpoint, null, AcceptPublicV1);
        return response?.Invoices ?? new List<InvoiceDto>();
    }

    public async Task<OrderDetailsDto?> GetOrderDetailsAsync(int accountId, string checkoutFormId)
    {
        var endpoint = $"{ApiBaseUrl}/order/checkout-forms/{checkoutFormId}";
        try
        {
            return await SendAsync<OrderDetailsDto>(accountId, HttpMethod.Get, endpoint, null, AcceptPublicV1);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Nie znaleziono zamówienia {CheckoutFormId} w głównym endpointzie Allegro.", checkoutFormId);
            var fallbackId = await ResolveCheckoutFormIdFromEventsAsync(accountId, checkoutFormId);
            if (string.IsNullOrWhiteSpace(fallbackId) || fallbackId == checkoutFormId)
            {
                return null;
            }

            var fallbackEndpoint = $"{ApiBaseUrl}/order/checkout-forms/{fallbackId}";
            return await SendAsync<OrderDetailsDto>(accountId, HttpMethod.Get, fallbackEndpoint, null, AcceptPublicV1);
        }
    }

    public async Task RefundPaymentAsync(int accountId, PaymentRefundRequestDto request)
    {
        var endpoint = $"{ApiBaseUrl}/payments/refunds";
        await SendAsync(accountId, HttpMethod.Post, endpoint, request, AcceptPublicV1);
    }

    private async Task<string?> ResolveCheckoutFormIdFromEventsAsync(int accountId, string checkoutFormId)
    {
        var endpoint = $"{ApiBaseUrl}/order/events?order.checkoutForm.id={Uri.EscapeDataString(checkoutFormId)}&limit=1";
        var response = await SendAsync<OrderEventsResponse>(accountId, HttpMethod.Get, endpoint, null, AcceptPublicV1);
        return response?.Events?.FirstOrDefault()?.Order?.CheckoutForm?.Id;
    }

    private async Task SendAsync(int accountId, HttpMethod method, string endpoint, object? payload, string acceptHeader)
    {
        _ = await SendAsync<object>(accountId, method, endpoint, payload, acceptHeader);
    }

    private async Task<T?> SendAsync<T>(int accountId, HttpMethod method, string endpoint, object? payload, string acceptHeader)
    {
        var token = await GetAccessTokenAsync(accountId);
        var request = BuildRequest(method, endpoint, payload, token, acceptHeader);
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Token Allegro wygasł lub jest nieprawidłowy dla konta {AccountId}. Odświeżam.", accountId);
            token = await RefreshTokenAsync(accountId);
            request = BuildRequest(method, endpoint, payload, token, acceptHeader);
            response = await _httpClient.SendAsync(request);
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Błąd Allegro API ({response.StatusCode}): {errorBody}", null, response.StatusCode);
        }

        if (typeof(T) == typeof(object))
        {
            return default;
        }

        var body = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(body, JsonOptions);
    }

    private static string BuildQueryString(IDictionary<string, string> parameters)
    {
        return string.Join("&", parameters
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
    }

    private static HttpRequestMessage BuildRequest(HttpMethod method, string endpoint, object? payload, string token, string acceptHeader)
    {
        var request = new HttpRequestMessage(method, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));

        if (payload != null)
        {
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private async Task<string> GetAccessTokenAsync(int accountId)
    {
        var credentials = await _credentialsService.GetCredentialsAsync(accountId);
        if (credentials == null)
        {
            throw new InvalidOperationException("Nie znaleziono danych konta Allegro.");
        }

        if (string.IsNullOrWhiteSpace(credentials.AccessToken)
            || credentials.ExpirationDate == null
            || credentials.ExpirationDate <= DateTime.Now.AddMinutes(5))
        {
            return await RefreshTokenAsync(accountId, credentials);
        }

        return credentials.AccessToken;
    }

    private async Task<string> RefreshTokenAsync(int accountId, AllegroAccountCredentials? cachedCredentials = null)
    {
        var credentials = cachedCredentials ?? await _credentialsService.GetCredentialsAsync(accountId);
        if (credentials == null)
        {
            throw new InvalidOperationException("Nie znaleziono danych konta Allegro.");
        }

        if (string.IsNullOrWhiteSpace(credentials.RefreshToken))
        {
            throw new InvalidOperationException("Brak refresh tokenu dla konta Allegro.");
        }

        var basicToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credentials.ClientId}:{credentials.ClientSecret}"));
        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, TokenUrl);
        tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicToken);
        tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = credentials.RefreshToken,
            ["redirect_uri"] = "http://localhost:8989/"
        });

        var response = await _httpClient.SendAsync(tokenRequest);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Nie udało się odświeżyć tokenu Allegro ({response.StatusCode}): {errorBody}");
        }

        var body = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<AllegroTokenResponse>(body, JsonOptions);
        if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            throw new InvalidOperationException("Brak poprawnej odpowiedzi z Allegro podczas odświeżania tokenu.");
        }

        var refreshToken = string.IsNullOrWhiteSpace(tokenResponse.RefreshToken)
            ? credentials.RefreshToken
            : tokenResponse.RefreshToken;
        var expiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);

        await _credentialsService.SaveTokensAsync(accountId, tokenResponse.AccessToken, refreshToken, expiresAt);

        return tokenResponse.AccessToken;
    }

    private sealed class AllegroTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public sealed class OrderDetailsDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("payment")]
        public OrderPaymentDto? Payment { get; set; }

        [JsonPropertyName("delivery")]
        public OrderDeliveryDto? Delivery { get; set; }

        [JsonPropertyName("buyer")]
        public OrderBuyerDto? Buyer { get; set; }

        [JsonPropertyName("lineItems")]
        public List<OrderLineItemDto>? LineItems { get; set; }

        [JsonPropertyName("invoice")]
        public OrderInvoiceDto? Invoice { get; set; }

        [JsonPropertyName("fulfillment")]
        public OrderFulfillmentDto? Fulfillment { get; set; }
    }

    public sealed class OrderPaymentDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    public sealed class OrderDeliveryDto
    {
        [JsonPropertyName("cost")]
        public OrderCostDto? Cost { get; set; }

        [JsonPropertyName("address")]
        public OrderDeliveryAddressDto? Address { get; set; }
    }

    public sealed class OrderCostDto
    {
        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

    public sealed class OrderDeliveryAddressDto
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
    }

    public sealed class OrderBuyerDto
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("address")]
        public OrderBuyerAddressDto? Address { get; set; }
    }

    public sealed class OrderBuyerAddressDto
    {
        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("postCode")]
        public string? PostCode { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }
    }

    public sealed class OrderLineItemDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("offer")]
        public OrderOfferDto? Offer { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public OrderCostDto? Price { get; set; }
    }

    public sealed class OrderOfferDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public sealed class OrderInvoiceDto
    {
        [JsonPropertyName("address")]
        public OrderInvoiceAddressDto? Address { get; set; }
    }

    public sealed class OrderInvoiceAddressDto
    {
        [JsonPropertyName("company")]
        public OrderInvoiceCompanyDto? Company { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("zipCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }
    }

    public sealed class OrderInvoiceCompanyDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("taxId")]
        public string? TaxId { get; set; }
    }

    public sealed class OrderFulfillmentDto
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public sealed class CustomerReturnListDto
    {
        [JsonPropertyName("count")]
        public int? Count { get; set; }

        [JsonPropertyName("customerReturns")]
        public List<CustomerReturnDto>? CustomerReturns { get; set; }
    }

    public sealed class CustomerReturnDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("referenceNumber")]
        public string? ReferenceNumber { get; set; }

        [JsonPropertyName("orderId")]
        public string? OrderId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("buyer")]
        public CustomerReturnBuyerDto? Buyer { get; set; }

        [JsonPropertyName("parcels")]
        public List<CustomerReturnParcelDto>? Parcels { get; set; }

        [JsonPropertyName("items")]
        public List<CustomerReturnItemDto>? Items { get; set; }
    }

    public sealed class CustomerReturnBuyerDto
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }
    }

    public sealed class CustomerReturnParcelDto
    {
        [JsonPropertyName("waybill")]
        public string? Waybill { get; set; }

        [JsonPropertyName("carrierId")]
        public string? CarrierId { get; set; }
    }

    public sealed class CustomerReturnItemDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("offerId")]
        public string? OfferId { get; set; }

        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }

        [JsonPropertyName("reason")]
        public CustomerReturnReasonDto? Reason { get; set; }
    }

    public sealed class CustomerReturnReasonDto
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("userComment")]
        public string? UserComment { get; set; }
    }

    public sealed class InvoiceListDto
    {
        [JsonPropertyName("invoices")]
        public List<InvoiceDto>? Invoices { get; set; }
    }

    public sealed class InvoiceDto
    {
        [JsonPropertyName("invoiceNumber")]
        public string? InvoiceNumber { get; set; }
    }

    private sealed class OrderEventsResponse
    {
        [JsonPropertyName("events")]
        public List<OrderEventDto>? Events { get; set; }
    }

    private sealed class OrderEventDto
    {
        [JsonPropertyName("order")]
        public OrderEventOrderDto? Order { get; set; }
    }

    private sealed class OrderEventOrderDto
    {
        [JsonPropertyName("checkoutForm")]
        public OrderEventCheckoutFormDto? CheckoutForm { get; set; }
    }

    private sealed class OrderEventCheckoutFormDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
