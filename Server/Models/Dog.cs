using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Dog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Breed { get; set; } = string.Empty;

        public int Age { get; set; }

        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        public double Weight { get; set; }

        [StringLength(1000)]
        public string MedicalNotes { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
