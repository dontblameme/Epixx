using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epixx.Models
{
    public class Pallet
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public long Barcode { get; set; }

        public int Width { get; set; } = 80;
        public int Height { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public string Status { get; set; }

        public string? Location { get; set; }

        public int? DriverId { get; set; }
        [ForeignKey("DriverId")]

        public Driver? Driver { get; set; }      // Navigation property
    }
}
