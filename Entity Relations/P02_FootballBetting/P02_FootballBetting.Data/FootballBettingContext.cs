using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data.Models;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {

        public FootballBettingContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Town> Towns { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Bet> Bets { get; set; }
        public virtual DbSet<PlayerStatistic> PlayersStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlayerStatistic>()
                .HasKey(ps => new { ps.PlayerId, ps.GameId });
        }
    }
}
