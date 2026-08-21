using back_end.Data;
using back_end.Models;
using back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace back_end.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        private readonly DBContext _context;
        private readonly ILogger<FolderRepository> _logger;
        public FolderRepository(DBContext context, ILogger<FolderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddAsync(Folder folder)
        {
            await _context.Folders.AddAsync(folder);
            var result = await _context.SaveChangesAsync();
            return result > 0;
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
            Folder? folder = await _context.Folders.FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted);
            if (folder == null)
            {
                _logger.LogWarning("Folder not found!");
                return false;
            }
            folder.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Update(Folder folder)
        {
            _context.Folders.Update(folder);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
