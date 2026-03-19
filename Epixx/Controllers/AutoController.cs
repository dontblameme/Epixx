using Epixx.Data;
using Epixx.Models.DTO;
using Epixx.Models.Entities;
using Epixx.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epixx.Controllers
{
    [Authorize]
    public class AutoController : Controller
    {
        private readonly StoreService _storeservice;
        private readonly DriverService _driverservice;
        private readonly PalletService _palletservice;
        private readonly Random _rnd = new Random();
        private readonly AppDbContext _db;
        public AutoController(StoreService storeservice, DriverService driverservice, PalletService palletstorageservice, AppDbContext context) 
        { 
            _db = context;
            _palletservice = palletstorageservice;
            _storeservice = storeservice;
            _driverservice = driverservice;
        }
        public int GetPalletsCountOnDriver()
        {
            return _driverservice.FetchAllPalletsFromDriverByStatus("PackingAreaConfirmation").Count();
        }
        public void CancelMission()
        {
            _driverservice.UnassignAllPalletsFromDriver();
        }
        [HttpPost]
        public void CheckOffPalletsFromDriver([FromBody] string status)
        {
            var pallets = _driverservice.FetchAllPalletsFromDriverByStatus(status);
            foreach(var pallet in pallets)
            {
                _palletservice.RemovePallet(pallet);
            }
        }
        [HttpGet]
        public IActionResult PalletTransferConfirmation()
        {
            var pallets = _driverservice.FetchAllPalletsFromDriverByStatus("ConfirmTransfer");
            var palletDTOs = pallets.Select(p => new PalletTransferDTO
            {
                Barcode = p.Barcode,
                Description = p.Description,
                Location = p.Location,
                Height = p.Height,
                Weight = p.Weight,
                Destination = p.Destination
            }).ToList();
            return View(palletDTOs);
        }
        [HttpGet]
        public IActionResult PackingAreaConfirmation()
        {
            List<PalletAndStoreDTO> palletDTOs = new List<PalletAndStoreDTO>();

            List<Pallet>? pallets = _driverservice.FetchAllPalletsFromDriverByStatus("PackingAreaConfirmation");
            
            int storeid = _storeservice.GetStoreIdByPalletId(pallets[0].Id);
            var store = _storeservice.GetStoreByStoreId(storeid);
            var packingareaname = _storeservice.GetPackingAreaNameByStoreId(storeid);

            foreach (var pallet in pallets)
            {
                palletDTOs.Add(new PalletAndStoreDTO
                {
                    Barcode = pallet.Barcode,
                    Description = pallet.Description,
                    Location = pallet.Location,
                    Height = pallet.Height,
                    Weight = pallet.Weight,
                    StoreName = store.Name,
                    Code = store.Code,
                    PackingAreaName = packingareaname


                });
            }
            return View(palletDTOs);
        }
        [HttpPost]
        public async Task<IActionResult> AssignPalletToQueue([FromBody] SpotDTO dto)
        {
            var pallet = _palletservice.FetchPalletByLocation(dto.Spot);

            if (pallet == null)
                return BadRequest("Pallet not found.");

            _palletservice.UpdatePalletStatus("PackingAreaConfirmation", pallet.Id);

            return Ok();
        }
        [HttpPost]
        public string ChangePalletStatusToConfirmTransfer([FromBody] string locationOrBarcode)
        {
            Pallet? pallet = _palletservice.FetchPalletByLocation(locationOrBarcode)
                 ?? _palletservice.FetchPalletByBarcode(locationOrBarcode);

            if (pallet != null)
            {
                _palletservice.UpdatePalletStatus("ConfirmTransfer", pallet.Id);
                return pallet.Barcode.ToString() == locationOrBarcode ? "Barcode" : "Location";
            }
            return string.Empty;
        }
        [HttpPost]
        public IActionResult ChangePalletLocation([FromBody] string barcode)
        {
            var pallet = _palletservice.FetchPalletByBarcode(barcode);

            if (pallet == null)
                return BadRequest("Pallet not found.");
            _palletservice.ChangePalletLocationToDestination(pallet);
            _driverservice.RemovePalletFromDriver(pallet);
   
            return Ok();
        }
        [HttpGet]
        public IActionResult ConfirmTransfer()
        {
            var pallets = _driverservice.FetchAllPalletsFromDriverByStatus("ConfirmTransfer");
            return Ok(pallets.Any());
        }
        [HttpGet]
        public IActionResult CheckIfNoPalletsAreScanned()
        {
            var anypallets =_driverservice.FetchAllPalletsFromDriverByStatus("ConfirmTransfer").Any();
            return Ok(anypallets);
        }
        [HttpGet]
        public IActionResult TwoTenCheck(int incomingpalletheight, string status)
        {
            var pallets = _driverservice.FetchAllPalletsFromDriverByStatus(status);
            if(pallets.Count() == 0 && incomingpalletheight == 210)
                return Ok(false);
            if(!pallets.Any(p => p.Height == 210) && incomingpalletheight == 210)
                return Ok(true);
            return Ok(false);
        }
        [HttpGet]
        public IActionResult PalletTransfer()
        {
            var pallets = _driverservice.FetchAllPalletsFromDriver();
            if (!pallets.Any())
                pallets = _palletservice.GetPalletsForTransfer();

            var palletDTOs = new List<PalletTransferDTO>();
            palletDTOs = _palletservice.AssignDestinationsToPallets(pallets);

            return View(palletDTOs);
        }
        [HttpGet]
        public IActionResult FindAutoMission()
        {
            // Random choice between 0 or 1
            bool choosePalletTransfer = _rnd.Next(2) == 0;
            var driverPalletS = _driverservice.GetAllPalletsFromDriver();
            List<string> ?palletStatus = _db.Pallets.Where(p => p.DriverId == _driverservice.GetDriverId()).Select(p => p.Status).ToList();
            if (driverPalletS.Count > 0)
            {
                if (palletStatus.Contains("PackingAreaConfirmation"))
                {
                    return RedirectToAction("PackingAreaConfirmation", "Auto");
                }
                if (palletStatus.Contains("ConfirmTransfer"))
                {
                    return RedirectToAction("PalletTransferConfirmation", "Auto");
                }
                if (palletStatus.Contains("PackingAreaTransfer"))
                {
                    return RedirectToAction("PackingAreaTransfer", "Auto");
                }
                if(palletStatus.Contains("PalletTransfer"))
                {
                    return RedirectToAction("PalletTransfer", "Auto");
                }
                
            }
            else
            {
                int id = _storeservice.GetStoreId();
                var packingAreaTransferPalletCount = _storeservice.GetPalletsByStoreId(id).Count;
                if(_palletservice.GetPalletTransferCount() > 0 && packingAreaTransferPalletCount > 0)
                {
                    if (choosePalletTransfer)
                        return RedirectToAction("PalletTransfer", "Auto");
                    else
                        return RedirectToAction("PackingAreaTransfer", "Auto");
                }
                else if(_palletservice.GetPalletTransferCount() > 0)
                {
                    return RedirectToAction("PalletTransfer", "Auto");
                }
                else
                {
                    return RedirectToAction("PackingAreaTransfer", "Auto");

                }
            }
            return RedirectToAction("Index", "Home");
        
        }

        [HttpGet]
        public IActionResult PackingAreaTransfer()       
        {
            _driverservice.SetDriverTask(DriverTask.Auto);
            var pallets = new List<Pallet>();
            int id = 0;
            List<PalletAndStoreDTO> palletDTOs = new List<PalletAndStoreDTO>();
            if(_driverservice.FetchAllPalletsFromDriverByStatus("PackingAreaTransfer").Count != 0)
            {
                pallets = _driverservice.FetchAllPalletsFromDriverByStatus("PackingAreaTransfer");
                id = _storeservice.GetStoreIdByPalletId(pallets[0].Id);
            }
            else
            {
                id = _storeservice.GetStoreId();
                pallets = _storeservice.GetPalletsByStoreId(id);
                foreach (var pallet in pallets)
                {
                    _driverservice.AddPalletToDriver(pallet);
                }
            }           
            var store = _storeservice.GetStoreByStoreId(id);
            var packingareaname = _storeservice.GetPackingAreaNameByStoreId(id);
            foreach (var pallet in pallets)
            {
                palletDTOs.Add(new PalletAndStoreDTO
                {
                    Barcode = pallet.Barcode,
                    Description = pallet.Description,
                    Location = pallet.Location,
                    Height = pallet.Height,
                    Weight = pallet.Weight,
                    StoreName = store.Name,
                    PackingAreaName = packingareaname
                });
            }
            return View(palletDTOs);
        }
    }
}
