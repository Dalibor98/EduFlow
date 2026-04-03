using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok("Assignment created succesfully.");
        }
    }
}
