using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamacjeAPI.Models;

[Table("Uzytkownicy")]
public class User
{
    [Key]
    [Column("IdUzytkownika")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("Login")]
    public string Login { get; set; } = string.Empty;

    [Required]
    [Column("HasloHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("NazwaWyswietlana")]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("Email")]
    public string? Email { get; set; }

    [Column("Aktywny")]
    public bool IsActive { get; set; } = true;

    [Column("DataDodania")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("OstatnieLogowanie")]
    public DateTime? LastLogin { get; set; }

    // Navigation properties
    public virtual ICollection<Zgloszenie> AssignedZgloszenia { get; set; } = new List<Zgloszenie>();
}
