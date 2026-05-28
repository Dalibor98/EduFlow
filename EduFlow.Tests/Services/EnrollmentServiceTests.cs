using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Implementations;
using Moq;
using EduFlow.Models;
using Microsoft.Extensions.Logging;


namespace EduFlow.Tests.Services
{
    public class EnrollmentServiceTests
    {

        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IEnrollmentRepository> _enrollmentRepositoryMock;
        private readonly EnrollmentService _enrollmentService;
        private readonly Mock<ILogger<EnrollmentService>> _loggerMock;

        public EnrollmentServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();
            _loggerMock = new Mock<ILogger<EnrollmentService>>();
            _enrollmentService = new(_courseRepositoryMock.Object, _enrollmentRepositoryMock.Object,_loggerMock.Object);
        }


        [Fact]
        public async Task EnrollAsync_CourseDoesntExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Course?)null);
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _enrollmentService.EnrollAsync(1, 1)
            );
        }

        [Fact]
        public async Task EnrollAsync_StudentAlreadyEnrolled_ThrowsArgumentException()
        {
            _enrollmentRepositoryMock
                .Setup(r => r.IsUserEnrolledAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Course());

            await Assert.ThrowsAsync<ArgumentException>(
                () => _enrollmentService.EnrollAsync(1, 1)
            );
        }

        [Fact]
        public async Task EnrollAsync_ValidEnrollment_Succeeds()
        {
            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Course());
            _enrollmentRepositoryMock
                .Setup(r => r.IsUserEnrolledAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            await _enrollmentService.EnrollAsync(1, 1);
            _enrollmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Enrollment>()), Times.Once);
            _enrollmentRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UnenrollAsync_CourseDoesntExist_ThrowsKeyNotFoundException()
        {
            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Course?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _enrollmentService.UnenrollAsync(1, 1)
                );
        }

        [Fact]
        public async Task UnenrollAsync_StudentNotEnrolled_ThrowsArgumentException()
        {
            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Course());

            _enrollmentRepositoryMock
                .Setup(r => r.GetByUserAndCourseAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Enrollment?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _enrollmentService.UnenrollAsync(1, 1)
                );

        }


        [Fact]
        public async Task UnenrollAsync_ValidEnrollment_Succeeds()
        {
            _courseRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Course());

            _enrollmentRepositoryMock
                .Setup(r => r.GetByUserAndCourseAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Enrollment());

            await _enrollmentService.UnenrollAsync(1, 1);
            _enrollmentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Enrollment>()), Times.Once);
            _enrollmentRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]

        public async Task GetMyEnrollmentsAsync_ReturnsEnrollments()
        {
            var enrollments = new List<Enrollment>
            {
                new Enrollment { Id = 1, UserId = 1, CourseId = 1 },
                new Enrollment { Id = 2, UserId = 1, CourseId = 2 }
            };
            _enrollmentRepositoryMock
            .Setup(r => r.GetAllByUserIdAsync(It.IsAny<int>()))
            .ReturnsAsync(enrollments);
            

            var result = await _enrollmentService.GetMyEnrollmentsAsync(1);

            Assert.Equal(enrollments, result);
        }
    }
}


