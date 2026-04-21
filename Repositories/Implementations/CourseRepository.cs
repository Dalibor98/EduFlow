using EduFlow.Data;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Repositories.Implementations
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Course?> GetByIdAndProfessorAsync(int courseId, int professorId)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.ProfessorId == professorId);
        }
    }
}
