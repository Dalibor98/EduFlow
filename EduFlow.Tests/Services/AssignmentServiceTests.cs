using EduFlow.Repositories.Implementations;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Implementations;
using Moq;

namespace EduFlow.Tests.Services
{
    public class AssignmentServiceTests
    {

        private readonly Mock<IAssignmentRepository> _assignmentRepositoryMock;
        private readonly Mock<IModuleRepository> _moduleRepositoryMock;
        private readonly Mock<IAssignmentSubmissionRepository> _assignmentSubmissionRepositoryMock;
        private readonly AssignmentService _assignmentService;

        public AssignmentServiceTests()
        {
            _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
            _moduleRepositoryMock = new Mock<IModuleRepository>();
            _assignmentSubmissionRepositoryMock = new Mock<IAssignmentSubmissionRepository>();
            _assignmentService = new AssignmentService(
                _moduleRepositoryMock.Object,
                _assignmentRepositoryMock.Object,
                _assignmentSubmissionRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateAssignment_ModuleDoesntExist_ThrowsKeyNotFoundException()
        {
            _moduleRepositoryMock
                .Setup(r => r.GetByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Models.Module?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _assignmentService.CreateAssignmentAsync(1, 1, "Test Assignment", "Description", 100, DateTime.UtcNow.AddDays(7))
            );
        }

        [Fact]
        public async Task CreateAssignment_TitleAlreadyExists_ThrowsArgumentException()
        {
            _moduleRepositoryMock
                .Setup(r => r.GetByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Models.Module());

            _assignmentRepositoryMock
                .Setup(r => r.TitleExistsInModuleAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _assignmentService.CreateAssignmentAsync(1, 1, "Test Assignment", "Description", 100, DateTime.UtcNow.AddDays(7))
            );
        }

        [Fact]
        public async Task CreateAssignment_ValidInput_CreatesAssignment()
        {
            _moduleRepositoryMock
                .Setup(r => r.GetByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Models.Module());

            await _assignmentService.CreateAssignmentAsync(1, 1, "Test Assignment", "Description", 100, DateTime.UtcNow.AddDays(7));
            _assignmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Models.Assignment>()), Times.Once);
            _assignmentRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SubmitAssignmentAsync_AssignmentDoesntExist_ThrowsUnauthorizedAccessException()
        {
            _assignmentRepositoryMock
                .Setup(r => r.GetByIdWithEnrollmentCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Models.Assignment?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _assignmentService.SubmitAssignmentAsync(1, 1, "My answer")
            );
        }

        [Fact]
        public async Task SubmitAssignmentAsync_AssignmentSubmissionAlreadyExists_ThrowsArgumentException()
        {
            _assignmentRepositoryMock
                .Setup(r => r.GetByIdWithEnrollmentCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Models.Assignment());

            _assignmentSubmissionRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _assignmentService.SubmitAssignmentAsync(1, 1, "My answer")
            );
        }


        [Fact]
        public async Task SubmitAssignmentAsync_ValidInput_SubmitsAssignment()
        {
            _assignmentRepositoryMock
                .Setup(r => r.GetByIdWithEnrollmentCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Models.Assignment());

            await _assignmentService.SubmitAssignmentAsync(1, 1, "My answer");

            _assignmentSubmissionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Models.AssignmentSubmission>()), Times.Once);
            _assignmentSubmissionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GradeSubmissionAsync_SubmissionDoesntExist_ThrowsKeyNotFoundException()
        {
            _assignmentSubmissionRepositoryMock
                .Setup(r => r.GetSubmissionByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Models.AssignmentSubmission?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _assignmentService.GradeSubmissionAsync(1, 1, 1));
        }

        [Fact]
        public async Task GradeSubmissionAsync_ScoreOutOfRange_ThrowsArgumentException()
        {
            var assignment = new Models.Assignment { MaxScore = 100 };
            var submission = new Models.AssignmentSubmission { Assignment = assignment };

            _assignmentSubmissionRepositoryMock
                .Setup(r => r.GetSubmissionByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(submission);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _assignmentService.GradeSubmissionAsync(1, 150, 1));
        }


        [Fact]
        public async Task GradeSubmissionAsync_ValidData_Succeeds()
        {
            var assignment = new Models.Assignment { MaxScore = 100 };
            var submission = new Models.AssignmentSubmission { Assignment = assignment };

            _assignmentSubmissionRepositoryMock
                .Setup(r => r.GetSubmissionByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(submission);

            var result = await _assignmentService.GradeSubmissionAsync(1, 85, 1);

            _assignmentSubmissionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.Equal("Assignment graded successfully.", result);
        }


        [Fact]

        public async Task GradeSubmissionAsync_OverrideGrade_Succeeds()
        {
            var assignment = new Models.Assignment { MaxScore = 100 };

            var submission = new Models.AssignmentSubmission { Assignment = assignment, Score = 80 };

            _assignmentSubmissionRepositoryMock
                .Setup(r => r.GetSubmissionByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(submission);

            var result = await _assignmentService.GradeSubmissionAsync(1, 90, 1);

            _assignmentSubmissionRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.Equal("Assignment grade overridden successfully.", result);
        }
    }
}

