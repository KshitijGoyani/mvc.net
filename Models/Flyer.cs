using System.ComponentModel.DataAnnotations;

namespace Lab5.Models
{
    public class Flyer
    {
        [Key]
        public int FlyerId { get; set; }

        [Required]
        [StringLength(255)] // To limit file name length
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(2048)] // To limit URL length
        public string Url { get; set; } = string.Empty;

        // Foreign key for Store
        [Required]
        public string StoreId { get; set; }

        // Navigation property for Store
        public Store Store { get; set; }

    }
}
