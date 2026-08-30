using back_end.Data;
using back_end.DTOs.Projections;
using back_end.Models;
using back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace back_end.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContext _context;

        public UserRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(User user)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
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

        public async Task<bool> DeleteAsync(User user)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Remove(user);
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

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAndIsDeleteFalse(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<LoginUserProjection?> GetUserForLoginAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Email == email && u.IsVerified && !u.IsDeleted)
                .Select(u => new LoginUserProjection
                {
                    Id = u.Id,
                    Email = u.Email,
                    Password = u.Password,
                    Role = u.Role,
                    IsVerified = u.IsVerified,
                    IsDeleted = u.IsDeleted,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(User user)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Update(user);
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
        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.GoogleId == googleId &&
                    !u.IsDeleted);
        }
    }
}
