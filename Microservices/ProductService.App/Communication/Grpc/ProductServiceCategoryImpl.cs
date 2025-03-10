using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Productservice.Proto;
using ProductService.App.Interfaces.Services;
using ProductService.Shared.Dtos;

namespace ProductService.App.Communication.Grpc
{
    public class ProductServiceCategoryImpl : ProductServiceCategory.ProductServiceCategoryBase
    {
        private readonly ILogger<ProductServiceCategoryImpl> _logger;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public ProductServiceCategoryImpl(ILogger<ProductServiceCategoryImpl> logger, ICategoryService categoryService, IMapper mapper)
        {
            _logger = logger;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public override async Task<GrpcResponseDto> Create(GrpcCreateCategoryDto request, ServerCallContext context)
        {
            _logger.LogInformation("Create category request recieved");

            var createCategoryDto = _mapper.Map<CreateCategoryDto>(request);
            var result = await _categoryService.CreateAsync(createCategoryDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            _logger.LogInformation("Category created successfully");
            return response;
        }

        public override async Task<GrpcGetCategoryDtoListResponseDto> GetAll(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Get all categories request recieved");

            var result = await _categoryService.GetAllAsync();
            var response = _mapper.Map<GrpcGetCategoryDtoListResponseDto>(result);

            _logger.LogInformation("Categories retrieved successfully");
            return response;
        }

        public override async Task<GrpcResponseDto> Update(GrpcUpdateCategoryDto request, ServerCallContext context)
        {
            _logger.LogInformation("Update category request recieved");

            var updateCategoryDto = _mapper.Map<UpdateCategoryDto>(request);
            var result = await _categoryService.UpdateAsync(updateCategoryDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            _logger.LogInformation("Category updated successfully");
            return response;
        }

        public override async Task<GrpcResponseDto> Delete(GrpcDeleteCategoryDto request, ServerCallContext context)
        {
            _logger.LogInformation("Delete category request recieved");

            var deleteCategoryDto = _mapper.Map<DeleteCategoryDto>(request);
            var result = await _categoryService.DeleteAsync(deleteCategoryDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            _logger.LogInformation("Category deleted successfully");
            return response;
        }
    }
}
