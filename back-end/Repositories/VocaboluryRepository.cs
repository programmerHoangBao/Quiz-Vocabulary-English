using back_end.Data;
using back_end.Models;
using back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace back_end.Repositories
{
    public class VocaboluryRepository : IVocaboluryRepository
    {
        private readonly DBContext _context;
        public VocaboluryRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Vocabolury vocabolury)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Vocaboluries.AddAsync(vocabolury);
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

        public async Task<int> AddRangeAsync(List<Vocabolury> vocaboluries)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.AddRangeAsync(vocaboluries);
                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    await transaction.RollbackAsync();
                    return 0;
                }
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<(List<Vocabolury> vocaboluries, int TotalItems)> GetVocaboluriesByTopicId(
            Guid topicId, 
            int pageNumber, 
            int pageSize
        )
        {
            IQueryable<Vocabolury> query = _context.Vocaboluries
                .Where(v => v.TopicId == topicId && !v.IsDeleted)
                .OrderByDescending(v => v.Created);

            int totalItems = await query.CountAsync();

            List<Vocabolury> vocaboluries = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (vocaboluries, totalItems);
        }

        public async Task<Vocabolury?> GetVocaboluryByIdAsync(Guid id)
        {
            return await _context.Vocaboluries.FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<bool> SoftDeleteByIdAsync(Guid id)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                Vocabolury? vocabolury = await _context.Vocaboluries.FirstOrDefaultAsync(
                    v =>v.Id == id && !v.IsDeleted
                );
                if (vocabolury == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                vocabolury.IsDeleted = true;
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

        public async Task<bool> UpdateAsync(Vocabolury vocabolury)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Vocaboluries.Update(vocabolury);
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
