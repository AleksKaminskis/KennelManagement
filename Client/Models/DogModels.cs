using System.ComponentModel.DataAnnotations;

namespace Client.Models
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

    public class DogFormModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Breed is required")]
        [StringLength(100)]
        public string Breed { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required")]
        [Range(0, 30, ErrorMessage = "Age must be between 0 and 30")]
        public int Age { get; set; }

        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "Weight is required")]
        [Range(0, 200, ErrorMessage = "Weight must be between 0 and 200")]
        public double Weight { get; set; }

        [StringLength(1000)]
        public string MedicalNotes { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }
    }
}
