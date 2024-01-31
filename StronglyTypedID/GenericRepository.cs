
using Microsoft.EntityFrameworkCore;

public class GenericRepository<TEntity, TId> where TEntity : class, IId<TId>, new()
{
    private readonly DbSet<TEntity> _dbSet;
    private readonly MyDbContext _context;

    public GenericRepository(MyDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(TId id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            var softDeletableEntity = entity as SoftDeletableEntity;
            if (softDeletableEntity != null)
            {
                softDeletableEntity.SoftDelete();
                await _context.SaveChangesAsync();
            }
        }
    }

}

