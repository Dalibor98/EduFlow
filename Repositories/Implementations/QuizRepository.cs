using EduFlow.Data;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;

namespace EduFlow.Repositories.Implementations
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        public QuizRepository(AppDbContext context) : base(context)
        {
        }
    }
}
