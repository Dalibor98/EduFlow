using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public CourseController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("create-course")]
        [Authorize(Roles ="Professor")]
        public async Task <IActionResult> CreateCourse(CourseCreateDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var professor = _context.Users.FirstOrDefault(u => u.Email == email);
            if (professor == null)
            {
                return Unauthorized("Professor doesn't exist");
            }
            var course = new Course
            {
                Description = dto.Description,
                Title = dto.Title,
                CreatedAt = DateTime.UtcNow,
                ProfessorId = professor.Id,
            };
            _context.Add(course);
            await _context.SaveChangesAsync();
            return Ok("Course created succesfully.");
        }
    }
}
