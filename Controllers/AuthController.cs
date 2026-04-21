using EduFlow.DTOs.Auth;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
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
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if(await _userRepository.GetByEmailAsync(dto.Email) != null)
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
            await _userRepository.AddAsync(user);

            await _userRepository.SaveChangesAsync();

            return Ok("Student registered succesfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                var token = GenerateToken(user);
                return Ok(token);
            }
            return Unauthorized("Invalid credentials.");
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"] 
                ?? throw new InvalidOperationException("JWT Secret is not configured.")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _configuration["JwtSettings:Issuer"] 
                ?? throw new InvalidOperationException("JWT Issuer is not configured.");

            var audience = _configuration["JwtSettings:Audience"] 
                ?? throw new InvalidOperationException("JWT Audience is not configured.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var token = new JwtSecurityToken(issuer, audience,claims, expires: DateTime.UtcNow.AddHours(1),signingCredentials:credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}