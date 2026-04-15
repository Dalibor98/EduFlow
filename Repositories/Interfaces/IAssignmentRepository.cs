using EduFlow.Models;

namespace EduFlow.Repositories.Interfaces
{
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        Task <Assignment?> GetByIdWithEnrollmentCheckAsync(int assignmentId, int userId);
        Task<bool> TitleExistsInModuleAsync(string title, int moduleId);

    }
}
