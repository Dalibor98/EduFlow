using EduFlow.Models;

namespace EduFlow.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetByIdAndProfessorAsync(int courseId, int professorId);

        Task<bool> TitleExistsForProfessorAsync(string title, int professorId);
    }
}
