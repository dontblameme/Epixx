csharp Epixx\Controllers\WarehouseController.cs
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
        [HttpPost]
        public IActionResult StoreInboundShipment([FromBody] string storedpalletbarcode)
        {
            var pallet = _driverservice.FetchSpecificPalletByBarcode(storedpalletbarcode);
            _palletstorageservice.PlacePalletInWarehouse(pallet);
            _driverservice.RemovePalletFromDriver(pallet);

            var result = _driverservice.FetchAllPalletsFromDriver();
            return new JsonResult(result)
            {
                SerializerOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                }
            };
        }
        [HttpGet]
        public IActionResult StoreInboundShipment()
        {
            var pallets = _driverservice.FetchAllPalletsFromDriver();
            return View(pallets);
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
            Pallet palletwithlocation = new Pallet();
            palletwithlocation = pallet;
            palletwithlocation.Location = location;
            _driverservice.AddPalletToDriver(palletwithlocation);

            return new JsonResult(palletwithlocation)
            {
                SerializerOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                }
            };
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