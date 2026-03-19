using Epixx.Data;
using Epixx.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Epixx.Services
{
    public class StoreService
    {
        private readonly AppDbContext _db;
        private readonly Random _rnd = new Random();

        public StoreService(AppDbContext db)
        {
            _db = db;
        }

        public int GetStoreCount()
        {
            return _db.Stores.Count();
        }

        public int GetStoreIdByPalletId(int palletId)
        {
            var storeId = _db.Pallets
                .Where(p => p.Id == palletId)
                .Select(p => p.StoreId)
                .FirstOrDefault();

            return storeId ?? -1;
        }

        public string GetPackingAreaNameByStoreId(int id)
        {
            var store = GetStoreByStoreId(id);

            if (store == null) return "";

            return _db.LoadingDocks
                .Where(ld => ld.Id == store.LoadingDockId)
                .Select(ld => ld.Name)
                .FirstOrDefault() ?? "";
        }

        public int GetStoreId()
        {
            var mostCommonStoreId = _db.Pallets
                .Where(p => p.StoreId != null)
                .GroupBy(p => p.StoreId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return mostCommonStoreId ?? -1;
        }

        public List<Pallet> GetPalletsByStoreId(int id)
        {
            return _db.Pallets
                .Where(p => p.StoreId == id && p.Status == "PackingAreaTransfer")
                .Take(5)
                .ToList();
        }

        public Store? GetStoreByStoreId(int id)
        {
            return _db.Stores
                .Where(s => s.Id == id)
                .FirstOrDefault();
        }

        public List<string> GetAllDummyStores()
        {
            return new List<string>
            {
                "Stockholm","Göteborg","Malmö","Uppsala","Västerås","Örebro","Linköping",
                "Helsingborg","Jönköping","Norrköping","Lund","Umeå","Gävle","Borås","Eskilstuna",
                "Södertälje","Karlstad","Täby","Växjö","Halmstad","Sundsvall","Luleå","Trollhättan",
                "Östersund","Borlänge","Falun","Kristianstad","Kalmar","Skövde","Karlskrona",
                "Uddevalla","Ystad","Visby","Piteå","Kiruna","Varberg","Motala","Nyköping",
                "Kungsbacka","Alingsås","Hudiksvall","Sandviken","Mora","Arvika","Hässleholm",
                "Åre","Ängelholm","Oskarshamn","Enköping"
            };
        }

        public void CreateDummyStores()
        {
            int rand = _rnd.Next(1, 6);
            var storenames = GetAllDummyStores();

            var createdDocks = new List<LoadingDock>();

            for (int j = 0; j < 3; j++)
            {
                char name = (char)('A' + j);

                var dock = new LoadingDock { Name = name.ToString() };
                _db.LoadingDocks.Add(dock);
                _db.SaveChanges();

                for (int i = 1; i <= rand; i++)
                {
                    string storename = storenames[_rnd.Next(storenames.Count)];
                    storenames.Remove(storename);

                    var store = new Store
                    {
                        Name = storename,
                        Code = _rnd.Next(100, 999),
                        LoadingDockId = dock.Id
                    };

                    _db.Stores.Add(store);
                }
            }

            _db.SaveChanges();
        }
    }
}
