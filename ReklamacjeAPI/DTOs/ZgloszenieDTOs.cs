namespace ReklamacjeAPI.DTOs;


public class ZgloszenieListDto
{
    public int IdZgloszenia { get; set; }
    public string NrZgloszenia { get; set; } = string.Empty;
    public DateTime DataZgloszenia { get; set; }
    public string? StatusOgolny { get; set; }
    public int? Priorytet { get; set; }
    
    public KlientMinimalDto? Klient { get; set; }
    public ProduktMinimalDto? Produkt { get; set; }
    public UserMinimalDto? PrzypisanyDo { get; set; }
}






