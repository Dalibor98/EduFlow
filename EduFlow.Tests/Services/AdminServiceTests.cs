using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace EduFlow.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<AdminService>> _loggerMock;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AdminService>>();
            _adminService = new AdminService(_userRepositoryMock.Object,_loggerMock.Object);
        }

        [Fact]
        public async Task RegisterProfessorAsync_EmailAlreadyExists_ThrowsArgumentException()
        {
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new Models.User() { Email = "test@example.com" });

            await Assert.ThrowsAsync<ArgumentException>(
                () => _adminService.RegisterProfessorAsync("John Doe", "test@example.com", "123")
            );
        }

        [Fact]
        public async Task RegisterProfessorAsync_ValidInput_CreatesProfessor()
        {
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Models.User?)null);

            await _adminService.RegisterProfessorAsync("John Doe", "test@test.com", "123");

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Models.User>()), Times.Once);
            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
