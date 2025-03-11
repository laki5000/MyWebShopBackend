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
    }
}
