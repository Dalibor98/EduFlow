using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class QuizService : IQuizService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger<QuizService> _logger;

        public QuizService(IQuizRepository quizRepository,IModuleRepository moduleRepository, ILogger<QuizService> logger)
        {
            _quizRepository = quizRepository;
            _moduleRepository = moduleRepository;
            _logger = logger;
        }
        public async Task CreateQuizAsync(int moduleId, string title, string description, int userId)
        {
            var module = await _moduleRepository.GetByIdWithOwnershipCheckAsync(moduleId, userId);
            if (module == null)
            {
                _logger.LogWarning("Create quiz failed: module {ModuleId} not found or access denied for user {UserId}", moduleId, userId);
                throw new KeyNotFoundException("Module not found or access denied.");
            }

            if (await _quizRepository.TitleExistsInModuleAsync(title, moduleId))
            {
                throw new ArgumentException("Quiz with this title already exists.");
            }

            var quiz = new Quiz
            {
                Title = title,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                ModuleId = moduleId
            };

            await _quizRepository.AddAsync(quiz);
            await _quizRepository.SaveChangesAsync();
            _logger.LogInformation("Quiz {QuizId} created in module {ModuleId} by user {UserId}", quiz.Id, moduleId, userId);
        }
    }
}
