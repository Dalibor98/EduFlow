using EduFlow.Data;
using EduFlow.Repositories.Interfaces;

namespace EduFlow.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        
        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
