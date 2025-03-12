using AutoMapper;
using ProductService.App.Interfaces.Repositories;
using ProductService.App.Interfaces.Services;
using ProductService.App.Models;
using ProductService.Shared.Dtos;
using Shared.Dtos;
using Shared.Enums;

namespace ProductService.App.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly ILogger<ProductServiceImpl> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductServiceImpl(ILogger<ProductServiceImpl> logger, ICategoryService categoryService, IProductRepository productRepository, IMapper mapper)
        {
            _logger = logger;
            _categoryService = categoryService;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto> CreateAsync(CreateProductDto createProductDto)
        {
            var existsByTitle = await _productRepository.ExistsByTitleAsync(createProductDto.Title);
            if (existsByTitle)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.TITLE_ALREADY_EXISTS);
                _logger.LogError("Product creation failed: Title already exists");
                return errorResult;
            }

            var categoryExists = await _categoryService.ExistsByIdAsync(createProductDto.CategoryId);
            if (!categoryExists)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.CATEGORY_NOT_FOUND);
                _logger.LogError("Product creation failed: Category not found");
                return errorResult;
            }

            var entity = _mapper.Map<Product>(createProductDto);
            entity.Id = Guid.NewGuid().ToString();
            entity.Status = DetermineProductStatus(entity);
            entity.OwnerId = createProductDto.CreatedBy!;
            await _productRepository.AddAsync(entity);

            var result = ApiResponseDto.Success();
            return result;
        }

        private async Task<Product?> FindByIdAsync(string id)
        {
            var entity = await _productRepository.FindByIdAsync(id);
            if (entity is null || entity.Status is ObjectStatus.DELETED)
            {
                return null;
            }
            return entity;
        }

        public async Task<ApiResponseDto> UpdateAsync(UpdateProductDto updateProductDto)
        {
            var entity = await FindByIdAsync(updateProductDto.Id!);
            if (entity is null)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.PRODUCT_NOT_FOUND);
                _logger.LogError("Product update failed: Product not found");
                return errorResult;
            }

            var isProductOwner = entity.OwnerId == updateProductDto.UpdatedBy;
            if (!isProductOwner)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.NOT_PRODUCT_OWNER);
                _logger.LogError("Product update failed: Not product owner");
            }

            if (updateProductDto.CategoryId is not null)
            {
                var categoryExists = await _categoryService.ExistsByIdAsync(updateProductDto.CategoryId);
                if (!categoryExists)
                {
                    var errorResult = ApiResponseDto.Fail(ErrorCode.CATEGORY_NOT_FOUND);
                    _logger.LogError("Product creation failed: Category not found");
                    return errorResult;
                }
                entity.CategoryId = updateProductDto.CategoryId;
            }
            if (updateProductDto.Title is not null)
            {
                var existsByTitle = await _productRepository.ExistsByTitleAsync(updateProductDto.Title);
                if (existsByTitle)
                {
                    var errorResult = ApiResponseDto.Fail(ErrorCode.TITLE_ALREADY_EXISTS);
                    _logger.LogError("Product update failed: Title already exists");
                    return errorResult;
                }
                entity.Title = updateProductDto.Title;
            }
            if (updateProductDto.Description is not null)
            {
                entity.Description = updateProductDto.Description;
            }
            if (updateProductDto.Price is not null)
            {
                entity.Price = updateProductDto.Price;
            }
            if (updateProductDto.StockQuantity is not null)
            {
                entity.StockQuantity = updateProductDto.StockQuantity;
            }
            entity.UpdatedBy = updateProductDto.UpdatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.Status = DetermineProductStatus(entity);
            await _productRepository.UpdateAsync(entity);

            var result = ApiResponseDto.Success();
            return result;
        }

        public async Task<ApiResponseDto> DeleteAsync(DeleteProductDto deleteProductDto)
        {
            var entity = await FindByIdAsync(deleteProductDto.Id!);
            if (entity is null)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.PRODUCT_NOT_FOUND);
                _logger.LogError("Product deletion failed: Product not found");
                return errorResult;
            }

            var isProductOwner = entity.OwnerId == deleteProductDto.DeletedBy;
            if (!isProductOwner)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.NOT_PRODUCT_OWNER);
                _logger.LogError("Product deletion failed: Not product owner");
                return errorResult;
            }

            entity.Status = ObjectStatus.DELETED;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = deleteProductDto.DeletedBy;
            await _productRepository.UpdateAsync(entity);

            var result = ApiResponseDto.Success();
            return result;
        }

        private static ObjectStatus DetermineProductStatus(Product product)
        {
            var entityHasQuantity = product.StockQuantity is not null && product.StockQuantity > 0;
            var entityHasPrice = product.Price is not null && product.Price > 0;
            var result = entityHasQuantity && entityHasPrice ? ObjectStatus.AVAILABLE : ObjectStatus.CREATED;

            return result;
        }
    }
}
