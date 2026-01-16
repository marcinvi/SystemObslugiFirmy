namespace ReklamacjeAPI.Models;

public class Uzytkownik
{
    public int IdUzytkownika { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Haslo { get; set; } = string.Empty; // Hashed password
    public string? Email { get; set; }
    public string NazwaWyswietlana { get; set; } = string.Empty;
    public string? Imie { get; set; }
    public string? Nazwisko { get; set; }
    public bool CzyAktywny { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public DateTime DataDodania { get; set; } = DateTime.Now;
    
    // Navigation properties
    public virtual ICollection<Zgloszenie>? ZgloszeniaStworzone { get; set; }
    public virtual ICollection<Zgloszenie>? ZgloszeniaPrzypisane { get; set; }
    public virtual ICollection<Dzialanie>? Dzialania { get; set; }
}
