using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ProductService.App.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<ApiResponseDto> CreateAsync(CreateCategoryDto createCategoryDto);
        Task<ApiResponseDto> UpdateAsync(UpdateCategoryDto updateCategoryDto);
        Task<ApiResponseDto> DeleteAsync(DeleteCategoryDto deleteCategoryDto);
    }
}
