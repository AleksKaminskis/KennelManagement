using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDataAsync(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Users
            await SeedUsersAsync(userManager);

            // Seed Customers
            await SeedCustomersAsync(context);

            // Seed Kennels
            await SeedKennelsAsync(context);

            // Seed Dogs
            await SeedDogsAsync(context);

            // Seed Bookings
            await SeedBookingsAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Staff", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Seed Admin
            if (await userManager.FindByEmailAsync("admin@kennel.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@kennel.com",
                    Email = "admin@kennel.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed Staff
            if (await userManager.FindByEmailAsync("staff@kennel.com") == null)
            {
                var staff = new ApplicationUser
                {
                    UserName = "staff@kennel.com",
                    Email = "staff@kennel.com",
                    FirstName = "Staff",
                    LastName = "Member",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(staff, "Staff123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staff, "Staff");
                }
            }

            // Seed Customer
            if (await userManager.FindByEmailAsync("customer@kennel.com") == null)
            {
                var customer = new ApplicationUser
                {
                    UserName = "customer@kennel.com",
                    Email = "customer@kennel.com",
                    FirstName = "John",
                    LastName = "Customer",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(customer, "Customer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }
            }
        }

        private static async Task SeedCustomersAsync(ApplicationDbContext context)
        {
            if (!context.Customers.Any())
            {
                var customers = new List<Customer>
            {
                new Customer
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@email.com",
                    PhoneNumber = "+353-1-234-5678",
                    Address = "123 Main St, Dublin, Ireland"
                },
                new Customer
                {
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah.j@email.com",
                    PhoneNumber = "+353-1-234-5679",
                    Address = "456 Oak Ave, Dublin, Ireland"
                },
                new Customer
                {
                    FirstName = "Michael",
                    LastName = "Brown",
                    Email = "m.brown@email.com",
                    PhoneNumber = "+353-1-234-5680",
                    Address = "789 Glanmire, Cork, Ireland"
                }
            };

                await context.Customers.AddRangeAsync(customers);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedKennelsAsync(ApplicationDbContext context)
        {
            if (!context.Kennels.Any())
            {
                var kennels = new List<Kennel>
            {
                new Kennel { KennelNumber = "K001", Size = "Small", IsOccupied = false },
                new Kennel { KennelNumber = "K002", Size = "Small", IsOccupied = false },
                new Kennel { KennelNumber = "K003", Size = "Medium", IsOccupied = false },
                new Kennel { KennelNumber = "K004", Size = "Medium", IsOccupied = false },
                new Kennel { KennelNumber = "K005", Size = "Large", IsOccupied = false },
                new Kennel { KennelNumber = "K006", Size = "Large", IsOccupied = false },
                new Kennel { KennelNumber = "K007", Size = "Extra Large", IsOccupied = false },
                new Kennel { KennelNumber = "K008", Size = "Extra Large", IsOccupied = false }
            };

                await context.Kennels.AddRangeAsync(kennels);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedDogsAsync(ApplicationDbContext context)
        {
            if (!context.Dogs.Any())
            {
                var customers = await context.Customers.ToListAsync();

                if (customers.Count >= 3)
                {
                    var dogs = new List<Dog>
                {
                    new Dog
                    {
                        Name = "Max",
                        Breed = "Golden Retriever",
                        Age = 3,
                        Color = "Golden",
                        Weight = 30.5,
                        MedicalNotes = "Up to date on all vaccinations",
                        CustomerId = customers[0].Id
                    },
                    new Dog
                    {
                        Name = "Bella",
                        Breed = "Labrador",
                        Age = 2,
                        Color = "Black",
                        Weight = 28.0,
                        MedicalNotes = "Allergic to chicken",
                        CustomerId = customers[0].Id
                    },
                    new Dog
                    {
                        Name = "Charlie",
                        Breed = "Beagle",
                        Age = 5,
                        Color = "Tri-color",
                        Weight = 12.5,
                        MedicalNotes = "Requires medication twice daily",
                        CustomerId = customers[1].Id
                    },
                    new Dog
                    {
                        Name = "Luna",
                        Breed = "German Shepherd",
                        Age = 4,
                        Color = "Black and Tan",
                        Weight = 35.0,
                        MedicalNotes = "No special requirements",
                        CustomerId = customers[2].Id
                    }
                };

                    await context.Dogs.AddRangeAsync(dogs);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedBookingsAsync(ApplicationDbContext context)
        {
            if (!context.Bookings.Any())
            {
                var dogs = await context.Dogs.ToListAsync();
                var kennels = await context.Kennels.ToListAsync();

                if (dogs.Any() && kennels.Any())
                {
                    var bookings = new List<Booking>
                {
                    new Booking
                    {
                        DogId = dogs[0].Id,
                        KennelId = kennels[0].Id,
                        CheckInDate = DateTime.UtcNow.AddDays(5),
                        CheckOutDate = DateTime.UtcNow.AddDays(10),
                        SpecialRequirements = "Needs evening walks",
                        Status = "Confirmed",
                        TotalCost = 250.00m
                    },
                    new Booking
                    {
                        DogId = dogs[1].Id,
                        KennelId = kennels[2].Id,
                        CheckInDate = DateTime.UtcNow.AddDays(7),
                        CheckOutDate = DateTime.UtcNow.AddDays(14),
                        SpecialRequirements = "Special diet provided",
                        Status = "Pending",
                        TotalCost = 350.00m
                    }
                };

                    await context.Bookings.AddRangeAsync(bookings);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
