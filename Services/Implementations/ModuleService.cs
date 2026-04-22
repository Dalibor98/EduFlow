using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class ModuleService : IModuleService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public ModuleService(ICourseRepository courseRepository, IModuleRepository moduleRepository,IEnrollmentRepository enrollmentRepository)
        { 
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _enrollmentRepository = enrollmentRepository;
        }
        public async Task CreateModuleAsync(int courseId, string title, string description, int userId)
        {
            var course = await _courseRepository.GetByIdAndProfessorAsync(courseId, userId);

            if (course == null)
            {
                throw new ArgumentException("Course not found or acess denied.");
            }

            var module = new Module
            {
                Title = title,
                Description = description,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow,
            };

            await _moduleRepository.AddAsync(module);
            await _moduleRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Module>> GetMyModulesAsync(int courseId,int userId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);

            if (course == null)
            {
                throw new ArgumentException("Course not found.");
            }

            var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(userId, courseId);
            if (enrollment == null)
            {
                throw new UnauthorizedAccessException("Permission denied.");
            }

            var modules = await _moduleRepository.GetModulesByCourseIdAsync(courseId);

            return modules;
        }
    }
}
