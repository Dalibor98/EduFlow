using EduFlow.Models;

namespace EduFlow.Repositories.Interfaces
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetAllByUserIdAsync(int userId);
        Task<bool> IsUserEnrolledAsync(int userId, int courseId);
        Task<Enrollment?> GetByUserAndCourseAsync(int userId, int courseId);
    }
}
