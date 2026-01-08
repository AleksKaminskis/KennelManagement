using System.ComponentModel.DataAnnotations;

namespace Client.Models
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

    public class BookingFormModel
    {
        [Required(ErrorMessage = "Dog is required")]
        public int DogId { get; set; }

        [Required(ErrorMessage = "Kennel is required")]
        public int KennelId { get; set; }

        [Required(ErrorMessage = "Check-in date is required")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Check-out date is required")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [StringLength(1000)]
        public string SpecialRequirements { get; set; } = string.Empty;

        [Required(ErrorMessage = "Total cost is required")]
        [Range(0, 10000, ErrorMessage = "Total cost must be between 0 and 10000")]
        public decimal TotalCost { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
