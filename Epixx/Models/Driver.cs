namespace Epixx.Models
{
    public class Driver
    {
        public string Name { get; set; } = "Steven";
        public List<Pallet> pallets { get; set; } = new List<Pallet>();
    }
}
