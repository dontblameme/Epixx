using Epixx.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Epixx.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Pallet> Pallets { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<PalletSpot> PalletSpots { get; set; }
        public DbSet<PalletType> PalletTypes { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<LoadingDock> LoadingDocks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            // Driver (1) -> Pallet (many)
            modelBuilder.Entity<Driver>()
                .HasMany(d => d.pallets)
                .WithOne(p => p.Driver)
                .HasForeignKey(p => p.DriverId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<PalletType>()
                .HasIndex(pt => pt.Description)
                .IsUnique();
            // PalletSpot -> Pallet (optional, unidirectional)
            modelBuilder.Entity<PalletSpot>()
                .HasOne(ps => ps.CurrentPallet)
                .WithOne()
                .HasForeignKey<PalletSpot>(ps => ps.CurrentPalletId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
