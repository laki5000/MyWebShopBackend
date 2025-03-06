using AuthService.Shared.Dtos;
using Shared.Dtos;

namespace AuthService.Interfaces.Services
{
    public interface IAspNetUserService
    {
        Task<ApiResponseDto<string>> CreateAsync(CreateAspNetUserDto createAspNetUserDto);
        Task<ApiResponseDto<string>> LoginAsync(LoginAspNetUserDto loginAspNetUserDto);
        Task<ApiResponseDto> DeleteAsync(string userId, bool forceDelete);
        Task<ApiResponseDto> CompleteCreationAsync(string userId);
    }
}
