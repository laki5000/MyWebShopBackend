using Microsoft.EntityFrameworkCore;
using Shared.BaseClasses.Interfaces.Repositories;
using Shared.Enums;

namespace Shared.BaseClasses.Repositories
{
    public class BasePgRepositoryImpl<T> : IBasePgRepository<T> where T : class
    {
        protected readonly DbContext _context;

        public BasePgRepositoryImpl(DbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<T?> FindByIdAsync(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                return null;
            }

            foreach (var property in entry.OriginalValues.Properties)
            {
                var currentValue = entry.CurrentValues[property];
                var originalValue = entry.OriginalValues[property];

                if (currentValue == null || Equals(currentValue, originalValue))
                {
                    entry.Property(property.Name).IsModified = false;
                }
                else
                {
                    entry.Property(property.Name).IsModified = true;
                }
            }

            var isModified = entry.OriginalValues.Properties.Any(p => entry.Property(p.Name).IsModified);
            if (isModified)
            {
                entity.GetType().GetProperty("UpdatedAt")?.SetValue(entity, DateTime.Now);
                await _context.SaveChangesAsync();
            }

            return entity;
        }

        public async Task<T> SoftDeleteAsync(T entity, string deletedBy)
        {
            entity.GetType().GetProperty("Status")?.SetValue(entity, ObjectStatus.DELETED);
            entity.GetType().GetProperty("DeletedBy")?.SetValue(entity, deletedBy);
            entity.GetType().GetProperty("DeletedAt")?.SetValue(entity, DateTime.Now);

            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
