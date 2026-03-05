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
    }
}
