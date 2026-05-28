using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ICourseRepository courseRepository, ILogger<CourseService> logger)
        {
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task CreateCourseAsync(string description, string title, int userId)
        {
           if(await _courseRepository.TitleExistsForProfessorAsync(title, userId))
            {
                throw new ArgumentException("Course with the same title already exists.");
            }

            var course = new Course
            {
                Description = description,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                ProfessorId = userId,
            };

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();
            _logger.LogInformation("Course created. ProfessorId: {ProfessorId}, CourseId: {CourseId}", userId, course.Id);
        }
    }
}
