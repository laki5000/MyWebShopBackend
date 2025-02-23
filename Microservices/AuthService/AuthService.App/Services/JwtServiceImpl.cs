using AuthService.Configurations;
using AuthService.Interfaces.Services;
using AuthService.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class JwtServiceImpl : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtServiceImpl(IOptions<AppSettings> appSettings)
        {
            _jwtSettings = appSettings.Value.JwtSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public string GenerateToken(AspNetUser user)
        {
            var userName = user.UserName ?? throw new ArgumentNullException(nameof(user.UserName));
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenStr;
        }
    }
}
