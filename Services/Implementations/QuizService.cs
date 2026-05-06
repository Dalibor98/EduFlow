using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class QuizService : IQuizService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository,IModuleRepository moduleRepository)
        {
            _quizRepository = quizRepository;
            _moduleRepository = moduleRepository;
        }
        public async Task CreateQuizAsync(int moduleId, string title, string description, int userId)
        {
            var module = await _moduleRepository.GetByIdWithOwnershipCheckAsync(moduleId, userId);
            if (module == null)
            {
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
        }
    }
}
