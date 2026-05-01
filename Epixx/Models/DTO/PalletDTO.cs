namespace Epixx.Models.DTO
{
    public class PalletDTO
    {
        public long Barcode { get; set; }
        public string Description { get; set; }
        public string Destination { get; set; }
        public int Height { get; set; }
        public double Weight { get; set; }
    }
}