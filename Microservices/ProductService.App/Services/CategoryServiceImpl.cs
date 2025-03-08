using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ProductService.App.Interfaces.Repositories;
using ProductService.App.Interfaces.Services;
using ProductService.App.Models;
using ProductService.Shared.Dtos;
using Shared.Dtos;
using Shared.Enums;

namespace ProductService.App.Services
{
    public class CategoryServiceImpl : ICategoryService
    {
        private readonly ILogger<CategoryServiceImpl> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryServiceImpl(ILogger<CategoryServiceImpl> logger, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto> CreateAsync(CreateCategoryDto createCategoryDto)
        {
            var existsByName = await _categoryRepository.ExistsByNameAsync(createCategoryDto.Name);
            if (existsByName)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.NAME_ALREADY_EXISTS);
                _logger.LogError("Category creation failed: Name already exists");
                return errorResult;
            }

            var entity = _mapper.Map<Category>(createCategoryDto);
            entity.Status = ObjectStatus.CREATED;
            entity.Id = Guid.NewGuid().ToString();
            await _categoryRepository.AddAsync(entity);

            var result = ApiResponseDto.Success();
            return result;
        }

        public async Task<ApiResponseDto> UpdateAsync(UpdateCategoryDto updateCategoryDto)
        {
            var entity = _mapper.Map<Category>(updateCategoryDto);
            entity = await _categoryRepository.UpdateAsync(entity);
            if (entity is null)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.CATEGORY_NOT_FOUND);
                _logger.LogError("Category update failed: Category not found");
                return errorResult;
            }

            var result = ApiResponseDto.Success();
            return result;
        }

        public async Task<ApiResponseDto> DeleteAsync(DeleteCategoryDto deleteCategoryDto)
        {
            var entity = await FindByIdAsync(deleteCategoryDto.Id);
            if (entity is null)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.CATEGORY_NOT_FOUND);
                _logger.LogError("Category deletion failed: Category not found");
                return errorResult;
            }

            await _categoryRepository.SoftDeleteAsync(entity, deleteCategoryDto.DeletedBy!);
            
            var result = ApiResponseDto.Success();
            return result;
        }

        private async Task<Category?> FindByIdAsync(string id)
        {
            var entity = await _categoryRepository.FindByIdAsync(id);
            if (entity is null || entity.Status is ObjectStatus.DELETED)
            {
                return null;
            }
            return entity;
        }
    }
}
