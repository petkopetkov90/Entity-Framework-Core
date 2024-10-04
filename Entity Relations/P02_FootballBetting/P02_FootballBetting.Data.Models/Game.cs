using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        public int GameId { get; set; }

        public int HomeTeamGoals { get; set; }

        public int AwayTeamGoals { get; set; }

        public decimal HomeTeamBetRate { get; set; }

        public decimal AwayTeamBetRate { get; set; }

        public decimal DrawBetRate { get; set; }

        public DateTime DateTime { get; set; }

        [MaxLength(10)]
        public string Result { get; set; }

        public int HomeTeamId { get; set; }
        [ForeignKey(nameof(HomeTeamId))]
        public virtual Team HomeTeam { get; set; }

        public int AwayTeamId { get; set; }
        [ForeignKey(nameof(AwayTeamId))]
        public virtual Team AwayTeam { get; set; }

        public virtual ICollection<Bet> Bets { get; set; }

        public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; }
    }
}
