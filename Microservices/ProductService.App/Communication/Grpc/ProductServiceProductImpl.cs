using AutoMapper;
using Grpc.Core;
using Productservice.Proto;
using ProductService.App.Interfaces.Services;
using ProductService.Shared.Dtos;

namespace ProductService.App.Communication.Grpc
{
    public class ProductServiceProductImpl : ProductServiceProduct.ProductServiceProductBase
    {
        private readonly ILogger<ProductServiceProductImpl> _logger;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductServiceProductImpl(ILogger<ProductServiceProductImpl> logger, IProductService productService, IMapper mapper)
        {
            _logger = logger;
            _productService = productService;
            _mapper = mapper;
        }

        public override async Task<GrpcResponseDto> Create(GrpcCreateProductDto request, ServerCallContext context)
        {
            _logger.LogInformation("Create product request recieved");

            var createProductDto = _mapper.Map<CreateProductDto>(request);
            var result = await _productService.CreateAsync(createProductDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            _logger.LogInformation("Product created successfully");
            return response;
        }

        public override async Task<GrpcResponseDto> Update(GrpcUpdateProductDto request, ServerCallContext context)
        {
            _logger.LogInformation("Update product request recieved");

            var updateProductDto = _mapper.Map<UpdateProductDto>(request);
            var result = await _productService.UpdateAsync(updateProductDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            _logger.LogInformation("Product updated successfully");
            return response;
        }

        public override async Task<GrpcResponseDto> Delete(GrpcDeleteProductDto request, ServerCallContext context)
        {
            _logger.LogInformation("Delete product request recieved");

            var deleteProductDto = _mapper.Map<DeleteProductDto>(request);
            var result = await _productService.DeleteAsync(deleteProductDto);
            var response = _mapper.Map<GrpcResponseDto>(result);

            _logger.LogInformation("Product deleted successfully");
            return response;
        }
    }
}
