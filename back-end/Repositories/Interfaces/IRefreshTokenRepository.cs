using back_end.Models;

namespace back_end.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> AddAsync(RefreshToken refreshToken);
    }
}
