using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using EduFlow.Services.Interfaces;

namespace EduFlow.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {

        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
        {
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
        }
        public async Task EnrollAsync(int userId, int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);

            if (course == null)
            {
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
                throw new KeyNotFoundException("Course doesn't exist");
            }

            var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(userId, courseId);

            if (enrollment == null)
            {
                throw new  ArgumentException("Student is not yet enrolled in this course");
            }

            await _enrollmentRepository.DeleteAsync(enrollment);
            await _enrollmentRepository.SaveChangesAsync();
        }

    }
}
