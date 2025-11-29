using System.ComponentModel.DataAnnotations;

namespace Epixx.Models
{
    public class LoadingDock
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public List<Store> Stores { get; set; } 
    }
}
