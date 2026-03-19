using Epixx.Models.Entities;
using System.ComponentModel.DataAnnotations;

public class PalletType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vad pallen innehåller är obligatoriskt")]
    public string Description { get; set; }

    public int Width { get; set; } = 80;

    [Required(ErrorMessage = "Höjd är obligatoriskt")]
    public int? Height { get; set; }

    [Required(ErrorMessage = "Antal är obligatoriskt")]
    public int? Amount { get; set; }

    public Category Category { get; set; } = Category.Miscellaneous;

    [Required(ErrorMessage = "Vikt är obligatoriskt")]
    public double? Weight { get; set; }
}