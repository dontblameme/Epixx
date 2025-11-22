namespace Epixx.Models
{
    public class Driver
    {
        public string Name { get; set; } = "Steven";
        public List<PalletWithFullLocationVM> pallets { get; set; } = new List<PalletWithFullLocationVM>();
    }
}
