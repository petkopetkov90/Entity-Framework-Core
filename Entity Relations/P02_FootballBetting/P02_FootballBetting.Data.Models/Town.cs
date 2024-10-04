using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    [Table("Towns")]
    public class Town
    {
        [Key]
        public int TownId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }

        public virtual ICollection<Team> Teams { get; set; }

        public virtual ICollection<Player> Players { get; set; }
    }
}
