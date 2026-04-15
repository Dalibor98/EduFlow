using EduFlow.Data;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Enrollment>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Enrollments.Where(e => e.UserId == userId).ToListAsync();
        }
        public async Task<bool> ExistsAsync(int userId, int courseId)
        {
            return await _context.Enrollments.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }
    }
}
