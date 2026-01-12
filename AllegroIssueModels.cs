using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Reklamacje_Dane.Allegro.Issues
{
    public class IssuesListResponse
    {
        [JsonProperty("issues")]
        public List<Issue> Issues { get; set; }
    }

    public class Issue
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("referenceNumber")] public string ReferenceNumber { get; set; }
        [JsonProperty("decisionDueDate")] public DateTime? DecisionDueDate { get; set; }
        [JsonProperty("openedDate")] public DateTime OpenedDate { get; set; }
        [JsonProperty("closedAt")] public DateTime? ClosedAt { get; set; }
        [JsonProperty("subject")] public string Subject { get; set; }
        [JsonProperty("checkoutForm")] public CheckoutForm CheckoutForm { get; set; }
        [JsonProperty("buyer")] public Buyer Buyer { get; set; }
        [JsonProperty("currentState")] public CurrentState CurrentState { get; set; }
        [JsonProperty("chat")] public Chat Chat { get; set; }
        [JsonProperty("reason")] public Reason Reason { get; set; }
        [JsonProperty("expectations")] public List<Expectation> Expectations { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("product")] public Product Product { get; set; }
        [JsonProperty("offer")] public Offer Offer { get; set; }
    }

    public class Product
    {
        [JsonProperty("id")] public string Id { get; set; }
    }

    public class Offer
    {
        [JsonProperty("id")] public string Id { get; set; }
    }

    public class CheckoutForm
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        // POPRAWKA: Dodano brakującą właściwość LineItems, która jest niezbędna do dodania numeru przesyłki
        [JsonProperty("lineItems")]
        public List<LineItem> LineItems { get; set; }
    }

    // NOWOŚĆ: Dodano brakujące klasy pomocnicze dla LineItems
    public class LineItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("offer")]
        public OfferInfo Offer { get; set; }
    }

    public class OfferInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Buyer
    {
        [JsonProperty("login")] public string Login { get; set; }
    }

    public class CurrentState
    {
        [JsonProperty("status")] public string Status { get; set; }
    }

    public class Chat
    {
        [JsonProperty("messagesCount")] public int MessagesCount { get; set; }
        [JsonProperty("initialMessage")] public InitialMessage InitialMessage { get; set; }
    }

    public class InitialMessage
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("text")] public string Text { get; set; }
        [JsonProperty("createdAt")] public DateTime CreatedAt { get; set; }
    }
    public class ChatMessageResponse
    {
        [JsonProperty("chat")] public List<ChatMessage> Chat { get; set; }
    }

    public class ChatMessage
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("text")] public string Text { get; set; }
        [JsonProperty("author")] public PostPurchaseIssueAuthor Author { get; set; }
        [JsonProperty("createdAt")] public DateTime CreatedAt { get; set; }
        [JsonProperty("attachments")] public List<PostPurchaseIssueAttachment> Attachments { get; set; }
    }

    public class PostPurchaseIssueAuthor
    {
        [JsonProperty("login")] public string Login { get; set; }
        [JsonProperty("role")] public string Role { get; set; }
    }

    public class PostPurchaseIssueAttachment
    {
        [JsonProperty("fileName")] public string FileName { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
    }

    public class NewMessageRequest
    {
        [JsonProperty("text")] public string Text { get; set; }
        [JsonProperty("type")] public string Type { get; set; } = "REGULAR";
        [JsonProperty("attachments", NullValueHandling = NullValueHandling.Ignore)]
        public List<NewMessageAttachment> Attachments { get; set; }
    }

    public partial class NewMessageAttachment
    {
        [JsonProperty("id")] public string Id { get; set; }
    }

    public class ChangeStatusRequest
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("partialRefund")] public PartialRefund PartialRefund { get; set; }
    }

    public class PartialRefund
    {
        [JsonProperty("amount")] public string Amount { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; } = "PLN";
    }
    public class Reason
    {
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }

    public class Expectation
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("refund")] public PostPurchaseIssueRefund Refund { get; set; }
    }

    public class PostPurchaseIssueRefund
    {
        [JsonProperty("amount")] public string Amount { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }

    }
}

