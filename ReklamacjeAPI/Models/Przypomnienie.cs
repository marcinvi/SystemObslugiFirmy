using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReklamacjeAPI.Models
{
    [Table("Przypomnienia")]
    public class Przypomnienie
    {
        [Key]
        public int Id { get; set; }

        [Column("Tresc")]
        public string Tresc { get; set; }

        [Column("DataPrzypomnienia")]
        public DateTime DataPrzypomnienia { get; set; }

        [Column("Status")]
        public string? Status { get; set; } // np. 'Nowe', 'Active'

        [Column("PrzypisanyUzytkownik")]
        public string? PrzypisanyUzytkownik { get; set; }

        [Column("DotyczyZgloszenia")]
        public string? DotyczyZgloszenia { get; set; }

        [Column("Priorytet")]
        public string? Priorytet { get; set; }
    }
}