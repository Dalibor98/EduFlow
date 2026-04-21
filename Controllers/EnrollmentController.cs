
using EduFlow.Data;
using EduFlow.DTOs.Enrollment;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        public EnrollmentController(IUserRepository userRepository, ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
        }


        [HttpPost("enroll")]
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> Enroll(EnrollmentCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var course = await _courseRepository.GetByIdAsync(dto.CourseId);

            if (course == null)
            {
                return BadRequest("Course doesn't exist");
            }

            if(await _enrollmentRepository.IsUserEnrolledAsync(userId, dto.CourseId))
            {
                return BadRequest("Student is already enrolled in this course");
            }

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = dto.CourseId,
                EnrolledAt = DateTime.UtcNow
            };

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();

            return Ok("User enrolled succesfully");
        }

        [HttpDelete("unenroll/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Unenroll(int courseId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                return BadRequest("Course doesn't exist");
            }

            var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(userId, courseId);

            if (enrollment == null)
            {
                return BadRequest("Student is not yet enrolled in this course");
            }

            await _enrollmentRepository.DeleteAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
            return Ok("Unenrolled successfully.");
        }

        [HttpGet("myenrollments")]
        [Authorize(Roles="Student")]

        public async Task<IActionResult> GetMyEnrollemts()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var enrollments = await _enrollmentRepository.GetAllByUserIdAsync(userId);

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
