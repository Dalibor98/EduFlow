namespace EduFlow.Services.Interfaces
{
    public interface IAdminService
    {
        Task RegisterProfessorAsync(string fullName, string email, string password);
    }
}
