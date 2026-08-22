using back_end.Data;
using back_end.Models;
using back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;

namespace back_end.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly DBContext _context;

        public TopicRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Topic topic)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Topics.AddAsync(topic);
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

        public async Task<Guid?> GetOwnerIdByIdAsync(Guid id)
        {
            return await _context.Topics
                .Where(t => t.Id == id && !t.IsDeleted)
                .Select(t => (Guid?)t.Folder.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<Topic?> GetTopicByIdAsync(Guid id)
        {
            return await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<(List<Topic> Topics, int TotalItems)> GetTopicsByFolderIdAsync(
            Guid folderId,
            int pageNumber,
            int pageSize
        )
        {
            IQueryable<Topic> query = _context.Topics
                .Where(t => t.FolderId == folderId && !t.IsDeleted)
                .OrderByDescending(f => f.Created);

            int totalItems = await query.CountAsync();

            List<Topic> topics = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (topics, totalItems);
        }

        public async Task<bool> SoftDeleteByIdAsync(Guid id)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
                if (topic == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                topic.IsDeleted = true;
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

        public async Task<bool> UpdateAsync(Topic topic)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Topics.Update(topic);
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
