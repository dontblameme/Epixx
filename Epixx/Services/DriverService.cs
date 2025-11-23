using Epixx.Data;
using Epixx.Models;
using Microsoft.EntityFrameworkCore;

namespace Epixx.Services
{
    public class DriverService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceScopeFactory _scopeFactory;

        public DriverService(IServiceScopeFactory scopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            _scopeFactory = scopeFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentDriverName()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("User");
        }

        private Driver? GetDriver(AppDbContext db)
        {
            var username = GetCurrentDriverName();
            if (username == null) return null;

            return db.Drivers
                .Include(d => d.pallets)
                .FirstOrDefault(d => d.Name == username);
        }

        // ----------------------------------------------
        // REMOVE ALL PALLETS
        // ----------------------------------------------
        public void RemoveAllPalletsFromDriver()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var driver = GetDriver(db);
            if (driver == null) return;

            var palletIds = driver.pallets.Select(p => p.Id).ToList();
            foreach(var palletId in palletIds)
            {
                var p = db.Pallets.SingleOrDefault(p => p.Id == palletId);
                p.Location = null;
            }
            
            // Clear all pallet spot references
            var spots = db.PalletSpots
                .Include(s => s.CurrentPallet)
                .Where(s => s.CurrentPallet != null && palletIds.Contains(s.CurrentPallet.Id))
                .ToList();

            foreach (var spot in spots)
            {
                spot.CurrentPalletId = null;
                spot.CurrentPallet = null;
            }

            // Clear driver's list
            driver.pallets.Clear();

            db.SaveChanges();
        }

        // ----------------------------------------------
        // REMOVE PALLET BY BARCODE
        // ----------------------------------------------
        public Pallet? RemovePalletFromDriverByBarcode(string barcode)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var driver = GetDriver(db);
            if (driver == null) return null;

            long code = long.Parse(barcode);

            var pallet = driver.pallets.FirstOrDefault(p => p.Barcode == code);
            if (pallet == null) return null;

            // Remove pallet from palletspot
            var spot = db.PalletSpots
                .Include(s => s.CurrentPallet)
                .FirstOrDefault(s => s.CurrentPallet != null && s.CurrentPallet.Id == pallet.Id);

            if (spot != null)
                spot.CurrentPallet = null;

            driver.pallets.Remove(pallet);

            db.SaveChanges();

            return pallet;
        }

        // ----------------------------------------------
        // REMOVE PALLET (using pallet.Id)
        // ----------------------------------------------
        public void RemovePalletFromDriver(Pallet pallet)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var driver = GetDriver(db);
            if (driver == null) return;

            // find attached EF entity
            var palletEF = driver.pallets.FirstOrDefault(p => p.Id == pallet.Id);
            if (palletEF == null) return;

            // Remove from spot
            var spot = db.PalletSpots
                .Include(s => s.CurrentPallet)
                .FirstOrDefault(s => s.CurrentPallet != null && s.CurrentPallet.Id == pallet.Id);

            if (spot != null)
                spot.CurrentPallet = null;

            driver.pallets.Remove(palletEF);

            db.SaveChanges();
        }

        // ----------------------------------------------
        // CHECK OWNERSHIP
        // ----------------------------------------------
        public bool CheckIfPalletIsAssignedToDriver(Pallet pallet)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var driver = GetDriver(db);
            if (driver == null) return false;

            return driver.pallets.Any(p => p.Id == pallet.Id);
        }

        // ----------------------------------------------
        // FETCH PALLETS
        // ----------------------------------------------
        public List<Pallet> FetchAllPalletsFromDriver()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return GetDriver(db)?.pallets.ToList() ?? new List<Pallet>();
        }

        public Pallet? FetchSpecificPalletByBarcode(string barcode)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var driver = GetDriver(db);
            if (driver == null) return null;

            long code = long.Parse(barcode);
            return driver.pallets.FirstOrDefault(p => p.Barcode == code);
        }

        // ----------------------------------------------
        // ADD PALLET
        // ----------------------------------------------
        public void AddPalletToDriver(Pallet pallet)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var driver = GetDriver(db);
            if (driver == null) return;

            // ensure attached pallet
            var palletEF = db.Pallets.Find(pallet.Id);
            if (palletEF == null) return;
            palletEF.Location = pallet.Location;
            driver.pallets.Add(palletEF);
            db.SaveChanges();
        }

        // ----------------------------------------------
        // CREATE DRIVER
        // ----------------------------------------------
        public string GetOrCreateDriver(string name)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var driver = db.Drivers.FirstOrDefault(d => d.Name == name);

            if (driver == null)
            {
                driver = new Driver { Name = name };
                db.Drivers.Add(driver);
                db.SaveChanges();
            }

            return driver.Name;
        }
    }
}
