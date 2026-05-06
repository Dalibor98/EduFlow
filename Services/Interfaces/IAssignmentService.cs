namespace EduFlow.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task CreateAssignmentAsync(int moduleId,int userId, string title, string description, int maxScore, DateTime dueAt);
        Task SubmitAssignmentAsync(int assignmentId, int userId, string? answer);
        Task<string> GradeSubmissionAsync(int submissionId, int score, int userId);
    }
}
