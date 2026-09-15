using back_end.Models;

namespace back_end.Repositories.Interfaces
{
    public interface IVocabularyProgressRepository
    {
        Task<bool> AddVocabularyProgressAsync(VocabularyProgress vocabularyProgress);
        Task<bool> UpdateVocabularyProgressAsync(VocabularyProgress vocabularyProgress);
        Task<List<VocabularyProgress>> GetDueVocabulariesAsync(Guid userId);
        Task<VocabularyProgress?> GetVocabularyProgressByUserIdAndVocabularyIdAsync(Guid userId, Guid vocabularyId);
    }
}
