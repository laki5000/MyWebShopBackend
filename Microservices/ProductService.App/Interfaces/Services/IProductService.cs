using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ProductService.App.Interfaces.Services
{
    public interface IProductService
    {
        Task<ApiResponseDto> CreateAsync(CreateProductDto createProductDto);
        Task<ApiResponseDto> UpdateAsync(UpdateProductDto updateProductDto);
        Task<ApiResponseDto> DeleteAsync(DeleteProductDto deleteProductDto);
    }
}
