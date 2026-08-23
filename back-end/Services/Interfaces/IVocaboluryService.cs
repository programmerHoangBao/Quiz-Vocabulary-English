using back_end.DTOs;
using back_end.DTOs.Vocabolury.Requests;
using back_end.DTOs.Vocabolury.Responses;

namespace back_end.Services.Interfaces
{
    public interface IVocaboluryService
    {
        Task<ApiResponse<VocaboluryResponse?>> GetVocaboluryById(Guid id);
        Task<ApiResponse<object?>> GetVocaboluriesByTopicId(Guid topicId, int pageNumber, int pageSize);
        Task<ApiResponse<VocaboluryResponse?>> CreateVocabolury(CreateVocaboluryRequest req);
        Task<ApiResponse<object?>> UpdateVocabolury(UpdateVocaboluryRequest req);
        Task<ApiResponse<object?>> SoftDeleteById(Guid id);
    }
}
