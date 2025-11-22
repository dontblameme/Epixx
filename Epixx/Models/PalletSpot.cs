namespace Epixx.Models
{
    public class PalletSpot
    {
        public int Height { get; set; }
        public string Location { get; set; }
        public Pallet? CurrentPallet { get; set; }
    }
}
