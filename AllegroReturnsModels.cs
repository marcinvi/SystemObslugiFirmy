// Plik: AllegroReturnsModels.cs (WERSJA Z POPRAWK¥ B£ÊDÓW CS0246)
// Opis: Dodano brakuj¹ce definicje klas RejectCustomerReturnRequest
//       i RejectionDetails, aby naprawiæ b³êdy kompilacji.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Reklamacje_Dane.Allegro.Returns
{
    public class AllegroCustomerReturnsResponse
    {
        [JsonProperty("customerReturns")]
        public List<AllegroCustomerReturn> CustomerReturns { get; set; }
    }

    public class AllegroCustomerReturn
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("referenceNumber")]
        public string ReferenceNumber { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("buyer")]
        public AllegroReturnBuyer Buyer { get; set; }

        [JsonProperty("parcels")]
        public List<AllegroReturnParcel> Parcels { get; set; }

        [JsonProperty("items")]
        public List<AllegroReturnItem> Items { get; set; }
    }

    public class AllegroReturnBuyer
    {
        [JsonProperty("login")]
        public string Login { get; set; }
    }

    public class AllegroReturnParcel
    {
        [JsonProperty("waybill")]
        public string Waybill { get; set; }

        [JsonProperty("carrierId")]
        public string CarrierId { get; set; }
    }

    public class AllegroReturnItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        [JsonProperty("reason")]
        public ReturnReason Reason { get; set; }
    }

    public class ReturnReason
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("userComment")]
        public string UserComment { get; set; }
    }

    public class AllegroInvoiceInfo
    {
        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }
    }

    // ########## POPRAWKA - DODANO BRAKUJ¥CE KLASY ##########
    public class RejectCustomerReturnRequest
    {
        [JsonProperty("rejection")]
        public RejectionDetails Rejection { get; set; }
    }

    public class RejectionDetails
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
    // ########################################################

    // ########## DODANE KLASY - LISTA ZWROTÓW I REJECJA ##########
    /// <summary>
    /// Reprezentuje wynik metody GET /order/customer-returns.
    /// Zawiera liczbê pasuj¹cych zwrotów oraz listê zwrotów.
    /// </summary>
    public class AllegroCustomerReturnList
    {
        [JsonProperty("count")]
        public int? Count { get; set; }

        [JsonProperty("customerReturns")]
        public List<AllegroCustomerReturn> CustomerReturns { get; set; }
    }

    /// <summary>
    /// Alias u³atwiaj¹cy serializacjê ¿¹dania odrzucenia zwrotu.
    /// </summary>
    public class RejectionRequest
    {
        [JsonProperty("rejection")]
        public RejectionDetails Rejection { get; set; }
    }
    // ########################################################
}
