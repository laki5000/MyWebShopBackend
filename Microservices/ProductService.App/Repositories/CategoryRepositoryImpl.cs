using Microsoft.EntityFrameworkCore;
using ProductService.App.Data;
using ProductService.App.Interfaces.Repositories;
using ProductService.App.Models;
using Shared.BaseClasses.Repositories;

namespace ProductService.App.Repositories
{
    public class CategoryRepositoryImpl : BasePgRepositoryImpl<Category>, ICategoryRepository
    {
        protected new readonly ProductDbContext _context;

        public CategoryRepositoryImpl(ProductDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name);
        }
    }
}
