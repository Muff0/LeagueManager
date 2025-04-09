using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class LeagueContext : DbContext
    {
        public LeagueContext(DbContextOptions<LeagueContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<PlayerSeason> PlayerSeasons { get; set; }
        public DbSet<PlayerMatch> PlayerMatches { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
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
                .HasKey(pm => new {pm.PlayerId,pm.MatchId});

            modelBuilder.Entity<PlayerMatch>()
                .HasOne(pm => pm.Match)
                .WithMany(mm => mm.PlayerMatches)
                .HasForeignKey(pm => pm.MatchId);

            modelBuilder.Entity<PlayerMatch>()
                .HasOne(pm => pm.Player)
                .WithMany(pp => pp.PlayerMatches)
                .HasForeignKey(pm => pm.PlayerId);

            modelBuilder.Entity<PlayerSeason>()
                .HasKey(ps => new { ps.PlayerId,ps.SeasonId});

            modelBuilder.Entity<Season>()
                .HasMany(p => p.PlayerSeasons)
                .WithOne(ps => ps.Season)
                .HasForeignKey(ps => ps.SeasonId);
        }
    }
}
