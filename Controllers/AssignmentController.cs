using EduFlow.DTOs;
using EduFlow.DTOs.Assignment;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
        _assignmentService = assignmentService;
        }

        [HttpPost("{moduleId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> CreateAssignment(int moduleId, AssignmentCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); 
            await _assignmentService.CreateAssignmentAsync(moduleId,userId,dto.Title,dto.Description,dto.MaxScore,dto.DueAt);

            return Ok("Assignment created succesfully.");
        }

        [HttpPost("submit-assignment/{assignmentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitAssignment(int assignmentId,AssignmentSubmissionCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _assignmentService.SubmitAssignmentAsync(assignmentId, userId, dto.Answer);
            return Ok("Assignment has been created succesfully.");
        }

        [HttpPatch("{submissionId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> GradeSubmission(int submissionId,AssignmentGradeDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var message = await _assignmentService.GradeSubmissionAsync(submissionId, dto.Score, userId);

            return Ok(message);
        }
    }
}
