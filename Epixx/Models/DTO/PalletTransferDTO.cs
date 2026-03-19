namespace Epixx.Models.DTO
{
    public class PalletTransferDTO
    {
        public long Barcode { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Height { get; set; }
        public double Weight { get; set; }
        public string Destination { get; set; }
    }
}
