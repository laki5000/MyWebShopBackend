namespace Shared.BaseClasses.Interfaces.Repositories
{
    public interface IBasePgRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T?> FindByIdAsync(string id);
        Task<T?> UpdateAsync(T entity);
        Task<T> SoftDeleteAsync(T entity, string deletedBy);
    }
}
