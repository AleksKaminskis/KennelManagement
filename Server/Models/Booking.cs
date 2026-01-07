using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int DogId { get; set; }
        public Dog Dog { get; set; } = null!;

        public int KennelId { get; set; }
        public Kennel Kennel { get; set; } = null!;

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [StringLength(1000)]
        public string SpecialRequirements { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, CheckedIn, CheckedOut, Cancelled

        public decimal TotalCost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
