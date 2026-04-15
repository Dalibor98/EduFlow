using EduFlow.Models;

namespace EduFlow.Repositories.Interfaces
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetAllByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int userId, int courseId);
    }
}
