using Epixx.Data;
using Epixx.Models.DTO;
using Epixx.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Epixx.Services
{
    public class PalletService
    {
        private readonly DriverService _driver;
        private readonly AppDbContext _db;
        private readonly Random _rnd = new Random();

        public PalletService(
            AppDbContext db,
            DriverService driver)
        {
            _db = db;
            _driver = driver;
        }
        public void CreateNewPalletType(PalletType palletType)
        {
            _db.PalletTypes.Add(palletType);
            _db.SaveChanges();
        }
        public List<PalletType> GetAllPalletTypes()
        {
            return _db.PalletTypes.ToList();
        }
        public void RemovePallet(Pallet pallet)
        {
            _db.Pallets.Remove(pallet);
            _db.SaveChanges();
        }
        public int GetPalletTransferCount()
        {
            return _db.Pallets.Count(p => p.Status == "PalletTransfer");
        }
        public List<Pallet> GetPalletsForTransfer()
        {
            Dictionary<string, int> rowPalletCount = new Dictionary<string, int>();
            var pallets = new List<Pallet>();

            var ps = _db.Pallets
                .Where(p => p.Status == "PalletTransfer")
                .OrderBy(p => p.Id)
                .Take(10)
                .ToList();

            var rownames = _db.Rows.Select(r => r.Name).ToList();

            foreach (var row in rownames)
            {
                var actualRow = row.Substring(0, 2);

                var palletsInRow = ps
                    .Where(p => p.Location != null && p.Location.StartsWith(actualRow))
                    .ToList();

                rowPalletCount[actualRow] = palletsInRow.Count;
            }
            var mostPalletsInRow = rowPalletCount.MaxBy(x => x.Value).Key;
            pallets = ps.Where(p => p.Location != null && p.Location.StartsWith(mostPalletsInRow)).ToList();
            return pallets;
        }
       
        public List<Pallet> GetPalletsByStatus(string status)
        {
            return _db.Pallets.Where(p => p.Status == status).ToList();
        }

        public void UpdatePalletStatus(string status, int palletId)
        {
            var pallet = _db.Pallets.SingleOrDefault(p => p.Id == palletId);

            if (pallet == null)
                return;

            pallet.Status = status;
            _db.SaveChanges();
        }
        public Pallet? FetchPalletByBarcode(string barcode)
        {
            long code = long.Parse(barcode);
            return _db.Pallets.AsNoTracking().FirstOrDefault(p => p.Barcode == code);
        }
        public Pallet? FetchPalletByLocation(string location)
        {
            var pallet = _db.Pallets.FirstOrDefault(p => p.Location == location);
            return pallet;
        }
        public void ChangePalletLocationToDestination(Pallet pallet)
        {
            var oldSpot = _db.PalletSpots.Include(s => s.CurrentPallet).SingleOrDefault(s => s.CurrentPalletId == pallet.Id);
            var newSpot = _db.PalletSpots.Include(s => s.CurrentPallet).SingleOrDefault(s => s.Location == pallet.Destination);
            var p = _db.Pallets.SingleOrDefault(p => p.Id == pallet.Id);
            oldSpot.CurrentPalletId = null;
            oldSpot.CurrentPallet = null;
            newSpot.CurrentPalletId = pallet.Id;
            newSpot.CurrentPallet = pallet;
            p.Location = pallet.Destination;
            p.Destination = null;
            p.Status = "Stored";
            _db.SaveChanges();
        }
        public void PlacePalletInWarehouse(int palletId, string location, string status)
        {
            var pallet = _db.Pallets.SingleOrDefault(p => p.Id == palletId);
            var spot = _db.PalletSpots.Include(s => s.CurrentPallet).SingleOrDefault(s => s.Location == location);

            if (pallet == null || spot == null) return;

            using var transaction = _db.Database.BeginTransaction();
            try
            {
                // Assign new spot
                spot.CurrentPalletId = pallet.Id;
                spot.CurrentPallet = pallet;
                pallet.Status = status;
                pallet.Location = location;
                _db.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }

        public Pallet SetPalletDestination(int id, string destination)
        {
            var pallet = _db.Pallets.FirstOrDefault(p => p.Id == id);
            if (pallet == null)
                return null;

            pallet.Destination = destination;
            _db.SaveChanges();
            return pallet;
        }

        public List<PalletTransferDTO> AssignDestinationsToPallets(List<Pallet> pallets)
        {
            var dtoList = new List<PalletTransferDTO>();
            var now = DateTime.UtcNow;
            var driverId = _driver.GetDriverId();

            foreach (var pallet in pallets)
            {
                PalletSpot? candidateSpot = null;

                // 🔁 Retry (hanterar concurrency)
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    now = DateTime.UtcNow;

                    candidateSpot = _db.PalletSpots
                        .Where(s =>
                            s.Height >= pallet.Height &&
                            s.CurrentPallet == null &&
                            (s.ReservedUntil == null || s.ReservedUntil < now) &&
                            s.Category == pallet.Category &&
                            s.Location.EndsWith("1"))
                        .FirstOrDefault();

                    if (candidateSpot == null)
                    {
                        candidateSpot = _db.PalletSpots
                            .Where(s =>
                                s.Height >= pallet.Height &&
                                s.CurrentPallet == null &&
                                (s.ReservedUntil == null || s.ReservedUntil < now) &&
                                s.Location.EndsWith("1"))
                            .FirstOrDefault();
                    }

                    if (candidateSpot == null)
                        break;

                    // 🔒 Försök reservera
                    candidateSpot.ReservedByDriverId = driverId;
                    candidateSpot.ReservedUntil = now.AddMinutes(10);

                    try
                    {
                        _db.SaveChanges();
                        break; // success
                    }
                    catch
                    {
                        // Någon annan hann före → retry
                        _db.Entry(candidateSpot).State = EntityState.Detached;
                        candidateSpot = null;
                    }
                }

                if (candidateSpot == null)
                    continue;

                _driver.AddPalletToDriver(pallet);

                pallet.Destination = candidateSpot.Location;

                dtoList.Add(new PalletTransferDTO
                {
                    Barcode = pallet.Barcode,
                    Description = pallet.Description,
                    Height = pallet.Height,
                    Weight = pallet.Weight,
                    Location = pallet.Location,
                    Destination = candidateSpot.Location,
                });
            }

            _db.SaveChanges();

            return dtoList;
        }

        public string FindPalletSpotLocation(int palletId)
        {
            var pallet = _db.Pallets.SingleOrDefault(p => p.Id == palletId);

            if (pallet == null)
                return "Ingen plats hittades";

            var spots = _db.PalletSpots
                .Include(s => s.CurrentPallet)
                .Where(s => s.Category == pallet.Category && s.Height > pallet.Height)
                .ToList();

            string loc = CheckSpotValidity(spots, palletId);
            if (loc != "No spot found")
            {
                return loc;
            }
                

            spots = _db.PalletSpots
                .Include(s => s.CurrentPallet)
                .Where(s => s.Height >= pallet.Height)
                .ToList();

            loc = CheckSpotValidity(spots, palletId);
            if (loc != "No spot found")
                return loc;

            return "Ingen plats hittades";
        }

        private string CheckSpotValidity(List<PalletSpot> spots, int palletId)
        {
            foreach (var spot in spots)
            {
                if (spot.CurrentPallet != null)
                    continue;

                if (spot.Location[^1] == '1')
                    continue;

                if (_db.Pallets.Any(p => p.Location == spot.Location && p.Id != palletId))
                    continue;

                return spot.Location;
            }

            return "No spot found";
        }
    }
}
