using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab5.Models
{
    public class Subscription
    {
        [Key, Column(Order = 0)]
        public int CustomerId { get; set; }

        [Key, Column(Order = 1)]
        public string StoreId { get; set; }

        public Customer Customer { get; set; }
        public Store Store { get; set; }

    }
}
