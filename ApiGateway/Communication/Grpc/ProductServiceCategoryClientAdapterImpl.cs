using ApiGateway.Interfaces.Grpc;
using AutoMapper;
using Productservice.Proto;
using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ApiGateway.Communication.Grpc
{
    public class ProductServiceCategoryClientAdapterImpl : IProductServiceCategoryClientAdapter
    {
        private readonly ILogger<ProductServiceCategoryClientAdapterImpl> _logger;
        private readonly ProductServiceCategory.ProductServiceCategoryClient _productServiceCategoryClient;
        private readonly IMapper _mapper;

        public ProductServiceCategoryClientAdapterImpl(ILogger<ProductServiceCategoryClientAdapterImpl> logger, ProductServiceCategory.ProductServiceCategoryClient productServiceCategoryClient, IMapper mapper)
        {
            _logger = logger;
            _productServiceCategoryClient = productServiceCategoryClient;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto> CreateAsync(CreateCategoryDto request)
        {
            _logger.LogInformation("Create category request sent");

            var grpcCreateCategoryDto = _mapper.Map<GrpcCreateCategoryDto>(request);
            var result = await _productServiceCategoryClient.CreateAsync(grpcCreateCategoryDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            _logger.LogInformation("Category created successfully");

            return _mapper.Map<ApiResponseDto>(response);
        }

        public async Task<ApiResponseDto> UpdateAsync(UpdateCategoryDto request)
        {
            _logger.LogInformation("Update category request sent");

            var grpcUpdateCategoryDto = _mapper.Map<GrpcUpdateCategoryDto>(request);
            var result = await _productServiceCategoryClient.UpdateAsync(grpcUpdateCategoryDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            _logger.LogInformation("Category updated successfully");
            return _mapper.Map<ApiResponseDto>(response);
        }

        public async Task<ApiResponseDto> DeleteAsync(DeleteCategoryDto request)
        {
            _logger.LogInformation("Delete category request sent");

            var grpcDeleteCategoryDto = _mapper.Map<GrpcDeleteCategoryDto>(request);
            var result = await _productServiceCategoryClient.DeleteAsync(grpcDeleteCategoryDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            _logger.LogInformation("Category deleted successfully");
            return _mapper.Map<ApiResponseDto>(response);
        }
    }
}
