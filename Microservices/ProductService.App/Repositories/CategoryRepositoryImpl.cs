using Microsoft.EntityFrameworkCore;
using ProductService.App.Data;
using ProductService.App.Interfaces.Repositories;
using ProductService.App.Models;
using Shared.BaseClasses.Repositories;
using Shared.Enums;
using System.Linq.Expressions;

namespace ProductService.App.Repositories
{
    public class CategoryRepositoryImpl : BasePgRepositoryImpl<Category>, ICategoryRepository
    {
        protected new readonly ProductDbContext _context;

        public CategoryRepositoryImpl(ProductDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name, string id)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name && c.Id != id);
        }

        public async Task<List<Category>> GetAllNotDeletedAsync()
        {
            Expression<Func<Category, bool>> expression = c => c.Status != ObjectStatus.DELETED;
            var result = await GetAllWithExpressionAsync(expression);

            return result;
        }
    }
}
