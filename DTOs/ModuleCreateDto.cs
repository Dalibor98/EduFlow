namespace EduFlow.DTOs
{
    public class ModuleCreateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CourseId { get; set; }
    }
}
          