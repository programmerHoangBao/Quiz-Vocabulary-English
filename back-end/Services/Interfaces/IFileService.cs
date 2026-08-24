using back_end.DTOs.Vocabolury.Requests;

namespace back_end.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string? imageUrl);

        Task<List<VocabularyImportRow>> ParseAsync(IFormFile file);
    }
}
