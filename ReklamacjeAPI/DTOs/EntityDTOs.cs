namespace ReklamacjeAPI.DTOs;

// Klient DTOs

public class KlientMinimalDto
{
    public int IdKlienta { get; set; }
    public string ImieNazwisko { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Email { get; set; }
}



// Produkt DTOs

public class ProduktMinimalDto
{
    public int IdProduktu { get; set; }
    public string Nazwa { get; set; } = string.Empty;
    public string? Producent { get; set; }
    public string? Model { get; set; }
}

public class UserMinimalDto
{
    public int IdUzytkownika { get; set; }
    public string NazwaWyswietlana { get; set; } = string.Empty;
}
