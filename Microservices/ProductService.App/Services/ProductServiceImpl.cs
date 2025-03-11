using AutoMapper;
using ProductService.App.Interfaces.Repositories;
using ProductService.App.Interfaces.Services;
using ProductService.Shared.Dtos;
using Shared.Dtos;

namespace ProductService.App.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly ILogger<ProductServiceImpl> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductServiceImpl(ILogger<ProductServiceImpl> logger, IProductRepository productRepository, IMapper mapper)
        {
            _logger = logger;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public Task<ApiResponseDto> CreateAsync(CreateProductDto createProductDto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponseDto> UpdateAsync(UpdateProductDto updateProductDto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponseDto> DeleteAsync(DeleteProductDto deleteProductDto)
        {
            throw new NotImplementedException();
        }
    }
}
