using ApiGateway.BaseClasses.Controllers;
using AuthService.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/v1/role")]
    public class RoleController : BaseController
    {
        [Authorize(Roles = nameof(Role.ADMIN))]
        [HttpGet("get-roles")]
        public IActionResult GetRoles()
        {
            return NoContent();
        }
    }
}
