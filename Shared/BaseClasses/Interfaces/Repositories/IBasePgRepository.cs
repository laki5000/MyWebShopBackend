namespace Shared.BaseClasses.Interfaces.Repositories
{
    public interface IBasePgRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
    }
}
