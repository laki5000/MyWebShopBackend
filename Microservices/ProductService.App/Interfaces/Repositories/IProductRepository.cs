using ProductService.App.Models;
using Shared.BaseClasses.Interfaces.Repositories;

namespace ProductService.App.Interfaces.Repositories
{
    public interface IProductRepository : IBasePgRepository<Product>
    {
    }
}
