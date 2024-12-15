using System.Collections.Generic;
using Lab5.Models;

namespace Lab5.Models.ViewModels
{
    public class DealsViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Store> Stores { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
