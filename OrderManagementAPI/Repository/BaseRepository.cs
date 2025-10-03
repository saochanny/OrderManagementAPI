using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Config;

namespace OrderManagementAPI.Repository;

public class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T>
    where T : class
{
    private readonly DbSet<T> _dbSet = context.Set<T>();  // We use DbSet for dynamic Type

    public async Task<T> SaveAsync(T entity)
    {
        if (context.Entry(entity).State == EntityState.Detached)
        {
            await _dbSet.AddAsync(entity);
        }
        else
        {
            _dbSet.Update(entity);
        }
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> FindByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(object id)
    {
        var entity = await FindByIdAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with id {id} not found.");
        }

        _dbSet.Remove(entity);
        await context.SaveChangesAsync();
    }
}