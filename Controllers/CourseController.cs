using EduFlow.DTOs.Course;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        [HttpPost("create-course")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> CreateCourse(CourseCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var course = new Course
            {
                Description = dto.Description,
                Title = dto.Title,
                CreatedAt = DateTime.UtcNow,
                ProfessorId = userId,
            };

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();
            return Ok("Course created successfully.");
        }
    }
}