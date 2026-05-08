using EduFlow.Data;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class AssignmentSubmissionRepository : Repository<AssignmentSubmission>, IAssignmentSubmissionRepository
    {
        public AssignmentSubmissionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(int userId, int assignmentId)
        {
            return await _context.AssignmentSubmissions
                .AnyAsync(s => s.UserId == userId && s.AssignmentId == assignmentId);
        }

        public async Task<AssignmentSubmission?> GetSubmissionByIdWithOwnershipCheckAsync(int submissionId, int professorId)
        {
            return await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                .Where(s => s.Id == submissionId)
                .Where(s => s.Assignment.Module.Course.ProfessorId == professorId)
                .FirstOrDefaultAsync();
        }
    }
}
