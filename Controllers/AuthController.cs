using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if(_context.Users.Any(u => u.Email == dto.Email))
            {
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Student",
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok("User registered succesfully");
        }
    }
}