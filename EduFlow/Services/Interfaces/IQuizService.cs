namespace EduFlow.Services.Interfaces
{
    public interface IQuizService
    {
        Task CreateQuizAsync(int moduleId, string title, string description, int userId);
    }
}
