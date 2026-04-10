using System.ComponentModel.DataAnnotations;

namespace EduFlow.DTOs.Assignment
{
    public class AssignmentCreateDto
    {
        [Required]
        [MaxLength(100)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Description { get; set; }

        [Required]
        [Range(1,int.MaxValue)]
        public int MaxScore { get; set; }
        
        [Required]
        public required DateTime DueAt {  get; set; } 
    }
}
