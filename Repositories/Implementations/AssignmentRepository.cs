using EduFlow.Data;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Assignment?> GetByIdWithEnrollmentCheckAsync(int assignmentId, int userId)
        {
            return await _context.Assignments
                .Where(a => a.Id == assignmentId)
                .Where(a => a.Module.Course.Enrollments.Any(e => e.UserId == userId))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> TitleExistsInModuleAsync(string title, int moduleId)
        {
            return await _context.Assignments.AnyAsync(a => a.Title == title && a.ModuleId == moduleId);
        }
    }
}
