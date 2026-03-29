using EduFlow.Data;
using EduFlow.DTOs;
using EduFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModuleController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Professor")]
        [HttpPost("create-module")]

        public async Task<IActionResult> CreateModule(ModuleCreateDto _dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var professor = await _context.Users.FirstOrDefaultAsync(u=> u.Email == email);

            if (professor == null)
            {
                return BadRequest("Professor doesn't exist.");
            }

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.ProfessorId == professor.Id && c.Id == _dto.CourseId);
            if(course == null)
            {
                return BadRequest("Course not found or acess denied.");
            }

            var module = new Module
            {
                Title = _dto.Title,
                Description = _dto.Description,
                CourseId = _dto.CourseId,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Add(module);
            await _context.SaveChangesAsync();
            return Ok("Module created successfully.");
        }
    }
}
