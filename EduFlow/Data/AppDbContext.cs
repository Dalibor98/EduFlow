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
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany()
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);

            var seedDate = new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    CreatedAt = seedDate,
                    Email = "admin@test.com",
                    PasswordHash = "$2a$11$fEPkIRkJbuIJNswVMnqL/OZIEhXbPJE0qFt7ScYa1vIjDAko/n.kK",
                    FullName = "Dalibor Naspalic",
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    CreatedAt = seedDate,
                    Email = "professor@test.com",
                    PasswordHash = "$2a$11$45oGuVIWkiHCMrO681plx.0gmABwOFPec1mhU/2Gjtidy6z37Wqaa",
                    FullName = "Sample Professor",
                    Role = "Professor"
                },
                new User
                {
                    Id = 3,
                    CreatedAt = seedDate,
                    Email = "student@test.com",
                    PasswordHash = "$2a$11$r6mINeiM1wLrDd8rDuVPEudiV32my8lYrqnTZkNukaUisVMVo.5du",
                    FullName = "Sample Student",
                    Role = "Student"
                }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Title = "Introduction to ASP.NET Core",
                    Description = "A beginner friendly course covering the fundamentals of building REST APIs with ASP.NET Core 8.",
                    CreatedAt = seedDate,
                    ProfessorId = 2
                }
            );

            modelBuilder.Entity<Module>().HasData(
                new Module
                {
                    Id = 1,
                    Title = "Getting Started with Controllers",
                    Description = "Learn how controllers, routing, and model binding work together in ASP.NET Core.",
                    CourseId = 1,
                    CreatedAt = seedDate
                }
            );

            modelBuilder.Entity<Quiz>().HasData(
                new Quiz
                {
                    Id = 1,
                    Title = "Controllers Basics Quiz",
                    Description = "A short quiz to test your understanding of controller fundamentals.",
                    ModuleId = 1,
                    CreatedAt = seedDate
                }
            );

            modelBuilder.Entity<Assignment>().HasData(
                new Assignment
                {
                    Id = 1,
                    Title = "Build Your First Controller",
                    Description = "Create a simple controller with two GET endpoints and submit your code.",
                    ModuleId = 1,
                    MaxScore = 100,
                    DueAt = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment
                {
                    Id = 1,
                    UserId = 3,
                    CourseId = 1,
                    EnrolledAt = seedDate
                }
            );
        }
    }
}
