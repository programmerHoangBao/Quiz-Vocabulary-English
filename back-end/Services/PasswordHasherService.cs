using back_end.Configurations.Settings;
using back_end.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace back_end.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly SecuritySetting _securitySetting;

        public PasswordHasherService(IConfiguration configuration, IOptions<SecuritySetting> securityOptions)
        {
            _securitySetting = securityOptions.Value;
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_securitySetting.SecrectKey));
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashedBytes = hmac.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
