using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAssignmentSubmissionRepository _assignmentSubmissionRepository;

        public AssignmentService(IModuleRepository moduleRepository, IAssignmentRepository assignmentRepository, IAssignmentSubmissionRepository assignmentSubmissionRepository)
        {
            _moduleRepository = moduleRepository;
            _assignmentRepository = assignmentRepository;
            _assignmentSubmissionRepository = assignmentSubmissionRepository;
        }

        public async Task CreateAssignmentAsync(int moduleId, int userId, string title, string description, int maxScore, DateTime dueAt)
        {

            var module = await _moduleRepository.GetByIdWithOwnershipCheckAsync(moduleId, userId);

            if (module == null)
            {
                throw new KeyNotFoundException("Module not found or access denied.");
            }

            if (await _assignmentRepository.TitleExistsInModuleAsync(title, moduleId))
            {
                throw new ArgumentException("Assignment with this title already exists.");
            }
            var assignment = new Assignment
            {
                Title = title,
                Description = description,
                ModuleId = moduleId,
                MaxScore = maxScore,
                DueAt = dueAt,
            };

            await _assignmentRepository.AddAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();
        }

        public async Task<string> GradeSubmissionAsync(int submissionId, int score, int userId)
        {

            var assignmentSubmission = await _assignmentSubmissionRepository.GetSubmissionByIdWithOwnershipCheckAsync(submissionId, userId);

            if (assignmentSubmission == null)
            {
                throw new KeyNotFoundException("Submission does not exist or not authorized.");
            }

            if (score < 0 || score > assignmentSubmission.Assignment.MaxScore)
            {
                throw new ArgumentException($"Score must be positive and not exceed: {assignmentSubmission.Assignment.MaxScore}");
            }

            var message = assignmentSubmission.Score == null ? "Assignment graded successfully." : "Assignment grade overridden successfully.";

            assignmentSubmission.Score = score;
            await _assignmentSubmissionRepository.SaveChangesAsync();

            return message;
        }

        public async Task SubmitAssignmentAsync(int assignmentId, int userId, string? answer)
        {

            var assignment = await _assignmentRepository.GetByIdWithEnrollmentCheckAsync(assignmentId, userId);

            if (assignment == null)
            {
                throw new UnauthorizedAccessException("Assignment not found or access denied.");
            }

            var duplicateCheck = await _assignmentSubmissionRepository.ExistsAsync(userId, assignmentId);


            if (duplicateCheck)
            {
                throw new ArgumentException("Student had submitted his response");
            }

            var assignmentSubmission = new AssignmentSubmission
            {
                UserId = userId,
                AssignmentId = assignmentId,
                Answer = answer,
                SubmissionTime = DateTime.UtcNow,
            };

            await _assignmentSubmissionRepository.AddAsync(assignmentSubmission);
            await _assignmentSubmissionRepository.SaveChangesAsync();
        }
    }
}
