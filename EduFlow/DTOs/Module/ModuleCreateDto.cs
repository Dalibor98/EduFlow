using System.ComponentModel.DataAnnotations;

namespace EduFlow.DTOs.Module
{
    public class ModuleCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = null!;
    }
}
          