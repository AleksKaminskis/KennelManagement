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
    public class DogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DogsController> _logger;

        public DogsController(ApplicationDbContext context, ILogger<DogsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DogDto>>> GetDogs()
        {
            try
            {
                var dogs = await _context.Dogs
                    .Include(d => d.Customer)
                    .Select(d => new DogDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Breed = d.Breed,
                        Age = d.Age,
                        Color = d.Color,
                        Weight = d.Weight,
                        MedicalNotes = d.MedicalNotes,
                        CustomerId = d.CustomerId,
                        CustomerName = $"{d.Customer.FirstName} {d.Customer.LastName}",
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return Ok(dogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dogs");
                return StatusCode(500, new { message = "An error occurred while retrieving dogs" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DogDto>> GetDog(int id)
        {
            try
            {
                var dog = await _context.Dogs
                    .Include(d => d.Customer)
                    .Where(d => d.Id == id)
                    .Select(d => new DogDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        Breed = d.Breed,
                        Age = d.Age,
                        Color = d.Color,
                        Weight = d.Weight,
                        MedicalNotes = d.MedicalNotes,
                        CustomerId = d.CustomerId,
                        CustomerName = $"{d.Customer.FirstName} {d.Customer.LastName}",
                        CreatedAt = d.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (dog == null)
                {
                    return NotFound(new { message = "Dog not found" });
                }

                return Ok(dog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dog {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the dog" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<DogDto>> CreateDog([FromBody] CreateDogDto createDto)
        {
            try
            {
                // Verify customer exists
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == createDto.CustomerId);
                if (!customerExists)
                {
                    return BadRequest(new { message = "Customer not found" });
                }

                var dog = new Dog
                {
                    Name = createDto.Name,
                    Breed = createDto.Breed,
                    Age = createDto.Age,
                    Color = createDto.Color,
                    Weight = createDto.Weight,
                    MedicalNotes = createDto.MedicalNotes,
                    CustomerId = createDto.CustomerId
                };

                _context.Dogs.Add(dog);
                await _context.SaveChangesAsync();

                var customer = await _context.Customers.FindAsync(createDto.CustomerId);
                var dogDto = new DogDto
                {
                    Id = dog.Id,
                    Name = dog.Name,
                    Breed = dog.Breed,
                    Age = dog.Age,
                    Color = dog.Color,
                    Weight = dog.Weight,
                    MedicalNotes = dog.MedicalNotes,
                    CustomerId = dog.CustomerId,
                    CustomerName = $"{customer!.FirstName} {customer.LastName}",
                    CreatedAt = dog.CreatedAt
                };

                return CreatedAtAction(nameof(GetDog), new { id = dog.Id }, dogDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dog");
                return StatusCode(500, new { message = "An error occurred while creating the dog" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateDog(int id, [FromBody] UpdateDogDto updateDto)
        {
            try
            {
                var dog = await _context.Dogs.FindAsync(id);
                if (dog == null)
                {
                    return NotFound(new { message = "Dog not found" });
                }

                // Verify customer exists
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == updateDto.CustomerId);
                if (!customerExists)
                {
                    return BadRequest(new { message = "Customer not found" });
                }

                dog.Name = updateDto.Name;
                dog.Breed = updateDto.Breed;
                dog.Age = updateDto.Age;
                dog.Color = updateDto.Color;
                dog.Weight = updateDto.Weight;
                dog.MedicalNotes = updateDto.MedicalNotes;
                dog.CustomerId = updateDto.CustomerId;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating dog {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the dog" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDog(int id)
        {
            try
            {
                var dog = await _context.Dogs.FindAsync(id);
                if (dog == null)
                {
                    return NotFound(new { message = "Dog not found" });
                }

                _context.Dogs.Remove(dog);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting dog {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the dog" });
            }
        }
    }
}
