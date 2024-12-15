using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Lab5.Models.ViewModels;

namespace Lab5.Controllers
{
    public class StoresController : Controller
    {
        private readonly DealsFinderDbContext _context;

        public StoresController(DealsFinderDbContext context)
        {
            _context = context;
        }

        // GET: Stores
        public async Task<IActionResult> Index(string id = null)
        {
            var stores = await _context.Stores.Include(s => s.Subscriptions).ToListAsync();
            var customers = await _context.Customers.ToListAsync();
            var subscriptions = await _context.Subscriptions.ToListAsync();

            var viewModel = new DealsViewModel
            {
                Stores = stores,
                Customers = customers,
                Subscriptions = subscriptions
            };

            // Handle selected store
            if (!string.IsNullOrEmpty(id))
            {
                var selectedStore = stores.FirstOrDefault(s => s.Id == id);
                if (selectedStore != null)
                {
                    ViewData["SelectedStoreTitle"] = selectedStore.Title;
                    ViewData["Subscribers"] = subscriptions
                        .Where(sub => sub.StoreId == id)
                        .Select(sub => customers.FirstOrDefault(c => c.Id == sub.CustomerId))
                        .ToList();
                }
            }

            return View(viewModel);
        }


        // GET: Stores/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "Invalid store ID.");
                return NotFound();
            }

            var store = await _context.Stores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                ModelState.AddModelError("", "Store not found.");
                return NotFound();
            }

            return View(store);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] Store store)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(store);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Please try again.");
                }
            }
            return View(store);
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "Invalid store ID.");
                return NotFound();
            }

            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                ModelState.AddModelError("", "Store not found.");
                return NotFound();
            }
            return View(store);
        }

        // POST: Stores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] Store store)
        {
            if (id != store.Id)
            {
                ModelState.AddModelError("", "Store ID mismatch.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
                    {
                        ModelState.AddModelError("", "Store no longer exists in the database.");
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Concurrency error occurred. Please try again.");
                        throw;
                    }
                }
            }
            return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Stores
                .FirstOrDefaultAsync(m => m.Id == id);

            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var store = await _context.Stores
                .Include(s => s.Subscriptions) // Include related subscriptions
                .Include(s => s.Flyers) // Include related flyers
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store == null)
            {
                return NotFound();
            }

            // Check if the store has active subscriptions
            if (store.Subscriptions != null && store.Subscriptions.Any())
            {
                TempData["ErrorMessage"] = "This store cannot be deleted because it has active subscriptions.";
                return RedirectToAction("Error");
            }

            // Check if the store has associated flyers
            if (store.Flyers != null && store.Flyers.Any())
            {
                TempData["ErrorMessage"] = "This store cannot be deleted because it has associated flyers.";
                return RedirectToAction("Error");
            }

            // If no subscriptions or flyers exist, delete the store
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Error Page
        public IActionResult Error()
        {
            ViewData["Message"] = TempData["ErrorMessage"];
            return View();
        }
    

        private bool StoreExists(string id)
        {
            return _context.Stores.Any(e => e.Id == id);
        }
    }
}
