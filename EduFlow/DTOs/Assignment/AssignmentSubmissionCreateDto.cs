using System.ComponentModel.DataAnnotations;

namespace EduFlow.DTOs
{
    public class AssignmentSubmissionCreateDto
    {
        [MaxLength(5000)]
        public string? Answer {  get; set; }
    }
}
