using System.Collections.Generic;

namespace Lab5.Models.ViewModels
{
    public class CustomerSubscriptionViewModel
    {
        public Customer Customer { get; set; }
        public IEnumerable<StoreSubscriptionViewModel> Subscriptions { get; set; }
    }

    public class StoreSubscriptionViewModel
    {
        public string StoreId { get; set; }
        public string Title { get; set; }
        public bool IsMember { get; set; }
    }
}
