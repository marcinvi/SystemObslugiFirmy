using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamacjeAPI.Models;

[Table("Produkty")]
public class Produkt
{
    [Key]
    [Column("IdProduktu")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("Nazwa")]
    public string Nazwa { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("Producent")]
    public string? Producent { get; set; }

    [MaxLength(100)]
    [Column("Model")]
    public string? Model { get; set; }

    [MaxLength(50)]
    [Column("NumerSeryjny")]
    public string? NumerSeryjny { get; set; }

    [Column("Opis")]
    public string? Opis { get; set; }

    [Column("DataDodania")]
    public DateTime DataDodania { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Zgloszenie> Zgloszenia { get; set; } = new List<Zgloszenie>();
}
