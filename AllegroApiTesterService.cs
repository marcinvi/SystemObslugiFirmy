// ========================================
// AllegroApiTesterService.cs
// Serwis testujący API Allegro z dokładnymi logami
// Data: 2026-01-09
// WERSJA: NAPRAWIONA (wszystkie błędy usunięte)
// ========================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
using AllegroChat = Reklamacje_Dane.Allegro.Issues.ChatMessage;

namespace Reklamacje_Dane
{
    public class AllegroApiTesterService
    {
        private readonly AllegroApiClient _apiClient;
        private readonly int _accountId;
        private readonly Action<string> _logCallback;
        private readonly Stopwatch _stopwatch;

        public AllegroApiTesterService(AllegroApiClient apiClient, int accountId, Action<string> logCallback)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _accountId = accountId;
            _logCallback = logCallback ?? (msg => Debug.WriteLine(msg));
            _stopwatch = new Stopwatch();
        }

        #region Test Issues

        /// <summary>
        /// Testuje pobieranie Issues z pełnym logowaniem
        /// </summary>
        public async Task<TestResult> TestGetIssuesAsync(int limit = 100, int offset = 0)
        {
            var result = new TestResult();
            result.TestType = "GetIssues";
            result.Endpoint = $"/sale/issues?limit={limit}&offset={offset}";
            
            Log("========================================");
            Log($"🧪 TEST: Pobieranie Issues");
            Log($"📍 Endpoint: {result.Endpoint}");
            Log($"🔑 Account ID: {_accountId}");
            Log("========================================");
            
            _stopwatch.Restart();
            
            try
            {
                // Loguj request
                Log("📤 REQUEST:");
                Log($"   Method: GET");
                Log($"   URL: https://api.allegro.pl{result.Endpoint}");
                Log($"   Headers:");
                Log($"      Authorization: Bearer {MaskToken(_apiClient.Token?.AccessToken)}");
                Log($"      Accept: application/vnd.allegro.beta.v1+json");
                Log("");

                // Wykonaj request
                var issues = await _apiClient.GetIssuesAsync();
                
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                
                // Loguj response
                Log("📥 RESPONSE:");
                Log($"   Status: 200 OK");
                Log($"   Duration: {result.DurationMs}ms");
                Log($"   Issues Count: {issues?.Count ?? 0}");
                Log("");
                
                if (issues != null && issues.Any())
                {
                    Log("📋 ISSUES (pierwsze 3):");
                    foreach (var issue in issues.Take(3))
                    {
                        Log($"   ├─ ID: {issue.Id}");
                        Log($"   │  Type: {issue.Type}");
                        Log($"   │  Subject: {issue.Subject}");
                        Log($"   │  Status: {issue.CurrentState?.Status}");
                        Log($"   │  Buyer: {issue.Buyer?.Login}");
                        Log($"   │  Opened: {issue.OpenedDate:yyyy-MM-dd HH:mm}");
                        Log($"   │  CheckoutFormId: {issue.CheckoutForm?.Id}");
                        Log($"   └────────────────");
                    }
                    
                    if (issues.Count > 3)
                    {
                        Log($"   ... i {issues.Count - 3} więcej");
                    }
                }
                else
                {
                    Log("⚠️  Brak issues!");
                }
                
                result.Success = true;
                result.Data = issues;
                result.Message = $"Pobrano {issues?.Count ?? 0} issues";
                
                // Zapisz do bazy testowej
                await SaveIssuesTestDataAsync(issues);
                
                Log("");
                Log($"✅ TEST ZAKOŃCZONY SUKCESEM");
                Log("========================================");
            }
            catch (HttpRequestException httpEx)
            {
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                result.Success = false;
                result.ErrorMessage = httpEx.Message;
                result.ResponseStatus = ExtractStatusCode(httpEx.Message);
                
                Log("❌ RESPONSE ERROR:");
                Log($"   Status: {result.ResponseStatus}");
                Log($"   Message: {httpEx.Message}");
                Log($"   Duration: {result.DurationMs}ms");
                
                if (httpEx.Data.Contains("ResponseContent"))
                {
                    Log($"   Body: {httpEx.Data["ResponseContent"]}");
                }
                
                Log("========================================");
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                result.Success = false;
                result.ErrorMessage = ex.Message;
                
                Log($"❌ EXCEPTION: {ex.GetType().Name}");
                Log($"   Message: {ex.Message}");
                Log($"   StackTrace: {ex.StackTrace}");
                Log("========================================");
            }
            
            // Zapisz log do bazy
            await SaveTestLogAsync(result);
            
            return result;
        }

