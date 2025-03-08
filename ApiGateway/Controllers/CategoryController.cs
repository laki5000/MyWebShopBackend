using ApiGateway.BaseClasses.Controllers;
using ApiGateway.Interfaces.Grpc;
using AuthService.Shared.Dtos;
using AuthService.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Shared.Dtos;
using System.Security.Claims;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/v1/category")]
    public class CategoryController : BaseController
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IProductServiceCategoryClientAdapter _productServiceCategoryClientAdapter;

        public CategoryController(ILogger<CategoryController> logger, IProductServiceCategoryClientAdapter productServiceCategoryClientAdapter)
        {
            _logger = logger;
            _productServiceCategoryClientAdapter = productServiceCategoryClientAdapter;
        }

        [Authorize(Roles = nameof(Role.ADMIN))]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            var userId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            createCategoryDto.CreatedBy = userId;

            var productServiceResult = await _productServiceCategoryClientAdapter.CreateAsync(createCategoryDto);
            if (!productServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to create category for {Name}. Error: {Error}", createCategoryDto.Name, productServiceResult.ErrorCode);
                var result = GetObjectResult(productServiceResult);
                return result;
            }

            return Ok(productServiceResult);
        }
    }
}
