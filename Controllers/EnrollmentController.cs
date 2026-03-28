
using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrollmentController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("enroll")]
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> Enroll(EnrollmentDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var student = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var course = await _context.Courses.FirstOrDefaultAsync(c=>c.Id == dto.CourseId);

            if (course == null)
            {
                return BadRequest("Course doesn't exist");
            }

            if (student == null)
            {
                return Unauthorized("Student doesn't exist."); 
            }

            if(await _context.Enrollments.AnyAsync(e => e.UserId == student.Id && e.CourseId == dto.CourseId))
            {
                return BadRequest("Student is already enrolled in this course");
            }

            var enrollemt = new Enrollment
            {
                UserId = student.Id,
                CourseId = dto.CourseId,
                EnrolledAt = DateTime.UtcNow
            };

            _context.Add(enrollemt);
            await _context.SaveChangesAsync();

            return Ok("User enrolled succesfully");
        }

        [HttpDelete("unenroll/{courseId}")]
        [Authorize(Roles ="Student")]

        public async Task<IActionResult> Unenroll(int courseId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var student = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return BadRequest("Course doesn't exist");
            }

            if (student == null)
            {
                return Unauthorized("Student doesn't exist.");
            }

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == student.Id && e.CourseId == courseId);

            if (enrollment == null)
            {
                return BadRequest("Student is not yet enrolled in this course");
            }
            else
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                return Ok("Unenrolled succesfully.");
            }
        }

        [HttpGet("myenrollments")]
        [Authorize(Roles="Student")]

        public async Task<IActionResult> GetMyEnrollemts()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var student = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            

            if (student == null)
            {
                return Unauthorized("Student doesn't exist.");
            }

            var response = await _context.Enrollments
                .Where(e => e.UserId == student.Id)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.UserId,
                    CourseId = e.CourseId,
                    StudentFullName = e.User.FullName
                })
                .ToListAsync();

            return Ok(response);

        }
    }
}
