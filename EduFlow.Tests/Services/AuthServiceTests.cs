using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace EduFlow.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;
        private readonly Mock<ILogger<AuthService>> _loggerMock;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object,_loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_NewEmail_Succeeds()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act
            await _authService.RegisterAsync("John Doe", "john@test.com", "password123");

            // Assert
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ExistingEmail_ThrowsArgumentException()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _authService.RegisterAsync("John Doe", "john@test.com", "password123")
            );
        }

        [Fact]
        public async Task LoginAsync_NonExistingEmail_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync("", "password123")
            );
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User { PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword") });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync("", "wrongpassword")
            );
        }

        [Fact]
        public async Task LoginAsync_GoodCredentials_ReturnsToken()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                    Email = "test@test.com",
                    Role = "Student"
                });

            _configurationMock
                .Setup(c => c["JwtSettings:Secret"]).Returns("atLeastAThirtyThreeCharactersLongSecretKey");

            _configurationMock
                .Setup(c => c["JwtSettings:Issuer"]).Returns("eduflow-portal");

            _configurationMock
                .Setup(c => c["JwtSettings:Audience"]).Returns("eduflow-client");

            // Act
            var token = await _authService.LoginAsync("", "correctpassword");

            // Assert
            Assert.NotEmpty(token);
        }
    }
}