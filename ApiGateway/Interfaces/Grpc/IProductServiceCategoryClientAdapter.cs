using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ApiGateway.Interfaces.Grpc
{
    public interface IProductServiceCategoryClientAdapter
    {
        Task<ApiResponseDto> CreateAsync(CreateCategoryDto request);
    }
}
