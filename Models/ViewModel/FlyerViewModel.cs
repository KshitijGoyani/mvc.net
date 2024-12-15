

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
namespace Lab5.Models.ViewModel
{
    public class FileInputViewModel
    {
        public string StoreId { get; set; }
        public string StoreTitle { get; set; }
        public IFormFile File { get; set; }
    }

    public class FlyerViewModel
    {
        public Store Store { get; set; } // The store for which flyers are being managed
        public IEnumerable<Flyer> Flyers { get; set; } // List of existing flyers for the store
        public FileInputViewModel FileInput { get; set; } // Input for uploading a new flyer
    }
}
