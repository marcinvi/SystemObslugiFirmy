using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamacjeAPI.Models;

[Table("Zgloszenia")]
public class Zgloszenie
{
    [Key]
    [Column("IdZgloszenia")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("NrZgloszenia")]
    public string NrZgloszenia { get; set; } = string.Empty;

    [Column("DataZgloszenia")]
    public DateTime DataZgloszenia { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("IdKlienta")]
    public int IdKlienta { get; set; }

    [Column("IdProduktu")]
    public int? IdProduktu { get; set; }

    [Column("Usterka")]
    public string? Usterka { get; set; }

    [MaxLength(50)]
    [Column("StatusOgolny")]
    public string StatusOgolny { get; set; } = "Nowe";

    [Column("PrzypisanyDo")]
    public int? PrzypisanyDo { get; set; }

    [Column("DataModyfikacji")]
    public DateTime DataModyfikacji { get; set; } = DateTime.UtcNow;

    [Column("DataZamkniecia")]
    public DateTime? DataZamkniecia { get; set; }

    [Column("Priorytet")]
    public string? Priorytet { get; set; }

    [Column("Uwagi")]
    public string? Uwagi { get; set; }

    // Navigation properties
    [ForeignKey("IdKlienta")]
    public virtual Klient Klient { get; set; } = null!;

    [ForeignKey("IdProduktu")]
    public virtual Produkt? Produkt { get; set; }

    [ForeignKey("PrzypisanyDo")]
    public virtual User? AssignedUser { get; set; }

    public virtual ICollection<Dzialanie> Dzialania { get; set; } = new List<Dzialanie>();
    public virtual ICollection<Plik> Pliki { get; set; } = new List<Plik>();
}
