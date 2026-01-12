// Plik: AllegroOrderModels.cs (WERSJA Z POPRAWK¥ B£ÊDÓW)
// Opis: Dodano brakuj¹ce w³aœciwoœci Id, Price i Cost oraz definicje
//       klas Price i Cost, aby naprawiæ b³êdy kompilacji.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Reklamacje_Dane.Allegro
{
    public class AllegroToken
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }
        [JsonProperty("refresh_token")] public string RefreshToken { get; set; }
        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public class Seller { [JsonProperty("id")] public string Id { get; set; } }

    public class OrderDetails
    {
        // ########## POPRAWKA - Dodano brakuj¹ce pole ID ##########
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("buyer")] public OrderBuyer Buyer { get; set; }
        [JsonProperty("delivery")] public Delivery Delivery { get; set; }
        [JsonProperty("seller")] public Seller Seller { get; set; }
        [JsonProperty("boughtAt")] public DateTime? BoughtAt { get; set; }
        [JsonProperty("lineItems")] public List<LineItem> LineItems { get; set; }
        [JsonProperty("payment")] public Payment Payment { get; set; }
        [JsonProperty("fulfillment")] public Fulfillment Fulfillment { get; set; }
        [JsonProperty("invoice")] public Invoice Invoice { get; set; }
    }

    public class Payment { [JsonProperty("id")] public string Id { get; set; } [JsonProperty("type")] public string Type { get; set; } }
    public class Fulfillment { [JsonProperty("status")] public string Status { get; set; } }

    public class OrderBuyer
    {
        [JsonProperty("firstName")] public string FirstName { get; set; }
        [JsonProperty("lastName")] public string LastName { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("phoneNumber")] public string PhoneNumber { get; set; }
        [JsonProperty("address")] public BuyerAddress Address { get; set; }
    }

    public class BuyerAddress
    {
        [JsonProperty("street")] public string Street { get; set; }
        [JsonProperty("postCode")] public string PostCode { get; set; }
        [JsonProperty("city")] public string City { get; set; }
    }

    public class Delivery
    {
        [JsonProperty("address")] public Address Address { get; set; }
        // ########## POPRAWKA - Dodano brakuj¹ce pole Cost ##########
        [JsonProperty("cost")] public Cost Cost { get; set; }
    }

    // ########## POPRAWKA - Dodano brakuj¹c¹ klasê Cost ##########
    public class Cost
    {
        [JsonProperty("amount")] public string Amount { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }
    }

    public class Invoice
    {
        [JsonProperty("address")] public InvoiceAddress Address { get; set; }

        // ########## DODANE ##########
        // Dodano pole invoiceNumber zgodnie z dokumentacj¹ Allegro.
        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }
    }

    public class Company
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("taxId")] public string TaxId { get; set; }
    }

    public class LineItem
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("offer")] public Offer Offer { get; set; }
        [JsonProperty("quantity")] public int Quantity { get; set; }
        // ########## POPRAWKA - Dodano brakuj¹ce pole Price ##########
        [JsonProperty("price")] public Price Price { get; set; }
    }

    // ########## POPRAWKA - Dodano brakuj¹c¹ klasê Price ##########
    public class Price
    {
        [JsonProperty("amount")] public string Amount { get; set; }
        [JsonProperty("currency")] public string Currency { get; set; }
    }

    public class Offer
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
    }
    public class Address
    {
        [JsonProperty("firstName")] public string FirstName { get; set; }
        [JsonProperty("lastName")] public string LastName { get; set; }
        [JsonProperty("street")] public string Street { get; set; }
        [JsonProperty("zipCode")] public string ZipCode { get; set; }
        [JsonProperty("city")] public string City { get; set; }
        [JsonProperty("phoneNumber")] public string PhoneNumber { get; set; }
        [JsonProperty("companyName")] public string CompanyName { get; set; }
    }

    public class InvoiceAddress
    {
        [JsonProperty("company")] public Company Company { get; set; }
        [JsonProperty("street")] public string Street { get; set; }
        [JsonProperty("zipCode")] public string ZipCode { get; set; }
        [JsonProperty("city")] public string City { get; set; }

        [JsonIgnore] public string CompanyName => Company?.Name;
        [JsonIgnore] public string TaxId => Company?.TaxId;
    }
}

// ########## DODANE KLASY - MODELE ZWROTU WP£ATY ##########
// Poni¿sze klasy umieszczono w dodatkowej przestrzeni nazw
// Reklamacje_Dane.Allegro.Models, aby umo¿liwiæ serializacjê ¿¹dania
// zwrotu p³atnoœci (POST /payments/refunds) zgodnie z dokumentacj¹ Allegro.
namespace Reklamacje_Dane.Allegro.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Reprezentuje ¿¹danie zwrotu p³atnoœci.
    /// </summary>
    public class PaymentRefundRequest
    {
        [JsonProperty("payment")]
        public PaymentId Payment { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("lineItems", NullValueHandling = NullValueHandling.Ignore)]
        public List<RefundLineItem> LineItems { get; set; }

        [JsonProperty("delivery", NullValueHandling = NullValueHandling.Ignore)]
        public RefundValue Delivery { get; set; }

        [JsonProperty("sellerComment", NullValueHandling = NullValueHandling.Ignore)]
        public string SellerComment { get; set; }
    }

    public class PaymentId
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class RefundLineItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("quantity", NullValueHandling = NullValueHandling.Ignore)]
        public int? Quantity { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public RefundValue Value { get; set; }
    }

    public class RefundValue
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
