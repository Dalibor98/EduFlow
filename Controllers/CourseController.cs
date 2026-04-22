using EduFlow.DTOs.Course;
using EduFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost("create-course")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> CreateCourse(CourseCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _courseService.CreateCourseAsync(dto.Description, dto.Title, userId);

            return Ok("Course created succesfully.");
        }
    }
}