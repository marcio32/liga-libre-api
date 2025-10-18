using LigaLibre.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LigaLibre.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Club> Club { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<Match> Match { get; set; }
        public DbSet<Referee> Referee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Club>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
                entity.Property(x => x.City).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Email).IsRequired().HasMaxLength(100);
                entity.Property(x => x.Phone).HasMaxLength(20);
                entity.Property(x => x.Address).HasMaxLength(200);
                entity.Property(x => x.StadiumName).HasMaxLength(100);
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Nationality).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

                entity.HasOne(e => e.Club).WithMany(c => c.Players).HasForeignKey(e => e.ClubId);

            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Stadium).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.HomeClub)
                .WithMany(c => c.HomeMatches)
                .HasForeignKey(e => e.HomeClubId)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.AwayClub)
                .WithMany(c => c.AwayMatches)
                .HasForeignKey(e => e.AwayClubId)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Referee)
                .WithMany(r => r.Matches)
                .HasForeignKey(e => e.RefereeId); ;

            });

            modelBuilder.Entity<Referee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.LicenseNumber).HasMaxLength(20);
                entity.HasIndex(e=> e.LicenseNumber).IsUnique();
            });
        }


    }
}
