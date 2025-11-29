namespace Epixx.Models
{
    public class RowConfig
    {
        public string Name { get; set; }
        public int Spots { get; set; }
        public Category Type { get; set; } = Category.Miscellaneous;
    }
}
