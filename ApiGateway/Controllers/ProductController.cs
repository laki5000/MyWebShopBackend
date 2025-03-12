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
    [Route("api/v1/product")]
    public class ProductController : BaseController
    {
        private ILogger<ProductController> _logger;
        private IProductServiceProductClientAdapter _productServiceProductClientAdapter;

        public ProductController(ILogger<ProductController> logger, IProductServiceProductClientAdapter productServiceProductClientAdapter)
        {
            _logger = logger;
            _productServiceProductClientAdapter = productServiceProductClientAdapter;
        }

        [Authorize(Roles = $"{nameof(Role.ADMIN)},{nameof(Role.VENDOR)}")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
        {
            var userId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            createProductDto.CreatedBy = userId;

            var productServiceResult = await _productServiceProductClientAdapter.CreateAsync(createProductDto);
            if (!productServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to create product for {Title}. Error: {Error}", createProductDto.Title, productServiceResult.ErrorCode);
                var result = GetObjectResult(productServiceResult);
                return result;
            }

            return Ok(productServiceResult);
        }

        [Authorize(Roles = $"{nameof(Role.ADMIN)},{nameof(Role.VENDOR)}")]
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto updateProductDto, [FromRoute] string id)
        {
            var userId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            updateProductDto.UpdatedBy = userId;
            updateProductDto.Id = id;

            var productServiceResult = await _productServiceProductClientAdapter.UpdateAsync(updateProductDto);
            if (!productServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to update product for {Id}. Error: {Error}", updateProductDto.Id, productServiceResult.ErrorCode);
                var result = GetObjectResult(productServiceResult);
                return result;
            }

            return Ok(productServiceResult);
        }

        [Authorize(Roles = $"{nameof(Role.ADMIN)},{nameof(Role.VENDOR)}")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var userId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deleteProductDto = new DeleteProductDto
            {
                Id = id,
                DeletedBy = userId
            };

            var productServiceResult = await _productServiceProductClientAdapter.DeleteAsync(deleteProductDto);
            if (!productServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to delete product for {Id}. Error: {Error}", deleteProductDto.Id, productServiceResult.ErrorCode);
                var result = GetObjectResult(productServiceResult);
                return result;
            }

            return Ok(productServiceResult);
        }
    }
}
