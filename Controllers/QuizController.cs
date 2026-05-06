using EduFlow.DTOs.Quiz;
using EduFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
       

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost("{moduleId}")]
        [Authorize(Roles = "Professor")]
        public async Task<IActionResult> CreateQuiz(int moduleId, QuizCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _quizService.CreateQuizAsync(moduleId,dto.Title, dto.Description, userId);

            return Ok("Quiz created successfully.");
        }
    }
}