using EduFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace EduFlow.Data
{
    public class AppDbContext : DbContext
    {   
        public DbSet<User> Users { get; set;}
        public DbSet<Course> Courses { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Module> Modules {  get; set; }
        public DbSet<Assignment> Assignments {  get; set; }
        public DbSet <Enrollment> Enrollments {  get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc),
                Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FullName = "Dalibor Naspalic",
                Role = "Admin"

            });
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany()
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
