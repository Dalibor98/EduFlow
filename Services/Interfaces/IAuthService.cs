using EduFlow.DTOs.Auth;
using EduFlow.Models;

namespace EduFlow.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync (string email, string password);
        Task<User> RegisterAsync(string fullName, string email, string password);
    }
}
