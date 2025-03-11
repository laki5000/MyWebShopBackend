using ApiGateway.Interfaces.Grpc;
using AutoMapper;
using Productservice.Proto;
using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ApiGateway.Communication.Grpc
{
    public class ProductServiceProductClientAdapterImpl : IProductServiceProductClientAdapter
    {
        private readonly ILogger<ProductServiceProductClientAdapterImpl> _logger;
        private readonly ProductServiceProduct.ProductServiceProductClient _productServiceProductClient;
        private readonly IMapper _mapper;

        public ProductServiceProductClientAdapterImpl(ILogger<ProductServiceProductClientAdapterImpl> logger, ProductServiceProduct.ProductServiceProductClient productServiceProductClient, IMapper mapper)
        {
            _logger = logger;
            _productServiceProductClient = productServiceProductClient;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto> CreateAsync(CreateProductDto request)
        {
            _logger.LogInformation("Create product request sent");

            var grpcCreateProductDto = _mapper.Map<GrpcCreateProductDto>(request);
            var result = await _productServiceProductClient.CreateAsync(grpcCreateProductDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            _logger.LogInformation("Product created successfully");
            return response;
        }

        public async Task<ApiResponseDto> UpdateAsync(UpdateProductDto request)
        {
            _logger.LogInformation("Update product request sent");

            var grpcUpdateProductDto = _mapper.Map<GrpcUpdateProductDto>(request);
            var result = await _productServiceProductClient.UpdateAsync(grpcUpdateProductDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            _logger.LogInformation("Product updated successfully");
            return response;
        }

        public async Task<ApiResponseDto> DeleteAsync(DeleteProductDto request)
        {
            _logger.LogInformation("Delete product request sent");

            var grpcDeleteProductDto = _mapper.Map<GrpcDeleteProductDto>(request);
            var result = await _productServiceProductClient.DeleteAsync(grpcDeleteProductDto);
            var response = _mapper.Map<ApiResponseDto>(result);

            _logger.LogInformation("Product deleted successfully");
            return response;
        }
    }
}
