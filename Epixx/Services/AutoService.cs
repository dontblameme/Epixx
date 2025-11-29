using Epixx.Data;
using Epixx.Models;

namespace Epixx.Services
{
    public class AutoService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Random _rnd = new Random();

        public AutoService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
       
        public Pallet GetRandomPallet()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var storedpallets = db.Pallets.Where(p => p.Status == "Stored").ToList();
                if (storedpallets.Count == 0)
                    return null;
                var index = _rnd.Next(storedpallets.Count);
                var pallet = storedpallets[index];
                return pallet;
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var pallet = GetRandomPallet();
                var store = db.Stores
                    .OrderBy(s => Guid.NewGuid())
                    .FirstOrDefault();
                if (store == null || pallet == null)
                    continue;
                pallet.Status = "PackingAreaTransfer";
                pallet.StoreId = store.Id;
                db.Pallets.Update(pallet);
                await db.SaveChangesAsync();
                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
