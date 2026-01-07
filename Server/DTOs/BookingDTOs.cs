using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int DogId { get; set; }
        public string DogName { get; set; } = string.Empty;
        public int KennelId { get; set; }
        public string KennelNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string SpecialRequirements { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBookingDto
    {
        [Required]
        public int DogId { get; set; }

        [Required]
        public int KennelId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [StringLength(1000)]
        public string SpecialRequirements { get; set; } = string.Empty;

        [Range(0, 10000)]
        public decimal TotalCost { get; set; }
    }

    public class UpdateBookingDto
    {
        [Required]
        public int DogId { get; set; }

        [Required]
        public int KennelId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [StringLength(1000)]
        public string SpecialRequirements { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        [Range(0, 10000)]
        public decimal TotalCost { get; set; }
    }
}
