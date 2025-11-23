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
        private readonly PalletStorageService _palletstorageservice;
        private readonly WarehouseService _warehouseservice;
        private readonly DriverService _driverservice;
        public WarehouseController(DriverService driverservice,InboundShipmentService shipmentservice, PalletStorageService palletservice, WarehouseService warehouseservice)
        {
            _driverservice = driverservice;
            _warehouseservice = warehouseservice;
            _palletstorageservice = palletservice;
            _shipmentservice = shipmentservice;
        }
        public void GenerateWarehouseDummyData()
        {
            _palletstorageservice.PlaceDummyPalletsInWarehouse();
        }
        [HttpPost]
        public IActionResult StoreInboundShipment([FromBody] string storedpalletbarcode)
        {
            var pallet = _driverservice.FetchSpecificPalletByBarcode(storedpalletbarcode);
            if(pallet == null)
                return Json(null);
            _palletstorageservice.PlacePalletInWarehouseAsync(pallet.Id, pallet.Location);
            _driverservice.RemovePalletFromDriver(pallet);

            var result = _driverservice.FetchAllPalletsFromDriver();
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
            var pallets = _driverservice.FetchAllPalletsFromDriver();
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
        public IActionResult _PalletTable()
        {
            var pallets = _shipmentservice.GetPallets();
            return PartialView("_PalletTable", pallets);
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
            string location = _palletstorageservice.FindPalletSpotLocation(pallet);
            pallet.Location = location;
            PalletDTO palletDTO = new PalletDTO();
            palletDTO.Barcode = pallet.Barcode;
            palletDTO.Description = pallet.Description;
            palletDTO.Location = location;
            palletDTO.Height = pallet.Height;
            _driverservice.AddPalletToDriver(pallet);


            return Json(palletDTO);
        }
        public IActionResult AutoSkj()
        {
            return View();
        }
        public IActionResult InboundLogistics()
        {
            var pallets = _shipmentservice.GetPallets();
            return View(pallets);
        }
    }
}
