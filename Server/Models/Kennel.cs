using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Kennel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string KennelNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string Size { get; set; } = string.Empty; // Small, Medium, Large

        public bool IsOccupied { get; set; } = false;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
