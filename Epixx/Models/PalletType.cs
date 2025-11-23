using System.ComponentModel.DataAnnotations;

namespace Epixx.Models
{
    public class PalletType
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; } // descriptive name
        public int Width { get; set; } = 80; //width of the pallet in cm (important when choosing a place to place them.
        public int Height { get; set; } //height of the pallet in cm
        [Required]
        public int Amount { get; set; }
    }
}
