using EduFlow.Models;

namespace EduFlow.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task EnrollAsync(int userId, int courseId);
        Task UnenrollAsync(int userId, int courseId);
        Task<IEnumerable<Enrollment>> GetMyEnrollmentsAsync(int userId);
    }
}
