using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Lab5.Models.ViewModels;

namespace Lab5.Controllers
{
    public class CustomersController : Controller
    {
        private readonly DealsFinderDbContext _context;

        public CustomersController(DealsFinderDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int? id)
        {
            var customers = await _context.Customers.ToListAsync();

            if (id != null)
            {
                // Fetch the selected customer and their subscriptions
                var selectedCustomer = await _context.Customers
                    .Include(c => c.Subscriptions)
                    .ThenInclude(s => s.Store)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (selectedCustomer != null)
                {
                    ViewData["SelectedCustomerName"] = $"{selectedCustomer.FirstName} {selectedCustomer.LastName}";
                    ViewData["SubscribedStores"] = selectedCustomer.Subscriptions
                        .Select(sub => sub.Store)
                        .ToList();
                }
            }

            var viewModel = new DealsViewModel
            {
                Customers = customers,
                Stores = _context.Stores.ToList() // Include all stores for completeness
            };

            return View(viewModel);
        }



        // GET: Customers/EditSubscriptions/5
        public IActionResult EditSubscriptions(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load the customer with subscriptions
            var customer = _context.Customers
                .Include(c => c.Subscriptions)
                .ThenInclude(s => s.Store)
                .FirstOrDefault(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Fetch all stores into memory
            var allStores = _context.Stores.ToList();

            // Perform the projection and sorting in memory
            var subscriptions = allStores
                .Select(store => new StoreSubscriptionViewModel
                {
                    StoreId = store.Id,
                    Title = store.Title,
                    IsMember = customer.Subscriptions.Any(sub => sub.StoreId == store.Id)
                })
                .OrderBy(s => !s.IsMember) // Sort subscribed stores first
                .ToList();

            var viewModel = new CustomerSubscriptionViewModel
            {
                Customer = customer,
                Subscriptions = subscriptions
            };

            return View(viewModel);
        }


        // POST: Customers/AddSubscription
        [HttpPost]
        public async Task<IActionResult> AddSubscription(int customerId, string storeId)
        {
            var subscription = new Subscription
            {
                CustomerId = customerId,
                StoreId = storeId
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(EditSubscriptions), new { id = customerId });
        }

        // POST: Customers/RemoveSubscription
        [HttpPost]
        public async Task<IActionResult> RemoveSubscription(int customerId, string storeId)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(sub => sub.CustomerId == customerId && sub.StoreId == storeId);

            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(EditSubscriptions), new { id = customerId });
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
