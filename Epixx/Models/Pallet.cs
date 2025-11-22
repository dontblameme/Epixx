namespace Epixx.Models
{
    public class Pallet
    {
        public long PalletID { get; set; }
        public string Name { get; set; } //Pallet loong name less readable
        public string Description { get; set; } // descriptive name
        public long Barcode { get; set; } //streckkod
        public int Width { get; set; } = 80; //width of the pallet in cm (important when choosing a place to place them.
        public int Height { get; set; } = 130; //height of the pallet in cm
        public int Amount {  get; set; }
        public string? Status { get; set; } // current status of the pallet, either in storage, awaiting storage or needing to be placed at the loading dock.
        public string Location { get; set; } //RA1185 6 for example
    }
}
