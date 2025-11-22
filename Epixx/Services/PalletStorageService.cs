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
        public void RemovePalletSpotFromLocation(Pallet pallet)
        {            
            _driver.pallets.Remove(pallet);            
        }
        public void PlacePalletInWarehouse(Pallet pallet)
        {
            var row = pallet.Location.Remove(2);
            foreach (Row r in _warehouseservice.Warehouse)
            {
                foreach(var palletspot in r.PalletSpots)
                {
                    if(palletspot.Location == pallet.Location)
                    {
                        palletspot.CurrentPallet = pallet;
                        Console.WriteLine("Pallet successfully added to the warehouse: " + palletspot.CurrentPallet.Location);
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
                    string location = palletspot.Location;
                    if (palletspot.CurrentPallet == null && palletspot.Height > pallet.Height && location[location.Length - 1] != '1' && !_driver.pallets.Any(p => p.Location == palletspot.Location))
                    {                                
                        _driver.pallets.Add(pallet);
                        return palletspot.Location;
                    }
                }
            }
            
            return "Ingen plats hittades";
        }
    }
}
