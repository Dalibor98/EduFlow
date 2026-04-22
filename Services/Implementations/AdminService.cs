using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;

        public AdminService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task RegisterProfessorAsync(string fullName, string email, string password)
        {
            if (await _userRepository.GetByEmailAsync(email) != null)
            {
                throw new ArgumentException("Professor with this email exists");
            }
            var professor = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                FullName = fullName,
                Role = "Professor"
            };

            await _userRepository.AddAsync(professor);

            await _userRepository.SaveChangesAsync();
        }
    }
}
