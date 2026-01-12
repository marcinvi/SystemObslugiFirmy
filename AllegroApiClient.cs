using Newtonsoft.Json;
using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
using Reklamacje_Dane.Allegro.Models;
using Reklamacje_Dane.Allegro.Returns;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ReturnsInvoice = Reklamacje_Dane.Allegro.Invoice;

namespace Reklamacje_Dane
{
    public class AllegroRefreshTokenException : Exception
    {
        public AllegroRefreshTokenException(string message) : base(message) { }
    }

    public class AllegroApiClient
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private const string RedirectUri = "http://localhost:8989/";
        private const string AuthUrl = "https://allegro.pl/auth/oauth/authorize";
        private const string TokenUrl = "https://allegro.pl/auth/oauth/token";
        private const string ApiUrl = "https://api.allegro.pl";
        private readonly System.Net.Http.HttpClient _httpClient;
        public AllegroToken Token { get; set; }
        private int _accountId;
        private static readonly MediaTypeWithQualityHeaderValue ApiPublicV1 = new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json");
        private static readonly MediaTypeWithQualityHeaderValue ApiBetaV1 = new MediaTypeWithQualityHeaderValue("application/vnd.allegro.beta.v1+json");

        public AllegroApiClient(string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentException("ClientId i ClientSecret nie mogą być puste.");
            }
            _clientId = clientId;
            _clientSecret = clientSecret;
            _httpClient = new System.Net.Http.HttpClient();
        }

        #region Authorization Methods
        public async Task InitializeAsync(int accountId)
        {
            this._accountId = accountId;
            string query = "SELECT AccessTokenEncrypted, RefreshTokenEncrypted, TokenExpirationDate FROM AllegroAccounts WHERE Id = @AccountId LIMIT 1";
            using (var connection = new MySqlConnection(DatabaseHelper.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var accessTokenEncrypted = reader["AccessTokenEncrypted"] as string;
                            var refreshTokenEncrypted = reader["RefreshTokenEncrypted"] as string;
                            var expirationDateStr = reader["TokenExpirationDate"] as string;
                            if (string.IsNullOrEmpty(accessTokenEncrypted) || string.IsNullOrEmpty(refreshTokenEncrypted))
                            {
                                throw new InvalidOperationException("Brak tokenu w bazie dla tego konta. Wymagana autoryzacja.");
                            }
                            this.Token = new AllegroToken
                            {
                                AccessToken = EncryptionHelper.DecryptString(accessTokenEncrypted),
                                RefreshToken = EncryptionHelper.DecryptString(refreshTokenEncrypted),
                                ExpirationDate = DateTime.Parse(expirationDateStr)
                            };
                            if (DateTime.Now >= Token.ExpirationDate.AddMinutes(-5))
                            {
                                await RefreshTokenAsync();
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Nie znaleziono konta o podanym ID w bazie danych.");
                        }
                    }
                }
            }
        }
        private async Task SaveTokenToDatabaseAsync()
        {
            if (this.Token == null) return;
            var accessTokenEncrypted = EncryptionHelper.EncryptString(this.Token.AccessToken);
            var refreshTokenEncrypted = EncryptionHelper.EncryptString(this.Token.RefreshToken);
            string query = @"
                UPDATE AllegroAccounts 
                SET 
                    AccessTokenEncrypted = @AccessToken,
                    RefreshTokenEncrypted = @RefreshToken,
                    TokenExpirationDate = @ExpirationDate,
                    IsAuthorized = 1
                WHERE 
                    Id = @AccountId";
            using (var connection = new MySqlConnection(DatabaseHelper.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AccessToken", accessTokenEncrypted);
                    command.Parameters.AddWithValue("@RefreshToken", refreshTokenEncrypted);
                    command.Parameters.AddWithValue("@ExpirationDate", this.Token.ExpirationDate.ToString("o"));
                    command.Parameters.AddWithValue("@AccountId", this._accountId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task AuthorizeAsync()
        {
            string authUrl = $"{AuthUrl}?response_type=code&client_id={_clientId}&redirect_uri={System.Net.WebUtility.UrlEncode(RedirectUri)}";
            Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(RedirectUri);
                listener.Start();
                var context = await listener.GetContextAsync();
                string code = context.Request.QueryString.Get("code");
                var response = context.Response;
                string responseString = "<html><head><meta charset='utf-8'></head><body><h1>Sukces!</h1><p>Możesz teraz zamknąć to okno przeglądarki i wrócić do aplikacji.</p></body></html>";
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                output.Close();
                listener.Stop();
                if (!string.IsNullOrEmpty(code))
                {
                    await GetTokenAsync(code);
                }
                else
                {
                    throw new Exception("Nie udało się uzyskać kodu autoryzacyjnego od Allegro.");
                }
            }
        }
        private async Task GetTokenAsync(string code)
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });
            var response = await _httpClient.PostAsync(TokenUrl, content);
            await HandleFailedResponse(response, "pobierania tokena");
            var json = await response.Content.ReadAsStringAsync();
            Token = JsonConvert.DeserializeObject<AllegroToken>(json);
            Token.ExpirationDate = DateTime.Now.AddSeconds(Token.ExpiresIn);
            await SaveTokenToDatabaseAsync();
        }
        public async Task<AllegroToken> RefreshTokenAsync()
        {
            if (Token == null || string.IsNullOrEmpty(Token.RefreshToken))
            {
                throw new InvalidOperationException("Brak tokenu odświeżającego. Wymagana ponowna autoryzacja.");
            }
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", Token.RefreshToken),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });
            var response = await _httpClient.PostAsync(TokenUrl, content);
            await HandleFailedResponse(response, "odświeżania tokena", isTokenRefresh: true);
            var json = await response.Content.ReadAsStringAsync();
            var newToken = JsonConvert.DeserializeObject<AllegroToken>(json);
            if (string.IsNullOrEmpty(newToken.RefreshToken))
            {
                newToken.RefreshToken = this.Token.RefreshToken;
            }
            newToken.ExpirationDate = DateTime.Now.AddSeconds(newToken.ExpiresIn);
            this.Token = newToken;
            await SaveTokenToDatabaseAsync();
            return newToken;
        }
        #endregion

        public async Task<List<Issue>> GetIssuesAsync(string status = null, DateTime? fromDate = null)
        {
            var allIssues = new List<Issue>();
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(status))
            {
                queryParams.Add($"currentState.status={status}");
            }
            if (fromDate.HasValue)
            {
                queryParams.Add($"openedAt.gte={fromDate.Value.ToUniversalTime():o}");
            }

            int limit = 100;
            int offset = 0;
            while (true)
            {
                var pagedResponse = await GetPagedIssuesAsync(limit, offset, queryParams);
                if (pagedResponse.Any())
                {
                    allIssues.AddRange(pagedResponse);
                    if (pagedResponse.Count < limit) break;
                    offset += limit;
                }
                else
                {
                    break;
                }
            }
            return allIssues;
        }

        private async Task<List<Issue>> GetPagedIssuesAsync(int limit, int offset, List<string> queryParams)
        {
            var finalParams = new List<string>(queryParams)
            {
                $"limit={limit}",
                $"offset={offset}"
            };

            string endpoint = $"/sale/issues?{string.Join("&", finalParams)}";
            var response = await GetAsync<IssuesListResponse>(endpoint, ApiBetaV1);
            return response?.Issues ?? new List<Issue>();
        }

        #region Other API Methods
        public async Task<List<Reklamacje_Dane.Allegro.Issues.ChatMessage>> GetChatAsync(string issueId)
        {
            var response = await GetAsync<Reklamacje_Dane.Allegro.Issues.ChatMessageResponse>($"/sale/issues/{issueId}/chat", ApiBetaV1);
            return response?.Chat ?? new List<Reklamacje_Dane.Allegro.Issues.ChatMessage>();
        }
        public async Task SendMessageAsync(string issueId, NewMessageRequest message)
        {
            await PostAsync($"/sale/issues/{issueId}/message", message, ApiBetaV1);
        }
        public async Task<NewMessageAttachment> UploadAttachmentAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Plik nie istnieje.", filePath);
            var fileInfo = new FileInfo(filePath);
            var declarationRequest = new { fileName = fileInfo.Name, size = fileInfo.Length };
            var declarationResult = await PostAsync<NewMessageAttachment>("/sale/issues/attachments", declarationRequest, ApiBetaV1);
            string uploadUrl = declarationResult.Url;
            if (string.IsNullOrEmpty(uploadUrl))
            {
                throw new Exception("Nie otrzymano adresu URL do wysłania pliku od Allegro.");
            }
            var fileBytes = File.ReadAllBytes(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetMimeType(filePath));
            var uploadRequest = new HttpRequestMessage(HttpMethod.Put, new Uri(uploadUrl)) { Content = fileContent };
            var uploadResponse = await _httpClient.SendAsync(uploadRequest);
            await HandleFailedResponse(uploadResponse, "wysyłania pliku załącznika");
            return declarationResult;
        }
        public async Task ChangeClaimStatusAsync(string issueId, ChangeStatusRequest statusChange)
        {
            await PostAsync($"/sale/issues/{issueId}/status", statusChange, ApiBetaV1);
        }
        public async Task<AllegroCustomerReturnList> GetCustomerReturnsAsync(int limit = 1000, int offset = 0, Dictionary<string, string> filters = null)
        {
            if (filters == null) filters = new Dictionary<string, string>();
            filters["limit"] = limit.ToString();
            filters["offset"] = offset.ToString();
            return await GetAsync<AllegroCustomerReturnList>($"/order/customer-returns?{BuildQueryString(filters)}", ApiBetaV1);
        }
        public async Task<AllegroCustomerReturn> GetCustomerReturnDetailsAsync(string customerReturnId)
        {
            return await GetAsync<AllegroCustomerReturn>($"/order/customer-returns/{customerReturnId}", ApiPublicV1);
        }
        public async Task RejectCustomerReturnAsync(string customerReturnId, RejectCustomerReturnRequest rejection)
        {
            await PostAsync($"/order/customer-returns/{customerReturnId}/rejection", rejection, ApiPublicV1);
        }

        // #################### POCZĄTEK OSTATECZNEJ POPRAWKI ####################
        public async Task<OrderDetails> GetOrderDetailsByCheckoutFormIdAsync(string checkoutFormId)
        {
            if (string.IsNullOrEmpty(checkoutFormId))
            {
                return null;
            }

            try
            {
                // Szybka ścieżka: najpierw spróbuj standardowego endpointu
                return await GetAsync<OrderDetails>($"/order/checkout-forms/{checkoutFormId}", ApiPublicV1);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                System.Diagnostics.Debug.WriteLine($"[INFO] Nie znaleziono zamówienia {checkoutFormId} przez standardowy endpoint. Próbuję przez dziennik zdarzeń...");

                // Ścieżka awaryjna: jeśli 404, spróbuj przez dziennik zdarzeń (dla starszych zamówień)
                try
                {
                    var orderEvents = await GetOrderEventsAsync(checkoutFormId: checkoutFormId);
                    var foundEvent = orderEvents?.events?.FirstOrDefault(e => e.order?.checkoutForm?.id == checkoutFormId);

                    if (foundEvent != null)
                    {
                        // Mamy zdarzenie, więc teraz możemy pobrać szczegóły zamówienia
                        return await GetAsync<OrderDetails>($"/order/checkout-forms/{foundEvent.order.checkoutForm.id}", ApiPublicV1);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[OSTRZEŻENIE] Nie znaleziono zamówienia {checkoutFormId} również w dzienniku zdarzeń.");
                        return null;
                    }
                }
                catch (Exception fallbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[BŁĄD KRYTYCZNY] Wystąpił błąd podczas próby pobrania zamówienia {checkoutFormId} z dziennika zdarzeń: {fallbackEx.Message}");
                    return null;
                }
            }
        }
        // #################### KONIEC OSTATECZNEJ POPRAWKI ####################

        public async Task<EventStats> GetEventStatsAsync()
        {
            return await GetAsync<EventStats>("/order/event-stats", ApiPublicV1);
        }

        // Poprawiona sygnatura, aby przyjmować checkoutFormId
        public async Task<OrderEventsList> GetOrderEventsAsync(string from = null, int limit = 100, string type = null, string checkoutFormId = null)
        {
            var queryParams = new Dictionary<string, string> { { "limit", limit.ToString() } };
            if (!string.IsNullOrEmpty(from)) queryParams["from"] = from;
            if (!string.IsNullOrEmpty(type)) queryParams["type"] = type;
            if (!string.IsNullOrEmpty(checkoutFormId)) queryParams["order.checkoutForm.id"] = checkoutFormId;

            return await GetAsync<OrderEventsList>($"/order/events?{BuildQueryString(queryParams)}", ApiPublicV1);
        }

        public async Task<CheckoutFormsList> GetCheckoutFormsAsync(Dictionary<string, string> filters = null)
        {
            return await GetAsync<CheckoutFormsList>($"/order/checkout-forms?{BuildQueryString(filters)}", ApiPublicV1);
        }
        public async Task UpdateFulfillmentStatusAsync(string checkoutFormId, string status, string revision = null)
        {
            var payload = new { status };
            var endpoint = $"/order/checkout-forms/{checkoutFormId}/fulfillment";
            if (!string.IsNullOrEmpty(revision))
            {
                endpoint += $"?checkoutForm.revision={revision}";
            }
            await PutAsync(endpoint, payload, ApiPublicV1);
        }
        public async Task<List<ReturnsInvoice>> GetInvoicesForOrderAsync(string orderId)
        {
            var result = await GetInvoicesAsync(orderId);
            return result?.invoices ?? new List<ReturnsInvoice>();
        }
        public async Task<Invoice> AddInvoiceMetadataAsync(string checkoutFormId, string fileName, string invoiceNumber = null)
        {
            var payload = new { file = new { name = fileName }, invoiceNumber };
            return await PostAsync<Invoice>($"/order/checkout-forms/{checkoutFormId}/invoices", payload, ApiPublicV1);
        }
        public async Task UploadInvoiceFileAsync(string checkoutFormId, string invoiceId, string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("Plik faktury nie istnieje", filePath);
            var fileBytes = File.ReadAllBytes(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{ApiUrl}/order/checkout-forms/{checkoutFormId}/invoices/{invoiceId}/file")
            {
                Content = fileContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            request.Headers.Accept.Add(ApiPublicV1);
            var response = await _httpClient.SendAsync(request);
            await HandleFailedResponse(response, "wysyłania pliku faktury");
        }
        public async Task<InvoicesList> GetInvoicesAsync(string checkoutFormId)
        {
            return await GetAsync<InvoicesList>($"/order/checkout-forms/{checkoutFormId}/invoices", ApiPublicV1);
        }
        public async Task<Shipment> AddShipmentAsync(string checkoutFormId, string carrierId, string waybill, List<string> lineItemIds = null)
        {
            var payload = new
            {
                carrierId,
                waybill,
                lineItems = lineItemIds?.Select(id => new { id }).ToList()
            };
            return await PostAsync<Shipment>($"/order/checkout-forms/{checkoutFormId}/shipments", payload, ApiPublicV1);
        }
        public async Task<ShipmentsList> GetShipmentsAsync(string checkoutFormId)
        {
            return await GetAsync<ShipmentsList>($"/order/checkout-forms/{checkoutFormId}/shipments", ApiPublicV1);
        }
        public async Task<TrackingHistory> GetTrackingHistoryAsync(string carrierId, IEnumerable<string> waybills)
        {
            if (waybills == null || !waybills.Any()) return new TrackingHistory();
            var waybillParams = string.Join("&", waybills.Select(w => $"waybill={System.Net.WebUtility.UrlEncode(w)}"));
            return await GetAsync<TrackingHistory>($"/order/carriers/{carrierId}/tracking?{waybillParams}", ApiPublicV1);
        }
        public async Task<Refund> RefundPaymentAsync(PaymentRefundRequest refundRequest)
        {
            return await PostAsync<Refund>("/payments/refunds", refundRequest, ApiPublicV1);
        }
        public async Task<RefundsList> GetPaymentRefundsAsync(Dictionary<string, string> filters = null)
        {
            return await GetAsync<RefundsList>($"/payments/refunds?{BuildQueryString(filters)}", ApiPublicV1);
        }
        public async Task<RefundClaim> CreateRefundClaimAsync(string lineItemId, int quantity)
        {
            var payload = new { lineItem = new { id = lineItemId }, quantity };
            return await PostAsync<RefundClaim>("/order/refund-claims", payload, ApiPublicV1);
        }
        public async Task<RefundClaimsList> GetRefundClaimsAsync(Dictionary<string, string> filters = null)
        {
            return await GetAsync<RefundClaimsList>($"/order/refund-claims?{BuildQueryString(filters)}", ApiPublicV1);
        }
        public async Task<RefundClaim> GetRefundClaimDetailsAsync(string claimId)
        {
            return await GetAsync<RefundClaim>($"/order/refund-claims/{claimId}", ApiPublicV1);
        }
        public async Task CancelRefundClaimAsync(string claimId)
        {
            await DeleteAsync($"/order/refund-claims/{claimId}", ApiPublicV1);
        }
        public async Task<T> GetAsync<T>(string endpoint)
        {
            return await GetAsync<T>(endpoint, ApiBetaV1);
        }
        public async Task<T> GetAsync<T>(string endpoint, MediaTypeWithQualityHeaderValue acceptHeader)
        {
            if (Token == null) throw new InvalidOperationException("Klient API nie jest autoryzowany.");
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiUrl}{endpoint}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(acceptHeader);
            var response = await _httpClient.SendAsync(request);
            await HandleFailedResponse(response, $"wykonywania żądania GET dla {endpoint}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
        public async Task PostAsync(string endpoint, object payload, MediaTypeWithQualityHeaderValue acceptHeader)
        {
            await PostAsync<object>(endpoint, payload, acceptHeader);
        }
        public async Task<TResponse> PostAsync<TResponse>(string endpoint, object payload, MediaTypeWithQualityHeaderValue acceptHeader)
        {
            if (Token == null) throw new InvalidOperationException("Klient API nie jest autoryzowany.");
            var jsonContent = JsonConvert.SerializeObject(payload, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, acceptHeader.MediaType);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrl}{endpoint}") { Content = httpContent };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            request.Headers.Accept.Add(acceptHeader);
            var response = await _httpClient.SendAsync(request);
            await HandleFailedResponse(response, $"wykonywania żądania POST dla {endpoint}");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(json);
        }
        public async Task PutAsync(string endpoint, object payload, MediaTypeWithQualityHeaderValue acceptHeader)
        {
            if (Token == null) throw new InvalidOperationException("Klient API nie jest autoryzowany.");
            var jsonContent = JsonConvert.SerializeObject(payload, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, acceptHeader.MediaType);
            var request = new HttpRequestMessage(HttpMethod.Put, $"{ApiUrl}{endpoint}") { Content = httpContent };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            request.Headers.Accept.Add(acceptHeader);
            var response = await _httpClient.SendAsync(request);
            await HandleFailedResponse(response, $"wykonywania żądania PUT dla {endpoint}");
        }
        public async Task DeleteAsync(string endpoint, MediaTypeWithQualityHeaderValue acceptHeader)
        {
            if (Token == null) throw new InvalidOperationException("Klient API nie jest autoryzowany.");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{ApiUrl}{endpoint}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
            request.Headers.Accept.Add(acceptHeader);
            var response = await _httpClient.SendAsync(request);
            await HandleFailedResponse(response, $"wykonywania żądania DELETE dla {endpoint}");
        }
        private static string BuildQueryString(IDictionary<string, string> queryParams)
        {
            if (queryParams == null || queryParams.Count == 0) return string.Empty;
            var parts = new List<string>();
            foreach (var kv in queryParams)
            {
                if (kv.Value != null)
                {
                    parts.Add($"{System.Net.WebUtility.UrlEncode(kv.Key)}={System.Net.WebUtility.UrlEncode(kv.Value)}");
                }
            }
            return string.Join("&", parts);
        }
        private async Task HandleFailedResponse(HttpResponseMessage response, string actionDescription, bool isTokenRefresh = false)
        {
            if (!response.IsSuccessStatusCode)
            {
                string traceId = response.Headers.TryGetValues("trace-id", out var values) ? values.FirstOrDefault() : "Brak";
                string errorContent = await response.Content.ReadAsStringAsync();
                if (isTokenRefresh && errorContent.Contains("invalid_grant"))
                {
                    throw new AllegroRefreshTokenException($"Błąd 'invalid_grant' podczas odświeżania tokena. Wymagana ponowna autoryzacja. Trace-ID: {traceId}");
                }
                var ex = new HttpRequestException($"Błąd podczas '{actionDescription}': {(int)response.StatusCode} ({response.ReasonPhrase})\n\nTrace-ID: {traceId}");
                ex.Data["ResponseContent"] = errorContent;
                throw ex;
            }
        }
        private string GetMimeType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            switch (extension)
            {
                case ".jpg": case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".pdf": return "application/pdf";
                default: return "application/octet-stream";
            }
        }
        #endregion

        #region Data Models
        public class EventStats { public LatestEvent latestEvent { get; set; } }
        public class LatestEvent { public string id { get; set; } public DateTime occurredAt { get; set; } }
        public class OrderEventsList { public List<OrderEvent> events { get; set; } }
        public class OrderEvent { public string id { get; set; } public string type { get; set; } public DateTime occurredAt { get; set; } public OrderInEvent order { get; set; } }
        public class OrderInEvent { public CheckoutFormInEvent checkoutForm { get; set; } }
        public class CheckoutFormInEvent { public string id { get; set; } }
        public class CheckoutFormsList { public List<OrderDetails> checkoutForms { get; set; } }
        public class CheckoutForm : OrderDetails { }
        public class InvoicesList { public List<ReturnsInvoice> invoices { get; set; } }
        public class ShipmentsList { public List<Shipment> shipments { get; set; } }
        public class Shipment { public string id { get; set; } }
        public class TrackingHistory { }
        public class Invoice { public string id { get; set; } }
        public class Refund { public string id { get; set; } }
        public class RefundsList { public List<Refund> refunds { get; set; } }
        public class RefundClaim { public string id { get; set; } }
        public class RefundClaimsList { public List<RefundClaim> claims { get; set; } }
        #endregion
    }
}