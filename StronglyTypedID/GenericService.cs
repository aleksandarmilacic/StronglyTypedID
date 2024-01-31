
using Microsoft.EntityFrameworkCore;

public class GenericService<TEntity, TId> where TEntity : class, IId<TId>, new()
{
    private readonly GenericRepository<TEntity, TId> _repository;

    public GenericService(MyDbContext context)
    {
        _repository = new GenericRepository<TEntity, TId>(context);
    }

    public async Task<TEntity> GetByIdAsync(TId id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task CreateAsync(TEntity entity)
    {
        await _repository.AddAsync(entity);
        // Additional business logic can be added here
    }

    public async Task UpdateAsync(TEntity entity)
    {
        // Business logic before update
        await _repository.UpdateAsync(entity);
        // Business logic after update
    }

    public async Task DeleteAsync(TId id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            await _repository.DeleteAsync(entity);
            // Additional business logic can be added here
        }
    }

    // Add more service methods as needed...
}
