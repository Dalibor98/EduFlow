namespace EduFlow.Models
{
    public class AssignmentSubmission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int? Score { get; set; }
        public int AssignmentId { get; set; }
        public string? Answer { get; set; }
        public DateTime SubmissionTime { get; set; }
        public Assignment Assignment { get; set; } = null!;
    }
}
