using System.ComponentModel.DataAnnotations;

namespace EduFlow.DTOs.Quiz
{
    public class QuizCreateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public required string Description { get; set; } 
    }
}
