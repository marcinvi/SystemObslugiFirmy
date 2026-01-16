using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamacjeAPI.Models;

[Table("Pliki")]
public class Plik
{
    [Key]
    [Column("IdPliku")]
    public int Id { get; set; }

    [Required]
    [Column("IdZgloszenia")]
    public int IdZgloszenia { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("NazwaPliku")]
    public string NazwaPliku { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("SciezkaPliku")]
    public string SciezkaPliku { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("TypPliku")]
    public string? TypPliku { get; set; }

    [Column("RozmiarPliku")]
    public long? RozmiarPliku { get; set; }

    [Column("DataDodania")]
    public DateTime DataDodania { get; set; } = DateTime.UtcNow;

    [Column("DodanyPrzez")]
    public int? DodanyPrzez { get; set; }

    // Navigation properties
    [ForeignKey("IdZgloszenia")]
    public virtual Zgloszenie Zgloszenie { get; set; } = null!;

    [ForeignKey("DodanyPrzez")]
    public virtual User? UploadedBy { get; set; }
}
