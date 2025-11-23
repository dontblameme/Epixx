using Epixx.Models;
using Microsoft.EntityFrameworkCore;

namespace Epixx.Data
{
    public class AppDbContext : DbContext
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Driver (1) -> Pallet (many)
            modelBuilder.Entity<Driver>()
                .HasMany(d => d.pallets)
                .WithOne(p => p.Driver)
                .HasForeignKey(p => p.DriverId)
                .OnDelete(DeleteBehavior.SetNull);

            // PalletSpot -> Pallet (optional, unidirectional)
            modelBuilder.Entity<PalletSpot>()
                .HasOne(ps => ps.CurrentPallet)
                .WithOne()
                .HasForeignKey<PalletSpot>(ps => ps.CurrentPalletId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}
