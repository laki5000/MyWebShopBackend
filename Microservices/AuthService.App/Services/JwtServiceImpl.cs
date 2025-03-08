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
        private readonly ILogger<JwtServiceImpl> _logger;
        private readonly JwtSettings _jwtSettings;

        public JwtServiceImpl(ILogger<JwtServiceImpl> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _jwtSettings = appSettings.Value.JwtSettings;
        }

        public string GenerateToken(AspNetUser user, IList<string> roles)
        {
            if (user.UserName is null)
            {
                _logger.LogError("Token generation failed: UserName is null for user ID {UserId}", user.Id);
                throw new ArgumentNullException(nameof(user.UserName));
            }

            var claims = GetClaims(user, roles);
            var credentials = GetSigningCredentials();
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

        private Claim[] GetClaims(AspNetUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims.ToArray();
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }
}
