using Epixx.Data;
using Epixx.Models;
using Microsoft.EntityFrameworkCore;

public class InboundShipmentService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Random _rnd = new Random();

    public InboundShipmentService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public List<Pallet> GetPallets()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var pallets = db.Pallets.Where(p => p.Status == "Awaiting Storage").ToList();
            return pallets;
        }
    }
    public void RemovePallet(Pallet pallet)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Pallets.Remove(pallet);
            db.SaveChanges();
        }
    }
    private List<PalletType> createDummyPalletTypes()
    {
        List<PalletType> palletTypes = new();
        palletTypes.Add(new PalletType
        {
            Description = "Vinterkängor 42",
            Height = 130,
            Amount = 55,
          
        });
        palletTypes.Add(new PalletType
        {
            Description = "Naturtrogen julgran 210 cm",
            Height = 150,
            Amount = 21,

        });
        palletTypes.Add(new PalletType
        {
            Description = "Julgranskrage Ø50/70 cm",
            Height = 130,
            Amount = 40,

        });
        palletTypes.Add(new PalletType
        {
            Description = "Julgranskrage Ø50/70 cm",
            Height = 130,
            Amount = 40,

        });
        palletTypes.Add(new PalletType
        {
            Description = "Julgransfot för konstgran",
            Height = 150,
            Amount = 20,

        });
        palletTypes.Add(new PalletType
        {
            Description = "Pilejacka herr",
            Height = 150,
            Amount = 200
        });
        palletTypes.Add(new PalletType
        {

            Description = "MIG-svets 100 A",
            Height = 210,
            Amount = 2
        });
        return palletTypes;
    }
    private List<Pallet> CreateNewPallets(PalletType pallettype)
    {
        List<Pallet> pallets = new();
        int count = _rnd.Next(2, 15);
        for(int i = 0; i < count; i++)
        {
            Pallet pallet = new Pallet();
            pallet.Barcode = _rnd.Next(10000000, 999999999);
            pallet.Status = "Awaiting Storage";
            pallet.Description = pallettype.Description;
            pallet.Amount = pallettype.Amount;
            pallet.Height = pallettype.Height;
            pallets.Add(pallet);
           

        }
        return pallets;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // One-time DB access before the loop
        using (var scope = _scopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!db.PalletTypes.Any())
            {
                // populate initial dummy data
                db.AddRange(createDummyPalletTypes());
                await db.SaveChangesAsync(stoppingToken);
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            int countTypes = await db.PalletTypes.CountAsync();
            int index = _rnd.Next(countTypes);
            int count = await db.Pallets.CountAsync();
            //Get a random pallet type from db
            var randompalletType = await db.PalletTypes.Skip(index).FirstOrDefaultAsync(); 
            if(count < 50)
            {
                var newpallets = CreateNewPallets(randompalletType);
                db.Pallets.AddRange(newpallets);
                await db.SaveChangesAsync(stoppingToken);
            }
            // periodic work
            await Task.Delay(30000, stoppingToken);
        }

    }
}
