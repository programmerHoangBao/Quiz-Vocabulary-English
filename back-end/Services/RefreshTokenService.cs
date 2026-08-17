using back_end.Configurations.Settings;
using back_end.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace back_end.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly SecuritySetting _securitySetting;
        public RefreshTokenService(IOptions<SecuritySetting> options)
        {
            _securitySetting = options.Value;
        }
        public string GenerateToken()
        {
            byte[] ramdomByte = RandomNumberGenerator.GetBytes(64);
            return WebEncoders.Base64UrlEncode(ramdomByte);
        }

        public string HashToken(string token)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_securitySetting.JwtSecretKey));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hash);
        }
    }
}
