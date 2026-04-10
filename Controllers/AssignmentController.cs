using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.DTOs.Assignment;
using EduFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssignmentController (AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{moduleId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> CreateAssignment(int moduleId, AssignmentCreateDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var professor = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (professor == null)
            {
                return NotFound("Professor doesn't exist.");
            }

            var module = await _context.Modules
                .Where(m => m.Id == moduleId && m.Course.ProfessorId == professor.Id)
                .FirstOrDefaultAsync();

            if (module == null)
            {
                return NotFound("Module not found or access denied.");
            }     

            if(await _context.Assignments.AnyAsync(a => a.Title == dto.Title && a.ModuleId == moduleId))
            {
                return BadRequest("Assignment with this title already exists.");
            }
            var assignment = new Assignment
            {
                Title = dto.Title,
                Description = dto.Description,
                ModuleId = moduleId,
                MaxScore = dto.MaxScore,
                DueAt = dto.DueAt,
            };

            await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
            return Ok("Assignment created succesfully.");
        }

        [HttpPost("submit-assignment/{assignmentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitAssignment(int assignmentId,AssignmentSubmissionCreateDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var student = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (student == null)
            {
                return BadRequest("Not authorized.");
            }

            var assignment = await _context.Assignments
                .Where(a => a.Id == assignmentId && a.Module.Course.Enrollments.Any(e => e.UserId == student.Id))
                .FirstOrDefaultAsync();

            if(assignment == null)
            {
                return BadRequest("Assignment not found or access denied.");
            }

            var duplicateCheck = await _context.AssignmentSubmissions
                .AnyAsync(a => a.UserId == student.Id && a.AssignmentId == assignmentId);
                

            if (duplicateCheck)
            {
                return BadRequest("Student had submitted his response");
            }

            var assignmentSubmission = new AssignmentSubmission
            {
                UserId = student.Id,
                AssignmentId = assignmentId,
                Answer = dto.Answer,
                SubmissionTime = DateTime.UtcNow,
            };

            await _context.AssignmentSubmissions.AddAsync(assignmentSubmission);
            await _context.SaveChangesAsync();
            return Ok("Assignment has been created succesfully.");
        }

        [HttpPatch("{submissionId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GradeSubmission(int submissionId,AssignmentGradeDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var professor = await _context.Users.FirstOrDefaultAsync(u=> u.Email == email);
            if (professor == null)
            {
                return BadRequest("Not authorized.");
            }

            var assignmentSubmission = await _context.AssignmentSubmissions
                .Include(asub=> asub.Assignment)
                .Where(asub=> asub.Id == submissionId && asub.Assignment.Module.Course.ProfessorId==professor.Id)
                .FirstOrDefaultAsync();

            if (assignmentSubmission == null)
            {
                return BadRequest("Submission does not exist or not authorized.");
            }

            if (dto.Score < 0 || dto.Score > assignmentSubmission.Assignment.MaxScore)
            {
                return BadRequest($"Score must be positive and not exceed: {assignmentSubmission.Assignment.MaxScore}");
            }

            if (assignmentSubmission.Score == null)
            {
                assignmentSubmission.Score = dto.Score;
                await _context.SaveChangesAsync();
                return Ok("Assignment graded succesfully.");
            }

            assignmentSubmission.Score = dto.Score;
            await _context.SaveChangesAsync();
            return Ok("Assignment grade overriden succesfully.");
        }
    }
}
