using Epixx.Models;
using Epixx.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epixx.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly InboundShipmentService _shipmentservice;
        private readonly PalletService _palletservice;
        private readonly StoreService _storeservice;
        private readonly DriverService _driverservice;
        public WarehouseController(DriverService driverservice,InboundShipmentService shipmentservice, PalletService palletservice, StoreService storeservice)
        {
            _storeservice = storeservice;
            _driverservice = driverservice;
            _palletservice = palletservice;
            _shipmentservice = shipmentservice;
        }
        public void GenerateWarehouseDummyData()
        {
            _palletservice.PlaceDummyPalletsInWarehouse();
            _storeservice.CreateDummyStores();
        }
        [HttpPost]
        public IActionResult StoreInboundShipment([FromBody] string storedpalletbarcode)
        {
            var pallet = _driverservice.FetchSpecificPalletByBarcode(storedpalletbarcode);
            if(pallet == null)
                return Json(null);
            _palletservice.PlacePalletInWarehouseAsync(pallet.Id, pallet.Location);
            _driverservice.RemovePalletFromDriver(pallet);

            var result = _driverservice.FetchAllPalletsFromDriverAwaitingStorage();
            List<PalletDTO> palletDTOs = new List<PalletDTO>();
            foreach (var p in result)
            {
                palletDTOs.Add(new PalletDTO
                {
                    Barcode = p.Barcode,
                    Description = p.Description,
                    Location = p.Location 
                });
            }
            return Json(palletDTOs);
        }
        [HttpGet]
        public IActionResult StoreInboundShipment()
        {
            List<PalletDTO> palletDTOs = new List<PalletDTO>();
            var pallets = _driverservice.FetchAllPalletsFromDriverAwaitingStorage();
            foreach(var pallet in pallets)
            {
                palletDTOs.Add(new PalletDTO
                {
                    Barcode = pallet.Barcode,
                    Description = pallet.Description,
                    Location = pallet.Location
                });
            }
            return View(palletDTOs);
        }
        [HttpGet]
        public void CancelMission()
        {
            _driverservice.RemoveAllPalletsFromDriver();
        } 
        [HttpPost]
        public void RemovePalletFromQueueByBarcode([FromBody] string barcode)
        {
           _driverservice.RemovePalletFromDriverByBarcode(barcode);
        }
        [HttpPost]
        public IActionResult GetPalletsByBarcodes([FromBody] string barcode)
        {
            int number;
            if (!int.TryParse(barcode, out number))
                return Json(null);
            if (barcode == null)
                return Json(null);
            var pallet = _shipmentservice.GetPallets().FirstOrDefault(p => p.Barcode == long.Parse(barcode));
            if (pallet == null)
                return Json(null);
            string location = _palletservice.FindPalletSpotLocation(pallet);
            pallet.Location = location;
            PalletDTO palletDTO = new PalletDTO();
            palletDTO.Barcode = pallet.Barcode;
            palletDTO.Description = pallet.Description;
            palletDTO.Location = location;
            palletDTO.Height = pallet.Height;
            palletDTO.Weight = pallet.Weight;
            _driverservice.AddPalletToDriver(pallet);


            return Json(palletDTO);
        }
        public IActionResult InboundLogistics()
        {

            DoubleDTO doubleDTO = new();
            List<PalletDTO> palletDTOs = new();
            List<DriverPalletDTO> driverpalletDTOs = new();
            _driverservice.SetDriverTask(DriverTask.InboundLogistics);
            var pallets = _shipmentservice.GetPallets();
            var driverPallets = _driverservice.FetchAllPalletsFromDriverAwaitingStorage();
            if (driverPallets.Any())
            {
                foreach(var dp in driverPallets)
                {
                    driverpalletDTOs.Add(new DriverPalletDTO
                    {
                        Barcode = dp.Barcode,
                        Location = dp.Location,
                        Height = dp.Height,
                        Weight = dp.Weight
                    });
                }
            }
            foreach(var p in pallets)
            {
                if(driverPallets.Any(dp => dp.Barcode == p.Barcode))
                    continue;
                palletDTOs.Add(new PalletDTO
                {
                    Barcode = p.Barcode,
                    Description = p.Description,
                    Location = p.Location,
                    Height = p.Height,
                    Weight = p.Weight
                });
            }
            doubleDTO.DriverPallet = driverpalletDTOs;
            doubleDTO.Pallet = palletDTOs;


            return View(doubleDTO);
        }
    }
}
