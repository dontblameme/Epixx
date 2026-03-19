using Epixx.Data;
using Epixx.Migrations;
using Epixx.Models.DTO;
using Epixx.Models.Entities;
using Epixx.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Epixx.Controllers
{
    [Authorize]

    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PalletService _palletservice;
        private readonly AppDbContext _db;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, PalletService palletService, AppDbContext context)
        {
            _palletservice = palletService;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = context;
        }
        [HttpPost]
        public IActionResult PalletsAwaitingStorage(InboundPalletDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return RedirectToAction("Index");
        }
        public IActionResult PalletsAwaitingStorage()
        {
            ViewBag.PalletTypes = _palletservice.GetAllPalletTypes();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PalletTypeCreation(PalletType model)
        {
            if(_db.PalletTypes.Any(p => p.Description == model.Description))
            {
                ModelState.AddModelError("Description", "Denna palltyp finns redan");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _palletservice.CreateNewPalletType(model);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> PalletTypeCreation()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // returns List<string>
                userViewModels.Add(new UserDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "Ingen roll"
                });
            }

            return View(userViewModels);
        }
        // --- Create new user (GET) ---
        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _roleManager.Roles.ToList();
            return View();
        }

        // --- Create new user (POST) ---
        [HttpPost]
        public async Task<IActionResult> CreateUser(string username, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "Alla fält krävs");
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View();
            }
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Användaren finns redan");
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View();
            }
            var user = new IdentityUser { UserName = username, Email = username, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", string.Join(", ", result.Errors.Select(e => e.Description)));
                ViewBag.Roles = _roleManager.Roles.ToList();
                return View();
            }

            // Assign role
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("Index");
        }
    }
}
