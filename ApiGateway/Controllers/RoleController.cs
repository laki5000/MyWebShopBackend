using ApiGateway.BaseClasses.Controllers;
using ApiGateway.Interfaces.Grpc;
using AuthService.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/v1/role")]
    public class RoleController : BaseController
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IAuthServiceRoleClientAdapter _authServiceRoleClientAdapter;

        public RoleController(ILogger<RoleController> logger, IAuthServiceRoleClientAdapter authServiceRoleClientAdapter)
        {
            _logger = logger;
            _authServiceRoleClientAdapter = authServiceRoleClientAdapter;
        }

        [Authorize(Roles = nameof(Role.ADMIN))]
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roleServiceResult = await _authServiceRoleClientAdapter.GetAllAsync();
            if(!roleServiceResult.IsSuccess)
            {
                _logger.LogWarning("Failed to get all roles. Error: {Error}", roleServiceResult.ErrorCode);
                var result = GetObjectResult(roleServiceResult);
                return result;
            }

            return Ok(roleServiceResult);
        }
    }
}
