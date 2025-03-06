using Shared.Dtos;

namespace ApiGateway.Interfaces.Grpc
{
    public interface IAuthServiceRoleClientAdapter
    {
        Task<ApiResponseDto<List<string>>> GetAllAsync();
    }
}
