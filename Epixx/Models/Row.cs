namespace Epixx.Models
{
    public class Row
    {
        public string Name { get; set; }
        public List<PalletSpot> PalletSpots { get; set; } = new List<PalletSpot>();
    }
}
