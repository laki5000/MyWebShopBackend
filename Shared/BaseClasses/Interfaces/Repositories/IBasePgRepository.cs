﻿using System.Linq.Expressions;

namespace Shared.BaseClasses.Interfaces.Repositories
{
    public interface IBasePgRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<T?> FindByIdAsync(string id);
        Task<List<T>> GetAllWithExpressionAsync(Expression<Func<T, bool>> expression);
        Task<T> UpdateAsync(T entity);
    }
}
