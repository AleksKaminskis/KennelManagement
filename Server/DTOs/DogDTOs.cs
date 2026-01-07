using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class DogDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Color { get; set; } = string.Empty;
        public double Weight { get; set; }
        public string MedicalNotes { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDogDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Breed { get; set; } = string.Empty;

        [Range(0, 30)]
        public int Age { get; set; }

        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Range(0, 200)]
        public double Weight { get; set; }

        [StringLength(1000)]
        public string MedicalNotes { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }
    }

    public class UpdateDogDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Breed { get; set; } = string.Empty;

        [Range(0, 30)]
        public int Age { get; set; }

        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Range(0, 200)]
        public double Weight { get; set; }

        [StringLength(1000)]
        public string MedicalNotes { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }
    }
}
