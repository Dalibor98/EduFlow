using System.ComponentModel.DataAnnotations;

namespace EduFlow.DTOs.Assignment
{
    public class AssignmentGradeDto
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Score { get; set; }
    }
}
