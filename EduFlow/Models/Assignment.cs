namespace EduFlow.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public int MaxScore { get; set; }
        public DateTime DueAt  { get; set; }
    }
}
