using AutoMapper;
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

        private async Task<Category?> FindByIdAsync(string id)
        {
            var entity = await _categoryRepository.FindByIdAsync(id);
            if (entity is null || entity.Status is ObjectStatus.DELETED)
            {
                return null;
            }
            return entity;
        }

        public async Task<bool> ExistsByIdAsync(string id)
        {
            var exists = await _categoryRepository.ExistsByIdAsync(id);
            return exists;
        }

        public async Task<ApiResponseDto<List<GetCategoryDto>>> GetAllAsync()
        {
            var entities = await _categoryRepository.GetAllNotDeletedAsync();
            var dtos = _mapper.Map<List<GetCategoryDto>>(entities);
            var result = ApiResponseDto<List<GetCategoryDto>>.Success(dtos);
            return result;
        }

        public async Task<ApiResponseDto> UpdateAsync(UpdateCategoryDto updateCategoryDto)
        {
            var entity = await FindByIdAsync(updateCategoryDto.Id!);
            if (entity is null)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.CATEGORY_NOT_FOUND);
                _logger.LogError("Category update failed: Category not found");
                return errorResult;
            }

            var nameChanged = updateCategoryDto.Name is not null && updateCategoryDto.Name != entity.Name;
            if (nameChanged)
            {
                var existsByName = await _categoryRepository.ExistsByNameAsync(updateCategoryDto.Name!);
                if (existsByName)
                {
                    var errorResult = ApiResponseDto.Fail(ErrorCode.NAME_ALREADY_EXISTS);
                    _logger.LogError("Category update failed: Name already exists");
                    return errorResult;
                }
                entity.Name = updateCategoryDto.Name!;
            }

            var isChanged = nameChanged;
            if (!isChanged)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.NOT_MODIFIED);
                _logger.LogError("Category update failed: No changes detected");
                return errorResult;
            }

            entity.UpdatedBy = updateCategoryDto.UpdatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            await _categoryRepository.UpdateAsync(entity);

            var result = ApiResponseDto.Success();
            return result;
        }

        public async Task<ApiResponseDto> DeleteAsync(DeleteCategoryDto deleteCategoryDto)
        {
            var entity = await FindByIdAsync(deleteCategoryDto.Id!);
            if (entity is null)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.CATEGORY_NOT_FOUND);
                _logger.LogError("Category deletion failed: Category not found");
                return errorResult;
            }

            entity.Status = ObjectStatus.DELETED;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = deleteCategoryDto.DeletedBy;
            await _categoryRepository.UpdateAsync(entity);

            var result = ApiResponseDto.Success();
            return result;
        }
    }
}
