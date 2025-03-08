using AuthService.App.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;

namespace AuthService.App.Services
{
    public class RoleServiceImpl : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleServiceImpl(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<ApiResponseDto<List<string>>> GetAllAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var result = ApiResponseDto<List<string>>.Success(roles!);

            return result;
        }
    }
}
