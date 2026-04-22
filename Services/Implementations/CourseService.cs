using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task CreateCourseAsync(string description, string title, int userId)
        {
            var course = new Course
            {
                Description = description,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                ProfessorId = userId,
            };

            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();
        }
    }
}
