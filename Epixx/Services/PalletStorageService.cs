using Epixx.Models;

namespace Epixx.Services
{
    public class PalletStorageService
    {
        private readonly WarehouseService _warehouseservice;
        private readonly InboundShipmentService _inboundShipmentService;
        public Driver _driver {  get; set; }
        public PalletStorageService(InboundShipmentService inboundShipmentService ,WarehouseService warehouse, Driver driver) 
        {
            _inboundShipmentService = inboundShipmentService;
            _warehouseservice = warehouse;
            _driver = driver;
        }
        public void RemovePalletSpotFromLocation(PalletWithFullLocationVM pallet)
        {            
            _driver.pallets.Remove(pallet);            
        }
        public void PlacePalletInWarehouse(PalletWithFullLocationVM pallet)
        {
            var row = pallet.FullLocation.Remove(2);
            var location = pallet.FullLocation.Remove(0, 2);
            foreach (Row r in _warehouseservice.Warehouse)
            {
                if (r.Name == row)
                {
                    foreach (var palletspot in r.PalletSpots)
                    {
                        palletspot.CurrentPallet = pallet.Pallet;
                        palletspot.CurrentPallet.Location = "Stored";
                    }
                }
            }
        }
        public string FindPalletSpotLocation(Pallet pallet)
        {
            foreach (var row in _warehouseservice.Warehouse)
            {
                foreach (var palletspot in row.PalletSpots)
                {
                    string name = row.Name.Remove(2, 1);
                    string location = palletspot.Location;
                    string fulllocation = name + location;
                    if (palletspot.CurrentPallet == null && palletspot.Height > pallet.Height && location[location.Length - 1] != '1' && !_driver.pallets.Any(p => p.FullLocation == fulllocation))
                    {                       
                        PalletWithFullLocationVM palletWithFullName = new PalletWithFullLocationVM();
                        palletWithFullName.Pallet = pallet;
                        palletWithFullName.FullLocation = name + palletspot.Location;
                        _driver.pallets.Add(palletWithFullName);
                        return palletWithFullName.FullLocation;
                         
                    }
                }
            }
            
            return "Ingen plats hittades";
        }
    }
}
