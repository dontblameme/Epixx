using Epixx.Models;
using Epixx.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Epixx.Controllers
{
    public class HomeController : Controller
    {
        private readonly DriverService _driverservice;
        public HomeController(DriverService service, WarehouseService warehouse)
        {
            _driverservice = service;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(); 
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {

            if(username == "admin" && password == "1234")
            {
                string name = _driverservice.GetOrCreateDriver("Steven");
                HttpContext.Session.SetString("User", name);

                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Home");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
