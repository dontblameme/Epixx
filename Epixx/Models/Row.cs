using System.ComponentModel.DataAnnotations;

namespace Epixx.Models
{
    public enum Category
    {
        Clothing, Electronics, Furniture, Food, Miscellaneous
    }
    public class Row
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Category Type { get; set; }
        public List<PalletSpot> PalletSpots { get; set; } = new List<PalletSpot>();
    }
}
