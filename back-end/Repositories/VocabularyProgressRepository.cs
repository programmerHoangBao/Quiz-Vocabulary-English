using back_end.Data;
using back_end.Models;
using back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace back_end.Repositories
{
    public class VocabularyProgressRepository : IVocabularyProgressRepository
    {
        private readonly DBContext _context;

        public VocabularyProgressRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddVocabularyProgressAsync(VocabularyProgress vocabularyProgress)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.VocabularyProgresses.AddAsync(vocabularyProgress);
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

        public async Task<List<VocabularyProgress>> GetDueVocabulariesAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            return await _context.VocabularyProgresses
                .Include(x => x.Vocabulary)
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsDeleted &&
                    x.NextReviewAt.HasValue &&
                    x.NextReviewAt.Value <= now
                )
                .OrderBy(x => x.NextReviewAt)
                .ToListAsync();
        }

        public async Task<VocabularyProgress?> GetVocabularyProgressByUserIdAndVocabularyIdAsync(
            Guid userId, 
            Guid vocabularyId
        )
        {
            return await _context.VocabularyProgresses
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.VocabularyId == vocabularyId &&
                    !x.IsDeleted
                );
        }

        public async Task<bool> UpdateVocabularyProgressAsync(VocabularyProgress vocabularyProgress)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                _context.VocabularyProgresses.Update(vocabularyProgress);
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
