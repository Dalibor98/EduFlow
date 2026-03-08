using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EduFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                var token = GenerateToken(user);
                return Ok(token);
            }
            return Unauthorized("Invalid credentials.");
        }

        [Authorize(Roles = "Student")]
        [HttpGet("test")]
        public async Task<IActionResult> TestMethd()
        {
            return Ok();
        }
        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(issuer, audience,claims, expires: DateTime.UtcNow.AddHours(1),signingCredentials:credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}