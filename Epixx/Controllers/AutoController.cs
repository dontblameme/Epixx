using Epixx.Models;
using Epixx.Services;
using Microsoft.AspNetCore.Mvc;

namespace Epixx.Controllers
{
    public class AutoController : Controller
    {
        private readonly StoreService _storeservice;
        private readonly DriverService _driverservice;
        private readonly PalletService _palletservice;
        public AutoController(StoreService storeservice, DriverService driverservice, PalletService palletstorageservice) 
        { 
            _palletservice = palletstorageservice;
            _storeservice = storeservice;
            _driverservice = driverservice;
        }
        public int GetPalletsCountOnDriver()
        {
            return _driverservice.FetchAllPalletsFromDriverPackingAreaConfirmation().Count;
        }
        public void CancelMission()
        {
            _driverservice.RemovePalletsFromDriverByStatus("PackingAreaTransfer");
        }
        public void CheckOffPalletsFromDriver()
        {
            var pallets = _driverservice.FetchAllPalletsFromDriverPackingAreaConfirmation();
            foreach(var pallet in pallets)
            {
                _palletservice.RemovePallet(pallet);
            }
        }
        [HttpGet]
        public IActionResult PackingAreaConfirmation()
        {
            List<PalletAndStoreDTO> palletDTOs = new List<PalletAndStoreDTO>();

            List<Pallet>? pallets = _driverservice.FetchAllPalletsFromDriverPackingAreaConfirmation();
            if (pallets == null || pallets.Count == 0)
            {
                return RedirectToAction("PackingAreaTransfer", "Auto");
            }
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
        public void AssignPalletToQueue([FromBody] string spot)
        {
            var pallet = _palletservice.FetchPalletByLocation(spot);
            _palletservice.UpdatePalletStatus("PackingAreaConfirmation", pallet.Id);

        }
        [HttpGet]
        public IActionResult PackingAreaTransfer()       
        {
            _driverservice.SetDriverTask(DriverTask.Auto);
            var pallets = new List<Pallet>();
            int id = 0;
            List<PalletAndStoreDTO> palletDTOs = new List<PalletAndStoreDTO>();
            if (_driverservice.FetchAllPalletsFromDriverPackingAreaConfirmation().Count != 0)
            {
               return RedirectToAction("PackingAreaConfirmation", "Auto");
            }
            else if(_driverservice.FetchAllPalletsFromDriverPackingAreaTransfer().Count != 0)
            {
                pallets = _driverservice.FetchAllPalletsFromDriverPackingAreaTransfer();
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
            if (store == null || pallets == null)
            {
                return RedirectToAction("Index", "Home");
            }
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
