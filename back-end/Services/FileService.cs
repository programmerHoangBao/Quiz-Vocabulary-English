using back_end.DTOs.Vocabolury.Requests;
using back_end.Exceptions;
using back_end.Records;
using back_end.Services.Interfaces;
using ClosedXML.Excel;
using CsvHelper;
using System.Globalization;

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

        public Task<List<VocabularyImportRow>> ParseAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
            if (file == null || file.Length == 0 || !allowedExtensions.Contains(extension))
            {
                throw new BusinessException(ErrorRecord.InvalidFile);
            }
            if (extension == ".csv")
            {
                return ParseCSVAsync(file);
            }
            return ParseExcelAsync(file);
        }
        private async Task<List<VocabularyImportRow>> ParseCSVAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            using var csv = new CsvReader(
                reader,
                CultureInfo.InvariantCulture
            );

            var records = csv.GetRecords<VocabularyImportRow>();

            return await Task.FromResult(records.ToList());
        }
        private async Task<List<VocabularyImportRow>> ParseExcelAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            using var workbook = new XLWorkbook(stream);

            var worksheet = workbook.Worksheets.First();

            var rows = worksheet.RowsUsed();

            var result = new List<VocabularyImportRow>();

            var header = rows.First();

            var headers = header.Cells()
                .Select(x => x.GetString().Trim())
                .ToList();

            int GetColumn(string name)
            {
                return headers.FindIndex(
                    x => x.Equals(name, StringComparison.OrdinalIgnoreCase)
                ) + 1;
            }

            foreach (var row in rows.Skip(1))
            {
                result.Add(new VocabularyImportRow
                {
                    Word = row.Cell(GetColumn("Word"))
                        .GetString()
                        .Trim(),

                    Meaning = row.Cell(GetColumn("Meaning"))
                        .GetString()
                        .Trim(),

                    PartOfSpeech = row.Cell(GetColumn("PartOfSpeech"))
                        .GetString()
                        .Trim(),

                    ExampleEn = row.Cell(GetColumn("ExampleEn"))
                        .GetString()
                        .Trim(),

                    ExampleVn = row.Cell(GetColumn("ExampleVn"))
                        .GetString()
                        .Trim(),

                    IpaUk = row.Cell(GetColumn("IpaUk"))
                        .GetString()
                        .Trim(),

                    IpaUs = row.Cell(GetColumn("IpaUs"))
                        .GetString()
                        .Trim(),

                    //ImageUrl = row.Cell(GetColumn("ImageUrl"))
                    //    .GetString()
                    //    .Trim()
                });
            }

            return await Task.FromResult(result);
        }
    }
}
