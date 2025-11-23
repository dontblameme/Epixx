using System.ComponentModel.DataAnnotations;

namespace Epixx.Models
{
    public class Store
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "Falköping"; //Store name
        [Required]
        public int Capacity { get; set; } //How many pallets it can store before departure
        [Required]
        public int Code { get; set; } //koden föraren måste ange för att placera pallen
        public List<Pallet>? Pallets { get; set; }
    }
}
