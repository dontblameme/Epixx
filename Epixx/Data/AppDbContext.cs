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
        public DbSet<Store> Stores { get; set; }
       
    }
}
