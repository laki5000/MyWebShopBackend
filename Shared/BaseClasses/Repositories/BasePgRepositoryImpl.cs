using Microsoft.EntityFrameworkCore;
using Shared.BaseClasses.Interfaces.Repositories;

namespace Shared.BaseClasses.Repositories
{
    public class BasePgRepositoryImpl<T> : IBasePgRepository<T> where T : class
    {
        protected readonly DbContext _context;

        public BasePgRepositoryImpl(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
