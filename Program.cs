using Lab5.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;

namespace Lab5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(); // Add Razor Pages support

            // Configure database connection
            var connection = builder.Configuration.GetConnectionString("DefaultDBConnection");
            builder.Services.AddDbContext<DealsFinderDbContext>(options => options.UseSqlServer(connection));

            // Configure Azure Blob Storage
            var blobConnection = builder.Configuration.GetConnectionString("AzureBlobStorage");
            builder.Services.AddSingleton(new BlobServiceClient(blobConnection));

            // Add session support
            builder.Services.AddSession();

            var app = builder.Build();

            // Database seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DealsFinderDbContext>();
                    DbInitializer.Initialize(context); // Optional: If you have a DbInitializer for seeding.
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            // Map routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Map Razor Pages for flyers
            app.MapRazorPages();

            app.Run();
        }
    }
}
