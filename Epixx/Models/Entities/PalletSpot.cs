using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epixx.Models.Entities
{
    public class PalletSpot
    {
        public int Id { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public string Location { get; set; }
        public int? CurrentPalletId { get; set; }
        [ForeignKey("CurrentPalletId")]
        public Pallet? CurrentPallet { get; set; }
        [Required]
        public Category Category { get; set; }
        public int? ReservedByDriverId { get; set; }
        public DateTime? ReservedUntil { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
