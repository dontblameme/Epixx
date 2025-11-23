namespace Epixx.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Steven";
        public List<Pallet> pallets { get; set; } = new List<Pallet>();
    }
}
