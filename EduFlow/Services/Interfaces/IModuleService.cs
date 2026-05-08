using EduFlow.Models;

namespace EduFlow.Services.Interfaces
{
    public interface IModuleService
    {
        Task CreateModuleAsync(int courseId, string title, string description , int userId);
        Task<IEnumerable<Module>> GetMyModulesAsync(int courseId, int userId);
    }
}
