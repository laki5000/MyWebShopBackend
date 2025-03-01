using AuthService.Models;

namespace AuthService.Interfaces.Services
{
    public interface IJwtService
    {
        public string GenerateToken(AspNetUser user, IList<string> roles);
    }
}
