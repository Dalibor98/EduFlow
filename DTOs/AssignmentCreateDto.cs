namespace EduFlow.DTOs
{
    public class AssignmentCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxScore { get; set; }
        public DateTime DueAt {  get; set; } 
    }
}
