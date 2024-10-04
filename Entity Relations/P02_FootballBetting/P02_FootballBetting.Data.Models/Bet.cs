using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models
{
    [Table("Bets")]
    public class Bet
    {
        [Key]
        public int BetId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(10)]

        public string Prediction { get; set; }

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public int GameId { get; set; }
        [ForeignKey(nameof(GameId))]
        public virtual Game Game { get; set; }
    }
}
