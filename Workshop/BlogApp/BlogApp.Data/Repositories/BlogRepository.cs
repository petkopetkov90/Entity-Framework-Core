using BlogApp.Data.Data;
using BlogApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Repositories
{
    public class BlogRepository<T>(ApplicationDbContext context) : IBlogRepository<T>
    where T : class
    {

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await context.Set<T>().AsNoTracking().ToListAsync();

            return entities;
        }

        public async Task<T> FindByIdAsync(int id)
        {
            var entity = await context.Set<T>().FindAsync(id);

            return entity;
        }

        public async Task AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await context.Set<T>().FindAsync(id);

            if (entity is not null)
            {
                context.Set<T>().Remove(entity);
            }
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
