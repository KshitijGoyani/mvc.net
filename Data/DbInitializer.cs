using System;
using System.Linq;
using Lab5.Models;

namespace Lab5.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DealsFinderDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Customers.Any())
            {
                return;   // DB has already been seeded
            }

            var customers = new Customer[]
            {
                new Customer { FirstName = "Carson", LastName = "Alexander", BirthDate = DateTime.Parse("1995-01-09") },
                new Customer { FirstName = "Meredith", LastName = "Alonso", BirthDate = DateTime.Parse("1992-09-05") },
                new Customer { FirstName = "Arturo", LastName = "Anand", BirthDate = DateTime.Parse("1993-10-09") },
                new Customer { FirstName = "Gytis", LastName = "Barzdukas", BirthDate = DateTime.Parse("1992-01-09") }
            };

            foreach (var customer in customers)
            {
                context.Customers.Add(customer);
            }
            context.SaveChanges();

            // Seed Stores
            var stores = new Store[]
            {
                new Store { Id = "A1", Title = "Alpha", Fee = 300 },
                new Store { Id = "B1", Title = "Beta", Fee = 130 },
                new Store { Id = "O1", Title = "Omega", Fee = 390 }
            };

            foreach (var store in stores)
            {
                context.Stores.Add(store);
            }
            context.SaveChanges();

            var subscriptions = new Subscription[]
            {
                new Subscription { CustomerId = customers[0].Id, StoreId = "A1" },
                new Subscription { CustomerId = customers[0].Id, StoreId = "B1" },
                new Subscription { CustomerId = customers[0].Id, StoreId = "O1" },
                new Subscription { CustomerId = customers[1].Id, StoreId = "A1" },
                new Subscription { CustomerId = customers[1].Id, StoreId = "B1" },
                new Subscription { CustomerId = customers[2].Id, StoreId = "A1" }
            };

            foreach (var subscription in subscriptions)
            {
                context.Subscriptions.Add(subscription);
            }
            context.SaveChanges();
        }
    }
}
