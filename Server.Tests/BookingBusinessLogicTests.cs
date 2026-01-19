using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Server.Controllers;
using Server.Data;
using Server.DTOs;
using Server.Models;
using Xunit;

namespace Server.Tests
{
    public class BookingBusinessLogicTests
    {
        private ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateBooking_Overlap_ReturnsBadRequest()
        {
            // Arrange
            await using var context = CreateInMemoryContext(nameof(CreateBooking_Overlap_ReturnsBadRequest));
            var kennel = new Kennel { KennelNumber = "K-OLAP", Size = "Small", IsOccupied = false };
            context.Kennels.Add(kennel);
            await context.SaveChangesAsync();

            // Ensure dogs exist in the database so the controller's dog existence check passes
            var existingDog = new Dog { Name = "Fido", Breed = "Mixed", Age = 3, CustomerId = 0 };
            var newDog = new Dog { Name = "Buddy", Breed = "Beagle", Age = 2, CustomerId = 0 };
            context.Dogs.AddRange(existingDog, newDog);
            await context.SaveChangesAsync();

            // existing booking that occupies the kennel from day 2 to day 5
            var existing = new Booking
            {
                DogId = existingDog.Id,
                KennelId = kennel.Id,
                CheckInDate = new DateTime(2026, 2, 2),
                CheckOutDate = new DateTime(2026, 2, 5),
                Status = "Confirmed",
                TotalCost = 150m
            };
            context.Bookings.Add(existing);
            await context.SaveChangesAsync();

            var controller = new BookingsController(context, NullLogger<BookingsController>.Instance);

            // Attempt to create a new booking that overlaps (day 4 to day 7)
            var createDto = new CreateBookingDto
            {
                DogId = newDog.Id,
                KennelId = kennel.Id,
                CheckInDate = new DateTime(2026, 2, 4),
                CheckOutDate = new DateTime(2026, 2, 7),
                TotalCost = 150m
            };

            // Act
            var result = await controller.CreateBooking(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            var bad = result.Result as BadRequestObjectResult;
            Assert.Contains("not available", bad?.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateBooking_SwapKennel_UpdatesOccupancy()
        {
            // Arrange
            await using var context = CreateInMemoryContext(nameof(UpdateBooking_SwapKennel_UpdatesOccupancy));

            var kennelA = new Kennel { KennelNumber = "A", Size = "Small", IsOccupied = false };
            var kennelB = new Kennel { KennelNumber = "B", Size = "Small", IsOccupied = false };
            context.Kennels.AddRange(kennelA, kennelB);
            await context.SaveChangesAsync();

            // Booking initially in kennelA and CheckedIn
            var booking = new Booking
            {
                DogId = 10,
                KennelId = kennelA.Id,
                CheckInDate = DateTime.UtcNow.Date,
                CheckOutDate = DateTime.UtcNow.Date.AddDays(2),
                Status = "CheckedIn",
                TotalCost = 100m
            };
            context.Bookings.Add(booking);

            // Save and mark kennelA occupied to simulate current state
            await context.SaveChangesAsync();
            kennelA.IsOccupied = true;
            context.Kennels.Update(kennelA);
            await context.SaveChangesAsync();

            var controller = new BookingsController(context, NullLogger<BookingsController>.Instance);

            var updateDto = new UpdateBookingDto
            {
                DogId = booking.DogId,
                KennelId = kennelB.Id, // swap to B
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                SpecialRequirements = booking.SpecialRequirements,
                Status = "CheckedIn", // remain checked in
                TotalCost = booking.TotalCost
            };

            // Act
            var result = await controller.UpdateBooking(booking.Id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var updatedKennelA = await context.Kennels.FindAsync(kennelA.Id);
            var updatedKennelB = await context.Kennels.FindAsync(kennelB.Id);

            // After swap, kennelA should no longer be occupied (no other checked-in bookings)
            Assert.False(updatedKennelA!.IsOccupied);
            // KennelB should become occupied because this booking is CheckedIn
            Assert.True(updatedKennelB!.IsOccupied);
        }

        [Fact]
        public async Task UpdateBooking_SwapKennel_OldKennelStillOccupiedIfOtherCheckedIn()
        {
            // Arrange
            await using var context = CreateInMemoryContext(nameof(UpdateBooking_SwapKennel_OldKennelStillOccupiedIfOtherCheckedIn));

            var kennelA = new Kennel { KennelNumber = "A2", Size = "Small", IsOccupied = false };
            var kennelB = new Kennel { KennelNumber = "B2", Size = "Small", IsOccupied = false };
            context.Kennels.AddRange(kennelA, kennelB);
            await context.SaveChangesAsync();

            // Other booking remains CheckedIn in kennelA
            var other = new Booking
            {
                DogId = 20,
                KennelId = kennelA.Id,
                CheckInDate = DateTime.UtcNow.Date.AddDays(-1),
                CheckOutDate = DateTime.UtcNow.Date.AddDays(1),
                Status = "CheckedIn",
                TotalCost = 100m
            };
            context.Bookings.Add(other);

            // Booking to swap from kennelA to kennelB
            var booking = new Booking
            {
                DogId = 21,
                KennelId = kennelA.Id,
                CheckInDate = DateTime.UtcNow.Date,
                CheckOutDate = DateTime.UtcNow.Date.AddDays(2),
                Status = "CheckedIn",
                TotalCost = 120m
            };
            context.Bookings.Add(booking);

            await context.SaveChangesAsync();

            // Precondition: kennelA should be occupied
            kennelA.IsOccupied = true;
            context.Kennels.Update(kennelA);
            await context.SaveChangesAsync();

            var controller = new BookingsController(context, NullLogger<BookingsController>.Instance);

            var updateDto = new UpdateBookingDto
            {
                DogId = booking.DogId,
                KennelId = kennelB.Id,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                SpecialRequirements = booking.SpecialRequirements,
                Status = "CheckedIn",
                TotalCost = booking.TotalCost
            };

            // Act
            var result = await controller.UpdateBooking(booking.Id, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var updatedKennelA = await context.Kennels.FindAsync(kennelA.Id);
            var updatedKennelB = await context.Kennels.FindAsync(kennelB.Id);

            // kennelA should remain occupied because `other` booking is still CheckedIn
            Assert.True(updatedKennelA!.IsOccupied);
            // kennelB becomes occupied due to this booking
            Assert.True(updatedKennelB!.IsOccupied);
        }
    }
}
