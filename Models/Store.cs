using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Lab5.Models
{
    public class Store
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [Display(Name = "Registration Number")]
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Fee { get; set; }

        // Navigation property for Subscriptions
        public ICollection<Subscription> Subscriptions { get; set; }

        // Navigation property for Flyers (One-to-Many relationship)
        public ICollection<Flyer> Flyers { get; set; }

    }
}
