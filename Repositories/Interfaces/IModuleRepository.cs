using EduFlow.Models;

namespace EduFlow.Repositories.Interfaces
{
    public interface IModuleRepository : IRepository<Module>
    {
        Task<IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId);
        Task<Module?> GetByIdWithOwnershipCheckAsync(int moduleId, int professorId);
    }
}
