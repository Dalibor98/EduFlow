namespace EduFlow.Models
{
    public class Module
    {
        public int Id {  get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CourseId { get; set;}
        public Course Course { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ICollection<Quiz> Quizzes { get; set; } = new HashSet<Quiz>();
        public ICollection<Assignment> Assignments { get; set; } = new HashSet<Assignment>();
    }
}
