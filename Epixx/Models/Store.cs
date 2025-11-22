namespace Epixx.Models
{
    public class Store
    {
        public string Name { get; set; } = "Falköping"; //Store name
        public int Capacity { get; set; } = Random.Shared.Next(50, 101); //random number between 50-100
        public List<Pallet>? Pallets { get; set; }
    }
}
