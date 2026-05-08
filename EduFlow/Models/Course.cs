namespace EduFlow.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int ProfessorId { get; set; }
        public User Professor { get; set; } = null!;
        public ICollection<Module> Modules {  get; set; } = new HashSet<Module>();
        public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
    }
}