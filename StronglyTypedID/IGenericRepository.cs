public interface IGenericRepository<TEntity, TId> where TEntity : class, IId<TId>
{
    Task<TEntity> GetByIdAsync(TId id);
}
