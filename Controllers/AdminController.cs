using EduFlow.DTOs.Auth;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public AdminController(IUserRepository userRepository) 
        {
            _userRepository = userRepository;   
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-professor")]
        public async Task<IActionResult> RegisterProfessor(RegisterDto dto)
        {
            if(await _userRepository.GetByEmailAsync(dto.Email) != null)
            {
                return BadRequest("Professor with this email exists");
            }
            var professor = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                FullName = dto.FullName,
                Role = "Professor"

            };

            await _userRepository.AddAsync(professor);

            await _userRepository.SaveChangesAsync();
            return Ok("Professor registered succesfully.");
        }
        
    }
}
