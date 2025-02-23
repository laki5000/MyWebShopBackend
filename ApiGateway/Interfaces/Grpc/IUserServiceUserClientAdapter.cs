using Shared.Dtos;
using UserService.Shared.Dtos;

namespace ApiGateway.Interfaces.Grpc
{
    public interface IUserServiceUserClientAdapter
    {
        Task<ApiResponseDto> CreateAsync(CreateUserDto request);
    }
}
