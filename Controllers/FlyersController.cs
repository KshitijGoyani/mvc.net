using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Lab5.Data;
using Lab5.Models;
using Lab5.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Controllers
{
    public class FlyersController : Controller
    {
        private readonly DealsFinderDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public FlyersController(DealsFinderDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Flyers/Index/{id}
        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var store = await _context.Stores
                .Include(s => s.Flyers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store == null)
            {
                return NotFound();
            }

            var viewModel = new FlyerViewModel
            {
                Store = store,
                Flyers = store.Flyers,
                FileInput = new FileInputViewModel
                {
                    StoreId = store.Id,
                    StoreTitle = store.Title
                }
            };

            return View(viewModel);
        }

        // GET: Flyers/Create/{id}
        [HttpGet]
        public IActionResult Create(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var viewModel = new FlyerViewModel
            {
                FileInput = new FileInputViewModel
                {
                    StoreId = id,
                    StoreTitle = _context.Stores.FirstOrDefault(s => s.Id == id)?.Title
                }
            };

            return View(viewModel);
        }

        // POST: Flyers/Create/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlyerViewModel model)
        {
            if (string.IsNullOrEmpty(model.FileInput.StoreId))
            {
                ModelState.AddModelError("", "Invalid store ID.");
                return View(model);
            }

            if (model.FileInput.File == null)
            {
                ModelState.AddModelError("FileInput.File", "Please select a file to upload.");
                return View(model);
            }

            try
            {
                // Validate file extension and size
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(model.FileInput.File.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("FileInput.File", "Only JPG and PNG files are allowed.");
                    return View(model);
                }

                if (model.FileInput.File.Length == 0)
                {
                    ModelState.AddModelError("FileInput.File", "File cannot be empty.");
                    return View(model);
                }

                // Upload to Azure Blob Storage
                var containerClient = _blobServiceClient.GetBlobContainerClient("flyers");
                await containerClient.CreateIfNotExistsAsync();
                await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                string uniqueFileName = Guid.NewGuid() + fileExtension;
                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                using (var stream = model.FileInput.File.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                var flyer = new Flyer
                {
                    FileName = uniqueFileName,
                    Url = blobClient.Uri.ToString(),
                    StoreId = model.FileInput.StoreId
                };

                // Save to database
                _context.Flyers.Add(flyer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { id = model.FileInput.StoreId });
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while uploading the file.");
                return View(model);
            }
        }

        // GET: Flyers/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var flyer = await _context.Flyers
                .Include(f => f.Store)
                .FirstOrDefaultAsync(f => f.FlyerId == id);

            if (flyer == null)
            {
                return NotFound();
            }

            return View(flyer);
        }

        // POST: Flyers/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flyer = await _context.Flyers.FindAsync(id);

            if (flyer != null)
            {
                try
                {
                    // Delete from Azure Blob Storage
                    var containerClient = _blobServiceClient.GetBlobContainerClient("flyers");
                    var blobClient = containerClient.GetBlobClient(flyer.FileName);
                    await blobClient.DeleteIfExistsAsync();

                    // Remove from database
                    _context.Flyers.Remove(flyer);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the flyer.";
                    return RedirectToAction("Error");
                }
            }

            return RedirectToAction(nameof(Index), new { id = flyer?.StoreId });
        }

        // GET: Flyers/Error
        public IActionResult Error()
        {
            ViewData["Message"] = TempData["ErrorMessage"];
            return View();
        }
    }
}
