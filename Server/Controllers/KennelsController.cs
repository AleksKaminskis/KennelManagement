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
    public class KennelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KennelsController> _logger;

        public KennelsController(ApplicationDbContext context, ILogger<KennelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KennelDto>>> GetKennels()
        {
            try
            {
                var kennels = await _context.Kennels
                    .Select(k => new KennelDto
                    {
                        Id = k.Id,
                        KennelNumber = k.KennelNumber,
                        Size = k.Size,
                        IsOccupied = k.IsOccupied,
                        Notes = k.Notes,
                        CreatedAt = k.CreatedAt
                    })
                    .ToListAsync();

                return Ok(kennels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving kennels");
                return StatusCode(500, new { message = "An error occurred while retrieving kennels" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KennelDto>> GetKennel(int id)
        {
            try
            {
                var kennel = await _context.Kennels
                    .Where(k => k.Id == id)
                    .Select(k => new KennelDto
                    {
                        Id = k.Id,
                        KennelNumber = k.KennelNumber,
                        Size = k.Size,
                        IsOccupied = k.IsOccupied,
                        Notes = k.Notes,
                        CreatedAt = k.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (kennel == null)
                {
                    return NotFound(new { message = "Kennel not found" });
                }

                return Ok(kennel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving kennel {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the kennel" });
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<KennelDto>>> GetAvailableKennels()
        {
            try
            {
                var kennels = await _context.Kennels
                    .Where(k => !k.IsOccupied)
                    .Select(k => new KennelDto
                    {
                        Id = k.Id,
                        KennelNumber = k.KennelNumber,
                        Size = k.Size,
                        IsOccupied = k.IsOccupied,
                        Notes = k.Notes,
                        CreatedAt = k.CreatedAt
                    })
                    .ToListAsync();

                return Ok(kennels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available kennels");
                return StatusCode(500, new { message = "An error occurred while retrieving available kennels" });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<KennelDto>> CreateKennel([FromBody] CreateKennelDto createDto)
        {
            try
            {
                var kennel = new Kennel
                {
                    KennelNumber = createDto.KennelNumber,
                    Size = createDto.Size,
                    Notes = createDto.Notes,
                    IsOccupied = false
                };

                _context.Kennels.Add(kennel);
                await _context.SaveChangesAsync();

                var kennelDto = new KennelDto
                {
                    Id = kennel.Id,
                    KennelNumber = kennel.KennelNumber,
                    Size = kennel.Size,
                    IsOccupied = kennel.IsOccupied,
                    Notes = kennel.Notes,
                    CreatedAt = kennel.CreatedAt
                };

                return CreatedAtAction(nameof(GetKennel), new { id = kennel.Id }, kennelDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating kennel");
                return StatusCode(500, new { message = "An error occurred while creating the kennel" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateKennel(int id, [FromBody] UpdateKennelDto updateDto)
        {
            try
            {
                var kennel = await _context.Kennels.FindAsync(id);
                if (kennel == null)
                {
                    return NotFound(new { message = "Kennel not found" });
                }

                kennel.KennelNumber = updateDto.KennelNumber;
                kennel.Size = updateDto.Size;
                kennel.IsOccupied = updateDto.IsOccupied;
                kennel.Notes = updateDto.Notes;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating kennel {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the kennel" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteKennel(int id)
        {
            try
            {
                var kennel = await _context.Kennels.FindAsync(id);
                if (kennel == null)
                {
                    return NotFound(new { message = "Kennel not found" });
                }

                _context.Kennels.Remove(kennel);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting kennel {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the kennel" });
            }
        }
    }
}
