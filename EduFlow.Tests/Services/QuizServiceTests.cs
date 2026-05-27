using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Implementations;
using EduFlow.Models;
using Moq;

namespace EduFlow.Tests.Services
{
    public class QuizServiceTests
    {
        private readonly Mock<IModuleRepository> _moduleRepositoryMock;
        private readonly Mock<IQuizRepository> _quizRepositoryMock;
        private readonly QuizService _quizService;


        public QuizServiceTests()
        {
            _moduleRepositoryMock = new Mock<IModuleRepository>();
            _quizRepositoryMock = new Mock<IQuizRepository>();
            _quizService = new(_quizRepositoryMock.Object, _moduleRepositoryMock.Object);
        }


        [Fact]
        public async Task CreateQuizAsync_ModuleNotFound_ThrowsKeyNotFoundException()
        {
            _moduleRepositoryMock
                .Setup(r => r.GetByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Module?)null);
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _quizService.CreateQuizAsync(1, "Quiz Title", "Quiz Description", 1)
            );
        }

        [Fact]
        public async Task CreateQuizAsync_TitleExists_ThrowsArgumentException()
        {
            _moduleRepositoryMock
                .Setup(r => r.GetByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Module());
            _quizRepositoryMock
                .Setup(r => r.TitleExistsInModuleAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(true);
            await Assert.ThrowsAsync<ArgumentException>(
                () => _quizService.CreateQuizAsync(1, "Existing Quiz Title", "Quiz Description", 1)
            );
        }


        [Fact]
        public async Task CreateQuizAsync_ValidCreation_Succeeds()
        {
            _moduleRepositoryMock
                .Setup(r => r.GetByIdWithOwnershipCheckAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Module());

            //We dont need to setup TitleExistsInModuleAsync to return false, because the default value would return false, which is what we want for this test case.

            await _quizService.CreateQuizAsync(1, "New Quiz Title", "Quiz Description", 1);
            _quizRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Quiz>()), Times.Once);
            _quizRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
    