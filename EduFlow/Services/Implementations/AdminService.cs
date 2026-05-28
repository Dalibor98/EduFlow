using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IUserRepository userRepository, ILogger<AdminService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
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
            _logger.LogInformation("Professor {ProfessorId} registered", professor.Id);
        }
    }
}
