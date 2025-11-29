using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epixx.Models
{
    public class Store
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "Falköping"; //Store name
        [Required]
        public int Code { get; set; } //koden föraren måste ange för att placera pallen
        public int? LoadingDockId { get; set; }
        [ForeignKey("LoadingDockId")]
        public List<Pallet>? Pallets { get; set; }
    }
}
