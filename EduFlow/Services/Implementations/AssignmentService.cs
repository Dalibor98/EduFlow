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
        private readonly ILogger<AssignmentService> _logger;

        public AssignmentService(IModuleRepository moduleRepository, IAssignmentRepository assignmentRepository, IAssignmentSubmissionRepository assignmentSubmissionRepository, ILogger<AssignmentService> logger)
        {
            _moduleRepository = moduleRepository;
            _assignmentRepository = assignmentRepository;
            _assignmentSubmissionRepository = assignmentSubmissionRepository;
            _logger = logger;
        }

        public async Task CreateAssignmentAsync(int moduleId, int userId, string title, string description, int maxScore, DateTime dueAt)
        {

            var module = await _moduleRepository.GetByIdWithOwnershipCheckAsync(moduleId, userId);

            if (module == null)
            {
                _logger.LogWarning("Create Assignment failed: module {ModuleId} not found or access denied for user {UserId}", moduleId, userId);

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
            _logger.LogInformation("Assignment {AssignmentId} created in module {ModuleId} by user {UserId}", assignment.Id, moduleId, userId);
        }

        public async Task<string> GradeSubmissionAsync(int submissionId, int score, int userId)
        {

            var assignmentSubmission = await _assignmentSubmissionRepository.GetSubmissionByIdWithOwnershipCheckAsync(submissionId, userId);

            if (assignmentSubmission == null)
            {
                _logger.LogWarning("Grading submission failed: submission {SubmissionId} not found or access denied for user {UserId}", submissionId, userId);

                throw new KeyNotFoundException("Submission does not exist or not authorized.");
            }

            if (score < 0 || score > assignmentSubmission.Assignment.MaxScore)
            {
                throw new ArgumentException($"Score must be positive and not exceed: {assignmentSubmission.Assignment.MaxScore}");
            }

            var message = assignmentSubmission.Score == null ? "Assignment graded successfully." : "Assignment grade overridden successfully.";

            var isOverride = assignmentSubmission.Score != null;

            assignmentSubmission.Score = score;

            await _assignmentSubmissionRepository.SaveChangesAsync();

            if (isOverride)
                _logger.LogInformation("Submission {SubmissionId} grade overridden to {Score} by user {UserId}", submissionId, score, userId);
            else
                _logger.LogInformation("Submission {SubmissionId} graded {Score} by user {UserId}", submissionId, score, userId);

            return message;
        }

        public async Task SubmitAssignmentAsync(int assignmentId, int userId, string? answer)
        {

            var assignment = await _assignmentRepository.GetByIdWithEnrollmentCheckAsync(assignmentId, userId);

            if (assignment == null)
            {
                _logger.LogWarning("Assignment submission failed: assignment {AssignmentId} not found or access denied for user {UserId}", assignmentId, userId);

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
            _logger.LogInformation( "Assignment {AssignmentId} submitted by user {UserId}", assignmentId, userId);

        }
    }
}
