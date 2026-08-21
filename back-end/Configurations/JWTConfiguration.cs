using back_end.Configurations.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace back_end.Configurations
{
    public static class JWTConfiguration
    {
        public static IServiceCollection AddJWTAuthentication(
            this IServiceCollection services,
            SecuritySetting securitySetting)
        {
            if (string.IsNullOrEmpty(securitySetting.JwtSecretKey))
            {
                throw new InvalidOperationException(
                    "JWT SecretKey is not configured.");
            }

            var key = Encoding.UTF8.GetBytes(
                securitySetting.JwtSecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = securitySetting.JwtIssuer,
                        ValidAudience = securitySetting.JwtAudience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(key)
                    };
            });

            return services;
        }
    }
}