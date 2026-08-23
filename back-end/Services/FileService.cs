using back_end.Exceptions;
using back_end.Records;
using back_end.Services.Interfaces;

namespace back_end.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BusinessException(ErrorRecord.InvalidFile);
            }

            if (file.Length > MaxFileSize)
            {
                throw new BusinessException(ErrorRecord.FileTooLarge);
            }

            string extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                throw new BusinessException(ErrorRecord.InvalidFileFormat);
            }

            string uploadDirectory = Path.Combine(
                _environment.WebRootPath,
                "uploads"
            );

            Directory.CreateDirectory(uploadDirectory);

            string fileName = $"{Guid.NewGuid()}{extension}";

            string filePath = Path.Combine(
                uploadDirectory,
                fileName
            );

            await using FileStream stream = new FileStream(
                filePath,
                FileMode.Create
            );

            await file.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }

        public Task DeleteAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return Task.CompletedTask;
            }

            string relativePath = imageUrl
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            string filePath = Path.Combine(
                _environment.WebRootPath,
                relativePath
            );

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}
