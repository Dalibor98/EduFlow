using EduFlow.Models;

namespace EduFlow.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetByIdAndProfessorAsync(int courseId, int professorId);
    }
}
