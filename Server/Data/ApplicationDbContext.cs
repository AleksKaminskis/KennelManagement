using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<Kennel> Kennels { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Dog>()
                .HasOne(d => d.Customer)
                .WithMany(c => c.Dogs)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Dog)
                .WithMany(d => d.Bookings)
                .HasForeignKey(b => b.DogId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Kennel)
                .WithMany(k => k.Bookings)
                .HasForeignKey(b => b.KennelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision
            builder.Entity<Booking>()
                .Property(b => b.TotalCost)
                .HasPrecision(18, 2);

            // Add indexes
            builder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            builder.Entity<Kennel>()
                .HasIndex(k => k.KennelNumber)
                .IsUnique();
        }
    }
}
