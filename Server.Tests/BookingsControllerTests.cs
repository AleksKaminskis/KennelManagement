using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Server.Controllers;
using Server.Data;
using Server.Models;
using Xunit;

namespace Server.Tests
{
    public class BookingsControllerTests
    {
        private ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task UpdateBookingStatus_CheckedIn_SetsKennelOccupied()
        {
            // Arrange
            await using var context = CreateInMemoryContext(nameof(UpdateBookingStatus_CheckedIn_SetsKennelOccupied));
            var kennel = new Kennel { KennelNumber = "K1", Size = "Small", IsOccupied = false };
            context.Kennels.Add(kennel);
            await context.SaveChangesAsync();

            var booking = new Booking
            {
                DogId = 1,
                KennelId = kennel.Id,
                CheckInDate = DateTime.UtcNow,
                CheckOutDate = DateTime.UtcNow.AddDays(1),
                Status = "Pending",
                TotalCost = 0m
            };
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var controller = new BookingsController(context, NullLogger<BookingsController>.Instance);

            // Act
            var result = await controller.UpdateBookingStatus(booking.Id, "CheckedIn");

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedKennel = await context.Kennels.FindAsync(kennel.Id);
            Assert.True(updatedKennel!.IsOccupied);
        }

        [Fact]
        public async Task UpdateBookingStatus_CheckedOut_RecomputesOccupancy()
        {
            // Arrange
            await using var context = CreateInMemoryContext(nameof(UpdateBookingStatus_CheckedOut_RecomputesOccupancy));
            var kennel = new Kennel { KennelNumber = "K2", Size = "Medium", IsOccupied = false };
            context.Kennels.Add(kennel);
            await context.SaveChangesAsync();

            // Another booking already checked in (simulates other occupant)
            var otherBooking = new Booking
            {
                DogId = 2,
                KennelId = kennel.Id,
                CheckInDate = DateTime.UtcNow.AddDays(-1),
                CheckOutDate = DateTime.UtcNow.AddDays(1),
                Status = "CheckedIn"
            };
            context.Bookings.Add(otherBooking);

            // Booking that we'll update away from CheckedIn
            var booking = new Booking
            {
                DogId = 3,
                KennelId = kennel.Id,
                CheckInDate = DateTime.UtcNow,
                CheckOutDate = DateTime.UtcNow.AddDays(1),
                Status = "CheckedIn"
            };
            context.Bookings.Add(booking);

            await context.SaveChangesAsync();

            // Precondition: kennel should be occupied
            kennel.IsOccupied = true;
            context.Kennels.Update(kennel);
            await context.SaveChangesAsync();

            var controller = new BookingsController(context, NullLogger<BookingsController>.Instance);

            // Act: change this booking to CheckedOut
            var result = await controller.UpdateBookingStatus(booking.Id, "CheckedOut");

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedKennel = await context.Kennels.FindAsync(kennel.Id);
            // Because otherBooking remains CheckedIn, kennel should stay occupied
            Assert.True(updatedKennel!.IsOccupied);
        }
    }
}
