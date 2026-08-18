using back_end.Configurations.Settings;
using back_end.DTOs.Projections;
using back_end.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace back_end.Services
{
    public class JwtService : IJwtService
    {
        private readonly SecuritySetting _securitySetting;
        public JwtService(IOptions<SecuritySetting> options)
        {
            _securitySetting = options.Value;
        }
        public string GenerateAccessToken(LoginUserProjection user)
        {
            var claims = new List<Claim>
        {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role.ToString())
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_securitySetting.JwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _securitySetting.JwtIssuer,
                audience: _securitySetting.JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_securitySetting.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
