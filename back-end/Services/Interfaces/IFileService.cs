namespace back_end.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string? imageUrl);
    }
}
