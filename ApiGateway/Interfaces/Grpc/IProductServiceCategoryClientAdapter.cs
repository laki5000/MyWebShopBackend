using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ApiGateway.Interfaces.Grpc
{
    public interface IProductServiceCategoryClientAdapter
    {
        Task<ApiResponseDto> CreateAsync(CreateCategoryDto request);
        Task<ApiResponseDto<List<GetCategoryDto>>> GetAllAsync();
        Task<ApiResponseDto> UpdateAsync(UpdateCategoryDto request);
        Task<ApiResponseDto> DeleteAsync(DeleteCategoryDto request);
    }
}
