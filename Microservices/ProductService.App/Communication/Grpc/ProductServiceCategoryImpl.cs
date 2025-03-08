using AutoMapper;
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
    }
}
