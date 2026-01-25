using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class KennelDto
    {
        public int Id { get; set; }
        public string KennelNumber { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Occupancy information
        public string DogName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerPhone { get; set; } = string.Empty;
    }

    public class KennelFormModel
    {
        [Required(ErrorMessage = "Kennel number is required")]
        [StringLength(50)]
        public string KennelNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Size is required")]
        [StringLength(50)]
        public string Size { get; set; } = string.Empty;

        public bool IsOccupied { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
}
