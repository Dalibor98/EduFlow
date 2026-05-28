using Castle.Core.Logging;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace EduFlow.Tests.Services
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly CourseService _courseService;
        private readonly Mock<ILogger<CourseService>> _loggerMock;

        public CourseServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _loggerMock = new Mock<ILogger<CourseService>>();
            _courseService = new CourseService(_courseRepositoryMock.Object,_loggerMock.Object);
        }


        [Fact]
        public async Task CreateCourseAsync_TitleExists_ThrowsArgumentException()
        {
            _courseRepositoryMock
                .Setup(r => r.TitleExistsForProfessorAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _courseService.CreateCourseAsync("Description", "Existing Title", 1)
            );
        }

        [Fact]

        public async Task CreateCourseAsync_ValidCreation_Succeeds()
        {
            _courseRepositoryMock
                .Setup(r => r.TitleExistsForProfessorAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            await _courseService.CreateCourseAsync("Description", "New Title", 1);

            _courseRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EduFlow.Models.Course>()), Times.Once);
            _courseRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        }
    }
}
