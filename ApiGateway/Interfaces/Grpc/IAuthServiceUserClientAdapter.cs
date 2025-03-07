using AuthService.Shared.Dtos;
using Shared.Dtos;

namespace ApiGateway.Interfaces.Grpc
{
    public interface IAuthServiceUserClientAdapter
    {
        Task<ApiResponseDto<string>> CreateAsync(CreateAspNetUserDto request);
        Task<ApiResponseDto<string>> LoginAsync(LoginAspNetUserDto request);
        Task<ApiResponseDto> ChangePasswordAsync(ChangeAspNetUserPasswordDto request);
    }
}
