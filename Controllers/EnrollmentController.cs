using EduFlow.DTOs.Enrollment;
using EduFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }


        [HttpPost("enroll/{courseId}")]
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _enrollmentService.EnrollAsync(userId,courseId);
            
            return Ok("User enrolled succesfully");
        }

        [HttpDelete("unenroll/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _enrollmentService.UnenrollAsync(userId, courseId);
            
            return Ok("Unenrolled successfully.");
        }

        [HttpGet("myenrollments")]
        [Authorize(Roles="Student")]

        public async Task<IActionResult> GetMyEnrollemts()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var enrollments = await _enrollmentService.GetMyEnrollmentsAsync(userId);

            var response = enrollments
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.UserId,
                    CourseId = e.CourseId,
                    StudentFullName = e.User.FullName
                });
                
            return Ok(response);
        }
    }
}
