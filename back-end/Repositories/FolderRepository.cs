using back_end.Data;
using back_end.Models;
using back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace back_end.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        private readonly DBContext _context;
        public FolderRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Folder folder)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Folders.AddAsync(folder);
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

        public async Task<Folder?> GetFolderById(Guid folderId)
        {
            return await _context.Folders.FirstOrDefaultAsync(f => f.Id == folderId);
        }

        public async Task<(List<Folder> Folders, int TotalItems)> GetFoldersByUserId(Guid userId, int pageNumber, int pageSize)
        {
            IQueryable<Folder> query = _context.Folders
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.Created);

            int totalItems = await query.CountAsync();

            List<Folder> folders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (folders, totalItems);
        }

        public async Task<bool> SoftDelete(Guid folderId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                Folder? folder = await _context.Folders
                    .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);
                if (folder == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                folder.IsDeleted = true;
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

        public async Task<bool> Update(Folder folder)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Folders.Update(folder);
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
