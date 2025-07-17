using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class LeagueContext : DbContext
    {
        public LeagueContext(DbContextOptions<LeagueContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<PlayerSeason> PlayerSeasons { get; set; }
        public DbSet<PlayerMatch> PlayerMatches { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<ReviewSchedule> ReviewSchedules { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Match>()
                .HasIndex(mm => mm.LeagoKey);

            modelBuilder.Entity<Player>()
                .HasMany(p => p.PlayerSeasons)
                .WithOne(ps => ps.Player)
                .HasForeignKey(ps => ps.PlayerId);

            modelBuilder.Entity<PlayerMatch>()
                .HasKey(pm => new { pm.PlayerId, pm.MatchId });

            modelBuilder.Entity<PlayerMatch>()
                .HasOne(pm => pm.Match)
                .WithMany(mm => mm.PlayerMatches)
                .HasForeignKey(pm => pm.MatchId);

            modelBuilder.Entity<PlayerMatch>()
                .HasOne(pm => pm.Player)
                .WithMany(pp => pp.PlayerMatches)
                .HasForeignKey(pm => pm.PlayerId);

            modelBuilder.Entity<PlayerSeason>()
                .HasKey(ps => new { ps.PlayerId, ps.SeasonId });

            modelBuilder.Entity<Review>()
                .HasOne(re => re.OwnerPlayer)
                .WithMany(pl => pl.Reviews)
                .HasForeignKey(re => re.OwnerPlayerId);

            modelBuilder.Entity<Review>()
                .HasOne(re => re.ReviewSchedule)
                .WithOne(rs => rs.Review)
                .HasForeignKey(nameof(ReviewSchedule), nameof(ReviewSchedule.ReviewId));

            modelBuilder.Entity<Review>()
                .HasOne(re => re.PlayerSeason)
                .WithMany(ps => ps.Reviews)
                .HasForeignKey(re => new { re.OwnerPlayerId, re.SeasonId });

            modelBuilder.Entity<Season>()
                .HasMany(p => p.PlayerSeasons)
                .WithOne(ps => ps.Season)
                .HasForeignKey(ps => ps.SeasonId);
        }
    }
}