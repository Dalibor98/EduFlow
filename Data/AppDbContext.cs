using EduFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {
        }
        public DbSet<User> Users { get; set;}
        public DbSet<Course> Courses { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Module> Modules {  get; set; }
        public DbSet<Assignment> Assignments {  get; set; }
        public DbSet <Enrollment> Enrollments {  get; set; }
    }
}
