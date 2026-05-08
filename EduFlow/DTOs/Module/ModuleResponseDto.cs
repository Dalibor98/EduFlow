namespace EduFlow.DTOs.Module
{
    public class ModuleResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
