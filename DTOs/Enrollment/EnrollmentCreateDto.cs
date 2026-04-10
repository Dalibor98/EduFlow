using System.ComponentModel.DataAnnotations;

namespace EduFlow.DTOs.Enrollment
{
    public class EnrollmentCreateDto
    {
        [Required]
        public required int CourseId {get; set;}
    }
}
