using EduFlow.Data;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EduFlow.Repositories.Implementations
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        public QuizRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> TitleExistsInModuleAsync(string title, int moduleId)
        {
            return await _context.Quizzes.AnyAsync(q => q.Title == title && q.ModuleId == moduleId);
        }
    }
}
