namespace WebApiEntityFramework.Contracts.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetById(string id);
        Task CreateAsync(TEntity entity);
        Task CreateAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(TEntity entityToUpdate);
        Task DeleteAsync(string id);
        Task DeleteAsync(TEntity entityToDelete);

    }
}
