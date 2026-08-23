using back_end.Models;

namespace back_end.Repositories.Interfaces
{
    public interface IVocaboluryRepository
    {
        Task<Vocabolury?> GetVocaboluryByIdAsync(Guid id);
        Task<(List<Vocabolury> vocaboluries, int TotalItems)> GetVocaboluriesByTopicId(
            Guid topicId,
            int pageNumber,
            int pageSize
        );
        Task<bool> AddAsync(Vocabolury vocabolury);
        Task<bool> UpdateAsync(Vocabolury vocabolury);
        Task<bool> SoftDeleteByIdAsync(Guid id);
        Task<int> AddRangeAsync(List<Vocabolury> vocaboluries);
    }
}
