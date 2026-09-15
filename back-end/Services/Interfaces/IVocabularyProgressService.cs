using back_end.DTOs;
using back_end.DTOs.VocabularyProgress.Requests;

namespace back_end.Services.Interfaces
{
    public interface IVocabularyProgressService
    {
        Task<ApiResponse<object?>> CreateVocabularyProgressAsync(
            CreateVocabularyProgressRequest req
        );
        Task<ApiResponse<object?>> GetDueVocabulariesAsync(Guid userId);
        Task<ApiResponse<object?>> SubmitReviewAsync(SubmitReviewRequest req);
    }
}
