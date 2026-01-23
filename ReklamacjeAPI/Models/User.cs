using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamacjeAPI.Models;

[Table("Uzytkownicy")]
public class User
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("Login")]
    public string Login { get; set; } = string.Empty;

    [Required]
    [Column("Hasło")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("Nazwa Wyświetlana")]
    public string? DisplayName { get; set; }

    [Column("Rola")]
    public string? Role { get; set; }

    [Column("Email")]
    public string? Email { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    [NotMapped]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public DateTime? LastLogin { get; set; }

    [NotMapped]
    public string DisplayNameOrLogin => string.IsNullOrWhiteSpace(DisplayName) ? Login : DisplayName;

    // Navigation properties
    public virtual ICollection<Zgloszenie> AssignedZgloszenia { get; set; } = new List<Zgloszenie>();
}
