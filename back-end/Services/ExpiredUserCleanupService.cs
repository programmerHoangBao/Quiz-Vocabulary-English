using Microsoft.EntityFrameworkCore;
using back_end.Data;

namespace back_end.Services
{
    public class ExpiredUserCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpiredUserCleanupService> _logger;
        private static readonly TimeSpan CleanupInterval = TimeSpan.FromSeconds(30);

        public ExpiredUserCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<ExpiredUserCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Expired User Cleanup Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredUsersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up expired users.");
                }

                await Task.Delay(CleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Expired User Cleanup Service stopped.");
        }

        private async Task CleanupExpiredUsersAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

            var now = DateTime.UtcNow;

            var expiredUsers = await dbContext.Users
                .Where(user => !user.IsVerified && user.OtpExpiry < now)
                .ToListAsync(cancellationToken);

            if (expiredUsers.Count == 0)
            {
                return;
            }

            dbContext.Users.RemoveRange(expiredUsers);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted {Count} expired unverified user(s).", expiredUsers.Count);
        }
    }
}
