using ProductService.App.Models;
using Shared.BaseClasses.Interfaces.Repositories;

namespace ProductService.App.Interfaces.Repositories
{
    public interface ICategoryRepository : IBasePgRepository<Category>
    {
        Task<bool> ExistsByIdAsync(string id);
        Task<bool> ExistsByNameAsync(string name);
        Task<List<Category>> GetAllNotDeletedAsync();
    }
}
