using EduFlow.Models;

namespace EduFlow.Repositories.Interfaces
{
    public interface IAssignmentSubmissionRepository : IRepository<AssignmentSubmission>
    {
        Task<AssignmentSubmission?> GetSubmissionByIdWithOwnershipCheckAsync(int submissionId, int professorId);
        Task<bool> ExistsAsync(int userId, int assignmentId);
    }
}
