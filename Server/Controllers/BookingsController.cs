using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(ApplicationDbContext context, ILogger<BookingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            try
            {
                var bookings = await _context.Bookings
                    .Include(b => b.Dog)
                    .Include(b => b.Kennel)
                    .Select(b => new BookingDto
                    {
                        Id = b.Id,
                        DogId = b.DogId,
                        DogName = b.Dog.Name,
                        KennelId = b.KennelId,
                        KennelNumber = b.Kennel.KennelNumber,
                        CheckInDate = b.CheckInDate,
                        CheckOutDate = b.CheckOutDate,
                        SpecialRequirements = b.SpecialRequirements,
                        Status = b.Status,
                        TotalCost = b.TotalCost,
                        CreatedAt = b.CreatedAt
                    })
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings");
                return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Dog)
                    .Include(b => b.Kennel)
                    .Where(b => b.Id == id)
                    .Select(b => new BookingDto
                    {
                        Id = b.Id,
                        DogId = b.DogId,
                        DogName = b.Dog.Name,
                        KennelId = b.KennelId,
                        KennelNumber = b.Kennel.KennelNumber,
                        CheckInDate = b.CheckInDate,
                        CheckOutDate = b.CheckOutDate,
                        SpecialRequirements = b.SpecialRequirements,
                        Status = b.Status,
                        TotalCost = b.TotalCost,
                        CreatedAt = b.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the booking" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] CreateBookingDto createDto)
        {
            try
            {
                // Verify dog exists
                var dogExists = await _context.Dogs.AnyAsync(d => d.Id == createDto.DogId);
                if (!dogExists)
                {
                    return BadRequest(new { message = "Dog not found" });
                }

                // Verify kennel exists and is available
                var kennel = await _context.Kennels.FindAsync(createDto.KennelId);
                if (kennel == null)
                {
                    return BadRequest(new { message = "Kennel not found" });
                }

                // Check for overlapping bookings
                var hasOverlap = await _context.Bookings
                    .AnyAsync(b => b.KennelId == createDto.KennelId &&
                                 b.Status != "Cancelled" &&
                                 ((createDto.CheckInDate >= b.CheckInDate && createDto.CheckInDate < b.CheckOutDate) ||
                                  (createDto.CheckOutDate > b.CheckInDate && createDto.CheckOutDate <= b.CheckOutDate) ||
                                  (createDto.CheckInDate <= b.CheckInDate && createDto.CheckOutDate >= b.CheckOutDate)));

                if (hasOverlap)
                {
                    return BadRequest(new { message = "Kennel is not available for the selected dates" });
                }

                var booking = new Booking
                {
                    DogId = createDto.DogId,
                    KennelId = createDto.KennelId,
                    CheckInDate = createDto.CheckInDate,
                    CheckOutDate = createDto.CheckOutDate,
                    SpecialRequirements = createDto.SpecialRequirements,
                    TotalCost = createDto.TotalCost,
                    Status = "Pending"
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var dog = await _context.Dogs.FindAsync(createDto.DogId);
                var bookingDto = new BookingDto
                {
                    Id = booking.Id,
                    DogId = booking.DogId,
                    DogName = dog!.Name,
                    KennelId = booking.KennelId,
                    KennelNumber = kennel.KennelNumber,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    SpecialRequirements = booking.SpecialRequirements,
                    Status = booking.Status,
                    TotalCost = booking.TotalCost,
                    CreatedAt = booking.CreatedAt
                };

                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, bookingDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, new { message = "An error occurred while creating the booking" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto updateDto)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                // Check for overlapping bookings if dates or kennel changed
                if (booking.KennelId != updateDto.KennelId ||
                    booking.CheckInDate != updateDto.CheckInDate ||
                    booking.CheckOutDate != updateDto.CheckOutDate)
                {
                    var hasOverlap = await _context.Bookings
                        .AnyAsync(b => b.Id != id &&
                                     b.KennelId == updateDto.KennelId &&
                                     b.Status != "Cancelled" &&
                                     ((updateDto.CheckInDate >= b.CheckInDate && updateDto.CheckInDate < b.CheckOutDate) ||
                                      (updateDto.CheckOutDate > b.CheckInDate && updateDto.CheckOutDate <= b.CheckOutDate) ||
                                      (updateDto.CheckInDate <= b.CheckInDate && updateDto.CheckOutDate >= b.CheckOutDate)));

                    if (hasOverlap)
                    {
                        return BadRequest(new { message = "Kennel is not available for the selected dates" });
                    }
                }

                booking.DogId = updateDto.DogId;
                booking.KennelId = updateDto.KennelId;
                booking.CheckInDate = updateDto.CheckInDate;
                booking.CheckOutDate = updateDto.CheckOutDate;
                booking.SpecialRequirements = updateDto.SpecialRequirements;
                booking.Status = updateDto.Status;
                booking.TotalCost = updateDto.TotalCost;
                booking.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the booking" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found" });
                }
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the booking" });
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string status)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                booking.Status = status;
                booking.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the booking status" });
            }
        }
    }
}
