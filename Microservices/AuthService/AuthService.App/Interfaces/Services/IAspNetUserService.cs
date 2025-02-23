using AuthService.Shared.Dtos;
using Shared.Dtos;

namespace AuthService.Interfaces.Services
{
    public interface IAspNetUserService
    {
        Task<ApiResponseDto<AspNetUserDto>> CreateAsync(CreateAspNetUserDto createAspNetUserDto);
        Task<ApiResponseDto<string>> LoginAsync(LoginAspNetUserDto loginAspNetUserDto);
    }
}
