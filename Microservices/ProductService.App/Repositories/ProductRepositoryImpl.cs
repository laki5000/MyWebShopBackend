using Microsoft.EntityFrameworkCore;
using ProductService.App.Data;
using ProductService.App.Interfaces.Repositories;
using ProductService.App.Models;
using Shared.BaseClasses.Repositories;

namespace ProductService.App.Repositories
{
    public class ProductRepositoryImpl : BasePgRepositoryImpl<Product>, IProductRepository
    {
        protected new readonly ProductDbContext _context;

        public ProductRepositoryImpl(ProductDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByTitleAsync(string title)
        {
            return await _context.Products.AnyAsync(p => p.Title == title);
        }

        public async Task<bool> ExistsByTitleAsync(string title, string id)
        {
            return await _context.Products.AnyAsync(p => p.Title == title && p.Id != id);
        }
    }
}
