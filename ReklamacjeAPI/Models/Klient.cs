using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamacjeAPI.Models;

[Table("Klienci")]
public class Klient
{
    [Key]
    [Column("IdKlienta")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("ImieNazwisko")]
    public string ImieNazwisko { get; set; } = string.Empty;

    [MaxLength(20)]
    [Column("Telefon")]
    public string? Telefon { get; set; }

    [MaxLength(100)]
    [Column("Email")]
    public string? Email { get; set; }

    [MaxLength(200)]
    [Column("Adres")]
    public string? Adres { get; set; }

    [MaxLength(10)]
    [Column("KodPocztowy")]
    public string? KodPocztowy { get; set; }

    [MaxLength(100)]
    [Column("Miasto")]
    public string? Miasto { get; set; }

    [Column("DataDodania")]
    public DateTime DataDodania { get; set; } = DateTime.UtcNow;

    [Column("Uwagi")]
    public string? Uwagi { get; set; }

    // Navigation properties
    public virtual ICollection<Zgloszenie> Zgloszenia { get; set; } = new List<Zgloszenie>();
}
