using Shared.Dtos;
using UserService.Shared.Dtos;

namespace UserService.App.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponseDto> CreateAsync(CreateUserDto createUserDto);
    }
}
