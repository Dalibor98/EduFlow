using System.ComponentModel.DataAnnotations;

namespace EduFlow.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(60)]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(60)]
        public required string Email { get; set; } 

        [Required]
        [MinLength(10)]
        [MaxLength(60)]
        public required string Password { get; set; } 
    }
}
