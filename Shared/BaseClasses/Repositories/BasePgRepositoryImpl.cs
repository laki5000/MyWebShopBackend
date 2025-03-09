﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
