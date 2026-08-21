using back_end.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace back_end.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Guid? UserId
        {
            get
            {
                string? userId =
                    _httpContextAccessor.HttpContext?
                        .User
                        .FindFirstValue(ClaimTypes.NameIdentifier);
                        //.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated;
                //_logger.LogInformation(isAuthenticated.ToString());
                //foreach (var claim in _httpContextAccessor.HttpContext.User.Claims)
                //{
                //    _logger.LogInformation($"Type: {claim.Type}, Value: {claim.Value}");
                //}
                if (!Guid.TryParse(userId, out Guid id))
                {
                    return null;
                }

                return id;
            }
        }
    }
}
