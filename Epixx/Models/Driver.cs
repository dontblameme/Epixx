namespace Epixx.Models
{
    public enum DriverTask
    {
        Auto,
        InboundLogistics
    }
    public class Driver
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Steven";
        public DriverTask? CurrentTask { get; set; } 

        public List<Pallet> pallets { get; set; } = new List<Pallet>();
    }
}
