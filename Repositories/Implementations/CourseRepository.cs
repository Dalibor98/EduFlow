using EduFlow.Data;
using EduFlow.Models;
using EduFlow.Repositories.Interfaces;

namespace EduFlow.Repositories.Implementations
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }
    }
}
