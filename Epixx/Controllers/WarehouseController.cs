using Epixx.Models;
using Epixx.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Epixx.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly InboundShipmentService _shipmentservice;
        private readonly PalletStorageService _palletstorageservice;
        private readonly WarehouseService _warehouseservice;
        public WarehouseController(InboundShipmentService shipmentservice, PalletStorageService palletservice, WarehouseService warehouseservice)
        {
            _warehouseservice = warehouseservice;
            _palletstorageservice = palletservice;
            _shipmentservice = shipmentservice;
        }
        [HttpPost]
        public IActionResult StoreInboundShipment([FromBody] string storedpalletbarcode)
        {
            var pallet = _palletstorageservice._driver.pallets.FirstOrDefault(p => p.Barcode == long.Parse(storedpalletbarcode));
            _palletstorageservice.PlacePalletInWarehouse(pallet);
            _shipmentservice.RemovePallet(pallet);
            _palletstorageservice._driver.pallets.Remove(pallet);
            return Json(_palletstorageservice._driver.pallets);
        }
        [HttpGet]
        public IActionResult StoreInboundShipment()
        {
            var pallets = _palletstorageservice._driver.pallets;
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
            _palletstorageservice._driver.pallets.Clear();
        } 
        [HttpPost]
        public void RemovePalletFromQueueByBarcode([FromBody] string barcode)
        {
            var pallet = _palletstorageservice._driver.pallets.FirstOrDefault(p => p.Barcode == long.Parse(barcode));
            if (pallet == null) return;
            _palletstorageservice.RemovePalletSpotFromLocation(pallet);
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

            return Json(palletwithlocation);
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
