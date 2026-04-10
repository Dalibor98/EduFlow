using EduFlow.Data;
using EduFlow.DTOs.Quiz;
using EduFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public QuizController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{moduleId}")]
        [Authorize(Roles = "Professor")]

        public async Task<IActionResult> CreateQuiz (int moduleId, QuizCreateDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var professor = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (professor == null)
            {
                return BadRequest("Professor doesn't exist.");
            }
            var module = await _context.Modules.FirstOrDefaultAsync(m=> m.Id == moduleId);
            if (module == null)
            {
                return BadRequest("Module doesn't exist.");
            }
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.ProfessorId == professor.Id && c.Id == module.CourseId);
            if (course == null)
            {
                return BadRequest("Course not found or acess denied.");
            }
            if(await _context.Quizzes.AnyAsync(q=>q.Title == dto.Title && q.ModuleId == moduleId))
            {
                return BadRequest("Quiz with this title already exists");
            }
            var quiz = new Quiz
            {
                Title = dto.Title,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                ModuleId = moduleId
            };

            _context.Add(quiz);
            await _context.SaveChangesAsync();
            return Ok("Quiz created successfully.");
        }
    }
}
