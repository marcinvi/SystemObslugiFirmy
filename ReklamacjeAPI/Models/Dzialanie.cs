using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamacjeAPI.Models;

[Table("Dzialania")]
public class Dzialanie
{
    [Key]
    [Column("IdDzialania")]
    public int Id { get; set; }

    [Required]
    [Column("IdZgloszenia")]
    public int IdZgloszenia { get; set; }

    [Required]
    [Column("IdUzytkownika")]
    public int IdUzytkownika { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("TypDzialania")]
    public string TypDzialania { get; set; } = string.Empty;

    [Column("Opis")]
    public string? Opis { get; set; }

    [Column("DataDzialania")]
    public DateTime DataDzialania { get; set; } = DateTime.UtcNow;

    [Column("StatusPoprzedni")]
    public string? StatusPoprzedni { get; set; }

    [Column("StatusNowy")]
    public string? StatusNowy { get; set; }

    // Navigation properties
    [ForeignKey("IdZgloszenia")]
    public virtual Zgloszenie Zgloszenie { get; set; } = null!;

    [ForeignKey("IdUzytkownika")]
    public virtual User User { get; set; } = null!;
}
