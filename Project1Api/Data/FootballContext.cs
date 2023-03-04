using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Project1Api.Models;
using System.Linq;
using System.Numerics;

namespace Project1Api.Data
{
    public class FootballContext : DbContext
    {
        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public FootballContext(DbContextOptions<FootballContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
                UserName ??= "Unknown";
            }
            else
            {
                //No HttpContext so seeding data
                UserName = "Seed Data";
            }
        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerTeam> PlayerTeams { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique index for EMail
            modelBuilder.Entity<Player>()
                .HasIndex(p => p.EMail)
                .IsUnique();

            // Many to Many Primary Key
            modelBuilder.Entity<PlayerTeam>()
                .HasKey(p => new { p.PlayerID, p.TeamID });

            modelBuilder.Entity<Team>()     // You can't delete a Team that has PlayerTeams
                .HasMany(p => p.PlayerTeams)
                .WithOne(t => t.Team)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<League>()   // You can't delete a League that has Teams
                .HasMany(t => t.Teams)
                .WithOne(l => l.League)
                .OnDelete(DeleteBehavior.Restrict);

            // If a Player is deleted then all records of the Player being on any Teams should also be deleted
            // Therefore there's no cascade delete restriction for the Player towards PlayerTeam entity
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }

    internal record NewRecord(string LeagueID, IEnumerable<int> Item);
}
