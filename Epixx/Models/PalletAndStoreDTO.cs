namespace Epixx.Models
{
    public class PalletAndStoreDTO
    {
        public long Barcode { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Height { get; set; }
        public double Weight { get; set; }
        public string StoreName { get; set; }
        public int? Code { get; set; }
        public string PackingAreaName { get; set; }
    }
}
