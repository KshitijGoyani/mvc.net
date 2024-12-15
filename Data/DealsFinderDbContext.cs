using Microsoft.EntityFrameworkCore;
using Lab5.Models;

namespace Lab5.Data
{
    public class DealsFinderDbContext : DbContext
    {
        public DealsFinderDbContext(DbContextOptions<DealsFinderDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Flyer> Flyers { get; set; } // Add DbSet for Flyers

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Customer table
            modelBuilder.Entity<Customer>().ToTable("Customer");

            // Configure Store table
            modelBuilder.Entity<Store>().ToTable("Store");

            // Configure Subscription table
            modelBuilder.Entity<Subscription>().ToTable("Subscription");

            // Composite key for Subscription table
            modelBuilder.Entity<Subscription>()
                .HasKey(s => new { s.CustomerId, s.StoreId });

            // Configure Flyer table
            modelBuilder.Entity<Flyer>().ToTable("Flyer");

            modelBuilder.Entity<Flyer>()
                .Property(f => f.FileName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Flyer>()
                .Property(f => f.Url)
                .IsRequired();

            // Set up one-to-many relationship between Store and Flyer
            modelBuilder.Entity<Flyer>()
                .HasOne(f => f.Store)
                .WithMany(s => s.Flyers)
                .HasForeignKey(f => f.StoreId)
                .OnDelete(DeleteBehavior.Cascade); // If a store is deleted, its flyers are also deleted

        }
    }
}
