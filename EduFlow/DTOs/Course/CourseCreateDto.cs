using System.ComponentModel.DataAnnotations;

namespace EduFlow.DTOs.Course
{
    public class CourseCreateDto
    {
        [Required]
        [MaxLength(500)]
        public required string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }
    }
}
