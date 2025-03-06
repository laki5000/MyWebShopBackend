using Shared.Dtos;

namespace AuthService.App.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ApiResponseDto<List<string>>> GetAllAsync();
    }
}
