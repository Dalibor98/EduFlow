using EduFlow.Data;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class ModuleRepository : Repository<Module> , IModuleRepository
    {
        public ModuleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Module?> GetByIdWithOwnershipCheckAsync(int moduleId, int professorId)
        {
            return await _context.Modules.FirstOrDefaultAsync(m => m.Id == moduleId && m.Course.ProfessorId == professorId);
        }

        public async Task <IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId)
        {
            return await _context.Modules.Where(m => m.CourseId == courseId).ToListAsync();
        }
    }
}
