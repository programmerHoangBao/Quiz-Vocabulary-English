using back_end.Data;
using back_end.Models;
using back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace back_end.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DBContext _context;
        public RefreshTokenRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(RefreshToken refreshToken)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.AddAsync(refreshToken);
                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash &&
                    !x.IsDeleted &&
                    x.RevokeAt == null);
        }

        public async Task<bool> RevokeAsync(Guid id)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Id == id);

            if (refreshToken == null)
            {
                return false;
            }

            refreshToken.RevokeAt = DateTime.UtcNow;
            refreshToken.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
