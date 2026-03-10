using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AdminController(AppDbContext context,IConfiguration configuration) 
        {
            _configuration = configuration;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-professor")]
        public async Task<IActionResult> RegisterProfessor(RegisterDto dto)
        {
            if(_context.Users.Any(u=>u.Email == dto.Email))
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

            _context.Users.Add(professor);

            await _context.SaveChangesAsync();
            return Ok("Professor registered succesfully.");
        }
        
    }
}
