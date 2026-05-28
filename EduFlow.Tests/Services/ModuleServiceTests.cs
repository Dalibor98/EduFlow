using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace EduFlow.Tests.Services
{
    public class ModuleServiceTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IModuleRepository> _moduleRepositoryMock;
        private readonly Mock<IEnrollmentRepository> _enrollmentRepositoryMock;
        private readonly Mock<ILogger<ModuleService>> _loggerMock;
        private readonly ModuleService _moduleService;

        public ModuleServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _moduleRepositoryMock = new Mock<IModuleRepository>();
            _enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();
            _loggerMock= new Mock<ILogger<ModuleService>>();
            _moduleService = new ModuleService(
                _courseRepositoryMock.Object,
                _moduleRepositoryMock.Object,
                _enrollmentRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateModuleAsync_CourseNotFoundOrNotOwned_ThrowsArgumentException()
        {
            _courseRepositoryMock
                .Setup(r => r.GetByIdAndProfessorAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Models.Course?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _moduleService.CreateModuleAsync(1, "Title", "Description", 1)
            );
        }

        [Fact]
        public async Task CreateModuleAsync_ValidInput_CreatesModule()
        {
            _courseRepositoryMock
                .Setup(r => r.GetByIdAndProfessorAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Models.Course());

            await _moduleService.CreateModuleAsync(1, "Title", "Description", 1);

            _moduleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Models.Module>()), Times.Once);
            _moduleRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetMyModulesAsync_CourseDoesntExist_ThrowsArgumentException()
        {
            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Models.Course?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _moduleService.GetMyModulesAsync(1, 1)
            );
        }

        [Fact]
        public async Task GetMyModulesAsync_NotEnrolled_ThrowsUnauthorizedAccessException()
        {
            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Models.Course());

            _enrollmentRepositoryMock
                .Setup(r => r.GetByUserAndCourseAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Models.Enrollment?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _moduleService.GetMyModulesAsync(1, 1)
            );
        }

        [Fact]
        public async Task GetMyModulesAsync_ValidRequest_ReturnsModules()
        {
            var modules = new List<Models.Module>
            {
                new Models.Module { Id = 1, Title = "Module 1", CourseId = 1 },
                new Models.Module { Id = 2, Title = "Module 2", CourseId = 1 }
            };

            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Models.Course());

            _enrollmentRepositoryMock
                .Setup(r => r.GetByUserAndCourseAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Models.Enrollment());

            _moduleRepositoryMock
                .Setup(r => r.GetModulesByCourseIdAsync(It.IsAny<int>()))
                .ReturnsAsync(modules);

            var result = await _moduleService.GetMyModulesAsync(1, 1);

            Assert.Equal(modules, result);
        }
    }
}