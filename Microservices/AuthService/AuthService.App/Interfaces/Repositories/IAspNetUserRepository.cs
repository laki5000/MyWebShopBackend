using AuthService.Models;
using Shared.BaseClasses.Interfaces.Repositories;

namespace AuthService.Interfaces.Repositories
{
    public interface IAspNetUserRepository : IBasePgRepository<AspNetUser>
    {
        Task<bool> ExistsByNormalizedUserNameAsync(string normalizedUserName);
        Task<AspNetUser?> GetByNormalizedUserNameAsync(string normalizedUserName);
    }
}
