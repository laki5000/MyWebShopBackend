using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ApiGateway.Interfaces.Grpc
{
    public interface IProductServiceProductClientAdapter
    {
        Task<ApiResponseDto> CreateAsync(CreateProductDto request);
        Task<ApiResponseDto> UpdateAsync(UpdateProductDto request);
        Task<ApiResponseDto> DeleteAsync(DeleteProductDto request);
    }
}
