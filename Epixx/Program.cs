using Epixx.Data;
using Epixx.Models;
using Epixx.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<WarehouseService>();

builder.Services.AddSingleton<DriverService>();
builder.Services.AddSingleton<PalletService>();
builder.Services.AddSingleton<InboundShipmentService>();
builder.Services.AddSingleton<AutoService>();
builder.Services.AddSingleton<StoreService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<InboundShipmentService>());
builder.Services.AddHostedService<AutoService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var warehouseservice = scope.ServiceProvider.GetRequiredService<WarehouseService>();

    db.Database.Migrate();

    if (!db.Rows.Any())
    {
        var rows = warehouseservice.Warehouse;
        db.Rows.AddRange(rows);

        db.SaveChanges();
    }

}

app.UseSession();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
