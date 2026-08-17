using back_end.DTOs.Projections;
using back_end.Models;

namespace back_end.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> AddAsync(User user);
        Task<bool> DeleteAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<LoginUserProjection> GetUserForLoginAsync(string email);
    }
}
