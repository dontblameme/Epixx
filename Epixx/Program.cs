using Epixx.Data;
using Epixx.Data.Seed;
using Epixx.Models;
using Epixx.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1️⃣ DbContext ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 2️⃣ Identity ---
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// --- 2️⃣a Configure Identity cookies ---
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Login";           // redirect here if not logged in
    options.AccessDeniedPath = "/Home/AccessDenied"; // optional page if role access denied
});

// --- 3️⃣ MVC, session, etc ---
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// --- 4️⃣ Your custom services ---
builder.Services.AddSingleton<WarehouseService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<PalletService>();
builder.Services.AddScoped<StoreService>();
builder.Services.AddHostedService<ReservationCleanupService>();

var app = builder.Build();

// --- 5️⃣ Run migrations & seed data ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    var warehouseService = services.GetRequiredService<WarehouseService>();

    // Apply migrations
    db.Database.Migrate();

    // Seed Identity roles and admin user
    await IdentitySeeder.Seed(services);

    // Seed warehouse rows if empty
    if (!db.Rows.Any())
    {
        db.Rows.AddRange(warehouseService.Warehouse);
        db.SaveChanges();
    }
}

// --- 6️⃣ Middleware ---
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// --- 7️⃣ Routes ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();