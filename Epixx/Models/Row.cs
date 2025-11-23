using System.ComponentModel.DataAnnotations;

namespace Epixx.Models
{
    public class Row
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<PalletSpot> PalletSpots { get; set; } = new List<PalletSpot>();
    }
}
