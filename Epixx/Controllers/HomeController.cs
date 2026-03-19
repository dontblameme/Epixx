using Epixx.Models;
using Epixx.Models.DTO;
using Epixx.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Epixx.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly PalletService _palletservice;
        private readonly DriverService _driverservice;
        private readonly StoreService _storeservice;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(DriverService service, WarehouseService warehouse, PalletService palletservice, StoreService storeservice, SignInManager<IdentityUser> signInManager)
        {
            _storeservice = storeservice;
            _palletservice = palletservice;
            _driverservice = service;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View(); 
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Användarnamn och lösenord krävs");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                var driverName = _driverservice.GetOrCreateDriver(username);
                HttpContext.Session.SetString("User", driverName);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove("User");
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Index()
        {
            var indexDTO = new IndexDTO();
            int count = _palletservice.GetPalletsByStatus("PackingAreaTransfer").Count;
            count += _palletservice.GetPalletsByStatus("PalletTransfer").Count;
            indexDTO.Count = count;
            indexDTO.CountTwo = _storeservice.GetStoreCount();
            return View(indexDTO);
        }
    }
}
