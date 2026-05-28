using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {

        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository, ILogger<EnrollmentService> logger)
        {
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _logger = logger;
        }

        public async Task EnrollAsync(int userId, int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);

            if (course == null)
            {
                _logger.LogWarning("Enroll failed: course {CourseId} not found", courseId);
                throw new KeyNotFoundException("Course doesn't exist");
            }

            if (await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId))
            {
                throw new ArgumentException("Student is already enrolled in this course");
            }

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow
            };

            await _enrollmentRepository.AddAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
            _logger.LogInformation("User {UserId} enrolled in course {CourseId}", userId, courseId);

        }

        public async Task<IEnumerable<Enrollment>> GetMyEnrollmentsAsync(int userId)
        {
            return await _enrollmentRepository.GetAllByUserIdAsync(userId);
        }

        public async Task UnenrollAsync(int userId, int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Unenroll failed: course {CourseId} not found", courseId);
                throw new KeyNotFoundException("Course doesn't exist");
            }

            var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(userId, courseId);

            if (enrollment == null)
            {
                throw new  ArgumentException("Student is not yet enrolled in this course");
            }

            await _enrollmentRepository.DeleteAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
            _logger.LogInformation("User {UserId} unenrolled from course {CourseId}", userId, courseId);
        }

    }
}
