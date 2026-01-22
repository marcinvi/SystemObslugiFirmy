namespace ReklamacjeAPI.DTOs;

public class ReturnListItemDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Waybill { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? StatusAllegro { get; set; }
    public string? StatusWewnetrzny { get; set; }
    public string? StanProduktu { get; set; }
    public string? DecyzjaHandlowca { get; set; }
    public int? HandlowiecId { get; set; }
    public bool IsManual { get; set; }
}

public class ReturnDetailsDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? StatusWewnetrzny { get; set; }
    public string? StatusAllegro { get; set; }
    public string? BuyerLogin { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string? BuyerPhone { get; set; }
    public string? BuyerAddress { get; set; }
    public string? BuyerAddressRaw { get; set; }
    public string? BuyerPhoneRaw { get; set; }
    public string? DeliveryName { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? DeliveryPhone { get; set; }
    public string? Waybill { get; set; }
    public string? CarrierName { get; set; }
    public string? ProductName { get; set; }
    public string? OfferId { get; set; }
    public int? Quantity { get; set; }
    public string? Reason { get; set; }
    public string? UwagiMagazynu { get; set; }
    public int? StanProduktuId { get; set; }
    public string? StanProduktuName { get; set; }
    public int? PrzyjetyPrzezId { get; set; }
    public string? PrzyjetyPrzezName { get; set; }
    public DateTime? DataPrzyjecia { get; set; }
    public int? DecyzjaHandlowcaId { get; set; }
    public string? DecyzjaHandlowcaName { get; set; }
    public string? KomentarzHandlowca { get; set; }
    public DateTime? DataDecyzji { get; set; }
    public bool IsManual { get; set; }
}

public class ReturnWarehouseUpdateRequest
{
    public int StanProduktuId { get; set; }
    public string? UwagiMagazynu { get; set; }
    public DateTime DataPrzyjecia { get; set; }
    public int PrzyjetyPrzezId { get; set; }
}

public class ReturnForwardToSalesRequest
{
    public int StanProduktuId { get; set; }
    public string? UwagiMagazynu { get; set; }
}

public class ReturnDecisionRequest
{
    public int DecyzjaId { get; set; }
    public string? Komentarz { get; set; }
}

public class ReturnDecisionResponse
{
    public int ReturnId { get; set; }
    public string StatusWewnetrzny { get; set; } = string.Empty;
    public string DecyzjaHandlowca { get; set; } = string.Empty;
    public DateTime DataDecyzji { get; set; }
}

public class ReturnManualCreateRequest
{
    public string NumerListu { get; set; } = string.Empty;
    public string? Produkt { get; set; }
    public string? Przewoznik { get; set; }
    public int StanProduktuId { get; set; }
    public string? UwagiMagazynu { get; set; }
    public string BuyerFullName { get; set; } = string.Empty;
    public string? BuyerStreet { get; set; }
    public string? BuyerZipCode { get; set; }
    public string? BuyerCity { get; set; }
    public string? BuyerPhone { get; set; }
    public List<int> WybraniHandlowcy { get; set; } = new();
}

public class ReturnActionDto
{
    public int Id { get; set; }
    public int ReturnId { get; set; }
    public DateTime Data { get; set; }
    public string Uzytkownik { get; set; } = string.Empty;
    public string Tresc { get; set; } = string.Empty;
}

public class ReturnActionCreateRequest
{
    public string Tresc { get; set; } = string.Empty;
}

public class MessageDto
{
    public int Id { get; set; }
    public int NadawcaId { get; set; }
    public int OdbiorcaId { get; set; }
    public string? Tytul { get; set; }
    public string Tresc { get; set; } = string.Empty;
    public DateTime DataWyslania { get; set; }
    public int? DotyczyZwrotuId { get; set; }
    public bool CzyPrzeczytana { get; set; }
}

public class MessageCreateRequest
{
    public int NadawcaId { get; set; }
    public int OdbiorcaId { get; set; }
    public string? Tytul { get; set; }
    public string Tresc { get; set; } = string.Empty;
    public int? DotyczyZwrotuId { get; set; }
}

public class ReturnSummaryItemDto
{
    public int Id { get; set; }
    public string NumerZwrotu { get; set; } = string.Empty;
    public string Produkt { get; set; } = string.Empty;
    public string KtoPrzyjal { get; set; } = string.Empty;
    public string KtoPodjalDecyzje { get; set; } = string.Empty;
    public string JakaDecyzja { get; set; } = string.Empty;
    public string UwagiMagazynu { get; set; } = string.Empty;
    public string UwagiHandlowca { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class ReturnSummaryStatsDto
{
    public int Total { get; set; }
    public int DoDecyzji { get; set; }
    public int Zakonczone { get; set; }
}

public class WarehouseSearchItemDto
{
    public string NrZgloszenia { get; set; } = string.Empty;
    public string? Sn { get; set; }
    public string? Model { get; set; }
    public string? Klient { get; set; }
    public int ProduktId { get; set; }
    public string? Kategoria { get; set; }
}

public class WarehouseIntakeRequest
{
    public string NrZgloszenia { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? NumerSeryjny { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Lokalizacja { get; set; } = string.Empty;
    public bool CzyDawca { get; set; }
    public string? Uwagi { get; set; }
    public List<string> Czesci { get; set; } = new();
}

public class ReturnSummaryResponse
{
    public List<ReturnSummaryItemDto> Items { get; set; } = new();
    public ReturnSummaryStatsDto Stats { get; set; } = new();
}

public class StatusDto
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = string.Empty;
    public string Typ { get; set; } = string.Empty;
}

public class ForwardToComplaintRequest
{
    public int ReturnId { get; set; }
    public string? PowodKlienta { get; set; }
    public string? UwagiMagazynu { get; set; }
    public string? UwagiHandlowca { get; set; }
    public string Przekazal { get; set; } = string.Empty;
    public ComplaintCustomerDto DaneKlienta { get; set; } = new();
    public ComplaintProductDto Produkt { get; set; } = new();
}

public class ComplaintCustomerDto
{
    public string Imie { get; set; } = string.Empty;
    public string Nazwisko { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefon { get; set; }
    public ComplaintAddressDto Adres { get; set; } = new();
}

public class ComplaintAddressDto
{
    public string? Ulica { get; set; }
    public string? Kod { get; set; }
    public string? Miasto { get; set; }
}

public class ComplaintProductDto
{
    public string Nazwa { get; set; } = string.Empty;
    public string? NrFaktury { get; set; }
    public string? NrSeryjny { get; set; }
}
