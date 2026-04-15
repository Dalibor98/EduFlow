using EduFlow.DTOs;
using EduFlow.DTOs.Assignment;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAssignmentSubmissionRepository _assignmentSubmissionRepository;
        private readonly IModuleRepository _moduleRepository;

        public AssignmentController(IAssignmentRepository assignmentRepository,
            IAssignmentSubmissionRepository assignmentSubmissionRepository,
            IUserRepository userRepository,
            IModuleRepository moduleRepository)
        {
            _assignmentRepository = assignmentRepository;
            _assignmentSubmissionRepository = assignmentSubmissionRepository;
            _moduleRepository = moduleRepository;
        }

        [HttpPost("{moduleId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> CreateAssignment(int moduleId, AssignmentCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); 

            var module = await _moduleRepository.GetByIdWithOwnershipCheckAsync(moduleId, userId);

            if (module == null)
            {
                return NotFound("Module not found or access denied.");
            }     

            if(await _assignmentRepository.TitleExistsInModuleAsync(dto.Title, moduleId))
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

            await _assignmentRepository.AddAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();
            return Ok("Assignment created succesfully.");
        }

        [HttpPost("submit-assignment/{assignmentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitAssignment(int assignmentId,AssignmentSubmissionCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


            var assignment = await _assignmentRepository.GetByIdWithEnrollmentCheckAsync(assignmentId, userId);

            if(assignment == null)
            {
                return BadRequest("Assignment not found or access denied.");
            }

            var duplicateCheck = await _assignmentSubmissionRepository.ExistsAsync(userId, assignmentId);


            if (duplicateCheck)
            {
                return BadRequest("Student had submitted his response");
            }

            var assignmentSubmission = new AssignmentSubmission
            {
                UserId = userId,
                AssignmentId = assignmentId,
                Answer = dto.Answer,
                SubmissionTime = DateTime.UtcNow,
            };

            await _assignmentSubmissionRepository.AddAsync(assignmentSubmission);
            await _assignmentSubmissionRepository.SaveChangesAsync();
            return Ok("Assignment has been created succesfully.");
        }

        [HttpPatch("{submissionId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GradeSubmission(int submissionId,AssignmentGradeDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var assignmentSubmission = await _assignmentSubmissionRepository.GetSubmissionByIdWithOwnershipCheckAsync(submissionId, userId);

            if (assignmentSubmission == null)
            {
                return BadRequest("Submission does not exist or not authorized.");
            }

            if (dto.Score < 0 || dto.Score > assignmentSubmission.Assignment.MaxScore)
            {
                return BadRequest($"Score must be positive and not exceed: {assignmentSubmission.Assignment.MaxScore}");
            }

            var message = assignmentSubmission.Score == null ? "Assignment graded successfully." : "Assignment grade overridden successfully.";
            assignmentSubmission.Score = dto.Score;
            await _assignmentSubmissionRepository.SaveChangesAsync();
            return Ok(message);
        }
    }
}
