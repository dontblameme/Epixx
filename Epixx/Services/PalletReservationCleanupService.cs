using Epixx.Data;
using Epixx.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Epixx.Services
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(20);

        public ReservationCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var now = DateTime.UtcNow;
                var expiredSpots = await db.PalletSpots
                    .Where(s => s.ReservedUntil != null && s.ReservedUntil < now)
                    .ToListAsync<PalletSpot>(stoppingToken);

                foreach (var spot in expiredSpots)
                {
                    var expiredPallet = await db.Pallets.Where(p => p.DriverId == spot.ReservedByDriverId).FirstOrDefaultAsync(stoppingToken);
                    if(expiredPallet != null)
                        expiredPallet.DriverId = null;
                    spot.ReservedByDriverId = null;
                    spot.ReservedUntil = null;
                }

                if (expiredSpots.Any())
                {
                    await db.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
