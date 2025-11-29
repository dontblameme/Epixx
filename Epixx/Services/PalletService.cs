using Epixx.Data;
using Epixx.Models;
using Microsoft.EntityFrameworkCore;
namespace Epixx.Services
{
    public class PalletService
    {
        private readonly WarehouseService _warehouseservice;
        private readonly InboundShipmentService _inboundshipmentservice;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly DriverService _driver;
        public PalletService(InboundShipmentService inboundShipmentService, WarehouseService warehouse, DriverService driver, IServiceScopeFactory scope)
        {
            _inboundshipmentservice = inboundShipmentService;
            _warehouseservice = warehouse;
            _driver = driver;
            _scopeFactory = scope;
        }
        public void RemovePallet(Pallet pallet)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Pallets.Remove(pallet);
            db.SaveChanges();
        }
        public void UpdatePalletStatus(string status, int palletId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var pallet = db.Pallets
                .FirstOrDefault(p => p.Id == palletId);
            if (pallet == null)
                return;
            pallet.Status = status;
            db.SaveChanges();
        }
        public Pallet FetchPalletByLocation(string location)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var pallet = db.Pallets
                .FirstOrDefault(p => p.Location == location);
            if (pallet == null)
                return null;

            return pallet;
        }
        public void PlaceDummyPalletsInWarehouse()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            bool anyPalletSpotsFilled = db.PalletSpots.Any(sp => sp.CurrentPalletId != null);
            if (anyPalletSpotsFilled)
            {
                return; // Exit if any pallet spots are already filled
            }
            var newdummypallets = _inboundshipmentservice.CreateDummyDataPallets();
            db.Pallets.AddRange(newdummypallets);
            db.SaveChanges();
            var pallets = db.Pallets.ToList();
            foreach (var pallet in pallets)
            {
                var location = FindPalletSpotLocation(pallet);
                if (location != "Ingen plats hittades")
                {
                    PlacePalletInWarehouseAsync(pallet.Id, location);
                }
            }
        }
        public async Task PlacePalletInWarehouseAsync(int palletId, string location)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Fetch the pallet
            var pallet = await db.Pallets.SingleOrDefaultAsync(p => p.Id == palletId);
            // Fetch the target spot including any current pallet
            var spot = await db.PalletSpots
                                .Include(sp => sp.CurrentPallet)
                                .SingleOrDefaultAsync(sp => sp.Location == location);
            if (pallet.Location == null)
                pallet.Location = location;
            // Detach pallet from any previous spot
            var oldSpot = await db.PalletSpots.SingleOrDefaultAsync(s => s.CurrentPalletId == pallet.Id);
            if (oldSpot != null)
            {
                oldSpot.CurrentPalletId = null;
                oldSpot.CurrentPallet = null;
            }

            // Assign pallet to the spot
            spot.CurrentPalletId = pallet.Id;
            spot.CurrentPallet = pallet;

            // Update pallet status
            pallet.Status = "Stored";

            // Save changes in a transaction to ensure consistency
            using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public string FindPalletSpotLocation(Pallet pallet)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var username = _driver.GetCurrentDriverName();
            var driver = db.Drivers
                .Include(d => d.pallets)
                .FirstOrDefault(d => d.Name == username);

            // Fetch all valid candidate spots
            var spots = db.PalletSpots
                .Include(s => s.CurrentPallet)
                .Where(s => s.Category == pallet.Category) // category match
                .Where(s => s.Height >= pallet.Height) // height requirement
                .ToList();
            string location = CheckSpotValidity(spots, pallet.Id, driver);
            if (location != "No spot found")
            {
                return location;
            }
            else
            {
                spots = db.PalletSpots
                .Include(s => s.CurrentPallet)
                .Where(s => s.Height >= pallet.Height)
                .ToList();
                location = CheckSpotValidity(spots, pallet.Id, driver);
                if (location != "No spot found")
                    return location;
            }

            return "Ingen plats hittades";
        }
        private string CheckSpotValidity(List<PalletSpot> spots, int palletId, Driver driver)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            foreach (var spot in spots)
            {
                // spot is occupied?
                if (spot.CurrentPallet != null)
                    continue;

                // spot location ending with '1'?
                if (spot.Location[^1] == '1')
                    continue;

                // driver has another pallet assigned here?
                if (driver.pallets.Any(p => p.Location == spot.Location))
                    continue;

                // another pallet already has this location?
                if (db.Pallets.Any(p => p.Location == spot.Location && p.Id != palletId))
                    continue;

                return spot.Location;
            }
            return "No spot found";
        }
    }
}
