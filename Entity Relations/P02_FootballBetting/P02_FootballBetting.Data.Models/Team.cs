using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    [Table("Teams")]
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string LogoUrl { get; set; }

        [MaxLength(3)]
        [MinLength(3)]
        public string Initials { get; set; }

        public decimal Budget { get; set; }

        public int PrimaryKitColorId { get; set; }
        [ForeignKey(nameof(PrimaryKitColorId))]
        public Color PrimaryKitColor { get; set; }

        public int SecondaryKitColorId { get; set; }
        [ForeignKey(nameof(SecondaryKitColorId))]
        public Color SecondaryKitColor { get; set; }

        public int TownId { get; set; }
        [ForeignKey(nameof(TownId))]
        public Town Town { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        [InverseProperty(nameof(Game.HomeTeam))]
        public virtual ICollection<Game> HomeGames { get; set; }

        [InverseProperty(nameof(Game.AwayTeam))]
        public virtual ICollection<Game> AwayGames { get; set; }

    }
}
