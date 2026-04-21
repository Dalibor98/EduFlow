using EduFlow.DTOs.Quiz;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IModuleRepository _moduleRepository;

        public QuizController(IQuizRepository quizRepository, IModuleRepository moduleRepository)
        {
            _quizRepository = quizRepository;
            _moduleRepository = moduleRepository;
        }

        [HttpPost("{moduleId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> CreateQuiz(int moduleId, QuizCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var module = await _moduleRepository.GetByIdWithOwnershipCheckAsync(moduleId, userId);
            if (module == null)
            {
                return NotFound("Module not found or access denied.");
            }

            if (await _quizRepository.TitleExistsInModuleAsync(dto.Title, moduleId))
            {
                return BadRequest("Quiz with this title already exists.");
            }

            var quiz = new Quiz
            {
                Title = dto.Title,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                ModuleId = moduleId
            };

            await _quizRepository.AddAsync(quiz);
            await _quizRepository.SaveChangesAsync();
            return Ok("Quiz created successfully.");
        }
    }
}