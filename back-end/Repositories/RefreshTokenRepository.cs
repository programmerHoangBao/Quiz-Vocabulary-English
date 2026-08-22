using back_end.Data;
using back_end.Models;
using back_end.Repositories.Interfaces;

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
    }
}
