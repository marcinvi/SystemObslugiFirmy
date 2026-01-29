namespace ReklamacjeAPI.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public int NadawcaId { get; set; }
    public int OdbiorcaId { get; set; }
    public string NadawcaNazwa { get; set; } = string.Empty;
    public string OdbiorcaNazwa { get; set; } = string.Empty;
    public string? Tytul { get; set; }
    public string Tresc { get; set; } = string.Empty;
    public DateTime DataWyslania { get; set; }
    public int? DotyczyZwrotuId { get; set; }
    public bool CzyPrzeczytana { get; set; }
}

public class ReturnResendRequest
{
    public DateTime DataWysylki { get; set; }
    public string? Przewoznik { get; set; }
    public string? NumerListu { get; set; }
}
