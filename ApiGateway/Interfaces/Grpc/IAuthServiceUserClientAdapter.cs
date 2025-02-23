using AuthService.Shared.Dtos;
using Shared.Dtos;

namespace ApiGateway.Interfaces.Grpc
{
    public interface IAuthServiceUserClientAdapter
    {
        Task<ApiResponseDto<AspNetUserDto>> CreateAsync(CreateAspNetUserDto request);
        Task<ApiResponseDto<string>> LoginAsync(LoginAspNetUserDto request);
    }
}
