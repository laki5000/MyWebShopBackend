﻿using AutoMapper;
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
            entity.Status = ObjectStatus.CREATED;
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
                var errorResult = ApiResponseDto.Fail(ErrorCode.NOT_OWNER_OF_PRODUCT);
                _logger.LogError("Product update failed: Not owner of product");
            }

            var categoryChanged = updateProductDto.CategoryId is not null && updateProductDto.CategoryId != entity.CategoryId;
            if (categoryChanged)
            {
                var categoryExists = await _categoryService.ExistsByIdAsync(updateProductDto.CategoryId!);
                if (!categoryExists)
                {
                    var errorResult = ApiResponseDto.Fail(ErrorCode.CATEGORY_NOT_FOUND);
                    _logger.LogError("Product creation failed: Category not found");
                    return errorResult;
                }
                entity.CategoryId = updateProductDto.CategoryId!;
            }
            var titleChanged = updateProductDto.Title is not null && updateProductDto.Title != entity.Title;
            if (titleChanged)
            {
                var existsByTitle = await _productRepository.ExistsByTitleAsync(updateProductDto.Title!);
                if (existsByTitle)
                {
                    var errorResult = ApiResponseDto.Fail(ErrorCode.TITLE_ALREADY_EXISTS);
                    _logger.LogError("Product update failed: Title already exists");
                    return errorResult;
                }
                entity.Title = updateProductDto.Title!;
            }
            var descriptionChanged = updateProductDto.Description is not null && updateProductDto.Description != entity.Description;
            if (descriptionChanged)
            {
                entity.Description = updateProductDto.Description!;
            }
            var priceChanged = updateProductDto.Price is not null && updateProductDto.Price != entity.Price;
            if (priceChanged)
            {
                entity.Price = updateProductDto.Price;
            }
            var stockQuantityChanged = updateProductDto.StockQuantity is not null && updateProductDto.StockQuantity != entity.StockQuantity;
            if (stockQuantityChanged)
            {
                entity.StockQuantity = updateProductDto.StockQuantity;
            }

            var isChanged = categoryChanged || titleChanged || descriptionChanged || priceChanged || stockQuantityChanged;
            if (!isChanged)
            {
                var errorResult = ApiResponseDto.Fail(ErrorCode.NOT_MODIFIED);
                _logger.LogError("Product update failed: No changes detected");
                return errorResult;
            }

            entity.UpdatedBy = updateProductDto.UpdatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
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
                var errorResult = ApiResponseDto.Fail(ErrorCode.NOT_OWNER_OF_PRODUCT);
                _logger.LogError("Product deletion failed: Not owner of product");
                return errorResult;
            }

            entity.Status = ObjectStatus.DELETED;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = deleteProductDto.DeletedBy;
            await _productRepository.UpdateAsync(entity);

            var result = ApiResponseDto.Success();
            return result;
        }
    }
}
