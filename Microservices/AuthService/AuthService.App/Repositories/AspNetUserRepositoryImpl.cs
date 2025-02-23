using AuthService.Data;
using AuthService.Interfaces.Repositories;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Shared.BaseClasses.Repositories;

namespace AuthService.Repositories
{
    public class AspNetUserRepositoryImpl : BasePgRepositoryImpl<AspNetUser>, IAspNetUserRepository 
    {
        protected new readonly AuthDbContext _context;

        public AspNetUserRepositoryImpl(AuthDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistsByNormalizedUserNameAsync(string normalizedUserName)
        {
            var result = await _context.AspNetUsers.AnyAsync(x => x.NormalizedUserName == normalizedUserName);

            return result;
        }

        public async Task<AspNetUser?> GetByNormalizedUserNameAsync(string normalizedUserName)
        {
            var result = await _context.AspNetUsers.FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName);

            return result;
        }
    }
}
