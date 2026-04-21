using EduFlow.Models;
namespace EduFlow.Repositories.Interfaces
{
    public interface IQuizRepository : IRepository<Quiz>
    {
        Task<bool> TitleExistsInModuleAsync(string title, int moduleId);
    }
}
