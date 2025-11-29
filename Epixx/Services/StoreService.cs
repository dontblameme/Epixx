using Epixx.Data;
using Epixx.Models;
using Microsoft.EntityFrameworkCore;

namespace Epixx.Services
{
    public class StoreService
    {
        public readonly IServiceScopeFactory _scopeFactory;
        private readonly Random _rnd = new Random();
        public StoreService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public int GetStoreIdByPalletId(int palletId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var storeid = db.Pallets
                    .Where(p => p.Id == palletId)
                    .Select(p => p.StoreId)
                    .FirstOrDefault();
                if(storeid != null)
                    return storeid.Value;
                return -1;
            }
        }
        public string GetPackingAreaNameByStoreId(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var store = GetStoreByStoreId(id);
                var loadingdockname = db.LoadingDocks
                    .Where(ld => ld.Id == store.LoadingDockId)
                    .Select(ld => ld.Name)
                    .FirstOrDefault();
                return loadingdockname;
            }
        }
        public int GetStoreId()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var mostCommonStoreId = db.Pallets
                    .Where(p => p.StoreId != null)          
                    .GroupBy(p => p.StoreId)                
                    .OrderByDescending(g => g.Count())      
                    .Select(g => g.Key)                     
                    .FirstOrDefault();
                if(mostCommonStoreId != null)
                    return mostCommonStoreId.Value;
                return -1;
            }
        }
        public List<Pallet> GetPalletsByStoreId(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var pallets = db.Pallets
                    .Where(p => p.StoreId == id && p.Status == "PackingAreaTransfer")
                    .Take(5)
                    .ToList();
                return pallets;
            }
        }
        public Store GetStoreByStoreId(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var store = db.Stores
                    .Where(s => s.Id == id)
                    .FirstOrDefault();
                return store;
            }
        }
        public List<string> GetAllDummyStores()
        {
            List<string> storenames = new List<string>
            {
                "Stockholm",
                "Göteborg",
                "Malmö",
                "Uppsala",
                "Västerås",
                "Örebro",
                "Linköping",
                "Helsingborg",
                "Jönköping",
                "Norrköping",
                "Lund",
                "Umeå",
                "Gävle",
                "Borås",
                "Eskilstuna",
                "Södertälje",
                "Karlstad",
                "Täby",
                "Växjö",
                "Halmstad",
                "Sundsvall",
                "Luleå",
                "Trollhättan",
                "Östersund",
                "Borlänge",
                "Falun",
                "Kristianstad",
                "Kalmar",
                "Skövde",
                "Karlskrona",
                "Uddevalla",
                "Ystad",
                "Visby",
                "Piteå",
                "Kiruna",
                "Varberg",
                "Motala",
                "Nyköping",
                "Kungsbacka",
                "Alingsås",
                "Hudiksvall",
                "Sandviken",
                "Mora",
                "Arvika",
                "Hässleholm",
                "Åre",
                "Ängelholm",
                "Oskarshamn",
                "Enköping"
            };
            return storenames;

        }
        public void CreateDummyStores()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                int rand = _rnd.Next(1, 6);
                var storenames = GetAllDummyStores();
                List<LoadingDock> docks = new List<LoadingDock>();
                for (int j = 0; j < 3; j++)
                {
                    char name = (char)('A' + j -1);
                    var dock = new LoadingDock
                    {
                        Name = name.ToString(),

                    };
                    docks.Add(dock);
                    db.LoadingDocks.Add(dock);
                    db.SaveChanges();
                    int dockid = db.LoadingDocks.Where(d => d.Name == dock.Name).Select(d => d.Id).FirstOrDefault();
                    if ((dockid == null))
                    {
                        return;
                    }
                    for (int i = 1; i <= rand; i++)
                    {
                        string storename = storenames[_rnd.Next(storenames.Count)];
                        var store = new Store
                        {
                            Name = storename,
                            Code = _rnd.Next(100, 999),
                            LoadingDockId = dockid
                        };
                        storenames.Remove(storename);
                        db.Stores.Add(store);
                    }
                }
               
                db.SaveChanges();
            }
        }
    }
}