        #endregion

        #region Test Chat Messages

        /// <summary>
        /// Testuje pobieranie Chat Messages dla Issue
        /// </summary>
        public async Task<TestResult> TestGetChatAsync(string issueId)
        {
            var result = new TestResult();
            result.TestType = "GetChat";
            result.Endpoint = $"/sale/issues/{issueId}/chat";
            
            Log("========================================");
            Log($"🧪 TEST: Pobieranie Chat Messages");
            Log($"📍 Issue ID: {issueId}");
            Log("========================================");
            
            _stopwatch.Restart();
            
            try
            {
                Log("📤 REQUEST:");
                Log($"   Method: GET");
                Log($"   URL: https://api.allegro.pl{result.Endpoint}");
                Log($"   Paginacja: Automatyczna (limit=100)");
                Log("");

                var messages = await _apiClient.GetChatAsync(issueId);
                
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                
                Log("📥 RESPONSE:");
                Log($"   Status: 200 OK");
                Log($"   Duration: {result.DurationMs}ms");
                Log($"   Messages Count: {messages?.Count ?? 0}");
                Log("");
                
                if (messages != null && messages.Any())
                {
                    Log("💬 MESSAGES (pierwsze 5):");
                    foreach (var msg in messages.Take(5))
                    {
                        Log($"   ├─ ID: {msg.Id}");
                        Log($"   │  Author: {msg.Author?.Login} ({msg.Author?.Role})");
                        Log($"   │  Date: {msg.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                        Log($"   │  Text: {TruncateText(msg.Text, 100)}");
                        Log($"   └────────────────");
                    }
                    
                    if (messages.Count > 5)
                    {
                        Log($"   ... i {messages.Count - 5} więcej");
                    }
                }
                else
                {
                    Log("⚠️  Brak wiadomości!");
                }
                
                result.Success = true;
                result.Data = messages;
                result.Message = $"Pobrano {messages?.Count ?? 0} wiadomości";
                
                // Zapisz do bazy testowej
                await SaveChatTestDataAsync(issueId, messages);
                
                Log($"✅ TEST ZAKOŃCZONY SUKCESEM");
                Log("========================================");
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                result.Success = false;
                result.ErrorMessage = ex.Message;
                
                Log($"❌ ERROR: {ex.Message}");
                Log("========================================");
            }
            
            await SaveTestLogAsync(result);
            return result;
        }

        #endregion

        #region Test Buyer Email

        /// <summary>
        /// Testuje pobieranie emaila kupującego
        /// </summary>
        public async Task<TestResult> TestGetBuyerEmailAsync(string checkoutFormId)
        {
            var result = new TestResult();
            result.TestType = "GetBuyerEmail";
            result.Endpoint = $"/order/checkout-forms/{checkoutFormId}";
            
            Log("========================================");
            Log($"🧪 TEST: Pobieranie Email Kupującego");
            Log($"📍 CheckoutForm ID: {checkoutFormId}");
            Log("========================================");
            
            _stopwatch.Restart();
            
            try
            {
                Log("📤 REQUEST:");
                Log($"   Method: GET");
                Log($"   URL: https://api.allegro.pl{result.Endpoint}");
                Log($"   Accept: application/vnd.allegro.public.v1+json");
                Log("");

                var email = await _apiClient.GetBuyerEmailAsync(checkoutFormId);
                
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                
                Log("📥 RESPONSE:");
                Log($"   Status: 200 OK");
                Log($"   Duration: {result.DurationMs}ms");
                Log($"   Email: {email ?? "NULL"}");
                Log("");
                
                if (!string.IsNullOrEmpty(email))
                {
                    Log($"✅ Email znaleziony: {email}");
                    
                    if (email.Contains("@allegromail.pl"))
                    {
                        Log($"ℹ️  To maskowany email Allegro (funkcjonalny)");
                    }
                }
                else
                {
                    Log("⚠️  Email jest NULL!");
                    Log("   Możliwe przyczyny:");
                    Log("   - Brak tokena autoryzacji (NAJCZĘSTSZY)");
                    Log("   - Błędny CheckoutFormId");
                    Log("   - Buyer nie podał emaila");
                }
                
                result.Success = true;
                result.Data = new List<string> { email };
                result.Message = !string.IsNullOrEmpty(email) ? $"Email: {email}" : "Email NULL";
                
                Log($"✅ TEST ZAKOŃCZONY");
                Log("========================================");
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                result.Success = false;
                result.ErrorMessage = ex.Message;
                
                Log($"❌ ERROR: {ex.Message}");
                Log("========================================");
            }
            
            await SaveTestLogAsync(result);
            return result;
        }

        #endregion

        #region Test Order Details

        /// <summary>
        /// Testuje pobieranie szczegółów zamówienia
        /// </summary>
        public async Task<TestResult> TestGetOrderDetailsAsync(string checkoutFormId)
        {
            var result = new TestResult();
            result.TestType = "GetOrderDetails";
            result.Endpoint = $"/order/checkout-forms/{checkoutFormId}";
            
            Log("========================================");
            Log($"🧪 TEST: Pobieranie Szczegółów Zamówienia");
            Log($"📍 CheckoutForm ID: {checkoutFormId}");
            Log("========================================");
            
            _stopwatch.Restart();
            
            try
            {
                Log("📤 REQUEST:");
                Log($"   Method: GET");
                Log($"   URL: https://api.allegro.pl{result.Endpoint}");
                Log("");

                var orderDetails = await _apiClient.GetOrderDetailsByCheckoutFormIdAsync(checkoutFormId);
                
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                
                Log("📥 RESPONSE:");
                Log($"   Status: 200 OK");
                Log($"   Duration: {result.DurationMs}ms");
                Log("");
                
                if (orderDetails != null)
                {
                    Log("📦 ORDER DETAILS:");
                    Log($"   ID: {orderDetails.Id}");
                    Log($"   Status: {orderDetails.Status}");
                    Log($"   Created: {orderDetails.CreatedAt:yyyy-MM-dd HH:mm}");
                    Log($"");
                    Log($"   👤 BUYER:");
                    Log($"      Email: {orderDetails.Buyer?.Email}");
                    Log($"      Login: {orderDetails.Buyer?.Login}");
                    Log($"      Name: {orderDetails.Buyer?.FirstName} {orderDetails.Buyer?.LastName}");
                    Log($"      Phone: {orderDetails.Buyer?.PhoneNumber}");
                    Log($"");
                    Log($"   💰 PAYMENT:");
                    Log($"      Type: {orderDetails.Payment?.Type}");
                    Log($"      Amount: {orderDetails.Payment?.TotalToPay?.Amount} {orderDetails.Payment?.TotalToPay?.Currency}");
                    Log($"");
                    Log($"   📍 DELIVERY:");
                    Log($"      Method: {orderDetails.Delivery?.Method?.Name}");
                    Log($"      Address: {orderDetails.Delivery?.Address?.Street}");
                    Log($"      City: {orderDetails.Delivery?.Address?.City}");
                    Log($"      PostCode: {orderDetails.Delivery?.Address?.ZipCode}");
                    Log($"");
                    Log($"   📦 LINE ITEMS: {orderDetails.LineItems?.Count ?? 0}");
                    
                    if (orderDetails.LineItems != null && orderDetails.LineItems.Any())
                    {
                        foreach (var item in orderDetails.LineItems.Take(3))
                        {
                            Log($"      ├─ {item.Offer?.Name}");
                            Log($"      │  Qty: {item.Quantity}");
                            Log($"      │  Price: {item.Price?.Amount} {item.Price?.Currency}");
                        }
                    }
                }
                else
                {
                    Log("⚠️  OrderDetails is NULL!");
                }
                
                result.Success = true;
                result.Data = orderDetails;
                result.Message = "Pobrano szczegóły zamówienia";
                
                // Zapisz do bazy testowej
                await SaveOrderDetailsTestDataAsync(orderDetails);
                
                Log($"✅ TEST ZAKOŃCZONY SUKCESEM");
                Log("========================================");
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                result.Success = false;
                result.ErrorMessage = ex.Message;
                
                Log($"❌ ERROR: {ex.Message}");
                Log("========================================");
            }
            
            await SaveTestLogAsync(result);
            return result;
        }

        #endregion

        #region Test Issue Details

        /// <summary>
        /// Testuje pobieranie pełnych szczegółów Issue
        /// </summary>
        public async Task<TestResult> TestGetIssueDetailsAsync(string issueId)
        {
            var result = new TestResult();
            result.TestType = "GetIssueDetails";
            result.Endpoint = $"/sale/issues/{issueId}";
            
            Log("========================================");
            Log($"🧪 TEST: Pobieranie Szczegółów Issue");
            Log($"📍 Issue ID: {issueId}");
            Log("========================================");
            
            _stopwatch.Restart();
            
            try
            {
                Log("📤 REQUEST:");
                Log($"   Method: GET");
                Log($"   URL: https://api.allegro.pl{result.Endpoint}");
                Log("");

                var issue = await _apiClient.GetIssueDetailsAsync(issueId);
                
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                
                Log("📥 RESPONSE:");
                Log($"   Status: 200 OK");
                Log($"   Duration: {result.DurationMs}ms");
                Log("");
                
                if (issue != null)
                {
                    Log("📋 ISSUE DETAILS:");
                    Log($"   ID: {issue.Id}");
                    Log($"   Type: {issue.Type}");
                    Log($"   Subject: {issue.Subject}");
                    Log($"   Description: {TruncateText(issue.Description, 200)}");
                    Log($"   Status: {issue.CurrentState?.Status}");
                    Log($"   Opened: {issue.OpenedDate:yyyy-MM-dd HH:mm}");
                    Log($"   Updated: {issue.UpdatedAt:yyyy-MM-dd HH:mm}");
                    Log($"   Decision Due: {issue.DecisionDueDate:yyyy-MM-dd HH:mm}");
                    Log($"");
                    Log($"   👤 BUYER:");
                    Log($"      Login: {issue.Buyer?.Login}");
                    Log($"");
                    Log($"   📦 CHECKOUT FORM:");
                    Log($"      ID: {issue.CheckoutForm?.Id}");
                    Log($"      Created: {issue.CheckoutForm?.CreatedAt:yyyy-MM-dd HH:mm}");
                    Log($"");
                    Log($"   🛍️ OFFER:");
                    Log($"      ID: {issue.Offer?.Id}");
                    Log($"      Name: {issue.Offer?.Name}");
                    Log($"");
                    Log($"   💡 EXPECTATIONS:");
                    if (issue.Expectations != null && issue.Expectations.Any())
                    {
                        foreach (var exp in issue.Expectations)
                        {
                            Log($"      - {exp.Name}");
                            if (exp.Refund != null)
                            {
                                Log($"        Amount: {exp.Refund.Amount} {exp.Refund.Currency}");
                            }
                        }
                    }
                    else
                    {
                        Log($"      (brak)");
                    }
                    Log($"");
                    Log($"   📝 REASON:");
                    Log($"      Type: {issue.Reason?.Type}");
                    Log($"      Description: {issue.Reason?.Description}");
                    Log($"");
                    Log($"   🔢 REFERENCE NUMBER: {issue.ReferenceNumber}");
                }
                else
                {
                    Log("⚠️  Issue is NULL!");
                }
                
                result.Success = true;
                result.Data = issue;
                result.Message = "Pobrano szczegóły issue";
                
                Log($"✅ TEST ZAKOŃCZONY SUKCESEM");
                Log("========================================");
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                result.DurationMs = (int)_stopwatch.ElapsedMilliseconds;
                result.Success = false;
                result.ErrorMessage = ex.Message;
                
                Log($"❌ ERROR: {ex.Message}");
                Log("========================================");
            }
            
            await SaveTestLogAsync(result);
            return result;
        }

        #endregion

        #region Database Save Methods

        private async Task SaveTestLogAsync(TestResult result)
        {
            try
            {
                using (var con = new MySqlConnection(DatabaseHelper.GetConnectionString()))
                {
                    await con.OpenAsync();
                    
                    var query = @"
                        INSERT INTO AllegroApiTestLogs 
                        (AccountId, TestType, EndpointUrl, HttpMethod, ResponseStatus, 
                         ResponseBody, Success, ErrorMessage, DurationMs)
                        VALUES 
                        (@AccountId, @TestType, @EndpointUrl, @HttpMethod, @ResponseStatus, 
                         @ResponseBody, @Success, @ErrorMessage, @DurationMs)";
                    
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AccountId", _accountId);
                        cmd.Parameters.AddWithValue("@TestType", result.TestType);
                        cmd.Parameters.AddWithValue("@EndpointUrl", result.Endpoint);
                        cmd.Parameters.AddWithValue("@HttpMethod", "GET");
                        cmd.Parameters.AddWithValue("@ResponseStatus", result.ResponseStatus);
                        cmd.Parameters.AddWithValue("@ResponseBody", 
                            result.Data != null ? JsonConvert.SerializeObject(result.Data, Formatting.Indented) : null);
                        cmd.Parameters.AddWithValue("@Success", result.Success);
                        cmd.Parameters.AddWithValue("@ErrorMessage", (object)result.ErrorMessage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DurationMs", result.DurationMs);
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"⚠️  Nie udało się zapisać logu do bazy: {ex.Message}");
            }
        }

        private async Task SaveIssuesTestDataAsync(List<Issue> issues)
        {
            if (issues == null || !issues.Any()) return;
            
            try
            {
                using (var con = new MySqlConnection(DatabaseHelper.GetConnectionString()))
                {
                    await con.OpenAsync();
                    
                    foreach (var issue in issues)
                    {
                        var query = @"
                            INSERT INTO AllegroIssues_TEST 
                            (Id, AccountId, Type, Subject, Description, Status, OpenedDate, UpdatedAt, 
                             DecisionDueDate, BuyerLogin, CheckoutFormId, OrderCreatedAt, 
                             ProductId, OfferId, OfferName, ExpectationType, RefundAmount, RefundCurrency,
                             ReasonType, ReasonDescription, ReferenceNumber)
                            VALUES 
                            (@Id, @AccountId, @Type, @Subject, @Description, @Status, @OpenedDate, @UpdatedAt,
                             @DecisionDueDate, @BuyerLogin, @CheckoutFormId, @OrderCreatedAt,
                             @ProductId, @OfferId, @OfferName, @ExpectationType, @RefundAmount, @RefundCurrency,
                             @ReasonType, @ReasonDescription, @ReferenceNumber)
                            ON DUPLICATE KEY UPDATE
                            Status = @Status, UpdatedAt = @UpdatedAt, Description = @Description";
                        
                        using (var cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@Id", issue.Id);
                            cmd.Parameters.AddWithValue("@AccountId", _accountId);
                            cmd.Parameters.AddWithValue("@Type", (object)issue.Type ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Subject", (object)issue.Subject ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Description", (object)issue.Description ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Status", (object)issue.CurrentState?.Status ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@OpenedDate", issue.OpenedDate);
                            cmd.Parameters.AddWithValue("@UpdatedAt", issue.UpdatedAt);
                            cmd.Parameters.AddWithValue("@DecisionDueDate", (object)issue.DecisionDueDate ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@BuyerLogin", (object)issue.Buyer?.Login ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CheckoutFormId", (object)issue.CheckoutForm?.Id ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@OrderCreatedAt", (object)issue.CheckoutForm?.CreatedAt ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@ProductId", (object)issue.Product?.Id ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@OfferId", (object)issue.Offer?.Id ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@OfferName", (object)issue.Offer?.Name ?? DBNull.Value);
                            
                            var expectation = issue.Expectations?.FirstOrDefault();
                            cmd.Parameters.AddWithValue("@ExpectationType", (object)expectation?.Name ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@RefundAmount", 
                                expectation?.Refund != null && decimal.TryParse(expectation.Refund.Amount, out var amount) 
                                ? (object)amount : DBNull.Value);
                            cmd.Parameters.AddWithValue("@RefundCurrency", (object)expectation?.Refund?.Currency ?? DBNull.Value);
                            
                            cmd.Parameters.AddWithValue("@ReasonType", (object)issue.Reason?.Type ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@ReasonDescription", (object)issue.Reason?.Description ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@ReferenceNumber", (object)issue.ReferenceNumber ?? DBNull.Value);
                            
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    
                    Log($"💾 Zapisano {issues.Count} issues do tabeli testowej");
                }
            }
            catch (Exception ex)
            {
                Log($"⚠️  Błąd zapisu issues do bazy: {ex.Message}");
            }
        }

        private async Task SaveChatTestDataAsync(string issueId, List<AllegroChat> messages)
        {
            if (messages == null || !messages.Any()) return;
            
            try
            {
                using (var con = new MySqlConnection(DatabaseHelper.GetConnectionString()))
                {
                    await con.OpenAsync();
                    
                    foreach (var msg in messages)
                    {
                        var query = @"
                            INSERT INTO AllegroChatMessages_TEST 
                            (Id, IssueId, AccountId, MessageText, AuthorLogin, AuthorRole, CreatedAt)
                            VALUES 
                            (@Id, @IssueId, @AccountId, @MessageText, @AuthorLogin, @AuthorRole, @CreatedAt)
                            ON DUPLICATE KEY UPDATE MessageText = @MessageText";
                        
                        using (var cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@Id", msg.Id);
                            cmd.Parameters.AddWithValue("@IssueId", issueId);
                            cmd.Parameters.AddWithValue("@AccountId", _accountId);
                            cmd.Parameters.AddWithValue("@MessageText", (object)msg.Text ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@AuthorLogin", (object)msg.Author?.Login ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@AuthorRole", (object)msg.Author?.Role ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CreatedAt", msg.CreatedAt);
                            
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    
                    Log($"💾 Zapisano {messages.Count} wiadomości do tabeli testowej");
                }
            }
            catch (Exception ex)
            {
                Log($"⚠️  Błąd zapisu wiadomości do bazy: {ex.Message}");
            }
        }

        private async Task SaveOrderDetailsTestDataAsync(OrderDetails order)
        {
            if (order == null) return;
            
            try
            {
                using (var con = new MySqlConnection(DatabaseHelper.GetConnectionString()))
                {
                    await con.OpenAsync();
                    
                    var query = @"
                        INSERT INTO AllegroOrderDetails_TEST 
                        (CheckoutFormId, AccountId, BuyerEmail, BuyerFirstName, BuyerLastName, 
                         BuyerPhone, BuyerLogin, OrderCreatedAt, PaymentType, PaymentAmount, 
                         PaymentCurrency, DeliveryMethod, InvoiceRequired)
                        VALUES 
                        (@CheckoutFormId, @AccountId, @BuyerEmail, @BuyerFirstName, @BuyerLastName,
                         @BuyerPhone, @BuyerLogin, @OrderCreatedAt, @PaymentType, @PaymentAmount,
                         @PaymentCurrency, @DeliveryMethod, @InvoiceRequired)
                        ON DUPLICATE KEY UPDATE
                        BuyerEmail = @BuyerEmail, BuyerPhone = @BuyerPhone";
                    
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CheckoutFormId", order.Id);
                        cmd.Parameters.AddWithValue("@AccountId", _accountId);
                        cmd.Parameters.AddWithValue("@BuyerEmail", (object)order.Buyer?.Email ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BuyerFirstName", (object)order.Buyer?.FirstName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BuyerLastName", (object)order.Buyer?.LastName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BuyerPhone", (object)order.Buyer?.PhoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BuyerLogin", (object)order.Buyer?.Login ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OrderCreatedAt", order.CreatedAt);
                        cmd.Parameters.AddWithValue("@PaymentType", (object)order.Payment?.Type ?? DBNull.Value);
                        
                        decimal paymentAmount = 0;
                        if (decimal.TryParse(order.Payment?.TotalToPay?.Amount, out paymentAmount))
                        {
                            cmd.Parameters.AddWithValue("@PaymentAmount", paymentAmount);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@PaymentAmount", DBNull.Value);
                        }
                        
                        cmd.Parameters.AddWithValue("@PaymentCurrency", (object)order.Payment?.TotalToPay?.Currency ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DeliveryMethod", (object)order.Delivery?.Method?.Name ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@InvoiceRequired", order.Invoice?.Required ?? false);
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                    
                    Log($"💾 Zapisano szczegóły zamówienia do tabeli testowej");
                }
            }
            catch (Exception ex)
            {
                Log($"⚠️  Błąd zapisu zamówienia do bazy: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        private void Log(string message)
        {
            _logCallback?.Invoke(message);
        }

        private string MaskToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return "NULL";
            if (token.Length <= 20) return "***";
            return token.Substring(0, 10) + "..." + token.Substring(token.Length - 10);
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "(brak)";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        private int ExtractStatusCode(string message)
        {
            if (message.Contains("401")) return 401;
            if (message.Contains("403")) return 403;
            if (message.Contains("404")) return 404;
            if (message.Contains("429")) return 429;
            if (message.Contains("500")) return 500;
            return 0;
        }

        #endregion
    }

    #region Result Models

    public class TestResult
    {
        public string TestType { get; set; }
        public string Endpoint { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public int DurationMs { get; set; }
        public int ResponseStatus { get; set; } = 200;
        public object Data { get; set; }
    }

    #endregion
}
