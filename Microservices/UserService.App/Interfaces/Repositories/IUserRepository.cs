using Shared.BaseClasses.Interfaces.Repositories;
using UserService.App.Models;

namespace UserService.App.Interfaces.Repositories
{
    public interface IUserRepository : IBasePgRepository<User>
    {
    }
}
