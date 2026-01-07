using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class KennelDto
    {
        public int Id { get; set; }
        public string KennelNumber { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateKennelDto
    {
        [Required]
        [StringLength(50)]
        public string KennelNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Size { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateKennelDto
    {
        [Required]
        [StringLength(50)]
        public string KennelNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Size { get; set; } = string.Empty;

        public bool IsOccupied { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
}
