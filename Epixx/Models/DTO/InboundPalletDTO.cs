using System.ComponentModel.DataAnnotations;

namespace Epixx.Models.DTO
{
    public class InboundPalletDTO
    {
        [Required(ErrorMessage = "Palltyp är obligatoriskt")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Antal pallar är obligatoriskt")]
        public int? AmountOfPallets { get; set; }

    }
}
