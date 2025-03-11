using ApiGateway.BaseClasses.Controllers;
using ApiGateway.Interfaces.Grpc;
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

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var productServiceResult = await _productServiceCategoryClientAdapter.GetAllAsync();
            if (!productServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to get all categories. Error: {Error}", productServiceResult.ErrorCode);
                var result = GetObjectResult(productServiceResult);
                return result;
            }

            return Ok(productServiceResult);
        }

        [Authorize(Roles = nameof(Role.ADMIN))]
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryDto updateCategoryDto, [FromRoute] string id)
        {
            var userId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            updateCategoryDto.UpdatedBy = userId;
            updateCategoryDto.Id = id;

            var productServiceResult = await _productServiceCategoryClientAdapter.UpdateAsync(updateCategoryDto);
            if (!productServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to update category for {Id}. Error: {Error}", updateCategoryDto.Id, productServiceResult.ErrorCode);
                var result = GetObjectResult(productServiceResult);
                return result;
            }

            return Ok(productServiceResult);
        }

        [Authorize(Roles = nameof(Role.ADMIN))]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var userId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deleteCategoryDto = new DeleteCategoryDto
            {
                Id = id,
                DeletedBy = userId
            };

            var productServiceResult = await _productServiceCategoryClientAdapter.DeleteAsync(deleteCategoryDto);
            if (!productServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to delete category for {Id}. Error: {Error}", deleteCategoryDto.Id, productServiceResult.ErrorCode);
                var result = GetObjectResult(productServiceResult);
                return result;
            }

            return Ok(productServiceResult);
        }
    }
}
