namespace EduFlow.Services.Interfaces
{
    public interface ICourseService
    {
        Task CreateCourseAsync(string description, string title, int userId);
    }
}
